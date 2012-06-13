// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnicodeTableReader.cs" company="">
//   
// </copyright>
// <summary>
//   The unicode table reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// The unicode table reader.
    /// </summary>
    /// <remarks></remarks>
    public class UnicodeTableReader
    {
        #region Constants and Fields

        /// <summary>
        /// The ut.
        /// </summary>
        public UnicodeTable[] ut;

        /// <summary>
        /// The file path.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// The matg offset.
        /// </summary>
        private readonly int matgOffset;

        /// <summary>
        /// The startup path.
        /// </summary>
        //private string startupPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeTableReader"/> class.
        /// </summary>
        /// <param name="File">The file.</param>
        /// <param name="MatgOffset">The matg offset.</param>
        /// <remarks></remarks>
        public UnicodeTableReader(string File, int MatgOffset)
        {
            this.filePath = File;
            this.matgOffset = MatgOffset;
            this.GetOffsetsAndSizes(false);
        }

        #endregion

        #region Enums

        /// <summary>
        /// The language.
        /// </summary>
        /// <remarks></remarks>
        public enum Language
        {
            /// <summary>
            /// The english.
            /// </summary>
            English, 

            /// <summary>
            /// The japanese.
            /// </summary>
            Japanese, 

            /// <summary>
            /// The german.
            /// </summary>
            German, 

            /// <summary>
            /// The french.
            /// </summary>
            French, 

            /// <summary>
            /// The spanish.
            /// </summary>
            Spanish, 

            /// <summary>
            /// The italian.
            /// </summary>
            Italian, 

            /// <summary>
            /// The korean.
            /// </summary>
            Korean, 

            /// <summary>
            /// The chinese.
            /// </summary>
            Chinese, 

            /// <summary>
            /// The portuguese.
            /// </summary>
            Portuguese
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get offsets and sizes.
        /// </summary>
        /// <param name="scan">The scan.</param>
        /// <remarks></remarks>
        private void GetOffsetsAndSizes(bool scan)
        {
            ut = new UnicodeTable[9];
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            for (int counter = 0; counter < 9; counter++)
            {
                br.BaseStream.Position = matgOffset + 400 + 28 * counter;
                ut[counter] = new UnicodeTable(
                    false, 
                    counter, 
                    br.ReadInt32(), 
                    br.ReadInt32(), 
                    (int)br.BaseStream.Position - matgOffset, 
                    br.ReadInt32(), 
                    (int)br.BaseStream.Position - matgOffset, 
                    br.ReadInt32(), 
                    filePath);
            }

            br.Close();
            fs.Close();
        }

        #endregion

        /// <summary>
        /// The unicode table.
        /// </summary>
        /// <remarks></remarks>
        public class UnicodeTable
        {
            #region Constants and Fields

            /// <summary>
            /// The si ds.
            /// </summary>
            public sid[] SIDs;

            /// <summary>
            /// The us.
            /// </summary>
            public unicodeString[] US;

            /// <summary>
            /// The count.
            /// </summary>
            public int count;

            /// <summary>
            /// The index offset.
            /// </summary>
            public int indexOffset;

            /// <summary>
            /// The index pointer offset.
            /// </summary>
            public int indexPointerOffset;

            /// <summary>
            /// The language.
            /// </summary>
            public Language language;

            /// <summary>
            /// The scanned.
            /// </summary>
            public bool scanned;

            /// <summary>
            /// The table offset.
            /// </summary>
            public int tableOffset;

            /// <summary>
            /// The table pointer offset.
            /// </summary>
            public int tablePointerOffset;

            /// <summary>
            /// The table size.
            /// </summary>
            public int tableSize;

            /// <summary>
            /// The file path.
            /// </summary>
            private readonly string filePath;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="UnicodeTable"/> class.
            /// </summary>
            /// <param name="iscanned">if set to <c>true</c> [iscanned].</param>
            /// <param name="iLanguage">The i language.</param>
            /// <param name="icount">The icount.</param>
            /// <param name="itablesize">The itablesize.</param>
            /// <param name="iindexPointerOffset">The iindex pointer offset.</param>
            /// <param name="iindexOffset">The iindex offset.</param>
            /// <param name="itablePointerOffset">The itable pointer offset.</param>
            /// <param name="itableOffset">The itable offset.</param>
            /// <param name="ifilePath">The ifile path.</param>
            /// <remarks></remarks>
            public UnicodeTable(
                bool iscanned, 
                int iLanguage, 
                int icount, 
                int itablesize, 
                int iindexPointerOffset, 
                int iindexOffset, 
                int itablePointerOffset, 
                int itableOffset, 
                string ifilePath)
            {
                this.scanned = iscanned;
                this.language = (Language)iLanguage;
                this.count = icount;
                this.tableSize = itablesize;
                this.indexPointerOffset = iindexPointerOffset;
                this.indexOffset = iindexOffset;
                this.tablePointerOffset = itablePointerOffset;
                this.tableOffset = itableOffset;
                this.filePath = ifilePath;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public void Read()
            {
                SIDs = new sid[count];
                US = new unicodeString[count];
                ReadIndex();
                ReadTable();
            }

            #endregion

            #region Methods

            /// <summary>
            /// The read index.
            /// </summary>
            /// <remarks></remarks>
            private void ReadIndex()
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                br.BaseStream.Position = this.indexOffset;
                for (int counter = 0; counter < count; counter++)
                {
                    SIDs[counter] = new sid(br.ReadInt16(), br.ReadByte(), br.ReadByte());
                    US[counter] = new unicodeString(br.ReadInt32());
                }

                br.Close();
                fs.Close();
                for (int counter = 0; counter < count - 1; counter++)
                {
                    US[counter].size = US[counter + 1].offset - US[counter].offset - 1;
                }

                US[count - 1].size = tableSize - US[count - 1].offset;
            }

            /// <summary>
            /// The read table.
            /// </summary>
            /// <remarks></remarks>
            private void ReadTable()
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                Encoding decode = Encoding.Unicode;
                for (int counter = 0; counter < count; counter++)
                {
                    br.BaseStream.Position = this.tableOffset + US[counter].offset;
                    byte[] temp = br.ReadBytes(US[counter].size);
                    US[counter].uString = decode.GetString(temp);
                }
            }

            #endregion
        }

        /// <summary>
        /// The sid.
        /// </summary>
        /// <remarks></remarks>
        public class sid
        {
            #region Constants and Fields

            /// <summary>
            /// The id.
            /// </summary>
            public int id;

            /// <summary>
            /// The size.
            /// </summary>
            public byte size;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="sid"/> class.
            /// </summary>
            /// <param name="iid">The iid.</param>
            /// <param name="uselessByte">The useless byte.</param>
            /// <param name="isize">The isize.</param>
            /// <remarks></remarks>
            public sid(int iid, byte uselessByte, byte isize)
            {
                this.size = isize;
                this.id = iid;
            }

            #endregion
        }

        /// <summary>
        /// The unicode string.
        /// </summary>
        /// <remarks></remarks>
        public class unicodeString
        {
            #region Constants and Fields

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The size.
            /// </summary>
            public int size;

            /// <summary>
            /// The u string.
            /// </summary>
            public string uString;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="unicodeString"/> class.
            /// </summary>
            /// <param name="ioffset">The ioffset.</param>
            /// <remarks></remarks>
            public unicodeString(int ioffset)
            {
                this.offset = ioffset;
            }

            #endregion
        }
    }
}