// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreSelections.cs" company="">
//   
// </copyright>
// <summary>
//   Tag value restore selection form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaEditor2
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Plugins;

    using Microsoft.Win32;
    using Globals;
    using System.IO;
    
    /// <summary>
    /// The custom plugin editor.
    /// </summary>
    /// <remarks></remarks>
    public partial class RestoreSelection : Form
    {
        #region Constants and Fields

        class tagInfo
        {
            public int size;
            public int chunkCount;
            public int chunkSelection;
            public int offset;

            public tagInfo(int Size)
            {
                size = Size;
            }

        }

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;
        /// <summary>
        /// The active meta
        /// </summary>
        private HaloMap.Meta.Meta meta;
        /// <summary>
        /// Store the file into a memory stream so it doesn't have to be continuously opened / closed
        /// </summary>
        private MemoryStream fStream;
        /// <summary>
        /// Contains the lesser of the source or dest amounts (amounts must match for restore)
        /// </summary>
        private int MaxSelections;
        /// <summary>
        /// Keeps a copy of all tree nodes for rebuilding tree structures (aka when reflexives are changed)
        /// </summary>
        private TreeNode baseTree;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPluginEditor"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public RestoreSelection(Map map, HaloMap.Meta.Meta meta, ref string filename)
        {
            InitializeComponent();
            this.map = map;
            this.meta = meta;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Globals.Prefs.pathExtractsFolder;
            ofd.FileName = filename;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                fStream = new MemoryStream((int)fs.Length);
                fStream.Write(br.ReadBytes((int)fs.Length), 0, (int)fs.Length);
                br.Close();
                fs.Close();
                
                tvSourceTags.loadMeta(meta, map.HaloVersion, fStream);
                /*
                tvSourceTags.Nodes.Clear();
                IFPIO ifpx = IFPHashMap.GetIfp(meta.type, map);

                // path stores parent offsets
                string path = string.Empty;

                // Creates a tree listing recursively
                TreeNode[] tns = CreateTree(ifpx.items, path);

                // Create a TreeViewCollection to be passed to our function
                baseTree = new TreeNode();
                baseTree.Nodes.AddRange(tns);

                // Checks the tree listing and removes any reflexives that have a 0 count in the file
                // Source Nodes
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                fStream = new MemoryStream((int)fs.Length);
                fStream.Write(br.ReadBytes((int)fs.Length), 0, (int)fs.Length);
                br.Close();
                fs.Close();
                br = new BinaryReader(fStream);
                // Displays our created source tree
                tvSourceTags.Nodes.AddRange(verifyReflexives(br, baseTree.Nodes, 0));

                /*
                tns = new TreeNode[tncs.Count];
                tncs.CopyTo(tns, 0);
                tvSourceTags.Nodes.AddRange(tns);
                */

                // Destination Nodes
                /*
                BinaryReader br = new BinaryReader(meta.MS);
                tvDestTags.Nodes.AddRange(verifyReflexives(br, baseTree.Nodes, 0));
                */

            }
            else
                filename = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a tree listing of all tags & reflexives (called recursively)
        /// </summary>
        /// <param name="ifps">The ifps.</param>
        /// <param name="path">The offset to the given node (such as "100\8\24")</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private TreeNode[] CreateTree(object[] ifps, string path)
        {
            System.Collections.Generic.List<TreeNode> tnc = new System.Collections.Generic.List<TreeNode>();
            //TreeNode[] tnc = new TreeNode[ifps.Length];
            for (int x = 0; x < ifps.Length; x++)
            {
                IFPIO.BaseObject ifpObj = (IFPIO.BaseObject)ifps[x];
                if (x < (ifps.Length - 1) && ifps[x + 1] is IFPIO.Ident && ifps[x] is IFPIO.TagType)
                {
                    continue;
                }

                string tempchunkname = ifpObj.name;
                if (tempchunkname == null)
                {
                    tempchunkname = "  <null>  ";
                }

                TreeNode ChunkNumberNode = new TreeNode(tempchunkname);
                ChunkNumberNode.ToolTipText = "[" + ifpObj.ObjectType.ToString().ToLower() + "]\n" + "Offset: " +
                                              ifpObj.offset;

                ChunkNumberNode.Name = path + ifpObj.offset;
                //if (ra.isOpen)
                //{
                //    ChunkNumberNode.Checked = false;
                //    string s = ra.getValue(ChunkNumberNode.Name);
                //    if (s != null)
                //    {
                //        bool check = bool.Parse(s);
                //        if (check)
                //        {
                //            ChunkNumberNode.Checked = true;
                //        }
                //    }
                //}

                if (ifps[x] is IFPIO.Reflexive)
                {
                    // TreeNode reflexnode = new TreeNode("Name: " + ((IFPIO.Reflexive)ifps[x]).name + " - Offset: " + ((IFPIO.Reflexive)ifps[x]).offset.ToString());
                    ChunkNumberNode.Nodes.AddRange(
                        CreateTree(((IFPIO.Reflexive)ifps[x]).items, path + ifpObj.offset + "\\"));
                    ChunkNumberNode.Tag = new tagInfo(((IFPIO.Reflexive)ifpObj).chunkSize);
                }

                tnc.Add(ChunkNumberNode);
            }

            return tnc.ToArray();
        }

        private TreeNodeCollection verifyReflexivesRecursive(BinaryReader br, TreeNodeCollection tnc, int baseOffset)
        {
            for (int i = 0; i < tnc.Count; i++)
            {
                TreeNode tn = tnc[i];
                string[] offsets = tn.Name.Split('\\');
                if (tn.Nodes.Count > 0)
                {
                    br.BaseStream.Position = baseOffset + int.Parse(offsets[offsets.Length - 1]);
                    int count = br.ReadInt32();
                    int offset = br.ReadInt32() - meta.magic - meta.offset;
                    if (count == 0)
                    {
                        tnc.Remove(tn);
                        i--;
                        continue;
                    }
                    else
                    {
                        tagInfo ti = (tagInfo)tn.Tag;
                        ti.chunkCount = count;
                        ti.offset = offset;
                        tn.Text += " [" + (ti.chunkSelection + 1).ToString() + "/" + ti.chunkCount.ToString() + "]";
                        TreeNodeCollection tnt = verifyReflexivesRecursive(br, tn.Nodes, offset);
                        TreeNode[] tna = new TreeNode[tnt.Count];
                        tnt.CopyTo(tna, 0);
                        tn.Nodes.Clear();
                        tn.Nodes.AddRange(tna);
                    }
                }
            }
            return tnc;
        }


        /// <summary>
        /// Checks the stream against the treenodes and builds a new tree, removing any zero count reflexives
        /// </summary>
        /// <param name="br">The stream passed in as a BinaryReader</param>
        /// <param name="tnc">The TreeNodeCollection listing</param>
        /// <param name="baseOffset">the base offset in the stream for these nodes</param>
        private TreeNode[] verifyReflexives(BinaryReader br, TreeNodeCollection tnc, int baseOffset)
        {
            System.Collections.Generic.List<TreeNode> tnl = new System.Collections.Generic.List<TreeNode>();

            for (int i = 0; i < tnc.Count; i++)
            {
                // Clone each node (w/ sub-nodes) for top-level so we don't touch original tree
                TreeNode tn = (TreeNode)tnc[i].Clone();

                string[] offsets = tn.Name.Split('\\');
                if (tn.Nodes.Count > 0)
                {
                    br.BaseStream.Position = baseOffset + int.Parse(offsets[offsets.Length - 1]);
                    int count = br.ReadInt32();
                    int offset = br.ReadInt32() - meta.magic - meta.offset;
                    if (count != 0)
                    {
                        tagInfo ti = (tagInfo)tn.Tag;
                        ti.chunkCount = count;
                        ti.offset = offset;
                        int t = tn.Text.LastIndexOf("[");
                        if (t > -1)
                            tn.Text = tn.Text.Substring(0, t);
                        tn.Text += " [" + (ti.chunkSelection + 1).ToString() + "/" + ti.chunkCount.ToString() + "]";
                        TreeNodeCollection tnt = verifyReflexivesRecursive(br, tn.Nodes, offset);
                        TreeNode[] tna = new TreeNode[tnt.Count];
                        tnt.CopyTo(tna, 0);
                        tn.Nodes.Clear();
                        tn.Nodes.AddRange(tna);
                        tnl.Add(tn);
                    }
                }
            }
            return tnl.ToArray();
        }

        /// <summary>
        /// The RestoreSelection form closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void RestoreSelection_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// Set the selected value to position @ offset
        /// </summary>
        /// <param name="offset">value listings seperated by backslash (\) markers.  Ex: 120\16\4</param>
        public void setControl(string offset)
        {

        }

        private void checkRestoreValues(TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                // Check all child nodes
                if (tn.Nodes.Count > 0)
                    checkRestoreValues(tn.Nodes);
                
                // Don't restore un-checked nodes
                if (!tn.Checked)
                    continue;

                string[] offsets = tvSourceTags.SelectedNode.Name.Split('\\');

                BinaryReader br = new BinaryReader(fStream);

                uint mOffset = 0;

                for (int i = 0; i < offsets.Length; i++)
                {
                    uint offset = uint.Parse(offsets[i]);
                    br.BaseStream.Position = mOffset + offset;

                    // All offset listings before the last one are reflexives
                    if (i != (offsets.Length - 1))
                    {
                        uint mCount = br.ReadUInt32();
                        mOffset = br.ReadUInt32();
                    }
                    // The last offset listing is the value we need to get
                    else
                    {
                        byte[] ba = new byte[4];
                        br.Read(ba, 0, ba.Length);
                        float f = BitConverter.ToSingle(ba, 0);
                    }
                }
            }
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void savetagTypeBtn_Click(object sender, EventArgs e)
        {
            checkRestoreValues(tvSourceTags.Nodes);            
        }

        /// <summary>
        /// The button 5_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button5_Click(object sender, EventArgs e)
        {
            if (tvSourceTags.Nodes.Count == 0)
            {
                return;
            }

            // We only need to set the first nodes of selection, as the sub-nodes are updated on treeView1_AfterCheck()
            TreeNode tn = tvSourceTags.Nodes[0];
            while (tn != null)
            {
                tn.Checked = true;
                tn = tn.NextNode;
            }
        }

        /// <summary>
        /// The button 6_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button6_Click(object sender, EventArgs e)
        {
            if (tvSourceTags.Nodes.Count == 0)
            {
                return;
            }

            // We only need to set the first nodes of selection, as the sub-nodes are updated on treeView1_AfterCheck()
            TreeNode tn = tvSourceTags.Nodes[0];
            while (tn != null)
            {
                tn.Checked = false;
                tn = tn.NextNode;
            }
        }

        #endregion

        private string getDataFromBinaryStream(BinaryReader br, string type)
        {
            string tempS = string.Empty;
            int tempI;

            switch (type)
            {
                case "byte":
                    return br.ReadByte().ToString();
                case "float":
                    return br.ReadSingle().ToString();
                case "ident":
                case "int":
                    return br.ReadInt32().ToString();
                case "enum":
                    return br.ReadInt16().ToString();
                case "long_enum":
                    return br.ReadInt32().ToString();
                case "word_flags":
                    tempI = br.ReadInt16();
                    for (int i = 0; i < 16; i++)
                        tempS += (tempI & (1 << i)) > 0 ? 1 : 0;
                    return tempS;
                case "long_flags":
                    tempI = br.ReadInt32();
                    for (int i = 0; i < 32; i++)
                        tempS += (tempI & (1 << i)) > 0 ? 1 : 0;
                    return tempS;
                case "short":
                    return br.ReadInt16().ToString();
                case "stringid":
                    int id = br.ReadInt16();
                    byte t = br.ReadByte();
                    byte l = br.ReadByte();
                    if (id == -1 || map.Strings.Length[id] != l)
                        return "null";
                    else if (l == 0)
                        return "string.Empty";
                    else
                        return map.Strings.Name[id];
                case "unknown":
                case "unused":
                case "struct":
                    return "------";
                default:
                    return string.Empty;
            }
        }

        private int getOffset(Stream stream, int baseOffset, int objOffset)
        {
            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Position = baseOffset + objOffset;
            int mCount = br.ReadInt32();
            int mOffset = br.ReadInt32() - meta.magic - meta.offset;

            if (mCount == 0)
                return -1;
            else 
                return mOffset;
        }

        private string compareValues(int source, int dest)
        {
            /*
            MaxSelections = Math.Min(source, dest);

            lblSourceDestDiff.Visible = false;
            // If we have reflexive chunks or if it is a reflexive tag, disable the button
            if (source != 0)
            {
                btnRestoreValues.Enabled = false;
                lblSourceDestDiff.ForeColor = System.Drawing.Color.Red;
                lblSourceDestDiff.Text = "No selections made";
                lblSourceDestDiff.Visible = true;

            }
            else if (tn[lastNode].Nodes.Count > 0)
                btnRestoreValues.Enabled = false;
            else
                btnRestoreValues.Enabled = true;
            */
            return string.Empty;
        }

        private void loadValues(TreeNode tn, ListBox lb)
        {
            lb.Items.Clear();
            if (tn.Nodes.Count > 0 || tn.ForeColor == System.Drawing.Color.LightGray)
                return;

            reflexiveData rd = (reflexiveData)tn.Tag;
            BinaryReader br = new BinaryReader(fStream);
            for (int i = 0; i < rd.chunkCount; i++)
            {
                br.BaseStream.Position = rd.baseOffset + rd.reflexive.offset + i * rd.reflexive.chunkSize;
                
                //getDataFromBinaryStream(br, ObjectType.ToString());
            }
        }

        private void tvSourceTags_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvSourceTags.SelectedNode == null)
                return;
            int lastNode = tvSourceTags.SelectedNode.Level;
            TreeNode[] tn = new TreeNode[lastNode + 1];
            tn[lastNode] = tvSourceTags.SelectedNode;
            for (int x = lastNode - 1; x >= 0; x--)
                tn[x] = tn[x + 1].Parent;


            string[] offsets = tn[lastNode].Name.Split('\\');

            #region source data
            int baseOffsetS = 0;
            int chunkCountS = 0;            
            for (int x = 0; x < offsets.Length - 1; x++)
            {
                tagInfo ti = ((tagInfo)tn[x].Tag);
                baseOffsetS = getOffset(fStream, baseOffsetS, int.Parse(offsets[x])) + ti.chunkSelection * ti.size;
                if (x == offsets.Length - 2)
                {
                    chunkCountS = ti.chunkCount;
                }
            }

            int size = 0;
            if (offsets.Length > 1)
                size = ((tagInfo)tn[lastNode-1].Tag).size;
            lbSourceIndices.Items.Clear();

            if (baseOffsetS != -1)
            {
                BinaryReader brS = new BinaryReader(fStream);
                for (int i = 0; i < chunkCountS; i++)
                {
                    brS.BaseStream.Position = baseOffsetS + int.Parse(offsets[offsets.Length - 1]) + i * size;
                    lbSourceIndices.Items.Add(getDataFromBinaryStream(brS, tn[lastNode].ToolTipText.Substring(1, tn[lastNode].ToolTipText.IndexOf(']') - 1)));
                }
            }

            if (lastNode != 0)
            {
                reflexiveData rd = ((reflexiveData)tn[lastNode - 1].Tag);
                baseOffsetS = rd.baseOffset;
                // Don't show all for refelxives as we can't do full reflexives anyways
                if (tn[lastNode].Nodes.Count == 0)
                    chunkCountS = rd.chunkCount;

                if (lastNode > 1)
                {
                    while (panel1.Controls.Count > lastNode - 1)
                        panel1.Controls.RemoveAt(panel1.Controls.Count - 1);
                    for (int x = 0; x <= lastNode - 2; x++)
                    {
                        rd = (reflexiveData)tn[x + 1].Tag;

                        ComboBox cb;
                        if (panel1.Controls.Count <= x)
                        {
                            cb = new ComboBox();
                            cb.Dock = DockStyle.Right;
                            cb.DropDownStyle = ComboBoxStyle.DropDownList;
                            cb.Size = new System.Drawing.Size(80, 21);
                            cb.Size = new System.Drawing.Size(panel1.Width / (tn.Length - 2), 21);
                            cb.SelectedIndexChanged += new EventHandler(cbSourceReflexiveNumber_SelectedIndexChanged);
                            panel1.Controls.Add(cb);
                        }
                        else
                        {
                            cb = (ComboBox)panel1.Controls[x];
                            cb.Size = new System.Drawing.Size(panel1.Width / (tn.Length - 2), 21);
                        }
                        if (rd.chunkCount == 0)
                        {
                            lblSourceReflexiveNumber.Enabled = false;
                            cb.Enabled = false;
                        }
                        else
                        {
                            if (cb.Items.Count != rd.chunkCount)
                            {
                                cb.Items.Clear();
                                for (int i = 0; i < rd.chunkCount; i++)
                                    cb.Items.Add((i + 1).ToString() + ": " + tn[x + 1].Text.Substring(0, tn[x + 1].Text.LastIndexOf('[')));
                                cb.SelectedIndex = 0;
                            }
                            lblSourceReflexiveNumber.Enabled = true;
                            cb.Enabled = true;
                        }
                    }
                }
                else
                {
                    lblSourceReflexiveNumber.Enabled = false;
                    cbSourceReflexiveNumber.Enabled = false;
                }
                
            }
            #endregion
            loadValues(tn[lastNode], lbSourceIndices);

            btnSelectAll_Click(sender, e);
        }

        private void tvDestTags_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int lastNode = tvDestTags.SelectedNode.Level;
            TreeNode[] tn = new TreeNode[lastNode + 1];
            tn[lastNode] = tvDestTags.SelectedNode;

            string[] offsets = tn[lastNode].Name.Split('\\');

            #region source data
            int baseOffsetS = 0;
            int baseOffsetD = 0;
            int chunkCountS = 0;
            int chunkCountD = 0;
            for (int x = 0; x < offsets.Length; x++)
            {
                tagInfo ti = ((tagInfo)tn[x].Tag);
                baseOffsetS = getOffset(fStream, baseOffsetS, int.Parse(offsets[x])) + ti.chunkSelection * ti.size;
                //baseOffsetD = getOffset(meta.MS, baseOffsetD, int.Parse(offsets[x])) + ti.chunkSelectionD * ti.size;
                if (x == offsets.Length - 1)
                {
                    chunkCountS = ti.chunkCount;
                }
            }

            /*
            BinaryReader brS = new BinaryReader(fsS);
            int baseOffsetS = 0;
            int chunkCountS = 1;
            if (sn.Parent != null)
            {
                tagInfo ti = ((tagInfo)sn.Parent.Tag);
                baseOffsetS = ti.offset;
                // Don't show all for refelxives as we can't do full reflexives anyways
                if (sn.Nodes.Count == 0)
                    chunkCountS = ti.chunkCount;

                if (sn.Parent.Parent != null)
                {
                    ti = (tagInfo)sn.Parent.Parent.Tag;
                    if (comboBox1.Items.Count != ti.chunkCount)
                    {
                        comboBox1.Items.Clear();
                        for (int i = 0; i < ti.chunkCount; i++)
                            comboBox1.Items.Add(i + 1);
                    }
                    lblReflexiveNumber.Enabled = true;
                    comboBox1.Enabled = true;
                }
                else
                {
                    lblReflexiveNumber.Enabled = false;
                    comboBox1.Enabled = false;
                }

            }
             * 
            int size = 0;
            if (offsets.Length > 1)
                size = ((tagInfo)sn.Parent.Tag).size;
            lbSourceIndices.Items.Clear();
            for (int i = 0; i < chunkCountS; i++)
            {
                brS.BaseStream.Position = baseOffsetS + int.Parse(offsets[offsets.Length - 1]) + i * size;
                lbSourceIndices.Items.Add(getDataFromBinaryStream(brS, sn.ToolTipText.Substring(1, sn.ToolTipText.IndexOf(']')-1 )));
            }
            brS.Close();
            */
            #endregion

            /*
            #region destination data
            BinaryReader brD = new BinaryReader(meta.MS);
            int baseOffsetD = 0;
            int chunkCountD = 1;
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                brD.BaseStream.Position = baseOffsetD + int.Parse(offsets[i]);
                if (tn[lastNode].Nodes.Count == 0)
                    chunkCountD = brD.ReadInt32();
                else
                    brD.ReadInt32();
                baseOffsetD = (brD.ReadInt32() - meta.magic - meta.offset);
                if (chunkCountD == 0)
                    break;
            }

            lbDestIndices.Items.Clear();
            for (int i = 0; i < chunkCountD; i++)
            {
                brD.BaseStream.Position = baseOffsetD + int.Parse(offsets[offsets.Length - 1]) + i * size;
                lbDestIndices.Items.Add(getDataFromBinaryStream(brD, tn[lastNode].ToolTipText.Substring(1, tn[lastNode].ToolTipText.IndexOf(']') - 1)));
            }
            #endregion


            MaxSelections = Math.Min(chunkCountS, chunkCountD);

            lblSourceDestDiff.Visible = false;
            // If we have reflexive chunks or if it is a reflexive tag, disable the button
            if (chunkCountS != 0)
            {
                btnRestoreValues.Enabled = false;
                lblSourceDestDiff.ForeColor = System.Drawing.Color.Red;
                lblSourceDestDiff.Text = "No selections made";
                lblSourceDestDiff.Visible = true;

            }
            else if (tn[lastNode].Nodes.Count > 0)
                btnRestoreValues.Enabled = false;
            else
                btnRestoreValues.Enabled = true;

            btnSelectAll_Click(sender, e);
            */
        }

        void lbIndices_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (lb.SelectedIndex == -1)
                    return;
                object o = lb.Tag;
                
                // Get our index number by mouse location within the box
                int index = e.Y / lb.ItemHeight + lb.TopIndex;
                
                // Make sure the mouse is WITHIN the box
                if (e.X < 0 || e.X > lb.Width)
                    return;
                
                // lb.Tag holds wether we are mass selecting (true) or deselecting (false)
                // If it is null, we are making the original selection choice
                if (o == null)
                    lb.Tag = lb.GetSelected(index);
                else
                {
                    // Only allow as many selections as can be cloned
                    if ((bool)lb.Tag == true && lb.SelectedItems.Count >= MaxSelections)
                        return;

                    // We need this check b/c of the way we get our index, it is possible for it to be
                    // out of bounds
                    if (index >= 0 && index < lb.Items.Count)
                        lb.SetSelected(index, (bool)lb.Tag);
                }
            }
            else
            {
                lb.Tag = null;
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            // This way selections between source and dest will match, even if they contain
            // different numbers of reflexives
            for (int i = 0; i < MaxSelections; i++)
            {
                lbSourceIndices.SetSelected(i, true);
                lbDestIndices.SetSelected(i, true);
            }
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbSourceIndices.Items.Count; i++)
                lbSourceIndices.SetSelected(i, false);
            for (int i = 0; i < lbDestIndices.Items.Count; i++)
                lbDestIndices.SetSelected(i, false);
        }

        private void lbIndices_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selSource = lbSourceIndices.SelectedItems.Count;
            int selDest = lbDestIndices.SelectedItems.Count;
            lblSourceDestDiff.ForeColor = System.Drawing.Color.Red;
            btnRestoreValues.Enabled = false;
            if (selSource > selDest)
            {
                lblSourceDestDiff.Text = "Source +" + (selSource - selDest) + " too many";
            }
            else if (selSource < selDest)
            {
                lblSourceDestDiff.Text = "Destination +" + (selDest - selSource) + " too many";
            }
            else
            {
                if (selSource == 0)
                {
                    lblSourceDestDiff.Text = "No chunks selected";
                }
                else
                {
                    lblSourceDestDiff.ForeColor = System.Drawing.Color.Blue;
                    lblSourceDestDiff.Text = "Selections match";
                    btnRestoreValues.Enabled = true;
                }
            }
        }

        private void cbSourceReflexiveNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Save selections

            // Figure out which combobox was changed (position will denote level of TreeView Node)
            ComboBox cb = (ComboBox)sender;
            int level = cb.Parent.Controls.IndexOf(cb);

            TreeNode tn = tvSourceTags.SelectedNode;
            while ((tn.Level - 1) > level)
                tn = tn.Parent;
            ((reflexiveData)tn.Tag).chunkSelected = cb.SelectedIndex;

            tvSourceTags.refreshTreeListing(tn);
            /*
                    int i = tn[x].Text.LastIndexOf(" [");
                    tn[x].Text = tn[x].Text.Substring(0, i);
                    tn[x].Text += " [" + (rd.chunkSelected + 1).ToString() + "/" + rd.chunkCount.ToString() + "]";
            */

            // Update Source/Dest box listings
            tvSourceTags_AfterSelect(sender, null);

            // Restore selections
        }
    }
}