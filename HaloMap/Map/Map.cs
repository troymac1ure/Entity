// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Map.cs" company="">
//   
// </copyright>
// <summary>
//   The possible msp types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.ChunkCloning;
    using HaloMap.H2MetaContainers;
    using HaloMap.Meta;

    #region Enumerations

    /// <summary>
    /// The possible msp types
    /// </summary>
    /// <remarks></remarks>
    public enum MapTypes
    {
        /// <summary>
        /// The internal.
        /// </summary>
        Internal, 

        /// <summary>
        /// The main menu.
        /// </summary>
        MainMenu, 

        /// <summary>
        /// The sp shared.
        /// </summary>
        SPShared, 

        /// <summary>
        /// The mp shared.
        /// </summary>
        MPShared, 

        /// <summary>
        /// The bitmaps.
        /// </summary>
        Bitmaps
    }

    #endregion Enumerations

    /// <summary>
    /// The map.
    /// </summary>
    /// <remarks></remarks>
    public class Map
    {
        #region Constants and Fields

        /// <summary>
        /// The br.
        /// </summary>
        public BinaryReader BR;

        /// <summary>
        /// The bsp.
        /// </summary>
        public BSPContainer BSP;

        /// <summary>
        /// The bw.
        /// </summary>
        public BinaryWriter BW;

        /// <summary>
        /// The bitmap libary.
        /// </summary>
        public BitmapLibraryLayout BitmapLibary;

        /// <summary>
        /// The display type.
        /// </summary>
        public Meta.ItemType DisplayType;

        /// <summary>
        /// The fs.
        /// </summary>
        public FileStream FS;

        /// <summary>
        /// The file names.
        /// </summary>
        public FileNamesInfo FileNames;

        /// <summary>
        /// The functions.
        /// </summary>
        public MiscFunctions Functions;

        /// <summary>
        /// The halo version.
        /// </summary>
        public HaloVersionEnum HaloVersion;

        /// <summary>
        /// The index header.
        /// </summary>
        public IndexHeaderInfo IndexHeader;

        /// <summary>
        /// The map header.
        /// </summary>
        public MapHeaderInfo MapHeader;

        /// <summary>
        /// The meta info.
        /// </summary>
        public ObjectIndexInfo MetaInfo;

        /// <summary>
        /// The primary magic.
        /// </summary>
        public int PrimaryMagic;

        /// <summary>
        /// The secondary magic.
        /// </summary>
        public int SecondaryMagic;

        /// <summary>
        /// The selected meta.
        /// </summary>
        public Meta SelectedMeta;

        /// <summary>
        /// The strings.
        /// </summary>
        public StringsInfo Strings;

        /// <summary>
        /// The unicode.
        /// </summary>
        public UnicodeTableReader Unicode;

        /// <summary>
        /// The file path.
        /// </summary>
        public string filePath;

        /// <summary>
        /// The is open.
        /// </summary>
        public bool isOpen;

        /// <summary>
        /// The open map type.
        /// </summary>
        public MapTypes openMapType;

        /// <summary>
        /// The ugh.
        /// </summary>
        public ugh_ ugh;

        /// <summary>
        /// Gets the chunk tools.
        /// </summary>
        /// <remarks></remarks>
        public ChunkAdder ChunkTools { get; private set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// An internal constructor so this class must
        /// be made via static map.FromFile...
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks></remarks>
        internal Map(string fileName)
        {
            filePath = fileName;
            Functions = new MiscFunctions(this);
            ChunkTools = new ChunkAdder(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get meta from tag index.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="map">The map.</param>
        /// <param name="manualScan">The manual scan.</param>
        /// <param name="parse">The parse.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Meta GetMetaFromTagIndex(int tag, Map map, bool manualScan, bool parse)
        {
            map.OpenMap(MapTypes.Internal);

            Meta meta = new Meta(map);

            meta.TagIndex = tag;
            meta.ScanMetaItems(manualScan, parse);

            map.CloseMap();

            meta.SortItemsByOffset();

            return meta;
        }

        /// <summary>
        /// Loads a .map file and returns it as
        /// a map object
        /// </summary>
        /// <param name="fileName">The file Name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Map LoadFromFile(string fileName)
        {
            // create the map object and give it the map filename
            Map map = new Map(fileName);

            // open the map
            map.OpenMap(MapTypes.Internal);

            // if the open failed, nothing we can do further
            if (!map.isOpen)
            {
                return null;
            }

            // Get map version, (what halo this is)
            map.BR.BaseStream.Position = 4;
            int version = map.BR.ReadInt32();

            // set the halo game type in the map object
            switch (version)
            {
                case 609:
                    map.HaloVersion = HaloVersionEnum.HaloCE;
                    break;
                case 8:
                    map.HaloVersion = HaloVersionEnum.Halo2;
                    map.BR.BaseStream.Position = 0x24;
                    switch (map.BR.ReadInt32())
                    {
                        case -1:
                            map.HaloVersion = HaloVersionEnum.Halo2Vista;
                            break;
                        case 0:
                            map.HaloVersion = HaloVersionEnum.Halo2;
                            break;
                    }
                    break;
                case 5:
                    map.HaloVersion = HaloVersionEnum.Halo1;
                    break;

                    // if we dont support the map, dont open it
                default:
                    MessageBox.Show("This is not a supported map type.");
                    map.CloseMap();
                    return null;
            }

            // read from the map
            map.LoadMap();

            // close file and return map obj
            map.CloseMap();
            return map;
        }

        /// <summary>
        /// Reloads the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns the reloaded Map</returns>
        /// <remarks>This needs to use a return value, because the loaded map changes were not being saved. They would be
        /// lost when the function returned and we ?cannot pass Map as a ref?.</remarks>
        public static Map Refresh(Map map)
        {
            map.CloseMap();
            Meta tempmeta = new Meta(map);
            if (map.SelectedMeta != null)
            {
                tempmeta = map.SelectedMeta;
            }

            map = LoadFromFile(map.filePath);            

            if (tempmeta.MS != null)
            {
                int tagIndex = map.Functions.ForMeta.FindByNameAndTagType(tempmeta.type, tempmeta.name);
                // SelectedMeta belongs to map above, so keeps original map "locked"
                Meta meta = Map.GetMetaFromTagIndex(tagIndex, map, tempmeta.scannedwithent, tempmeta.parsed);
                map.SelectedMeta = meta;
                tempmeta.Dispose();
            }
            return map;
        }

        /// <summary>
        /// The buffer read write.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="BW">The bw.</param>
        /// <param name="size">The size.</param>
        /// <remarks></remarks>
        public void BufferReadWrite(ref BinaryReader BR, ref BinaryWriter BW, int size)
        {
            int buffersize = 0xA00000;
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

        /// <summary>
        /// The close map.
        /// </summary>
        /// <remarks></remarks>
        public void CloseMap()
        {
            if (isOpen == false)
            {
                return;
            }

            BR.Close();
            if (BW != null)
            {
                BW.Close();
            }

            FS.Close();
            isOpen = false;
        }

        /// <summary>
        /// Opens the
        /// </summary>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public void OpenMap(MapTypes type)
        {
            OpenMap(type, true);
        }

        /// <summary>
        /// Opens a Halo/Halo 2 based map for reading / writing (not readonly).
        /// </summary>
        /// <param name="type">MapTypes.(Internal, MainMenu, MPShared, SPShared, Bitmaps)</param>
        /// <param name="readOnly">opens map file as read-only (does NOT apply to internal map file)</param>
        /// <remarks></remarks>
        public void OpenMap(MapTypes type, bool readOnly)
        {
            if (isOpen && type == this.openMapType)
            {
                if (!(type != MapTypes.Internal && (FS.CanWrite == readOnly)))
                {
                    return;
                }
            }

            CloseMap();
            try
            {
                string temp = string.Empty;
                //int x = 0;
                switch (type)
                {
                    case MapTypes.Internal:
                        temp = filePath;
                        break;
                    case MapTypes.MainMenu:

                        // x = filePath.LastIndexOf('\\');
                        // temp = filePath.Substring(0, x + 1) + "mainmenu.map";
                        temp = Prefs.pathMainmenu;
                        if ((!Prefs.useDefaultMaps) && (filePath.LastIndexOf('\\') != -1))
                        {
                            string currentMainMenu = filePath.Substring(0, filePath.LastIndexOf('\\') + 1) +
                                                     "mainmenu.map";
                            if (File.Exists(currentMainMenu))
                            {
                                temp = currentMainMenu;
                            }
                        }

                        break;
                    case MapTypes.MPShared:
                        temp = Prefs.pathShared;
                        if ((!Prefs.useDefaultMaps) && (filePath.LastIndexOf('\\') != -1))
                        {
                            string currentMPShared = filePath.Substring(0, filePath.LastIndexOf('\\') + 1) +
                                                     "shared.map";
                            if (File.Exists(currentMPShared))
                            {
                                temp = currentMPShared;
                            }
                        }

                        break;
                    case MapTypes.SPShared:
                        temp = Prefs.pathSPShared;
                        if ((!Prefs.useDefaultMaps) && (filePath.LastIndexOf('\\') != -1))
                        {
                            string currentSPShared = filePath.Substring(0, filePath.LastIndexOf('\\') + 1) +
                                                     "single_player_shared.map";
                            if (File.Exists(currentSPShared))
                            {
                                temp = currentSPShared;
                            }
                        }

                        break;
                    case MapTypes.Bitmaps:
                        temp = Prefs.pathBitmaps;
                        break;
                }

                FileAccess fa = FileAccess.Read;
                if (type == MapTypes.Internal)
                {
                    fa = FileAccess.ReadWrite;
                }

                if (type != MapTypes.Internal && !readOnly)
                {
                    if (
                        MessageBox.Show(
                            "You are about to write to a shared file\n" + temp + "\nContinue?", 
                            "Write to shared file?", 
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        fa = FileAccess.ReadWrite;
                    }
                    else
                    {
                        return;
                    }
                }

                FS = new FileStream(temp, FileMode.Open, fa);
                BR = new BinaryReader(FS);
                if (fa != FileAccess.Read)
                {
                    BW = new BinaryWriter(FS);
                }

                isOpen = true;
                openMapType = type;
            }
            catch (Exception ex)
            {
                if (type.ToString() == "Internal")
                {
                    Global.ShowErrorMsg(filePath + " - Not Found\n", ex);
                }
                else
                {
                    Global.ShowErrorMsg(type + " - Map Not Found\n", ex);
                }

                isOpen = false;
            }
        }

        /// <summary>
        /// The sign.
        /// </summary>
        /// <remarks></remarks>
        public void Sign()
        {
            if (HaloVersion != HaloVersionEnum.Halo2 &&
                HaloVersion != HaloVersionEnum.Halo2Vista)
            {
                MessageBox.Show("Only Sign Halo 2 Maps");
                return;
            }

            OpenMap(MapTypes.Internal);
            int result = 0;
            this.BR.BaseStream.Seek(2048, SeekOrigin.Begin);
            const int bufferSize = 16384;
            int sizeCheck;
            do
            {
                byte[] buffer = this.BR.ReadBytes(bufferSize);
                sizeCheck = buffer.Length;
                for (int x = 0; x < buffer.Length; x += 4)
                {
                    result ^= BitConverter.ToInt32(buffer, x);
                }
            }
            while (sizeCheck == bufferSize);
            this.BW.BaseStream.Seek(720, SeekOrigin.Begin);
            this.BW.Write(result);

            this.MapHeader.signature = result;
            CloseMap();
        }

        #endregion

        #region Methods

        /// <summary>
        /// reads info from the current open map file and
        /// stores in it this map object
        /// </summary>
        /// <remarks></remarks>
        private void LoadMap()
        {
            switch (HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                    {
                        MapHeader = new MapHeaderInfo(ref BR, HaloVersion);
                        IndexHeader = new IndexHeaderInfo(ref BR, this);
                        MetaInfo = new ObjectIndexInfo(ref BR, this);
                        FileNames = new FileNamesInfo(ref BR, this);
                        Strings = new StringsInfo(ref BR, this);
                        CloseMap();

                        Unicode = new UnicodeTableReader(this.filePath, this.MetaInfo.Offset[0]);

                        break;
                    }

                case HaloVersionEnum.Halo2Vista:
                    {
                        MapHeader = new MapHeaderInfo(ref BR, HaloVersion);
                        IndexHeader = new IndexHeaderInfo(ref BR, this);
                        MetaInfo = new ObjectIndexInfo(ref BR, this);
                        FileNames = new FileNamesInfo(ref BR, this);
                        Strings = new StringsInfo(ref BR, this);
                        CloseMap();

                        Unicode = new UnicodeTableReader(this.filePath, this.MetaInfo.Offset[0]);

                        break;
                    }

                case HaloVersionEnum.HaloCE:
                    {
                        BitmapLibary = new BitmapLibraryLayout(this);
                        MapHeader = new MapHeaderInfo(ref BR, HaloVersion);
                        IndexHeader = new IndexHeaderInfo(ref BR, this);
                        MetaInfo = new ObjectIndexInfo(ref BR, this);
                        FileNames = new FileNamesInfo(ref BR, this);
                        this.MapHeader.fileNamesSize = FileNames.FileNameStringsSize;
                        this.MapHeader.offsetTofileNames = FileNames.FileNameStringsOffset;
                        break;
                    }

                case HaloVersionEnum.Halo1:
                    {
                        // BitmapLibary = new BitmapLibraryLayout(this);
                        MapHeader = new MapHeaderInfo(ref BR, HaloVersion);
                        IndexHeader = new IndexHeaderInfo(ref BR, this);
                        MetaInfo = new ObjectIndexInfo(ref BR, this);
                        FileNames = new FileNamesInfo(ref BR, this);
                        this.MapHeader.fileNamesSize = FileNames.FileNameStringsSize;
                        this.MapHeader.offsetTofileNames = FileNames.FileNameStringsOffset;
                        break;
                    }
            }

            switch (HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    ugh_.GetUghContainerInfo(this);
                    this.BSP = new BSPContainer(this);

                    break;
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    this.BSP = new BSPContainer(this);
                    break;
            }

            DisplayType = Meta.ItemType.Reflexive;
        }

        #endregion
    }

    /// <summary>
    /// The supported halo games by this editor
    /// </summary>
    /// <remarks></remarks>
    public enum HaloVersionEnum
    {
        /// <summary>
        /// Halo 2 Xbox
        /// </summary>
        Halo2, 

        /// <summary>
        /// Halo 2 Vista
        /// </summary>
        Halo2Vista,

        /// <summary>
        /// Halo CE
        /// </summary>
        HaloCE, 

        /// <summary>
        /// Halo 1
        /// </summary>
        Halo1
    }
}