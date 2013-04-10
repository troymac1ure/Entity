// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuEdit.cs" company="">
//   
// </copyright>
// <summary>
//   The mainmenu edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using entity.MapForms;

    using HaloMap;
    using HaloMap.DDSFunctions;
    using HaloMap.Map;
    using HaloMap.Meta;

    using Globals;

    using HaloMap.RawData;

    /// <summary>
    /// The mainmenu edit.
    /// </summary>
    /// <remarks></remarks>
    public partial class MainmenuEdit : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The mp levels.
        /// </summary>
        private readonly List<mapInfo> MPLevels = new List<mapInfo>();

        /// <summary>
        /// The camp scenarios.
        /// </summary>
        private readonly List<campaignScenarios> campScenarios = new List<campaignScenarios>();

        /// <summary>
        /// The campaign levels.
        /// </summary>
        private readonly List<mapInfo> campaignLevels = new List<mapInfo>();

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The static i ds.
        /// </summary>
        private readonly IDInfo[] staticIDs = new[]
            {
                // Campaigns
                new IDInfo(true, "the heretic", 1), 
                new IDInfo(true, "armory", 101), 
                new IDInfo(true, "cairo station", 105), 
                new IDInfo(true, "outskirts", 301), 
                new IDInfo(true, "metropolis", 305), 
                new IDInfo(true, "the arbiter", 401), 
                new IDInfo(true, "oracle", 405), 
                new IDInfo(true, "delta halo", 501), 
                new IDInfo(true, "regret", 505), 
                new IDInfo(true, "sacred icon", 601), 
                new IDInfo(true, "quarantine zone", 605), 
                new IDInfo(true, "gravemind", 701), 
                new IDInfo(true, "high charity", 801), 
                new IDInfo(true, "uprising", 705), 
                new IDInfo(true, "the great journey", 805), // Multiplayer
                new IDInfo(false, "ascension", 80), 
                new IDInfo(false, "beaver creek", 100), 
                new IDInfo(false, "burial mounds", 60), 
                new IDInfo(false, "coagulation", 110), 
                new IDInfo(false, "colossus", 70), 
                new IDInfo(false, "ivory tower", 10), 
                new IDInfo(false, "foundation", 120), 
                new IDInfo(false, "headlong", 800), 
                new IDInfo(false, "lockout", 50), 
                new IDInfo(false, "midship", 20), 
                new IDInfo(false, "waterworks", 40), 
                new IDInfo(false, "zanzibar", 30), 
            };

        /// <summary>
        /// The mp sort order.
        /// </summary>
        private List<int[]> MPSortOrder = new List<int[]>();

        /// <summary>
        /// The br.
        /// </summary>
        private BinaryReader br;

        /// <summary>
        /// The bw.
        /// </summary>
        private BinaryWriter bw;

        /// <summary>
        /// The campaign sort order.
        /// </summary>
        private List<int[]> campaignSortOrder = new List<int[]>();

        /// <summary>
        /// The current drag.
        /// </summary>
        private mapInfo currentDrag;

        /// <summary>
        /// The current levels.
        /// </summary>
        private List<mapInfo> currentLevels;

        /// <summary>
        /// The current sort order.
        /// </summary>
        //private List<int[]> currentSortOrder;

        /// <summary>
        /// The old selected index.
        /// </summary>
        private int oldSelectedIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainmenuEdit"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MainmenuEdit()
        {
            this.Cursor = Cursors.WaitCursor;
            InitializeComponent();

            map = Map.LoadFromFile(Prefs.pathMainmenu);
            if (map == null)
            {
                MessageBox.Show("Mainmenu load failed!");
                return;
            }

            loadMainMenuData();
            loadBitmaps();
            this.Cursor = Cursors.Arrow;

            MessageBox.Show("Campaign Data changes will not be saved in this version!");
        }

        #endregion

        #region Enums

        /// <summary>
        /// The languages.
        /// </summary>
        /// <remarks></remarks>
        private enum languages
        {
            /// <summary>
            /// The english.
            /// </summary>
            English = 0, 

            /// <summary>
            /// The japanese.
            /// </summary>
            Japanese = 1, 

            /// <summary>
            /// The german.
            /// </summary>
            German = 2, 

            /// <summary>
            /// The french.
            /// </summary>
            French = 3, 

            /// <summary>
            /// The spanish.
            /// </summary>
            Spanish = 4, 

            /// <summary>
            /// The italian.
            /// </summary>
            Italian = 5, 

            /// <summary>
            /// The korean.
            /// </summary>
            Korean = 6, 

            /// <summary>
            /// The chinese.
            /// </summary>
            Chinese = 7, 

            /// <summary>
            /// The portuguese.
            /// </summary>
            Portuguese = 8
        }

        #endregion

        #region Methods

        /// <summary>
        /// The mainmenu edit_ drag drop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MainmenuEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filename in files)
                {
                    addMap(filename);
                }
            }
        }

        /// <summary>
        /// The mainmenu edit_ drag enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MainmenuEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// The mainmenu edit_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MainmenuEdit_Load(object sender, EventArgs e)
        {
            currentLevels = MPLevels;
            cbLanguage.Items.AddRange(Enum.GetNames(typeof(languages)));
            cbLanguage.SelectedIndex = 0;
            loadMapNames(0, true);
            ofdLoadMap.Filter = "Map files (*.map)|*.map";
            ofdLoadMap.InitialDirectory = Prefs.pathMapsFolder;
            ofdLoadMap.Multiselect = true;
        }

        /// <summary>
        /// The add map.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
        private void addMap(string filename)
        {
            if (currentLevels[currentLevels.Count - 1].mapID != -1)
            {
                MessageBox.Show("There are already " + currentLevels.Count + " maps! No more can be added.");
                return;
            }

            Map tempmap = Map.LoadFromFile(filename);
            if (tempmap == null)
            {
                MessageBox.Show("Unable to load MAP: " + filename);
            }

            // Set default gametype numbers
            gametypes gameTypes = new gametypes();

            // Load SCNR tag
            for (int i = 0; i < tempmap.IndexHeader.metaCount; i++)
            {
                if (tempmap.MetaInfo.TagType[i] == "scnr")
                {
                    tempmap.SelectedMeta = Map.GetMetaFromTagIndex(1, tempmap, false, false);
                    break;
                }
            }

            Meta m = tempmap.SelectedMeta;
            BinaryReader br = new BinaryReader(m.MS);

            // Netgame flags, size 32
            br.BaseStream.Position = 280;
            int netgameFlagsCount = br.ReadInt32();
            int netgameFlagsOffset = br.ReadInt32() - m.offset - m.magic;

            for (int i = 0; i < netgameFlagsCount; i++)
            {
                // offset 16 is netgame type (0-1=CTF, 2-3=Assault, 4=Odball, 6=Race, 9=Headhunter, 10=Territories, 11-18=KOTH)
                br.BaseStream.Position = netgameFlagsOffset + i * 32 + 16;
                int gameType = br.ReadInt16();
                int teamNum = br.ReadInt16();

                // We only need to check CTF & Assault. For all else, it will default to 8 teams max
                if (gameType == 0 || gameType == 1)
                {
                    gameTypes.gameTypes[(int)gametypes.gameTypesNames.CTF].Teams[teamNum] = true;
                }
                else if (gameType == 2 || gameType == 3)
                {
                    gameTypes.gameTypes[(int)gametypes.gameTypesNames.Assault].Teams[teamNum] = true;
                }
            }

            // Start new MapID at 90 for MP, 101 for Campaign
            int newID = currentLevels[0].campaignNumber == -1 ? 90 : 201;
            for (int i = 0; i < currentLevels.Count; i++)
            {
                if (currentLevels[i].MapID == newID)
                {
                    // increase MapID by 10 for MP, 100 for Campaign and restart search
                    newID = currentLevels[i].MapID + (currentLevels[0].campaignNumber == -1 ? 10 : 100);
                    i = -1;
                }
            }

            mapInfo newMap = new mapInfo(tempmap.MapHeader.mapName.Trim('\0'), tempmap.MapHeader.scenarioPath, newID);
            gameTypes.Save(newMap);

            // Load Map Picture as raw data (for transfer) and Bitmap (for display)
            tempmap.OpenMap(MapTypes.Internal);
            m = MapForm.GetMapBitmapMeta(tempmap);

            //////////////////////////////////
            // Backwash for example has no internal picture, so it errors.
            //////////////////////////////////
            if (m != null)
            {
                newMap.screenShotRaw = m.raw.rawChunks[0];
                ParsedBitmap pm = new ParsedBitmap(ref m, tempmap);
                
                newMap.screenShot = pm.FindChunkAndDecode(0, 0, 0, ref m, tempmap, 0, 0);
            }
            else
            {
                newMap.screenShotRaw = new RawDataChunk();
                newMap.screenShotRaw.MS = new MemoryStream();
                newMap.screenShotRaw.pointerMetaOffset = 104;
            }

            tempmap.CloseMap();

            // Save the current Map Bitmap Offset listing in case a map has been added, then removed, so we don't keep
            // adding new un-needed Raw Listings
            newMap.screenShotOffset = currentLevels[lbMapListing.Items.Count].screenShotOffset;

            // Insert the new map & remove the current listing to make sure that we keep the same # of maps total for
            // when we rewrite
            currentLevels[this.lbMapListing.Items.Count] = newMap;
            this.lbMapListing.Items.Add(newMap.Name);
        }

        /// <summary>
        /// The btn add map_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnAddMap_Click(object sender, EventArgs e)
        {
            if (ofdLoadMap.ShowDialog() == DialogResult.OK)
            {
                string s = this.Text;
                this.Enabled = false;
                for (int x = 1; x <= ofdLoadMap.FileNames.Length; x++)
                {
                    // We do this to keep the selected names in the order they were selected...
                    if (x == ofdLoadMap.FileNames.Length)
                    {
                        x = 0;
                    }

                    this.Text = "Please Wait - [adding " + ofdLoadMap.FileNames[x] + "]";
                    Application.DoEvents();
                    addMap(ofdLoadMap.FileNames[x]);
                    lbMapListing.SelectedIndex = lbMapListing.Items.Count - 1;
                    if (x == 0)
                    {
                        break;
                    }
                }

                this.Enabled = true;
                this.Text = s;
            }
        }

        /// <summary>
        /// The btn copy to all_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnCopyToAll_Click(object sender, EventArgs e)
        {
            string name = tbMapName.Text;
            string desc = tbMapDescription.Text;
            for (int i = 0; i < currentLevels[lbMapListing.SelectedIndex].Names.Length; i++)
            {
                currentLevels[lbMapListing.SelectedIndex].Names[i] = name;
                currentLevels[lbMapListing.SelectedIndex].Descriptions[i] = desc;
            }
        }

        /// <summary>
        /// The btn load image_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog getImage = new OpenFileDialog();
            getImage.InitialDirectory = Prefs.pathBitmapsFolder;
            getImage.DefaultExt = "dds";
            getImage.Filter = "DDS & Map files|*.dds;*.map|DDS files (*.dds)|*.dds|Map Files (*.map)|*.map";
            if (getImage.ShowDialog() == DialogResult.OK)
            {
                Prefs.pathBitmapsFolder = getImage.FileName.Substring(0, getImage.FileName.LastIndexOf('\\'));
                
                if (getImage.FileName.EndsWith("dds", StringComparison.OrdinalIgnoreCase))
                {
                    

                    FileStream fs = new FileStream(getImage.FileName, FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);

                    DDS.DDS_HEADER_STRUCTURE dds = new DDS.DDS_HEADER_STRUCTURE();
                    dds.ReadStruct(ref br);

                    int tempsize = dds.ddsd.width * dds.ddsd.height;
                    switch (dds.ddsd.ddfPixelFormat.FourCC)
                    {
                        case "DXT1":
                            tempsize /= 2;
                            break;
                        case "DXT2":
                        case "DXT3":
                        case "DXT4":
                        case "DXT5":

                            // tempsize /= 1;
                            break;

                            // for non-compressed
                        default:
                            tempsize *= dds.ddsd.ddfPixelFormat.RGBBitCount >> 3;
                            break;
                    }

                    int widthPad = 0;
                    if (dds.ddsd.width % 16 != 0)
                        widthPad = 16 - dds.ddsd.width % 16;

                    int byteStep = dds.ddsd.ddfPixelFormat.RGBBitCount / 8;
                    byte[] guh = new byte[tempsize + (dds.ddsd.height * widthPad * byteStep)];
                    if (widthPad == 0)
                    {
                        br.BaseStream.Read(guh, 0, guh.Length);
                    }
                    else
                    {
                        // Change data to include padding
                        for (int h = 0; h < dds.ddsd.height; h++)
                        {
                            br.BaseStream.Read(
                                guh, h * (dds.ddsd.width + widthPad) * byteStep, dds.ddsd.width * byteStep);
                        }
                    }

                    br.Close();
                    fs.Close();

                    

                    int tagNum =
                        map.Functions.ForMeta.FindMetaByID(
                            currentLevels[lbMapListing.SelectedIndex].Preview_Image_Ident);
                    if (tagNum != -1)
                    {
                        map.SelectedMeta = Map.GetMetaFromTagIndex(tagNum, map, false, false);
                    }

                    #region Confirm 224x207 A8R8G8B8 file and save to CurrentLevel[].ScreenShot & Cur...[].ScreenShotRaw

                    ParsedBitmap.BitmapFormat bmf = DDS.getBitmapFormat(dds);
                    if ((bmf == ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8) && (dds.ddsd.width + widthPad == 224) &&
                        (dds.ddsd.height == 207))
                    {
                        currentLevels[lbMapListing.SelectedIndex].screenShotRaw.MS.Position = 0;
                        currentLevels[lbMapListing.SelectedIndex].screenShotRaw.MS.Write(
                            guh, 0, (int)Math.Min(map.SelectedMeta.raw.rawChunks[0].MS.Length, guh.Length));
                        currentLevels[lbMapListing.SelectedIndex].screenShotRaw.size =
                            (int)currentLevels[lbMapListing.SelectedIndex].screenShotRaw.MS.Length;
                        currentLevels[lbMapListing.SelectedIndex].screenShot = ParsedBitmap.DecodeBitm(
                            guh, 
                            dds.ddsd.height, 
                            dds.ddsd.width, 
                            dds.ddsd.depth, 
                            dds.ddsd.ddfPixelFormat.RGBBitCount, 
                            ParsedBitmap.BitmapType.BITM_TYPE_2D, 
                            bmf, 
                            false, 
                            null, 
                            -1, 
                            -1);
                        pbMapBitmap.Image = currentLevels[lbMapListing.SelectedIndex].screenShot;
                    }
                    else
                    {
                        MessageBox.Show("Bitmap must be a 224x207 A8R8G8B8 file");
                    }

                    #endregion
                }
                else if (getImage.FileName.EndsWith("map", StringComparison.OrdinalIgnoreCase))
                {
                    this.Enabled = false;
                    Map tempmap = Map.LoadFromFile(getImage.FileName);
                    this.Enabled = true;
                    if (tempmap == null)
                    {
                        MessageBox.Show("Unable to load image from map: " + getImage.FileName);
                        return;
                    }

                    // Load Map Picture as raw data (for transfer) and Bitmap (for display)
                    tempmap.OpenMap(MapTypes.Internal);
                    Meta m = MapForm.GetMapBitmapMeta(tempmap);

                    //////////////////////////////////
                    // Backwash for example has no internal picture, so it errors.
                    //////////////////////////////////
                    if (m != null)
                    {
                        currentLevels[lbMapListing.SelectedIndex].screenShotRaw = m.raw.rawChunks[0];
                        ParsedBitmap pm = new ParsedBitmap(ref m, tempmap);
                        
                        currentLevels[lbMapListing.SelectedIndex].screenShot = pm.FindChunkAndDecode(
                            0, 0, 0, ref m, tempmap, 0, 0);
                    }
                    else
                    {
                        // No map listing found
                    }

                    tempmap.CloseMap();
                    pbMapBitmap.Image = currentLevels[lbMapListing.SelectedIndex].screenShot;
                }
            }
        }

        /// <summary>
        /// The btn remove map_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnRemoveMap_Click(object sender, EventArgs e)
        {
            // Remove the map and insert a new empty map to the last listing to make sure that we keep the same # of maps total for when we rewrite
            currentLevels.RemoveAt(lbMapListing.SelectedIndex);
            currentLevels.Add(new mapInfo(string.Empty, string.Empty, -1));

            int temp = lbMapListing.SelectedIndex;
            if (lbMapListing.SelectedIndex > 0)
            {
                lbMapListing.Items.RemoveAt(lbMapListing.SelectedIndex);
                if (temp >= lbMapListing.Items.Count)
                {
                    temp--;
                }

                lbMapListing.SelectedIndex = temp;
            }
            else
            {
                lbMapListing.Items.RemoveAt(0);
                if (lbMapListing.Items.Count > 0)
                {
                    lbMapListing.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// The btn save image_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImage = new SaveFileDialog();
            saveImage.InitialDirectory = Prefs.pathBitmapsFolder;
            saveImage.DefaultExt = "dds";
            saveImage.Filter = "DDS files (*.dds)|*.dds";
            if (saveImage.ShowDialog() == DialogResult.OK)
            {
                Prefs.pathBitmapsFolder = saveImage.FileName.Substring(0, saveImage.FileName.LastIndexOf('\\'));
                
                DDS.DDS_HEADER_STRUCTURE dds = new DDS.DDS_HEADER_STRUCTURE();
                ParsedBitmap.BitmapInfo bi = new ParsedBitmap.BitmapInfo(
                    ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8, false);
                bi.width = 224;
                bi.height = 207;
                bi.depth = 1;
                bi.bitsPerPixel = 32;
                dds.generate(ref bi);
                FileStream fs = new FileStream(saveImage.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                dds.WriteStruct(ref bw);
                bw.Write(
                    currentLevels[lbMapListing.SelectedIndex].screenShotRaw.MS.ToArray(), 
                    0, 
                    (int)currentLevels[lbMapListing.SelectedIndex].screenShotRaw.MS.Length);
                bw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// The btn save main menu_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSaveMainMenu_Click(object sender, EventArgs e)
        {
            // set Campaign Level sorts
            int sort = 0;
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                if (campaignLevels[i].MapID != -1)
                {
                    campaignLevels[i].Sort_Order = sort++ * 10 + 10;
                }
                else
                {
                    campaignLevels[i].Sort_Order = 0;
                }
            }

            // Campaign Level re-sort to original order
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                for (int j = 0; j < campaignLevels.Count; j++)
                {
                    if (campaignLevels[j].originalNumber == i)
                    {
                        mapInfo temp = campaignLevels[j];
                        campaignLevels[j] = campaignLevels[i];
                        campaignLevels[i] = temp;
                        break;
                    }
                }
            }

            // saveMainMenuData();

            // set MP Level sorts
            sort = 0;
            for (int i = 0; i < MPLevels.Count; i++)
            {
                if (MPLevels[i].MapID != -1)
                {
                    MPLevels[i].Sort_Order = sort++ * 10 + 10;
                }
                else
                {
                    MPLevels[i].Sort_Order = 0;
                }
            }

            // MP Level re-sort to original order
            for (int i = 0; i < MPLevels.Count; i++)
            {
                for (int j = 0; j < MPLevels.Count; j++)
                {
                    if (MPLevels[j].originalNumber == i && MPLevels[j].mapID != -1)
                    {
                        mapInfo temp = MPLevels[j];
                        MPLevels[j] = MPLevels[i];
                        MPLevels[i] = temp;
                        break;
                    }
                }
            }

            saveMainMenuData();
            this.Dispose();
        }

        /// <summary>
        /// The btn sort alphabetically_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSortAlphabetically_Click(object sender, EventArgs e)
        {
            lbMapListing.Sorted = true;
            lbMapListing.Sorted = false;
            for (int i = 0; i < currentLevels.Count; i++)
            {
                // skip sorting of all unused maps
                if (currentLevels[i].mapID == -1)
                {
                    continue;
                }

                for (int j = 0; j < i; j++)
                {
                    if ((currentLevels[i].mapID != -1 && currentLevels[j].mapID == -1) ||
                        (string.Compare(currentLevels[i].Name, currentLevels[j].Name, true) < 0))
                    {
                        currentLevels.Insert(j, currentLevels[i]);
                        currentLevels.RemoveAt(i + 1);
                    }
                }
            }
        }

        /// <summary>
        /// The cb language_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadMapNames(cbLanguage.SelectedIndex, true);
        }

        /// <summary>
        /// The editable box_ leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void editableBox_Leave(object sender, EventArgs e)
        {
            currentLevels[lbMapListing.SelectedIndex].Scenario_Path = tbScenarioName.Text;
            currentLevels[lbMapListing.SelectedIndex].Names[cbLanguage.SelectedIndex] = tbMapName.Text;
            currentLevels[lbMapListing.SelectedIndex].Descriptions[cbLanguage.SelectedIndex] = tbMapDescription.Text;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_None = (byte)nbNone.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_CTF = (byte)nbCTF.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Slayer = (byte)nbSlayer.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Oddball = (byte)nbOddball.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_KOTH = (byte)nbKOTH.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Race = (byte)nbRace.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Headhunter = (byte)nbHeadhunter.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Juggernaught = (byte)nbJuggernaught.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Territories = (byte)nbTerritories.Value;
            currentLevels[lbMapListing.SelectedIndex].Max_Teams_Assault = (byte)nbAssault.Value;
            lbMapListing.Items[lbMapListing.SelectedIndex] = tbMapName.Text;
        }

        /// <summary>
        /// The item insert.
        /// </summary>
        /// <param name="lb">The lb.</param>
        /// <param name="itemToInsert">The item to insert.</param>
        /// <param name="position">The position.</param>
        /// <remarks></remarks>
        private void itemInsert(ListBox lb, object itemToInsert, Point position)
        {
            int totalHeight = 0;
            int itemNum = -1;
            do
            {
                totalHeight += lb.GetItemHeight(++itemNum);
            }
            while (position.Y > totalHeight && itemNum < lb.Items.Count - 1);
            itemNum += lbMapListing.TopIndex;
            if (itemNum >= lb.Items.Count)
            {
                itemNum = lb.Items.Count - 1;
            }

            if (lb.Items[itemNum] != itemToInsert)
            {
                int oldPos = lb.Items.IndexOf(itemToInsert);
                if (oldPos < itemNum)
                {
                    lb.Items.Insert(itemNum + 1, itemToInsert);
                    lb.Items.RemoveAt(oldPos);
                    currentLevels.Insert(itemNum + 1, currentLevels[oldPos]);
                    currentLevels.RemoveAt(oldPos);
                }
                else
                {
                    lb.Items.Insert(itemNum, itemToInsert);
                    lb.Items.RemoveAt(oldPos + 1);
                    currentLevels.Insert(itemNum, currentLevels[oldPos]);
                    currentLevels.RemoveAt(oldPos + 1);
                }

                lb.SelectedItem = itemToInsert;
            }
        }

        /// <summary>
        /// The lb map listing_ mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lbMapListing_MouseMove(object sender, MouseEventArgs e)
        {
            if (lbMapListing.SelectedIndex == -1)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (currentDrag == null)
                {
                    lbMapListing_SelectedIndexChanged(sender, null);
                    currentDrag = currentLevels[lbMapListing.SelectedIndex];
                }

                itemInsert((ListBox)sender, ((ListBox)sender).SelectedItem, e.Location);
            }
        }

        /// <summary>
        /// The lb map listing_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lbMapListing_MouseUp(object sender, MouseEventArgs e)
        {
            currentDrag = null;
        }

        /// <summary>
        /// The lb map listing_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lbMapListing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbMapListing.SelectedIndex == -1 || currentDrag != null)
            {
                return;
            }

            mapInfo temp = currentLevels[lbMapListing.SelectedIndex];
            tbScenarioName.Text = temp.Scenario_Path;
            tbMapName.Text = temp.Names[cbLanguage.SelectedIndex];
            tbMapDescription.Text = temp.Descriptions[cbLanguage.SelectedIndex];
            pbMapBitmap.Image = temp.screenShot;
            if (temp.campaignNumber == -1)
            {
                nbNone.Value = temp.Max_Teams_None;
                nbCTF.Value = temp.Max_Teams_CTF;
                nbSlayer.Value = temp.Max_Teams_Slayer;
                nbOddball.Value = temp.Max_Teams_Oddball;
                nbKOTH.Value = temp.Max_Teams_KOTH;
                nbRace.Value = temp.Max_Teams_Race;
                nbHeadhunter.Value = temp.Max_Teams_Race;
                nbJuggernaught.Value = temp.Max_Teams_Juggernaught;
                nbTerritories.Value = temp.Max_Teams_Territories;
                nbAssault.Value = temp.Max_Teams_Assault;
            }
            else
            {
                nbNone.Value = temp.campaignNumber;
            }
        }

        /// <summary>
        /// The load bitmaps.
        /// </summary>
        /// <remarks></remarks>
        private void loadBitmaps()
        {
            Map thisMap = map;

            // Load Map Bitmap & RawChunk for Campaign
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                int metaNum = map.Functions.ForMeta.FindMetaByID(campaignLevels[i].Preview_Image_Ident);
                if (metaNum != -1)
                {
                    Meta m = Map.GetMetaFromTagIndex(metaNum, map, false, false);

                    campaignLevels[i].screenShotRaw = m.raw.rawChunks[0];
                    campaignLevels[i].screenShotOffset = m.raw.rawChunks[0].offset;
                    ParsedBitmap pm = new ParsedBitmap(ref m, map);
                    
                    campaignLevels[i].screenShot = pm.FindChunkAndDecode(0, 0, 0, ref m, map, 0, 0);
                }
            }

            // Load Map Bitmap & RawChunk for MP
            for (int i = 0; i < MPLevels.Count; i++)
            {
                int metaNum = map.Functions.ForMeta.FindMetaByID(MPLevels[i].Preview_Image_Ident);
                if (metaNum != -1)
                {
                    Meta m = Map.GetMetaFromTagIndex(metaNum, map, false, false);

                    MPLevels[i].screenShotRaw = m.raw.rawChunks[0];
                    MPLevels[i].screenShotOffset = m.raw.rawChunks[0].offset;
                    ParsedBitmap pm = new ParsedBitmap(ref m, map);
                    
                    MPLevels[i].screenShot = pm.FindChunkAndDecode(0, 0, 0, ref m, map, 0, 0);
                }

                // Checks the offsets to see if they are shared.
                // Allows us to know if we need to internalize a new bitmap when adding maps
                for (int j = 0; j < i; j++)
                {
                    if (MPLevels[i].screenShotOffset == MPLevels[j].screenShotOffset)
                    {
                        if (MPLevels[i].mapID != -1 && MPLevels[j].mapID == -1)
                        {
                            MPLevels[j].screenShotOriginal = false;
                        }
                        else if (MPLevels[i].mapID == -1 && MPLevels[j].mapID != -1)
                        {
                            MPLevels[i].screenShotOriginal = false;
                        }
                        else
                        {
                            MPLevels[i].screenShotOriginal = false;
                            MPLevels[j].screenShotOriginal = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The load main menu data.
        /// </summary>
        /// <remarks></remarks>
        private void loadMainMenuData()
        {
            // Load MATG tag (globals\globals), should always be at location 0
            Meta meta = Map.GetMetaFromTagIndex(0, map, false, false);
            br = new BinaryReader(meta.MS);
            int matgOffset = meta.offset;

            // Runtime level Data
            br.BaseStream.Position = 368;
            int RuntimeLevelCount = br.ReadInt32(); // Should always be 1
            int RuntimeLevelOffset = br.ReadInt32() - map.SecondaryMagic - matgOffset;

            

            br.BaseStream.Position = RuntimeLevelOffset + 0;
            int CampaignIDCount = br.ReadInt32();
            int CampaignIDOffset = br.ReadInt32() - map.SecondaryMagic - matgOffset;
            for (int i = 0; i < CampaignIDCount; i++)
            {
                br.BaseStream.Position = CampaignIDOffset + i * 264;
                campaignScenarios cs = new campaignScenarios(br, matgOffset);
                campScenarios.Add(cs);
            }

            

            // UI Level Data, Size 24
            br.BaseStream.Position = 376;
            int UILevelCount = br.ReadInt32(); // Should always be 1
            int UILevelOffset = br.ReadInt32() - map.SecondaryMagic - matgOffset;

            #region Campaign Levels, Size 2896

            br.BaseStream.Position = UILevelOffset + 8;
            int CampaignLevelCount = br.ReadInt32();
            int CampaignLevelOffset = br.ReadInt32() - map.SecondaryMagic - matgOffset;

            for (int i = 0; i < CampaignLevelCount; i++)
            {
                br.BaseStream.Position = CampaignLevelOffset + i * 2896;
                mapInfo cLevel = new mapInfo(br, matgOffset, campScenarios);
                cLevel.originalNumber = i;
                for (int j = 0; j <= i; j++)
                {
                    if (j == i || cLevel.MapID == -1)
                    {
                        campaignLevels.Add(cLevel);
                        break;
                    }
                    else if (cLevel.Sort_Order < campaignLevels[j].Sort_Order || cLevel.MapID == -1)
                    {
                        campaignLevels.Insert(j, cLevel);
                        break;
                    }
                }
            }

            #endregion

            #region Multiplayer Levels, Size 3172

            br.BaseStream.Position = UILevelOffset + 16;
            int MPLevelCount = br.ReadInt32();
            int MPLevelOffset = br.ReadInt32() - map.SecondaryMagic - matgOffset;

            for (int i = 0; i < MPLevelCount; i++)
            {
                br.BaseStream.Position = MPLevelOffset + i * 3172;
                mapInfo mLevel = new mapInfo(br, matgOffset);
                mLevel.originalNumber = i;

                for (int j = 0; j <= i; j++)
                {
                    if (j == i || mLevel.MapID == -1)
                    {
                        MPLevels.Add(mLevel);
                        break;
                    }
                    else if (mLevel.Sort_Order < MPLevels[j].Sort_Order || MPLevels[j].MapID == -1)
                    {
                        MPLevels.Insert(j, mLevel);
                        break;
                    }
                }
            }

            #endregion

            br.Close();
        }

        /// <summary>
        /// The load map names.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="keepSelectedIndex">The keep selected index.</param>
        /// <remarks></remarks>
        private void loadMapNames(int language, bool keepSelectedIndex)
        {
            int temp = lbMapListing.SelectedIndex;
            lbMapListing.Items.Clear();
            for (int i = 0; i < currentLevels.Count; i++)
            {
                currentLevels[i].CurrentLanguage = language;
                if (currentLevels[i].mapID != -1)
                {
                    if (currentLevels[i].Name != string.Empty)
                    {
                        lbMapListing.Items.Add(currentLevels[i].Name);
                    }
                    else
                    {
                        lbMapListing.Items.Add("-= Nameless =-");
                    }
                }
            }

            try
            {
                if (keepSelectedIndex && temp != -1)
                {
                    lbMapListing.SelectedIndex = temp;
                }
                else
                {
                    lbMapListing.SelectedIndex = 0;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The rb campaign_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void rbCampaign_CheckedChanged(object sender, EventArgs e)
        {
            if (currentLevels == campaignLevels)
            {
                return;
            }

            currentLevels = campaignLevels;
            label1.Visible = false;
            lblNone.Text = "Campaign #";
            lblCTF.Visible = false;
            lblSlayer.Visible = false;
            lblOddball.Visible = false;
            lblKOTH.Visible = false;
            lblRace.Visible = false;
            lblHeadhunter.Visible = false;
            lblJuggernaught.Visible = false;
            lblTerritories.Visible = false;
            lblAssault.Visible = false;

            nbCTF.Visible = false;
            nbSlayer.Visible = false;
            nbOddball.Visible = false;
            nbKOTH.Visible = false;
            nbRace.Visible = false;
            nbHeadhunter.Visible = false;
            nbJuggernaught.Visible = false;
            nbTerritories.Visible = false;
            nbAssault.Visible = false;

            int temp = lbMapListing.SelectedIndex;
            loadMapNames(cbLanguage.SelectedIndex, false);
            lbMapListing.SelectedIndex = oldSelectedIndex;
            oldSelectedIndex = temp;
        }

        /// <summary>
        /// The rb multiplayer_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void rbMultiplayer_CheckedChanged(object sender, EventArgs e)
        {
            if (currentLevels == MPLevels)
            {
                return;
            }

            currentLevels = MPLevels;
            label1.Visible = true;
            lblNone.Text = "None";
            lblCTF.Visible = true;
            lblSlayer.Visible = true;
            lblOddball.Visible = true;
            lblKOTH.Visible = true;
            lblRace.Visible = true;
            lblHeadhunter.Visible = true;
            lblJuggernaught.Visible = true;
            lblTerritories.Visible = true;
            lblAssault.Visible = true;

            nbCTF.Visible = true;
            nbSlayer.Visible = true;
            nbOddball.Visible = true;
            nbKOTH.Visible = true;
            nbRace.Visible = true;
            nbHeadhunter.Visible = true;
            nbJuggernaught.Visible = true;
            nbTerritories.Visible = true;
            nbAssault.Visible = true;

            int temp = lbMapListing.SelectedIndex;
            loadMapNames(cbLanguage.SelectedIndex, false);
            lbMapListing.SelectedIndex = oldSelectedIndex;
            oldSelectedIndex = temp;
        }

        /// <summary>
        /// The save bitmaps.
        /// </summary>
        /// <remarks></remarks>
        private void saveBitmaps()
        {
            Map thisMap = map;

            // Save Map Bitmap & RawChunk for Campaign
            /*
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                for (int j = 0; j < map.IndexHeader.metaCount; j++)
                {
                    char[] temp = (char[])campaignLevels[i].Preview_Image_Tag.Clone();
                    Array.Reverse(temp);
                    if (thisMap.MetaInfo.TagType[j] == new string(temp) &&
                        thisMap.MetaInfo.Ident[j] == campaignLevels[i].Preview_Image_Ident)
                    {
                        MMMap.loadmeta(j);
                        Meta m = thisMap.SelectedMeta;
                        campaignLevels[i].screenShotRaw = m.raw.rawChunks[0];
                        campaignLevels[i].screenShotOffset = m.raw.rawChunks[0].offset;
                        Raw.ParsedBitmap pm = new Raw.ParsedBitmap(ref m, map);
                        campaignLevels[i].screenShot = pm.FindChunkAndDecode(0, 0, 0, ref m, map, 0, 0);

                        break;
                    }
                }
            }
            */
            // Save Map Bitmap & RawChunk for MP
            for (int i = 0; i < MPLevels.Count; i++)
            {
                // Only save bitmaps for actual levels
                if (MPLevels[i].mapID == -1)
                {
                    continue;
                }

                int TagIndex = map.Functions.ForMeta.FindMetaByID(MPLevels[i].Preview_Image_Ident);
                if (TagIndex != -1)
                {
                    Meta m = Map.GetMetaFromTagIndex(TagIndex, map, false, false);

                    m.raw.rawChunks[0].MS = MPLevels[i].screenShotRaw.MS;

                    // If we are using a shared image, insert a new
                    if (!MPLevels[i].screenShotOriginal)
                    {
                        RawDataContainer rdc = new RawDataContainer();
                        rdc.rawChunks.Add(MPLevels[i].screenShotRaw);
                        ParsedBitmap.bitmapInternalizeRaw(ref rdc, thisMap);
                        MPLevels[i].screenShotOffset = MPLevels[i].screenShotRaw.offset;
                        MPLevels[i].screenShotOriginal = true;
                    }

                    map.OpenMap(MapTypes.Internal);

                    // Write Raw Data to map file
                    map.BW.BaseStream.Position = MPLevels[i].screenShotOffset;
                    map.BW.Write(
                        m.raw.rawChunks[0].MS.ToArray(), 
                        0, 
                        (int)Math.Min(m.raw.rawChunks[0].size, m.raw.rawChunks[0].MS.Length));
                    string s = map.FileNames.Name[TagIndex];

                    // Write offset to map file
                    // Offset 68 = Bitmap Data reflexive
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 68;
                    int tempc = map.BR.ReadInt32();
                    int tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                    // int offsetToOffset = tempr + 28 - map.MetaInfo.Offset[TagIndex];
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] +
                                                 MPLevels[i].screenShotRaw.pointerMetaOffset;
                    map.BW.Write(MPLevels[i].screenShotOffset);
                    map.CloseMap();
                }
            }
        }

        /// <summary>
        /// The save campaign data.
        /// </summary>
        /// <remarks></remarks>
        private void saveCampaignData()
        {
            Map thisMap = map;

            int matgOffset = thisMap.SelectedMeta.offset;
            br = new BinaryReader(thisMap.SelectedMeta.MS);
            bw = new BinaryWriter(thisMap.SelectedMeta.MS);

            // Runtime level Data
            br.BaseStream.Position = 368;
            int RuntimeLevelCount = br.ReadInt32(); // Should always be 1
            int RuntimeLevelOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;

            

            br.BaseStream.Position = RuntimeLevelOffset + 0;
            bw.Write(campScenarios.Count);
            int CampaignIDOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                br.BaseStream.Position = CampaignIDOffset + i * 264;

                ////////
                // Need to write code for saving campaign scenarios
                ////////

                // campScenarios[i].write();
            }

            

            // UI Level Data, Size 24
            br.BaseStream.Position = 376;
            int UILevelCount = br.ReadInt32(); // Should always be 1
            int UILevelOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;

            #region Campaign Levels, Size 2896

            br.BaseStream.Position = UILevelOffset + 8;
            bw.Write(campaignLevels.Count);
            int CampaignLevelOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;

            int nextUp = -1;
            for (int i = 0; i < campaignLevels.Count; i++)
            {
                // Puts listings back into original position
                // If original listing is gone, inserts a new listing in that spot
                // or if neither, nulls it out
                for (int j = 0; j <= campaignLevels.Count; j++)
                {
                    if (j == campaignLevels.Count)
                    {
                        if (nextUp != -1)
                        {
                            br.BaseStream.Position = CampaignLevelOffset + i * 2896;
                            campaignLevels[nextUp].write(i, bw, matgOffset, campScenarios);
                        }
                        else
                        {
                            // Null it out
                        }
                    }
                    else if (i == campaignLevels[j].originalNumber || (i == -1 && campaignLevels[j].mapID == -1))
                    {
                        br.BaseStream.Position = CampaignLevelOffset + i * 2896;
                        campaignLevels[j].write(i, bw, matgOffset, campScenarios);
                        break;
                    }
                    else if (campaignLevels[j].originalNumber == -1 && nextUp == -1)
                    {
                        nextUp = j;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// The save mp data.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        private void saveMPData(Meta meta)
        {
            Map thisMap = map;
            thisMap.SelectedMeta = meta;

            int matgOffset = thisMap.SelectedMeta.offset;
            br = new BinaryReader(thisMap.SelectedMeta.MS);
            bw = new BinaryWriter(thisMap.SelectedMeta.MS);

            // UI Level Data, Size 24
            br.BaseStream.Position = 376;
            int UILevelCount = br.ReadInt32(); // Should always be 1
            int UILevelOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;

            

            br.BaseStream.Position = UILevelOffset + 16;
            bw.Write(MPLevels.Count);
            int MPLevelOffset = br.ReadInt32() - thisMap.SecondaryMagic - matgOffset;

            for (int i = 0; i < MPLevels.Count; i++)
            {
                // Puts listings back into original position
                // If original listing is gone, inserts a new listing in that spot
                // or if neither, nulls it out
                int nextUp = -1;
                for (int j = 0; j < MPLevels.Count; j++)
                {
                    // If the level listing was originally at this spot and not empty, it takes precedence
                    if (i == MPLevels[j].originalNumber && MPLevels[j].MapID != -1)
                    {
                        nextUp = j;
                        break;
                    }
                        
                        // If we added a map, place it in an opening
                    else if (MPLevels[j].originalNumber == -1 && nextUp == -1)
                    {
                        nextUp = j;
                    }
                }

                br.BaseStream.Position = MPLevelOffset + i * 3172;
                if (nextUp != -1)
                {
                    MPLevels[nextUp].originalNumber = i;
                    MPLevels[nextUp].Preview_Image_Tag = "mtib".ToCharArray();
                    int tagNum = thisMap.Functions.ForMeta.FindByNameAndTagType(
                        "bitm", "ui\\code_global_bitmaps\\multiplayer" + i);
                    MPLevels[nextUp].Preview_Image_Ident = thisMap.MetaInfo.Ident[tagNum];
                    MPLevels[nextUp].write(i, bw, matgOffset);
                }
                else
                {
                    // Null out level listing
                    mapInfo mi = new mapInfo(string.Empty, string.Empty, -1);
                    mi.Preview_Image_Tag = "mtib".ToCharArray();
                    int tagNum = thisMap.Functions.ForMeta.FindByNameAndTagType("bitm", "ui\\code_global_bitmaps\\multiplayer" + i.ToString());
                    mi.Preview_Image_Ident = thisMap.MetaInfo.Ident[tagNum];
                    mi.write(i, bw, matgOffset);
                }
            }

            
        }

        /// <summary>
        /// The save main menu data.
        /// </summary>
        /// <remarks></remarks>
        private void saveMainMenuData()
        {
            // Load MATG tag (globals\globals), should always be at location 0
            saveMPData(Map.GetMetaFromTagIndex(0, map, false, false));

            // saveCampaignData();
            map.OpenMap(MapTypes.Internal);
            map.BW.BaseStream.Position = map.SelectedMeta.offset;
            map.BW.BaseStream.Write(map.SelectedMeta.MS.ToArray(), 0, map.SelectedMeta.size);
            map.CloseMap();
            saveBitmaps();
            map.Sign();
        }

        #endregion

        /// <summary>
        /// The id info.
        /// </summary>
        /// <remarks></remarks>
        private struct IDInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The campaign.
            /// </summary>
            private bool campaign;

            /// <summary>
            /// The id.
            /// </summary>
            private int id;

            /// <summary>
            /// The name.
            /// </summary>
            private string name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IDInfo"/> struct.
            /// </summary>
            /// <param name="Campaign">if set to <c>true</c> [campaign].</param>
            /// <param name="Name">The name.</param>
            /// <param name="ID">The ID.</param>
            /// <remarks></remarks>
            public IDInfo(bool Campaign, string Name, int ID)
            {
                this.campaign = Campaign;
                this.name = Name;
                this.id = ID;
            }

            #endregion
        }

        /// <summary>
        /// The campaign scenarios.
        /// </summary>
        /// <remarks></remarks>
        public class campaignScenarios
        {
            #region Constants and Fields

            /// <summary>
            /// The campaign num.
            /// </summary>
            public int campaignNum;

            /// <summary>
            /// The map id.
            /// </summary>
            public int mapID;

            /// <summary>
            /// The scenario name.
            /// </summary>
            public string scenarioName;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="campaignScenarios"/> class.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <param name="tagOffset">The tag offset.</param>
            /// <remarks></remarks>
            public campaignScenarios(BinaryReader br, int tagOffset)
            {
                campaignNum = br.ReadInt32();
                mapID = br.ReadInt32();
                scenarioName = new string(br.ReadChars(256)).Trim('\0');
            }

            #endregion
        }

        /// <summary>
        /// The gametypes.
        /// </summary>
        /// <remarks></remarks>
        public class gametypes
        {
            #region Constants and Fields

            /// <summary>
            /// The game types.
            /// </summary>
            public teams[] gameTypes;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="gametypes"/> class.
            /// </summary>
            /// <remarks></remarks>
            public gametypes()
            {
                this.gameTypes = new teams[10];
                this.gameTypes[0] = new teams(0); // None
                this.gameTypes[1] = new teams(2); // CTF
                this.gameTypes[2] = new teams(8); // Slayer
                this.gameTypes[3] = new teams(8); // Oddball
                this.gameTypes[4] = new teams(8); // KOTH
                this.gameTypes[5] = new teams(8); // Race
                this.gameTypes[6] = new teams(8); // Headhunter
                this.gameTypes[7] = new teams(8); // Juggernaught
                this.gameTypes[8] = new teams(8); // Territories
                this.gameTypes[9] = new teams(2); // Assault
            }

            #endregion

            #region Enums

            /// <summary>
            /// The game types names.
            /// </summary>
            /// <remarks></remarks>
            public enum gameTypesNames
            {
                /// <summary>
                /// The none.
                /// </summary>
                None = 0, 

                /// <summary>
                /// The ctf.
                /// </summary>
                CTF = 1, 

                /// <summary>
                /// The slayer.
                /// </summary>
                Slayer = 2, 

                /// <summary>
                /// The oddball.
                /// </summary>
                Oddball = 3, 

                /// <summary>
                /// The koth.
                /// </summary>
                KOTH = 4, 

                /// <summary>
                /// The race.
                /// </summary>
                Race = 5, 

                /// <summary>
                /// The headhunter.
                /// </summary>
                Headhunter = 6, 

                /// <summary>
                /// The juggernaught.
                /// </summary>
                Juggernaught = 7, 

                /// <summary>
                /// The territories.
                /// </summary>
                Territories = 8, 

                /// <summary>
                /// The assault.
                /// </summary>
                Assault = 9
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The save.
            /// </summary>
            /// <param name="mi">The mi.</param>
            /// <remarks></remarks>
            public void Save(mapInfo mi)
            {
                mi.Max_Teams_None = this.gameTypes[(int)gameTypesNames.None].GetTotal();
                mi.Max_Teams_CTF = this.gameTypes[(int)gameTypesNames.CTF].GetTotal();
                mi.Max_Teams_Slayer = this.gameTypes[(int)gameTypesNames.Slayer].GetTotal();
                mi.Max_Teams_Oddball = this.gameTypes[(int)gameTypesNames.Oddball].GetTotal();
                mi.Max_Teams_KOTH = this.gameTypes[(int)gameTypesNames.KOTH].GetTotal();
                mi.Max_Teams_Race = this.gameTypes[(int)gameTypesNames.Race].GetTotal();
                mi.Max_Teams_Headhunter = this.gameTypes[(int)gameTypesNames.Headhunter].GetTotal();
                mi.Max_Teams_Juggernaught = this.gameTypes[(int)gameTypesNames.Juggernaught].GetTotal();
                mi.Max_Teams_Territories = this.gameTypes[(int)gameTypesNames.Territories].GetTotal();
                mi.Max_Teams_Assault = this.gameTypes[(int)gameTypesNames.Assault].GetTotal();
            }

            #endregion
        }

        /// <summary>
        /// The map info.
        /// </summary>
        /// <remarks></remarks>
        public class mapInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The descriptions.
            /// </summary>
            public string[] Descriptions;

            /// <summary>
            /// The flags.
            /// </summary>
            public int Flags; // bitmask32 name="Flags" offset="3152" <option name="Unlockable" value="0" />

            /// <summary>
            /// The max_ teams_ assault.
            /// </summary>
            public byte Max_Teams_Assault; // offset="3165"

            /// <summary>
            /// The max_ teams_ ctf.
            /// </summary>
            public byte Max_Teams_CTF; // offset="3157"

            /// <summary>
            /// The max_ teams_ headhunter.
            /// </summary>
            public byte Max_Teams_Headhunter; // offset="3162"

            /// <summary>
            /// The max_ teams_ juggernaught.
            /// </summary>
            public byte Max_Teams_Juggernaught; // offset="3163"

            /// <summary>
            /// The max_ teams_ koth.
            /// </summary>
            public byte Max_Teams_KOTH; // offset="3160"

            /// <summary>
            /// The max_ teams_ none.
            /// </summary>
            public byte Max_Teams_None; // offset="3156"

            /// <summary>
            /// The max_ teams_ oddball.
            /// </summary>
            public byte Max_Teams_Oddball; // offset="3159"

            /// <summary>
            /// The max_ teams_ race.
            /// </summary>
            public byte Max_Teams_Race; // offset="3161"

            /// <summary>
            /// The max_ teams_ slayer.
            /// </summary>
            public byte Max_Teams_Slayer; // offset="3158"

            /// <summary>
            /// The max_ teams_ stub_10.
            /// </summary>
            public byte Max_Teams_Stub_10; // offset="3166"

            /// <summary>
            /// The max_ teams_ stub_11.
            /// </summary>
            public byte Max_Teams_Stub_11; // offset="3167"

            /// <summary>
            /// The max_ teams_ stub_12.
            /// </summary>
            public byte Max_Teams_Stub_12; // offset="3168"

            /// <summary>
            /// The max_ teams_ stub_13.
            /// </summary>
            public byte Max_Teams_Stub_13; // offset="3169"

            /// <summary>
            /// The max_ teams_ stub_14.
            /// </summary>
            public byte Max_Teams_Stub_14; // offset="3170"

            /// <summary>
            /// The max_ teams_ stub_15.
            /// </summary>
            public byte Max_Teams_Stub_15; // offset="3171"

            /// <summary>
            /// The max_ teams_ territories.
            /// </summary>
            public byte Max_Teams_Territories; // offset="3164"

            /// <summary>
            /// The names.
            /// </summary>
            public string[] Names;

            /// <summary>
            /// The preview_ image_ ident.
            /// </summary>
            public int Preview_Image_Ident; // offset 8

            /// <summary>
            /// The preview_ image_ tag.
            /// </summary>
            public char[] Preview_Image_Tag;

            /// <summary>
            /// The scenario_ path.
            /// </summary>
            public string Scenario_Path; // offset="2892"

            /// <summary>
            /// The sort_ order.
            /// </summary>
            public int Sort_Order; // offset="3148"

            /// <summary>
            /// The campaign number.
            /// </summary>
            public int campaignNumber;

            /// <summary>
            /// The map id.
            /// </summary>
            public int mapID;

            /// <summary>
            /// The original number.
            /// </summary>
            public int originalNumber;

            /// <summary>
            /// The screen shot.
            /// </summary>
            public Bitmap screenShot; // DDS converted to Bitmap

            /// <summary>
            /// The screen shot offset.
            /// </summary>
            public int screenShotOffset; // The mainmenu offset of the data

            /// <summary>
            /// The screen shot original.
            /// </summary>
            public bool screenShotOriginal; // True if not shared with any other screenshot

            /// <summary>
            /// The screen shot raw.
            /// </summary>
            public RawDataChunk screenShotRaw; // The raw DDS data

            /// <summary>
            /// The current language.
            /// </summary>
            private int currentLanguage;

            /// <summary>
            /// The name.
            /// </summary>
            private string name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="mapInfo"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="path">The path.</param>
            /// <param name="mapID">The map ID.</param>
            /// <remarks></remarks>
            public mapInfo(string name, string path, int mapID)
            {
                this.originalNumber = -1;

                // set default language as English
                currentLanguage = 0;
                this.name = name;
                for (int i = 0; i < this.name.Length; i++)
                {
                    if (i == 0)
                    {
                        this.name = char.ToUpper(name[0]) + name.Substring(1);
                    }
                    else if (this.name[i - 1] == ' ')
                    {
                        this.name = name.Substring(0, i) + char.ToUpper(name[i]) + name.Substring(i + 1);
                    }
                }

                Names = new string[9];
                for (int i = 0; i < Names.Length; i++)
                {
                    Names[i] = this.name;
                }

                Descriptions = new string[9];
                for (int i = 0; i < Descriptions.Length; i++)
                {
                    Descriptions[i] = string.Empty;
                }

                this.Scenario_Path = path;
                this.campaignNumber = -1;
                this.mapID = mapID;
                this.Sort_Order = mapID != -1 ? mapID : 0;
                screenShotOriginal = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="mapInfo"/> class.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <param name="tagOffset">The tag offset.</param>
            /// <remarks></remarks>
            public mapInfo(BinaryReader br, int tagOffset)
            {
                campaignNumber = -1;
                mapID = br.ReadInt32();
                Preview_Image_Tag = br.ReadChars(4);
                Preview_Image_Ident = br.ReadInt32();

                Encoding decode = Encoding.Unicode;
                Names = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = br.ReadBytes(64);
                    Names[i] = decode.GetString(tempbytes).Trim('\0');
                }

                Descriptions = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = br.ReadBytes(256);
                    Descriptions[i] = decode.GetString(tempbytes).Trim('\0');
                }

                Scenario_Path = new string(br.ReadChars(256));
                Sort_Order = br.ReadInt32();
                Flags = br.ReadInt32();
                Max_Teams_None = br.ReadByte();
                Max_Teams_CTF = br.ReadByte();
                Max_Teams_Slayer = br.ReadByte();
                Max_Teams_Oddball = br.ReadByte();
                Max_Teams_KOTH = br.ReadByte();
                Max_Teams_Race = br.ReadByte();
                Max_Teams_Headhunter = br.ReadByte();
                Max_Teams_Juggernaught = br.ReadByte();
                Max_Teams_Territories = br.ReadByte();
                Max_Teams_Assault = br.ReadByte();
                Max_Teams_Stub_10 = br.ReadByte();
                Max_Teams_Stub_11 = br.ReadByte();
                Max_Teams_Stub_12 = br.ReadByte();
                Max_Teams_Stub_13 = br.ReadByte();
                Max_Teams_Stub_14 = br.ReadByte();
                Max_Teams_Stub_15 = br.ReadByte();

                // set default language as English
                currentLanguage = 0;
                this.name = Names[0];
                screenShotOriginal = true;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="mapInfo"/> class.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <param name="tagOffset">The tag offset.</param>
            /// <param name="CampaignScenarios">The campaign scenarios.</param>
            /// <remarks></remarks>
            public mapInfo(BinaryReader br, int tagOffset, List<campaignScenarios> CampaignScenarios)
            {
                campaignNumber = br.ReadInt32();
                mapID = br.ReadInt32();
                this.Sort_Order = mapID;
                for (int i = 0; i < CampaignScenarios.Count; i++)
                {
                    if (CampaignScenarios[i].campaignNum == this.campaignNumber &&
                        CampaignScenarios[i].mapID == this.mapID)
                    {
                        this.Scenario_Path = CampaignScenarios[i].scenarioName;
                        break;
                    }
                }

                Preview_Image_Tag = br.ReadChars(4);
                Preview_Image_Ident = br.ReadInt32();

                Encoding decode = Encoding.Unicode;
                Names = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = br.ReadBytes(64);
                    Names[i] = decode.GetString(tempbytes).Trim('\0');
                }

                Descriptions = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = br.ReadBytes(256);
                    Descriptions[i] = decode.GetString(tempbytes).Trim('\0');
                }

                // set default language as English
                currentLanguage = 0;
                this.name = Names[0];
                screenShotOriginal = true;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets CurrentLanguage.
            /// </summary>
            /// <value>The current language.</value>
            /// <remarks></remarks>
            public int CurrentLanguage
            {
                get
                {
                    return this.currentLanguage;
                }

                set
                {
                    this.currentLanguage = value;
                    this.name = this.Names[CurrentLanguage];
                }
            }

            /// <summary>
            /// Gets MapID.
            /// </summary>
            /// <remarks></remarks>
            public int MapID
            {
                get
                {
                    return this.mapID;
                }
            }

            /// <summary>
            /// Gets Name.
            /// </summary>
            /// <remarks></remarks>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            #endregion

            // For MP Levels
            #region Public Methods

            /// <summary>
            /// The write.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="bw">The bw.</param>
            /// <param name="mainoffset">The mainoffset.</param>
            /// <remarks></remarks>
            public void write(int position, BinaryWriter bw, int mainoffset)
            {
                bw.Write(mapID);
                bw.Write(Preview_Image_Tag);
                bw.Write(Preview_Image_Ident);

                Encoding decode = Encoding.Unicode;
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = decode.GetBytes(Names[i]);
                    Array.Resize(ref tempbytes, 64);
                    bw.Write(tempbytes); // 64
                }

                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = decode.GetBytes(Descriptions[i]);
                    Array.Resize(ref tempbytes, 256);
                    bw.Write(tempbytes); // 256
                }

                // Make sure Scenario Path is 256. So pad to 257, then remove to 256.
                bw.Write(Scenario_Path.PadRight(257, '\0').Remove(256).ToCharArray());
                bw.Write(Sort_Order);
                bw.Write(Flags);
                bw.Write(Max_Teams_None);
                bw.Write(Max_Teams_CTF);
                bw.Write(Max_Teams_Slayer);
                bw.Write(Max_Teams_Oddball);
                bw.Write(Max_Teams_KOTH);
                bw.Write(Max_Teams_Race);
                bw.Write(Max_Teams_Headhunter);
                bw.Write(Max_Teams_Juggernaught);
                bw.Write(Max_Teams_Territories);
                bw.Write(Max_Teams_Assault);
                bw.Write(Max_Teams_Stub_10);
                bw.Write(Max_Teams_Stub_11);
                bw.Write(Max_Teams_Stub_12);
                bw.Write(Max_Teams_Stub_13);
                bw.Write(Max_Teams_Stub_14);
                bw.Write(Max_Teams_Stub_15);
            }

            // For Campaign Levels
            /// <summary>
            /// The write.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="bw">The bw.</param>
            /// <param name="mainoffset">The mainoffset.</param>
            /// <param name="campScenarios">The camp scenarios.</param>
            /// <remarks></remarks>
            public void write(int position, BinaryWriter bw, int mainoffset, List<campaignScenarios> campScenarios)
            {
                bw.Write(campaignNumber);
                bw.Write(mapID);
                bw.Write(Preview_Image_Tag);
                bw.Write(Preview_Image_Ident);

                Encoding decode = Encoding.Unicode;
                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = decode.GetBytes(Names[i]);
                    Array.Resize(ref tempbytes, 64);
                    bw.Write(tempbytes); // 64
                }

                for (int i = 0; i < 9; i++)
                {
                    byte[] tempbytes = decode.GetBytes(Descriptions[i]);
                    Array.Resize(ref tempbytes, 256);
                    bw.Write(tempbytes); // 256
                }
            }

            #endregion
        }

        /// <summary>
        /// The teams.
        /// </summary>
        /// <remarks></remarks>
        public class teams
        {
            #region Constants and Fields

            /// <summary>
            /// The teams.
            /// </summary>
            public bool[] Teams;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="teams"/> class.
            /// </summary>
            /// <param name="numberOfTeams">The number of teams.</param>
            /// <remarks></remarks>
            public teams(int numberOfTeams)
            {
                this.Teams = new bool[9];
                this.Teams[(int)teamColors.neutral] = numberOfTeams == 0;
                this.Teams[(int)teamColors.red] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.blue] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.yellow] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.green] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.purple] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.orange] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.brown] = numberOfTeams-- > 0;
                this.Teams[(int)teamColors.pink] = numberOfTeams-- > 0;
            }

            #endregion

            #region Enums

            /// <summary>
            /// The team colors.
            /// </summary>
            /// <remarks></remarks>
            public enum teamColors
            {
                /// <summary>
                /// The red.
                /// </summary>
                red = 0, 

                /// <summary>
                /// The blue.
                /// </summary>
                blue = 1, 

                /// <summary>
                /// The yellow.
                /// </summary>
                yellow = 2, 

                /// <summary>
                /// The green.
                /// </summary>
                green = 3, 

                /// <summary>
                /// The purple.
                /// </summary>
                purple = 4, 

                /// <summary>
                /// The orange.
                /// </summary>
                orange = 5, 

                /// <summary>
                /// The brown.
                /// </summary>
                brown = 6, 

                /// <summary>
                /// The pink.
                /// </summary>
                pink = 7, 

                /// <summary>
                /// The neutral.
                /// </summary>
                neutral = 8
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The get total.
            /// </summary>
            /// <returns>The get total.</returns>
            /// <remarks></remarks>
            public byte GetTotal()
            {
                for (byte i = 0; i < this.Teams.Length; i++)
                {
                    if (this.Teams[i] != true)
                    {
                        return i;
                    }
                }

                return 0;
            }

            #endregion
        }

        // These are the IDs listed by bungie for the original maps
    }
}