// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MiscFunctions.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for MathFunctions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System;
    using System.IO;

    using HaloMap.Plugins;
    using HaloMap.RawData;

    /// <summary>
    /// Summary description for MathFunctions.
    /// </summary>
    /// <remarks></remarks>
    public class MiscFunctions
    {
        #region Constants and Fields

        /// <summary>
        /// 
        /// </summary>
        private Map map;

        /// <summary>
        /// The meta.
        /// </summary>
        /// <remarks></remarks>
        public MetaFunctions ForMeta { get; private set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MiscFunctions"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public MiscFunctions(Map map)
        {
            this.map = map;
            ForMeta = new MetaFunctions(map);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The format if ps in directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public static void FormatIFPsInDirectory(string path)
        {
            string[] temps = Directory.GetFiles(path, "*.ifp2");

            for (int counter = 0; counter < 2; counter++)
            {
                foreach (string s in temps)
                {
                    IFPIO ifp = new IFPIO();
                    ifp.ReadIFP(s);
                    string tempentstring = s.Replace("ifp2", "ent");
                    tempentstring = tempentstring.Replace("ifp", "ent");
                    ifp.IFPOutput(tempentstring);
                }

                temps = Directory.GetFiles(path, "*.ifp");
            }
        }

        /// <summary>
        /// The find model by base class.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <returns>The find model by base class.</returns>
        /// <remarks></remarks>
        public int FindModelByBaseClass(int tagIndex)
        {
            if (tagIndex == -1)
            {
                return -1;
            }

            int alreadyOpen;
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    switch (map.MetaInfo.TagType[tagIndex])
                    {
                        case "bipd":
                        case "bloc":
                        case "char":
                        case "ctrl":
                        case "eqip":
                        case "itmc":
                        case "mach":
                        case "scen":
                        case "vehc":
                        case "vehi":
                        case "weap":
                            break;
                        default:
                            return -1;
                    }

                    alreadyOpen = map.isOpen ? (int)map.openMapType : -1;
                    if (alreadyOpen != (int)MapTypes.Internal)
                    {
                        map.OpenMap(MapTypes.Internal);
                    }

                    if (map.MetaInfo.TagType[tagIndex] == "char")
                    {
                        string ss = map.FileNames.Name[tagIndex];

                        // Get offset to BIPD listing
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 16;
                        tagIndex = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                        if (tagIndex == -1)
                            return -1;
                    }

                    if (map.MetaInfo.TagType[tagIndex] == "vehc" | map.MetaInfo.TagType[tagIndex] == "itmc")
                    {
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 20;
                        tagIndex = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                     
                        if (tagIndex == -1)
                            return -1;
                    }

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 56;
                    int temptag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (temptag == -1)
                    {
                        return -1;
                    }

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[temptag] + 4;
                    int temptag2 = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (alreadyOpen == -1)
                    {
                        map.CloseMap();
                    }
                    else
                    {
                        map.OpenMap((MapTypes)alreadyOpen);
                    }

                    return temptag2;
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    alreadyOpen = map.isOpen ? -1 : (int)map.openMapType;
                    if (alreadyOpen != (int)MapTypes.Internal)
                    {
                        map.OpenMap(MapTypes.Internal);
                    }

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 56;
                    switch (map.MetaInfo.TagType[tagIndex])
                    {
                        case "itmc":
                        case "vehi":
                        case "bipd":
                        case "eqip":
                        case "weap":
                        case "scen":
                        case "mach":
                            break;
                        default:
                            return -1;
                    }

                    if (map.MetaInfo.TagType[tagIndex] == "itmc")
                    {
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 140;
                        tagIndex = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    }

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 52;
                    int temptag2x = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (alreadyOpen == -1)
                    {
                        map.CloseMap();
                    }
                    else
                    {
                        map.OpenMap((MapTypes)alreadyOpen);
                    }

                    if (temptag2x == -1)
                    {
                        return -1;
                    }

                    return temptag2x;
            }

            return -1;
        }

        /// <summary>
        /// The padding.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="paddingchunksize">The paddingchunksize.</param>
        /// <returns>The padding.</returns>
        /// <remarks></remarks>
        public int Padding(int size, int paddingchunksize)
        {
            int temp1 = 0;
            Math.DivRem(size, paddingchunksize, out temp1);
            if (temp1 == 0)
            {
                return 0;
            }

            int temp2 = paddingchunksize - temp1;
            return temp2;
        }

        /// <summary>
        /// The parse pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public void ParsePointer(ref int pointer, ref MapTypes type)
        {
            long tempmap = pointer & 0XC0000000;
            pointer = pointer & 0X3FFFFFFF;
            if (tempmap == 0)
            {
                type = MapTypes.Internal;
                return;
            }
            else if (tempmap == 0X80000000)
            {
                type = MapTypes.MPShared;

                return;
            }
            else if (tempmap == 0XC0000000)
            {
                type = MapTypes.SPShared;
                return;
            }
            else if (tempmap == 0X40000000)
            {
                type = MapTypes.MainMenu;
                return;
            }
        }

        #endregion

        /// <summary>
        /// Summary description for MetaFunctions.
        /// </summary>
        /// <remarks></remarks>
        public class MetaFunctions
        {
            /// <summary>
            /// 
            /// </summary>
            private Map map;

            /// <summary>
            /// Initializes a new instance of the <see cref="MetaFunctions"/> class.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <remarks></remarks>
            public MetaFunctions(Map map)
            {
                this.map = map;
            }

            #region Public Methods

            /// <summary>
            /// The check for raw.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public RawDataContainerType CheckForRaw(string type)
            {
                switch (type)
                {
                    case "PRTM":
                        return RawDataContainerType.PRTM;
                    case "DECR":
                        return RawDataContainerType.DECR;
                    case "ltmp":
                        return RawDataContainerType.LightMap;
                    case "mod2":
                        return RawDataContainerType.Model;
                    case "mode":
                        return RawDataContainerType.Model;
                    case "bitm":
                        return RawDataContainerType.Bitmap;
                    case "jmad":
                        return RawDataContainerType.Animation;
                    case "sbsp":
                        return RawDataContainerType.BSP;
                    case "weat":
                        return RawDataContainerType.Weather;
                    case "snd!":
                        return RawDataContainerType.Sound;
                    case "ugh!":
                        return RawDataContainerType.CoconutsModel;
                    default:
                        return RawDataContainerType.Empty;
                }
            }

            /// <summary>
            /// The find by name and tag type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="name">The name.</param>
            /// <returns>The find by name and tag type.</returns>
            /// <remarks></remarks>
            public int FindByNameAndTagType(string type, string name)
            {
                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    if (type == map.MetaInfo.TagType[x] && name == map.FileNames.Name[x])
                    {
                        return x;
                    }
                }

                return -1;
            }

            /// <summary>
            /// The find meta by id.
            /// </summary>
            /// <param name="id">The id.</param>
            /// <returns>The find meta by id.</returns>
            /// <remarks></remarks>
            public int FindMetaByID(int id)
            {
                int xx = 0;
                object xxx = map.MetaInfo.identHT[id];
                if (xxx == null)
                {
                    return -1;
                }

                xx = (int)xxx;
                return xx;
            }

            /// <summary>
            /// The find meta by offset.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <returns>The find meta by offset.</returns>
            /// <remarks></remarks>
            public int FindMetaByOffset(int offset)
            {
                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    if (map.HaloVersion == HaloVersionEnum.HaloCE)
                    {
                        if (map.MetaInfo.external[x])
                        {
                            continue;
                        }
                    }

                    if (offset >= map.MetaInfo.Offset[x] && offset < map.MetaInfo.Offset[x] + map.MetaInfo.Size[x])
                    {
                        return x;
                    }
                }

                return -1;
            }

            /// <summary>
            /// The read raw.
            /// </summary>
            /// <param name="tagIndex">Index of the tag.</param>
            /// <param name="dontreadraw">The dontreadraw.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public RawDataContainer ReadRaw(int tagIndex, bool dontreadraw)
            {
                RawDataContainer tempcontainer = new RawDataContainer();

                switch (CheckForRaw(map.MetaInfo.TagType[tagIndex]))
                {
                    case RawDataContainerType.PRTM:
                        tempcontainer = new PRTM();
                        break;
                    case RawDataContainerType.DECR:
                        tempcontainer = new DECR();
                        break;
                    case RawDataContainerType.Animation:
                        tempcontainer = new Animation();
                        break;
                    case RawDataContainerType.Model:
                        tempcontainer = new Model();
                        break;
                    case RawDataContainerType.Bitmap:
                        tempcontainer = new BitmapRaw();
                        break;
                    case RawDataContainerType.BSP:
                        tempcontainer = new BSPRaw();
                        break;

                    case RawDataContainerType.Weather:
                        tempcontainer = new Weather();
                        break;
                    case RawDataContainerType.Sound:
                        tempcontainer = new Sound();
                        break;
                    case RawDataContainerType.CoconutsModel:
                        tempcontainer = new UghRawContainer();
                        break;
                    case RawDataContainerType.LightMap:
                        tempcontainer = new LightmapRaw();
                        break;
                    default:
                        break;
                }

                tempcontainer.Read(tagIndex, map, dontreadraw);
                return tempcontainer;
            }

            #endregion
        }
    }
}