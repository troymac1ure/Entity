// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChunkClonerWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The chunk cloner window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Windows.Forms;

    using HaloMap.ChunkCloning;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;

    /// <summary>
    /// The chunk cloner window.
    /// </summary>
    /// <remarks></remarks>
    public partial class ChunkClonerWindow : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The result.
        /// </summary>
        public bool result;

        /// <summary>
        /// The tag number.
        /// </summary>
        private readonly int TagNumber;

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The metasplit.
        /// </summary>
        private MetaSplitter metasplit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkClonerWindow"/> class.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public ChunkClonerWindow(int tagIndex, Map map)
        {
            this.map = map;
            TagNumber = tagIndex;

            // Draws all components on Form
            InitializeComponent();
            SplitIFP();
            TreeNode tn = new TreeNode("Header");

            // Adds nodes to tree in Chunk Cloner Window
            DisplaySplit(metasplit.Header, tn);
            tn.Expand();
            tn.Nodes[0].Expand();
            treeView1.Nodes.Add(tn);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This expands a TreeView to the SearchPath given recursively
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="SearchPath">The Search Path.</param>
        /// <param name="PathSeperator">The Path Seperator.</param>
        /// <remarks></remarks>
        public static void ExpandToNode(TreeNodeCollection tree, string SearchPath, string PathSeperator)
        {
            string s;
            if (SearchPath.IndexOf(PathSeperator) == -1)
            {
                s = SearchPath;
            }
            else
            {
                s = SearchPath.Substring(0, SearchPath.IndexOf(PathSeperator));
            }

            foreach (TreeNode node in tree)
            {
                if (node.Text == s)
                {
                    node.Expand();
                    node.TreeView.SelectedNode = node;
                    ExpandToNode(node.Nodes, SearchPath.Substring(SearchPath.IndexOf(PathSeperator) + 1), PathSeperator);
                }
            }
        }

        /// <summary>
        /// The find selected node.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <remarks></remarks>
        public void FindSelectedNode(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            if (reflex.ChunkResources.Count == 0 && reflex.Chunks.Count == 0)
            {
                return;
            }

            int refcount = 0;
            foreach (TreeNode node in tn.Nodes)
            {
                if (node == treeView1.SelectedNode)
                {
                    if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                    {
                        map.ChunkTools.ChunkClipBoard = reflex.Chunks[refcount];
                        return;
                    }
                    else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                    {
                        do
                        {
                            if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                            {
                                map.ChunkTools.ChunkClipBoard = (MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount];

                                // map.ChunkTools.ChunkClipBoard=(MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount];
                                return;
                            }

                            refcount++;
                        }
                        while (refcount != -1);
                    }
                }

                if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                {
                    FindSelectedNode(reflex.Chunks[refcount], node);

                    refcount++;
                }
                else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                {
                    do
                    {
                        if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                        {
                            FindSelectedNode((MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount], node);
                            refcount++;
                            break;
                        }

                        refcount++;
                    }
                    while (refcount != -1);

                    // refcount++;
                }
            }
        }

        /// <summary>
        /// The find selected node and add.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MetaSplitter.SplitReflexive FindSelectedNodeAndAdd(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            if (reflex.ChunkResources.Count == 0 && reflex.Chunks.Count == 0)
            {
                return reflex;
            }

            int refcount = 0;
            foreach (TreeNode node in tn.Nodes)
            {
                if (node == treeView1.SelectedNode)
                {
                    if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                    {
                        if (map.ChunkTools.ChunkClipBoard.splitReflexiveType ==
                            MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                        {
                            MetaSplitter.SplitReflexive split;
                            split = reflex.Chunks[refcount];
                            for (int x = 0; x < split.ChunkResources.Count; x++)
                            {
                                if (split.ChunkResources[x].type == Meta.ItemType.Reflexive)
                                {
                                    MetaSplitter.SplitReflexive tempref;
                                    tempref = split.ChunkResources[x] as MetaSplitter.SplitReflexive;
                                    if (tempref.description == map.ChunkTools.ChunkClipBoard.description &&
                                        tempref.chunksize == map.ChunkTools.ChunkClipBoard.chunksize)
                                    {
                                        tempref.Chunks.AddRange(map.ChunkTools.ChunkClipBoard.Chunks);

                                        // }
                                        split.ChunkResources[x] = tempref;
                                        reflex.Chunks[refcount] = split;
                                        return reflex;
                                    }
                                }
                            }

                            if (reflex != map.ChunkTools.ChunkClipBoard)
                            {
                                if (reflex.offset <= reflex.Chunks[refcount].chunksize - 8)
                                {
                                    int lowindex = -1;
                                    int lowoffset = 0;

                                    int highoffset = reflex.Chunks[refcount].chunksize;
                                    for (int e = 0; e < reflex.Chunks[refcount].ChunkResources.Count; e++)
                                    {
                                        Meta.Item mi = reflex.Chunks[refcount].ChunkResources[e];

                                        if (mi.offset < map.ChunkTools.ChunkClipBoard.offset && mi.offset > lowoffset)
                                        {
                                            lowindex = e;
                                            lowoffset = mi.offset;
                                        }
                                    }

                                    if (lowindex == -1)
                                    {
                                        reflex.Chunks[refcount].ChunkResources.Insert(0, map.ChunkTools.ChunkClipBoard);
                                    }
                                    else
                                    {
                                        reflex.Chunks[refcount].ChunkResources.Insert(
                                            lowindex + 1, map.ChunkTools.ChunkClipBoard);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "The offset of the reflexive you are trying to paste is too high for this chunk size");
                                }
                            }
                            else
                            {
                                MessageBox.Show("You can not add a reflexive inside of itself");
                            }
                        }
                        else
                        {
                            reflex.Chunks.Insert(refcount + 1, map.ChunkTools.ChunkClipBoard);

                            // MessageBox.Show("Add chunks to reflexives not other chunks");
                        }

                        return reflex;

                        // reflex.chunkcount += map.ChunkTools.ChunkClipBoard.Chunks.Count;
                    }
                    else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                    {
                        do
                        {
                            if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                            {
                                // ((MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount]).Chunks.Add(ChunkAdder.ChunkClipBoard);
                                if (map.ChunkTools.ChunkClipBoard.splitReflexiveType ==
                                    MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                                {
                                    ((MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount]).Chunks.Add(
                                        map.ChunkTools.ChunkClipBoard);
                                }
                                else
                                {
                                    MessageBox.Show("Add reflexives to chunks not other reflexives");
                                }

                                // reflex.chunkcount += map.ChunkTools.ChunkClipBoard.ChunkResources.Count;
                                return reflex;
                            }

                            refcount++;
                        }
                        while (refcount != -1);
                    }
                }

                if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                {
                    reflex.Chunks[refcount] = FindSelectedNodeAndAdd(reflex.Chunks[refcount], node);

                    refcount++;
                }
                else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                {
                    do
                    {
                        if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                        {
                            reflex.ChunkResources[refcount] =
                                FindSelectedNodeAndAdd(
                                    (MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount], node);
                            refcount++;
                            break;
                        }

                        refcount++;
                    }
                    while (refcount != -1);

                    // refcount++;
                }
            }

            return reflex;
        }

        /// <summary>
        /// The find selected node and clone.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MetaSplitter.SplitReflexive FindSelectedNodeAndClone(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            if (reflex.ChunkResources.Count == 0 && reflex.Chunks.Count == 0)
            {
                return reflex;
            }

            int refcount = 0;
            foreach (TreeNode node in tn.Nodes)
            {
                if (node == treeView1.SelectedNode)
                {
                    if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                    {
                        int fff = Convert.ToInt32(chunkamount.Text);
                        for (int x = 0; x < fff; x++)
                        {
                            reflex.Chunks.Insert(refcount + 1, reflex.Chunks[refcount]);

                            /*****
                                 * I thought we had to do this, but I really don't know. The offsets are messed, but
                                 * I think fixed when the map is refreshed.
                                 *****
                                MetaSplitter.SplitReflexive newChunk = new MetaSplitter.SplitReflexive(reflex.Chunks[refcount]);
                                // Is there ever any more than one ChunkResource?
                                for (int xx = 0; xx < newChunk.ChunkResources.Count; xx++)
                                {
                                    newChunk.ChunkResources[xx].mapOffset += (x + 1) * newChunk.chunksize;
                                    ((MetaSplitter.SplitIdent)newChunk.ChunkResources[xx]).inchunknumber = ((MetaSplitter.SplitIdent)(reflex.Chunks[refcount].ChunkResources[xx])).inchunknumber + x;
                                    ((MetaSplitter.SplitIdent)newChunk.ChunkResources[xx]).ident = ((MetaSplitter.SplitIdent)(reflex.Chunks[refcount].ChunkResources[xx])).ident;
                                    ((MetaSplitter.SplitIdent)newChunk.ChunkResources[xx]).pointstotagname = ((MetaSplitter.SplitIdent)(reflex.Chunks[refcount].ChunkResources[xx])).pointstotagname;
                                    ((MetaSplitter.SplitIdent)newChunk.ChunkResources[xx]).pointstoTagIndex = ((MetaSplitter.SplitIdent)(reflex.Chunks[refcount].ChunkResources[xx])).pointstoTagIndex;
                                    ((MetaSplitter.SplitIdent)newChunk.ChunkResources[xx]).pointstotagtype = ((MetaSplitter.SplitIdent)(reflex.Chunks[refcount].ChunkResources[xx])).pointstotagtype;
                                }
                                reflex.Chunks.Insert(refcount+1,newChunk);
                                */
                        }

                        return reflex;

                        // reflex.chunkcount += map.ChunkTools.ChunkClipBoard.Chunks.Count;
                    }
                    else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                    {
                        MessageBox.Show("Only Clone Chunks");
                        return reflex;
                    }
                }

                if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                {
                    reflex.Chunks[refcount] = FindSelectedNodeAndClone(reflex.Chunks[refcount], node);

                    refcount++;
                }
                else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                {
                    do
                    {
                        if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                        {
                            reflex.ChunkResources[refcount] =
                                FindSelectedNodeAndClone(
                                    (MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount], node);
                            refcount++;
                            break;
                        }

                        refcount++;
                    }
                    while (refcount != -1);

                    // refcount++;
                }
            }

            return reflex;
        }

        /// <summary>
        /// The find selected node and delete.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MetaSplitter.SplitReflexive FindSelectedNodeAndDelete(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            if (reflex.ChunkResources.Count == 0 && reflex.Chunks.Count == 0)
            {
                return reflex;
            }

            int refcount = 0;
            foreach (TreeNode node in tn.Nodes)
            {
                if (node == treeView1.SelectedNode)
                {
                    if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                    {
                        reflex.Chunks.RemoveAt(refcount);
                        return reflex;

                        // reflex.chunkcount += map.ChunkTools.ChunkClipBoard.Chunks.Count;
                    }
                    else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                    {
                        do
                        {
                            if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                            {
                                reflex.ChunkResources.RemoveAt(refcount);
                                return reflex;
                            }

                            refcount++;
                        }
                        while (refcount != -1);
                    }
                }

                if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                {
                    reflex.Chunks[refcount] = FindSelectedNodeAndDelete(reflex.Chunks[refcount], node);

                    refcount++;
                }
                else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                {
                    do
                    {
                        if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                        {
                            reflex.ChunkResources[refcount] =
                                FindSelectedNodeAndDelete(
                                    (MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount], node);
                            refcount++;
                            break;
                        }

                        refcount++;
                    }
                    while (refcount != -1);

                    // refcount++;
                }
            }

            return reflex;
        }

        /// <summary>
        /// The find selected node and over write.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="tn">The tn.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MetaSplitter.SplitReflexive FindSelectedNodeAndOverWrite(MetaSplitter.SplitReflexive reflex, TreeNode tn)
        {
            if (reflex.ChunkResources.Count == 0 && reflex.Chunks.Count == 0)
            {
                return reflex;
            }

            int refcount = 0;
            foreach (TreeNode node in tn.Nodes)
            {
                if (node == treeView1.SelectedNode)
                {
                    if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                    {
                        if (map.ChunkTools.ChunkClipBoard.splitReflexiveType ==
                            MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                        {
                            reflex.Chunks[refcount] = map.ChunkTools.ChunkClipBoard;
                        }
                        else
                        {
                            MessageBox.Show("Overwrite chunks with chunks");
                        }

                        return reflex;
                    }
                    else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                    {
                        do
                        {
                            if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                            {
                                if (map.ChunkTools.ChunkClipBoard.splitReflexiveType ==
                                    MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                                {
                                    reflex.ChunkResources[refcount] = map.ChunkTools.ChunkClipBoard;
                                }
                                else
                                {
                                    MessageBox.Show("Overwrite reflexives with reflexives");
                                }

                                return reflex;
                            }

                            refcount++;
                        }
                        while (refcount != -1);
                    }
                }

                if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Container)
                {
                    reflex.Chunks[refcount] = FindSelectedNodeAndOverWrite(reflex.Chunks[refcount], node);

                    refcount++;
                }
                else if (reflex.splitReflexiveType == MetaSplitter.SplitReflexive.SplitReflexiveType.Chunk)
                {
                    do
                    {
                        if (reflex.ChunkResources[refcount].type == Meta.ItemType.Reflexive)
                        {
                            reflex.ChunkResources[refcount] =
                                FindSelectedNodeAndOverWrite(
                                    (MetaSplitter.SplitReflexive)reflex.ChunkResources[refcount], node);
                            refcount++;
                            break;
                        }

                        refcount++;
                    }
                    while (refcount != -1);

                    // refcount++;
                }
            }

            return reflex;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get base type string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The get base type string.</returns>
        /// <remarks></remarks>
        private static string GetBaseTypeString(string name)
        {
            string[] split = name.Split('-');
            string tempstring = split[0].Trim();
            return tempstring;
        }

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

                /**** This uses the labels in the .ENT plugins ***/
                foreach (Meta.Item mi in reflex.Chunks[x].ChunkResources)
                {
                    if (mi.description == reflex.label)
                    {
                        if (mi.type == Meta.ItemType.String)
                        {
                            tempchunk = ((Meta.String)mi).name;
                            break;
                        }

                        if (mi.type == Meta.ItemType.Ident)
                        {
                            tempchunk = ((Meta.Ident)mi).pointstotagname;
                            string[] splits = tempchunk.Split('\\');
                            tempchunk = splits[splits.Length - 1] + "." + ((Meta.Ident)mi).pointstotagtype;
                            break;
                        }
                    }
                }
                /**/

                string tempchunkname = "Chunk - #" + x + " - Name: " + tempchunk + " - Size: " +
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
        /// The split ifp.
        /// </summary>
        /// <remarks></remarks>
        private void SplitIFP()
        {
            Meta m = new Meta(map);

            // Reads the current TAGNumber Meta
            m.ReadMetaFromMap(TagNumber, false);

            try
            {
                IFPIO ifpx = IFPHashMap.GetIfp(m.type, map.HaloVersion);
                m.headersize = ifpx.headerSize;

                // Scans IFP and loads IDENTS, REFLEXIVES & STRINGS into "m" for Reference List
                m.scanner.ScanWithIFP(ref ifpx);
                metasplit = new MetaSplitter();
                metasplit.SplitWithIFP(ref ifpx, ref m, map);

            }
            catch (Exception ex)
            {
                Globals.Global.ShowErrorMsg(string.Empty, ex);
            }

        }

        /// <summary>
        /// The addchunks_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void addchunks_Click(object sender, EventArgs e)
        {
            FindSelectedNodeAndOverWrite(metasplit.Header, treeView1.Nodes[0]);
            treeView1.Nodes[0].Nodes.Clear();
            DisplaySplit(metasplit.Header, treeView1.Nodes[0]);

            // treeView1.ExpandAll();
        }

        // "Add Meta To Map" in "Chunk Cloner"
        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);

            map.ChunkTools.Add(TagNumber, metasplit);
            result = true;
            this.Close();
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button2_Click(object sender, EventArgs e)
        {
            FindSelectedNodeAndAdd(metasplit.Header, treeView1.Nodes[0]);
            treeView1.Nodes[0].Nodes.Clear();
            DisplaySplit(metasplit.Header, treeView1.Nodes[0]);

            // treeView1.ExpandAll();
        }

        // Chunk Cloner "Clone" button
        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }

            treeView1.BeginUpdate();
            string Path = treeView1.SelectedNode.FullPath;

            // Duplicate node in tree only:
            FindSelectedNodeAndClone(metasplit.Header, treeView1.Nodes[0]);
            treeView1.Nodes[0].Nodes.Clear();
            DisplaySplit(metasplit.Header, treeView1.Nodes[0]);

            // Reselects node after clone
            ExpandToNode(treeView1.Nodes, Path, treeView1.PathSeparator);
            if (treeView1.SelectedNode.NextNode != null)
            {
                treeView1.SelectedNode.Collapse();
                treeView1.SelectedNode = treeView1.SelectedNode.NextNode;
                ExpandToNode(treeView1.Nodes, treeView1.SelectedNode.FullPath, treeView1.PathSeparator);
            }

            treeView1.EndUpdate();
            treeView1.SelectedNode.EnsureVisible();
        }

        // Chunk Clone "Delete Reflex/Chunk" button
        /// <summary>
        /// The button 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button4_Click(object sender, EventArgs e)
        {
            treeView1.BeginUpdate();
            TreeNode temp = treeView1.SelectedNode.Parent;
            if (treeView1.SelectedNode.PrevNode != null)
            {
                temp = treeView1.SelectedNode.PrevNode;
            }

            string Path = temp.FullPath;
            FindSelectedNodeAndDelete(metasplit.Header, treeView1.Nodes[0]);
            treeView1.Nodes[0].Nodes.Clear();
            DisplaySplit(metasplit.Header, treeView1.Nodes[0]);

            // Reselects Parent Node after delete
            ExpandToNode(treeView1.Nodes, Path, treeView1.PathSeparator);
            treeView1.EndUpdate();
            treeView1.SelectedNode.EnsureVisible();
        }

        /// <summary>
        /// The chunkamount_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void chunkamount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int.Parse(chunkamount.Text);
            }
            catch
            {
                int ee = 0;
                chunkamount.Text = ee.ToString();
            }
        }

        /// <summary>
        /// The collapse_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void collapse_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
            treeView1.Nodes[0].Expand();
            treeView1.Nodes[0].Nodes[0].Expand();
        }

        /// <summary>
        /// The copybutton_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void copybutton_Click(object sender, EventArgs e)
        {
            FindSelectedNode(metasplit.Header, treeView1.Nodes[0]);
        }

        // Recursively adds nodes to Chunk Tree Listing

        /// <summary>
        /// The expand_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void expand_Click(object sender, EventArgs e)
        {
            treeView1.Nodes[0].ExpandAll();
        }

        /// <summary>
        /// The savemeta_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void savemeta_Click(object sender, EventArgs e)
        {
            Meta tempm = MetaBuilder.BuildMeta(metasplit, map); // (Meta) metas[TagIndex];

            tempm.RelinkReferences();
            SaveFileDialog save = new SaveFileDialog();
            string[] split = metasplit.name.Split('\\');
            string temp = split[split.Length - 1] + "." + metasplit.type;
            temp = temp.Replace('<', '_');
            temp = temp.Replace('>', '_');
            save.FileName = temp;
            if (save.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            tempm.SaveMetaToFile(save.FileName, true);
            MessageBox.Show("Done");
        }

        #endregion
    }
}