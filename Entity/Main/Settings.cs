// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
//   
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;

    using Microsoft.Win32;

    // using System.Linq;

    /// <summary>
    /// The settings.
    /// </summary>
    /// <remarks></remarks>
    public partial class Settings : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Settings()
        {
            InitializeComponent();

            tbMainmenuFile.Text = Prefs.pathMainmenu;
            tbSharedFile.Text = Prefs.pathShared;
            tbSPSharedFile.Text = Prefs.pathSPShared;
            tbBitmapsFile.Text = Prefs.pathBitmaps;

            tbMapsFolder.Text = Prefs.pathMapFolder;
            tbCleanMapsFolder.Text = Prefs.pathCleanMaps;
            tbMapsFolder.Text = Prefs.pathMapFolder;
            tbPluginFolder.Text = Prefs.pathPluginsFolder;
            tbBitmapsFolder.Text = Prefs.pathBitmapsFolder;
            tbExtractsFolder.Text = Prefs.pathExtractsFolder;
            tbPatchFolder.Text = Prefs.pathPatchFolder;
            useDefaultMaps.Checked = Prefs.useDefaultMaps;
            cbUseRegistry.Checked = Prefs.useRegistryEntries;
            checkUpdates.SelectedItem = Prefs.checkUpdate;

            /*
            if (checkUpdates.SelectedIndex == -1)
                checkUpdates.SelectedIndex = 0;
             */
        }

        #endregion

        #region Methods

        /// <summary>
        /// The find directory.
        /// </summary>
        /// <param name="initialDirectory">The initial directory.</param>
        /// <param name="viewName">The view name.</param>
        /// <returns>The find directory.</returns>
        /// <remarks></remarks>
        private string FindDirectory(string initialDirectory, string viewName)
        {
            // Create an OpenFileDialog object.
            FolderBrowserDialog openFolder1 = new FolderBrowserDialog();

            // Initialize the OpenFileDialog to look for text files.
            openFolder1.SelectedPath = initialDirectory;

            // Check if the user selected a file from the OpenFileDialog.
            if (openFolder1.ShowDialog() == DialogResult.OK)
            {
                return openFolder1.SelectedPath;
            }
            else
            {
                return initialDirectory;
            }
        }

        /// <summary>
        /// The find file.
        /// </summary>
        /// <param name="initPath">The init path.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="viewName">The view name.</param>
        /// <returns>The find file.</returns>
        /// <remarks></remarks>
        private string FindFile(string initPath, string fileName, string viewName)
        {
            if (initPath.LastIndexOf("\\") != -1)
            {
                initPath = initPath.Substring(0, initPath.LastIndexOf("\\"));
            }

            // Create an OpenFileDialog object.
            OpenFileDialog openFile1 = new OpenFileDialog();

            // Initialize the OpenFileDialog to look for text files.
            openFile1.InitialDirectory = initPath;
            openFile1.Filter = viewName + " File|" + fileName;
            openFile1.FileName = fileName;

            // Check if the user selected a file from the OpenFileDialog.
            if (openFile1.ShowDialog() == DialogResult.OK)
            {
                return openFile1.FileName;
            }
            else
            {
                return initPath + "\\" + fileName;
            }
        }

        /// <summary>
        /// The button bitmaps_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnBitmapsFile_Click(object sender, EventArgs e)
        {
            tbBitmapsFile.Text = FindFile(tbBitmapsFile.Text, "bitmaps.map", "Bitmaps (Only for Halo1 & Halo CE)");
        }

        private void btnBitmapFolder_Click(object sender, EventArgs e)
        {
            tbBitmapsFolder.Text = FindDirectory(tbBitmapsFolder.Text, "Bitmap Extract Directory");
        }

        /// <summary>
        /// The button cancel_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The btn clean maps_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnCleanMapsFolder_Click(object sender, EventArgs e)
        {
            tbCleanMapsFolder.Text = FindDirectory(tbCleanMapsFolder.Text, "Clean Maps Directory");
        }

        private void btnExtractFolder_Click(object sender, EventArgs e)
        {
            tbExtractsFolder.Text = FindDirectory(tbExtractsFolder.Text, "Meta Extracts Directory");
        }
        /// <summary>
        /// The btn mainmenu_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnMainmenuFile_Click(object sender, EventArgs e)
        {
            tbMainmenuFile.Text = FindFile(tbMainmenuFile.Text, "mainmenu.map", "MainMenu");
        }

        /// <summary>
        /// The btn map folder_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnMapsFolder_Click(object sender, EventArgs e)
        {
            tbMapsFolder.Text = FindDirectory(tbMapsFolder.Text, "Maps Directory");
        }

        /// <summary>
        /// The btn patch folder_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnPatchFolder_Click(object sender, EventArgs e)
        {
            tbPatchFolder.Text = FindDirectory(tbPatchFolder.Text, "Patches Directory");
        }

        /// <summary>
        /// The btn plugin dir_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnPluginFolder_Click(object sender, EventArgs e)
        {
            tbPluginFolder.Text = FindDirectory(tbPluginFolder.Text, "Plugins Directory");
        }

        /// <summary>
        /// The btn sp shared_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSPSharedFile_Click(object sender, EventArgs e)
        {
            tbSPSharedFile.Text = FindFile(tbSPSharedFile.Text, "single_player_shared.map", "SP_Shared");
        }

        /// <summary>
        /// The btn save_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Prefs.pathMainmenu = tbMainmenuFile.Text;
            Prefs.pathShared = tbSharedFile.Text;
            Prefs.pathSPShared = tbSPSharedFile.Text;
            Prefs.pathBitmaps = tbBitmapsFile.Text;
            Prefs.pathMapFolder = tbMapsFolder.Text;
            Prefs.pathCleanMaps = tbCleanMapsFolder.Text;
            Prefs.pathPluginsFolder = tbPluginFolder.Text;
            Prefs.pathBitmapsFolder = tbBitmapsFolder.Text;
            Prefs.pathExtractsFolder = tbExtractsFolder.Text;
            Prefs.pathPatchFolder = tbPatchFolder.Text;
            Prefs.useDefaultMaps = useDefaultMaps.Checked;
            Prefs.useRegistryEntries = cbUseRegistry.Checked;
            Prefs.checkUpdate = (Prefs.updateFrequency)checkUpdates.SelectedItem;

            Prefs.Save();

            this.Close();
        }

        /// <summary>
        /// The btn shared_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnSharedFile_Click(object sender, EventArgs e)
        {
            tbSharedFile.Text = FindFile(tbSharedFile.Text, "shared.map", "Shared");
        }

        /// <summary>
        /// For those who wish to not have settings stored in their registry, allow them to remove it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearRegistry_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will remove all Halo 2 entires from the registry. Are you sure you wish to continue?",
                                "Remove Halo 2 Registry Entries?", 
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RegistryAccess.removeKey(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2);
            }
        }

        /// <summary>
        /// Allow the option to use the registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUseRegistry_CheckedChanged(object sender, EventArgs e)
        {
            btnClearRegistry.Enabled = !cbUseRegistry.Checked;
        }

        #endregion
    }
}