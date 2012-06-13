// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Swizzler.cs" company="">
//   
// </copyright>
// <summary>
//   Swizzles an array of pixels.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.DDSFunctions
{
    /// <summary>
    /// Swizzles an array of pixels.
    /// </summary>
    /// <remarks></remarks>
    public sealed class Swizzler
    {
        #region Public Methods

        /// <summary>
        /// The swizzle.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="bitCount">The bit count.</param>
        /// <param name="deswizzle">The deswizzle.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] Swizzle(
            byte[] raw, int offset, int width, int height, int depth, int bitCount, bool deswizzle)
        {
            if (raw.Length == 0)
                return new byte[0];

            if (depth < 1)
            {
                depth = 1;
            }

            bitCount /= 8;
            int a = 0, b = 0;
            int tempsize = raw.Length; // width * height * bitCount;
            byte[] data = new byte[tempsize];
            MaskSet masks = new MaskSet(width, height, depth);

            offset = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (deswizzle)
                        {
                            a = ((((z * height) + y) * width) + x) * bitCount;
                            b = Swizzle(x, y, z, masks) * bitCount;

                            // a = ((y * width) + x) * bitCount;
                            // b = (Swizzle(x, y, -1, masks)) * bitCount;
                        }
                        else
                        {
                            b = ((((z * height) + y) * width) + x) * bitCount;
                            a = Swizzle(x, y, z, masks) * bitCount;

                            // b = ((y * width) + x) * bitCount;
                            // a = (Swizzle(x, y, -1, masks)) * bitCount;
                        }

                        for (int i = offset; i < bitCount + offset; i++)
                        {
                            data[a + i] = raw[b + i];
                        }
                    }
                }
            }

            // for(int u = 0; u < offset; u++)
            // data[u] = raw[u];
            // for(int v = offset + (height * width * depth * bitCount); v < data.Length; v++)
            // 	data[v] = raw[v];
            return data;
        }

        /// <summary>
        /// The swizzle.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="bitCount">The bit count.</param>
        /// <param name="deswizzle">The deswizzle.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] Swizzle(byte[] raw, int width, int height, int depth, int bitCount, bool deswizzle)
        {
            return Swizzle(raw, 0, width, height, depth, bitCount, deswizzle);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The swizzle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="masks">The masks.</param>
        /// <returns>The swizzle.</returns>
        /// <remarks></remarks>
        private static int Swizzle(int x, int y, int z, MaskSet masks)
        {
            return SwizzleAxis(x, masks.x) | SwizzleAxis(y, masks.y) | (z == -1 ? 0 : SwizzleAxis(z, masks.z));
        }

        /// <summary>
        /// The swizzle axis.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="mask">The mask.</param>
        /// <returns>The swizzle axis.</returns>
        /// <remarks></remarks>
        private static int SwizzleAxis(int val, int mask)
        {
            int bit = 1;
            int result = 0;

            while (bit <= mask)
            {
                int test = mask & bit;
                if (test != 0)
                {
                    result |= val & bit;
                }
                else
                {
                    val <<= 1;
                }

                bit <<= 1;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// The mask set.
        /// </summary>
        /// <remarks></remarks>
        private class MaskSet
        {
            #region Constants and Fields

            /// <summary>
            /// The x.
            /// </summary>
            public readonly int x;

            /// <summary>
            /// The y.
            /// </summary>
            public readonly int y;

            /// <summary>
            /// The z.
            /// </summary>
            public readonly int z;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="MaskSet"/> class.
            /// </summary>
            /// <param name="w">The w.</param>
            /// <param name="h">The h.</param>
            /// <param name="d">The d.</param>
            /// <remarks></remarks>
            public MaskSet(int w, int h, int d)
            {
                int bit = 1;
                int index = 1;

                while (bit < w || bit < h || bit < d)
                {
                    // if (bit == 0) { break; }
                    if (bit < w)
                    {
                        x |= index;
                        index <<= 1;
                    }

                    if (bit < h)
                    {
                        y |= index;
                        index <<= 1;
                    }

                    if (bit < d)
                    {
                        z |= index;
                        index <<= 1;
                    }

                    bit <<= 1;
                }
            }

            #endregion
        }
    }
}