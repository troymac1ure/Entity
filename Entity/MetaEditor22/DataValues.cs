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
    public partial class DataValues : BaseField
    {
        #region Fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //public int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        public object Value;
        public IFPIO.ObjectEnum ValueType;
        private bool isNulledOutReflexive = true;
        #endregion
        public DataValues(Meta meta, string iEntName, Map map, int iOffsetInChunk, IFPIO.ObjectEnum type, int iLineNumber)
        {
            this.meta = meta;
            this.LineNumber = iLineNumber;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.Controls[2].Text = type.ToString();
            this.ValueType = type;
            switch (this.ValueType)
            {
                case IFPIO.ObjectEnum.Byte:
                    this.size = 1;
                    break;
                case IFPIO.ObjectEnum.Short:
                case IFPIO.ObjectEnum.UShort:
                    this.size = 2;
                    break;
                case IFPIO.ObjectEnum.Int:
                case IFPIO.ObjectEnum.UInt:
                case IFPIO.ObjectEnum.Float:
                case IFPIO.ObjectEnum.Unknown:
                    this.size = 4;
                    break;
                case IFPIO.ObjectEnum.Unused:
                    this.size = 0;
                    break;
            }
            this.AutoSize = false;
        }

        public override void BaseField_Leave(object sender, EventArgs e)
        {
            if (this.ParentForm == null)
                return;
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(meta.MS);
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
                bw.BaseStream.Position = this.offsetInMap - meta.offset;
            /*
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
            else
            {
                map.OpenMap(MapTypes.Internal);
                bw = map.BW;
                map.SelectedMeta.MS.Position = this.offsetInMap;
                // ...
                map.CloseMap();
            }
            */
            switch (this.ValueType)
            {
                case IFPIO.ObjectEnum.Byte:
                    if (int.Parse(this.Value.ToString()) > byte.MaxValue) this.Value = byte.MaxValue;
                    bw.Write(byte.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.Int:
                    bw.Write(int.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.Short:
                    if (int.Parse(this.Value.ToString()) > short.MaxValue) this.Value = short.MaxValue;
                    bw.Write(short.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.UInt:
                    bw.Write(uint.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.UShort:
                    if (int.Parse(this.Value.ToString()) > ushort.MaxValue) this.Value = ushort.MaxValue;
                    bw.Write(ushort.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.Float:
                case IFPIO.ObjectEnum.Unknown:
                    bw.Write(float.Parse(this.Value.ToString()));
                    break;
                case IFPIO.ObjectEnum.Unused:
                    return;
                default:
                    throw new NotImplementedException("Unknown Type While Saving: " + this.ValueType);
            }

            this.Controls[1].Text = this.Value.ToString();
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
            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        this.Value = BR.ReadInt16();
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        this.Value = BR.ReadInt32();
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        this.Value = BR.ReadUInt16();
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        this.Value = BR.ReadUInt32();
                        break;
                    }
                case IFPIO.ObjectEnum.Float:
                    {
                        this.Value = BR.ReadSingle();
                        break;
                    }
                case IFPIO.ObjectEnum.Unknown:
                    {
                        this.Value = BR.ReadSingle();
                        break;
                    }
                case IFPIO.ObjectEnum.Byte:
                    {
                        this.Value = BR.ReadByte();
                        break;
                    }
            }
            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();
            if (this.ValueType != IFPIO.ObjectEnum.Unused)
                this.Controls[1].Text = this.Value.ToString();
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
                switch (ValueType)
                {
                    case IFPIO.ObjectEnum.Short:
                        {
                            map.BW.Write(Convert.ToInt16(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Int:
                        {
                            map.BW.Write(Convert.ToInt32(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.UShort:
                        {
                            map.BW.Write(Convert.ToUInt16(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.UInt:
                        {
                            map.BW.Write(Convert.ToUInt32(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Float:
                        {
                            map.BW.Write(Convert.ToSingle(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Unknown:
                        {
                            map.BW.Write(Convert.ToSingle(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte:
                        {
                            map.BW.Write(Convert.ToByte(this.Value));
                            break;
                        }
                }
            }
            catch
            {
                MessageBox.Show("Something is wrong with this " +this.ValueType.ToString()+ this.EntName + " Offset " + this.chunkOffset.ToString());
            }
            if (openedMap == true)
                map.CloseMap();
        }
        public void Poke()
        {
            uint Address = (uint)(this.offsetInMap+map.SelectedMeta.magic);
            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(this.Value), 16);
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(this.Value),32);
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(this.Value),16);
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(this.Value),32);
                        break;
                    }
                case IFPIO.ObjectEnum.Float:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat(Convert.ToSingle(this.Value)), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.Unknown:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)HaloMap.RealTimeHalo.RTH_Imports.ConvertFloat(Convert.ToSingle(this.Value)), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.Byte:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(this.Value),8);
                        break;
                    }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused)
                this.Value = textBox1.Text;
        }
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
    }
}
