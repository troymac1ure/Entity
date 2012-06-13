// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DDS_Convert.cs" company="">
//   
// </copyright>
// <summary>
//   The dd s_ convert.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.DDSFunctions
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using HaloMap.RawData;

    /// <summary>
    /// The dd s_ convert.
    /// </summary>
    /// <remarks></remarks>
    public class DDS_Convert
    {
        #region Public Methods

        /// <summary>
        /// The decode dds.
        /// </summary>
        /// <param name="sourceBytes">The source bytes.</param>
        /// <param name="b2">The b 2.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Bitmap DecodeDDS(byte[] sourceBytes, ParsedBitmap.BitmapInfo b2)
        {
            ////
            //// Check type name and handle 2D, 3D & Cubemaps
            ////
            // b2.typename;
            ////
            byte[] poo = new byte[0];
            IntPtr ptr = new IntPtr();

            PixelFormat o = new PixelFormat();
            int poolength;
            byte[] fu;
            DecodeDXT decode = new DecodeDXT();

            int oldwidth = b2.width;

            if (b2.width % 16 != 0)
                b2.width = (ushort)(b2.width + (16 - (b2.width % 16)));

            int stride = b2.width;

            if (b2.swizzle)
            {
                sourceBytes = Swizzler.Swizzle(sourceBytes, b2.width, b2.height, b2.depth, b2.bitsPerPixel, true);
            }

            switch (b2.formatname)
            {
                    #region DXT1

                case (ParsedBitmap.BitmapFormat)14:
                    if (b2.swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    sourceBytes = decode.DecodeDXT1(b2.height, b2.width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;

                    #endregion

                    #region DXT2/3

                case (ParsedBitmap.BitmapFormat)15:
                    if (b2.swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    sourceBytes = decode.DecodeDXT23(b2.height, b2.width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;

                    #endregion

                    #region DXT 4/5

                case (ParsedBitmap.BitmapFormat)16:
                    if (b2.swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    sourceBytes = decode.DecodeDXT45(b2.height, b2.width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;

                    #endregion

                    #region A8R8G8B8

                case (ParsedBitmap.BitmapFormat)11:
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;

                    #endregion

                    #region X8R8G8B8

                case (ParsedBitmap.BitmapFormat)10:
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;

                    #endregion

                    #region // 16 bit \\

                    #region A4R4G4B4

                case (ParsedBitmap.BitmapFormat)9:
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 2];
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        fu[r * 2 + 0] = (byte)((sourceBytes[r + 1] & 0xFF) >> 0); // Blue
                        fu[r * 2 + 1] = (byte)((sourceBytes[r + 0] & 0xFF) >> 0); // Green
                        fu[r * 2 + 2] = (byte)((sourceBytes[r + 0] & 0xFF) >> 0); // Red
                        fu[r * 2 + 3] = 255; // (byte)(((sourceBytes[r + 1] & 0xFF) >> 0));        // Alpha
                    }

                    sourceBytes = fu;
                    break;

                    #endregion

                    #region G8B8

                case (ParsedBitmap.BitmapFormat)22:
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength / 2 * 4];

                    // These are actually signed (+/-128), so convert to unsigned
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        fu[r * 2 + 0] = (byte)(sourceBytes[r + 1] + 128); // Blue
                        fu[r * 2 + 1] = (byte)(sourceBytes[r + 1] + 128); // Green
                        fu[r * 2 + 2] = (byte)(sourceBytes[r + 0] + 128); // Red
                        fu[r * 2 + 3] = (byte)(sourceBytes[r + 0] + 128); // Alpha
                    }

                    sourceBytes = fu;
                    break;

                    #endregion

                    #region A1R5G5B5

                case (ParsedBitmap.BitmapFormat)8:
                    stride *= 2;
                    o = PixelFormat.Format16bppRgb555;
                    break;

                    #endregion

                    #region R5G6B5

                case (ParsedBitmap.BitmapFormat)6:
                    stride *= 2;
                    o = PixelFormat.Format16bppRgb565;
                    break;

                    #endregion

                    #region A8Y8

                case (ParsedBitmap.BitmapFormat)3:
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength / 2 * 4];
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        fu[r * 2 + 0] = sourceBytes[r + 1];
                        fu[r * 2 + 1] = sourceBytes[r + 1];
                        fu[r * 2 + 2] = sourceBytes[r + 1];
                        fu[r * 2 + 3] = sourceBytes[r + 0];
                    }

                    sourceBytes = fu;
                    stride *= 4;
                    break;

                    #endregion

                    #endregion

                    #region // 8 bit \\

                    #region P8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        fu[r + 0] = sourceBytes[e];
                        fu[r + 1] = sourceBytes[e];
                        fu[r + 2] = sourceBytes[e];
                        fu[r + 3] = 255;
                    }

                    sourceBytes = fu;
                    stride *= 4;
                    break;

                    #endregion

                    #region A8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        fu[r + 0] = sourceBytes[e];
                        fu[r + 1] = sourceBytes[e];
                        fu[r + 2] = sourceBytes[e];
                        fu[r + 3] = 255;
                    }

                    sourceBytes = fu;
                    stride *= 4;
                    break;

                    #endregion

                    #region AY8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_AY8:
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;

                        /*
                        fu[r + 0] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15 + 128);
                        fu[r + 1] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15 + 128);
                        fu[r + 2] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15 + 128);
                        */
                        fu[r + 0] = (byte)(((sourceBytes[e] & 0xF0) >> 4) * 255 / 15);
                        fu[r + 1] = (byte)(((sourceBytes[e] & 0xF0) >> 4) * 255 / 15);
                        fu[r + 2] = (byte)(((sourceBytes[e] & 0xF0) >> 4) * 255 / 15);
                        fu[r + 3] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15);

                        /*
                        fu[r + 0] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15);
                        fu[r + 1] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15);
                        fu[r + 2] = (byte)((sourceBytes[e] & 0x0F) * 255 / 15);
                        fu[r + 3] = (byte)(((sourceBytes[e] & 0xF0) >> 3) * 255 / 15);
                        if (sourceBytes[e] == 0)
                            fu[r + 3] = 0;
                        else
                            fu[r + 3] = 255;
                        */
                    }

                    sourceBytes = fu;
                    stride *= 4;
                    break;

                    #endregion

                    #region Y8

                case (ParsedBitmap.BitmapFormat)1:
                    o = PixelFormat.Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        fu[r + 0] = sourceBytes[e];
                        fu[r + 1] = sourceBytes[e];
                        fu[r + 2] = sourceBytes[e];
                        fu[r + 3] = 255;
                    }

                    sourceBytes = fu;
                    stride *= 4;
                    break;

                    #endregion

                    /*
                #region LightMap
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_LIGHTMAP:
                    o = PixelFormat.Format32bppArgb;// Format32bppArgb;
                    poolength = sourceBytes.Length;
                    fu = new byte[poolength * 4];
                    int bspnumber = ident;
                    int paletteindex = -1;

                    if (visualchunkindex < 0)
                    {
                        int wtf = 0 - (visualchunkindex + 1);
                        paletteindex = map.BSP.sbsp[bspnumber].SceneryChunk_LightMap_Index[wtf];
                    }
                    if (paletteindex == -1)
                        for (int i = 0; i < map.BSP.sbsp[bspnumber].VisualChunk_Bitmap_Index.Length; i++)
                            if (map.BSP.sbsp[bspnumber].VisualChunk_Bitmap_Index[i] == visualchunkindex)
                            {
                                paletteindex = map.BSP.sbsp[bspnumber].VisualChunk_LightMap_Index[visualchunkindex];
                                break;
                            }

                    if (paletteindex == -1)
                        for (int i = 0; i < map.BSP.sbsp[bspnumber].SceneryChunk_Bitmap_Index.Length; i++)
                            if (map.BSP.sbsp[bspnumber].SceneryChunk_Bitmap_Index[i] == visualchunkindex)
                            {
                                paletteindex = map.BSP.sbsp[bspnumber].SceneryChunk_LightMap_Index[i];
                                break;
                            }

                    if (paletteindex == 255) return null;
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        fu[r + 0] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][sourceBytes[e]].r;
                        fu[r + 1] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][sourceBytes[e]].g;
                        fu[r + 2] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][sourceBytes[e]].b;
                        fu[r + 3] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][sourceBytes[e]].a;

                    }
                    sourceBytes = fu;
                    stride *= 4;
                    break;
                #endregion
                */
                    #endregion

                default:
                    return null;
            }

            Marshal.FreeHGlobal(ptr);
            ptr = Marshal.AllocHGlobal(sourceBytes.Length);

            RtlMoveMemory(ptr, sourceBytes, sourceBytes.Length);

            return new Bitmap(b2.width, b2.height, stride, o, ptr);
        }

        /// <summary>
        /// The encode dds.
        /// </summary>
        /// <param name="bitm">The bitm.</param>
        /// <param name="b2">The b 2.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] EncodeDDS(Bitmap bitm, ref ParsedBitmap.BitmapInfo b2)
        {
            ////
            //// Check type name and handle 2D, 3D & Cubemaps
            ////
            // b2.typename;
            ////

            // Get bitmap data into array
            BitmapData bmd = bitm.LockBits(
                new Rectangle(new Point(0, 0), bitm.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmd.Scan0;
            b2.width = (ushort)bitm.Width;
            b2.height = (ushort)bitm.Height;
            b2.depth = 1;
            b2.mipMapCount = 0;
            b2.pixelOffset = 0;
            b2.tagtype = "mtib".ToCharArray();
            b2.typename = ParsedBitmap.BitmapType.BITM_TYPE_2D;

            int stride = bmd.Stride;

            // Declare an array to hold the bytes of the bitmap.
            int bitmSize = stride * bitm.Height;
            byte[] bitmData = new byte[bitmSize];
            Marshal.Copy(ptr, bitmData, 0, bitmSize);
            bitm.UnlockBits(bmd);

            byte[] fu;

            // Misc.DecodeDXT decode = new Entity.Misc.DecodeDXT();

            switch (b2.formatname)
            {
                    /*
                #region DXT1
                case (ParsedBitmap.BitmapFormat)14:
                    sourceBytes = decode.DecodeDXT1(bitm.Height, bitm.Width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;
                #endregion
                #region DXT2/3
                case (ParsedBitmap.BitmapFormat)15:
                    sourceBytes = decode.DecodeDXT23(bitm.Height, bitm.Width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;
                #endregion
                #region DXT 4/5
                case (ParsedBitmap.BitmapFormat)16:
                    sourceBytes = decode.DecodeDXT45(b2.height, bitm.Width, sourceBytes);
                    stride *= 4;
                    o = PixelFormat.Format32bppArgb;
                    break;
                #endregion
                */
                    #region A8R8G8B8

                case (ParsedBitmap.BitmapFormat)11:
                    b2.bitsPerPixel = 32;
                    break;

                    #endregion

                    #region Lightmaps

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_LIGHTMAP:
                    b2.bitsPerPixel = 32;
                    break;

                    #endregion

                    #region X8R8G8B8

                case (ParsedBitmap.BitmapFormat)10:
                    b2.bitsPerPixel = 32;
                    break;

                    #endregion

                    #region // 16 bit \\

                    #region A1R5G5B5

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5:
                    b2.bitsPerPixel = 16;

                    fu = new byte[bitmSize / 2];
                    for (int r = 0; r < fu.Length; r += 2)
                    {
                        ushort temp = 0;
                        temp += (ushort)((bitmData[r * 2 + 0] * 0x1F / 255) << 0); // 5-bit Blue
                        temp += (ushort)((bitmData[r * 2 + 1] * 0x1F / 255) << 5); // 5-bit Green
                        temp += (ushort)((bitmData[r * 2 + 2] * 0x1F / 255) << 10); // 5-bit Red
                        temp += (ushort)((bitmData[r * 2 + 3] / 255) << 15); // 1-bit Alpha
                        fu[r + 0] = (byte)(temp & 0xFF);
                        fu[r + 1] = (byte)((temp >> 8) & 0xFF);
                    }

                    bitmData = fu;
                    stride /= 2;
                    break;

                    #endregion

                    #region A4R4G4B4

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4:
                    b2.bitsPerPixel = 16;

                    fu = new byte[bitmSize / 2];
                    for (int r = 0; r < fu.Length; r += 2)
                    {
                        fu[r + 1] = (byte)((bitmData[r * 2 + 0] * 15 / 255) << 4);
                            
                            // Take blue channel and store as 4-bit B
                        fu[r + 1] += (byte)(bitmData[r * 2 + 1] * 15 / 255); // Take green channel and store as 4-bit G
                        fu[r + 0] = (byte)((bitmData[r * 2 + 2] * 15 / 255) << 4);
                            
                            // Take red channel and store as 4-bit R
                        fu[r + 0] += (byte)(bitmData[r * 2 + 3] * 15 / 255); // Take alpha channel and store as 4-bit A
                    }

                    bitmData = fu;
                    stride /= 2;
                    break;

                    #endregion

                    #region A8Y8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8:
                    b2.bitsPerPixel = 16;

                    fu = new byte[bitmSize / 2];
                    for (int r = 0; r < fu.Length; r += 2)
                    {
                        fu[r + 0] = bitmData[r * 2 + 3]; // Store Alpha channel as (sbyte) G8
                        fu[r + 1] = bitmData[r * 2 + 0]; // Store Blue channel as (sbyte) B8
                    }

                    bitmData = fu;
                    stride /= 2;
                    break;

                    #endregion

                    #region G8B8

                case (ParsedBitmap.BitmapFormat)22:
                    b2.bitsPerPixel = 16;

                    fu = new byte[bitmSize / 2];
                    for (int r = 0; r < fu.Length; r += 2)
                    {
                        fu[r + 0] = (byte)(bitmData[r * 2 + 1] + 128); // Store Green channel as (sbyte) G8
                        fu[r + 1] = (byte)(bitmData[r * 2 + 2] + 128); // Store Blue channel as (sbyte) B8
                    }

                    bitmData = fu;
                    stride /= 2;
                    break;

                    #endregion

                    #region R5G6B5

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_R5G6B5:
                    b2.bitsPerPixel = 16;

                    fu = new byte[bitmSize / 2];
                    for (int r = 0; r < fu.Length; r += 2)
                    {
                        ushort temp = 0;
                        temp += (ushort)(bitmData[r * 2 + 0] * 0x1F / 255); // 5-bit Red
                        temp += (ushort)((bitmData[r * 2 + 1] * 0x3F / 255) << 5); // 5-bit Green
                        temp += (ushort)((bitmData[r * 2 + 2] * 0x1F / 255) << 11); // 5-bit Blue
                        fu[r + 0] = (byte)(temp & 0xFF);
                        fu[r + 1] = (byte)((temp >> 8) & 0xFF);
                    }

                    bitmData = fu;
                    stride /= 2;
                    break;

                    #endregion

                    #endregion

                    #region // 8 bit \\

                    #region AY8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_AY8:
                    b2.bitsPerPixel = 8;

                    fu = new byte[bitmSize / 4];
                    for (int r = 0; r < fu.Length; r++)
                    {
                        fu[r] = (byte)((bitmData[r * 4 + 0] * 0x0F / 255) << 4);
                            
                            // Take red channel and store as 4-bit Y
                        fu[r] += (byte)(bitmData[r * 4 + 3] * 0x0F / 255); // Take alpha channel and store as 4-bit A
                    }

                    bitmData = fu;
                    stride /= 4;
                    break;

                    #endregion

                    #region A8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                    b2.bitsPerPixel = 8;

                    fu = new byte[bitmSize / 4];
                    for (int r = 0; r < fu.Length; r++)
                    {
                        fu[r] = bitmData[r * 4]; // Take red channel only and store as Y value
                    }

                    bitmData = fu;
                    stride /= 4;
                    break;

                    #endregion

                    #region P8

                case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                    b2.bitsPerPixel = 8;

                    fu = new byte[bitmSize / 4];
                    for (int r = 0; r < fu.Length; r++)
                    {
                        fu[r] = bitmData[r * 4]; // Take red channel only and store as Y value
                    }

                    bitmData = fu;
                    stride /= 4;
                    break;

                    #endregion

                    #region Y8

                case (ParsedBitmap.BitmapFormat)1:
                    b2.bitsPerPixel = 8;

                    fu = new byte[bitmSize / 4];
                    for (int r = 0; r < fu.Length; r++)
                    {
                        fu[r] = bitmData[r * 4]; // Take red channel only and store as Y value
                    }

                    bitmData = fu;
                    stride /= 4;
                    break;

                    #endregion

                    #endregion
                default:
                    MessageBox.Show(b2.formatname.ToString());
                    break;
            }

            b2.type = (ushort)b2.typename;

            if (b2.swizzle)
            {
                bitmData = Swizzler.Swizzle(bitmData, bitm.Width, bitm.Height, b2.depth, b2.bitsPerPixel, false);
            }

            return bitmData;
        }

        /// <summary>
        /// The convert dd sto bitm info.
        /// </summary>
        /// <param name="dds">The dds.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ParsedBitmap.BitmapInfo convertDDStoBitmInfo(DDS.DDS_HEADER_STRUCTURE dds)
        {
            ParsedBitmap.BitmapInfo bi = new ParsedBitmap.BitmapInfo(DDS.getBitmapFormat(dds), false);
            bi.width = (ushort)dds.ddsd.width;
            bi.height = (ushort)dds.ddsd.height;
            bi.depth = (ushort)dds.ddsd.depth;
            bi.mipMapCount = (ushort)dds.ddsd.MipMapCount;
            bi.bitsPerPixel = (ushort)dds.ddsd.ddfPixelFormat.RGBBitCount;
            bi.format = (ushort)bi.formatname;
            bi.type = (ushort)bi.typename;
            bi.tagtype = "mtib".ToCharArray();
            bi.flags = (ushort)dds.ddsd.flags;
            return bi;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The rtl move memory.
        /// </summary>
        /// <param name="src">The src.</param>
        /// <param name="crap">The crap.</param>
        /// <param name="cb">The cb.</param>
        /// <remarks></remarks>
        [DllImport("kernel32.dll")]
        private static extern void RtlMoveMemory(IntPtr src, byte[] crap, int cb);

        #endregion
    }
}