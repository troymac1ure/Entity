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
    public partial class EntStrings : BaseField
    {
        #region Fields
        //public int LineNumber;
        //private int mapIndex;
        //public int chunkOffset;
        //private int offsetInMap;
        //public string EntName = "Error in getting plugin element name";

        private string value = "";
        private int length;
        private bool entUnicode;
        private bool isNulledOutReflexive = true;
        #endregion
        public EntStrings(Meta meta, string iEntName, Map map, int iOffsetInChunk, int ilength, bool itype, int iLineNumber)
        {
            this.meta = meta;
            this.LineNumber = iLineNumber;
            this.entUnicode = itype;
            this.length = ilength;
            this.size = this.length;
            this.map = map;
            this.EntName = iEntName;
            this.chunkOffset = iOffsetInChunk;
            InitializeComponent();
            this.label2.Text = EntName;
            this.Dock = DockStyle.Top;
            this.Size = this.PreferredSize;
            this.AutoSize = false;
            this.Leave += new EventHandler(String_Leave);
        }

        void String_Leave(object sender, EventArgs e)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(meta.MS);
            if (((WinMetaEditor)this.ParentForm).checkSelectionInCurrentTag())
                bw.BaseStream.Position = this.offsetInMap - meta.offset;

            if (this.entUnicode)
            {
                byte[] tempbytes = Encoding.Unicode.GetBytes(this.value); // Encoding.Convert(Encoding.ASCII, Encoding.Unicode, );                
                int bytesToWrite = tempbytes.Length <= this.length ? tempbytes.Length : this.length;

                bw.Write(tempbytes, 0, bytesToWrite);
                tempbytes = new byte[this.length - bytesToWrite];
                for (int i = 0; i < tempbytes.Length; i++)
                    tempbytes[i] = 0;
                bw.Write(tempbytes);
            }
            else
            {
                // Write as an indexed char[] or it adds length bytes to the front of the string
                char[] c = this.value.PadRight(this.length, '\0').ToCharArray();
                bw.Write(c, 0, c.Length);
            }
        }

        public void Populate(int iOffset, bool useMemoryStream)
        {
            this.isNulledOutReflexive = false;
            System.IO.BinaryReader BR = new System.IO.BinaryReader(map.SelectedMeta.MS);
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
                this.offsetInMap += map.SelectedMeta.offset;

            if (this.entUnicode)
            {
                this.value = Encoding.Unicode.GetString(BR.ReadBytes(this.length));
            }
            else
            {
                this.value = new String(BR.ReadChars(this.length));
            }
            this.value = this.value.TrimEnd('\0');
            //this.RemoveNullCharFromValue();
            this.textBox1.Text = this.value;
            this.textBox1.Size = this.textBox1.PreferredSize;
            this.textBox1.Width += 5;

            // ...and then close the file once we are done!
            if (!useMemoryStream)
                map.CloseMap();

        }
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
        public override void Save()
        {
            if (this.isNulledOutReflexive == true)
                return;

            // Why doesn't it allow saving of unicode Strings??
            if (this.entUnicode == true)
                return;

            if (this.value == "e1_mars_entry12")
                this.value = "e1_mars_entry12";
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }            
            map.BW.BaseStream.Position = this.offsetInMap;
            for (int counter = 0; counter < this.length / 4; counter++)
            {
                map.BW.Write((int)0);
            }
            map.BW.BaseStream.Position = this.offsetInMap;
            //if (this.entUnicode == true)
            //{
            //    for (int counter = 0; counter < this.value.Length; counter++)
            //    {
            //        map.BA.Write((char)this.value[counter]);
            //        if (map.BA.Position < (long)this.offsetInMap + 2 * (counter+1))
            //            map.BA.Position++;
            //    }
            //}
            //else
            //{
                for (int counter = 0; counter < this.value.Length; counter++)
                {
                    map.BW.Write((char)this.value[counter]);
                }
            //}            
            if (openedMap == true)
                map.CloseMap();
        }
        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            this.textBox1.Size = this.textBox1.PreferredSize;
            //this.label2.Location = new Point(((TextBox)sender).Location.X + ((TextBox)sender).PreferredSize.Width + 5,6);
            this.value = ((TextBox)sender).Text;
        }
        public void SetFocus(int LineToCheck)
        {
            if (this.LineNumber == LineToCheck)
                this.Focus();
        }
    }
}
