using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HaloMap.RawData;

namespace entity.BitmapOps
{
    public partial class BitmapOperations : Form
    {
        public cType inp;
        public cType outp;
        public List<formatTypes> formats;

        public class formatTypes
        {
            public string name;
            public int index;
            public int bits;
            public bool compressed;

            public formatTypes(string name, int index, int bits, bool compressed)
            {
                this.name = name;
                this.index = index;
                this.bits = bits;
                this.compressed = compressed;
            }
        }

        public enum classTypes
        {
            DDS,
            Jpeg,
            Bitmap
        }

        public class cType
        {
            public classTypes classType;
            public Stream stream;
            
            public int BitsPerPixel;
            public int Width;
            public int Height;
            public int Depth;
        }

        public class DDS : cType
        {
            public int streamOffset;
            public int streamSize;
            
            public ushort Format;
            public ParsedBitmap.BitmapFormat FormatName;
            public bool Compressed;
            public bool Swizzle;
            public int MipMapCount;
            public int chunkNumber;

            public DDS(int BPP, int width, int height, int depth, int streamOffset, int streamSize, int chunkNumber)
            {
                this.classType = classTypes.DDS;
                this.BitsPerPixel = BPP;
                this.Width = width;
                this.Height = height;
                this.Depth = depth;
                this.streamOffset = streamOffset;
                this.streamSize = streamSize;
                this.chunkNumber = chunkNumber;
            }

            public DDS(ParsedBitmap.BitmapInfo bmInfo, FileStream fs)
            {
                this.classType = classTypes.DDS;
                this.BitsPerPixel = bmInfo.bitsPerPixel;
                this.Width = bmInfo.width;
                this.Height = bmInfo.height;
                this.Depth = bmInfo.depth;
                this.Format = bmInfo.format;
                this.FormatName = bmInfo.formatname;
                this.Compressed = bmInfo.format.ToString().ToUpper().Contains("DXT");
                this.Swizzle = bmInfo.swizzle;
                this.MipMapCount = bmInfo.mipMapCount;

                this.stream = fs;
                this.streamOffset = (int)fs.Position;
                this.streamSize = (int)fs.Length;
            }
/*
            enum Bits8
            {
                A8 = 0,
                Y8 = 1,
                AY8 = 2,
                P8 = 17,
            }

            enum Bits16
            {
                A8Y8 = 3,
                R5G6B5 = 6,
                A1R5G5B5 = 8,
                A4R4G4B4 = 9,
                G8B8 = 22,
            }

            enum Bits32
            {
                X8R8G8B8 = 10,
                A8R8G8B8 = 11,
            }

            enum Compressed
            {
                DXT1 = 14,
                DXT2AND3 = 15,
                DXT4AND5 = 16,
            }
 */
        }

        public class JPG : cType
        {
            public JPG()
            {
                this.classType = classTypes.Jpeg;
            }
        }

        public class BMP : cType
        {
            public BMP()
            {
                this.classType = classTypes.Bitmap;
            }
        }

        public BitmapOperations(cType inp)
        {
            InitializeComponent();

            this.inp = inp;
            setDetails(inp, true);
        }

        public List<formatTypes> getDDSFormats()
        {
            formats = new List<formatTypes>();
            string[] strs = Enum.GetNames(typeof(ParsedBitmap.BitmapType));
            foreach (string s in strs)
                if (s.StartsWith("BITM_FORMAT_"))
                {
                    int bits = 0;
                    for (int i = 0; i < s.Length; i++)
                        if (char.IsDigit(s[i]))
                            bits += int.Parse(s[i].ToString());
                    int o = (int)Enum.Parse(typeof(ParsedBitmap.BitmapType), s);
                    formats.Add( new formatTypes( s.Substring(12),
                                                  (int)Enum.Parse(typeof(ParsedBitmap.BitmapType), s),
                                                  s.Contains("DXT") ? 32 : bits,
                                                  s.Contains("DXT")));
                }
            return formats;
        }

        public void setFormats(cType input)
        {
            switch (input.classType)
            {
                case classTypes.DDS:
                    DDS inpDDS = input as DDS;
                    formats = getDDSFormats();
                    comboBoxFormat.Items.Clear();
                    foreach (formatTypes ft in formats)
                    {
                        if (ft.compressed == inpDDS.Compressed &&
                            ft.bits == inpDDS.BitsPerPixel)
                            comboBoxFormat.Items.Add(ft.name);
                    }
                    break;
                case classTypes.Jpeg:
                    break;
                case classTypes.Bitmap:
                    break;
            }

        }

        public void setDetails(cType input, bool orig)
        {
            if (orig)
            {
                dataOrigType.Text = input.classType.ToString();
                dataOrigBPP.Text = input.BitsPerPixel.ToString();
                dataOrigWidth.Text = input.Width.ToString();
                dataOrigHeight.Text = input.Height.ToString();
            }

            switch (input.classType)
            {
                case classTypes.DDS:
                    DDS DDSInput = input as DDS;
                    radioButtonDDS.Checked = true;
                    // Compressed/Uncompressed
                    if (DDSInput.Compressed)
                    {
                        radioButtonCompressed.Checked = true;
                        radioButton32Bit.Checked = true;
                        radioButton16Bit.Enabled = false;
                        radioButton8Bit.Enabled = false;
                    }
                    else
                    {
                        radioButtonUncompressed.Checked = true;
                        radioButton16Bit.Enabled = true;
                        radioButton8Bit.Enabled = true;
                        switch (DDSInput.BitsPerPixel)
                        {
                            case 8:
                                radioButton8Bit.Checked = true;
                                break;
                            case 16:
                                radioButton16Bit.Checked = true;
                                break;
                            case 32:
                                radioButton32Bit.Checked = true;
                                break;
                        }
                    }

                    if (orig)
                    {
                        dataOrigFormat.Text = DDSInput.FormatName.ToString();
                        for (int i = 1; i < DDSInput.MipMapCount; i++)
                            comboBoxMipmaps.Items.Add(i);
                        if (comboBoxMipmaps.Items.Count > 0)
                        {
                            checkBoxMipMaps.Enabled = true;
                            checkBoxMipMaps.Checked = true;
                            comboBoxMipmaps.SelectedIndex = comboBoxMipmaps.Items.Count - 1;
                        }
                        else
                        {
                            checkBoxMipMaps.Checked = false;
                            checkBoxMipMaps.Enabled = false;
                        }
                    }

                    setFormats(DDSInput);
                    break;
                case classTypes.Jpeg:
                    JPG JPGInput = input as JPG;
                    radioButtonJpeg.Checked = true;
                    break;
                case classTypes.Bitmap:
                    BMP BMPInput = input as BMP;
                    radioButtonBitmap.Checked = true;
                    break;
            }
        }

        public Bitmap DDSRead(DDS inp)
        {
            inp.stream.Position = inp.streamOffset;
            byte[] b = new byte[inp.streamSize - inp.streamOffset];
            inp.stream.Read(b, 0, b.Length);

            Bitmap bm = ParsedBitmap.DecodeBitm( b,
                                              inp.Height,
                                              inp.Width,
                                              inp.Depth,
                                              inp.BitsPerPixel,
                                              inp.
                                              inp.FormatName,
                                              inp.Swizzle,
                                              0,        // mapnumber only matters for lightmaps
                                              0,        // vcindex only matters for lightmaps
                                              0);       // ident only matters for lightmaps
            return bm;
        }

        public void DDSConvert(cType input, cType output)
        {

        }

        public cType convert(cType inputType)
        {
            Bitmap bm = null;

            // Decoding section
            if (inputType is DDS)
            {
                //bm = DDSRead(inputType as DDS);
                DDSConvert(inp, outp);
            }
            else if (inputType is JPG)
                bm = new Bitmap(inputType.stream);
            else if (inputType is BMP)
                bm = new Bitmap(inputType.stream);

            // Encoding section
            bm.Save(outp.stream, System.Drawing.Imaging.ImageFormat.Bmp);
            if (outp is DDS)
            {
                DDS inp = inputType as DDS;
                //switch (formats) { }

                return outp;
            }
            else if (outp is JPG)
            {
                cType outputType = new JPG();
                bm.Save(outp.stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return outputType;
            }
            else if (outp is BMP)
            {
                cType outputType = new BMP();
                bm.Save(outp.stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return outputType;
            }
            
            return null;
        }

        private void radioButtonDDS_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // Make sure every DDS option is available
                groupBoxCompression.Enabled = true;
                groupBoxBPP.Enabled = true;
                radioButton32Bit.Enabled = true;
                radioButton16Bit.Enabled = true;
                radioButton8Bit.Enabled = true;
                labelFormat.Enabled = true;
                comboBoxFormat.Enabled = true;
                checkBoxMipMaps.Enabled = true;
                comboBoxMipmaps.Enabled = true;
                setDetails(getSettings(),false);
            }
        }

        private void radioButtonJpeg_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // Make sure JPG options are available
                groupBoxCompression.Enabled = false;
                groupBoxBPP.Enabled = true;
                radioButton32Bit.Enabled = false;
                radioButton16Bit.Enabled = false;
                radioButton8Bit.Enabled = false;
                labelFormat.Enabled = false;
                comboBoxFormat.Enabled = false;
                checkBoxMipMaps.Enabled = true;
                comboBoxMipmaps.Enabled = true;
                setDetails(getSettings(), false);
            }
        }

        private void radioButtonBitmap_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // Make sure BMP options are available
                groupBoxCompression.Enabled = false;
                groupBoxBPP.Enabled = true;
                radioButton32Bit.Enabled = true;
                radioButton16Bit.Enabled = true;
                radioButton8Bit.Enabled = true;
                labelFormat.Enabled = true;
                comboBoxFormat.Enabled = true;
                checkBoxMipMaps.Enabled = false;
                comboBoxMipmaps.Enabled = false;
                setDetails(getSettings(), false);
            }
        }
        
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Focused)
                setDetails(getSettings(), false);
        }

        private void checkBoxMipMaps_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                comboBoxMipmaps.Enabled = true;
            else
                comboBoxMipmaps.Enabled = false;
        }

        private cType getSettings()
        {
            if (radioButtonDDS.Checked)
            {
                int bits = radioButton8Bit.Checked ? 8 : radioButton16Bit.Checked ? 16 : radioButton32Bit.Checked ? 32: 0;
                DDS outp = new DDS(bits, inp.Width, inp.Height, inp.Depth, 0, 0, 0);
                outp.Compressed = radioButtonCompressed.Checked;
                if (comboBoxFormat.SelectedItem != null)
                    outp.FormatName = (ParsedBitmap.BitmapFormat) Enum.Parse(typeof(ParsedBitmap.BitmapType), "BITM_FORMAT_" + comboBoxFormat.SelectedItem.ToString());
                return outp;
            }
            else if (radioButtonJpeg.Checked)
            {
                JPG outp = new JPG();
                return outp;
            }
            else if (radioButtonBitmap.Checked)
            {
                BMP outp = new BMP();
                return outp;
            }
            return null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            outp = getSettings();
            outp = convert(this.inp);
            
        }
    }
}
