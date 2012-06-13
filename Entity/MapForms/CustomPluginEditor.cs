// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomPluginEditor.cs" company="">
//   
// </copyright>
// <summary>
//   The custom plugin editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Plugins;

    using Microsoft.Win32;
using Globals;

    /// <summary>
    /// The custom plugin editor.
    /// </summary>
    /// <remarks></remarks>
    public partial class CustomPluginEditor : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The changes made.
        /// </summary>
        private bool changesMade; // If the selected tag was edited, this causes reload in Meta Editor

        /// <summary>
        /// The tags changed.
        /// </summary>
        private bool tagsChanged; // If visibility state of tags are changed, this causes TreeView to reload

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPluginEditor"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public CustomPluginEditor(Map map)
        {
            InitializeComponent();
            this.map = map;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The set name box.
        /// </summary>
        /// <param name="initalName">The inital name.</param>
        /// <returns>The set name box.</returns>
        /// <remarks></remarks>
        public string setNameBox(string initalName)
        {
            initalName = GetNameDialog.Show(
                "Choose new plugin name", "Input a name for the new plugin:", initalName, "&Set Name");
            return initalName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create tree.
        /// </summary>
        /// <param name="ifps">The ifps.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private TreeNode[] CreateTree(object[] ifps, string path)
        {
            TreeNode[] tnc = new TreeNode[ifps.Length];
            int count = 0;
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
                }

                tnc[count++] = ChunkNumberNode;
            }

            TreeNode[] tempTN = new TreeNode[count];
            for (int i = 0; i < count; i++)
            {
                tempTN[i] = (TreeNode)tnc[i].Clone();
            }

            return tempTN;
        }

        /// <summary>
        /// The custom plugin editor_ form closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void CustomPluginEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Prefs.Save();

            MapForm mf = (MapForm)this.Owner;

            // If the visiblity state of any tags changed, reload the tags TreeView
            if (tagsChanged)
            {
                mf.RefreshTreeView();
            }

            // if the selected tag type was changed, reload that tag type
            if (changesMade && map.SelectedMeta != null)
            {
                // Null the type so it reloads the tag completely
                mf.metaEditor1.selectedTagType = null;
                mf.LoadMeta(map.SelectedMeta.TagIndex);
            }
        }

        /// <summary>
        /// The custom plugin editor_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void CustomPluginEditor_Load(object sender, EventArgs e)
        {
            // Add each tag listing
            foreach (DictionaryEntry de in map.MetaInfo.TagTypes)
            {
                treeViewTags.Nodes.Add((string)de.Key);
            }

            treeViewTags.Sort();

            // Load all Custom Plugin Names
            foreach (Prefs.CustomPluginMask pluginMask in Prefs.CustomPluginMasks)
            {
                comboBoxPluginName.Items.Add(pluginMask.Name);
            }

            // set to same selection as in mapform
            MapForm mf = (MapForm)((Form)sender).Owner;
            if (mf.comboBox1.SelectedIndex == 0) comboBoxPluginName.SelectedIndex = 0;
            else comboBoxPluginName.SelectedIndex = mf.comboBox1.SelectedIndex - 1;

            if (map.SelectedMeta != null && treeViewTags.Nodes.Count > 0)
            {
                TreeNode tn = treeViewTags.Nodes[0];
                while (tn != null)
                {
                    if (tn.Text == map.SelectedMeta.type)
                    {
                        treeViewTags.SelectedNode = treeViewTags.Nodes[treeViewTags.Nodes.IndexOf(tn)];
                        break;
                    }

                    tn = tn.NextNode;
                }
            }

            saveTagBtn.Enabled = false;
            tagsChanged = false;
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void savetagTypeBtn_Click(object sender, EventArgs e)
        {
            //// Remove old registry entry first
            //RegistryAccess.removeKey(
            //    Registry.CurrentUser, 
            //    CustomPluginPath + comboBoxPluginName.SelectedItem + "\\" + treeViewTags.SelectedNode.Text);

            //// Write to registry / file
            //RegistryAccess.setValue(
            //    Registry.CurrentUser, 
            //    CustomPluginPath + comboBoxPluginName.SelectedItem + "\\" + treeViewTags.SelectedNode.Text, 
            //    string.Empty, 
            //    treeViewTags.SelectedNode.Checked);
            //writeNodesToReg(treeView1.Nodes[0]);

            // RegistryAccess ra = new RegistryAccess(Microsoft.Win32.Registry.CurrentUser, CustomPluginPath);
            saveTagBtn.Enabled = false;
            changesMade = true;
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void renameBtn_Click(object sender, EventArgs e)
        {
            string name = GetNameDialog.Show(
                "Rename the plugin", "Select new name for the plugin:", comboBoxPluginName.Text, "&Rename");
            if (name != null)
            {
                Prefs.CustomPluginMasks[comboBoxPluginName.SelectedIndex].Name = name;
                comboBoxPluginName.Items[comboBoxPluginName.SelectedIndex] = new string(name.ToCharArray());
            }
        }

        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dupBtn_Click(object sender, EventArgs e)
        {
            string name = GetNameDialog.Show(
                "Select duplicated plugin name", 
                "Select name for the duplicated plugin:", 
                comboBoxPluginName.Text, 
                "&Duplicate");
            if (name != null)
            {
                Prefs.CustomPluginMask newMask = new Prefs.CustomPluginMask();
                newMask.Name = Prefs.CustomPluginMasks[comboBoxPluginName.SelectedIndex].Name;
                newMask.VisibleTagTypes.AddRange(Prefs.CustomPluginMasks[comboBoxPluginName.SelectedIndex].VisibleTagTypes); 
                Prefs.CustomPluginMasks .Add(newMask);

                comboBoxPluginName.SelectedIndex = comboBoxPluginName.Items.Add(name);
            }
        }

        /// <summary>
        /// The button 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void delBtn_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Delete plugin:\n" + comboBoxPluginName.Text, "Delete Plugin?", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                Prefs.CustomPluginMasks.RemoveAt(comboBoxPluginName.SelectedIndex);
                int oldIndex = comboBoxPluginName.SelectedIndex;
                comboBoxPluginName.Items.RemoveAt(comboBoxPluginName.SelectedIndex);
                if (comboBoxPluginName.SelectedIndex == -1)
                {
                    if (comboBoxPluginName.Items.Count == 0)
                    {
                        this.Close();
                    }
                    else if (oldIndex > 0 && comboBoxPluginName.Items.Count > oldIndex - 1)
                    {
                        comboBoxPluginName.SelectedIndex = oldIndex - 1;
                    }
                    else
                    {
                        comboBoxPluginName.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// The button 5_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button5_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count == 0)
            {
                return;
            }

            // We only need to set the first nodes of selection, as the sub-nodes are updated on treeView1_AfterCheck()
            TreeNode tn = treeView1.Nodes[0];
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
            if (treeView1.Nodes.Count == 0)
            {
                return;
            }

            // We only need to set the first nodes of selection, as the sub-nodes are updated on treeView1_AfterCheck()
            TreeNode tn = treeView1.Nodes[0];
            while (tn != null)
            {
                tn.Checked = false;
                tn = tn.NextNode;
            }
        }

        // Plugin Names Box
        /// <summary>
        /// The combo box plugin name_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void comboBoxPluginName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reload tag box check marks
            if (treeViewTags.Nodes.Count <= 0)
            {
                return;
            }

            Prefs.CustomPluginMask mask = Prefs.CustomPluginMasks[comboBoxPluginName.SelectedIndex];
            TreeNode tn = treeViewTags.Nodes[0];
            while (tn != null)
            {
                if (mask.VisibleTagTypes.Contains(tn.Text))
                {
                    tn.Checked = true;
                }
                else
                {
                    tn.Checked = false;
                }

                // CreateTree(ifpx.items, tn, path);
                tn = tn.NextNode;
            }

            // Reload selected tag info
            tn = treeViewTags.SelectedNode;
            treeViewTags.SelectedNode = null;
            treeViewTags.SelectedNode = tn;
        }

        // Recursively adds nodes to Chunk Tree Listing

        /// <summary>
        /// The tree view 1_ after check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            saveTagBtn.Enabled = true;
            TreeNode tn = e.Node;
            foreach (TreeNode tnt in tn.Nodes)
            {
                tnt.Checked = tn.Checked;
            }

            tagsChanged = true;
            treeViewTags.SelectedNode.Checked = true;
        }

        /// <summary>
        /// The tree view tags_ after check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeViewTags_AfterCheck(object sender, TreeViewEventArgs e)
        {
            Prefs.CustomPluginMask mask = Prefs.CustomPluginMasks[comboBoxPluginName.SelectedIndex];
            if (e.Node.Checked)
            {
                if (!mask.VisibleTagTypes.Contains(e.Node.Text)) mask.VisibleTagTypes.Add(e.Node.Text);
            }
            else
            {
                mask.VisibleTagTypes.Remove(e.Node.Text);
            }
            tagsChanged = true;
        }

        /// <summary>
        /// The tree view tags_ after select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeViewTags_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.Nodes.Clear();
            IFPIO ifpx = IFPHashMap.GetIfp(e.Node.Text, map.HaloVersion);

            // path stores parent offsets
            string path = string.Empty;

            treeView1.Nodes.AddRange(CreateTree(ifpx.items, path));
        }

        /// <summary>
        /// The write nodes to reg.
        /// </summary>
        /// <param name="tn">The tn.</param>
        /// <remarks></remarks>
        private void writeNodesToReg(TreeNode tn)
        {
            while (tn != null)
            {
                //RegistryAccess.setValue(
                //    Registry.CurrentUser, 
                //    CustomPluginPath + comboBoxPluginName.SelectedItem + "\\" + treeViewTags.SelectedNode.Text, 
                //    tn.Name, 
                //    tn.Checked);
                if (tn.Nodes.Count > 0)
                {
                    writeNodesToReg(tn.Nodes[0]);
                }

                string s = tn.Text;
                tn = tn.NextNode;
            }
        }

        #endregion
    }
}