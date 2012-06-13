using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using HaloMap.Map;
using HaloMap.Meta;
using HaloMap.Plugins;

namespace entity.MetaEditor2
{
    class TagTreeView : TreeView
    {
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

        IFPIO ifp;
        /// <summary>
        /// The Halo version being referred to
        /// </summary>
        HaloVersionEnum HaloVersion;
        /// <summary>
        /// The source Meta
        /// </summary>
        Meta meta = null;
        /// <summary>
        /// For files, holds the filename for always using up-to-date data
        /// </summary>
        string fileName;
        /// <summary>
        /// The stream used to hold the raw file data
        /// </summary>
        private MemoryStream memStream;

        [DefaultValue(true)]
        bool hideUnusedReflexives;
        [DefaultValue(true)]
        bool sortByName;

		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool HideUnusedReflexives
        {
            get { return hideUnusedReflexives; }
            set
            {
                hideUnusedReflexives = value;
                createTreeListing();
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool SortByName
        {
            get { return sortByName; }
            set
            {
                sortByName = value;
                createTreeListing();
            }
        }

        public TagTreeView()
        {
        }

        private void loadStream(Stream stream)
        {
            if (memStream != null)
                memStream.Dispose();
            // Copy the stream to memory
            memStream = new MemoryStream();
            stream.Position = 0;
            byte[] b = new byte[stream.Length];
            stream.Read(b, 0, b.Length);
            memStream.Write(b, 0, b.Length);
        }

        private void loadFile()
        {
            if (fileName == string.Empty)
                return;
            FileStream fs = new FileStream(this.fileName, FileMode.Open);
            fs.Position = 0;
            byte[] b = new byte[fs.Length];
            fs.Read(b, 0, b.Length);
            fs.Close();
            if (memStream != null)
                memStream.Dispose();
            memStream = new MemoryStream(b);
        }

        public void loadMeta(Meta meta, HaloVersionEnum HaloVersion, string FileName)
        {
            this.fileName = fileName;
            this.HaloVersion = HaloVersion;
            this.meta = meta;

            loadFile();

            createTreeListing();
        }

        public void loadMeta(Meta meta, HaloVersionEnum HaloVersion, Stream stream)
        {
            this.fileName = string.Empty;
            this.HaloVersion = HaloVersion;
            this.meta = meta;

            loadStream(stream);

            createTreeListing();
        }

        private void createTreeListing()
        {
            if (meta == null)
                return;
            ifp = HaloMap.Plugins.IFPHashMap.GetIfp(meta.type, HaloVersion);

            #region Save info about our current Selected Node
            TreeNode node = this.SelectedNode;
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

            this.Nodes.Clear();
            this.Sorted = sortByName;
            this.Nodes.Add("0", ".:[ MAIN ]:.");
            reflexiveData rd = new reflexiveData();
            this.Nodes[0].Tag = rd;
            rd.node = this.Nodes[0];
            rd.chunkCount = 1;
            rd.chunkSelected = 0;
            rd.baseOffset = 0; // meta.offset;
            rd.inTagNumber = meta.TagIndex;
            refData.Clear();
            refData.Add(rd);

            BinaryReader br = new BinaryReader(memStream);
            this.Nodes[0].Nodes.AddRange(loadTreeReflexives(br, 0, ifp.items, true)); //meta.offset

            //this.ExpandAll();
            this.Nodes[0].Expand();

            #region Re-Select our previously selected node
            TreeNodeCollection nodes = this.Nodes[0].Nodes;
            this.Enabled = false;
            this.SelectedNode = this.Nodes[0];
            for (int i = 1; i < path.Length; i++)
            {
                foreach (TreeNode tn in nodes)
                {
                    if (((reflexiveData)tn.Tag).reflexive.offset.ToString() == path[i])
                    {
                        this.SelectedNode = tn;
                        nodes = tn.Nodes;
                        break;
                    }
                }
            }
            // If we didn't get the right node, deselect all nodes
            if (this.SelectedNode.Level != path.Length - 1)
                this.SelectedNode = null;
            this.Enabled = true;
            #endregion
        }

        public TreeNode[] loadTreeReflexives(BinaryReader BR, int metaOffset, object[] items, bool enabled)
        {
            List<TreeNode> tns = new List<TreeNode>();
            foreach (object o in items)
            {
                TreeNode tn = new TreeNode(((IFPIO.BaseObject)o).name);
                if (o is IFPIO.Reflexive)
                {
                    IFPIO.Reflexive IFPR = (IFPIO.Reflexive)o;
                    reflexiveData rd = new reflexiveData();
                    tn.Tag = rd;
                    rd.node = tn;
                    rd.reflexive = IFPR;
                    tn.ForeColor = Color.LightGray;
                    tn.Name = IFPR.name;
                    tn.ToolTipText = "Offset: " + rd.reflexive.offset.ToString();

                    if (enabled)
                    {
                        BR.BaseStream.Position = IFPR.offset + metaOffset;
                        rd.chunkCount = BR.ReadInt32();
                        if (rd.chunkCount > 0)
                        {
                            rd.chunkSelected = 0;
                            tn.ForeColor = Color.Black;
                            rd.baseOffset = BR.ReadInt32() - meta.magic - meta.offset;
                            // Some listings are actually in other tags!
                            // Check [BLOC] "objects\\vehicles\\wraith\\garbage\\wing_boost\\wing_boost"
                            //   > attachments[0] = [BLOC] "objects\\vehicles\\ghost\\garbage\\seat\\seat"
                            if (rd.baseOffset < 0 || rd.baseOffset >= memStream.Length)
                            {
                                rd.chunkCount = 0;
                            }
                            else
                            {
                                rd.inTagNumber = meta.TagIndex;
                            }
                            /*
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
                            */
                        }
                    }
                    tn.Text += " [" + rd.chunkCount.ToString() + "]";

                    refData.Add(rd);

                    // Add if non-existant, otherwise update Text
                    /*
                    if (rd.inTagNumber == meta.TagIndex)
                        tn.Nodes.AddRange(loadTreeReflexives(BR, meta.offset + rd.baseOffset + rd.chunkSelected * rd.reflexive.chunkSize, IFPR.items, rd.chunkCount != 0));
                    else
                    */
                    tn.Nodes.AddRange(loadTreeReflexives(BR, rd.baseOffset + rd.chunkSelected * rd.reflexive.chunkSize, IFPR.items, rd.chunkCount != 0));

                    if (rd.chunkCount != 0 | !hideUnusedReflexives)
                        tns.Add(tn);
                }
                else
                {
                    //tns.Add(tn);
                }
            }
            return tns.ToArray();
        }

        public void refreshTreeListing(TreeNode parent)
        {
            if (((reflexiveData)parent.Tag).chunkCount == 0)
            {
                parent.ForeColor = Color.LightGray;
                refreshTreeSubNodes(parent);
                return;
            }
            parent.ForeColor = parent.Parent.ForeColor;
            
            // If available, re-load data
            loadFile();
            BinaryReader BR = new BinaryReader(memStream);

            this.SuspendLayout();
            if (parent != this.Nodes[0])
            {
                reflexiveData rd = (reflexiveData)parent.Tag;
                if (parent.ForeColor == Color.LightGray)
                {
                    /*
                    BR.BaseStream.Position = ((reflexiveData)parent.Parent.Tag).baseOffset + rd.reflexive.offset;
                    if (((reflexiveData)parent.Parent.Tag).inTagNumber == meta.TagIndex)
                        BR.BaseStream.Position += meta.offset
                    */
                    ((reflexiveData)parent.Tag).chunkCount = 0;
                    return;
                }
                else
                {
                    BR.BaseStream.Position = ((reflexiveData)parent.Parent.Tag).baseOffset + rd.reflexive.offset;
                    rd.chunkCount = BR.ReadInt32();
                    rd.baseOffset = BR.ReadInt32() - meta.magic + rd.chunkSelected * rd.reflexive.chunkSize - meta.offset;
                }
                
                /*
                rd.inTagNumber = map.Functions.ForMeta.FindMetaByOffset(rd.baseOffset);
                if (rd.inTagNumber == meta.TagIndex)
                    rd.baseOffset -= meta.offset;
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
                */
            }
            refreshTreeSubNodes(parent);
            this.ResumeLayout();
        }

        private void refreshTreeSubNodes(TreeNode parent)
        {
            BinaryReader BR = new BinaryReader(memStream);
            foreach (TreeNode tn in parent.Nodes)
            {
                reflexiveData rd = (reflexiveData)tn.Tag;
                /*
                if (rd.inTagNumber != meta.TagIndex)
                {
                    map.OpenMap(MapTypes.Internal);
                    BA = map.BA;
                    BA.Position = ((reflexiveData)parent.Tag).baseOffset + rd.reflexive.offset;
                    if (((reflexiveData)parent.Tag).inTagNumber == meta.TagIndex)
                        BA.Position += meta.offset;
                }
                else
                */
                if (parent.ForeColor == Color.LightGray)
                {
                    rd.chunkCount = 0;
                }
                else
                {
                    BR.BaseStream.Position = ((reflexiveData)parent.Tag).baseOffset + rd.reflexive.offset;
                    rd.chunkCount = BR.ReadInt32();
                    rd.baseOffset = BR.ReadInt32() - meta.magic + rd.chunkSelected * rd.reflexive.chunkSize - meta.offset;
                }

                /*
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
                */
                
                tn.Text = tn.Name + " [" + rd.chunkCount.ToString() + "]";
                refreshTreeListing(tn);
            }
        }

    }
}
