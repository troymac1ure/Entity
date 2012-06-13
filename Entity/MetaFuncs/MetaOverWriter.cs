// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaOverWriter.cs" company="">
//   
// </copyright>
// <summary>
//   The meta over writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaFuncs
{
    using System.Collections;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;

    /// <summary>
    /// The meta over writer.
    /// </summary>
    /// <remarks></remarks>
    public sealed class MetaOverWriter
    {
        #region Constants and Fields

        /// <summary>
        /// The offset of shift.
        /// </summary>
        public static int OffsetOfShift;

        /// <summary>
        /// The size of shift.
        /// </summary>
        public static int SizeOfShift;

        /// <summary>
        /// The tag number.
        /// </summary>
        public static int TagIndex;

        #endregion

        #region Public Methods

        /// <summary>
        /// The fix reflexives.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public static void FixReflexives(ArrayList metas, Map map)
        {
            int where = map.MapHeader.indexOffset + map.MapHeader.metaStart;

            int howfar = 0;
            int[] oldoffset = (int[])map.MetaInfo.Offset.Clone();
            int padding = 0;
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                int fuck = map.IndexHeader.tagsOffset + (x * 16);
                int offset = oldoffset[x] + padding;
                if (x > map.ChunkTools.TagIndex)
                {
                    offset += map.ChunkTools.SizeOfShift;
                }

                int offsetwithmagic = offset + map.SecondaryMagic;
                int size = m.size;
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
                            byte[] tempb = new byte[diff];
                            map.BW.BaseStream.Position = offset;
                            map.BW.Write(tempb);

                            int tempsize = ((Meta)metas[x - 1]).size;
                            tempsize += diff;
                            int temploc = map.IndexHeader.tagsOffset + ((x - 1) * 16) + 12;
                            map.BW.BaseStream.Position = temploc;
                            map.BW.Write(tempsize);

                            offset = tempoffset;
                            howfar += diff;
                            padding += diff;
                            break;
                        }

                        tempoffset++;
                    }
                    while (fuck != -54454);
                }

                if (m.type != "sbsp" && m.type != "ltmp")
                {
                }
                else
                {
                    continue;
                }

                howfar += size;

                map.BW.BaseStream.Position = offset;
                map.BW.BaseStream.Write(m.MS.ToArray(), 0, m.size);

                map.MetaInfo.Offset[x] = offset;

                // if (x == 0x334) { MessageBox.Show("test"); }
                // if (x == 0) {
                // map.BW.BaseStream.Position = fuck + 8;
                // map.BW.Write(offsetwithmagic);
                // map.BW.Write(size);
                // continue; }

                // if (oldoffset[x-1] == oldoffset[x]) {
                // map.MetaInfo.Offset[x] = map.MetaInfo.Offset[x - 1];
                // }
                offsetwithmagic = map.MetaInfo.Offset[x] + map.SecondaryMagic;
                map.BW.BaseStream.Position = fuck + 8;
                map.BW.Write(offsetwithmagic);
                map.BW.Write(size);
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];
                if (m.type == "sbsp" || m.type == "ltmp")
                {
                    continue;
                }

                int offset = map.MetaInfo.Offset[x];
                for (int xx = 0; xx < m.items.Count; xx++)
                {
                    Meta.Item i = m.items[xx];
                    switch (i.type)
                    {
                        case Meta.ItemType.Reflexive:
                            Meta.Reflexive reflex = (Meta.Reflexive)i;
                            int newreflex = reflex.translation + offset + map.SecondaryMagic;
                            if (reflex.intag != m.TagIndex)
                            {
                                continue;
                            }

                            if (reflex.pointstoTagIndex != m.TagIndex)
                            {
                                newreflex = map.MetaInfo.Offset[reflex.pointstoTagIndex] + reflex.translation +
                                            map.SecondaryMagic;

                                // if (reflex.pointstoTagIndex > ChunkAdder.TagIndex) { newreflex += ChunkAdder.SizeOfShift; }
                            }

                            map.BW.BaseStream.Position = offset + reflex.offset;
                            map.BW.Write(reflex.chunkcount);
                            map.BW.Write(newreflex);
                            break;
                    }
                }
            }

            int prevsize = map.MapHeader.fileSize - (map.MapHeader.indexOffset + map.MapHeader.metaStart);
            int paddingx = map.Functions.Padding(howfar, 512);
            byte[] tempbb = new byte[paddingx];
            map.BW.BaseStream.Position = map.MapHeader.indexOffset + map.MapHeader.metaStart + howfar;
            map.BW.Write(tempbb);
            howfar += paddingx;

            int sizediff = howfar - prevsize;
            int newfilesize = map.MapHeader.fileSize + sizediff;
            map.BW.BaseStream.Position = 8;
            map.BW.Write(newfilesize);
            int metasize = map.MapHeader.metaSize + sizediff;
            int combined = map.MapHeader.combinedSize + sizediff;
            map.BW.BaseStream.Position = 24;
            map.BW.Write(metasize);
            map.BW.Write(combined);
            map.FS.SetLength(newfilesize);
        }

        /// <summary>
        /// The over write.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="newMeta">The new meta.</param>
        /// <remarks></remarks>
        public static void OverWrite(Map map, int tagIndex, ref Meta newMeta)
        {
            TagIndex = tagIndex;
            if (map.MetaInfo.TagType[tagIndex] == "sbsp")
            {
                MessageBox.Show("Can't OverWrite The Bsp");
                return;
            }

            newMeta.RelinkReferences();
            ArrayList metas = new ArrayList(0);
            map.OpenMap(MapTypes.Internal);
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if (tagIndex == x)
                {
                    newMeta.type = map.MetaInfo.TagType[x];
                    newMeta.name = map.FileNames.Name[x];
                    SizeOfShift = newMeta.size - map.MetaInfo.Size[x];
                    metas.Add(newMeta);
                    continue;
                }

                Meta m = new Meta(map);
                m.ReadMetaFromMap(x, true);
                IFPIO ifpx = IFPHashMap.GetIfp(m.type, map.HaloVersion);

                m.headersize = ifpx.headerSize;
                m.scanner.ScanWithIFP(ref ifpx);

                // metaScanner.ScanManually(ref m, ref map);
                metas.Add(m);
            }

            FixReflexives(metas, map);
            map.CloseMap();
        }

        #endregion
    }
}