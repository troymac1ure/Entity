using System;
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
    /// Bitmasks are a list of options, accessed through a bit value.
    /// <para>Example:</para>
    /// <para>  bit 0 (1): Visible           bit 4  (16): Deaf</para>
    /// <para>  bit 1 (2): Can Fly         bit 5  (32): Mute</para>
    /// <para>  bit 2 (4): Can't Move   bit 6  (64): Team Killer</para>
    /// <para>  bit 3 (8): Blind              bit 7 (128): No Ragdoll</para>
    /// </summary>
    public partial class Bitmask : BaseField
    {
        #region fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //public int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        private object value;
        private int optionNamePadding=50;
        private bool isNulledOutReflexive = true;
        private bool[] Bits;
        public int bitCount;
        public IFPIO.Option[] Options;
        private bool[] visibleBits;
        #endregion
        #region Constructor
        /// <summary>
        /// The Bitmask class
        /// </summary>
        /// <param name="meta">The controls meta data</param>
        /// <param name="iEntName">The identifying name of the meta string</param>
        /// <param name="map">The metas map file</param>
        /// <param name="iOffsetInChunk">The offset to the string in the memory stream</param>
        /// <param name="iBitCount">8 for Bitmask8, 16 for Bitmask16, 32 for Bitmask32 types</param>
        /// <param name="ioptions">The array of options available</param>
        /// <param name="iLineNumber"></param>
        public Bitmask(Meta meta, string iEntName, Map map, int iOffsetInChunk, int iBitCount, IFPIO.Option[] ioptions, int iLineNumber)
        {
            this.meta = meta;
            this.LineNumber = iLineNumber;
            this.Options = ioptions;
            this.Bits = new bool[iBitCount];
            this.visibleBits = new bool[iBitCount];
            this.bitCount = iBitCount;
            this.size = iBitCount / 8;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.label1.Text = "Bitmask" + iBitCount.ToString();
            this.label1.Left -= iBitCount.ToString().Length * 8;
            this.getLongestName();
            this.MakeControls();
            this.Size = this.PreferredSize;
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.AutoSize = true;
        }

        public override void BaseField_Leave(object sender, EventArgs e)
        {
            System.IO.BinaryWriter BW = new System.IO.BinaryWriter(meta.MS);
            if (map.Functions.ForMeta.FindMetaByOffset(((Bitmask)this).offsetInMap) == meta.TagIndex)
            {
                BW.BaseStream.Position = this.offsetInMap - meta.offset;
            }
            else
            {
                //((WinMetaEditor)this.TopLevelControl).ExternalData.Items.Add();
            }

            try
            {
                uint intToShift = 1;
                uint tempIntToWrite = 0;
                for (int counter = 0; counter < this.bitCount; counter++)
                {
                    if (this.Bits[counter] == true)
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
                            BW.Write(Convert.ToByte(this.value));
                            break;
                        }
                    case 16:
                        {
                            BW.Write(Convert.ToUInt16(this.value));
                            break;
                        }
                    case 32:
                        {
                            BW.Write(Convert.ToUInt32(this.value));
                            break;
                        }
                }
            }
            catch
            {
                MessageBox.Show("Man the hatches! Error in writing bitmask info to map!");
            }
        }

        #endregion
        #region Methods/Functions
        #region Setting up the bitmask controls
        //Cookie for who ever figures out what this method does
        private void getLongestName()
        {
            if (this.Options == null)
                return;
            for (int counter = 0; counter < this.Options.Length; counter++)
            {
                this.checkBox1.Text = this.Options[counter].name;
                if (this.checkBox1.PreferredSize.Width>this.optionNamePadding)
                    this.optionNamePadding = this.checkBox1.PreferredSize.Width+5;
            }
        }
        //Load the the int into bits whose values (1 or 0) are stored in a bool array
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
        //Make the checkboxes, give them names and line them up
        private void MakeControls()
        {
            foreach (Control c in this.Controls[0].Controls) c.Dispose();
            this.Controls[0].Controls.Clear();

            for (int counter = 0; counter < this.bitCount; counter++)
            {
                //Making the checkbox
                CheckBox tempcheckBox1 = new CheckBox();
                tempcheckBox1.AutoSize = true;
                tempcheckBox1.Size = new System.Drawing.Size(80, 17);
                tempcheckBox1.Text = "bit " + counter.ToString();
                tempcheckBox1.Text.PadLeft(this.optionNamePadding);
                tempcheckBox1.UseVisualStyleBackColor = true;
                tempcheckBox1.CheckedChanged += new EventHandler(checkBox1_CheckedChanged);
                tempcheckBox1.Tag = counter.ToString();
                this.visibleBits[counter] = false;
                //Fill in names
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
            //Place the check box in the right spot
            int down = 19;
            int over = 6;
            int CounterForPlacementKeeping = 0;
            for (int counter = 0; counter < this.bitCount; counter++)
            {
                if (this.visibleBits[counter] ==false)
                    if (MetaEditor.MetaEditor.ShowInvisibles == false)
                        continue;                    
                this.Controls[0].Controls[counter].Location = new Point(over, down);
                over += this.optionNamePadding;
                this.Controls[0].Controls[counter].Visible = true;                
                if ((CounterForPlacementKeeping+1) % 3 == 0)
                {
                    down += 23;
                    over = 6;
                }
                CounterForPlacementKeeping++;
            }
        }
        private void BoolsToControls()
        {
            for (int counter = 0; counter < this.bitCount; counter++)
            {
                ((CheckBox)this.groupBox1.Controls[counter]).Checked = this.Bits[counter];
            }
        }
        #endregion
        #region Load and Save
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

            //Decide how big the bitmask is
            switch (this.bitCount)
            {
                case 8:
                    {
                        this.value = BR.ReadByte();
                        break;
                    }
                case 16:
                    {
                        this.value = BR.ReadUInt16();
                        break;
                    }
                case 32:
                    {
                        this.value = BR.ReadUInt32();
                        break;
                    }
            }
            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();
            //convert this.value (an object) into a bool array, then update the checkboxes with that bool array
            BitsToBool();
            BoolsToControls();
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
            map.BW.BaseStream.Position = this.offsetInMap;
            try
            {
                uint intToShift = 1;
                uint tempIntToWrite = 0;
                for (int counter = 0; counter < this.bitCount; counter++)
                {
                    if (this.Bits[counter] == true)
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
            catch
            {
                MessageBox.Show("Man the hatches! Error in writing bitmask info to map!");
            }
            if (openedMap == true)
                map.CloseMap();
        }
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            switch (this.bitCount)
            {
                case 8:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(this.value), 8);
                        break;
                    }
                case 16:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(this.value), 16);
                        break;
                    }
                case 32:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(this.value), 32);
                        break;
                    }
            }
        }
        #endregion
        #region Events
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //make the bool array match the check boxes
            this.Bits[Convert.ToInt32(((CheckBox)sender).Tag)] = ((CheckBox)sender).Checked;
        }
        #endregion
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
        #endregion
        public string Value { get { return this.value.ToString(); } }
    }
}
