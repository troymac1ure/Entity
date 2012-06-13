// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ident.cs" company="">
//   
// </copyright>
// <summary>
//   The ident.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The ident.
    /// </summary>
    public partial class Ident : BaseField
    {
        // public int LineNumber;
        // private Map map;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        #region Constants and Fields

        /// <summary>
        /// The ident int 32.
        /// </summary>
        public int identInt32;

        /// <summary>
        /// The tag index.
        /// </summary>
        public int tagIndex;

        /// <summary>
        /// The tag name.
        /// </summary>
        public string tagName;

        /// <summary>
        /// The tag type.
        /// </summary>
        public string tagType;

        /// <summary>
        /// The does have tag type.
        /// </summary>
        private readonly bool doesHaveTagType;

        /// <summary>
        /// The add events.
        /// </summary>
        private bool AddEvents = true;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        /// <summary>
        /// The tag combo box indexer.
        /// </summary>
        private int tagComboBoxIndexer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ident"/> class.
        /// </summary>
        /// <param name="iEntName">
        /// The i ent name.
        /// </param>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="iOffsetInChunk">
        /// The i offset in chunk.
        /// </param>
        /// <param name="idoesHaveTagType">
        /// The idoes have tag type.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public Ident(string iEntName, Map map, int iOffsetInChunk, bool idoesHaveTagType, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.doesHaveTagType = idoesHaveTagType;
            this.map = map;
            this.EntName = iEntName;
            this.chunkOffset = iOffsetInChunk;
            InitializeComponent();
            this.Controls[0].Text = EntName;
            this.Dock = DockStyle.Top;
            this.Size = this.PreferredSize;
            this.AutoSize = false;
            this.Enter += Ident_Enter;
            this.Leave += Ident_Leave;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The poke.
        /// </summary>
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            this.FindByNameAndTagType();
            RTH_Imports.Poke(Address, (uint)this.identInt32, 32);
        }

        // 32086704
        /// <summary>
        /// The populate.
        /// </summary>
        /// <param name="iOffset">
        /// The i offset.
        /// </param>
        public void Populate(int iOffset)
        {
            this.isNulledOutReflexive = false;
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.offsetInMap = iOffset + this.chunkOffset;

            int tempint = map.BR.ReadInt32();
            this.tagIndex = map.Functions.ForMeta.FindMetaByID(tempint);
            if (this.tagIndex != -1)
            {
                this.tagName = map.FileNames.Name[this.tagIndex];
                this.tagType = map.MetaInfo.TagType[this.tagIndex];
            }
            else
            {
                this.tagName = "null";
                this.tagType = "null";
            }

            this.identInt32 = tempint;
            if (this.offsetInMap == 0)
            {
            }

            if (openedMap)
            {
                map.CloseMap();
            }

            this.comboBox1.Size = this.comboBox1.PreferredSize;
            if (AddEvents)
            {
                AddEvents = false;
                this.comboBox1.MouseClick += this.comboBox1_MouseClick;
                this.comboBox1.DropDownClosed += this.TagName_DropDownClose;
                this.comboBox2.SelectedIndexChanged += this.TagType_SelectedIndexChanged;
                this.comboBox2.DropDown += this.TagType_DropDown;
                this.comboBox1.TextChanged += this.comboBox1_TextChanged;
            }

            this.UpdateIdentBoxes();
        }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            if (this.isNulledOutReflexive)
            {
                return;
            }

            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            this.FindByNameAndTagType();
            if (this.doesHaveTagType)
            {
                map.BW.BaseStream.Position = this.offsetInMap - 4;
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

            map.BW.BaseStream.Position = this.offsetInMap;
            map.BW.Write(this.identInt32);
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The set focus.
        /// </summary>
        /// <param name="LineToCheck">
        /// The line to check.
        /// </param>
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
            {
                this.Focus();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The fill tag box with names.
        /// </summary>
        /// <param name="changeTagType">
        /// The change tag type.
        /// </param>
        private void FillTagBoxWithNames(bool changeTagType)
        {
            this.tagType = this.comboBox2.Text;
            this.comboBox1.Items.Clear();
            this.comboBox1.Sorted = true;
            if (this.Controls[2].Text != "null")
            {
                for (int counter = 0; counter < map.FileNames.Name.Length; counter++)
                {
                    if (this.tagType == map.MetaInfo.TagType[counter])
                    {
                        this.comboBox1.Items.Add(map.FileNames.Name[counter]);
                    }
                }
            }

            this.comboBox1.Sorted = false;
            this.comboBox1.Items.Add("null");
            if (changeTagType)
            {
                this.comboBox1.SelectedIndex = 0;
                this.tagComboBoxIndexer = 0;
                this.tagName = (string)this.comboBox1.Items[0];
            }
            else
            {
                this.tagComboBoxIndexer = this.comboBox1.Items.IndexOf(tagName);
            }
        }

        /// <summary>
        /// The find by name and tag type.
        /// </summary>
        private void FindByNameAndTagType()
        {
            this.tagName = this.comboBox1.Text;
            this.tagType = this.comboBox2.Text;
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if (this.tagType == map.MetaInfo.TagType[x] && this.tagName == map.FileNames.Name[x])
                {
                    this.identInt32 = map.MetaInfo.Ident[x];
                    return;
                }
            }

            this.identInt32 = -1;
        }

        /// <summary>
        /// The ident_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Ident_Enter(object sender, EventArgs e)
        {
            if (this.Parent.Controls.Owner is ReflexiveControl)
            {
                ReflexiveControl rc = (ReflexiveControl)this.Parent.Controls.Owner;
                Control pc = this.Parent.Controls[this.Parent.Controls.Count - 1];
                int nameNum = pc.Controls.Count - 1;
                while (nameNum >= 0 && !(pc.Controls[nameNum] is Label))
                {
                    nameNum--;
                }

                if (nameNum < 0)
                {
                    return; // safety
                }

                ReflexiveControl.ME.setReflexiveText(pc.Controls[nameNum].Text, rc.selectedChunk, rc.chunkCount - 1);
            }
            else
            {
                ReflexiveControl.ME.setReflexiveText("Main", 0, 0);
            }
        }

        /// <summary>
        /// The ident_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Ident_Leave(object sender, EventArgs e)
        {
            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
        }

        /// <summary>
        /// The tag name_ drop down close.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TagName_DropDownClose(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString() == "null")
            {
                this.comboBox2.Text = "null";
            }
        }

        /// <summary>
        /// The tag type_ drop down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TagType_DropDown(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();
            IEnumerator i = map.MetaInfo.TagTypes.Keys.GetEnumerator();
            while (i.MoveNext())
            {
                ((ComboBox)sender).Items.Add(i.Current);
            }

            ((ComboBox)sender).Sorted = true;
            ((ComboBox)sender).Sorted = false;
            ((ComboBox)sender).Items.Add("null");
        }

        /// <summary>
        /// The tag type_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TagType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBox1.MouseClick -= this.comboBox1_MouseClick;
            this.FillTagBoxWithNames(true);
        }

        /// <summary>
        /// The update ident boxes.
        /// </summary>
        private void UpdateIdentBoxes()
        {
            this.comboBox1.Text = this.tagName;
            this.comboBox2.Text = this.tagType;
        }

        /// <summary>
        /// The combo box 1_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            this.FillTagBoxWithNames(false);
            ((ComboBox)sender).MouseClick -= this.comboBox1_MouseClick;
        }

        /// <summary>
        /// The combo box 1_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ((ComboBox)sender).Width = (((ComboBox)sender).Text.Length * 5) + 50;
        }

        #endregion
    }
}