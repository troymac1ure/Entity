using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace H2Font
{
    class H2FontClass
    {
        /// <summary>
        /// Holds the data from the Index
        /// </summary>
        public class idClass
        {
            /// <summary>
            /// The Index number of the entry
            /// </summary>
            public int index { get; set; }
            /// <summary>
            /// Pointer to the entry in the UTF table
            /// </summary>
            public CharInfo cInfo { get; set; }

            public idClass(int Index)
            {
                index = Index;
                cInfo = null;
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(index);
            }
        }

        /// <summary>
        /// Data located in the UTF table
        /// </summary>
        public class CharInfo
        {
            public List<idClass> IdClasses { get; set; }
            public int Index { get; set; }
            public byte[] CompressedImageData { get; set; }
            public byte[] ImageData { get; set; }
            public PixelFormat PixelFormat { get; set; }
            public int Bpp { get; set; }

            /// <summary>
            ///  This is the width between characters when displayed in game
            ///  May be used to space out characters or have them overlap [unverified]
            /// </summary>
            public short DisplayWidth { get; set; }
            /// <summary>
            /// The total number of bytes in the run for the character
            /// </summary>
            public short Size { get; set; }
            /// <summary>
            /// The width used in calculations of the byte run
            /// </summary>
            public short Width { get; set; }
            /// <summary>
            /// The total height at the end of the byte run
            /// </summary>
            public short Height { get; set; }
            /// <summary>
            /// Unknown. Possibly used as a shift value (either vertical or horizontal?)
            /// </summary>
            public short FType { get; set; }
            public short Unk1 { get; set; }
            /// <summary>
            /// The location in the file of the data
            /// </summary>
            public int Offset { get; set; }

            public CharInfo(int index)
            {
                this.Index = index;
                this.ImageData = null;
                this.IdClasses = new List<idClass>();
                this.PixelFormat = PixelFormat.Format32bppArgb;
                this.Bpp = 4;
            }

            public void Read(BinaryReader br)
            {
                DisplayWidth = br.ReadInt16();
                Size = br.ReadInt16();
                Width = br.ReadInt16();
                Height = br.ReadInt16();
                FType = br.ReadInt16();
                Unk1 = br.ReadInt16();
                Offset = br.ReadInt32();
            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(DisplayWidth);
                bw.Write(Size);
                bw.Write(Width);
                bw.Write(Height);
                bw.Write(FType);
                bw.Write(Unk1);
                bw.Write(Offset);
            }

            public Bitmap getImage(bool forceImageReload)
            {
                if (ImageData == null || forceImageReload)
                {
                    decodeData();
                }
                // Do not set this to more than 1. Set the pictureBox width instead.
                Bitmap bm = new Bitmap(Math.Max(1, (int)Width), Height, PixelFormat);
                BitmapData bd = bm.LockBits(
                    new Rectangle(new Point(0, 0), bm.Size),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bm.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(ImageData, 0, bd.Scan0, ImageData.Length);
                bm.UnlockBits(bd);
                return bm;                
            }

            public byte[] getImageRaw()
            {
                return ImageData;
            }

            internal void decodeData()
            {
                int bytes = Width * Height * Bpp;
                ImageData = new byte[bytes];
                //for (int i = 0; i < ci.Data.Length; i++)
                //    ci.Data[i] = 0x80;

                int Y = 0;
                byte alpha = 0xff;
                byte red = 0xff;
                byte green = 0xff;
                byte blue = 0xff;

                int Count = 0;
                // Contains the data to be written as a percent of pure color (0-100)
                byte[] runData = new byte[0];

                for (int Z = 0; Z < CompressedImageData.Length; Z++)
                {
                    byte Code = CompressedImageData[Z];
                    if (Code == 0x00)
                    {
                        alpha = (byte)(((CompressedImageData[Z + 1] & 0xF0) >> 4) * 255 / 15);
                        red = (byte)((CompressedImageData[Z + 1] & 0x0F) * 255 / 15);
                        green = (byte)(((CompressedImageData[Z + 2] & 0xF0) >> 4) * 255 / 15);
                        blue = (byte)((CompressedImageData[Z + 2] & 0x0F) * 255 / 15);

                        Z += 2; // 2 Bytes following are color (A4R4G4B4) 

                        // Draw a single pixel with the color change
                        Code = 0x41;
                    }

                    if ((Code & 0x80) == 0x80)
                    {
                        switch (Code)
                        {
                            #region good codes (not 100%, but they all work decently)
                            case 0x80:
                                runData = new byte[] { 00 };
                                break;
                            case 0x81:
                                runData = new byte[] { 00, 100, 100, 100, 100 };
                                break;
                            case 0x82:
                                runData = new byte[] { 00, 100, 100, 100 };
                                break;
                            case 0x83:
                                runData = new byte[] { 00, 100, 100 };
                                break;
                            case 0x89:
                                runData = new byte[] { 00, 100, 100, 100, 100 };
                                break;
                            case 0x8A:
                                runData = new byte[] { 00, 100, 100, 100 };
                                break;
                            case 0x8B:
                                runData = new byte[] { 00, 100, 100 };
                                break;
                            case 0x8C:
                                runData = new byte[] { 25, 00, 00, 00, 00, 00 };
                                break;
                            case 0x8D:
                                runData = new byte[] { 25, 00, 00, 00, 00 };
                                break;
                            case 0x8E:
                                runData = new byte[] { 25, 00, 00, 00 };
                                break;
                            case 0x8F:
                                runData = new byte[] { 50, 00, 00 };
                                break;
                            case 0x91:
                                runData = new byte[] { 25, 100, 100, 100, 100 };
                                break;
                            case 0x92:
                                runData = new byte[] { 25, 100, 100, 100 };
                                break;
                            case 0x93:
                                runData = new byte[] { 25, 100, 100 };
                                break;
                            case 0x94:
                                runData = new byte[] { 50, 00, 00, 00, 00, 00 };
                                break;
                            case 0x95:
                                runData = new byte[] { 25, 00, 00, 00, 00 };
                                break;
                            case 0x96:
                                runData = new byte[] { 25, 00, 00, 00 };
                                break;
                            case 0x97:
                                runData = new byte[] { 25, 00, 00 };
                                break;
                            case 0x98:
                                runData = new byte[] { 25, 00 };
                                break;
                            case 0x99:
                                runData = new byte[] { 25, 100, 100, 100, 100 };
                                break;
                            case 0x9A:
                                runData = new byte[] { 25, 100, 100, 100 };
                                break;
                            case 0x9B:
                                runData = new byte[] { 25, 100, 100 };
                                break;
                            case 0x9C:
                                runData = new byte[] { 35, 00, 00, 00, 00, 00 };
                                break;
                            case 0x9D:
                                runData = new byte[] { 35, 00, 00, 00, 00 };
                                break;
                            case 0x9E:
                                runData = new byte[] { 35, 00, 00, 00 };
                                break;
                            case 0x9F:
                                runData = new byte[] { 35, 00, 00 };
                                break;
                            case 0xA0:
                                runData = new byte[] { 25 };
                                break;
                            case 0xA1:
                                runData = new byte[] { 25, 100, 100, 100, 100 };
                                break;
                            case 0xA2:
                                runData = new byte[] { 25, 100, 100, 100 };
                                break;
                            case 0xA3:
                                runData = new byte[] { 25, 100, 100 };
                                break;
                            case 0xA4:
                                runData = new byte[] { 50, 00, 00, 00, 00, 00 };
                                break;
                            case 0xA5:
                                runData = new byte[] { 50, 00, 00, 00, 00 };
                                break;
                            case 0xA6:
                                runData = new byte[] { 50, 00, 00, 00 };
                                break;
                            case 0xA7:
                                runData = new byte[] { 50, 00, 00 };
                                break;
                            case 0xA9:
                                runData = new byte[] { 50, 100, 100, 100, 100 };
                                break;
                            case 0xAA:
                                runData = new byte[] { 50, 100, 100, 100 };
                                break;
                            case 0xAB:
                                runData = new byte[] { 50, 100, 100 };
                                break;
                            case 0xAC:
                                runData = new byte[] { 50, 00, 00, 00, 00, 00 };
                                break;
                            case 0xAD:
                                runData = new byte[] { 50, 00, 00, 00, 00 };
                                break;
                            case 0xAE:
                                runData = new byte[] { 50, 00, 00, 00 };
                                break;
                            case 0xAF:
                                runData = new byte[] { 50, 00, 00 };
                                break;
                            case 0xB0:
                                runData = new byte[] { 50 };
                                break;
                            case 0xB1:
                                runData = new byte[] { 50, 100, 100, 100, 100 };
                                break;
                            case 0xB2:
                                runData = new byte[] { 50, 100, 100, 100 };
                                break;
                            case 0xB3:
                                runData = new byte[] { 50, 100, 100 };
                                break;
                            case 0xB4:
                                runData = new byte[] { 75, 00, 00, 00, 00, 00 };
                                break;
                            case 0xB5:
                                runData = new byte[] { 75, 00, 00, 00, 00 };
                                break;
                            case 0xB6:
                                runData = new byte[] { 75, 00, 00, 00 };
                                break;
                            case 0xB7:
                                runData = new byte[] { 50, 00, 00 };
                                break;
                            case 0xBC:
                                runData = new byte[] { 100, 00, 00, 00, 00, 00 };
                                break;
                            case 0xBD:
                                runData = new byte[] { 100, 00, 00, 00, 00 };
                                break;
                            case 0xBE:
                                runData = new byte[] { 100, 00, 00, 00 };
                                break;
                            case 0xBF:
                                runData = new byte[] { 100, 00, 00 };
                                break;
                            case 0xC1:
                                runData = new byte[] { 00, 15 };
                                break;
                            case 0xC2:
                                runData = new byte[] { 00, 25 };
                                break;
                            case 0xC3:
                                runData = new byte[] { 00, 35 };
                                break;
                            case 0xC4:
                                runData = new byte[] { 00, 50 };
                                break;
                            case 0xC5:
                                runData = new byte[] { 00, 60 };
                                break;
                            case 0xC6:
                                runData = new byte[] { 00, 75 };
                                break;
                            case 0xC7:
                                runData = new byte[] { 00, 100 };
                                break;
                            case 0xC8:
                                runData = new byte[] { 50, 0 };
                                break;
                            case 0xC9:
                                runData = new byte[] { 50, 50 };
                                break;
                            case 0xCA:
                                runData = new byte[] { 75, 50 };
                                break;
                            case 0xCB:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xCC:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xCD:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xCE:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xCF:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xD0:
                                runData = new byte[] { 25, 00 };
                                break;
                            case 0xD1:
                                runData = new byte[] { 25, 15 };
                                break;
                            case 0xD2:
                                runData = new byte[] { 50, 25 };
                                break;
                            case 0xD3:
                                runData = new byte[] { 50, 25 };
                                break;
                            case 0xD4:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xD5:
                                runData = new byte[] { 25, 75 };
                                break;
                            case 0xD6:
                                runData = new byte[] { 25, 75 };
                                break;
                            case 0xD7:
                                runData = new byte[] { 25, 100 };
                                break;
                            case 0xD8:
                                runData = new byte[] { 25, 00 };
                                break;
                            case 0xD9:
                                runData = new byte[] { 00, 25 };
                                break;
                            case 0xDA:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xDB:
                                runData = new byte[] { 25, 25 };
                                break;
                            case 0xDC:
                                runData = new byte[] { 50, 50 };
                                break;
                            case 0xDD:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xDE:
                                runData = new byte[] { 25, 25 };
                                break;
                            case 0xDF:
                                runData = new byte[] { 25, 100 };
                                break;
                            case 0xE0:
                                runData = new byte[] { 50, 00 };
                                break;
                            case 0xE1:
                                runData = new byte[] { 75, 00 };
                                break;
                            case 0xE2:
                                runData = new byte[] { 100, 00 };
                                break;
                            case 0xE3:
                                runData = new byte[] { 15, 50 };
                                break;
                            case 0xE4:
                                runData = new byte[] { 50, 50 };
                                break;
                            case 0xE5:
                                runData = new byte[] { 25, 100 };
                                break;
                            case 0xE6:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xE7:
                                runData = new byte[] { 25, 100 };
                                break;
                            case 0xE8:
                                runData = new byte[] { 50, 00 };
                                break;
                            case 0xE9:
                                runData = new byte[] { 100, 25 };
                                break;
                            case 0xEA:
                                runData = new byte[] { 75, 25 };
                                break;
                            case 0xEB:
                                runData = new byte[] { 50, 25 };
                                break;
                            case 0xEC:
                                runData = new byte[] { 50, 25 };
                                break;
                            case 0xED:
                                runData = new byte[] { 75, 50 };
                                break;
                            case 0xEE:
                                runData = new byte[] { 25, 50 };
                                break;
                            case 0xEF:
                                runData = new byte[] { 50, 100 };
                                break;
                            case 0xF0:
                                runData = new byte[] { 50, 00 };
                                break;
                            case 0xF1:
                                runData = new byte[] { 75, 35 };
                                break;
                            case 0xF2:
                                runData = new byte[] { 75, 50 };
                                break;
                            case 0xF3:
                                runData = new byte[] { 50, 25 };
                                break;
                            case 0xF4:
                                runData = new byte[] { 50, 50 };
                                break;
                            case 0xF5:
                                runData = new byte[] { 75, 50 };
                                break;
                            case 0xF6:
                                runData = new byte[] { 50, 50 };
                                break;
                            case 0xF7:
                                runData = new byte[] { 50, 100 };
                                break;
                            case 0xF8:
                                runData = new byte[] { 100, 00 };
                                break;
                            case 0xF9:
                                runData = new byte[] { 50, 00 };
                                break;
                            case 0xFA:
                                runData = new byte[] { 100, 25 };
                                break;
                            case 0xFB:
                                runData = new byte[] { 75, 25 };
                                break;
                            case 0xFC:
                                runData = new byte[] { 100, 25 };
                                break;
                            case 0xFD:
                                runData = new byte[] { 100, 50 };
                                break;
                            case 0xFE:
                                runData = new byte[] { 100, 50 };
                                break;
                            #endregion
                        }

                        // Draw coded pixel run (always draw @ 100% alpha?)
                        drawPixels(Y, runData, Color.FromArgb(0xff, red, green, blue));
                        Count = runData.Length;
                    }
                    else
                    {
                        // Calculate out run value (lower 6 bits [0-5])
                        Count = Code & 0x3F;

                        // Create a buffer of given size and fill with 100% pixel opacity
                        runData = new byte[Count];
                        for (int j = 0; j < runData.Length; j++)
                            runData[j] = 100;

                        // if bit[6] is set, draw run value, otherwise just 'skip' run length of pixels below
                        if ((Code & 0x40) == 0x40)
                        {
                            drawPixels(Y, runData, Color.FromArgb(alpha, red, green, blue));
                        }

                    }

                    // Offset location accordingly

                    Y = Y + Count;
                }
            }

            /// <summary>
            /// This will draw a pixel run into a bitmap's raw BitmapData in a A8R8G8B8 format
            /// </summary>
            /// <param name="db"></param>
            /// <param name="offset"></param>
            /// <param name="runData"></param>
            /// <param name="color"></param>
            private void drawPixels(int offset, byte[] runData, Color color)
            {
                for (int X = 0; X < runData.Length; X++)
                {
                    int x = (X + offset) % Width;
                    int y = (X + offset) / Width;
                    int stride = Width * this.Bpp;

                    if (((x * this.Bpp) + y * (stride)) >= ImageData.Length)
                        break;
                    ImageData[(x * this.Bpp) +
                          y * stride] = (byte)(color.B * runData[X] / 100);
                    ImageData[(x * this.Bpp + 1) +
                          y * stride] = (byte)(color.G * runData[X] / 100);
                    ImageData[(x * this.Bpp + 2) +
                          y * stride] = (byte)(color.R * runData[X] / 100);
                    ImageData[(x * this.Bpp + 3) +
                          y * stride] = (byte)(color.A * runData[X] / 100);
                }
            }
        }

        public class H2Font
        {

            private string _Name;
            public string Name { get { return _Name; } }
            private byte _Bpp = 0;
            public byte BitsPerPixel { get { return _Bpp; } }
            private idClass[] ids = new idClass[65536];
            /// <summary>
            /// The number of charInfo entries
            /// </summary>
            public int charInfoCount { get { return charInfos.Count; } }
            private bool _isLoaded;
            public bool isLoaded { get { return _isLoaded; } }

            // The CharInfo entries as listed in the file
            List<CharInfo> charInfos = new List<CharInfo>();
            // The CharInfo entries in index order
            CharInfo[] charInfoPos = new CharInfo[65536];
            MemoryStream ms;

            PixelFormat _pixelFormat;
            public PixelFormat pixelFormat
            {
                get
                {
                    return this._pixelFormat;
                }
                set
                {
                    this._pixelFormat = value;
                    this._Bpp = (byte)(Image.GetPixelFormatSize(value) / 8);
                    foreach (CharInfo ci in charInfos)
                    {
                        ci.Bpp = _Bpp;
                        ci.PixelFormat = pixelFormat;
                    }
                }
            }


            public H2Font(string name)
            {
                _Name = name;
                readFont(name);
                if (isLoaded) 
                    this.pixelFormat = PixelFormat.Format32bppArgb;
            }

            public override string ToString()
            {
                return _Name.Substring(_Name.LastIndexOf("\\") + 1);
            }

            public CharInfo getCharByIndex(int index)
            {
                return charInfos[index];
            }


            /// <summary>
            /// Loads a font file into memory, disposing of any previously loaded font first
            /// </summary>
            /// <param name="filename">The filename of the font to load</param>
            private void readFont(string filename)
            {
                try
                {
                    // Load the entrie file into a memory stream for quick access
                    FileStream fs = new FileStream(filename, FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);
                    this.ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length));
                    fs.Close();
                    br = new BinaryReader(ms);
                    br.BaseStream.Position = 0x020c;
                    int indexCount = br.ReadInt32();

                    #region Load UTF Table Entries
                    // Read individual character info for each Index entry
                    // Share any common UTF entries
                    for (int i = 0; i < indexCount; i++)
                    {
                        br.BaseStream.Position = 0x40400 + i * 0x10;
                        CharInfo cInfo = new CharInfo(i);
                        cInfo.Read(br);
                        charInfos.Add(cInfo);
                        br.BaseStream.Position = cInfo.Offset;
                        cInfo.CompressedImageData = br.ReadBytes(cInfo.Size);
                    }
                    #endregion

                    #region Load Index Table entries
                    // Read index table (4 bytes per entry)
                    for (int i = 0; i < ids.Length; i++)
                    {
                        br.BaseStream.Position = 0x400 + i * 4;
                        ids[i] = new idClass(br.ReadInt32());
                        ids[i].cInfo = charInfos[ids[i].index];
                        charInfoPos[ids[i].index] = charInfos[ids[i].index];
                        charInfos[ids[i].index].IdClasses.Add(ids[i]);
                    }
                    #endregion
                    _isLoaded = true;
                }
                catch
                {
                    _isLoaded = false;
                }

            }

            /// <summary>
            /// Same as loadCharInfo, but using a character reference as opposed to charInfo
            /// </summary>
            /// <param name="c"></param>
            /// <param name="decodeImage"></param>
            /// <returns></returns>
            private CharInfo loadUTFInfo(char c, bool decodeImage)
            {
                return loadCharInfo(ids[c].cInfo, decodeImage);
            }

            /// <summary>
            /// Loads the passed character data into a bitmap contained within a picturebox
            /// </summary>
            /// <param name="ci">The character info to load</param>
            /// <param name="decodeImage">True to decode image into bitmap readable data block</param>
            /// <returns></returns>
            private CharInfo loadCharInfo(CharInfo ci, bool decodeImage)
            {
                if (ci == null)
                    return null;

                ms.Position = ci.Offset;
                if (ci.CompressedImageData == null)
                {
                    ci.CompressedImageData = new byte[ci.Size];
                    ms.Read(ci.CompressedImageData, 0, ci.CompressedImageData.Length);
                }
                if (ci.ImageData == null && decodeImage)
                {
                    ci.decodeData();
                }
                return ci;
            }

            public Bitmap writeString(string text)
            {
                if (text == string.Empty)
                    return new Bitmap(1, 1);
                // Calculate required width & height for the final image
                int width = 0;
                int height = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    CharInfo ci = loadUTFInfo(c, true);
                    width += ci.DisplayWidth + ci.FType;
                    height = Math.Max(height, ci.Height);
                }

                byte[] Data = new byte[width * height * this.BitsPerPixel];
                int startX = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    CharInfo ci = charInfoPos[ids[text[i]].index];

                    for (int y = 0; y < ci.Height; y++)
                        for (int x = 0; x < ci.Width; x++)
                            for (int z = 0; z < BitsPerPixel; z++)
                                Data[((x + startX) + (y * width)) * BitsPerPixel + z] = ci.ImageData[(x + y * ci.Width) * BitsPerPixel + z];
                    startX += ci.DisplayWidth + ci.FType;
                }

                Bitmap bm = new Bitmap(width, height);
                BitmapData bd = bm.LockBits(
                    new Rectangle(new Point(0, 0), bm.Size),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bm.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(Data, 0, bd.Scan0, Data.Length);
                bm.UnlockBits(bd);
                return bm;
            }

            public Bitmap writeString(string text, int wrapWidth)
            {
                if (text == string.Empty)
                    return null;
                List<string> texts = new List<string>();
                List<int> heights = new List<int>();
                int textStart = 0;
                int tHeight = 0;
                int tWidth = 0;
                while (textStart < text.Length)
                {
                    int widthCount = 0;
                    int count = 0;
                    int height = 1;
                    CharInfo ci = charInfoPos[ids[text[count++]].index];
                    while (widthCount + ci.DisplayWidth + ci.FType < wrapWidth &&
                        textStart + count < text.Length)
                    {
                        if (text[textStart + count] == '\r' || text[textStart + count] == '\n')
                            break;
                        if (height < ci.Height)
                            height = ci.Height;
                        widthCount += ci.DisplayWidth + ci.FType;
                        ci = charInfoPos[ids[text[count++]].index];
                    }
                    if (textStart + count < text.Length)
                    {
                        if (text[textStart + count] != '\r' && text[textStart + count] != '\n')
                        {
                            if (text.Substring(textStart, count).Contains(' '))
                                while (text[textStart + --count] != ' ') ;
                        }
                        else
                            while (text[textStart + count] == '\r' || text[textStart + count] == '\n')
                            {
                                count++;
                            }
                    }

                    texts.Add(text.Substring(textStart, count).TrimStart(new char[] {' ','\r','\n'}).TrimEnd(new char[] {' ','\r','\n'}));
                    heights.Add(height);
                    tHeight += height;
                    if (widthCount + ci.DisplayWidth + ci.FType > tWidth)
                        tWidth = widthCount + ci.DisplayWidth + ci.FType;
                    textStart += count;
                }
                tWidth = Math.Min(tWidth + 10, wrapWidth);

                Bitmap textBitm = new Bitmap(tWidth, tHeight);
                using (Graphics g = Graphics.FromImage(textBitm))
                {
                    int height = 0;
                    for (int i = 0; i < texts.Count; i++)
                    {
                        Bitmap b = writeString(texts[i]);
                        g.DrawImage(b, new Point(0, height));
                        height += heights[i];
                        b.Dispose();
                    }
                }

                return textBitm;
            }

            public char addCharacter(int index, Image image)
            {
                Bitmap bm = (Bitmap)image;
                BitmapData bd = bm.LockBits(
                    new Rectangle(new Point(0, 0), bm.Size),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    bm.PixelFormat);
                int bytes  = Math.Abs(bd.Stride) * bm.Height;
                byte[] bmBytes = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, bmBytes, 0, bytes);
                bm.UnlockBits(bd);

                if (image.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    int sBpp = (Image.GetPixelFormatSize(bm.PixelFormat) / 8);
                    int stride = Math.Abs(bd.Stride);
                    byte[] bmBytes2 = new byte[bytes * 4 / sBpp];
                    for (int i = 0; i < bd.Height; i++)
                        for (int ii = 0; ii < bd.Width; ii++)
                        {
                            switch (image.PixelFormat)
                            {
                                case PixelFormat.Format8bppIndexed:
                                    int colorIndex = bmBytes[i * stride + ii * sBpp];
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 0] = bm.Palette.Entries[colorIndex].B;  // b
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 1] = bm.Palette.Entries[colorIndex].G;  // g 
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 2] = bm.Palette.Entries[colorIndex].R;  // r
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 3] = bm.Palette.Entries[colorIndex].A;  // a
                                    break;
                                case PixelFormat.Format24bppRgb:
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 0] = bmBytes[i * stride + ii * sBpp + 0];  // b
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 1] = bmBytes[i * stride + ii * sBpp + 1];  // g 
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 2] = bmBytes[i * stride + ii * sBpp + 2];  // r
                                    bmBytes2[(i * bm.Width * 4) + ii * 4 + 3] = 0xff;  // a
                                    break;
                                default:
                                    MessageBox.Show(image.PixelFormat.ToString() + " format not supported!");
                                    return '\u0000';

                            }
                        }
                    bmBytes = bmBytes2;
                    bytes = bm.Width * 4 * bm.Height;
                }

                #region Create a compressed image from image data
                List<byte> data = new List<byte>();
                // Not the same as Color.White due to Color.Name being different
                Color color = Color.FromArgb( 0xff, 0xff, 0xff, 0xff );

                for (int i = 0; i < bytes / 4; i++)
                {
                    byte run = 1;
                    byte blue = bmBytes[i * 4 + 0];
                    byte green = bmBytes[i * 4 + 1];
                    byte red = bmBytes[i * 4 + 2];
                    byte alpha = bmBytes[i * 4 + 3];
                    if (alpha == 0)
                    {
                        while ( (i + run) < bytes / 4 && bmBytes[(i + run) * 4 + 3] == 0 && run < 0x3F)
                            run++;
                        data.Add(run);
                    }
                    else
                    {
                        Color temp = Color.FromArgb(alpha, red, green, blue);
                        if (temp != color)
                        {
                            // Add color change (Remember this auto-draws one pixel as well
                            data.Add((byte)0x00);
                            data.Add((byte)(((alpha * 15 / 255) << 4) + (red * 15 / 255)));
                            data.Add((byte)(((green * 15 / 255) << 4) + (blue * 15 / 255)));
                            color = temp;
                        }
                        else
                        {
                            while ((i + run) < bytes / 4
                                && (Math.Abs(bmBytes[i * 4 + 0] - bmBytes[(i + run) * 4 + 0]) < 5)
                                && (Math.Abs(bmBytes[i * 4 + 1] - bmBytes[(i + run) * 4 + 1]) < 5)
                                && (Math.Abs(bmBytes[i * 4 + 2] - bmBytes[(i + run) * 4 + 2]) < 5)
                                && (Math.Abs(bmBytes[i * 4 + 3] - bmBytes[(i + run) * 4 + 3]) < 5)
                                && run < 0x3F)
                                run++;
                            data.Add((byte)(0x40 + run));
                        }
                    }
                    i += run - 1;
                }
                #endregion

                // Check if compressed size is too large to be imported
                if (data.Count > short.MaxValue)
                {
                    MessageBox.Show("Compressed image size must not be larger than " + short.MaxValue.ToString("n0") + " bytes\nCurrent image compresses to " + data.Count.ToString("n0") + " bytes");
                    return '\u0000';
                }
                
                // For Index Placements
                CharInfo ci;
                try
                {
                    ci = charInfos[index];
                    if (ci.Index != index)
                    {
                        ci = new CharInfo(index);
                        charInfos.Add(ci);
                    }
                }
                catch
                {
                    ci = new CharInfo(index);
                    charInfos.Add(ci);
                }
                ci.Width = (short)image.Width;
                ci.Height = (short)image.Height;
                ci.DisplayWidth = (short)(ci.Width - 2);
                ci.Unk1 = charInfos[0].Unk1;           // Seems to be same throughout file
                ci.Offset = (int)ms.Length + 0x10;     // New data will be at end of file + one charInfo entry
                ids[ci.Index].cInfo = ci;
                ci.IdClasses.Add(ids[ci.Index]);
                ci.CompressedImageData = data.ToArray();
                ci.Size = (short)ci.CompressedImageData.Length;
                return (char)ci.Index;
            }

            public void assignUTF(int utfIndex, int index)            
            {
                ids[utfIndex].cInfo = charInfos[index];
                ids[utfIndex].index = index;
                charInfos[index].IdClasses.Add(ids[utfIndex]);
            }

            public void writeFont(string Filename)
            {
                FileStream fs = new FileStream(Filename, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                BinaryReader br = new BinaryReader(ms);
                bw.BaseStream.Position = 0;
                br.BaseStream.Position = 0;
                bw.Write(br.ReadBytes(0x400));

                br.BaseStream.Position = 0x020C;
                int charCount = br.ReadInt32();
                bw.BaseStream.Position = 0x020C;
                bw.Write(charInfos.Count);
                bw.BaseStream.Position = 0x400;
                //int charDiff = (charInfos.Count - charCount) * 0x10;

                for (int i = 0; i < ids.Length; i++)
                {
                    ids[i].Write(bw);
                }

                int dataStart = 0x40400 + charInfos.Count * 0x10;

                // Should always be @ position 0x40400 now
                for (int i = 0; i < charInfos.Count; i++)
                {
                    loadCharInfo(charInfos[i], false);

                    charInfos[i].Offset = dataStart;
                    bw.BaseStream.Position = 0x40400 + i * 0x10;
                    charInfos[i].Write(bw);

                    bw.BaseStream.Position = charInfos[i].Offset;
                    bw.Write(charInfos[i].CompressedImageData);
                    dataStart += charInfos[i].CompressedImageData.Length;
                }

                fs.Close();
            }
        }
    }
}
