// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaEditor.cs" company="">
//   
// </copyright>
// <summary>
//   The meta editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using global::MetaEditor.Components;
    using global::MetaEditor.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The meta editor.
    /// </summary>
    /// <remarks></remarks>
    public partial class MetaEditor : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The show bitmask 16 s.
        /// </summary>
        public static bool ShowBitmask16s = true;

        /// <summary>
        /// The show bitmask 32 s.
        /// </summary>
        public static bool ShowBitmask32s = true;

        /// <summary>
        /// The show bitmask 8 s.
        /// </summary>
        public static bool ShowBitmask8s = true;

        /// <summary>
        /// The show block index 16 s.
        /// </summary>
        public static bool ShowBlockIndex16s = true;

        /// <summary>
        /// The show block index 32 s.
        /// </summary>
        public static bool ShowBlockIndex32s = true;

        /// <summary>
        /// The show block index 8 s.
        /// </summary>
        public static bool ShowBlockIndex8s = true;

        /// <summary>
        /// The show bytes.
        /// </summary>
        public static bool ShowBytes = true;

        /// <summary>
        /// The show enum 16 s.
        /// </summary>
        public static bool ShowEnum16s = true;

        /// <summary>
        /// The show enum 32 s.
        /// </summary>
        public static bool ShowEnum32s = true;

        /// <summary>
        /// The show enum 8 s.
        /// </summary>
        public static bool ShowEnum8s = true;

        /// <summary>
        /// The show floats.
        /// </summary>
        public static bool ShowFloats = true;

        /// <summary>
        /// The show idents.
        /// </summary>
        public static bool ShowIdents = true;

        /// <summary>
        /// The show ints.
        /// </summary>
        public static bool ShowInts = true;

        /// <summary>
        /// The show invisibles.
        /// </summary>
        public static bool ShowInvisibles;

        /// <summary>
        /// The show reflexives.
        /// </summary>
        public static bool ShowReflexives = true;

        /// <summary>
        /// The show si ds.
        /// </summary>
        public static bool ShowSIDs = true;

        /// <summary>
        /// The show shorts.
        /// </summary>
        public static bool ShowShorts = true;

        /// <summary>
        /// The show string 256 s.
        /// </summary>
        public static bool ShowString256s = true;

        /// <summary>
        /// The show string 32 s.
        /// </summary>
        public static bool ShowString32s = true;

        /// <summary>
        /// The show uints.
        /// </summary>
        public static bool ShowUints = true;

        /// <summary>
        /// The show undefineds.
        /// </summary>
        public static bool ShowUndefineds = true;

        /// <summary>
        /// The show unicode string 256 s.
        /// </summary>
        public static bool ShowUnicodeString256s = true;

        /// <summary>
        /// The show unicode string 64 s.
        /// </summary>
        public static bool ShowUnicodeString64s = true;

        /// <summary>
        /// The show ushorts.
        /// </summary>
        public static bool ShowUshorts = true;

        /// <summary>
        /// The _ paint.
        /// </summary>
        public static bool _Paint = true;

        /// <summary>
        /// The auto save.
        /// </summary>
        public bool AutoSave;

        /// <summary>
        /// The ifp.
        /// </summary>
        public IFPIO ifp;

        /// <summary>
        /// The map index.
        /// </summary>
        public Map map;

        /// <summary>
        /// The plugin name.
        /// </summary>
        public string pluginName;

        /// <summary>
        /// The selected tag type.
        /// </summary>
        public string selectedTagType;

        /// <summary>
        /// The string swap.
        /// </summary>
        public MEStringsSelector stringSwap;

        /// <summary>
        /// The w m_ paint.
        /// </summary>
        private const short WM_PAINT = 0x00f;

        /// <summary>
        /// The tool tip 1.
        /// </summary>
        private readonly ToolTip ToolTip1 = new ToolTip();

        /// <summary>
        /// The info text.
        /// </summary>
        private readonly List<string> infoText = new List<string>();

        /// <summary>
        /// The me height.
        /// </summary>
        //private int MEHeight = 480;

        /// <summary>
        /// The alpha control.
        /// </summary>
        private DataValues alphaControl;

        /// <summary>
        /// The blue control.
        /// </summary>
        private DataValues blueControl;

        /// <summary>
        /// The green control.
        /// </summary>
        private DataValues greenControl;

        /// <summary>
        /// The red control.
        /// </summary>
        private DataValues redControl;

        /// <summary>
        /// The tab index.
        /// </summary>
        private int tabIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaEditor"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MetaEditor()
        {
            InitializeComponent();
            ShiftColors.ReadPlugin();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The ident loader_ drop down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void IdentLoader_DropDown(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.AddRange(map.FileNames.Name);
        }

        /// <summary>
        /// The ident loader_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void IdentLoader_DropDownClosed(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();
        }

        /// <summary>
        /// The reload meta for same tag type.
        /// </summary>
        /// <param name="reloadreflexive">The reloadreflexive.</param>
        /// <remarks></remarks>
        public void ReloadMetaForSameTagType(bool reloadreflexive)
        {
            map.OpenMap(MapTypes.Internal);

            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                string s = this.Controls[0].Controls[counter].GetType().ToString();
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            if (reloadreflexive == false)
                            {
                                break;
                            }

                            ((ReflexiveControl)this.Controls[0].Controls[counter]).LoadMetaIntoControls(
                                map.SelectedMeta.offset, map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.Float":
                        {
                            ((Float)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Populate(map.SelectedMeta.offset);
                            break;
                        }
                }

                if (this.Controls[0].Controls[counter] is BaseField)
                {
                    ToolTip1.SetToolTip(
                        this.Controls[0].Controls[counter], 
                        "offset: " + ((BaseField)(this.Controls[0].Controls[counter])).offsetInMap.ToString("x"));
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The save button_ drop down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void SaveButton_DropDown(object sender, EventArgs e)
        {
            this.SaveRecursively();
            this.ReloadMetaForSameTagType(false);
        }

        /// <summary>
        /// The load controls.
        /// </summary>
        /// <param name="map">The map number.</param>
        /// <remarks></remarks>
        public void loadControls(Map map)
        {
            this.map = map;
            if (map.SelectedMeta == null)
            {
                return;
            }

            this.setInfoText("Loading Tag");
            ToolTip1.InitialDelay = 800;
            _Paint = false;
            ReflexiveControl.ME = this;

            this.Controls.Clear();

            // Thanks to RapiD for pointing this fix out! If you just clear them, they stay in memory!
            // UPDATE: was using FOREACH loop before, but due to the method, it would only remove half the controls
            //    leaving a memory leak here!
            for (int count = 0; count < this.panel1.Controls.Count; count++)
            {
                this.panel1.Controls[count].Dispose();
            }

            this.panel1.Controls.Clear();
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.panel1.Top = this.Padding.Top + this.toolStrip1.Height;
            this.panel1.Height = this.Height -
                                 (this.Padding.Top + this.Padding.Bottom + this.toolStrip1.Height +
                                  this.statusStrip1.Height);
            this.Controls[0].Hide();
            this.Refresh();
            this.SuspendLayout();
            ifp = IFPHashMap.GetIfp(map.SelectedMeta.type, map.HaloVersion);
            LoadENTControls(ifp.items);
            this.ResumeLayout(true);
            this.Controls[0].Show();
            _Paint = true;
            this.ReloadMetaForSameTagType(true);
            this.restoreInfoText();
        }

        /// <summary>
        /// The restore info text.
        /// </summary>
        /// <remarks></remarks>
        public void restoreInfoText()
        {
            if (infoText.Count > 0)
            {
                toolStripInformation.Text = infoText[infoText.Count - 1];
                infoText.RemoveAt(infoText.Count - 1);
            }
            else
            {
                toolStripInformation.Text = ".: Idle :.";
            }

            if (toolStripInformation.Text == ".: Idle :.")
            {
                toolStripInformation.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// The set info text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public void setInfoText(string text)
        {
            Point temp = ((Panel)this.Controls[0]).AutoScrollPosition;
            infoText.Add(toolStripInformation.Text);
            toolStripInformation.ForeColor = Color.Red;
            toolStripInformation.Text = ".: " + text + " :.";
            ((Panel)this.Controls[0]).AutoScrollPosition = new Point(-temp.X, -temp.Y);
            Application.DoEvents();
        }

        /// <summary>
        /// The set reflexive text.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="Current">The current.</param>
        /// <param name="Total">The total.</param>
        /// <remarks></remarks>
        public void setReflexiveText(string Name, int Current, int Total)
        {
            int y = this.panel1.AutoScrollPosition.Y;
            this.toolStripReflexiveNumber.Text = Name + ": (" + Current.ToString() + " / " + Total.ToString() + ")";
            this.panel1.AutoScrollPosition = new Point(0, -y);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        protected override void WndProc(ref Message m)
        {
            // Code courtesy of Mark Mihevc
            // sometimes we want to eat the paint message so we don't have to see all the
            // flicker from when we select the text to change the color.

            if (m.Msg == WM_PAINT)
            {
                if (_Paint)
                {
                    base.WndProc(ref m); // if we decided to paint this control, just call the RichTextBox WndProc
                }
                else
                {
                    m.Result = IntPtr.Zero;
                }

                // not painting, must set this to IntPtr.Zero if not painting otherwise serious problems.
            }
            else
            {
                base.WndProc(ref m); // message other than WM_PAINT, jsut do what you normally do.
            }
        }

        /// <summary>
        /// The load ent controls.
        /// </summary>
        /// <param name="entArray">The ent array.</param>
        /// <remarks></remarks>
        private void LoadENTControls(object[] entArray)
        {
            this.selectedTagType = map.SelectedMeta.type;

            this.toolStripTagType.Text = "[" + this.selectedTagType + "]";
            this.toolStripTagName.Text = map.SelectedMeta.name;

            // this.Padding = new Padding(10);
            int colorSpaceCount = 4;

            // Custom Plugins access
            //ra = new RegistryAccess(
            //    Registry.CurrentUser, 
            //    RegistryAccess.RegPaths.Halo2CustomPlugins + pluginName + "\\" + this.selectedTagType);
            //if (pluginName == null)
            //{
            //    ra.CloseReg();
            //}

            if (entArray != null)
                foreach (object o in entArray)
                {
                    IFPIO.BaseObject tempbase = (IFPIO.BaseObject)o;
                    if (tempbase.visible == false)
                    {
                        if (ShowInvisibles == false)
                        {
                            continue;
                        }
                    }

                    // skip hidden custom plugins variables (mark reflexives to be removed if empty)
                    bool skipEmptyReflex = false;
                    //if (ra.isOpen && ra.getValue(tempbase.offset.ToString()) == bool.FalseString)
                    //{
                    //    if (tempbase.ObjectType == IFPIO.ObjectEnum.Struct)
                    //    {
                    //        skipEmptyReflex = true;
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }
                    //}

                    switch (tempbase.ObjectType)
                    {
                        case IFPIO.ObjectEnum.Struct:
                            {
                                if (ShowReflexives == false)
                                {
                                    break;
                                }

                                // tempLabel is a blank space located above reflexives
                                Label tempLabel = new Label();
                                tempLabel.AutoSize = true;
                                tempLabel.Location = new Point(0, 0);
                                tempLabel.Name = "label1";
                                tempLabel.Dock = DockStyle.Top;
                                tempLabel.Size = new Size(35, 13);
                                tempLabel.TabIndex = tabIndex;

                                // tempReflexive is the reflexive and all data (incl other reflexives) within it
                                ReflexiveControl tempReflexive = new ReflexiveControl(
                                    map,
                                    map.SelectedMeta.offset,
                                    ((IFPIO.Reflexive)tempbase).HasCount,
                                    tempbase.lineNumber,
                                    this);

                                // tempReflexive.Location = new System.Drawing.Point(10, 0);
                                tempReflexive.Name = "reflexive";
                                tempReflexive.TabIndex = tabIndex;
                                tempReflexive.LoadENTControls(
                                    (IFPIO.Reflexive)tempbase,
                                    ((IFPIO.Reflexive)tempbase).items,
                                    true,
                                    0,
                                    ref tabIndex,
                                    tempbase.offset.ToString());

                                // Label, Combobox & Button are always added ( = 3)
                                if (!(tempReflexive.Controls.Count <= 2 && skipEmptyReflex))
                                {
                                    this.Controls[0].Controls.Add(tempLabel);
                                    tempLabel.BringToFront();
                                    this.Controls[0].Controls.Add(tempReflexive);
                                    tempReflexive.BringToFront();
                                }

                                break;
                            }

                        case IFPIO.ObjectEnum.Ident:
                            {
                                if (ShowIdents == false)
                                {
                                    break;
                                }

                                Ident tempident = new Ident(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.Ident)tempbase).hasTagType,
                                    tempbase.lineNumber);
                                tempident.Name = "ident";
                                tempident.TabIndex = tabIndex;
                                tempident.Populate(map.SelectedMeta.offset);
                                tempident.Tag = "[" + tempident.Controls[2].Text + "] " + tempident.Controls[1].Text;
                                tempident.Controls[1].ContextMenuStrip = identContext;
                                this.Controls[0].Controls.Add(tempident);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.StringID:
                            {
                                if (ShowSIDs == false)
                                {
                                    break;
                                }

                                SID tempSID = new SID(tempbase.name, map, tempbase.offset, tempbase.lineNumber);
                                tempSID.Name = "sid";
                                tempSID.TabIndex = tabIndex;
                                tempSID.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempSID);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Float:
                            {
                                if (ShowFloats == false)
                                {
                                    break;
                                }

                                DataValues tempFloat = new DataValues(
                                    tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Float, tempbase.lineNumber);
                                tempFloat.TabIndex = tabIndex;
                                tempFloat.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempFloat);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.String32:
                            {
                                if (ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String32)
                                {
                                    break;
                                }

                                EntStrings tempstring = new EntStrings(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.IFPString)tempbase).size,
                                    ((IFPIO.IFPString)tempbase).type,
                                    tempbase.lineNumber);
                                tempstring.Name = "string";
                                tempstring.TabIndex = tabIndex;
                                tempstring.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempstring);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.UnicodeString256:
                            {
                                if (ShowUnicodeString256s == false)
                                {
                                    break;
                                }

                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.String256:
                            {
                                if (ShowString256s == false)
                                {
                                    break;
                                }

                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.UnicodeString64:
                            {
                                if (ShowUnicodeString64s == false)
                                {
                                    break;
                                }

                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.String:
                            {
                                if (ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String32)
                                {
                                    break;
                                }

                                EntStrings tempstring = new EntStrings(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.IFPString)tempbase).size,
                                    ((IFPIO.IFPString)tempbase).type,
                                    tempbase.lineNumber);
                                tempstring.Name = "string";
                                tempstring.TabIndex = tabIndex;
                                tempstring.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempstring);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Int:
                            {
                                if (((IFPIO.IFPInt)tempbase).entIndex.nulled)
                                {
                                    if ((ShowInts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Int) ||
                                        (ShowShorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Short) ||
                                        (ShowUshorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.UShort) ||
                                        (ShowUints == false && tempbase.ObjectType == IFPIO.ObjectEnum.UInt))
                                    {
                                        break;
                                    }

                                    DataValues tempdatavalues = new DataValues(
                                        tempbase.name, map, tempbase.offset, tempbase.ObjectType, tempbase.lineNumber);
                                    tempdatavalues.TabIndex = tabIndex;
                                    tempdatavalues.Populate(map.SelectedMeta.offset);
                                    this.Controls[0].Controls.Add(tempdatavalues);
                                    this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                }
                                else
                                {
                                    if ((ShowBlockIndex32s == false &&
                                         (tempbase.ObjectType == IFPIO.ObjectEnum.Int |
                                          tempbase.ObjectType == IFPIO.ObjectEnum.UInt)) ||
                                        (ShowBlockIndex16s == false &&
                                         (tempbase.ObjectType == IFPIO.ObjectEnum.Short |
                                          tempbase.ObjectType == IFPIO.ObjectEnum.UShort)) ||
                                        (ShowBlockIndex8s == false && tempbase.ObjectType == IFPIO.ObjectEnum.Byte))
                                    {
                                        break;
                                    }

                                    Indices tempdatavalues = new Indices(
                                        tempbase.name,
                                        map,
                                        tempbase.offset,
                                        tempbase.ObjectType,
                                        ((IFPIO.IFPInt)tempbase).entIndex);
                                    tempdatavalues.TabIndex = tabIndex;
                                    this.Controls[0].Controls.Add(tempdatavalues);
                                    this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                }

                                break;
                            }

                        case IFPIO.ObjectEnum.Short:
                            {
                                goto case IFPIO.ObjectEnum.Int;
                            }

                        case IFPIO.ObjectEnum.UShort:
                            {
                                goto case IFPIO.ObjectEnum.Int;
                            }

                        case IFPIO.ObjectEnum.UInt:
                            {
                                goto case IFPIO.ObjectEnum.Int;
                            }

                        case IFPIO.ObjectEnum.Unknown:
                            {
                                if (ShowUndefineds == false)
                                {
                                    break;
                                }

                                DataValues tempUnknown = new DataValues(
                                    tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Unknown, tempbase.lineNumber);
                                tempUnknown.TabIndex = tabIndex;
                                tempUnknown.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempUnknown);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Byte_Flags:
                            {
                                if (ShowBitmask8s == false)
                                {
                                    break;
                                }

                                Bitmask tempbitmask = new Bitmask(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.Bitmask)tempbase).bitmaskSize,
                                    ((IFPIO.Bitmask)tempbase).options,
                                    tempbase.lineNumber);
                                tempbitmask.TabIndex = tabIndex;
                                tempbitmask.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempbitmask);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Word_Flags:
                            {
                                if (ShowBitmask16s == false)
                                {
                                    break;
                                }

                                Bitmask tempbitmask = new Bitmask(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.Bitmask)tempbase).bitmaskSize,
                                    ((IFPIO.Bitmask)tempbase).options,
                                    tempbase.lineNumber);
                                tempbitmask.TabIndex = tabIndex;
                                tempbitmask.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempbitmask);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Long_Flags:
                            {
                                if (ShowBitmask32s == false)
                                {
                                    break;
                                }

                                Bitmask tempbitmask = new Bitmask(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.Bitmask)tempbase).bitmaskSize,
                                    ((IFPIO.Bitmask)tempbase).options,
                                    tempbase.lineNumber);
                                tempbitmask.TabIndex = tabIndex;
                                tempbitmask.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempbitmask);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Char_Enum:
                            {
                                if (ShowEnum8s == false)
                                {
                                    break;
                                }

                                Enums tempenum = new Enums(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.IFPEnum)tempbase).enumSize,
                                    ((IFPIO.IFPEnum)tempbase).options,
                                    tempbase.lineNumber);
                                tempenum.TabIndex = tabIndex;
                                tempenum.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempenum);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Enum:
                            {
                                if (ShowEnum16s == false)
                                {
                                    break;
                                }

                                Enums tempenum = new Enums(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.IFPEnum)tempbase).enumSize,
                                    ((IFPIO.IFPEnum)tempbase).options,
                                    tempbase.lineNumber);
                                tempenum.TabIndex = tabIndex;
                                tempenum.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempenum);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Long_Enum:
                            {
                                if (ShowEnum32s == false)
                                {
                                    break;
                                }

                                Enums tempenum = new Enums(
                                    tempbase.name,
                                    map,
                                    tempbase.offset,
                                    ((IFPIO.IFPEnum)tempbase).enumSize,
                                    ((IFPIO.IFPEnum)tempbase).options,
                                    tempbase.lineNumber);
                                tempenum.TabIndex = tabIndex;
                                tempenum.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempenum);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.Byte:
                            {
                                if (((IFPIO.IFPByte)tempbase).entIndex.nulled)
                                {
                                    if (ShowBytes == false)
                                    {
                                        break;
                                    }

                                    DataValues tempByte = new DataValues(
                                        tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Byte, tempbase.lineNumber);
                                    tempByte.TabIndex = tabIndex;
                                    this.Controls[0].Controls.Add(tempByte);
                                    this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                }
                                else
                                {
                                    if (ShowBlockIndex8s == false)
                                    {
                                        break;
                                    }

                                    Indices tempdatavalues = new Indices(
                                        tempbase.name,
                                        map,
                                        tempbase.offset,
                                        tempbase.ObjectType,
                                        ((IFPIO.IFPByte)tempbase).entIndex);
                                    tempdatavalues.TabIndex = tabIndex;
                                    this.Controls[0].Controls.Add(tempdatavalues);
                                    this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                }

                                break;
                            }

                        case IFPIO.ObjectEnum.Unused:
                            {
                                DataValues tempUnknown = new DataValues(
                                    tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Unused, tempbase.lineNumber);
                                tempUnknown.TabIndex = tabIndex;
                                tempUnknown.Populate(map.SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempUnknown);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                break;
                            }

                        case IFPIO.ObjectEnum.TagType:
                            continue;
                    }

                    if (!(tempbase is IFPIO.Reflexive))
                    {
                        ToolTip1.SetToolTip(this.Controls[0].Controls[0].Controls[0], "offset: " + tempbase.offset);
                    }

                    if (this.Controls[0].Controls.Count > 0 && this.Controls[0].Controls[0] is DataValues)
                    {
                        // if (((tempbase.name.ToLower().Contains(" a") & tempbase.name[tempbase.name.ToLower().IndexOf(" a")]) ||
                        // tempbase.name.ToLower().Contains("alpha"))& alphaControl == null)
                        if (ColorWheel.checkForColor(tempbase.name, alphaControl, " a", "alpha"))
                        {
                            alphaControl = (DataValues)this.Controls[0].Controls[0];
                            colorSpaceCount = 0;
                        }

                            // if (tempbase.name.ToLower().Contains(" r") & redControl == null)
                        else if (ColorWheel.checkForColor(tempbase.name, redControl, " r", "red"))
                        {
                            redControl = (DataValues)this.Controls[0].Controls[0];
                            colorSpaceCount = 0;
                        }

                            // if (tempbase.name.ToLower().Contains(" g") & greenControl == null)
                        else if (ColorWheel.checkForColor(tempbase.name, greenControl, " g", "green"))
                        {
                            greenControl = (DataValues)this.Controls[0].Controls[0];
                            colorSpaceCount = 0;
                        }

                            // if (tempbase.name.ToLower().Contains(" b") & blueControl == null)
                        else if (ColorWheel.checkForColor(tempbase.name, blueControl, " b", "blue"))
                        {
                            blueControl = (DataValues)this.Controls[0].Controls[0];
                            colorSpaceCount = 0;
                        }
                        else
                        {
                            colorSpaceCount++;
                            if (colorSpaceCount == 1)
                            {
                                alphaControl = null;
                                redControl = null;
                                greenControl = null;
                                blueControl = null;
                            }
                        }

                        if (redControl != null & greenControl != null & blueControl != null)
                        {
                            // Create the new ColorWheel class, indicating
                            // the locations of the color wheel itself, the
                            // brightness area, and the position of the selected color.
                            ColorWheel cw = new ColorWheel();

                            if (alphaControl != null)
                            {
                                cw.setTextBox(alphaControl.textBox1, Color.White);
                            }

                            cw.setTextBox(redControl.textBox1, Color.Red);
                            cw.setTextBox(greenControl.textBox1, Color.Green);
                            cw.setTextBox(blueControl.textBox1, Color.Blue);

                            // p.I.AddRange(new Rectangle[] { SelectedColorRectangle });
                            cw.Dock = DockStyle.Top;
                            this.Controls[0].Controls.Add(cw);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();

                            // Reset for next batch
                            colorSpaceCount++;
                            alphaControl = null;
                            redControl = null;
                            greenControl = null;
                            blueControl = null;
                        }
                    }
                    else
                    {
                        colorSpaceCount++;
                    }

                    tabIndex++;
                }

            //ra.CloseReg();
        }

        /// <summary>
        /// The meta editor_ size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MetaEditor_SizeChanged(object sender, EventArgs e)
        {
            int totalSize = this.Padding.Top + this.Padding.Bottom;
            int temp = -1;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i] is Panel)
                {
                    temp = i;
                }
                else
                {
                    totalSize += this.Controls[i].Height;
                }
            }

            this.panel1.Height = this.Height - totalSize - 2;
            this.panel1.Top = this.Padding.Top + this.toolStrip1.Height;
            this.panel1.Width = this.Width - (this.Padding.Left + this.Padding.Right + 10);
        }

        /// <summary>
        /// The poke recursively.
        /// </summary>
        /// <remarks></remarks>
        private void PokeRecursively()
        {
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).PokeRecursively();
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The save recursively.
        /// </summary>
        /// <remarks></remarks>
        private void SaveRecursively()
        {
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).SaveRecursively();
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Float":
                        {
                            ((Float)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The set focus recursively.
        /// </summary>
        /// <param name="LineToCheck">The line to check.</param>
        /// <remarks></remarks>
        private void SetFocusRecursively(int LineToCheck)
        {
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).SetFocusRecursively(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The butt ok_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttOk_Click(object sender, EventArgs e)
        {
            this.loadControls(map);
        }

        /// <summary>
        /// The copy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectedText != string.Empty)
            {
                Clipboard.SetDataObject(c.SelectedText);
            }
        }

        /// <summary>
        /// The cut tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectedText != string.Empty)
            {
                Clipboard.SetDataObject(c.SelectedText);
                c.Text = c.Text.Remove(c.SelectionStart, c.SelectionLength);
            }
        }

        /// <summary>
        /// The delete tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectionLength != 0)
            {
                c.Text = c.Text.Remove(c.SelectionStart, c.SelectionLength);
            }
        }

        /// <summary>
        /// The goto tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            Form currentMapForm = this.FindForm().MdiParent.ActiveMdiChild;

            // Sets the window title to the desired tag, then sets it back to what it was.
            // The window will pick up any passed tags and activate the tag.
            string temp = currentMapForm.Text;
            currentMapForm.Text = c.Parent.Tag.ToString();
            currentMapForm.Text = temp;
        }

        /// <summary>
        /// The ident context_ opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void identContext_Opening(object sender, CancelEventArgs e)
        {
            Control c = identContext.SourceControl;
            if (!c.Focused)
            {
                c.Focus();
            }

            string tag = string.Empty;
            if (c.Parent.Tag != null)
            {
                tag = c.Parent.Tag.ToString();
            }

            if (tag != string.Empty)
            {
                this.gotoToolStripMenuItem.Text = "Jump to \"" + tag + "\"";
                gotoToolStripMenuItem.Visible = true;
            }
            else
            {
                gotoToolStripMenuItem.Visible = false;
            }
        }

        /// <summary>
        /// The list ent items close.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void listEntItemsClose(object sender, EventArgs e)
        {
            this.SetFocusRecursively(((IFPIO.BaseObject)((Button)sender).Tag).lineNumber);
        }

        /// <summary>
        /// The paste tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (Clipboard.GetText() != string.Empty)
            {
                c.SelectedText = Clipboard.GetText();
            }
        }

        /// <summary>
        /// The tool strip button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (HaloMap.RealTimeHalo.RTH_Imports.IsConnected)
                this.PokeRecursively();
            else
                MessageBox.Show("No debug box initialized");
        }

        /// <summary>
        /// The tool strip button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                ListEntItems LEI = new ListEntItems(map, this.ifp);
                LEI.Controls[0].Click += listEntItemsClose;
                LEI.Show();
            }
            catch
            {
            }
        }

        /// <summary>
        /// The tool strip color shifting_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripColorShifting_Click(object sender, EventArgs e)
        {
            ColorShifting.ColorShifting CS = new ColorShifting.ColorShifting();
            CS.buttSave.Click += buttOk_Click;
            CS.Show();
        }

        /// <summary>
        /// The tool strip label 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            ToolStripButton tsButton = (ToolStripButton)sender;
            if (tsButton.Text != "[Auto-Save]")
            {
                this.AutoSave = true;
                tsButton.Text = "[Auto-Save]";
                tsButton.Owner.Items.Find("ButtonSave", false)[0].Enabled = false;
            }
            else
            {
                this.AutoSave = false;
                tsButton.Text = "[ Manual ]";
                tsButton.Owner.Items.Find("ButtonSave", false)[0].Enabled = true;
            }
        }

        /// <summary>
        /// The tool strip menu item 3_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Meta Editor Version 1.3" + Environment.NewLine + "Improvements: TroyMac1ure" + Environment.NewLine +
                "Lead Programer: TheTyckoMan" + Environment.NewLine + "Awesome Map Dlls: Pokecancer" +
                Environment.NewLine + "RTH dll: Shalted" + Environment.NewLine + "Lots of support: The Brok3n Halo Team");
        }

        /// <summary>
        /// The tool strip show all_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripShowAll_Click(object sender, EventArgs e)
        {
            ItemsToShow its = new ItemsToShow();
            its.buttOK.Click += this.buttOk_Click;
            its.Show();
        }

        #endregion
    }

    /// <summary>
    /// The shift colors.
    /// </summary>
    /// <remarks></remarks>
    public static class ShiftColors
    {
        #region Constants and Fields

        /// <summary>
        /// The blue to shift.
        /// </summary>
        public static int BlueToShift = 20;

        /// <summary>
        /// The green to shift.
        /// </summary>
        public static int GreenToShift = 20;

        /// <summary>
        /// The red to shift.
        /// </summary>
        public static int RedToShift = 20;

        /// <summary>
        /// The starting blue.
        /// </summary>
        public static int StartingBlue = 128;

        /// <summary>
        /// The starting green.
        /// </summary>
        public static int StartingGreen = 128;

        /// <summary>
        /// The starting red.
        /// </summary>
        public static int StartingRed = 128;

        #endregion

        #region Public Methods

        /// <summary>
        /// The read plugin.
        /// </summary>
        /// <remarks></remarks>
        public static void ReadPlugin()
        {
            string xmlpath = Global.StartupPath + "\\Meta Editor Settings.xml";
            if (!File.Exists(xmlpath))
            {
                return; // this will happen when we are in the designer
            }

            FileStream fs = new FileStream(xmlpath, FileMode.Open, FileAccess.ReadWrite);
            XmlTextReader xtr = new XmlTextReader(fs);
            while (xtr.Read())
            {
                switch (xtr.Name.ToLower())
                {
                    case "colors":
                        {
                            try
                            {
                                SetVariables(
                                    Convert.ToInt32(xtr.GetAttribute("StartingRed")), 
                                    Convert.ToInt32(xtr.GetAttribute("StartingBlue")), 
                                    Convert.ToInt32(xtr.GetAttribute("StartingGreen")), 
                                    Convert.ToInt32(xtr.GetAttribute("RedToShift")), 
                                    Convert.ToInt32(xtr.GetAttribute("BlueToShift")), 
                                    Convert.ToInt32(xtr.GetAttribute("GreenToShift")));
                            }
                            catch
                            {
                            }

                            break;
                        }
                }
            }

            xtr.Close();
            fs.Close();
        }

        /// <summary>
        /// The set variables.
        /// </summary>
        /// <param name="Stred">The stred.</param>
        /// <param name="Stblue">The stblue.</param>
        /// <param name="Stgreen">The stgreen.</param>
        /// <param name="Shred">The shred.</param>
        /// <param name="Shblue">The shblue.</param>
        /// <param name="Shgreen">The shgreen.</param>
        /// <remarks></remarks>
        public static void SetVariables(int Stred, int Stblue, int Stgreen, int Shred, int Shblue, int Shgreen)
        {
            if (Stred < 256 && Stred > -1)
            {
                StartingRed = Stred;
            }

            if (Stblue < 256 && Stblue > -1)
            {
                StartingBlue = Stblue;
            }

            if (Stgreen < 256 && Stgreen > -1)
            {
                StartingGreen = Stgreen;
            }

            RedToShift = Shred;
            BlueToShift = Shblue;
            GreenToShift = Shgreen;
        }

        /// <summary>
        /// The shift blue.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The shift blue.</returns>
        /// <remarks></remarks>
        public static int ShiftBlue(int layer)
        {
            return Shifter(layer, BlueToShift, StartingBlue);
        }

        /// <summary>
        /// The shift green.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The shift green.</returns>
        /// <remarks></remarks>
        public static int ShiftGreen(int layer)
        {
            return Shifter(layer, GreenToShift, StartingGreen);
        }

        /// <summary>
        /// The shift red.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The shift red.</returns>
        /// <remarks></remarks>
        public static int ShiftRed(int layer)
        {
            return Shifter(layer, RedToShift, StartingRed);
        }

        /// <summary>
        /// The shifter.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="tempshifter">The tempshifter.</param>
        /// <param name="tempStarting">The temp starting.</param>
        /// <returns>The shifter.</returns>
        /// <remarks></remarks>
        public static int Shifter(int layer, int tempshifter, int tempStarting)
        {
            bool plusorminus = true;
            int tempForReturn = tempStarting;
            for (int counter = 0; counter < layer; counter++)
            {
                if (plusorminus)
                {
                    tempForReturn += tempshifter;
                }
                else
                {
                    tempForReturn -= tempshifter;
                }

                if (tempForReturn > 255)
                {
                    plusorminus = !plusorminus;
                    int temptemp = tempForReturn - 254;
                    tempForReturn = 254 - temptemp;
                }

                if (tempForReturn < 0)
                {
                    plusorminus = !plusorminus;
                    int temptemp = 0 - tempForReturn;
                    tempForReturn = 0 + temptemp;
                }
            }

            return tempForReturn;
        }

        /// <summary>
        /// The write plugin.
        /// </summary>
        /// <remarks></remarks>
        public static void WritePlugin()
        {
            XmlTextWriter xtw = new XmlTextWriter(Global.StartupPath + "\\Meta Editor Settings.xml", Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("Meta_Editor_Settings");
            xtw.WriteStartElement("colors");
            xtw.WriteAttributeString("RedToShift", RedToShift.ToString());
            xtw.WriteAttributeString("GreenToShift", GreenToShift.ToString());
            xtw.WriteAttributeString("BlueToShift", BlueToShift.ToString());
            xtw.WriteAttributeString("StartingRed", StartingRed.ToString());
            xtw.WriteAttributeString("StartingGreen", StartingGreen.ToString());
            xtw.WriteAttributeString("StartingBlue", StartingBlue.ToString());
            xtw.WriteEndElement();
            xtw.WriteEndElement();
            xtw.Close();
        }

        #endregion
    }
}