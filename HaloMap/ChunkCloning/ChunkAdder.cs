// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChunkAdder.cs" company="">
//   
// </copyright>
// <summary>
//   The chunk adder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.ChunkCloning
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;

    /// <summary>
    /// The chunk adder.
    /// </summary>
    /// <remarks></remarks>
    public class ChunkAdder
    {
        #region Constants and Fields

        /// <summary>
        /// 
        /// </summary>
        private Map map;

        /// <summary>
        /// The chunk clip board.
        /// </summary>
        public MetaSplitter.SplitReflexive ChunkClipBoard;

        /// <summary>
        /// The offset of shift.
        /// </summary>
        public int OffsetOfShift;

        /// <summary>
        /// The size of shift.
        /// </summary>
        public int SizeOfShift;

        /// <summary>
        /// The tag number.
        /// </summary>
        public int TagIndex;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkAdder"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public ChunkAdder(Map map)
        {
            this.map = map;
        }

        #region Public Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="metasplit">The metasplit.</param>
        /// <remarks></remarks>
        public void Add(int tagIndex, MetaSplitter metasplit)
        {
            // TagIndex - Global Variable
            this.TagIndex = tagIndex;

            ArrayList metas = new ArrayList(0);

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                // sender.setProgressBar(x / map.IndexHeader.metaCount);
                Meta m = new Meta(map);
                m.ReadMetaFromMap(x, true);

                // Read meta layout of TAG from .ENT file
                IFPIO ifpx = IFPHashMap.GetIfp(m.type, map.HaloVersion);

                m.headersize = ifpx.headerSize;

                if (m.type == "sbsp")
                {
                }
                else
                {
                    // anything but "sbsp"
                    m.scanner.ScanWithIFP(ref ifpx);

                    // metaScanner.ScanManually(ref m, ref map);
                }

                metas.Add(m);
            }

            // sender.setProgressBar(0);
            Meta targetTag = (Meta)metas[tagIndex];

            Meta tempm = MetaBuilder.BuildMeta(metasplit, map); // (Meta) metas[TagIndex];
            metas[tagIndex] = tempm;

            // ((Meta)metas[TagIndex]).RelinkReferences(map);
            SizeOfShift = tempm.size - targetTag.size;

            // Map IS already open? I guess it's a safety check.
            map.OpenMap(MapTypes.Internal);
            FixReflexives(metas);

            map.CloseMap();
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void Add(Meta meta)
        {
            TagIndex = meta.TagIndex;

            ArrayList metas = new ArrayList(0);
            for (int x = 0; x < meta.Map.IndexHeader.metaCount; x++)
            {
                if (TagIndex == x)
                {
                    metas.Add(meta);
                    continue;
                }

                Meta m = new Meta(meta.Map);
                m.ReadMetaFromMap(x, true);

                IFPIO ifpx = IFPHashMap.GetIfp(m.type, meta.Map.HaloVersion);
                m.headersize = ifpx.headerSize;

                if (m.type == "sbsp")
                {
                }
                else
                {
                    m.scanner.ScanWithIFP(ref ifpx);
                }

                // metaScanner.ScanManually(ref m, ref meta.Map);
                metas.Add(m);
            }

            int diff = meta.size - meta.Map.MetaInfo.Size[TagIndex];
            SizeOfShift = diff;
            meta.Map.OpenMap(MapTypes.Internal);
            FixReflexives(metas);
            meta.Map.CloseMap();
        }

        /// <summary>
        /// The fix reflexives.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <remarks></remarks>
        public void FixReflexives(ArrayList metas)
        {
            int where = map.MapHeader.indexOffset + map.MapHeader.metaStart;

            int howfar = 0;
            int[] oldoffset = (int[])map.MetaInfo.Offset.Clone();
            int padding = 0;

            // Contains the offset for our cloned tag
            int myTagOffset = oldoffset[TagIndex];
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];

                int tagOffset = map.IndexHeader.tagsOffset + (x * 16);
                int offset = oldoffset[x];

                // Once we get to our updated chunk, all chunks afterwards need the SizeOfShift added on to the offset
                // WRONG! Chunk offsets are NOT in order, so we need to update any Chunks AFTER the offset
                // This is why the chunk cloner was messed up for so long!
                if (offset > myTagOffset)
                {
                    offset += SizeOfShift + padding;
                }

                int offsetwithmagic = offset + map.SecondaryMagic;
                int size = m.size;

                // I believe this aligns the folowing tags on a boundry of 8, but needs to be 16 as is
                // very important for all HAVOK tags & data
                if (m.type == "phmo" | m.type == "coll" | m.type == "spas")
                {
                    int tempoffset = offset;
                    do
                    {
                        // tempss -> Hex(tempoffset)
                        string tempss = tempoffset.ToString("X");
                        char[] tempc = tempss.ToCharArray();
                        int xxx = tempc.Length;

                        // Check if the last number/letter of the Hex value equals m.padding?
                        // If so, our desired padding is correct
                        if (m.padding == tempc[xxx - 1])
                        {
                            // Determine the difference between
                            int diff = tempoffset - offset;
                            byte[] tempb = new byte[diff];
                            map.BW.BaseStream.Position = offset;
                            map.BW.Write(tempb);

                            // tempsize = size of the record before
                            int tempsize = ((Meta)metas[x - 1]).size;

                            // Change the size of the record before to reflect the difference?
                            tempsize += diff;

                            // each TAG pointer is 16 bytes long. INT at +12 of previous chunk is TAG size (?)
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
                    while (tagOffset != -54454);
                }

                if (m.type == "sbsp" || m.type == "ltmp")
                {
                    continue;
                }

                howfar += size;

                map.MetaInfo.Offset[x] = offset;

                offsetwithmagic = map.MetaInfo.Offset[x] + map.SecondaryMagic;
                map.BW.BaseStream.Position = tagOffset + 8;
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

                m.offset = map.MetaInfo.Offset[x];
                m.WriteReferences();
                map.BW.BaseStream.Position = map.MetaInfo.Offset[x];
                map.BW.BaseStream.Write(m.MS.ToArray(), 0, m.size);
            }

            int prevsize = map.MapHeader.fileSize - (map.MapHeader.indexOffset + map.MapHeader.metaStart);
            int paddingx = map.Functions.Padding(howfar, 512);
            byte[] tempbb = new byte[paddingx];
            map.BW.BaseStream.Position = map.MapHeader.indexOffset + map.MapHeader.metaStart + howfar;
            map.BW.Write(tempbb);
            howfar += paddingx;

            int sizediff = howfar - prevsize;
            int newfilesize = map.MapHeader.fileSize + sizediff;
            map.BW.BaseStream.Position = MapDataInfo.FileSizeOffset;
            map.BW.Write(newfilesize);
            int metasize = map.MapHeader.metaSize + sizediff;
            int combined = map.MapHeader.combinedSize + sizediff;
            map.BW.BaseStream.Position = MapDataInfo.MetaSizeOffset;
            map.BW.Write(metasize);
            map.BW.Write(combined);
            map.FS.SetLength(newfilesize); // Add padding to 512k sizes
        }

        #endregion

        /// <summary>
        /// The chunk container.
        /// </summary>
        /// <remarks></remarks>
        public class ChunkContainer
        {
            #region Constants and Fields

            /// <summary>
            /// The chunks.
            /// </summary>
            public List<ChunkContainer> Chunks = new List<ChunkContainer>();

            /// <summary>
            /// The ms.
            /// </summary>
            public MemoryStream MS = new MemoryStream();

            /// <summary>
            /// The references.
            /// </summary>
            public List<Meta.Item> References;

            /// <summary>
            /// The mapoffset.
            /// </summary>
            public int mapoffset;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The parent.
            /// </summary>
            public string parent;

            /// <summary>
            /// The size.
            /// </summary>
            public int size;

            /// <summary>
            /// The translation.
            /// </summary>
            public int translation;

            #endregion
        }

        /// <summary>
        /// The map data info.
        /// </summary>
        /// <remarks></remarks>
        public static class MapDataInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The bsp string id offset.
            /// </summary>
            public const int BSPStringIDOffset = 0x0154; // 340

            /// <summary>
            /// The combined size offset.
            /// </summary>
            public const int CombinedSizeOffset = 0x001B; // 28

            /// <summary>
            /// The file size offset.
            /// </summary>
            public const int FileSizeOffset = 0x0008; // 8

            /// <summary>
            /// The meta size offset.
            /// </summary>
            public const int MetaSizeOffset = 0x0018; // 24

            #endregion
        }
    }
}