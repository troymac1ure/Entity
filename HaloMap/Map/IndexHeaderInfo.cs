// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexHeaderInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The index header info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;

    /// <summary>
    /// The index header info.
    /// </summary>
    /// <remarks></remarks>
    public class IndexHeaderInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The indices object count.
        /// </summary>
        public int IndicesObjectCount;

        /// <summary>
        /// The model indices offset.
        /// </summary>
        public int ModelIndicesOffset;

        /// <summary>
        /// The model raw data offset.
        /// </summary>
        public int ModelRawDataOffset;

        /// <summary>
        /// The model raw data size.
        /// </summary>
        public int ModelRawDataSize;

        /// <summary>
        /// The vertice object count.
        /// </summary>
        public int VerticeObjectCount;

        /// <summary>
        /// The constant.
        /// </summary>
        public int constant;

        /// <summary>
        /// ID number of the globals tag [MATG]
        /// </summary>
        public int matgID;

        /// <summary>
        /// The meta count.
        /// </summary>
        public int metaCount;

        /// <summary>
        /// ID Number of the Scenario tag [SCNR]
        /// </summary>
        public int scnrID;

        /// <summary>
        /// The tag type count.
        /// </summary>
        public int tagTypeCount;

        /// <summary>
        /// The tags offset.
        /// </summary>
        public int tagsOffset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexHeaderInfo"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public IndexHeaderInfo(ref BinaryReader BR, Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                    LoadHalo2IndexHeaderInfo(ref BR, map);
                    break;
                case HaloVersionEnum.Halo2Vista:
                    LoadHalo2IndexHeaderInfo(ref BR, map);
                    break;
                case HaloVersionEnum.HaloCE:
                    LoadHaloCEIndexHeaderInfo(ref BR, map);
                    break;
                case HaloVersionEnum.Halo1:
                    LoadHalo1IndexHeaderInfo(ref BR, map);
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load halo 1 index header info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHalo1IndexHeaderInfo(ref BinaryReader BR, Map map)
        {
            BR.BaseStream.Position = map.MapHeader.indexOffset;
            constant = BR.ReadInt32();
            map.PrimaryMagic = constant - (map.MapHeader.indexOffset + 36);

            BR.ReadInt32();
            BR.ReadInt32();
            metaCount = BR.ReadInt32();
            VerticeObjectCount = BR.ReadInt32();
            ModelRawDataOffset = BR.ReadInt32() - map.PrimaryMagic;
            IndicesObjectCount = BR.ReadInt32();
            ModelIndicesOffset = BR.ReadInt32() - map.PrimaryMagic;
            tagsOffset = map.MapHeader.indexOffset + 36;
        }

        /// <summary>
        /// The load halo 2 index header info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHalo2IndexHeaderInfo(ref BinaryReader BR, Map map)
        {
            BR.BaseStream.Position = map.MapHeader.indexOffset;
            constant = BR.ReadInt32();
            map.PrimaryMagic = constant - (map.MapHeader.indexOffset + 32);
            tagTypeCount = BR.ReadInt32();
            tagsOffset = BR.ReadInt32() - map.PrimaryMagic;
            scnrID = BR.ReadInt32();
            matgID = BR.ReadInt32();

            BR.BaseStream.Position = map.MapHeader.indexOffset + 24;
            metaCount = BR.ReadInt32();
            
            /// ** This causes failure when setting a different MATG tag as active as the tag is now located **
            /// ** at the end of the meta, not the start
            //  BR.BaseStream.Position = tagsOffset + 8;

            /// Either need to search through all index listings for lowest offset or ???
            /// However, this is all useless if H2 uses the below method; in which case MATG is unable to 
            /// be copied & replaced

            int metaNum = 0;
            int metaOfs = int.MaxValue;
            for (int i = 0; i < metaCount; i++)
            {
                BR.BaseStream.Position = tagsOffset + i * 16 + 8;
                int ofs = BR.ReadInt32();
                if (ofs < metaOfs)
                {
                    metaOfs = ofs;
                    metaNum = i;
                }
            }

            map.SecondaryMagic = metaOfs - (map.MapHeader.indexOffset + map.MapHeader.metaStart);
            
            // Old method
            // BR.BaseStream.Position = tagsOffset + 8;
            // map.SecondaryMagic = BR.ReadInt32() - (map.MapHeader.indexOffset + map.MapHeader.metaStart);

        }

        /// <summary>
        /// The load halo ce index header info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHaloCEIndexHeaderInfo(ref BinaryReader BR, Map map)
        {
            BR.BaseStream.Position = map.MapHeader.indexOffset;
            constant = BR.ReadInt32();
            map.PrimaryMagic = constant - (map.MapHeader.indexOffset + 40);

            BR.ReadInt32();
            BR.ReadInt32();
            metaCount = BR.ReadInt32();
            VerticeObjectCount = BR.ReadInt32();
            ModelRawDataOffset = BR.ReadInt32();
            IndicesObjectCount = BR.ReadInt32();
            ModelIndicesOffset = BR.ReadInt32();
            ModelRawDataSize = BR.ReadInt32();
            tagsOffset = map.MapHeader.indexOffset + 40;
        }

        #endregion
    }
}