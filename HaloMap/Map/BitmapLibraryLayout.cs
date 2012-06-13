// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapLibraryLayout.cs" company="">
//   
// </copyright>
// <summary>
//   The bitmap library layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;

    /// <summary>
    /// The bitmap library layout.
    /// </summary>
    /// <remarks></remarks>
    public class BitmapLibraryLayout
    {
        #region Constants and Fields

        /// <summary>
        /// The bitmap count.
        /// </summary>
        public int BitmapCount;

        /// <summary>
        /// The bitmap index offset.
        /// </summary>
        public int BitmapIndexOffset;

        /// <summary>
        /// The file name strings offset.
        /// </summary>
        public int FileNameStringsOffset;

        /// <summary>
        /// The file size.
        /// </summary>
        public int FileSize;

        /// <summary>
        /// The meta offset.
        /// </summary>
        public int[] MetaOffset;

        /// <summary>
        /// The meta size.
        /// </summary>
        public int[] MetaSize;

        /// <summary>
        /// The name.
        /// </summary>
        public string[] Name;

        /// <summary>
        /// The raw offset.
        /// </summary>
        public int[] RawOffset;

        /// <summary>
        /// The raw size.
        /// </summary>
        public int[] RawSize;

        /// <summary>
        /// The string length.
        /// </summary>
        public int[] StringLength;

        /// <summary>
        /// The string offset.
        /// </summary>
        public int[] StringOffset;

        /// <summary>
        /// The error.
        /// </summary>
        public bool error;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapLibraryLayout"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public BitmapLibraryLayout(Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.HaloCE:

                    // try
                    // {
                    HaloCEBitmapLibraryLayout(ref map.BR);

                    // }
                    // catch
                    // {
                    // error = true;
                    // }
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The halo ce bitmap library layout.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <remarks></remarks>
        public void HaloCEBitmapLibraryLayout(ref BinaryReader BR)
        {
            FileSize = (int)BR.BaseStream.Length;
            BR.BaseStream.Position = 4;
            FileNameStringsOffset = BR.ReadInt32();
            BitmapIndexOffset = BR.ReadInt32();
            BitmapCount = BR.ReadInt32();
            StringLength = new int[BitmapCount];
            StringOffset = new int[BitmapCount];
            MetaOffset = new int[BitmapCount];
            MetaSize = new int[BitmapCount];
            RawOffset = new int[BitmapCount];
            RawSize = new int[BitmapCount];
            BR.BaseStream.Position = BitmapIndexOffset;
            for (int x = 0; x < BitmapCount; x++)
            {
                StringOffset[x] = BR.ReadInt32();
                RawSize[x] = BR.ReadInt32();
                RawOffset[x] = BR.ReadInt32();
                if (x != 0)
                {
                    StringLength[x - 1] = StringOffset[x] - StringOffset[x - 1] - 1;
                    if (x == BitmapCount - 1)
                    {
                        StringLength[x] = BitmapIndexOffset - StringOffset[x] - 1;
                    }
                }
            }

            //int xx = 0;
            Name = new string[BitmapCount];
            for (int x = 0; x < BitmapCount; x++)
            {
                BR.BaseStream.Position = FileNameStringsOffset + StringOffset[x];
                Name[x] = new string(BR.ReadChars(StringLength[x]));
            }
        }

        #endregion
    }
}