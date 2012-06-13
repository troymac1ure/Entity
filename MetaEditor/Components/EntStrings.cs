// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntStrings.cs" company="">
//   
// </copyright>
// <summary>
//   The ent strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The ent strings.
    /// </summary>
    public partial class EntStrings : BaseField
    {
        #region Constants and Fields

        /// <summary>
        /// The ent unicode.
        /// </summary>
        private readonly bool entUnicode;

        /// <summary>
        /// The length.
        /// </summary>
        private readonly int length;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        // public int LineNumber;
        // private Map map;
        // public int chunkOffset;
        // private int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        /// <summary>
        /// The value.
        /// </summary>
        private string value = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntStrings"/> class.
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
        /// <param name="ilength">
        /// The ilength.
        /// </param>
        /// <param name="itype">
        /// The itype.
        /// </param>
        /// <param name="iLineNumber">
        /// The i line number.
        /// </param>
        public EntStrings(string iEntName, Map map, int iOffsetInChunk, int ilength, bool itype, int iLineNumber)
        {
            this.LineNumber = iLineNumber;
            this.entUnicode = itype;
            this.length = ilength;
            this.map = map;
            this.EntName = iEntName;
            this.chunkOffset = iOffsetInChunk;
            InitializeComponent();
            this.Controls[1].Text = EntName;
            this.Dock = DockStyle.Top;
            this.Size = this.PreferredSize;
            this.AutoSize = false;
            this.Enter += EntStrings_Enter;
            this.Leave += String_Leave;
        }

        #endregion

        #region Public Methods

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
            Encoding decode = Encoding.Unicode;
            byte[] tempbytes = map.BR.ReadBytes(this.length);
            this.value = decode.GetString(tempbytes);
            this.RemoveNullCharFromValue();
            this.Controls[0].Text = this.value;
            this.Controls[0].Size = this.Controls[0].PreferredSize;
            this.Controls[0].Width += 5;
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

            // Why doesn't it allow saving of unicode Strings??
            if (this.entUnicode)
            {
                return;
            }

            if (this.value == "e1_mars_entry12")
            {
                this.value = "e1_mars_entry12";
            }

            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BW.BaseStream.Position = this.offsetInMap;
            for (int counter = 0; counter < this.length / 4; counter++)
            {
                map.BW.Write(0);
            }

            map.BW.BaseStream.Position = this.offsetInMap;

            // if (this.entUnicode == true)
            // {
            // for (int counter = 0; counter < this.value.Length; counter++)
            // {
            // map.BW.Write((char)this.value[counter]);
            // if (map.BW.BaseStream.Position < (long)this.offsetInMap + 2 * (counter+1))
            // map.BW.BaseStream.Position++;
            // }
            // }
            // else
            // {
            for (int counter = 0; counter < this.value.Length; counter++)
            {
                map.BW.Write(this.value[counter]);
            }

            // }
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
        /// The ent strings_ enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void EntStrings_Enter(object sender, EventArgs e)
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
        /// The remove null char from value.
        /// </summary>
        private void RemoveNullCharFromValue()
        {
            List<char> tempchar = new List<char>(0);
            tempchar.AddRange(this.value.ToCharArray());
            for (int counter = 0; counter < tempchar.Count; counter++)
            {
                if (tempchar[counter] == '\0')
                {
                    tempchar.RemoveAt(counter);
                    counter--;
                }
            }

            this.value = new string(tempchar.ToArray());
        }

        /// <summary>
        /// The string_ leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void String_Leave(object sender, EventArgs e)
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
            this.Controls[0].Size = this.Controls[0].PreferredSize;
            this.Controls[1].Location =
                new Point(((TextBox)sender).Location.X + ((TextBox)sender).PreferredSize.Width + 5, 6);
            this.value = ((TextBox)sender).Text;
        }

        #endregion
    }
}