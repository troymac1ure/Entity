// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bitmask.cs" company="">
//   
// </copyright>
// <summary>
//   The bitmask.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The bitmask.
    /// </summary>
    public partial class Bitmask : BaseField
    {
        #region Constants and Fields

        /// <summary>
        /// The options.
        /// </summary>
        public IFPIO.Option[] Options;

        /// <summary>
        /// The bit count.
        /// </summary>
        public int bitCount;

        /// <summary>
        /// The bits.
        /// </summary>
        private readonly bool[] Bits;

        /// <summary>
        /// The visible bits.
        /// </summary>
        private readonly bool[] visibleBits;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        /// <summary>
        /// The option name padding.
        /// </summary>
        private int optionNamePadding = 50;

        // public int LineNumber;
        // private Map map;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        /// <summary>
        /// The value.
        /// </summary>
        private object value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEntName"></param>
        /// <param name="map"></param>
        /// <param name="iOffsetInChunk"></param>
        /// <param name="iBitCount"></param>
        /// <param name="ioptions"></param>
        /// <param name="iLineNumber"></param>
        public Bitmask(string iEntName, Map map, int iOffsetInChunk, int iBitCount, IFPIO.Option[] ioptions, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.Options = ioptions;
            this.Bits = new bool[iBitCount];
            this.visibleBits = new bool[iBitCount];
            this.bitCount = iBitCount;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.label1.Text = "Bitmask" + iBitCount;
            this.label1.Left -= iBitCount.ToString().Length * 8;
            this.getLongestName();
            this.MakeControls();
            this.Size = this.PreferredSize;
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.AutoSize = true;
            this.Enter += Bitmask_Enter;
            this.Leave += Bitmask_Leave;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Value.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value.ToString();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The poke.
        /// </summary>
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            switch (this.bitCount)
            {
                case 8:
                    {
                        RTH_Imports.Poke(Address, Convert.ToByte(this.value), 8);
                        break;
                    }

                case 16:
                    {
                        RTH_Imports.Poke(Address, (uint)Convert.ToInt16(this.value), 16);
                        break;
                    }

                case 32:
                    {
                        RTH_Imports.Poke(Address, (uint)Convert.ToInt32(this.value), 32);
                        break;
                    }
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

            // Open up map
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            // set offsets
            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.offsetInMap = iOffset + this.chunkOffset;

            // Decide how big the bitmask is
            switch (this.bitCount)
            {
                case 8:
                    {
                        this.value = map.BR.ReadByte();
                        break;
                    }

                case 16:
                    {
                        this.value = map.BR.ReadUInt16();
                        break;
                    }

                case 32:
                    {
                        this.value = map.BR.ReadUInt32();
                        break;
                    }
            }

            // convert this.value (an object) into a bool array, then update the checkboxes with that bool array
            BitsToBool();
            BoolsToControls();
            if (openedMap)
            {
                map.CloseMap();
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

            map.BW.BaseStream.Position = this.offsetInMap;
            try
            {
                uint intToShift = 1;
                uint tempIntToWrite = 0;
                for (int counter = 0; counter < this.bitCount; counter++)
                {
                    if (this.Bits[counter])
                    {
                        tempIntToWrite += intToShift;
                    }

                    intToShift <<= 1;
                }

                this.value = tempIntToWrite;
                switch (this.bitCount)
                {
                    case 8:
                        {
                            map.BW.Write(Convert.ToByte(this.value));
                            break;
                        }

                    case 16:
                        {
                            map.BW.Write(Convert.ToUInt16(this.value));
                            break;
                        }

                    case 32:
                        {
                            map.BW.Write(Convert.ToUInt32(this.value));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Man the hatches! Error in writing bitmask info to map!", ex);
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
        /// The bitmask_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Bitmask_Enter(object sender, EventArgs e)
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
        /// The bitmask_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Bitmask_Leave(object sender, EventArgs e)
        {
            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
        }

        // Load the the int into bits whose values (1 or 0) are stored in a bool array
        /// <summary>
        /// The bits to bool.
        /// </summary>
        private void BitsToBool()
        {
            uint tempANDInt = 1;
            uint tempvalue = Convert.ToUInt32(this.value);
            for (int counter = 0; counter < this.bitCount; counter++)
            {
                this.Bits[counter] = (tempANDInt & tempvalue) == tempANDInt ? true : false;
                tempANDInt <<= 1;
            }
        }

        /// <summary>
        /// The bools to controls.
        /// </summary>
        private void BoolsToControls()
        {
            for (int counter = 0; counter < this.bitCount; counter++)
            {
                ((CheckBox)this.Controls[0].Controls[counter]).Checked = this.Bits[counter];
            }
        }

        /// <summary>
        /// The make controls.
        /// </summary>
        private void MakeControls()
        {
            foreach (Control c in this.Controls[0].Controls)
            {
                c.Dispose();
            }

            this.Controls[0].Controls.Clear();

            for (int counter = 0; counter < this.bitCount; counter++)
            {
                // Making the checkbox
                CheckBox tempcheckBox1 = new CheckBox();
                tempcheckBox1.AutoSize = true;
                tempcheckBox1.Size = new Size(80, 17);
                tempcheckBox1.Text = "bit " + counter;
                tempcheckBox1.Text.PadLeft(this.optionNamePadding);
                tempcheckBox1.UseVisualStyleBackColor = true;
                tempcheckBox1.CheckedChanged += checkBox1_CheckedChanged;
                tempcheckBox1.Tag = counter.ToString();
                this.visibleBits[counter] = false;

                // Fill in names
                if (this.Options != null)
                {
                    for (int counter2 = 0; counter2 < this.Options.Length; counter2++)
                    {
                        if (this.Options[counter2].value == counter)
                        {
                            this.visibleBits[counter] = true;
                            tempcheckBox1.Text = this.Options[counter2].name;

                            break;
                        }
                    }
                }

                tempcheckBox1.Visible = false;
                this.Controls[0].Controls.Add(tempcheckBox1);
            }

            // Place the check box in the right spot
            int down = 19;
            int over = 6;
            int CounterForPlacementKeeping = 0;
            for (int counter = 0; counter < this.bitCount; counter++)
            {
                if (this.visibleBits[counter] == false)
                {
                    if (MetaEditor.ShowInvisibles == false)
                    {
                        continue;
                    }
                }

                this.Controls[0].Controls[counter].Location = new Point(over, down);
                over += this.optionNamePadding;
                this.Controls[0].Controls[counter].Visible = true;
                if ((CounterForPlacementKeeping + 1) % 3 == 0)
                {
                    down += 23;
                    over = 6;
                }

                CounterForPlacementKeeping++;
            }
        }

        /// <summary>
        /// The check box 1_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // make the bool array match the check boxes
            this.Bits[Convert.ToInt32(((CheckBox)sender).Tag)] = ((CheckBox)sender).Checked;
        }

        // Cookie for who ever figures out what this method does
        /// <summary>
        /// The get longest name.
        /// </summary>
        private void getLongestName()
        {
            if (this.Options == null)
            {
                return;
            }

            for (int counter = 0; counter < this.Options.Length; counter++)
            {
                this.checkBox1.Text = this.Options[counter].name;
                if (this.checkBox1.PreferredSize.Width > this.optionNamePadding)
                {
                    this.optionNamePadding = this.checkBox1.PreferredSize.Width + 5;
                }
            }
        }

        #endregion

        // Make the checkboxes, give them names and line them up
    }
}