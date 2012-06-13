namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using HaloMap.RawData;
    using HaloMap.Render;

    public partial class BSPViewer : Form
	{
        public TrackBar speedBar;
        private TD.SandDock.DockContainer bottomSandDock;
        private CheckBox BSPLighting;
        private CheckBox BSPPermutations;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private CheckBox CameraCulling;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckedListBox checkedListBox1;
        private CheckedListBox checkedListBox2;
        private ComboBox comboBox1;
        private System.ComponentModel.IContainer components = null;
        private CheckBox DeselectOne;
        private TD.SandDock.DockControl dockControl1;
        private TD.SandDock.DockControl dockControl2;
        private TD.SandDock.DockControl dockControl3;
        private TD.SandDock.DockControl dockControl4;
        private TD.SandDock.DockControl dockControl5;
        private TD.SandDock.DockControl dockControl6;
        private System.Windows.Forms.Button fcordbutton;
        private GroupBox fcordgb;
        private Label fcordlx;
        private Label fcordly;
        private Label fcordlz;
        private TextBox fcordx;
        private TextBox fcordy;
        private TextBox fcordz;
        private System.Windows.Forms.Button findspawn;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private System.Windows.Forms.ContextMenuStrip identContext;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TD.SandDock.DockContainer leftSandDock;
        private System.Windows.Forms.Button mi4;
        private CheckBox NoCulling;
        List<SpawnLoads.SceneryInfo> ObstacleList; //= new List<SpawnLoads.SceneryInfo>();
        private Panel panel1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private CheckBox RenderSky;
        private TD.SandDock.DockContainer rightSandDock;
        private TD.SandDock.SandDockManager sandDockManager1;
        private System.Windows.Forms.Button SaveChanges;
        List<SpawnLoads.SceneryInfo> SceneryList; //= new List<SpawnLoads.SceneryInfo>();
        private System.Windows.Forms.Button SelectAllSpawns;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectCurrentToolStripMenuItem;
        List<int> SelectedSpawn = new List<int>();
        private System.Windows.Forms.ToolStripMenuItem selectFreezeAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFreezeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectUnFreezeAllMenuItem;
        List<SpawnLoads.SceneryInfo> SoundsList; //= new List<SpawnLoads.SceneryInfo>();
        List<ParsedModel> SpawnModel = new List<ParsedModel>();
        private Label speedLabel;
        private ToolStrip statusStrip;
        private ToolStrip toolStrip;
        private ToolStripLabel toolStripBlankLabel;
        private ToolStripButton toolStripButtonReset;
        private ToolStripDropDownButton ToolStripDropDownButtonRotatePitch;
        private ToolStripDropDownButton toolStripDropDownButtonRotateRoll;
        private ToolStripDropDownButton ToolStripDropDownButtonRotateYaw;
        private ToolStripLabel toolStripLabel2;
        private ToolStripMenuItem ToolStripMenuItemRP180;
        private ToolStripMenuItem ToolStripMenuItemRP45CCW;
        private ToolStripMenuItem ToolStripMenuItemRP45CW;
        private ToolStripMenuItem ToolStripMenuItemRP90CCW;
        private ToolStripMenuItem ToolStripMenuItemRP90CW;
        private ToolStripMenuItem ToolStripMenuItemRR180;
        private ToolStripMenuItem ToolStripMenuItemRR45CCW;
        private ToolStripMenuItem ToolStripMenuItemRR45CW;
        private ToolStripMenuItem ToolStripMenuItemRR90CCW;
        private ToolStripMenuItem ToolStripMenuItemRR90CW;
        private ToolStripMenuItem ToolStripMenuItemRY180;
        private ToolStripMenuItem ToolStripMenuItemRY45CCW;
        private ToolStripMenuItem ToolStripMenuItemRY45CW;
        private ToolStripMenuItem ToolStripMenuItemRY90CCW;
        private ToolStripMenuItem ToolStripMenuItemRY90CW;
        private TD.SandDock.DockContainer topSandDock;
        private TrackBar trackBar1;
        private TrackBar trackBar2;
        private TrackBar trackBar3;
        private TreeView treeView1;
        private ToolStripButton tsButtonType;
        private ToolStripLabel tsLabel1;
        private ToolStripLabel tsLabel2;
        private ToolStripLabel tsLabelCount;
        private ToolStripLabel tsLabelPitch;
        private ToolStripLabel tsLabelRoll;
        private ToolStripLabel tsLabelX;
        private ToolStripLabel tsLabelY;
        private ToolStripLabel tsLabelYaw;
        private ToolStripLabel tsLabelZ;
        private ToolStripTextBox tsTextBoxPitch;
        private ToolStripTextBox tsTextBoxRoll;
        private ToolStripTextBox tsTextBoxX;
        private ToolStripTextBox tsTextBoxY;
        private ToolStripTextBox tsTextBoxYaw;
        private ToolStripTextBox tsTextBoxZ;
        List<SpawnLoads.CollectionInfo> WeaponsList; //= new List<SpawnLoads.CollectionInfo>();

        public new void Dispose()
        {
            //Reset the lightmaps
            Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);
            ReloadFromArray();
            
            treeView1.Dispose();
            // Clear all List<> to avoid memory leaks?? or is this automatic?
            /*
            if (WeaponsList != null) WeaponsList.Clear();
            if (ObstacleList != null) ObstacleList.Clear();
            if (SceneryList != null) SceneryList.Clear();
            if (SoundsList != null) SoundsList.Clear();
            if (SpawnModel != null) SpawnModel.Clear();
            */

            for (int i = 0; i < bsp.Shaders.Shader.Length; i++)
                bsp.Shaders.Shader[i].Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            bsp.Spawns.Spawn.Clear();
            bsp.Dispose();
            bsp = null;
            cam.Dispose();
            cam = null;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BSPViewer));
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsButtonType = new System.Windows.Forms.ToolStripButton();
            this.tsLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsLabelCount = new System.Windows.Forms.ToolStripLabel();
            this.tsLabelX = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxX = new System.Windows.Forms.ToolStripTextBox();
            this.tsLabelY = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxY = new System.Windows.Forms.ToolStripTextBox();
            this.tsLabelZ = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxZ = new System.Windows.Forms.ToolStripTextBox();
            this.tsLabelYaw = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxYaw = new System.Windows.Forms.ToolStripTextBox();
            this.tsLabelPitch = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxPitch = new System.Windows.Forms.ToolStripTextBox();
            this.tsLabelRoll = new System.Windows.Forms.ToolStripLabel();
            this.tsTextBoxRoll = new System.Windows.Forms.ToolStripTextBox();
            this.sandDockManager1 = new TD.SandDock.SandDockManager();
            this.identContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectFreezeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFreezeAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectUnFreezeAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speedBar = new System.Windows.Forms.TrackBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.rightSandDock = new TD.SandDock.DockContainer();
            this.bottomSandDock = new TD.SandDock.DockContainer();
            this.topSandDock = new TD.SandDock.DockContainer();
            this.dockControl6 = new TD.SandDock.DockControl();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.dockControl5 = new TD.SandDock.DockControl();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dockControl4 = new TD.SandDock.DockControl();
            this.fcordgb = new System.Windows.Forms.GroupBox();
            this.fcordlz = new System.Windows.Forms.Label();
            this.fcordly = new System.Windows.Forms.Label();
            this.fcordlx = new System.Windows.Forms.Label();
            this.fcordx = new System.Windows.Forms.TextBox();
            this.fcordbutton = new System.Windows.Forms.Button();
            this.fcordy = new System.Windows.Forms.TextBox();
            this.fcordz = new System.Windows.Forms.TextBox();
            this.dockControl3 = new TD.SandDock.DockControl();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.dockControl2 = new TD.SandDock.DockControl();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.findspawn = new System.Windows.Forms.Button();
            this.mi4 = new System.Windows.Forms.Button();
            this.SelectAllSpawns = new System.Windows.Forms.Button();
            this.dockControl1 = new TD.SandDock.DockControl();
            this.label5 = new System.Windows.Forms.Label();
            this.CameraCulling = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BSPPermutations = new System.Windows.Forms.CheckBox();
            this.BSPLighting = new System.Windows.Forms.CheckBox();
            this.DeselectOne = new System.Windows.Forms.CheckBox();
            this.SaveChanges = new System.Windows.Forms.Button();
            this.NoCulling = new System.Windows.Forms.CheckBox();
            this.RenderSky = new System.Windows.Forms.CheckBox();
            this.leftSandDock = new TD.SandDock.DockContainer();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripBlankLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.ToolStripDropDownButtonRotateYaw = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStripMenuItemRY45CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRY90CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRY180 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRY90CW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRY45CW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripDropDownButtonRotatePitch = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStripMenuItemRP45CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRP90CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRP180 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRP90CW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRP45CW = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonRotateRoll = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStripMenuItemRR45CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRR90CCW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRR180 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRR90CW = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRR45CW = new System.Windows.Forms.ToolStripMenuItem();
            this.cbBSPTextures = new System.Windows.Forms.CheckBox();
            this.statusStrip.SuspendLayout();
            this.identContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
            this.dockControl6.SuspendLayout();
            this.dockControl5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.dockControl4.SuspendLayout();
            this.fcordgb.SuspendLayout();
            this.dockControl3.SuspendLayout();
            this.dockControl2.SuspendLayout();
            this.dockControl1.SuspendLayout();
            this.leftSandDock.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(592, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 28);
            this.label3.TabIndex = 8;
            this.label3.Text = ".:LOADING BSP:.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2});
            this.statusStrip.Location = new System.Drawing.Point(0, 625);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(795, 25);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(78, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // tsLabel1
            // 
            this.tsLabel1.AutoSize = false;
            this.tsLabel1.Name = "tsLabel1";
            this.tsLabel1.Size = new System.Drawing.Size(60, 22);
            this.tsLabel1.Text = "tsLabel1";
            // 
            // tsButtonType
            // 
            this.tsButtonType.AutoSize = false;
            this.tsButtonType.AutoToolTip = false;
            this.tsButtonType.Name = "tsButtonType";
            this.tsButtonType.Size = new System.Drawing.Size(60, 22);
            this.tsButtonType.Text = "tsButtonType";
            this.tsButtonType.Click += new System.EventHandler(this.tsButton_Click);
            // 
            // tsLabel2
            // 
            this.tsLabel2.AutoSize = false;
            this.tsLabel2.Name = "tsLabel2";
            this.tsLabel2.Size = new System.Drawing.Size(30, 22);
            this.tsLabel2.Text = "tsLabel2";
            // 
            // tsLabelCount
            // 
            this.tsLabelCount.Name = "tsLabelCount";
            this.tsLabelCount.Size = new System.Drawing.Size(38, 22);
            this.tsLabelCount.Text = "tsLabelCount";
            // 
            // tsLabelX
            // 
            this.tsLabelX.AutoSize = false;
            this.tsLabelX.Name = "tsLabelX";
            this.tsLabelX.Size = new System.Drawing.Size(33, 22);
            this.tsLabelX.Text = "tsLabelX";
            // 
            // tsTextBoxX
            // 
            this.tsTextBoxX.AutoSize = false;
            this.tsTextBoxX.Name = "tsTextBoxX";
            this.tsTextBoxX.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxX.Text = "tsTextBoxX";
            this.tsTextBoxX.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxX.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxX.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // tsLabelY
            // 
            this.tsLabelY.AutoSize = false;
            this.tsLabelY.Name = "tsLabelY";
            this.tsLabelY.Size = new System.Drawing.Size(28, 22);
            this.tsLabelY.Text = "tsLabelY";
            // 
            // tsTextBoxY
            // 
            this.tsTextBoxY.AutoSize = false;
            this.tsTextBoxY.Name = "tsTextBoxY";
            this.tsTextBoxY.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxY.Text = "tsTextBoxY";
            this.tsTextBoxY.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxY.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxY.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // tsLabelZ
            // 
            this.tsLabelZ.AutoSize = false;
            this.tsLabelZ.Name = "tsLabelZ";
            this.tsLabelZ.Size = new System.Drawing.Size(28, 22);
            this.tsLabelZ.Text = "tsLabelZ";
            // 
            // tsTextBoxZ
            // 
            this.tsTextBoxZ.AutoSize = false;
            this.tsTextBoxZ.Name = "tsTextBoxZ";
            this.tsTextBoxZ.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxZ.Text = "tsTextBoxZ";
            this.tsTextBoxZ.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxZ.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxZ.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // tsLabelYaw
            // 
            this.tsLabelYaw.AutoSize = false;
            this.tsLabelYaw.Name = "tsLabelYaw";
            this.tsLabelYaw.Size = new System.Drawing.Size(28, 22);
            this.tsLabelYaw.Text = "tsLabelYaw";
            // 
            // tsTextBoxYaw
            // 
            this.tsTextBoxYaw.AutoSize = false;
            this.tsTextBoxYaw.Name = "tsTextBoxYaw";
            this.tsTextBoxYaw.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxYaw.Text = "tsTextBoxYaw";
            this.tsTextBoxYaw.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxYaw.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxYaw.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // tsLabelPitch
            // 
            this.tsLabelPitch.AutoSize = false;
            this.tsLabelPitch.Name = "tsLabelPitch";
            this.tsLabelPitch.Size = new System.Drawing.Size(28, 22);
            this.tsLabelPitch.Text = "tsLabelPitch";
            // 
            // tsTextBoxPitch
            // 
            this.tsTextBoxPitch.AutoSize = false;
            this.tsTextBoxPitch.Name = "tsTextBoxPitch";
            this.tsTextBoxPitch.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxPitch.Text = "tsTextBoxPitch";
            this.tsTextBoxPitch.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxPitch.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxPitch.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // tsLabelRoll
            // 
            this.tsLabelRoll.AutoSize = false;
            this.tsLabelRoll.Name = "tsLabelRoll";
            this.tsLabelRoll.Size = new System.Drawing.Size(28, 22);
            this.tsLabelRoll.Text = "tsLabelRoll";
            // 
            // tsTextBoxRoll
            // 
            this.tsTextBoxRoll.AutoSize = false;
            this.tsTextBoxRoll.Name = "tsTextBoxRoll";
            this.tsTextBoxRoll.Size = new System.Drawing.Size(45, 22);
            this.tsTextBoxRoll.Text = "tsTextBoxRoll";
            this.tsTextBoxRoll.LostFocus += new System.EventHandler(this.tsTextBox_LostFocus);
            this.tsTextBoxRoll.GotFocus += new System.EventHandler(this.tsTextBox_GotFocus);
            this.tsTextBoxRoll.TextChanged += new System.EventHandler(this.tsTextBox_Change);
            // 
            // sandDockManager1
            // 
            this.sandDockManager1.OwnerForm = this;
            // 
            // identContext
            // 
            this.identContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFreezeMenuItem,
            this.selectFreezeAllMenuItem,
            this.selectUnFreezeAllMenuItem,
            this.selectCurrentToolStripMenuItem,
            this.selectGroupToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem});
            this.identContext.Name = "identContext";
            this.identContext.Size = new System.Drawing.Size(189, 158);
            this.identContext.Opening += new System.ComponentModel.CancelEventHandler(this.identContext_Opening);
            // 
            // selectFreezeMenuItem
            // 
            this.selectFreezeMenuItem.Name = "selectFreezeMenuItem";
            this.selectFreezeMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectFreezeMenuItem.Text = "Freeze";
            this.selectFreezeMenuItem.Visible = false;
            this.selectFreezeMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectFreezeAllMenuItem
            // 
            this.selectFreezeAllMenuItem.Name = "selectFreezeAllMenuItem";
            this.selectFreezeAllMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectFreezeAllMenuItem.Text = "Freeze All Unselected";
            this.selectFreezeAllMenuItem.Visible = false;
            this.selectFreezeAllMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectUnFreezeAllMenuItem
            // 
            this.selectUnFreezeAllMenuItem.Name = "selectUnFreezeAllMenuItem";
            this.selectUnFreezeAllMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectUnFreezeAllMenuItem.Text = "Unfreeze All";
            this.selectUnFreezeAllMenuItem.Visible = false;
            this.selectUnFreezeAllMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectCurrentToolStripMenuItem
            // 
            this.selectCurrentToolStripMenuItem.Name = "selectCurrentToolStripMenuItem";
            this.selectCurrentToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectCurrentToolStripMenuItem.Text = "Select";
            this.selectCurrentToolStripMenuItem.Visible = false;
            this.selectCurrentToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectGroupToolStripMenuItem
            // 
            this.selectGroupToolStripMenuItem.Name = "selectGroupToolStripMenuItem";
            this.selectGroupToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectGroupToolStripMenuItem.Text = "Select ...";
            this.selectGroupToolStripMenuItem.Visible = false;
            this.selectGroupToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectAllToolStripMenuItem.Text = "Select ...";
            this.selectAllToolStripMenuItem.Visible = false;
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.selectNoneToolStripMenuItem.Tag = "DES-1";
            this.selectNoneToolStripMenuItem.Text = "Deselect All";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // speedBar
            // 
            this.speedBar.Location = new System.Drawing.Point(757, 13);
            this.speedBar.Maximum = 150;
            this.speedBar.Minimum = 1;
            this.speedBar.Name = "speedBar";
            this.speedBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.speedBar.Size = new System.Drawing.Size(45, 200);
            this.speedBar.TabIndex = 18;
            this.speedBar.TickFrequency = 5;
            this.speedBar.Value = 1;
            this.speedBar.ValueChanged += new System.EventHandler(this.speedBar_ValueChanged);
            this.speedBar.Scroll += new System.EventHandler(this.speedBar_Scroll);
            this.speedBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.speedBar_KeyDown);
            // 
            // speedLabel
            // 
            this.speedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedLabel.Location = new System.Drawing.Point(757, 0);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(38, 13);
            this.speedLabel.TabIndex = 19;
            this.speedLabel.Text = "speed";
            this.speedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rightSandDock
            // 
            this.rightSandDock.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightSandDock.Guid = new System.Guid("63cd0407-8e90-4f94-bab3-8ea89fde67af");
            this.rightSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
            this.rightSandDock.Location = new System.Drawing.Point(795, 0);
            this.rightSandDock.Manager = this.sandDockManager1;
            this.rightSandDock.Name = "rightSandDock";
            this.rightSandDock.Size = new System.Drawing.Size(0, 625);
            this.rightSandDock.TabIndex = 14;
            // 
            // bottomSandDock
            // 
            this.bottomSandDock.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomSandDock.Guid = new System.Guid("8f427dca-24ab-40d5-9b32-732d9d46a574");
            this.bottomSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
            this.bottomSandDock.Location = new System.Drawing.Point(0, 625);
            this.bottomSandDock.Manager = this.sandDockManager1;
            this.bottomSandDock.Name = "bottomSandDock";
            this.bottomSandDock.Size = new System.Drawing.Size(795, 0);
            this.bottomSandDock.TabIndex = 15;
            // 
            // topSandDock
            // 
            this.topSandDock.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSandDock.Guid = new System.Guid("53192ca4-cccc-4c39-bb57-5e16efa82d64");
            this.topSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
            this.topSandDock.Location = new System.Drawing.Point(254, 0);
            this.topSandDock.Manager = this.sandDockManager1;
            this.topSandDock.Name = "topSandDock";
            this.topSandDock.Size = new System.Drawing.Size(541, 0);
            this.topSandDock.TabIndex = 16;
            // 
            // dockControl6
            // 
            this.dockControl6.Controls.Add(this.button5);
            this.dockControl6.Controls.Add(this.button4);
            this.dockControl6.Controls.Add(this.checkedListBox2);
            this.dockControl6.Guid = new System.Guid("8a474c02-64bc-429b-8d89-83c818109d18");
            this.dockControl6.Location = new System.Drawing.Point(0, 18);
            this.dockControl6.Name = "dockControl6";
            this.dockControl6.Size = new System.Drawing.Size(250, 584);
            this.dockControl6.TabIndex = 5;
            this.dockControl6.Text = "BSP Selections";
            this.dockControl6.Enter += new System.EventHandler(this.dockControl6_Enter);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(90, 7);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(76, 27);
            this.button5.TabIndex = 2;
            this.button5.Text = "Select None";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 7);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 27);
            this.button4.TabIndex = 1;
            this.button4.Text = "Select All";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(0, 40);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(250, 529);
            this.checkedListBox2.TabIndex = 0;
            this.checkedListBox2.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox2_ItemCheck);
            // 
            // dockControl5
            // 
            this.dockControl5.Controls.Add(this.button3);
            this.dockControl5.Controls.Add(this.groupBox2);
            this.dockControl5.Controls.Add(this.panel1);
            this.dockControl5.Controls.Add(this.groupBox1);
            this.dockControl5.Guid = new System.Guid("f641181f-8274-43e0-8421-3f46471b23a2");
            this.dockControl5.Location = new System.Drawing.Point(0, 18);
            this.dockControl5.Name = "dockControl5";
            this.dockControl5.Size = new System.Drawing.Size(250, 584);
            this.dockControl5.TabIndex = 4;
            this.dockControl5.Text = "Lightmap Palettes";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(29, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(193, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Apply";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.trackBar3);
            this.groupBox2.Controls.Add(this.trackBar2);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Location = new System.Drawing.Point(10, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 178);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Color Adjustment";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(110, 149);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Preview";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 149);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(73, 129);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(63, 17);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Colorize";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(119, 106);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(91, 17);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.Text = "Color Balance";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(20, 106);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(98, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Hue\\Saturation";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(6, 78);
            this.trackBar3.Maximum = 100;
            this.trackBar3.Minimum = -100;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(219, 45);
            this.trackBar3.TabIndex = 3;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(6, 49);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Minimum = -100;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(219, 45);
            this.trackBar2.TabIndex = 2;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(6, 19);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = -100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(219, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(10, 229);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 224);
            this.panel1.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(10, 459);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 100);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lightmap Chunk Selection";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 33);
            this.label2.TabIndex = 7;
            this.label2.Text = "Choose what chunk you would like to edit for the lightmap";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Chunk:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(63, 75);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(150, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(19, 52);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(58, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Edit All";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // dockControl4
            // 
            this.dockControl4.Closable = false;
            this.dockControl4.Controls.Add(this.fcordgb);
            this.dockControl4.Guid = new System.Guid("a94fefcb-cae4-4234-86b6-d85bc85e0023");
            this.dockControl4.Location = new System.Drawing.Point(0, 18);
            this.dockControl4.Name = "dockControl4";
            this.dockControl4.Size = new System.Drawing.Size(250, 584);
            this.dockControl4.TabIndex = 3;
            this.dockControl4.Text = "Tools";
            // 
            // fcordgb
            // 
            this.fcordgb.Controls.Add(this.fcordlz);
            this.fcordgb.Controls.Add(this.fcordly);
            this.fcordgb.Controls.Add(this.fcordlx);
            this.fcordgb.Controls.Add(this.fcordx);
            this.fcordgb.Controls.Add(this.fcordbutton);
            this.fcordgb.Controls.Add(this.fcordy);
            this.fcordgb.Controls.Add(this.fcordz);
            this.fcordgb.Location = new System.Drawing.Point(3, 3);
            this.fcordgb.Name = "fcordgb";
            this.fcordgb.Size = new System.Drawing.Size(244, 129);
            this.fcordgb.TabIndex = 5;
            this.fcordgb.TabStop = false;
            this.fcordgb.Text = "Coordinate Finder";
            // 
            // fcordlz
            // 
            this.fcordlz.AutoSize = true;
            this.fcordlz.Location = new System.Drawing.Point(25, 74);
            this.fcordlz.Name = "fcordlz";
            this.fcordlz.Size = new System.Drawing.Size(17, 13);
            this.fcordlz.TabIndex = 6;
            this.fcordlz.Text = "Z:";
            // 
            // fcordly
            // 
            this.fcordly.AutoSize = true;
            this.fcordly.Location = new System.Drawing.Point(25, 48);
            this.fcordly.Name = "fcordly";
            this.fcordly.Size = new System.Drawing.Size(17, 13);
            this.fcordly.TabIndex = 5;
            this.fcordly.Text = "Y:";
            // 
            // fcordlx
            // 
            this.fcordlx.AutoSize = true;
            this.fcordlx.Location = new System.Drawing.Point(25, 22);
            this.fcordlx.Name = "fcordlx";
            this.fcordlx.Size = new System.Drawing.Size(17, 13);
            this.fcordlx.TabIndex = 4;
            this.fcordlx.Text = "X:";
            // 
            // fcordx
            // 
            this.fcordx.Location = new System.Drawing.Point(48, 19);
            this.fcordx.Name = "fcordx";
            this.fcordx.Size = new System.Drawing.Size(152, 20);
            this.fcordx.TabIndex = 0;
            this.fcordx.Text = "0";
            // 
            // fcordbutton
            // 
            this.fcordbutton.Location = new System.Drawing.Point(74, 97);
            this.fcordbutton.Name = "fcordbutton";
            this.fcordbutton.Size = new System.Drawing.Size(75, 23);
            this.fcordbutton.TabIndex = 3;
            this.fcordbutton.Text = "Find!";
            this.fcordbutton.UseVisualStyleBackColor = true;
            this.fcordbutton.Click += new System.EventHandler(this.fcordbutton_Click);
            // 
            // fcordy
            // 
            this.fcordy.Location = new System.Drawing.Point(48, 45);
            this.fcordy.Name = "fcordy";
            this.fcordy.Size = new System.Drawing.Size(152, 20);
            this.fcordy.TabIndex = 1;
            this.fcordy.Text = "0";
            // 
            // fcordz
            // 
            this.fcordz.Location = new System.Drawing.Point(48, 71);
            this.fcordz.Name = "fcordz";
            this.fcordz.Size = new System.Drawing.Size(152, 20);
            this.fcordz.TabIndex = 2;
            this.fcordz.Text = "0";
            // 
            // dockControl3
            // 
            this.dockControl3.Closable = false;
            this.dockControl3.Controls.Add(this.treeView1);
            this.dockControl3.Guid = new System.Guid("99e2161d-408c-498f-acde-40db65d6db24");
            this.dockControl3.Location = new System.Drawing.Point(0, 18);
            this.dockControl3.Name = "dockControl3";
            this.dockControl3.Size = new System.Drawing.Size(250, 584);
            this.dockControl3.TabIndex = 2;
            this.dockControl3.Text = "Spawns";
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.identContext;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(250, 584);
            this.treeView1.TabIndex = 1;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            // 
            // dockControl2
            // 
            this.dockControl2.Closable = false;
            this.dockControl2.Controls.Add(this.checkBox4);
            this.dockControl2.Controls.Add(this.checkBox3);
            this.dockControl2.Controls.Add(this.checkedListBox1);
            this.dockControl2.Controls.Add(this.findspawn);
            this.dockControl2.Controls.Add(this.mi4);
            this.dockControl2.Controls.Add(this.SelectAllSpawns);
            this.dockControl2.Guid = new System.Guid("3838baa4-58e1-4863-b0d8-82180bab7409");
            this.dockControl2.Location = new System.Drawing.Point(0, 18);
            this.dockControl2.Name = "dockControl2";
            this.dockControl2.Size = new System.Drawing.Size(250, 584);
            this.dockControl2.TabIndex = 1;
            this.dockControl2.Text = "Edit";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(23, 350);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(184, 17);
            this.checkBox4.TabIndex = 8;
            this.checkBox4.Text = "Auto select item on treeview click";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(15, 107);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(104, 17);
            this.checkBox3.TabIndex = 7;
            this.checkBox3.Text = "View All Spawns";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 130);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(225, 214);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SpawnList_Check);
            // 
            // findspawn
            // 
            this.findspawn.Location = new System.Drawing.Point(23, 72);
            this.findspawn.Name = "findspawn";
            this.findspawn.Size = new System.Drawing.Size(175, 23);
            this.findspawn.TabIndex = 6;
            this.findspawn.Text = "Find First Selected Spawn";
            this.findspawn.UseVisualStyleBackColor = true;
            this.findspawn.Click += new System.EventHandler(this.findspawn_Click);
            // 
            // mi4
            // 
            this.mi4.Location = new System.Drawing.Point(23, 43);
            this.mi4.Name = "mi4";
            this.mi4.Size = new System.Drawing.Size(175, 23);
            this.mi4.TabIndex = 5;
            this.mi4.Text = "Move All Spawns Here";
            this.mi4.UseVisualStyleBackColor = true;
            this.mi4.Click += new System.EventHandler(this.mi4_Click);
            // 
            // SelectAllSpawns
            // 
            this.SelectAllSpawns.Location = new System.Drawing.Point(23, 14);
            this.SelectAllSpawns.Name = "SelectAllSpawns";
            this.SelectAllSpawns.Size = new System.Drawing.Size(175, 23);
            this.SelectAllSpawns.TabIndex = 4;
            this.SelectAllSpawns.Text = "Select All Spawns";
            this.SelectAllSpawns.UseVisualStyleBackColor = true;
            this.SelectAllSpawns.Click += new System.EventHandler(this.SelectAllSpawns_Click);
            // 
            // dockControl1
            // 
            this.dockControl1.Closable = false;
            this.dockControl1.Controls.Add(this.cbBSPTextures);
            this.dockControl1.Controls.Add(this.label5);
            this.dockControl1.Controls.Add(this.CameraCulling);
            this.dockControl1.Controls.Add(this.label4);
            this.dockControl1.Controls.Add(this.BSPPermutations);
            this.dockControl1.Controls.Add(this.BSPLighting);
            this.dockControl1.Controls.Add(this.DeselectOne);
            this.dockControl1.Controls.Add(this.SaveChanges);
            this.dockControl1.Controls.Add(this.NoCulling);
            this.dockControl1.Controls.Add(this.RenderSky);
            this.dockControl1.Guid = new System.Guid("baf66d5e-5ae2-42bd-a116-5b600f86e7af");
            this.dockControl1.Location = new System.Drawing.Point(0, 18);
            this.dockControl1.Name = "dockControl1";
            this.dockControl1.Size = new System.Drawing.Size(250, 584);
            this.dockControl1.TabIndex = 0;
            this.dockControl1.Text = "Main";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 238);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(214, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Enable options below to speed up rendering";
            // 
            // CameraCulling
            // 
            this.CameraCulling.AutoSize = true;
            this.CameraCulling.Location = new System.Drawing.Point(25, 264);
            this.CameraCulling.Name = "CameraCulling";
            this.CameraCulling.Size = new System.Drawing.Size(207, 17);
            this.CameraCulling.TabIndex = 7;
            this.CameraCulling.Text = "Perform Camera Culling (slight glitches)";
            this.CameraCulling.UseVisualStyleBackColor = true;
            this.CameraCulling.CheckedChanged += new System.EventHandler(this.CameraCulling_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(216, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Disable options below to speed up rendering";
            // 
            // BSPPermutations
            // 
            this.BSPPermutations.AutoSize = true;
            this.BSPPermutations.Checked = true;
            this.BSPPermutations.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BSPPermutations.Location = new System.Drawing.Point(25, 185);
            this.BSPPermutations.Name = "BSPPermutations";
            this.BSPPermutations.Size = new System.Drawing.Size(141, 17);
            this.BSPPermutations.TabIndex = 5;
            this.BSPPermutations.Text = "Show BSP Permutations";
            this.BSPPermutations.UseVisualStyleBackColor = true;
            this.BSPPermutations.CheckedChanged += new System.EventHandler(this.BSPPermutations_CheckedChanged);
            // 
            // BSPLighting
            // 
            this.BSPLighting.AutoSize = true;
            this.BSPLighting.Checked = true;
            this.BSPLighting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BSPLighting.Location = new System.Drawing.Point(25, 162);
            this.BSPLighting.Name = "BSPLighting";
            this.BSPLighting.Size = new System.Drawing.Size(117, 17);
            this.BSPLighting.TabIndex = 4;
            this.BSPLighting.Text = "Show BSP Lighting";
            this.BSPLighting.UseVisualStyleBackColor = true;
            this.BSPLighting.CheckedChanged += new System.EventHandler(this.BSPLighting_CheckedChanged);
            // 
            // DeselectOne
            // 
            this.DeselectOne.AutoSize = true;
            this.DeselectOne.Location = new System.Drawing.Point(23, 57);
            this.DeselectOne.Name = "DeselectOne";
            this.DeselectOne.Size = new System.Drawing.Size(164, 17);
            this.DeselectOne.TabIndex = 3;
            this.DeselectOne.Text = "Deselect one object at a time";
            this.DeselectOne.UseVisualStyleBackColor = true;
            // 
            // SaveChanges
            // 
            this.SaveChanges.Location = new System.Drawing.Point(12, 17);
            this.SaveChanges.Name = "SaveChanges";
            this.SaveChanges.Size = new System.Drawing.Size(215, 23);
            this.SaveChanges.TabIndex = 1;
            this.SaveChanges.Text = "Save Changes";
            this.SaveChanges.UseVisualStyleBackColor = true;
            this.SaveChanges.Click += new System.EventHandler(this.SaveChanges_Click);
            // 
            // NoCulling
            // 
            this.NoCulling.AutoSize = true;
            this.NoCulling.Location = new System.Drawing.Point(25, 119);
            this.NoCulling.Name = "NoCulling";
            this.NoCulling.Size = new System.Drawing.Size(163, 17);
            this.NoCulling.TabIndex = 2;
            this.NoCulling.Text = "Show backfaces (No Culling)";
            this.NoCulling.UseVisualStyleBackColor = true;
            // 
            // RenderSky
            // 
            this.RenderSky.AutoSize = true;
            this.RenderSky.Location = new System.Drawing.Point(25, 139);
            this.RenderSky.Name = "RenderSky";
            this.RenderSky.Size = new System.Drawing.Size(149, 17);
            this.RenderSky.TabIndex = 2;
            this.RenderSky.Text = "<Attempt To> Render Sky";
            this.RenderSky.UseVisualStyleBackColor = true;
            // 
            // leftSandDock
            // 
            this.leftSandDock.Controls.Add(this.dockControl1);
            this.leftSandDock.Controls.Add(this.dockControl2);
            this.leftSandDock.Controls.Add(this.dockControl3);
            this.leftSandDock.Controls.Add(this.dockControl4);
            this.leftSandDock.Controls.Add(this.dockControl5);
            this.leftSandDock.Controls.Add(this.dockControl6);
            this.leftSandDock.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftSandDock.Guid = new System.Guid("7b82af44-7394-4006-9310-e7e7e6293930");
            this.leftSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400, System.Windows.Forms.Orientation.Horizontal, new TD.SandDock.LayoutSystemBase[] {
            ((TD.SandDock.LayoutSystemBase)(new TD.SandDock.ControlLayoutSystem(250, 625, new TD.SandDock.DockControl[] {
                        this.dockControl1,
                        this.dockControl2,
                        this.dockControl3,
                        this.dockControl4,
                        this.dockControl5,
                        this.dockControl6}, this.dockControl1)))});
            this.leftSandDock.Location = new System.Drawing.Point(0, 0);
            this.leftSandDock.Manager = this.sandDockManager1;
            this.leftSandDock.Name = "leftSandDock";
            this.leftSandDock.Size = new System.Drawing.Size(254, 625);
            this.leftSandDock.TabIndex = 13;
            // 
            // toolStrip
            // 
            this.toolStrip.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBlankLabel,
            this.toolStripButtonReset,
            this.ToolStripDropDownButtonRotateYaw,
            this.ToolStripDropDownButtonRotatePitch,
            this.toolStripDropDownButtonRotateRoll});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(254, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(541, 23);
            this.toolStrip.TabIndex = 21;
            this.toolStrip.Text = "toolStrip1";
            this.toolStrip.Visible = false;
            // 
            // toolStripBlankLabel
            // 
            this.toolStripBlankLabel.AutoSize = false;
            this.toolStripBlankLabel.Name = "toolStripBlankLabel";
            this.toolStripBlankLabel.Size = new System.Drawing.Size(20, 0);
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonReset.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReset.Image")));
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(44, 20);
            this.toolStripButtonReset.Text = "Reset";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // ToolStripDropDownButtonRotateYaw
            // 
            this.ToolStripDropDownButtonRotateYaw.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripDropDownButtonRotateYaw.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemRY45CCW,
            this.ToolStripMenuItemRY90CCW,
            this.ToolStripMenuItemRY180,
            this.ToolStripMenuItemRY90CW,
            this.ToolStripMenuItemRY45CW});
            this.ToolStripDropDownButtonRotateYaw.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripDropDownButtonRotateYaw.Image")));
            this.ToolStripDropDownButtonRotateYaw.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripDropDownButtonRotateYaw.Name = "ToolStripDropDownButtonRotateYaw";
            this.ToolStripDropDownButtonRotateYaw.Size = new System.Drawing.Size(86, 20);
            this.ToolStripDropDownButtonRotateYaw.Text = "Rotate Yaw";
            // 
            // ToolStripMenuItemRY45CCW
            // 
            this.ToolStripMenuItemRY45CCW.Name = "ToolStripMenuItemRY45CCW";
            this.ToolStripMenuItemRY45CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRY45CCW.Text = "45* CCW";
            this.ToolStripMenuItemRY45CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRY90CCW
            // 
            this.ToolStripMenuItemRY90CCW.Name = "ToolStripMenuItemRY90CCW";
            this.ToolStripMenuItemRY90CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRY90CCW.Text = "90* CCW";
            this.ToolStripMenuItemRY90CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRY180
            // 
            this.ToolStripMenuItemRY180.Name = "ToolStripMenuItemRY180";
            this.ToolStripMenuItemRY180.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRY180.Text = "180*";
            this.ToolStripMenuItemRY180.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRY90CW
            // 
            this.ToolStripMenuItemRY90CW.Name = "ToolStripMenuItemRY90CW";
            this.ToolStripMenuItemRY90CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRY90CW.Text = "90* CW";
            this.ToolStripMenuItemRY90CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRY45CW
            // 
            this.ToolStripMenuItemRY45CW.Name = "ToolStripMenuItemRY45CW";
            this.ToolStripMenuItemRY45CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRY45CW.Text = "45* CW";
            this.ToolStripMenuItemRY45CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripDropDownButtonRotatePitch
            // 
            this.ToolStripDropDownButtonRotatePitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripDropDownButtonRotatePitch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemRP45CCW,
            this.ToolStripMenuItemRP90CCW,
            this.ToolStripMenuItemRP180,
            this.ToolStripMenuItemRP90CW,
            this.ToolStripMenuItemRP45CW});
            this.ToolStripDropDownButtonRotatePitch.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripDropDownButtonRotatePitch.Image")));
            this.ToolStripDropDownButtonRotatePitch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripDropDownButtonRotatePitch.Name = "ToolStripDropDownButtonRotatePitch";
            this.ToolStripDropDownButtonRotatePitch.Size = new System.Drawing.Size(89, 20);
            this.ToolStripDropDownButtonRotatePitch.Text = "Rotate Pitch";
            // 
            // ToolStripMenuItemRP45CCW
            // 
            this.ToolStripMenuItemRP45CCW.Name = "ToolStripMenuItemRP45CCW";
            this.ToolStripMenuItemRP45CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRP45CCW.Text = "45* CCW";
            this.ToolStripMenuItemRP45CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRP90CCW
            // 
            this.ToolStripMenuItemRP90CCW.Name = "ToolStripMenuItemRP90CCW";
            this.ToolStripMenuItemRP90CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRP90CCW.Text = "90* CCW";
            this.ToolStripMenuItemRP90CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRP180
            // 
            this.ToolStripMenuItemRP180.Name = "ToolStripMenuItemRP180";
            this.ToolStripMenuItemRP180.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRP180.Text = "180*";
            this.ToolStripMenuItemRP180.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRP90CW
            // 
            this.ToolStripMenuItemRP90CW.Name = "ToolStripMenuItemRP90CW";
            this.ToolStripMenuItemRP90CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRP90CW.Text = "90* CW";
            this.ToolStripMenuItemRP90CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRP45CW
            // 
            this.ToolStripMenuItemRP45CW.Name = "ToolStripMenuItemRP45CW";
            this.ToolStripMenuItemRP45CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRP45CW.Text = "45* CW";
            this.ToolStripMenuItemRP45CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // toolStripDropDownButtonRotateRoll
            // 
            this.toolStripDropDownButtonRotateRoll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonRotateRoll.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemRR45CCW,
            this.ToolStripMenuItemRR90CCW,
            this.ToolStripMenuItemRR180,
            this.ToolStripMenuItemRR90CW,
            this.ToolStripMenuItemRR45CW});
            this.toolStripDropDownButtonRotateRoll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonRotateRoll.Image")));
            this.toolStripDropDownButtonRotateRoll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonRotateRoll.Name = "toolStripDropDownButtonRotateRoll";
            this.toolStripDropDownButtonRotateRoll.Size = new System.Drawing.Size(83, 20);
            this.toolStripDropDownButtonRotateRoll.Text = "Rotate Roll";
            // 
            // ToolStripMenuItemRR45CCW
            // 
            this.ToolStripMenuItemRR45CCW.Name = "ToolStripMenuItemRR45CCW";
            this.ToolStripMenuItemRR45CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRR45CCW.Text = "45* CCW";
            this.ToolStripMenuItemRR45CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRR90CCW
            // 
            this.ToolStripMenuItemRR90CCW.Name = "ToolStripMenuItemRR90CCW";
            this.ToolStripMenuItemRR90CCW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRR90CCW.Text = "90* CCW";
            this.ToolStripMenuItemRR90CCW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRR180
            // 
            this.ToolStripMenuItemRR180.Name = "ToolStripMenuItemRR180";
            this.ToolStripMenuItemRR180.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRR180.Text = "180*";
            this.ToolStripMenuItemRR180.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRR90CW
            // 
            this.ToolStripMenuItemRR90CW.Name = "ToolStripMenuItemRR90CW";
            this.ToolStripMenuItemRR90CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRR90CW.Text = "90* CW";
            this.ToolStripMenuItemRR90CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // ToolStripMenuItemRR45CW
            // 
            this.ToolStripMenuItemRR45CW.Name = "ToolStripMenuItemRR45CW";
            this.ToolStripMenuItemRR45CW.Size = new System.Drawing.Size(143, 22);
            this.ToolStripMenuItemRR45CW.Text = "45* CW";
            this.ToolStripMenuItemRR45CW.Click += new System.EventHandler(this.ToolStripMenuItemRotate_Click);
            // 
            // cbBSPTextures
            // 
            this.cbBSPTextures.AutoSize = true;
            this.cbBSPTextures.Checked = true;
            this.cbBSPTextures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBSPTextures.Location = new System.Drawing.Point(25, 208);
            this.cbBSPTextures.Name = "cbBSPTextures";
            this.cbBSPTextures.Size = new System.Drawing.Size(121, 17);
            this.cbBSPTextures.TabIndex = 9;
            this.cbBSPTextures.Text = "Show BSP Textures";
            this.cbBSPTextures.UseVisualStyleBackColor = true;
            this.cbBSPTextures.CheckedChanged += new System.EventHandler(this.cbBSPTextures_CheckedChanged);
            // 
            // BSPViewer
            // 
            this.ClientSize = new System.Drawing.Size(795, 650);
            this.ContextMenuStrip = this.identContext;
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.speedBar);
            this.Controls.Add(this.topSandDock);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.leftSandDock);
            this.Controls.Add(this.rightSandDock);
            this.Controls.Add(this.bottomSandDock);
            this.Controls.Add(this.statusStrip);
            this.Name = "BSPViewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseEnter += new System.EventHandler(this.leftSandDock_MouseLeave);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.identContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.dockControl6.ResumeLayout(false);
            this.dockControl5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dockControl4.ResumeLayout(false);
            this.fcordgb.ResumeLayout(false);
            this.fcordgb.PerformLayout();
            this.dockControl3.ResumeLayout(false);
            this.dockControl2.ResumeLayout(false);
            this.dockControl2.PerformLayout();
            this.dockControl1.ResumeLayout(false);
            this.dockControl1.PerformLayout();
            this.leftSandDock.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label label5;
        private CheckBox cbBSPTextures;
	}
}

