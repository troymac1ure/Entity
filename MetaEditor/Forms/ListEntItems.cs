// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListEntItems.cs" company="">
//   
// </copyright>
// <summary>
//   The list ent items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The list ent items.
    /// </summary>
    /// <remarks></remarks>
    public partial class ListEntItems : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The ent.
        /// </summary>
        private IFPIO Ent;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntItems"/> class.
        /// </summary>
        /// <param name="imap">The imap.</param>
        /// <param name="entItems">The ent items.</param>
        /// <remarks></remarks>
        public ListEntItems(Map imap, IFPIO entItems)
        {
            if (imap.SelectedMeta == null)
            {
                this.Close();
            }

            this.Ent = entItems;
            this.map = imap;
            InitializeComponent();
            this.EntItemsTreeView.Nodes.AddRange(LoadEntItemsIntoTreeView(0));
            this.buttGoTo.Tag = this.Ent.ENTElements[0];
            DialogResult DR =
                MessageBox.Show(
                    "No N00bs Beyond This Point.....That Is IF You Want To Keep Working Plugins.....",
                    string.Empty,
                    MessageBoxButtons.OKCancel);
            if (DR == DialogResult.Cancel)
            {
                MessageBox.Show(
                    "You have taken the first step in the NA (N00bs Anonymous) program.  Please continue down the path to becoming 1337");
                this.Close();
            }

            this.combType.Items.AddRange(Enum.GetNames(typeof(IFPIO.ObjectEnum)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add item to ent elements.
        /// </summary>
        /// <param name="TheEnum">The the enum.</param>
        /// <remarks></remarks>
        private void AddItemToEntElements(string TheEnum)
        {
            switch (TheEnum)
            {
                case "Reflexive":
                    {
                        this.Ent.ENTElements.Add(
                            new IFPIO.Reflexive(-1, -1, false, "unnamed", string.Empty, null, -1, true, -1, -1));
                        break;
                    }

                #region Strings

                case "String32":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPString("unnamed", false, -1, 32, false, -1, -1, -1));
                        break;
                    }

                case "UnicodeString256":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPString("unnamed", false, -1, 256, true, -1, -1, -1));
                        break;
                    }

                case "UnicodeString64":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPString("unnamed", false, -1, 64, true, -1, -1, -1));
                        break;
                    }

                case "String256":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPString("unnamed", false, -1, 256, false, -1, -1, -1));
                        break;
                    }

                #endregion

                #region Ints and Bytes

                case "Int":
                    {
                        this.Ent.ENTElements.Add(
                            new IFPIO.IFPInt(
                                -1,
                                (IFPIO.ObjectEnum)Enum.Parse(typeof(IFPIO.ObjectEnum), TheEnum),
                                false,
                                "unnamed",
                                new IFPIO.Index(
                                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, -1),
                                -1,
                                -1,
                                -1));
                        break;
                    }

                case "Short":
                    {
                        goto case "Int";
                    }

                case "UShort":
                    {
                        goto case "Int";
                    }

                case "UInt":
                    {
                        goto case "Int";
                    }

                case "Byte":
                    {
                        this.Ent.ENTElements.Add(
                            new IFPIO.IFPByte(
                                -1,
                                false,
                                "unnamed",
                                new IFPIO.Index(
                                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, -1),
                                -1,
                                -1,
                                -1));
                        break;
                    }

                #endregion

                #region Bitmasks

                case "Bitmask32":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Bitmask(-1, false, "unnamed", null, 32, -1, -1, -1));
                        break;
                    }

                case "Bitmask16":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Bitmask(-1, false, "unnamed", null, 16, -1, -1, -1));
                        break;
                    }

                case "Bitmask8":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Bitmask(-1, false, "unnamed", null, 8, -1, -1, -1));
                        break;
                    }

                #endregion

                #region Enums

                case "Enum32":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPEnum(-1, false, "unnamed", null, 32, -1, -1, -1));
                        break;
                    }

                case "Enum16":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPEnum(-1, false, "unnamed", null, 16, -1, -1, -1));
                        break;
                    }

                case "Enum8":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPEnum(-1, false, "unnamed", null, 8, -1, -1, -1));
                        break;
                    }

                #endregion

                #region Idents

                case "TagBlock":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.TagBlock("unnamed", false, -1, -1, -1, -1));
                        break;
                    }

                // To be outdated
                case "TagType":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.TagType(-1, false, "unnamed", -1, -1, -1));
                        break;
                    }

                case "Ident":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Ident("unnamed", false, -1, false, -1, -1, -1));
                        break;
                    }

                #endregion

                #region Unused

                case "Unused":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Unused(-1, 0, -1, -1, -1));
                        break;
                    }

                #endregion

                #region Floats

                case "Unknown":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.Unknown(-1, false, "unnamed", -1, -1, -1));
                        break;
                    }

                case "Float":
                    {
                        this.Ent.ENTElements.Add(new IFPIO.IFPFloat(-1, false, "unnamed", -1, -1, -1));
                        break;
                    }

                #endregion
            }
        }

        /// <summary>
        /// The bitmask load bit names.
        /// </summary>
        /// <remarks></remarks>
        private void BitmaskLoadBitNames()
        {
            this.combBitmaskBits.Items.Clear();
            this.txtbBitmaskName.Text = string.Empty;
            this.txtbBitmaskBitNumber.Text = string.Empty;
            if (((IFPIO.Bitmask)this.buttGoTo.Tag).options == null)
            {
                return;
            }

            List<int> optionIndexers = new List<int>(0);
            this.combBitmaskBits.Items.Add(string.Empty);
            optionIndexers.Add(-1);
            for (int counter = 0; counter < ((IFPIO.Bitmask)this.buttGoTo.Tag).options.Length; counter++)
            {
                this.combBitmaskBits.Items.Add(
                    ((IFPIO.Option)((IFPIO.Bitmask)this.buttGoTo.Tag).options[counter]).name + " : Bit " +
                    ((IFPIO.Option)((IFPIO.Bitmask)this.buttGoTo.Tag).options[counter]).value);
                optionIndexers.Add(counter);
            }

            this.combBitmaskBits.Tag = optionIndexers.ToArray();
            this.combBitmaskBits.SelectedIndex = 0;
        }

        /// <summary>
        /// The butt write plugin_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ButtWritePlugin_Click(object sender, EventArgs e)
        {
            this.buttSaveCurrentItem.PerformClick();
            for (int counter = 0; counter < 2; counter++)
            {
                this.Ent.EntOutput(string.Empty, true);
                this.Ent = IFPHashMap.GetIfp(this.map.SelectedMeta.type, this.map.HaloVersion);
            }

            this.UpdateTreeViewItems();
        }

        /// <summary>
        /// The ent items tree view_ after select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void EntItemsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.buttGoTo.Tag = this.EntItemsTreeView.SelectedNode.Tag;
            this.txtName.Text = ((IFPIO.BaseObject)this.buttGoTo.Tag).name;
            this.combType.Text = ((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType.ToString();

            // ((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType = (IFPIO.ObjectEnum)System.Enum.Parse(typeof(IFPIO.ObjectEnum), this.combType.Text);
            this.radBTrue.Checked = ((IFPIO.BaseObject)this.buttGoTo.Tag).visible;
            this.radBFalse.Checked = !((IFPIO.BaseObject)this.buttGoTo.Tag).visible;
            this.txtbOffset.Text = ((IFPIO.BaseObject)this.buttGoTo.Tag).offset.ToString();
            this.HideEntControls();
            switch (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType)
            {
                case IFPIO.ObjectEnum.Struct:
                    {
                        this.radbReflexiveHCTrue.Checked = ((IFPIO.Reflexive)this.buttGoTo.Tag).HasCount;
                        this.radbReflexiveHCFalse.Checked = !((IFPIO.Reflexive)this.buttGoTo.Tag).HasCount;
                        this.txtbReflexiveChunkSize.Text = ((IFPIO.Reflexive)this.buttGoTo.Tag).chunkSize.ToString();
                        this.LoadReflexiveLabel(
                            ((IFPIO.Reflexive)this.buttGoTo.Tag).child, ((IFPIO.Reflexive)this.buttGoTo.Tag).label);
                        this.panReflexiveContainer.Show();
                        break;
                    }

                #region Strings

                case IFPIO.ObjectEnum.String32:
                    {
                        this.radbStringString.Checked = !((IFPIO.IFPString)this.buttGoTo.Tag).type;
                        this.radbStringUnicode.Checked = ((IFPIO.IFPString)this.buttGoTo.Tag).type;
                        this.combStringSize.Text = ((IFPIO.IFPString)this.buttGoTo.Tag).size.ToString();
                        this.panStringContainer.Show();
                        break;
                    }

                case IFPIO.ObjectEnum.UnicodeString256:
                    {
                        goto case IFPIO.ObjectEnum.String32;
                    }

                case IFPIO.ObjectEnum.UnicodeString64:
                    {
                        goto case IFPIO.ObjectEnum.String32;
                    }

                case IFPIO.ObjectEnum.String256:
                    {
                        goto case IFPIO.ObjectEnum.String32;
                    }

                #endregion

                #region Ints and Bytes

                case IFPIO.ObjectEnum.Int:
                    {
                        this.panIndices.Show();
                        if (((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.nulled)
                        {
                            this.indicesEnable(false);
                        }
                        else
                        {
                            this.indicesEnable(true);
                            this.LoadIndices(((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex);
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

                case IFPIO.ObjectEnum.Byte:
                    {
                        this.panIndices.Show();
                        if (((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex.nulled)
                        {
                            this.indicesEnable(false);
                        }
                        else
                        {
                            this.indicesEnable(true);
                            this.LoadIndices(((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex);
                        }

                        break;
                    }

                #endregion

                #region Bitmasks

                case IFPIO.ObjectEnum.Byte_Flags:
                case IFPIO.ObjectEnum.Word_Flags:
                case IFPIO.ObjectEnum.Long_Flags:
                    {
                        this.panBitmask.Show();
                        this.BitmaskLoadBitNames();
                        break;
                    }

                #endregion

                #region Enums

                case IFPIO.ObjectEnum.Char_Enum:
                case IFPIO.ObjectEnum.Enum:
                case IFPIO.ObjectEnum.Long_Enum:
                    {
                        this.panEnums.Show();
                        this.EnumLoadItemNames();
                        break;
                    }

                #endregion
            }
        }

        /// <summary>
        /// The ent items tree view_ node mouse double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void EntItemsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.buttGoTo.Tag = this.EntItemsTreeView.SelectedNode.Tag;
            this.buttGoTo.PerformClick();
        }

        /// <summary>
        /// The enum load item names.
        /// </summary>
        /// <remarks></remarks>
        private void EnumLoadItemNames()
        {
            this.combEnumsItems.Items.Clear();
            this.txtbEnumsName.Text = string.Empty;
            this.txtbEnumsValue.Text = string.Empty;
            if (((IFPIO.IFPEnum)this.buttGoTo.Tag).options == null)
            {
                return;
            }

            List<int> optionIndexers = new List<int>(0);
            this.combEnumsItems.Items.Add(string.Empty);
            optionIndexers.Add(-1);
            for (int counter = 0; counter < ((IFPIO.IFPEnum)this.buttGoTo.Tag).options.Length; counter++)
            {
                this.combEnumsItems.Items.Add(
                    ((IFPIO.Option)((IFPIO.IFPEnum)this.buttGoTo.Tag).options[counter]).name + " : Value " +
                    ((IFPIO.Option)((IFPIO.IFPEnum)this.buttGoTo.Tag).options[counter]).value);
                optionIndexers.Add(counter);
            }

            this.combEnumsItems.Tag = optionIndexers.ToArray();
            this.combEnumsItems.SelectedIndex = 0;
        }

        /// <summary>
        /// The hide ent controls.
        /// </summary>
        /// <remarks></remarks>
        private void HideEntControls()
        {
            this.panReflexiveContainer.Hide();
            this.panStringContainer.Hide();
            this.panIndices.Hide();
            this.panBitmask.Hide();
            this.panEnums.Hide();
        }

        /// <summary>
        /// The load ent items into tree view.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private TreeNode[] LoadEntItemsIntoTreeView(int counter)
        {
            List<TreeNode> tempnodes = new List<TreeNode>(0);
            while (counter != -1)
            {
                switch (this.Ent.ENTElements[counter].ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        {
                            tempnodes.Add(
                                new TreeNode(
                                    this.Ent.ENTElements[counter].name,
                                    this.LoadEntItemsIntoTreeView(this.Ent.ENTElements[counter].child)));
                            tempnodes[tempnodes.Count - 1].Tag = this.Ent.ENTElements[counter];
                            break;
                        }

                    case IFPIO.ObjectEnum.Unused:
                        {
                            tempnodes.Add(
                                new TreeNode("Unused Size : " + ((IFPIO.Unused)this.Ent.ENTElements[counter]).size));
                            tempnodes[tempnodes.Count - 1].Tag = this.Ent.ENTElements[counter];
                            break;
                        }

                    default:
                        {
                            tempnodes.Add(new TreeNode(this.Ent.ENTElements[counter].name));
                            tempnodes[tempnodes.Count - 1].Tag = this.Ent.ENTElements[counter];
                            break;
                        }
                }

                counter = this.Ent.ENTElements[counter].siblingNext;
            }

            return tempnodes.ToArray();
        }

        /// <summary>
        /// The load indices.
        /// </summary>
        /// <param name="EntIndex">The ent index.</param>
        /// <remarks></remarks>
        private void LoadIndices(IFPIO.Index EntIndex)
        {
            if (EntIndex.reflexiveLayer == "root")
            {
                this.UpdateIndices(0, EntIndex);
            }
            else if (EntIndex.reflexiveLayer == "oneup")
            {
                // this is confusing to say the least......I have to get the parent's parent's child.......
                try
                {
                    this.UpdateIndices(
                        this.Ent.ENTElements[this.Ent.ENTElements[((IFPIO.BaseObject)this.buttGoTo.Tag).parent].parent].
                            child,
                        EntIndex);
                }
                catch
                {
                    this.UpdateIndices(0, EntIndex);
                }
            }

            this.combIndicesLayer.Text = EntIndex.reflexiveLayer;
        }

        /// <summary>
        /// The load reflexive label.
        /// </summary>
        /// <param name="Counter">The counter.</param>
        /// <param name="ReflexiveLabel">The reflexive label.</param>
        /// <remarks></remarks>
        private void LoadReflexiveLabel(int Counter, string ReflexiveLabel)
        {
            this.combReflexiveLabel.Items.Clear();
            this.combReflexiveLabel.Items.Add(string.Empty);
            while (Counter != -1)
            {
                if (this.Ent.ENTElements[Counter].name != null)
                {
                    this.combReflexiveLabel.Items.Add(this.Ent.ENTElements[Counter].name);
                }

                Counter = this.Ent.ENTElements[Counter].siblingNext;
            }

            this.combReflexiveLabel.Text = ReflexiveLabel;
        }

        /// <summary>
        /// The update indices.
        /// </summary>
        /// <param name="Counter">The counter.</param>
        /// <param name="EntIndex">The ent index.</param>
        /// <remarks></remarks>
        private void UpdateIndices(int Counter, IFPIO.Index EntIndex)
        {
            List<int> tempIndexers = new List<int>(0);
            List<string> tempNames = new List<string>(0);
            this.combIndicesRToIndex.Items.Clear();
            this.combIndicesItem.Items.Clear();
            tempNames.Add(string.Empty);
            tempIndexers.Add(-1);
            int comboboxitemindexer = 0;
            int comboboxreflexiveindexer = 0;
            int tempitemindexer = 0;
            while (Counter != -1)
            {
                switch (this.Ent.ENTElements[Counter].ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        {
                            tempIndexers.Add(Counter);
                            tempNames.Add(this.Ent.ENTElements[Counter].name);
                            if (this.Ent.ENTElements[Counter].offset == EntIndex.ReflexiveOffset)
                            {
                                tempitemindexer = this.Ent.ENTElements[Counter].child;
                                comboboxreflexiveindexer = tempIndexers.Count - 1;
                            }

                            goto default;
                        }

                    default:
                        {
                            Counter = this.Ent.ENTElements[Counter].siblingNext;
                            break;
                        }
                }
            }

            this.combIndicesRToIndex.Items.AddRange(tempNames.ToArray());
            this.combIndicesRToIndex.Tag = tempIndexers.ToArray();
            tempIndexers.Clear();
            tempNames.Clear();
            tempNames.Add(string.Empty);
            tempIndexers.Add(-1);
            while (tempitemindexer != -1)
            {
                switch (this.Ent.ENTElements[tempitemindexer].ObjectType)
                {
                    case IFPIO.ObjectEnum.Ident:
                        {
                            tempIndexers.Add(tempitemindexer);
                            tempNames.Add(this.Ent.ENTElements[tempitemindexer].name);
                            if (this.Ent.ENTElements[tempitemindexer].offset == EntIndex.ItemOffset)
                            {
                                comboboxitemindexer = tempIndexers.Count - 1;
                            }

                            goto default;
                        }

                    case IFPIO.ObjectEnum.StringID:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.Float:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.Short:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.UShort:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.Int:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.UInt:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.String32:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.UnicodeString256:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.String256:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    case IFPIO.ObjectEnum.UnicodeString64:
                        {
                            goto case IFPIO.ObjectEnum.Ident;
                        }

                    default:
                        {
                            tempitemindexer = this.Ent.ENTElements[tempitemindexer].siblingNext;
                            break;
                        }
                }
            }

            this.combIndicesItem.Items.AddRange(tempNames.ToArray());
            this.combIndicesItem.Tag = tempIndexers.ToArray();
            this.combIndicesItem.SelectedIndex = comboboxitemindexer;
            this.combIndicesRToIndex.SelectedIndex = comboboxreflexiveindexer;

            // return tempint;
        }

        /// <summary>
        /// The update tree view items.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateTreeViewItems()
        {
            this.EntItemsTreeView.BeginUpdate();
            this.EntItemsTreeView.Nodes.Clear();
            this.EntItemsTreeView.Nodes.AddRange(LoadEntItemsIntoTreeView(0));
            this.EntItemsTreeView.EndUpdate();
            this.EntItemsTreeView.SelectedNode = this.EntItemsTreeView.Nodes[0];
        }

        /// <summary>
        /// The butt add child of selected_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttAddChildOfSelected_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The butt add item after selected_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttAddItemAfterSelected_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The butt add item before selected_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttAddItemBeforeSelected_Click(object sender, EventArgs e)
        {
            AddItemToEntElements(this.combType.Text);
            int EntIndexer = this.Ent.ENTElements.IndexOf((IFPIO.BaseObject)this.buttGoTo.Tag);

            // fix Sibling Previous
            if (this.Ent.ENTElements[EntIndexer].siblingPrevious != -1)
            {
                this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].siblingPrevious =
                    this.Ent.ENTElements[EntIndexer].siblingPrevious;
                this.Ent.ENTElements[EntIndexer].siblingPrevious = this.Ent.ENTElements.Count - 1;
                this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].siblingNext = EntIndexer;
                this.Ent.ENTElements[this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].siblingPrevious].siblingNext =
                    this.Ent.ENTElements.Count - 1;
            }
            else if (this.Ent.ENTElements[EntIndexer].parent == -1)
            {
                MessageBox.Show(
                    "Adding to the first element is disabled atm, please wait until a working version is released to add to the beginning of the plugin");

                // this.Ent.ENTElements[EntIndexer].siblingPrevious = 0;
                // this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].siblingNext = this.Ent.ENTElements.Count-1;
                // IFPIO.BaseObject temp1 = this.Ent.ENTElements[0];
                // IFPIO.BaseObject temp2 = this.Ent.ENTElements[this.Ent.ENTElements.Count - 1];
                // this.Ent.ENTElements[this.Ent.ENTElements.Count - 1] = temp1;
                // this.Ent.ENTElements[0] = temp2;
                // for (int counter = 1; counter < this.Ent.ENTElements.Count-1; counter++)
                // {
                // if(
                // }
            }
            else
            {
                this.Ent.ENTElements[EntIndexer].siblingPrevious = this.Ent.ENTElements.Count - 1;
                this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].siblingNext = EntIndexer;
            }

            // fix parent
            if (this.Ent.ENTElements[EntIndexer].parent != -1)
            {
                this.Ent.ENTElements[this.Ent.ENTElements.Count - 1].parent = this.Ent.ENTElements[EntIndexer].parent;
                if (this.Ent.ENTElements[this.Ent.ENTElements[EntIndexer].parent].child == EntIndexer)
                {
                    this.Ent.ENTElements[this.Ent.ENTElements[EntIndexer].parent].child = this.Ent.ENTElements.Count - 1;
                }
            }

            this.UpdateTreeViewItems();
        }

        /// <summary>
        /// The butt bitmask create_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttBitmaskCreate_Click(object sender, EventArgs e)
        {
            List<object> temparray = new List<object>(0);
            if (((IFPIO.Bitmask)this.buttGoTo.Tag).options != null)
            {
                temparray.AddRange(((IFPIO.Bitmask)this.buttGoTo.Tag).options);
            }

            temparray.Add(new IFPIO.Option(this.txtbBitmaskName.Text, this.txtbBitmaskBitNumber.Text, -1));
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options = temparray.ToArray();
            this.BitmaskLoadBitNames();
        }

        /// <summary>
        /// The butt bitmask delete_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttBitmaskDelete_Click(object sender, EventArgs e)
        {
            if (this.combBitmaskBits.SelectedIndex < 1)
            {
                return;
            }

            List<object> temparray = new List<object>(0);
            temparray.AddRange(((IFPIO.Bitmask)this.buttGoTo.Tag).options);
            temparray.RemoveAt(((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]);
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options = temparray.Count > 0 ? temparray.ToArray() : null;
            this.BitmaskLoadBitNames();
        }

        /// <summary>
        /// The butt bitmask move down_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttBitmaskMoveDown_Click(object sender, EventArgs e)
        {
            if (this.combBitmaskBits.SelectedIndex < 1 ||
                this.combBitmaskBits.SelectedIndex > this.combBitmaskBits.Items.Count - 2)
            {
                return;
            }

            object tempItem1 =
                ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                    ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]];
            object tempItem2 =
                ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                    ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex + 1]];
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]] = tempItem2;
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex + 1]] = tempItem1;
            this.BitmaskLoadBitNames();
        }

        /// <summary>
        /// The butt bitmask move up_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttBitmaskMoveUp_Click(object sender, EventArgs e)
        {
            if (this.combBitmaskBits.SelectedIndex < 2)
            {
                return;
            }

            object tempItem1 =
                ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                    ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]];
            object tempItem2 =
                ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                    ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex - 1]];
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]] = tempItem2;
            ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex - 1]] = tempItem1;
            this.BitmaskLoadBitNames();
        }

        /// <summary>
        /// The butt bitmask save_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttBitmaskSave_Click(object sender, EventArgs e)
        {
            if (this.combBitmaskBits.SelectedIndex < 1)
            {
                return;
            }

            ((IFPIO.Option)
             ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                 ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]]).value =
                Convert.ToInt32(this.txtbBitmaskBitNumber.Text);
            ((IFPIO.Option)
             ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                 ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]]).name =
                this.txtbBitmaskName.Text;
            this.BitmaskLoadBitNames();
        }

        /// <summary>
        /// The butt enums create_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttEnumsCreate_Click(object sender, EventArgs e)
        {
            List<object> temparray = new List<object>(0);
            if (((IFPIO.IFPEnum)this.buttGoTo.Tag).options != null)
            {
                temparray.AddRange(((IFPIO.IFPEnum)this.buttGoTo.Tag).options);
            }

            temparray.Add(new IFPIO.Option(this.txtbEnumsName.Text, this.txtbEnumsValue.Text, -1));
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options = temparray.ToArray();
            this.EnumLoadItemNames();
        }

        /// <summary>
        /// The butt enums delete_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttEnumsDelete_Click(object sender, EventArgs e)
        {
            if (this.combEnumsItems.SelectedIndex < 1)
            {
                return;
            }

            List<object> temparray = new List<object>(0);
            temparray.AddRange(((IFPIO.IFPEnum)this.buttGoTo.Tag).options);
            temparray.RemoveAt(((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]);
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options = temparray.Count > 0 ? temparray.ToArray() : null;
            this.EnumLoadItemNames();
        }

        /// <summary>
        /// The butt enums move item down one_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttEnumsMoveItemDownOne_Click(object sender, EventArgs e)
        {
            if (this.combEnumsItems.SelectedIndex < 1 ||
                this.combEnumsItems.SelectedIndex > this.combEnumsItems.Items.Count - 2)
            {
                return;
            }

            object tempItem1 =
                ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                    ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]];
            object tempItem2 =
                ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                    ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex + 1]];
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]] = tempItem2;
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex + 1]] = tempItem1;
            this.EnumLoadItemNames();
        }

        /// <summary>
        /// The butt enums move up one_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttEnumsMoveUpOne_Click(object sender, EventArgs e)
        {
            if (this.combEnumsItems.SelectedIndex < 2)
            {
                return;
            }

            object tempItem1 =
                ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                    ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]];
            object tempItem2 =
                ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                    ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex - 1]];
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]] = tempItem2;
            ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex - 1]] = tempItem1;
            this.EnumLoadItemNames();
        }

        /// <summary>
        /// The butt enums save_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttEnumsSave_Click(object sender, EventArgs e)
        {
            if (this.combEnumsItems.SelectedIndex < 1)
            {
                return;
            }

            ((IFPIO.Option)
             ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                 ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]]).value =
                Convert.ToInt32(this.txtbEnumsValue.Text);
            ((IFPIO.Option)
             ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                 ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]]).name = this.txtbEnumsName.Text;
            this.EnumLoadItemNames();
        }

        /// <summary>
        /// The butt go to_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttGoTo_Click(object sender, EventArgs e)
        {
            this.EntItemsTreeView.SelectedNode.Tag = this.buttGoTo.Tag;
            this.Close();
        }

        /// <summary>
        /// The butt index create_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttIndexCreate_Click(object sender, EventArgs e)
        {
            this.indicesEnable(true);
            if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
            {
                ((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex.nulled = false;
            }
            else
            {
                ((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.nulled = false;
            }
        }

        /// <summary>
        /// The butt index delete_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttIndexDelete_Click(object sender, EventArgs e)
        {
            this.indicesEnable(false);
            if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
            {
                ((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex.nulled = true;
            }
            else
            {
                ((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.nulled = true;
            }
        }

        /// <summary>
        /// The butt item delete_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttItemDelete_Click(object sender, EventArgs e)
        {
            int tempIndexer = this.Ent.ENTElements.IndexOf((IFPIO.BaseObject)this.buttGoTo.Tag);

            // fix parent
            if (this.Ent.ENTElements[tempIndexer].parent != -1)
            {
                if (tempIndexer == this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].parent].child)
                {
                    if (this.Ent.ENTElements[tempIndexer].siblingNext != -1)
                    {
                        this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].parent].child =
                            this.Ent.ENTElements[tempIndexer].siblingNext;
                    }
                    else
                    {
                        this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].parent].child = -1;
                    }
                }
            }

            // Fix Next Sibling
            if (this.Ent.ENTElements[tempIndexer].siblingNext != -1)
            {
                if (this.Ent.ENTElements[tempIndexer].siblingPrevious != -1)
                {
                    this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].siblingNext].siblingPrevious =
                        this.Ent.ENTElements[tempIndexer].siblingPrevious;
                }
                else
                {
                    this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].siblingNext].siblingPrevious = -1;
                }
            }

            // Fix Previous Sibling
            if (this.Ent.ENTElements[tempIndexer].siblingPrevious != -1)
            {
                if (this.Ent.ENTElements[tempIndexer].siblingNext != -1)
                {
                    this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].siblingPrevious].siblingNext =
                        this.Ent.ENTElements[tempIndexer].siblingNext;
                }
                else
                {
                    this.Ent.ENTElements[this.Ent.ENTElements[tempIndexer].siblingPrevious].siblingNext = -1;
                }
            }

            this.UpdateTreeViewItems();
        }

        /// <summary>
        /// The butt save current item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttSaveCurrentItem_Click(object sender, EventArgs e)
        {
            try
            {
                int tempIndexer = this.Ent.ENTElements.IndexOf((IFPIO.BaseObject)this.buttGoTo.Tag);
                this.Ent.ENTElements[tempIndexer].visible = this.radBTrue.Checked;
                this.Ent.ENTElements[tempIndexer].name = this.txtName.Text;
                this.EntItemsTreeView.SelectedNode.Tag = this.Ent.ENTElements[tempIndexer];
                this.EntItemsTreeView.SelectedNode.Text = this.txtName.Text;
                switch (this.Ent.ENTElements[tempIndexer].ObjectType)
                {


                    case IFPIO.ObjectEnum.Struct:
                        {
                            ((IFPIO.Reflexive)this.Ent.ENTElements[tempIndexer]).HasCount =
                                this.radbReflexiveHCTrue.Checked;
                            ((IFPIO.Reflexive)this.Ent.ENTElements[tempIndexer]).label = this.combReflexiveLabel.Text;
                            break;
                        }



                    #region Strings

                    case IFPIO.ObjectEnum.String32:
                        {
                            ((IFPIO.IFPString)this.Ent.ENTElements[tempIndexer]).size =
                                Convert.ToInt32(this.combStringSize.Text);
                            ((IFPIO.IFPString)this.Ent.ENTElements[tempIndexer]).type = this.radbStringUnicode.Checked;
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

                    #endregion

                    #region Ints and Bytes

                    case IFPIO.ObjectEnum.Int:
                        {
                            if (this.buttIndexDelete.Visible && this.combIndicesItem.Text != string.Empty &&
                                this.combIndicesRToIndex.Text != string.Empty)
                            {
                                int reflexiveIndexer =
                                    ((int[])this.combIndicesRToIndex.Tag)[this.combIndicesRToIndex.SelectedIndex];
                                int itemIndexer = ((int[])this.combIndicesItem.Tag)[this.combIndicesItem.SelectedIndex];
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.nulled = false;
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.ItemOffset =
                                    this.Ent.ENTElements[itemIndexer].offset;
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.ReflexiveOffset =
                                    this.Ent.ENTElements[reflexiveIndexer].offset;
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.ReflexiveSize =
                                    ((IFPIO.Reflexive)this.Ent.ENTElements[reflexiveIndexer]).chunkSize;
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.reflexiveLayer =
                                    this.combIndicesLayer.Text;
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.ItemType =
                                    this.Ent.ENTElements[itemIndexer].ObjectType.ToString().ToLower();
                            }
                            else
                            {
                                ((IFPIO.IFPInt)this.Ent.ENTElements[tempIndexer]).entIndex.nulled = true;
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

                    case IFPIO.ObjectEnum.Byte:
                        {
                            if (this.buttIndexDelete.Visible && this.combIndicesItem.Text != string.Empty &&
                                this.combIndicesRToIndex.Text != string.Empty)
                            {
                                int reflexiveIndexer =
                                    ((int[])this.combIndicesRToIndex.Tag)[this.combIndicesRToIndex.SelectedIndex];
                                int itemIndexer = ((int[])this.combIndicesItem.Tag)[this.combIndicesItem.SelectedIndex];
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.nulled = false;
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.ItemOffset =
                                    this.Ent.ENTElements[itemIndexer].offset;
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.ReflexiveOffset =
                                    this.Ent.ENTElements[reflexiveIndexer].offset;
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.ReflexiveSize =
                                    ((IFPIO.Reflexive)this.Ent.ENTElements[reflexiveIndexer]).chunkSize;
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.reflexiveLayer =
                                    this.combIndicesLayer.Text;
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.ItemType =
                                    this.Ent.ENTElements[itemIndexer].ObjectType.ToString().ToLower();
                            }
                            else
                            {
                                ((IFPIO.IFPByte)this.Ent.ENTElements[tempIndexer]).entIndex.nulled = true;
                            }

                            break;
                        }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Please select a node from the treeview", ex);
            }
        }

        /// <summary>
        /// The comb bitmask bits_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combBitmaskBits_DropDownClosed(object sender, EventArgs e)
        {
            if (this.combBitmaskBits.SelectedIndex > 0)
            {
                this.txtbBitmaskName.Text =
                    ((IFPIO.Option)
                     ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                         ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]]).name;
                this.txtbBitmaskBitNumber.Text =
                    ((IFPIO.Option)
                     ((IFPIO.Bitmask)this.buttGoTo.Tag).options[
                         ((int[])this.combBitmaskBits.Tag)[this.combBitmaskBits.SelectedIndex]]).value.ToString();
            }
            else if (this.combBitmaskBits.SelectedIndex == 0)
            {
                this.txtbBitmaskName.Text = string.Empty;
                this.txtbBitmaskBitNumber.Text = string.Empty;
            }
        }

        /// <summary>
        /// The comb enums items_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combEnumsItems_DropDownClosed(object sender, EventArgs e)
        {
            if (this.combEnumsItems.SelectedIndex > 0)
            {
                this.txtbEnumsName.Text =
                    ((IFPIO.Option)
                     ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                         ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]]).name;
                this.txtbEnumsValue.Text =
                    ((IFPIO.Option)
                     ((IFPIO.IFPEnum)this.buttGoTo.Tag).options[
                         ((int[])this.combEnumsItems.Tag)[this.combEnumsItems.SelectedIndex]]).value.ToString();
            }
            else if (this.combEnumsItems.SelectedIndex == 0)
            {
                this.txtbEnumsName.Text = string.Empty;
                this.txtbEnumsValue.Text = string.Empty;
            }
        }

        /// <summary>
        /// The comb indices layer_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combIndicesLayer_DropDownClosed(object sender, EventArgs e)
        {
            switch (((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString())
            {
                case "oneup":
                    {
                        if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
                        {
                            ((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex.reflexiveLayer = "oneup";
                        }
                        else
                        {
                            ((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.reflexiveLayer = "oneup";
                        }

                        break;
                    }

                default:
                    {
                        if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
                        {
                            ((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex.reflexiveLayer = "root";
                        }
                        else
                        {
                            ((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.reflexiveLayer = "root";
                        }

                        ((ComboBox)sender).Text = "root";
                        break;
                    }
            }

            if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
            {
                this.LoadIndices(((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex);
            }
            else
            {
                this.LoadIndices(((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex);
            }
        }

        /// <summary>
        /// The comb indices r to index_ drop down closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combIndicesRToIndex_DropDownClosed(object sender, EventArgs e)
        {
            IFPIO.Index tempIndex;
            if (((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType == IFPIO.ObjectEnum.Byte)
            {
                tempIndex = ((IFPIO.IFPByte)this.buttGoTo.Tag).entIndex;
            }
            else
            {
                tempIndex = ((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex;
            }

            if (((ComboBox)sender).SelectedIndex != 0 && ((ComboBox)sender).SelectedIndex != -1)
            {
                tempIndex.reflexiveLayer = this.combIndicesLayer.Text;
                tempIndex.ReflexiveOffset =
                    this.Ent.ENTElements[((int[])((ComboBox)sender).Tag)[((ComboBox)sender).SelectedIndex]].offset;
            }

            this.LoadIndices(tempIndex);
        }

        // Not right yet, but getting there
        /// <summary>
        /// The comb type_ selection change committed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // ((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType = (IFPIO.ObjectEnum)System.Enum.Parse(typeof(IFPIO.ObjectEnum), this.combType.Text);
            int tempIndexer = (Int32)Enum.Parse(typeof(IFPIO.ObjectEnum), this.combType.Text);
            string name = ((IFPIO.BaseObject)this.buttGoTo.Tag).name;
            int offset = ((IFPIO.BaseObject)this.buttGoTo.Tag).offset;
            int lineNumber = ((IFPIO.BaseObject)this.buttGoTo.Tag).lineNumber;
            bool visible = ((IFPIO.BaseObject)this.buttGoTo.Tag).visible;
            int parent = ((IFPIO.BaseObject)this.buttGoTo.Tag).parent;
            int prevSibling = ((IFPIO.BaseObject)this.buttGoTo.Tag).siblingPrevious;
            this.HideEntControls();

            switch ((IFPIO.ObjectEnum)tempIndexer)
            {


                case IFPIO.ObjectEnum.Struct:
                    {
                        this.buttGoTo.Tag = new IFPIO.Reflexive(
                            lineNumber, offset, visible, name, string.Empty, null, 0, false, parent, prevSibling);
                        this.radbReflexiveHCTrue.Checked = ((IFPIO.Reflexive)this.buttGoTo.Tag).HasCount;
                        this.radbReflexiveHCFalse.Checked = !((IFPIO.Reflexive)this.buttGoTo.Tag).HasCount;
                        this.txtbReflexiveChunkSize.Text = ((IFPIO.Reflexive)this.buttGoTo.Tag).chunkSize.ToString();
                        this.LoadReflexiveLabel(
                            ((IFPIO.Reflexive)this.buttGoTo.Tag).child, ((IFPIO.Reflexive)this.buttGoTo.Tag).label);
                        this.panReflexiveContainer.Show();
                        break;
                    }



                #region Strings

                case IFPIO.ObjectEnum.String32:
                case IFPIO.ObjectEnum.String256:
                case IFPIO.ObjectEnum.UnicodeString64:
                case IFPIO.ObjectEnum.UnicodeString256:
                    {
                        bool unicode = false;
                        int ifpSize = 256;
                        switch ((IFPIO.ObjectEnum)tempIndexer)
                        {
                            case IFPIO.ObjectEnum.String32:
                                {
                                    ifpSize = 32;
                                    break;
                                }

                            case IFPIO.ObjectEnum.String256:
                                {
                                    ifpSize = 256;
                                    break;
                                }

                            case IFPIO.ObjectEnum.UnicodeString64:
                                {
                                    ifpSize = 64;
                                    unicode = true;
                                    break;
                                }

                            case IFPIO.ObjectEnum.UnicodeString256:
                                {
                                    ifpSize = 256;
                                    unicode = true;
                                    break;
                                }
                        }

                        this.buttGoTo.Tag = new IFPIO.IFPString(
                            name, visible, offset, ifpSize, unicode, lineNumber, parent, prevSibling);
                        this.radbStringString.Checked = !((IFPIO.IFPString)this.buttGoTo.Tag).type;
                        this.radbStringUnicode.Checked = ((IFPIO.IFPString)this.buttGoTo.Tag).type;
                        this.combStringSize.Text = ((IFPIO.IFPString)this.buttGoTo.Tag).size.ToString();
                        this.panStringContainer.Show();
                        break;
                    }

                #endregion

                #region Ints and Bytes

                case IFPIO.ObjectEnum.Float:
                    {
                        this.buttGoTo.Tag = new IFPIO.IFPFloat(offset, visible, name, lineNumber, parent, prevSibling);
                        break;
                    }

                case IFPIO.ObjectEnum.Byte:
                case IFPIO.ObjectEnum.Short:
                case IFPIO.ObjectEnum.UShort:
                case IFPIO.ObjectEnum.UInt:
                case IFPIO.ObjectEnum.Int:
                    {
                        IFPIO.Index Index = new IFPIO.Index(
                            string.Empty, string.Empty, string.Empty, ((IFPIO.BaseObject)this.buttGoTo.Tag).ObjectType.ToString(), string.Empty, lineNumber);
                        this.buttGoTo.Tag = new IFPIO.IFPInt(
                            offset,
                            (IFPIO.ObjectEnum)tempIndexer,
                            visible,
                            name,
                            Index,
                            lineNumber,
                            parent,
                            prevSibling);
                        this.panIndices.Show();
                        if (((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex.nulled)
                        {
                            this.indicesEnable(false);
                        }
                        else
                        {
                            this.indicesEnable(true);
                            this.LoadIndices(((IFPIO.IFPInt)this.buttGoTo.Tag).entIndex);
                        }

                        break;
                    }

                #endregion

                #region Bitmasks

                case IFPIO.ObjectEnum.Byte_Flags:
                case IFPIO.ObjectEnum.Word_Flags:
                case IFPIO.ObjectEnum.Long_Flags:
                    {
                        this.buttGoTo.Tag = new IFPIO.Bitmask(
                            offset, visible, name, null, 0, lineNumber, parent, prevSibling);
                        this.panBitmask.Show();
                        this.BitmaskLoadBitNames();
                        break;
                    }

                #endregion

                #region Enums

                case IFPIO.ObjectEnum.Char_Enum:
                case IFPIO.ObjectEnum.Enum:
                case IFPIO.ObjectEnum.Long_Enum:
                    {
                        this.buttGoTo.Tag = new IFPIO.IFPEnum(
                            offset, visible, name, null, 0, lineNumber, parent, prevSibling);
                        this.panEnums.Show();

                        // this.EnumLoadItemNames();
                        break;
                    }

                #endregion
            }
        }

        /// <summary>
        /// The indices enable.
        /// </summary>
        /// <param name="iEnabled">The i enabled.</param>
        /// <remarks></remarks>
        private void indicesEnable(bool iEnabled)
        {
            this.buttIndexCreate.Visible = !iEnabled;
            this.buttIndexDelete.Visible = iEnabled;
            this.combIndicesLayer.Enabled = iEnabled;
            this.combIndicesRToIndex.Enabled = iEnabled;
            this.combIndicesItem.Enabled = iEnabled;
        }

        #endregion

        // 12, 382
        // 514, 451
    }
}