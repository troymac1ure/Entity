using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using entity.Forms.Meta_Editor;
using System.IO;
using System.Xml;
using HaloMap.Plugins;
using entity.MetaEditor2;
using entity.MetaFuncs;


namespace entity.Forms.MetaEditor
{
    public partial class MetaEditor : UserControl
    {

        #region Constructor
        public MetaEditor()
        {
            InitializeComponent();
            ShiftColors.ReadPlugin();
        }
        #endregion
        #region Fields
        #region Visible/Hidden object types variables
        public static bool ShowReflexives = true;
        public static bool ShowIdents = true;
        public static bool ShowSIDs = true;
        public static bool ShowFloats = true;
        public static bool ShowString32s = true;
        public static bool ShowUnicodeString64s = true;
        public static bool ShowString256s = true;
        public static bool ShowUnicodeString256s = true;
        public static bool ShowInts = true;
        public static bool ShowUints = true;
        public static bool ShowShorts = true;
        public static bool ShowUshorts = true;
        public static bool ShowBitmask32s = true;
        public static bool ShowBitmask16s = true;
        public static bool ShowBitmask8s = true;
        public static bool ShowEnum32s = true;
        public static bool ShowEnum16s = true;
        public static bool ShowEnum8s = true;
        public static bool ShowBlockIndex32s = true;
        public static bool ShowBlockIndex16s = true;
        public static bool ShowBlockIndex8s = true;
        public static bool ShowBytes = true;
        public static bool ShowUndefineds = true;
        public static bool ShowInvisibles = false;
        #endregion
        public bool AutoSave = false;
        private List<string> infoText = new List<string>();
        public int mapIndex;
        public IFPIO ifp;
        private int tabIndex = 0;
        public string selectedTagType;
        public static bool _Paint = true;
        private int MEHeight = 480;
        const short WM_PAINT = 0x00f;
        System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();

        #region Color Wheel variables
        private DataValues alphaControl;
        private DataValues redControl;
        private DataValues greenControl;
        private DataValues blueControl;
        #endregion 
        public MEStringsSelector stringSwap;

        #region Custom Plugins variables
        public string pluginName;
        #endregion
        #endregion

        #region Main methods
        public void loadControls(int MapNumber)
        {
            this.mapIndex = MapNumber;
            if (Maps.map[mapIndex].SelectedMeta == null) return;
            this.setInfoText("Loading Tag");
            ToolTip1.InitialDelay = 800;
            _Paint = false;
            ReflexiveControl.ME = this;

            this.Controls.Clear();
            // Thanks to RapiD for pointing this fix out! If you just clear them, they stay in memory!
            foreach (Control control in this.panel1.Controls)
            {
                control.Dispose();
            }
            this.panel1.Controls.Clear();
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.panel1.Top = this.Padding.Top + this.toolStrip1.Height;
            this.panel1.Height = this.Height - (this.Padding.Top + this.Padding.Bottom + this.toolStrip1.Height + this.statusStrip1.Height);
            this.Controls[0].Hide();
            this.Refresh();
            this.SuspendLayout();
            ifp = IFP.IFPHashMap.GetIfp(Maps.map[mapIndex].SelectedMeta.type,mapIndex);
            LoadENTControls(ifp.items);
            this.ResumeLayout(true);
            this.Controls[0].Show();
            _Paint = true;
            this.ReloadMetaForSameTagType(true);
            this.restoreInfoText();
        }
        
        private void LoadENTControls(object[] entArray)
        {
            this.selectedTagType = Maps.map[mapIndex].SelectedMeta.type;

            this.toolStripTagType.Text = "[" + this.selectedTagType.ToString() + "]";
            this.toolStripTagName.Text = Maps.map[mapIndex].SelectedMeta.name;
            //this.Padding = new Padding(10);
            int colorSpaceCount = 4;
            
            // Custom Plugins access
            ra = new RegistryAccess(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2CustomPlugins + pluginName + "\\" + this.selectedTagType);
            if (pluginName == null)
                ra.CloseReg();

            foreach (object o in entArray)
            {                
                IFPIO.BaseObject tempbase = (IFPIO.BaseObject)o;
                if (tempbase.visible == false)
                    if (Meta_Editor.MetaEditor.ShowInvisibles == false)
                        continue;

                // skip hidden custom plugins variables (mark reflexives to be removed if empty)
                bool skipEmptyReflex = false;
                if (ra.isOpen && ra.getValue(tempbase.offset.ToString()) == bool.FalseString)
                    if (tempbase.ObjectType == IFPIO.ObjectEnum.Struct)
                        skipEmptyReflex = true;
                    else
                        continue;


                switch (tempbase.ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        {
                            if (Meta_Editor.MetaEditor.ShowReflexives == false)
                                break;
                            // tempLabel is a blank space located above reflexives
                            Label tempLabel = new Label();
                            tempLabel.AutoSize = true;
                            tempLabel.Location = new System.Drawing.Point(0, 0);
                            tempLabel.Name = "label1";
                            tempLabel.Dock = DockStyle.Top;                            
                            tempLabel.Size = new System.Drawing.Size(35, 13);
                            tempLabel.TabIndex = tabIndex;

                            // tempReflexive is the reflexive and all data (incl other reflexives) within it
                            ReflexiveControl tempReflexive = new ReflexiveControl(mapIndex, Maps.map[mapIndex].SelectedMeta.offset, ((IFPIO.Reflexive)tempbase).HasCount, tempbase.lineNumber, this);
                            //tempReflexive.Location = new System.Drawing.Point(10, 0);
                            tempReflexive.Name = "reflexive";
                            tempReflexive.TabIndex = tabIndex;
                            tempReflexive.LoadENTControls( (IFPIO.Reflexive)tempbase, ((IFPIO.Reflexive)tempbase).items,
                                                           true, 0, ref tabIndex, tempbase.offset.ToString());

                            // Label, Combobox & Button are always added ( = 3)
                            if (!(tempReflexive.Controls.Count <= 2 && skipEmptyReflex))
                            {
                                this.Controls[0].Controls.Add(tempLabel);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                                this.Controls[0].Controls.Add(tempReflexive);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Ident:
                        {
                            if (Meta_Editor.MetaEditor.ShowIdents == false)
                                break;
                            Ident tempident = new Ident(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.Ident)tempbase).hasTagType, tempbase.lineNumber);
                            tempident.Name = "ident";
                            tempident.TabIndex = tabIndex;
                            tempident.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            tempident.Tag = "[" + tempident.Controls[2].Text + "] " 
                                            + tempident.Controls[1].Text;
                            tempident.Controls[1].ContextMenuStrip = identContext;
                            this.Controls[0].Controls.Add(tempident);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.StringID:
                        {
                            if (Meta_Editor.MetaEditor.ShowSIDs == false)
                                break;
                            SID tempSID = new SID(tempbase.name, mapIndex, tempbase.offset, tempbase.lineNumber);
                            tempSID.Name = "sid";
                            tempSID.TabIndex = tabIndex;
                            tempSID.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempSID);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Float:
                        {
                            if (Meta_Editor.MetaEditor.ShowFloats == false)
                                break;
                            DataValues tempFloat = new DataValues(tempbase.name, mapIndex, tempbase.offset, IFPIO.ObjectEnum.Float, tempbase.lineNumber);
                            tempFloat.TabIndex = tabIndex;
                            tempFloat.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempFloat);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.String32:
                        {
                            if (Meta_Editor.MetaEditor.ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String32)
                                break;
                            EntStrings tempstring = new EntStrings(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.IFPString)tempbase).size, ((IFPIO.IFPString)tempbase).type, tempbase.lineNumber);
                            tempstring.Name = "string";
                            tempstring.TabIndex = tabIndex;
                            tempstring.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempstring);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.UnicodeString256:
                        {
                            if (Meta_Editor.MetaEditor.ShowUnicodeString256s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;                            
                        }
                    case IFPIO.ObjectEnum.String256:
                        {
                            if (Meta_Editor.MetaEditor.ShowString256s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;
                        }
                    case IFPIO.ObjectEnum.UnicodeString64:
                        {
                            if (Meta_Editor.MetaEditor.ShowUnicodeString64s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;
                        }
                    case IFPIO.ObjectEnum.String:
                        {
                            if (Meta_Editor.MetaEditor.ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String32)
                                break;
                            EntStrings tempstring = new EntStrings(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.IFPString)tempbase).size, ((IFPIO.IFPString)tempbase).type, tempbase.lineNumber);
                            tempstring.Name = "string";
                            tempstring.TabIndex = tabIndex;
                            tempstring.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempstring);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Int:
                        {
                            if (((IFPIO.IFPInt)tempbase).entIndex.nulled == true)
                            {
                                if ((Meta_Editor.MetaEditor.ShowInts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Int)
                                    || (Meta_Editor.MetaEditor.ShowShorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Short)
                                    || (Meta_Editor.MetaEditor.ShowUshorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.UShort)
                                    || (Meta_Editor.MetaEditor.ShowUints == false && tempbase.ObjectType == IFPIO.ObjectEnum.UInt))
                                    break;
                                DataValues tempdatavalues = new DataValues(tempbase.name, mapIndex, tempbase.offset, tempbase.ObjectType, tempbase.lineNumber);
                                tempdatavalues.TabIndex = tabIndex;
                                tempdatavalues.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                                this.Controls[0].Controls.Add(tempdatavalues);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            }
                            else
                            {
                                if ((Meta_Editor.MetaEditor.ShowBlockIndex32s == false && (tempbase.ObjectType == IFPIO.ObjectEnum.Int | tempbase.ObjectType == IFPIO.ObjectEnum.UInt))
                                    || (Meta_Editor.MetaEditor.ShowBlockIndex16s == false && (tempbase.ObjectType == IFPIO.ObjectEnum.Short | tempbase.ObjectType == IFPIO.ObjectEnum.UShort))
                                    || (Meta_Editor.MetaEditor.ShowBlockIndex8s == false && tempbase.ObjectType == IFPIO.ObjectEnum.Byte))
                                    break;
                                Indices tempdatavalues = new Indices(tempbase.name, mapIndex, tempbase.offset, tempbase.ObjectType, ((IFPIO.IFPInt)tempbase).entIndex);
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
                            if (Meta_Editor.MetaEditor.ShowUndefineds == false)
                                break;
                            DataValues tempUnknown = new DataValues(tempbase.name, mapIndex, tempbase.offset, IFPIO.ObjectEnum.Unknown, tempbase.lineNumber);
                            tempUnknown.TabIndex = tabIndex;
                            tempUnknown.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempUnknown);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte_Flags:
                        {
                            if (Meta_Editor.MetaEditor.ShowBitmask8s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.Bitmask)tempbase).bitmaskSize, ((IFPIO.Bitmask)tempbase).options, tempbase.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            tempbitmask.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempbitmask);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Word_Flags:
                        {
                            if (Meta_Editor.MetaEditor.ShowBitmask16s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.Bitmask)tempbase).bitmaskSize, ((IFPIO.Bitmask)tempbase).options, tempbase.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            tempbitmask.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempbitmask);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Long_Flags:
                        {
                            if (Meta_Editor.MetaEditor.ShowBitmask32s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.Bitmask)tempbase).bitmaskSize, ((IFPIO.Bitmask)tempbase).options, tempbase.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            tempbitmask.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempbitmask);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Char_Enum:
                        {
                            if (Meta_Editor.MetaEditor.ShowEnum8s == false)
                                break;
                            Enums tempenum = new Enums(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.IFPEnum)tempbase).enumSize, ((IFPIO.IFPEnum)tempbase).options, tempbase.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            tempenum.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempenum);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Enum:
                        {
                            if (Meta_Editor.MetaEditor.ShowEnum16s == false)
                                break;
                            Enums tempenum = new Enums(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.IFPEnum)tempbase).enumSize, ((IFPIO.IFPEnum)tempbase).options, tempbase.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            tempenum.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempenum);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Long_Enum:
                        {
                            if (Meta_Editor.MetaEditor.ShowEnum32s == false)
                                break;
                            Enums tempenum = new Enums(tempbase.name, mapIndex, tempbase.offset, ((IFPIO.IFPEnum)tempbase).enumSize, ((IFPIO.IFPEnum)tempbase).options, tempbase.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            tempenum.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempenum);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte:
                        {
                            if (((IFPIO.IFPByte)tempbase).entIndex.nulled == true)
                            {
                                if (Meta_Editor.MetaEditor.ShowBytes == false)
                                    break;
                                DataValues tempByte = new DataValues(tempbase.name, mapIndex, tempbase.offset, IFPIO.ObjectEnum.Byte, tempbase.lineNumber);
                                tempByte.TabIndex = tabIndex;
                                this.Controls[0].Controls.Add(tempByte);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            }
                            else
                            {
                                if (Meta_Editor.MetaEditor.ShowBlockIndex8s == false)
                                    break;
                                Indices tempdatavalues = new Indices(tempbase.name, mapIndex, tempbase.offset, tempbase.ObjectType, ((IFPIO.IFPByte)tempbase).entIndex);
                                tempdatavalues.TabIndex = tabIndex;
                                this.Controls[0].Controls.Add(tempdatavalues);
                                this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Unused:
                        {
                            DataValues tempUnknown = new DataValues(tempbase.name, mapIndex, tempbase.offset, IFPIO.ObjectEnum.Unused, tempbase.lineNumber);
                            tempUnknown.TabIndex = tabIndex;
                            tempUnknown.Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            this.Controls[0].Controls.Add(tempUnknown);
                            this.Controls[0].Controls[this.Controls[0].Controls.Count - 1].BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.TagType:
                        continue;
                        break;
                }

                if (!(tempbase is IFPIO.Reflexive))
                    ToolTip1.SetToolTip(this.Controls[0].Controls[0].Controls[0], "offset: " + tempbase.offset.ToString());

                if (this.Controls[0].Controls.Count > 0 && this.Controls[0].Controls[0] is DataValues)
                {
                    //if (((tempbase.name.ToLower().Contains(" a") & tempbase.name[tempbase.name.ToLower().IndexOf(" a")]) ||
                    //     tempbase.name.ToLower().Contains("alpha"))& alphaControl == null)
                    if (ColorWheel.checkForColor(tempbase.name, alphaControl, " a", "alpha"))
                    {
                        alphaControl = (DataValues)this.Controls[0].Controls[0];
                        colorSpaceCount = 0;
                    }
                    //if (tempbase.name.ToLower().Contains(" r") & redControl == null)
                    else if (ColorWheel.checkForColor(tempbase.name, redControl, " r", "red"))
                    {
                        redControl = (DataValues)this.Controls[0].Controls[0];
                        colorSpaceCount = 0;
                    }
                    //if (tempbase.name.ToLower().Contains(" g") & greenControl == null)
                    else if (ColorWheel.checkForColor(tempbase.name, greenControl, " g", "green"))
                    {
                        greenControl = (DataValues)this.Controls[0].Controls[0];
                        colorSpaceCount = 0;
                    }
                    //if (tempbase.name.ToLower().Contains(" b") & blueControl == null)
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
                            cw.setTextBox(alphaControl.textBox1, Color.White);
                        cw.setTextBox(redControl.textBox1, Color.Red);
                        cw.setTextBox(greenControl.textBox1, Color.Green);
                        cw.setTextBox(blueControl.textBox1, Color.Blue);

                        //p.I.AddRange(new Rectangle[] { SelectedColorRectangle });
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
                else colorSpaceCount++;
                tabIndex++;
            }
            ra.CloseReg();
        }

        public void ReloadMetaForSameTagType(bool reloadreflexive)
        {
            Maps.map[mapIndex].OpenMap(MapTypes.Internal);
            
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "entity.Forms.Meta_Editor.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.ReflexiveControl":
                        {
                            if (reloadreflexive == false)
                                break;
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).LoadMetaIntoControls(Maps.map[mapIndex].SelectedMeta.offset, Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Float":
                        {
                            ((Float)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Populate(Maps.map[mapIndex].SelectedMeta.offset);
                            break;
                        }

                }
                if (this.Controls[0].Controls[counter] is entity.Forms.Meta_Editor.BaseField)
                    ToolTip1.SetToolTip(this.Controls[0].Controls[counter], "offset: " + (((BaseField)(this.Controls[0].Controls[counter])).offsetInMap.ToString("x")));
                
            }

            Maps.map[mapIndex].CloseMap();
        }
        #endregion
        #region Misc. Methods
        #endregion
        public void IdentLoader_DropDown(object sender, EventArgs e)
        {
            ((System.Windows.Forms.ComboBox)sender).Items.AddRange((object[])Maps.map[mapIndex].FileNames.Name);
        }
        public void SaveButton_DropDown(object sender, EventArgs e)
        {
            this.SaveRecursively();
            this.ReloadMetaForSameTagType(false);
        }
        private void SaveRecursively()
        {
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "entity.Forms.Meta_Editor.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).SaveRecursively();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Float":
                        {
                            ((Float)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).Save();
                            break;
                        }
                }
            }
        }
        private void PokeRecursively()
        {
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "entity.Forms.Meta_Editor.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).PokeRecursively();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).Poke();
                            break;
                        }
                }
            }
        }
        private void SetFocusRecursively(int LineToCheck)
        {            
            for (int counter = 0; counter < this.Controls[0].Controls.Count; counter++)
            {
                switch (this.Controls[0].Controls[counter].GetType().ToString())
                {
                    case "entity.Forms.Meta_Editor.SID":
                        {
                            ((SID)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.Controls[0].Controls[counter]).SetFocusRecursively(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Ident":
                        {
                            ((Ident)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.EntStrings":
                        {
                            ((EntStrings)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Bitmask":
                        {
                            ((Bitmask)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.DataValues":
                        {
                            ((DataValues)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Enums":
                        {
                            ((Enums)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                    case "entity.Forms.Meta_Editor.Indices":
                        {
                            ((Indices)this.Controls[0].Controls[counter]).SetFocus(LineToCheck);
                            break;
                        }
                }
            }
        }
        public void IdentLoader_DropDownClosed(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();
        }
        
        
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // Code courtesy of Mark Mihevc
            // sometimes we want to eat the paint message so we don't have to see all the
            // flicker from when we select the text to change the color.

            if (m.Msg == WM_PAINT)
            {
                if (_Paint)
                    base.WndProc(ref m);   // if we decided to paint this control, just call the RichTextBox WndProc
                else
                    m.Result = IntPtr.Zero;   //  not painting, must set this to IntPtr.Zero if not painting otherwise serious problems.
            }
            else
                base.WndProc(ref m);   // message other than WM_PAINT, jsut do what you normally do.
        }
           

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.PokeRecursively();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                ListEntItems LEI = new ListEntItems(mapIndex, this.ifp);
                ((Button)LEI.Controls[0]).Click += new EventHandler(listEntItemsClose);
                LEI.Show();
            }
            catch
            {
            }
            
        }
        private void listEntItemsClose(object sender, EventArgs e)
        {
            this.SetFocusRecursively(((IFPIO.BaseObject)((Button)sender).Tag).lineNumber);            
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Meta Editor Version 1.3"
                + System.Environment.NewLine + "Improvements: TroyMac1ure"
                + System.Environment.NewLine + "Lead Programer: TheTyckoMan"
                + System.Environment.NewLine + "Awesome Map Dlls: Pokecancer"
                + System.Environment.NewLine + "RTH dll: Shalted"
                + System.Environment.NewLine + "Lots of support: The Brok3n Halo Team");
        }
        #region To Show Plugin Elements Or Not To Show Plugin Elements, THAT Is The Question...
        private void toolStripShowAll_Click(object sender, EventArgs e)
        {
            ItemsToShow its = new ItemsToShow();
            its.buttOK.Click += new System.EventHandler(this.buttOk_Click);
            its.Show();
        }
        private void buttOk_Click(object sender, EventArgs e)
        {
            this.loadControls(mapIndex);
        }

        private void toolStripColorShifting_Click(object sender, EventArgs e)
        {
            ColorShifting CS = new ColorShifting();
            CS.buttSave.Click += new EventHandler(buttOk_Click);
            CS.Show();
        }
        #endregion

        private void MetaEditor_SizeChanged(object sender, EventArgs e)
        {
            int totalSize = this.Padding.Top + this.Padding.Bottom;
            int temp = -1;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i] is Panel)
                    temp = i;
                else
                    totalSize += this.Controls[i].Height;
            }
            this.panel1.Height = this.Height - totalSize - 2;
            this.panel1.Top = this.Padding.Top + this.toolStrip1.Height;
            this.panel1.Width = this.Width - (this.Padding.Left + this.Padding.Right + 10);
        }

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

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectedText != "")
            {
                Clipboard.SetDataObject(c.SelectedText);
                c.Text = c.Text.Remove(c.SelectionStart, c.SelectionLength);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectedText != "")
                Clipboard.SetDataObject(c.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (Clipboard.GetText() != "")
            {
                c.SelectedText = Clipboard.GetText();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectionLength != 0)
                c.Text = c.Text.Remove(c.SelectionStart, c.SelectionLength);
        }

        private void identContext_Opening(object sender, CancelEventArgs e)
        {
            Control c = identContext.SourceControl as Control;
            if (!c.Focused) c.Focus();
            String tag = "";
            if (c.Parent.Tag != null)
                tag = c.Parent.Tag.ToString();
            if (tag != "")
            {
                this.gotoToolStripMenuItem.Text = "Jump to \"" + tag + "\"";
                gotoToolStripMenuItem.Visible = true;
            }
            else
                gotoToolStripMenuItem.Visible = false;
        }

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

        public void setInfoText(string text)
        {
            Point temp = ((Panel)this.Controls[0]).AutoScrollPosition;
            infoText.Add(toolStripInformation.Text);
            toolStripInformation.ForeColor = Color.Red;
            toolStripInformation.Text = ".: " + text + " :.";
            ((Panel)this.Controls[0]).AutoScrollPosition = new Point(-temp.X, -temp.Y);
            Application.DoEvents();
        }

        public void restoreInfoText()
        {
            if (infoText.Count > 0)
            {
                toolStripInformation.Text = infoText[infoText.Count - 1].ToString();
                infoText.RemoveAt(infoText.Count - 1);
            }
            else
                toolStripInformation.Text = ".: Idle :.";
            if (toolStripInformation.Text == ".: Idle :.")
                toolStripInformation.ForeColor = Color.Gray;
        }
            
        public void setReflexiveText(string Name, int Current, int Total)
        {
            this.toolStripReflexiveNumber.Text = Name + ": (" + Current.ToString() + " / " + Total.ToString() + ")";
        }

    }
    
    public static class ShiftColors
    {
        public static int StartingRed = 128;
        public static int StartingBlue = 128;
        public static int StartingGreen = 128;
        public static int RedToShift = 20;
        public static int BlueToShift = 20;
        public static int GreenToShift = 20;                
        public static void SetVariables(int Stred, int Stblue, int Stgreen, int Shred, int Shblue, int Shgreen)
        {
            if (Stred < 256 && Stred > -1)
                ShiftColors.StartingRed = Stred;
            if (Stblue < 256 && Stblue > -1)
                ShiftColors.StartingBlue = Stblue;
            if (Stgreen < 256 && Stgreen > -1)
                ShiftColors.StartingGreen = Stgreen;
            ShiftColors.RedToShift = Shred;
            ShiftColors.BlueToShift = Shblue;
            ShiftColors.GreenToShift = Shgreen;
        }
        public static int ShiftRed(int layer)
        {
            return Shifter(layer, RedToShift, StartingRed);
        }
        public static int ShiftBlue(int layer)
        {
            return Shifter(layer, BlueToShift, StartingBlue);
        }
        public static int ShiftGreen(int layer)
        {
            return Shifter(layer, GreenToShift, StartingGreen);
        }
        public static int Shifter(int layer,int tempshifter,int tempStarting)
        {
            bool plusorminus = true;
            int tempForReturn = tempStarting;
            for (int counter = 0; counter < layer; counter++)
            {
                if (plusorminus == true)
                    tempForReturn += tempshifter;
                else
                    tempForReturn -= tempshifter;
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
        public static void ReadPlugin()
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\Meta Editor Settings.xml", FileMode.Open, FileAccess.ReadWrite);
            XmlTextReader xtr = new XmlTextReader(fs);
            while (xtr.Read())
            {
                switch (xtr.Name.ToLower())
                {
                    case "colors":
                        {
                            try
                            {
                                ShiftColors.SetVariables(Convert.ToInt32(xtr.GetAttribute("StartingRed")),
                                    Convert.ToInt32(xtr.GetAttribute("StartingBlue")), Convert.ToInt32(xtr.GetAttribute("StartingGreen"))
                                    , Convert.ToInt32(xtr.GetAttribute("RedToShift")), Convert.ToInt32(xtr.GetAttribute("BlueToShift")),
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
        public static void WritePlugin()
        {
            XmlTextWriter xtw = new XmlTextWriter(Application.StartupPath + "\\Meta Editor Settings.xml", Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("Meta_Editor_Settings");
            xtw.WriteStartElement("colors");
            xtw.WriteAttributeString("RedToShift", ShiftColors.RedToShift.ToString());
            xtw.WriteAttributeString("GreenToShift", ShiftColors.GreenToShift.ToString());
            xtw.WriteAttributeString("BlueToShift", ShiftColors.BlueToShift.ToString());
            xtw.WriteAttributeString("StartingRed", ShiftColors.StartingRed.ToString());
            xtw.WriteAttributeString("StartingGreen", ShiftColors.StartingGreen.ToString());
            xtw.WriteAttributeString("StartingBlue", ShiftColors.StartingBlue.ToString());
            xtw.WriteEndElement();
            xtw.WriteEndElement();
            xtw.Close();
        }
    }
}
