// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileNamesInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The file names info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// The file names info.
    /// </summary>
    /// <remarks></remarks>
    public class FileNamesInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The file name strings offset.
        /// </summary>
        public int FileNameStringsOffset;

        /// <summary>
        /// The file name strings size.
        /// </summary>
        public int FileNameStringsSize;

        /// <summary>
        /// The length.
        /// </summary>
        public int[] Length;

        /// <summary>
        /// The name.
        /// </summary>
        public string[] Name;

        /// <summary>
        /// The offset.
        /// </summary>
        public int[] Offset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNamesInfo"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public FileNamesInfo(ref BinaryReader BR, Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    Halo2FileNamesInfo(ref BR, map);
                    break;
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    HaloCEFileNamesInfo(ref BR, map);
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The halo 2 file names info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void Halo2FileNamesInfo(ref BinaryReader BR, Map map)
        {
            Name = new string[map.IndexHeader.metaCount];
            Length = new int[map.MapHeader.fileCount];
            Offset = new int[map.MapHeader.fileCount];

            // 	Reads In Offsets Of	Meta Names
            BR.BaseStream.Position = map.MapHeader.offsetTofileIndex;
            for (int x = 0; x < map.MapHeader.fileCount; x++)
            {
                Offset[x] = BR.ReadInt32();
            }

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if (x < map.MapHeader.fileCount)
                {
                    bool brokenMap = false;

                    // figures out length of string
                    int len;
                    if (x != map.MapHeader.fileCount - 1)
                    {
                        len = Offset[x + 1] - Offset[x];

                        // For broken maps..
                        if (len <= 0)
                        {
                            brokenMap = true;
                            len = 1000;
                            for (int temp = 0; temp < map.MapHeader.fileCount; temp++)
                            {
                                if ((Offset[temp] > Offset[x]) && (len > (Offset[temp] - Offset[x])))
                                {
                                    len = Offset[temp] - Offset[x];
                                }
                            }
                        }
                    }
                    else
                    {
                        len = map.MapHeader.fileNamesSize - Offset[x];
                    }

                    Length[x] = len - 1;

                    ///	Reads in string
                    BR.BaseStream.Position = map.MapHeader.offsetTofileNames + Offset[x];
                    Name[x] = new string(BR.ReadChars(len - 1));

                    // more for broken maps..
                    if (brokenMap)
                    {
                        MessageBox.Show("This map has broken Strings. An attempt will be made to fix it.");
                    }

                    int z;
                    for (z = 0; z < len - 1; z++)
                    {
                        if (Name[x][z] == 0)
                        {
                            break;
                        }
                    }

                    if (z < Length[x])
                    {
                        Length[x] = z;
                        Name[x] = Name[x].Remove(z);
                    }
                }
                else
                {
                    int tempint = x - map.MapHeader.fileCount;
                    Name[x] = "Unknown Meta (" + tempint + ")";
                }
            }
        }

        /// <summary>
        /// The halo ce file names info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void HaloCEFileNamesInfo(ref BinaryReader BR, Map map)
        {
            Name = new string[map.IndexHeader.metaCount];
            Length = new int[map.IndexHeader.metaCount];
            Offset = new int[map.IndexHeader.metaCount];

            // 	Reads In Offsets Of	Meta Names
            BR.BaseStream.Position = map.MapHeader.offsetTofileIndex;
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                Offset[x] = map.MetaInfo.stringoffset[x];
            }

            if (map.HaloVersion == HaloVersionEnum.Halo1)
            {
                FileNameStringsOffset = map.MapHeader.indexOffset + 36 + (map.IndexHeader.metaCount * 32);
                FileNameStringsSize = map.IndexHeader.ModelRawDataOffset - FileNameStringsOffset;
            }
            else
            {
                FileNameStringsOffset = map.MapHeader.indexOffset + 40 + (map.IndexHeader.metaCount * 32);
                FileNameStringsSize = map.MetaInfo.Offset[0] - FileNameStringsOffset;
            }

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                // figures out length of string
                int len;
                if (x != map.IndexHeader.metaCount - 1)
                {
                    len = Offset[x + 1] - Offset[x];
                }
                else
                {
                    len = FileNameStringsOffset + FileNameStringsSize - Offset[x];
                }

                Length[x] = len - 1;

                ///	Reads in string
                BR.BaseStream.Position = Offset[x];
                Name[x] = new string(BR.ReadChars(len - 1));
            }
        }

        #endregion
    }
}