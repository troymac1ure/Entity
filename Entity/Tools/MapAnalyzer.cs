// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapAnalyzer.cs" company="">
//   
// </copyright>
// <summary>
//   The lay out chunk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Tools
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using HaloMap;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    /// <summary>
    /// The lay out chunk.
    /// </summary>
    /// <remarks></remarks>
    public class LayOutChunk
    {
        #region Constants and Fields

        /// <summary>
        /// The ms.
        /// </summary>
        public MemoryStream MS;

        /// <summary>
        /// The endoffset.
        /// </summary>
        public int endoffset;

        /// <summary>
        /// The raw pieces.
        /// </summary>
        public ArrayList rawPieces = new ArrayList(0);

        /// <summary>
        /// The raw type.
        /// </summary>
        public RawDataContainerType rawType;

        /// <summary>
        /// The shift.
        /// </summary>
        public int shift;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        /// <summary>
        /// The startoffset.
        /// </summary>
        public int startoffset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayOutChunk"/> class.
        /// </summary>
        /// <param name="mapsize">The mapsize.</param>
        /// <remarks></remarks>
        public LayOutChunk(int mapsize)
        {
            startoffset = mapsize;
            endoffset = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void Read(Map map)
        {
            map.BR.BaseStream.Position = startoffset;
            MS = new MemoryStream(size);
            BinaryWriter BW = new BinaryWriter(MS);

            // MS.Write(map.BR.ReadBytes(size), 0, size);
            int buffersize = 0x1900000;
            int currread = 0;
            do
            {
                int tempsize = size - currread;
                int buffsize = buffersize;
                if (tempsize < buffersize)
                {
                    buffsize = size - currread;
                    BW.Write(map.BR.ReadBytes(buffsize));
                    break;
                }

                BW.Write(map.BR.ReadBytes(buffsize));
                currread += buffersize;
                GC.Collect(0);
                GC.WaitForPendingFinalizers();
            }
            while (currread != -1);
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="BW">The bw.</param>
        /// <param name="offset">The offset.</param>
        /// <remarks></remarks>
        public void Write(BinaryWriter BW, int offset)
        {
            BW.BaseStream.Position = offset;
            BinaryReader BR = new BinaryReader(MS);

            int size = (int)MS.Length;
            int buffersize = 0x1400000;
            int currread = 0;
            do
            {
                int tempsize = size - currread;
                int buffsize = buffersize;
                if (tempsize < buffersize)
                {
                    buffsize = size - currread;
                    BW.Write(BR.ReadBytes(buffsize));
                    break;
                }

                BW.Write(BR.ReadBytes(buffsize));
                currread += buffersize;
                GC.Collect(0);
                GC.WaitForPendingFinalizers();
            }
            while (currread != -1);
        }

        #endregion
    }

    /// <summary>
    /// The lay out sorter.
    /// </summary>
    /// <remarks></remarks>
    public class LayOutSorter : IComparer
    {
        #region Implemented Interfaces

        #region IComparer

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The compare.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.
        /// -or-
        ///   <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        ///   </exception>
        /// <remarks></remarks>
        public int Compare(object x, object y)
        {
            return ((LayOutChunk)x).startoffset.CompareTo(((LayOutChunk)y).startoffset);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The map analyzer.
    /// </summary>
    /// <remarks></remarks>
    public class MapAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// The scan map for lay out.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="addexternalchunks">The addexternalchunks.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MapLayout ScanMapForLayOut(Map map, bool addexternalchunks)
        {
            MapLayout layout = new MapLayout();

            for (int x = 0; x < map.BSP.sbsp.Length; x++)
            {
                LayOutChunk l = new LayOutChunk(0);
                l.rawType = RawDataContainerType.BSPMeta;
                l.startoffset = map.BSP.sbsp[x].offset;
                l.endoffset = map.BSP.sbsp[x].offset + map.BSP.sbsp[x].size;
                l.size = map.BSP.sbsp[x].size;
                layout.chunks.Add(l);
            }

            LayOutChunk lo = new LayOutChunk(0);
            lo.rawType = RawDataContainerType.Header;
            lo.startoffset = 0;
            lo.size = 2048;
            lo.endoffset = lo.startoffset + lo.size;
            layout.chunks.Add(lo);

            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                lo = new LayOutChunk(0);
                lo.rawType = RawDataContainerType.StringsIndex;
                lo.startoffset = map.MapHeader.offsetToStringIndex;
                lo.size = map.MapHeader.scriptReferenceCount * 4;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);

                lo = new LayOutChunk(0);
                lo.rawType = RawDataContainerType.Strings1;
                lo.startoffset = map.MapHeader.offsetToStringNames1;
                lo.size = map.MapHeader.scriptReferenceCount * 128;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);

                lo = new LayOutChunk(0);
                lo.rawType = RawDataContainerType.Strings2;
                lo.startoffset = map.MapHeader.offsetToStringNames2;
                lo.size = map.MapHeader.sizeOfScriptReference;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);

                lo = new LayOutChunk(0);
                lo.rawType = RawDataContainerType.Crazy;
                lo.startoffset = map.MapHeader.offsetToCrazy;
                lo.size = map.MapHeader.sizeOfCrazy;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);

                lo = new LayOutChunk(0);
                lo.rawType = RawDataContainerType.FileNamesIndex;
                lo.startoffset = map.MapHeader.offsetTofileIndex;
                lo.size = map.MapHeader.fileCount * 4;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);
            }

            lo = new LayOutChunk(0);
            lo.rawType = RawDataContainerType.FileNames;
            lo.startoffset = map.MapHeader.offsetTofileNames;
            lo.size = map.MapHeader.fileNamesSize;
            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                lo.size += map.Functions.Padding(lo.size, 512);
            }

            lo.endoffset = lo.startoffset + lo.size;
            layout.chunks.Add(lo);

            lo = new LayOutChunk(0);
            lo.rawType = RawDataContainerType.MetaIndex;
            lo.startoffset = map.MapHeader.indexOffset;
            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                lo.size = map.MapHeader.metaStart;
                lo.size += map.Functions.Padding(lo.size, 512);

                // map.MapHeader.fileSize - map.MapHeader.indexOffset;
            }
            else
            {
                lo.size = map.MapHeader.offsetTofileNames - map.MapHeader.indexOffset;

                // map.MetaInfo.Offset[map.IndexHeader.metaCount - 1] + map.MetaInfo.Size[map.IndexHeader.metaCount-1];
            }

            lo.endoffset = lo.startoffset + lo.size;
            layout.chunks.Add(lo);

            lo = new LayOutChunk(0);
            lo.rawType = RawDataContainerType.MetaData;
            lo.startoffset = map.MetaInfo.Offset[0];

            // How can you adjust for padding past the end of the file???
            lo.size = map.MapHeader.fileSize - lo.startoffset;
            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                int padding = map.Functions.Padding(lo.startoffset + lo.size, 4096);
                lo.size += padding;
            }

            lo.endoffset = lo.startoffset + lo.size;
            layout.chunks.Add(lo);

            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                for (int x = 0; x < map.Unicode.ut.Length; x++)
                {
                    lo = new LayOutChunk(0);
                    lo.rawType = RawDataContainerType.UnicodeNamesIndex;
                    lo.startoffset = map.Unicode.ut[x].indexOffset;
                    lo.size = map.Unicode.ut[x].count * 8;
                    lo.size += map.Functions.Padding(lo.size, 512);
                    lo.endoffset = lo.startoffset + lo.size;
                    layout.chunks.Add(lo);

                    lo = new LayOutChunk(0);
                    lo.rawType = RawDataContainerType.UnicodeNames;
                    lo.startoffset = map.Unicode.ut[x].tableOffset;
                    lo.size = map.Unicode.ut[x].tableSize;
                    lo.size += map.Functions.Padding(lo.size, 512);
                    lo.endoffset = lo.startoffset + lo.size;
                    layout.chunks.Add(lo);
                }
            }

            map.OpenMap(MapTypes.Internal);
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                    map.HaloVersion == HaloVersionEnum.Halo2Vista)
                {
                    if (map.MetaInfo.TagType[x] == "snd!" | map.MetaInfo.TagType[x] == "ltmp")
                    {
                        continue;
                    }

                    Application.DoEvents();
                }

                Meta m = new Meta(map);
                m.offset = map.MetaInfo.Offset[x];

                // checks if type has raw data
                m.rawType = map.Functions.ForMeta.CheckForRaw(map.MetaInfo.TagType[x]);

                if (m.rawType != RawDataContainerType.Empty)
                {
                    m.raw = map.Functions.ForMeta.ReadRaw(x, true);
                    LayOutChunk l = null;

                    int layoutChunkIndex = (m.rawType == RawDataContainerType.BSP) ? -1 : layout.FindByType(m.rawType);
                    if (layoutChunkIndex == -1)
                    {
                        l = new LayOutChunk(map.MapHeader.fileSize);
                        l.rawType = m.rawType;
                        layout.chunks.Add(l);
                        layoutChunkIndex = layout.FindByType(m.rawType);
                    }
                    else
                    {
                        l = (LayOutChunk)layout.chunks[layoutChunkIndex];
                    }

                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        if (r.offset == -1) { continue; }

                        if (r.rawLocation == MapTypes.Internal)
                        {
                            RawInfoChunk tempr = new RawInfoChunk();
                            tempr.offset = (uint)r.offset;
                            tempr.size = r.size;
                            tempr.rawType = r.rawDataType;
                            tempr.location = r.rawLocation;
                            tempr.offsetOfPointer = m.offset + r.pointerMetaOffset;
                            l.rawPieces.Add(tempr);
                        }
                        else if (addexternalchunks)
                        {
                            RawInfoChunk tempr = new RawInfoChunk();
                            tempr.offset = (uint)r.offset;
                            tempr.size = r.size;
                            tempr.rawType = r.rawDataType;
                            tempr.location = r.rawLocation;
                            tempr.offsetOfPointer = m.offset + r.pointerMetaOffset;
                            l.rawPieces.Add(tempr);
                        }

                        if (r.offset < l.startoffset && r.rawLocation == MapTypes.Internal)
                        {
                            l.startoffset = r.offset;
                            l.size = l.endoffset - l.startoffset;
                        }

                        if (r.offset + r.size > l.endoffset && r.rawLocation == MapTypes.Internal)
                        {
                            l.endoffset = r.offset + r.size;
                            l.endoffset += map.Functions.Padding(l.endoffset, 512);
                            l.size = l.endoffset - l.startoffset;
                        }
                    }
                }

                m = null;
                GC.WaitForPendingFinalizers();

                // GC.Collect();
            }

            map.CloseMap();

            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                lo = new LayOutChunk(0);
                LayOutChunk templo = (LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.Model)];
                lo.rawType = RawDataContainerType.Sound;
                lo.startoffset = 2048;
                lo.size = templo.startoffset - 2048;
                lo.size += map.Functions.Padding(lo.size, 512);
                lo.endoffset = lo.startoffset + lo.size;
                layout.chunks.Add(lo);
            }

            for (int y = 0; y < layout.chunks.Count; y++)
            {
                LayOutChunk l = (LayOutChunk)layout.chunks[y];
                if (l.size == 0)
                {
                    layout.chunks.RemoveAt(y);
                    y--;
                }
            }

            //layout.SortChunksByOffset();
            //layout.SortRawByOffset();

            return layout;
        }

        #endregion
    }

    /// <summary>
    /// The map layout.
    /// </summary>
    /// <remarks></remarks>
    public class MapLayout
    {
        #region Constants and Fields

        /// <summary>
        /// The chunks.
        /// </summary>
        public ArrayList chunks = new ArrayList(0);

        /// <summary>
        /// The metas.
        /// </summary>
        public ArrayList metas = new ArrayList(0);

        #endregion

        #region Public Methods

        /// <summary>
        /// The find by type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The find by type.</returns>
        /// <remarks></remarks>
        public int FindByType(RawDataContainerType type)
        {
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk c = (LayOutChunk)chunks[x];
                if (c.rawType == type)
                {
                    return x;
                }
            }

            return -1;
        }

        /// <summary>
        /// The find by type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="index">The index.</param>
        /// <returns>The find by type.</returns>
        /// <remarks></remarks>
        public int FindByType(RawDataContainerType type, int index)
        {
            int tempc = -1;
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk c = (LayOutChunk)chunks[x];
                if (c.rawType == type)
                {
                    tempc++;
                    if (tempc == index)
                    {
                        return x;
                    }
                }
            }

            return -1;
        }

        public int FindLastByType(RawDataContainerType type)
        {
            int index = -1;
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk c = (LayOutChunk)chunks[x];
                if (c.rawType == type)
                    index = x;
            }
            return index;
        }

        /// <summary>
        /// The read chunks.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void ReadChunks(Map map)
        {
            map.OpenMap(MapTypes.Internal);
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk l = (LayOutChunk)chunks[x];
                l.Read(map);
                GC.Collect(0);
                GC.WaitForPendingFinalizers();
            }

            map.CloseMap();
        }

        /// <summary>
        /// The save to xml.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void SaveToXml(string path, Map map)
        {
            XmlTextWriter xtw = new XmlTextWriter(path, Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("MapLayOut");
            xtw.WriteAttributeString("Map", map.filePath);
            int tempindex = path.LastIndexOf("\\");
            string tempfilepath = path.Substring(0, tempindex) + "\\" +
                                  path.Substring(tempindex + 1, path.Length - tempindex - 1) + " - Meta Chunks\\";
            Directory.CreateDirectory(tempfilepath);
            map.OpenMap(MapTypes.Internal);
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk c = (LayOutChunk)chunks[x];

                // c.Read(map);
                FileStream FS = new FileStream(
                    tempfilepath + "MapMetaChunk[" + x + "] - " + c.rawType + ".meta", FileMode.Create);
                BinaryWriter BW = new BinaryWriter(FS);

                // BW.Write(c.MS.ToArray());
                // c.Write(BW, 0);
                map.BR.BaseStream.Position = c.startoffset;
                map.BufferReadWrite(ref map.BR, ref BW, c.size);
                BW.Close();
                FS.Close();

                xtw.WriteStartElement("LayOutChunk");
                xtw.WriteAttributeString("Type", c.rawType.ToString());
                xtw.WriteAttributeString("StartOffset", c.startoffset.ToString("X"));
                xtw.WriteAttributeString("EndOffset", c.endoffset.ToString("X"));
                xtw.WriteAttributeString("Size", c.size.ToString("X"));

                for (int xx = 0; xx < c.rawPieces.Count; xx++)
                {
                    RawInfoChunk cc = (RawInfoChunk)c.rawPieces[xx];
                    xtw.WriteStartElement("RawDataPiece");
                    xtw.WriteAttributeString("Type", cc.rawType.ToString());
                    xtw.WriteAttributeString("Offset", cc.offset.ToString("X"));
                    xtw.WriteAttributeString("Size", cc.size.ToString("X"));
                    xtw.WriteAttributeString("PointerOffset", cc.offsetOfPointer.ToString("X"));
                    xtw.WriteEndElement();
                }

                xtw.WriteEndElement();
            }

            xtw.WriteEndElement();
            xtw.Close();
            map.CloseMap();
        }

        /// <summary>
        /// The sort chunks by offset.
        /// </summary>
        /// <remarks></remarks>
        public void SortChunksByOffset()
        {
            chunks.Sort(new LayOutSorter());
        }

        /// <summary>
        /// The sort raw by offset.
        /// </summary>
        /// <remarks></remarks>
        public void SortRawByOffset()
        {
            for (int x = 0; x < chunks.Count; x++)
            {
                LayOutChunk l = (LayOutChunk)chunks[x];
                l.rawPieces.Sort(new RawInfoSorter());
            }
        }

        #endregion
    }

    /// <summary>
    /// The raw info chunk.
    /// </summary>
    /// <remarks></remarks>
    public class RawInfoChunk
    {
        #region Constants and Fields

        /// <summary>
        /// The location.
        /// </summary>
        public MapTypes location;

        /// <summary>
        /// The offset.
        /// </summary>
        public uint offset;

        /// <summary>
        /// The offset of pointer.
        /// </summary>
        public int offsetOfPointer;

        /// <summary>
        /// The raw type.
        /// </summary>
        public RawDataType rawType;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        #endregion
    }

    /// <summary>
    /// The raw info sorter.
    /// </summary>
    /// <remarks></remarks>
    public class RawInfoSorter : IComparer
    {
        #region Implemented Interfaces

        #region IComparer

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The compare.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.
        /// -or-
        ///   <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        ///   </exception>
        /// <remarks></remarks>
        public int Compare(object x, object y)
        {
            return ((RawInfoChunk)x).offset.CompareTo(((RawInfoChunk)y).offset);
        }

        #endregion

        #endregion
    }
}