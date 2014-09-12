// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//   
// </copyright>
// <summary>
//   The enums.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Globals;

    using HaloMap.Map;
    using HaloMap.Plugins;
    using HaloMap.RealTimeHalo;

    /// <summary>
    /// The enums.
    /// </summary>
    public partial class Enums : BaseField
    {
        #region Constants and Fields

        /// <summary>
        /// The options.
        /// </summary>
        public IFPIO.Option[] Options;

        /// <summary>
        /// The enum type.
        /// </summary>
        public int enumType;

        // public int LineNumber;
        // private Map map;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";

        /// <summary>
        /// The value.
        /// </summary>
        public int value;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Enums"/> class.
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
        /// <param name="iType">
        /// The i type.
        /// </param>
        /// <param name="ioptions">
        /// The ioptions.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public Enums(string iEntName, Map map, int iOffsetInChunk, int iType, IFPIO.Option[] ioptions, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.Options = ioptions;
            this.enumType = iType;
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
            this.Enter += Enums_Enter;
            this.Leave += Enum_Leave;
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
            this.UpdateValue();
            switch (this.enumType)
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
                        RTH_Imports.Poke(Address, (uint)this.value, 32);
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
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            switch (this.enumType)
            {
                case 8:
                    {
                        this.value = map.BR.ReadByte();
                        break;
                    }

                case 16:
                    {
                        this.value = map.BR.ReadInt16();
                        break;
                    }

                case 32:
                    {
                        this.value = map.BR.ReadInt32();
                        break;
                    }
            }

            this.offsetInMap = iOffset + this.chunkOffset;
            UpdateComboBox();
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
            catch (Exception e)
            {
                Global.ShowErrorMsg(
                    "Something is wrong with this enum " + this.EntName + " Offset " + this.chunkOffset, e);
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
        /// The enum_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Enum_Leave(object sender, EventArgs e)
        {
            if (ReflexiveControl.ME.AutoSave)
            {
                ReflexiveControl.ME.setInfoText("Saving...");
                this.Save();
                ReflexiveControl.ME.restoreInfoText();
            }
        }

        /// <summary>
        /// The enums_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Enums_Enter(object sender, EventArgs e)
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
        /// The update combo box.
        /// </summary>
        private void UpdateComboBox()
        {
            // add all the values from the options list to a temp list<string>.
            List<int> tempint = new List<int>(0);
            if (this.Options != null)
            {
                for (int counter = 0; counter < this.Options.Length; counter++)
                {
                    tempint.Add(this.Options[counter].value);
                }
            }

            // see if the value in the map is in the list, if not, add it.
            if (tempint.IndexOf(this.value) == -1)
            {
                tempint.Add(this.value);
            }

            tempint.TrimExcess();
            tempint.Sort();
            int tempindex = tempint.IndexOf(this.value);

            // go through and put the option name in the strings after the ": "
            List<string> tempstring = new List<string>(0);
            for (int counter = 0; counter < tempint.Count; counter++)
            {
                tempstring.Add(tempint[counter].ToString());
            }

            if (this.Options != null)
            {
                for (int counter = 0; counter < this.Options.Length; counter++)
                {
                    if (tempint.IndexOf(this.Options[counter].value) != -1)
                    {
                        tempstring[tempint.IndexOf(this.Options[counter].value)] += ": " + this.Options[counter].name;
                    }
                }
            }

            this.comboBox1.Items.Clear();
            this.comboBox1.Items.AddRange(tempstring.ToArray());
            this.comboBox1.SelectedIndex = tempindex;
        }

        /// <summary>
        /// The update value.
        /// </summary>
        private void UpdateValue()
        {
            // Since I mixed the name with the value, we have to take out the names.
            string tempstring = this.comboBox1.Text;
            for (int counter = 0; counter < tempstring.Length; counter++)
            {
                if (tempstring[counter] == ':')
                {
                    tempstring = tempstring.Substring(0, counter);
                    break;
                }
            }

            // if some n00b (read xbox7887) decided to type something stupid into the enum box, here we just test to see if it'll work.
            // if not, the value stays the same as what was read in
            try
            {
                this.value = Convert.ToInt32(tempstring);
            }
            catch
            {
            }
        }

        #endregion
    }
}