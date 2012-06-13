// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecodeDXT.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for DecodeDXT.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.DDSFunctions
{
    using System;

    /// <summary>
    /// Summary description for DecodeDXT.
    /// </summary>
    /// <remarks></remarks>
    public class DecodeDXT
    {
        #region Constants and Fields

        /// <summary>
        /// The il.
        /// </summary>
        private readonly ImageLib il = new ImageLib();

        #endregion

        #region Public Methods

        /// <summary>
        /// The decode dx t 1.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="SourceData">The source data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] DecodeDXT1(int height, int width, byte[] SourceData)
        {
            byte[] DestData;
            ImageLib.RGBA_COLOR_STRUCT[] Color = new ImageLib.RGBA_COLOR_STRUCT[5];
            int i;
            int dptr = 0;
            ImageLib.RGBA_COLOR_STRUCT CColor;
            int CData;
            int ChunksPerHLine = width / 4;
            bool trans;
            ImageLib.RGBA_COLOR_STRUCT zeroColor;
            int c1, c2;
            DestData = new byte[((width * height) * 4)];
            if (ChunksPerHLine == 0)
            {
                ChunksPerHLine += 1;
            }

            for (i = 0; i < (width * height); i += 16)
            {
                c1 = (SourceData[dptr + 1] << 8) | SourceData[dptr];
                c2 = (SourceData[dptr + 3] << 8) | SourceData[dptr + 2];

                if (c1 > c2)
                {
                    trans = false;
                }
                else
                {
                    trans = true;
                }

                Color[0] = il.short_to_rgba(c1);
                Color[1] = il.short_to_rgba(c2);
                if (!trans)
                {
                    Color[2] = il.GradientColors(Color[0], Color[1]);
                    Color[3] = il.GradientColors(Color[1], Color[0]);
                }
                else
                {
                    zeroColor = Color[0];
                    Color[2] = il.GradientColorsHalf(Color[0], Color[1]);
                    Color[3] = zeroColor;
                }

                CData = (SourceData[dptr + 4] << 0) | (SourceData[dptr + 5] << 8) | (SourceData[dptr + 6] << 16) |
                        (SourceData[dptr + 7] << 24);
                int ChunkNum = i / 16;
                long XPos = ChunkNum % ChunksPerHLine;
                long YPos = (ChunkNum - XPos) / ChunksPerHLine;
                long ttmp;
                long ttmp2;
                int sizeh = height < 4 ? height : 4;
                int sizew = width < 4 ? width : 4;
                int x, y;
                for (x = 0; x < sizeh; x++)
                {
                    for (y = 0; y < sizew; y++)
                    {
                        CColor = Color[CData & 3];
                        CData >>= 2;
                        ttmp = ((YPos * 4 + x) * width + XPos * 4 + y) * 4;
                        ttmp2 = il.rgba_to_int(CColor);
                        DestData[ttmp] = Convert.ToByte(CColor.b);
                        DestData[ttmp + 1] = Convert.ToByte(CColor.g);
                        DestData[ttmp + 2] = Convert.ToByte(CColor.r);
                        DestData[ttmp + 3] = Convert.ToByte(CColor.a);
                    }
                }

                dptr += 8;
            }

            return DestData;
        }

        /// <summary>
        /// The decode dx t 23.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="SourceData">The source data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] DecodeDXT23(int height, int width, byte[] SourceData)
        {
            byte[] DestData;
            ImageLib.RGBA_COLOR_STRUCT[] Color = new ImageLib.RGBA_COLOR_STRUCT[5];
            int i;
            ImageLib.RGBA_COLOR_STRUCT CColor;
            int CData;
            int ChunksPerHLine = width / 4;
            //ImageLib.RGBA_COLOR_STRUCT zeroColor = new ImageLib.RGBA_COLOR_STRUCT();
            ImageLib.RGBA_COLOR_STRUCT c1, c2, c3, c4;
            DestData = new byte[((width * height) * 4)];
            if (ChunksPerHLine == 0)
            {
                ChunksPerHLine += 1;
            }

            for (i = 0; i < (width * height); i += 16)
            {
                c1 = il.short_to_rgba(SourceData[i + 8] | (SourceData[i + 9] << 8));
                c2 = il.short_to_rgba(SourceData[i + 10] | (SourceData[i + 11] << 8));
                c3 = il.GradientColors(Color[0], Color[1]);
                c4 = il.GradientColors(Color[1], Color[0]);
                Color[0] = c1;
                Color[1] = c2;
                Color[2] = c3;
                Color[3] = c4;

                CData = (SourceData[i + 12] << 0) | (SourceData[i + 13] << 8) | (SourceData[i + 14] << 16) |
                        (SourceData[i + 15] << 24);

                int ChunkNum = i / 16;
                long XPos = ChunkNum % ChunksPerHLine;
                long YPos = (ChunkNum - XPos) / ChunksPerHLine;
                long ttmp;
                int alpha;
                int sizeh = height < 4 ? height : 4;
                int sizew = width < 4 ? width : 4;
                int x, y;
                for (x = 0; x < sizeh; x++)
                {
                    alpha = SourceData[i + (2 * x)] | SourceData[i + (2 * x) + 1] << 8;
                    for (y = 0; y < sizew; y++)
                    {
                        CColor = Color[CData & 3];
                        CData >>= 2;
                        CColor.a = (alpha & 15) * 16;
                        alpha >>= 4;
                        ttmp = ((YPos * 4 + x) * width + XPos * 4 + y) * 4;

                        DestData[ttmp] = (byte)CColor.b;
                        DestData[ttmp + 1] = (byte)CColor.g;
                        DestData[ttmp + 2] = (byte)CColor.r;
                        DestData[ttmp + 3] = (byte)CColor.a;
                    }
                }
            }

            return DestData;
        }

        /// <summary>
        /// The decode dx t 45.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="SourceData">The source data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] DecodeDXT45(int height, int width, byte[] SourceData)
        {
            byte[] DestData;
            ImageLib.RGBA_COLOR_STRUCT[] Color = new ImageLib.RGBA_COLOR_STRUCT[4];
            int i;
            ImageLib.RGBA_COLOR_STRUCT CColor;
            int CData;
            int ChunksPerHLine = width / 4;
            //ImageLib.RGBA_COLOR_STRUCT zeroColor = new ImageLib.RGBA_COLOR_STRUCT();
            DestData = new byte[(width * height) * 4];

            if (ChunksPerHLine == 0)
            {
                ChunksPerHLine += 1;
            }

            for (i = 0; i < (width * height); i += 16)
            {
                Color[0] = il.short_to_rgba(SourceData[i + 8] | (SourceData[i + 9] << 8));
                Color[1] = il.short_to_rgba(SourceData[i + 10] | (SourceData[i + 11] << 8));
                Color[2] = il.GradientColors(Color[0], Color[1]);
                Color[3] = il.GradientColors(Color[1], Color[0]);

                CData = (SourceData[i + 12] << 0) | (SourceData[i + 13] << 8) | (SourceData[i + 14] << 16) |
                        (SourceData[i + 15] << 24);

                byte[] Alpha = new byte[8];
                Alpha[0] = SourceData[i];
                Alpha[1] = SourceData[i + 1];

                // Do the alphas
                if (Alpha[0] > Alpha[1])
                {
                    // 8-alpha block:  derive the other six alphas.
                    // Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
                    Alpha[2] = (byte)((6 * Alpha[0] + 1 * Alpha[1] + 3) / 7); // bit code 010
                    Alpha[3] = (byte)((5 * Alpha[0] + 2 * Alpha[1] + 3) / 7); // bit code 011
                    Alpha[4] = (byte)((4 * Alpha[0] + 3 * Alpha[1] + 3) / 7); // bit code 100
                    Alpha[5] = (byte)((3 * Alpha[0] + 4 * Alpha[1] + 3) / 7); // bit code 101
                    Alpha[6] = (byte)((2 * Alpha[0] + 5 * Alpha[1] + 3) / 7); // bit code 110
                    Alpha[7] = (byte)((1 * Alpha[0] + 6 * Alpha[1] + 3) / 7); // bit code 111
                }
                else
                {
                    // 6-alpha block.
                    // Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
                    Alpha[2] = (byte)((4 * Alpha[0] + 1 * Alpha[1] + 2) / 5); // Bit code 010
                    Alpha[3] = (byte)((3 * Alpha[0] + 2 * Alpha[1] + 2) / 5); // Bit code 011
                    Alpha[4] = (byte)((2 * Alpha[0] + 3 * Alpha[1] + 2) / 5); // Bit code 100
                    Alpha[5] = (byte)((1 * Alpha[0] + 4 * Alpha[1] + 2) / 5); // Bit code 101
                    Alpha[6] = 0; // Bit code 110
                    Alpha[7] = 255; // Bit code 111
                }

                // Byte	Alpha
                // 0	Alpha_0
                // 1	Alpha_1
                // 2	(0)(2) (2 LSBs), (0)(1), (0)(0)
                // 3	(1)(1) (1 LSB), (1)(0), (0)(3), (0)(2) (1 MSB)
                // 4	(1)(3), (1)(2), (1)(1) (2 MSBs)
                // 5	(2)(2) (2 LSBs), (2)(1), (2)(0)
                // 6	(3)(1) (1 LSB), (3)(0), (2)(3), (2)(2) (1 MSB)
                // 7	(3)(3), (3)(2), (3)(1) (2 MSBs)
                // (0
                // Read an int and a short
                long tmpdword;
                int tmpword;
                long alphaDat;
                tmpword = SourceData[i + 2] | (SourceData[i + 3] << 8);
                tmpdword = SourceData[i + 4] | (SourceData[i + 5] << 8) | (SourceData[i + 6] << 16) |
                           (SourceData[i + 7] << 24);

                alphaDat = tmpword | (tmpdword << 16);

                int ChunkNum = i / 16;
                long XPos = ChunkNum % ChunksPerHLine;
                long YPos = (ChunkNum - XPos) / ChunksPerHLine;
                long ttmp;
                int sizeh = height < 4 ? height : 4;
                int sizew = width < 4 ? width : 4;
                int x, y;
                for (x = 0; x < sizeh; x++)
                {
                    for (y = 0; y < sizew; y++)
                    {
                        CColor = Color[CData & 3];
                        CData >>= 2;
                        CColor.a = Alpha[alphaDat & 7];
                        alphaDat >>= 3;
                        ttmp = ((YPos * 4 + x) * width + XPos * 4 + y) * 4;
                        if ((CColor.a != 0) |
                            ((CColor.b != 0 | CColor.g != 0 | CColor.r != 0) &
                             (CColor.b != 255 | CColor.g != 255 | CColor.r != 255)))
                        {
                            //int fdfd = 0;
                        }

                        DestData[ttmp] = (byte)CColor.b;
                        DestData[ttmp + 1] = (byte)CColor.g;
                        DestData[ttmp + 2] = (byte)CColor.r;
                        DestData[ttmp + 3] = (byte)CColor.a;
                    }
                }
            }

            return DestData;
        }

        #endregion

        /// <summary>
        /// The image lib.
        /// </summary>
        /// <remarks></remarks>
        public class ImageLib
        {
            #region Public Methods

            /// <summary>
            /// The gradient colors.
            /// </summary>
            /// <param name="Col1">The col 1.</param>
            /// <param name="Col2">The col 2.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public RGBA_COLOR_STRUCT GradientColors(RGBA_COLOR_STRUCT Col1, RGBA_COLOR_STRUCT Col2)
            {
                RGBA_COLOR_STRUCT ret;
                ret.r = (Col1.r * 2 + Col2.r) / 3;
                ret.g = (Col1.g * 2 + Col2.g) / 3;
                ret.b = (Col1.b * 2 + Col2.b) / 3;
                ret.a = 255;
                return ret;
            }

            /// <summary>
            /// The gradient colors half.
            /// </summary>
            /// <param name="Col1">The col 1.</param>
            /// <param name="Col2">The col 2.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public RGBA_COLOR_STRUCT GradientColorsHalf(RGBA_COLOR_STRUCT Col1, RGBA_COLOR_STRUCT Col2)
            {
                RGBA_COLOR_STRUCT ret;
                ret.r = Col1.r / 2 + Col2.r / 2;
                ret.g = Col1.g / 2 + Col2.g / 2;
                ret.b = Col1.b / 2 + Col2.b / 2;
                ret.a = 255;
                return ret;
            }

            /// <summary>
            /// The rgba_to_int.
            /// </summary>
            /// <param name="c">The c.</param>
            /// <returns>The rgba_to_int.</returns>
            /// <remarks></remarks>
            public int rgba_to_int(RGBA_COLOR_STRUCT c)
            {
                return (c.a << 24) | (c.r << 16) | (c.g << 8) | c.b;
            }

            /// <summary>
            /// The short_to_rgba.
            /// </summary>
            /// <param name="color">The color.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public RGBA_COLOR_STRUCT short_to_rgba(int color)
            {
                RGBA_COLOR_STRUCT rc;
                color = Convert.ToUInt16(color);
                rc.r = (((color >> 11) & 31) * 255) / 31;
                rc.g = (((color >> 5) & 63) * 255) / 63;
                rc.b = (((color >> 0) & 31) * 255) / 31;
                rc.a = 255;
                return rc;
            }

            #endregion

            /// <summary>
            /// The rgb a_ colo r_ struct.
            /// </summary>
            /// <remarks></remarks>
            public struct RGBA_COLOR_STRUCT
            {
                #region Constants and Fields

                /// <summary>
                /// The a.
                /// </summary>
                public int a;

                /// <summary>
                /// The b.
                /// </summary>
                public int b;

                /// <summary>
                /// The g.
                /// </summary>
                public int g;

                /// <summary>
                /// The r.
                /// </summary>
                public int r;

                #endregion
            }
        }
    }
}