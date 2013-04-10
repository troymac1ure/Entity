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
    public partial class Enums : BaseField
    {
        #region Fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //public int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        public int value;
        public int enumType;
        private bool isNulledOutReflexive = true;
        public object[] Options;
        #endregion
        #region Constructor
        public Enums(Meta meta, string iEntName, Map map, int iOffsetInChunk, int iType, object[] ioptions, int iLineNumber)
        {
            this.meta = meta;
            this.LineNumber = iLineNumber;
            this.Options = ioptions;
            this.enumType = iType;
            this.size = iType / 8;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.Size = this.PreferredSize;
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            switch (iType)
            {
                case 8:
                    {
                        this.Controls[2].Text = "Enum8";
                        break;
                    }
                case 16:
                    {
                        this.Controls[2].Text = "Enum16";
                        break;
                    }
                case 32:
                    {
                        this.Controls[2].Text = "Enum32";
                        break;
                    }
            }
            this.AutoSize = false;
        }
        #endregion

        #region Methods
        public override void BaseField_Leave(object sender, EventArgs e)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(meta.MS);
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
                bw.BaseStream.Position = this.offsetInMap - meta.offset;
            try
            {
                this.value = int.Parse(this.comboBox1.Text);
            }
            catch
            {
                this.value = this.comboBox1.SelectedIndex;
            }
            switch (this.enumType)
            {
                case 8:
                    {
                        bw.Write(Convert.ToByte(this.value));
                        break;
                    }
                case 16:
                    {
                        bw.Write(Convert.ToInt16(this.value));
                        break;
                    }
                case 32:
                    {
                        bw.Write(this.value);
                        break;
                    }
            }

        }
        #endregion

        #region Load and Save
        private void UpdateComboBox()
        {
            //add all the values from the options list to a temp list<string>.
            List<int> tempint = new List<int>(0);
            if (this.Options != null)
            {
                for (int counter = 0; counter < this.Options.Length; counter++)
                {
                    tempint.Add(((IFPIO.Option)this.Options[counter]).value);
                }
            }
            //see if the value in the map is in the list, if not, add it.
            if (tempint.IndexOf(this.value) == -1)
                tempint.Add(this.value);
            tempint.TrimExcess();
            tempint.Sort();
            int tempindex = tempint.IndexOf(this.value);
            //go through and put the option name in the strings after the ": "
            List<string> tempstring = new List<string>(0);
            for (int counter = 0; counter < tempint.Count; counter++)
            {
                tempstring.Add(tempint[counter].ToString());
            }
            if (this.Options != null)
            {
                for (int counter = 0; counter < this.Options.Length; counter++)
                {
                    if (tempint.IndexOf(((IFPIO.Option)this.Options[counter]).value) != -1)
                        tempstring[tempint.IndexOf(((IFPIO.Option)this.Options[counter]).value)] += ": " + ((IFPIO.Option)this.Options[counter]).name.ToString();
                }
            }
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(tempstring.ToArray());
            this.comboBox1.SelectedIndex = tempindex;
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

            switch (this.enumType)
            {
                case 8:
                    {
                        this.value = (int)BR.ReadByte();
                        break;
                    }
                case 16:
                    {
                        this.value = (int)BR.ReadInt16();
                        break;
                    }
                case 32:
                    {
                        this.value = BR.ReadInt32();
                        break;
                    }

            }
            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();
            UpdateComboBox();
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
            this.UpdateValue();
            map.BW.BaseStream.Position = this.offsetInMap;
            try
            {
                switch (this.enumType)
                {
                    case 8:
                        {
                            map.BW.Write(Convert.ToByte(this.value));
                            break;
                        }
                    case 16:
                        {
                            map.BW.Write(Convert.ToInt16(this.value));
                            break;
                        }
                    case 32:
                        {
                            map.BW.Write(this.value);
                            break;
                        }
                }
            }
            catch
            {
                MessageBox.Show("Something is wrong with this enum " + this.EntName + " Offset " + this.chunkOffset.ToString());
            }
            if (openedMap == true)
                map.CloseMap();
        }
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            this.UpdateValue();
            switch (this.enumType)
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
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)this.value, 32);
                        break;
                    }
            }
        }
        private void UpdateValue()
        {
            //Since I mixed the name with the value, we have to take out the names.
            string tempstring = this.comboBox1.Text;
            for (int counter = 0; counter < tempstring.Length; counter++)
            {
                if (tempstring[counter] == ':')
                {
                    tempstring = tempstring.Substring(0, counter);
                    break;
                }
            }
            //if some n00b (read xbox7887) decided to type something stupid into the enum box, here we just test to see if it'll work.
            //if not, the value stays the same as what was read in
            try
            {
                this.value = Convert.ToInt32(tempstring);
            }
            catch
            {
            }
        }
        #endregion
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
        public string Value { get { return this.value.ToString(); } }
    }
}
