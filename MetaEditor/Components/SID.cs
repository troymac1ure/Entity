// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SID.cs" company="">
//   
// </copyright>
// <summary>
//   The sid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using global::MetaEditor.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The sid.
    /// </summary>
    public partial class SID : BaseField
    {
        // public int LineNumber;
        // private Map mapindex;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        #region Constants and Fields

        /// <summary>
        /// The sid indexer.
        /// </summary>
        public int sidIndexer;

        /// <summary>
        /// The w m_ paint.
        /// </summary>
        private const short WM_PAINT = 0x00f;

        /// <summary>
        /// The sid indexer list.
        /// </summary>
        private readonly List<int> sidIndexerList = new List<int>(0);

        /// <summary>
        /// The add events.
        /// </summary>
        private bool AddEvents = true;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SID"/> class.
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
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public SID(string iEntName, Map map, int iOffsetInChunk, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.Size = this.PreferredSize;
            this.Dock = DockStyle.Top;
            this.label3.Text = EntName;
            this.AutoSize = false;
            this.Enter += SID_Enter;
            this.Leave += SID_Leave;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The poke.
        /// </summary>
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            try
            {
                uint StringID =
                    (uint)(((ushort)this.sidIndexer) | ((byte)map.Strings.Length[this.sidIndexer] << 24));
                RTH_Imports.Poke(Address, StringID, 32);
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg(
                    "Net: Something is wrong with this Sid " + this.EntName + " Offset " + this.chunkOffset, ex);
            }
        }

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
            this.sidIndexer = map.BR.ReadInt16();
            byte tempnull = map.BR.ReadByte();
            byte sidLength = map.BR.ReadByte();
            try
            {
                string s = map.Strings.Name[this.sidIndexer];
                if (s.Length == sidLength)
                {
                    this.comboBox1.Text = s;
                }
                else
                {
                    this.comboBox1.Text = string.Empty;
                }
            }
            catch
            {
                this.comboBox1.Text = "error reading sid";
            }

            this.offsetInMap = iOffset + this.chunkOffset;
            if (openedMap)
            {
                map.CloseMap();
            }

            if (AddEvents)
            {
                this.comboBox1.TextChanged += this.comboBox1_TextChanged;
                AddEvents = false;
            }
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

            try
            {
                map.BW.BaseStream.Position = this.offsetInMap;
                map.BW.Write((short)this.sidIndexer);
                map.BW.BaseStream.Position += 1;
                map.BW.Write((byte)map.Strings.Length[this.sidIndexer]);
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg(
                    "Something is wrong with this Sid " + this.EntName + " Offset " + this.chunkOffset, ex);
            }

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
        /// The sid loader_ drop down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SIDLoader_DropDown(object sender, EventArgs e)
        {
            string tempSidString = ((ComboBox)sender).Text;
            SID sid = (SID)((Control)sender).Parent;

            Control ME = this.Parent;
            while (!(ME is MetaEditor) && !(ME == null))
            {
                ME = ME.Parent;
            }

            if (ME != null)
            {
                // Always use the same string swapper, then we only have to load it once!
                MEStringsSelector sSwap = ((MetaEditor)ME).stringSwap;

                if (sSwap == null)
                {
                    sSwap = new MEStringsSelector(map.Strings.Name);
                }

                sSwap.SelectedID = sid.sidIndexer;
                sSwap.ShowDialog();

                // this.Enabled = true;
                if (sid.sidIndexer != sSwap.SelectedID)
                {
                    sid.sidIndexer = sSwap.SelectedID;
                    ((ComboBox)sender).SelectedIndex = -1;
                    ((ComboBox)sender).Text = map.Strings.Name[sSwap.SelectedID];
                }
            }
            else
            {
                ((ComboBox)sender).Items.Clear();
                this.sidIndexerList.Clear();
                ((ComboBox)sender).Items.Add(string.Empty);
                this.sidIndexerList.Add(0);
                for (int counter = 0; counter < map.Strings.Name.Length; counter++)
                {
                    if (map.Strings.Name[counter].Contains(tempSidString))
                    {
                        ((ComboBox)sender).Items.Add(map.Strings.Name[counter]);
                        string xe = map.Strings.Name[counter];
                        this.sidIndexerList.Add(counter);
                        if (counter == sidIndexer)
                        {
                            ((ComboBox)sender).SelectedIndex = this.sidIndexerList.Count - 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The sid loader_ drop down close.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SIDLoader_DropDownClose(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex != -1)
            {
                try
                {
                    sidIndexer = sidIndexerList[((ComboBox)sender).SelectedIndex];
                }
                catch
                {
                    sidIndexer = 0;
                }
            }
        }

        /// <summary>
        /// The si d_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SID_Enter(object sender, EventArgs e)
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
        /// The si d_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SID_Leave(object sender, EventArgs e)
        {
            // Check for typed value
            SID sid = (SID)sender;
            if (sid.comboBox1.Text != map.Strings.Name[sid.sidIndexer])
            {
                for (int i = 0; i < map.Strings.Name.Length; i++)
                {
                    if (map.Strings.Name[i].ToLower() == sid.comboBox1.Text.ToLower())
                    {
                        sid.sidIndexer = i;
                        break;
                    }
                }

                sid.comboBox1.Text = map.Strings.Name[sid.sidIndexer];
            }

            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
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
        }

        #endregion
    }
}