// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DDS.cs" company="">
//   
// </copyright>
// <summary>
//   The dds.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.DDSFunctions
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    /// <summary>
    /// The dds.
    /// </summary>
    /// <remarks></remarks>
    public class DDS
    {
        #region Enums

        /// <summary>
        /// The dds file formats.
        /// </summary>
        /// <remarks></remarks>
        public enum DDSFileFormats
        {
            /// <summary>
            /// The dx t 1.
            /// </summary>
            DXT1, // _4bpp_DXT1_ARGB_1BitAlpha,
            /// <summary>
            /// The dx t 3.
            /// </summary>
            DXT3, // _8bpp_DXT3_ARGB_ExplicitAlpha,
            /// <summary>
            /// The dx t 5.
            /// </summary>
            DXT5, // _8bpp_DXT5_ARGB_InterpolatedAlpha,
            /// <summary>
            /// The alpha_8_bit.
            /// </summary>
            Alpha_8_bit, // _8bpp_8_A_alpha,
            /// <summary>
            /// The a 1 r 5 g 5 b 5_16_bit.
            /// </summary>
            A1R5G5B5_16_bit, // _16bpp_1555_ARGB_Unsigned,
            /// <summary>
            /// The a 4 r 4 g 4 b 4_16_bit.
            /// </summary>
            A4R4G4B4_16_bit, // _16bpp_444_ARGB_Unsigned,
            /// <summary>
            /// The r 5 g 6 b 5_16_bit.
            /// </summary>
            R5G6B5_16_bit, // _16bpp_565_RGB_Unsigned,
            /// <summary>
            /// The alpha luminance_16_bit.
            /// </summary>
            AlphaLuminance_16_bit, // _16bpp_88_AL_AlphaLuminance,
            /// <summary>
            /// The x 8 r 8 g 8 b 8_32_bit.
            /// </summary>
            X8R8G8B8_32_bit, // _32bpp_8888_XRGB_Unsigned,
            /// <summary>
            /// The a 8 r 8 g 8 b 8_32_bit.
            /// </summary>
            A8R8G8B8_32_bit, // _32bpp_8888_ARGB_Unsigned,
            /// <summary>
            /// The bit m_ forma t_ lightmap.
            /// </summary>
            BITM_FORMAT_LIGHTMAP,
        }

        /// <summary>
        /// The dds file type.
        /// </summary>
        /// <remarks></remarks>
        public enum DDSFileType
        {
            /// <summary>
            /// The _2 d.
            /// </summary>
            _2D,

            /// <summary>
            /// The _3 d.
            /// </summary>
            _3D,

            /// <summary>
            /// The _ cube map.
            /// </summary>
            _CubeMap
        }

        /// <summary>
        /// The h 2 dds formats.
        /// </summary>
        /// <remarks></remarks>
        public enum H2DDSFormats
        {
            /// <summary>
            /// The a 8.
            /// </summary>
            A8 = DDSFileFormats.Alpha_8_bit, // ._8bpp_8_A_alpha,
            /// <summary>
            /// The y 8.
            /// </summary>
            Y8 = DDSFileFormats.Alpha_8_bit, // ._8bpp_8_A_alpha,
            /// <summary>
            /// The p 8.
            /// </summary>
            P8 = DDSFileFormats.Alpha_8_bit, // ._8bpp_8_A_alpha,
            /// <summary>
            /// The a y 8.
            /// </summary>
            AY8 = DDSFileFormats.A1R5G5B5_16_bit, // ._16bpp_1555_ARGB_Unsigned,
            /// <summary>
            /// The a 8 y 8.
            /// </summary>
            A8Y8 = DDSFileFormats.AlphaLuminance_16_bit, // ._16bpp_88_AL_AlphaLuminance,
            /// <summary>
            /// The r 5 g 6 b 5.
            /// </summary>
            R5G6B5 = DDSFileFormats.R5G6B5_16_bit, // _16bpp_565_RGB_Unsigned,
            /// <summary>
            /// The a 1 r 5 g 5 b 5.
            /// </summary>
            A1R5G5B5 = DDSFileFormats.A1R5G5B5_16_bit, // ._16bpp_1555_ARGB_Unsigned,
            /// <summary>
            /// The a 4 r 4 g 4 b 4.
            /// </summary>
            A4R4G4B4 = DDSFileFormats.A4R4G4B4_16_bit, // ._16bpp_444_ARGB_Unsigned,
            /// <summary>
            /// The x 8 r 8 g 8 b 8.
            /// </summary>
            X8R8G8B8 = DDSFileFormats.X8R8G8B8_32_bit, // ._32bpp_8888_XRGB_Unsigned,
            /// <summary>
            /// The a 8 r 8 g 8 b 8.
            /// </summary>
            A8R8G8B8 = DDSFileFormats.A8R8G8B8_32_bit, // ._32bpp_8888_ARGB_Unsigned,
            /// <summary>
            /// The dx t 1.
            /// </summary>
            DXT1 = DDSFileFormats.DXT1, // ._4bpp_DXT1_ARGB_1BitAlpha,
            /// <summary>
            /// The dx t 3.
            /// </summary>
            DXT3 = DDSFileFormats.DXT3, // _8bpp_DXT3_ARGB_ExplicitAlpha,
            /// <summary>
            /// The dx t 5.
            /// </summary>
            DXT5 = DDSFileFormats.DXT5, // _8bpp_DXT5_ARGB_InterpolatedAlpha,
            /// <summary>
            /// The lightmap.
            /// </summary>
            Lightmap = DDSFileFormats.BITM_FORMAT_LIGHTMAP,

            /// <summary>
            /// The g 8 b 8.
            /// </summary>
            G8B8 = DDSFileFormats.AlphaLuminance_16_bit // _16bpp_88_AL_AlphaLuminance
        }

        // DDS
        // The dwFlags member of the modified DDSURFACEDESC2 structure can be set to one or more of the following values.
        /// <summary>
        /// The dds enum.
        /// </summary>
        /// <remarks></remarks>
        public enum DDSEnum
        {
            /// <summary>
            /// The dds d_ caps.
            /// </summary>
            DDSD_CAPS = 0x1,

            /// <summary>
            /// The dds d_ height.
            /// </summary>
            DDSD_HEIGHT = 0x2,

            /// <summary>
            /// The dds d_ width.
            /// </summary>
            DDSD_WIDTH = 0x4,

            /// <summary>
            /// The dds d_ pitch.
            /// </summary>
            DDSD_PITCH = 0x8,

            /// <summary>
            /// The dds d_ pixelformat.
            /// </summary>
            DDSD_PIXELFORMAT = 0x1000,

            /// <summary>
            /// The dds d_ mipmapcount.
            /// </summary>
            DDSD_MIPMAPCOUNT = 0x20000,

            /// <summary>
            /// The dds d_ linearsize.
            /// </summary>
            DDSD_LINEARSIZE = 0x80000,

            /// <summary>
            /// The dds d_ depth.
            /// </summary>
            DDSD_DEPTH = 0x800000,

            // dwFlags
            /// <summary>
            /// The ddp f_ alphapixels.
            /// </summary>
            DDPF_ALPHAPIXELS = 0x00000001, // The surface has alpha channel information in the pixel format.
            /// <summary>
            /// The ddp f_ alpha.
            /// </summary>
            DDPF_ALPHA = 0x00000002, // The pixel format contains alpha only information
            /// <summary>
            /// The ddp f_ fourcc.
            /// </summary>
            DDPF_FOURCC = 0x00000004, // The FourCC code is valid.
            /// <summary>
            /// The ddp f_ paletteindexe d 4.
            /// </summary>
            DDPF_PALETTEINDEXED4 = 0x00000008, // The surface is 4-bit color indexed.
            /// <summary>
            /// The ddp f_ paletteindexedt o 8.
            /// </summary>
            DDPF_PALETTEINDEXEDTO8 = 0x00000010,

            // The surface is indexed into a palette which stores indices into the destination surface's 8-bit palette.
            /// <summary>
            /// The ddp f_ paletteindexe d 8.
            /// </summary>
            DDPF_PALETTEINDEXED8 = 0x00000020, // The surface is 8-bit color indexed.
            /// <summary>
            /// The ddp f_ rgb.
            /// </summary>
            DDPF_RGB = 0x00000040, // The RGB data in the pixel format structure is valid.
            /// <summary>
            /// The ddp f_ compressed.
            /// </summary>
            DDPF_COMPRESSED = 0x00000080,

            // The surface will accept pixel data in the format specified and compress it during the write.
            /// <summary>
            /// The ddp f_ rgbtoyuv.
            /// </summary>
            DDPF_RGBTOYUV = 0x00000100,

            // The surface will accept RGB data and translate it during the write to YUV data.
            // The format of the data to be written will be contained in the pixel format structure.  The DDPF_RGB flag will be set.
            /// <summary>
            /// The ddp f_ yuv.
            /// </summary>
            DDPF_YUV = 0x00000200, // pixel format is YUV - YUV data in pixel format struct is valid
            /// <summary>
            /// The ddp f_ zbuffer.
            /// </summary>
            DDPF_ZBUFFER = 0x00000400, // pixel format is a z buffer only surface
            /// <summary>
            /// The ddp f_ paletteindexe d 1.
            /// </summary>
            DDPF_PALETTEINDEXED1 = 0x00000800, // The surface is 1-bit color indexed.
            /// <summary>
            /// The ddp f_ paletteindexe d 2.
            /// </summary>
            DDPF_PALETTEINDEXED2 = 0x00001000, // The surface is 2-bit color indexed.
            /// <summary>
            /// The ddp f_ zpixels.
            /// </summary>
            DDPF_ZPIXELS = 0x00002000, // The surface contains Z information in the pixels

            // 'The dwCaps1 member of the DDSCAPS2 structure can be set to one or more of the following values.
            /// <summary>
            /// The ddscap s_ complex.
            /// </summary>
            DDSCAPS_COMPLEX = 0x8,

            /// <summary>
            /// The ddscap s_ texture.
            /// </summary>
            DDSCAPS_TEXTURE = 0x1000,

            /// <summary>
            /// The ddscap s_ mipmap.
            /// </summary>
            DDSCAPS_MIPMAP = 0x400000,

            // The dwCaps2 member of the DDSCAPS2 structure can be set to one or more of the following values.
            /// <summary>
            /// The ddscap s 2_ cubemap.
            /// </summary>
            DDSCAPS2_CUBEMAP = 0x200,

            /// <summary>
            /// The ddscap s 2_ cubema p_ positivex.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,

            /// <summary>
            /// The ddscap s 2_ cubema p_ negativex.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,

            /// <summary>
            /// The ddscap s 2_ cubema p_ positivey.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,

            /// <summary>
            /// The ddscap s 2_ cubema p_ negativey.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,

            /// <summary>
            /// The ddscap s 2_ cubema p_ positivez.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,

            /// <summary>
            /// The ddscap s 2_ cubema p_ negativez.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,

            /// <summary>
            /// The ddscap s 2_ volume.
            /// </summary>
            DDSCAPS2_VOLUME = 0x200000
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts Meta Data to a dds file.
        /// </summary>
        /// <param name="m">Bitmap Meta</param>
        /// <param name="pm">ParsedBitmap Data</param>
        /// <param name="bw">Binary Writer Stream</param>
        /// <param name="index">The index # of the ParsedBitmap Data</param>
        /// <returns>BitmapInfo containing the format of the extracted DDS</returns>
        /// <remarks></remarks>
        public static ParsedBitmap.BitmapInfo ExtractDDS(Meta m, ParsedBitmap pm, ref BinaryWriter bw, int index)
        {
            return ExtractDDS(m, pm, ref bw, index, pm.Properties[index]);
        }

        /// <summary>
        /// Extracts meta data to a dds stream.
        /// </summary>
        /// <param name="m">Bitmap Meta</param>
        /// <param name="pm">ParsedBitmap Data</param>
        /// <param name="bw">Binary Writer Stream</param>
        /// <param name="index">The index # of the ParsedBitmap Data</param>
        /// <param name="bi">BitmapInfo Data</param>
        /// <returns>BitmapInfo containing the format of the extracted DDS</returns>
        /// <remarks></remarks>
        public static ParsedBitmap.BitmapInfo ExtractDDS(
            Meta m, ParsedBitmap pm, ref BinaryWriter bw, int index, ParsedBitmap.BitmapInfo bi)
        {
            DDS_HEADER_STRUCTURE dds = new DDS_HEADER_STRUCTURE();
            ParsedBitmap.BitmapInfo pmProp = pm.Properties[index];
            dds.generate(ref pmProp);

            // Get our raw data from the Meta, LOD 0 of our selected index
            int tempNum = -1;
            for (int i = 0; i < m.raw.rawChunks.Count; i++)
                if (((BitmapRawDataChunk)m.raw.rawChunks[i]).inchunk == index)
                {
                    tempNum = i;
                    break;
                }
            if (tempNum == -1)
                return null;


            byte[] tempChunk = m.raw.rawChunks[tempNum].MS.ToArray();
            //int bytesToWrite = 0;
            int inOffset = 0;
            
            // If the original size needs padding, then use it, but not otherwise
            bool useWidthPad = pmProp.width % 16 != 0 ? true : false;

            bw.BaseStream.SetLength(128);
            bw.BaseStream.Position = 0;

            for (int cubemap = 0; cubemap < (pmProp.typename == ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP ? 6 : 1); cubemap++)
            {
                inOffset = cubemap * (tempChunk.Length / 6);
                int outOffset = 0;
                int tWidth = pmProp.width;
                int tHeight = pmProp.height;
                int tDepth = pmProp.depth;
                int mipMapCount = 0;

                // Halo 2 only uses mipmaps down to 2x__, instead of 1x1 (could be 2x1, 2x2, 2x4, etc),
                // but is padded enough we should be able to get a 1x1 extracted from the padded data
                while (tWidth != 0 && tHeight != 0)
                {

                    int mipSize;

                    // Any compressed images must have each mimap at least 16 bytes (4x4 image)
                    // This concerns mipmaps such as 2x4, 2x2, 1x1, etc
                    switch (pmProp.formatname)
                    {
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1:
                            mipSize = Math.Max(Math.Max(1, tWidth) * Math.Max(1, tHeight) * Math.Max(1, tDepth) * (pmProp.bitsPerPixel >> 3) / 8, 8);
                            //mipSize = Math.Max(1, tWidth / 4) * Math.Max(1, tHeight / 4) * 8;
                            if (dds.ddsd.PitchOrLinearSize == 0)
                                dds.ddsd.PitchOrLinearSize = mipSize;
                            //dds.ddsd.PitchOrLinearSize = Math.Max(1, ((tWidth + 3) / 4)) * 8;
                            break;
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3:
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5:
                            mipSize = Math.Max(Math.Max(1, tWidth) * Math.Max(1, tHeight) * Math.Max(1, tDepth), 16) * (pmProp.bitsPerPixel >> 3) / 4;
                            //mipSize = Math.Max(1, tWidth / 4) * Math.Max(1, tHeight / 4) * 16;
                            if (dds.ddsd.PitchOrLinearSize == 0)
                                dds.ddsd.PitchOrLinearSize = mipSize;
                            break;
                        default:
                            mipSize = Math.Max(Math.Max(1, tWidth) * Math.Max(1, tHeight) * Math.Max(1, tDepth), 8) * (pmProp.bitsPerPixel >> 3);
                            break;
                    }
                    
                    #region Decode swizzled images
                    if (pmProp.swizzle)
                    {
                        try
                        {
                            byte[] tempSwizzle = new byte[mipSize];
                            Array.Copy(tempChunk, inOffset + outOffset, tempSwizzle, 0, tempSwizzle.Length);
                            tempSwizzle = Swizzler.Swizzle(tempSwizzle, tWidth, tHeight, tDepth > 1 ? tDepth : -1, pmProp.bitsPerPixel, true);
                            Array.Copy(tempSwizzle, 0, tempChunk, inOffset + outOffset, tempSwizzle.Length);
                        }
                        catch
                        {
                            // We cannot generate all mipmaps, so just continue with the amount we have
                            break;
                        }
                    }
                    #endregion

                    // H2 Bitmap Data is padded to a minimum of:
                    //      1 BPP/DXT2/3/4/5    = 128 bytes
                    //      2 BPP/DXT1          = 256 bytes
                    //      4 BPP               = 512 bytes
                    // We need to remove the extra padding when 
                    // extracting to a DDS. DDS pads DXT1 to 8 bytes minimum & DXT2/3/4/5 to 16 bytes minimum
                    // bytesToWrite is the width * height size. tempChunk2 will contain a stripped version
                    #region Remove padding

                    int widthPad = 0;
                    //if (pmProp.typename != ParsedBitmap.BitmapType.BITM_TYPE_3D && tWidth % 16 != 0)
                    if (useWidthPad && tWidth % 16 != 0)
                        widthPad = 16 - tWidth % 16;

                    if (!pmProp.formatname.ToString().Contains("DXT") && widthPad != 0)
                    {
                        //byte[] tempChunk2 = new byte[tWidth * tHeight * tDepth * byteStep];
                        byte[] tempChunk2 = new byte[mipSize];
                        try
                        {
                            // tempPadSize = Padded scanline length
                            int tempPadSize = (tWidth + widthPad) * (pmProp.bitsPerPixel >> 3);
                            int paddedSize = tHeight * tempPadSize;

                            // Copy each line without the padding
                            for (int d = 0; d < tDepth; d++)
                                for (int h = 0; h < tHeight; h++)
                                {
                                    Array.Copy(
                                        tempChunk,
                                        inOffset + outOffset + (d * paddedSize) + (h * tempPadSize),
                                        tempChunk2,
                                        (d * mipSize) + (h * tWidth * (pmProp.bitsPerPixel >> 3)),
                                        tWidth * (pmProp.bitsPerPixel >> 3));
                                }
                            // Make a copy of the bytes beyond our current mipmap
                            paddedSize *= tDepth;
                            byte[] endBytes = new byte[tempChunk.Length - paddedSize - (inOffset + outOffset)];
                            Array.Copy(tempChunk, inOffset + outOffset + paddedSize, endBytes, 0, endBytes.Length);
                            Array.Resize(
                                ref tempChunk,
                                inOffset + outOffset + tempChunk2.Length + endBytes.Length
                                );
                            Array.Copy(tempChunk2, 0, tempChunk, inOffset + outOffset, tempChunk2.Length);
                            Array.Copy(endBytes, 0, tempChunk, inOffset + outOffset + tempChunk2.Length, endBytes.Length);
                        }
                        catch
                        {
                            // We cannot generate all mipmaps, so just continue with the amount we have
                            break;
                        }
                    }
                    #endregion

                    outOffset += mipSize;
                    if ((pmProp.mipMapCount == 0) ||
                       (outOffset > tempChunk.Length))
                        break;
                    tWidth >>= 1;
                    tHeight >>= 1;
                    tDepth = Math.Max(tDepth >> 1, 1);
                    mipMapCount++;
                }
                // For exporting, use mipmaps right down to 1x1, instead of Halo's 2x2
                dds.ddsd.MipMapCount = mipMapCount;

                // G8B8 is based on a 128 value for 0 and adds -/+, so adjust values to 128
                if (pmProp.formatname == ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8)
                {
                    for (int ii = 0; ii < tempChunk.Length; ii++)
                    {
                        tempChunk[ii] += 128;
                    }
                }

                byte[] bChunk = new byte[outOffset];
                Array.Copy(tempChunk, inOffset, bChunk, 0, Math.Min(outOffset, tempChunk.Length));

                if (cubemap == 0)
                {
                    dds.WriteStruct(ref bw);
                    bw.BaseStream.Position = 128;
                }
                bw.Write(bChunk);
            }
            
            return pmProp;
        }

        /// <summary>
        /// Injects a dds stream to Meta Data.
        /// </summary>
        /// <param name="m">Bitmap Meta</param>
        /// <param name="pm">ParsedBitmap Data</param>
        /// <param name="br">Binary Reader Stream</param>
        /// <param name="index">The index # of the ParsedBitmap Data</param>
        /// <remarks></remarks>
        public static void InjectDDS(Meta m, ParsedBitmap pm, ref BinaryReader br, int index)
        {
            InjectDDS(m, pm, ref br, index, pm.Properties[index]);
        }

        /// <summary>
        /// Injects a dds stream to Meta Data.
        /// </summary>
        /// <param name="m">Bitmap Meta</param>
        /// <param name="pm">ParsedBitmap Data</param>
        /// <param name="br">Binary Reader Stream</param>
        /// <param name="index">The index # of the ParsedBitmap Data</param>
        /// <param name="bi">BitmapInfo Data</param>
        /// <remarks></remarks>
        public static void InjectDDS(
            Meta m, ParsedBitmap pm, ref BinaryReader br, int index, ParsedBitmap.BitmapInfo bi)
        {
            DDS_HEADER_STRUCTURE dds = new DDS_HEADER_STRUCTURE();
            ParsedBitmap.BitmapInfo pmProp = pm.Properties[index];

            // ' Read dds header
            dds.ReadStruct(ref br);

            // Check padded values as we can allow bitmaps of different size if they have the same padding
            int pmPropPaddedWidth = pmProp.width + (pmProp.width % 16 == 0 ? 0 : (16 - pmProp.width % 16));
            int ddsPaddedWidth = dds.ddsd.width + (dds.ddsd.width % 16 == 0 ? 0 : (16 - dds.ddsd.width % 16));
            if (ddsPaddedWidth != pmPropPaddedWidth || pmProp.height != dds.ddsd.height)
            {
                MessageBox.Show("ERROR (Bitmap #" + index + "): Images have different padded dimensions!");
                return;
            }
            // Set new image width
            pmProp.width = (ushort)dds.ddsd.width;

            #region Discover if we are doing a bitmap, 3D or cubemap
            ParsedBitmap.BitmapType type = ParsedBitmap.BitmapType.BITM_TYPE_2D;
            if ((dds.ddsd.ddsCaps.caps2 & (int)DDSEnum.DDSCAPS2_VOLUME) > 0)
                type = ParsedBitmap.BitmapType.BITM_TYPE_3D;
            else if ((dds.ddsd.ddsCaps.caps2 & (int)DDSEnum.DDSCAPS2_CUBEMAP) > 0)
                type = ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP;
            #endregion

            // If they are different types
            if (pmProp.typename != type)
            {
                MessageBox.Show("ERROR: Images are different types!" +
                    "\nInjected image = " + type +
                    "\nInternal image = " + pmProp.typename);
                return;
            }
            if (bi.formatname.ToString().ToUpper().Contains("DXT"))
                bi.bitsPerPixel = 32; // Some programs don't save these values, so to be sure

            int rawsize = (int)br.BaseStream.Length - 128;
            br.BaseStream.Position = 128;
            byte[] bChunk = br.ReadBytes(rawsize);

            int cubemap = ((dds.ddsd.ddsCaps.caps2 & (int)DDSEnum.DDSCAPS2_CUBEMAP) > 0 ? 6 : 1);
            byte[][][] mipStreams = new byte[cubemap][][];
            for (int i = 0; i < mipStreams.Length; i++)
                mipStreams[i] = new byte[Math.Max(dds.ddsd.MipMapCount, 1)][];

            // 6 loops for cubemaps, 1 for all else
            for (int c = 0; c < cubemap; c++)
            {
                // the starting offset of each cubemap side
                int mipOffset = c * (bChunk.Length / cubemap);
                int inOffset = mipOffset;

                // Loop for each mipmap within each cube face (if it is a cubemap)
                for (int i = 0; i < mipStreams[c].Length; i++)
                {
                    #region calculate width, height & stream length of mipmap
                    // each mipmap is 1/2 width & height
                    int tWidth = ddsPaddedWidth / (1 << i);
                    int tHeight = dds.ddsd.height / (1 << i);

                    // Contains the size of the current MipMap
                    int mipStreamLength;
                    switch (bi.formatname)
                    {
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1:
                            mipStreamLength = Math.Max(1, tWidth / 4) * Math.Max(1, tHeight / 4) * 8;
                            break;
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3:
                        case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5:
                            mipStreamLength = Math.Max(1, tWidth / 4) * Math.Max(1, tHeight / 4) * 16;
                            break;
                        default:
                            mipStreamLength = Math.Max(1, tWidth) * Math.Max(1, tHeight) * (dds.ddsd.ddfPixelFormat.RGBBitCount >> 3);
                            break;
                    }
                    #endregion

                    mipStreams[c][i] = new byte[mipStreamLength];
                    if (dds.ddsd.width == ddsPaddedWidth)
                        Array.Copy(bChunk, mipOffset, mipStreams[c][i], 0, mipStreams[c][i].Length);
                    else
                    {
                        int byteStep = (dds.ddsd.ddfPixelFormat.RGBBitCount >> 3);
                        Array.Clear(mipStreams[c][i], 0, mipStreams[c][i].Length);
                        // Copy each line, adding padding to the output
                        for (int h = 0; h < (dds.ddsd.height); h++)
                        {
                            Array.Copy(
                                bChunk,
                                mipOffset + h * dds.ddsd.width * byteStep,
                                mipStreams[c][i],
                                h * ddsPaddedWidth * byteStep,
                                dds.ddsd.width * byteStep);
                        }
                    }
                    mipOffset += mipStreams[c][i].Length;
                }


                #region conversion routine
                // Temporary. Remove after conversion routine verified / corrected
                    // Don't allow different BPP injections
                if ((pmProp.bitsPerPixel != bi.bitsPerPixel) ||
                    // Don't allow compressed types to be converted to
                    ((pmProp.formatname != bi.formatname) && pmProp.formatname.ToString().Contains("DXT")))
                {
                    string tempInfo =
                        "\nBPP: " + pmProp.bitsPerPixel.ToString() + ", " + bi.bitsPerPixel.ToString() +
                        "\nDXT: " + pmProp.formatname.ToString() + ", " + bi.formatname.ToString() + " (DXT = " + pmProp.formatname.ToString().Contains("DXT") + ")";
                    MessageBox.Show("Incompatible format types\n Check Bits Per Pixel & Format type\n *NOTE* No conversions to any DXT (compressed) format supported!\n" + tempInfo);
                    return;
                }

                // H2 pads to a 128 boundry (not cubemaps)
                if (cubemap <= 1 && mipOffset % 128 != 0)
                    mipOffset += (128 - (mipOffset % 128));

                /*
                if (pmProp.formatname != bi.formatname)
                {
                    byte[] temp = new byte[mipStreamLength];

                    // Check if we are at the end of Data, such as less mipmaps injected than the original
                    if (inOffset + mipStreamLength > bAll.Length)
                    {
                        return;
                    }

                    Array.Copy(bAll, inOffset, temp, 0, mipStreamLength);
                    Bitmap ba = DDS_Convert.DecodeDDS(temp, bi);

                    /***** This testes to make sure it was being decoded properly
                    Form1 f = new Form1();
                    System.Windows.Forms.PictureBox p1 = new System.Windows.Forms.PictureBox();
                    p1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    p1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    p1.Location = new System.Drawing.Point(300, 10);
                    p1.Size = new System.Drawing.Size(70, 70);
                    p1.Image = ba;
                    f.Controls.Add(p1);
                    f.ShowDialog();
                    *******/

                /*// We need to make sure that it doesn't swizzle it here as it does it later!
                bool tb = pmProp.swizzle;
                pmProp.swizzle = false;
                bChunk = DDS_Convert.EncodeDDS(ba, ref pmProp);
                pmProp.swizzle = tb;
                ba.Dispose();
                bi.width = (ushort)(bi.width / 2);
                bi.height = (ushort)(bi.height / 2);
                tWidth = pmProp.width;
                tHeight = pmProp.height;
                pmProp.width = (ushort)origWidth;
                pmProp.height = (ushort)origHeight;
            }
            */
                #endregion


                #region Add padding, byte changes, etc                
                for (int mi = 0; mi < mipStreams[c].Length; mi++)
                {
                    int mipWidth = pmProp.width >> mi;

                    // We don't need to pad the saved data as we pad any data read in
                    /*
                    if (pmProp.width % 16 != 0)
                        //if (mipWidth % 16 != 0)
                    {
                        int paddedWidth = 16 - (mipWidth % 16);
                        int byteStep = pmProp.bitsPerPixel / 8;
                        byte[] paddedChunk = new byte[(mipWidth + paddedWidth) * pmProp.height * byteStep];

                        Array.Clear(paddedChunk, 0, paddedChunk.Length); // Make sure all bytes are zero to start!                        

                        // Copy each line, adding padding to the output
                        for (int h = 0; h < (pmProp.height >> mi); h++)
                        {
                            Array.Copy(
                                mipStreams[c][mi],
                                h * mipWidth * byteStep,
                                paddedChunk,
                                h * (mipWidth + paddedWidth) * byteStep,
                                mipWidth * byteStep);
                        }
                        mipStreams[c][mi] = paddedChunk;
                    }
                    */

                    // G8B8 is based on a 128 value for 0 and adds -/+, so adjust values to 128
                    if (pmProp.formatname == ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8)
                    {
                        for (int mii = 0; mii < mipStreams[c][mi].Length; mii++)
                        {
                            mipStreams[c][mi][mii] += 128;
                        }
                    }
                }
                #endregion

                int lodNumber = 0;
                // First one is the main pic, not mipmap
                for (int j = 0; j < m.raw.rawChunks.Count; j++)
                {
                    // If the selected index is the desired index and the LOD # starts at the mipmap #
                    if (((BitmapRawDataChunk)m.raw.rawChunks[j]).inchunk == index &&
                        ((BitmapRawDataChunk)m.raw.rawChunks[j]).num == lodNumber)
                    {

                        m.Map.OpenMap(m.raw.rawChunks[j].rawLocation, false);
                        if (!m.Map.isOpen)
                        {
                            return;
                        }

                        // Recaculate the total length (may have changed from above with conversion code)
                        int totalLength = 0;
                        for (int temp = lodNumber; temp < mipStreams[c].Length; temp++)
                            totalLength += mipStreams[c][temp].Length;

                        // H2 pads to a 128 boundry for each Bpp (not on cubemaps)
                        switch (pmProp.formatname)
                        {                               
                            case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1:
                                if (totalLength % (256) != 0)
                                    totalLength += 256 - (totalLength % (256));
                                break;
                            case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3:
                            case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5:
                                if (totalLength % (128) != 0)
                                    totalLength += 128 - (totalLength % (128));
                                break;
                            default:
                                if (totalLength % (128 * pmProp.bitsPerPixel >> 3) != 0)
                                    totalLength += (128 * (pmProp.bitsPerPixel >> 3) - (totalLength % (128 * pmProp.bitsPerPixel >> 3)));
                                break;
                        }
                        byte[] tempChunk = new byte[totalLength];
                        int outOffset = 0;
                        for (int temp = lodNumber; temp < mipStreams[c].Length; temp++)
                        {
                            if (pmProp.swizzle)
                            {
                                Array.Copy(
                                    Swizzler.Swizzle(
                                        mipStreams[c][temp],
                                        dds.ddsd.width >> temp,
                                        dds.ddsd.height >> temp,
                                        -1,
                                        dds.ddsd.ddfPixelFormat.RGBBitCount,
                                        false),
                                    0,
                                    tempChunk,
                                    outOffset,
                                    mipStreams[c][temp].Length);

                            }
                            else
                                Array.Copy(mipStreams[c][temp], 0, tempChunk, outOffset, mipStreams[c][temp].Length);
                            outOffset += mipStreams[c][temp].Length;
                        }

                        // Append onto existing data for cubemaps
                        m.Map.BW.BaseStream.Position = m.raw.rawChunks[j].offset + totalLength * c;
                        m.Map.BW.Write(tempChunk, 0, Math.Min(tempChunk.Length, m.raw.rawChunks[j].size - (totalLength * c)));


                        if (c == cubemap - 1)
                        {
                            m.Map.OpenMap(MapTypes.Internal);
                            if (((BitmapRawDataChunk)m.raw.rawChunks[j]).num == 0)
                            {
                                m.Map.BW.BaseStream.Position = m.offset + m.raw.rawChunks[j].pointerMetaOffset - 28;
                                pm.Properties[index].Write(ref m.Map.BW);
                            }
                            
                            m.Map.BW.BaseStream.Position = m.offset + m.raw.rawChunks[j].pointerMetaOffset + 24;
                            m.Map.BW.Write(Math.Min(tempChunk.Length * cubemap, m.raw.rawChunks[j].size));
                            m.Map.CloseMap();
                        }
                        
                        lodNumber++;
                    }
                }
                inOffset += mipOffset;
            }
        }

        /// <summary>
        /// The get bitmap format.
        /// </summary>
        /// <param name="dds">The dds.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ParsedBitmap.BitmapFormat getBitmapFormat(DDS_HEADER_STRUCTURE dds)
        {
            if (dds.ddsd.ddfPixelFormat.FourCC.StartsWith("DXT"))
            {
                switch (dds.ddsd.ddfPixelFormat.FourCC)
                {
                    case "DXT1":
                        return ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1;
                    case "DXT2":
                    case "DXT3":
                        return ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3;
                    case "DXT4":
                    case "DXT5":
                    default:
                        return ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5;
                }
            }
            else if (dds.ddsd.ddfPixelFormat.RGBAlphaBitMask != 0)
            {
                if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0x8000) && (dds.ddsd.ddfPixelFormat.RBitMask == 0x7C00) &&
                    (dds.ddsd.ddfPixelFormat.GBitMask == 0x03E0) && (dds.ddsd.ddfPixelFormat.BBitMask == 0x001F))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0xF000) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0x0F00) && (dds.ddsd.ddfPixelFormat.GBitMask == 0x00F0) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x000F))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0x00000000) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0x00FF0000) &&
                         (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000FF00) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x000000FF))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_X8R8G8B8;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0xFF000000) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0x00FF0000) &&
                         (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000FF00) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x000000FF))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0xFF00) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0x00FF) && 
                         (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x0000))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0xFF) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0x0000) &&
                         (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x0000))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A8;
                }
                else if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0x00FF) &&
                         (dds.ddsd.ddfPixelFormat.RBitMask == 0xFF00) && (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x0000))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8;
                }

                    /*
                    else // Same as above, so we need to post process to figure out which one as A8Y8 is byte & G8B8 is sbyte
                    if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0x00FF) &&
                        (dds.ddsd.ddfPixelFormat.RBitMask == 0xFF00) &&
                        (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000) &&
                        (dds.ddsd.ddfPixelFormat.BBitMask == 0x0000))
                        return Raw.ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8;
                    */
                else
                {
                    ////
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A8;
                }
            }
            else
            {
                if ((dds.ddsd.ddfPixelFormat.RGBAlphaBitMask == 0x00000000) &&
                    (dds.ddsd.ddfPixelFormat.RBitMask == 0x00FF0000) && (dds.ddsd.ddfPixelFormat.GBitMask == 0x0000FF00) &&
                    (dds.ddsd.ddfPixelFormat.BBitMask == 0x000000FF))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_X8R8G8B8;
                }
                else if ((dds.ddsd.ddfPixelFormat.RBitMask == 0xF800) && (dds.ddsd.ddfPixelFormat.GBitMask == 0x07E0) &&
                         (dds.ddsd.ddfPixelFormat.BBitMask == 0x001F))
                {
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_R5G6B5;
                }
                else
                {
                    ////
                    return ParsedBitmap.BitmapFormat.BITM_FORMAT_A8;
                }
            }
        }

        /// <summary>
        /// The get bitmap type.
        /// </summary>
        /// <param name="ddsd">The ddsd.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ParsedBitmap.BitmapType getBitmapType(DDSURFACEDESC2 ddsd)
        {
            if ((ddsd.ddsCaps.caps2 & (int)DDSEnum.DDSCAPS2_VOLUME) != 0)
            {
                return ParsedBitmap.BitmapType.BITM_TYPE_3D;
            }
            else if ((ddsd.ddsCaps.caps2 & (int)DDSEnum.DDSCAPS2_CUBEMAP) != 0)
            {
                return ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP;
            }

                // else if ((ddsd.ddfPixelFormat. & (int)DDSEnum.DDSCAPS2_CUBEMAP) != 0)
            // return Entity.Raw.ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP;
            else
            {
                return ParsedBitmap.BitmapType.BITM_TYPE_2D;
            }
        }

        /// <summary>
        /// The get dds type.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DDSFileFormat getDDSType(ParsedBitmap.BitmapInfo info)
        {
            string[] formatDescriptions = new[]
                {
                    "DXT1       ARGB    4 bpp | 1 bit alpha", "DXT3       ARGB    8 bpp | explicit alpha", 
                    "DXT5       ARGB    8 bpp | interpolated alpha", "8             A    8 bpp | alpha", 
                    "8.8          AL   16 bpp | alpha/luminance", "4.4.4.4     RGB   16 bpp | unsigned", 
                    "1.5.5.5    ARGB   16 bpp | unsigned", "5.6.5       RGB   16 bpp | unsigned", 
                    "4.4        ARGB   16 bpp | unsigned", "X.8.8.8    XRGB   32 bpp | unsigned", 
                    "8.8.8.8    ARGB   32 bpp | unsigned", "                         | lightmap"
                };
            DDSFileFormat ddsff = new DDSFileFormat();
            string temp = "(";
            switch (info.typename)
            {
                case ParsedBitmap.BitmapType.BITM_TYPE_2D:
                    ddsff.Type = DDSFileType._2D;
                    temp += "2D";
                    break;
                case ParsedBitmap.BitmapType.BITM_TYPE_3D:
                    ddsff.Type = DDSFileType._3D;
                    temp += "3D";
                    break;
                case ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP:
                    ddsff.Type = DDSFileType._CubeMap;
                    temp += "CUBEMAP";
                    break;
            }

            temp += ") ";
            ddsff.BMFormat = info.formatname;
            switch (info.formatname)
            {
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1:
                    ddsff.Format = DDSFileFormats.DXT1;
                    temp += formatDescriptions[0];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3:
                    ddsff.Format = DDSFileFormats.DXT3;
                    temp += formatDescriptions[1];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5:
                    ddsff.Format = DDSFileFormats.DXT5;
                    temp += formatDescriptions[2];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_Y8:
                    ddsff.Format = DDSFileFormats.Alpha_8_bit;
                    temp += formatDescriptions[3];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8:
                    ddsff.Format = DDSFileFormats.AlphaLuminance_16_bit;
                    temp += formatDescriptions[4];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4:
                    ddsff.Format = DDSFileFormats.A4R4G4B4_16_bit;
                    temp += formatDescriptions[5];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5:
                    ddsff.Format = DDSFileFormats.A1R5G5B5_16_bit;
                    temp += formatDescriptions[6];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_R5G6B5:
                    ddsff.Format = DDSFileFormats.R5G6B5_16_bit;
                    temp += formatDescriptions[7];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8:
                    ddsff.Format = DDSFileFormats.AlphaLuminance_16_bit;
                    temp += formatDescriptions[8];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_AY8:
                    ddsff.Format = DDSFileFormats.AlphaLuminance_16_bit;
                    temp += formatDescriptions[8];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_X8R8G8B8:
                    ddsff.Format = DDSFileFormats.X8R8G8B8_32_bit;
                    temp += formatDescriptions[9];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8:
                    ddsff.Format = DDSFileFormats.A8R8G8B8_32_bit;
                    temp += formatDescriptions[10];
                    break;
                case ParsedBitmap.BitmapFormat.BITM_FORMAT_LIGHTMAP:
                    ddsff.Format = DDSFileFormats.BITM_FORMAT_LIGHTMAP;
                    temp += formatDescriptions[11];
                    break;
            }

            ddsff.Description = temp;
            return ddsff; // (temp + "\"");
        }

        #endregion

        /// <summary>
        /// The ddpixelformat.
        /// </summary>
        /// <remarks></remarks>
        public struct DDPIXELFORMAT
        {
            #region Constants and Fields

            /// <summary>
            /// The b bit mask.
            /// </summary>
            public uint BBitMask;

            /// <summary>
            /// The flags.
            /// </summary>
            public int Flags; // '4

            /// <summary>
            /// The four cc.
            /// </summary>
            public string FourCC; // 'DXT1, DXT2, etc..

            /// <summary>
            /// The g bit mask.
            /// </summary>
            public uint GBitMask;

            /// <summary>
            /// The r bit mask.
            /// </summary>
            public uint RBitMask;

            /// <summary>
            /// The rgb alpha bit mask.
            /// </summary>
            public uint RGBAlphaBitMask;

            /// <summary>
            /// The rgb bit count.
            /// </summary>
            public int RGBBitCount;

            /// <summary>
            /// The size.
            /// </summary>
            public int size; // '32

            #endregion

            //////////////////////////////////////////////////////
            // Generate the DDPIXELFORMAT chunk of the header
            //////////////////////////////////////////////////////
            #region Public Methods

            /// <summary>
            /// The generate.
            /// </summary>
            /// <param name="b2">The b 2.</param>
            /// <remarks></remarks>
            public void generate(ParsedBitmap.BitmapInfo b2)
            {
                // Size of structure. This member must be set to 32.
                size = 32;

                // Flags to indicate valid fields. Uncompressed formats will usually use DDPF_RGB to indicate
                // an RGB format, while compressed formats will use DDPF_FOURCC with a four-character code.
                // This is accomplished in the below structure.

                // This is the four-character code for compressed formats. dwFlags should include DDPF_FOURCC in
                // this case. For DXTn compression, this is set to "DXT1", "DXT2", "DXT3", "DXT4", or "DXT5".
                Flags = 0;
                switch ((int)b2.formatname)
                {
                    case 0xE:
                        FourCC = "DXT1";
                        Flags = Flags + (int)DDSEnum.DDPF_FOURCC;
                        break;
                    case 0xF:
                        FourCC = "DXT3";
                        Flags = Flags + (int)DDSEnum.DDPF_FOURCC;
                        break;
                    case 0x10:
                        FourCC = "DXT5";
                        Flags = Flags + (int)DDSEnum.DDPF_FOURCC;
                        break;
                    default:
                        FourCC = "\0\0\0\0";
                        Flags = Flags + (int)DDSEnum.DDPF_RGB;
                        break;
                }

                // For RGB formats, this is the total number of bits in the format. dwFlags should include DDPF_RGB
                // in this case. This value is usually 16, 24, or 32. For A8R8G8B8, this value would be 32.
                RGBBitCount = b2.bitsPerPixel;

                // For RGB formats dwFlags should include DDPF_ALPHAPIXELS if it contains an alpha channel.
                // For A8R8G8B8, this value would be 0xff000000.
                switch (b2.formatname)
                {
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8:
                        Flags = Flags + (int)DDSEnum.DDPF_ALPHAPIXELS;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_Y8:
                        Flags = Flags + (int)DDSEnum.DDPF_ALPHA;
                        Flags = Flags - (int)DDSEnum.DDPF_RGB; // These formats are ALPHA ONLY, no RGB
                        break;
                }

                // For RGB formats, this contains the masks for the red, green, and blue channels. For A8R8G8B8, these
                // values would be 0x00ff0000, 0x0000ff00, and 0x000000ff respectively.
                switch (b2.formatname)
                {
                    /*
                RGBAlphaBitMask = 0x0000;
                    RBitMask = 0x00FF;
                    GBitMask = 0xFF00;
                    BBitMask = 0x0000;
                    Flags = 0x00080000;   // Unknown flag. Used for 2 color dds files by photoshop?
                    break;
                */
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8:
                        RGBAlphaBitMask = 0xFF00;
                        RBitMask = 0x00FF;
                        GBitMask = 0x0000;
                        BBitMask = 0x0000;
                        Flags -= (int)DDSEnum.DDPF_RGB; // This format doesn't use RGB
                        Flags += 0x00020000; // Unknown flag. Used for unsigned 16-bit dds files by photoshop?
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_AY8:
                        RGBAlphaBitMask = 0x0000;
                        RBitMask = 0x000F;
                        GBitMask = 0x00F0;
                        BBitMask = 0x0000;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_Y8:
                        RGBAlphaBitMask = 0x00FF;
                        RBitMask = 0x0000;
                        GBitMask = 0x0000;
                        BBitMask = 0x0000;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_R5G6B5:
                        RGBAlphaBitMask = 0x0000;
                        RBitMask = 0xF800;
                        GBitMask = 0x07E0;
                        BBitMask = 0x001F;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5:
                        RGBAlphaBitMask = 0x8000;
                        RBitMask = 0x7C00;
                        GBitMask = 0x03E0;
                        BBitMask = 0x001F;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4:
                        RGBAlphaBitMask = 0xF000;
                        RBitMask = 0x0F00;
                        GBitMask = 0x00F0;
                        BBitMask = 0x000F;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_X8R8G8B8:
                        RGBAlphaBitMask = 0x00000000;
                        RBitMask = 0x00FF0000;
                        GBitMask = 0x0000FF00;
                        BBitMask = 0x000000FF;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8:
                        RGBAlphaBitMask = 0xFF000000;
                        RBitMask = 0x00FF0000;
                        GBitMask = 0x0000FF00;
                        BBitMask = 0x000000FF;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_LIGHTMAP:
                        RGBAlphaBitMask = 0x00000000;
                        RBitMask = 0x000000FF;
                        GBitMask = 0x000000FF;
                        BBitMask = 0x000000FF;
                        Flags -= (int)DDSEnum.DDPF_RGB; // This format doesn't use RGB
                        Flags += 0x00020000; // Unknown flag. Used for unsigned 16-bit dds files by photoshop?
                        break;
                    default:
                        RGBAlphaBitMask = 0x0;
                        RBitMask = 0;
                        GBitMask = 0;
                        BBitMask = 0;
                        break;
                }
            }

            /// <summary>
            /// The read struct.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <remarks></remarks>
            public void readStruct(ref BinaryReader br)
            {
                size = br.ReadInt32(); // 0x4C
                Flags = br.ReadInt32(); // 0x50
                FourCC = new string(br.ReadChars(4)); // 0x54
                RGBBitCount = br.ReadInt32(); // 0x58
                RBitMask = br.ReadUInt32(); // 0x5C
                GBitMask = br.ReadUInt32(); // 0x60
                BBitMask = br.ReadUInt32(); // 0x64
                RGBAlphaBitMask = br.ReadUInt32(); // 0x68
            }

            /// <summary>
            /// The write struct.
            /// </summary>
            /// <param name="bw">The bw.</param>
            /// <remarks></remarks>
            public void writeStruct(ref BinaryWriter bw)
            {
                bw.Write(size);
                bw.Write(Flags);
                bw.Write(FourCC.ToCharArray());

                bw.Write(RGBBitCount);
                bw.Write(RBitMask);
                bw.Write(GBitMask);
                bw.Write(BBitMask);
                bw.Write(RGBAlphaBitMask);
            }

            #endregion
        }

        /// <summary>
        /// The ddscap s 2.
        /// </summary>
        /// <remarks></remarks>
        public struct DDSCAPS2
        {
            #region Constants and Fields

            /// <summary>
            /// The reserved.
            /// </summary>
            public int[] Reserved;

            /// <summary>
            /// The caps 1.
            /// </summary>
            public int caps1;

            /// <summary>
            /// The caps 2.
            /// </summary>
            public int caps2;

            #endregion

            //////////////////////////////////////////////////////
            // Generate the DDSCAPS2 chunk of the header
            //////////////////////////////////////////////////////
            #region Public Methods

            /// <summary>
            /// The generate.
            /// </summary>
            /// <param name="b2">The b 2.</param>
            /// <remarks></remarks>
            public void generate(ref ParsedBitmap.BitmapInfo b2)
            {
                // DDS files should always include DDSCAPS_TEXTURE. If the file contains mipmaps, DDSCAPS_MIPMAP
                // should be set. For any DDS file with more than one main surface,such as a mipmaps, cubic
                // environment map, or volume texture, DDSCAPS_COMPLEX should also be set.
                caps1 += (int)DDSEnum.DDSCAPS_TEXTURE;
                if (b2.mipMapCount > 0)
                {
                    caps1 += (int)DDSEnum.DDSCAPS_MIPMAP;
                }

                caps1 += (int)DDSEnum.DDSCAPS_COMPLEX;

                // For cubic environment maps, DDSCAPS2_CUBEMAP should be included as well as one or more faces of
                // the map (DDSCAPS2_CUBEMAP_POSITIVEX, DDSCAPS2_CUBEMAP_NEGATIVEX, DDSCAPS2_CUBEMAP_POSITIVEY,
                // DDSCAPS2_CUBEMAP_NEGATIVEY, DDSCAPS2_CUBEMAP_POSITIVEZ, DDSCAPS2_CUBEMAP_NEGATIVEZ). For volume
                // textures, DDSCAPS2_VOLUME should be included.
                // ******************************************************************************
                // This is where I stopped - this code needs to be added for cubemaps.
                // ******************************************************************************
                caps2 = 0;
                if (b2.typename == ParsedBitmap.BitmapType.BITM_TYPE_3D)
                {
                    caps2 += (int)DDSEnum.DDSCAPS2_VOLUME;
                }

                if (b2.typename == ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP)
                {
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_NEGATIVEX;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_NEGATIVEY;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_NEGATIVEZ;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_POSITIVEX;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_POSITIVEY;
                    caps2 += (int)DDSEnum.DDSCAPS2_CUBEMAP_POSITIVEZ;
                }
            }

            /// <summary>
            /// The read struct.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <remarks></remarks>
            public void readStruct(ref BinaryReader br)
            {
                caps1 = br.ReadInt32();
                caps2 = br.ReadInt32();
                Reserved = new int[2];
                Reserved[0] = br.ReadInt32();
                Reserved[1] = br.ReadInt32();
            }

            /// <summary>
            /// The write struct.
            /// </summary>
            /// <param name="bw">The bw.</param>
            /// <remarks></remarks>
            public void writeStruct(ref BinaryWriter bw)
            {
                bw.Write(caps1);
                bw.Write(caps2);
                bw.Write(0); // Reserved[0]);
                bw.Write(0); // Reserved[1]);
            }

            #endregion
        }

        //////////////////////////////////////////////////////

        /// <summary>
        /// The ddsurfacedes c 2.
        /// </summary>
        /// <remarks></remarks>
        public struct DDSURFACEDESC2
        {
            #region Constants and Fields

            /// <summary>
            /// The mip map count.
            /// </summary>
            public int MipMapCount; // Total Number of MipMaps

            /// <summary>
            /// The pitch or linear size.
            /// </summary>
            public int PitchOrLinearSize;

            /// <summary>
            /// The reserved 1.
            /// </summary>
            public int[] Reserved1;

            /// <summary>
            /// The reserved 2.
            /// </summary>
            public int Reserved2;

            /// <summary>
            /// The ddf pixel format.
            /// </summary>
            public DDPIXELFORMAT ddfPixelFormat;

            /// <summary>
            /// The dds caps.
            /// </summary>
            public DDSCAPS2 ddsCaps;

            /// <summary>
            /// The depth.
            /// </summary>
            public int depth; // Only used for volume textures.

            /// <summary>
            /// The flags.
            /// </summary>
            public int flags;

            /// <summary>
            /// The height.
            /// </summary>
            public int height;

            /// <summary>
            /// The size_of_structure.
            /// </summary>
            public int size_of_structure;

            /// <summary>
            /// The width.
            /// </summary>
            public int width;

            #endregion

            //////////////////////////////////////////////////////
            // Generate the DDSURFACEDESC2 chunk of the header
            //////////////////////////////////////////////////////
            #region Public Methods

            /// <summary>
            /// The generate.
            /// </summary>
            /// <param name="b2">The b 2.</param>
            /// <remarks></remarks>
            public void generate(ref ParsedBitmap.BitmapInfo b2)
            {
                Reserved1 = new int[11];

                // 'Size of structure. This member must be set to 124.
                size_of_structure = 124;

                flags = 0;

                // Flags to indicate valid fields. Always include DDSD_CAPS, DDSD_PIXELFORMAT,
                // DDSD_WIDTH, DDSD_HEIGHT and either DDSD_PITCH (for uncompressed) or DDSD_LINEARSIZE (for compressed).
                flags = flags + (int)DDSEnum.DDSD_CAPS;
                flags = flags + (int)DDSEnum.DDSD_PIXELFORMAT;
                flags = flags + (int)DDSEnum.DDSD_WIDTH;
                flags = flags + (int)DDSEnum.DDSD_HEIGHT;
                switch (b2.formatname)
                {
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT1:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT2AND3:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_DXT4AND5:
                        flags = flags + (int)DDSEnum.DDSD_LINEARSIZE;
                        break;
                    default:
                        flags = flags + (int)DDSEnum.DDSD_PITCH;
                        break;
                }

                // Height of the main image in pixels
                height = b2.height;

                // Width of the main image in pixels
                width = b2.width;

                int RGBBitCount = 0;
                switch (b2.formatname)
                {
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_X8R8G8B8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8R8G8B8:
                        RGBBitCount = 32;
                        break;
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A1R5G5B5:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A4R4G4B4:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8Y8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_G8B8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_R5G6B5:
                        RGBBitCount = 16;
                        break;

                    // AY8 is same as Raw.ParsedBitmap.BitmapType.BITM_TYPE_CUBEMAP:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_LIGHTMAP:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_A8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_P8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_Y8:
                    case ParsedBitmap.BitmapFormat.BITM_FORMAT_AY8:
                        RGBBitCount = 8;
                        break;
                }

                // For uncompressed formats, this is the number of bytes per scan line (DWORD aligned) for the main
                // image. dwFlags should include DDSD_PITCH in this case. For compressed formats, this is the (total)
                // number of bytes for the main image. dwFlags should be include DDSD_LINEARSIZE in this case.
                PitchOrLinearSize = width * (RGBBitCount / 8);

                // For volume textures, this is the depth of the volume.
                // dwFlags should include DDSD_DEPTH in this case.

                depth = b2.depth > 1 ? b2.depth : 0; // UPDATE: There are volume textures / 3D bitmaps
                if (depth > 0)
                {
                    flags = flags + (int)DDSEnum.DDSD_DEPTH;
                }

                // For items with mipmap levels, this is the total number of levels in the mipmap
                // chain of the main image. dwFlags should include DDSD_MIPMAPCOUNT in this case
                MipMapCount = b2.mipMapCount;
                if (MipMapCount > 1)
                {
                    flags = flags + (int)DDSEnum.DDSD_MIPMAPCOUNT;
                }

                // A 32-byte value that specifies the pixel format structure.
                ddfPixelFormat.generate(b2);

                // A 16-byte value that specifies the capabilities structure.
                ddsCaps.generate(ref b2);
                // End Sub
            }

            /// <summary>
            /// The read struct.
            /// </summary>
            /// <param name="br">The br.</param>
            /// <remarks></remarks>
            public void readStruct(ref BinaryReader br)
            {
                size_of_structure = br.ReadInt32();
                flags = br.ReadInt32();
                height = br.ReadInt32();
                width = br.ReadInt32();
                PitchOrLinearSize = br.ReadInt32();
                depth = br.ReadInt32();
                MipMapCount = br.ReadInt32();

                Reserved1 = new int[11]; // 11 ints long
                for (int x = 0; x < 11; x++)
                {
                    Reserved1[x] = br.ReadInt32();
                }

                ddfPixelFormat.readStruct(ref br);
                ddsCaps.readStruct(ref br);
                Reserved2 = br.ReadInt32();
            }

            /// <summary>
            /// The write struct.
            /// </summary>
            /// <param name="bw">The bw.</param>
            /// <remarks></remarks>
            public void writeStruct(ref BinaryWriter bw)
            {
                bw.Write(size_of_structure); // 0x04
                bw.Write(flags); // 0x08
                bw.Write(height); // 0x0C
                bw.Write(width); // 0x10
                bw.Write(PitchOrLinearSize); // 0x14
                bw.Write(depth); // 0x18
                bw.Write(MipMapCount); // 0x1C

                for (int x = 0; x < 11; x++)
                {
                    // 0x20 -> 0x48
                    bw.Write(Reserved1[x]);
                }

                ddfPixelFormat.writeStruct(ref bw); // 0x4C
                ddsCaps.writeStruct(ref bw); // 0x6C
                bw.Write(Reserved2);
            }

            #endregion
        }

        /// <summary>
        /// The dd s_ heade r_ structure.
        /// </summary>
        /// <remarks></remarks>
        public struct DDS_HEADER_STRUCTURE
        {
            #region Constants and Fields

            /// <summary>
            /// The ddsd.
            /// </summary>
            public DDSURFACEDESC2 ddsd;

            /// <summary>
            /// The magic.
            /// </summary>
            public string magic;

            #endregion

            // Generate a DDS Header
            //////////////////////////////////////////////////////
            #region Public Methods

            /// <summary>
            /// The read struct.
            /// </summary>
            /// <param name="BR">The br.</param>
            /// <remarks></remarks>
            public void ReadStruct(ref BinaryReader BR)
            {
                magic = new string(BR.ReadChars(4));
                ddsd.readStruct(ref BR);
            }

            /// <summary>
            /// The write struct.
            /// </summary>
            /// <param name="BW">The bw.</param>
            /// <remarks></remarks>
            public void WriteStruct(ref BinaryWriter BW)
            {
                BW.Write(magic.ToCharArray());
                ddsd.writeStruct(ref BW);
            }

            /// <summary>
            /// The generate.
            /// </summary>
            /// <param name="b2">The b 2.</param>
            /// <remarks></remarks>
            public void generate(ref ParsedBitmap.BitmapInfo b2)
            {
                magic = "DDS ";
                ddsd.generate(ref b2);
            }

            #endregion
        }

        /// <summary>
        /// The dds file format.
        /// </summary>
        /// <remarks></remarks>
        public class DDSFileFormat
        {
            #region Constants and Fields

            /// <summary>
            /// The bm format.
            /// </summary>
            public ParsedBitmap.BitmapFormat BMFormat;

            /// <summary>
            /// The description.
            /// </summary>
            public string Description;

            /// <summary>
            /// The format.
            /// </summary>
            public DDSFileFormats Format;

            /// <summary>
            /// The type.
            /// </summary>
            public DDSFileType Type;

            #endregion
        }
    }
}