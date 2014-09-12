// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Builder.cs" company="">
//   
// </copyright>
// <summary>
//   The builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Tools
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Windows.Forms;


    using HaloMap;
    using HaloMap.ChunkCloning;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.RawData;

    /// <summary>
    /// The builder.
    /// </summary>
    /// <remarks></remarks>
    public class Builder
    {
        #region Public Methods

        /// <summary>
        /// The build map from info file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="map">The map.</param>
        /// <param name="addsounds">The addsounds.</param>
        /// <remarks></remarks>
        public void BuildMapFromInfoFile(string inputFile, ref MapLayout layout, Map map, bool addsounds)
        {
            ArrayList metas = new ArrayList(0);
            string[] split = inputFile.Split('.');
            if (split[split.Length - 1] == "info")
            {
                FileStream FS = new FileStream(inputFile, FileMode.Open);
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
                    if (addsounds == false)
                    {
                        if (m.type == "snd!")
                        {
                            continue;
                        }
                    }

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
                        for (int x = 0; x < metas.Count; x++)
                        {
                            if (((Meta)metas[x]).name == m.name && ((Meta)metas[x]).type == m.type)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (exists == false)
                    {
                        metas.Add(m);
                    }
                }
                while (temps != null);

                SR.Close();
                FS.Close();
            }
            else
            {
                Meta m = new Meta(map);
                m.LoadMetaFromFile(inputFile);
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
                    metas.Add(m);
                }
            }

            MapBuilder(metas, ref layout, map, addsounds);
        }

        /// <summary>
        /// The map builder.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="map">The map.</param>
        /// <param name="addsounds">The addsounds.</param>
        /// <remarks></remarks>
        public void MapBuilder(ArrayList metas, ref MapLayout layout, Map map, bool addsounds)
        {
            string[] filestofix = new string[0];
            if (map.MapHeader.mapType != MapTypes.Internal)
            {
                if (
                    MessageBox.Show(
                        "This map is an external resource and updating it will effect all the other maps. Continue?",
                        string.Empty,
                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }

                OpenFileDialog openfiles = new OpenFileDialog();
                openfiles.Multiselect = true;
                openfiles.Filter = "Halo 2 Map (*.map)| *.map";
                openfiles.ShowDialog();

                filestofix = openfiles.FileNames;
            }

            if (addsounds == false)
            {
                for (int x = 0; x < metas.Count; x++)
                {
                    if (((Meta)metas[x]).type == "snd!")
                    {
                        metas.RemoveAt(x);
                        x--;
                    }
                }
            }

            int totalshift = 0;

            ArrayList strings = new ArrayList();
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                for (int y = 0; y < m.items.Count; y++)
                {
                    Meta.Item ii = m.items[y];
                    if (ii.type == Meta.ItemType.String)
                    {
                        Meta.String iii = (Meta.String)ii;
                        if (Array.IndexOf(map.Strings.Name, iii.name) == -1)
                        {
                            if (strings.IndexOf(iii.name) == -1)
                            {
                                strings.Add(iii.name);
                            }
                        }
                    }
                }
            }


            #region read ugh meta
            map.OpenMap(MapTypes.Internal);
            Meta ughmeta = new Meta(map);
            ughmeta.ReadMetaFromMap(map.IndexHeader.metaCount - 1, true);

            map.BR.BaseStream.Position = ughmeta.offset + 84;
            int oldUghRawInfoReflexiveStart = map.BR.ReadInt32() - map.SecondaryMagic;

            IFPIO ifp = IFPHashMap.GetIfp("ugh!", map.HaloVersion);
            ughmeta.rawType = RawDataContainerType.Empty;
            ughmeta.headersize = ifp.headerSize;

            ughmeta.scanner.ScanWithIFP(ref ifp);
            #endregion


            #region get model info

            int tempint = layout.FindByType(RawDataContainerType.Model);
            LayOutChunk loc = (LayOutChunk)layout.chunks[tempint];
            LayOutChunk loc2 = null;
            int oldModelRawStartOffset = loc.startoffset;

            #endregion

            #region sound raw data

            int sndshift = 0;
            int sndpermcount = 0;
            int sndchoicecount = 0;
            int sndchunk1count = 0;
            int addedsoundnames = 0;

            MetaSplitter metasplit = new MetaSplitter();
            metasplit.SplitWithIFP(ref ifp, ref ughmeta, map);
            map.OpenMap(MapTypes.Internal);
            int soundnameindex = 0;
            int soundpermutationindex = 0;
            int soundchoiceindex = 0;
            int soundchunk1index = 0;
            for (int x = 0; x < metasplit.Header.Chunks[0].ChunkResources.Count; x++)
            {
                if (metasplit.Header.Chunks[0].ChunkResources[x].type == Meta.ItemType.Reflexive)
                {
                    Meta.Reflexive r = (Meta.Reflexive)metasplit.Header.Chunks[0].ChunkResources[x];
                    if (r.offset == 16)
                    {
                        soundnameindex = x;
                    }

                    if (r.offset == 32)
                    {
                        soundpermutationindex = x;
                    }

                    if (r.offset == 40)
                    {
                        soundchoiceindex = x;
                    }

                    if (r.offset == 64)
                    {
                        soundchunk1index = x;
                    }
                }
            }

            MetaSplitter.SplitReflexive soundnamereflexive =
                metasplit.Header.Chunks[0].ChunkResources[soundnameindex] as MetaSplitter.SplitReflexive;
            MetaSplitter.SplitReflexive soundpermutationreflexive =
                metasplit.Header.Chunks[0].ChunkResources[soundpermutationindex] as MetaSplitter.SplitReflexive;
            MetaSplitter.SplitReflexive soundchoicereflexive =
                metasplit.Header.Chunks[0].ChunkResources[soundchoiceindex] as MetaSplitter.SplitReflexive;
            MetaSplitter.SplitReflexive soundchunk1reflexive =
                metasplit.Header.Chunks[0].ChunkResources[soundchunk1index] as MetaSplitter.SplitReflexive;
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.Sound)
                {
                    Sound tempsound = (Sound)m.raw;
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    int y = 0;

                    // writes new index to meta
                    BW.BaseStream.Position = 8;
                    ushort temppermindex = (ushort)(map.ugh.Permutations.Count + sndpermcount);
                    BW.Write(temppermindex);

                    for (int e = 0; e < tempsound.Permutations.Length; e++)
                    {
                        for (int ee = 0; ee < tempsound.Permutations[e].choicecount; ee++)
                        {
                            int nindex = map.ugh.SoundNames.IndexOf(tempsound.Permutations[e].Choices[ee].Name);
                            if (nindex == -1)
                            {
                                int nindex2 = strings.IndexOf(tempsound.Permutations[e].Choices[ee].Name);
                                if (nindex2 == -1)
                                {
                                    strings.Add(tempsound.Permutations[e].Choices[ee].Name);
                                    nindex2 = strings.Count - 1;
                                }

                                nindex2 += map.Strings.Name.Length;
                                map.ugh.SoundNames.Add(tempsound.Permutations[e].Choices[ee].Name);
                                tempsound.Permutations[e].Choices[ee].NameIndex = (ushort)(map.ugh.SoundNames.Count - 1);

                                MetaSplitter.SplitReflexive tempss = new MetaSplitter.SplitReflexive();
                                tempss.chunksize = soundnamereflexive.Chunks[0].chunksize;
                                tempss.MS = new MemoryStream(tempss.chunksize);
                                BinaryWriter soundnameBW = new BinaryWriter(tempss.MS);
                                soundnameBW.BaseStream.Position = 0;
                                byte z = 0;
                                soundnameBW.Write((ushort)nindex2);
                                soundnameBW.Write(z);
                                soundnameBW.Write((byte)tempsound.Permutations[e].Choices[ee].Name.Length);
                                soundnamereflexive.Chunks.Add(tempss);
                                addedsoundnames++;
                            }
                            else
                            {
                                tempsound.Permutations[e].Choices[ee].NameIndex = (ushort)nindex;
                            }
                        }
                    }

                    // cycle through permutations and write them
                    for (int e = 0; e < tempsound.Permutations.Length; e++)
                    {
                        MetaSplitter.SplitReflexive tempss = new MetaSplitter.SplitReflexive();
                        tempss.chunksize = soundpermutationreflexive.Chunks[0].chunksize;
                        tempss.MS = new MemoryStream(tempss.chunksize);
                        BinaryWriter permBW = new BinaryWriter(tempss.MS);
                        permBW.BaseStream.Position = 0;

                        permBW.Write(tempsound.Permutations[e].unknown1);
                        permBW.Write(tempsound.Permutations[e].unknown2);
                        tempsound.Permutations[e].choiceindex = (ushort)(map.ugh.Choices.Count + sndchoicecount);
                        permBW.Write(tempsound.Permutations[e].choiceindex);
                        permBW.Write(tempsound.Permutations[e].choicecount);
                        soundpermutationreflexive.Chunks.Add(tempss);
                        for (int ee = 0; ee < tempsound.Permutations[e].choicecount; ee++)
                        {
                            MetaSplitter.SplitReflexive tempsss = new MetaSplitter.SplitReflexive();
                            tempsss.chunksize = soundchoicereflexive.Chunks[0].chunksize;
                            tempsss.MS = new MemoryStream(tempsss.chunksize);
                            BinaryWriter choiceBW = new BinaryWriter(tempsss.MS);
                            choiceBW.BaseStream.Position = 0;

                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].NameIndex);
                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].unknown1);
                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].unknown2);

                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].unknown3);
                            tempsound.Permutations[e].Choices[ee].soundindex =
                                (ushort)(map.ugh.SoundChunks1.Count + sndchunk1count);
                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].soundindex);
                            choiceBW.Write(tempsound.Permutations[e].Choices[ee].soundcount);
                            soundchoicereflexive.Chunks.Add(tempsss);

                            for (int eee = 0; eee < tempsound.Permutations[e].Choices[ee].soundcount; eee++)
                            {
                                RawDataChunk r = m.raw.rawChunks[y];

                                // write raw to map
                                int tempintx = loc.startoffset + sndshift;
                                map.BW.BaseStream.Position = tempintx;
                                map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                                // write padding
                                int tempinty = map.Functions.Padding(r.size, 512);
                                byte[] tempbytes = new byte[tempinty];
                                map.BW.Write(tempbytes);
                                sndshift += r.size + tempinty;
                                y++;

                                MetaSplitter.SplitReflexive tempssss = new MetaSplitter.SplitReflexive();
                                tempssss.chunksize = soundchoicereflexive.Chunks[0].chunksize;
                                tempssss.MS = new MemoryStream(tempssss.chunksize);
                                BinaryWriter sndchnk1BW = new BinaryWriter(tempssss.MS);
                                sndchnk1BW.BaseStream.Position = 0;
                                sndchnk1BW.Write(tempintx);
                                sndchnk1BW.Write(tempsound.Permutations[e].Choices[ee].SoundChunks1[eee].size);
                                sndchnk1BW.Write(tempsound.Permutations[e].Choices[ee].SoundChunks1[eee].unknown1);
                                sndchnk1BW.Write(tempsound.Permutations[e].Choices[ee].SoundChunks1[eee].unknown2);
                                soundchunk1reflexive.Chunks.Add(tempssss);
                                sndchunk1count++;
                            }

                            sndchoicecount++;
                        }

                        sndpermcount++;
                    }

                    metas[x] = m;
                }
            }

            totalshift += sndshift;
            if (sndshift > 0)
            {
                metasplit.Header.Chunks[0].ChunkResources[soundnameindex] = soundnamereflexive;
                metasplit.Header.Chunks[0].ChunkResources[soundpermutationindex] = soundpermutationreflexive;
                metasplit.Header.Chunks[0].ChunkResources[soundchoiceindex] = soundchoicereflexive;
                metasplit.Header.Chunks[0].ChunkResources[soundchunk1index] = soundchunk1reflexive;

                ughmeta = MetaBuilder.BuildMeta(metasplit, map);
            }

            metas.Add(ughmeta);

            #endregion

            #region model raw data

            int modeshift = 0;

            loc.startoffset += totalshift;
            loc.endoffset = loc.startoffset + loc.size;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.Model)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.endoffset + modeshift;

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

                    metas[x] = m;
                }
            }

            loc.size += modeshift;
            loc.endoffset = loc.startoffset + loc.size;

            layout.chunks[tempint] = loc;
            totalshift += modeshift;

            #endregion

            #region bsp raw

            bool foundbsptoimport = false;
            Meta bspToImport = null;
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.type == "sbsp")
                {
                    foundbsptoimport = true;
                    bspToImport = m;
                    metas.RemoveAt(x);
                    break;
                }
            }

            int bspshift = 0;
            int newprimarymagicconstant = map.BSP.sbsp[0].magic + ((LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.BSPMeta)]).startoffset;
            int oldBSPRawStartOffset = ((LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.BSP)]).startoffset;

            if (foundbsptoimport)
            {
                tempint = layout.FindByType(RawDataContainerType.BSP);
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                int tempint2 = layout.FindByType(RawDataContainerType.BSPMeta);
                loc2 = (LayOutChunk)layout.chunks[tempint2];
                uint bsprawsize = (uint)loc.size;
                uint oldbspsize = (uint)loc.size;
                int bspmagic = map.BSP.sbsp[0].magic;
                bool found = false;

                bsprawsize = 0;
                BinaryWriter BW = new BinaryWriter(bspToImport.MS);
                int[] tempoff = new int[bspToImport.raw.rawChunks.Count];

                for (int y = 0; y < bspToImport.raw.rawChunks.Count; y++)
                {
                    found = false;
                    RawDataChunk r = (RawDataChunk)bspToImport.raw.rawChunks[y];
                    for (int yy = 0; yy < y; yy++)
                    {
                        RawDataChunk rr = (RawDataChunk)bspToImport.raw.rawChunks[yy];
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

                    if (found == true) continue;

                    tempoff[y] = loc.startoffset + (int)bsprawsize;

                    // MessageBox.Show("Test");
                    // writes new pointer to loaded meta
                    BW.BaseStream.Position = r.pointerMetaOffset;
                    BW.Write(tempoff[y]);
                    BW.Write(r.size);

                    // writes raw to map file
                    map.BW.BaseStream.Position = loc.startoffset + bsprawsize;
                    map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                    // write padding
                    int tempinty = map.Functions.Padding(r.size, 512);
                    byte[] tempbytes = new byte[tempinty];
                    map.BW.Write(tempbytes);
                    bsprawsize += (uint)(r.size + tempinty);
                }

                totalshift += (int)(bsprawsize - oldbspsize);
                bspshift += (int)(bsprawsize - oldbspsize);

                loc.size = (int)bsprawsize;
                loc.endoffset = (int)(loc.startoffset + bsprawsize);
                layout.chunks[tempint] = loc;
                loc2.MS = bspToImport.MS;
                loc2.size = bspToImport.size;
                loc2.endoffset = loc2.startoffset + bspToImport.size;
                layout.chunks[tempint2] = loc2;
                bspmagic = bspToImport.magic;
                newprimarymagicconstant = bspmagic + bspToImport.offset;
            }
            else
            {
                //Writes SBSP Raw Data
                for (int sbspIndex = 0; sbspIndex < map.BSP.sbsp.Length; sbspIndex++)
                {
                    loc = (LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.BSP, sbspIndex)];
                    loc.startoffset += totalshift;

                    map.BW.BaseStream.Position = loc.startoffset;
                    map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
                    loc.endoffset = loc.startoffset + loc.size;
                }
            }

            #endregion

            #region weather raw data

            int weathershift = 0;
            tempint = layout.FindByType(RawDataContainerType.Weather);
            if (tempint != -1)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            }
            else
            {
                loc = new LayOutChunk(0);
                loc.rawType = RawDataContainerType.Weather;
                int tx = layout.FindByType(RawDataContainerType.BSP);
                loc.startoffset = ((LayOutChunk)layout.chunks[tx]).endoffset;
                loc.size = 0;
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.Weather)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.startoffset + loc.size + weathershift;

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

                    metas[x] = m;
                }
            }

            loc.size += weathershift;
            loc.endoffset = loc.startoffset + loc.size;
            if (tempint == -1)
            {
                if (weathershift > 0)
                {
                    layout.chunks.Add(loc);
                }

                ;
            }
            else
            {
                layout.chunks[tempint] = loc;
            }

            totalshift += weathershift;

            #endregion

            #region decr raw data

            int decrshift = 0;
            tempint = layout.FindByType(RawDataContainerType.DECR);
            if (tempint != -1)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            }
            else
            {
                loc = new LayOutChunk(0);
                loc.rawType = RawDataContainerType.DECR;
                int tx = layout.FindByType(RawDataContainerType.Weather);
                if (tx == -1)
                {
                    tx = layout.FindByType(RawDataContainerType.BSP);
                }

                loc.startoffset = ((LayOutChunk)layout.chunks[tx]).endoffset;
                loc.size = 0;
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.DECR)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.startoffset + loc.size + decrshift;

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

                    metas[x] = m;
                }
            }

            loc.size += decrshift;
            loc.endoffset = loc.startoffset + loc.size;
            if (tempint == -1)
            {
                if (decrshift > 0)
                {
                    layout.chunks.Add(loc);
                }

                ;
            }
            else
            {
                layout.chunks[tempint] = loc;
            }

            totalshift += decrshift;

            #endregion

            #region prtm raw data

            int prtmshift = 0;
            tempint = layout.FindByType(RawDataContainerType.PRTM);
            if (tempint != -1)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            }
            else
            {
                loc = new LayOutChunk(0);
                loc.rawType = RawDataContainerType.PRTM;
                int tx = layout.FindByType(RawDataContainerType.DECR);
                if (tx == -1)
                {
                    tx = layout.FindByType(RawDataContainerType.Weather);
                    if (tx == -1)
                    {
                        tx = layout.FindByType(RawDataContainerType.BSP);
                    }
                }

                loc.startoffset = ((LayOutChunk)layout.chunks[tx]).endoffset;
                loc.size = 0;
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.PRTM)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.startoffset + loc.size + prtmshift;

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

                    metas[x] = m;
                }
            }

            loc.size += prtmshift;
            loc.endoffset = loc.startoffset + loc.size;
            if (tempint == -1)
            {
                if (prtmshift > 0)
                {
                    layout.chunks.Add(loc);
                }

                ;
            }
            else
            {
                layout.chunks[tempint] = loc;
            }

            totalshift += prtmshift;

            #endregion

            #region Coconut Model Raw Data
            int coconutModelShift = 0;
            tempint = layout.FindByType(RawDataContainerType.CoconutsModel);
            if (tempint != -1)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            }
            else
            {
                loc = new LayOutChunk(0);
                loc.rawType = RawDataContainerType.CoconutsModel;
                int tx = layout.FindByType(RawDataContainerType.PRTM);
                if (tx == -1)
                {
                    tx = layout.FindByType(RawDataContainerType.DECR);
                    if (tx == -1)
                    {
                        tx = layout.FindByType(RawDataContainerType.Weather);
                        if (tx == -1)
                        {
                            tx = layout.FindByType(RawDataContainerType.BSP);
                        }
                    }
                }

                loc.startoffset = ((LayOutChunk)layout.chunks[tx]).endoffset;
                loc.size = 0;
            }
            //for (int x = 0; x < metas.Count; x++)
            //{
            //    Meta m = ((Meta)metas[x]);
            //    if (m.rawType == RawDataContainerType.CoconutsModel)
            //    {
            //        BinaryWriter BW = new BinaryWriter(m.MS);
            //        for (int y = 0; y < m.raw.rawChunks.Count; y++)
            //        {
            //            RawDataChunk r = (RawDataChunk)m.raw.rawChunks[y];
            //            int tempintx = loc.startoffset + loc.size + coconutModelShift;
            //            //writes new pointer to loaded meta
            //            BW.BaseStream.Position = r.pointerMetaOffset - 4;
            //            BW.Write(r.size);
            //            BW.Write(tempintx);

            //            //writes raw to map file
            //            map.BW.BaseStream.Position = tempintx;
            //            map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);
            //            //write padding
            //            int tempinty = map.Functions.Padding(r.size, 512);
            //            byte[] tempbytes = new byte[tempinty];
            //            map.BW.Write(tempbytes);
            //            coconutModelShift += r.size + tempinty;
            //        }
            //        metas[x] = m;
            //    }
            //}
            //loc.size += coconutModelShift;
            loc.endoffset = loc.startoffset + loc.size;
            if (tempint == -1)
            {
                if (coconutModelShift > 0) { layout.chunks.Add(loc); };
            }
            else
            {
                layout.chunks[tempint] = loc;
            }
            totalshift += coconutModelShift;
            #endregion

            #region jmad raw data

            int jmadshift = 0;
            tempint = layout.FindByType(RawDataContainerType.Animation);
            if (tempint != -1)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                loc.startoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            }
            else
            {
                loc = new LayOutChunk(0);
                loc.rawType = RawDataContainerType.Animation;
                int tx = layout.FindByType(RawDataContainerType.PRTM);
                if (tx == -1)
                {
                    tx = layout.FindByType(RawDataContainerType.DECR);
                    if (tx == -1)
                    {
                        tx = layout.FindByType(RawDataContainerType.Weather);
                        if (tx == -1)
                        {
                            tx = layout.FindByType(RawDataContainerType.BSP);
                        }
                    }
                }

                loc.startoffset = ((LayOutChunk)layout.chunks[tx]).endoffset;
                loc.size = 0;
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.Animation)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.startoffset + loc.size + jmadshift;

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

                    metas[x] = m;
                }
            }

            loc.size += jmadshift;
            loc.endoffset = loc.startoffset + loc.size;
            if (tempint == -1)
            {
                if (jmadshift > 0)
                {
                    layout.chunks.Add(loc);
                }

                ;
            }
            else
            {
                layout.chunks[tempint] = loc;
            }

            totalshift += jmadshift;

            #endregion

            #region bsp meta
            int bspToImportOffset = 0;
            for (int sbspIndex = 0; sbspIndex < map.BSP.sbsp.Length; sbspIndex++)
            {
                int oldbspmetasize = map.BSP.sbsp[sbspIndex].size;

                loc = (LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.BSPMeta, sbspIndex)];
                loc.startoffset += totalshift;
                loc.endoffset += totalshift;

                if (foundbsptoimport == false)
                {
                    LayOutChunk loc3 = (LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.BSP, sbspIndex)];
                    BinaryWriter BSPMetaBW = new BinaryWriter(loc.MS);

                    //Writes SBSP Raw Pointers
                    for (int w = 0; w < loc3.rawPieces.Count; w++)
                    {
                        RawInfoChunk r = (RawInfoChunk)loc3.rawPieces[w];
                        r.offset += (uint)(modeshift + sndshift);

                        BSPMetaBW.BaseStream.Position = r.offsetOfPointer - map.BSP.sbsp[sbspIndex].offset;
                        BSPMetaBW.Write(r.offset);
                    }
                }

                int bspoffset = loc.startoffset;
                int bspmetasize = loc.size;

                loc2 = (LayOutChunk)layout.chunks[layout.FindByType(RawDataContainerType.MetaData)];

                if (sbspIndex == 0) bspToImportOffset = bspoffset;

                #region Update SBSP info in SCNR
                //writes new pointer and magic and size to scenario meta
                BinaryWriter BWX = new BinaryWriter(loc2.MS);
                int scnrOffset = map.MetaInfo.Offset[3] - map.MapHeader.metaStart - map.MapHeader.indexOffset;
                BWX.BaseStream.Position = scnrOffset + map.BSP.sbsp[sbspIndex].pointerOffset;

                BWX.Write(bspoffset);
                BWX.Write(loc.size);
                BWX.Write(newprimarymagicconstant);

                //writes sbsp meta to map file
                map.BW.BaseStream.Position = bspoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);

                //write padding
                int bspmetashift = bspmetasize - oldbspmetasize;
                totalshift += bspmetashift;

                if (bspmetashift < 0) bspmetashift = 0;
                else
                {
                    map.BW.Write(new byte[bspmetashift]);
                    totalshift += bspmetashift;
                }
                #endregion
            }
            #endregion

            #region stringnames1

            tempint = layout.FindByType(RawDataContainerType.Strings1);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            int tempoldsize = map.MapHeader.scriptReferenceCount * 128;
            int oldpadding = map.Functions.Padding(tempoldsize, 512);
            int tempnewsrsize = strings.Count * 128;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, tempoldsize);
            byte[] tempb = new byte[tempnewsrsize];
            map.BW.BaseStream.Write(tempb, 0, tempnewsrsize);
            int padding = map.Functions.Padding(tempoldsize + tempnewsrsize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Write(tempb, 0, padding);
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = loc.startoffset + tempoldsize + (x * 128);
                char[] tempc = ((String)strings[x]).ToCharArray();
                map.BW.Write(tempc);
            }

            loc.size += -oldpadding + tempnewsrsize + padding;
            loc.endoffset = loc.startoffset + loc.size;
            totalshift += -oldpadding + tempnewsrsize + padding;
            map.BW.BaseStream.Position = 352;
            map.BW.Write(loc.startoffset);
            int newcount = map.MapHeader.scriptReferenceCount + strings.Count;
            map.BW.Write(newcount);
            layout.chunks[tempint] = loc;

            #endregion

            // MessageBox.Show("Test");
            #region stringsindex

            tempint = layout.FindByType(RawDataContainerType.StringsIndex);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            tempoldsize = map.MapHeader.scriptReferenceCount * 4;
            oldpadding = map.Functions.Padding(tempoldsize, 512);
            tempnewsrsize = strings.Count * 4;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, tempoldsize);
            tempb = new byte[tempnewsrsize];
            map.BW.BaseStream.Write(tempb, 0, tempnewsrsize);
            padding = map.Functions.Padding(tempoldsize + tempnewsrsize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Write(tempb, 0, padding);
            int temporary = map.MapHeader.sizeOfScriptReference;
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = loc.startoffset + tempoldsize + (x * 4);
                map.BW.Write(temporary);
                temporary += ((String)strings[x]).Length + 1;
            }

            loc.size += -oldpadding + tempnewsrsize + padding;
            loc.endoffset = loc.startoffset + loc.size;
            totalshift += -oldpadding + tempnewsrsize + padding;

            map.BW.BaseStream.Position = 364;
            map.BW.Write(loc.startoffset);

            layout.chunks[tempint] = loc;

            #endregion

            #region strings2

            tempint = layout.FindByType(RawDataContainerType.Strings2);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            tempoldsize = map.MapHeader.sizeOfScriptReference;
            oldpadding = map.Functions.Padding(tempoldsize, 512);
            tempnewsrsize = temporary - map.MapHeader.sizeOfScriptReference;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, tempoldsize);
            tempb = new byte[tempnewsrsize];
            map.BW.BaseStream.Write(tempb, 0, tempnewsrsize);
            padding = map.Functions.Padding(tempoldsize + tempnewsrsize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Write(tempb, 0, padding);
            temporary = loc.startoffset + map.MapHeader.sizeOfScriptReference;
            byte zero = 0;
            for (int x = 0; x < strings.Count; x++)
            {
                map.BW.BaseStream.Position = temporary;
                char[] h = ((String)strings[x]).ToCharArray();
                map.BW.Write(h);
                map.BW.Write(zero);
                temporary += ((String)strings[x]).Length + 1;
            }

            loc.size += -oldpadding + tempnewsrsize + padding;
            loc.endoffset = loc.startoffset + loc.size;
            totalshift += -oldpadding + tempnewsrsize + padding;
            int bleh = loc.size - padding;
            map.BW.BaseStream.Position = 360;
            map.BW.Write(bleh);
            map.BW.BaseStream.Position = 368;
            map.BW.Write(loc.startoffset);
            layout.chunks[tempint] = loc;

            #endregion

            #region file names

            tempint = layout.FindByType(RawDataContainerType.FileNames);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            tempoldsize = map.MapHeader.fileNamesSize;
            oldpadding = map.Functions.Padding(tempoldsize, 512);

            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(
                loc.MS.ToArray(), 0, tempoldsize - map.FileNames.Length[map.IndexHeader.metaCount - 1] - 1);

            temporary = loc.startoffset + tempoldsize - map.FileNames.Length[map.IndexHeader.metaCount - 1] - 1;
            zero = 0;
            for (int x = 0; x < metas.Count; x++)
            {
                map.BW.BaseStream.Position = temporary;
                char[] h = ((Meta)metas[x]).name.ToCharArray();
                map.BW.Write(h);
                map.BW.Write(zero);
                temporary += ((Meta)metas[x]).name.Length + 1;
            }

            tempnewsrsize = temporary - loc.startoffset - tempoldsize;
            padding = map.Functions.Padding(tempoldsize + tempnewsrsize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Write(tempb, 0, padding);

            loc.size += -oldpadding + tempnewsrsize + padding;
            loc.endoffset = loc.startoffset + loc.size;
            totalshift += -oldpadding + tempnewsrsize + padding;
            map.BW.BaseStream.Position = 704;
            newcount = map.MapHeader.fileCount + metas.Count - 1;
            map.BW.Write(newcount);
            map.BW.Write(loc.startoffset);
            int hhh = temporary - loc.startoffset;
            map.BW.Write(hhh);
            layout.chunks[tempint] = loc;

            #endregion

            #region files index

            tempint = layout.FindByType(RawDataContainerType.FileNamesIndex);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            tempoldsize = map.MapHeader.fileCount * 4;
            oldpadding = map.Functions.Padding(tempoldsize, 512);
            tempnewsrsize = (metas.Count * 4) - 4;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, tempoldsize);
            tempb = new byte[tempnewsrsize];
            map.BW.BaseStream.Write(tempb, 0, tempnewsrsize);
            padding = map.Functions.Padding(tempoldsize + tempnewsrsize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Write(tempb, 0, padding);
            temporary = map.MapHeader.fileNamesSize - map.FileNames.Length[map.IndexHeader.metaCount - 1] - 1;
            for (int x = 0; x < metas.Count; x++)
            {
                map.BW.BaseStream.Position = loc.startoffset + tempoldsize + (x * 4) - 4;
                map.BW.Write(temporary);
                temporary += ((Meta)metas[x]).name.Length + 1;
            }

            loc.size += -oldpadding + tempnewsrsize + padding;
            loc.endoffset = loc.startoffset + loc.size;

            totalshift += -oldpadding + tempnewsrsize + padding;
            map.BW.BaseStream.Position = 716;
            map.BW.Write(loc.startoffset);
            layout.chunks[tempint] = loc;

            tempint = layout.FindByType(RawDataContainerType.UnicodeNamesIndex);
            for (int x = 0; x < 8; x++)
            {
                loc = (LayOutChunk)layout.chunks[tempint + (x * 2)];
                loc.startoffset += totalshift;
                loc.endoffset += totalshift;
                map.BW.BaseStream.Position = loc.startoffset;
                map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
                layout.chunks[tempint + (x * 2)] = loc;
                loc2 = (LayOutChunk)layout.chunks[tempint + (x * 2) + 1];
                loc2.startoffset += totalshift;
                loc2.endoffset += totalshift;
                map.BW.BaseStream.Position = loc2.startoffset;
                map.BW.BaseStream.Write(loc2.MS.ToArray(), 0, loc2.size);
                layout.chunks[tempint + (x * 2) + 1] = loc2;
            }

            tempint = layout.FindByType(RawDataContainerType.MetaData);
            for (int x = 0; x < 9; x++)
            {
                loc = (LayOutChunk)layout.chunks[tempint];
                BinaryWriter BW = new BinaryWriter(loc.MS);
                BW.BaseStream.Position = map.MetaInfo.Offset[0] - map.MapHeader.metaStart - map.MapHeader.indexOffset +
                                         map.Unicode.ut[x].indexPointerOffset;
                map.Unicode.ut[x].indexOffset += totalshift;
                map.Unicode.ut[x].tableOffset += totalshift;
                BW.Write(map.Unicode.ut[x].indexOffset);
                BW.Write(map.Unicode.ut[x].tableOffset);
                layout.chunks[tempint] = loc;
            }

            tempint = layout.FindByType(RawDataContainerType.Crazy);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            loc.endoffset += totalshift;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            map.BW.BaseStream.Position = 344;
            map.BW.Write(loc.startoffset);
            layout.chunks[tempint] = loc;

            #endregion

            int bitmshift = 0;
            tempint = layout.FindByType(RawDataContainerType.Bitmap);
            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            int bitmappad = 0;
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.rawType == RawDataContainerType.Bitmap)
                {
                    BinaryWriter BW = new BinaryWriter(m.MS);
                    for (int y = 0; y < m.raw.rawChunks.Count; y++)
                    {
                        RawDataChunk r = m.raw.rawChunks[y];
                        int tempintx = loc.startoffset + loc.size + bitmshift;

                        // writes new pointer to loaded meta
                        BW.BaseStream.Position = r.pointerMetaOffset;
                        BW.Write(tempintx);

                        // BW.Write(r.size);
                        // writes raw to map file
                        map.BW.BaseStream.Position = tempintx;
                        map.BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);

                        // write padding
                        int tempinty = map.Functions.Padding(r.size, 512);
                        byte[] tempbytes = new byte[tempinty];
                        map.BW.Write(tempbytes);
                        bitmshift += r.size + tempinty;

                        bitmappad = tempinty;
                    }

                    metas[x] = m;
                }
            }

            totalshift += bitmshift;
            loc.size += bitmshift;
            loc.endoffset = loc.startoffset + loc.size;
            layout.chunks[tempint] = loc;

            tempint = layout.FindByType(RawDataContainerType.MetaIndex);

            loc = (LayOutChunk)layout.chunks[tempint];
            loc.startoffset += totalshift;
            loc.endoffset += totalshift;

            tempint = layout.FindByType(RawDataContainerType.MetaData);

            loc2 = (LayOutChunk)layout.chunks[tempint];
            loc2.startoffset += totalshift;
            loc2.endoffset += totalshift;

            map.BW.BaseStream.Position = loc.startoffset;
            map.BW.BaseStream.Write(loc.MS.ToArray(), 0, loc.size);
            map.BW.BaseStream.Position = loc2.startoffset;
            // Changed this line to include Min(MS.Length) as it would pad past end of file. ???
            map.BW.BaseStream.Write(loc2.MS.ToArray(), 0, Math.Min(loc2.size, (int)loc2.MS.Length));
            map.BW.BaseStream.Position = 16;
            map.BW.Write(loc.startoffset);
            newcount = metas.Count + map.IndexHeader.metaCount - 1;
            map.BW.BaseStream.Position = loc.startoffset + 24;
            map.BW.Write(newcount);

            map.BR.BaseStream.Position = map.IndexHeader.tagsOffset + totalshift + 8;
            map.SecondaryMagic = map.BR.ReadInt32() - (loc.startoffset + map.MapHeader.metaStart);

            int newUghOffset = 0;

            int where = map.MetaInfo.Offset[map.IndexHeader.metaCount - 1] + totalshift;

            int howfar = 0;

            // Recreate Meta Table Index
            int metaTableStart = map.IndexHeader.tagsOffset + ((map.IndexHeader.metaCount - 1) * 16);
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];

                int metaOffset = metaTableStart + totalshift + (x * 16);

                char[] metatype = m.type.ToCharArray();
                Array.Reverse(metatype);

                // Create Ident Values?
                int ident = map.MetaInfo.Ident[map.IndexHeader.metaCount - 1] + (x * 65537);
                int offset = where + howfar;

                if (x == metas.Count - 1)
                {
                    newUghOffset = offset;
                    int wherex = map.MetaInfo.Offset[0] + totalshift + 756;
                    map.BW.BaseStream.Position = wherex;
                    map.BW.Write(ident);
                }

                // I believe this aligns the folowing tags on a boundry of 16 as is
                // very important for all HAVOK tags & data
                #region alignment fix

                if (m.type == "phmo" | m.type == "coll" | m.type == "spas" | m.type == "vehi")
                {
                    int tempoffset = offset;
                    do
                    {
                        // convert our 'tempoffset', which is our offset increased until the padding lines up, to a hex
                        string tempss = tempoffset.ToString("X");
                        char[] tempc = tempss.ToCharArray();

                        // We use the last hex digit to see if our padding is right (0-15) for 16 byte alignment?
                        if (m.padding == tempc[tempc.Length - 1])
                        {
                            // If we had to add bytes to get our offset to line up, figure out how many and pad
                            // the end of the last tag
                            int diff = tempoffset - offset;
                            tempb = new byte[diff];
                            map.BW.BaseStream.Position = offset;
                            map.BW.Write(tempb);
                            int tempsize;

                            if (x == 0)
                            {
                                // The first tag has no tag before it, so we pad the MetaInfo instead
                                tempsize = map.MetaInfo.Size[map.IndexHeader.metaCount - 2];

                                // int temploc = map.IndexHeader.tagsOffset + ((map.IndexHeader.metaCount - 2) * 16) + totalshift + 12;
                            }
                            else
                            {
                                // Retrieve the last tags size and add the difference onto it
                                tempsize = ((Meta)metas[x - 1]).size;

                                // int temploc = metaTableStart + totalshift + ((x - 1) * 16) + 12;
                            }

                            // Update the size of previous tag
                            int temploc = metaTableStart + totalshift + ((x - 1) * 16) + 12;
                            tempsize += diff;
                            map.BW.BaseStream.Position = temploc;
                            map.BW.Write(tempsize);

                            offset = tempoffset;
                            howfar += diff;
                            break;
                        }

                        tempoffset++;
                    }
                    while (0 != 1);
                }

                #endregion

                int offsetwithmagic = offset + map.SecondaryMagic;

                // each record 16 bytes
                map.BW.BaseStream.Position = metaOffset;
                map.BW.Write(metatype); // Offset  0
                map.BW.Write(ident); // Offset  4
                map.BW.Write(offsetwithmagic); // Offset  8
                map.BW.Write(m.size); // Offset 12
                howfar += m.size;

                map.BW.BaseStream.Position = offset;
                map.BW.BaseStream.Write(m.MS.ToArray(), 0, m.size);

                for (int xx = 0; xx < m.items.Count; xx++)
                {
                    Meta.Item i = m.items[xx];
                    switch (i.type)
                    {
                        case Meta.ItemType.Ident:
                            Meta.Ident id = (Meta.Ident)i;
                            if (id.pointstoTagIndex == -1 | x == metas.Count - 1)
                            {
                                for (int e = 0; e < metas.Count; e++)
                                {
                                    Meta tempm = (Meta)metas[e];
                                    if (tempm.name == id.pointstotagname && tempm.type == id.pointstotagtype)
                                    {
                                        id.ident = map.MetaInfo.Ident[map.IndexHeader.metaCount - 1] + (e * 65537);
                                        break;
                                    }

                                    if (e == metas.Count - 1)
                                    {
                                        if (id.pointstotagname != "Null")
                                        {
                                            int sss = Array.IndexOf(map.MetaInfo.TagType, id.pointstotagtype);
                                            id.ident = sss != -1 ? map.MetaInfo.Ident[sss] : -1;

                                            // id.ident=-1;
                                        }
                                        else
                                        {
                                            id.ident = -1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                id.ident = map.MetaInfo.Ident[id.pointstoTagIndex];
                            }

                            map.BW.BaseStream.Position = offset + id.offset;
                            map.BW.Write(id.ident);
                            break;

                        case Meta.ItemType.Reflexive:
                            Meta.Reflexive reflex = (Meta.Reflexive)i;
                            int newreflex = reflex.translation + offset + map.SecondaryMagic;
                            /*
                            // Handle referenced reflexives                            
                            if (reflex.pointstoTagIndex != m.TagIndex)
                            {
                                newreflex = totalshift + reflex.translation + map.MetaInfo.Offset[reflex.pointstoTagIndex] + map.SecondaryMagic;
                            }
                            */
                            map.BW.BaseStream.Position = offset + reflex.offset;
                            map.BW.Write(reflex.chunkcount);
                            map.BW.Write(newreflex);
                            break;

                        case Meta.ItemType.String:
                            Meta.String s = (Meta.String)i;
                            short stringnum = 0;
                            byte stringlength = 0;
                            for (int e = 0; e < map.MapHeader.scriptReferenceCount; e++)
                            {
                                if (s.name == map.Strings.Name[e])
                                {
                                    stringnum = (short)e;
                                    stringlength = (byte)map.Strings.Length[e];
                                }
                            }

                            for (int e = 0; e < strings.Count; e++)
                            {
                                if (((string)strings[e]) == s.name)
                                {
                                    stringnum = (short)(map.MapHeader.scriptReferenceCount + e);
                                    stringlength = (byte)s.name.Length;
                                }
                            }

                            map.BW.BaseStream.Position = offset + s.offset;
                            map.BW.Write(stringnum);
                            map.BW.Write(zero);
                            map.BW.Write(stringlength);
                            break;
                    }
                }
            }

            // totalshift+=howfar;
            int tempfilesize = map.MapHeader.fileSize + totalshift + howfar -
                               map.MetaInfo.Size[map.IndexHeader.metaCount - 1]; // (int)map.BW.BaseStream.Length;

            // map.MapHeader.fileSize+totalshift+howfar-map.MetaInfo.Size [map.IndexHeader.metaCount-1];
            padding = map.Functions.Padding(tempfilesize, 512);
            tempb = new byte[padding];
            map.BW.BaseStream.Position = tempfilesize;
            map.BW.Write(tempb);
            tempfilesize += padding;

            int olddifference = map.MapHeader.fileSize - map.MapHeader.indexOffset;
            int difference = tempfilesize - (map.MapHeader.indexOffset + totalshift);
            int metasize = map.MapHeader.metaSize + howfar - map.MetaInfo.Size[map.IndexHeader.metaCount - 1] + padding;

            // int combined = map.MapHeader.combinedSize + difference;
            int combined = map.MapHeader.combinedSize - olddifference + difference;

            // (int)bspmetasize + difference;
            // MessageBox.Show("test");
            // combined
            // metasize -= bspmetashift;
            // map.MapHeader.combinedSize + howfar - map.MetaInfo.Size[map.IndexHeader.metaCount - 1] + padding;
            // (int)bspmetasize + difference - bspmetashift;
            map.BW.BaseStream.Position = 8;
            map.BW.Write(tempfilesize);
            map.BW.BaseStream.Position = 24;
            map.BW.Write(metasize);
            map.BW.Write(combined);

            map.BW.BaseStream.SetLength(tempfilesize);

            #region write new bsp meta
            if (foundbsptoimport)
            {
                int tempbspmagic = map.BSP.sbsp[0].magic + map.BSP.sbsp[0].offset - bspToImportOffset;

                for (int xx = 0; xx < bspToImport.items.Count; xx++)
                {
                    Meta.Item i = (Meta.Item)bspToImport.items[xx];
                    switch (i.type)
                    {
                        case Meta.ItemType.Ident:
                            Meta.Ident id = (Meta.Ident)i;
                            if (id.pointstoTagIndex == -1)
                            {
                                for (int e = 0; e < metas.Count; e++)
                                {
                                    Meta tempm = (Meta)metas[e];
                                    if (bspToImport.name == id.pointstotagname && bspToImport.type == id.pointstotagtype)
                                    {
                                        id.ident = map.BSP.sbsp[0].ident;
                                        break;
                                    }
                                    else if (tempm.name == id.pointstotagname && tempm.type == id.pointstotagtype)
                                    {
                                        id.ident = map.MetaInfo.Ident[map.IndexHeader.metaCount - 1] + (e * 65537); ;
                                        break;
                                    }
                                    if (e == metas.Count - 1)
                                    {
                                        int sss = Array.IndexOf(map.MetaInfo.TagType, id.intagtype);
                                        id.ident = sss != -1 ? map.MetaInfo.Ident[sss] : -1;

                                    }
                                }
                            }
                            else
                            {
                                id.ident = map.MetaInfo.Ident[id.pointstoTagIndex];
                            }
                            map.BW.BaseStream.Position = bspToImportOffset + id.offset;
                            map.BW.Write(id.ident);
                            break;
                        case Meta.ItemType.String:
                            Meta.String s = (Meta.String)i;
                            short stringnum = 0;
                            byte stringlength = 0;
                            for (int e = 0; e < map.MapHeader.scriptReferenceCount; e++)
                            {
                                if (s.name == map.Strings.Name[e])
                                {
                                    stringnum = (short)e;
                                    stringlength = (byte)map.Strings.Length[e];
                                }
                            }
                            for (int e = 0; e < strings.Count; e++)
                            {
                                if (((string)strings[e]) == s.name)
                                {

                                    stringnum = (short)(map.MapHeader.scriptReferenceCount + e);
                                    stringlength = (byte)s.name.Length;
                                }
                            }

                            map.BW.BaseStream.Position = bspToImportOffset + s.offset;
                            map.BW.Write(stringnum);
                            map.BW.Write(new byte());
                            map.BW.Write(stringlength);
                            break;

                        case Meta.ItemType.Reflexive:
                            Meta.Reflexive rr = (Meta.Reflexive)i;
                            int newreflex = bspToImportOffset + rr.translation + tempbspmagic;
                            map.BW.BaseStream.Position = bspToImportOffset + rr.offset + 4;
                            // map.BW.Write(newreflex);

                            break;
                    }

                }


            }
            #endregion

            #region Fix Raw Pointers

            uint startOfModelTable = uint.MaxValue;
            for (int x = 0; x < layout.chunks.Count; x++)
            {
                loc = (LayOutChunk)layout.chunks[x];
                for (int xx = 0; xx < loc.rawPieces.Count; xx++)
                {
                    RawInfoChunk r = (RawInfoChunk)loc.rawPieces[xx];
                    switch (r.rawType)
                    {
                        case RawDataType.mode1:
                        case RawDataType.mode2:
                            if (r.offset < startOfModelTable) startOfModelTable = r.offset; break;
                    }
                }
            }

            map.BR.BaseStream.Position = newUghOffset + 84;
            int newUghRawInfoReflexiveStart = map.BR.ReadInt32() - map.SecondaryMagic;
            int ughShift = newUghRawInfoReflexiveStart - oldUghRawInfoReflexiveStart - totalshift;//howfar - ((Meta)metas[metas.Count - 1]).size;

            for (int x = 0; x < layout.chunks.Count; x++)
            {
                loc = (LayOutChunk)layout.chunks[x];
                for (int xx = 0; xx < loc.rawPieces.Count; xx++)
                {
                    RawInfoChunk r = (RawInfoChunk)loc.rawPieces[xx];
                    r.offsetOfPointer += totalshift;
                    map.BW.BaseStream.Position = r.offsetOfPointer;
                    switch (r.rawType)
                    {
                        case RawDataType.mode1:
                            r.offset += (uint)sndshift;
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.mode2:
                            r.offset += (uint)sndshift;
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.weat:
                            r.offset += (uint)(sndshift + modeshift + bspshift);
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.DECR:
                            r.offset += (uint)(sndshift + modeshift + bspshift + weathershift);
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.PRTM:
                            r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift);
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.snd2:
                            r.offsetOfPointer += ughShift;
                            map.BW.BaseStream.Position = r.offsetOfPointer;
                            if (r.offset >= startOfModelTable) r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift + prtmshift);
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.jmad:
                            r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift + prtmshift + coconutModelShift);
                            map.BW.Write(r.offset);
                            break;
                        case RawDataType.bitm:
                            r.offset += (uint)(totalshift - bitmshift);
                            map.BW.Write(r.offset);
                            break;
                    }
                }
            }

            layout.chunks[tempint] = loc;

            // 	layout.SortChunksByOffset();
            map.CloseMap();

            for (int y = 0; y < filestofix.Length; y++)
            {
                Map mapid = Map.LoadFromFile(filestofix[y]);
                MapAnalyzer analyze = new MapAnalyzer();
                MapLayout lo = analyze.ScanMapForLayOut(mapid, true);

                mapid.OpenMap(MapTypes.Internal);

                for (int x = 0; x < lo.chunks.Count; x++)
                {
                    loc = (LayOutChunk)lo.chunks[x];
                    for (int xx = 0; xx < loc.rawPieces.Count; xx++)
                    {
                        RawInfoChunk r = (RawInfoChunk)loc.rawPieces[xx];
                        if (r.location != map.MapHeader.mapType)
                        {
                            continue;
                        }

                        mapid.BW.BaseStream.Position = r.offsetOfPointer;
                        switch (r.rawType)
                        {
                            case RawDataType.bitm:
                                r.offset += (uint)(totalshift - bitmshift);
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            case RawDataType.DECR:
                                r.offset += (uint)(sndshift + modeshift + bspshift + weathershift);
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            //case RawDataType.snd2:
                            //    r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift + prtmshift);
                            //    r.offset |= 0X40000000;
                            //    Maps.map[mapid].BW.Write(r.offset);
                            //    break;
                            case RawDataType.jmad:
                                r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift + prtmshift + coconutModelShift);
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            case RawDataType.mode1:
                                r.offset += (uint)sndshift;
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            case RawDataType.mode2:
                                r.offset += (uint)sndshift;
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            case RawDataType.PRTM:
                                r.offset += (uint)(sndshift + modeshift + bspshift + weathershift + decrshift);
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                            case RawDataType.weat:
                                r.offset += (uint)(sndshift + modeshift + bspshift);
                                r.offset |= 0X40000000;
                                mapid.BW.Write(r.offset);
                                break;
                        }
                    }
                }

            #endregion
                mapid.CloseMap();
                mapid.Sign();
            }
        }

            #endregion
    }
}