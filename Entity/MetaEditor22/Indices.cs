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

namespace entity.MetaEditor2
{
    public partial class Indices : BaseField
    {        
        #region Fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //public int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        public int Value;
        private IFPIO.Index EntIndex;
        private List<string> tempvalue;
        private string selectionBoxValueString;
        private int indexedReflexiveChunkCount;
        private int indexedReflexiveTranslatedOffset;
        private IFPIO.ObjectEnum ItemType;
        public IFPIO.ObjectEnum ValueType;
        private int indexedReflexiveOffset;
        private bool isNulledOutReflexive = true;
        #endregion
        public Indices(Meta meta, string iEntName, Map map, int iOffsetInChunk, IFPIO.ObjectEnum iValueType, IFPIO.Index iIndex)
        {
            this.meta = meta;
            this.LineNumber = iIndex.lineNumber;
            this.Dock = DockStyle.Top;
            this.map = map;
            switch (iValueType)
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
            }
            this.chunkOffset = iOffsetInChunk;
            this.EntName = iEntName;
            this.EntIndex = iIndex;
            this.tempvalue = new List<string>(0);
            #region Set Block Indice Reference Type
            switch (this.EntIndex.ItemType.ToLower())
            {
                case "ident":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Ident; // entity.MetaEditor.DataValues.ENTType.ident;
                        break;
                    }
                case "stringid":
                    {
                        this.ItemType = IFPIO.ObjectEnum.StringID; // entity.MetaEditor.DataValues.ENTType.sid;
                        break;
                    }
                case "float":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Float; // entity.MetaEditor.DataValues.ENTType.Float;
                        break;
                    }
                case "short":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Short; // entity.MetaEditor.DataValues.ENTType.Int16;
                        break;
                    }
                case "ushort":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UShort; // entity.MetaEditor.DataValues.ENTType.Uint16;
                        break;
                    }
                case "int":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Int; // entity.MetaEditor.DataValues.ENTType.Int32;
                        break;
                    }
                case "uint":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UInt; // entity.MetaEditor.DataValues.ENTType.UInt32;
                        break;
                    }
                case "string32":
                    {
                        this.ItemType = IFPIO.ObjectEnum.String32; // entity.MetaEditor.DataValues.ENTType.string32;
                        break;
                    }
                case "unicodestring64":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UnicodeString64; // entity.MetaEditor.DataValues.ENTType.unicodestring64;
                        break;
                    }
                case "string256":
                    {
                        this.ItemType = IFPIO.ObjectEnum.String256; // entity.MetaEditor.DataValues.ENTType.string256;
                        break;
                    }
                case "unicodestring256":
                    {
                        goto case "string256";
                    }
                default:
                    {
                        this.ItemType = IFPIO.ObjectEnum.Unknown; // entity.MetaEditor.DataValues.ENTType.nothing;
                        break;
                    }                
            }
            #endregion
            InitializeComponent();
            switch (iValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        this.label1.Text = "Int16 Block Index";
                        this.ValueType = IFPIO.ObjectEnum.Short;
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        this.label1.Text = "Int32 Block Index";
                        this.ValueType = IFPIO.ObjectEnum.Int;
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        this.label1.Text = "Uint16 Block Index";
                        this.ValueType = IFPIO.ObjectEnum.UShort;
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        this.label1.Text = "Uint32 Block Index";
                        this.ValueType = IFPIO.ObjectEnum.UInt;
                        break;
                    }
                case IFPIO.ObjectEnum.Byte:
                    {
                        this.label1.Text = "Byte Block Index";
                        this.ValueType = IFPIO.ObjectEnum.Byte;
                        break;
                    }
            }
            this.label2.Text = this.EntName;
        }

        public override void BaseField_Leave(object sender, EventArgs e)
        {
            System.IO.BinaryWriter BW = new System.IO.BinaryWriter(map.SelectedMeta.MS);
            if (map.Functions.ForMeta.FindMetaByOffset(((Indices)this).offsetInMap) == map.SelectedMeta.TagIndex)
            {
                BW.BaseStream.Position = this.offsetInMap - map.SelectedMeta.offset;
            }
            else
            {
                //((WinMetaEditor)this.TopLevelControl).ExternalData.Items.Add();
            }

            string tempstring1 = this.comboBox1.Text;
            if (tempstring1.Contains(" Is Invalid. On Line ") || tempstring1.Contains("Something is wrong with this ") || tempstring1.Contains(" : Value is Too Small To Be An Index") || tempstring1.Contains(" : Value is Too Large To Be The Indexer"))
                return;
            if (tempstring1 == "nulled")
                this.Value = -1;
            if (tempstring1.Contains(" : "))
            {
                int counter;
                for (counter = 0; counter < tempstring1.Length; counter++)
                {
                    if (tempstring1[counter] == ' ')
                    {
                        break;
                    }
                }
                this.Value = Convert.ToInt32(tempstring1.Substring(0, counter));
            }
            try
            {
                switch (ValueType)
                {
                    case IFPIO.ObjectEnum.Short:
                        {
                            BW.Write(Convert.ToInt16(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Int:
                        {
                            BW.Write(Convert.ToInt32(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.UShort:
                        {
                            BW.Write(Convert.ToUInt16(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.UInt:
                        {
                            BW.Write(Convert.ToUInt32(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Float:
                        {
                            BW.Write(Convert.ToSingle(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Unknown:
                        {
                            BW.Write(Convert.ToSingle(this.Value));
                            break;
                        }
                    case IFPIO.ObjectEnum.Byte:
                        {
                            BW.Write(Convert.ToByte(this.Value));
                            break;
                        }
                }
            }
            catch
            {
                MessageBox.Show("Something is wrong with this " + this.ValueType.ToString() + this.EntName + " Offset " + this.chunkOffset.ToString());
            }
        }

        public void Populate(int iOffset, int iIndexedReflexiveOffset)
        {
            this.isNulledOutReflexive = false;
            System.IO.BinaryReader BR = new System.IO.BinaryReader(meta.MS);

            if (this.EntIndex.reflexiveLayer.ToLower() == "root")
                this.indexedReflexiveOffset = map.SelectedMeta.offset + this.EntIndex.ReflexiveOffset;
            else if (this.EntIndex.reflexiveLayer.ToLower() == "oneup")
                this.indexedReflexiveOffset = iIndexedReflexiveOffset + this.EntIndex.ReflexiveOffset;
            
            /*
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }
            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            */
            BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.offsetInMap = map.SelectedMeta.offset + iOffset + this.chunkOffset;

            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        this.Value = (int)BR.ReadInt16();
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        this.Value = BR.ReadInt32();
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        this.Value = (int)BR.ReadUInt16();
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        this.Value = (int)BR.ReadUInt32();
                        break;
                    }
                case IFPIO.ObjectEnum.Byte:
                    {
                        this.Value = (int)BR.ReadByte();
                        break;
                    }
            }
            UpdateSelectionList(false);
            /*
            if (openedMap == true)
                map.CloseMap();
            */
        }
        private void UpdateSelectionList(bool readAll)
        {
            this.ReadReflexive();
            this.GetItems(readAll);
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(tempvalue.ToArray());

            if (this.Value >= this.indexedReflexiveChunkCount)
            {
                this.comboBox1.Items.Add(this.Value.ToString() + " : Value is Too Large To Be The Indexer");
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            }
            else if (this.Value < -1)
            {
                this.comboBox1.Items.Add(this.Value.ToString() + " : Value is Too Small To Be An Index");
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            }
            else if (this.Value == -1)
            {
                if (readAll == false)
                    this.comboBox1.Text = "nulled";
                else
                    this.comboBox1.SelectedIndex = this.indexedReflexiveChunkCount;
            }
            else
            {
                if (readAll == false)
                    this.comboBox1.Text = this.selectionBoxValueString;
                else
                    this.comboBox1.SelectedIndex = this.Value;
            }
        }
        private void ReadReflexive()
        {
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }
            map.BR.BaseStream.Position = this.indexedReflexiveOffset;
            this.indexedReflexiveChunkCount = map.BR.ReadInt32();
            this.indexedReflexiveTranslatedOffset = map.BR.ReadInt32() - map.SelectedMeta.magic;
            if (openedMap == true)
                map.CloseMap();
        }
        private void GetItems(bool readAll)
        {
            this.tempvalue.Clear();
            if (this.indexedReflexiveChunkCount < 0)
            {
                tempvalue.Add("Index " + this.EntName + " Is Invalid. On Line " + this.EntIndex.lineNumber.ToString());
                return;
            }
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }            
            int countervalue = 0;
            int maxcount = this.indexedReflexiveChunkCount;
            if (readAll == false && this.Value > -1)
            {
                countervalue = this.Value;
                maxcount = this.Value + 1;
            }
            else if (readAll == false)
            {
                countervalue = 0;
                maxcount = 0;
            }
            string tempValueString = "";
            for (int counter = countervalue; counter < maxcount; counter++)
            {
                try
                {
                    map.BR.BaseStream.Position = this.indexedReflexiveTranslatedOffset + (this.EntIndex.ReflexiveSize * counter) + this.EntIndex.ItemOffset;
                    switch (this.EntIndex.ItemType.ToLower())
                    {
                        case "short":
                            {
                                tempValueString = counter.ToString() + " : " + map.BR.ReadInt16().ToString();
                                break;
                            }
                        case "int":
                            {
                                tempValueString = counter.ToString() + " : " + map.BR.ReadInt32().ToString();
                                break;
                            }
                        case "ushort":
                            {
                                tempValueString = counter.ToString() + " : " + map.BR.ReadUInt16().ToString();
                                break;
                            }
                        case "uint":
                            {
                                tempValueString = counter.ToString() + " : " + map.BR.ReadUInt32().ToString();
                                break;
                            }
                        case "float":
                            {
                                tempValueString = counter.ToString() + " : " + map.BR.ReadSingle().ToString();
                                break;
                            }
                        case "stringid":
                            {
                                int sidIndexer = map.BR.ReadInt16();
                                if (sidIndexer > -1 && sidIndexer < map.Strings.Name.Length)
                                {
                                    tempValueString = counter.ToString() + " : " + map.Strings.Name[sidIndexer];
                                }
                                else
                                {
                                    tempValueString = counter.ToString() + " : Not a String ID";
                                }
                                break;
                            }
                        case "ident":
                            {                                
                                int tempint = map.BR.ReadInt32();                                
                                int tagIndex = map.Functions.ForMeta.FindMetaByID(tempint);
                                if (tagIndex != -1)
                                {
                                    if (tempint != -1)
                                    {
                                        tempValueString = counter.ToString() + " : " + map.MetaInfo.TagType[tagIndex] + " - " + map.FileNames.Name[tagIndex];
                                    }
                                    else
                                    {
                                        tempValueString = counter.ToString() + " : Probably is NOT an ident, choose another data type";
                                    }
                                }
                                else if(tagIndex == -1)
                                {
                                    tempValueString = counter.ToString() + " : nulled ident";
                                }
                                else
                                {
                                    tempValueString = counter.ToString() + " : Probably is NOT an ident, choose another data type";
                                }
                                break;
                            }
                        case "string32":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(32);
                                tempValueString = counter.ToString() + " : " + decode.GetString(tempbytes);
                                break;
                            }
                         case "unicodestring64":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(64);
                                tempValueString = counter.ToString() + " : " + decode.GetString(tempbytes);
                                break;
                            }
                        case "string256":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(256);
                                tempValueString = counter.ToString() + " : " + decode.GetString(tempbytes);
                                break;
                            }
                        case "unicodestring256":
                            {
                                goto case "string256";
                            }

                    }
                }
                catch
                {
                    tempValueString = counter.ToString() + " : Error with index on line number " + this.EntIndex.lineNumber.ToString();
                }
                tempvalue.Add(tempValueString);
                if (counter == this.Value)
                    this.selectionBoxValueString = tempValueString;
            }
            tempvalue.Add("nulled");
            if (openedMap == true)
                map.CloseMap();
        }
        public override void Save()
        {
            if (this.isNulledOutReflexive == true)
                return;
            string tempstring1 = this.comboBox1.Text;
            if (tempstring1.Contains(" Is Invalid. On Line ") || tempstring1.Contains("Something is wrong with this ") || tempstring1.Contains(" : Value is Too Small To Be An Index") || tempstring1.Contains(" : Value is Too Large To Be The Indexer"))
                return;
            if (tempstring1 == "nulled")
                this.Value = -1;
            if (tempstring1.Contains(" : "))
            {
                int counter;
                for (counter = 0; counter < tempstring1.Length; counter++)
                {
                    if (tempstring1[counter] == ' ')
                    {
                        break;
                    }
                }
                this.Value = Convert.ToInt32(tempstring1.Substring(0, counter));
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
                MessageBox.Show("Something is wrong with this " + this.ValueType.ToString() + this.EntName + " Offset " + this.chunkOffset.ToString());
            }
            if (openedMap == true)
                map.CloseMap();
        }
        public void Poke()
        {
            if (this.isNulledOutReflexive == true)
                return;
            string tempstring1 = this.comboBox1.Text;
            if (tempstring1.Contains(" Is Invalid. On Line ") || tempstring1.Contains("Something is wrong with this ") || tempstring1.Contains(" : Value is Too Small To Be An Index") || tempstring1.Contains(" : Value is Too Large To Be The Indexer"))
                return;
            if (tempstring1 == "nulled")
                this.Value = -1;
            if (tempstring1.Contains(" : "))
            {
                int counter;
                for (counter = 0; counter < tempstring1.Length; counter++)
                {
                    if (tempstring1[counter] == ' ')
                    {
                        break;
                    }
                }
                this.Value = Convert.ToInt32(tempstring1.Substring(0, counter));
            }
            uint Address = (uint)(this.offsetInMap + map.SelectedMeta.magic);
            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt16(this.Value), 16);
                        break;
                    }
                case IFPIO.ObjectEnum.Int:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToInt32(this.Value), 32);
                        break;
                    }
                case IFPIO.ObjectEnum.UShort:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt16(this.Value), 16);
                        break;
                    }
                case IFPIO.ObjectEnum.UInt:
                    {
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToUInt32(this.Value), 32);
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
                        HaloMap.RealTimeHalo.RTH_Imports.Poke(Address, (uint)Convert.ToByte(this.Value), 8);
                        break;
                    }
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            //if (tempvalue.Count != ((ComboBox)sender).Items.Count)
            UpdateSelectionList(true);
            this.comboBox1.SelectedIndex = this.Value;
        }
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
    }
}
