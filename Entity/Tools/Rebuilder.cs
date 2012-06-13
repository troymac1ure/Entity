// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rebuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The rebuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Tools
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using entity.MapForms;

    using HaloMap;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.RawData;

    /// <summary>
    /// The rebuilder.
    /// </summary>
    /// <remarks></remarks>
    public partial class Rebuilder : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The meta list.
        /// </summary>
        private readonly ArrayList MetaList = new ArrayList();

        /// <summary>
        /// The skies.
        /// </summary>
        private readonly ArrayList Skies = new ArrayList();

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The imported meta list.
        /// </summary>
        private ArrayList ImportedMetaList = new ArrayList();

        /// <summary>
        /// The count.
        /// </summary>
        private int count;

        /// <summary>
        /// The matg.
        /// </summary>
        private Meta matg;

        /// <summary>
        /// The scnr.
        /// </summary>
        private Meta scnr;

        /// <summary>
        /// The skycount.
        /// </summary>
        private int skycount;

        /// <summary>
        /// The sncl.
        /// </summary>
        private Meta sncl;

        /// <summary>
        /// The spk.
        /// </summary>
        private Meta spk;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Rebuilder"/> class.
        /// </summary>
        /// <param name="mapForm">The map form.</param>
        /// <remarks></remarks>
        public Rebuilder(MapForm mapForm)
        {
            this.map = mapForm.map;
            InitializeComponent();
            Application.DoEvents();
            mapForm.formFuncs.AddMetasToTreeView(map, treeView1, MapForm.FormFunctions.MetaView.FolderView, true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The map rebuilder.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <remarks></remarks>
        public void MapRebuilder(ref MapLayout layout)
        {
            int totalshift = 0;

            // find new strings
            ///
            ///
            ArrayList strings = new ArrayList();
            foreach (string s in map.Strings.Name)
            {
                strings.Add(s);
            }

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                for (int y = 0; y < m.items.Count; y++)
                {
                    Meta.Item ii = m.items[y];
                    if (ii.type == Meta.ItemType.String)
                    {
                        Meta.String iii = (Meta.String)ii;

                        if (strings.IndexOf(iii.name) == -1)
                        {
                            strings.Add(iii.name);
                        }
                    }
                }
            }

            ///read ugh to meta
            ///
            ///
            map.OpenMap(MapTypes.Internal);
            Meta ughmeta = new Meta(map);
            ughmeta.ReadMetaFromMap(map.IndexHeader.metaCount - 1, false);
            IFPIO ifp = IFPHashMap.GetIfp("ugh!", map.HaloVersion);

            ughmeta.headersize = ifp.headerSize;
            ughmeta.scanner.ScanWithIFP(ref ifp);
            MetaList.Add(ughmeta);

            string temps = string.Empty;

            ///get model info
            int tempint = layout.FindByType(RawDataContainerType.Model);
            LayOutChunk loc = (LayOutChunk)layout.chunks[tempint];

            //////////////////////////////////////
            ///model raw data
            ///
            int modeshift = 0;

            loc.startoffset += totalshift;

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.Model)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];

                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset;
                            BW.Write(tempintxx);
                            BW.Write(r.size);
                            continue;
                        }

                        int tempintx = loc.startoffset + modeshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        modeshift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            loc.size = modeshift;
            loc.endoffset = loc.startoffset + loc.size;

            layout.chunks[tempint] = loc;

            int curroffset = loc.endoffset;

            /////////////////////////////////////
            /// bsp raw
            ///

            int bspcount = 0;
            int[] bspmagic = new int[20];
            int[] bspmagicreflexive = new int[20];
            int[] bsprawoffset = new int[20];
            int[] bsprawsize = new int[20];
            int totalbsprawsize = 0;
            bool found = false;

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.type == "sbsp")
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    int[] tempoff = new int[m.raw.rawChunks.Count];
                    int thisbsprawsize = 0;
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        found = false;
                        RawDataChunk r = m.raw.rawChunks[y];
                        for (int yy = 0; yy < y; yy++)
                        {
                            RawDataChunk rr = m.raw.rawChunks[yy];
                            if (rr.offset == r.offset && rr.rawLocation == r.rawLocation)
                            {
                                tempoff[y] = tempoff[yy];

                                // writes new pointer to loaded meta
                                BW.BaseStream.Position = r.pointerMetaOffset;
                                BW.Write(tempoff[y]);
                                BW.Write(r.size);
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }

                        tempoff[y] = curroffset + thisbsprawsize;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempoff[y]);
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = curroffset + thisbsprawsize;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        thisbsprawsize += r.size + tempinty;
                        totalbsprawsize += r.size + tempinty;
                    }

                    bsprawsize[bspcount] = thisbsprawsize;
                    bspmagic[bspcount] = m.magic;
                    bspmagicreflexive[bspcount] = m.magic + m.offset;

                    curroffset += bsprawsize[bspcount];

                    // bspmagic=m.magic;
                    // newmagicreflexive=bspmagic+m.offset;
                    bspcount += 1;
                    MetaList[x] = m;
                }
            }

            //////////////////////////////////////
            ///weather raw data
            ///
            int weathershift = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.Weather)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset;
                            BW.Write(tempintxx);
                            BW.Write(r.size);
                            continue;
                        }

                        int tempintx = curroffset + weathershift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        weathershift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            curroffset += weathershift;

            //////////////////////////////////////
            ///decr raw data
            ///
            int decrshift = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.DECR)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset;
                            BW.Write(tempintxx);
                            BW.Write(r.size);
                            continue;
                        }

                        int tempintx = curroffset + decrshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        decrshift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            curroffset += decrshift;

            //////////////////////////////////////
            ///prtm raw data
            ///
            int prtmshift = 0;

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.PRTM)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset;
                            BW.Write(tempintxx);
                            BW.Write(r.size);
                            continue;
                        }

                        int tempintx = curroffset + prtmshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        prtmshift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            curroffset += prtmshift;

            //////////////////////////////////////
            ///jmad raw data
            ///
            int jmadshift = 0;

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.Animation)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset - 4;
                            BW.Write(r.size);
                            BW.Write(tempintxx);
                            continue;
                        }

                        int tempintx = curroffset + jmadshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset - 4;
                        BW.Write(r.size);
                        BW.Write(tempintx);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        jmadshift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            curroffset += jmadshift;

            //////////////////////////////////////
            ///bsp meta data
            ///

            int[] bspmetaoffset = new int[20];
            int[] bspmetasize = new int[20];

            int tempcount = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.type == "sbsp")
                {
                    Meta mm = (Meta)MetaList[3];
                    if (mm.type == "scnr")
                    {
                        BinaryWriter BWX = new BinaryWriter(mm.MS);
                        int tempoffx = map.BSP.sbsp[tempcount].pointerOffset;
                        BWX.BaseStream.Position = tempoffx;

                        BWX.Write(curroffset);
                        BWX.Write(m.size);
                        BWX.Write(bspmagicreflexive[tempcount]);
                        MetaList[3] = mm;
                    }

                    map.BW.BaseStream.Position = curroffset;
                    map.BW.BaseStream.Write(m.MS.ToArray(), 0, m.size);
                    bspmetasize[tempcount] = m.size;
                    bspmetaoffset[tempcount] = curroffset;
                    curroffset += m.size;
                    tempcount++;
                }
            }

            ////stringnames1
            ///

            byte[] tempb = new byte[strings.Count * 128];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, strings.Count * 128);
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = curroffset + (x * 128);
                char[] tempc = ((String)strings[x]).ToCharArray();
                map.BW.Write(tempc);
            }

            map.BW.BaseStream.Position = 352;
            map.BW.Write(curroffset);
            int newcount = strings.Count;
            map.BW.Write(newcount);

            curroffset += strings.Count * 128;

            int padding = map.Functions.Padding(curroffset, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, padding);
            curroffset += padding;

            ////stringsindex
            ///

            int tempnewsrsize = strings.Count * 4;
            map.BW.BaseStream.Position = curroffset;

            int temporary = 0;
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = curroffset + (x * 4);
                map.BW.Write(temporary);
                temporary += ((String)strings[x]).Length + 1;
            }

            map.BW.BaseStream.Position = 364;
            map.BW.Write(curroffset);
            curroffset += tempnewsrsize;

            padding = map.Functions.Padding(curroffset, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, padding);
            curroffset += padding;

            ////strings2
            ///

            temporary = 0;
            byte zero = 0;
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = curroffset + temporary;
                char[] h = ((String)strings[x]).ToCharArray();
                map.BW.Write(h);
                map.BW.Write(zero);
                temporary += ((String)strings[x]).Length + 1;
            }

            map.BW.BaseStream.Position = 360;
            map.BW.Write(temporary);
            map.BW.BaseStream.Position = 368;
            map.BW.Write(curroffset);
            curroffset += temporary;

            padding = map.Functions.Padding(curroffset, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, padding);
            curroffset += padding;

            ////file names
            ///

            temporary = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                map.BW.BaseStream.Position = curroffset + temporary;
                char[] h = ((Meta)MetaList[x]).name.ToCharArray();
                map.BW.Write(h);
                map.BW.Write(zero);
                temporary += ((Meta)MetaList[x]).name.Length + 1;
            }

            map.BW.BaseStream.Position = 704;
            newcount = MetaList.Count;
            map.BW.Write(newcount);
            map.BW.Write(curroffset);
            map.BW.Write(temporary);
            curroffset += temporary;

            padding = map.Functions.Padding(curroffset, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, padding);
            curroffset += padding;

            ////files index
            ///
            temporary = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                map.BW.BaseStream.Position = curroffset + (x * 4);
                map.BW.Write(temporary);
                temporary += ((Meta)MetaList[x]).name.Length + 1;
            }

            map.BW.BaseStream.Position = 716;
            map.BW.Write(curroffset);

            curroffset += MetaList.Count * 4;
            padding = map.Functions.Padding(curroffset, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = curroffset;
            map.BW.BaseStream.Write(tempb, 0, padding);
            curroffset += padding;

            tempint = layout.FindByType(RawDataContainerType.UnicodeNamesIndex);
            for (int x = 0; x < 9; x++)
            {
                map.Unicode.ut[x].indexOffset = curroffset;
                if (x != 8)
                {
                    loc = (LayOutChunk)layout.chunks[tempint + (x * 2)];
                    loc.startoffset = curroffset;
                    loc.endoffset = loc.startoffset + loc.size;
                    map.BW.BaseStream.Position = loc.startoffset;
                    map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
                    layout.chunks[tempint + (x * 2)] = loc;
                    curroffset += loc.size;
                }

                map.Unicode.ut[x].tableOffset = curroffset;
                if (x != 8)
                {
                    LayOutChunk loc2 = (LayOutChunk)layout.chunks[tempint + (x * 2) + 1];
                    loc2.startoffset = curroffset;
                    loc2.endoffset = loc2.startoffset + loc.size;
                    map.BW.BaseStream.Position = loc2.startoffset;
                    map.BW.BaseStream.Write(loc2.MS.ToArray(), 0, loc2.size);
                    layout.chunks[tempint + (x * 2) + 1] = loc2;

                    curroffset += loc2.size;
                }
            }

            Meta tempmatg = (Meta)MetaList[0];
            BinaryWriter BWXX = new BinaryWriter(tempmatg.MS);
            for (int x = 0; x < 9; x++)
            {
                BWXX.BaseStream.Position = map.Unicode.ut[x].indexPointerOffset;
                BWXX.Write(map.Unicode.ut[x].indexOffset);
                BWXX.Write(map.Unicode.ut[x].tableOffset);
            }

            MetaList[0] = tempmatg;

            tempint = layout.FindByType(RawDataContainerType.Crazy);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset = curroffset;

            // loc.endoffset+=totalshift;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            map.BW.BaseStream.Position = 344;
            map.BW.Write(loc.startoffset);
            layout.chunks[tempint] = loc;

            curroffset += loc.size;

            //////////////////////////////////////
            ///bitmap raw data
            ///
            int bitmshift = 0;
            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                if (m.rawType == RawDataContainerType.Bitmap)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.rawLocation != MapTypes.Internal)
                        {
                            int tempintxx = r.offset;
                            if (r.rawLocation == MapTypes.MPShared)
                            {
                                tempintxx |= int.Parse("80000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.SPShared)
                            {
                                tempintxx |= int.Parse("C0000000", NumberStyles.HexNumber);
                            }
                            else if (r.rawLocation == MapTypes.MainMenu)
                            {
                                tempintxx |= int.Parse("40000000", NumberStyles.HexNumber);
                            }

                            // writes new pointer to loaded meta
                            BW.BaseStream.Position = r.pointerMetaOffset;
                            BW.Write(tempintxx);
                            BW.BaseStream.Position = r.pointerMetaOffset + 24;
                            BW.Write(r.size);
                            continue;
                        }

                        int tempintx = curroffset + bitmshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);
                        BW.BaseStream.Position = r.pointerMetaOffset + 24;
                        BW.Write(r.size);

                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        bitmshift += r.size + tempinty;
                    }

                    MetaList[x] = m;
                }
            }

            curroffset += bitmshift;

            tempint = layout.FindByType(RawDataContainerType.MetaIndex);

            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset = curroffset;

            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            map.BW.BaseStream.Position = 16;
            map.BW.Write(loc.startoffset);
            newcount = MetaList.Count;
            map.BW.BaseStream.Position = loc.startoffset + 24;
            map.BW.Write(newcount);

            int tagsoff = map.IndexHeader.tagsOffset - map.MapHeader.indexOffset;
            map.PrimaryMagic = map.IndexHeader.constant - (curroffset + 32);
            map.SecondaryMagic = map.PrimaryMagic + bspmetasize[0];

            // map.SecondaryMagic=map.BR.ReadInt32()-(loc.startoffset+map.MapHeader.metaStart);
            int where = curroffset + map.MapHeader.metaStart;
            tempcount = 0;
            int howfar = 0;
            int[] newoffset = new int[MetaList.Count];
            int[] newident = new int[MetaList.Count];

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                int fuck = curroffset + tagsoff + (x * 16);

                char[] metatype = m.type.ToCharArray();
                Array.Reverse(metatype);
                int ident = map.MetaInfo.Ident[0] + (x * 65537);
                int offset = where + howfar;

                if (x == MetaList.Count - 1)
                {
                    int wherex = curroffset + map.MapHeader.metaStart + 756;
                    map.BW.BaseStream.Position = wherex;
                    map.BW.Write(ident);
                }

                if (m.type == "phmo" | m.type == "coll" | m.type == "spas")
                {
                    int tempoffset = offset;
                    do
                    {
                        string tempss = tempoffset.ToString("X");
                        char[] tempc = tempss.ToCharArray();
                        int xxx = tempc.Length;
                        if (m.padding == tempc[xxx - 1])
                        {
                            int diff = tempoffset - offset;
                            tempb = new byte[diff];
                            map.BW.BaseStream.Position = offset;
                            map.BW.Write(tempb);

                            int tempsize = ((Meta)MetaList[x - 1]).size;
                            tempsize += diff;
                            int temploc = fuck - 4;
                            map.BW.BaseStream.Position = temploc;
                            map.BW.Write(tempsize);

                            offset = tempoffset;
                            howfar += diff;
                            break;
                        }

                        tempoffset++;
                    }
                    while (temps != null);
                }

                newoffset[x] = offset;
                newident[x] = ident;
                int offsetwithmagic = offset + map.SecondaryMagic;
                int size = m.size;

                map.BW.BaseStream.Position = fuck;
                map.BW.Write(metatype);
                map.BW.Write(ident);

                if (m.type != "sbsp" && m.type != "ltmp")
                {
                    map.BW.Write(offsetwithmagic);
                    map.BW.Write(size);
                    howfar += m.size;
                    map.BW.BaseStream.Position = offset;
                    map.BW.BaseStream.Write(m.MS.ToArray(), 0, m.size);
                }
                else
                {
                    int zeroi = 0;
                    map.BW.Write(zeroi);
                    map.BW.Write(zeroi);
                    if (m.type == "sbsp")
                    {
                        offset = bspmetaoffset[tempcount];
                        newoffset[x] = offset;
                        tempcount++;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            for (int x = 0; x < MetaList.Count; x++)
            {
                Meta m = (Meta)MetaList[x];
                for (int xx = 0; xx < m.items.Count; xx++)
                {
                    Meta.Item i = m.items[xx];
                    for (int e = 0; e < MetaList.Count; e++)
                    {
                        Meta tempm = (Meta)MetaList[e];
                        if (tempm.name == i.intagname && tempm.type == i.intagtype)
                        {
                            i.intag = e;
                            break;
                        }
                    }

                    if (i.intag != x)
                    {
                        continue;
                    }

                    switch (i.type)
                    {
                        case Meta.ItemType.Ident:
                            Meta.Ident id = (Meta.Ident)i;
                            id.ident = -1;
                            for (int e = 0; e < MetaList.Count; e++)
                            {
                                Meta tempm = (Meta)MetaList[e];
                                if (tempm.name == id.pointstotagname && tempm.type == id.pointstotagtype)
                                {
                                    id.ident = newident[e];
                                    break;
                                }
                            }

                            map.BW.BaseStream.Position = newoffset[x] + id.offset;
                            map.BW.Write(id.ident);
                            break;
                        case Meta.ItemType.Reflexive:
                            if (m.type != "sbsp")
                            {
                                Meta.Reflexive reflex = (Meta.Reflexive)i;
                                for (int e = 0; e < MetaList.Count; e++)
                                {
                                    Meta tempm = (Meta)MetaList[e];
                                    if (reflex.pointstotagname == tempm.name && reflex.pointstotagtype == tempm.type)
                                    {
                                        reflex.pointstoTagIndex = e;
                                        break;
                                    }
                                }

                                int newreflex = reflex.translation + newoffset[reflex.pointstoTagIndex] +
                                                map.SecondaryMagic;
                                map.BW.BaseStream.Position = newoffset[x] + reflex.offset;
                                map.BW.Write(reflex.chunkcount);
                                map.BW.Write(newreflex);
                            }

                            break;
                        case Meta.ItemType.String:
                            Meta.String s = (Meta.String)i;
                            short stringnum = 0;
                            byte stringlength = 0;
                            for (int e = 0; e < strings.Count; e++)
                            {
                                if (s.name == (string)strings[e])
                                {
                                    stringnum = (short)e;
                                    stringlength = (byte)((string)strings[e]).Length;
                                    break;
                                }
                            }

                            map.BW.BaseStream.Position = newoffset[x] + s.offset;
                            map.BW.Write(stringnum);
                            map.BW.Write(zero);
                            map.BW.Write(stringlength);
                            break;
                    }
                }
            }

            // totalshift+=howfar;
            int tempfilesize = curroffset + map.MapHeader.metaStart + howfar;

            // map.MapHeader.fileSize+totalshift+howfar-map.MetaInfo.Size [map.IndexHeader.metaCount-1];
            padding = map.Functions.Padding(tempfilesize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = tempfilesize;
            map.BW.Write(tempb);
            tempfilesize += padding;

            int olddifference = map.MapHeader.fileSize - (map.MapHeader.indexOffset);
            int difference = tempfilesize - (map.MapHeader.indexOffset + totalshift);
            int metasize = tempfilesize - (curroffset + map.MapHeader.metaStart);

            int combined = bspmetasize[0] + (tempfilesize - curroffset);

            map.BW.BaseStream.Position = 8;
            map.BW.Write(tempfilesize);
            map.BW.BaseStream.Position = 24;
            map.BW.Write(metasize);
            map.BW.Write(combined);

            map.BW.BaseStream.SetLength(tempfilesize);

            map.CloseMap();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The recursively check metas.
        /// </summary>
        /// <param name="tn">The tn.</param>
        /// <remarks></remarks>
        private void RecursivelyCheckMetas(TreeNode tn)
        {
            foreach (TreeNode n in tn.Nodes)
            {
                if (n.Checked == false)
                {
                    continue;
                }

                if (n.Text.IndexOf('.') == -1)
                {
                    RecursivelyCheckMetas(n);
                    continue;
                }

                StatusLabel1.Text = "Processing: " + n.Text + "...";
                Application.DoEvents();
                int id = 0;
                for (int xx = 0; xx < map.IndexHeader.metaCount; xx++)
                {
                    string[] tempn = map.FileNames.Name[xx].Split('\\');

                    string tempi = tempn[tempn.Length - 1] + "." + map.MetaInfo.TagType[xx];
                    int i = map.FileNames.Name[xx].LastIndexOf('\\');
                    string tempp = string.Empty;
                    if (i != -1)
                    {
                        tempp = map.FileNames.Name[xx].Substring(0, i) + "\\";
                    }

                    if (n.Text == tempi && n.Tag.ToString() == map.FileNames.Name[xx] + "." + map.MetaInfo.TagType[xx])
                    {
                        id = xx;
                        break;
                    }
                }

                if (map.MetaInfo.TagType[id] == "ltmp")
                {
                    Meta templtmp = new Meta(map);
                    templtmp.TagIndex = id;
                    templtmp.type = map.MetaInfo.TagType[id];
                    templtmp.name = map.FileNames.Name[id];
                    templtmp.offset = 0;
                    templtmp.size = 0;
                    templtmp.ident = map.MetaInfo.Ident[id];
                    templtmp.MS = new MemoryStream(0);
                    templtmp.rawType = RawDataContainerType.Empty;
                    templtmp.items = new List<Meta.Item>();
                    MetaList.Add(templtmp);
                    continue;
                }

                if (map.MetaInfo.TagType[id] == "ugh!")
                {
                    continue;
                }

                if (map.MetaInfo.TagType[id] == "snd!")
                {
                    // dontscanraw = true;
                }

                Meta m = new Meta(map);
                m.ReadMetaFromMap(id, false);
                if (m.type != "sbsp" && m.type != "jmad")
                {
                    IFPIO ifp = IFPHashMap.GetIfp(m.type, map.HaloVersion);

                    // m.parsed = true;
                    m.headersize = ifp.headerSize;
                    m.scanner.ScanWithIFP(ref ifp);
                }
                else
                {
                    m.scanner.ScanManually();
                }

                switch (m.type.Trim())
                {
                    case "matg":
                        matg = m;
                        
                        break;
                    case "sncl":
                        sncl = m;
                        break;
                    case "spk!":
                        spk = m;
                        
                        break;
                    case "scnr":
                        scnr = m;
                        
                        break;

                    case "sky":
                        this.Skies.Add(m);
                        skycount++;
                        break;
                    default:
                        MetaList.Add(m);
                        count++;
                        break;
                }

                RecursivelyCheckMetas(n);
            }
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            if (addinfo.ShowDialog() != DialogResult.Cancel)
            {
                ListViewItem l = new ListViewItem(addinfo.FileName);
                l.Checked = true;
                listView2.Items.Add(l);
            }
        }

        /// <summary>
        /// The extract map button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void extractMapButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            count = 4 + map.BSP.sbsp.Length;

            // MetaList.Capacity = 10000;
            StatusLabel1.Text = "Processing Map Metas...";
            Application.DoEvents();

            if (treeView1.Nodes[0].Checked)
            {
                RecursivelyCheckMetas(treeView1.Nodes[0]);
            }

            StatusLabel1.Text = "Processing Imported Metas...";
            Application.DoEvents();

            foreach (ListViewItem l in listView2.Items)
            {
                if (l.Checked == false)
                {
                    continue;
                }

                StatusLabel1.Text = "Processing: " + l.Text + "...";
                Application.DoEvents();

                string[] split = l.Text.Split('.');
                if (split[split.Length - 1] != "info")
                {
                    Meta m = new Meta(map);
                    m.LoadMetaFromFile(l.Text);
                    MetaList.Add(m);
                }
                else
                {
                    FileStream FS = new FileStream(l.Text, FileMode.Open);
                    StreamReader SR = new StreamReader(FS);
                    string temps = string.Empty;
                    do
                    {
                        temps = SR.ReadLine();

                        if (temps == null)
                        {
                            break;
                        }

                        Meta m = new Meta(map);
                        m.LoadMetaFromFile(temps);
                        bool exists = false;
                        for (int x = 0; x < map.IndexHeader.metaCount; x++)
                        {
                            if (map.FileNames.Name[x] == m.name && map.MetaInfo.TagType[x] == m.type)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (exists == false)
                        {
                            for (int x = 0; x < MetaList.Count; x++)
                            {
                                if (((Meta)MetaList[x]).name == m.name && ((Meta)MetaList[x]).type == m.type)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                        }

                        if (exists == false)
                        {
                            switch (m.type.Trim())
                            {
                                case "matg":
                                    matg = m;
                                    break;
                                case "sncl":
                                    sncl = m;
                                    break;
                                case "spk!":
                                    spk = m;
                                    break;
                                case "scnr":
                                    scnr = m;
                                    break;

                                case "sky":
                                    this.Skies.Add(m);
                                    skycount++;
                                    break;
                                default:
                                    MetaList.Add(m);
                                    count++;
                                    break;
                            }
                        }
                    }
                    while (temps != null);

                    SR.Close();
                    FS.Close();
                }
            }

            for (int x = skycount - 1; x > -1; x--)
            {
                MetaList.Insert(0, Skies[x]);
            }

            MetaList.Insert(0, scnr);
            MetaList.Insert(0, spk);
            MetaList.Insert(0, sncl);
            MetaList.Insert(0, matg);
            StatusLabel1.Text = "Starting Rebuild....";
            Application.DoEvents();
            MapAnalyzer ma = new MapAnalyzer();
            MapLayout lay = ma.ScanMapForLayOut(map, false);
            lay.ReadChunks(map);

            MapRebuilder(ref lay);
            MetaList.Clear();
            StatusLabel1.Text = "Done";
            this.Enabled = true;
        }

        #endregion

        // float temp=(float) x / (float)map.IndexHeader.metaCount;
        // temp*=100;
        // ProgressBar1.Value = (int)temp;
    }
}