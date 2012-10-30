// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapForm.cs" company="">
//   
// </copyright>
// <summary>
//   The map form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    using entity.MetaEditor2;
    using entity.MetaFuncs;
    using entity;
    using entity.Renderers;
    using entity.Tools;

    using Globals;

    using HaloMap;
    using HaloMap.ChunkCloning;
    using HaloMap.DDSFunctions;
    using HaloMap.H2MetaContainers;
    using HaloMap.Libraries;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.RawData;
    using HaloMap.RealTimeHalo;
    using HaloMap.Render;

    using Microsoft.DirectX.Direct3D;
    using Microsoft.Win32;

    /// <summary>
    /// The map form.
    /// </summary>
    /// <remarks></remarks>
    public partial class MapForm : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The rth info.
        /// </summary>
        public RTHData RTHInfo;

        /// <summary>
        /// The form funcs.
        /// </summary>
        public FormFunctions formFuncs = new FormFunctions();

        /// <summary>
        /// Used in reference sorting, contains column # for sort
        /// </summary>
        private int sortColumn = -1;
        /// <summary>
        /// Used in reference sorting, contains boolean for reverse sort
        /// </summary>
        private bool sortReverse = false;

        /// <summary>
        /// The meta view.
        /// </summary>
        public FormFunctions.MetaView metaView = FormFunctions.MetaView.TagTypeView;

        /// <summary>
        /// The s swap.
        /// </summary>
        public MEStringsSelector sSwap;

        /// <summary>
        /// The plugins.
        /// </summary>
        private readonly LibraryManager plugins = new LibraryManager();

        /// <summary>
        /// The info selected.
        /// </summary>
        private bool infoSelected;

        /// <summary>
        /// The new name.
        /// </summary>
        private string newName;

        /// <summary>
        /// The selectedplugin.
        /// </summary>
        private int selectedplugin = -1;

        /// <summary>
        /// Keeps track of the currently viewed bitmap (for animated bitmaps)
        /// </summary>
        private int bitmapCount = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapForm"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public MapForm(Map map)
        {
            RTHInfo = new RTHData();
            InitializeComponent();

            this.map = map;
            this.Text = map.filePath;

            // this is here so that we can see the panel in design mode
            // this.splitContainer4.SplitterDistance = this.splitContainer4.Height - 27;
            searchComboBox.SelectedIndex = 0;

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            toolTip.SetToolTip(
                this.recursiveCheckBox, 
                "Utilizes all tags listed within the currently selected tag instead of just the root tag");
            toolTip.SetToolTip(
                this.parsedCheckBox, "Calculates the actual data size of the tag instead of reading it from file");
            toolTip.SetToolTip(this.soundsCheckBox, "Adds sounds into map when building");
            toolTip.SetToolTip(
                this.scanbspwithifp, "Scans reflexives, idents & strings using plugins instead of scanning manually");
            toolTip.SetToolTip(this.saveMetaButton, "Exports tag data");

            // Load Strings Table in a background process
            ThreadStart ts = delegate { LoadStringsBackgroundWorker(map.Strings.Name); };
            Thread thr = new Thread(ts);
            thr.Start();

            // attempt to load custom skin
            try
            {
                LoadSkin();
            }
            catch
            {
                // Don't show skin errors EVERY time a map is opened. We perform a check when
                // the main form is loaded and display the error once then.

                // Global.ShowErrorMsg("There was an error while loading the skin",ex);
            }

            // display magic
            primaryMagicBox.Text = map.PrimaryMagic.ToString("X");
            secondaryMagicBox.Text = map.SecondaryMagic.ToString("X");

            // show tags in tree
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);

            // Add plugins to menu
            plugins.Plugin.Add(Global.StartupPath + "\\Libraries");
            UpdatePluginsMenu();

            // display map image on form
            if (map.HaloVersion == HaloVersionEnum.Halo2 || 
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                DisplayMapBitmap();
            }

            // format listview columns
            formFuncs.AddColumnsToListView(references, map.DisplayType);

            // Update the "Visual Editor" button to support multiple BSPs if
            // there are more than one
            if (map.BSP.bspcount > 1)
            {
                int index = toolStrip2.Items.IndexOf(toolStripBSPEditor);
                toolStrip2.Items.RemoveAt(index);
                toolStrip2.Items.Insert(index, toolStripBSPEditorDropDown);

                toolStripBSPEditorDropDown.DropDownItems.Clear();
                for (int x = 0; x < map.BSP.bspcount; x++)
                {
                    // create a dropdown button for each bsp
                    ToolStripMenuItem tsMenuItem = new ToolStripMenuItem();
                    tsMenuItem.Name = "toolStripMenuItem" + x;

                    // tsMenuItem.Size = new System.Drawing.Size(179, 22);
                    tsMenuItem.Text = map.FileNames.Name[map.BSP.sbsp[x].TagIndex];
                    tsMenuItem.Click += this.toolStripBSPEditor_Click;
                    toolStripBSPEditorDropDown.DropDownItems.Add(tsMenuItem);
                }
            }

            foreach (Prefs.CustomPluginMask mask in Prefs .CustomPluginMasks)
            {
                comboBox1.Items.Add(mask.Name);
            }

            // Default to Complete Plugin set
            comboBox1.SelectedItem = comboBox1.Items[0];

            // Set focus to the TAG tree list
            treeView1.Select();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets map.
        /// </summary>
        /// <remarks></remarks>
        public Map map { get; private set; }

        /// <summary>
        /// Gets or sets pictureBox.
        /// </summary>
        /// <value>The picture box.</value>
        /// <remarks></remarks>
        public Bitmap pictureBox
        {
            get
            {
                return (Bitmap)pictureBox1.Image;
            }

            set
            {
                pictureBox1.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets statusBarText.
        /// </summary>
        /// <value>The status bar text.</value>
        /// <remarks></remarks>
        public string statusBarText
        {
            get
            {
                return statusbar.Text;
            }

            set
            {
                statusbar.Text = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A list of all the available editor panels
        /// </summary>
        public enum EditorModes
        {
            LastMode = -1,
            ReferenceEditor = 0,
            MetaEditor1 = 1,
            MetaEditor2 = 2,
            HexViewer = 3,
            BitmapViewer = 4,
            PluginViewer = 5
        }

        /// <summary>
        /// Returns the currently viewed editor panel
        /// </summary>
        /// <returns>The currently viewed editor panel</returns>
        public EditorModes getEditorMode()
        {
            if (this.splitContainer2.Panel1.Controls.Count == 0)
                return EditorModes.LastMode;
            switch (this.splitContainer2.Panel1.Controls[0].Name)
            {
                case "hexView1":
                    return EditorModes.HexViewer;
                case "LibraryPanel":
                    return EditorModes.PluginViewer;
                case "ltmpTools":
                    return EditorModes.BitmapViewer;
                case "MetaEditorPanel":
                    return EditorModes.MetaEditor1;
                case "MetaEditor2Panel":
                    return EditorModes.MetaEditor2;
                case "references":
                    return EditorModes.ReferenceEditor;
                default:
                    return EditorModes.LastMode;
            }
        }

        /// <summary>
        /// Sets the currently viewed editor panel
        /// </summary>
        /// <param name="editMode">The edit panel to view</param>
        public void setEditorMode(EditorModes editMode)
        {
            // If we want the last used panel, see which panel is one level down and activate it
            if (editMode == EditorModes.LastMode && this.splitContainer2.Panel1.Controls.Count > 1)
            {
                switch (this.splitContainer2.Panel1.Controls[1].Name)
                {
                    case "hexView1":
                        editMode = EditorModes.HexViewer;
                        break;
                    case "LibraryPanel":
                        editMode = EditorModes.PluginViewer;
                        break;
                    case "ltmpTools":
                        editMode = EditorModes.BitmapViewer;
                        break;
                    case "MetaEditorPanel":
                        editMode = EditorModes.MetaEditor1;
                        break;
                    case "MetaEditor2Panel":
                        editMode = EditorModes.MetaEditor2;
                        break;
                    case "references":
                        editMode = EditorModes.ReferenceEditor;
                        break;
                    default:
                        return;
                }
            }

            // Only allow bitmaps to select Bitmap Viewer
            if (editMode == EditorModes.BitmapViewer && map.SelectedMeta.type != "bitm")
            {
                ltmpTools.Visible = true;
                return;
            }

            // "Check" the selected panel
            hexEditorToolStripMenuItem.Checked = (editMode == EditorModes.HexViewer);
            bitmapEditorToolStripMenuItem.Checked = (editMode == EditorModes.BitmapViewer);
            metaEditorToolStripMenuItem.Checked = (editMode == EditorModes.MetaEditor1);
            metaEditorNewToolStripMenuItem.Checked = (editMode == EditorModes.MetaEditor2);
            pluginsToolStripMenuItem.Checked = (editMode == EditorModes.PluginViewer);
            referenceEditorToolStripMenuItem.Checked = (editMode == EditorModes.ReferenceEditor);

            Control c = null;
            switch (editMode)
            {
                case EditorModes.BitmapViewer:
                    c = ltmpTools;
                    break;
                case EditorModes.HexViewer:
                    c = hexView1;
                    break;
                case EditorModes.MetaEditor1:
                    c = MetaEditorPanel;
                    break;
                case EditorModes.MetaEditor2:
                    c = MetaEditor2Panel;
                    break;
                case EditorModes.PluginViewer:
                    c = LibraryPanel;
                    break;
                case EditorModes.ReferenceEditor:
                    c = references;
                    break;
            }

            if (c != null)
            {
                c.Visible = true;

                if (c.Parent == splitContainer2.Panel1)
                {
                    c.BringToFront();

                    // Hide all other panels, except for Bitmap Viewer
                    for (int i = 1; i < this.splitContainer2.Panel1.Controls.Count; i++)
                    {
                        if (this.splitContainer2.Panel1.Controls[i] != ltmpTools)
                            this.splitContainer2.Panel1.Controls[i].Visible = false;
                    }

                    // This method allows us to have an auto-bitmap viewer
                    if (map.SelectedMeta != null && map.SelectedMeta.type == "bitm")
                        if (editMode != EditorModes.BitmapViewer)
                            ltmpTools.Visible = false;
                        else
                            this.splitContainer2.Panel1.Controls[1].Visible = true;
                            //ltmpTools.SendToBack();
                }
            }

            if (map.SelectedMeta != null)
            {
                LoadMeta(map.SelectedMeta.TagIndex);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                map.OpenMap(MapTypes.Internal);
                map.SelectedMeta = GetMapBitmapMeta(map);
                map.CloseMap();
                this.selectTag(map.SelectedMeta.TagIndex);
            }
            if (map.SelectedMeta.type == "bitm")
            {
                this.ltmpTools.Visible = true;
                this.ltmpTools.BringToFront();
            }
        }

        /// <summary>
        /// The get map bitmap meta.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Meta GetMapBitmapMeta(Map map)
        {
            Meta m = null;
            map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 920;
            int tempc2 = map.BR.ReadInt32();
            int tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;
            map.BR.BaseStream.Position = tempr2 + 16;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            if (tempc != 0)
            {
                map.BR.BaseStream.Position = tempr + 8;
                int temptag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                m = new Meta(map);
                m.ReadMetaFromMap(temptag, false);
            }
            else
            {
                map.BR.BaseStream.Position = tempr2 + 8;
                tempc = map.BR.ReadInt32();
                if (tempc != 0)
                {
                    tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                    map.BR.BaseStream.Position = tempr + 12;
                    int temptag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    m = new Meta(map);
                    m.ReadMetaFromMap(temptag, false);
                }
            }

            return m;
        }

        /// <summary>
        /// The display map bitmap.
        /// </summary>
        /// <remarks></remarks>
        public void DisplayMapBitmap()
        {
            map.OpenMap(MapTypes.Internal);
            try
            {
                Meta m = GetMapBitmapMeta(map);
                if (m != null)
                {
                    ParsedBitmap pm = new ParsedBitmap(ref m, map);
                    //if (bitmMainPtr != IntPtr.Zero)
                    //{
                    //    Marshal.FreeHGlobal(bitmMainPtr);
                    //}

                    pictureBox1.Image = pm.FindChunkAndDecode(0, 0, 0, ref m, map, 0, 0);
                }
            }
            catch
            {
                // Who cares if we can't load the map bitmap due to a missing MainMenu.map?
            }
            finally
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The load meta.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <remarks></remarks>
        public void LoadMeta(int tag)
        {
            Meta meta = Map.GetMetaFromTagIndex(tag, map, scanbspwithifp.Checked, parsedCheckBox.Checked);
            map.SelectedMeta = meta;

            buttonInternalize.Visible = false;
            if (meta.type == "bitm")
            {
                // ;&&Map.HaloVersion!= Map.HaloVersionEnum.Halo1 )
                ParsedBitmap pm = new ParsedBitmap(ref meta, map);

                Bitmap b = pm.FindChunkAndDecode(0, 0, 0, ref meta, map, 0, 0);

                // Raw.ParsedBitmap.BitmapInfo bmInfo = new Entity.Raw.ParsedBitmap.BitmapInfo(pm.Properties[0].formatname, pm.Properties[0].swizzle);
                // b = DDS_Convert.DecodeDDS( DDS_Convert.EncodeDDS(b, ref bmInfo), bmInfo );
                pictureBox1.Image = b;
                statusbar.Text = pm.Properties[0].width.ToString().PadLeft(4) + " X " +
                                 pm.Properties[0].height.ToString().PadRight(4) + " " +
                                 ("(" + pm.Properties[0].typename.ToString().Remove(0, 10) + ") ").PadRight(10) +
                                 pm.Properties[0].formatname.ToString().Remove(0, 12).PadRight(10) + " - Swizzle:" +
                                 pm.Properties[0].swizzle + "- Location: " + meta.raw.rawChunks[0].rawLocation;
                if (meta.raw.rawChunks[0].rawLocation != MapTypes.Internal)
                {
                    buttonInternalize.Visible = true;
                }
                timer1.Start();
            }
            else
            {
                statusbar.Text = string.Empty;
            }

            // Main Form Offsets/Idents/etc
            metaOffsetBox.Text = "0x" + meta.offset.ToString("X8") + "\n" + meta.offset.ToString("N0") + " bytes";
            metaSizeBox.Text = "0x" + meta.size.ToString("X4") + "\n" + meta.size.ToString("N0") + " bytes";
            metaIdentBox.Text = meta.ident.ToString("X");
            metaTypeBox.Text = meta.type;
            if (meta.raw != null)
            {
                int tempRawSize = 0;
                for (int i = 0; i < meta.raw.rawChunks.Count; i++)
                {
                    tempRawSize += meta.raw.rawChunks[i].size;
                }
                metaRawBox.Text = "0x" + tempRawSize.ToString("X4") + "\n" + tempRawSize.ToString("N0") + " bytes";
            }
            else
                metaRawBox.Text = "n/a";

            switch (meta.type)
            {
                case "PRTM":
                    rawDataDropDown.DropDown = prtmcontext;
                    rawDataDropDown.Enabled = true;
                    break;
                case "mode":
                case "mod2":
                    rawDataDropDown.DropDown = ModelContextStrip;
                    rawDataDropDown.Enabled = true;
                    break;
                case "bitm":
                    rawDataDropDown.DropDown = BitmapContextStrip;
                    rawDataDropDown.Enabled = true;
                    if (ltmpTools.Visible)
                    {
                        bitmapEditorToolStripMenuItem_Click(bitmapEditorToolStripMenuItem, null);
                    }

                    break;
                case "sbsp":
                    rawDataDropDown.DropDown = BSPcontextMenu;
                    rawDataDropDown.Enabled = true;
                    break;
                case "coll":
                    rawDataDropDown.DropDown = collContextMenu;
                    rawDataDropDown.Enabled = true;
                    break;
                default:
                    rawDataDropDown.Enabled = false;
                    break;
            }


            switch (getEditorMode())
            {
                case EditorModes.HexViewer:
                        hexView1.Reload(meta, map);
                        hexView1.Setup(map.filePath);
                    break;
                case EditorModes.MetaEditor1:
                    if (map.SelectedMeta != null)
                    {
                        if (this.metaEditor1.selectedTagType == map.SelectedMeta.type)
                        {
                            this.metaEditor1.ReloadMetaForSameTagType(true);
                        }
                        else
                        {
                            this.metaEditor1.loadControls(map);
                        }
                    }
                    break;
                case EditorModes.MetaEditor2:
                    if (map.SelectedMeta != null)
                    {
                        if (wME == null || wME.IsDisposed)
                        {
                            wME = new entity.MetaEditor2.WinMetaEditor(this, map);
                            wME.BackgroundColor = this.LibraryPanel.BackColor;
                            wME.ForegroundColor = this.LibraryPanel.ForeColor;

                            wME.TopLevel = false;
                            this.MetaEditor2Panel.Controls.Add(wME);

                            MetaEditor2Panel.BringToFront();

                            wME.FormBorderStyle = FormBorderStyle.None;
                            wME.FormClosed += new FormClosedEventHandler(wME_FormClosed);
                        }
                        // If we are not switching from a different editor mode (on MetaEditor2) and
                        // we already have the current tag loaded in the editor, then we can load a duplicate
                        wME.addNewTab(map.SelectedMeta, getEditorMode() == EditorModes.MetaEditor2 &&
                                                        wME.tabs.Tabs.Count > 0 &&
                                                        wME.tabs.SelectedTab.Text == ("[" + map.SelectedMeta.type + "] " + map.SelectedMeta.name.Substring(map.SelectedMeta.name.LastIndexOf('\\') + 1)));
                        wME.Dock = DockStyle.Fill;
                    }
                    break;
                case EditorModes.ReferenceEditor:
                    formFuncs.AddReferencesToListView(meta, references, map.DisplayType);
                    break;
            }

            if (selectedplugin != -1 && LibraryPanel.Visible)
            {
                if (plugins.Plugin[selectedplugin].tagtype == meta.type ||
                    plugins.Plugin[selectedplugin].tagtype == "!*.*")
                {
                    plugins.Plugin[selectedplugin].Run(map, ref progressbar);
                }
            }

            UpdatePluginsMenu();

            Prefs.Save();
            AddToHistory(tag);
        }

        /// <summary>
        /// Load custom user skin
        /// </summary>
        /// <remarks></remarks>
        public void LoadSkin()
        {
            Color Skin_Panel2_Color = this.panel2.BackColor;
            Color Skin_Panel2_Text_Color = label4.ForeColor;
            Color Skin_Panel2_Button_Color = signMapButton.BackColor;
            Color Skin_Panel2_ButtonText_Color = signMapButton.ForeColor;
            //Color Skin_Panel2_DisconnectButton_Color = buttDisconnect.BackColor;
            //Color Skin_Panel2_DisconnectButton_TextColor = buttDisconnect.ForeColor;
            Color Skin_Panel3_Color = this.panel3.BackColor;
            Color Skin_Panel3_Text_Color = metaOffsetBox.ForeColor;
            Color Skin_Panel3_Button_Color = saveMetaButton.BackColor;
            Color Skin_Panel3_ButtonText_Color = saveMetaButton.ForeColor;
            Color Skin_LibraryPanel_Color = this.LibraryPanel.BackColor;
            Color Skin_LibraryPanel_Text_Color = this.references.ForeColor;
            Color Skin_Treeview1_Background_Color = treeView1.BackColor;
            Color Skin_Treeview1_Text_Color = treeView1.ForeColor;

            StreamReader SettingsStreamReader = new StreamReader(Global.StartupPath + "\\Skins\\Settings.xml");
            XmlTextReader SettingsXMLReader = new XmlTextReader(SettingsStreamReader);
            XmlDocument settingsxmlreader = new XmlDocument();
            settingsxmlreader.Load(SettingsXMLReader);
            XmlNodeList settingsnodelist = settingsxmlreader.SelectNodes("skin/settings");

            // Use Skin
            XmlNode SettingsInfo = settingsnodelist.Item(0).SelectSingleNode("Use_Skin");
            string Use_Skin = SettingsInfo.InnerText;

            // Skin Path
            SettingsInfo = settingsnodelist.Item(0).SelectSingleNode("Skin_Path");
            string Skin_Path = Global.StartupPath + "\\skins\\" + SettingsInfo.InnerText;
            SettingsStreamReader.Close();

            if (Use_Skin == "true")
            {
                #region XMLReader

                if (File.Exists(Skin_Path))
                {
                    StreamReader sr = new StreamReader(Skin_Path);
                    XmlTextReader xr = new XmlTextReader(sr);
                    XmlDocument settingsxml = new XmlDocument();
                    settingsxml.Load(xr);
                    XmlNodeList FormSettingsXmlNode = settingsxml.SelectNodes("skin/settings");
                    XmlNode colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_Color");
                    Skin_Panel2_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_Text_Color");
                    Skin_Panel2_Text_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_Button_Color");
                    Skin_Panel2_Button_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_ButtonText_Color");
                    Skin_Panel2_ButtonText_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    //colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_DisconnectButton_Color");
                    //Skin_Panel2_DisconnectButton_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    //colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel2_DisconnectButton_TextColor");
                    //Skin_Panel2_DisconnectButton_TextColor = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel3_Color");
                    Skin_Panel3_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel3_Text_Color");
                    Skin_Panel3_Text_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel3_Button_Color");
                    Skin_Panel3_Button_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Panel3_ButtonText_Color");
                    Skin_Panel3_ButtonText_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_LibraryPanel_Color");
                    Skin_LibraryPanel_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_LibraryPanel_Text_Color");
                    Skin_LibraryPanel_Text_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Treeview1_Background_Color");
                    Skin_Treeview1_Background_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    colorfinder = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Treeview1_Text_Color");
                    Skin_Treeview1_Text_Color = Color.FromArgb(Convert.ToInt32(colorfinder.InnerText));
                    sr.Close();

                    #endregion

                    #region Panel2Shit

                    // Set up the Panel Color
                    this.panel2.BackColor = Skin_Panel2_Color;
                    primaryMagicBox.BackColor = Skin_Panel2_Color;
                    secondaryMagicBox.BackColor = Skin_Panel2_Color;
                    splitContainer1.BackColor = Skin_Panel2_Color;
                    this.BackColor = Skin_Panel2_Color;

                    // Set all the font color
                    label4.ForeColor = Skin_Panel2_Text_Color;
                    label6.ForeColor = Skin_Panel2_Text_Color;
                    primaryMagicBox.ForeColor = Skin_Panel2_Text_Color;
                    secondaryMagicBox.ForeColor = Skin_Panel2_Text_Color;

                    // Set the button color
                    signMapButton.BackColor = Skin_Panel2_Button_Color;
                    buildButton.BackColor = Skin_Panel2_Button_Color;
                    button1.BackColor = Skin_Panel2_Button_Color;
                    analyzeMapButton.BackColor = Skin_Panel2_Button_Color;
                    //buttConnect.BackColor = Skin_Panel2_Button_Color;

                    // Set the button font color
                    signMapButton.ForeColor = Skin_Panel2_ButtonText_Color;
                    buildButton.ForeColor = Skin_Panel2_ButtonText_Color;
                    button1.ForeColor = Skin_Panel2_ButtonText_Color;
                    analyzeMapButton.ForeColor = Skin_Panel2_ButtonText_Color;
                    //buttConnect.ForeColor = Skin_Panel2_ButtonText_Color;

                    // Set the disconnect button color and font
                    //buttDisconnect.BackColor = Skin_Panel2_DisconnectButton_Color;
                    //buttDisconnect.ForeColor = Skin_Panel2_DisconnectButton_TextColor;

                    #endregion

                    #region Panel3Shit

                    // Set up the Panel Color
                    this.panel3.BackColor = Skin_Panel3_Color;
                    metaOffsetBox.BackColor = Skin_Panel3_Color;
                    metaSizeBox.BackColor = Skin_Panel3_Color;
                    metaIdentBox.BackColor = Skin_Panel3_Color;
                    metaTypeBox.BackColor = Skin_Panel3_Color;
                    metaRawBox.BackColor = Skin_Panel3_Color;

                    // Set up the font color
                    label1.ForeColor = Skin_Panel3_Text_Color;
                    label2.ForeColor = Skin_Panel3_Text_Color;
                    label3.ForeColor = Skin_Panel3_Text_Color;
                    label5.ForeColor = Skin_Panel3_Text_Color;
                    lblRawSize.ForeColor = Skin_Panel3_Text_Color;
                    metaOffsetBox.ForeColor = Skin_Panel3_Text_Color;
                    metaSizeBox.ForeColor = Skin_Panel3_Text_Color;
                    metaIdentBox.ForeColor = Skin_Panel3_Text_Color;
                    metaTypeBox.ForeColor = Skin_Panel3_Text_Color;
                    metaRawBox.ForeColor = Skin_Panel3_Text_Color;
                    recursiveCheckBox.ForeColor = Skin_Panel3_Text_Color;
                    parsedCheckBox.ForeColor = Skin_Panel3_Text_Color;
                    soundsCheckBox.ForeColor = Skin_Panel3_Text_Color;
                    scanbspwithifp.ForeColor = Skin_Panel3_Text_Color;
                    recursiveCheckBox.BackColor = Skin_Panel3_Color;
                    parsedCheckBox.BackColor = Skin_Panel3_Color;
                    soundsCheckBox.BackColor = Skin_Panel3_Color;
                    scanbspwithifp.BackColor = Skin_Panel3_Color;

                    // Set up the button color
                    saveMetaButton.BackColor = Skin_Panel3_Button_Color;
                    loadMetaButton.BackColor = Skin_Panel3_Button_Color;

                    // Set up the button text color
                    saveMetaButton.ForeColor = Skin_Panel3_ButtonText_Color;
                    loadMetaButton.ForeColor = Skin_Panel3_ButtonText_Color;

                    #endregion

                    #region LibraryPanelColors

                    // Set up the panel color
                    this.LibraryPanel.BackColor = Skin_LibraryPanel_Color;
                    references.BackColor = Skin_LibraryPanel_Color;
                    references.ForeColor = Skin_LibraryPanel_Text_Color;
                    ltmpTools.BackColor = Skin_LibraryPanel_Color;

                    #endregion

                    #region Treeview1Colors

                    // Set up the treeview background color
                    treeView1.BackColor = Skin_Treeview1_Background_Color;

                    // Set up the treeview font color
                    treeView1.ForeColor = Skin_Treeview1_Text_Color;

                    #endregion
                }
            }
        }

        /// <summary>
        /// The refresh tree view.
        /// </summary>
        /// <remarks></remarks>
        public void RefreshTreeView()
        {
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);

            // For the Complete plugin set, stop here
            if (comboBox1.SelectedIndex == 0)
            {
                return;
            }

            Prefs.CustomPluginMask pluginMask = Prefs.CustomPluginMasks[comboBox1.SelectedIndex - 1];

            metaEditor1.pluginName = comboBox1.SelectedItem.ToString();

            // Check quick list
            TreeNode tn = treeView1.Nodes.Count > 1 ? treeView1.Nodes[1] : null;
            for (int i = 0; i < 2; i++)
            {
                while (tn != null)
                {
                    bool removeNode = false;
                    if (!pluginMask.VisibleTagTypes.Contains(tn.Text)) removeNode = true;

                    if (removeNode)
                    {
                        TreeNode oldNode = tn;
                        tn = tn.NextNode;
                        oldNode.Remove();
                    }
                    else tn = tn.NextNode;
                }

                // Check main listings
                if (treeView1.Nodes[0].Nodes.Count == 0) break;
                tn = treeView1.Nodes[0].Nodes[0];
            }
        }

        /// <summary>
        /// Selects the current tag in the Treeview Listing (adds to quick list if not already ppresent) and loads the meta
        /// </summary>
        /// <param name="tagNum">The tag number to be selected and loaded</param>
        public void selectTag(int tagNum)
        {
            if (tagNum == -1)
            {
                return;
            }
            
            string tagType = map.MetaInfo.TagType[tagNum];
            string tagName = map.FileNames.Name[tagNum];

            addToQuickList(tagType, tagName);
            AddToHistory(tagNum);

            if (toolStripTagView.Checked)
            {
                // Check our quick list
                for (int count = 0; count < treeView1.Nodes.Count; count++)
                {
                    if (treeView1.Nodes[count].Text == tagType)
                    {
                        for (int count2 = 0; count2 < treeView1.Nodes[count].Nodes.Count; count2++)
                        {
                            if (treeView1.Nodes[count].Nodes[count2].Text == tagName)
                            {
                                treeView1.SelectedNode = treeView1.Nodes[count].Nodes[count2];
                                return;
                            }
                        }
                    }
                }

                // should never get here since we added tag to our list, but backup code anyways...
                // otherwise, check the main listing and put into out quick list
                for (int count = 0; count < treeView1.Nodes[0].Nodes.Count; count++)
                {
                    if (treeView1.Nodes[0].Nodes[count].Text == tagType)
                    {
                        for (int count2 = 0; count2 < treeView1.Nodes[0].Nodes[count].Nodes.Count; count2++)
                        {
                            if (treeView1.Nodes[0].Nodes[count].Nodes[count2].Text == tagName)
                            {
                                treeView1.SelectedNode = treeView1.Nodes[0].Nodes[count].Nodes[count2];
                                for (int count3 = 0; count3 < treeView1.Nodes.Count; count3++)
                                {
                                    if (treeView1.Nodes[count3].Text == tagType)
                                    {
                                        for (int count4 = 0; count4 < treeView1.Nodes[count3].Nodes.Count; count4++)
                                        {
                                            if (treeView1.Nodes[count3].Nodes[count4].Text == tagName)
                                            {
                                                treeView1.SelectedNode = treeView1.Nodes[count3].Nodes[count4];
                                                return;
                                            }
                                        }
                                    }
                                }

                                return;
                            }
                        }
                    }
                }
            }
            else if (toolStripFolderView.Checked)
            {
                Global.ShowErrorMsg("Jump to tag only supported in Tag View mode.", null);
            }
            else if (toolStripInfoView.Checked)
            {
                Global.ShowErrorMsg("Jump to tag only supported in Tag View mode.", null);
            }
        }

        /// <summary>
        /// The set map name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <remarks></remarks>
        public void setMapName(string newName)
        {
            newName = newName.ToLower();
            string newPath;
            if (!newName.Contains("\\"))
            {
                string[] sa = map.MapHeader.scenarioPath.Split('\\');
                string temp = "";
                for (int i = 0; i < sa.Length - 2; i++)
                    temp += sa[i] + "\\";
                newPath = temp + newName + "\\" + newName;
            }
            else
            {
                newPath = newName;
                string[] sa = newPath.Split('\\');
                newName = sa[sa.Length - 1];
            }

            bool wasOpen = map.isOpen;
            map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = 408;
            byte[] ba = (System.Text.Encoding.ASCII).GetBytes(newName.PadRight(36, '\0').ToCharArray());
            map.BR.BaseStream.Write(ba, 0, 36);
            map.BR.BaseStream.Position = 444;
            ba = (System.Text.Encoding.ASCII).GetBytes(newPath.PadRight(256, '\0').ToCharArray());
            map.BR.BaseStream.Write(ba, 0, 256);
            map.MapHeader.mapName = newName;
            map.MapHeader.scenarioPath = newPath;
            if (!wasOpen)
                map.CloseMap();
            MessageBox.Show("The map has been renamed to:\n" + newName + "\n\nThe internal path is:\n" + newPath);
        }

        /// <summary>
        /// The set progress bar.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
        public void SetProgressBar(int value)
        {
            progressbar.Minimum = 0;
            progressbar.Maximum = 100;
            if (value < this.progressbar.Minimum)
            {
                value = this.progressbar.Minimum;
            }

            if (value > this.progressbar.Maximum)
            {
                value = this.progressbar.Maximum;
            }

            this.progressbar.Value = value;
        }

        /// <summary>
        /// Gets relevant plugins from plugin manager and loads them into the menu
        /// </summary>
        /// <remarks></remarks>
        public void UpdatePluginsMenu()
        {
            // clear menu
            pluginsToolStripMenuItem.DropDownItems.Clear();

            // get selected tag type if there is a selected tag, so
            // we can show plugins for this type
            string curTagType = null;
            if (map.SelectedMeta != null)
            {
                curTagType = map.SelectedMeta.type;
            }

            // iterate plugins in plugin manager
            for (int i = 0; i < plugins.Plugin.Count; i++)
            {
                // if plugin is of type (any) or matches the selected tag, add it to the menu
                if (plugins.Plugin[i].tagtype == "*.*" || plugins.Plugin[i].tagtype == "!*.*" ||
                    plugins.Plugin[i].tagtype == curTagType)
                {
                    // create menu item and set text to plugin name
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(plugins.Plugin[i].GetName());

                    // give item a click event method
                    menuItem.Click += pluginToolStripMenuItemdescription_Click;

                    // add
                    pluginsToolStripMenuItem.DropDownItems.Add(menuItem);
                }
            }
            if (pluginsToolStripMenuItem.DropDownItems.Count == 0)
                pluginsToolStripMenuItem.Enabled = false;
            else
                pluginsToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// The scanandpassmetasfordraganddrop.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList scanandpassmetasfordraganddrop()
        {
            map.OpenMap(MapTypes.Internal);
            ArrayList metas = map.SelectedMeta.RecursivelyLoadMetas(parsedCheckBox.Checked, progressbar);
            map.CloseMap();
            return metas;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add to history.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <remarks></remarks>
        private void AddToHistory(int tag)
        {
            ToolStripMenuItem tsItem;

            string name = "[" + map.MetaInfo.TagType[tag] + "] " + map.FileNames.Name[tag];
            ToolStripItem[] tsItems = toolStripHistoryList.DropDownItems.Find(name, false);
            if (tsItems.Length == 0)
            {
                tsItem = new ToolStripMenuItem();
                tsItem.Tag = new[] { tag.ToString(), map.MetaInfo.TagType[tag], map.FileNames.Name[tag] };
                tsItem.Name = name;
                tsItem.Text = name;
                tsItem.Click += tsItem_Click;
            }
            else
            {
                tsItem = (ToolStripMenuItem)tsItems[0];
                toolStripHistoryList.DropDownItems.Remove(tsItem);
            }

            toolStripHistoryList.DropDownItems.Insert(0, tsItem);
            while (toolStripHistoryList.DropDownItems.Count > 12)
            {
                toolStripHistoryList.DropDownItems.RemoveAt(toolStripHistoryList.DropDownItems.Count - 1);
            }
        }

        /// <summary>
        /// The ask for tag name.
        /// </summary>
        /// <remarks></remarks>
        private void AskForTagName()
        {
            

            Form newForm = new Form();
            newForm.Text = "Select new tag name";
            newForm.Size = new Size(500, 110);
            newForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            newForm.FormClosing += this.newForm_FormClosing;

            

            #region label data

            Label newLabel = new Label();
            newLabel.Text = "New Name";
            newLabel.Location = new Point(10, 14);
            newLabel.Size = new Size(68, 18);

            #endregion

            #region Textbox Data

            TextBox newTextBox = new TextBox();
            newTextBox.Location = new Point(80, 10);
            newTextBox.Size = new Size(400, 10);
            newTextBox.Text = newName; // Set the name to our predicted name

            #endregion

            #region Button Data

            Button newAddButton = new Button();
            newAddButton.Text = "Create new tag";
            newAddButton.AutoSize = false;
            newAddButton.Size = new Size(160, 30);
            newAddButton.Location = new Point(170, 40);
            newAddButton.Parent = newForm;
            newAddButton.Click += this.newAddButton_Click;

            #endregion

            #region Add Controls

            newForm.Controls.Add(newLabel);
            newForm.Controls.Add(newTextBox);
            newForm.Controls.Add(newAddButton);
            newForm.ShowDialog();

            #endregion
        }

        /// <summary>
        /// Recursively adds nodes to Chunk Tree Listing
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <remarks></remarks>
        private void DisplaySplit(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            for (int x = 0; x < reflex.Chunks.Count; x++)
            {
                string tempchunk = reflex.description;
                foreach (Meta.Item mi in reflex.Chunks[x].ChunkResources)
                {
                    if (mi.type == Meta.ItemType.Ident)
                    {
                        tempchunk = "[" + ((Meta.Ident)mi).pointstotagtype + "] " + ((Meta.Ident)mi).pointstotagname;
                        TreeNode ChunkNumberNode = new TreeNode(tempchunk);
                        tn.Nodes.Add(ChunkNumberNode);

                        // break;
                    }
                }

                string tempchunkname = tempchunk;

                MetaSplitter.SplitReflexive split = reflex.Chunks[x];
                for (int y = 0; y < split.ChunkResources.Count; y++)
                {
                    if (split.ChunkResources[y].type == Meta.ItemType.Reflexive)
                    {
                        DisplaySplit((MetaSplitter.SplitReflexive)split.ChunkResources[y], tn);
                    }
                }
            }
        }

        /// <summary>
        /// is only available for Idents, so we don't need to check
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void FloodfillSwapItem_Click(object sender, EventArgs e)
        {
            if (parsedCheckBox.Checked)
            {
                MessageBox.Show("Turn off parsing first");
                return;
            }

            if (references.SelectedIndices.Count == 0)
            {
                return;
            }

            List<int> idTags = new List<int>();
            List<int> idOffs = new List<int>();

            Meta.Item item = null;

            // Find our first ident 
            int selectedIndex = (int)formFuncs.ListItems[references.SelectedIndices[0]].Tag;
            item = map.SelectedMeta.items[selectedIndex];

            Meta.Ident id = (Meta.Ident)item;
            int iOffx = map.MetaInfo.Offset[map.SelectedMeta.TagIndex] + id.offset;
            idTags.Add(id.pointstoTagIndex);
            idOffs.Add(iOffx);
            IdentSwapper iSwap = new IdentSwapper();
            iSwap.LoadStuff(idTags, idOffs, map);
            if (iSwap.ShowDialog() != DialogResult.OK)
            {
                iSwap.Dispose();
                return;
            }

            string tType = ((identSwapLayout)iSwap.Controls[1]).tagtype.Text;
            string tName = ((identSwapLayout)iSwap.Controls[1]).tagname.Text;
            iSwap.Dispose();

            map.OpenMap(MapTypes.Internal);

            // Start at position #1 as we just did #0
            for (int x = 1; x < references.SelectedIndices.Count; x++)
            {
                selectedIndex = (int)formFuncs.ListItems[references.SelectedIndices[x]].Tag;
                item = map.SelectedMeta.items[selectedIndex];

                map.BW.BaseStream.Position = map.MetaInfo.Offset[map.SelectedMeta.TagIndex] + item.offset;
                int temp = map.Functions.ForMeta.FindByNameAndTagType(tType, tName);
                if (temp != -1)
                {
                    map.BW.Write(map.MetaInfo.Ident[temp]);
                }
                else
                {
                    map.BW.Write(temp);
                }
            }

            map.CloseMap();

            LoadMeta(map.SelectedMeta.TagIndex);
            sortReferences(-1, false);
        }

        /// <summary>
        /// The get child node names.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <param name="sb">The sb.</param>
        /// <param name="tn">The tn.</param>
        /// <remarks></remarks>
        private void GetChildNodeNames(int indent, ref StringBuilder sb, TreeNode tn)
        {
            while (tn != null)
            {
                sb.AppendLine(string.Empty.PadLeft(indent) + tn.Text);
                if (tn.Nodes.Count > 0)
                {
                    GetChildNodeNames(indent + 2, ref sb, tn.Nodes[0]);
                }

                tn = tn.NextNode;
            }
        }

        /// <summary>
        /// Jumps to the hilighted tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JumpToTagItem_Click(object sender, EventArgs e)
        {
            if (references.FocusedItem == null)
                return;
            // gets the tag number from the 2nd & 3rd column from the selected item in the References ListView
            int tagNum = map.Functions.ForMeta.FindByNameAndTagType( references.FocusedItem.SubItems[2].Text, 
                                                                     references.FocusedItem.SubItems[3].Text);
            selectTag(tagNum);
        }

        /// <summary>
        /// load strings in another thread with this method,
        /// as loading strings can be sloow
        /// </summary>
        /// <param name="names">The names.</param>
        /// <remarks></remarks>
        private void LoadStringsBackgroundWorker(object names)
        {
            if (!this.Disposing && !this.IsDisposed)
                try
                {
                    sSwap = new MEStringsSelector((string[])names, this);
                }
                catch
                {
                }
        }

        /// <summary>
        /// The map form_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MapForm_TextChanged(object sender, EventArgs e)
        {
            string[] tagInfo = this.Text.Split(' ');
            if (tagInfo[0].Length == 6)
            {
                string tagType = tagInfo[0].Substring(1, 4);
                int id = map.Functions.ForMeta.FindByNameAndTagType(tagType, tagInfo[1]);
                selectTag(id);
            }
        }

        /// <summary>
        /// Show Idents click
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ShowIdent_Click(object sender, EventArgs e)
        {
            map.DisplayType = Meta.ItemType.Ident;
            formFuncs.AddColumnsToListView(references, map.DisplayType);
            formFuncs.AddReferencesToListView(map.SelectedMeta, references, map.DisplayType);
        }

        /// <summary>
        /// Show Reflexives click
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ShowReflex_Click(object sender, EventArgs e)
        {
            map.DisplayType = Meta.ItemType.Reflexive;
            formFuncs.AddColumnsToListView(references, map.DisplayType);
            formFuncs.AddReferencesToListView(map.SelectedMeta, references, map.DisplayType);
        }

        /// <summary>
        /// The show string_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ShowString_Click(object sender, EventArgs e)
        {
            map.DisplayType = Meta.ItemType.String;
            formFuncs.AddColumnsToListView(references, map.DisplayType);
            formFuncs.AddReferencesToListView(map.SelectedMeta, references, map.DisplayType);
        }

        /// <summary>
        /// Sorts the references by column in ascending/descending order
        /// </summary>
        /// <param name="column">The column to sort by, -1 for previous sort method</param>
        /// <param name="reverse">FALSE to sort ascending, TRUE to sort descending. This value is ignored if column is set to -1</param>
        public void sortReferences(int column, bool reverse)
        {
            // If set to -1, use last sort method
            if (column == -1)
                column = sortColumn;

            // If no sort has been selected, return
            if (sortColumn == -1)
                return;

            // Keep track of all selected values
            ListView.SelectedIndexCollection selections = references.SelectedIndices;

            for (int i = 0; i < references.Items.Count; i++)
                for (int ii = i + 1; ii < references.Items.Count; ii++)
                {
                    int maxCount = references.Items.Count;
                    bool ordered = true;

                    int c1, c2;
                    if (int.TryParse(references.Items[i].SubItems[column].Text, out c1) &&
                        int.TryParse(references.Items[ii].SubItems[column].Text, out c2))
                    {
                        if (!sortReverse)
                        {
                            if (c1 > c2)
                                ordered = false;
                        }
                        else
                        {
                            if (c1 < c2)
                                ordered = false;
                        }
                    }
                    else
                    {
                        int strComp = string.Compare(references.Items[i].SubItems[column].Text, references.Items[ii].SubItems[column].Text);
                        if ((!sortReverse && strComp > 0) || (sortReverse && strComp < 0))
                            ordered = false;
                    }


                    if (!ordered)
                    {
                        ListViewItem lvi = formFuncs.ListItems[i];
                        formFuncs.ListItems[i] = formFuncs.ListItems[ii];
                        formFuncs.ListItems[ii] = lvi;

                        /*
                        // Keep any selected values
                        for (int iii = 0; iii < selections.Count; iii++)
                        {
                            if (selections[iii] == i)
                            {
                                formFuncs.ListItems[i].Selected = false;
                                formFuncs.ListItems[ii].Selected = true;
                            }
                        }
                        */
                    }
                }
            references.Invalidate();
        }

        /// <summary>
        /// The swap item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SwapItem_Click(object sender, EventArgs e)
        {
            if (parsedCheckBox.Checked)
            {
                MessageBox.Show("Turn off parsing first");
                return;
            }

            if (references.SelectedIndices.Count == 0)
            {
                return;
            }

            List<int> idTags = new List<int>();
            List<int> idOffs = new List<int>();

            for (int x = 0; x < references.SelectedIndices.Count; x++)
            {
                int d = references.SelectedIndices[x];
                int tempcount = 0;

                // Tag contains the position value of the item in SelectedMeta.Items
                int selectedIndex = (int)formFuncs.ListItems[d].Tag;

                Meta.Item item = map.SelectedMeta.items[selectedIndex];

                if (item.type == map.DisplayType)
                {
                    switch (item.type)
                    {
                        case Meta.ItemType.Ident:
                            Meta.Ident id = (Meta.Ident)item;
                            IdentSwapper iSwap = new IdentSwapper();
                            int iOffx = map.MetaInfo.Offset[map.SelectedMeta.TagIndex] + id.offset;
                            idTags.Add(id.pointstoTagIndex);
                            idOffs.Add(iOffx);
                            if (x == references.SelectedIndices.Count - 1)
                            {
                                iSwap.LoadStuff(idTags, idOffs, map);

                                iSwap.ShowDialog();
                                iSwap.Dispose();
                                x = references.SelectedIndices.Count;
                            }

                            break;
                        case Meta.ItemType.String:
                            Meta.String str = (Meta.String)item;
                            if (sSwap == null)
                            {
                                sSwap = new MEStringsSelector(map.Strings.Name, this);
                            }

                            sSwap.SelectedID = str.id;
                            sSwap.ShowDialog();

                            // this.Enabled = true;

                            if (str.id != sSwap.SelectedID)
                            {
                                str.id = sSwap.SelectedID;
                                str.name = map.Strings.Name[sSwap.SelectedID];

                                // int sOffx = Map.MetaInfo.Offset[Map.SelectedMeta.TagIndex] + str.offset;
                                int sOffx = str.mapOffset;
                                map.OpenMap(MapTypes.Internal);
                                map.BW.BaseStream.Position = sOffx;
                                map.BW.Write((UInt16)str.id);
                                map.BR.ReadByte();
                                map.BW.Write((Byte)map.Strings.Length[sSwap.SelectedID]);
                                map.CloseMap();
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            map.CloseMap();
            LoadMeta(map.SelectedMeta.TagIndex);
            sortReferences(-1, false);
        }

        /// <summary>
        /// The add to quick list.
        /// </summary>
        /// <param name="tagType">The tag type.</param>
        /// <param name="tagName">The tag name.</param>
        /// <remarks></remarks>
        private void addToQuickList(string tagType, string tagName)
        {
            // If we are using the registry, update it immediately
            if (Prefs.useRegistryEntries)
            {
                RegistryAccess.setValue(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + tagType + @"\", tagName, true);
            }

            bool isNewTagType = false;
            bool isNewTagPath = false;

            Prefs.QuickAccessTagType quickAccess = Prefs.GetQuickAccessTagType(tagType);
            if (quickAccess == null)
            {
                quickAccess = new Prefs.QuickAccessTagType();
                quickAccess.TagType = tagType;
                Prefs.QuickAccessTagTypes.Add(quickAccess);
                isNewTagType = true;
            }
            if (!quickAccess.TagPaths.Contains(tagName))
            {
                quickAccess.TagPaths.Add(tagName);
                isNewTagPath = true;
            }

            // already in the quick list
            if (!isNewTagType && !isNewTagPath) return;

            if (isNewTagType)
            {
                this.treeView1.Nodes.Add(tagType, tagType);
                this.treeView1.Nodes[this.treeView1.Nodes.IndexOfKey(tagType)].Nodes.Add(tagName);
            }
            else
            {
                int tagNum = this.treeView1.Nodes.IndexOfKey(tagType);
                bool found = false;
                for (int i = 0; i < this.treeView1.Nodes[tagNum].Nodes.Count; i++)
                {
                    if (this.treeView1.Nodes[tagNum].Nodes[i].Text == tagName)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    this.treeView1.Nodes[tagNum].Nodes.Add(tagName);
                }
            }
        }

        /// <summary>
        /// The analyze map button_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void analyzeMapButton_Click_1(object sender, EventArgs e)
        {
            saveMetaDialog.FileName = "Layout.xml";
            if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            DateTime dt = DateTime.Now;
            MapAnalyzer analyze = new MapAnalyzer();
            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);
            layout.SaveToXml(saveMetaDialog.FileName, map);
            layout = null;
            analyze = null;
            GC.Collect();
            DateTime dt2 = DateTime.Now;
            TimeSpan dt3 = dt2.Subtract(dt);
            
            MessageBox.Show(dt3.ToString());
        }

        /// <summary>
        /// The bitmap editor tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void bitmapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ltmpTools.Visible = true;
            ltmpTools.BringToFront();
            Global.ClearControls(ltmpTools);
            
            BitmapControl bc = new BitmapControl(map);
            bc.Dock = DockStyle.Fill;
            ltmpTools.Controls.Add(bc);
        }

        /// <summary>
        /// The bspcollision viewer tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void bspcollisionViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BSPModel.BSPCollision coll = new BSPModel.BSPCollision(map.SelectedMeta);
            BSPCollisionViewer bv = new BSPCollisionViewer(coll, map);
        }

        /// <summary>
        /// The build button_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buildButton_Click_1(object sender, EventArgs e)
        {
            if (openInfoFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            this.Enabled = false;
            this.Focus();

            MapAnalyzer analyze = new MapAnalyzer();

            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);
            Builder build = new Builder();
            build.BuildMapFromInfoFile(openInfoFileDialog.FileName, ref layout, map, soundsCheckBox.Checked);

            map = Map.Refresh(map);
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            this.Enabled = true;
            MessageBox.Show("Done");
        }

        /*
        /// <summary>
        /// The butt connect_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttConnect_Click(object sender, EventArgs e)
        {
                if (!RTHInfo.Connected)
                {
                    if (RTH_Imports.InitRTH(XboxIP.Text) < 0)
                    {
                        MessageBox.Show("Couldn't Connect to xbox at: " + XboxIP.Text);
                        RTHInfo.XBoxIPAddy = "0.0.0.0";
                        RTHInfo.Connected = false;
                    }
                    else
                    {
                        RTH_Imports.loadDebugMap(map);
                        MessageBox.Show("Connected to Xbox at: " + XboxIP.Text);
                        RTHInfo.XBoxIPAddy = XboxIP.Text;
                        RTHInfo.Connected = true;
                    }
                }
                else
                {
                    MessageBox.Show("Already connected at: " + RTHInfo.XBoxIPAddy);
                }
        }
         
        /// <summary>
        /// The butt disconnect_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttDisconnect_Click(object sender, EventArgs e)
        {
            if (RTHInfo.Connected)
            {
                RTH_Imports.DeInitRTH();
                RTHInfo.Connected = false;
                RTHInfo.XBoxIPAddy = "0.0.0.0";
            }
        }
        */

        /// <summary>
        /// The button internalize_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttonInternalize_Click(object sender, EventArgs e)
        {
            ParsedBitmap.bitmapInternalize(map.SelectedMeta);
            LoadMeta(map.SelectedMeta.TagIndex);
        }

        /// <summary>
        /// The check if ps button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkIFPsButton_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);
            this.Cursor = Cursors.WaitCursor;
            MetaItemComparer mi = new MetaItemComparer(this);
            this.Cursor = Cursors.Arrow;
            map.CloseMap();
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The chunkclone_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void chunkclone_Click(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            map.OpenMap(MapTypes.Internal);

            // Creates the chunk cloner window
            ChunkClonerWindow cw = new ChunkClonerWindow(map.SelectedMeta.TagIndex, map);

            cw.ShowDialog();
            if (cw.result)
            {
                int i = map.SelectedMeta.TagIndex;
                Meta.ItemType me = map.DisplayType;
                map = Map.Refresh(map);
                LoadMeta(i);

                // Done when in Relexive Display mode
                formFuncs.AddReferencesToListView(map.SelectedMeta, references, me);
                MessageBox.Show("Done");
            }

            map.CloseMap();
        }

        /// <summary>
        /// The clear tag quick list tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void clearTagQuickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    "Are you sure you want to clear quick list?", "Clear Tag Quick List?", MessageBoxButtons.YesNo) ==
                DialogResult.No)
            {
                return;
            }

            // If using registry, remove all tag entries
            if (Prefs.useRegistryEntries)
            {
                RegistryAccess.removeKey(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags");
            }

            Prefs.QuickAccessTagTypes.Clear();

            while (treeView1.Nodes.Count > 1)
            {
                treeView1.Nodes.RemoveAt(1);
            }
        }

        /// <summary>
        /// The close open custom plugin optionsbutton 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void closeOpenCustomPluginOptionsbutton4_Click(object sender, EventArgs e)
        {
            // LowerOptionsBar.Tag = !(bool)LowerOptionsBar.Tag;
            if (splitContainer4.SplitterDistance < splitContainer4.Size.Height - 30)
            {
                splitContainer4.SplitterDistance = splitContainer4.Size.Height - 27;
                buttonLowerOptions.Cursor = Cursors.PanNorth;
                buttonLowerOptions.Text = "Open Options";
            }
            else
            {
                splitContainer4.SplitterDistance = splitContainer4.Size.Height -
                                                   (LowerOptionsBar.PreferredSize.Height + 30);
                buttonLowerOptions.Cursor = Cursors.PanSouth;
                buttonLowerOptions.Text = "Close Options";
            }
        }

        /// <summary>
        /// The collison viewer tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void collisonViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadMeta(map.SelectedMeta.TagIndex);
            coll coll = new coll(ref map.SelectedMeta);
            CollisionViewer cv = new CollisionViewer(coll, map);
        }

        /// <summary>
        /// The convert ce to h 2 tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void convertCEToH2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] split = map.SelectedMeta.name.Split('\\');
            string temp = split[split.Length - 1] + "." + map.SelectedMeta.type;
            temp = temp.Replace('<', '_');
            temp = temp.Replace('>', '_');
            saveMetaDialog.FileName = temp;
            if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            switch (map.SelectedMeta.type)
            {
                case "bitm":
                    ParsedBitmap pb = new ParsedBitmap(ref map.SelectedMeta, map);
                    pb.ConvertCEtoH2ParsedBitmap(ref map.SelectedMeta, map);
                    break;
            }

            map.SelectedMeta.SaveMetaToFile(saveMetaDialog.FileName, true);
        }

        // Used for Injection Selection box

        /// <summary>
        /// The convert to bump map tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void convertToBumpMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveBitmapDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Panel p = new Panel();
            Renderer r = new Renderer();
            r.CreateDevice(p);
            map.OpenMap(MapTypes.Internal);
            ParsedBitmap pm = new ParsedBitmap(ref map.SelectedMeta, map);
            
            Bitmap b = pm.FindChunkAndDecode(0, 0, 0, ref map.SelectedMeta, map, 0, 0);
            ShaderInfo s = new ShaderInfo();
            s.BumpMapBitmap = b;
            map.CloseMap();
            s.MakeTextures(ref r.device);
            TextureLoader.Save(saveBitmapDialog1.FileName, ImageFileFormat.Dds, s.NormalMap);
        }

        /// <summary>
        /// The custom p lugin edit_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void customPLuginEdit_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == comboBox1.Items[0])
            {
                MessageBox.Show("The Complete Plugin Set cannot be edited.");
                return;
            }

            CustomPluginEditor cpe = new CustomPluginEditor(map);
            cpe.Owner = this;
            cpe.ShowDialog();
            string tempS = (string)cpe.comboBoxPluginName.SelectedItem;
            cpe.Dispose();

            // Remove all but the Complete Listing
            while (comboBox1.Items.Count > 1)
            {
                comboBox1.Items.RemoveAt(1);
            }

            foreach (Prefs.CustomPluginMask pluginMask in Prefs.CustomPluginMasks)
            {
                comboBox1.Items.Add(pluginMask.Name);
            }

            comboBox1.SelectedItem = tempS;
        }

        /// <summary>
        /// The custom p lugin_ new_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void customPLugin_New_Click(object sender, EventArgs e)
        {
            string name = GetNameDialog.Show(
                "Choose new plugin name", "Input a name for the new plugin:", string.Empty, "&Set Name");
            if (name == null || name == string.Empty)
            {
                return;
            }

            if (!this.comboBox1.Items.Contains(name))
            {
                Prefs.CustomPluginMask mask = new Prefs.CustomPluginMask();
                mask.Name = name;
                Prefs.CustomPluginMasks.Add(mask);

                this.comboBox1.SelectedItem = comboBox1.Items[this.comboBox1.Items.Add(name)];
            }

            CustomPluginEditor cpe = new CustomPluginEditor(map);
            cpe.Owner = this;
            cpe.ShowDialog();
        }

        /// <summary>
        /// The custom p lugins_combo box 1_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void customPLugins_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTreeView();
        }

        /// <summary>
        /// The duplicate recursively tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void duplicateRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            this.Enabled = false;
            this.Focus();

            MapAnalyzer analyze = new MapAnalyzer();

            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);
            Builder build = new Builder();
            ArrayList metas = new ArrayList();
            metas = scanandpassmetasfordraganddrop();
            for (int x = 0; x < metas.Count; x++)
            {
                string[] temps = ((Meta)metas[x]).name.Split('(', ')', ' ');
                string newname = temps[0];
                int tempcount = 1;
                do
                {
                    bool done = false;
                    newname = temps[0] + " (" + tempcount + ")";
                    int index = Array.IndexOf(map.FileNames.Name, newname);
                    if (index == -1)
                    {
                        break;
                    }

                    do
                    {
                        if (map.MetaInfo.TagType[index] == ((Meta)metas[x]).type)
                        {
                            tempcount += 1;
                            break;
                        }

                        index = Array.IndexOf(map.FileNames.Name, newname, index + 1);
                        if (index == -1)
                        {
                            done = true;
                            break;
                        }
                    }
                    while (tempcount != -1);
                    if (done)
                    {
                        break;
                    }
                }
                while (tempcount != -1);
                ((Meta)metas[x]).name = newname;
            }

            string temp = ((Meta)metas[0]).name;
            addToQuickList(map.SelectedMeta.type, temp);
            build.MapBuilder(metas, ref layout, map, soundsCheckBox.Checked);
            map = Map.Refresh(map);
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            setNodePath(treeView1, temp);
            this.Enabled = true;

            // MessageBox.Show("Done");
        }

        /// <summary>
        /// The duplicate tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            string nodePath = treeView1.SelectedNode.FullPath;
            this.Enabled = false;
            this.Focus();

            MapAnalyzer analyze = new MapAnalyzer();

            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);
            Builder build = new Builder();
            ArrayList metas = new ArrayList();

            string[] temps = map.SelectedMeta.name.Split('(', ')', ' ');
            newName = temps[0];
            int tempcount = 1;
            do
            {
                bool done = false;
                newName = temps[0] + " (" + tempcount + ")";
                int index = Array.IndexOf(map.FileNames.Name, newName);
                if (index == -1)
                {
                    break;
                }

                do
                {
                    if (map.MetaInfo.TagType[index] == map.SelectedMeta.type)
                    {
                        tempcount += 1;
                        break;
                    }

                    index = Array.IndexOf(map.FileNames.Name, newName, index + 1);
                    if (index == -1)
                    {
                        done = true;
                        break;
                    }
                }
                while (tempcount != -1);
                if (done)
                {
                    break;
                }
            }
            while (tempcount != -1);

            // Create a form for renaming a new tag
            AskForTagName();

            if (newName != string.Empty)
            {
                addToQuickList(map.SelectedMeta.type, newName);
                map.SelectedMeta.name = newName;
                metas.Add(map.SelectedMeta);
                build.MapBuilder(metas, ref layout, map, soundsCheckBox.Checked);
                
                // Leaves map locked!
                map = Map.Refresh(map);
                //

                formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);

                setNodePath(treeView1, newName);

                // MessageBox.Show("Done");
            }

            this.Enabled = true;
        }

        /// <summary>
        /// The expand mesh x 3 tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void expandMeshX3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BSPModel.BSPCollision coll = new BSPModel.BSPCollision(map.SelectedMeta);
            map.OpenMap(MapTypes.Internal);

            for (int x = 0; x < coll.Vertices.Length; x++)
            {
                coll.Vertices[x].X *= 3;
                coll.Vertices[x].Y *= 3;
                coll.Vertices[x].Z *= 3;
                map.BW.BaseStream.Position = map.SelectedMeta.offset + coll.VerticeReflexiveTranslation + (x * 16);

                // info.BW.Write(coll.Vertices[x].X);
                // info.BW.Write(coll.Vertices[x].Y);
                // info.BW.Write(coll.Vertices[x].Z);
            }

            for (int x = 0; x < coll.Planes.Length; x++)
            {
                coll.Planes[x].W *= 3;
                map.BW.BaseStream.Position = map.SelectedMeta.offset + coll.PlaneReflexiveTranslation + (x * 16) + 12;

                map.BW.Write(coll.Planes[x].W);
            }

            LoadMeta(map.SelectedMeta.TagIndex);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The export collison to obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void exportCollisonToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            BSPModel.BSPCollision pm = new BSPModel.BSPCollision(map.SelectedMeta);

            // string[] temps = info.SelectedMeta.name.Split('\\');
            // pm.Name = temps[temps.Length - 1];
            pm.ExtractCollisonMesh(saveMetaDialog.FileName);

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The export mesh to obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void exportMeshToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveMetaDialog.InitialDirectory = Prefs.pathExtractsFolder;
            saveMetaDialog.DefaultExt = "obj";
            saveMetaDialog.FileName = map.SelectedMeta.name.Substring(map.SelectedMeta.name.LastIndexOf('\\') + 1);
            saveMetaDialog.Filter = "Object file (*.obj)|*.obj";
            if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Prefs.pathExtractsFolder = saveMetaDialog.FileName.Substring(
                0, saveMetaDialog.FileName.LastIndexOf('\\'));
            
            Form tempForm = new Form();
            tempForm.ControlBox = false;
            tempForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            tempForm.Size = new Size(250, 100);
            Label tempLabel = new Label();
            tempLabel.AutoSize = true;
            tempLabel.Location = new Point(20, 20);
            tempLabel.Text = "Loading BSP, please wait...";
            ProgressBar tempPB = new ProgressBar();
            tempPB.Location = new Point(20, 60);
            tempPB.Size = new Size(210, 20);
            tempPB.Minimum = 0;
            tempPB.Maximum = 100;
            tempForm.Controls.Add(tempLabel);
            tempForm.Controls.Add(tempPB);
            tempForm.Show();
            Application.DoEvents();

            

            BSPModel pm = new BSPModel(ref map.SelectedMeta);
            string[] temps = saveMetaDialog.FileName.Split('\\');
            string[] temps2 = temps[temps.Length - 1].Split('.');
            pm.Name = temps2[0];

            // pm.LoadModelStructure(ref Map.SelectedMeta, map);
            // pm.LoadPermutations(ref Map.SelectedMeta, map);
            // pm.LoadUnknowns(ref Map.SelectedMeta, map);
            // pm.Shaders = new Entity.BSP.BSPModel.BSPShaderContainer( pm, ref Map.SelectedMeta, map);
            tempLabel.Text = "Extracting BSP as 2 files...";
            tempPB.Value = 80;
            Application.DoEvents();
            pm.ExtractModelAsSingleMesh(saveMetaDialog.FileName);
            pm.Dispose();

            #region Remove form that shows progress

            tempForm.Dispose();

            #endregion

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The export mesh tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void exportMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            ParsedModel pm = new ParsedModel(ref map.SelectedMeta);
            pm.ExtractMesh(folderBrowserDialog.SelectedPath);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The export obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            BSPModel pm = new BSPModel(ref map.SelectedMeta);
            string[] temps = map.SelectedMeta.name.Split('\\');
            pm.Name = temps[temps.Length - 1];

            pm.ExtractModel(folderBrowserDialog.SelectedPath);

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The export scripts tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void exportScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Scripts s = new Scripts(map);
            s.WriteScriptInfotoText("C:\\scripts.txt");
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The extract mesh to obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void extractMeshToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            coll coll = new coll(ref map.SelectedMeta);
            coll.ExtractMeshes(saveMetaDialog.FileName);
        }

        /// <summary>
        /// The extract prtmobj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void extractPRTMOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            PRTMModel pm = new PRTMModel(ref map.SelectedMeta);

            pm.ExtractMeshesToOBJ(folderBrowserDialog.SelectedPath);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The fix system link tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void fixSystemLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            List<int> ids = new List<int>();
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                switch (map.MetaInfo.TagType[x])
                {
                    case "bipd":
                    case "bloc":
                    case "ctrl":

                    case "jpt!":
                    case "mach":

                    case "scen":
                    case "ssce":
                    case "vehi":
                        ids.Add(map.MetaInfo.Ident[x]);
                        count++;
                        break;
                    case "eqip":
                    case "garb":
                    case "proj":
                        ids.Add(map.MetaInfo.Ident[x]);
                        ids.Add(map.MetaInfo.Ident[x]);
                        count += 2;
                        break;
                    case "weap":
                        ids.Add(map.MetaInfo.Ident[x]);
                        ids.Add(map.MetaInfo.Ident[x]);
                        ids.Add(map.MetaInfo.Ident[x]);
                        count += 3;
                        break;
                }
            }

            map.OpenMap(MapTypes.Internal);
            Meta m = new Meta(map);
            m.ReadMetaFromMap(3, true);

            IFPIO io = IFPHashMap.GetIfp("scnr", map.HaloVersion);
            m.headersize = io.headerSize;
            m.scanner.ScanWithIFP(ref io);

            MetaSplitter metasplit = new MetaSplitter();
            metasplit.SplitWithIFP(ref io, ref m, map);

            for (int x = 0; x < metasplit.Header.Chunks[0].ChunkResources.Count; x++)
            {
                // Offset 984 = [SCNR] Predicted Resources
                if (metasplit.Header.Chunks[0].ChunkResources[x].offset == 984)
                {
                    MetaSplitter.SplitReflexive reflex =
                        (MetaSplitter.SplitReflexive)metasplit.Header.Chunks[0].ChunkResources[x];

                    // count = # of chunks incl. added/removed
                    // reflex.Chunks.Count = # of chunks listed in Predicted Resources (?)
                    int diff = count - reflex.Chunks.Count;

                    // Add/Remove chunks to match the difference
                    for (int y = 0; y < diff; y++)
                    {
                        MetaSplitter.SplitReflexive MetaChunk = new MetaSplitter.SplitReflexive();
                        MetaChunk.splitReflexiveType = MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk;
                        MetaChunk.chunksize = 4;

                        MetaChunk.MS = new MemoryStream(4);
                        reflex.Chunks.Add(MetaChunk);
                    }

                    for (int y = 0; y < reflex.Chunks.Count; y++)
                    {
                        BinaryWriter BW = new BinaryWriter(reflex.Chunks[y].MS);
                        BW.Write(ids[y]);
                    }

                    metasplit.Header.Chunks[0].ChunkResources[x] = reflex;
                    break;
                }
            }

            Meta newmeta = MetaBuilder.BuildMeta(metasplit, map);

            map.OpenMap(MapTypes.Internal);

            map.ChunkTools.Add(newmeta);
            map.CloseMap();

            // info.OpenMap(MapTypes.Internal);
            // info.BW.BaseStream.Position = m.offset + r.translation;
            // for (int x = 0; x < count; x++)
            // {
            // info.BW.Write(ids[x]);
            // }
            // info.CloseMap();
            map = Map.Refresh(map);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The folder hierarchy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void folderHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tagTypeToolStripMenuItem.Checked = false;
            folderHierarchyToolStripMenuItem.Checked = true;
            toolStripTagView.Checked = false;
            toolStripFolderView.Checked = true;
            toolStripInfoView.Checked = false;
            metaView = FormFunctions.MetaView.FolderView;

            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
        }

        /// <summary>
        /// The hex editor tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void hexEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setEditorMode(EditorModes.HexViewer);

        }

        /// <summary>
        /// The info hierarchy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void infoHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Used for Sorted/Not Sorted
            if ((folderHierarchyToolStripMenuItem.Checked == false) && (toolStripTagView.Checked == false) &&
                (toolStripFolderView.Checked == false))
            {
                infoSelected = !infoSelected;
            }

            tagTypeToolStripMenuItem.Checked = false;
            folderHierarchyToolStripMenuItem.Checked = false;
            toolStripTagView.Checked = false;
            toolStripFolderView.Checked = false;
            toolStripInfoView.Checked = true;
            metaView = FormFunctions.MetaView.InfoView;

            formFuncs.AddMetasToTreeView(map, treeView1, metaView, infoSelected);
        }

        /// <summary>
        /// The inject bsp visual mesh tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void injectBSPVisualMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Prefs.pathExtractsFolder;
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Prefs.pathExtractsFolder = folderBrowserDialog.SelectedPath;
            
            Form tempForm = new Form();
            tempForm.ControlBox = false;
            tempForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            tempForm.Size = new Size(250, 100);
            Label tempLabel = new Label();
            tempLabel.AutoSize = true;
            tempLabel.Location = new Point(20, 20);
            tempLabel.Text = "Loading BSP, please wait...";
            ProgressBar tempPB = new ProgressBar();
            tempPB.Location = new Point(20, 60);
            tempPB.Size = new Size(210, 20);
            tempPB.Minimum = 0;
            tempPB.Maximum = 100;
            tempForm.Controls.Add(tempLabel);
            tempForm.Controls.Add(tempPB);
            tempForm.Show();
            Application.DoEvents();

            BSPModel pm = new BSPModel(ref map.SelectedMeta);
            tempLabel.Text = "Loading Model...";
            tempPB.Value = 35;
            Application.DoEvents();
            Meta addme = pm.InjectModel(folderBrowserDialog.SelectedPath, map.SelectedMeta);

            /*
            addme.name += "(new)";
            addme.name = getNameDialog.Show("Choose injection name", "Meta Name:", addme.name, "OK");
            */
            ArrayList oi = new ArrayList();
            oi.Add(addme);

            tempLabel.Text = "Analyzing Layout...";
            tempPB.Value = 60;
            Application.DoEvents();

            MapAnalyzer analyze = new MapAnalyzer();
            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);

            tempLabel.Text = "Rebuilding Map...";
            tempPB.Value = 70;
            Application.DoEvents();

            Builder build = new Builder();
            build.MapBuilder(oi, ref layout, map, false);

            tempLabel.Text = "Refreshing Map...";
            tempPB.Value = 95;
            Application.DoEvents();
            map = Map.Refresh(map);
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            this.Enabled = true;

            #region Remove form that shows progress

            tempForm.Dispose();

            #endregion

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The inject bitmap tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void injectBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int numDDS = 1;
            for (int i = 0; i < map.SelectedMeta.items.Count; i++)
            {
                if (map.SelectedMeta.items[i].offset == 68)
                {
                    numDDS = ((MetaScanner.XReflex)map.SelectedMeta.items[i]).chunkcount;
                    break;
                }
            }

            bool injectAll = false;
            string pathName = string.Empty;

            if (map.SelectedMeta.raw.rawChunks[0].rawLocation != MapTypes.Internal)
            {
                DialogResult dr =
                    MessageBox.Show(
                        "This bitmap is located in the \"" +
                        map.SelectedMeta.raw.rawChunks[0].rawLocation.ToString().ToLower() +
                        ".map\" file.\n\nSelect 'YES' to inject to " +
                        map.SelectedMeta.raw.rawChunks[0].rawLocation.ToString().ToLower() +
                        ".map \nSelect 'NO' to internalize it first", 
                        "Confirm Injection to External Map", 
                        MessageBoxButtons.YesNoCancel, 
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Cancel)
                {
                    return;
                }
                else if (dr == DialogResult.No)
                {
                    buttonInternalize_Click(null, null);
                }
            }

            if (numDDS > 1)
            {
                DialogResult result =
                    MessageBox.Show(
                        "Current Bitmap contains " + numDDS + " bitmap chunks.\n\nDo you wish to inject all bitmaps?", 
                        "Inject All Bitmaps?", 
                        MessageBoxButtons.YesNo);
                injectAll = result == DialogResult.Yes;
            }

            if (injectAll)
            {
                openBitmapDialog1.Title = "Select a file from the set to Inject";
            }
            else
            {
                openBitmapDialog1.Title = "Select file to Inject";
            }

            openBitmapDialog1.InitialDirectory = Prefs.pathBitmapsFolder;
            openBitmapDialog1.FileName = map.SelectedMeta.name.Substring(map.SelectedMeta.name.LastIndexOf('\\')+1) + ".dds";
            if (openBitmapDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            Prefs.pathBitmapsFolder = openBitmapDialog1.FileName.Substring(
                0, openBitmapDialog1.FileName.LastIndexOf('\\'));

            switch (openBitmapDialog1.FilterIndex)
            {
                case 1:
                    ParsedBitmap bm = new ParsedBitmap(ref map.SelectedMeta, map);
                    FileStream fs;

                    if (injectAll)
                    {
                        string[] s = new string[3];

                        // s[0] = path
                        s[0] = openBitmapDialog1.FileName.Substring(0, openBitmapDialog1.FileName.LastIndexOf('\\') + 1);

                        // s[1] = filename (without numbering, eg "ASDF_001" => "ASDF")
                        s[1] = openBitmapDialog1.FileName.Substring(
                            s[0].Length, openBitmapDialog1.FileName.LastIndexOf('.') - s[0].Length);
                        int Length = s[1].Length - s[1].LastIndexOf('_');
                        s[1] = s[1].Remove(s[1].Length - Length);

                        // s[2] = extension
                        s[2] = openBitmapDialog1.FileName.Substring(openBitmapDialog1.FileName.LastIndexOf('.'));

                        for (int count = 0; count < numDDS; count++)
                        {
                            fs = new FileStream(
                                s[0] + s[1] + "_" + count.ToString().PadLeft(Length - 1, '0') + s[2], FileMode.Open);
                            BinaryReader br = new BinaryReader(fs);
                            DDS.InjectDDS(map.SelectedMeta, bm, ref br, count);
                            br.Close();
                            fs.Close();
                        }
                    }
                    else
                    {
                        BitmapInjectionForm bif = new BitmapInjectionForm(map, openBitmapDialog1.FileName, bm, numDDS);
                        bif.ShowDialog();
                        bif.Dispose();

                    }

                    break;
            }

            // Refresh bitmap
            for (int i = 0; i < ltmpTools.Controls.Count; i++)
            {
                if (ltmpTools.Controls[i] is BitmapControl)
                {
                    LoadMeta(map.SelectedMeta.TagIndex);
                    BitmapControl bc = (BitmapControl)ltmpTools.Controls[i];
                    bc.RefreshDisplay();
                    break;
                }
            }
        }

        /// <summary>
        /// The inject collision mesh obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void injectCollisionMeshOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadMetaDialog.Filter = "Wavefront Object Files (*.obj) | *.obj";
            if (loadMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            BSPModel.BSPCollision coll = new BSPModel.BSPCollision(map.SelectedMeta);
            coll.InjectCollisonMesh(loadMetaDialog.FileName, map.SelectedMeta);
            LoadMeta(map.SelectedMeta.TagIndex);
        }

        /// <summary>
        /// The inject mesh from obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void injectMeshFromOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Prefs.pathExtractsFolder;
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Prefs.pathExtractsFolder = folderBrowserDialog.SelectedPath;
            
            Form tempForm = new Form();
            tempForm.ControlBox = false;
            tempForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            tempForm.Size = new Size(250, 100);
            Label tempLabel = new Label();
            tempLabel.AutoSize = true;
            tempLabel.Location = new Point(20, 20);
            tempLabel.Text = "Loading BSP, please wait...";
            ProgressBar tempPB = new ProgressBar();
            tempPB.Location = new Point(20, 60);
            tempPB.Size = new Size(210, 20);
            tempPB.Minimum = 0;
            tempPB.Maximum = 100;
            tempForm.Controls.Add(tempLabel);
            tempForm.Controls.Add(tempPB);
            tempForm.Show();
            Application.DoEvents();

            BSPModel pm = new BSPModel(ref map.SelectedMeta);
            tempLabel.Text = "Loading Model...";
            tempPB.Value = 35;
            Application.DoEvents();
            Meta addme = pm.InjectModel(folderBrowserDialog.SelectedPath, map.SelectedMeta);

            /*
            addme.name += "(new)";
            addme.name = getNameDialog.Show("Choose injection name", "Meta Name:", addme.name, "OK");
            */
            ArrayList oi = new ArrayList();
            oi.Add(addme);

            tempLabel.Text = "Analyzing Layout...";
            tempPB.Value = 60;
            Application.DoEvents();

            MapAnalyzer analyze = new MapAnalyzer();
            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);

            tempLabel.Text = "Rebuilding Map...";
            tempPB.Value = 70;
            Application.DoEvents();

            Builder build = new Builder();
            build.MapBuilder(oi, ref layout, map, false);

            tempLabel.Text = "Refreshing Map...";
            tempPB.Value = 95;
            Application.DoEvents();
            map = Map.Refresh(map);
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            this.Enabled = true;

            #region Remove form that shows progress

            tempForm.Dispose();

            #endregion

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The inject obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void injectOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            ParsedModel pm = new ParsedModel(ref map.SelectedMeta);
            Meta addme = pm.InjectModel(folderBrowserDialog.SelectedPath, map.SelectedMeta);
            addme.name += "(new)";
            addme.name = GetNameDialog.Show("Choose injection name", "Meta Name:", addme.name, "OK");

            ArrayList oi = new ArrayList();
            oi.Add(addme);
            MapAnalyzer analyze = new MapAnalyzer();

            MapLayout layout = analyze.ScanMapForLayOut(map, false);
            layout.ReadChunks(map);
            Builder build = new Builder();
            build.MapBuilder(oi, ref layout, map, false);

            map = Map.Refresh(map);
            formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            this.Enabled = true;
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The lightmap viewer tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lightmapViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LightmapViewer lv = new LightmapViewer(ref map.SelectedMeta);
        }

        /// <summary>
        /// The load bsp.
        /// </summary>
        /// <param name="BSPNum">The bsp num.</param>
        /// <remarks></remarks>
        private void loadBSP(int BSPId)
        {
            this.Cursor = Cursors.WaitCursor;
            //// Load the BSP viewer
            Meta meta = new Meta(map);
            meta.TagIndex = BSPId; // load BSP into MemoryStream (MS)
            meta.ScanMetaItems(true, false);
            BSPModel bsp = new BSPModel(ref meta);
            this.Cursor = Cursors.Arrow;
            if (bsp.BspNumber != -1)
            {
                BSPViewer bv = new BSPViewer(bsp, map);
                bv.Dispose();
                bv = null;
            }

            meta.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// The load meta button_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void loadMetaButton_Click_1(object sender, EventArgs e)
        {
            loadMetaDialog.Filter = "XML Files (*.xml) | *.xml";
            loadMetaDialog.InitialDirectory = Prefs.pathExtractsFolder;
            if (loadMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            Prefs.pathExtractsFolder = loadMetaDialog.InitialDirectory;
            
            Meta newm = new Meta(map);

            // remove the xml extension now that we are forcing loading of XML files
            loadMetaDialog.FileName = loadMetaDialog.FileName.Substring(0, loadMetaDialog.FileName.Length - 4);
            if (loadMetaDialog.FileName.EndsWith("raw"))
            {
                MessageBox.Show("Do not inject raw files, they are injected with the parent tags!");
                return;
            }

            newm.LoadMetaFromFile(loadMetaDialog.FileName);

            if (newm.size >= 0 && newm.size <= map.SelectedMeta.size)
            {
                map.OpenMap(MapTypes.Internal);
                map.BW.BaseStream.Position = map.SelectedMeta.offset;
                map.BW.BaseStream.Write(newm.MS.ToArray(), 0, newm.size);
                map.CloseMap();
                formFuncs.AddReferencesToListView(map.SelectedMeta, references, map.DisplayType);
                MessageBox.Show("Done");
            }
            else
            {
                MessageBox.Show("Meta Won't Fit");
                return;
            }
        }

        /// <summary>
        /// The make bsp sticky tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void makeBSPStickyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BSPModel.BSPCollision coll = new BSPModel.BSPCollision(map.SelectedMeta);
            map.OpenMap(MapTypes.Internal);

            /*
            for (int x = 0; x < coll.SurfaceReflexiveCount; x++)
            {
                byte i = 4;
                map.BW.BaseStream.Position = map.SelectedMeta.offset + coll.SurfaceReflexiveTranslation + (x * 8) + 4;
                map.BW.Write(i);
            }
            */

            map.CloseMap();
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The meta editor new tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void metaEditorNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setEditorMode(EditorModes.MetaEditor2);
        }

        void wME_FormClosed(object sender, FormClosedEventArgs e)
        {
            setEditorMode(EditorModes.LastMode);
        }

        /// <summary>
        /// The meta editor tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void metaEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setEditorMode(EditorModes.MetaEditor1);

            if (map.SelectedMeta != null)
            {
                metaEditor1.loadControls(map);
            }
        }

        /// <summary>
        /// The meta list tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void metaListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (metaTreeToolStripMenuItem.Checked == false)
            {
                splitContainer1.Panel1Collapsed = false;
                metaTreeToolStripMenuItem.Checked = true;
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
                metaTreeToolStripMenuItem.Checked = false;
            }
        }

        /// <summary>
        /// The new add button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newAddButton_Click(object sender, EventArgs e)
        {
            Form f = (Form)((Button)sender).Parent;
            f.Close();
            newName = f.Controls[1].Text;
        }

        /// <summary>
        /// The new form_ form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                newName = string.Empty;
            }
        }

        /// <summary>
        /// The output list to file tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void outputListToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.AddExtension = true;
            sd.CheckFileExists = false;
            sd.DefaultExt = "txt";
            sd.FileName = "taglist";
            sd.Filter = "Text files (*.txt)|*.txt";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                if (metaView == FormFunctions.MetaView.InfoView)
                {
                    TreeNode tn = treeView1.Nodes[0];
                    while (tn != null)
                    {
                        sb.AppendLine(tn.Text);
                        tn = tn.NextNode;
                    }
                }
                else
                {
                    GetChildNodeNames(0, ref sb, treeView1.Nodes[0]);
                }

                using (StreamWriter sw = new StreamWriter(sd.FileName))
                {
                    sw.Write(sb.ToString());
                }

                MessageBox.Show("Done!");
            }
        }

        /// <summary>
        /// The overwrite meta tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void overwriteMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadMetaDialog.Filter = "XML Files (*.xml) | *.xml";
            if (loadMetaDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Meta newm = new Meta(map);

            // remove the xml extension now that we are forcing loading of XML files
            loadMetaDialog.FileName = loadMetaDialog.FileName.Substring(0, loadMetaDialog.FileName.Length - 4);
            if (loadMetaDialog.FileName.EndsWith("raw"))
            {
                MessageBox.Show("Do not inject raw files, they are injected with the parent tags!");
                return;
            }

            newm.LoadMetaFromFile(loadMetaDialog.FileName);
            MetaOverWriter.OverWrite(map, map.SelectedMeta.TagIndex, ref newm);
            int i = map.SelectedMeta.TagIndex;
            Meta.ItemType me = map.DisplayType;
            map = Map.Refresh(map);
            LoadMeta(i);
            formFuncs.AddReferencesToListView(map.SelectedMeta, references, me);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The parsed check box_ checked changed_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void parsedCheckBox_CheckedChanged_1(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            LoadMeta(map.SelectedMeta.TagIndex);
        }

        /// <summary>
        /// The plugin tool strip menu itemdescription_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void pluginToolStripMenuItemdescription_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < plugins.Plugin.Count; x++)
            {
                if (plugins.Plugin[x].PluginName == ((ToolStripDropDownItem)sender).Text)
                {
                    selectedplugin = x;
                    referenceEditorToolStripMenuItem.Checked = false;
                    metaEditorToolStripMenuItem.Checked = false;
                    hexEditorToolStripMenuItem.Checked = false;
                    LibraryPanel.Controls.Clear();
                    Application.DoEvents();
                    plugins.Plugin[x].Run(map, ref progressbar);
                    LibraryPanel.Controls.Add(plugins.Plugin[x]);
                    MetaEditorPanel.Visible = false;
                    hexView1.Visible = false;
                    references.Visible = false;
                    LibraryPanel.Visible = true;
                    break;
                }
            }
        }

        /// <summary>
        /// The portal viewer tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void portalViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PortalViewer pv = new PortalViewer(ref map.SelectedMeta);
        }

        /// <summary>
        /// The rebuilder tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void rebuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);
            Rebuilder re = new Rebuilder(this);
            re.ShowDialog();
            map.CloseMap();
            if (re.DialogResult != DialogResult.Cancel)
            {
                map = Map.Refresh(map);
                formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            }
        }

        /// <summary>
        /// The reference tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setEditorMode(EditorModes.ReferenceEditor);
        }

        /// <summary>
        /// Occurs when the user clicks on a column header. Used for sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void references_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sortColumn == e.Column)
                sortReverse = !sortReverse;
            else
            {
                sortReverse = false;
                sortColumn = e.Column;
            }
            sortReferences(sortColumn, sortReverse);
        }

        /// <summary>
        /// The references_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void references_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            #region Hilights tag under right-mouse click
            // Should always be ListView, just safety
            if (sender is ListView)
            {
                ListView lv = (ListView)sender;
                ListViewItem lvi = lv.GetItemAt(e.X, e.Y);                
                if (lvi != null && lvi.Selected == false)
                {
                    foreach (int item in lv.SelectedIndices)
                    {
                        lv.Items[item].Selected = false;
                    }
                    lvi.Selected = true;
                }
            }
            #endregion

            if (map.DisplayType == Meta.ItemType.Ident)
            {
                this.FloodfillSwapItem.Visible = true;
                this.JumpToTagItem.Visible = true;
            }
            else
            {
                this.FloodfillSwapItem.Visible = false;
                this.JumpToTagItem.Visible = false;
            }

            displayMenu.Show(references, new Point(e.X, e.Y));

            return;
        }

        /// <summary>
        /// The remove from quick list tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void removeFromQuickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If using registry, remove the selected tag 
            if (Prefs.useRegistryEntries)
            {
                if (treeView1.SelectedNode.FullPath == treeView1.SelectedNode.Name)
                {
                    RegistryAccess.removeKey(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + treeView1.SelectedNode.Text);
                }
                else
                {
                    RegistryAccess.removeValue(Microsoft.Win32.Registry.CurrentUser,
                                                RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + treeView1.SelectedNode.Parent.Text,
                                                treeView1.SelectedNode.Text);
                }
            }

            if (treeView1.SelectedNode.Parent == null) // remove tag type
            {
                // If using registry, remove the selected tag type
                if (Prefs.useRegistryEntries)
                {
                    RegistryAccess.removeKey(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + treeView1.SelectedNode);
                }

                Prefs.QuickAccessTagType quickAccess = Prefs.GetQuickAccessTagType(treeView1.SelectedNode.Text);
                Prefs.QuickAccessTagTypes.Remove(quickAccess);

                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }
            else if (treeView1.SelectedNode.Parent.Nodes.Count <= 1) // remove tag type
            {
                // If using registry, remove the selected tag type
                if (Prefs.useRegistryEntries)
                {
                    RegistryAccess.removeKey(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + treeView1.SelectedNode.Parent.Text);
                }

                Prefs.QuickAccessTagType quickAccess = Prefs.GetQuickAccessTagType(treeView1.SelectedNode.Parent.Text);
                Prefs.QuickAccessTagTypes.Remove(quickAccess); 
                
                treeView1.Nodes.Remove(treeView1.SelectedNode.Parent);
            }
            else // remove tag path
            {
                Prefs.QuickAccessTagType quickAccess = Prefs.GetQuickAccessTagType(treeView1.SelectedNode.Parent.Text);
                quickAccess.TagPaths.Remove(treeView1.SelectedNode.Text);

                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }
        }

        /// <summary>
        /// The rename map button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void renameMapButton_Click(object sender, EventArgs e)
        {
            string newName = map.MapHeader.mapName;
            askForName a = new askForName( "Rename Map Internally", "Enter new map name", "&Rename Map", 
                                           map.MapHeader.scenarioPath, newName, true);
            if (a.ShowDialog() == DialogResult.OK)
            {
                this.setMapName(a.getTextBoxValue());
                a.Dispose();
            }
        }

        /// <summary>
        /// Renames a TAG without rebuilding the map.
        /// Allows names to be expanded by however many characters are available in the padding.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int extraSpace = map.FileNames.Offset[map.FileNames.Offset.Length - 1];
            extraSpace = map.MapHeader.offsetTofileIndex - map.MapHeader.offsetTofileNames - extraSpace - 1;

            newName = treeView1.SelectedNode.Text;
            do
            {
                // Create a form for renaming a new tag                
                askForName a = new askForName("Rename Tag", "Enter new tag name", "&Rename Tag", newName, newName.Substring(newName.LastIndexOf('\\') + 1), true);
                if (a.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                newName = a.getTextBoxValue();
                a.Dispose();

                // Check if there is enough space for new name
                if (extraSpace - (newName.Length - treeView1.SelectedNode.Text.Length) < 0)
                {
                    MessageBox.Show(
                        "Name is " + (-(extraSpace - (newName.Length - treeView1.SelectedNode.Text.Length))) +
                        " characters too long!");
                }
            }
            while (extraSpace - (newName.Length - treeView1.SelectedNode.Text.Length) < 0);

            if (newName == treeView1.SelectedNode.Text)
            {
                return;
            }

            if (newName != string.Empty)
            {
                int id = map.Functions.ForMeta.FindByNameAndTagType(
                    treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text);
                if (id == -1)
                {
                    MessageBox.Show("Tag name cannot be found in Meta!");
                    return;
                }

                int offset = map.FileNames.Offset[id];
                int diff = newName.Length - treeView1.SelectedNode.Text.Length;
                map.FileNames.Name[id] = newName;
                map.OpenMap(MapTypes.Internal);
                BinaryReader br = map.BR;
                BinaryWriter bw = map.BW;

                // If we are removing characters, null out data at end
                if (diff < 0)
                {
                    bw.BaseStream.Position = map.MapHeader.offsetTofileNames +
                                             map.FileNames.Offset[map.FileNames.Offset.Length - 1] +
                                             map.FileNames.Length[map.FileNames.Name.Length - 1];
                    bw.Write(new byte[-diff]);
                }

                // reposition filename within file and update the filename index
                for (int x = map.FileNames.Offset.Length - 1; x >= 0; x--)
                {
                    if (map.FileNames.Offset[x] >= offset)
                    {
                        br.BaseStream.Position = map.MapHeader.offsetTofileIndex + x * 4;
                        int o = br.ReadInt32();

                        map.FileNames.Offset[x] += diff;
                        bw.BaseStream.Position = map.MapHeader.offsetTofileIndex + x * 4;
                        bw.Write(map.FileNames.Offset[x]);
                        bw.BaseStream.Position = map.MapHeader.offsetTofileNames + map.FileNames.Offset[x];
                        bw.Write(map.FileNames.Name[x].ToCharArray());
                        bw.Write((byte)0);
                    }
                    else
                    {
                        break;
                    }
                }

                map.CloseMap();
                toolStripHistoryList.DropDownItems[0].Name = "[" + map.MetaInfo.TagType[id] + "] " + newName;
                toolStripHistoryList.DropDownItems[0].Text = toolStripHistoryList.DropDownItems[0].Name;
                toolStripHistoryList.DropDownItems[0].Tag = new[]
                    {
                       id.ToString(), map.MetaInfo.TagType[id], map.FileNames.Name[id] 
                    };
                addToQuickList(map.MetaInfo.TagType[id], map.FileNames.Name[id]);
                RefreshTreeView();
                tsItem_Click(toolStripHistoryList.DropDownItems[0], null);
            }
        }

        /// <summary>
        /// The save bitmap tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void saveBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] s = map.SelectedMeta.name.Split('\\');
            saveBitmapDialog1.FileName = s[s.Length - 1];
            saveBitmapDialog1.InitialDirectory = Prefs.pathBitmapsFolder;
            if (saveBitmapDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            Prefs.pathBitmapsFolder = saveBitmapDialog1.FileName.Substring(
                0, saveBitmapDialog1.FileName.LastIndexOf('\\'));
            
            // for (int index = 0; index < Map.SelectedMeta.raw.rawChunks.Count; index++)
            // {

            switch (saveBitmapDialog1.FilterIndex)
            {
                    // DDS
                case 1:

                    // Split save name up into parts
                    s = new string[3];
                    s[0] = saveBitmapDialog1.FileName.Substring(0, saveBitmapDialog1.FileName.LastIndexOf('\\') + 1);
                    s[1] = saveBitmapDialog1.FileName.Substring(
                        s[0].Length, saveBitmapDialog1.FileName.LastIndexOf('.') - s[0].Length);
                    s[2] = saveBitmapDialog1.FileName.Substring(saveBitmapDialog1.FileName.LastIndexOf('.'));

                    int numDDS = 1;
                    for (int i = 0; i < map.SelectedMeta.items.Count; i++)
                    {
                        if (map.SelectedMeta.items[i].offset == 68)
                        {
                            numDDS = ((MetaScanner.XReflex)map.SelectedMeta.items[i]).chunkcount;
                            break;
                        }
                    }

                    // If there is more than 1 DDS images, make a directory to hold them
                    if (numDDS > 1)
                    {
                        Directory.CreateDirectory(s[0] + s[1]);
                    }

                    ParsedBitmap bm = new ParsedBitmap(ref map.SelectedMeta, map);

                    bool convert = false;

                    /*
                    if (MessageBox.Show("Do you wish to save in the original format?", "DDS Extraction", MessageBoxButtons.YesNo) == DialogResult.No)
                        convert = true;
                    */
                    List<extractionInfo> extractInfo = new List<extractionInfo>();

                    // # of DDS files (not raw chunks as some are mipmaps)
                    for (int index = 0; index < numDDS; index++)
                    {
                        FileStream fs;

                        // Create the file, either in the direcotry if multiple or the single file
                        if (numDDS > 1)
                        {
                            fs =
                                new FileStream(
                                    s[0] + s[1] + "\\" + s[1] + "_" + index.ToString().PadLeft(2, '0') + s[2], 
                                    FileMode.Create);
                        }
                        else
                        {
                            fs = new FileStream(saveBitmapDialog1.FileName, FileMode.Create);
                        }

                        BinaryWriter br = new BinaryWriter(fs);

                        ParsedBitmap.BitmapInfo chunkInfo = null;

                        // Conversion section disabled ATM. Maybe add it later...
                        if (convert)
                        {
                            // Create Form to choose Bitmap Format
                            Form tf = new Form();
                            tf.ControlBox = false;
                            tf.Size = new Size(250, 120);
                            tf.Text = "DDS output format";
                            Label tl = new Label();
                            tl.AutoSize = true;
                            tl.Location = new Point(10, 2);
                            tl.Text = "Select output format:";
                            ComboBox tcb = new ComboBox();
                            tcb.DropDownStyle = ComboBoxStyle.DropDownList;
                            tcb.Location = new Point(10, 20);
                            tcb.Size = new Size(200, 20);
                            DDS.DDSFileFormat ff = DDS.getDDSType(bm.Properties[index]);
                            string[] sa = Enum.GetNames(typeof(DDS.DDSFileFormats));
                            foreach (string st in sa)
                            {
                                tcb.Items.Add(st.Replace('_', ' '));
                                if (ff.Format.ToString() == st)
                                {
                                    tcb.SelectedIndex = tcb.Items.Count - 1;
                                }
                            }

                            Button tb = new Button();
                            tb.Location = new Point(80, 50);
                            tb.DialogResult = DialogResult.OK;
                            tb.Text = "&Save";

                            tf.Controls.Add(tl);
                            tf.Controls.Add(tcb);
                            tf.Controls.Add(tb);
                            tf.ShowDialog();
                            string n = Enum.GetName(typeof(DDS.H2DDSFormats), tcb.SelectedIndex);
                            ParsedBitmap.BitmapInfo bi =
                                new ParsedBitmap.BitmapInfo(ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8, false);

                            // DDS_Convert.DecodeDDS();
                            chunkInfo = DDS.ExtractDDS(map.SelectedMeta, bm, ref br, index, bi);
                        }
                        else
                        {
                            chunkInfo = DDS.ExtractDDS(map.SelectedMeta, bm, ref br, index);
                        }

                        // This is used to tell user expected DDS format when reinjecting
                        DDS.DDSFileFormat DDSFF = DDS.getDDSType(chunkInfo);
                        for (int count = 0; count <= extractInfo.Count; count++)
                        {
                            if (count == extractInfo.Count)
                            {
                                extractInfo.Add(
                                    new extractionInfo(
                                        DDSFF.Description, fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1)));
                                break;
                            }
                            else if (extractInfo[count].name == DDSFF.Format.ToString())
                            {
                                extractInfo[count].lists.Add(fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1));
                            }
                        }

                        br.Close();
                        fs.Close();
                    }

                    // string tempS = "When reinjecting, make sure the following files are saved as the type specified:\n\n";
                    int retCount = 0;
                    string tempS = "The following files have been saved as the type specified:\n\n";
                    for (int i = 0; i < extractInfo.Count; i++)
                    {
                        for (int ii = 0; ii < extractInfo[i].lists.Count; ii++)
                        {
                            tempS += extractInfo[i].lists[ii].PadRight(50) + "  " + extractInfo[i].name + "\n";
                            retCount++;
                        }
                    }

                    int maxLength = Screen.PrimaryScreen.Bounds.Height / 20 - 4;
                    while (retCount > maxLength)
                    {
                        int ret = 0;
                        for (int cc = 0; cc < maxLength; cc++)
                        {
                            ret = tempS.IndexOf('\n', ret + 1);
                            if (ret == -1)
                            {
                                break;
                            }
                        }

                        if (ret == -1)
                        {
                            break;
                        }

                        MessageBox.Show(tempS.Substring(0, ret));
                        tempS = tempS.Substring(ret);
                        retCount -= maxLength;
                    }

                    MessageBox.Show(tempS);
                    break;

                    // BMP
                case 2:
                    pictureBox1.Image.Save(saveBitmapDialog1.FileName, ImageFormat.Bmp);
                    break;
                    // JPG
                case 3:
                    pictureBox1.Image.Save(saveBitmapDialog1.FileName, ImageFormat.Jpeg);
                    break;
            }

            // }
        }

        /// <summary>
        /// The save meta button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void saveMetaButton_Click(object sender, EventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            if (recursiveCheckBox.Checked == false)
            {
                string[] split = map.SelectedMeta.name.Split('\\');
                string temp = split[split.Length - 1];
                temp = temp.Replace('<', '_');
                temp = temp.Replace('>', '_');
                saveMetaDialog.FileName = temp; // + "." + Map.SelectedMeta.type;
                saveMetaDialog.InitialDirectory = Prefs.pathExtractsFolder;
                if (saveMetaDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                Prefs.pathExtractsFolder = saveMetaDialog.InitialDirectory;

                map.SelectedMeta.SaveMetaToFile(saveMetaDialog.FileName, true);
            }
            else
            {
                folderBrowserDialog.SelectedPath = Prefs.pathExtractsFolder;
                if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                Prefs.pathExtractsFolder = folderBrowserDialog.SelectedPath;

                map.OpenMap(MapTypes.Internal);
                map.SelectedMeta.SaveRecursive(
                    folderBrowserDialog.SelectedPath, parsedCheckBox.Checked, progressbar);
                map.CloseMap();
                MessageBox.Show("Done");
            }
        }

        /// <summary>
        /// The search button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void searchButton_Click(object sender, EventArgs e)
        {
            // Search selected TAG
            FindType[] finds = new FindType[0];
            if (this.searchComboBox.SelectedIndex == 0)
            {
                Control metaPanel = metaEditor1.Controls[0];
                searchPanel(metaPanel, searchTextBox.Text, ref finds);
            }
        }

        /// <summary>
        /// The search panel.
        /// </summary>
        /// <param name="dataControl">The data control.</param>
        /// <param name="searchValue">The search value.</param>
        /// <param name="finds">The finds.</param>
        /// <remarks></remarks>
        private void searchPanel(Control dataControl, string searchValue, ref FindType[] finds)
        {
            if (dataControl.HasChildren)
            {
                foreach (Control childControl in dataControl.Controls)
                {
                    searchPanel(childControl, searchValue, ref finds);
                    if (childControl.Name == "reflexive")
                    {
                        // childControl.
                    }

                    if (childControl.Text.Contains(searchValue))
                    {
                        MessageBox.Show(childControl.Name + " - " + childControl.Text);
                        finds = new FindType[finds.Length + 1];
                        finds[finds.Length - 1].Text = childControl.Text;
                        finds[finds.Length - 1].Location = childControl;
                    }
                }
            }
        }

        /// <summary>
        /// The search text box_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void searchTextBox_Click(object sender, EventArgs e)
        {
            // searchTextBox_Enter(sender, e);
        }

        /// <summary>
        /// The search text box_ enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            searchTextBox.SelectAll();
        }

        /// <summary>
        /// The search text box_ key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                searchButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Sets the selected MATG tag to the active one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setActiveMatgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);

            // Find offset of selected MATG tag
            int matgIndex = map.Functions.ForMeta.FindMetaByID(map.IndexHeader.matgID);

            int matgIndexOffset = map.IndexHeader.tagsOffset + matgIndex * 16 + 8;
            // Save information for active MATG
            map.BR.BaseStream.Position = matgIndexOffset;
            byte[] matgData = map.BR.ReadBytes(8);

            int tagIndexOffset = map.IndexHeader.tagsOffset + map.SelectedMeta.TagIndex * 16 + 8;
            // Save information for new MATG
            map.BR.BaseStream.Position = tagIndexOffset;
            byte[] newData = map.BR.ReadBytes(8);

            // Store information for new MATG into active MATG
            map.BW.BaseStream.Position = matgIndexOffset;
            map.BW.Write(newData);

            // Store information for old MATG into inactive MATG
            map.BW.BaseStream.Position = tagIndexOffset;
            map.BW.Write(matgData);

            map.CloseMap();

            string message = "[matg] " + map.FileNames.Name[map.SelectedMeta.TagIndex] + " has been moved to the active tag.\n" +
                             "\nMake all edits to " + "[matg] " + map.FileNames.Name[matgIndex];



            // Update map
            map = Map.Refresh(map);

            string name = "[matg] " + map.FileNames.Name[matgIndex];

            selectTag(matgIndex);

            MessageBox.Show(message);
        }

        /// <summary>
        /// Sets the selected SCNR tag to the active one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setActiveScnrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);

            // Find offset of selected SCNR tag
            int scnrIndex = map.Functions.ForMeta.FindMetaByID(map.IndexHeader.scnrID);

            int scnrIndexOffset = map.IndexHeader.tagsOffset + scnrIndex * 16 + 8;
            // Save information for currently active SCNR
            map.BR.BaseStream.Position = scnrIndexOffset;
            byte[] scnrData = map.BR.ReadBytes(8);

            int tagIndexOffset = map.IndexHeader.tagsOffset + map.SelectedMeta.TagIndex * 16 + 8;
            // Save information for (currently selected) new SCNR
            map.BR.BaseStream.Position = tagIndexOffset;
            byte[] newData = map.BR.ReadBytes(8);

            // Store information for new SCNR into active SCNR
            map.BW.BaseStream.Position = scnrIndexOffset;
            map.BW.Write(newData);

            // Store information for old SCNR into inactive SCNR
            map.BW.BaseStream.Position = tagIndexOffset;
            map.BW.Write(scnrData);

            map.CloseMap();

            string message = "[scnr] " + map.FileNames.Name[map.SelectedMeta.TagIndex] + " has been moved to the active tag.\n" +
                             "\nMake all edits to " + "[scnr] " + map.FileNames.Name[scnrIndex];



            // Update map
            map = Map.Refresh(map);

            string name = "[scnr] " + map.FileNames.Name[scnrIndex];
            //setNodePath(treeView1, map.FileNames.Name[scnrIndex]);

            selectTag(scnrIndex);

            MessageBox.Show(message);
        }

        /// <summary>
        /// The set node path.
        /// </summary>
        /// <param name="tv">The tv.</param>
        /// <param name="fullPath">The full path.</param>
        /// <remarks></remarks>
        private void setNodePath(TreeView tv, string fullPath)
        {
            TreeNode[] tn = treeView1.Nodes.Find(fullPath, true);
            tv.SelectedNode = tn[tn.Length - 1];
        }

        /// <summary>
        /// The sign map button_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void signMapButton_Click_1(object sender, EventArgs e)
        {
            map.Sign();
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The tag type tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tagTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tagTypeToolStripMenuItem.Checked = true;
            folderHierarchyToolStripMenuItem.Checked = false;
            toolStripTagView.Checked = true;
            toolStripFolderView.Checked = false;
            toolStripInfoView.Checked = false;
            metaView = FormFunctions.MetaView.TagTypeView;
            RefreshTreeView();
        }

        /// <summary>
        /// The to obj tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            ParsedModel pm = new ParsedModel(ref map.SelectedMeta);
            pm.ExtractMeshesToOBJ(folderBrowserDialog.SelectedPath);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The to x tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            ParsedModel pm = new ParsedModel(ref map.SelectedMeta);
            pm.ExtractMeshesToX(folderBrowserDialog.SelectedPath);
            MessageBox.Show("Done");
        }

        /// <summary>
        /// The tool strip bsp editor_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripBSPEditor_Click(object sender, EventArgs e)
        {
            // ToolStripDropDownItem tsItem = (ToolStripDropDownItem)sender;
            if (map.BSP.bspcount == 1)
            {
                int BSPId = map.Functions.ForMeta.FindMetaByID(map.BSP.sbsp[0].ident);
                loadBSP(BSPId);
            }
            else if (((ToolStripItem)sender).Name != "toolStripBSPEditorDropDown")
            {
                int i = toolStripBSPEditorDropDown.DropDownItems.IndexOf((ToolStripMenuItem)sender);
                int BSPId = map.Functions.ForMeta.FindMetaByID(map.BSP.sbsp[i].ident);
                loadBSP(BSPId);
            }
        }

        /// <summary>
        /// The tools window tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolsToolStripMenuItem.Checked == false)
            {
                splitContainer2.Panel1Collapsed = false;
                toolsToolStripMenuItem.Checked = true;
            }
            else
            {
                splitContainer2.Panel1Collapsed = true;
                toolsToolStripMenuItem.Checked = false;
            }
        }

        /// <summary>
        /// The tree view 1_ after select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // This allows for the bitmap viewer to be automatically shown when a [bitm] tag is selected and the
            // default viewer for [bitm] is still bitmap viewer
            if (e.Node.Parent != null)
                if (e.Node.Parent.Text == "bitm" && ltmpTools.Visible)
                {
                    ltmpTools.Visible = true;
                    ltmpTools.BringToFront();
                }
                else
                {                    
                    ltmpTools.SendToBack();
                }

            // Display the tags by the selected listing view
            switch (metaView)
            {
                #region TagType View
                case FormFunctions.MetaView.TagTypeView:
                    if (e.Node.Parent == null)
                    {
                        // *** Do we really want to NULL out our selection?
                        // Map.SelectedMeta = null;
                        return;
                    }

                    int tagNum = map.Functions.ForMeta.FindByNameAndTagType(e.Node.Parent.Text, e.Node.Text);
                    if (tagNum != -1)
                    {
                        // If we are within the <ALL TAGS> listing, add to registry and main tag listing
                        if (((TreeView)sender).SelectedNode.FullPath.StartsWith(this.treeView1.Nodes[0].Text))
                        {
                            addToQuickList(map.MetaInfo.TagType[tagNum], map.FileNames.Name[tagNum]);
                        }

                        LoadMeta(tagNum);
                        return;
                    }

                    break;

                #endregion

                #region FolderView

                case FormFunctions.MetaView.FolderView:
                    for (int x = 0; x < map.IndexHeader.metaCount; x++)
                    {
                        string[] tempn = map.FileNames.Name[x].Split('\\');

                        string tempi = tempn[tempn.Length - 1] + "." + map.MetaInfo.TagType[x];
                        int i = map.FileNames.Name[x].LastIndexOf('\\');
                        string tempp = string.Empty;
                        if (i != -1)
                        {
                            tempp = map.FileNames.Name[x].Substring(0, i) + "\\";
                        }

                        // MessageBox.Show(e.Node.Tag.ToString());
                        if (e.Node.Text == tempi &&
                            e.Node.Tag.ToString() == map.FileNames.Name[x] + "." + map.MetaInfo.TagType[x])
                        {
                            LoadMeta(x);
                            return;
                        }
                    }

                    map.SelectedMeta = null;
                    break;

                    #endregion

                #region InfoView

                case FormFunctions.MetaView.InfoView:
                    TreeNode tn = e.Node;
                    if (tn.Text.Contains("<HEADER>"))
                    {
                        Meta m = new Meta(map);
                        m.TagIndex = 99999;
                        m.offset = 0;
                        m.size = 2048;
                        m.ident = 0;
                        m.type = "HEAD";
                        map.OpenMap(MapTypes.Internal);
                        map.BR.BaseStream.Position = m.offset;
                        m.MS = new MemoryStream(m.size);
                        m.MS.Write(map.BR.ReadBytes(m.size), 0, m.size);
                        map.CloseMap();
                        map.SelectedMeta = m;

                        // Main Form Offsets/Idents/etc
                        metaOffsetBox.Text = "0x" + m.offset.ToString("X8") + "\n" + m.offset.ToString("N0") + " bytes";
                        metaSizeBox.Text = "0x" + m.size.ToString("X4") + "\n" + m.size.ToString("N0") + " bytes";
                        metaIdentBox.Text = m.ident.ToString("X");
                        metaTypeBox.Text = m.type;
                        if (referenceEditorToolStripMenuItem.Checked)
                        {
                            formFuncs.AddReferencesToListView(map.SelectedMeta, references, map.DisplayType);
                        }

                        if (metaEditorToolStripMenuItem.Checked)
                        {
                            metaEditor1.loadControls(map);
                        }

                        if (hexEditorToolStripMenuItem.Checked)
                        {
                            hexView1.Reload(map.SelectedMeta, map);
                            hexView1.Setup(map.filePath);
                        }
                    }

                    if (tn.Tag == null)
                    {
                        tn = e.Node.Parent;
                    }

                    if (tn == null)
                    {
                        return;
                    }

                    for (int x = 0; x < map.IndexHeader.metaCount; x++)
                    {
                        string fullPath = map.FileNames.Name[x] + "." + map.MetaInfo.TagType[x];
                        if (tn.Tag.ToString() == fullPath)
                        {
                            if (map.SelectedMeta == null)
                            {
                                LoadMeta(x);
                            }
                            else if (map.SelectedMeta.TagIndex != x)
                            {
                                LoadMeta(x);
                            }

                            return;
                        }
                    }

                    map.SelectedMeta = null;
                    break;

                #endregion
            }
        }

        /// <summary>
        /// The tree view 1_ before expand.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (metaView != FormFunctions.MetaView.InfoView)
            {
                return;
            }

            string[] mReflex = e.Node.Text.Split('.');
            if (mReflex.Length != 3)
            {
                return;
            }

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if ((mReflex[2] == map.FileNames.Name[x]) && (mReflex[1] == "[" + map.MetaInfo.TagType[x] + "]"))
                {
                    map.OpenMap(MapTypes.Internal);
                    Meta m = new Meta(map);
                    m.ReadMetaFromMap(x, false);

                    IFPIO ifpx = IFPHashMap.GetIfp(m.type, map.HaloVersion);
                    m.headersize = ifpx.headerSize;

                    // Scans IFP and loads IDENTS, REFLEXIVES & STRINGS into "m" for Reference List
                    m.scanner.ScanWithIFP(ref ifpx);
                    MetaSplitter metasplit = new MetaSplitter();
                    metasplit.SplitWithIFP(ref ifpx, ref m, map);

                    e.Node.Nodes.Clear();

                    if (map.MetaInfo.TagType[x] != "sbsp")
                    {
                        DisplaySplit(metasplit.Header, e.Node);
                    }

                    map.CloseMap();
                }
            }
        }

        /// <summary>
        /// Makes right clicks select node under cursofr as well
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == MouseButtons.Right)
            {
                TreeNode c = treeView1.GetNodeAt(me.Location);
                treeView1.SelectedNode = c;
            }
        }

        /// <summary>
        /// The tree view 1_ drag drop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {            
            MapForm f = (MapForm)e.Data.GetData(this.GetType());
            this.treeView1.PathSeparator = ".";
            f.treeView1.PathSeparator = ".";
            string lastPath = f.treeView1.SelectedNode.FullPath;
            f.treeView1.PathSeparator = "\\";
            if (f == this)
            {
                return;
            }

            // try
            // {
            this.Enabled = false;
            this.Focus();
            ArrayList metas = new ArrayList(0);

            metas = f.scanandpassmetasfordraganddrop();

            int x = 0;
            while (x < metas.Count)
            {
                Meta temp = (Meta)metas[x];
                string tagtype = temp.type;
                string tagname = temp.name;
                int tempint = map.Functions.ForMeta.FindByNameAndTagType(tagtype, tagname);
                if (tempint != -1)
                {
                    metas.RemoveAt(x);
                    continue;
                }

                temp.RelinkReferences();
                x++;
            }

            if (x > 0)
            {
                MapAnalyzer analyze = new MapAnalyzer();
                MapLayout layout = analyze.ScanMapForLayOut(map, false);
                layout.ReadChunks(map);
                Builder build = new Builder();
                build.MapBuilder(metas, ref layout, map, soundsCheckBox.Checked);
                map = Map.Refresh(map);
                formFuncs.AddMetasToTreeView(map, treeView1, metaView, false);
            }

            ChunkClonerWindow.ExpandToNode(treeView1.Nodes, lastPath, treeView1.PathSeparator);
            this.treeView1.PathSeparator = "\\";
            this.Enabled = true;
            MessageBox.Show(x + " metas transferred\nDone");

            // }
            // catch
            // {
            this.Enabled = true;

            // }
        }

        /// <summary>
        /// The tree view 1_ drag enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// The tree view 1_ item drag.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (map.SelectedMeta == null)
            {
                return;
            }

            this.DoDragDrop(this, DragDropEffects.Copy | DragDropEffects.Move);
        }

        /// <summary>
        /// The tree view 1_ key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((byte)e.KeyChar == 22)
            {
                // Ctrl + v
                if (map.SelectedMeta == null)
                {
                    return;
                }

                if (map.SelectedMeta.type == "PRTM")
                {
                    // PRTM Viewer
                    PRTMModel pm = new PRTMModel(ref map.SelectedMeta);
                    ModelViewer mv = new ModelViewer(pm);
                }
                else if (map.SelectedMeta.type == "mod2" || map.SelectedMeta.type == "mode")
                {
                    // MODE Viewer
                    ParsedModel pm = new ParsedModel(ref map.SelectedMeta);
                    ModelViewer mv = new ModelViewer(pm);
                    pm.Dispose();
                    pm = null;
                    mv.Dispose();
                    mv = null;
                }
                else if (map.SelectedMeta.type == "coll")
                {
                    collisonViewerToolStripMenuItem_Click(this, null);
                }
                else if (map.SelectedMeta.type == "sbsp")
                {
                    loadBSP(map.SelectedMeta.TagIndex);
                }
            }
        }

        /// <summary>
        /// The tree view 1_ node mouse double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                this.metaEditorNewToolStripMenuItem_Click(sender, e);
            }
        }

        /// <summary>
        /// The treeviewcontext_ opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeviewcontext_Opening(object sender, CancelEventArgs e)
        {
            // If click is on MATG or SCNR tag, show option to make active
            if (treeView1.SelectedNode.Parent != null)
            {
                /*
                 * Issues lie with duplicating / moving MATG as it is used to calculate the secondary magic
                 * 
                 * INDEXHEADERINFO.CS: LoadHalo2IndexHeaderInfo(ref BinaryReader BR, Map map)
                 * See line 148: (Using MATG to calculate secondary magic)
                 *  BR.BaseStream.Position = tagsOffset + 8;
                 *  map.SecondaryMagic = BR.ReadInt32() - (map.MapHeader.indexOffset + map.MapHeader.metaStart);
                 
                if (treeView1.SelectedNode.Parent.Text == "matg")
                    setActiveMatgToolStripMenuItem.Visible = true;
                else
                    setActiveMatgToolStripMenuItem.Visible = false;
                */

                if (treeView1.SelectedNode.Parent.Text == "scnr")
                    setActiveScnrToolStripMenuItem.Visible = true;
                else
                    setActiveScnrToolStripMenuItem.Visible = false;
            }

            if (treeView1.SelectedNode.Parent == treeView1.Nodes[0] || treeView1.SelectedNode.Parent == null)
            {
                duplicateToolStripMenuItem.Visible = false;
                duplicateRecursivelyToolStripMenuItem.Visible = false;
            }
            else
            {
                duplicateToolStripMenuItem.Visible = true;
                duplicateRecursivelyToolStripMenuItem.Visible = true;
            }

            removeFromQuickListToolStripMenuItem.Visible = true;
            clearTagQuickListToolStripMenuItem.Visible = true;
            if (metaView != FormFunctions.MetaView.TagTypeView)
            {
                removeFromQuickListToolStripMenuItem.Visible = false;
                clearTagQuickListToolStripMenuItem.Visible = false;
            }

            if (treeView1.SelectedNode.FullPath.StartsWith(treeView1.Nodes[0].Text))
            {
                removeFromQuickListToolStripMenuItem.Visible = false;
            }
        }

        /// <summary>
        /// The ts item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsItem_Click(object sender, EventArgs e)
        {
            string[] s = (String[])((ToolStripMenuItem)sender).Tag;
            int tagNum = int.Parse(s[0]);

            if (this.treeView1.Nodes.ContainsKey(map.MetaInfo.TagType[tagNum]))
            {
                treeView1.SelectedNode =
                    this.treeView1.Nodes[this.treeView1.Nodes.IndexOfKey(map.MetaInfo.TagType[tagNum])];
                foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                {
                    if (tn.Text == map.FileNames.Name[tagNum])
                    {
                        treeView1.SelectedNode = tn;
                        break;
                    }
                }
            }

            LoadMeta(tagNum);

            // throw new NotImplementedException();
        }

        /// <summary>
        /// The view bsp tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void viewBSPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadBSP(map.SelectedMeta.TagIndex);
        }

        /// <summary>
        /// The view model tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void viewModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParsedModel pm = new ParsedModel(ref map.SelectedMeta);

            ModelViewer mv = new ModelViewer(pm);

            pm.Dispose();
            pm = null;
            mv.Dispose();
            mv = null;
        }

        /// <summary>
        /// The view prtm tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void viewPRTMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PRTMModel pm = new PRTMModel(ref map.SelectedMeta);

            ModelViewer mv = new ModelViewer(pm);
        }

        #endregion

        /// <summary>
        /// The find type.
        /// </summary>
        /// <remarks></remarks>
        public struct FindType
        {
            #region Constants and Fields

            /// <summary>
            /// The location.
            /// </summary>
            public Control Location;

            /// <summary>
            /// The text.
            /// </summary>
            public string Text;

            #endregion
        }

        /// <summary>
        /// The extraction info.
        /// </summary>
        /// <remarks></remarks>
        private class extractionInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The lists.
            /// </summary>
            public readonly List<string> lists;

            /// <summary>
            /// The name.
            /// </summary>
            public readonly string name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="extractionInfo"/> class.
            /// </summary>
            /// <param name="Name">The name.</param>
            /// <param name="list">The list.</param>
            /// <remarks></remarks>
            public extractionInfo(string Name, string list)
            {
                this.name = Name;
                lists = new List<string>();
                lists.Add(list);
            }

            #endregion
        }

        /// <summary>
        /// Summary description for FormFunctions.
        /// </summary>
        /// <remarks></remarks>
        public class FormFunctions
        {
            #region Constants and Fields

            /// <summary>
            /// The list items.
            /// </summary>
            public List<ListViewItem> ListItems = new List<ListViewItem>();

            /// <summary>
            /// The root.
            /// </summary>
            private readonly Hashtable root = new Hashtable();

            #endregion

            #region Enums

            /// <summary>
            /// The meta view.
            /// </summary>
            /// <remarks></remarks>
            public enum MetaView
            {
                /// <summary>
                /// The folder view.
                /// </summary>
                FolderView,

                /// <summary>
                /// The tag type view.
                /// </summary>
                TagTypeView,

                /// <summary>
                /// The info view.
                /// </summary>
                InfoView
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The add columns to list view.
            /// </summary>
            /// <param name="templv">The templv.</param>
            /// <param name="type">The type.</param>
            /// <remarks></remarks>
            public void AddColumnsToListView(ListView templv, Meta.ItemType type)
            {
                templv.Columns.Clear();
                switch (type)
                {
                    case Meta.ItemType.Reflexive:
                        templv.Columns.Add("Description", 120, HorizontalAlignment.Center);

                        templv.Columns.Add("Offset", 80, HorizontalAlignment.Center);

                        templv.Columns.Add("Translation", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("ChunkCount", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("ChunkSize", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("Points To Tag Type", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("Points To Tag Name", 180, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Type", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Name", 180, HorizontalAlignment.Center);
                        break;
                    case Meta.ItemType.Ident:
                        templv.Columns.Add("Description", 120, HorizontalAlignment.Center);
                        templv.Columns.Add("Offset", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("Points To Tag Type", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("Points To Tag Name", 180, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Type", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Name", 1800, HorizontalAlignment.Center);
                        break;
                    case Meta.ItemType.String:
                        templv.Columns.Add("Description", 120, HorizontalAlignment.Center);
                        templv.Columns.Add("Offset", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("Name", 120, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Type", 80, HorizontalAlignment.Center);
                        templv.Columns.Add("In Tag Name", 180, HorizontalAlignment.Center);
                        break;
                }
            }

            /// <summary>
            /// The add metas to list view.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="templv">The templv.</param>
            /// <remarks></remarks>
            public void AddMetasToListView(ref Map map, ListView templv)
            {
                templv.VirtualMode = true;
                templv.VirtualListSize = 0;
                templv.RetrieveVirtualItem -= metalist_RetrieveVirtualItem;
                templv.BeginUpdate();
                templv.AutoScrollOffset = new Point(0, 0);
                templv.Columns.Clear();
                templv.View = View.Details;
                templv.CheckBoxes = true;
                templv.Columns.Add("Size", 80, HorizontalAlignment.Center);
                templv.Columns.Add("Tag Type", 80, HorizontalAlignment.Center);
                templv.Columns.Add("Tag Name", 180, HorizontalAlignment.Center);
                ListItems.Clear();

                // templv.Sort = false;
                ListViewItem l;
                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    l = new ListViewItem(map.MetaInfo.Size[x].ToString());

                    l.SubItems.Add(map.MetaInfo.TagType[x]);
                    l.SubItems.Add(map.FileNames.Name[x]);
                    l.Checked = true;

                    ListItems.Add(l);
                }

                try
                {
                    templv.VirtualListSize = ListItems.Count;
                }
                catch
                {
                }

                // templv.
                templv.RetrieveVirtualItem += templv_RetrieveVirtualItem;
                templv.EndUpdate();

                Application.DoEvents();
            }

            /// <summary>
            /// The add references to list view.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <param name="templv">The templv.</param>
            /// <param name="type">The type.</param>
            /// <remarks></remarks>
            public void AddReferencesToListView(Meta meta, ListView templv, Meta.ItemType type)
            {
                if (meta == null)
                {
                    return;
                }
                
                ///// Annoying error ?here? on Vista, etc /////
                try
                {
                    templv.VirtualMode = true;
                    templv.VirtualListSize = 0;
                    templv.RetrieveVirtualItem -= templv_RetrieveVirtualItem;
                    templv.BeginUpdate();
                    templv.AutoScrollOffset = new Point(0, 0);
                    ListItems.Clear();

                    templv.View = View.Details;

                    ListViewItem l;
                    for (int x = 0; x < meta.items.Count; x++)
                    {
                        Meta.Item i = meta.items[x];

                        if (i.type != type)
                        {
                            continue;
                        }

                        switch (i.type)
                        {
                            case Meta.ItemType.Reflexive:
                                Meta.Reflexive r = (Meta.Reflexive)i;
                                l = new ListViewItem(r.description);
                                l.Tag = x;

                                l.SubItems.Add(r.offset.ToString());
                                l.SubItems.Add(r.translation.ToString());
                                l.SubItems.Add(r.chunkcount.ToString());
                                l.SubItems.Add(r.chunksize.ToString());
                                l.SubItems.Add(r.pointstotagtype);
                                l.SubItems.Add(r.pointstotagname);
                                l.SubItems.Add(r.intagtype);
                                l.SubItems.Add(r.intagname);
                                ListItems.Add(l);
                                break;
                            case Meta.ItemType.Ident:
                                Meta.Ident id = (Meta.Ident)i;
                                l = new ListViewItem(id.description);
                                l.Tag = x;

                                l.SubItems.Add(id.offset.ToString());
                                l.SubItems.Add(id.pointstotagtype);
                                l.SubItems.Add(id.pointstotagname);
                                l.SubItems.Add(id.intagtype);
                                l.SubItems.Add(id.intagname);
                                ListItems.Add(l);
                                break;
                            case Meta.ItemType.String:
                                Meta.String s = (Meta.String)i;
                                l = new ListViewItem(s.description);
                                l.Tag = x;

                                l.SubItems.Add(s.offset.ToString());
                                l.SubItems.Add(s.name);
                                l.SubItems.Add(s.intagtype);
                                l.SubItems.Add(s.intagname);
                                ListItems.Add(l);
                                break;
                        }

                        // System.Windows.Forms.Layout.LayoutEngine
                        // Application.DoEvents();
                    }

                    templv.VirtualListSize = ListItems.Count;

                    templv.RetrieveVirtualItem += templv_RetrieveVirtualItem;
                    templv.EndUpdate();
                }
                catch (Exception exc)
                {
                    Global.ShowErrorMsg("Reference viewer, ListView population exception", exc);
                }
                Application.DoEvents();
            }

            #endregion

            // Hashtable curDirectory;
            #region Methods

            /// <summary>
            /// The add metas to tree view.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="tv">The tv.</param>
            /// <param name="mv">The mv.</param>
            /// <param name="checkbox">The checkbox.</param>
            /// <remarks></remarks>
            public void AddMetasToTreeView(Map map, TreeView tv, MetaView mv, bool checkbox)
            {
                switch (mv)
                {
                    case MetaView.TagTypeView:
                        AddMetaByTagTypeToTreeView(ref map, ref tv, false);
                        break;
                    case MetaView.FolderView:
                        AddMetaByFolderToTreeView(ref map, ref tv, checkbox);
                        break;
                    case MetaView.InfoView:
                        AddMetaByInfoToTreeView(ref map, ref tv, checkbox);
                        break;
                }
            }

            /// <summary>
            /// The add meta by folder to tree view.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="tv">The tv.</param>
            /// <param name="checkbox">The checkbox.</param>
            /// <remarks></remarks>
            private void AddMetaByFolderToTreeView(ref Map map, ref TreeView tv, bool checkbox)
            {
                string[] steps;
                string step;
                Hashtable currentDir;
                TreeNode currentNode;
                string path;

                // Clear current treeview
                currentDir = root;
                currentDir.Clear();
                tv.Nodes.Clear();
                tv.Sorted = false;
                tv.Nodes.Add(FolderNode(map.filePath, string.Empty));
                tv.Nodes[0].Checked = true;
                if (checkbox)
                {
                    tv.CheckBoxes = true;
                }

                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    string temp = map.FileNames.Name[x];
                    steps = temp.Split('\\');
                    currentDir = root;
                    currentNode = tv.Nodes[0];
                    path = string.Empty;

                    for (int i = 0; i < (steps.Length - 1); i++)
                    {
                        step = steps[i];
                        path = path + step + "\\";
                        if (!currentDir.ContainsKey(step))
                        {
                            currentDir.Add(step, new Hashtable());
                            TreeNode tempx = FolderNode(step, path);
                            tempx.Checked = checkbox;
                            currentNode.Nodes.Add(tempx);
                        }

                        currentDir = (Hashtable)currentDir[step];
                        currentNode = FindNode(currentNode, path);
                    }

                    currentDir.Add(steps[steps.GetUpperBound(0)] + "." + map.MetaInfo.TagType[x], x);
                    TreeNode tn = FolderNode(
                        steps[steps.GetUpperBound(0)] + "." + map.MetaInfo.TagType[x],
                        map.FileNames.Name[x] + "." + map.MetaInfo.TagType[x]);
                    tn.Checked = checkbox;
                    currentNode.Nodes.Add(tn);

                    // MessageBox.Show(path);
                }

                tv.Nodes[0].Expand();
                tv.Sorted = true;
            }

            /// <summary>
            /// The add meta by info to tree view.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="tv">The tv.</param>
            /// <param name="checkbox">The checkbox.</param>
            /// <remarks></remarks>
            private void AddMetaByInfoToTreeView(ref Map map, ref TreeView tv, bool checkbox)
            {
                tv.Nodes.Clear();
                tv.Sorted = false;
                Hashtable temptable = new Hashtable();
                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    tv.Nodes.Add(
                        map.MetaInfo.Offset[x].ToString().PadLeft(10) + ".[" + map.MetaInfo.TagType[x] + "]." +
                        map.FileNames.Name[x]);
                    tv.Nodes[x].Tag = map.FileNames.Name[x] + "." + map.MetaInfo.TagType[x];

                    // string[] tempn = map.FileNames.Name[x].Split('\\');

                    // Add a node for the '+' to show up
                    TreeNode tn = new TreeNode(tv.Name);
                    tn.Tag = tv.Name;
                    tn.Checked = checkbox;
                    tv.Nodes[x].Nodes.Add(tn);
                }

                tv.Nodes.Insert(0, "   0000000 <HEADER>");
                if (checkbox)
                {
                    tv.Sorted = true;
                }
                else
                {
                    tv.Sorted = false;
                }
            }

            /// <summary>
            /// The add meta by tag type to tree view.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="tv">The tv.</param>
            /// <param name="checkbox">The checkbox.</param>
            /// <remarks></remarks>
            private void AddMetaByTagTypeToTreeView(ref Map map, ref TreeView tv, bool checkbox)
            {
                tv.Nodes.Clear();
                TreeNodeCollection accessNodes = tv.Nodes;

                tv.Nodes.Add(" <ALL TAGS> ");
                accessNodes = tv.Nodes[0].Nodes;

                tv.Sorted = false;
                if (checkbox)
                {
                    tv.CheckBoxes = true;
                }

                Hashtable temptableQuick = new Hashtable();
                Hashtable temptableMain = new Hashtable();
                IEnumerator i = map.MetaInfo.TagTypes.Keys.GetEnumerator();
                while (i.MoveNext())
                {
                    temptableMain.Add(i.Current, accessNodes.Count);
                    accessNodes.Add((string)i.Current, (string)i.Current);

                    // quick access tagtypes
                    for (int a = 0; a < Prefs.QuickAccessTagTypes.Count; a++)
                    {
                        if (Prefs.QuickAccessTagTypes[a].TagType == (string)i.Current)
                        {
                            temptableQuick.Add(i.Current, tv.Nodes.Count);
                            tv.Nodes.Add((string)i.Current, (string)i.Current);
                            break;
                        }
                    }
                }

                for (int x = 0; x < map.IndexHeader.metaCount; x++)
                {
                    TreeNode tn = new TreeNode(map.FileNames.Name[x]);
                    tn.Name = tn.Text;
                    tn.Checked = checkbox;
                    object tempobj = temptableMain[map.MetaInfo.TagType[x]];
                    accessNodes[(int)tempobj].Nodes.Add(tn);

                    tempobj = temptableQuick[map.MetaInfo.TagType[x]];
                    if (tempobj != null)
                    {
                        Prefs.QuickAccessTagType quickAccess = Prefs.GetQuickAccessTagType(map.MetaInfo.TagType[x]);
                        for (int a = 0; a < quickAccess.TagPaths.Count; a++)
                        {
                            if (quickAccess.TagPaths[a] == map.FileNames.Name[x])
                            {
                                tv.Nodes[(int)tempobj].Nodes.Add((TreeNode)tn.Clone());
                                break;
                            }
                        }
                    }
                }

                tv.Sorted = true;
            }

            // Recursively adds nodes to Chunk Tree Listing
            /// <summary>
            /// The display split.
            /// </summary>
            /// <param name="reflex">The reflex.</param>
            /// <param name="tn">The tn.</param>
            /// <remarks></remarks>
            private void DisplaySplit(MetaSplitter.SplitReflexive reflex, TreeNode tn)
            {
                for (int x = 0; x < reflex.Chunks.Count; x++)
                {
                    string tempchunk = reflex.description;

                    foreach (Meta.Item mi in reflex.Chunks[x].ChunkResources)
                    {
                        if (mi.description == reflex.label)
                        {
                            tempchunk = ((Meta.String)mi).name;
                            break;
                        }

                        if (mi.type == Meta.ItemType.String)
                        {
                            tempchunk = ((Meta.String)mi).name;
                        }

                        if (mi.type == Meta.ItemType.Ident)
                        {
                            tempchunk = ((Meta.Ident)mi).pointstotagname;
                            string[] splits = tempchunk.Split('\\');
                            tempchunk = splits[splits.Length - 1] + "." + ((Meta.Ident)mi).pointstotagtype;
                            break;
                        }
                    }

                    string tempchunkname = "Chunk - Number: " + x + " - Name: " + tempchunk + " - Size: " +
                                           reflex.Chunks[x].chunksize;

                    TreeNode ChunkNumberNode = new TreeNode(tempchunkname);
                    MetaSplitter.SplitReflexive split = reflex.Chunks[x];
                    for (int y = 0; y < split.ChunkResources.Count; y++)
                    {
                        if (split.ChunkResources[y].type == Meta.ItemType.Reflexive)
                        {
                            TreeNode reflexnode =
                                new TreeNode(
                                    "Reflexive - Name: " + split.ChunkResources[y].description + " - Size: " +
                                    ((MetaSplitter.SplitReflexive)split.ChunkResources[y]).chunksize + " - Offset: " +
                                    split.ChunkResources[y].offset);
                            DisplaySplit((MetaSplitter.SplitReflexive)split.ChunkResources[y], reflexnode);
                            ChunkNumberNode.Nodes.Add(reflexnode);
                        }
                    }

                    tn.Nodes.Add(ChunkNumberNode);
                }
            }

            /// <summary>
            /// The find node.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            private TreeNode FindNode(TreeNode root, string path)
            {
                for (int i = 0; i < root.Nodes.Count; i++)
                {
                    if ((string)root.Nodes[i].Tag == path)
                    {
                        return root.Nodes[i];
                    }

                    if (path.StartsWith((string)root.Nodes[i].Tag))
                    {
                        return FindNode(root.Nodes[i], path);
                    }
                }

                return root;
            }

            /// <summary>
            /// The folder node.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            private TreeNode FolderNode(string text, string path)
            {
                TreeNode node = new TreeNode(text);
                node.Tag = path;
                return node;
            }

            /// <summary>
            /// The metalist_ retrieve virtual item.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            private void metalist_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
            {
                try
                {
                    e.Item = ListItems[e.ItemIndex];
                }
                catch (Exception ex)
                {
                    Global.ShowErrorMsg("Retrieve Virtual Item Exception, " + e.ItemIndex + " / " + ListItems.Count, ex);
                }
            }

            /// <summary>
            /// The templv_ retrieve virtual item.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            private void templv_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
            {
                try
                {
                    e.Item = ListItems[e.ItemIndex];
                }
                catch (Exception ex)
                {
                    Global.ShowErrorMsg("Retrieve Virtual Item Exception, " + e.ItemIndex + " / " + ListItems.Count, ex);
                }
            }

            #endregion
        }

        #region Handles Undock / Dock events for editors
        /// <summary>
        /// Undocks the current editor mode from the window
        /// </summary>
        /// <param name="sender">The control to be undocked</param>
        /// <param name="e">unused</param>
        private void btnUndock_Click(object sender, EventArgs e)
        {
            Form floatForm = new Form();
            floatForm.Tag = this.getEditorMode();
            Control c = this.splitContainer2.Panel1.Controls[0];
            this.setEditorMode(MapForm.EditorModes.LastMode);
            floatForm.Owner = this;
            floatForm.Text = floatForm.Tag.ToString();
            floatForm.Size = c.Size;
            floatForm.Controls.Add(c);
            // When activating our .LastMode above, c gets un-visibled
            c.Visible = true;
            floatForm.FormClosing += new FormClosingEventHandler(floatForm_FormClosing);
            floatForm.Show();
            if (this.splitContainer2.Panel1.Controls.Count == 0)
                this.btnUndock.Enabled = false;
        }

        /// <summary>
        /// Handles the close button on the Floating Forms
        /// </summary>
        /// <param name="sender">The control to be docked</param>
        /// <param name="e">unused</param>
        void floatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Control c = ((Form)sender).Controls[0];
            this.splitContainer2.Panel1.Controls.Add(c);
            this.setEditorMode((EditorModes)((Form)sender).Tag);
            if (this.splitContainer2.Panel1.Controls.Count > 0)
                this.btnUndock.Enabled = true;
        }
        #endregion

        public static Control FindFocusedControl(Control control)
        { 
            var container = control as ContainerControl; 
            while (container != null)
            { 
                control = container.ActiveControl; 
                container = control as ContainerControl; 
            } 
            return control; 
        } 

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    setEditorMode(EditorModes.ReferenceEditor);
                    return true;
                case Keys.F2:
                    setEditorMode(EditorModes.MetaEditor2);
                    return true;
                case Keys.F3:
                    setEditorMode(EditorModes.MetaEditor1);
                    return true;
                case Keys.F4:
                    setEditorMode(EditorModes.HexViewer);
                    return true;
                case Keys.F5:
                    setEditorMode(EditorModes.BitmapViewer);
                    return true;
                default:
                    // Call the base class
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        } 

        private void MapForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            Control c = FindFocusedControl(this);
            if (c != null)
            {
                switch (c.GetType().ToString())
                {
                    case "System.Windows.Forms.ComboBox":
                    case "System.Windows.Forms.ListBox":
                    case "System.Windows.Forms.TextBox":
                        return;
                }
            }
            switch (e.KeyChar)
            {
                case (char)49:
                    setEditorMode(EditorModes.ReferenceEditor);
                    break;
                case (char)50:
                    setEditorMode(EditorModes.MetaEditor2);
                    break;
                case (char)51:
                    setEditorMode(EditorModes.MetaEditor1);
                    break;
                case (char)52:
                    setEditorMode(EditorModes.HexViewer);
                    break;
                case (char)53:
                    setEditorMode(EditorModes.BitmapViewer);
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (map.SelectedMeta.type != "bitm")
            {
                timer1.Stop();
                return;
            }

            ParsedBitmap pm = new ParsedBitmap(ref map.SelectedMeta, map);
            if (pm.Properties[0].typename != ParsedBitmap.BitmapType.BITM_TYPE_3D)
            {
                timer1.Stop();
                return;
            }
            Bitmap b = pm.FindChunkAndDecode(0, 0, 0, ref map.SelectedMeta, map, bitmapCount++, 0);
            pictureBox1.Image = b;
            if (bitmapCount > pm.Properties[0].depth)
                bitmapCount = 1;

        }        
    }
}