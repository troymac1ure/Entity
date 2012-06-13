// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HexView.cs" company="">
//   
// </copyright>
// <summary>
//   The hex view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.HexEditor
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Meta;
    using Be.Windows.Forms;

    /// <summary>
    /// The hex view.
    /// </summary>
    /// <remarks></remarks>
    public partial class HexView : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The map offset.
        /// </summary>
        public int MapOffset;

        /// <summary>
        /// The file path.
        /// </summary>
        public string filePath;

        /// <summary>
        /// The gt.
        /// </summary>
        private readonly GoToDialogBox gt = new GoToDialogBox();

        /// <summary>
        /// The offset.
        /// </summary>
        private int Offset;

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        /// <summary>
        /// The meta.
        /// </summary>
        private Meta meta;

        /// <summary>
        /// The selectionlength.
        /// </summary>
        private int selectionlength;

        /// <summary>
        /// The selectionoffset.
        /// </summary>
        private int selectionoffset;

        /// <summary>
        /// The txt bitmask.
        /// </summary>
        private string txtBitmask = string.Empty;

        /// <summary>
        /// The txt byte.
        /// </summary>
        private string txtByte = string.Empty;

        /// <summary>
        /// The txt float.
        /// </summary>
        private string txtFloat = string.Empty;

        /// <summary>
        /// The txt ident name.
        /// </summary>
        private string txtIdentName = string.Empty;

        /// <summary>
        /// The txt ident tag type.
        /// </summary>
        private string txtIdentTagType = string.Empty;

        /// <summary>
        /// The txt long.
        /// </summary>
        private string txtLong = string.Empty;

        /// <summary>
        /// The txt reflexive count.
        /// </summary>
        private string txtReflexiveCount = string.Empty;

        /// <summary>
        /// The txt reflexive tag name.
        /// </summary>
        private string txtReflexiveTagName = string.Empty;

        /// <summary>
        /// The txt reflexive tag type.
        /// </summary>
        private string txtReflexiveTagType = string.Empty;

        /// <summary>
        /// The txt reflexive translation.
        /// </summary>
        private string txtReflexiveTranslation = string.Empty;

        /// <summary>
        /// The txt sid.
        /// </summary>
        private string txtSID = string.Empty;

        /// <summary>
        /// The txt short.
        /// </summary>
        private string txtShort = string.Empty;

        /// <summary>
        /// The txt u byte.
        /// </summary>
        private string txtUByte = string.Empty;

        /// <summary>
        /// The txt u long.
        /// </summary>
        private string txtULong = string.Empty;

        /// <summary>
        /// The txt u short.
        /// </summary>
        private string txtUShort = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HexView"/> class.
        /// </summary>
        /// <remarks></remarks>
        public HexView()
        {
            InitializeComponent();
            this.Enabled = false;
        }

        #endregion

        // Thanks to neilck over at the code project for these 3 functions (GetBytes, IsHexDigit and HexToByte)
        #region Public Methods

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] GetBytes(string hexString)
        {
            string newString = string.Empty;
            char c;

            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                {
                    newString += c;
                }
            }

            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new string(new[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j += 2;
            }

            return bytes;
        }

        /// <summary>
        /// The colorize.
        /// </summary>
        /// <remarks></remarks>
        public void Colorize()
        {
            if (meta == null)
            {
                return;
            }

            long tempstart = this.hexBox1.SelectionStart;
            long templength = this.hexBox1.SelectionLength;

            // this.txtbHexViewer.DeselectAll();
            Color c = this.hexBox1.SelectionForeColor;
            
            for (int x = 0; x < meta.items.Count; x++)
            {
                if (meta.items[x].offset >= Offset && meta.items[x].offset < Offset + 476)
                {
                    
                    if (meta.items[x] is Meta.Reflexive)
                    {
                        this.hexBox1.Select((meta.items[x].offset - Offset), 8);
                        this.hexBox1.SelectionForeColor = Color.Red;
                    }
                    else if (meta.items[x] is Meta.String)
                    {
                        this.hexBox1.Select((meta.items[x].offset - Offset), 4);
                        this.hexBox1.SelectionForeColor = Color.Blue;
                    }
                    else if (meta.items[x] is Meta.Ident)
                    {
                        this.hexBox1.Select((meta.items[x].offset - Offset), 4);
                        this.hexBox1.SelectionForeColor = Color.YellowGreen;
                    }
                    
                }
            }

            this.hexBox1.Select(tempstart, templength);
            this.hexBox1.SelectionForeColor = c;

            // Application.DoEvents();
        }

        /// <summary>
        /// The convert hex string to byte array.
        /// </summary>
        /// <param name="hex">The hex.</param>
        /// <remarks></remarks>
        public void ConvertHexStringToByteArray(string hex)
        {
            if ((hex.Length == 0) || (meta == null))
            {
                return;
            }

            ClearTxt();
            byte[] bytes = GetBytes(hex);
            switch (bytes.Length)
            {
                case 1:
                    {
                        this.txtUByte = bytes[0].ToString();
                        this.txtByte = ConvertBytesTosbyte(bytes).ToString();
                        break;
                    }

                case 2:
                    {
                        this.txtShort = BitConverter.ToInt16(bytes, 0).ToString();

                        // ConvertBytesToint16(bytes).ToString();
                        this.txtUShort = BitConverter.ToUInt16(bytes, 0).ToString();
                        goto case 1;
                    }

                case 3:
                    {
                        goto case 2;
                    }

                case 4:
                    {
                        this.txtLong = BitConverter.ToInt32(bytes, 0).ToString();

                        // ConvertBytesToInt32(bytes).ToString();
                        this.txtULong = BitConverter.ToUInt32(bytes, 0).ToString();

                        // ConvertBytesToUInt32(bytes).ToString();=
                        this.txtFloat = BitConverter.ToSingle(bytes, 0).ToString();
                        int temp = BitConverter.ToInt32(bytes, 0);
                        int tagIndex = map.Functions.ForMeta.FindMetaByID(temp);
                        if (tagIndex != -1)
                        {
                            this.txtIdentTagType = map.MetaInfo.TagType[tagIndex];
                            this.txtIdentName = map.FileNames.Name[tagIndex];
                        }

                        int stringid = temp & 0xFFFF;
                        int stringcount = (temp >> 16) & 0xFFFF;
                        int middlebyte = stringcount & 0xFF;
                        stringcount = (stringcount >> 8) & 0xFF;

                        if (stringid > 0 && stringid < map.MapHeader.scriptReferenceCount &&
                            map.Strings.Length[stringid] == stringcount && middlebyte == 0)
                        {
                            this.txtSID = map.Strings.Name[stringid];
                        }

                        ConvertBytesToBoolArray(bytes);
                        goto case 2;
                    }

                case 8:
                    int temp1 = BitConverter.ToInt32(bytes, 0);
                    int temp2 = BitConverter.ToInt32(bytes, 4);
                    int rcount = temp1 & 0xFFFF;
                    int rzero = (rcount >> 8) & 0xFF;
                    rcount = rcount & 0xFF;
                    int rtranslation = temp2 - meta.magic;
                    int tagnumx = map.Functions.ForMeta.FindMetaByOffset(rtranslation);

                    if (tagnumx != -1 && rzero == 0)
                    {
                        rtranslation -= map.MetaInfo.Offset[tagnumx];
                        this.txtReflexiveCount = rcount.ToString();
                        this.txtReflexiveTagType = map.MetaInfo.TagType[tagnumx];
                        this.txtReflexiveTagName = map.FileNames.Name[tagnumx];
                        this.txtReflexiveTranslation = rtranslation.ToString();
                    }

                    goto case 4;
                default:
                    {
                        if (bytes.Length > 8)
                        {
                            goto case 8;
                        }

                        if (bytes.Length > 4)
                        {
                            goto case 4;
                        }

                        break;
                    }
            }
            UpdateTxtbInfo();
        }

        /// <summary>
        /// The get offset in meta.
        /// </summary>
        /// <returns>The get offset in meta.</returns>
        /// <remarks></remarks>
        public int GetOffsetInMeta()
        {
            return -1;
        }

        /// <summary>
        /// The reload.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void Reload(Meta m, Map map)
        {
            this.lblRawView.Visible = false;
            this.cbRawNumber.Visible = false;
            this.Enabled = false;

            if (m == null)
            {
                return;
            }

            this.map = map;
            meta = m;
            this.Enabled = true;

            if (m.raw != null)
            {
                this.lblRawView.Visible = true;
                this.cbRawNumber.Items.Clear();
                for (int i = 0; i < m.raw.rawChunks.Count; i++)
                {
                    this.cbRawNumber.Items.Add(i + " (" + m.raw.rawChunks[i].size.ToString("N0") + ")");
                }
                this.cbRawNumber.SelectedIndex = 0;
                if (m.raw.rawChunks.Count > 1)
                    this.cbRawNumber.Visible = true;
            }


            this.lblMetaView_Click(this, null);
        }

        /// <summary>
        /// The setup.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void Setup(string path)
        {
            filePath = path;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The hex to byte.
        /// </summary>
        /// <param name="hex">The hex.</param>
        /// <returns>The hex to byte.</returns>
        /// <exception cref="ArgumentException">
        ///   </exception>
        /// <remarks></remarks>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }

            byte newByte = byte.Parse(hex, NumberStyles.HexNumber);
            return newByte;
        }

        /// <summary>
        /// The is hex digit.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The is hex digit.</returns>
        /// <remarks></remarks>
        private static bool IsHexDigit(char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
            {
                return true;
            }

            if (numChar >= num1 && numChar < (num1 + 10))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The clear txt.
        /// </summary>
        /// <remarks></remarks>
        private void ClearTxt()
        {
            txtBitmask = string.Empty;
            txtByte = string.Empty;
            txtUByte = string.Empty;
            txtShort = string.Empty;
            txtUShort = string.Empty;
            txtLong = string.Empty;
            txtULong = string.Empty;
            txtFloat = string.Empty;
            txtIdentTagType = string.Empty;
            txtIdentName = string.Empty;
            txtSID = string.Empty;
            txtReflexiveCount = string.Empty;
            txtReflexiveTagName = string.Empty;
            txtReflexiveTagType = string.Empty;
            txtReflexiveTranslation = string.Empty;
        }

        /// <summary>
        /// The convert bytes to bool array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <remarks></remarks>
        private void ConvertBytesToBoolArray(byte[] bytes)
        {
            bool[] bitArray = new bool[32];
            int input = BitConverter.ToInt32(bytes, 0);
            int bittester = 1;
            for (int counter = 0; counter < 32; counter++)
            {
                bitArray[counter] = (input & bittester) > 0 ? true : false;
                bittester *= 2;
            }

            MakeBinaryStringFromBoolArray(bitArray);
        }

        /// <summary>
        /// The convert bytes tosbyte.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The convert bytes tosbyte.</returns>
        /// <remarks></remarks>
        private sbyte ConvertBytesTosbyte(byte[] bytes)
        {
            return (sbyte)bytes[0];
        }

        /// <summary>
        /// The hex view_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void HexView_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// The make binary string from bool array.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <remarks></remarks>
        private void MakeBinaryStringFromBoolArray(bool[] bits)
        {
            txtBitmask = string.Empty;
            for (int counter = 0; counter < bits.Length; counter++)
            {
                txtBitmask += bits[counter] ? "1" : "0";
            }
        }

        /// <summary>
        /// The update txtb info.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateTxtbInfo()
        {
            txtbInfo.Text = "Unsigned Byte      " + txtUByte + '\n' + "Signed Byte        " + txtByte + '\n' +
                            "Unsigned Short     " + txtUShort + '\n' + "Signed Short       " + txtShort + '\n' +
                            "Unsigned Long      " + txtULong + '\n' + "Signed Long        " + txtLong + '\n' +
                            "Float              " + txtFloat + '\n' + "Bitmask            " + txtBitmask + '\n';
            int p1 = txtbInfo.Text.Length;
            txtbInfo.Text +=
                            "Ident Tag Type     " + txtIdentTagType + '\n' + "Ident Name         " + txtIdentName + '\n';
            int p2 = txtbInfo.Text.Length;
            txtbInfo.Text +=
                            "StringID           " + txtSID + '\n';
            int p3 = txtbInfo.Text.Length;
            txtbInfo.Text +=
                            "Reflexive - Count  " + txtReflexiveCount + '\n' + 
                            "Points To - Type   " + txtReflexiveTagType + '\n' + "Points To - Name   " +
                            txtReflexiveTagName + '\n' + "Points To - Offset " + txtReflexiveTranslation;
            txtbInfo.Select(0, p1 - 0);
            txtbInfo.SelectionColor = Color.Black;
            txtbInfo.Select(p1, p2 - p1);
            txtbInfo.SelectionColor = Color.Green;
            txtbInfo.Select(p2, p3 - p2);
            txtbInfo.SelectionColor = Color.Blue;
            txtbInfo.Select(p3, txtbInfo.Text.Length - p3);
            txtbInfo.SelectionColor = Color.Red;
        }

        /// <summary>
        /// The go to tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gt.ShowDialog();
            if (gt.DialogResult != DialogResult.Cancel)
            {
                this.Offset = gt.Offset;
                /*
                this.txtbHexViewer.LoadHex(this.Offset);
                this.OffsetUpdater(this.Offset);

                vScrollBar1.Value = this.Offset;
                this.txtbHexViewer.SelectionLength = 0;
                this.txtbHexViewer.SelectionStart = 0;
                */
            }
        }

        /// <summary>
        /// The save tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(this.filePath, FileMode.Open);
            BinaryWriter BW = new BinaryWriter(fs);
            BW.BaseStream.Position = meta.offset;
            //BW.Write(this.txtbHexViewer.ByteArray);
            BW.Close();
            fs.Close();
        }

        /// <summary>
        /// The txtb hex viewer_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void txtbHexViewer_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// The txtb hex viewer_ key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void txtbHexViewer_KeyPress(object sender, KeyPressEventArgs e)
        {
            // if (e. == Keys.G && e.Control == true)
            // {
            // gt.ShowDialog();
            // if (gt.DialogResult != DialogResult.Cancel)
            // {
            // ///       this.Offset = gt.Offset;
            // this.txtbHexViewer.LoadHex(this.Offset);
            // this.OffsetUpdater(this.Offset);
            // }
            // }
        }

        #endregion

        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            hexBox1.CopyHex();
            IDataObject da = Clipboard.GetDataObject();
            if (da.GetDataPresent(typeof(string)))
            {
                string hexString = (string)da.GetData(typeof(string));
                ConvertHexStringToByteArray(hexString);
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            bool hc = hexBox1.ByteProvider.HasChanges();

            try
            {
                DynamicByteProvider dynamicByteProvider = hexBox1.ByteProvider as DynamicByteProvider;
                System.Collections.Generic.List<byte> data = dynamicByteProvider.Bytes;

                FileStream fs = new FileStream(this.filePath, FileMode.Open);
                BinaryWriter BW = new BinaryWriter(fs);
                BW.BaseStream.Position = meta.offset;
                BW.Write(data.ToArray());
                BW.Close();
                fs.Close();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }
        }

        private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
        {
            long bytePos = (hexBox1.CurrentLine - 1) * hexBox1.BytesPerLine + hexBox1.CurrentPositionInLine - 1;
            lblPosition.Text = string.Format("{0} / {1}",
                                    bytePos,
                                    hexBox1.ByteProvider.Length - 1);
            lblLocation.Text = string.Format("Ln {0}    Col {1}",
                                    hexBox1.CurrentLine, hexBox1.CurrentPositionInLine);
            if (hexBox1.SelectionLength == 0 && bytePos < hexBox1.ByteProvider.Length-1)
            {
                // We need to have a selection length of 1 for CopyHex() to work
                hexBox1.SelectionLength = 1;
                hexBox1.SelectionLength = 0;
            }
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            long gotoValue;
            if (!long.TryParse(cbGoto.Text, out gotoValue))
            {
                MessageBox.Show("Value not in correct format");
                return;
            }
            if (gotoValue < 0) 
                gotoValue = 0;
            if (gotoValue >= hexBox1.ByteProvider.Length)
                gotoValue = hexBox1.ByteProvider.Length - 1;
            cbGoto.Text = gotoValue.ToString();
            hexBox1.SelectionStart = gotoValue;
            hexBox1.SelectionLength = 1;
            hexBox1.Focus();
        }

        private void lblMetaView_Click(object sender, EventArgs e)
        {
            this.lblMetaView.BorderStyle = BorderStyle.Fixed3D;
            this.lblRawView.BorderStyle = BorderStyle.FixedSingle;
        
            try
            {
                DynamicByteProvider dynamicByteProvider;
                // try to open in write mode
                dynamicByteProvider = new DynamicByteProvider(meta.MS.ToArray());
                hexBox1.ByteProvider = dynamicByteProvider;
                hexBox1_SelectionStartChanged(this, null);
                Colorize();
                UpdateTxtbInfo();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
                return;
            }
        }

        private void lblRawView_Click(object sender, EventArgs e)
        {
            this.lblMetaView.BorderStyle = BorderStyle.FixedSingle;
            this.lblRawView.BorderStyle = BorderStyle.Fixed3D;
            
            try
            {
                DynamicByteProvider dynamicByteProvider;
                // try to open in write mode
                dynamicByteProvider = new DynamicByteProvider(meta.raw.rawChunks[this.cbRawNumber.SelectedIndex].MS.ToArray());
                hexBox1.ByteProvider = dynamicByteProvider;
                hexBox1_SelectionStartChanged(this, null);
                UpdateTxtbInfo();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
                return;
            }
        }

        private void cbRawNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblRawView_Click(sender, e);
        }
    }
}