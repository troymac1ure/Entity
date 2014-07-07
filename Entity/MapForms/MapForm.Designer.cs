namespace entity.MapForms
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using entity.HexEditor;
    using Be.Windows.Forms;

    public partial class MapForm : System.Windows.Forms.Form
    {

        private IContainer components;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SaveFileDialog saveMetaDialog;
        private System.Windows.Forms.OpenFileDialog loadMetaDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.OpenFileDialog openInfoFileDialog;
        private System.Windows.Forms.ContextMenu displayMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem ShowReflex;
        private System.Windows.Forms.MenuItem ShowString;
        private System.Windows.Forms.MenuItem ShowIdent;
        private System.Windows.Forms.MenuItem SwapItem;
        private System.Windows.Forms.MenuItem FloodfillSwapItem;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar progressbar;
        private ToolStripStatusLabel statusbar;
        private ListView references;
        private ColumnHeader refs;
        private Panel panel2;
        public PictureBox pictureBox1;
        private RichTextBox secondaryMagicBox;
        private RichTextBox primaryMagicBox;
        private Label label4;
        private Label label6;
        private Panel panel3;
        private CheckBox recursiveCheckBox;
        private CheckBox parsedCheckBox;
        private Button loadMetaButton;
        private Button saveMetaButton;
        private RichTextBox metaTypeBox;
        private Label label2;
        private RichTextBox metaIdentBox;
        private RichTextBox metaSizeBox;
        private RichTextBox metaOffsetBox;
        private Label label5;
        private Label label3;
        private Label label1;
        private Button signMapButton;
        private Button analyzeMapButton;
        private Button buildButton;
        public SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private Panel MetaEditorPanel;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem referenceEditorToolStripMenuItem;
        private ToolStripDropDownButton toolStripDropDownButton2;
        private ToolStripMenuItem metaTreeToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem metaEditorToolStripMenuItem;
        private ToolStripProgressBar toolStripProgressBar1;
        private MenuItem chunkclone;
        private ContextMenuStrip treeviewcontext;
        private ToolStripMenuItem setActiveMatgToolStripMenuItem;
        private ToolStripMenuItem setActiveScnrToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem duplicateToolStripMenuItem;
        private ToolStripMenuItem duplicateRecursivelyToolStripMenuItem;
        private ToolStripMenuItem hexEditorToolStripMenuItem;
        private ToolStripMenuItem sortByToolStripMenuItem;
        private ToolStripMenuItem tagTypeToolStripMenuItem;
        private ToolStripMenuItem folderHierarchyToolStripMenuItem;
        private ToolStripMenuItem rebuilderToolStripMenuItem;
        private CheckBox soundsCheckBox;
        private ToolStripMenuItem overwriteMetaToolStripMenuItem;
        private ContextMenuStrip BitmapContextStrip;
        private ToolStripMenuItem saveBitmapToolStripMenuItem;
        private SaveFileDialog saveBitmapDialog1;
        private ToolStripDropDownButton rawDataDropDown;
        private ContextMenuStrip ModelContextStrip;
        private ToolStripMenuItem exportModelToolStripMenuItem;
        private ContextMenuStrip BSPcontextMenu;
        private ToolStripMenuItem exportOBJToolStripMenuItem;
        private ContextMenuStrip collContextMenu;
        private ToolStripMenuItem extractMeshToOBJToolStripMenuItem;
        private ToolStripMenuItem collisonViewerToolStripMenuItem;
        private CheckBox scanbspwithifp;
        private ToolStripMenuItem exportMeshToolStripMenuItem;
        private ToolStripMenuItem expandMeshX3ToolStripMenuItem;
        private SaveFileDialog saveFileDialog1;
        private ToolStripMenuItem toOBJToolStripMenuItem;
        private ToolStripMenuItem toXToolStripMenuItem;
        private ToolStripMenuItem bSPCollisionToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem exportCollisonToOBJToolStripMenuItem;
        private ToolStripMenuItem bspcollisionViewerToolStripMenuItem;
        private ToolStripMenuItem makeBSPStickyToolStripMenuItem;
        private ToolStripMenuItem fixSystemLinkToolStripMenuItem;
        private ToolStripMenuItem injectMeshesToolStripMenuItem;
        private ToolStripMenuItem injectOBJToolStripMenuItem;
        private ToolStripMenuItem convertCEToH2ToolStripMenuItem;
        private Panel LibraryPanel;
        private Button button1;
        private ToolStripMenuItem pluginsToolStripMenuItem;
        private ToolStripMenuItem injectBitmapToolStripMenuItem;
        private ToolStripMenuItem convertToBumpMapToolStripMenuItem;
        private ToolStripMenuItem injectBSPVisualMeshToolStripMenuItem;
        private ToolStripMenuItem exportMeshToOBJToolStripMenuItem;
        private ContextMenuStrip prtmcontext;
        private ToolStripMenuItem viewPRTMToolStripMenuItem;
        private ToolStripMenuItem extractPRTMOBJToolStripMenuItem;
        private OpenFileDialog openBitmapDialog1;
        private ToolStripMenuItem viewBSPToolStripMenuItem;
        private ToolStripMenuItem viewModelToolStripMenuItem;
        private Panel ltmpTools;
        private ToolStripMenuItem bitmapEditorToolStripMenuItem;
        private ToolStripMenuItem exportScriptsToolStripMenuItem;
        private ToolStrip toolStrip2;
        private ToolStripButton toolStripTagView;
        private ToolStripButton toolStripFolderView;
        private ToolStripButton toolStripInfoView;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripBSPEditor;
        private ToolStripDropDownButton toolStripBSPEditorDropDown;
        private SplitContainer splitContainer3;
        private TextBox searchTextBox;
        private ComboBox searchComboBox;
        private Button searchButton;
        private Label searchLabel;
        private GroupBox searchGroupBox;
        private ToolStripMenuItem removeFromQuickListToolStripMenuItem;
        private ToolStripMenuItem clearTagQuickListToolStripMenuItem;
        private ToolStripMenuItem outputListToFileToolStripMenuItem;
        private ToolStripDropDownButton toolStripHistoryList;
        private Button buttonInternalize;
        private Button button3;
        private Button button2;
        public ComboBox comboBox1;
        private Panel LowerOptionsBar;
        private Button buttonLowerOptions;
        private SplitContainer splitContainer4;
        private GroupBox groupBox1;
        private Button renameMapButton;
        private ToolStripMenuItem injectMeshesFromOBJToolStripMenuItem;
        private ToolStripMenuItem viewersToolStripMenuItem;
        private ToolStripMenuItem lightmapViewerToolStripMenuItem;
        private ToolStripMenuItem metaEditorNewToolStripMenuItem;
        private ToolStripMenuItem portalViewerToolStripMenuItem;
        private ToolStripMenuItem bSPTreeViewerToolStripMenuItem;
        private MetaEditor2.WinMetaEditor wME;
        ToolTip toolTip = new ToolTip();

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
            this.sSwap = null;
        }

        #region Windows Form Generated Code
        private HexView hexView1;
        public MetaEditor.MetaEditor metaEditor1;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeviewcontext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setActiveMatgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setActiveScnrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateRecursivelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overwriteMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertCEToH2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromQuickListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputListToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderHierarchyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTagQuickListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMetaDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadMetaDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openInfoFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.displayMenu = new System.Windows.Forms.ContextMenu();
            this.SwapItem = new System.Windows.Forms.MenuItem();
            this.FloodfillSwapItem = new System.Windows.Forms.MenuItem();
            this.JumpToTagItem = new System.Windows.Forms.MenuItem();
            this.chunkclone = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.ShowReflex = new System.Windows.Forms.MenuItem();
            this.ShowIdent = new System.Windows.Forms.MenuItem();
            this.ShowString = new System.Windows.Forms.MenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusbar = new System.Windows.Forms.ToolStripStatusLabel();
            this.references = new System.Windows.Forms.ListView();
            this.refs = new System.Windows.Forms.ColumnHeader();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnUndock = new System.Windows.Forms.Button();
            this.buttonInternalize = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.referenceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metaEditorNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metaEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.StringEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuilderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixSystemLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.metaTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BitmapContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToBumpMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitmapEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondaryMagicBox = new System.Windows.Forms.RichTextBox();
            this.primaryMagicBox = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.metaRawBox = new System.Windows.Forms.RichTextBox();
            this.lblRawSize = new System.Windows.Forms.Label();
            this.scanbspwithifp = new System.Windows.Forms.CheckBox();
            this.soundsCheckBox = new System.Windows.Forms.CheckBox();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.parsedCheckBox = new System.Windows.Forms.CheckBox();
            this.loadMetaButton = new System.Windows.Forms.Button();
            this.saveMetaButton = new System.Windows.Forms.Button();
            this.metaTypeBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.metaIdentBox = new System.Windows.Forms.RichTextBox();
            this.metaSizeBox = new System.Windows.Forms.RichTextBox();
            this.metaOffsetBox = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.signMapButton = new System.Windows.Forms.Button();
            this.analyzeMapButton = new System.Windows.Forms.Button();
            this.buildButton = new System.Windows.Forms.Button();
            this.searchGroupBox = new System.Windows.Forms.GroupBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchComboBox = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripTagView = new System.Windows.Forms.ToolStripButton();
            this.toolStripFolderView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBSPEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripInfoView = new System.Windows.Forms.ToolStripButton();
            this.toolStripHistoryList = new System.Windows.Forms.ToolStripDropDownButton();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.LowerOptionsBar = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.renameMapButton = new System.Windows.Forms.Button();
            this.buttonLowerOptions = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.MetaEditor2Panel = new System.Windows.Forms.Panel();
            this.hexView1 = new entity.HexEditor.HexView();
            this.ltmpTools = new System.Windows.Forms.Panel();
            this.MetaEditorPanel = new System.Windows.Forms.Panel();
            this.metaEditor1 = new MetaEditor.MetaEditor();
            this.LibraryPanel = new System.Windows.Forms.Panel();
            this.toolStripBSPEditorDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.saveBitmapDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ModelContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectMeshesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BSPcontextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewBSPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bSPTreeViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bspcollisionViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightmapViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portalViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bSPCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCollisonToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandMeshX3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeBSPStickyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectBSPVisualMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMeshToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectMeshesFromOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractMeshToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collisonViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.prtmcontext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewPRTMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractPRTMOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBitmapDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.showAnimatedBitmapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeviewcontext.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.BitmapContextStrip.SuspendLayout();
            this.panel3.SuspendLayout();
            this.searchGroupBox.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.LowerOptionsBar.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.MetaEditorPanel.SuspendLayout();
            this.ModelContextStrip.SuspendLayout();
            this.BSPcontextMenu.SuspendLayout();
            this.collContextMenu.SuspendLayout();
            this.prtmcontext.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.ContextMenuStrip = this.treeviewcontext;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(3, 50, 3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(192, 527);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            // 
            // treeviewcontext
            // 
            this.treeviewcontext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setActiveMatgToolStripMenuItem,
            this.setActiveScnrToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.duplicateRecursivelyToolStripMenuItem,
            this.overwriteMetaToolStripMenuItem,
            this.convertCEToH2ToolStripMenuItem,
            this.removeFromQuickListToolStripMenuItem,
            this.outputListToFileToolStripMenuItem,
            this.sortByToolStripMenuItem,
            this.clearTagQuickListToolStripMenuItem});
            this.treeviewcontext.Name = "treeviewcontext";
            this.treeviewcontext.Size = new System.Drawing.Size(221, 246);
            this.treeviewcontext.Opening += new System.ComponentModel.CancelEventHandler(this.treeviewcontext_Opening);
            // 
            // setActiveMatgToolStripMenuItem
            // 
            this.setActiveMatgToolStripMenuItem.Name = "setActiveMatgToolStripMenuItem";
            this.setActiveMatgToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.setActiveMatgToolStripMenuItem.Text = "Set as active MATG Tag";
            this.setActiveMatgToolStripMenuItem.Visible = false;
            this.setActiveMatgToolStripMenuItem.Click += new System.EventHandler(this.setActiveMatgToolStripMenuItem_Click);
            // 
            // setActiveScnrToolStripMenuItem
            // 
            this.setActiveScnrToolStripMenuItem.Name = "setActiveScnrToolStripMenuItem";
            this.setActiveScnrToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.setActiveScnrToolStripMenuItem.Text = "Set as active Scenario Tag";
            this.setActiveScnrToolStripMenuItem.Visible = false;
            this.setActiveScnrToolStripMenuItem.Click += new System.EventHandler(this.setActiveScnrToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // duplicateRecursivelyToolStripMenuItem
            // 
            this.duplicateRecursivelyToolStripMenuItem.Name = "duplicateRecursivelyToolStripMenuItem";
            this.duplicateRecursivelyToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.duplicateRecursivelyToolStripMenuItem.Text = "Duplicate Recursively";
            this.duplicateRecursivelyToolStripMenuItem.Click += new System.EventHandler(this.duplicateRecursivelyToolStripMenuItem_Click);
            // 
            // overwriteMetaToolStripMenuItem
            // 
            this.overwriteMetaToolStripMenuItem.Name = "overwriteMetaToolStripMenuItem";
            this.overwriteMetaToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.overwriteMetaToolStripMenuItem.Text = "Overwrite Meta";
            this.overwriteMetaToolStripMenuItem.Visible = false;
            this.overwriteMetaToolStripMenuItem.Click += new System.EventHandler(this.overwriteMetaToolStripMenuItem_Click);
            // 
            // convertCEToH2ToolStripMenuItem
            // 
            this.convertCEToH2ToolStripMenuItem.Name = "convertCEToH2ToolStripMenuItem";
            this.convertCEToH2ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.convertCEToH2ToolStripMenuItem.Text = "Convert CE to H2";
            this.convertCEToH2ToolStripMenuItem.Visible = false;
            this.convertCEToH2ToolStripMenuItem.Click += new System.EventHandler(this.convertCEToH2ToolStripMenuItem_Click);
            // 
            // removeFromQuickListToolStripMenuItem
            // 
            this.removeFromQuickListToolStripMenuItem.Name = "removeFromQuickListToolStripMenuItem";
            this.removeFromQuickListToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.removeFromQuickListToolStripMenuItem.Text = "Remove From Tag Quick List";
            this.removeFromQuickListToolStripMenuItem.Click += new System.EventHandler(this.removeFromQuickListToolStripMenuItem_Click);
            // 
            // outputListToFileToolStripMenuItem
            // 
            this.outputListToFileToolStripMenuItem.Name = "outputListToFileToolStripMenuItem";
            this.outputListToFileToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.outputListToFileToolStripMenuItem.Text = "Output List to File...";
            this.outputListToFileToolStripMenuItem.Click += new System.EventHandler(this.outputListToFileToolStripMenuItem_Click);
            // 
            // sortByToolStripMenuItem
            // 
            this.sortByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagTypeToolStripMenuItem,
            this.folderHierarchyToolStripMenuItem});
            this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            this.sortByToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.sortByToolStripMenuItem.Text = "Sort By";
            // 
            // tagTypeToolStripMenuItem
            // 
            this.tagTypeToolStripMenuItem.Checked = true;
            this.tagTypeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tagTypeToolStripMenuItem.Name = "tagTypeToolStripMenuItem";
            this.tagTypeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.tagTypeToolStripMenuItem.Text = "Tag Type";
            this.tagTypeToolStripMenuItem.Click += new System.EventHandler(this.tagTypeToolStripMenuItem_Click);
            // 
            // folderHierarchyToolStripMenuItem
            // 
            this.folderHierarchyToolStripMenuItem.Name = "folderHierarchyToolStripMenuItem";
            this.folderHierarchyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.folderHierarchyToolStripMenuItem.Text = "Folder Hierarchy";
            this.folderHierarchyToolStripMenuItem.Click += new System.EventHandler(this.folderHierarchyToolStripMenuItem_Click);
            // 
            // clearTagQuickListToolStripMenuItem
            // 
            this.clearTagQuickListToolStripMenuItem.Name = "clearTagQuickListToolStripMenuItem";
            this.clearTagQuickListToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.clearTagQuickListToolStripMenuItem.Text = "Clear Tag Quick List";
            this.clearTagQuickListToolStripMenuItem.Click += new System.EventHandler(this.clearTagQuickListToolStripMenuItem_Click);
            // 
            // openInfoFileDialog
            // 
            this.openInfoFileDialog.Filter = "Info File (*.info) | *.info";
            // 
            // displayMenu
            // 
            this.displayMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SwapItem,
            this.FloodfillSwapItem,
            this.JumpToTagItem,
            this.chunkclone,
            this.menuItem1});
            // 
            // SwapItem
            // 
            this.SwapItem.Index = 0;
            this.SwapItem.Text = "Swap";
            this.SwapItem.Click += new System.EventHandler(this.SwapItem_Click);
            // 
            // FloodfillSwapItem
            // 
            this.FloodfillSwapItem.Index = 1;
            this.FloodfillSwapItem.Text = "Floodfill Swap";
            this.FloodfillSwapItem.Visible = false;
            this.FloodfillSwapItem.Click += new System.EventHandler(this.FloodfillSwapItem_Click);
            // 
            // JumpToTagItem
            // 
            this.JumpToTagItem.Index = 2;
            this.JumpToTagItem.Text = "Jump to Tag";
            this.JumpToTagItem.Click += new System.EventHandler(this.JumpToTagItem_Click);
            // 
            // chunkclone
            // 
            this.chunkclone.Index = 3;
            this.chunkclone.Text = "Clone Chunk";
            this.chunkclone.Click += new System.EventHandler(this.chunkclone_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ShowReflex,
            this.ShowIdent,
            this.ShowString});
            this.menuItem1.Text = "Display";
            // 
            // ShowReflex
            // 
            this.ShowReflex.Index = 0;
            this.ShowReflex.Text = "Reflexives";
            this.ShowReflex.Click += new System.EventHandler(this.ShowReflex_Click);
            // 
            // ShowIdent
            // 
            this.ShowIdent.Index = 1;
            this.ShowIdent.Text = "Idents";
            this.ShowIdent.Click += new System.EventHandler(this.ShowIdent_Click);
            // 
            // ShowString
            // 
            this.ShowString.Index = 2;
            this.ShowString.Text = "Strings";
            this.ShowString.Click += new System.EventHandler(this.ShowString_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressbar,
            this.statusbar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 669);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(899, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressbar
            // 
            this.progressbar.Name = "progressbar";
            this.progressbar.Size = new System.Drawing.Size(200, 16);
            this.progressbar.Step = 1;
            this.progressbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // statusbar
            // 
            this.statusbar.BackColor = System.Drawing.Color.LightGray;
            this.statusbar.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusbar.Name = "statusbar";
            this.statusbar.Size = new System.Drawing.Size(682, 17);
            this.statusbar.Spring = true;
            this.statusbar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // references
            // 
            this.references.AllowColumnReorder = true;
            this.references.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.references.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.refs});
            this.references.Dock = System.Windows.Forms.DockStyle.Fill;
            this.references.FullRowSelect = true;
            this.references.GridLines = true;
            this.references.HideSelection = false;
            this.references.Location = new System.Drawing.Point(0, 0);
            this.references.Name = "references";
            this.references.Size = new System.Drawing.Size(716, 669);
            this.references.TabIndex = 14;
            this.references.UseCompatibleStateImageBehavior = false;
            this.references.View = System.Windows.Forms.View.Details;
            this.references.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.references_ColumnClick);
            this.references.MouseDown += new System.Windows.Forms.MouseEventHandler(this.references_MouseDown);
            // 
            // refs
            // 
            this.refs.Text = "References";
            this.refs.Width = 5000;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SteelBlue;
            this.panel2.Controls.Add(this.btnUndock);
            this.panel2.Controls.Add(this.buttonInternalize);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.toolStripContainer1);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.secondaryMagicBox);
            this.panel2.Controls.Add(this.primaryMagicBox);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.signMapButton);
            this.panel2.Controls.Add(this.analyzeMapButton);
            this.panel2.Controls.Add(this.buildButton);
            this.panel2.Controls.Add(this.searchGroupBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(179, 669);
            this.panel2.TabIndex = 16;
            // 
            // btnUndock
            // 
            this.btnUndock.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnUndock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndock.Location = new System.Drawing.Point(9, 566);
            this.btnUndock.Name = "btnUndock";
            this.btnUndock.Size = new System.Drawing.Size(79, 23);
            this.btnUndock.TabIndex = 39;
            this.btnUndock.Text = "Undock";
            this.btnUndock.UseVisualStyleBackColor = false;
            this.btnUndock.Click += new System.EventHandler(this.btnUndock_Click);
            // 
            // buttonInternalize
            // 
            this.buttonInternalize.Location = new System.Drawing.Point(106, 404);
            this.buttonInternalize.Name = "buttonInternalize";
            this.buttonInternalize.Size = new System.Drawing.Size(69, 22);
            this.buttonInternalize.TabIndex = 37;
            this.buttonInternalize.Text = "Internalize";
            this.buttonInternalize.UseVisualStyleBackColor = true;
            this.buttonInternalize.Visible = false;
            this.buttonInternalize.Click += new System.EventHandler(this.buttonInternalize_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(9, 537);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 23);
            this.button1.TabIndex = 30;
            this.button1.Text = "Check IFPs";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.checkIFPsButton_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(179, 0);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(179, 25);
            this.toolStripContainer1.TabIndex = 29;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.rawDataDropDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(179, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.referenceEditorToolStripMenuItem,
            this.metaEditorNewToolStripMenuItem,
            this.metaEditorToolStripMenuItem,
            this.hexEditorToolStripMenuItem,
            this.toolStripSeparator2,
            this.StringEditorToolStripMenuItem,
            this.rebuilderToolStripMenuItem,
            this.fixSystemLinkToolStripMenuItem,
            this.pluginsToolStripMenuItem,
            this.exportScriptsToolStripMenuItem});
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton1.Text = "Tools";
            // 
            // referenceEditorToolStripMenuItem
            // 
            this.referenceEditorToolStripMenuItem.Checked = true;
            this.referenceEditorToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.referenceEditorToolStripMenuItem.Name = "referenceEditorToolStripMenuItem";
            this.referenceEditorToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.referenceEditorToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.referenceEditorToolStripMenuItem.Text = "Reference Editor";
            this.referenceEditorToolStripMenuItem.Click += new System.EventHandler(this.referenceToolStripMenuItem_Click);
            // 
            // metaEditorNewToolStripMenuItem
            // 
            this.metaEditorNewToolStripMenuItem.Name = "metaEditorNewToolStripMenuItem";
            this.metaEditorNewToolStripMenuItem.ShortcutKeyDisplayString = "F6";
            this.metaEditorNewToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.metaEditorNewToolStripMenuItem.Text = "Meta Editor Plus";
            this.metaEditorNewToolStripMenuItem.Click += new System.EventHandler(this.metaEditorNewToolStripMenuItem_Click);
            // 
            // metaEditorToolStripMenuItem
            // 
            this.metaEditorToolStripMenuItem.Name = "metaEditorToolStripMenuItem";
            this.metaEditorToolStripMenuItem.ShortcutKeyDisplayString = "F7";
            this.metaEditorToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.metaEditorToolStripMenuItem.Text = "Meta Editor";
            this.metaEditorToolStripMenuItem.Click += new System.EventHandler(this.metaEditorToolStripMenuItem_Click);
            // 
            // hexEditorToolStripMenuItem
            // 
            this.hexEditorToolStripMenuItem.Name = "hexEditorToolStripMenuItem";
            this.hexEditorToolStripMenuItem.ShortcutKeyDisplayString = "F8";
            this.hexEditorToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.hexEditorToolStripMenuItem.Text = "Hex Editor";
            this.hexEditorToolStripMenuItem.Click += new System.EventHandler(this.hexEditorToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // StringEditorToolStripMenuItem
            // 
            this.StringEditorToolStripMenuItem.Name = "StringEditorToolStripMenuItem";
            this.StringEditorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.StringEditorToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.StringEditorToolStripMenuItem.Text = "Unicode String Viewer";
            this.StringEditorToolStripMenuItem.Click += new System.EventHandler(this.StringEditorToolStripMenuItem_Click);
            // 
            // rebuilderToolStripMenuItem
            // 
            this.rebuilderToolStripMenuItem.Name = "rebuilderToolStripMenuItem";
            this.rebuilderToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.rebuilderToolStripMenuItem.Text = "Rebuilder";
            this.rebuilderToolStripMenuItem.Click += new System.EventHandler(this.rebuilderToolStripMenuItem_Click);
            // 
            // fixSystemLinkToolStripMenuItem
            // 
            this.fixSystemLinkToolStripMenuItem.Name = "fixSystemLinkToolStripMenuItem";
            this.fixSystemLinkToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.fixSystemLinkToolStripMenuItem.Text = "Fix System Link";
            this.fixSystemLinkToolStripMenuItem.Click += new System.EventHandler(this.fixSystemLinkToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.pluginsToolStripMenuItem.Text = "Plugins";
            // 
            // exportScriptsToolStripMenuItem
            // 
            this.exportScriptsToolStripMenuItem.Name = "exportScriptsToolStripMenuItem";
            this.exportScriptsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exportScriptsToolStripMenuItem.Text = "Export Scripts";
            this.exportScriptsToolStripMenuItem.Visible = false;
            this.exportScriptsToolStripMenuItem.Click += new System.EventHandler(this.exportScriptsToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metaTreeToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(54, 22);
            this.toolStripDropDownButton2.Text = "Display";
            this.toolStripDropDownButton2.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // metaTreeToolStripMenuItem
            // 
            this.metaTreeToolStripMenuItem.Checked = true;
            this.metaTreeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.metaTreeToolStripMenuItem.Name = "metaTreeToolStripMenuItem";
            this.metaTreeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.metaTreeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.metaTreeToolStripMenuItem.Text = "Meta Tree";
            this.metaTreeToolStripMenuItem.Click += new System.EventHandler(this.metaListToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Checked = true;
            this.toolsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsWindowToolStripMenuItem_Click);
            // 
            // rawDataDropDown
            // 
            this.rawDataDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rawDataDropDown.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.rawDataDropDown.Enabled = false;
            this.rawDataDropDown.Image = ((System.Drawing.Image)(resources.GetObject("rawDataDropDown.Image")));
            this.rawDataDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rawDataDropDown.Name = "rawDataDropDown";
            this.rawDataDropDown.Size = new System.Drawing.Size(29, 22);
            this.rawDataDropDown.Text = "toolStripDropDownButton3";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.ContextMenuStrip = this.BitmapContextStrip;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 271);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(168, 155);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // BitmapContextStrip
            // 
            this.BitmapContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveBitmapToolStripMenuItem,
            this.injectBitmapToolStripMenuItem,
            this.convertToBumpMapToolStripMenuItem,
            this.bitmapEditorToolStripMenuItem,
            this.showAnimatedBitmapsToolStripMenuItem});
            this.BitmapContextStrip.Name = "contextMenuStrip1";
            this.BitmapContextStrip.Size = new System.Drawing.Size(200, 136);
            // 
            // saveBitmapToolStripMenuItem
            // 
            this.saveBitmapToolStripMenuItem.Name = "saveBitmapToolStripMenuItem";
            this.saveBitmapToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.saveBitmapToolStripMenuItem.Text = "Save Bitmap";
            this.saveBitmapToolStripMenuItem.Click += new System.EventHandler(this.saveBitmapToolStripMenuItem_Click);
            // 
            // injectBitmapToolStripMenuItem
            // 
            this.injectBitmapToolStripMenuItem.Name = "injectBitmapToolStripMenuItem";
            this.injectBitmapToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.injectBitmapToolStripMenuItem.Text = "Inject Bitmap";
            this.injectBitmapToolStripMenuItem.Click += new System.EventHandler(this.injectBitmapToolStripMenuItem_Click);
            // 
            // convertToBumpMapToolStripMenuItem
            // 
            this.convertToBumpMapToolStripMenuItem.Name = "convertToBumpMapToolStripMenuItem";
            this.convertToBumpMapToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.convertToBumpMapToolStripMenuItem.Text = "Convert To BumpMap";
            this.convertToBumpMapToolStripMenuItem.Click += new System.EventHandler(this.convertToBumpMapToolStripMenuItem_Click);
            // 
            // bitmapEditorToolStripMenuItem
            // 
            this.bitmapEditorToolStripMenuItem.Name = "bitmapEditorToolStripMenuItem";
            this.bitmapEditorToolStripMenuItem.ShortcutKeyDisplayString = "F9";
            this.bitmapEditorToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.bitmapEditorToolStripMenuItem.Text = "Bitmap Editor";
            this.bitmapEditorToolStripMenuItem.Click += new System.EventHandler(this.bitmapEditorToolStripMenuItem_Click);
            // 
            // secondaryMagicBox
            // 
            this.secondaryMagicBox.BackColor = System.Drawing.Color.SteelBlue;
            this.secondaryMagicBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.secondaryMagicBox.Location = new System.Drawing.Point(16, 481);
            this.secondaryMagicBox.Name = "secondaryMagicBox";
            this.secondaryMagicBox.ReadOnly = true;
            this.secondaryMagicBox.Size = new System.Drawing.Size(160, 16);
            this.secondaryMagicBox.TabIndex = 27;
            this.secondaryMagicBox.Text = "";
            // 
            // primaryMagicBox
            // 
            this.primaryMagicBox.BackColor = System.Drawing.Color.SteelBlue;
            this.primaryMagicBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.primaryMagicBox.Location = new System.Drawing.Point(16, 447);
            this.primaryMagicBox.Name = "primaryMagicBox";
            this.primaryMagicBox.ReadOnly = true;
            this.primaryMagicBox.Size = new System.Drawing.Size(160, 16);
            this.primaryMagicBox.TabIndex = 26;
            this.primaryMagicBox.Text = "";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 464);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(162, 16);
            this.label4.TabIndex = 25;
            this.label4.Text = "Secondary Magic:";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 429);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 16);
            this.label6.TabIndex = 24;
            this.label6.Text = "Primary Magic:";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.metaRawBox);
            this.panel3.Controls.Add(this.lblRawSize);
            this.panel3.Controls.Add(this.scanbspwithifp);
            this.panel3.Controls.Add(this.soundsCheckBox);
            this.panel3.Controls.Add(this.recursiveCheckBox);
            this.panel3.Controls.Add(this.parsedCheckBox);
            this.panel3.Controls.Add(this.loadMetaButton);
            this.panel3.Controls.Add(this.saveMetaButton);
            this.panel3.Controls.Add(this.metaTypeBox);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.metaIdentBox);
            this.panel3.Controls.Add(this.metaSizeBox);
            this.panel3.Controls.Add(this.metaOffsetBox);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(7, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(168, 235);
            this.panel3.TabIndex = 19;
            // 
            // metaRawBox
            // 
            this.metaRawBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.metaRawBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metaRawBox.Location = new System.Drawing.Point(57, 118);
            this.metaRawBox.Name = "metaRawBox";
            this.metaRawBox.ReadOnly = true;
            this.metaRawBox.Size = new System.Drawing.Size(100, 32);
            this.metaRawBox.TabIndex = 34;
            this.metaRawBox.Text = "";
            // 
            // lblRawSize
            // 
            this.lblRawSize.BackColor = System.Drawing.Color.Transparent;
            this.lblRawSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblRawSize.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRawSize.Location = new System.Drawing.Point(8, 116);
            this.lblRawSize.Name = "lblRawSize";
            this.lblRawSize.Size = new System.Drawing.Size(60, 19);
            this.lblRawSize.TabIndex = 33;
            this.lblRawSize.Text = "Raw:";
            // 
            // scanbspwithifp
            // 
            this.scanbspwithifp.BackColor = System.Drawing.Color.Transparent;
            this.scanbspwithifp.Checked = true;
            this.scanbspwithifp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scanbspwithifp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scanbspwithifp.Location = new System.Drawing.Point(74, 205);
            this.scanbspwithifp.Name = "scanbspwithifp";
            this.scanbspwithifp.Size = new System.Drawing.Size(93, 24);
            this.scanbspwithifp.TabIndex = 32;
            this.scanbspwithifp.Text = "Scan With Ent";
            this.scanbspwithifp.UseVisualStyleBackColor = false;
            // 
            // soundsCheckBox
            // 
            this.soundsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.soundsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.soundsCheckBox.Location = new System.Drawing.Point(8, 204);
            this.soundsCheckBox.Name = "soundsCheckBox";
            this.soundsCheckBox.Size = new System.Drawing.Size(76, 24);
            this.soundsCheckBox.TabIndex = 31;
            this.soundsCheckBox.Text = "Sounds";
            this.soundsCheckBox.UseVisualStyleBackColor = false;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.recursiveCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.recursiveCheckBox.Location = new System.Drawing.Point(8, 156);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(76, 24);
            this.recursiveCheckBox.TabIndex = 30;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.UseVisualStyleBackColor = false;
            // 
            // parsedCheckBox
            // 
            this.parsedCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.parsedCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.parsedCheckBox.Location = new System.Drawing.Point(8, 180);
            this.parsedCheckBox.Name = "parsedCheckBox";
            this.parsedCheckBox.Size = new System.Drawing.Size(76, 24);
            this.parsedCheckBox.TabIndex = 29;
            this.parsedCheckBox.Text = "Parsed";
            this.parsedCheckBox.UseVisualStyleBackColor = false;
            this.parsedCheckBox.CheckedChanged += new System.EventHandler(this.parsedCheckBox_CheckedChanged_1);
            // 
            // loadMetaButton
            // 
            this.loadMetaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadMetaButton.Location = new System.Drawing.Point(84, 181);
            this.loadMetaButton.Name = "loadMetaButton";
            this.loadMetaButton.Size = new System.Drawing.Size(75, 23);
            this.loadMetaButton.TabIndex = 28;
            this.loadMetaButton.Text = "Inject";
            this.loadMetaButton.Click += new System.EventHandler(this.loadMetaButton_Click_1);
            // 
            // saveMetaButton
            // 
            this.saveMetaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveMetaButton.Location = new System.Drawing.Point(84, 157);
            this.saveMetaButton.Name = "saveMetaButton";
            this.saveMetaButton.Size = new System.Drawing.Size(75, 23);
            this.saveMetaButton.TabIndex = 27;
            this.saveMetaButton.Text = "Save";
            this.saveMetaButton.Click += new System.EventHandler(this.saveMetaButton_Click);
            // 
            // metaTypeBox
            // 
            this.metaTypeBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.metaTypeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metaTypeBox.Location = new System.Drawing.Point(57, 97);
            this.metaTypeBox.Name = "metaTypeBox";
            this.metaTypeBox.ReadOnly = true;
            this.metaTypeBox.Size = new System.Drawing.Size(100, 16);
            this.metaTypeBox.TabIndex = 26;
            this.metaTypeBox.Text = "";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 25;
            this.label2.Text = "Type:";
            // 
            // metaIdentBox
            // 
            this.metaIdentBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.metaIdentBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metaIdentBox.Location = new System.Drawing.Point(57, 77);
            this.metaIdentBox.Name = "metaIdentBox";
            this.metaIdentBox.ReadOnly = true;
            this.metaIdentBox.Size = new System.Drawing.Size(100, 16);
            this.metaIdentBox.TabIndex = 24;
            this.metaIdentBox.Text = "";
            // 
            // metaSizeBox
            // 
            this.metaSizeBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.metaSizeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metaSizeBox.Location = new System.Drawing.Point(57, 46);
            this.metaSizeBox.Name = "metaSizeBox";
            this.metaSizeBox.ReadOnly = true;
            this.metaSizeBox.Size = new System.Drawing.Size(100, 32);
            this.metaSizeBox.TabIndex = 23;
            this.metaSizeBox.Text = "";
            // 
            // metaOffsetBox
            // 
            this.metaOffsetBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.metaOffsetBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metaOffsetBox.Location = new System.Drawing.Point(57, 10);
            this.metaOffsetBox.Name = "metaOffsetBox";
            this.metaOffsetBox.ReadOnly = true;
            this.metaOffsetBox.Size = new System.Drawing.Size(100, 32);
            this.metaOffsetBox.TabIndex = 22;
            this.metaOffsetBox.Text = "";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "Ident:";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 17;
            this.label3.Text = "Size:";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 15;
            this.label1.Text = "Offset:";
            // 
            // signMapButton
            // 
            this.signMapButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.signMapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.signMapButton.Location = new System.Drawing.Point(9, 508);
            this.signMapButton.Name = "signMapButton";
            this.signMapButton.Size = new System.Drawing.Size(78, 23);
            this.signMapButton.TabIndex = 18;
            this.signMapButton.Text = "Sign";
            this.signMapButton.UseVisualStyleBackColor = false;
            this.signMapButton.Click += new System.EventHandler(this.signMapButton_Click_1);
            // 
            // analyzeMapButton
            // 
            this.analyzeMapButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.analyzeMapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.analyzeMapButton.Location = new System.Drawing.Point(96, 537);
            this.analyzeMapButton.Name = "analyzeMapButton";
            this.analyzeMapButton.Size = new System.Drawing.Size(78, 23);
            this.analyzeMapButton.TabIndex = 12;
            this.analyzeMapButton.Text = "Analyze Map";
            this.analyzeMapButton.UseVisualStyleBackColor = false;
            this.analyzeMapButton.Click += new System.EventHandler(this.analyzeMapButton_Click_1);
            // 
            // buildButton
            // 
            this.buildButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.buildButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buildButton.Location = new System.Drawing.Point(96, 508);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(78, 23);
            this.buildButton.TabIndex = 11;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = false;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click_1);
            // 
            // searchGroupBox
            // 
            this.searchGroupBox.Controls.Add(this.searchLabel);
            this.searchGroupBox.Controls.Add(this.searchButton);
            this.searchGroupBox.Controls.Add(this.searchTextBox);
            this.searchGroupBox.Controls.Add(this.searchComboBox);
            this.searchGroupBox.Enabled = false;
            this.searchGroupBox.Location = new System.Drawing.Point(4, 591);
            this.searchGroupBox.Name = "searchGroupBox";
            this.searchGroupBox.Size = new System.Drawing.Size(175, 70);
            this.searchGroupBox.TabIndex = 36;
            this.searchGroupBox.TabStop = false;
            this.searchGroupBox.Text = "Meta Search";
            this.searchGroupBox.Visible = false;
            // 
            // searchLabel
            // 
            this.searchLabel.Location = new System.Drawing.Point(3, 45);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(62, 18);
            this.searchLabel.TabIndex = 35;
            this.searchLabel.Text = "Search In :";
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(123, 15);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(49, 22);
            this.searchButton.TabIndex = 34;
            this.searchButton.Text = "&Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(8, 17);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(114, 20);
            this.searchTextBox.TabIndex = 2;
            this.searchTextBox.Text = "<Enter Search Here>";
            this.searchTextBox.Click += new System.EventHandler(this.searchTextBox_Click);
            this.searchTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchTextBox_KeyPress);
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
            // 
            // searchComboBox
            // 
            this.searchComboBox.CausesValidation = false;
            this.searchComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchComboBox.FormattingEnabled = true;
            this.searchComboBox.Items.AddRange(new object[] {
            "Current Tag",
            "All Tags"});
            this.searchComboBox.Location = new System.Drawing.Point(64, 42);
            this.searchComboBox.Name = "searchComboBox";
            this.searchComboBox.Size = new System.Drawing.Size(108, 21);
            this.searchComboBox.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(76, 17);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(67, 19);
            this.button3.TabIndex = 42;
            this.button3.Text = "Edit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.customPLuginEdit_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 19);
            this.button2.TabIndex = 41;
            this.button2.Text = "New";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.customPLugin_New_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Complete Plugin Set"});
            this.comboBox1.Location = new System.Drawing.Point(3, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(165, 21);
            this.comboBox1.TabIndex = 43;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.customPLugins_comboBox1_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.SteelBlue;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1094, 691);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 17;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(192, 691);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.SplitterWidth = 2;
            this.splitContainer3.TabIndex = 2;
            this.splitContainer3.TabStop = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTagView,
            this.toolStripFolderView,
            this.toolStripSeparator1,
            this.toolStripBSPEditor,
            this.toolStripInfoView,
            this.toolStripHistoryList});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.toolStrip2.Size = new System.Drawing.Size(192, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripTagView
            // 
            this.toolStripTagView.Checked = true;
            this.toolStripTagView.CheckOnClick = true;
            this.toolStripTagView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripTagView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripTagView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTagView.Name = "toolStripTagView";
            this.toolStripTagView.Size = new System.Drawing.Size(34, 20);
            this.toolStripTagView.Text = "Tags";
            this.toolStripTagView.ToolTipText = "Show Tag Type View";
            this.toolStripTagView.Click += new System.EventHandler(this.tagTypeToolStripMenuItem_Click);
            // 
            // toolStripFolderView
            // 
            this.toolStripFolderView.CheckOnClick = true;
            this.toolStripFolderView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripFolderView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFolderView.Image")));
            this.toolStripFolderView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFolderView.Name = "toolStripFolderView";
            this.toolStripFolderView.Size = new System.Drawing.Size(46, 20);
            this.toolStripFolderView.Text = "Folders";
            this.toolStripFolderView.ToolTipText = "Show Folders View";
            this.toolStripFolderView.Click += new System.EventHandler(this.folderHierarchyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripBSPEditor
            // 
            this.toolStripBSPEditor.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripBSPEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBSPEditor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBSPEditor.Image")));
            this.toolStripBSPEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBSPEditor.Name = "toolStripBSPEditor";
            this.toolStripBSPEditor.Size = new System.Drawing.Size(69, 20);
            this.toolStripBSPEditor.Text = "Visual Editor";
            this.toolStripBSPEditor.ToolTipText = "BSP Visual Editor";
            this.toolStripBSPEditor.Click += new System.EventHandler(this.toolStripBSPEditor_Click);
            // 
            // toolStripInfoView
            // 
            this.toolStripInfoView.CheckOnClick = true;
            this.toolStripInfoView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripInfoView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripInfoView.Image")));
            this.toolStripInfoView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInfoView.Name = "toolStripInfoView";
            this.toolStripInfoView.Size = new System.Drawing.Size(31, 17);
            this.toolStripInfoView.Text = "Info";
            this.toolStripInfoView.Click += new System.EventHandler(this.infoHierarchyToolStripMenuItem_Click);
            // 
            // toolStripHistoryList
            // 
            this.toolStripHistoryList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripHistoryList.Image = ((System.Drawing.Image)(resources.GetObject("toolStripHistoryList.Image")));
            this.toolStripHistoryList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripHistoryList.Name = "toolStripHistoryList";
            this.toolStripHistoryList.Size = new System.Drawing.Size(29, 20);
            this.toolStripHistoryList.Text = "Session Tag History";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.LowerOptionsBar);
            this.splitContainer4.Panel2.Controls.Add(this.buttonLowerOptions);
            this.splitContainer4.Size = new System.Drawing.Size(192, 664);
            this.splitContainer4.SplitterDistance = 527;
            this.splitContainer4.SplitterWidth = 2;
            this.splitContainer4.TabIndex = 2;
            // 
            // LowerOptionsBar
            // 
            this.LowerOptionsBar.BackColor = System.Drawing.Color.LightGray;
            this.LowerOptionsBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LowerOptionsBar.Controls.Add(this.groupBox1);
            this.LowerOptionsBar.Controls.Add(this.renameMapButton);
            this.LowerOptionsBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LowerOptionsBar.Location = new System.Drawing.Point(0, 0);
            this.LowerOptionsBar.Name = "LowerOptionsBar";
            this.LowerOptionsBar.Size = new System.Drawing.Size(192, 113);
            this.LowerOptionsBar.TabIndex = 1;
            this.LowerOptionsBar.Tag = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(3, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 66);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom Plugin Selector (ME1 Only)";
            // 
            // renameMapButton
            // 
            this.renameMapButton.Location = new System.Drawing.Point(6, 8);
            this.renameMapButton.Name = "renameMapButton";
            this.renameMapButton.Size = new System.Drawing.Size(100, 23);
            this.renameMapButton.TabIndex = 39;
            this.renameMapButton.Text = "Rename Map";
            this.renameMapButton.UseVisualStyleBackColor = true;
            this.renameMapButton.Click += new System.EventHandler(this.renameMapButton_Click);
            // 
            // buttonLowerOptions
            // 
            this.buttonLowerOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonLowerOptions.Cursor = System.Windows.Forms.Cursors.PanSouth;
            this.buttonLowerOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonLowerOptions.Location = new System.Drawing.Point(0, 113);
            this.buttonLowerOptions.Name = "buttonLowerOptions";
            this.buttonLowerOptions.Size = new System.Drawing.Size(192, 22);
            this.buttonLowerOptions.TabIndex = 0;
            this.buttonLowerOptions.Text = "Close Options";
            this.buttonLowerOptions.UseVisualStyleBackColor = true;
            this.buttonLowerOptions.Click += new System.EventHandler(this.closeOpenCustomPluginOptionsbutton4_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.references);
            this.splitContainer2.Panel1.Controls.Add(this.MetaEditor2Panel);
            this.splitContainer2.Panel1.Controls.Add(this.hexView1);
            this.splitContainer2.Panel1.Controls.Add(this.ltmpTools);
            this.splitContainer2.Panel1.Controls.Add(this.MetaEditorPanel);
            this.splitContainer2.Panel1.Controls.Add(this.LibraryPanel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Size = new System.Drawing.Size(899, 669);
            this.splitContainer2.SplitterDistance = 716;
            this.splitContainer2.TabIndex = 14;
            // 
            // MetaEditor2Panel
            // 
            this.MetaEditor2Panel.BackColor = System.Drawing.Color.DimGray;
            this.MetaEditor2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetaEditor2Panel.Location = new System.Drawing.Point(0, 0);
            this.MetaEditor2Panel.Name = "MetaEditor2Panel";
            this.MetaEditor2Panel.Size = new System.Drawing.Size(716, 669);
            this.MetaEditor2Panel.TabIndex = 16;
            this.MetaEditor2Panel.Visible = false;
            // 
            // hexView1
            // 
            this.hexView1.BackColor = System.Drawing.Color.DarkGray;
            this.hexView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexView1.Enabled = false;
            this.hexView1.Location = new System.Drawing.Point(0, 0);
            this.hexView1.Name = "hexView1";
            this.hexView1.Padding = new System.Windows.Forms.Padding(10);
            this.hexView1.Size = new System.Drawing.Size(716, 669);
            this.hexView1.TabIndex = 0;
            this.hexView1.Visible = false;
            // 
            // ltmpTools
            // 
            this.ltmpTools.AutoScroll = true;
            this.ltmpTools.BackColor = System.Drawing.Color.White;
            this.ltmpTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltmpTools.Location = new System.Drawing.Point(0, 0);
            this.ltmpTools.Name = "ltmpTools";
            this.ltmpTools.Size = new System.Drawing.Size(716, 669);
            this.ltmpTools.TabIndex = 17;
            // 
            // MetaEditorPanel
            // 
            this.MetaEditorPanel.BackColor = System.Drawing.Color.PeachPuff;
            this.MetaEditorPanel.Controls.Add(this.metaEditor1);
            this.MetaEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetaEditorPanel.Location = new System.Drawing.Point(0, 0);
            this.MetaEditorPanel.Name = "MetaEditorPanel";
            this.MetaEditorPanel.Size = new System.Drawing.Size(716, 669);
            this.MetaEditorPanel.TabIndex = 15;
            this.MetaEditorPanel.Visible = false;
            // 
            // metaEditor1
            // 
            this.metaEditor1.AutoScroll = true;
            this.metaEditor1.BackColor = System.Drawing.Color.Gray;
            this.metaEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metaEditor1.Location = new System.Drawing.Point(0, 0);
            this.metaEditor1.Name = "metaEditor1";
            this.metaEditor1.Padding = new System.Windows.Forms.Padding(2);
            this.metaEditor1.Size = new System.Drawing.Size(716, 669);
            this.metaEditor1.TabIndex = 0;
            // 
            // LibraryPanel
            // 
            this.LibraryPanel.AutoScroll = true;
            this.LibraryPanel.BackColor = System.Drawing.Color.DarkGray;
            this.LibraryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LibraryPanel.Location = new System.Drawing.Point(0, 0);
            this.LibraryPanel.Name = "LibraryPanel";
            this.LibraryPanel.Size = new System.Drawing.Size(716, 669);
            this.LibraryPanel.TabIndex = 16;
            this.LibraryPanel.Visible = false;
            // 
            // toolStripBSPEditorDropDown
            // 
            this.toolStripBSPEditorDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripBSPEditorDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripBSPEditorDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBSPEditorDropDown.Name = "toolStripBSPEditorDropDown";
            this.toolStripBSPEditorDropDown.Size = new System.Drawing.Size(29, 20);
            this.toolStripBSPEditorDropDown.Text = "Visual Editor";
            this.toolStripBSPEditorDropDown.ToolTipText = "BSP Visual Editor";
            this.toolStripBSPEditorDropDown.Click += new System.EventHandler(this.toolStripBSPEditor_Click);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // saveBitmapDialog1
            // 
            this.saveBitmapDialog1.Filter = "DDS  (*.DDS) | *.DDS|Bitmap (*.BMP) | *.BMP|Jpg (*.JPG) | *.JPG";
            // 
            // ModelContextStrip
            // 
            this.ModelContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewModelToolStripMenuItem,
            this.exportModelToolStripMenuItem,
            this.exportMeshToolStripMenuItem,
            this.injectMeshesToolStripMenuItem});
            this.ModelContextStrip.Name = "contextMenuStrip2";
            this.ModelContextStrip.Size = new System.Drawing.Size(157, 92);
            // 
            // viewModelToolStripMenuItem
            // 
            this.viewModelToolStripMenuItem.Name = "viewModelToolStripMenuItem";
            this.viewModelToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.viewModelToolStripMenuItem.Text = "View Model";
            this.viewModelToolStripMenuItem.Click += new System.EventHandler(this.viewModelToolStripMenuItem_Click);
            // 
            // exportModelToolStripMenuItem
            // 
            this.exportModelToolStripMenuItem.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.exportModelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toOBJToolStripMenuItem,
            this.toXToolStripMenuItem});
            this.exportModelToolStripMenuItem.Name = "exportModelToolStripMenuItem";
            this.exportModelToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exportModelToolStripMenuItem.Text = "Export Meshes";
            // 
            // toOBJToolStripMenuItem
            // 
            this.toOBJToolStripMenuItem.Name = "toOBJToolStripMenuItem";
            this.toOBJToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.toOBJToolStripMenuItem.Text = "To .OBJ";
            this.toOBJToolStripMenuItem.Click += new System.EventHandler(this.toOBJToolStripMenuItem_Click);
            // 
            // toXToolStripMenuItem
            // 
            this.toXToolStripMenuItem.Name = "toXToolStripMenuItem";
            this.toXToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.toXToolStripMenuItem.Text = "To .X";
            this.toXToolStripMenuItem.Click += new System.EventHandler(this.toXToolStripMenuItem_Click);
            // 
            // exportMeshToolStripMenuItem
            // 
            this.exportMeshToolStripMenuItem.Name = "exportMeshToolStripMenuItem";
            this.exportMeshToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exportMeshToolStripMenuItem.Text = "Export Mesh";
            this.exportMeshToolStripMenuItem.Visible = false;
            this.exportMeshToolStripMenuItem.Click += new System.EventHandler(this.exportMeshToolStripMenuItem_Click);
            // 
            // injectMeshesToolStripMenuItem
            // 
            this.injectMeshesToolStripMenuItem.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.injectMeshesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.injectOBJToolStripMenuItem});
            this.injectMeshesToolStripMenuItem.Name = "injectMeshesToolStripMenuItem";
            this.injectMeshesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.injectMeshesToolStripMenuItem.Text = "Inject Meshes ";
            // 
            // injectOBJToolStripMenuItem
            // 
            this.injectOBJToolStripMenuItem.Name = "injectOBJToolStripMenuItem";
            this.injectOBJToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.injectOBJToolStripMenuItem.Text = "Inject .OBJ";
            this.injectOBJToolStripMenuItem.Click += new System.EventHandler(this.injectOBJToolStripMenuItem_Click);
            // 
            // BSPcontextMenu
            // 
            this.BSPcontextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewBSPToolStripMenuItem,
            this.viewersToolStripMenuItem,
            this.bSPCollisionToolStripMenuItem,
            this.expandMeshX3ToolStripMenuItem,
            this.makeBSPStickyToolStripMenuItem,
            this.exportOBJToolStripMenuItem,
            this.injectBSPVisualMeshToolStripMenuItem,
            this.exportMeshToOBJToolStripMenuItem,
            this.injectMeshesFromOBJToolStripMenuItem});
            this.BSPcontextMenu.Name = "BSPcontextMenu";
            this.BSPcontextMenu.Size = new System.Drawing.Size(233, 202);
            // 
            // viewBSPToolStripMenuItem
            // 
            this.viewBSPToolStripMenuItem.Name = "viewBSPToolStripMenuItem";
            this.viewBSPToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.viewBSPToolStripMenuItem.Text = "View BSP";
            this.viewBSPToolStripMenuItem.Click += new System.EventHandler(this.viewBSPToolStripMenuItem_Click);
            // 
            // viewersToolStripMenuItem
            // 
            this.viewersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bSPTreeViewerToolStripMenuItem,
            this.bspcollisionViewerToolStripMenuItem,
            this.lightmapViewerToolStripMenuItem,
            this.portalViewerToolStripMenuItem});
            this.viewersToolStripMenuItem.Name = "viewersToolStripMenuItem";
            this.viewersToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.viewersToolStripMenuItem.Text = "Viewers";
            // 
            // bSPTreeViewerToolStripMenuItem
            // 
            this.bSPTreeViewerToolStripMenuItem.Enabled = false;
            this.bSPTreeViewerToolStripMenuItem.Name = "bSPTreeViewerToolStripMenuItem";
            this.bSPTreeViewerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.bSPTreeViewerToolStripMenuItem.Text = "BSP Tree Viewer";
            // 
            // bspcollisionViewerToolStripMenuItem
            // 
            this.bspcollisionViewerToolStripMenuItem.Name = "bspcollisionViewerToolStripMenuItem";
            this.bspcollisionViewerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.bspcollisionViewerToolStripMenuItem.Text = "Collision Viewer";
            this.bspcollisionViewerToolStripMenuItem.Click += new System.EventHandler(this.bspcollisionViewerToolStripMenuItem_Click);
            // 
            // lightmapViewerToolStripMenuItem
            // 
            this.lightmapViewerToolStripMenuItem.Name = "lightmapViewerToolStripMenuItem";
            this.lightmapViewerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.lightmapViewerToolStripMenuItem.Text = "Lightmap Viewer";
            this.lightmapViewerToolStripMenuItem.Click += new System.EventHandler(this.lightmapViewerToolStripMenuItem_Click);
            // 
            // portalViewerToolStripMenuItem
            // 
            this.portalViewerToolStripMenuItem.Name = "portalViewerToolStripMenuItem";
            this.portalViewerToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.portalViewerToolStripMenuItem.Text = "Portal Viewer";
            this.portalViewerToolStripMenuItem.Click += new System.EventHandler(this.portalViewerToolStripMenuItem_Click);
            // 
            // bSPCollisionToolStripMenuItem
            // 
            this.bSPCollisionToolStripMenuItem.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.bSPCollisionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.exportCollisonToOBJToolStripMenuItem});
            this.bSPCollisionToolStripMenuItem.Name = "bSPCollisionToolStripMenuItem";
            this.bSPCollisionToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.bSPCollisionToolStripMenuItem.Text = "BSP Collision";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(208, 22);
            this.toolStripMenuItem2.Text = "Inject Collision Mesh .OBJ";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.injectCollisionMeshOBJToolStripMenuItem_Click);
            // 
            // exportCollisonToOBJToolStripMenuItem
            // 
            this.exportCollisonToOBJToolStripMenuItem.Name = "exportCollisonToOBJToolStripMenuItem";
            this.exportCollisonToOBJToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exportCollisonToOBJToolStripMenuItem.Text = "Export Collison to .OBJ";
            this.exportCollisonToOBJToolStripMenuItem.Click += new System.EventHandler(this.exportCollisonToOBJToolStripMenuItem_Click);
            // 
            // expandMeshX3ToolStripMenuItem
            // 
            this.expandMeshX3ToolStripMenuItem.Name = "expandMeshX3ToolStripMenuItem";
            this.expandMeshX3ToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.expandMeshX3ToolStripMenuItem.Text = "Expand Collision Mesh X3";
            this.expandMeshX3ToolStripMenuItem.Visible = false;
            this.expandMeshX3ToolStripMenuItem.Click += new System.EventHandler(this.expandMeshX3ToolStripMenuItem_Click);
            // 
            // makeBSPStickyToolStripMenuItem
            // 
            this.makeBSPStickyToolStripMenuItem.Name = "makeBSPStickyToolStripMenuItem";
            this.makeBSPStickyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.makeBSPStickyToolStripMenuItem.Text = "Make BSP Sticky";
            this.makeBSPStickyToolStripMenuItem.Visible = false;
            this.makeBSPStickyToolStripMenuItem.Click += new System.EventHandler(this.makeBSPStickyToolStripMenuItem_Click);
            // 
            // exportOBJToolStripMenuItem
            // 
            this.exportOBJToolStripMenuItem.Name = "exportOBJToolStripMenuItem";
            this.exportOBJToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.exportOBJToolStripMenuItem.Text = "Export BSP Meshes to .OBJs";
            this.exportOBJToolStripMenuItem.Click += new System.EventHandler(this.exportOBJToolStripMenuItem_Click);
            // 
            // injectBSPVisualMeshToolStripMenuItem
            // 
            this.injectBSPVisualMeshToolStripMenuItem.Name = "injectBSPVisualMeshToolStripMenuItem";
            this.injectBSPVisualMeshToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.injectBSPVisualMeshToolStripMenuItem.Text = "Inject BSP Visual Meshes";
            this.injectBSPVisualMeshToolStripMenuItem.Click += new System.EventHandler(this.injectBSPVisualMeshToolStripMenuItem_Click);
            // 
            // exportMeshToOBJToolStripMenuItem
            // 
            this.exportMeshToOBJToolStripMenuItem.Name = "exportMeshToOBJToolStripMenuItem";
            this.exportMeshToOBJToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.exportMeshToOBJToolStripMenuItem.Text = "Export Meshes To Single OBJ";
            this.exportMeshToOBJToolStripMenuItem.Click += new System.EventHandler(this.exportMeshToOBJToolStripMenuItem_Click);
            // 
            // injectMeshesFromOBJToolStripMenuItem
            // 
            this.injectMeshesFromOBJToolStripMenuItem.Name = "injectMeshesFromOBJToolStripMenuItem";
            this.injectMeshesFromOBJToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.injectMeshesFromOBJToolStripMenuItem.Text = "Inject Meshes From Single OBJ";
            this.injectMeshesFromOBJToolStripMenuItem.Click += new System.EventHandler(this.injectMeshFromOBJToolStripMenuItem_Click);
            // 
            // collContextMenu
            // 
            this.collContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractMeshToOBJToolStripMenuItem,
            this.collisonViewerToolStripMenuItem});
            this.collContextMenu.Name = "collContextMenu";
            this.collContextMenu.Size = new System.Drawing.Size(188, 48);
            // 
            // extractMeshToOBJToolStripMenuItem
            // 
            this.extractMeshToOBJToolStripMenuItem.Name = "extractMeshToOBJToolStripMenuItem";
            this.extractMeshToOBJToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.extractMeshToOBJToolStripMenuItem.Text = "Extract Mesh to .OBJ";
            this.extractMeshToOBJToolStripMenuItem.Click += new System.EventHandler(this.extractMeshToOBJToolStripMenuItem_Click);
            // 
            // collisonViewerToolStripMenuItem
            // 
            this.collisonViewerToolStripMenuItem.Name = "collisonViewerToolStripMenuItem";
            this.collisonViewerToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.collisonViewerToolStripMenuItem.Text = "Collison Viewer";
            this.collisonViewerToolStripMenuItem.Click += new System.EventHandler(this.collisonViewerToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = ".obj |*.obx|.X|*.X";
            // 
            // prtmcontext
            // 
            this.prtmcontext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPRTMToolStripMenuItem,
            this.extractPRTMOBJToolStripMenuItem});
            this.prtmcontext.Name = "prtmcontext";
            this.prtmcontext.Size = new System.Drawing.Size(177, 48);
            // 
            // viewPRTMToolStripMenuItem
            // 
            this.viewPRTMToolStripMenuItem.Name = "viewPRTMToolStripMenuItem";
            this.viewPRTMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.viewPRTMToolStripMenuItem.Text = "View Particle Model";
            this.viewPRTMToolStripMenuItem.Click += new System.EventHandler(this.viewPRTMToolStripMenuItem_Click);
            // 
            // extractPRTMOBJToolStripMenuItem
            // 
            this.extractPRTMOBJToolStripMenuItem.Name = "extractPRTMOBJToolStripMenuItem";
            this.extractPRTMOBJToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.extractPRTMOBJToolStripMenuItem.Text = "Extract PRTM .OBJ";
            this.extractPRTMOBJToolStripMenuItem.Click += new System.EventHandler(this.extractPRTMOBJToolStripMenuItem_Click);
            // 
            // openBitmapDialog1
            // 
            this.openBitmapDialog1.Filter = "DDS  (*.DDS) | *.DDS";
            // 
            // timer1
            // 
            this.timer1.Interval = 80;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // showAnimatedBitmapsToolStripMenuItem
            // 
            this.showAnimatedBitmapsToolStripMenuItem.Checked = true;
            this.showAnimatedBitmapsToolStripMenuItem.CheckOnClick = true;
            this.showAnimatedBitmapsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAnimatedBitmapsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showAnimatedBitmapsToolStripMenuItem.Name = "showAnimatedBitmapsToolStripMenuItem";
            this.showAnimatedBitmapsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showAnimatedBitmapsToolStripMenuItem.Text = "Show Animated Bitmaps";
            // 
            // MapForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(1094, 691);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Name = "MapForm";
            this.Text = "MapForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.TextChanged += new System.EventHandler(this.MapForm_TextChanged);
            this.treeviewcontext.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.BitmapContextStrip.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.searchGroupBox.ResumeLayout(false);
            this.searchGroupBox.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.ResumeLayout(false);
            this.LowerOptionsBar.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.MetaEditorPanel.ResumeLayout(false);
            this.ModelContextStrip.ResumeLayout(false);
            this.BSPcontextMenu.ResumeLayout(false);
            this.collContextMenu.ResumeLayout(false);
            this.prtmcontext.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private MenuItem JumpToTagItem;
        private Panel MetaEditor2Panel;
        private Button btnUndock;
        private Label lblRawSize;
        private RichTextBox metaRawBox;
        private Timer timer1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem StringEditorToolStripMenuItem;
        private ToolStripMenuItem showAnimatedBitmapsToolStripMenuItem;
    }
}

