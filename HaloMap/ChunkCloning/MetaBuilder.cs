// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The meta builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.ChunkCloning
{
    using System.Collections.Generic;
    using System.IO;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    /// <summary>
    /// The meta builder.
    /// </summary>
    /// <remarks></remarks>
    public sealed class MetaBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The meta stream.
        /// </summary>
        private static MemoryStream MetaStream;

        /// <summary>
        /// The metasize.
        /// </summary>
        private static int metasize;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        private static int TagIndex;

        #endregion

        #region Public Methods

        /// <summary>
        /// The build meta.
        /// </summary>
        /// <param name="metasplit">The metasplit.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Meta BuildMeta(MetaSplitter metasplit, Map map)
        {
            metasize = 0;
            MetaStream = new MemoryStream(metasplit.Header.chunksize);
            BinaryWriter BW = new BinaryWriter(MetaStream);

            // BW.BaseStream.Position = 0;
            // BW.Write(metasplit.Header.MS.ToArray(), 0, metasplit.Header.chunksize);
            metasize += metasplit.Header.chunksize;

            TagIndex = metasplit.TagIndex;

            Meta m = new Meta(map);

            List<Meta.Item> NewItems = new List<Meta.Item>();

            // Major error here! Size is not calculated right!
            RecursivelyAddPiecesToMeta(metasplit.Header, ref NewItems, ref BW);

            m.items = NewItems;
            if (MetaStream.Length % 4 != 0)
            {
                metasize += (int)MetaStream.Length % 4;
                MetaStream.SetLength(MetaStream.Length + MetaStream.Length % 4);
            }

            m.MS = MetaStream;
            m.size = metasize;
            m.type = metasplit.type;
            m.name = metasplit.name;
            m.rawType = metasplit.rawtype;
            m.raw = metasplit.raw;
            m.magic = metasplit.magic;
            m.offset = metasplit.offset;

            m.TagIndex = TagIndex;
            m.RelinkReferences();
            m.WriteReferences();

            // m.items.Clear();
            // MetaScanner Ms = new MetaScanner();
            if (m.rawType != RawDataContainerType.Empty)
            {
                map.OpenMap(MapTypes.Internal);
                m.raw = map.Functions.ForMeta.ReadRaw(m.TagIndex, false);
                map.CloseMap();
            }

            // map.OpenMap(MapTypes.Internal);
            // IFPIO ifp=IFP.IFPHashMap.GetIfp(m.type);
            // m.parsed = true;
            // Ms.ScanWithIFP(ref ifp, ref m, map) ;
            // map.CloseMap();
            return m;
        }

        /// <summary>
        /// The recursively add pieces to meta.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="BW">The bw.</param>
        /// <remarks></remarks>
        public static void RecursivelyAddPiecesToMeta(
            MetaSplitter.SplitReflexive reflex, ref List<Meta.Item> destination, ref BinaryWriter BW)
        {
            for (int x = 0; x < reflex.Chunks.Count; x++)
            {
                BW.BaseStream.Position = reflex.translation + (reflex.chunksize * x);
                BW.Write(reflex.Chunks[x].MS.ToArray(), 0, reflex.chunksize);
                for (int y = 0; y < reflex.Chunks[x].ChunkResources.Count; y++)
                {
                    Meta.Item i = reflex.Chunks[x].ChunkResources[y];

                    switch (i.type)
                    {
                        case Meta.ItemType.Reflexive:
                            MetaSplitter.SplitReflexive treflex = (MetaSplitter.SplitReflexive)i;
                            treflex.offset += reflex.translation + (reflex.chunksize * x);

                            treflex.translation = metasize;
                            treflex.chunkcount = treflex.Chunks.Count;

                            metasize += treflex.chunksize * treflex.chunkcount;
                            treflex.type = Meta.ItemType.Reflexive;
                            treflex.intag = TagIndex;
                            treflex.pointstoTagIndex = TagIndex;
                            destination.Add(treflex);
                            RecursivelyAddPiecesToMeta(treflex, ref destination, ref BW);
                            break;

                        case Meta.ItemType.Ident:
                            MetaSplitter.SplitIdent id = new MetaSplitter.SplitIdent((MetaSplitter.SplitIdent)i);
                            id.offset += reflex.translation + (reflex.chunksize * x);
                            id.intag = TagIndex;
                            destination.Add(id);

                            break;
                        case Meta.ItemType.String:
                            MetaSplitter.SplitString ss = (MetaSplitter.SplitString)i;
                            ss.offset += reflex.translation + (reflex.chunksize * x);
                            ss.intag = TagIndex;
                            destination.Add(ss);
                            break;
                    }
                }
            }
        }

        #endregion
    }
}