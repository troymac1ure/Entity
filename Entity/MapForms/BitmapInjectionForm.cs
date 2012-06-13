using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HaloMap.DDSFunctions;
using HaloMap.RawData;
//using HaloMap.Map;

namespace entity.MapForms
{
    public partial class BitmapInjectionForm : Form
    {
        ParsedBitmap bitm;
        string filename;
        int index;
        HaloMap.Map.Map map;
        DDS.DDS_HEADER_STRUCTURE dds;

        public BitmapInjectionForm(HaloMap.Map.Map Map, string Filename, ParsedBitmap ParsedBitm, int NumDDS)
        {
            InitializeComponent();

            this.bitm = ParsedBitm;
            this.filename = Filename;
            this.map = Map;

            // Load the DDS info & picture
            loadDDSInfo(filename, this.bitm);

            // ?????
            filename = filename.Substring(filename.LastIndexOf('\\') + 1);
                       
            // Populate Bitmap Injection box with number of bitmaps in current selection
            for (int i = 0; i < NumDDS; i++)
            {
                lbInjectionBitmap.Items.Add("Bitmap #" + i.ToString());
            }

            #region inject_Form
            this.Text = "Inject " + filename + " to...";
            if (lbInjectionBitmap.Items.Count > 0)
            {
                lbInjectionBitmap.SelectedIndex = 0;
            }

            #endregion

        }

        public int getBitmapLocation()
        {
            return (int.Parse(this.Text));
        }

        public void loadDDSInfo(string filename, ParsedBitmap pb)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            dds.ReadStruct(ref br);

            // Determine Format Type
            ParsedBitmap.BitmapType bitmapType = ParsedBitmap.BitmapType.BITM_TYPE_2D;
            if (((int)DDS.DDSEnum.DDSCAPS2_VOLUME & dds.ddsd.ddsCaps.caps2) > 0)
                bitmapType = ParsedBitmap.BitmapType.BITM_TYPE_3D;
            else if (((int)DDS.DDSEnum.DDSCAPS2_CUBEMAP & dds.ddsd.ddsCaps.caps2) > 0)
                bitmapType = ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP;


            // Used to display external (file) bitmap to be injected
            int tempsize = dds.ddsd.width * dds.ddsd.height;
            switch (dds.ddsd.ddfPixelFormat.FourCC)
            {
                case "DXT1":
                    tempsize /= 2;
                    break;
                case "DXT2":
                case "DXT3":
                case "DXT4":
                case "DXT5":

                    // tempsize /= 1;
                    break;

                // for non-compressed
                default:
                    tempsize *= dds.ddsd.ddfPixelFormat.RGBBitCount >> 3;
                    break;
            }

            int widthPad = 0;
            if (dds.ddsd.width % 16 != 0)
                widthPad = 16 - dds.ddsd.width % 16;

            int byteStep = dds.ddsd.ddfPixelFormat.RGBBitCount / 8;
            /*
            int totalSize = tempsize + (dds.ddsd.height * widthPad * byteStep);
            if (bitmapType == ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP)
                totalSize *= 6;
            byte[] guh = new byte[totalSize];
            */
            byte[] guh = new byte[br.BaseStream.Length-br.BaseStream.Position + (widthPad * dds.ddsd.height * byteStep)];
            if (widthPad == 0)
            {
                br.BaseStream.Read(guh, 0, guh.Length);
            }
            else
            {
                // Change data to include padding
                for (int h = 0; h < dds.ddsd.height; h++)
                {
                    br.BaseStream.Read(
                        guh, h * (dds.ddsd.width + widthPad) * byteStep, dds.ddsd.width * byteStep);
                }
            }
            
            // Determine DDS Format
            ParsedBitmap.BitmapFormat bmf = DDS.getBitmapFormat(dds);

            // G8B8 same as A8Y8 (found as A8Y8), but adjusted to 128 as center
            if (bmf == ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8 &&
                pb.Properties[0].formatname == ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8)
            {
                for (int ii = 0; ii < guh.Length; ii++)
                {
                    guh[ii] += 128;
                }

                bmf = ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8;
            }

            pbSourceDDS.Image = ParsedBitmap.DecodeBitm(
                guh,
                dds.ddsd.height,
                dds.ddsd.width,
                dds.ddsd.depth,
                dds.ddsd.ddfPixelFormat.RGBBitCount,
                bitmapType,
                bmf,
                false,
                null,
                -1,
                -1);

            br.Close();
            fs.Close();

            #region Fill the source info box with the DDS information
            this.lbSourceDDS.Items.Add("Aspect : " + dds.ddsd.width + "x" + dds.ddsd.height);
            int bpp = 32;
            string format = string.Empty;
            switch (dds.ddsd.ddfPixelFormat.FourCC)
            {
                case "DXT1":
                    format = "DXT1";
                    break;
                case "DXT2":
                case "DXT3":
                    format = "DXT2AND3";
                    break;
                case "DXT4":
                case "DXT5":
                    format = "DXT4AND5";
                    break;
                default:
                    bpp = dds.ddsd.ddfPixelFormat.RGBBitCount;
                    int aCount = 0;
                    int rCount = 0;
                    int gCount = 0;
                    int bCount = 0;

                    for (int i = 0; i < bpp; i++)
                    {
                        // # of alpha bits
                        if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask & (1 << i)) != 0)
                        {
                            aCount++;
                        }

                        // # of red bits
                        if ((dds.ddsd.ddfPixelFormat.RBitMask & (1 << i)) != 0)
                        {
                            rCount++;
                        }

                        // # of green bits
                        if ((dds.ddsd.ddfPixelFormat.GBitMask & (1 << i)) != 0)
                        {
                            gCount++;
                        }

                        // # of blue bits
                        if ((dds.ddsd.ddfPixelFormat.BBitMask & (1 << i)) != 0)
                        {
                            bCount++;
                        }
                    }

                    if ((aCount > 0) && ((dds.ddsd.ddfPixelFormat.Flags & 0x03) > 0))
                    {
                        format += "A" + aCount;
                    }
                    else if (bpp - (rCount + gCount + bCount) > 0)
                    {
                        format += "X" + (bpp - (rCount + gCount + bCount));
                    }

                    if (rCount > 0)
                    {
                        format += "R" + rCount;
                    }

                    if (gCount > 0)
                    {
                        format += "G" + gCount;
                    }

                    if (bCount > 0)
                    {
                        format += "B" + bCount;
                    }

                    break;
            }
            lbSourceDDS.Items.Add("BPP    : " + bpp);            
            lbSourceDDS.Items.Add("Type   : " + bitmapType.ToString().Substring(10));
            lbSourceDDS.Items.Add("Format : " + format); // remove "BITMAP_FORMAT_"
            lbSourceDDS.Items.Add("# Mips : " + Math.Max(dds.ddsd.MipMapCount - 1, 0));
            #endregion

        }

        #region Methods
        /// <summary>
        /// The temp button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void btnInject_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            ParsedBitmap.BitmapInfo bi = DDS_Convert.convertDDStoBitmInfo(this.dds);
            DDS.InjectDDS(map.SelectedMeta, bitm, ref br, lbInjectionBitmap.SelectedIndex, bi);
            br.Close();
            fs.Close();
            this.Close();
        }

        /// <summary>
        /// Occurse when the selected bitmap injection location is changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void lbInjectionBitmap_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbInjectionBitmap.SelectedIndex;

            lbDestBitmap.Items.Clear();
            lbDestBitmap.Items.Add("Aspect : " + this.bitm.Properties[index].width + "x" + this.bitm.Properties[index].height);
            lbDestBitmap.Items.Add("BPP    : " + this.bitm.Properties[index].bitsPerPixel);
            lbDestBitmap.Items.Add("Type   : " + this.bitm.Properties[index].typename.ToString().Substring(10));
            lbDestBitmap.Items.Add("Format : " + this.bitm.Properties[index].formatname.ToString().Substring(12));

            // remove "BITMAP_FORMAT_"
            lbDestBitmap.Items.Add("# Mips : " + this.bitm.Properties[index].mipMapCount);

            Bitmap bitm = this.bitm.FindChunkAndDecode(index, 0, 0, ref map.SelectedMeta, map, 0, 0);
            pbDestBitmap.Image = bitm;
        }
        #endregion

    }
}
