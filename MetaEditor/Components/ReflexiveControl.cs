// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexiveControl.cs" company="">
//   
// </copyright>
// <summary>
//   The reflexive control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using global::MetaEditor.Forms;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The reflexive control.
    /// </summary>
    public partial class ReflexiveControl : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The me.
        /// </summary>
        public static MetaEditor ME;

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber;

        /// <summary>
        /// The chunk count.
        /// </summary>
        public int chunkCount;

        /// <summary>
        /// The selected chunk.
        /// </summary>
        public int selectedChunk;

        /// <summary>
        /// The tool tip 1.
        /// </summary>
        private readonly ToolTip ToolTip1 = new ToolTip();

        /// <summary>
        /// The has chunk count.
        /// </summary>
        private readonly bool hasChunkCount = true;

        /// <summary>
        /// The map index.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The alpha control.
        /// </summary>
        private DataValues alphaControl;

        /// <summary>
        /// The blue control.
        /// </summary>
        private DataValues blueControl;

        /// <summary>
        /// The chunk size.
        /// </summary>
        private int chunkSize;

        /// <summary>
        /// The control indexer.
        /// </summary>
        //private int controlIndexer;

        /// <summary>
        /// The green control.
        /// </summary>
        private DataValues greenControl;

        /// <summary>
        /// The label offset.
        /// </summary>
        private int labelOffset;

        /// <summary>
        /// The map offset.
        /// </summary>
        private int mapOffset;

        /// <summary>
        /// The offset in meta.
        /// </summary>
        private int offsetInMeta;

        /// <summary>
        /// The one up reflexive block offset.
        /// </summary>
        private int oneUpReflexiveBlockOffset;

        /// <summary>
        /// The path.
        /// </summary>
        private string path = string.Empty;

        /// <summary>
        /// The red control.
        /// </summary>
        private DataValues redControl;

        /// <summary>
        /// The reflexive items.
        /// </summary>
        private object[] reflexiveItems;

        /// <summary>
        /// The reflexive label.
        /// </summary>
        private string reflexiveLabel;

        /// <summary>
        /// The translated offset.
        /// </summary>
        private int translatedOffset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflexiveControl"/> class.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="iHasChunkCount">
        /// The i has chunk count.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public ReflexiveControl(Map map, int offset, bool iHasChunkCount, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            InitializeComponent();
            this.map = map;
            this.mapOffset = offset;
            this.hasChunkCount = iHasChunkCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflexiveControl"/> class.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="iHasChunkCount">
        /// The i has chunk count.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        /// <param name="me">
        /// The me.
        /// </param>
        public ReflexiveControl(Map map, int offset, bool iHasChunkCount, int iLineNumber, MetaEditor me)
        {
            ToolTip1.InitialDelay = 800;
            this.LineNumber = iLineNumber;
            InitializeComponent();
            this.map = map;
            this.mapOffset = offset;
            this.hasChunkCount = iHasChunkCount;
            ME = me;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load ent controls.
        /// </summary>
        /// <param name="reflexive">
        /// The reflexive.
        /// </param>
        /// <param name="entArray">
        /// The ent array.
        /// </param>
        /// <param name="clearControls">
        /// The clear controls.
        /// </param>
        /// <param name="layer">
        /// The layer.
        /// </param>
        /// <param name="tabIndex">
        /// The tab index.
        /// </param>
        /// <param name="customPluginPath">
        /// The custom plugin path.
        /// </param>
        public void LoadENTControls(
            IFPIO.Reflexive reflexive, 
            object[] entArray, 
            bool clearControls, 
            int layer, 
            ref int tabIndex, 
            string customPluginPath)
        {
            // add padding, offset label vertically, Change button sizes
            // int over = 10;
            if (clearControls)
            {
                foreach (Control c in this.bottomPanel.Controls)
                {
                    c.Dispose();
                }

                this.bottomPanel.Controls.Clear();
            }

            this.reflexiveItems = entArray;
            this.reflexiveLabel = reflexive.label;
            this.AutoSize = true;
            this.BackColor = Color.DarkGray;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Dock = DockStyle.Top;

            // this.ForeColor = System.Drawing.Color.DarkGray;
            this.Padding = new Padding(10, 0, 0, 0);
            this.offsetInMeta = reflexive.offset;
            this.chunkSize = reflexive.chunkSize;
            int tempCBindex = this.Controls.Count;
            this.tempComboBox.Tag = reflexive.name;
            this.tempLabel.Text = reflexive.name;
            ToolTip1.SetToolTip(this.tempLabel, "offset: " + customPluginPath);

            int colorSpaceCount = 3;

            foreach (object o in entArray)
            {
                IFPIO.BaseObject tempbase = (IFPIO.BaseObject)o;
                if (tempbase.visible == false)
                {
                    if (MetaEditor.ShowInvisibles == false)
                    {
                        continue;
                    }
                }

                bool skipEmptyReflex = false;
                //if (MetaEditor.ra.isOpen &&
                //    MetaEditor.ra.getValue(customPluginPath + "\\" + tempbase.offset) == bool.FalseString)
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

                // ComboBox for changing types
                ComboBox cbox = new ComboBox();
                cbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                cbox.BackColor = Color.LightGray;
                cbox.DropDownStyle = ComboBoxStyle.DropDownList;
                cbox.Width = 95;
                cbox.Items.AddRange(Enum.GetNames(typeof(IFPIO.ObjectEnum)));
                cbox.Items.RemoveAt(0); // Remove reflexive listing
                cbox.SelectionChangeCommitted += fieldTypeChanged; // Remove reflexive listing

                switch (tempbase.ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        {
                            if (MetaEditor.ShowReflexives == false)
                            {
                                continue;
                            }

                            Label tempLabel1 = new Label();
                            tempLabel1.AutoSize = true;
                            tempLabel1.Location = new Point(0, 0);
                            tempLabel1.Name = "label1";
                            tempLabel1.Dock = DockStyle.Top;
                            tempLabel1.Size = new Size(35, 13);
                            tempLabel1.TabIndex = 1;
                            ReflexiveControl tempReflexive = new ReflexiveControl(
                                map, translatedOffset, ((IFPIO.Reflexive)tempbase).HasCount, tempbase.lineNumber);
                            tempReflexive.Name = "reflexive";
                            tempReflexive.LoadENTControls(
                                (IFPIO.Reflexive)tempbase, 
                                ((IFPIO.Reflexive)tempbase).items, 
                                true, 
                                layer + 1, 
                                ref tabIndex, 
                                customPluginPath + "\\" + tempbase.offset);

                            // Label, Combobox & Button are always added ( = 3)
                            if (!(tempReflexive.Controls.Count <= 2 && skipEmptyReflex))
                            {
                                bottomPanel.Controls.Add(tempLabel1);
                                tempLabel1.BringToFront();
                                bottomPanel.Controls.Add(tempReflexive);
                                tempReflexive.BringToFront();
                            }

                            break;
                        }

                    case IFPIO.ObjectEnum.Block:
                        {
                            if (MetaEditor.ShowIdents == false)
                            {
                                continue;
                            }

                            TagBlock tempBlock = new TagBlock(
                                tempbase.name, map, tempbase.offset, tempbase.lineNumber);
                            tempBlock.Name = "tagblock";
                            tempBlock.Controls[1].ContextMenuStrip = identContext;
                            bottomPanel.Controls.Add(tempBlock);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Ident:
                        {
                            if (MetaEditor.ShowIdents == false)
                            {
                                continue;
                            }

                            Ident tempident = new Ident(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.Ident)tempbase).hasTagType, 
                                tempbase.lineNumber);
                            tempident.Name = "ident";
                            tempident.Controls[1].ContextMenuStrip = identContext;
                            tempident.Controls[2].ContextMenuStrip = identContext;
                            bottomPanel.Controls.Add(tempident);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.StringID:
                        {
                            if (MetaEditor.ShowSIDs == false)
                            {
                                continue;
                            }

                            SID tempSID = new SID(tempbase.name, map, tempbase.offset, tempbase.lineNumber);
                            tempSID.Name = "sid";
                            bottomPanel.Controls.Add(tempSID);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Float:
                        {
                            if (MetaEditor.ShowFloats == false)
                            {
                                continue;
                            }

                            DataValues tempFloat = new DataValues(
                                tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Float, tempbase.lineNumber);

                            // Float tempFloat = new Float(tempbase.name, map, tempbase.offset);
                            tempFloat.Name = "float";
                            bottomPanel.Controls.Add(tempFloat);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.String32:
                        {
                            if (MetaEditor.ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String32)
                            {
                                continue;
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
                            bottomPanel.Controls.Add(tempstring);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.UnicodeString256:
                        {
                            if (MetaEditor.ShowUnicodeString256s == false)
                            {
                                continue;
                            }

                            goto case IFPIO.ObjectEnum.String32;
                        }

                    case IFPIO.ObjectEnum.UnicodeString64:
                        {
                            if (MetaEditor.ShowUnicodeString64s == false)
                            {
                                continue;
                            }

                            goto case IFPIO.ObjectEnum.String32;
                        }

                    case IFPIO.ObjectEnum.String256:
                        {
                            if (MetaEditor.ShowString256s == false)
                            {
                                continue;
                            }

                            goto case IFPIO.ObjectEnum.String32;
                        }

                    case IFPIO.ObjectEnum.String:
                        {
                            if (MetaEditor.ShowString32s == false && tempbase.ObjectType == IFPIO.ObjectEnum.String)
                            {
                                continue;
                            }

                            goto case IFPIO.ObjectEnum.String32;
                        }

                    case IFPIO.ObjectEnum.Int:
                        {
                            if (((IFPIO.IFPInt)tempbase).entIndex.nulled)
                            {
                                if ((MetaEditor.ShowInts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Int) ||
                                    (MetaEditor.ShowShorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.Short) ||
                                    (MetaEditor.ShowUshorts == false && tempbase.ObjectType == IFPIO.ObjectEnum.UShort) ||
                                    (MetaEditor.ShowUints == false && tempbase.ObjectType == IFPIO.ObjectEnum.UInt))
                                {
                                    continue;
                                }

                                DataValues tempdatavalues = new DataValues(
                                    tempbase.name, map, tempbase.offset, tempbase.ObjectType, tempbase.lineNumber);
                                bottomPanel.Controls.Add(tempdatavalues);
                                bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            }
                            else
                            {
                                if ((MetaEditor.ShowBlockIndex32s == false &&
                                     (tempbase.ObjectType == IFPIO.ObjectEnum.Int |
                                      tempbase.ObjectType == IFPIO.ObjectEnum.UInt)) ||
                                    (MetaEditor.ShowBlockIndex16s == false &&
                                     (tempbase.ObjectType == IFPIO.ObjectEnum.Short |
                                      tempbase.ObjectType == IFPIO.ObjectEnum.UShort)) ||
                                    (MetaEditor.ShowBlockIndex8s == false &&
                                     tempbase.ObjectType == IFPIO.ObjectEnum.Byte))
                                {
                                    continue;
                                }

                                Indices tempdatavalues = new Indices(
                                    tempbase.name, 
                                    map, 
                                    tempbase.offset, 
                                    tempbase.ObjectType, 
                                    ((IFPIO.IFPInt)tempbase).entIndex);
                                bottomPanel.Controls.Add(tempdatavalues);
                                bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
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
                            if (MetaEditor.ShowUndefineds == false)
                            {
                                continue;
                            }

                            DataValues tempUndefined = new DataValues(
                                tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Unknown, tempbase.lineNumber);

                            // Float tempFloat = new Float(tempbase.name, map, tempbase.offset);
                            tempUndefined.Name = "tempUndefined";
                            bottomPanel.Controls.Add(tempUndefined);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Byte_Flags:
                        {
                            if (MetaEditor.ShowBitmask8s == false && ((IFPIO.Bitmask)tempbase).bitmaskSize == 8)
                            {
                                continue;
                            }

                            Bitmask tempbitmask = new Bitmask(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.Bitmask)tempbase).bitmaskSize, 
                                ((IFPIO.Bitmask)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempbitmask);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Word_Flags:
                        {
                            if (MetaEditor.ShowBitmask16s == false && ((IFPIO.Bitmask)tempbase).bitmaskSize == 16)
                            {
                                continue;
                            }

                            Bitmask tempbitmask = new Bitmask(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.Bitmask)tempbase).bitmaskSize, 
                                ((IFPIO.Bitmask)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempbitmask);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Long_Flags:
                        {
                            if (MetaEditor.ShowBitmask32s == false && ((IFPIO.Bitmask)tempbase).bitmaskSize == 32)
                            {
                                continue;
                            }

                            Bitmask tempbitmask = new Bitmask(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.Bitmask)tempbase).bitmaskSize, 
                                ((IFPIO.Bitmask)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempbitmask);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Byte:
                        {
                            if (((IFPIO.IFPByte)tempbase).entIndex.nulled)
                            {
                                if (MetaEditor.ShowBytes == false)
                                {
                                    continue;
                                }

                                DataValues tempByte = new DataValues(
                                    tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Byte, tempbase.lineNumber);
                                bottomPanel.Controls.Add(tempByte);
                                bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            }
                            else
                            {
                                if (MetaEditor.ShowBlockIndex8s == false)
                                {
                                    continue;
                                }

                                Indices tempdatavalues = new Indices(
                                    tempbase.name, 
                                    map, 
                                    tempbase.offset, 
                                    tempbase.ObjectType, 
                                    ((IFPIO.IFPByte)tempbase).entIndex);
                                bottomPanel.Controls.Add(tempdatavalues);
                                bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            }

                            break;
                        }

                    case IFPIO.ObjectEnum.Char_Enum:
                        {
                            if (MetaEditor.ShowEnum8s == false)
                            {
                                continue;
                            }

                            Enums tempenum = new Enums(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.IFPEnum)tempbase).enumSize, 
                                ((IFPIO.IFPEnum)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempenum);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Enum:
                        {
                            if (MetaEditor.ShowEnum16s == false)
                            {
                                continue;
                            }

                            Enums tempenum = new Enums(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.IFPEnum)tempbase).enumSize, 
                                ((IFPIO.IFPEnum)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempenum);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Long_Enum:
                        {
                            if (MetaEditor.ShowEnum32s == false)
                            {
                                continue;
                            }

                            Enums tempenum = new Enums(
                                tempbase.name, 
                                map, 
                                tempbase.offset, 
                                ((IFPIO.IFPEnum)tempbase).enumSize, 
                                ((IFPIO.IFPEnum)tempbase).options, 
                                tempbase.lineNumber);
                            bottomPanel.Controls.Add(tempenum);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.Unused:
                        {
                            DataValues tempUnused = new DataValues(
                                tempbase.name, map, tempbase.offset, IFPIO.ObjectEnum.Unused, tempbase.lineNumber);
                            tempUnused.TabIndex = tabIndex;
                            tempUnused.Populate(map.SelectedMeta.offset);
                            bottomPanel.Controls.Add(tempUnused);
                            bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();
                            break;
                        }

                    case IFPIO.ObjectEnum.TagType:
                        continue;
                }

                if (!(tempbase is IFPIO.Reflexive))
                {
                    bottomPanel.Controls[0].ContextMenuStrip = identContext;
                    bottomPanel.Controls[0].Controls[1].ContextMenuStrip = identContext;
                    ToolTip1.SetToolTip(
                        this.bottomPanel.Controls[0].Controls[0], "offset: " + customPluginPath + "\\" + tempbase.offset);

                    // cbox.Location = new Point(this.bottomPanel.Controls[0].Width - 100, 4);
                    // cbox.SelectedIndex = cbox.Items.IndexOf(tempbase.ObjectType.ToString());
                    // this.bottomPanel.Controls[0].Controls.RemoveAt(this.bottomPanel.Controls[0].Controls.Count - 1);
                    this.bottomPanel.Controls[0].Controls[this.bottomPanel.Controls[0].Controls.Count - 1].Click +=
                        fieldTypeChanged;

                    // this.bottomPanel.Controls[0].Controls.Add(cbox);
                }

                if (bottomPanel.Controls.Count > 0 && bottomPanel.Controls[0] is DataValues)
                {
                    // if (((tempbase.name.ToLower().Contains(" a") & tempbase.name[tempbase.name.ToLower().IndexOf(" a")]) ||
                    // tempbase.name.ToLower().Contains("alpha"))& alphaControl == null)
                    if (ColorWheel.checkForColor(tempbase.name, alphaControl, " a", "alpha"))
                    {
                        alphaControl = (DataValues)bottomPanel.Controls[0];
                        colorSpaceCount = 0;
                    }
                        
                        // if (tempbase.name.ToLower().Contains(" r") & redControl == null)
                    else if (ColorWheel.checkForColor(tempbase.name, redControl, " r", "red"))
                    {
                        redControl = (DataValues)bottomPanel.Controls[0];
                        colorSpaceCount = 0;
                    }
                        
                        // if (tempbase.name.ToLower().Contains(" g") & greenControl == null)
                    else if (ColorWheel.checkForColor(tempbase.name, greenControl, " g", "green"))
                    {
                        greenControl = (DataValues)bottomPanel.Controls[0];
                        colorSpaceCount = 0;
                    }
                        
                        // if (tempbase.name.ToLower().Contains(" b") & blueControl == null)
                    else if (ColorWheel.checkForColor(tempbase.name, blueControl, " b", "blue"))
                    {
                        blueControl = (DataValues)bottomPanel.Controls[0];
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
                        bottomPanel.Controls.Add(cw);
                        bottomPanel.Controls[bottomPanel.Controls.Count - 1].BringToFront();

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
            }

            // used for customPlugin variables
            this.Size = this.PreferredSize;
            this.AutoScroll = false;
            this.AutoSize = false;
        }

        /// <summary>
        /// The load meta into controls.
        /// </summary>
        /// <param name="startingOffset">
        /// The starting offset.
        /// </param>
        /// <param name="ioneUpReflexiveBlockOffset">
        /// The ione up reflexive block offset.
        /// </param>
        public void LoadMetaIntoControls(int startingOffset, int ioneUpReflexiveBlockOffset)
        {
            this.oneUpReflexiveBlockOffset = ioneUpReflexiveBlockOffset;
            this.selectedChunk = 0;
            map.BR.BaseStream.Position = startingOffset + offsetInMeta;
            if (this.hasChunkCount)
            {
                this.chunkCount = map.BR.ReadInt32();
            }
            else
            {
                this.chunkCount = 1;
            }

            if (this.chunkCount != 0)
            {
                map.BR.BaseStream.Position = this.hasChunkCount
                                                      ? startingOffset + offsetInMeta + 4
                                                      : startingOffset + offsetInMeta;
                this.translatedOffset = map.BR.ReadInt32() - map.SelectedMeta.magic;
                this.UpdateReflexiveSelectionBox(true);
                this.LoadMetaIntoControlsForThisReflexive();
                if (!this.Enabled)
                {
                    this.Size = this.PreferredSize;
                }

                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
                this.Size = new Size(this.PreferredSize.Width, 24);
            }
        }

        /// <summary>
        /// The poke recursively.
        /// </summary>
        public void PokeRecursively()
        {
            ControlCollection reflControl = this.Controls[0].Controls;
            for (int counter = 0; counter < reflControl.Count; counter++)
            {
                switch (reflControl[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)reflControl[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)reflControl[counter]).PokeRecursively();
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)reflControl[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)reflControl[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)reflControl[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)reflControl[counter]).Poke();
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)reflControl[counter]).Poke();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The save recursively.
        /// </summary>
        public void SaveRecursively()
        {
            ControlCollection reflControl = this.Controls[0].Controls;
            for (int counter = 0; counter < reflControl.Count; counter++)
            {
                switch (reflControl[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)reflControl[counter]).SaveRecursively();
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Float":
                        {
                            ((Float)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)reflControl[counter]).Save();
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)reflControl[counter]).Save();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// The set focus recursively.
        /// </summary>
        /// <param name="LineToCheck">
        /// The line to check.
        /// </param>
        public void SetFocusRecursively(int LineToCheck)
        {
            ControlCollection reflControl = this.Controls[0].Controls;
            if (this.LineNumber == LineToCheck)
            {
                this.Focus();
            }

            for (int counter = 0; counter < reflControl.Count; counter++)
            {
                switch (reflControl[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)reflControl[counter]).SetFocusRecursively(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)reflControl[counter]).SetFocus(LineToCheck);
                            break;
                        }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The load meta into controls for this reflexive.
        /// </summary>
        private void LoadMetaIntoControlsForThisReflexive()
        {
            // Control bottomPanel = this.Controls[0];
            for (int counter = 0; counter < this.bottomPanel.Controls.Count; counter++)
            {
                string s = bottomPanel.Controls[counter].GetType().ToString();
                switch (bottomPanel.Controls[counter].GetType().ToString())
                {
                    case "MetaEditor.Components.SID":
                        {
                            ((SID)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.ReflexiveControl":
                        {
                            ((ReflexiveControl)this.bottomPanel.Controls[counter]).LoadMetaIntoControls(
                                translatedOffset + (this.selectedChunk * this.chunkSize), this.translatedOffset);
                            break;
                        }

                    case "MetaEditor.Components.TagBlock":
                        {
                            ((TagBlock)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.Ident":
                        {
                            ((Ident)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.Float":
                        {
                            ((Float)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.EntStrings":
                        {
                            ((EntStrings)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.DataValues":
                        {
                            ((DataValues)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.Bitmask":
                        {
                            ((Bitmask)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.Enums":
                        {
                            ((Enums)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize));
                            break;
                        }

                    case "MetaEditor.Components.Indices":
                        {
                            ((Indices)this.bottomPanel.Controls[counter]).Populate(
                                this.translatedOffset + (this.selectedChunk * this.chunkSize), 
                                this.oneUpReflexiveBlockOffset);
                            break;
                        }
                }

                if (this.bottomPanel.Controls[counter] is BaseField)
                {
                    ToolTip1.SetToolTip(
                        this.bottomPanel.Controls[counter], 
                        "offset: " + ((BaseField)this.bottomPanel.Controls[counter]).offsetInMap.ToString("x"));
                }
            }
        }

        /// <summary>
        /// The reflexive button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ReflexiveButton_Click(object sender, EventArgs e)
        {
            ME.SuspendLayout();
            Button b = (Button)sender;
            ReflexiveControl rc = (ReflexiveControl)b.Parent.Parent;
            if (b.Text == "-")
            {
                rc.Size = new Size(rc.PreferredSize.Width, b.Parent.Height);
                b.Text = "+";
            }
            else
            {
                rc.Size = rc.PreferredSize;
                b.Text = "-";
            }

            while (rc.Parent.Parent is ReflexiveControl)
            {
                rc = (ReflexiveControl)rc.Parent.Parent;
                rc.Size = rc.PreferredSize;
            }

            ME.ResumeLayout();
        }

        /// <summary>
        /// The reflexive loader_ close.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ReflexiveLoader_Close(object sender, EventArgs e)
        {
            if (!((Control)sender).Focused)
            {
                return;
            }

            // ME.setInfoText("Refreshing Reflexive...");
            this.ParentForm.ActiveControl.SuspendLayout();
            this.selectedChunk = ((ComboBox)sender).SelectedIndex;
            map.OpenMap(MapTypes.Internal);
            this.LoadMetaIntoControlsForThisReflexive();
            map.CloseMap();

            // ME.restoreInfoText();
            int cn = ((ComboBox)sender).Parent.Controls.IndexOf((Control)sender);
            ME.setReflexiveText(
                ((Control)sender).Parent.Controls[cn - 1].Text, this.selectedChunk, ((ComboBox)sender).Items.Count - 1);

            this.ParentForm.ActiveControl.ResumeLayout(false);
        }

        /// <summary>
        /// The reflexive loader_ drop down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ReflexiveLoader_DropDown(object sender, EventArgs e)
        {
            // ME.setInfoText("Loading Reflexives...");
            UpdateReflexiveSelectionBox(true);

            // ME.restoreInfoText();
        }

        /// <summary>
        /// The update reflexive selection box.
        /// </summary>
        /// <param name="loadAllChunks">
        /// The load all chunks.
        /// </param>
        private void UpdateReflexiveSelectionBox(bool loadAllChunks)
        {
            this.tempComboBox.Items.Clear();
            IFPIO.ObjectEnum labelType = IFPIO.ObjectEnum.Unused;

            

            for (int counter = 0; counter < this.reflexiveItems.Length; counter++)
            {
                if (((IFPIO.BaseObject)this.reflexiveItems[counter]).name == this.reflexiveLabel &&
                    ((IFPIO.BaseObject)this.reflexiveItems[counter]).ObjectType != IFPIO.ObjectEnum.TagType)
                {
                    IFPIO.BaseObject tempbase = (IFPIO.BaseObject)reflexiveItems[counter];
                    this.labelOffset = tempbase.offset;
                    switch (tempbase.ObjectType)
                    {
                        case IFPIO.ObjectEnum.Ident:
                            {
                                labelType = IFPIO.ObjectEnum.Ident;
                                break;
                            }

                        case IFPIO.ObjectEnum.StringID:
                            {
                                labelType = IFPIO.ObjectEnum.StringID;
                                break;
                            }

                        case IFPIO.ObjectEnum.Float:
                            {
                                labelType = IFPIO.ObjectEnum.Float;
                                break;
                            }

                        case IFPIO.ObjectEnum.String32:
                            {
                                labelType = ((IFPIO.IFPString)tempbase).type == false
                                                ? ((IFPIO.IFPString)tempbase).size == 32
                                                      ? IFPIO.ObjectEnum.String32
                                                      : IFPIO.ObjectEnum.String256
                                                : ((IFPIO.IFPString)tempbase).size == 64
                                                      ? IFPIO.ObjectEnum.UnicodeString64
                                                      : IFPIO.ObjectEnum.UnicodeString256;
                                break;
                            }

                        case IFPIO.ObjectEnum.UnicodeString256:
                            {
                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.String256:
                            {
                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.UnicodeString64:
                            {
                                goto case IFPIO.ObjectEnum.String32;
                            }

                        case IFPIO.ObjectEnum.Int:
                            {
                                labelType = IFPIO.ObjectEnum.Int;
                                break;
                            }

                        case IFPIO.ObjectEnum.Short:
                            {
                                labelType = IFPIO.ObjectEnum.Short;
                                break;
                            }

                        case IFPIO.ObjectEnum.UShort:
                            {
                                labelType = IFPIO.ObjectEnum.UShort;
                                break;
                            }

                        case IFPIO.ObjectEnum.UInt:
                            {
                                labelType = IFPIO.ObjectEnum.UInt;
                                break;
                            }

                        case IFPIO.ObjectEnum.Unknown:
                            {
                                goto case IFPIO.ObjectEnum.Float;
                            }
                    }

                    break;
                }
            }

            

            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            int tempChunkCount = loadAllChunks ? chunkCount : 1;

            for (int counter = 0; counter < tempChunkCount; counter++)
            {
                object tempvalue = string.Empty;
                map.BR.BaseStream.Position = this.translatedOffset + this.labelOffset + (this.chunkSize * counter);
                switch (labelType)
                {
                    case IFPIO.ObjectEnum.Short:
                        {
                            tempvalue = map.BR.ReadInt16();
                            break;
                        }

                    case IFPIO.ObjectEnum.Int:
                        {
                            tempvalue = map.BR.ReadInt32();
                            break;
                        }

                    case IFPIO.ObjectEnum.UShort:
                        {
                            tempvalue = map.BR.ReadUInt16();
                            break;
                        }

                    case IFPIO.ObjectEnum.UInt:
                        {
                            tempvalue = map.BR.ReadUInt32();
                            break;
                        }

                    case IFPIO.ObjectEnum.Float:
                        {
                            tempvalue = map.BR.ReadSingle();
                            break;
                        }

                    case IFPIO.ObjectEnum.Unknown:
                        {
                            tempvalue = map.BR.ReadSingle();
                            break;
                        }

                    case IFPIO.ObjectEnum.StringID:
                        {
                            int sidIndexer = map.BR.ReadInt16();
                            tempvalue = map.Strings.Name[sidIndexer];
                            break;
                        }

                    case IFPIO.ObjectEnum.Ident:
                        {
                            int tempint = map.BR.ReadInt32();
                            int tagIndex = map.Functions.ForMeta.FindMetaByID(tempint);
                            if (tempint != -1)
                            {
                                tempvalue = map.MetaInfo.TagType[tagIndex] + " - " +
                                            map.FileNames.Name[tagIndex];
                            }
                            else
                            {
                                tempvalue = "null";
                            }

                            break;
                        }

                    case IFPIO.ObjectEnum.String32:
                        {
                            Encoding decode = Encoding.Unicode;
                            byte[] tempbytes = map.BR.ReadBytes(32);
                            tempvalue = decode.GetString(tempbytes);
                            break;
                        }

                    case IFPIO.ObjectEnum.UnicodeString64:
                        {
                            Encoding decode = Encoding.Unicode;
                            byte[] tempbytes = map.BR.ReadBytes(64);
                            tempvalue = decode.GetString(tempbytes);
                            break;
                        }

                    case IFPIO.ObjectEnum.String256:
                        {
                            Encoding decode = Encoding.Unicode;
                            byte[] tempbytes = map.BR.ReadBytes(256);
                            tempvalue = decode.GetString(tempbytes);
                            break;
                        }

                    case IFPIO.ObjectEnum.UnicodeString256:
                        {
                            goto case IFPIO.ObjectEnum.String256;
                        }
                }

                this.tempComboBox.Items.Add(counter + " : " + tempvalue);

                // ((System.Windows.Forms.ComboBox)panelControl.Controls[controlIndexer]).Items.Add(counter.ToString() + " : " + tempvalue.ToString());
            }

            // This causes the combo box to close, therefore lagging and causing bugs!
            this.tempComboBox.SelectedIndex = this.selectedChunk;
            this.tempComboBox.SelectedItem = this.selectedChunk;
            this.tempComboBox.SelectionLength = 0;

            // ((System.Windows.Forms.ComboBox)panelControl.Controls[controlIndexer]).SelectedIndex = this.selectedChunk;
            // ((System.Windows.Forms.ComboBox)panelControl.Controls[controlIndexer]).SelectedItem = this.selectedChunk;
            // ((System.Windows.Forms.ComboBox)panelControl.Controls[controlIndexer]).Text = ((System.Windows.Forms.ComboBox)this.Controls[controlIndexer]).Items[0].ToString();
            // ((System.Windows.Forms.ComboBox)panelControl.Controls[controlIndexer]).SelectionLength = 0;
            this.ResumeLayout(false);
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The add button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void addButton_Click(object sender, EventArgs e)
        {
            // map.SelectedMeta.items.MS.Length;
            MetaEditor me = (MetaEditor)this.Parent.Parent;
            int line = this.LineNumber;
            foreach (IFPIO.BaseObject o in me.ifp.items)
            {
                if (o.lineNumber == line)
                {
                    IFPIO.Reflexive r = (IFPIO.Reflexive)o;

                    // IFPIO.BaseObject o2 = me.ifp.items[o.siblingNext];
                    // int diff = o2.lineNumber - line;
                    break;
                }
            }
        }

        /// <summary>
        /// The copy to all tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void copyToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Control c = ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
                if (c.Parent is Panel)
                {
                    c = c.Controls[1];
                }

                string cName = c.Parent.Controls[0].Text;
                string reflName = c.Parent.Parent.Parent.Controls[1].Controls[1].Text;
                ComboBox refl = (ComboBox)c.Parent.Parent.Parent.Controls[1].Controls[2];
                string Value = c.Text;
                if (c.Parent is Ident)
                {
                    Value = "[" + c.Parent.Controls[2].Text.PadLeft(4) + "] " + c.Parent.Controls[1].Text;
                }

                // else if (BN is Entity.BitmaskNode) Value = ((BitmaskNode)BN).Bits.ToString() + ((BitmaskNode)BN).Mask.ToString();
                int loc = -1;
                for (int x = 0; x < this.reflexiveItems.Length; x++)
                {
                    if (((IFPIO.BaseObject)this.reflexiveItems[x]).name == cName)
                    {
                        loc = x;
                        break;
                    }
                }

                if (loc == -1)
                {
                    MessageBox.Show("ERROR: Cannot find " + cName + " in " + reflName);
                    return;
                }

                // Give a chance to cancel
                if (
                    MessageBox.Show(
                        "This will change all the \"" + cName + "\" values within the \"" + reflName +
                        "\" reflexive to \"" + Value + "\".\nThis cannot be undone, continue?", 
                        "Floodfill Values?", 
                        MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                int CurrentBaseOffset = this.translatedOffset + ((IFPIO.BaseObject)this.reflexiveItems[loc]).offset;
                for (int count = 0; count < this.chunkCount; count++)
                {
                    map.OpenMap(MapTypes.Internal);
                    int mapOffset = CurrentBaseOffset + this.chunkSize * count;
                    switch (c.Parent.ToString())
                    {
                        case "Entity.MetaEditor.Ident":
                            char[] TagType = Value.Substring(1, 4).ToCharArray();
                            int TagPath = ((Ident)c.Parent).tagIndex;
                            map.BW.BaseStream.Position = mapOffset;
                            map.BW.Write(TagType);
                            map.BW.Write(map.MetaInfo.Ident[TagPath]);
                            break;

                            #region Bitmask

                        case "Entity.MetaEditor.Bitmask":
                            map.BW.BaseStream.Position = mapOffset;
                            switch (((Bitmask)c.Parent).bitCount)
                            {
                                case 8:
                                    map.BW.Write(byte.Parse(((Bitmask)c.Parent).Value));
                                    break;
                                case 16:
                                    map.BW.Write(short.Parse(((Bitmask)c.Parent).Value));
                                    break;
                                case 32:
                                    map.BW.Write(int.Parse(((Bitmask)c.Parent).Value));
                                    break;
                            }

                            break;

                            #endregion

                            #region DataValues

                        case "Entity.MetaEditor.DataValues":
                            map.BW.BaseStream.Position = mapOffset;
                            switch (((DataValues)c.Parent).ValueType)
                            {
                                case IFPIO.ObjectEnum.Byte:
                                    map.BW.Write(byte.Parse(Value));
                                    break;
                                case IFPIO.ObjectEnum.Short:
                                    map.BW.Write(short.Parse(Value));
                                    break;
                                case IFPIO.ObjectEnum.UShort:
                                    map.BW.Write(ushort.Parse(Value));
                                    break;
                                case IFPIO.ObjectEnum.Int:
                                    map.BW.Write(int.Parse(Value));
                                    break;
                                case IFPIO.ObjectEnum.Float:
                                case IFPIO.ObjectEnum.Unknown:
                                case IFPIO.ObjectEnum.Unused:
                                    map.BW.Write(Single.Parse(Value));
                                    break;
                                default:
                                    MessageBox.Show(
                                        ((DataValues)c.Parent).ValueType +
                                        " support not added (AKA forgotten), please inform developer!");
                                    count = chunkCount - 1;
                                    break;
                            }

                            break;

                            #endregion

                            #region Enums

                        case "Entity.MetaEditor.Enums":
                            map.BW.BaseStream.Position = mapOffset;
                            switch (((Enums)c.Parent).enumType)
                            {
                                case 8:
                                    map.BW.Write(byte.Parse(((Enums)c.Parent).Value));
                                    break;
                                case 16:
                                    map.BW.Write(short.Parse(((Enums)c.Parent).Value));
                                    break;
                                case 32:
                                    map.BW.Write(int.Parse(((Enums)c.Parent).Value));
                                    break;
                            }

                            break;

                            #endregion

                            #region StringID

                        case "Entity.MetaEditor.SID":
                            map.BW.BaseStream.Position = mapOffset;
                            map.BW.Write((short)((SID)c.Parent).sidIndexer);
                            map.BW.Write((byte)0);
                            map.BW.Write((byte)map.Strings.Length[((SID)c.Parent).sidIndexer]);
                            break;

                            #endregion

                        default:
                            MessageBox.Show(c.Parent + " support not added (AKA forgotten), please inform developer!");
                            count = chunkCount - 1;
                            break;
                    }

                    /*

                            switch (Type)
                            {
                                case "Entity.StringIDNode":
                                    ((StringIDNode)BN).StrIndex = short.Parse(Value);
                                    ((StringIDNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                                case "Entity.ValueNode":
                                    ((ValueNode)BN).Value = Value;
                                    ((ValueNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                                case "Entity.ValueIndexNode":
                                    ((ValueIndexNode)BN).Value = int.Parse(Value);
                                    ((ValueIndexNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                                /*
                                 case Entity.BitmaskNode:
                                    ((BitmaskNode)BN).Bits = Value;
                                    ((BitmaskNode)BN).Mask = Value;
                                    break;
                                 * /
                                case "Entity.EnumNode":
                                    ((EnumNode)BN).Value = Value;
                                    ((EnumNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                                case "Entity.StringNode":
                                    ((StringNode)BN).Text = Value;
                                    ((StringNode)BN)._valueChanged = true;
                                    ((StringNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                                case "Entity.UnknownNode":
                                    Array.Copy(Value.ToCharArray(), ((UnknownNode)BN).Value, Value.ToCharArray().Length);
                                    ((UnknownNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                    break;
                            }

                     */
                    map.CloseMap();
                }
            }
            catch
            {
            }

            /*
                    StructNode BNP = (StructNode)BN.Parent;
                    int selectedIndex = BNP.CurComboBoxSelectedIndex;
                    string Type = BN.ToString();
                    string Value = "";
                    if (BN is Entity.TagRefNode) Value = "[" + ((TagRefNode)BN).TagType.ToString().PadLeft(4) + "] " + ((TagRefNode)BN).TagPath.ToString();
                    else if (BN is Entity.StringIDNode) Value = ((StringIDNode)BN).StrIndex.ToString();
                    else if (BN is Entity.ValueNode) Value = ((ValueNode)BN).Value.ToString();
                    else if (BN is Entity.ValueIndexNode) Value = ((ValueIndexNode)BN).Value.ToString();
                    //else if (BN is Entity.BitmaskNode) Value = ((BitmaskNode)BN).Bits.ToString() + ((BitmaskNode)BN).Mask.ToString();
                    else if (BN is Entity.EnumNode) Value = ((EnumNode)BN).Value.ToString();
                    else if (BN is Entity.StringNode) Value = ((StringNode)BN).Text.ToString();
                    else if (BN is Entity.UnknownNode) Value = ((UnknownNode)BN).Value.ToString();

                    // Give a chance to cancel
                    if (MessageBox.Show("This will change all the \"" + BN.Name + "\" values within the \"" + BNP.Name + "\" reflexive to \"" + Value + "\".\nThis cannot be undone, continue?", "Floodfill Values?", MessageBoxButtons.YesNo) == DialogResult.No)
                    { return; }

                    for (int count = 0; count < BNP.ChunkCount; count++)
                    {
                        //BNP.CurrentBaseOffset = BNP.Translation + (selectedIndex * BNP.Size);
                        BitWriter bw = new BitWriter(_map.filePath, Endian.Little);

                        switch (Type)
                        {
                            case "Entity.TagRefNode":
                                ((TagRefNode)BN).TagType = Value.Substring(1, 4);
                                ((TagRefNode)BN).TagPath = Value.Substring(7);
                                ((TagRefNode)BN)._valueChanged = true;
                                ((TagRefNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            case "Entity.StringIDNode":
                                ((StringIDNode)BN).StrIndex = short.Parse(Value);
                                ((StringIDNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            case "Entity.ValueNode":
                                ((ValueNode)BN).Value = Value;
                                ((ValueNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            case "Entity.ValueIndexNode":
                                ((ValueIndexNode)BN).Value = int.Parse(Value);
                                ((ValueIndexNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            /*
                             case Entity.BitmaskNode:
                                ((BitmaskNode)BN).Bits = Value;
                                ((BitmaskNode)BN).Mask = Value;
                                break;
                             * /
                            case "Entity.EnumNode":
                                ((EnumNode)BN).Value = Value;
                                ((EnumNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            case "Entity.StringNode":
                                ((StringNode)BN).Text = Value;
                                ((StringNode)BN)._valueChanged = true;
                                ((StringNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                            case "Entity.UnknownNode":
                                Array.Copy(Value.ToCharArray(), ((UnknownNode)BN).Value, Value.ToCharArray().Length);
                                ((UnknownNode)BN).Write(bw, BNP.Translation + (count * BNP.Size));
                                break;
                        }
                        bw.Close();

                    }
            */
        }

        /// <summary>
        /// The copy tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (c.SelectionLength != 0)
            {
                c.Text = c.Text.Remove(c.SelectionStart, c.SelectionLength);
            }
        }

        /// <summary>
        /// The field type changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void fieldTypeChanged(object sender, EventArgs e)
        {
            // Get original data
            Control c = ((Control)sender).Parent;
            int origPlacement = c.Parent.Controls.IndexOf(c);
            Control origParent = c.Parent;

            ControlSwapper cs = new ControlSwapper(c);
            cs.ShowDialog();
            string name = string.Empty;
            int offset = -1;
            int chunkoffset = -1;
            int lineNum = -1;
            int sizeCount = 0;

            //////
            c.Enabled = true;
            origParent.Controls.Add(c);
            origParent.Controls.SetChildIndex(c, origPlacement);
            return;

            /////

            switch (c.ToString())
            {
                case "Entity.MetaEditor.Bitmask":
                    Bitmask bt = (Bitmask)c;
                    name = bt.EntName;
                    offset = bt.offsetInMap;
                    chunkoffset = bt.chunkOffset;
                    lineNum = bt.LineNumber;
                    sizeCount = bt.bitCount >> 3;
                    break;
                case "Entity.MetaEditor.DataValues":
                    DataValues dv = (DataValues)c;
                    name = dv.EntName;
                    offset = dv.offsetInMap;
                    chunkoffset = dv.chunkOffset;
                    lineNum = dv.LineNumber;
                    switch (dv.ValueType)
                    {
                        case IFPIO.ObjectEnum.Byte:
                            // case DataValues.ENTType.ub:
                            sizeCount = 1;
                            break;
                        case IFPIO.ObjectEnum.Short:
                        case IFPIO.ObjectEnum.UShort:
                            sizeCount = 2;
                            break;
                        case IFPIO.ObjectEnum.Float:
                        case IFPIO.ObjectEnum.Int:
                        case IFPIO.ObjectEnum.UInt:
                            sizeCount = 4;
                            break;
                    }

                    break;
            }

            Control c2 = null;
            switch (((string)((ComboBox)sender).SelectedItem).ToLower())
            {
                case "byte":
                    if (sizeCount < 1)
                    {
                        break;
                    }

                    DataValues dv = new DataValues(name, map, chunkoffset, IFPIO.ObjectEnum.Byte, lineNum);
                    dv.Controls.RemoveAt(dv.Controls.Count - 1);
                    c2 = dv;
                    sizeCount -= 1;
                    break;
                case "short":
                    if (sizeCount < 2)
                    {
                        break;
                    }

                    dv = new DataValues(name, map, chunkoffset, IFPIO.ObjectEnum.Short, lineNum);
                    dv.Controls.RemoveAt(dv.Controls.Count - 1);
                    c2 = dv;
                    sizeCount -= 2;
                    break;
                case "int":
                    if (sizeCount < 4)
                    {
                        break;
                    }

                    dv = new DataValues(name, map, chunkoffset, IFPIO.ObjectEnum.Int, lineNum);
                    dv.Controls.RemoveAt(dv.Controls.Count - 1);
                    c2 = dv;
                    sizeCount -= 4;
                    break;
                case "float":
                    if (sizeCount < 4)
                    {
                        break;
                    }

                    dv = new DataValues(name, map, chunkoffset, IFPIO.ObjectEnum.Float, lineNum);
                    dv.Controls.RemoveAt(dv.Controls.Count - 1);
                    c2 = dv;
                    sizeCount -= 4;
                    break;
                case "byte_flags":
                    if (sizeCount < 1)
                    {
                        break;
                    }

                    IFPIO.Option[] options = new IFPIO.Option[8];
                    for (int x = 0; x < 8; x++)
                    {
                        options[x] = new IFPIO.Option("bit " + x, x.ToString(), lineNum);
                    }

                    Bitmask bt = new Bitmask(name, map, chunkoffset, 8, options, lineNum);
                    bt.Controls.RemoveAt(bt.Controls.Count - 1);
                    c2 = bt;
                    sizeCount -= 1;
                    break;
                case "word_flags":
                    if (sizeCount < 2)
                    {
                        break;
                    }

                    options = new IFPIO.Option[16];
                    for (int x = 0; x < 16; x++)
                    {
                        options[x] = new IFPIO.Option("bit " + x, x.ToString(), lineNum);
                    }

                    bt = new Bitmask(name, map, chunkoffset, 16, options, lineNum);
                    bt.Controls.RemoveAt(bt.Controls.Count - 1);
                    c2 = bt;
                    sizeCount -= 2;
                    break;
                case "long_flags":
                    if (sizeCount < 4)
                    {
                        break;
                    }

                    options = new IFPIO.Option[32];
                    for (int x = 0; x < 32; x++)
                    {
                        options[x] = new IFPIO.Option("bit " + x, x.ToString(), lineNum);
                    }

                    bt = new Bitmask(name, map, chunkoffset, 32, options, lineNum);
                    bt.Controls.RemoveAt(bt.Controls.Count - 1);
                    c2 = bt;
                    sizeCount -= 4;
                    break;
            }

            if (c2 != null && sizeCount == 0)
            {
                c2.TabIndex = c.TabIndex;
                c2.Location = c.Location;
                c.Parent.Controls.Add(c2);
                c.Parent.Controls.SetChildIndex(c2, c.Parent.Controls.GetChildIndex(c));
                c2.Controls.Add((ComboBox)sender);
                c.Parent.Controls.Remove(c);
                c = c2.Parent;
                while (!(c is MetaEditor))
                {
                    c = c.Parent;
                }

                ((MetaEditor)c).ReloadMetaForSameTagType(true);
            }
        }

        /// <summary>
        /// The goto tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void identContext_Opening(object sender, CancelEventArgs e)
        {
            Control c = identContext.SourceControl;
            if (!c.Focused)
            {
                c.Focus();
            }

            gotoToolStripMenuItem.Visible = false;
            if (!(c.Parent is Ident))
            {
                return;
            }

            string tag = string.Empty;
            tag = "[" + c.Parent.Controls[2].Text + "] " + c.Text;
            c.Parent.Tag = tag;
            if (tag != string.Empty)
            {
                this.gotoToolStripMenuItem.Text = "Jump to \"" + tag + "\"";
                gotoToolStripMenuItem.Visible = true;
            }
        }

        /// <summary>
        /// The paste tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox c = identContext.SourceControl as ComboBox;
            if (Clipboard.GetText() != string.Empty)
            {
                c.SelectedText = Clipboard.GetText();
            }
        }

        /// <summary>
        /// The temp combo box_ key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void tempComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
            {
                Clipboard.SetDataObject(((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex], true);
            }

            if (e.KeyChar == 'a')
            {
                string tempStringToCopy = string.Empty;
                int ReflexiveChunkSelectionBoxItemLength = ((ComboBox)sender).Items.Count;
                for (int counter = 0; counter < ReflexiveChunkSelectionBoxItemLength; counter++)
                {
                    tempStringToCopy += ((ComboBox)sender).Items[counter] + Environment.NewLine;
                }

                Clipboard.SetDataObject(tempStringToCopy, true);
            }
        }

        #endregion
    }
}