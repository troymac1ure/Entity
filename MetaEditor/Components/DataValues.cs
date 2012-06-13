// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValues.cs" company="">
//   
// </copyright>
// <summary>
//   The data values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The data values.
    /// </summary>
    public partial class DataValues : BaseField
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
        public object Value;

        /// <summary>
        /// The value type.
        /// </summary>
        public IFPIO.ObjectEnum ValueType;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValues"/> class.
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
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public DataValues(string iEntName, Map map, int iOffsetInChunk, IFPIO.ObjectEnum type, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.Controls[2].Text = type.ToString();
            this.ValueType = type;
            this.AutoSize = false;
            this.Enter += DataValues_GotFocus;
            this.Leave += DataValues_LostFocus;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The poke.
        /// </summary>
        public void Poke()
        {
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
                        uint val = Convert.ToUInt32(Convert.ToSingle(this.Value));
                        RTH_Imports.Poke(Address, val, 32);
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

        // Updates values from file to ensure latest info
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
                        this.Value = map.BR.ReadUInt32();
                        break;
                    }

                case IFPIO.ObjectEnum.Float:
                    {
                        this.Value = map.BR.ReadSingle();
                        break;
                    }

                case IFPIO.ObjectEnum.Unknown:
                    {
                        this.Value = map.BR.ReadSingle();
                        break;
                    }

                case IFPIO.ObjectEnum.Byte:
                    {
                        this.Value = map.BR.ReadByte();
                        break;
                    }
            }

            if (this.ValueType == IFPIO.ObjectEnum.Unused)
            {
                this.Controls[1].Text = "unused";
            }
            else
            {
                this.Controls[1].Text = this.Value.ToString();
            }

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
        /// The data values_ got focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataValues_GotFocus(object sender, EventArgs e)
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
        /// The data values_ lost focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataValues_LostFocus(object sender, EventArgs e)
        {
            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
        }

        /// <summary>
        /// The text box 1_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Value = ((TextBox)sender).Text;
        }

        #endregion
    }
}