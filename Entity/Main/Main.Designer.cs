namespace entity.Main
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class Form1 : System.Windows.Forms.Form
	{

        #region Windows Form Designer generated code

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Make sure everything is closed, then exit.

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                while (Application.OpenForms.Count > 0)
                    Application.OpenForms[0].Dispose();
            }

            base.Dispose(disposing);
        }
        private System.Windows.Forms.OpenFileDialog openmapdialog;
        private FolderBrowserDialog folderBrowserDialog1;
        /*
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuFile;
        private System.Windows.Forms.OpenFileDialog openmapdialog;
        private System.Windows.Forms.MenuItem openmapmenu;
        private System.Windows.Forms.MenuItem menuWindows;
        private System.Windows.Forms.MenuItem tileHorizontal;
        private System.Windows.Forms.MenuItem tileVertical;
        private System.Windows.Forms.MenuItem tileCascade;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem makeDefaultMapViewer;
        private MenuItem menuTools;
        private MenuItem toolsFormatPlugins;
        private FolderBrowserDialog folderBrowserDialog1;
        private MenuItem toolsSkinOptions;
        private MenuItem toolsVideoSettings;
        private MenuItem toolsCheckForUpdates;
        private MenuItem toolsSettings;
        private MenuItem menuItem13;
        private MenuItem closeEntity;
        private MenuItem menuHelp;
        private MenuItem helpAbout;
        private MenuItem menuItem1;
        private MenuItem helpVEControls;
        private MenuItem closemapmenu;
        private MenuItem newmapmenu;
        private MenuItem mainmenuEditorMenu;
        private MenuItem mainmenuEdit;
        private MenuItem mainmenuAddthisMap;
        */
        private IContainer components;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openmapdialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStripDebug = new System.Windows.Forms.MenuStrip();
            this.tslblDebugIP = new System.Windows.Forms.ToolStripLabel();
            this.tstbDebugIP = new System.Windows.Forms.ToolStripTextBox();
            this.tsbtnDebugConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbtnDebugDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbtnDebugLoadMap = new System.Windows.Forms.ToolStripMenuItem();
            this.tslblDebugStatus = new System.Windows.Forms.ToolStripLabel();
            this.tsbtnDebugReset = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDebug = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscbPluginSet = new System.Windows.Forms.ToolStripComboBox();
            this.tsbtnEditPluginSet = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.newmapmenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openmapmenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closemapmenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.makeDefaultMapViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.closeEntity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsFormatPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsLatestPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSkinOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsVideoSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontal = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVertical = new System.Windows.Forms.ToolStripMenuItem();
            this.tileCascade = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.helpVEControls = new System.Windows.Forms.ToolStripMenuItem();
            this.helpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mainmenuEditorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mainmenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mainmenuAddthisMap = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPanelTop = new System.Windows.Forms.ToolStripPanel();
            this.contextMenuStripMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lockToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPanelBottom = new System.Windows.Forms.ToolStripPanel();
            this.menuStripDebug.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.mainMenu1.SuspendLayout();
            this.tsPanelTop.SuspendLayout();
            this.contextMenuStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // openmapdialog
            // 
            this.openmapdialog.Filter = "Map File (*.map)| *.map";
            // 
            // menuStripDebug
            // 
            this.menuStripDebug.AllowDrop = true;
            this.menuStripDebug.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripDebug.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStripDebug.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblDebugIP,
            this.tstbDebugIP,
            this.tsbtnDebugConnect,
            this.tsbtnDebugDisconnect,
            this.tsbtnDebugLoadMap,
            this.tslblDebugStatus,
            this.tsbtnDebugReset});
            this.menuStripDebug.Location = new System.Drawing.Point(669, 0);
            this.menuStripDebug.Name = "menuStripDebug";
            this.menuStripDebug.Size = new System.Drawing.Size(291, 25);
            this.menuStripDebug.Stretch = false;
            this.menuStripDebug.TabIndex = 5;
            this.menuStripDebug.Text = "Debug Xbox";
            // 
            // tslblDebugIP
            // 
            this.tslblDebugIP.Name = "tslblDebugIP";
            this.tslblDebugIP.Size = new System.Drawing.Size(61, 18);
            this.tslblDebugIP.Text = "  Debug IP:";
            // 
            // tstbDebugIP
            // 
            this.tstbDebugIP.Name = "tstbDebugIP";
            this.tstbDebugIP.Size = new System.Drawing.Size(100, 21);
            this.tstbDebugIP.Text = "<Auto>";
            this.tstbDebugIP.TextChanged += new System.EventHandler(this.tstbDebugIP_TextChanged);
            this.tstbDebugIP.Click += new System.EventHandler(this.tstbDebugIP_Click);
            // 
            // tsbtnDebugConnect
            // 
            this.tsbtnDebugConnect.Name = "tsbtnDebugConnect";
            this.tsbtnDebugConnect.Size = new System.Drawing.Size(59, 21);
            this.tsbtnDebugConnect.Text = "Connect";
            this.tsbtnDebugConnect.Click += new System.EventHandler(this.tsbtnDebugConnect_Click);
            // 
            // tsbtnDebugDisconnect
            // 
            this.tsbtnDebugDisconnect.Enabled = false;
            this.tsbtnDebugDisconnect.Name = "tsbtnDebugDisconnect";
            this.tsbtnDebugDisconnect.Size = new System.Drawing.Size(71, 21);
            this.tsbtnDebugDisconnect.Text = "Disconnect";
            this.tsbtnDebugDisconnect.Click += new System.EventHandler(this.tsbtnDebugDisconnect_Click);
            // 
            // tsbtnDebugLoadMap
            // 
            this.tsbtnDebugLoadMap.Enabled = false;
            this.tsbtnDebugLoadMap.Name = "tsbtnDebugLoadMap";
            this.tsbtnDebugLoadMap.Size = new System.Drawing.Size(92, 21);
            this.tsbtnDebugLoadMap.Text = "Auto-Load Map";
            this.tsbtnDebugLoadMap.Click += new System.EventHandler(this.tsbtnDebugLoadMap_Click);
            // 
            // tslblDebugStatus
            // 
            this.tslblDebugStatus.Name = "tslblDebugStatus";
            this.tslblDebugStatus.Size = new System.Drawing.Size(87, 18);
            this.tslblDebugStatus.Text = "[Not Connected]";
            // 
            // tsbtnDebugReset
            // 
            this.tsbtnDebugReset.Enabled = false;
            this.tsbtnDebugReset.Name = "tsbtnDebugReset";
            this.tsbtnDebugReset.Size = new System.Drawing.Size(47, 21);
            this.tsbtnDebugReset.Text = "Reset";
            this.tsbtnDebugReset.DoubleClick += new System.EventHandler(this.tsbtnDebugReset_DoubleClick);
            this.tsbtnDebugReset.Click += new System.EventHandler(this.tsbtnDebugReset_Click);
            // 
            // timerDebug
            // 
            this.timerDebug.Interval = 10000;
            this.timerDebug.Tick += new System.EventHandler(this.timerDebug_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tscbPluginSet,
            this.tsbtnEditPluginSet});
            this.menuStrip1.Location = new System.Drawing.Point(351, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(318, 25);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "Plugin Set";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(91, 18);
            this.toolStripLabel1.Text = "Active Plugin Set:";
            // 
            // tscbPluginSet
            // 
            this.tscbPluginSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbPluginSet.Name = "tscbPluginSet";
            this.tscbPluginSet.Size = new System.Drawing.Size(121, 21);
            this.tscbPluginSet.SelectedIndexChanged += new System.EventHandler(this.tscbPluginSet_SelectedIndexChanged);
            // 
            // tsbtnEditPluginSet
            // 
            this.tsbtnEditPluginSet.Name = "tsbtnEditPluginSet";
            this.tsbtnEditPluginSet.Size = new System.Drawing.Size(92, 21);
            this.tsbtnEditPluginSet.Text = "Edit Plugin Sets";
            this.tsbtnEditPluginSet.Click += new System.EventHandler(this.tsbtnEditPluginSet_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools,
            this.menuWindows,
            this.menuHelp,
            this.mainmenuEditorMenu});
            this.mainMenu1.Location = new System.Drawing.Point(3, 0);
            this.mainMenu1.Name = "mainMenu1";
            this.mainMenu1.Size = new System.Drawing.Size(348, 24);
            this.mainMenu1.Stretch = false;
            this.mainMenu1.TabIndex = 3;
            this.mainMenu1.Text = "MainMenu";
            this.mainMenu1.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.MainMenuStrip_ItemAdded);
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newmapmenu,
            this.openmapmenu,
            this.closemapmenu,
            this.menuItem6,
            this.makeDefaultMapViewer,
            this.menuItem13,
            this.closeEntity,
            this.menuItem1});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(35, 20);
            this.menuFile.Text = "File";
            // 
            // newmapmenu
            // 
            this.newmapmenu.Name = "newmapmenu";
            this.newmapmenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
            this.newmapmenu.Size = new System.Drawing.Size(256, 22);
            this.newmapmenu.Text = "&New Map";
            this.newmapmenu.Click += new System.EventHandler(this.newmapmenu_Click);
            // 
            // openmapmenu
            // 
            this.openmapmenu.Name = "openmapmenu";
            this.openmapmenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.openmapmenu.Size = new System.Drawing.Size(256, 22);
            this.openmapmenu.Text = "&Open Map";
            this.openmapmenu.Click += new System.EventHandler(this.openmapmenu_Click);
            // 
            // closemapmenu
            // 
            this.closemapmenu.Enabled = false;
            this.closemapmenu.Name = "closemapmenu";
            this.closemapmenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closemapmenu.Size = new System.Drawing.Size(256, 22);
            this.closemapmenu.Text = "Close Map";
            this.closemapmenu.Visible = false;
            this.closemapmenu.Click += new System.EventHandler(this.closemapmenu_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Name = "menuItem6";
            this.menuItem6.Size = new System.Drawing.Size(253, 6);
            // 
            // makeDefaultMapViewer
            // 
            this.makeDefaultMapViewer.Name = "makeDefaultMapViewer";
            this.makeDefaultMapViewer.Size = new System.Drawing.Size(256, 22);
            this.makeDefaultMapViewer.Text = "Make Entity the Default Map Viewer";
            this.makeDefaultMapViewer.Click += new System.EventHandler(this.makeDefaultMapEditor_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Name = "menuItem13";
            this.menuItem13.Size = new System.Drawing.Size(253, 6);
            // 
            // closeEntity
            // 
            this.closeEntity.Name = "closeEntity";
            this.closeEntity.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.closeEntity.Size = new System.Drawing.Size(256, 22);
            this.closeEntity.Text = "Exit";
            this.closeEntity.Click += new System.EventHandler(this.closeEntity_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(253, 6);
            this.menuItem1.Visible = false;
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsSettings,
            this.toolsFormatPlugins,
            this.toolsLatestPlugins,
            this.toolsSkinOptions,
            this.toolsVideoSettings,
            this.toolsCheckForUpdates});
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(44, 20);
            this.menuTools.Text = "Tools";
            // 
            // toolsSettings
            // 
            this.toolsSettings.Name = "toolsSettings";
            this.toolsSettings.Size = new System.Drawing.Size(179, 22);
            this.toolsSettings.Text = "Settings";
            this.toolsSettings.Click += new System.EventHandler(this.toolsSettings_Click);
            // 
            // toolsFormatPlugins
            // 
            this.toolsFormatPlugins.Name = "toolsFormatPlugins";
            this.toolsFormatPlugins.Size = new System.Drawing.Size(179, 22);
            this.toolsFormatPlugins.Text = "Convert IFP Plugins";
            this.toolsFormatPlugins.Click += new System.EventHandler(this.toolsFormatPlugins_Click);
            // 
            // toolsLatestPlugins
            // 
            this.toolsLatestPlugins.Name = "toolsLatestPlugins";
            this.toolsLatestPlugins.Size = new System.Drawing.Size(179, 22);
            this.toolsLatestPlugins.Text = "Update Plugins";
            this.toolsLatestPlugins.Click += new System.EventHandler(this.toolsLatestPlugins_Click);
            // 
            // toolsSkinOptions
            // 
            this.toolsSkinOptions.Name = "toolsSkinOptions";
            this.toolsSkinOptions.Size = new System.Drawing.Size(179, 22);
            this.toolsSkinOptions.Text = "Skin Options";
            this.toolsSkinOptions.Click += new System.EventHandler(this.toolsSkinOptions_Click);
            // 
            // toolsVideoSettings
            // 
            this.toolsVideoSettings.Name = "toolsVideoSettings";
            this.toolsVideoSettings.Size = new System.Drawing.Size(179, 22);
            this.toolsVideoSettings.Text = "Video Information";
            this.toolsVideoSettings.Click += new System.EventHandler(this.toolsVideoSettings_Click);
            // 
            // toolsCheckForUpdates
            // 
            this.toolsCheckForUpdates.Name = "toolsCheckForUpdates";
            this.toolsCheckForUpdates.Size = new System.Drawing.Size(179, 22);
            this.toolsCheckForUpdates.Text = "Check For Updates";
            this.toolsCheckForUpdates.Click += new System.EventHandler(this.toolsCheckForUpdates_Click);
            // 
            // menuWindows
            // 
            this.menuWindows.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileHorizontal,
            this.tileVertical,
            this.tileCascade});
            this.menuWindows.Name = "menuWindows";
            this.menuWindows.Size = new System.Drawing.Size(62, 20);
            this.menuWindows.Text = "Windows";
            // 
            // tileHorizontal
            // 
            this.tileHorizontal.Name = "tileHorizontal";
            this.tileHorizontal.Size = new System.Drawing.Size(152, 22);
            this.tileHorizontal.Text = "Tile Horizontal";
            this.tileHorizontal.Click += new System.EventHandler(this.tileHorizontal_Click);
            // 
            // tileVertical
            // 
            this.tileVertical.Name = "tileVertical";
            this.tileVertical.Size = new System.Drawing.Size(152, 22);
            this.tileVertical.Text = "Tyle Vertical";
            this.tileVertical.Click += new System.EventHandler(this.tileVertical_Click);
            // 
            // tileCascade
            // 
            this.tileCascade.Name = "tileCascade";
            this.tileCascade.Size = new System.Drawing.Size(152, 22);
            this.tileCascade.Text = "Cascade";
            this.tileCascade.Click += new System.EventHandler(this.tileCascade_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpContents,
            this.helpVEControls,
            this.helpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(40, 20);
            this.menuHelp.Text = "Help";
            // 
            // helpContents
            // 
            this.helpContents.Name = "helpContents";
            this.helpContents.Size = new System.Drawing.Size(186, 22);
            this.helpContents.Text = "Contents";
            this.helpContents.Click += new System.EventHandler(this.helpContents_Click);
            // 
            // helpVEControls
            // 
            this.helpVEControls.Name = "helpVEControls";
            this.helpVEControls.Size = new System.Drawing.Size(186, 22);
            this.helpVEControls.Text = "Visual Editor Controls";
            this.helpVEControls.Click += new System.EventHandler(this.helpVEControls_Click);
            // 
            // helpAbout
            // 
            this.helpAbout.Name = "helpAbout";
            this.helpAbout.Size = new System.Drawing.Size(186, 22);
            this.helpAbout.Text = "About";
            this.helpAbout.Click += new System.EventHandler(this.helpAbout_Click);
            // 
            // mainmenuEditorMenu
            // 
            this.mainmenuEditorMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainmenuEdit,
            this.mainmenuAddthisMap});
            this.mainmenuEditorMenu.Name = "mainmenuEditorMenu";
            this.mainmenuEditorMenu.Size = new System.Drawing.Size(67, 20);
            this.mainmenuEditorMenu.Text = "Mainmenu";
            // 
            // mainmenuEdit
            // 
            this.mainmenuEdit.Name = "mainmenuEdit";
            this.mainmenuEdit.Size = new System.Drawing.Size(147, 22);
            this.mainmenuEdit.Text = "&Edit";
            this.mainmenuEdit.Click += new System.EventHandler(this.mainmenuEdit_Click);
            // 
            // mainmenuAddthisMap
            // 
            this.mainmenuAddthisMap.Name = "mainmenuAddthisMap";
            this.mainmenuAddthisMap.Size = new System.Drawing.Size(147, 22);
            this.mainmenuAddthisMap.Text = "&Add this map";
            this.mainmenuAddthisMap.Visible = false;
            // 
            // tsPanelTop
            // 
            this.tsPanelTop.ContextMenuStrip = this.contextMenuStripMenu;
            this.tsPanelTop.Controls.Add(this.mainMenu1);
            this.tsPanelTop.Controls.Add(this.menuStrip1);
            this.tsPanelTop.Controls.Add(this.menuStripDebug);
            this.tsPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tsPanelTop.Location = new System.Drawing.Point(0, 0);
            this.tsPanelTop.MaximumSize = new System.Drawing.Size(8000, 50);
            this.tsPanelTop.Name = "tsPanelTop";
            this.tsPanelTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.tsPanelTop.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tsPanelTop.Size = new System.Drawing.Size(960, 25);
            this.tsPanelTop.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.toolStripPanel_ControlAdded);
            // 
            // contextMenuStripMenu
            // 
            this.contextMenuStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lockToolbarToolStripMenuItem});
            this.contextMenuStripMenu.Name = "contextMenuStripMenu";
            this.contextMenuStripMenu.Size = new System.Drawing.Size(156, 26);
            // 
            // lockToolbarToolStripMenuItem
            // 
            this.lockToolbarToolStripMenuItem.Name = "lockToolbarToolStripMenuItem";
            this.lockToolbarToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.lockToolbarToolStripMenuItem.Text = "Unlock Toolbar";
            this.lockToolbarToolStripMenuItem.Click += new System.EventHandler(this.lockToolbarToolStripMenuItem_Click);
            // 
            // tsPanelBottom
            // 
            this.tsPanelBottom.ContextMenuStrip = this.contextMenuStripMenu;
            this.tsPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsPanelBottom.Location = new System.Drawing.Point(0, 645);
            this.tsPanelBottom.MaximumSize = new System.Drawing.Size(8000, 25);
            this.tsPanelBottom.Name = "tsPanelBottom";
            this.tsPanelBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.tsPanelBottom.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tsPanelBottom.Size = new System.Drawing.Size(960, 0);
            this.tsPanelBottom.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.toolStripPanel_ControlAdded);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(960, 645);
            this.Controls.Add(this.tsPanelBottom);
            this.Controls.Add(this.tsPanelTop);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenu1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Entity";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStripDebug.ResumeLayout(false);
            this.menuStripDebug.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mainMenu1.ResumeLayout(false);
            this.mainMenu1.PerformLayout();
            this.tsPanelTop.ResumeLayout(false);
            this.tsPanelTop.PerformLayout();
            this.contextMenuStripMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Updates menu strip listings in Context Menu Strip
        /// </summary>
        private void updateContextMenuList()
        {
            foreach (MenuStrip ms in tsPanelTop.Controls)
            {
                // Don't include the mainmenu in our list
                if (ms == mainMenu1)
                    continue;
                int i = contextMenuStripMenu.Items.IndexOfKey(ms.Text);
                if (i == -1)
                {
                    ToolStripButton tsb = new ToolStripButton("  " + ms.Text);
                    tsb.Checked = ms.Visible;
                    tsb.CheckOnClick = true;
                    tsb.CheckedChanged += new System.EventHandler(tsb_CheckedChanged);                    
                    tsb.Name = ms.Name;
                    contextMenuStripMenu.Items.Add(tsb);
                }
            }
        }
        
        /// <summary>
        /// Occurs when a toolbar is selected / deselected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsb_CheckedChanged(object sender, System.EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;
            int i = tsPanelTop.Controls.IndexOfKey(tsb.Name);
            if (i != -1)
                tsPanelTop.Controls[i].Visible = tsb.Checked;
            else
            {
                i = tsPanelBottom.Controls.IndexOfKey(tsb.Name);
                if (i != -1)
                    tsPanelBottom.Controls[i].Visible = tsb.Checked;
            }
        }

        /// <summary>
        /// Updates all MenuStrip Controls
        /// </summary>
        private void updateMenuStripLock()
        {
            foreach (MenuStrip mStrip in this.tsPanelTop.Controls)
            {
                updateMenuStripLock(mStrip);
            }
            foreach (MenuStrip mStrip in this.tsPanelBottom.Controls)
            {
                updateMenuStripLock(mStrip);
            }
        }

        /// <summary>
        /// Checks Locked status of menu strip and sets grip controls accordingly
        /// </summary>
        private void updateMenuStripLock(MenuStrip mStrip)
        {
            if (mStrip == this.MainMenuStrip)
            {
                mStrip.GripStyle = ToolStripGripStyle.Hidden;
            }
            else
            {
                mStrip.GripStyle = tsPanelTop.Locked ? ToolStripGripStyle.Hidden : ToolStripGripStyle.Visible;
                if (mStrip.GripStyle == ToolStripGripStyle.Hidden)
                {
                    // Add divider for controls not against the left edge
                    if (mStrip.Location.X >= 10)
                    {
                        mStrip.Items.Insert(0, new ToolStripSeparator());
                        mStrip.Items[0].Name = "DefaultDivider";
                    }
                }
                else
                {
                    int temp = mStrip.Items.IndexOfKey("DefaultDivider");
                    if (temp != -1)
                        mStrip.Items.RemoveAt(temp);
                }
            }
        }

        void lockToolbarToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            tsPanelTop.Locked = !tsPanelTop.Locked;
            lockToolbarToolStripMenuItem.Text = tsPanelTop.Locked ? "Unlock Toolbar" : "Lock Toolbar";
            updateMenuStripLock();
        }

        void toolStripPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            updateMenuStripLock((MenuStrip)e.Control);
        }

        /// <summary>
        /// Removes the Minimize, Restore & Close buttons from being placed in out MenuStrip when maximized
        /// as they are placed in a strange location due to multiple toolstripmenus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainMenuStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
        {            
            MenuStrip ms = (MenuStrip)sender;
            for (int x = ms.Items.Count - 1; x >= 0; x--)
            {
                if (ms.Items[x].Text == "Mi&nimize" ||
                    ms.Items[x].Text == "&Restore" ||
                    ms.Items[x].Text == "&Close")
                    ms.Items.RemoveAt(x);
            }
        }

        /// <summary>
        /// Makes it so when the DebugIP box is clicked and "<Auto>" is written, it selects
        /// all letters for easy overtype
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tstbDebugIP_Click(object sender, System.EventArgs e)
        {
            if (this.tstbDebugIP.Text == "<Auto>")
                this.tstbDebugIP.SelectAll();
        }

        #endregion

        private MenuStrip menuStripDebug;
        private ToolStripLabel tslblDebugIP;
        private ToolStripTextBox tstbDebugIP;
        private ToolStripMenuItem tsbtnDebugConnect;
        private ToolStripMenuItem tsbtnDebugDisconnect;
        private ToolStripLabel tslblDebugStatus;
        private ToolStripMenuItem tsbtnDebugReset;
        private Timer timerDebug;
        private ToolStripMenuItem tsbtnDebugLoadMap;
        private MenuStrip menuStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox tscbPluginSet;
        private ToolStripMenuItem tsbtnEditPluginSet;
        private MenuStrip mainMenu1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem newmapmenu;
        private ToolStripMenuItem openmapmenu;
        private ToolStripMenuItem closemapmenu;
        private ToolStripSeparator menuItem6;
        private ToolStripMenuItem makeDefaultMapViewer;
        private ToolStripSeparator menuItem13;
        private ToolStripMenuItem closeEntity;
        private ToolStripSeparator menuItem1;
        private ToolStripMenuItem menuTools;
        private ToolStripMenuItem toolsSettings;
        private ToolStripMenuItem toolsFormatPlugins;
        private ToolStripMenuItem toolsSkinOptions;
        private ToolStripMenuItem toolsVideoSettings;
        private ToolStripMenuItem toolsCheckForUpdates;
        private ToolStripMenuItem menuWindows;
        private ToolStripMenuItem tileHorizontal;
        private ToolStripMenuItem tileVertical;
        private ToolStripMenuItem tileCascade;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem helpAbout;
        private ToolStripMenuItem helpVEControls;
        private ToolStripMenuItem mainmenuEditorMenu;
        private ToolStripMenuItem mainmenuEdit;
        private ToolStripMenuItem mainmenuAddthisMap;
        private ToolStripPanel tsPanelTop;
        private ContextMenuStrip contextMenuStripMenu;
        private ToolStripMenuItem lockToolbarToolStripMenuItem;
        private ToolStripPanel tsPanelBottom;
        private ToolStripMenuItem toolsLatestPlugins;
        private ToolStripMenuItem helpContents;

    }
}

