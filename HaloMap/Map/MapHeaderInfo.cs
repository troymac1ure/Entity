// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapHeaderInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The map header info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;

    /// <summary>
    /// The map header info.
    /// </summary>
    /// <remarks></remarks>
    public class MapHeaderInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The combined size.
        /// </summary>
        public int combinedSize;

        /// <summary>
        /// The file count.
        /// </summary>
        public int fileCount;

        /// <summary>
        /// The file names size.
        /// </summary>
        public int fileNamesSize;

        /// <summary>
        /// The file size.
        /// </summary>
        public int fileSize;

        /// <summary>
        /// The index offset.
        /// </summary>
        public int indexOffset;

        /// <summary>
        /// The map name.
        /// </summary>
        public string mapName;

        /// <summary>
        /// The map type.
        /// </summary>
        public MapTypes mapType;

        /// <summary>
        /// The meta size.
        /// </summary>
        public int metaSize;

        /// <summary>
        /// The meta start.
        /// </summary>
        public int metaStart;

        /// <summary>
        /// The offset to crazy.
        /// </summary>
        public int offsetToCrazy;

        /// <summary>
        /// The offset to string index.
        /// </summary>
        public int offsetToStringIndex;

        /// <summary>
        /// The offset to string names 1.
        /// </summary>
        public int offsetToStringNames1;

        /// <summary>
        /// The offset to string names 2.
        /// </summary>
        public int offsetToStringNames2;

        /// <summary>
        /// The offset tofile index.
        /// </summary>
        public int offsetTofileIndex;

        /// <summary>
        /// The offset tofile names.
        /// </summary>
        public int offsetTofileNames;

        /// <summary>
        /// The scenario path.
        /// </summary>
        public string scenarioPath;

        /// <summary>
        /// The script reference count.
        /// </summary>
        public int scriptReferenceCount;

        /// <summary>
        /// The signature.
        /// </summary>
        public int signature;

        /// <summary>
        /// The size of crazy.
        /// </summary>
        public int sizeOfCrazy;

        /// <summary>
        /// The size of script reference.
        /// </summary>
        public int sizeOfScriptReference;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapHeaderInfo"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="haloversion">The haloversion.</param>
        /// <remarks></remarks>
        public MapHeaderInfo(ref BinaryReader BR, HaloVersionEnum haloversion)
        {
            switch (haloversion)
            {
                case HaloVersionEnum.Halo2:
                    LoadHalo2MapHeaderInfo(ref BR);
                    break;
                case HaloVersionEnum.Halo2Vista:
                    LoadHaloCEMapHeaderInfo(ref BR);
                    break;
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    LoadHalo2MapHeaderInfo(ref BR);
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load halo 2 map header info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <remarks></remarks>
        public void LoadHalo2MapHeaderInfo(ref BinaryReader BR)
        {
            // map stuff
            BR.BaseStream.Position = 8;
            fileSize = BR.ReadInt32();
            BR.BaseStream.Position = 16;
            indexOffset = BR.ReadInt32();
            metaStart = BR.ReadInt32();
            metaSize = BR.ReadInt32();
            combinedSize = BR.ReadInt32();
            BR.BaseStream.Position = 340;
            sizeOfCrazy = BR.ReadInt32();
            offsetToCrazy = BR.ReadInt32();

            // string stuff
            BR.BaseStream.Position = 352;
            offsetToStringNames1 = BR.ReadInt32();
            scriptReferenceCount = BR.ReadInt32();
            sizeOfScriptReference = BR.ReadInt32();
            offsetToStringIndex = BR.ReadInt32();
            offsetToStringNames2 = BR.ReadInt32();

            // map names and code to check if it is an external map
            BR.BaseStream.Position = 408;
            mapName = new string(BR.ReadChars(36));
            BR.BaseStream.Position = 444;
            scenarioPath = new string(BR.ReadChars(64));
            mapType = MapTypes.Internal;
            if (scenarioPath.IndexOf("scenarios\\ui\\mainmenu\\mainmenu") != -1)
            {
                mapType = MapTypes.MainMenu;
            }

            if (scenarioPath.IndexOf("scenarios\\shared\\shared") != -1)
            {
                mapType = MapTypes.MPShared;
            }

            if (scenarioPath.IndexOf("scenarios\\shared\\single_player_shared") != -1)
            {
                mapType = MapTypes.SPShared;
            }

            // read in stuff about meta names
            BR.BaseStream.Position = 704;
            fileCount = BR.ReadInt32();
            offsetTofileNames = BR.ReadInt32();
            fileNamesSize = BR.ReadInt32();
            offsetTofileIndex = BR.ReadInt32();

            // signature
            signature = BR.ReadInt32();
        }

        /// <summary>
        /// Halo 2 Vista map header info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <remarks></remarks>
        public void LoadHaloCEMapHeaderInfo(ref BinaryReader BR)
        {
            BR.BaseStream.Position = 8;
            fileSize = BR.ReadInt32();
            BR.BaseStream.Position = 16;
            indexOffset = BR.ReadInt32();
            metaStart = BR.ReadInt32();
            metaSize = BR.ReadInt32();
            combinedSize = BR.ReadInt32();
            BR.BaseStream.Position = 340;
            sizeOfCrazy = BR.ReadInt32();
            offsetToCrazy = BR.ReadInt32();

            // string stuff
            BR.BaseStream.Position = 364;
            offsetToStringNames1 = BR.ReadInt32();
            scriptReferenceCount = BR.ReadInt32();
            sizeOfScriptReference = BR.ReadInt32();
            offsetToStringIndex = BR.ReadInt32();
            offsetToStringNames2 = BR.ReadInt32();

            /*
            BR.BaseStream.Position = 364;
            SIDMetaTableOffset = BR.ReadInt32();
            SIDCount = BR.ReadInt32();
            SIDTableSize = BR.ReadInt32();
            SIDIndexOffset = BR.ReadInt32();
            SIDTableOffset = BR.ReadInt32();
            */

            // map names and code to check if it is an external map
            BR.BaseStream.Position = 420;
            mapName = new string(BR.ReadChars(36));
            BR.BaseStream.Position = 456;
            scenarioPath = new string(BR.ReadChars(80));
            mapType = MapTypes.Internal;
            if (scenarioPath.IndexOf("scenarios\\ui\\mainmenu\\mainmenu") != -1)
            {
                mapType = MapTypes.MainMenu;
            }

            if (scenarioPath.IndexOf("scenarios\\shared\\shared") != -1)
            {
                mapType = MapTypes.MPShared;
            }

            if (scenarioPath.IndexOf("scenarios\\shared\\single_player_shared") != -1)
            {
                mapType = MapTypes.SPShared;
            }

            // read in stuff about meta names
            BR.BaseStream.Position = 716;
            fileCount = BR.ReadInt32();
            offsetTofileNames = BR.ReadInt32();
            fileNamesSize = BR.ReadInt32();
            offsetTofileIndex = BR.ReadInt32();

            //Model Raw
            /*
            BR.BaseStream.Position = 744;
            ModelRawTableStart = BR.ReadInt32();
            ModelRawTableSize = BR.ReadInt32();
            */

            // signature
            BR.BaseStream.Position = 752;
            signature = BR.ReadInt32();


        }
        #endregion
    }
}