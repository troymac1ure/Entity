using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HaloMap.Map;
using HaloMap.Meta;
using HaloMap.Plugins;

namespace entity.MetaEditor2
{
    public partial class MetaEditorControlPage : UserControl
    {
        #region Constants and Fields
        /// <summary>
        /// Our Binary Stream Reader & Writer variables, used for all populating & changing
        /// </summary>
        BinaryReader BR;
        BinaryWriter BW;
        /// <summary>
        /// Keeps a pointer to our currently selected control for when we hit "Restore Value" because the focus gets
        /// transferred to the button. This way we know what control we were on.
        /// </summary>
        BaseField CurrentControl;
        IFPIO ifp;
        Map map = null;
        MapForms.MapForm MapForm;
        Meta meta = null;
        /// <summary>
        /// A backup of the Meta.MS stream, used to define changes in the tag
        /// </summary>
        MemoryStream msBackup;
        /// <summary>
        /// Memory stream from a debug box, used for peeking and mass poking
        /// </summary>
        MemoryStream msDebug;
        Meta oldMeta = null;
        /// <summary>
        /// Keeps a list of any hidden reflexive nodes for quick show/hide of tree nodes
        /// </summary>
        TreeView tvHiddenNodes = new TreeView();

        /** Section for adding undo ability of external tag data
        private List<externalData> externalChanges = new List<externalData>();
      
        class externalData
        {
            public int offset;
            public byte[] value;

            public externalData(int offset, byte[] value)
            {
                this.offset = offset;
                this.value = value;
            }
        }

        public void addExternalListing(int offset, byte[] value)
        {
            foreach (externalData ed in externalChanges)
                if (ed.offset == offset)
                {
                    ed.value = value;
                    return;
                }
            
            externalChanges.Add(new externalData(offset, value));
        }

        public byte[] getExternalListing(int offset)
        {
            foreach (externalData ed in externalChanges)
                if (ed.offset == offset)
                    return ed.value;
            return new byte[0];
        }
        */

        public class reflexiveData
        {
            #region Constants and Fields
            public int baseOffset;
            public int chunkCount;
            public int chunkSelected;
            public int inTagNumber;
            public TreeNode node;
            public IFPIO.Reflexive reflexive;
            public ToolStripItem[] tsItems;
            #endregion

            #region Constructors and Destructors
            public reflexiveData()
            {
                node = null;
                reflexive = null;
                baseOffset = -1;
                chunkCount = 0;
                chunkSelected = -1;
                inTagNumber = -1;
            }
            #endregion
        }
        List<reflexiveData> refData = new List<reflexiveData>();
        #endregion

        #region Constructors and Destructors
        public MetaEditorControlPage(Meta meta, MapForms.MapForm mapForm)
        {
            InitializeComponent();

            this.MapForm = mapForm;
            this.map = mapForm.map;

            this.meta = meta;
            this.BR = new BinaryReader(meta.MS);
            // Create a backup of the Tags memory stream, for restoring, comparing, etc
            msBackup = new MemoryStream(meta.MS.ToArray());
            msDebug = new MemoryStream((int)meta.MS.Length);
                        
            createTreeListing();            
            this.treeViewTagReflexives.Sort();
            treeViewTagReflexives.SelectedNode = treeViewTagReflexives.Nodes[0];
        }
        #endregion

        #region Public Methods

        private void addReflexive(ToolStrip ts, String text)
        {
            ToolStripComboBox tsbc = new ToolStripComboBox();
            tsbc.AutoSize = false;
            tsbc.DropDownStyle = ComboBoxStyle.DropDownList;
            tsbc.DropDown += new EventHandler(tsbc_DropDown);
            tsbc.DropDownClosed += new EventHandler(tsbc_DropDownClosed);
            tsbc.SelectedIndexChanged += new EventHandler(tsbc_SelectedIndexChanged);
            tsbc.Size = new Size(49, 21);
            ts.Items.Insert(0, tsbc);
            if (text.Length > 23)
            {
                String t = text.Substring(0, 10) + "..." + text.Substring(text.Length - 10);
                text = t;
            }
            ToolStripLabel tsl = new ToolStripLabel(text);
            ts.Items.Insert(0, tsl);
            ToolStripSeparator tss = new ToolStripSeparator();
            ts.Items.Insert(0, tss);
        }

        /// <summary>
        /// Called before the panel is disposed of. Checks for any changes to the tag and propmts
        /// to save if changes found.
        /// </summary>
        void checkSave()
        {
            // Focus the Save button to make sure that the current field is updated by BaseField_Leave() function
            this.btnSave.Focus();

            byte[] bn = meta.MS.ToArray();
            byte[] bo = msBackup.ToArray();
            List<int> changedOffsets = new List<int>();
            for (int i = 0; i < bn.Length; i++)
                if (bn[i] != bo[i])
                    changedOffsets.Add(i);
            if (changedOffsets.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Changes were made to:\n[" + 
                                    this.meta.type + 
                                    "] " + 
                                    this.meta.name +
                                    "\nDo you wish to save?", "Save Changes?", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    btnSave_Click(this, null);
                }
                else
                {
                    meta.MS.Dispose();
                    meta.MS = new MemoryStream(msBackup.ToArray());
                }
            }
        }

        public bool checkSelectionInCurrentTag()
        {
            return (meta.TagIndex == ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber);
        }

        private void createTreeListing()
        {
            try
            {
                ifp = HaloMap.Plugins.IFPHashMap.GetIfp(meta.type, map.HaloVersion);

                #region Save info about our current Selected Node
                TreeNode node = treeViewTagReflexives.SelectedNode;
                string tempS = string.Empty;
                string[] path = new string[0];
                if (node != null)
                {
                    while (node.Level > 0)
                    {
                        tempS = "\\" + ((reflexiveData)node.Tag).reflexive.offset.ToString() + tempS;
                        node = node.Parent;
                    }
                    path = ("0" + tempS).Split('\\');
                }
                #endregion

                treeViewTagReflexives.Nodes.Clear();
                treeViewTagReflexives.Sorted = cbSortByName.Checked;
                treeViewTagReflexives.Nodes.Add("0", ".:[ MAIN ]:.");
                reflexiveData rd = new reflexiveData();
                treeViewTagReflexives.Nodes[0].Tag = rd;
                rd.node = treeViewTagReflexives.Nodes[0];
                rd.chunkCount = 1;
                rd.chunkSelected = 0;
                rd.baseOffset = 0; // meta.offset;
                rd.inTagNumber = meta.TagIndex;
                refData.Clear();
                refData.Add(rd);

                map.OpenMap(MapTypes.Internal);

                treeViewTagReflexives.Nodes[0].Nodes.AddRange(loadTreeReflexives(meta.offset, ifp.items, true));               

                map.CloseMap();

                //treeViewTagReflexives.ExpandAll();
                treeViewTagReflexives.Nodes[0].Expand();

                #region Re-Select our previously selected node
                TreeNodeCollection nodes = treeViewTagReflexives.Nodes[0].Nodes;
                treeViewTagReflexives.Enabled = false;
                treeViewTagReflexives.SelectedNode = treeViewTagReflexives.Nodes[0];
                for (int i = 1; i < path.Length; i++)
                {
                    foreach (TreeNode tn in nodes)
                    {
                        if (((reflexiveData)tn.Tag).reflexive.offset.ToString() == path[i])
                        {
                            treeViewTagReflexives.SelectedNode = tn;
                            nodes = tn.Nodes;
                            break;
                        }
                    }
                }
                // If we didn't get the right node, deselect all nodes
                if (treeViewTagReflexives.SelectedNode.Level != path.Length - 1)
                    treeViewTagReflexives.SelectedNode = null;
                treeViewTagReflexives.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                Globals.Global.ShowErrorMsg(string.Empty, ex);
            }
        }

        private TreeNode findNodeOffset(TreeNodeCollection tns, int offset)
        {
            TreeNode tn = null;
            for (int i = 0; i < tns.Count; i++)
            {
                int ofs = ((reflexiveData)tns[i].Tag).baseOffset;
                if (((reflexiveData)tns[i].Tag).inTagNumber == meta.TagIndex)
                    ofs += meta.offset;
                if (ofs == offset)
                    return tns[i];
                if (tns[i].Nodes.Count > 0)
                    tn = findNodeOffset(tns[i].Nodes, offset);
                if (tn != null)
                    break;
            }
            return tn;
        }

        public void gotoOffset(int offset)
        {
            foreach (Meta.Item mi in this.meta.items)
            {
                if (!(mi is Meta.Reflexive))
                    continue;

                map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = mi.mapOffset;
                int count = map.BR.ReadInt32();
                int ofs = map.BR.ReadInt32() - meta.magic;
                map.CloseMap();

                for (int i = 0; i < count; i++)
                    if ((ofs + ((Meta.Reflexive)mi).chunksize * i) == offset)
                    {
                        treeViewTagReflexives.SelectedNode = findNodeOffset(treeViewTagReflexives.Nodes[0].Nodes, ofs);
                        return;
                    }
            }
        }

        private void loadControls(TreeNode Location)
        {
            toolTip1.AutoPopDelay = 10000;
            reflexiveData rd = (reflexiveData)Location.Tag;

            if (rd.chunkCount > 100000) // Some very large number
                throw new Exception("\"" + rd.node.Text + "\" appears to contain " + rd.chunkCount + "chunks!\n"
                    + "Try reloading tag. If problem persists, you likely have a corrupt map!");

            int chNum = rd.chunkSelected;
            bool enabled = rd.chunkCount != 0;
            int metaOffset = rd.baseOffset;

            // Make our panel non-visibile while we create our controls
            Panel panel = this.panelMetaEditor;
            panel.Visible = false;

            // Dispose of each control and also add a safety  in case we somehow go into an endless loop (max 1000)
            int safetyCount = 1000;
            while (panel.Controls.Count > 0 && safetyCount > 0)
            {
                panel.Controls[0].Dispose();
                safetyCount--;
            }
            // Should be empty, but clear anyways.
            panel.Controls.Clear();

            Object[] o;
            if (rd.reflexive == null)
                o = ifp.items;
            else
                o = rd.reflexive.items;

            bool labelFound = false;
            int tabIndex = 0;
            foreach (IFPIO.BaseObject ctl in o)
            {
                tabIndex++;
                switch (ctl.ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        {
#if DEBUG
                            if (((IFPIO.Reflexive)ctl).chunkSize == 1)
                            {
                                StringBox sb = new StringBox(meta, ctl.name + " (reflexive)", map, ctl.offset, ctl.lineNumber, 0);
                                if (enabled) sb.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                                if (sb.size > 0)
                                {
                                    panelMetaEditor.Controls.Add(sb);
                                    sb.BringToFront();
                                }
                                //panelMetaEditor.Controls[0].Controls[2].GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                            }
#endif
                            /*
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
                            ReflexiveControl tempReflexive = new ReflexiveControl(mapnumber, meta.offset, ((IFPIO.Reflexive)ctl).HasCount, ctl.lineNumber, this);
                            //tempReflexive.Location = new System.Drawing.Point(10, 0);
                            tempReflexive.Name = "reflexive";
                            tempReflexive.TabIndex = tabIndex;
                            tempReflexive.LoadENTControls((IFPIO.Reflexive)ctl, ((IFPIO.Reflexive)ctl).items,
                                                           true, 0, ref tabIndex, ctl.offset.ToString());

                            // Label, Combobox & Button are always added ( = 3)
                            if (!(tempReflexive.Controls.Count <= 2 && skipEmptyReflex))
                            {
                                panelMetaEditor.Controls.Add(tempLabel);
                                panelMetaEditor.Controls[panelMetaEditor.Controls.Count - 1].BringToFront();
                                panelMetaEditor.Controls.Add(tempReflexive);
                                panelMetaEditor.Controls[panelMetaEditor.Controls.Count - 1].BringToFront();
                            }
                            break;
                            */
                            continue;
                        }
                    case IFPIO.ObjectEnum.Block:
                        {
                            Ident tempident = new Ident(meta, ctl.name, map, ctl.offset + 4, true, ctl.lineNumber);
                            tempident.Name = "ident";
                            tempident.TabIndex = tabIndex;
                            if (enabled) tempident.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            tempident.Tag = "[" + tempident.Controls[2].Text + "] "
                                            + tempident.Controls[1].Text;
                            //tempident.Controls[1].ContextMenuStrip = identContext;
                            panelMetaEditor.Controls.Add(tempident);
                            tempident.BringToFront();
                            panelMetaEditor.Controls[0].Controls[2].GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                            tempident.ContextMenuStrip = cmIdent;
                            break;
                            break;
                        }
                    case IFPIO.ObjectEnum.TagType:
                        continue;
                    case IFPIO.ObjectEnum.Ident:
                        {
                            if (MetaEditor.MetaEditor.ShowIdents == false)
                                break;
                            Ident tempident = new Ident(meta, ctl.name, map, ctl.offset, ((IFPIO.Ident)ctl).hasTagType, ctl.lineNumber);
                            tempident.Name = "ident";
                            tempident.TabIndex = tabIndex;
                            if (enabled) tempident.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            tempident.Tag = "[" + tempident.Controls[2].Text + "] "
                                            + tempident.Controls[1].Text;
                            //tempident.Controls[1].ContextMenuStrip = identContext;
                            panelMetaEditor.Controls.Add(tempident);
                            tempident.BringToFront();
                            panelMetaEditor.Controls[0].Controls[1].MouseEnter += new EventHandler(cbIdent_MouseEnter);
                            panelMetaEditor.Controls[0].Controls[1].MouseLeave += new EventHandler(cbIdent_MouseLeave);
                            panelMetaEditor.Controls[0].Controls[2].GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                            tempident.ContextMenuStrip = cmIdent;
                            break;
                        }
                    case IFPIO.ObjectEnum.StringID:
                        {
                            if (MetaEditor.MetaEditor.ShowSIDs == false)
                                break;
                            SID tempSID = new SID(meta, ctl.name, map, ctl.offset, ctl.lineNumber);
                            tempSID.Name = "sid";
                            tempSID.TabIndex = tabIndex;
                            if (enabled) tempSID.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempSID);
                            tempSID.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Float:
                        {
                            if (MetaEditor.MetaEditor.ShowFloats == false)
                                break;
                            DataValues tempFloat = new DataValues(meta, ctl.name, map, ctl.offset, IFPIO.ObjectEnum.Float, ctl.lineNumber);
                            tempFloat.TabIndex = tabIndex;
                            if (enabled) tempFloat.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempFloat);
                            tempFloat.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.String32:
                        {
                            if (MetaEditor.MetaEditor.ShowString32s == false && ctl.ObjectType == IFPIO.ObjectEnum.String32)
                                break;
                            EntStrings tempstring = new EntStrings(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPString)ctl).size, ((IFPIO.IFPString)ctl).type, ctl.lineNumber);
                            tempstring.Name = "string";
                            tempstring.TabIndex = tabIndex;
                            panelMetaEditor.Controls.Add(tempstring);
                            if (enabled) tempstring.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            tempstring.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.UnicodeString256:
                        {
                            if (MetaEditor.MetaEditor.ShowUnicodeString256s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;
                        }
                    case IFPIO.ObjectEnum.String256:
                        {
                            if (MetaEditor.MetaEditor.ShowString256s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;
                        }
                    case IFPIO.ObjectEnum.UnicodeString64:
                        {
                            if (MetaEditor.MetaEditor.ShowUnicodeString64s == false)
                                break;
                            goto case IFPIO.ObjectEnum.String32;
                        }
                    case IFPIO.ObjectEnum.String:
                        {
                            if (MetaEditor.MetaEditor.ShowString32s == false && ctl.ObjectType == IFPIO.ObjectEnum.String32)
                                break;
                            EntStrings tempstring = new EntStrings(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPString)ctl).size, ((IFPIO.IFPString)ctl).type, ctl.lineNumber);
                            tempstring.Name = "string";
                            tempstring.TabIndex = tabIndex;
                            if (enabled) tempstring.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempstring);
                            tempstring.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte:
                        {
                            if (((IFPIO.IFPByte)ctl).entIndex.nulled == true)
                            {
                                if (MetaEditor.MetaEditor.ShowBytes == false)
                                    break;
                                DataValues tempByte = new DataValues(meta, ctl.name, map, ctl.offset, IFPIO.ObjectEnum.Byte, ctl.lineNumber);
                                tempByte.TabIndex = tabIndex;
                                if (enabled) tempByte.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                                panelMetaEditor.Controls.Add(tempByte);
                                tempByte.BringToFront();
                            }
                            else
                            {
                                if (MetaEditor.MetaEditor.ShowBlockIndex8s == false)
                                    break;
                                Indices tempDataValues = new Indices(meta, ctl.name, map, ctl.offset, ctl.ObjectType, ((IFPIO.IFPByte)ctl).entIndex);
                                tempDataValues.TabIndex = tabIndex;
                                panelMetaEditor.Controls.Add(tempDataValues);
                                tempDataValues.BringToFront();
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Int:
                        {
                            if (((IFPIO.IFPInt)ctl).entIndex.nulled == true)
                            {
                                if ((MetaEditor.MetaEditor.ShowInts == false && ctl.ObjectType == IFPIO.ObjectEnum.Int)
                                    || (MetaEditor.MetaEditor.ShowShorts == false && ctl.ObjectType == IFPIO.ObjectEnum.Short)
                                    || (MetaEditor.MetaEditor.ShowUshorts == false && ctl.ObjectType == IFPIO.ObjectEnum.UShort)
                                    || (MetaEditor.MetaEditor.ShowUints == false && ctl.ObjectType == IFPIO.ObjectEnum.UInt))
                                    break;
                                DataValues tempdatavalues = new DataValues(meta, ctl.name, map, ctl.offset, ctl.ObjectType, ctl.lineNumber);
                                tempdatavalues.TabIndex = tabIndex;
                                if (enabled) tempdatavalues.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                                panelMetaEditor.Controls.Add(tempdatavalues);
                                tempdatavalues.BringToFront();
                            }
                            else
                            {
                                if ((MetaEditor.MetaEditor.ShowBlockIndex32s == false && (ctl.ObjectType == IFPIO.ObjectEnum.Int | ctl.ObjectType == IFPIO.ObjectEnum.UInt))
                                    || (MetaEditor.MetaEditor.ShowBlockIndex16s == false && (ctl.ObjectType == IFPIO.ObjectEnum.Short | ctl.ObjectType == IFPIO.ObjectEnum.UShort))
                                    || (MetaEditor.MetaEditor.ShowBlockIndex8s == false && ctl.ObjectType == IFPIO.ObjectEnum.Byte))
                                    break;
                                Indices tempdatavalues = new Indices(meta, ctl.name, map, ctl.offset, ctl.ObjectType, ((IFPIO.IFPInt)ctl).entIndex);
                                tempdatavalues.TabIndex = tabIndex;
                                panelMetaEditor.Controls.Add(tempdatavalues);
                                tempdatavalues.BringToFront();
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
                            if (MetaEditor.MetaEditor.ShowUndefineds == false)
                                break;
                            DataValues tempUnknown = new DataValues(meta, ctl.name, map, ctl.offset, IFPIO.ObjectEnum.Unknown, ctl.lineNumber);
                            tempUnknown.TabIndex = tabIndex;
                            if (enabled) tempUnknown.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempUnknown);
                            tempUnknown.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte_Flags:
                        {
                            if (MetaEditor.MetaEditor.ShowBitmask8s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(meta, ctl.name, map, ctl.offset, ((IFPIO.Bitmask)ctl).bitmaskSize, ((IFPIO.Bitmask)ctl).options, ctl.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            if (enabled) tempbitmask.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempbitmask);
                            tempbitmask.BringToFront();
                            foreach (Control cntl in tempbitmask.Controls[0].Controls)
                            {
                                cntl.GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                                toolTip1.SetToolTip(cntl, "Bit " + cntl.Tag.ToString() + " (Value = " + (1 << int.Parse(cntl.Tag.ToString())).ToString() + ")");
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Word_Flags:
                        {
                            if (MetaEditor.MetaEditor.ShowBitmask16s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(meta, ctl.name, map, ctl.offset, ((IFPIO.Bitmask)ctl).bitmaskSize, ((IFPIO.Bitmask)ctl).options, ctl.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            if (enabled) tempbitmask.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempbitmask);
                            tempbitmask.BringToFront();
                            foreach (Control cntl in tempbitmask.Controls[0].Controls)
                            {
                                cntl.GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                                toolTip1.SetToolTip(cntl, "Bit " + cntl.Tag.ToString() + " (Value = " + (1 << int.Parse(cntl.Tag.ToString())).ToString() + ")");
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Long_Flags:
                        {
                            if (MetaEditor.MetaEditor.ShowBitmask32s == false)
                                break;
                            Bitmask tempbitmask = new Bitmask(meta, ctl.name, map, ctl.offset, ((IFPIO.Bitmask)ctl).bitmaskSize, ((IFPIO.Bitmask)ctl).options, ctl.lineNumber);
                            tempbitmask.TabIndex = tabIndex;
                            if (enabled) tempbitmask.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempbitmask);
                            tempbitmask.BringToFront();

                            foreach (Control cntl in tempbitmask.Controls[0].Controls)
                            {
                                cntl.GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                                toolTip1.SetToolTip(cntl, "Bit " + cntl.Tag.ToString() + " (Value = " + (1 << int.Parse(cntl.Tag.ToString())).ToString() + ")");
                            }
                            break;
                        }
                    case IFPIO.ObjectEnum.Char_Enum:
                        {
                            if (MetaEditor.MetaEditor.ShowEnum8s == false)
                                break;
                            Enums tempenum = new Enums(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPEnum)ctl).enumSize, ((IFPIO.IFPEnum)ctl).options, ctl.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            if (enabled) tempenum.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempenum);
                            tempenum.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Enum:
                        {
                            if (MetaEditor.MetaEditor.ShowEnum16s == false)
                                break;
                            Enums tempenum = new Enums(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPEnum)ctl).enumSize, ((IFPIO.IFPEnum)ctl).options, ctl.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            if (enabled) tempenum.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempenum);
                            tempenum.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Long_Enum:
                        {
                            if (MetaEditor.MetaEditor.ShowEnum32s == false)
                                break;
                            Enums tempenum = new Enums(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPEnum)ctl).enumSize, ((IFPIO.IFPEnum)ctl).options, ctl.lineNumber);
                            tempenum.TabIndex = tabIndex;
                            if (enabled) tempenum.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempenum);
                            tempenum.BringToFront();
                            break;
                        }
                    case IFPIO.ObjectEnum.Unused:
                        {
                            DataValues tempUnknown = new DataValues(meta, ctl.name, map, ctl.offset, IFPIO.ObjectEnum.Unused, ctl.lineNumber);
                            tempUnknown.size = ((IFPIO.Unused)ctl).size;
                            tempUnknown.TabIndex = tabIndex;
                            if (enabled) tempUnknown.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            if (tempUnknown.ValueType == IFPIO.ObjectEnum.Unused)
                                tempUnknown.textBox1.Text = "unused (size " + tempUnknown.size + ")";
                            panelMetaEditor.Controls.Add(tempUnknown);
                            tempUnknown.BringToFront();

                            tempUnknown.meta.MS.Position = metaOffset + ctl.offset;
                            byte[] tempB = new byte[((IFPIO.Unused)ctl).size];
                            tempUnknown.meta.MS.Read(tempB, 0, tempB.Length);
                            toolTip1.SetToolTip(tempUnknown.Controls[1], toHex(tempB));
                            toolTip1.SetToolTip(tempUnknown.Controls[1], BitConverter.ToString(tempB).Replace('-', ' '));
                            toolTip1.IsBalloon = true;
                            break;
                        }
                    case IFPIO.ObjectEnum.ARGB_Color:
                        {
                            if (MetaEditor.MetaEditor.ShowFloats == false)
                                break;
                            argb_color tempARGBColor = new argb_color(meta, ctl.name, map, ctl.offset, ((IFPIO.IFPColor)ctl).hasAlpha, ((IFPIO.IFPColor)ctl).type, ctl.lineNumber);
                            tempARGBColor.TabIndex = tabIndex;
                            if (enabled) tempARGBColor.Populate(metaOffset, rd.inTagNumber == meta.TagIndex);
                            panelMetaEditor.Controls.Add(tempARGBColor);
                            tempARGBColor.BringToFront();
                            break;
                        }
                    default:
                        break;
                }

#if DEBUG
                if (((BaseField)panelMetaEditor.Controls[0]).size == 0)
                {
                    string s = panelMetaEditor.Controls[0].GetType().ToString();
                    switch (panelMetaEditor.Controls[0].GetType().ToString())
                    {
                        case "entity.MetaEditor2.DataValues":
                            if (((DataValues)panelMetaEditor.Controls[0]).ValueType != IFPIO.ObjectEnum.Unused)
                                MessageBox.Show("WARNING: 0 Sized control: " + ((DataValues)panelMetaEditor.Controls[0]).ValueType);
                            break;
                        default:
                            MessageBox.Show("WARNING: 0 Sized control: " + panelMetaEditor.Controls[0].Name);
                            break;
                    }
                }
#endif

                panelMetaEditor.Controls[0].Controls[1].GotFocus += new EventHandler(MetaEditorControlPage_GotFocus);
                if (ctl.ObjectType != IFPIO.ObjectEnum.Struct)
                {
                    int temp = ctl.offset;
                    // Take into account that idents actually start at -4 due to tags preceding
                    if (ctl.ObjectType == IFPIO.ObjectEnum.Ident)
                        temp -= 4;

                    string tip = 
                            "offset in reflexive: " + (temp).ToString() + " (" + toHex(temp) + ")" +
                            "\n         offset in tag: " + (rd.baseOffset + temp).ToString() + " (" + toHex(rd.baseOffset + temp) + ")" +
                            "\n        offset in map: " + (rd.baseOffset + meta.offset + temp).ToString() + " (" + toHex(rd.baseOffset + meta.offset + temp) + ")";

                    if (panelMetaEditor.Controls[0] is Bitmask)
                    {                        
                        Bitmask tempbitmask = ((Bitmask)panelMetaEditor.Controls[0]);
                        int bitValue = int.Parse(tempbitmask.Value);
                        string s = " = 0x" + bitValue.ToString("X4") + " (" + tempbitmask.Value + ")";
                        for (int i = 0; i < 32; i++)
                        {
                            s = ((1 << i & bitValue) == 0 ? "0" : "1") + s;
                            if (i % 4 == 3)
                                s = " " + s;
                        }
                        tip += "\n\nBITMASK VALUE:\n" + s;
                    }

                    toolTip1.SetToolTip(panelMetaEditor.Controls[0].Controls[0], tip);                            

                }

            }
            // Restore our panel visibility
            panel.Visible = true;
        }


        /// <summary>
        /// Loads all the labels into the dropdown selection box.
        /// </summary>
        string[] loadLabels(reflexiveData rd)
        {
            List<string> ls = new List<string>();
            IFPIO.BaseObject bo = null;

            if (rd.reflexive.label == "")
                return new string[rd.chunkCount];
            for (int i = 0; i <= rd.reflexive.items.Length; i++)
            {
                if (rd.reflexive.label == ((IFPIO.BaseObject)rd.reflexive.items[i]).name)
                {
                    bo = (IFPIO.BaseObject)rd.reflexive.items[i];
                    break;
                }
            }

            // If label does not exist, then return an empty array
            if (bo == null)
                return new string[rd.chunkCount];

            BinaryReader BR = new BinaryReader(meta.MS);

            // baseOffset contains the offset to the 0 index of the reflexive
            int baseOffset = rd.baseOffset - (rd.reflexive.chunkSize * rd.chunkSelected);
            // All values must return "s" which will be returned to the caller in a string array
            string s = null;
            // l is a general integer value for whatever
            int l;

            for (int i = 0; i < rd.chunkCount; i++)
            {
                BR.BaseStream.Position = baseOffset + (rd.reflexive.chunkSize * i) + bo.offset;
                IFPIO.ObjectEnum ObjType = bo.ObjectType;
                s = string.Empty;

                #region For Block Indexes (Indices)
                if (bo is IFPIO.IFPInt && (((IFPIO.IFPInt)bo)).entIndex != null)
                {
                    int value = -1;
                    switch (ObjType)
                    {
                        case IFPIO.ObjectEnum.Byte:
                            value = BR.ReadByte();
                            break;
                        case IFPIO.ObjectEnum.Short:
                            value = BR.ReadInt16();
                            break;
                        case IFPIO.ObjectEnum.UInt:
                            value = (int)BR.ReadUInt32();
                            break;
                        case IFPIO.ObjectEnum.UShort:
                            value = BR.ReadUInt16();
                            break;
                        case IFPIO.ObjectEnum.Int:
                            value = BR.ReadInt32();
                            break;
                    }
                    s += value.ToString() + ": ";
                    if (((IFPIO.IFPInt)bo).entIndex.reflexiveLayer == "root")
                    {
                        BR.BaseStream.Position = (((IFPIO.IFPInt)bo).entIndex.ReflexiveOffset);
                        int count = BR.ReadInt32();
                        int ofs = BR.ReadInt32() - meta.magic - meta.offset;
                        ofs += (((IFPIO.IFPInt)bo).entIndex.ReflexiveSize * value) + ((IFPIO.IFPInt)bo).entIndex.ItemOffset;
                        BR.BaseStream.Position = ofs;
                    }
                    else
                    {
                    }

                    switch (((IFPIO.IFPInt)bo).entIndex.ItemType.ToLower())
                    {
                        case "ident":
                            ObjType = IFPIO.ObjectEnum.Ident;
                            break;
                        case "string32":
                            ObjType = IFPIO.ObjectEnum.String32;
                            break;
                        case "string256":
                            ObjType = IFPIO.ObjectEnum.String256;
                            break;
                        case "stringid":
                            ObjType = IFPIO.ObjectEnum.StringID;
                            break;
                        default:
                            MessageBox.Show("Didn't add support for Index Block type \"" + ((IFPIO.IFPInt)bo).entIndex.ItemType + "\". Please inform developer.");
                            ObjType = IFPIO.ObjectEnum.Unused;
                            break;
                    }
                }
                #endregion

                switch (ObjType)
                {
                    #region Enum Labels
                    case IFPIO.ObjectEnum.Char_Enum:
                    case IFPIO.ObjectEnum.Enum:
                    case IFPIO.ObjectEnum.Long_Enum:
                        switch (((IFPIO.IFPEnum)bo).enumSize)
                        {
                            case 8:
                                l = (int)BR.ReadByte();
                                s += ((IFPIO.BaseObject)((IFPIO.IFPEnum)bo).options[l]).name;
                                break;
                            case 16:
                                l = BR.ReadInt16();
                                s += ((IFPIO.BaseObject)((IFPIO.IFPEnum)bo).options[l]).name;
                                break;
                            case 32:
                                l = BR.ReadInt32();
                                s += ((IFPIO.BaseObject)((IFPIO.IFPEnum)bo).options[l]).name;
                                break;
                        }
                        break;
                    #endregion
                    #region Bitmask Labels
                    case IFPIO.ObjectEnum.Byte_Flags:
                    case IFPIO.ObjectEnum.Word_Flags:
                    case IFPIO.ObjectEnum.Long_Flags:
                        l = 0;
                        switch (((IFPIO.Bitmask)bo).bitmaskSize)
                        {
                            case 8:
                                l = (int)BR.ReadByte();
                                break;
                            case 16:
                                l = (int)BR.ReadUInt16();
                                break;
                            case 32:
                                l = (int)BR.ReadUInt32();
                                break;
                        }
                        for (int ll = 0; ll < ((IFPIO.Bitmask)bo).bitmaskSize; ll++)
                        {
                            s += ((l >> ll) & 1) + s;
                        }
                        break;
                    #endregion
                    #region Tag / Ident Labels
                    case IFPIO.ObjectEnum.TagType:
                        l = BR.ReadInt32();
                        goto case IFPIO.ObjectEnum.Ident;
                    case IFPIO.ObjectEnum.Ident:
                        l = BR.ReadInt32();
                        l = map.Functions.ForMeta.FindMetaByID(l);
                        if (l != -1)
                        {
                            s += "[" + map.MetaInfo.TagType[l] + "] " +
                                map.FileNames.Name[l];
                        }
                        else
                        {
                            s += "[null] null";
                        }
                        break;
                    #endregion
                    #region Number Data Value Labels (Int, Float, Byt, UShort, etc)
                    case IFPIO.ObjectEnum.Byte:
                        l = BR.ReadByte();
                        s += (char)l + " (" + l.ToString() + ")";
                        break;
                    case IFPIO.ObjectEnum.Float:
                    case IFPIO.ObjectEnum.Unknown:
                        s += BR.ReadSingle().ToString();
                        break;
                    case IFPIO.ObjectEnum.Short:
                        s += BR.ReadInt16().ToString();
                        break;
                    case IFPIO.ObjectEnum.UInt:
                        s += BR.ReadUInt32().ToString();
                        break;
                    case IFPIO.ObjectEnum.UShort:
                        s += BR.ReadUInt16().ToString();
                        break;
                    case IFPIO.ObjectEnum.Int:
                        s += BR.ReadInt32().ToString();
                        break;
                    #endregion
                    #region String ID Labels
                    case IFPIO.ObjectEnum.StringID:
                        short sidIndexer = BR.ReadInt16();
                        byte tempnull = BR.ReadByte();
                        byte sidLength = BR.ReadByte();
                        s += map.Strings.Name[sidIndexer];
                        if (s.Length != sidLength)
                            s = null;
                        break;
                    #endregion
                    #region String Labels
                    case IFPIO.ObjectEnum.UnicodeString256:
                    case IFPIO.ObjectEnum.UnicodeString64:
                        Encoding decode = Encoding.Unicode;
                        byte[] tempbytes = BR.ReadBytes(((IFPIO.IFPString)bo).size);
                        s += decode.GetString(tempbytes);
                        s = s.TrimEnd('\0');
                        break;
                    case IFPIO.ObjectEnum.String32:
                    case IFPIO.ObjectEnum.String256:
                        s += new string(BR.ReadChars(((IFPIO.IFPString)bo).size));
                        s = s.TrimEnd('\0');
                        break;
                    #endregion
                    default:
                        throw new Exception("Whoops! Forgot to code in \"" + bo.ObjectType.ToString() + "\" data type for labels!\n" +
                                            "Slap the programmer and tell him about this message!");
                        return new string[rd.chunkCount];
                }
                ls.Add(s);
            }
            return ls.ToArray();
        }

        public TreeNode[] loadTreeReflexives(int metaOffset, object[] items, bool enabled)
        {
            List<TreeNode> tns = new List<TreeNode>();
            foreach (object o in items)
            {
                if (o is IFPIO.Reflexive)
                {
                    IFPIO.Reflexive IFPR = (IFPIO.Reflexive)o;
                    TreeNode tn = new TreeNode(IFPR.name);
                    reflexiveData rd = new reflexiveData();
                    tn.Tag = rd;
                    rd.node = tn;
                    rd.reflexive = IFPR;
                    tn.ForeColor = Color.LightGray;
                    tn.Name = IFPR.name;
                    tn.ToolTipText = "Offset: " + rd.reflexive.offset.ToString();

                    if (enabled)
                    {
                        map.BR.BaseStream.Position = IFPR.offset + metaOffset;
                        rd.chunkCount = map.BR.ReadInt32();
                        if (rd.chunkCount > 0)
                        {
                            rd.chunkSelected = 0;
                            tn.ForeColor = Color.Black;
                            rd.baseOffset = map.BR.ReadInt32() - meta.magic;
                            // Some listings are actually in other tags!
                            // Check [BLOC] "objects\\vehicles\\wraith\\garbage\\wing_boost\\wing_boost"
                            //   > attachments[0] = [BLOC] "objects\\vehicles\\ghost\\garbage\\seat\\seat"
                            rd.inTagNumber = map.Functions.ForMeta.FindMetaByOffset(rd.baseOffset);
                            if (rd.inTagNumber == -1)
                            {
                                tn.ToolTipText = "DATA ERROR! Possibly corrupt data or incorrect plugin.";
                                rd.chunkCount = 0;
                                rd.chunkSelected = -1;
                                continue;
                            }
                            if (rd.inTagNumber == meta.TagIndex)
                                rd.baseOffset -= meta.offset;
                            else
                            {
                                tn.ForeColor = Color.Red;
                                tn.ToolTipText = "Data Source Located in:\n[" + map.MetaInfo.TagType[rd.inTagNumber].ToLower() +
                                                "] " + map.FileNames.Name[rd.inTagNumber].ToLower();
                            }

                        }
                    }
                    tn.Text += " [" + rd.chunkCount.ToString() + "]";

                    refData.Add(rd);

                    // Add if non-existant, otherwise update Text
                    if (rd.inTagNumber == meta.TagIndex)
                        tn.Nodes.AddRange(loadTreeReflexives(meta.offset + rd.baseOffset + rd.chunkSelected * rd.reflexive.chunkSize, IFPR.items, rd.chunkCount != 0));
                    else
                        tn.Nodes.AddRange(loadTreeReflexives(rd.baseOffset + rd.chunkSelected * rd.reflexive.chunkSize, IFPR.items, rd.chunkCount != 0));
                    
                    if (rd.chunkCount != 0 | !cbHideUnused.Checked)
                        tns.Add(tn);
                }
            }
            return tns.ToArray();
        }

        private void refreshTreeListing(TreeNode parent)
        {
            if (((reflexiveData)parent.Tag).chunkCount == 0)
                return;

            treeViewTagReflexives.SuspendLayout();
            // See if we are in the MAIN of the tag
            if (parent != treeViewTagReflexives.Nodes[0])
            {
                reflexiveData rd = (reflexiveData)parent.Tag;
                if (rd.inTagNumber != meta.TagIndex && ((reflexiveData)parent.Parent.Tag).inTagNumber != meta.TagIndex)
                {
                    map.OpenMap(MapTypes.Internal);
                    BR = map.BR;
                    BR.BaseStream.Position = ((reflexiveData)parent.Parent.Tag).baseOffset + rd.reflexive.offset;
                }
                else
                {
                    BR = new BinaryReader(meta.MS);
                    BR.BaseStream.Position = ((reflexiveData)parent.Parent.Tag).baseOffset + rd.reflexive.offset;
                }
                rd.chunkCount = BR.ReadInt32();
                rd.baseOffset = BR.ReadInt32() - meta.magic + rd.chunkSelected * rd.reflexive.chunkSize;                

                rd.inTagNumber = map.Functions.ForMeta.FindMetaByOffset(rd.baseOffset);
                if (rd.inTagNumber == meta.TagIndex)
                {
                    rd.baseOffset -= meta.offset;
                    parent.ForeColor = Color.Black;
                    parent.ToolTipText = "Offset: " + rd.reflexive.offset.ToString();
                }
                else
                {
                    map.CloseMap();
                    if (rd.inTagNumber != -1)
                    {
                        parent.ForeColor = Color.Red;
                        parent.ToolTipText = "Data Source Located in:\n[" + map.MetaInfo.TagType[rd.inTagNumber].ToLower() +
                                        "] " + map.FileNames.Name[rd.inTagNumber].ToLower();
                    }
                }
            }
            refreshTreeSubNodes(parent);
            treeViewTagReflexives.ResumeLayout();
        }

        private void refreshTreeSubNodes(TreeNode parent)
        {
            foreach (TreeNode tn in parent.Nodes)
            {
                reflexiveData rd = (reflexiveData)tn.Tag;
                if (rd.inTagNumber != meta.TagIndex)
                {
                    map.OpenMap(MapTypes.Internal);
                    BR = map.BR;
                    BR.BaseStream.Position = ((reflexiveData)parent.Tag).baseOffset + rd.reflexive.offset;
                    if (((reflexiveData)parent.Tag).inTagNumber == meta.TagIndex)
                        BR.BaseStream.Position += meta.offset;
                }
                else
                {
                    BR = new BinaryReader(meta.MS);
                    BR.BaseStream.Position = ((reflexiveData)parent.Tag).baseOffset + rd.reflexive.offset;
                }
                rd.chunkCount = BR.ReadInt32();
                rd.baseOffset = BR.ReadInt32() - meta.magic + rd.chunkSelected * rd.reflexive.chunkSize;

                rd.inTagNumber = map.Functions.ForMeta.FindMetaByOffset(rd.baseOffset);
                if (rd.inTagNumber == meta.TagIndex)
                    rd.baseOffset -= meta.offset;
                else
                {
                    map.CloseMap();
                    if (rd.inTagNumber != -1)
                    {
                        tn.ForeColor = Color.Red;
                        tn.ToolTipText = "Data Source Located in:\n[" + map.MetaInfo.TagType[rd.inTagNumber].ToLower() +
                                        "] " + map.FileNames.Name[rd.inTagNumber].ToLower();
                    }
                }
                tn.Text = tn.Name + " [" + rd.chunkCount.ToString() + "]";
                refreshTreeListing(tn);
            }
        }

        public void ReloadMetaForSameReflexive(int offset)
        {
            reflexiveData rd = ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag);
            map.OpenMap(MapTypes.Internal);

            for (int counter = 0; counter < this.panelMetaEditor.Controls.Count; counter++)
            {
                switch (this.panelMetaEditor.Controls[counter].GetType().ToString())
                {
                    case "entity.MetaEditor2.SID":
                        {
                            ((SID)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    /*
                    case "entity.MetaEditor2.ReflexiveControl":
                        {
                            if (reloadreflexive == false)
                                break;
                            ((ReflexiveControl)this.panelMetaEditor.Controls[counter]).LoadMetaIntoControls(offset, offset);
                            break;
                        }
                    */
                    case "entity.MetaEditor2.Ident":
                        {
                            ((Ident)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    case "entity.MetaEditor2.EntStrings":
                        {
                            ((EntStrings)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    case "entity.MetaEditor2.Bitmask":
                        {
                            ((Bitmask)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    case "entity.MetaEditor2.DataValues":
                        {
                            ((DataValues)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    case "entity.MetaEditor2.Enums":
                        {
                            ((Enums)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    case "entity.MetaEditor2.Indices":
                        {
                            ((Indices)this.panelMetaEditor.Controls[counter]).Populate(offset, ((reflexiveData)rd.node.Parent.Tag).baseOffset);
                            break;
                        }
                    case "entity.MetaEditor2.StringBox":
                        break;
                    case "entity.MetaEditor2.argb_color":
                        {
                            ((argb_color)this.panelMetaEditor.Controls[counter]).Populate(offset, rd.inTagNumber == meta.TagIndex);
                            break;
                        }
                    default:
                        throw new Exception("Unhandled type: " + this.panelMetaEditor.Controls[counter].GetType().ToString());
                        break;
                }

                int temp = ((BaseField)this.panelMetaEditor.Controls[counter]).offsetInMap;
                // Take into account that idents actually start at -4 due to tags preceding
                if (this.panelMetaEditor.Controls[counter].GetType().ToString() == "entity.MetaEditor2.Ident")
                    temp -= 4;

                int tagnum = map.Functions.ForMeta.FindMetaByOffset(temp);

                // ****************
                // base offset = location of reflexive information (count & tag 0 offset)
                // tag offset is location of actual data!
                // ****************

                try
                {
                    string tip =
                            "offset in reflexive: " + (((BaseField)this.panelMetaEditor.Controls[counter]).chunkOffset).ToString() + " (" + toHex(((BaseField)this.panelMetaEditor.Controls[counter]).chunkOffset) + ")" +
                            "\noffset in tag: " + (temp - map.MetaInfo.Offset[tagnum]).ToString() + " (" + toHex(temp - map.MetaInfo.Offset[tagnum]) + ")" +
                            "\noffset in map: " + (temp).ToString() + " (" + toHex(temp) + ")";
                    if (tagnum == meta.TagIndex)
                        tip = "reflexive base offset: " + (rd.reflexive.offset).ToString() + " (" + toHex(rd.reflexive.offset) + ")\n" + tip;


                    if (panelMetaEditor.Controls[counter] is Bitmask)
                    {
                        Bitmask tempbitmask = ((Bitmask)panelMetaEditor.Controls[counter]);
                        int bitValue = int.Parse(tempbitmask.Value);
                        string s = " = 0x" + bitValue.ToString("X4") + " (" + tempbitmask.Value + ")";
                        for (int i = 0; i < 32; i++)
                        {
                            s = ((1 << i & bitValue) == 0 ? "0" : "1") + s;
                            if (i % 4 == 3)
                                s = " " + s;
                        }
                        tip += "\n\nBITMASK VALUE:\n" + s;
                    }

                    toolTip1.SetToolTip(panelMetaEditor.Controls[counter].Controls[0], tip);
                }
                catch
                {
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// Sets the colors for all the controls on the ME2 form
        /// </summary>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        public void setFormColors(Color foreColor, Color backColor)
        {
            this.BackColor = backColor;
            this.ForeColor = foreColor;
            this.btnTreeViewOpen.BackColor = backColor;
            this.btnTreeViewOpen.ForeColor = foreColor;
            this.panel1.BackColor = backColor;
            this.panel1.ForeColor = foreColor;
            this.panelMetaEditor.BackColor = backColor;
            this.panelMetaEditor.ForeColor = foreColor;
        }

        /// <summary>
        /// Displays an info pane and keeps it open for an amount of time.
        /// </summary>
        /// <param name="info">The text to display in the Info Pane</param>
        /// <param name="time">The time to display the info pane, in milliseconds</param>
        public void showInfoBox(string info, int time)
        {
            
            panelInfoPane.Text = info;

            panelInfoPane.Visible = true;
            tmr_MEControlPage.Stop();
            if (time > 0)
            {
                tmr_MEControlPage.Interval = time;
                tmr_MEControlPage.Start();
            }
        }

        private string toHex(int value)
        {
            byte[] tempB = BitConverter.GetBytes(value);
            return toHex(tempB);
        }

        private string toHex(byte[] value)
        {
            string tempS = string.Empty;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                int ii = value[i] >> 4;
                tempS += (char)(ii < 10 ? 48 + ii : 65 + ii - 10);
                ii = value[i] & 0x0f;
                tempS += (char)(ii < 10 ? 48 + ii : 65 + ii - 10);
                if (i != 0)
                    tempS += '-';
            }
            return tempS;
        }

        private void treeViewTagReflexives_DoubleClick(object sender, EventArgs e)
        {
            if (((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber == meta.TagIndex)
                return;

            WinMetaEditor wME = ((WinMetaEditor)this.ParentForm);
            Meta oldMeta = map.SelectedMeta;
            ((MapForms.MapForm)this.ParentForm.Owner).LoadMeta(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber);
            int tempTabNum = wME.addNewTab(map.SelectedMeta, true);
            map.SelectedMeta = oldMeta;
            
            ((MetaEditorControlPage)this).gotoOffset(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);

            wME.Show();
            wME.Focus();
        }

        #endregion

        #region Methods

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Copy the whole backup stream to the original stream
            meta.MS.Dispose();
            meta.MS = new MemoryStream(msBackup.ToArray());

            ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
            this.showInfoBox("Entire tag has been reset to last save / original values", 2000);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Save the stream to disk
            map.OpenMap(MapTypes.Internal);
            map.BW.BaseStream.Position = meta.offset;
            map.BW.Write(meta.MS.ToArray());
            map.CloseMap();

            // Save the stream to the backup stream
            msBackup.Dispose();
            msBackup = new MemoryStream(meta.MS.ToArray());

            showInfoBox("Tag Saved.", 1000);
        }

        #region Functions to control hiding of Tree View Reflexive Listing
        private void btnTreeViewOpen_MouseEnter(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = false;
            if (btnTreeViewOpen.Checked == false)
                btnTreeViewOpen.Text = "<<";
            else
                btnTreeViewOpen.Text = ">>";
        }

        private void treeViewTagReflexives_MouseLeave(object sender, EventArgs e)
        {
            Point tempP = PointToClient(MousePosition);
            if (!btnTreeViewOpen.Checked &&
                ((tempP.X <= this.splitContainer1.Panel1.Left || tempP.X >= this.splitContainer1.Panel1.Right) || tempP.Y <= this.splitContainer1.Panel1.Top))
            {                
                splitContainer1.Panel1Collapsed = true;
            }
        }

        private void btnTreeViewOpen_MouseLeave(object sender, EventArgs e)
        {
            btnTreeViewOpen.Text = ">>";
        }

        private void btnTreeViewOpen_Click(object sender, EventArgs e)
        {
            btnTreeViewOpen.Checked = !btnTreeViewOpen.Checked;
            if (btnTreeViewOpen.Checked == false)
                btnTreeViewOpen.Text = "<<";
            else
                btnTreeViewOpen.Text = ">>";
        }
        #endregion

        private void btnSaveValues_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Globals.Prefs.pathExtractsFolder;
            string[] nameSplit = this.meta.name.Split('\\');
            sfd.FileName = nameSplit[nameSplit.Length - 1];
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                map.SelectedMeta.SaveMetaToFile(sfd.FileName, true);
            }
        }

        private void btnRestoreValues_Click(object sender, EventArgs e)
        {
            string[] nameSplit = this.meta.name.Split('\\');
            string fileName = nameSplit[nameSplit.Length - 1];

            RestoreSelection rs = new RestoreSelection(map, meta, ref fileName);
            if (fileName != null)
            {
                if (CurrentControl != null)
                {
                    reflexiveData rd = (reflexiveData)CurrentControl.Tag;
                    string offset = rd.reflexive.offset.ToString();
                    while (rd.node.Parent != null)
                    {
                        rd = (reflexiveData)rd.node.Parent.Tag;
                        offset = rd.reflexive.offset.ToString() + '\\' + offset;
                    }
                    rs.setControl(offset);
                }
                rs.ShowDialog(this);
            }

            rs.Dispose();
        }

        private void cbIdent_MouseLeave(object sender, EventArgs e)
        {
            // If our tag is not null, make sure it actually is an image, then release the used memory and restore the 
            // original picture
            if (this.MapForm.pictureBox1.Tag != null && this.MapForm.pictureBox1.Tag is Image)
            {
                this.MapForm.pictureBox1.Image.Dispose();
                this.MapForm.pictureBox1.Image = (Image)this.MapForm.pictureBox1.Tag;
                this.MapForm.pictureBox1.Tag = null;
            }
        }

        private void cbIdent_MouseEnter(object sender, EventArgs e)
        {
            // Get the main Ident control to be used
            Ident id = (Ident)((Control)sender).Parent;

            if (id.tagType == "bitm" && id.identInt32 != -1)
            {
                // Read Ident name and load appropriate bitmap into MapForm picture box
                this.MapForm.pictureBox1.Tag = this.MapForm.pictureBox1.Image;                
                Meta meta = Map.GetMetaFromTagIndex(id.tagIndex, map, true, false);
                HaloMap.RawData.ParsedBitmap pm = new HaloMap.RawData.ParsedBitmap(ref meta, map);
                this.MapForm.pictureBox1.Image = pm.FindChunkAndDecode(0, 0, 0, ref meta, map, 0, 0);
            }
        }

        private void cbHideUnused_CheckedChanged(object sender, EventArgs e)
        {
            createTreeListing();
        }

        private void cbSortByName_CheckedChanged(object sender, EventArgs e)
        {
            createTreeListing();
        }

        /// <summary>
        /// Loads the meta for the current ident under the Context MenuStrip and then loads it into the Meta Editor
        /// </summary>
        /// <param name="sender">The ContextMenuStrip's ToolStripItem</param>
        /// <param name="e">unused</param>
        private void jumpToTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c = ((ContextMenuStrip)((ToolStripItem)sender).Owner).SourceControl;
            MapForms.MapForm mf = (MapForms.MapForm)((WinMetaEditor)this.ParentForm).Owner;
            int tagNum = map.Functions.ForMeta.FindByNameAndTagType(c.Controls[2].Text, c.Controls[1].Text);
            mf.selectTag(tagNum);
            ((WinMetaEditor)this.ParentForm).addNewTab(map.SelectedMeta, false);
        }

        private void MetaEditorControlPage_Activated(object sender, EventArgs e)
        {
            if ((((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).chunkCount) > 0)
                ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
        }

        public void MetaEditorControlPage_Closing(object sender, FormClosingEventArgs e)
        {
        }

        void MetaEditorControlPage_GotFocus(object sender, EventArgs e)
        {
            // For bitmasks, select the parent's parent
            if ((((Control)sender).Parent) is GroupBox)
                CurrentControl = (BaseField)((Control)sender).Parent.Parent;
            else
                CurrentControl = (BaseField)((Control)sender).Parent;
        }

        /// <summary>
        /// This is used to update the reflexive label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WinMEControl_LostFocus(object sender, EventArgs e)
        {
            ToolStripComboBox tscb = (ToolStripComboBox)this.tsNavigation.Items[tsNavigation.Items.Count - 1];
            tscb.Enabled = false;
            tscb.Items[tscb.SelectedIndex] = tscb.SelectedIndex.ToString() + " : \"" + ((Control)sender).Text + "\"";
            tscb.Enabled = true;
        }

        private void tmr_MEControlPage_Tick(object sender, EventArgs e)
        {
            panelInfoPane.Visible = false;
            tmr_MEControlPage.Stop();    
        }

        private void treeViewTagReflexives_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!treeViewTagReflexives.Enabled)
                return;

            TreeNode tn = e.Node;

            // Show a wait cursor while the selected reflexive is loading
            this.Cursor = Cursors.WaitCursor;

            try
            {
                loadControls(tn);

                tsNavigation.Items.Clear();
                while (tn.Parent != null)
                {
                    addReflexive(tsNavigation, tn.Text);
                    tsNavigation.Items[1].Name = tn.Name;

                    reflexiveData rd = (reflexiveData)tn.Tag;

                    // will throw exception if in the [MAIN] reflexive, so just ignore
                    /*
                    try
                    {
                        if (ctl.name == rd.reflexive.label && !labelFound)
                        {
                            labelFound = true;
                            panelMetaEditor.Controls[0].Controls[1].LostFocus += new EventHandler(WinMEControl_LostFocus);
                        }
                    }
                    catch { }
                    */

                    string[] s = new string[rd.chunkCount];
                    try
                    {
                        s = loadLabels(rd);
                    }
                    catch
                    {
                        // Errors occur with labels when reflexive lies in another tag, so just ignore
                    }

                    //  Create reflexive count comboBox listings
                    for (int i = 0; i < rd.chunkCount; i++)
                    {
                        if (s[i] != null)
                            ((ToolStripComboBox)tsNavigation.Items[2]).Items.Add(i + " : \"" + s[i] + "\"");
                        else
                            ((ToolStripComboBox)tsNavigation.Items[2]).Items.Add(i);
                    }

                    if (rd.chunkCount != 0)
                    {
                        if (rd.chunkSelected > rd.chunkCount)
                            rd.chunkSelected = rd.chunkCount - 1;
                        try
                        {
                            // Stop it from running ReloadMetaForSameReflexive() since we just loaded it
                            ((ToolStripComboBox)tsNavigation.Items[2]).Enabled = false;
                            ((ToolStripComboBox)tsNavigation.Items[2]).SelectedIndex = rd.chunkSelected;
                        }
                        catch
                        {
                        }
                        finally
                        {
                            ((ToolStripComboBox)tsNavigation.Items[2]).Enabled = true;
                        }
                    }

                    tn = tn.Parent;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Restore our mouse cursor
            this.Cursor = Cursors.Arrow;

            panelMetaEditor.Enabled = (((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).chunkCount != 0);

            // If the data is not in this tag, just disbale editing of it. Make it done in parent tag ONLY!
            if (meta.TagIndex != ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber)
                panelMetaEditor.Enabled = false;

            tsMetaMassEdit.Enabled = panelMetaEditor.Enabled;

            btnSave.Enabled = panelMetaEditor.Enabled;
            btnReset.Enabled = panelMetaEditor.Enabled;

            if (((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber != -1 &&
                ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber != meta.TagIndex)
                showInfoBox("This reflexive resides within \"[" +
                      map.MetaInfo.TagType[((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber] +
                      "] " +
                      map.FileNames.Name[((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber] +
                      "\"\r\nYou must edit these values in the parent tag." +
                      "\r\nDouble click the reflexive in the tree on the left to load the reflexive in the given tag.", 0);
            else
            {
                showInfoBox("", 1);
                if ((bool)treeViewTagReflexives.Tag)
                    tsbtnPeek.Checked = true;
            }

        }

        void treeViewTagReflexives_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            treeViewTagReflexives.Tag = tsbtnPeek.Checked;
            if (tsbtnPeek.Checked)
                tsbtnPeek.Checked = false;
        }

        private void treeViewTagReflexives_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == MouseButtons.Right)
            {
                TreeNode c = treeViewTagReflexives.GetNodeAt(me.Location);
                treeViewTagReflexives.SelectedNode = c;
            }

        }

        private void treeViewTagReflexives_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.Node.ForeColor == Color.Red) &
                ((e.State & TreeNodeStates.Selected) != 0))
            {
                e.Graphics.DrawString(e.Node.Text,
                        e.Node.TreeView.Font,
                        Brushes.PaleVioletRed,
                        new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width + 10, e.Bounds.Height - 2));
            }
            //Paint text of other nodes in default color
            else
            {
                e.DrawDefault = true;
            }
        }

        void tsbc_DropDown(object sender, EventArgs e)
        {
            ToolStripComboBox tscb = ((ToolStripComboBox)sender);
            int desiredWidth = 49;
            for (int i = 0; i < tscb.Items.Count; i++)
                if (TextRenderer.MeasureText(((object)tscb.Items[i]).ToString(), tscb.Font).Width + 8 > desiredWidth)
                    desiredWidth = TextRenderer.MeasureText(((object)tscb.Items[i]).ToString(), tscb.Font).Width + 16;
            tscb.Overflow = ToolStripItemOverflow.Never;
            tscb.Size = new Size(desiredWidth, 21);
        }

        void tsbc_DropDownClosed(object sender, EventArgs e)
        {
            ToolStripComboBox tscb = ((ToolStripComboBox)sender);
            tscb.Size = new Size(49, 21);
            tscb.Overflow = ToolStripItemOverflow.AsNeeded;
        }

        void tsbc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Did this to stop populating data twice on load, but stops external reflexives from loading data
            //if (!((ToolStripItem)sender).Enabled)
            //    return;

            #region Determine which comboBox was changed and modify related Reflexive Data accordingly
            ToolStripComboBox tscb = (ToolStripComboBox)sender;
            int tsGroupNumber = (tsNavigation.Items.IndexOf(tscb) - 2) / 3;
            TreeNode tn = treeViewTagReflexives.SelectedNode;
            List<TreeNode> tns = new List<TreeNode>();
            tns.Add(tn);
            for (int ii = tsGroupNumber; ii < ((tsNavigation.Items.Count - 2) / 3); ii++)
            {
                tn = tn.Parent;
                tns.Insert(0, tn);
            }
            reflexiveData rd = (reflexiveData)tn.Tag;
            rd.chunkSelected = tscb.SelectedIndex;
            #endregion
            // tn = TreeNode that was changed
            // tns = all nodes from selected node to changed node
            // tsGroupNumber = ComboBox # that was changed (from Left->Right)

            // Update sub-Combo Box listings for lead Combo Boxes that were changed
            //refreshTreeListing(treeViewTagReflexives.Nodes[0]);  // Slow to always refresh all!
            refreshTreeListing(tn);

            // Update tree listings counters
            int levelsUp = treeViewTagReflexives.SelectedNode.Level - rd.node.Level;
            for (int j = tsGroupNumber; j <= tsGroupNumber + levelsUp; j++)
            {
                rd = (reflexiveData)tns[j - tsGroupNumber].Tag;

                if (rd.chunkCount > 70000) // Some very large number
                    throw new Exception( "\"" + tsNavigation.Items[j * 3 + 1].Name + "\" appears to contain " + rd.chunkCount + "chunks!\n"
                        + "Try reloading tag. If problem persists, you likely have a corrupt map!" );

                tsNavigation.Items[j * 3 + 1].Text = tsNavigation.Items[j * 3 + 1].Name + " [" + rd.chunkCount + "]";
                int selection = ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).SelectedIndex;
                int count = ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Items.Count;
                #region check Combo Box and make sure it contains the proper amount of numbers
                if (rd.chunkCount > count)
                {
                    for (int ii = count; ii < rd.chunkCount; ii++)
                        ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Items.Add(ii);
                }
                else if (rd.chunkCount < count && rd.chunkCount > 0)
                {
                    for (int ii = count - 1; ii > rd.chunkCount - 1; ii--)
                        ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Items.RemoveAt(ii);
                    if (((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).SelectedIndex == -1)
                    {
                        ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Enabled = false;
                        ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).SelectedIndex = rd.chunkCount - 1;
                        ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Enabled = true;
                    }
                }
                else if (rd.chunkCount == 0)
                {
                    ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]).Items.Clear();
                }
                #endregion

                // Update label listings on combobox
                string[] s = loadLabels(rd);
                ToolStripComboBox tcb = ((ToolStripComboBox)tsNavigation.Items[j * 3 + 2]);
                // We need to disable update for this or it endless loops when changing selected index
                tcb.SelectedIndexChanged -= tsbc_SelectedIndexChanged;
                for (int ii = 0; ii < tcb.Items.Count; ii++)
                {
                    tcb.Items[ii] = ii + " : \"" + s[ii] + "\"";
                }
                // ...and re-enable afterwards
                tcb.SelectedIndexChanged += tsbc_SelectedIndexChanged;

            }

            tn = treeViewTagReflexives.SelectedNode;

            if ((((reflexiveData)tn.Tag).chunkCount) > 0)
                ReloadMetaForSameReflexive(((reflexiveData)tn.Tag).baseOffset);

            // Update status bar to reflect listings
            this.MapForm.statusBarText = string.Empty;
            int i = 0;
            foreach (ToolStripItem tsi in tsNavigation.Items)
            {
                if (++i % 3 == 0)
                    this.MapForm.statusBarText += tsi.Text + " \\ ";
            }
        }

        private void tsBtnResetValue_Click(object sender, EventArgs e)
        {
            int ofs = CurrentControl.offsetInMap - CurrentControl.meta.offset;
            byte[] b = new byte[CurrentControl.size];
            msBackup.Position = ofs;
            msBackup.Read(b, 0, CurrentControl.size);
            meta.MS.Position = ofs;
            meta.MS.Write(b, 0, CurrentControl.size);

            ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
            CurrentControl.Focus();
            this.showInfoBox("Current value reset to last save / original value", 2400);
        }

        private void tsBtnResetReflexive_Click(object sender, EventArgs e)
        {
            reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
            int ofs = rd.baseOffset;// CurrentControl.offsetInMap - CurrentControl.meta.offset;
            int chunkSize = 0;
            if (rd.reflexive == null)
                chunkSize = meta.headersize;
            else
                chunkSize = rd.reflexive.chunkSize;

            byte[] b = new byte[chunkSize];
            msBackup.Position = ofs;
            msBackup.Read(b, 0, b.Length);
            meta.MS.Position = ofs;
            meta.MS.Write(b, 0, b.Length);

            ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
            if (CurrentControl != null)
                CurrentControl.Focus();
            this.showInfoBox("Current reflexive values reset to last save / original values", 3400);
        }

        private void tsBtnResetReflexiveAll_Click(object sender, EventArgs e)
        {
            reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
            int chunkSize = 0;
            if (rd.reflexive == null)
                chunkSize = meta.headersize;
            else
                chunkSize = rd.reflexive.chunkSize;
            int ofs = rd.baseOffset - (rd.chunkSelected * chunkSize);

            byte[] b = new byte[chunkSize * rd.chunkCount];
            msBackup.Position = ofs;
            msBackup.Read(b, 0, b.Length);
            meta.MS.Position = ofs;
            meta.MS.Write(b, 0, b.Length);

            ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
            CurrentControl.Focus();
            this.showInfoBox("All values in all chunks of current reflexive reset to last save / original values", 5200);
        }

        private void tsBtnCopyToAll_Click(object sender, EventArgs e)
        {
            // Save the data to memory first, so we can just do a memory blitz copy
            CurrentControl.BaseField_Leave(sender, e);

            reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
            if (rd.reflexive == null) return; // no need to do this for the tag MAIN section, only ever has 1

            // Read the data from the current reflexive
            int ofs = CurrentControl.offsetInMap - CurrentControl.meta.offset;
            byte[] b = new byte[CurrentControl.size];
            meta.MS.Position = ofs;
            meta.MS.Read(b, 0, CurrentControl.size);

            // Start at the first reflexive
            ofs -= (rd.chunkSelected * rd.reflexive.chunkSize);

            int bitMask = 0;
            int bitOfs = 0;
            if (CurrentControl is entity.MetaEditor2.Bitmask)
            {
                CheckBox tempCB = (CheckBox)CurrentControl.ActiveControl;
                bitMask = 1 << int.Parse(tempCB.Tag.ToString());
                bitOfs = bitMask >> 8;
                bitMask -= bitOfs << 8;
            }

            for (int i = 0; i < rd.chunkCount; i++)
            {

                if (CurrentControl is entity.MetaEditor2.Bitmask)
                {
                    // Read the byte containing the changed bit
                    byte b2;
                    meta.MS.Position = ofs + i * rd.reflexive.chunkSize + bitOfs;
                    b2 = (byte)meta.MS.ReadByte();
                    // Modify the changed bit
                    b2 = (byte)((b2 & (byte.MaxValue - bitMask)) | (b[bitOfs] & bitMask));
                    // Write the changed byte back to the stream
                    meta.MS.Position = ofs + i * rd.reflexive.chunkSize + bitOfs;
                    meta.MS.WriteByte(b2);
                }
                else
                {
                    meta.MS.Position = ofs + i * rd.reflexive.chunkSize;
                    // Allows quickly offseting multiple placements
                    //b = BitConverter.GetBytes((float)(BitConverter.ToSingle(b,0) + 0.8f));
                    meta.MS.Write(b, 0, CurrentControl.size);
                }
            }
            CurrentControl.Focus();
            this.showInfoBox("Current value copied to all chunks of current reflexive", 3000);
        }

        private void tsExternalReferenceAdd_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeViewTagReflexives.SelectedNode;
            reflexiveData rd = (reflexiveData)tn.Tag;
            if (rd.reflexive == null)
            {
                MessageBox.Show("Not a reflexive!");
                return;
            }

            WinMetaEditor.references refs = new WinMetaEditor.references();
            if (rd.inTagNumber == this.meta.TagIndex)
            {
                refs.ident = this.meta.offset + rd.baseOffset + this.meta.magic;
                refs.offset = rd.baseOffset;
                refs.tagIndex = this.meta.TagIndex;
                refs.tagName = this.meta.name;
                refs.tagType = this.meta.type;
            }
            else
            {
                refs.tagIndex = map.Functions.ForMeta.FindMetaByOffset(rd.baseOffset);
                Meta m = new Meta(map);
                map.OpenMap(MapTypes.Internal);
                m.ReadMetaFromMap(refs.tagIndex, true);
                map.CloseMap();
                refs.ident = rd.baseOffset + m.magic;
                refs.offset = rd.baseOffset;
                refs.tagName = m.name;
                refs.tagType = m.type;
                m.Dispose();
            }
            refs.chunkCount = rd.chunkCount;
            refs.size = rd.reflexive.chunkSize;            
            refs.name = rd.reflexive.name;

            // Check for duplicates & remove
            List<WinMetaEditor.references> refList = ((WinMetaEditor)this.ParentForm).reflexiveReferences;
            for (int i = 0; i < refList.Count; i++)
            {
                if (refList[i].ident == refs.ident)
                    refList.RemoveAt(i--);
            }
            
            // Always add to top of list
            refList.Insert(0, refs);
        }

        private void tsExternalReferencePoint_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeViewTagReflexives.SelectedNode;
            reflexiveData rd = (reflexiveData)tn.Tag;
            List<WinMetaEditor.references> refList = ((WinMetaEditor)this.ParentForm).reflexiveReferences;

            #region Form Data
            Form f = new Form();
            f.MinimizeBox = false;
            f.MinimumSize = new Size(300, 130);
            f.Size = new Size(550, 90 + Math.Min(refList.Count * 12, 280));
            f.Text = "Select external reflexive to reference";

            ListBox lb = new ListBox();
            lb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lb.DataSource = refList;
            lb.Location = new Point(5, 5);
            lb.Size = new Size(f.Width - 18, f.Height - 80);

            /*
            for (int i = 0; i < refList.Count; i++)
            {
                // Only add same size references (mostly compatible; could add an incorrect but same size one) 
                if (refList[i].size == rd.reflexive.chunkSize)
                    lb.Items.Add(refList[i]);
            }
            */

            Button bSelect = new Button();
            bSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            bSelect.DialogResult = DialogResult.OK;
            bSelect.MinimumSize = new Size(50, 16);
            bSelect.Location = new Point( f.Width / 2 - bSelect.Width / 2, f.Height - bSelect.Height - 45);
            bSelect.Text = "Select";

            Button bRemove = new Button();
            bRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bRemove.Location = new Point(30, f.Height - bSelect.Height - 45);
            bRemove.Text = "Remove";
            bRemove.Click += (s, ev) =>
                {
                    refList.Remove((WinMetaEditor.references)lb.SelectedItem);
                    ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
                };
            lb.DoubleClick += (s, ev) =>
                {
                    f.DialogResult = DialogResult.OK;
                    f.Close();
                };

            f.Controls.Add(lb);
            f.Controls.Add(bRemove);
            f.Controls.Add(bSelect);
            #endregion

            if (f.ShowDialog() == DialogResult.OK)
            {
                // If changing a reflexive that is contained within the tag to an external, backup the original first
                if (rd.inTagNumber == meta.TagIndex)
                {
                    tsExternalReferenceAdd_Click(this, null);
                    showInfoBox("Original reflexive reference has been added to list (backed up)", 1000);
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1000);
                }

                WinMetaEditor.references r = (WinMetaEditor.references)lb.SelectedItem;
                if (r != null)
                {
                    BinaryWriter bw = new BinaryWriter(meta.MS);
                    meta.MS.Position = rd.reflexive.offset;
                    bw.Write(r.chunkCount);
                    bw.Write(r.ident);
                    
                    rd.baseOffset = r.offset;
                    rd.inTagNumber = r.tagIndex;

                    // Refresh the tree (to show black/internal or red/external and update tooltip)
                    refreshTreeListing(tn);
                    // Reselect to update external tag lockout and warning box
                    treeViewTagReflexives_AfterSelect(this, new TreeViewEventArgs(tn));
                }
            }
            f.Dispose();
        }

        #endregion

        #region Debug Peeking & Poking
        private void debugPokeValue()
        {
            reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
            // Idents must be poked individually with Tag Type, then Tag Name. It doesn't change if both
            // are poked at once.
            if (CurrentControl is Ident && CurrentControl.size == 8)
            {
                BR.BaseStream.Position = rd.baseOffset + CurrentControl.chunkOffset;
                Byte[] bytes = BR.ReadBytes(CurrentControl.size);
                HaloMap.RealTimeHalo.RTH_Imports.Poke(
                    (uint)(CurrentControl.offsetInMap + CurrentControl.meta.magic),
                    BitConverter.ToUInt32(bytes, 0),
                    4);
                HaloMap.RealTimeHalo.RTH_Imports.Poke(
                    (uint)(CurrentControl.offsetInMap + CurrentControl.meta.magic + 4),
                    BitConverter.ToUInt32(bytes, 4),
                    4);
            }
            else
            {
                BR.BaseStream.Position = rd.baseOffset + CurrentControl.chunkOffset;
                uint value = uint.MaxValue;
                switch (CurrentControl.size)
                {
                    case 1:
                        value = BR.ReadByte();
                        break;
                    case 2:
                        value = BitConverter.ToUInt16(BR.ReadBytes(CurrentControl.size), 0);
                        break;
                    case 4:
                        value = BitConverter.ToUInt32(BR.ReadBytes(CurrentControl.size), 0);
                        break;
                    default:
                        MessageBox.Show("Error: Cannot RTH " + CurrentControl.GetType() + " of size: " + CurrentControl.size);
                        return;
                }
                HaloMap.RealTimeHalo.RTH_Imports.Poke(
                    (uint)(CurrentControl.offsetInMap + CurrentControl.meta.magic),
                    value,
                    CurrentControl.size); 
            }
        }
        
        /// <summary>
        /// Pokes a full reflexive to the xbox
        /// </summary>
        /// <param name="rd">The reflexive to poke</param>
        private void debugPokeReflexive(reflexiveData rd)
        {          

            // The end of the block to read
            int endOffset = 0;

            // The start of the block to read
            int startOffset = endOffset;

            // The size of the block to read
            int readSize = 0;

            // For tag main section, use headersize instead of reflexive size
            int size = rd.reflexive == null ? meta.headersize : rd.reflexive.chunkSize;
            
            // Keeps track of the control we are checking
            int controlNum = panelMetaEditor.Controls.Count;

            // Initialize with chunkOffset & size = 0
            BaseField oldc = new BaseField();

            // Read in blocks, stopping at all 8-byte Idents and reflexives to be handled accordingly
            while (startOffset != size)
            {
                controlNum--;

                BaseField c = null;
                int sizeChange = 0;
                if (controlNum >= 0)
                {
                    c = (BaseField)panelMetaEditor.Controls[controlNum];
                    sizeChange = c.size;
                }
                else
                {
                    readSize = endOffset - startOffset;
                }

                if (readSize > 0)
                {
                    BR.BaseStream.Position = startOffset + rd.baseOffset;
                    byte[] buffer = BR.ReadBytes(readSize);
                    HaloMap.RealTimeHalo.RTH_Imports.Poke(
                        (uint)(meta.offset + rd.baseOffset + startOffset + meta.magic),
                        buffer,
                        readSize);
                    // If second part of Tag/Ident...
                    if (c != null && c.chunkOffset >= endOffset)
                        startOffset = c.chunkOffset;
                    else
                        startOffset = endOffset;
                }

                readSize = 0;

                // Idents must be poked individually with Tag Type, then Tag Name. It doesn't change if both
                // are poked at once.
                if (c is Ident && c.size == 8)
                {

                    // Not positive if this will work, but we can try!
                    readSize = c.chunkOffset - startOffset + 4; // TAG portion as well
                    sizeChange = 4;
                    if (readSize != 0)
                        controlNum++;
                }

                // Check for a reflexive between controls
                if (c != null && c.chunkOffset > endOffset)
                {
                    readSize = endOffset - startOffset;
                    endOffset = c.chunkOffset;
                    controlNum++;
                    sizeChange = 0;
                }
                endOffset += sizeChange;
                oldc = c;
            }
        }

        void tsbtnPeek_CheckedChanged(object sender, System.EventArgs e)
        {
            // Update any changes to our backup before loading Peek values
            if (CurrentControl != null)
                CurrentControl.BaseField_Leave(null, null);
            
            reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
            int ofs = rd.baseOffset;// CurrentControl.offsetInMap - CurrentControl.meta.offset;
            int chunkSize = 0;
            if (rd.reflexive == null)
                chunkSize = meta.headersize;
            else
                chunkSize = rd.reflexive.chunkSize;

            if (this.tsbtnPeek.Checked)
            {
                if (!HaloMap.RealTimeHalo.RTH_Imports.IsConnected)
                {
                    // Add update info here //
                    tsbtnPeek.Checked = false;
                    foreach (Control c in this.panelMetaEditor.Controls)
                    {
                        c.Controls[1].ForeColor = Color.Black;
                    }
                    showInfoBox("Debug xbox not connected!", 2000);
                    return;
                }
              
                // Backup out data
                byte[] b = new byte[chunkSize];
                meta.MS.Position = ofs;
                meta.MS.Read(b, 0, b.Length);
                msDebug.Position = ofs;
                msDebug.Write(b, 0, b.Length);
              
                // Read from our debug box
                uint offset = (uint)(meta.offset + meta.magic + ofs);
                b = (byte[])HaloMap.RealTimeHalo.RTH_Imports.Peek(offset, (uint)chunkSize);
                meta.MS.Position = ofs;
                meta.MS.Write(b, 0, b.Length);              
            }
            else
            {
                try
                {
                    // Write our Backed up data
                    byte[] b = new byte[chunkSize];
                    msDebug.Position = ofs;
                    msDebug.Read(b, 0, b.Length);
                    meta.MS.Position = ofs;
                    meta.MS.Write(b, 0, b.Length);
                }
                catch
                {
                }
            }

            foreach (Control c in this.panelMetaEditor.Controls)
            {
                if (c is Bitmask)
                {
                    for (int i = 0; i < c.Controls[0].Controls.Count; i++)

                        c.Controls[0].Controls[i].ForeColor = this.tsbtnPeek.Checked ? Color.Red : Color.Black;
                }
                else 
                {
                    if (c is Ident)
                        c.Controls[2].ForeColor = this.tsbtnPeek.Checked ? Color.Red : Color.Black;
                    c.Controls[1].ForeColor = this.tsbtnPeek.Checked ? Color.Red : Color.Black;
                }
            }
            ReloadMetaForSameReflexive(((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).baseOffset);
            if (CurrentControl != null)
                CurrentControl.Focus();
        }

        void CurrentControl_GotFocus(object sender, EventArgs e)
        {
            // Should never happen, but if so, update connection status
            if (!HaloMap.RealTimeHalo.RTH_Imports.IsConnected)
            {
                // Add update info here //
                showInfoBox("Debug xbox not connected!", 2000);
            }
            //CurrentControl.
        }

        private void tsbtnPoke_Click(object sender, EventArgs e)
        {
            if (!HaloMap.RealTimeHalo.RTH_Imports.IsConnected)
            {
                showInfoBox("No Debug Xbox Connected!", 2000);
                return;
            }

            // Update any changes to our backup before loading Peek values
            if (CurrentControl != null)
                CurrentControl.BaseField_Leave(null, null);

            switch (tscbApplyTo.SelectedIndex)
            {
                // Individual value
                case 0:
                    debugPokeValue();
                    break;
                // Reflexive 
                case 1:
                    debugPokeReflexive((reflexiveData)treeViewTagReflexives.SelectedNode.Tag);
                    break;
                // Individual value in all reflexive chunks
                case 2:
                    string backupText = tsbtnPoke.Text;
                    reflexiveData rd = (reflexiveData)treeViewTagReflexives.SelectedNode.Tag;
                    int sel = rd.chunkSelected;
                    panelMetaEditor.Enabled = false;
                    if (treeViewTagReflexives.SelectedNode.Parent != null)
                    {
                        for (int i = 0; i < rd.chunkCount; i++)
                        {
                            tsbtnPoke.Text = i.ToString() + "...";
                            Application.DoEvents();
                            if (i == sel) 
                                continue;

                            rd.chunkSelected = i;
                            refreshTreeListing(rd.node);
                            debugPokeReflexive((reflexiveData)rd);
                        }
                    }
                    rd.chunkSelected = sel;
                    tscbApplyTo.Text = sel.ToString() + "...";
                    Application.DoEvents();
                    refreshTreeListing(rd.node);
                    debugPokeReflexive((reflexiveData)rd);
                    panelMetaEditor.Enabled = true;
                    tsbtnPoke.Text = backupText;
                    break;
                // Full tag 
                case 3:
                    //debugPokeValue();
                    break;
            }

        }
        #endregion

        private void MetaEditorControlPage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    tsbtnPoke_Click(sender, null);
                    break;
                case Keys.Space:
                    tsbtnPeek.Checked = true;
                    tsbtnPeek_CheckedChanged(sender, null);
                    break;
            }
        }

        private void MetaEditorControlPage_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)                
            {
                case Keys.Space:
                    tsbtnPeek.Checked = false;
                    tsbtnPeek_CheckedChanged(sender, null);
                    break;
            }
        }

    }
}
