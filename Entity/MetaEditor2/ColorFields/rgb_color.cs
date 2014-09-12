using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HaloMap.Meta;
using HaloMap.Map;
using HaloMap.Plugins;

namespace entity.MetaEditor2
{
    public partial class argb_color : BaseField
    {
        #region Fields
        ColorWheel CW;
        DataValues Red;
        DataValues Green;
        DataValues Blue;
        DataValues Alpha;
        public IFPIO.ObjectEnum ValueType;
        bool hasAlpha;
        private byte eachSize;
        private bool isNulledOutReflexive = true;
        #endregion

        /// <summary>
        /// Creates a color wheel with associated (A)RGB text boxes
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="iEntName">Control display name</param>
        /// <param name="map"></param>
        /// <param name="iOffsetInChunk"></param>
        /// <param name="hasAlphaChannel">True if ARGB, False for RGB</param>
        /// <param name="valueType">Float for 0f-1f, Int for 0-255</param>
        /// <param name="iLineNumber"></param>
        public argb_color(Meta meta, string iEntName, Map map, int iOffsetInChunk, bool hasAlphaChannel, IFPIO.ObjectEnum valueType,int iLineNumber)
        {
            InitializeComponent();

            this.meta = meta;
            this.LineNumber = iLineNumber;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = " " + iEntName + " ";
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.hasAlpha = hasAlphaChannel;
            this.ValueType = valueType;
            switch (valueType)
            {
                case IFPIO.ObjectEnum.Byte:
                    this.eachSize = 1;
                    break;
                case IFPIO.ObjectEnum.Short:
                case IFPIO.ObjectEnum.UShort:
                    this.eachSize = 2;
                    break;
                case IFPIO.ObjectEnum.Int:
                case IFPIO.ObjectEnum.UInt:
                case IFPIO.ObjectEnum.Float:
                case IFPIO.ObjectEnum.Unknown:
                    this.eachSize = 4;
                    break;
            }
            this.size = (hasAlpha ? eachSize * 4 : eachSize * 3);
            
            CW = new ColorWheel();
            CW.Location = new Point(180, 5);

            Red = new DataValues(meta, "Red", map, iOffsetInChunk, valueType, iLineNumber);
            Red.BackColor = System.Drawing.Color.DarkRed;
            CW.setTextBox(Red.textBox1, Color.Red);

            Green = new DataValues(meta, "Green", map, iOffsetInChunk + eachSize, valueType, iLineNumber);
            Green.BackColor = System.Drawing.Color.FromArgb(10, 80, 35);
            CW.setTextBox(Green.textBox1, Color.Green);

            Blue = new DataValues(meta, "Blue", map, iOffsetInChunk + eachSize * 2, valueType, iLineNumber);
            Blue.BackColor = System.Drawing.Color.DarkBlue;
            CW.setTextBox(Blue.textBox1, Color.Blue);

            if (hasAlpha)
            {
                Alpha = new DataValues(meta, "Alpha", map, iOffsetInChunk + eachSize * 3, valueType, iLineNumber);
                Alpha.BackColor = System.Drawing.Color.Gray;
                CW.setTextBox(Alpha.textBox1, Color.White);
                gbRGBColor.Controls.Add(Alpha);
            }

            gbRGBColor.Controls.Add(CW);
            CW.BringToFront();
            gbRGBColor.Controls.Add(Blue);
            gbRGBColor.Controls.Add(Green);
            gbRGBColor.Controls.Add(Red);
        }

        private void writeTo(System.IO.BinaryWriter bw)
        {
            switch (this.ValueType)
            {
                case IFPIO.ObjectEnum.Byte:
                    bw.Write(byte.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(byte.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(byte.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(byte.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.Int:
                    bw.Write(int.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(int.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(int.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(int.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.Short:
                    bw.Write(short.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(short.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(short.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(short.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.UInt:
                    bw.Write(uint.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(uint.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(uint.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(uint.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.UShort:
                    bw.Write(ushort.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(ushort.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(ushort.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(ushort.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.Float:
                case IFPIO.ObjectEnum.Unknown:
                    bw.Write(float.Parse(this.Red.textBox1.Text.ToString()));
                    bw.Write(float.Parse(this.Blue.textBox1.Text.ToString()));
                    bw.Write(float.Parse(this.Green.textBox1.Text.ToString()));
                    if (hasAlpha)
                        bw.Write(float.Parse(this.Alpha.textBox1.Text.ToString()));
                    break;
                case IFPIO.ObjectEnum.Unused:
                    return;
                default:
                    throw new NotImplementedException("Unknown Type While Saving: " + this.ValueType);
            }
        }

        public override void BaseField_Leave(object sender, EventArgs e)
        {
            if (this.ParentForm == null)
                return;
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(meta.MS);
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
                bw.BaseStream.Position = this.offsetInMap - meta.offset;

            Red.Value = Red.textBox1.Text;
            Green.Value = Green.textBox1.Text;
            Blue.Value = Blue.textBox1.Text;

            writeTo(bw);
        }

        public void Populate(int iOffset, bool useMemoryStream)
        {
            this.isNulledOutReflexive = false;
            
            Red.Populate(iOffset, useMemoryStream);
            Green.Populate(iOffset, useMemoryStream);
            Blue.Populate(iOffset, useMemoryStream);
            if (hasAlpha)
                Alpha.Populate(iOffset, useMemoryStream);

            this.offsetInMap = Red.offsetInMap;

            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();

            CW.Refresh();
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
            try
            {
                map.BW.BaseStream.Position = this.offsetInMap;                
                writeTo(map.BW);
            }
            catch
            {
                MessageBox.Show("Something is wrong with this " + this.ValueType.ToString() + this.EntName + " Offset " + this.chunkOffset.ToString());
            }
            if (openedMap == true)
                map.CloseMap();
        }
        
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);

            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(Red.Value), 16);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(Blue.Value), 16);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(Green.Value), 16);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(Alpha.Value), 16);
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(Red.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(Blue.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(Green.Value), 32);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(Alpha.Value), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(Red.Value), 16);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(Blue.Value), 16);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(Green.Value), 16);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(Alpha.Value), 16);
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(Red.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(Blue.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(Green.Value), 32);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(Alpha.Value), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.Float:
                case IFPIO.ObjectEnum.Unknown:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat((float)Red.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat((float)Blue.Value), 32);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat((float)Green.Value), 32);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat((float)Alpha.Value), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.Byte:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(Red.Value), 8);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(Blue.Value), 8);
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(Green.Value), 8);
                        if (hasAlpha)
                            HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(Alpha.Value), 8);
                        break;
                    }
            }
        }

        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
    }
}
