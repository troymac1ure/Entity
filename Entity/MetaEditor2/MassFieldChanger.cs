using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HaloMap.Map;

namespace entity.MetaEditor2
{
    public partial class MassFieldChanger : Form
    {
        // The original control from the meta editor
        private BaseField FieldControl;
        // The memory stream from the meta
        private MemoryStream MS;
        // A way to read from the stream
        private BinaryReader BR;
        // A way to write to the stream
        private BinaryWriter BW;
        // The original parent reflexive data from the meta editor
        private reflexiveData RD;
        // The starting address of the first reflexive chunk field
        private int StartOffset;

        // Structure used for String IDs to allow name sorting
        private class SIDData
        {
            public int Index { get; set; }
            public string Name { get; set; }

            static public SIDData[] CreateArray(string[] names)
            {
                SIDData[] SIDs = new SIDData[names.Length];
                for (int x = 0; x < names.Length; x++)
                {
                    SIDs[x] = new SIDData();
                    SIDs[x].Index = x;
                    SIDs[x].Name = names[x];
                }
                return SIDs;
            }
        }

        /// <summary>
        /// Creates a clone of the passed memory stream and multiple controls of the
        /// passed in control type. Returns DialogResult.None if no changes have been written to the memory stream.
        /// Returns DialogResult.Yes if changes have been written to the original memory stream.
        /// </summary>
        /// <param name="control">Any child of type BaseField</param>
        /// <param name="rd">The associated reflexiveData information</param>
        public MassFieldChanger(BaseField control, reflexiveData rd)        
        {
            /* Create "loading" form to show while loading 
            Form LoadingForm = new Form();
            this.Tag = LoadingForm;
            */

            InitializeComponent();

            this.FieldControl = control;

            // Temporarily use our BinaryReader to create a Clone of the original memory stream            
            this.BR = new BinaryReader(control.meta.MS);
            this.BR.BaseStream.Position = 0; 
            this.MS = new MemoryStream(this.BR.ReadBytes((int)this.BR.BaseStream.Length));
            // Set the Reader/Writer to our cloned memory stream
            this.BR = new BinaryReader(MS);
            this.BW = new BinaryWriter(MS);
            this.RD = rd;

            this.Text = ": " + FieldControl.EntName + " (" + FieldControl.Controls[FieldControl.Controls.Count-1].Text + ")";
            reflexiveData RDParent = rd;
            while (RDParent.parent != null)
            {
                string s = "\\" + RDParent.reflexive.name;
                if (RDParent != rd)
                    s += "[" + RDParent.chunkSelected + "]";
                this.Text = s + this.Text;
                RDParent = RDParent.parent;
            }

            #region Fill in Start & End combo boxes ans select starting values
            for (int x = 0; x < RD.chunkCount; x++)
            {
                cbStartChunk.Items.Add(x.ToString());
                cbEndChunk.Items.Add(x.ToString());
            }
            // Start at chunk 0
            cbStartChunk.Text = "0";
            // Only load the first 100 chunks by default for starters
            cbEndChunk.Text = RD.chunkCount > 100 ? "99" : (RD.chunkCount-1).ToString();
            #endregion

            // States that no values have changed
            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// Occurs after the form has loaded and is shown for the first time.
        /// Used to load the controls and populate the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MassFieldChanger_Shown(object sender, EventArgs e)
        {
            GenerateFields(0, 50);
            PopulateSelectedChunks(50, 100);
            progressBar1.Value = 0;
        }

        /// <summary>
        /// Creates one control of the selected type for each chunk in the reflexive.
        /// Idents will have the tag types filled in & tag names will be auto filled when
        /// the tag type is selected.
        /// String IDs will have the strings list populated here.
        /// </summary>
        private void GenerateFields(int progressBarStart, int progressBarEnd)
        {
            this.SuspendLayout();
            this.progressBar1.Value = progressBarStart;
            try
            {
                pnlAutoFill.Visible = false;
                ToolTip t = new ToolTip();
                this.pnlFieldControls.Controls.Clear();
                this.Size = new Size(400, 500);
                pnlFieldControls.AutoScroll = true;
                //pnlFieldControls.AutoScrollMargin = new Size(20, pnlFieldControls.Height);
                pnlFieldControls.AutoScrollMinSize = new Size(20, pnlFieldControls.Height);

                int StartChunk = int.Parse(cbStartChunk.Text);
                int EndChunk = int.Parse(cbEndChunk.Text);

                StartOffset = RD.baseOffset + FieldControl.chunkOffset - (RD.chunkSelected * RD.reflexive.chunkSize);
                switch (FieldControl.GetType().ToString())
                {
                    case "entity.MetaEditor2.DataValues":
                        try
                        {
                            Control c = FieldControl.Controls[1];
                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x - StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();

                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                TextBox TB = new TextBox();
                                TB.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                TB.Location = new Point(150, 5);
                                TB.Height = c.Height;
                                TB.Width = c.Width;
                                TB.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                TB.Leave += new EventHandler(Field_Leave);
                                TB.TextChanged += new EventHandler(Field_TextChanged);
                                t.SetToolTip(TB, TB.Tag.ToString());

                                Casing.Controls.Add(TB);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();                               
                            }
                            pnlAutoFill.Visible = true;
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading DataValues fields");
                        }
                        break;

                    case "entity.MetaEditor2.EntStrings":
                        try
                        {
                            Control c = FieldControl.Controls[1];
                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x-StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();

                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                TextBox TB = new TextBox();
                                TB.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                TB.Location = new Point(150, 5);
                                TB.Height = c.Height;
                                TB.MaxLength = ((EntStrings)FieldControl).length;
                                TB.Width = Casing.Width - TB.Left - 30;
                                TB.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                TB.Leave += new EventHandler(Field_Leave);
                                t.SetToolTip(TB, TB.Tag.ToString());

                                Casing.Controls.Add(TB);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();

                            }
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading EntStrings fields");
                        }
                        break;

                    case "entity.MetaEditor2.Ident":
                        try
                        {
                            Control c = FieldControl.Controls[1];
                            this.Size = new Size(FieldControl.Width, 500);
                            System.Collections.IEnumerator i = FieldControl.map.MetaInfo.TagTypes.Keys.GetEnumerator();
                            List<string> TagTypes = new List<string>();
                            while (i.MoveNext())
                            {
                                TagTypes.Add((string)i.Current);
                            }
                            TagTypes.Sort();
                            TagTypes.Add((string)"null");

                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x - StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();

                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                ComboBox FieldTagType = new ComboBox();
                                FieldTagType.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                FieldTagType.DropDownStyle = ComboBoxStyle.DropDownList;
                                FieldTagType.Location = new Point(150, 5);
                                FieldTagType.Height = c.Height;
                                FieldTagType.Items.AddRange(TagTypes.ToArray());
                                FieldTagType.Width = FieldControl.Controls[2].Width;
                                FieldTagType.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                FieldTagType.SelectedIndexChanged += new EventHandler(FieldTagType_SelectedIndexChanged);
                                FieldTagType.Leave += new EventHandler(Field_Leave);
                                FieldTagType.TextChanged += new EventHandler(Field_TextChanged);
                                t.SetToolTip(FieldTagType, FieldTagType.Tag.ToString());
                                ComboBox FieldIdent = new ComboBox();
                                FieldIdent.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                FieldIdent.DropDownStyle = ComboBoxStyle.DropDownList;
                                FieldIdent.Location = new Point(250, 5);
                                FieldIdent.Height = c.Height;
                                FieldIdent.Width = FieldControl.Controls[1].Width;
                                if (((Ident)FieldControl).HasTagType)
                                    FieldIdent.Tag = StartOffset + x * RD.reflexive.chunkSize + 4;
                                else
                                    FieldIdent.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                FieldIdent.Leave += new EventHandler(Field_Leave);
                                t.SetToolTip(FieldIdent, FieldIdent.Tag.ToString());

                                Casing.Controls.Add(FieldTagType);
                                Casing.Controls.Add(FieldIdent);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading Tag/Ident fields");
                        }
                        break;

                    case "entity.MetaEditor2.SID":
                        try
                        {
                            Control c = FieldControl.Controls[1];
                            this.Size = new Size(FieldControl.Width, 500);
                            List<SIDData> SIDs = new List<SIDData>();
                            SIDs.AddRange(SIDData.CreateArray(FieldControl.map.Strings.Name));
                            SortStringIDs(ref SIDs);

                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x - StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();
                                
                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                ComboBox SIDBox = new ComboBox();
                                SIDBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                SIDBox.DropDownStyle = ComboBoxStyle.DropDownList;
                                SIDBox.Location = new Point(150, 5);
                                SIDBox.Height = c.Height;
                                SIDBox.Items.AddRange(SIDs.ToArray());
                                SIDBox.DisplayMember = "Name";
                                SIDBox.Width = FieldControl.Controls[1].Width;
                                SIDBox.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                SIDBox.Leave += new EventHandler(Field_Leave);
                                t.SetToolTip(SIDBox, SIDBox.Tag.ToString());

                                Casing.Controls.Add(SIDBox);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading String ID fields");
                        }
                        break;

                    case "entity.MetaEditor2.Enums":
                        try
                        {
                            ComboBox c = (ComboBox)FieldControl.Controls[1];
                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x - StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();

                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                ComboBox EnumsBox = new ComboBox();
                                EnumsBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                EnumsBox.Location = new Point(150, 5);
                                EnumsBox.Height = c.Height;
                                object[] objs = new object[c.Items.Count];
                                c.Items.CopyTo(objs, 0);
                                EnumsBox.Items.AddRange(objs);
                                EnumsBox.Width = FieldControl.Controls[1].Width;
                                EnumsBox.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                EnumsBox.Leave += new EventHandler(Field_Leave);
                                t.SetToolTip(EnumsBox, EnumsBox.Tag.ToString());

                                Casing.Controls.Add(EnumsBox);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();

                            }
                            pnlAutoFill.Visible = true;
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading Enums fields");
                        }
                        break;

                    case "entity.MetaEditor2.Indices":
                        try
                        {
                            Control c = FieldControl.Controls[1];
                            this.Size = new Size(FieldControl.Width, 500);
                            Indices indices = ((Indices)FieldControl);
                            // Make sure all values have been added to combobox (happens on dropdown)
                            indices.UpdateSelectionList(true);

                            for (int x = StartChunk; x <= EndChunk; x++)
                            {
                                // Update progress bar
                                this.progressBar1.Value = (x - StartChunk) * (progressBarEnd - progressBarStart) / Math.Max(1, (EndChunk - StartChunk));
                                this.progressBar1.Refresh();

                                Control Casing = new Control();
                                Casing.Dock = DockStyle.Top;
                                Casing.Padding = new Padding(4);
                                Casing.Size = new Size(this.Width, 31);
                                Label LBL = new Label();
                                LBL.Dock = DockStyle.Left;
                                LBL.Location = new Point(10, 4);
                                LBL.Size = new Size(140, 23);
                                LBL.Text = FieldControl.EntName + " #" + x.ToString();
                                LBL.TextAlign = ContentAlignment.MiddleLeft;
                                ComboBox Indices = new ComboBox();
                                Indices.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                                Indices.DropDownStyle = ComboBoxStyle.DropDownList;
                                Indices.Location = new Point(150, 5);
                                Indices.Height = c.Height;
                                Indices.Items.AddRange( indices.IndicesList.ToArray());
                                Indices.DisplayMember = "Name";
                                Indices.Width = Casing.Width - Indices.Left - 50;
                                Indices.Tag = StartOffset + x * RD.reflexive.chunkSize;
                                Indices.Leave += new EventHandler(Field_Leave);
                                t.SetToolTip(Indices, Indices.Tag.ToString());

                                Casing.Controls.Add(Indices);
                                Casing.Controls.Add(LBL);
                                pnlFieldControls.Controls.Add(Casing);
                                Casing.BringToFront();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("An error occured reading Indices/Block Index fields");
                        }
                        break;

                    default:
                        MessageBox.Show(FieldControl.GetType().ToString() + " fields are currently Unsupported.");
                        this.Close();
                        break;
                }
            }
            finally
            {
                this.ResumeLayout(true);
            }
        }

        // When the tag type selected index changes, refresh Tag Idents for that type
        void FieldTagType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox TagType = (ComboBox)sender;
            ComboBox TagIdent = (ComboBox)TagType.Parent.Controls[1];

            // See if tag type has changed. If not, save loading time and just exit.
            if (TagType.Parent.Tag != null && TagType.Parent.Tag.ToString() == TagType.Text)
                return;

            TagIdent.Items.Clear();
            if (TagType.Text == "null")
            {
                TagIdent.SelectedIndex = 0;
                TagIdent.Items.Add("null");
            }
            else
            {
                for (int x = 0; x < FieldControl.map.MetaInfo.TagType.Length; x++)
                {
                    if (FieldControl.map.MetaInfo.TagType[x] == TagType.Text)
                        TagIdent.Items.Add(FieldControl.map.FileNames.Name[x]);
                }
                // Sort the tags
                TagIdent.Sorted = true;
                TagIdent.Sorted = false;
                // Add the null tag
                TagIdent.Items.Add("null");
                // Select the null tag (no unselecteds)
                TagIdent.SelectedIndex = TagIdent.Items.Count - 1;
            }

            // Used to see if tag type has changed
            TagIdent.Parent.Tag = TagType.Text;
        }

        /// <summary>
        /// Populates the data for the given control
        /// </summary>
        /// <param name="c"></param>
        private void PopulateValue(Control c)
        {
            Control c2 = c.Controls[0];
            int offset = (int)c2.Tag;
            BR.BaseStream.Position = offset;
            switch (FieldControl.GetType().ToString())
            {
                case "entity.MetaEditor2.DataValues":
                    c2.Name = "DataValues";
                    switch (((DataValues)FieldControl).ValueType)
                    {
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Byte:
                            c2.Text = BR.ReadByte().ToString();
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Short:
                            c2.Text = BR.ReadInt16().ToString();
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UShort:
                            c2.Text = BR.ReadUInt16().ToString();
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Int:
                            c2.Text = BR.ReadInt32().ToString();
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UInt:
                            c2.Text = BR.ReadUInt32().ToString();
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Float:
                            c2.Text = BR.ReadSingle().ToString();
                            break;
                    }
                    break;

                case "entity.MetaEditor2.EntStrings":
                    EntStrings EntString = (EntStrings)FieldControl;
                    if (EntString.entUnicode)
                    {
                        c2.Text = Encoding.Unicode.GetString(BR.ReadBytes(EntString.length));
                    }
                    else
                    {
                        c2.Text = Encoding.ASCII.GetString(BR.ReadBytes(EntString.length));
                    }
                    c2.Text = c2.Text.TrimEnd('\0');

                    break;

                case "entity.MetaEditor2.Ident":
                    ComboBox c3 = (ComboBox)c.Controls[1];
                    if (((Ident)FieldControl).HasTagType)
                    {
                        byte[] TagTypeBytes = BR.ReadBytes(4);
                        Array.Reverse(TagTypeBytes);
                        string TagType = ASCIIEncoding.ASCII.GetString(TagTypeBytes);

                        if (TagTypeBytes[0] == (char)0xFF)      // Null values contain four characters of 255 (0xFF)
                            c2.Text = "null";
                        else
                            c2.Text = TagType;
                    }

                    int tempint = BR.ReadInt32();
                    int TagIndex = FieldControl.map.Functions.ForMeta.FindMetaByID(tempint);
                    if (TagIndex == -1)
                        c3.Text = "null";
                    else
                    {
                        if (!((Ident)FieldControl).HasTagType)
                            c2.Text = FieldControl.map.MetaInfo.TagType[TagIndex];
                        c3.Text = FieldControl.map.FileNames.Name[TagIndex];
                    }

                    break;

                case "entity.MetaEditor2.SID":
                    short SID = BR.ReadInt16();
                    BR.BaseStream.Position += 1;
                    byte SIDLength = BR.ReadByte();

                    if (SIDLength == FieldControl.map.Strings.Length[SID])
                        c2.Text = FieldControl.map.Strings.Name[SID];
                    else
                        c2.Text = string.Empty;
                    break;

                case "entity.MetaEditor2.Enums":
                    Enums Enum = (Enums)FieldControl;
                    switch (Enum.enumType)
                    {
                        case 8:
                            byte EValue8 = BR.ReadByte();
                            c2.Text = (string)((ComboBox)c2).Items[EValue8];
                            break;
                        case 16:
                            short EValue16 = BR.ReadInt16();
                            c2.Text = (string)((ComboBox)c2).Items[EValue16];
                            break;
                        case 32:
                            int EValue32 = BR.ReadInt32();
                            c2.Text = (string)((ComboBox)c2).Items[EValue32];
                            break;
                    }
                    break;

                case "entity.MetaEditor2.Indices":
                    Indices indices = (Indices)FieldControl;
                    int Value = 0;
                    switch (indices.ValueType)
                    {
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Short:
                            {
                                Value = BR.ReadInt16();
                                break;
                            }
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Int:
                            {
                                Value = BR.ReadInt32();
                                break;
                            }
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UShort:
                            {
                                Value = (int)BR.ReadUInt16();
                                break;
                            }
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UInt:
                            {
                                Value = (int)BR.ReadUInt32();
                                break;
                            }
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Byte:
                            {
                                Value = (int)BR.ReadByte();
                                break;
                            }
                    }
                    if (Value == -1)
                        c2.Text = indices.IndicesList[indices.IndicesList.Count - 1];
                    else
                        c2.Text = indices.IndicesList[Value];
                    break;
            }
        }

        /// <summary>
        /// Reads the data from the memory stream and fills in the visible fields.
        /// </summary>
        private void PopulateSelectedChunks(int progressBarStart, int progressBarEnd)
        {
            this.SuspendLayout();
            this.Enabled = false;
            try
            {
                foreach (Control c in this.pnlFieldControls.Controls)
                {
                    // Update progress bar
                    this.progressBar1.Value = progressBarStart + (pnlFieldControls.Controls.IndexOf(c) * (progressBarEnd-progressBarStart) / pnlFieldControls.Controls.Count);
                    this.progressBar1.Refresh();

                    PopulateValue(c);
                }
            }
            finally
            {
                this.Enabled = true;
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Arranges string IDs by name.
        /// </summary>
        /// <param name="SIDs"></param>
        private void SortStringIDs(ref List<SIDData> SIDs)
        {
            SIDs = SIDs.OrderBy(order => order.Name).ToList();
        }

        private void SaveFieldChange(Control c)
        {
            BW.BaseStream.Position = (int)c.Tag;
            switch (FieldControl.GetType().ToString())
            {
                // For bytes, (u)int16, (u)int32
                // *This section complete (? may still be missing types)*
                case "entity.MetaEditor2.DataValues":
                    long Value;
                    float FValue;
                    if (long.TryParse(c.Text, out Value))
                    {
                        switch (((DataValues)FieldControl).ValueType)
                        {
                            case HaloMap.Plugins.IFPIO.ObjectEnum.Byte:
                                BW.Write((byte)Value);
                                c.Text = ((byte)Value).ToString();
                                break;
                            case HaloMap.Plugins.IFPIO.ObjectEnum.Short:
                                BW.Write((Int16)Value);
                                c.Text = ((Int16)Value).ToString();
                                break;
                            case HaloMap.Plugins.IFPIO.ObjectEnum.UShort:
                                BW.Write((UInt16)Value);
                                c.Text = ((UInt16)Value).ToString();
                                break;
                            case HaloMap.Plugins.IFPIO.ObjectEnum.Int:
                                BW.Write((Int32)Value);
                                c.Text = ((Int32)Value).ToString();
                                break;
                            case HaloMap.Plugins.IFPIO.ObjectEnum.UInt:
                                BW.Write((UInt32)Value);
                                c.Text = ((UInt32)Value).ToString();
                                break;
                        }
                    }
                    else if (float.TryParse(c.Text, out FValue))
                    {
                        switch (((DataValues)FieldControl).ValueType)
                        {
                            case HaloMap.Plugins.IFPIO.ObjectEnum.Float:
                                BW.Write((float)FValue);
                                c.Text = ((float)FValue).ToString();
                                break;
                        }
                    }
                    else
                    {
                        this.Enabled = false;
                        // In case of error, reset to saved value
                        PopulateValue(c.Parent);
                        this.Enabled = true;
                    }
                    break;

                // *This section Complete*
                case "entity.MetaEditor2.EntStrings":
                    EntStrings EntString = (EntStrings)FieldControl;
                    byte[] StringBytes = Encoding.ASCII.GetBytes(c.Text.ToCharArray());
                    Array.Resize(ref StringBytes, EntString.length);
                    BW.Write(StringBytes);
                    break;

                // *This section Complete*
                case "entity.MetaEditor2.Ident":
                    ComboBox TagType = (ComboBox)c.Parent.Controls[0];
                    ComboBox Ident = (ComboBox)c.Parent.Controls[1];
                    
                    BW.BaseStream.Position = (int)TagType.Tag;

                    if (((Ident)FieldControl).HasTagType)
                    {
                        if (TagType.Text != "null")
                        {
                            byte[] TagTypeBytes = ASCIIEncoding.ASCII.GetBytes(TagType.Text);
                            Array.Reverse(TagTypeBytes);
                            BW.Write(TagTypeBytes);
                        }
                        else
                        {
                            byte[] TagTypeBytes = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
                            BW.Write(TagTypeBytes);
                        }
                    }
                    int Index = FieldControl.map.Functions.ForMeta.FindByNameAndTagType(TagType.Text, Ident.Text);
                    if (Index != -1)
                        BW.Write(FieldControl.map.MetaInfo.Ident[Index]);
                    else
                        BW.Write((int)-1);
                    break;

                // *This section Complete*
                case "entity.MetaEditor2.SID":
                    SIDData sd = (SIDData)((ComboBox)c).SelectedItem;
                    BW.Write((short)sd.Index);
                    BW.Write((byte)0);
                    BW.Write((byte)FieldControl.map.Strings.Length[sd.Index]);
                    //FieldControl.map.Strings.Name[(c.Text)];
                    break;

                // *This section Complete*
                case "entity.MetaEditor2.Enums":
                    Enums Enum = (Enums)FieldControl;
                    HaloMap.Plugins.IFPIO.Option o = (HaloMap.Plugins.IFPIO.Option)Enum.Options[((ComboBox)c).SelectedIndex];
                    switch (Enum.enumType)
                    {
                        case 8:
                            BW.Write((byte)o.value);
                            break;
                        case 16:
                            BW.Write((short)o.value);
                            break;
                        case 32:
                            BW.Write((int)o.value);
                            break;
                    }
                    break;

                // *This section Complete*
                case "entity.MetaEditor2.Indices":
                    Indices indices = (Indices)FieldControl;
                    int IndexValue = indices.IndicesList.IndexOf(c.Text);
                    // Last listing is for nulled resources
                    if (IndexValue == indices.IndicesList.Count - 1)
                        IndexValue = -1;
                    switch (indices.ValueType)
                    {
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Byte:
                            BW.Write((byte)IndexValue);
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Short:
                            BW.Write((short)IndexValue);
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UShort:
                            BW.Write((ushort)IndexValue);
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.Int:
                            BW.Write((int)IndexValue);
                            break;
                        case HaloMap.Plugins.IFPIO.ObjectEnum.UInt:
                            BW.Write((uint)IndexValue);
                            break;
                    }
                    break;

            }
        }

        private void Field_TextChanged(object sender, EventArgs e)
        {
            // Only used for Auto-fill reasons
            if (this.Enabled && !((Control)sender).Focused)
                SaveFieldChange((Control)sender);
        }

        private void Field_Leave(object sender, EventArgs e)
        {
            SaveFieldChange((Control)sender);
        }


        /// <summary>
        /// Resets values to last saved values or original if no save has taken place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetValues_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really reset values?", "Reset Fields?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.MS.Close();
                this.MS.Dispose();
                this.BR = new BinaryReader(FieldControl.meta.MS);
                this.BR.BaseStream.Position = 0;
                this.MS = new MemoryStream(this.BR.ReadBytes((int)this.BR.BaseStream.Length));
                this.BR = new BinaryReader(MS);
                PopulateSelectedChunks(0, 100);
                progressBar1.Value = 0;
            }
        }

        /// <summary>
        /// Save values to memory stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            this.BR.BaseStream.Position = 0;
            FieldControl.meta.MS.Close();
            FieldControl.meta.MS.Dispose();
            FieldControl.meta.MS = new MemoryStream(this.BR.ReadBytes((int)this.BR.BaseStream.Length));
            this.progressBar1.Value = 100;
            this.progressBar1.Refresh();
            System.Threading.Thread.Sleep(200);
            this.progressBar1.Value = 0;
            this.progressBar1.Refresh();
            
            // Let the owner form know we changed stuff
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        /// <summary>
        /// Adjusts visible fields according to initial value and change. Used for 
        /// incremental offsets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFill_Click(object sender, EventArgs e)
        {
            if (((DataValues)FieldControl).ValueType == HaloMap.Plugins.IFPIO.ObjectEnum.Float)
            {
                float InitialValue;
                float ChangeValue;
                if (float.TryParse(tbInitialValue.Text, out InitialValue)
                    && float.TryParse(tbChange.Text, out ChangeValue))
                {

                    int StartChunk = int.Parse(cbStartChunk.Text);
                    int EndChunk = int.Parse(cbEndChunk.Text);
                    for (int x = 0; x <= EndChunk - StartChunk; x++)
                    {
                        // Controls are stored bottom to top, so start at last control
                        pnlFieldControls.Controls[(EndChunk - StartChunk) - x].Controls[0].Text = (InitialValue + x * ChangeValue).ToString();
                    }
                }
            }
            else
            {
                int InitialValue;
                int ChangeValue;
                if (int.TryParse(tbInitialValue.Text, out InitialValue)
                    && int.TryParse(tbChange.Text, out ChangeValue))
                {

                    int StartChunk = int.Parse(cbStartChunk.Text);
                    int EndChunk = int.Parse(cbEndChunk.Text);
                    for (int x = 0; x <= EndChunk - StartChunk; x++)
                    {
                        // Controls are stored bottom to top, so start at last control
                        pnlFieldControls.Controls[(EndChunk - StartChunk) - x].Controls[0].Text = (InitialValue + x * ChangeValue).ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Raised when either the start chunk or end chunk number changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChunk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbStartChunk.Focused && !cbEndChunk.Focused)
                return;

            int StartValue;
            int EndValue;
            if (int.TryParse(cbStartChunk.Text, out StartValue) && int.TryParse(cbEndChunk.Text, out EndValue))
            {
                if (sender == cbStartChunk)
                {
                    if (StartValue > EndValue)
                    {
                        cbEndChunk.Text = cbStartChunk.Text;
                    }
                }
                else
                    if (sender == cbEndChunk)
                    {
                        if (EndValue < StartValue)
                        {
                            cbStartChunk.Text = cbEndChunk.Text;
                        }
                    }
            }
            // If a start/end chunk changed, start the update timer.
            timer1.Start();
        }

        /// <summary>
        /// When the start/end chunk combobox is dropped down, stop the control update timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbEndChunk_DropDown(object sender, EventArgs e)
        {
            // Remember if the timer was running on entry
            ((Control)sender).Tag = timer1.Enabled;
            // Pause the timer while changing chunk start/end values
            timer1.Stop();
        }

        /// <summary>
        /// When the start/end chunk combobox dropdown is closed, restart the timer if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbEndChunk_DropDownClosed(object sender, EventArgs e)
        {
            // If the value wasn't changed, but the timer was running before, start it again.
            if ((bool)((Control)sender).Tag == true)
                timer1.Start();
        }

        /// <summary>
        /// Occurs 1 second after changing the start or end chunk values. Allows both to be 
        /// changed before updating the controls so only one update is needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            GenerateFields(0, 50);
            PopulateSelectedChunks(50, 100);
            progressBar1.Value = 0;
        }

    }
}