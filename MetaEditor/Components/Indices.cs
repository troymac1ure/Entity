// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Indices.cs" company="">
//   
// </copyright>
// <summary>
//   The indices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The indices.
    /// </summary>
    public partial class Indices : BaseField
    {
        // public int LineNumber;
        // private Map map;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        #region Constants and Fields

        /// <summary>
        /// The value.
        /// </summary>
        public int Value;

        /// <summary>
        /// The value type.
        /// </summary>
        public IFPIO.ObjectEnum ValueType;

        /// <summary>
        /// The ent index.
        /// </summary>
        private readonly IFPIO.Index EntIndex;

        /// <summary>
        /// The tempvalue.
        /// </summary>
        private readonly List<string> tempvalue;

        /// <summary>
        /// The item type.
        /// </summary>
        private IFPIO.ObjectEnum ItemType;

        /// <summary>
        /// The indexed reflexive chunk count.
        /// </summary>
        private int indexedReflexiveChunkCount;

        /// <summary>
        /// The indexed reflexive offset.
        /// </summary>
        private int indexedReflexiveOffset;

        /// <summary>
        /// The indexed reflexive translated offset.
        /// </summary>
        private int indexedReflexiveTranslatedOffset;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        /// <summary>
        /// The selection box value string.
        /// </summary>
        private string selectionBoxValueString;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Indices"/> class.
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
        /// <param name="iValueType">
        /// The i value type.
        /// </param>
        /// <param name="iIndex">
        /// The i index.
        /// </param>
        public Indices(string iEntName, Map map, int iOffsetInChunk, IFPIO.ObjectEnum iValueType, IFPIO.Index iIndex)
        {
            this.LineNumber = iIndex.lineNumber;
            this.Dock = DockStyle.Top;
            this.map = map;
            this.chunkOffset = iOffsetInChunk;
            this.EntName = iEntName;
            this.EntIndex = iIndex;
            this.tempvalue = new List<string>(0);

            

            switch (this.EntIndex.ItemType.ToLower())
            {
                case "ident":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Ident; // Entity.MetaEditor.DataValues.ENTType.ident;
                        break;
                    }

                case "stringid":
                    {
                        this.ItemType = IFPIO.ObjectEnum.StringID; // Entity.MetaEditor.DataValues.ENTType.sid;
                        break;
                    }

                case "float":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Float; // Entity.MetaEditor.DataValues.ENTType.Float;
                        break;
                    }

                case "short":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Short; // Entity.MetaEditor.DataValues.ENTType.Int16;
                        break;
                    }

                case "ushort":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UShort; // Entity.MetaEditor.DataValues.ENTType.Uint16;
                        break;
                    }

                case "int":
                    {
                        this.ItemType = IFPIO.ObjectEnum.Int; // Entity.MetaEditor.DataValues.ENTType.Int32;
                        break;
                    }

                case "uint":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UInt; // Entity.MetaEditor.DataValues.ENTType.UInt32;
                        break;
                    }

                case "string32":
                    {
                        this.ItemType = IFPIO.ObjectEnum.String32;

                        // Entity.MetaEditor.DataValues.ENTType.string32;
                        break;
                    }

                case "unicodestring64":
                    {
                        this.ItemType = IFPIO.ObjectEnum.UnicodeString64;

                        // Entity.MetaEditor.DataValues.ENTType.unicodestring64;
                        break;
                    }

                case "string256":
                    {
                        this.ItemType = IFPIO.ObjectEnum.String256;

                        // Entity.MetaEditor.DataValues.ENTType.string256;
                        break;
                    }

                case "unicodestring256":
                    {
                        goto case "string256";
                    }

                default:
                    {
                        this.ItemType = IFPIO.ObjectEnum.Unknown;

                        // Entity.MetaEditor.DataValues.ENTType.nothing;
                        break;
                    }
            }

            

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
            this.Enter += Indices_Enter;
            this.Leave += Indices_Leave;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The poke.
        /// </summary>
        public void Poke()
        {
            if (this.isNulledOutReflexive)
            {
                return;
            }

            string tempstring1 = this.comboBox1.Text;
            if (tempstring1.Contains(" Is Invalid. On Line ") || tempstring1.Contains("Something is wrong with this ") ||
                tempstring1.Contains(" : Value is Too Small To Be An Index") ||
                tempstring1.Contains(" : Value is Too Large To Be The Indexer"))
            {
                return;
            }

            if (tempstring1 == "nulled")
            {
                this.Value = -1;
            }

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
                        RTH_Imports.Poke(Address, (uint)Convert.ToInt16(this.Value), 16);
                        break;
                    }

                case IFPIO.ObjectEnum.Int:
                    {
                        RTH_Imports.Poke(Address, (uint)Convert.ToInt32(this.Value), 32);
                        break;
                    }

                case IFPIO.ObjectEnum.UShort:
                    {
                        RTH_Imports.Poke(Address, Convert.ToUInt16(this.Value), 16);
                        break;
                    }

                case IFPIO.ObjectEnum.UInt:
                    {
                        RTH_Imports.Poke(Address, Convert.ToUInt32(this.Value), 32);
                        break;
                    }

                case IFPIO.ObjectEnum.Float:
                    {
                        RTH_Imports.Poke(Address, RTH_Imports.ConvertFloat(Convert.ToSingle(this.Value)), 32);
                        break;
                    }

                case IFPIO.ObjectEnum.Unknown:
                    {
                        RTH_Imports.Poke(Address, RTH_Imports.ConvertFloat(Convert.ToSingle(this.Value)), 32);
                        break;
                    }

                case IFPIO.ObjectEnum.Byte:
                    {
                        RTH_Imports.Poke(Address, Convert.ToByte(this.Value), 8);
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
        /// <param name="iIndexedReflexiveOffset">
        /// The i indexed reflexive offset.
        /// </param>
        public void Populate(int iOffset, int iIndexedReflexiveOffset)
        {
            this.isNulledOutReflexive = false;
            if (this.EntIndex.reflexiveLayer.ToLower() == "root")
            {
                this.indexedReflexiveOffset = map.SelectedMeta.offset + this.EntIndex.ReflexiveOffset;
            }
            else if (this.EntIndex.reflexiveLayer.ToLower() == "oneup")
            {
                this.indexedReflexiveOffset = iIndexedReflexiveOffset + this.EntIndex.ReflexiveOffset;
            }

            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.offsetInMap = iOffset + this.chunkOffset;
            switch (ValueType)
            {
                case IFPIO.ObjectEnum.Short:
                    {
                        this.Value = map.BR.ReadInt16();
                        break;
                    }

                case IFPIO.ObjectEnum.Int:
                    {
                        this.Value = map.BR.ReadInt32();
                        break;
                    }

                case IFPIO.ObjectEnum.UShort:
                    {
                        this.Value = map.BR.ReadUInt16();
                        break;
                    }

                case IFPIO.ObjectEnum.UInt:
                    {
                        this.Value = (int)map.BR.ReadUInt32();
                        break;
                    }

                case IFPIO.ObjectEnum.Byte:
                    {
                        this.Value = map.BR.ReadByte();
                        break;
                    }
            }

            UpdateSelectionList(false);
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

            string tempstring1 = this.comboBox1.Text;
            if (tempstring1.Contains(" Is Invalid. On Line ") || tempstring1.Contains("Something is wrong with this ") ||
                tempstring1.Contains(" : Value is Too Small To Be An Index") ||
                tempstring1.Contains(" : Value is Too Large To Be The Indexer"))
            {
                return;
            }

            if (tempstring1 == "nulled")
            {
                this.Value = -1;
            }

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
            catch (Exception ex)
            {
                Global.ShowErrorMsg(
                    "Something is wrong with this " + this.ValueType + this.EntName + " Offset " + this.chunkOffset, ex);
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
        /// The get items.
        /// </summary>
        /// <param name="readAll">
        /// The read all.
        /// </param>
        private void GetItems(bool readAll)
        {
            this.tempvalue.Clear();
            if (this.indexedReflexiveChunkCount < 0)
            {
                tempvalue.Add("Index " + this.EntName + " Is Invalid. On Line " + this.EntIndex.lineNumber);
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

            string tempValueString = string.Empty;
            for (int counter = countervalue; counter < maxcount; counter++)
            {
                try
                {
                    map.BR.BaseStream.Position = this.indexedReflexiveTranslatedOffset +
                                                      (this.EntIndex.ReflexiveSize * counter) + this.EntIndex.ItemOffset;
                    switch (this.EntIndex.ItemType.ToLower())
                    {
                        case "short":
                            {
                                tempValueString = counter + " : " + map.BR.ReadInt16();
                                break;
                            }

                        case "int":
                            {
                                tempValueString = counter + " : " + map.BR.ReadInt32();
                                break;
                            }

                        case "ushort":
                            {
                                tempValueString = counter + " : " + map.BR.ReadUInt16();
                                break;
                            }

                        case "uint":
                            {
                                tempValueString = counter + " : " + map.BR.ReadUInt32();
                                break;
                            }

                        case "float":
                            {
                                tempValueString = counter + " : " + map.BR.ReadSingle();
                                break;
                            }

                        case "stringid":
                            {
                                int sidIndexer = map.BR.ReadInt16();
                                if (sidIndexer > -1 && sidIndexer < map.Strings.Name.Length)
                                {
                                    tempValueString = counter + " : " + map.Strings.Name[sidIndexer];
                                }
                                else
                                {
                                    tempValueString = counter + " : Not a String ID";
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
                                        tempValueString = counter + " : " + map.MetaInfo.TagType[tagIndex] + " - " +
                                                          map.FileNames.Name[tagIndex];
                                    }
                                    else
                                    {
                                        tempValueString = counter +
                                                          " : Probably is NOT an ident, choose another data type";
                                    }
                                }
                                else if (tagIndex == -1)
                                {
                                    tempValueString = counter + " : nulled ident";
                                }
                                else
                                {
                                    tempValueString = counter + " : Probably is NOT an ident, choose another data type";
                                }

                                break;
                            }

                        case "string32":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(32);
                                tempValueString = counter + " : " + decode.GetString(tempbytes);
                                break;
                            }

                        case "unicodestring64":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(64);
                                tempValueString = counter + " : " + decode.GetString(tempbytes);
                                break;
                            }

                        case "string256":
                            {
                                Encoding decode = Encoding.UTF8;
                                byte[] tempbytes = map.BR.ReadBytes(256);
                                tempValueString = counter + " : " + decode.GetString(tempbytes);
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
                    tempValueString = counter + " : Error with index on line number " + this.EntIndex.lineNumber;
                }

                tempvalue.Add(tempValueString);
                if (counter == this.Value)
                {
                    this.selectionBoxValueString = tempValueString;
                }
            }

            tempvalue.Add("nulled");
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The indices_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Indices_Enter(object sender, EventArgs e)
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
        /// The indices_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Indices_Leave(object sender, EventArgs e)
        {
            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
        }

        /// <summary>
        /// The read reflexive.
        /// </summary>
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
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The update selection list.
        /// </summary>
        /// <param name="readAll">
        /// The read all.
        /// </param>
        private void UpdateSelectionList(bool readAll)
        {
            this.ReadReflexive();
            this.GetItems(readAll);
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(tempvalue.ToArray());

            if (this.Value >= this.indexedReflexiveChunkCount)
            {
                this.comboBox1.Items.Add(this.Value + " : Value is Too Large To Be The Indexer");
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            }
            else if (this.Value < -1)
            {
                this.comboBox1.Items.Add(this.Value + " : Value is Too Small To Be An Index");
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            }
            else if (this.Value == -1)
            {
                if (readAll == false)
                {
                    this.comboBox1.Text = "nulled";
                }
                else
                {
                    this.comboBox1.SelectedIndex = this.indexedReflexiveChunkCount;
                }
            }
            else
            {
                if (readAll == false)
                {
                    this.comboBox1.Text = this.selectionBoxValueString;
                }
                else
                {
                    this.comboBox1.SelectedIndex = this.Value;
                }
            }
        }

        /// <summary>
        /// The combo box 1_ drop down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            UpdateSelectionList(true);
        }

        #endregion
    }
}