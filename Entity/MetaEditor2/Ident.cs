using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HaloMap.Map;
using HaloMap.Meta;
using HaloMap.Plugins;
using HaloMap.RealTimeHalo;

namespace entity.MetaEditor2
{
    /// <summary>
    /// Tag Types / Idents are pointers to other meta within the map
    /// </summary>
    public partial class Ident : BaseField
    {
        #region Fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //public int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        public int identInt32;
        public string tagType;
        public int tagIndex;
        public string tagName;
        public bool HasTagType { get { return doesHaveTagType; } }
        private bool doesHaveTagType;
        private bool AddEvents = true;
        private int tagComboBoxIndexer = 0;
        private bool isNulledOutReflexive = true;
        private int longestName = -1;
        #endregion
        /// <summary>
        /// The (Tag Type &) Ident Class
        /// </summary>
        /// <param name="meta">The controls meta data</param>
        /// <param name="iEntName">The identifying name of the meta string</param>
        /// <param name="map">The metas map file</param>
        /// <param name="iOffsetInChunk">The offset to the string in the memory stream</param>
        /// <param name="idoesHaveTagType">States whether there is a preceding Tag Type or just an Ident</param>
        /// <param name="iLineNumber">The associated line number</param>
        public Ident(Meta meta, string iEntName, Map map, int iOffsetInChunk, bool idoesHaveTagType, int iLineNumber)
        {
            InitializeComponent();
            this.meta = meta;
            this.LineNumber = iLineNumber; 
            this.doesHaveTagType = idoesHaveTagType;

            // Even though !iDoesHaveTagType does not save a Tag Type, it is still loaded through the Ident
            if (!idoesHaveTagType)
                this.label1.Text = "Ident";
            this.size = 4 + (this.doesHaveTagType ? 4 : 0);
            this.map = map;
            this.EntName = iEntName;
            // Offset - 4 to account for Tag Type if applicable
            this.chunkOffset = iOffsetInChunk - (idoesHaveTagType ? 4 : 0);
            this.label4.Text = EntName;
            this.Dock = DockStyle.Top;
            //this.Size = this.PreferredSize;
            this.AutoSize = false;
        }

        public override void BaseField_Leave(object sender, EventArgs e)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(meta.MS);
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
                bw.BaseStream.Position = this.offsetInMap - meta.offset;
            this.tagType = this.cbTagType.Text;
            this.tagName = this.cbTagIdent.Text;
            if (this.tagType == "")
                this.tagType = "null";
            this.tagIndex = map.Functions.ForMeta.FindByNameAndTagType(this.tagType, this.tagName);
            if (this.tagIndex != -1)
                this.identInt32 = map.MetaInfo.Ident[this.tagIndex];
            else
                this.identInt32 = -1;
            if (this.doesHaveTagType == true)
            {
                if (this.tagType != "null")
                {
                    List<char> tempList = new List<char>(0);
                    tempList.AddRange(this.tagType.ToCharArray(0, 4));
                    tempList.TrimExcess();
                    tempList.Reverse();
                    char[] tempchar = tempList.ToArray();
                    bw.Write(tempchar);
                }
                else
                {
                    bw.Write((int)-1);
                }
            }
            bw.Write(this.identInt32);
        }

        private void FillTagBoxWithNames(bool changeTagType)
        {
            object temp = this.cbTagIdent.SelectedItem;
            this.tagType = this.cbTagType.Text;
            this.cbTagIdent.Items.Clear();
            this.cbTagIdent.Sorted = true;
            longestName = -1;
            if (this.cbTagType.Text != "null")
            {
                for (int counter = 0; counter < map.FileNames.Name.Length; counter++)
                    if (this.tagType == map.MetaInfo.TagType[counter])
                    {
                        this.cbTagIdent.Items.Add(map.FileNames.Name[counter]);
                        Size textSize = TextRenderer.MeasureText(map.FileNames.Name[counter], cbTagIdent.Font);
                        if (longestName == -1 || textSize.Width > longestName)
                            longestName = textSize.Width;
                    }
            }
            this.cbTagIdent.Sorted = false;
            this.cbTagIdent.Items.Add("null");
            try
            {
                this.cbTagIdent.SelectedItem = temp;
            }
            catch
            { 
                this.cbTagIdent.SelectedIndex = 0;
                this.tagName = (string)this.cbTagIdent.Items[0];
            }
        }

        public void Populate(int iOffset, bool useMemoryStream) 
        {
            this.isNulledOutReflexive = false;
            System.IO.BinaryReader BR = new System.IO.BinaryReader(meta.MS);
            //set offsets
            BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.offsetInMap = iOffset + this.chunkOffset;
            // If we need to read / save tag info directly to file...
            if (!useMemoryStream)
            {
                map.OpenMap(MapTypes.Internal);
                BR = map.BR;
                BR.BaseStream.Position = this.offsetInMap;
            }
            else
                this.offsetInMap += meta.offset;

            if (doesHaveTagType)
            {
                int nullCheck = BR.ReadInt32();
                if (nullCheck != -1)
                {
                    BR.BaseStream.Position -= 4;
                    char[] temptype = BR.ReadChars(4);
                    Array.Reverse(temptype);
                    this.tagType = new string(temptype);
                }
                else
                    this.tagType = "null";
            }
            else
                this.tagType = "null";


            int tempint = BR.ReadInt32();
            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();
            this.tagIndex = map.Functions.ForMeta.FindMetaByID(tempint);
            if (this.tagIndex != -1)
            {
                this.tagType = map.MetaInfo.TagType[this.tagIndex];
                this.tagName = map.FileNames.Name[this.tagIndex];
            }
            else
            {
                this.tagName = "null";
            }
            this.identInt32 = tempint;

            this.UpdateIdentBoxes();
        }
        
        private void UpdateIdentBoxes()
        {
            if (this.cbTagType.Items.Count <= 1)
            {
                this.cbTagType.Items.Clear();
                this.cbTagType.Items.Add(this.tagType);
                this.cbTagType.SelectedIndex = 0;
            }
            else
            {
                this.cbTagType.Text = this.tagType;
            }

            if (this.cbTagIdent.Items.Count <= 1)
            {
                this.cbTagIdent.Items.Clear();
                this.cbTagIdent.Items.Add(this.tagName);
                this.cbTagIdent.SelectedIndex = 0;
            }
            else
            {
                this.cbTagIdent.Text = this.tagName;
            }
        }
        
        public override void Save()
        {
            if (this.isNulledOutReflexive == true)
                return;
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }
            this.identInt32 = map.Functions.ForMeta.FindByNameAndTagType(this.tagType, this.tagName);

            map.BW.BaseStream.Position = this.offsetInMap;
            if (this.doesHaveTagType == true)
            {
                if (this.tagType != "null")
                {
                    List<char> tempList = new List<char>(0);
                    tempList.AddRange(this.tagType.ToCharArray(0, 4));
                    tempList.TrimExcess();
                    tempList.Reverse();
                    char[] tempchar = tempList.ToArray();
                    map.BW.Write(tempchar);
                }
            }
            map.BW.Write(this.identInt32);
            if (openedMap == true)
                map.CloseMap();
        }
        
        public void Poke()
        {
            // We only poke the ident, not the type
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic + (doesHaveTagType ? 4 : 0));
            this.identInt32 = map.Functions.ForMeta.FindByNameAndTagType(this.tagType, this.tagName);
            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)this.identInt32, 32);
        }
        
        #region ComboBox Events

        private void cbTagType_DropDown(object sender, EventArgs e)
        {
            object temp = cbTagType.SelectedItem;
            if (cbTagType.Items.Count <= 1)
            {
                cbTagType.Items.Clear();
                IEnumerator i = map.MetaInfo.TagTypes.Keys.GetEnumerator();
                while (i.MoveNext())
                {
                    cbTagType.Items.Add((string)i.Current);
                }
                cbTagType.Sorted = true;
                cbTagType.Sorted = false;
                cbTagType.Items.Add((string)"null");
                cbTagType.SelectedItem = temp;
            }
        }

        private void cbTagType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.cbTagIdent.MouseClick -= this.cbTagIdent_MouseClick;
            if (this.cbTagType.Focused)
                this.FillTagBoxWithNames(true);
        }


        private void cbTagIdent_DropDown(object sender, EventArgs e)
        {
            if (cbTagIdent.Items.Count <= 1)
                this.FillTagBoxWithNames(false);
            cbTagIdent.Width = longestName + 20;
        }

        private void cbTagIdent_DropDownClose(object sender, EventArgs e)
        {
            if (cbTagIdent.SelectedItem == null)
                return;
            if (cbTagIdent.SelectedItem.ToString() == "null")
            {
                this.cbTagType.Text = "null";
            }
            int width = TextRenderer.MeasureText(cbTagIdent.SelectedItem.ToString(), cbTagIdent.Font).Width + 20;
            if (width > (label1.Left - cbTagIdent.Left - 20))
                width = (label1.Left - cbTagIdent.Left - 20);
            cbTagIdent.Width = width;
            cbTagIdent.SelectionStart = cbTagIdent.Text.Length - 1;
            cbTagIdent.SelectionLength = 0;
        }

        private void cbTagIdent_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void cbTagIdent_TextChanged(object sender, EventArgs e)
        {
            if (!cbTagIdent.DroppedDown && cbTagIdent.Focused)
                cbTagIdent_DropDownClose(sender, e);
        }

        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
        #endregion

    }

}
