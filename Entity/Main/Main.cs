// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="">
//   
// </copyright>
// <summary>
//   The form 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    using entity.MapForms;

    using Globals;

    using HaloMap;
    using HaloMap.Map;

    using Microsoft.Win32;

    /// <summary>
    /// The form 1.
    /// </summary>
    /// <remarks></remarks>
    partial class Form1 : Form
    {
        // Used for Entity updates
        #region Constants and Fields

        /// <summary>
        /// The update ftp name.
        /// </summary>
        private static string updateFTPName = "h2misc@acemods.org";

        /// <summary>
        /// The update ftp pass.
        /// </summary>
        private static string updateFTPPass = "y,Nk+,G/ur5i";

        /// <summary>
        /// The update ftp server.
        /// </summary>
        private static string updateFTPServer = "ftp://ftp.acemods.org/Troy Mac1ures Folder (Reaper approved)/";

        /// <summary>
        /// The update nfo file.
        /// </summary>
        private static string updateNFOFile = "update.xml";

        /// <summary>
        /// The update upd file.
        /// </summary>
        private static string updateUpdFile = "UpdateEnt.exe";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Form1()
        {
            InitializeComponent();

            // Hopefully this will fix the sorting issues on Vista (7 too?). XP looks fine.
            this.tsPanelTop.Controls.Clear();
            this.tsPanelTop.Controls.Add(this.mainMenu1);
            this.tsPanelTop.Controls.Add(this.menuStrip1);
            this.tsPanelTop.Controls.Add(this.menuStripDebug);

            // Locks the toolbars in place. Left unlocked at design time for easy editing
            this.tsPanelTop.Locked = true;
            updateMenuStripLock();

            this.Text = Assembly.GetExecutingAssembly().GetName().Name + "  " +
                        Assembly.GetExecutingAssembly().GetName().Version.Major + "." +
                        Assembly.GetExecutingAssembly().GetName().Version.Minor + "." +
                        Assembly.GetExecutingAssembly().GetName().Version.Build;

            // load preferences & add recently opened files to Menu List
            if (Prefs.Load())
            {
                this.AddRecentFilesToMenu();
            }
            else
            {
                MessageBox.Show(
                        "Error: \"" + Prefs.FilePath + "\" not found.\n" +
                        "Please setup default map directories first.");
                Settings settings = new Settings();
                settings.ShowDialog();
            }

            #region Try to load the selected skin here & show an error ONCE if it fails (not while debugging)
#if !DEBUG
            try
            {                
                StreamReader SettingsStreamReader = new StreamReader(Global.StartupPath + "\\Skins\\Settings.xml");
                SettingsStreamReader.Close();
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("There was an error while loading the skin",ex);
            }
#endif
            #endregion

            #region check Update

            bool checkUpdate = false;
            TimeSpan tSpan;
            tSpan = DateTime.Now.Subtract(Prefs.lastCheck);
            switch (Prefs.checkUpdate)
            {
                case Prefs.updateFrequency.Always:
                    checkUpdate = true;
                    break;
                case Prefs.updateFrequency.Daily:
                    if (tSpan.Days > 0)
                    {
                        checkUpdate = true;
                    }

                    break;
                case Prefs.updateFrequency.Weekly:
                    if (tSpan.Days > 6)
                    {
                        checkUpdate = true;
                    }

                    break;
                case Prefs.updateFrequency.Monthly:
                    if (tSpan.Days > 30)
                    {
                        checkUpdate = true;
                    }

                    break;
            }

            if (checkUpdate)
            {
                Thread thr = new Thread(IsThereAnUpdate);
                thr.Start(false);
            }

            #endregion

            #region Load available plugins
            this.LoadPlugins();
            #endregion

            #region Load any dropped maps

            string[] temp = Environment.GetCommandLineArgs();
            if (temp.Length > 1)
            {
                TryLoadMapForm(temp[1]);
            }

            #endregion

        }

        #endregion

        #region Delegates

        /// <summary>
        /// The delegate which we will call from the thread to update the form
        /// </summary>
        /// <param name="BytesRead">The bytes read.</param>
        /// <param name="TotalBytes">The total bytes.</param>
        /// <remarks></remarks>
        private delegate void UpdateProgessCallback(long BytesRead, long TotalBytes);

        #endregion

        #region Properties

        /// <summary>
        /// Count of open maps
        /// </summary>
        /// <remarks></remarks>
        public int MapCount { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads plugin sets into the plugin combobox and selects the current plugin
        /// </summary>
        public void LoadPlugins()
        {
            Globals.PluginSetSelector.populate();
            tscbPluginSet.Items.AddRange(Globals.PluginSetSelector.getNames());
            tscbPluginSet.SelectedItem = Globals.PluginSetSelector.getActivePlugin();
            if (tscbPluginSet.SelectedIndex == -1 && tscbPluginSet.Items.Count > 0)
                tscbPluginSet.SelectedIndex = 0;
        }

        /// <summary>
        /// The reload skins.
        /// </summary>
        /// <remarks></remarks>
        public void ReloadSkins()
        {
            foreach (MapForm mapForm in this.MdiChildren)
            {
                mapForm.LoadSkin();
            }
        }

        /// <summary>
        /// Attempts to load a mapand bring up a map form for
        /// it. Function will fail if the map is corrupt and
        /// display an error message. Provide a null string to
        /// show openfileDialog. Returns a refernce to the created mapform
        /// if needed
        /// </summary>
        /// <param name="mapFileName">The map File Name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MapForm TryLoadMapForm(string mapFileName)
        {
            // Only allow for 10 map forms to be open at any one time, for
            // memory reasons
            if (MapCount >= 10)
            {
                MessageBox.Show("Too many maps open, close a map.");
                return null;
            }

            // show a dialog if null filename
            if (mapFileName == null)
            {
                openmapdialog.InitialDirectory = Prefs.pathMapFolder;
                // if the user cancels out the dialog, no map to open
                if (openmapdialog.ShowDialog() == DialogResult.Cancel)
                {
                    return null;
                }
                Prefs.pathMapFolder = openmapdialog.InitialDirectory;

                // set the file name to user chosen file
                mapFileName = openmapdialog.FileName;
            }

            // Check map isn't already loaded into a map form
            foreach (MapForm mapForm in this.MdiChildren)
            {
                if (mapForm.map.filePath == mapFileName)
                {
                    MessageBox.Show("This map is already open in this editor!..");
                    return mapForm;
                }
            }

            // Show a wait cursor while map is loading
            this.Cursor = Cursors.WaitCursor;

            // Attempt to load the map
            Map newMap = Map.LoadFromFile(mapFileName);
            // Restore our arrow cursor after map is loaded
            this.Cursor = Cursors.Arrow;
            // map failed to load for some reason
            if (newMap == null)
            {
                MessageBox.Show("Map failed to load...");
                return null;
            }

            // Show a wait cursor while map is loading
            this.Cursor = Cursors.WaitCursor;
            // create a map form for the map
            MapForm newMapForm = new MapForm(newMap);
            // Restore our arrow cursor after map is loaded
            this.Cursor = Cursors.Arrow;

            // set it to a child of this main form
            newMapForm.MdiParent = this;

            // keep track of open maps
            MapCount++;
            if (MapCount > 0)
            {
                closemapmenu.Enabled = true;
            }

            // store map in recent list
            UpdateRecentFiles(mapFileName);

            // give the form a close event
            newMapForm.FormClosed += newMapForm_FormClosed;
            // show form
            newMapForm.Show();

            return newMapForm;
        }
        #endregion

        #region Methods

        /// <summary>
        /// The form 1_ drag drop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string s in files)
                {
                    TryLoadMapForm(s);
                }
            }
        }

        /// <summary>
        /// The form 1_ drag enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void Form1_DragEnter(object sender, DragEventArgs e)
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
        /// The is there an update.
        /// </summary>
        /// <param name="showInf">The show inf.</param>
        /// <exception cref="WebException">
        ///   </exception>
        /// <remarks></remarks>
        private void IsThereAnUpdate(object showInf)
        {
            bool showInfo = (bool)showInf;
            WebClient wc = new WebClient();

            // Set our FTP Username & Password
            wc.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);

            // Download our Information File
            // [0] = Version Number
            // [1] = Build Number
            // [2] = Release Date
            // [3] = Update Filename
            // [4] = Change Log Filename
            // [5] = Informational URL
            // [6] = Updater Version
            int count = 0;
            string netVerInfo = null;
            while (netVerInfo == null)
            {
                try
                {
                    netVerInfo = wc.DownloadString(updateFTPServer + updateNFOFile);
                }
                catch (WebException e)
                {
                    // Give 5 chances to connect
                    count++;
                    if (count < 5)
                    {
                        Thread.Sleep(300);
                    }
                    else
                    {
                        if (showInfo)
                            throw e;
                        else
                            return;
                    }
                }
            }

            string updtVersion = string.Empty;
            string updtDate = string.Empty;
            string updtName = string.Empty;

            XmlDocument xd = new XmlDocument();
            try
            {
                xd.LoadXml(netVerInfo);
            }
            catch
            {
                if (showInfo)
                    MessageBox.Show("Error reading update file, " + updateNFOFile);
                return;
            }
            if (xd.FirstChild.Name == "entityupdater")
            {
                XmlNode xn = xd.FirstChild.FirstChild;
                while (xn != null)
                {
                    switch (xn.Name)
                    {
                        case "updateinfo":
                            XmlNode relInfoNode = xn.FirstChild;
                            while (relInfoNode != null)
                            {
                                switch (relInfoNode.Name)
                                {
                                    case "version":
                                        updtVersion = relInfoNode.InnerText.Replace(" ", string.Empty);
                                        break;
                                    case "date":
                                        updtDate = relInfoNode.InnerText;
                                        break;
                                    case "filename":
                                        updtName = relInfoNode.InnerText;
                                        break;
                                }

                                relInfoNode = relInfoNode.NextSibling;
                            }

                            break;
                    }

                    xn = xn.NextSibling;
                }
            }

            try
            {
                if (!File.Exists(Global.StartupPath + "\\" + updateUpdFile))
                {
                    wc.DownloadFile(updateFTPServer + updateUpdFile, Global.StartupPath + "\\" + updateUpdFile);
                }
                else
                {
                    // Get current info of our updater
                    FileVersionInfo updtFile = FileVersionInfo.GetVersionInfo(Global.StartupPath + "\\" + updateUpdFile);

                    int VerMaj = updtFile.FileMajorPart;
                    int VerMin = updtFile.FileMinorPart;
                    string[] s = updtVersion.Split('.');
                    if (int.Parse(s[0]) > VerMaj || (int.Parse(s[0]) == VerMaj && int.Parse(s[1]) > VerMin))
                    {
                        File.Delete(Global.StartupPath + "\\" + updateUpdFile);
                        wc.DownloadFile(updateFTPServer + updateUpdFile, Global.StartupPath + "\\" + updateUpdFile);
                    }
                }
            }
            catch
            {
            }

            wc.Dispose();

            // Run update checker
            string args = "/pid:" + Process.GetCurrentProcess().Id;
            if (!showInfo)
                args += " /silent";

            // Attach any open maps to be reopened
            foreach (MapForm mapForm in this.MdiChildren)
            {
                args += " /om:" + mapForm.map.filePath;
            }

            // Checks to see if Entity has been exited before running update routine (causes issues)
            if (Process.GetCurrentProcess().MainWindowHandle.ToString() != "0")
            {
                Prefs.lastCheck = DateTime.Now;
                Prefs.Save();

                Process.Start(Global.StartupPath + "\\" + updateUpdFile, args);
            }
        }

        /// <summary>
        /// Adds the recent files to menu.
        /// </summary>
        /// <remarks></remarks>
        private void AddRecentFilesToMenu()
        {            
            // Remove all previous Recent File listings
            ToolStripItem[] tsis = this.menuFile.DropDownItems.Find("recentFile", false);
            foreach (ToolStripItem tsi in tsis)
            {
                    this.menuFile.DropDownItems.Remove(tsi);
            }
            // Regenerate and add our new list
            int i = 0;
            foreach (Prefs.RecentFile recentMap in Prefs.RecentOpenedMaps)
            {
                // Don't allow us to add more than the max # of maps allowed
                if (i >= Prefs.MaxRecentFiles)
                    break;

                ToolStripMenuItem mItem = new ToolStripMenuItem();
                mItem.Name = "recentFile";
                mItem.ShortcutKeys = Keys.Alt | (Keys.D1 + i);
                mItem.Text = recentMap.Path;
                mItem.Tag = recentMap;
                mItem.Click += this.lastFile_Click;
                recentMap.MenuItem = mItem;
                this.menuFile.DropDownItems.Add(mItem);

                // Seperator
                menuItem1.Visible = true;

                i++;
            }

            // Using arrow keys in menu causes scrolling past end of menu; fixed if we increase menu size by 1
            this.menuFile.DropDown.AutoSize = false;
            this.menuFile.DropDown.Size = new System.Drawing.Size(
                    this.menuFile.DropDown.PreferredSize.Width,
                    this.menuFile.DropDown.PreferredSize.Height + 1);
        }

        /// <summary>
        /// Adds a map to the recent files list. If it already exists, the map is relocated at 
        /// the top of the list.
        /// </summary>
        /// <param name="newFile">The new filename to add to the list</param>
        /// <remarks></remarks>
        private void UpdateRecentFiles(string newFile)
        {
            Prefs.RecentFile rf = null;

            bool alreadyExists = false;
            foreach (Prefs.RecentFile recentMap in Prefs.RecentOpenedMaps)
            {
                if (newFile == recentMap.Path)
                {
                    rf = recentMap;
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                menuFile.DropDownItems.Remove(rf.MenuItem);
                Prefs.RecentOpenedMaps.Remove(rf);
            }
            else
            {
                rf = new Prefs.RecentFile();
                rf.Path = newFile;
            }

            Prefs.RecentOpenedMaps.Insert(0, rf);

            if (Prefs.RecentOpenedMaps.Count > Prefs.MaxRecentFiles)
            {
                menuFile.DropDownItems.Remove(Prefs.RecentOpenedMaps[Prefs.RecentOpenedMaps.Count - 1].MenuItem);
                Prefs.RecentOpenedMaps.RemoveAt(Prefs.RecentOpenedMaps.Count - 1);
            }

            this.AddRecentFilesToMenu();

            Prefs.Save();
            
        }

        /// <summary>
        /// The close entity_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void closeEntity_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        /// <summary>
        /// The closemapmenu_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void closemapmenu_Click(object sender, EventArgs e)
        {
            // Wrong Sender. MenuItem, not MapWindow....hmmm...

            // the menu item is the sender because that is the control that called this method ~Prey
            MapForm mapForm = (MapForm)this.ActiveMdiChild;
            mapForm.Close();
            mapForm.Dispose();
        }

        /// <summary>
        /// When the main form deactivates, we disable each open map window below the active MDI child window,
        /// otherwise the first OPENED map will always return to the focused position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            foreach (MapForm mf in this.MdiChildren)
            {
                if (this.ActiveMdiChild != mf)
                    mf.Enabled = false;
            }            
        }

        /// <summary>
        /// When the form regains focus, we re-enable each opened map. This keeps the maps in the current
        /// selected order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Activated(object sender, EventArgs e)
        {
            foreach (MapForm mf in this.MdiChildren)
            {
                mf.Enabled = true;
            }
        }

        /// <summary>
        /// The help about_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void helpAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        /// <summary>
        /// The help ve controls_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void helpVEControls_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "CONTROLS FOR THE VISUAL EDITOR\n" + "==============================\n" + " W - Camera Forwards\n" +
                " S - Camera Backwards\n" + " A - Camera Left\n" + " D - Camera Right\n" + " Z - Camera Down\n" +
                " X - Camera Up\n" + "\n" + " Right Mouse Button - Camera Look-Around\n" +
                " Left  Mouse Button - Select/Deselect Object/Spawn\n" + "\n" + "WHEN OBJECT/SPAWN SELECTED\n" +
                "==========================\n" + " Up/Down    Arrows = Move Object Along X-Axis\n" +
                " Left/Right Arrows = Move Object Along Y-Axis\n" + " Page Up/Page Down = Move Object Along Z-Axis\n" +
                "\n" + " SHIFT + Left Mouse Button - Rotate Object Yaw (Around Z-Axis)\n" +
                " ALT   + Left Mouse Button - Rotate Object Pitch (Around Y-Axis)\n" +
                " CTRL  + Left Mouse Button - Rotate Object Roll (Around X-Axis)\n", 
                "BSP Visual Editor Controls");
        }

        /// <summary>
        /// The last file_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lastFile_Click(object sender, EventArgs e)
        {
            TryLoadMapForm(((ToolStripItem)sender).Text);
        }

        /// <summary>
        /// Opens up the MAINMENU editor dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void mainmenuEdit_Click(object sender, EventArgs e)
        {
            MainmenuEdit mme = new MainmenuEdit();
            mme.ShowDialog();
        }

        /// <summary>
        /// Makes .MAP files open by default in Entity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void makeDefaultMapEditor_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    "Are you sure you would like to set Entity as your default editor?", 
                    "Set As Default Editor", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RegistryKey regKey;
                regKey = Registry.ClassesRoot.CreateSubKey("Entity\\");
                regKey.SetValue(string.Empty, "Halo Map File");
                regKey = Registry.ClassesRoot.CreateSubKey("Entity\\shell\\open\\command");
                regKey.SetValue(string.Empty, Application.ExecutablePath + " %1");
                regKey = Registry.ClassesRoot.CreateSubKey(".map");
                regKey.SetValue(string.Empty, "Entity");
                regKey.Close();
                MessageBox.Show("Entity is now your default map editor.", "Set Default Editor");
            }
        }

        /// <summary>
        /// Called when Entity is closed (closes all open instances of MAPFORM).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MapForm mapForm = (MapForm)sender;

            // make sure map stream is closed
            mapForm.map.CloseMap();

            // update map count
            MapCount--;
            if (MapCount <= 0)
            {
                closemapmenu.Enabled = false;
            }

            Prefs.Save();
        }

        /// <summary>
        /// Opens the dialog to create a new map from a clean version
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newmapmenu_Click(object sender, EventArgs e)
        {
            MapWizard newWizard = new MapWizard();
            newWizard.Owner = this;
            newWizard.Text = "Create a new map...";
            newWizard.Show();
        }

        /// <summary>
        /// Opens a .MAP file into a MAPFORM
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void openmapmenu_Click(object sender, EventArgs e)
        {
            // pass in null so the function shows an open file dialog
            // for the user to choose a map
            TryLoadMapForm(null);
        }

        /// <summary>
        /// The tile cascademenu item 5_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tileCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        /// <summary>
        /// The tile horizontal menu click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tileHorizontal_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        /// <summary>
        /// The tile vertical menu click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tileVertical_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        /// <summary>
        /// The tools check for updates_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsCheckForUpdates_Click(object sender, EventArgs e)
        {
            try
            {
                IsThereAnUpdate(true);
            }
            catch (WebException webEx)
            {
                switch (webEx.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        Global.ShowErrorMsg(webEx.Message, webEx);
                        break;
                    default:
                        Global.ShowErrorMsg("Internet Error", webEx);
                        break;
                }
            }
        }

        /// <summary>
        /// The tools format plugins_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsFormatPlugins_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Global.StartupPath;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            MiscFunctions.FormatIFPsInDirectory(folderBrowserDialog1.SelectedPath);
            MessageBox.Show("Converted IFP and IFP2 Files");
        }

        /// <summary>
        /// The tools settings_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        /// <summary>
        /// The tools skin options_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsSkinOptions_Click(object sender, EventArgs e)
        {
            SkinOptions skinopt = new SkinOptions();
            skinopt.mainform = this;
            skinopt.Show();
        }

        /// <summary>
        /// The tools video settings_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsVideoSettings_Click(object sender, EventArgs e)
        {
            VideoSettings vs = new VideoSettings();
            vs.ShowDialog();
        }

        private void tsbtnEditPluginSet_Click(object sender, EventArgs e)
        {
            Globals.PluginSetSelector pss = new Globals.PluginSetSelector();
            pss.Owner = this;
            pss.ShowDialog();

            object temp = tscbPluginSet.SelectedItem;
            tscbPluginSet.Items.Clear();
            tscbPluginSet.Items.AddRange(Globals.PluginSetSelector.getNames());
            tscbPluginSet.SelectedItem = temp;
            if (tscbPluginSet.SelectedItem == null)
                tscbPluginSet.SelectedIndex = 0;
        }

        private void tscbPluginSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbPluginSet.Focused)
            {
                RegistryAccess.setValue(Registry.CurrentUser,
                                     RegistryAccess.RegPaths.Halo2 + "PluginSets\\",
                                     "",
                                     tscbPluginSet.SelectedItem);
            }
            Prefs.pathPluginsFolder = Globals.PluginSetSelector.getPath(tscbPluginSet.SelectedItem.ToString());

            foreach (MapForm mf in this.MdiChildren)
            {
                if (mf.map.SelectedMeta != null)
                {
                    mf.map.SelectedMeta.type = string.Empty;
                    mf.LoadMeta(mf.map.SelectedMeta.TagIndex);
                    // Reload meta tag type
                }
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Prefs.Save();
        }

        private void mainMenu1_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                this.ContextMenu = new ContextMenu(new MenuItem[] {new MenuItem("Lock Toolbar") });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateContextMenuList();
        }

        private void helpContents_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.Size = new System.Drawing.Size(
                Screen.PrimaryScreen.WorkingArea.Width * 2 / 3,
                Screen.PrimaryScreen.WorkingArea.Height * 4 / 5);
            hf.Show();
        }

        private void toolsLatestPlugins_Click(object sender, EventArgs e)
        {
            PluginUpdater pu = new PluginUpdater();
            pu.Add("hlmt", new Version(1, 0, 1, 0));
            pu.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    helpContents_Click(this, null);
                    return true;
                case Keys.F2:
                    mainmenuEdit_Click(this, null);
                    return true;
                default:
                    // Call the base class
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        } 
        #endregion
   }
}