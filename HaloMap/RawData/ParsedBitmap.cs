// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParsedBitmap.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for Bitmap.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RawData
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using HaloMap.DDSFunctions;
    using HaloMap.Map;
    using HaloMap.Meta;

    using Globals;

    /// <summary>
    /// Summary description for Bitmap.
    /// </summary>
    /// <remarks></remarks>
    public class ParsedBitmap
    {
        #region Constants and Fields

        /// <summary>
        /// The bitmap header.
        /// </summary>
        public HaloBitmapHeader BitmapHeader;

        /// <summary>
        /// The properties.
        /// </summary>
        public BitmapInfo[] Properties;

        /// <summary>
        /// The headersize.
        /// </summary>
        public int headersize;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedBitmap"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public ParsedBitmap(ref Meta meta, Map map)
        {
            BitmapHeader = new HaloBitmapHeader(ref meta);
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    ReadH2ParsedBitmap(ref meta);
                    break;
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    ReadCEParsedBitmap(ref meta);
                    break;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// The bitmap format.
        /// </summary>
        /// <remarks></remarks>
        public enum BitmapFormat
        {
            /// <summary>
            /// The bit m_ forma t_ a 8.
            /// </summary>
            BITM_FORMAT_A8 = 0, 

            /// <summary>
            /// The bit m_ forma t_ y 8.
            /// </summary>
            BITM_FORMAT_Y8 = 1, 

            /// <summary>
            /// The bit m_ forma t_ a y 8.
            /// </summary>
            BITM_FORMAT_AY8 = 2, 

            /// <summary>
            /// The bit m_ forma t_ a 8 y 8.
            /// </summary>
            BITM_FORMAT_A8Y8 = 3, 

            /// <summary>
            /// The bit m_ forma t_ r 5 g 6 b 5.
            /// </summary>
            BITM_FORMAT_R5G6B5 = 6, 

            /// <summary>
            /// The bit m_ forma t_ a 1 r 5 g 5 b 5.
            /// </summary>
            BITM_FORMAT_A1R5G5B5 = 8, 

            /// <summary>
            /// The bit m_ forma t_ a 4 r 4 g 4 b 4.
            /// </summary>
            BITM_FORMAT_A4R4G4B4 = 9, 

            /// <summary>
            /// The bit m_ forma t_ x 8 r 8 g 8 b 8.
            /// </summary>
            BITM_FORMAT_X8R8G8B8 = 10, 

            /// <summary>
            /// The bit m_ forma t_ a 8 r 8 g 8 b 8.
            /// </summary>
            BITM_FORMAT_A8R8G8B8 = 11, 

            /// <summary>
            /// The bit m_ forma t_ dx t 1.
            /// </summary>
            BITM_FORMAT_DXT1 = 14, 

            /// <summary>
            /// The bit m_ forma t_ dx t 2 an d 3.
            /// </summary>
            BITM_FORMAT_DXT2AND3 = 15, 

            /// <summary>
            /// The bit m_ forma t_ dx t 4 an d 5.
            /// </summary>
            BITM_FORMAT_DXT4AND5 = 16, 

            /// <summary>
            /// The bit m_ forma t_ p 8.
            /// </summary>
            BITM_FORMAT_P8 = 17, 

            /// <summary>
            /// The bit m_ forma t_ lightmap.
            /// </summary>
            BITM_FORMAT_LIGHTMAP = 18, 

            /// <summary>
            /// The bit m_ forma t_ g 8 b 8.
            /// </summary>
            BITM_FORMAT_G8B8 = 22, // V8U8
            
            /// <summary>
            /// Unknown format, but very closely resembles G8B8
            /// </summary>
            BITM_FORMAT_UNKNOWN = 23 // ???
        }

        /// <summary>
        /// The bitmap type.
        /// </summary>
        /// <remarks></remarks>
        public enum BitmapType
        {
            /// <summary>
            /// The bit m_ typ e_2 d.
            /// </summary>
            BITM_TYPE_2D = 0, 

            /// <summary>
            /// The bit m_ typ e_3 d.
            /// </summary>
            BITM_TYPE_3D = 1, 

            /// <summary>
            /// The bit m_ typ e_ cubemap.
            /// </summary>
            BITM_TYPE_CUBEMAP = 2, 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The decode bitm.
        /// </summary>
        /// <param name="bitmBytes">The bitm bytes.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="bitsPerPixel">The bits per pixel.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="swizzle">The swizzle.</param>
        /// <param name="map">The map.</param>
        /// <param name="visualchunkindex">The visualchunkindex.</param>
        /// <param name="ident">The ident.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        ///   </exception>
        ///   
        /// <exception cref="Exception">
        ///   </exception>
        /// <remarks></remarks>
        public static Bitmap DecodeBitm(
            byte[] bitmBytes, 
            int height, 
            int width, 
            int depth, 
            int bitsPerPixel, 
            BitmapType type, 
            BitmapFormat format, 
            bool swizzle, 
            Map map, 
            int visualchunkindex, 
            int ident)
        {
            int stride = 0;

            #region Volume Textures / 3D
            if (type == BitmapType.BITM_TYPE_3D)
            {                
                List<Bitmap> images = new List<Bitmap>();
                Bitmap finalImage = null;
                List<IntPtr> tPtr = new List<IntPtr>();
                if (swizzle)
                {
                    bitmBytes = Swizzler.Swizzle(bitmBytes, width, height, depth, bitsPerPixel, true);
                }

                try
                {
                    // Make our new image large enough to handle a square of all the images together with a 2 pixel pad between
                    int imageSize = width * height * bitsPerPixel >> 3;
                    int rCount = (int)(Math.Sqrt(depth) + 0.5);
                    int tWidth = rCount * (width + 1);
                    int tHeight = (int)Math.Round(depth / rCount + 0.5f, MidpointRounding.AwayFromZero) * (height + 2);

                    // Display 3D bitmaps combined as a 2D bitmap
                    if (visualchunkindex == 0)
                    {
                        for (int i = 0; i < depth; i++)
                        {
                            byte[] tempBytes = new byte[imageSize];
                            Array.Copy(bitmBytes, i * imageSize, tempBytes, 0, imageSize);

                            stride = DecodeBitmap(
                                ref tempBytes,
                                height,
                                width,
                                1,
                                bitsPerPixel,
                                type,
                                format,
                                false,
                                map,
                                visualchunkindex,
                                ident);

                            tPtr.Add(Marshal.AllocHGlobal(tempBytes.Length));
                            RtlMoveMemory(tPtr[tPtr.Count - 1], tempBytes, tempBytes.Length);
                            Bitmap bitmap = new Bitmap(
                                width, height, stride, PixelFormat.Format32bppArgb, tPtr[tPtr.Count - 1]);

                            images.Add(bitmap);
                        }
                    }
                    // Display only one of the 3D images
                    else
                    {
                        byte[] tempBytes = new byte[imageSize];
                        Array.Copy(bitmBytes, (visualchunkindex - 1) * imageSize, tempBytes, 0, imageSize);

                        stride = DecodeBitmap(
                            ref tempBytes,
                            height,
                            width,
                            1,
                            bitsPerPixel,
                            type,
                            format,
                            false,
                            map,
                            visualchunkindex,
                            ident);
                        tPtr.Add(Marshal.AllocHGlobal(tempBytes.Length));
                        RtlMoveMemory(tPtr[tPtr.Count - 1], tempBytes, tempBytes.Length);
                        images.Add(new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, tPtr[tPtr.Count - 1]));
                        tWidth = width;
                        tHeight = height;
                    }

                    // create a bitmap to hold the combined image
                    finalImage = new Bitmap(tWidth, tHeight);

                    // get a graphics object from the image so we can draw on it
                    using (Graphics g = Graphics.FromImage(finalImage))
                    {
                        // set background color
                        g.Clear(Color.Empty);

                        // go through each image and draw it on the final image
                        int offset = 0;
                        foreach (Bitmap image in images)
                        {
                            g.DrawImage(
                                image,
                                new Rectangle(
                                    offset % tWidth, (offset / tWidth) * (image.Height + 1), image.Width, image.Height));
                            offset += image.Width + 1;
                        }
                    }
                    return finalImage;

                }
                catch (Exception ex)
                {
                    if (finalImage != null)
                    {
                        finalImage.Dispose();
                    }

                    throw ex;
                    //Global.ShowErrorMsg("Error loading bitmap", ex);
                }
                finally
                {
                    // clean up memory
                    foreach (Bitmap image in images)
                    {
                        image.Dispose();
                    }

                    foreach (IntPtr p in tPtr)
                    {
                        Marshal.FreeHGlobal(p);
                    }
                }
            }
            #endregion
            #region Cubemap
            else if (type == BitmapType.BITM_TYPE_CUBEMAP)
            {
                // stride = DecodeBitmap(ref bitmBytes, height, width, 1, bitsPerPixel, type, format, swizzle, map, visualchunkindex, ident);
                List<Bitmap> images = new List<Bitmap>();
                Bitmap finalImage = null;
                List<IntPtr> tPtr = new List<IntPtr>();

                // Don't think any cubemaps are swizzled, but...
                if (swizzle)
                {
                    int imageSize = bitmBytes.Length / 6; // width * height * bitsPerPixel >> 3;
                    for (int i = 0; i < 6; i++)
                    {
                        bitmBytes = Swizzler.Swizzle(bitmBytes, i * imageSize, width, height, depth, bitsPerPixel, true);
                    }
                }

                try
                {
                    // Make our new image large enough to handle a square of all the images together with a 2 pixel pad between
                    //int tWidth = 2 * (width + 2) + 10; // Add some extras..
                    int tWidth = 4 * width; // Add some extras..
                    int tHeight = 3 * height;
                    int imageSize = width * height * (bitsPerPixel >> 3);

                    // Total image size has each image (including it's mipmaps) padded to 256
                    int tImageSize = bitmBytes.Length / 6;

                    // Unused, just divide stream size by 6 as above
                    int ttImageSize = imageSize +
                                      ((imageSize / 6) % 256 == 0 ? 0 : ((256 * 6) - (imageSize % (256 * 6))));

                    // All cubemaps should be DXT1...
                    switch (format)
                    {
                        case BitmapFormat.BITM_FORMAT_DXT1:
                            imageSize = Math.Max(imageSize / 8, 8);
                            break;
                        case BitmapFormat.BITM_FORMAT_DXT2AND3:
                        case BitmapFormat.BITM_FORMAT_DXT4AND5:
                            imageSize = Math.Max(imageSize / 4, 16);
                            break;
                    }

                    //int mipmaps = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        int tw = width;
                        int th = height;
                        int tempSize = imageSize;
                        int offset = 0;
                        //while (tw > 2 & th > 2)
                        {
                            byte[] tempBytes = new byte[tempSize];

                            Array.Copy(bitmBytes, i * tImageSize + offset, tempBytes, 0, tempSize);

                            stride = DecodeBitmap(
                                ref tempBytes, 
                                th, 
                                tw, 
                                1, 
                                bitsPerPixel, 
                                type, 
                                format, 
                                false, 
                                map, 
                                visualchunkindex, 
                                ident);

                            tPtr.Add(Marshal.AllocHGlobal(tempBytes.Length));
                            RtlMoveMemory(tPtr[tPtr.Count - 1], tempBytes, tempBytes.Length);
                            Bitmap bitmap = new Bitmap(
                                tw, th, stride, PixelFormat.Format32bppArgb, tPtr[tPtr.Count - 1]);

                            images.Add(bitmap);
                            offset += tempSize;
                            tempSize /= 4;
                            tw /= 2;
                            th /= 2;
                        }
                    }

                    // create a bitmap to hold the combined image
                    finalImage = new Bitmap(tWidth, tHeight);

                    // get a graphics object from the image so we can draw on it
                    using (Graphics g = Graphics.FromImage(finalImage))
                    {
                        // set background color
                        g.Clear(Color.Empty);

                        //
                        int[] crossX = new int[6] { 2, 0, 1, 1, 1, 3 }; // Right, Left, Top, Bottom, Front, Back
                        int[] crossY = new int[6] { 1, 1, 0, 2, 1, 1 }; // Right, Left, Top, Bottom, Front, Back
                        // go through each image and draw it on the final image
                        int xOffset = 0;
                        int yOffset = 0;
                        int tempCount = 0;
                        foreach (Bitmap image in images)
                        {
                            /*
                            if (mipmapCount == 0)
                            {
                                xOffset = 0;
                                yOffset += image.Height + 2;
                                mipmapCount = images.Count / 6;
                            }
                            */
                            xOffset = crossX[tempCount] * image.Width;
                            yOffset = crossY[tempCount] * image.Height;

                            g.DrawImage(image, new Rectangle(xOffset, yOffset, image.Width, image.Height));
                            tempCount++;
                        }
                    }

                    return finalImage;
                }
                catch (Exception ex)
                {
                    if (finalImage != null)
                    {
                        finalImage.Dispose();
                    }

                    throw ex;
                    //Global.ShowErrorMsg("Error while processing bitmap", ex);
                }
                finally
                {
                    // clean up memory
                    foreach (Bitmap image in images)
                    {
                        image.Dispose();
                    }

                    foreach (IntPtr p in tPtr)
                    {
                        Marshal.FreeHGlobal(p);
                    }
                }
            }
            #endregion
            #region 2D Textures
            else
            {
                stride = DecodeBitmap(
                    ref bitmBytes, height, width, 1, bitsPerPixel, type, format, swizzle, map, visualchunkindex, ident);
            }
            #endregion

            IntPtr intPtr = Marshal.AllocHGlobal(bitmBytes.Length);
            RtlMoveMemory(intPtr, bitmBytes, bitmBytes.Length);
            Bitmap temp = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, intPtr);

            // This almost works, but some (stretched) bitmaps have 'blackouts' (ex. map floors)
            // Bitmap final = new Bitmap(temp);
            /*
            Bitmap final = new System.Drawing.Bitmap(temp.Width, temp.Height);
            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(final))
            {
                //set background color
                //g.Clear(System.Drawing.Color.Empty);
                g.DrawImage(temp, new System.Drawing.Rectangle(0, 0, temp.Width, temp.Height));
            }
            */
            // ******************* This is a huge memory leak! *************************//
            // Marshal.FreeHGlobal(ptr);
            // ******************* This is a huge memory leak! *************************//
            return temp;
        }

        /// <summary>
        /// The bitmap internalize.
        /// </summary>
        /// <param name="bitmapMeta">The bitmap meta.</param>
        /// <remarks></remarks>
        public static void bitmapInternalize(Meta bitmapMeta)
        {
            if (bitmapMeta.raw.rawChunks[0].rawLocation == MapTypes.Internal)
            {
                return;
            }

            string sharedMapPath = string.Empty;
            switch (bitmapMeta.raw.rawChunks[0].rawLocation)
            {
                case MapTypes.Internal:
                    sharedMapPath = bitmapMeta.Map.filePath;
                    break;
                case MapTypes.Bitmaps:
                    sharedMapPath = Prefs.pathBitmaps;
                    break;
                case MapTypes.MainMenu:
                    sharedMapPath = Prefs.pathMainmenu;
                    break;
                case MapTypes.MPShared:
                    sharedMapPath = Prefs.pathShared;
                    break;
                case MapTypes.SPShared:
                    sharedMapPath = Prefs.pathSPShared;
                    break;
            }

            Map sharedMap = Map.LoadFromFile(sharedMapPath);

            //long sizeToShift = 0;

            sharedMap.OpenMap(MapTypes.Internal);
            bitmapMeta.Map.OpenMap(MapTypes.Internal, false);

            // Find size of aall raw chunks and insert that amount into our bitmapMeta.Map
            long rawSize = biGetRawTotalSize(bitmapMeta.raw, true);
            shift(bitmapMeta.Map.FS, bitmapMeta.Map.MapHeader.indexOffset, rawSize);

            long offsetInShared;
            long offsetToWrite = bitmapMeta.Map.MapHeader.indexOffset;
            bitmapMeta.Map.BW.BaseStream.Position = offsetToWrite;
            byte[] rawData;

            // loop for each tag
            int tempRawSize;

            // loop for each chunk
            for (int j = 0; j < bitmapMeta.raw.rawChunks.Count; j++)
            {
                // loop for each LOD
                {
                    // for (int k = 0; k < bitmapMeta.raw.rawChunks.lod.Count; k++)
                    tempRawSize = bitmapMeta.raw.rawChunks[j].size; // .lods.get(k).rawSize;
                    offsetInShared = bitmapMeta.raw.rawChunks[j].offset;
                    rawData = new byte[tempRawSize];

                    // RandomAccessFileSP shared = sharedMaps.get(selectedTag.bitmChunks[j].rawLocation);

                    // reads the data into the array
                    sharedMap.BR.BaseStream.Position = offsetInShared;
                    sharedMap.BR.BaseStream.Read(rawData, 0, rawData.Length);

                    bitmapMeta.raw.rawChunks[j].offset = (int)bitmapMeta.Map.BW.BaseStream.Position;
                    bitmapMeta.raw.rawChunks[j].rawLocation = MapTypes.Internal;

                    bitmapMeta.Map.BW.Write(rawData);

                    // writes padding after chunk
                    int paddingSize = genPaddingSize(tempRawSize, 512);
                    bitmapMeta.Map.BW.Write(genPadding(paddingSize, 0));

                    offsetToWrite += tempRawSize + paddingSize;
                }
            }

            // fix pointers to point to internal bitmapMeta.Map
            for (int j = 0; j < bitmapMeta.raw.rawChunks.Count; j++)
            {
                {
                    // for (int k = 0; k < selectedTag.bitmChunks[j].lods.size(); k++)
                    bitmapMeta.Map.BW.BaseStream.Position = bitmapMeta.offset + rawSize +
                                                 bitmapMeta.raw.rawChunks[j].pointerMetaOffset; // +28; //+ (k * 4);
                    bitmapMeta.Map.BW.Write(bitmapMeta.raw.rawChunks[j].offset); // 37063680
                }
            }

            biUpdateMapInfo(bitmapMeta.Map, (int)rawSize);

            sharedMap.CloseMap();
            bitmapMeta.Map.CloseMap();
        }

        /// <summary>
        /// Internalizes a bitmap by passing raw data. This was primarily written for the bitmap internalization
        /// in the mainmenu editor.
        /// </summary>
        /// <param name="raw">Entity.RawDataContainer</param>
        /// <param name="map">Map to add data to</param>
        /// <remarks></remarks>
        public static void bitmapInternalizeRaw(ref RawDataContainer raw, Map map)
        {
            map.OpenMap(MapTypes.Internal, false);
            long rawSize = biGetRawTotalSize(raw, false);
            shift(map.FS, map.MapHeader.indexOffset, rawSize);

            //long offsetInShared;
            long offsetToWrite = map.MapHeader.indexOffset;
            map.BW.BaseStream.Position = offsetToWrite;
            //byte[] rawData;

            // loop for each tag
            //int tempRawSize;

            // loop for each chunk
            for (int j = 0; j < raw.rawChunks.Count; j++)
            {
                // loop for each LOD
                {
                    // for (int k = 0; k < bitmapMeta.raw.rawChunks.lod.Count; k++)
                    raw.rawChunks[j].offset = (int)map.BW.BaseStream.Position;
                    raw.rawChunks[j].rawLocation = MapTypes.Internal;

                    map.BW.Write(raw.rawChunks[j].MS.ToArray(), 0, raw.rawChunks[j].size);

                    // writes padding after chunk
                    int paddingSize = genPaddingSize(raw.rawChunks[j].size, 512);
                    map.BW.Write(genPadding(paddingSize, 0));

                    offsetToWrite += raw.rawChunks[j].size + paddingSize;
                }
            }

            biUpdateMapInfo(map, (int)rawSize);

            map.CloseMap();
        }

        /// <summary>
        /// The convert c eto h 2 parsed bitmap.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void ConvertCEtoH2ParsedBitmap(ref Meta meta, Map map)
        {
            meta.MS = new MemoryStream();
            meta.items.Clear();
            BinaryWriter BW = new BinaryWriter(meta.MS);
            BitmapHeader.WriteHeaderHaloBitmapHeader(ref BW);
            BW.BaseStream.Position = 32;
            BW.Write(new char[44]);
            int size = 76;

            int tempc = 1;
            int tempr = meta.offset + size + meta.magic;
            BW.BaseStream.Position = 60;
            BW.Write(tempc);
            BW.Write(tempr);
            Meta.Reflexive reflex = new Meta.Reflexive();
            reflex.offset = 60;
            reflex.translation = size;

            reflex.intag = meta.TagIndex;
            reflex.intagname = meta.name;
            reflex.intagtype = meta.type;

            reflex.chunkcount = tempc;
            reflex.pointstoTagIndex = meta.TagIndex;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            meta.items.Add(reflex);

            tempc = 1;
            tempr = meta.offset + size + 60 + meta.magic;
            BW.BaseStream.Position = 128;
            BW.Write(tempc);
            BW.Write(tempr);
            reflex = new Meta.Reflexive();
            reflex.offset = 128;
            reflex.translation = size + 60;

            reflex.intag = meta.TagIndex;
            reflex.intagname = meta.name;
            reflex.intagtype = meta.type;

            reflex.chunkcount = tempc;
            reflex.pointstoTagIndex = meta.TagIndex;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            meta.items.Add(reflex);
            BW.BaseStream.Position = size;
            BW.Write(new char[92]);
            size += 92;

            tempc = Properties.Length;
            tempr = meta.offset + size + meta.magic;
            BW.BaseStream.Position = 68;
            BW.Write(tempc);
            BW.Write(tempr);
            reflex = new Meta.Reflexive();
            reflex.offset = 68;
            reflex.translation = size;

            reflex.intag = meta.TagIndex;
            reflex.intagname = meta.name;
            reflex.intagtype = meta.type;

            reflex.chunkcount = tempc;
            reflex.pointstoTagIndex = meta.TagIndex;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            reflex.pointstotagname = meta.name;
            reflex.pointstotagtype = meta.type;
            meta.items.Add(reflex);

            for (int x = 0; x < tempc; x++)
            {
                BW.BaseStream.Position = size + (x * 116);
                BW.Write(new char[116]);

                BW.BaseStream.Position = size + (x * 116);
                Properties[x].Write(ref BW);
                int neg = -1;
                meta.raw.rawChunks[x].pointerMetaOffset = size + (x * 116) + 28;
                BW.BaseStream.Position = meta.raw.rawChunks[x].pointerMetaOffset + 4;
                BW.Write(neg);
                BW.Write(neg);
                BW.Write(neg);
                BW.Write(neg);
                BW.Write(neg);
                BW.Write(meta.raw.rawChunks[x].size);
                Meta.Ident id = new Meta.Ident();
                id.offset = size + (x * 116) + 76;
                BW.BaseStream.Position = id.offset;
                BW.Write(meta.ident);
                id.pointstoTagIndex = meta.TagIndex;
                id.pointstotagname = meta.name;
                id.pointstotagtype = meta.type;
                id.intag = meta.TagIndex;
                id.intagname = meta.name;
                id.intagtype = meta.type;
                id.ident = meta.ident;
                meta.items.Add(id);
            }

            meta.size = size + (Properties.Length * 116);
        }

        // visualchunkindex is for lightmaps only
        /// <summary>
        /// The find chunk and decode.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="chunknumber">The chunknumber.</param>
        /// <param name="mipmap">The mipmap.</param>
        /// <param name="meta">The meta.</param>
        /// <param name="map">The map.</param>
        /// <param name="visualchunkindex">The visualchunkindex.</param>
        /// <param name="bspnumber">The bspnumber.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Bitmap FindChunkAndDecode(
            int bitmap, 
            int chunknumber, 
            int mipmap, 
            ref Meta meta, 
            Map map, 
            int visualchunkindex, 
            int bspnumber)
        {
            for (int x = 0; x < meta.raw.rawChunks.Count; x++)
            {
                BitmapRawDataChunk tempb = (BitmapRawDataChunk)meta.raw.rawChunks[x];
                if (tempb.inchunk == bitmap && tempb.num == chunknumber)
                {
                    int width = this.Properties[bitmap].width >> chunknumber;
                    int height = this.Properties[bitmap].height >> chunknumber;
                    int depth = this.Properties[bitmap].depth >> chunknumber;
                    depth = depth < 1 ? 1 : depth;
                    int pixeloff = this.Properties[bitmap].pixelOffset;
                    int bpp = this.Properties[bitmap].bitsPerPixel;

                    int offset = 0;
                    byte[] guh;
                    #region 2D & 3D (Volume) Textures
                    if (this.Properties[bitmap].typename != BitmapType.BITM_TYPE_CUBEMAP)
                    {
                        for (int i = 0; i < mipmap; i++)
                        {
                            //offset += Math.Max(width >> i, 1) * Math.Max(height >> i, 1) * Math.Max(depth >> i, 1) * (bpp >> 3);
                            switch (this.Properties[bitmap].formatname)
                            {
                                case BitmapFormat.BITM_FORMAT_DXT1:
                                    offset += Math.Max( Math.Max(width >> i, 1) * Math.Max(height >> i, 1) * Math.Max(depth >> i, 1) * (bpp >> 3) / 8, 8);
                                    break;
                                case BitmapFormat.BITM_FORMAT_DXT2AND3:
                                case BitmapFormat.BITM_FORMAT_DXT4AND5:
                                    offset += Math.Max(Math.Max(width >> i, 1) * Math.Max(height >> i, 1) * Math.Max(depth >> i, 1) * (bpp >> 3) / 4, 16);
                                    break;
                                /*
                                case BitmapFormat.BITM_FORMAT_DXT1:
                                    offset /= 8;
                                    break;
                                case BitmapFormat.BITM_FORMAT_DXT2AND3:
                                case BitmapFormat.BITM_FORMAT_DXT4AND5:
                                    offset /= 4;
                                    break;
                                */
                                default:
                                    offset += Math.Max(width >> i, 1) * Math.Max(height >> i, 1) * Math.Max(depth >> i, 1) * (bpp >> 3);
                                    break;
                            }
                        }

                        int tempsize = tempb.size - offset;
                        guh = new byte[tempsize];
                        Array.Copy(tempb.MS.ToArray(), offset, guh, 0, tempsize);
                    }
                    #endregion
                    #region Cubemap
                    else
                    {
                        for (int i = 0; i < mipmap; i++)
                        {
                            offset += tempb.mipmaps[i].size;
                        }

                        int tempsize = tempb.size - offset * 6;
                        guh = new byte[tempsize];
                        tempsize /= 6;
                        for (int i = 0; i < 6; i++)
                        {
                            Array.Copy(
                                tempb.MS.ToArray(), i * (offset + tempsize) + offset, guh, i * tempsize, tempsize);
                        }
                    }
                    #endregion

                    width = Math.Max(width >> mipmap, 1);
                    height = Math.Max(height >> mipmap, 1);
                    depth = Math.Max(depth >> mipmap, 1);

                    int widthPad = 0;
                    if (this.Properties[bitmap].width % 16 != 0 && width % 16 != 0)
                        widthPad += 16 - (width % 16);

                    Bitmap b;
                    try
                    {
                        b = DecodeBitm(
                            guh,
                            height,
                            width + widthPad,
                            depth,
                            bpp,
                            this.Properties[bitmap].typename,
                            this.Properties[bitmap].formatname,
                            this.Properties[bitmap].swizzle,
                            map,
                            visualchunkindex,
                            bspnumber);
                    }
                    catch
                    {
                        b = DecodeBitm(
                            guh,
                            height,
                            width,
                            depth,
                            bpp,
                            this.Properties[bitmap].typename,
                            this.Properties[bitmap].formatname,
                            this.Properties[bitmap].swizzle,
                            map,
                            visualchunkindex,
                            bspnumber);
                    }
                    return b;
                }
            }

            return null;
        }

        /// <summary>
        /// The read ce parsed bitmap.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void ReadCEParsedBitmap(ref Meta meta)
        {
            using (BinaryReader BR = new BinaryReader(meta.MS))
            {
                int tempr;
                if (meta.Map.MetaInfo.external[meta.TagIndex])
                {
                    BR.BaseStream.Position = 96;
                    int tempc = BR.ReadInt32();
                    tempr = BR.ReadInt32();
                    headersize = tempr;
                    Properties = new BitmapInfo[tempc];
                    for (int x = 0; x < tempc; x++)
                    {
                        Properties[x] = new BitmapInfo(tempr + (48 * x), ref meta);
                    }
                }
                else
                {
                    BR.BaseStream.Position = 96;
                    int tempc = BR.ReadInt32();
                    tempr = BR.ReadInt32() - meta.Map.PrimaryMagic - meta.Map.MetaInfo.Offset[meta.TagIndex];
                    headersize = tempr;
                    Properties = new BitmapInfo[tempc];
                    for (int x = 0; x < tempc; x++)
                    {
                        Properties[x] = new BitmapInfo(tempr + (48 * x), ref meta);
                    }
                }
            }
        }

        /// <summary>
        /// The read h 2 parsed bitmap.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void ReadH2ParsedBitmap(ref Meta meta)
        {
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 68;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.Map.SecondaryMagic - meta.offset;
            headersize = tempr;
            Properties = new BitmapInfo[tempc];
            int okay = -1;
            for (int x = 0; x < tempc; x++)
            {
                Properties[x] = new BitmapInfo(tempr + (116 * x), ref meta);
                if (Properties[x].bitsPerPixel == 0)
                {
                    okay = x;
                }

                if (Properties[x].format == 23)
                {
                }
            }

            if (okay != -1)
            {
                MessageBox.Show(
                    Properties[okay].formatname +
                    "\nFormat not supported, please inform developer of above format name.");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The decode bitmap.
        /// </summary>
        /// <param name="fart">The fart.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="bitsPerPixel">The bits per pixel.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="swizzle">The swizzle.</param>
        /// <param name="map">The map.</param>
        /// <param name="visualchunkindex">The visualchunkindex.</param>
        /// <param name="ident">The ident.</param>
        /// <returns>The decode bitmap.</returns>
        /// <remarks></remarks>
        private static int DecodeBitmap(
            ref byte[] fart, 
            int height, 
            int width, 
            int depth, 
            int bitsPerPixel, 
            BitmapType type, 
            BitmapFormat format, 
            bool swizzle, 
            Map map, 
            int visualchunkindex, 
            int ident)
        {
            byte[] poo = new byte[0];

            int poolength;
            byte[] tempData;
            DecodeDXT decode = new DecodeDXT();

            int stride = width;

            if (swizzle)
            {
                fart = Swizzler.Swizzle(fart, width, height, depth, bitsPerPixel, true);
            }

            switch (format)
            {
                    #region DXT1

                case (BitmapFormat)14:
                    if (swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    fart = decode.DecodeDXT1(height, width, fart);
                    stride *= 4;
                    break;

                    #endregion

                    #region DXT2/3

                case (BitmapFormat)15:
                    if (swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    fart = decode.DecodeDXT23(height, width, fart);
                    stride *= 4;
                    break;

                    #endregion

                    #region DXT 4/5

                case (BitmapFormat)16:
                    if (swizzle)
                    {
                        MessageBox.Show("Swizzled");
                    }

                    fart = decode.DecodeDXT45(height, width, fart);
                    stride *= 4;
                    break;

                    #endregion

                    #region A8R8G8B8

                case (BitmapFormat)11:
                    stride *= 4;
                    break;

                    #endregion

                    #region X8R8G8B8

                case (BitmapFormat)10:
                    stride *= 4;

                    /*
                    poolength = fart.Length;
                    tempData = new byte[poolength];
                    for (int e = 0; e < poolength; e++)
                        tempData[e * 4 + 3] = 255;     // Alpha always 255
                    fart = tempData;
                    */
                    break;

                    #endregion

                    #region // 16 bit \\

                    #region A4R4G4B4

                case (BitmapFormat)9:
                    stride *= 4;
                    poolength = fart.Length;
                    tempData = new byte[poolength * 2];
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        tempData[r * 2 + 0] = (byte)((fart[r + 1] & 0xFF) >> 0); // Blue
                        tempData[r * 2 + 1] = (byte)((fart[r + 0] & 0xFF) >> 0); // Green
                        tempData[r * 2 + 2] = (byte)((fart[r + 0] & 0xFF) >> 0); // Red
                        tempData[r * 2 + 3] = 255; // (byte)(((fart[r + 1] & 0xFF) >> 0));        // Alpha
                    }

                    fart = tempData;
                    break;

                    #endregion

                    #region G8B8

                case (BitmapFormat)22:
                    stride *= 4;
                    poolength = fart.Length;
                    tempData = new byte[poolength / 2 * 4];

                    // These are actually signed (+/-128), so convert to unsigned
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        tempData[r * 2 + 0] = (byte)(fart[r + 1] + 128); // Blue
                        tempData[r * 2 + 1] = (byte)(fart[r + 1] + 128); // Green
                        tempData[r * 2 + 2] = (byte)(fart[r + 0] + 128); // Red
                        tempData[r * 2 + 3] = (byte)(fart[r + 0] + 128); // Alpha
                    }

                    fart = tempData;
                    break;

                    #endregion

                    #region A1R5G5B5

                case (BitmapFormat)8:
                    stride *= 4;
                    poolength = fart.Length;
                    tempData = new byte[poolength / 2 * 4];

                    for (int r = 0; r < fart.Length; r += 2)
                    {
                        int temp = fart[r + 0] + (fart[r + 1] << 8);
                        tempData[r * 2 + 0] = (byte)(((temp >> 0) & 0x1F) * 255 / 0x1F); // 5-bit Blue
                        tempData[r * 2 + 1] = (byte)(((temp >> 5) & 0x1F) * 255 / 0x1F); // 5-bit Green
                        tempData[r * 2 + 2] = (byte)(((temp >> 10) & 0x1F) * 255 / 0x1F); // 5-bit Red
                        tempData[r * 2 + 3] = (byte)(((temp >> 15) & 0x01) * 255); // 1-bit Alpha
                    }

                    fart = tempData;
                    break;

                    #endregion

                    #region R5G6B5

                case (BitmapFormat)6:
                    stride *= 4;
                    poolength = fart.Length;
                    tempData = new byte[poolength / 2 * 4];
                    for (int r = 0; r < fart.Length; r += 2)
                    {
                        int temp = fart[r + 0] + (fart[r + 1] << 8);
                        tempData[r * 2 + 0] = (byte)(((temp >> 0) & 0x1F) * 255 / 0x1F); // 5-bit Blue
                        tempData[r * 2 + 1] = (byte)(((temp >> 5) & 0x3F) * 255 / 0x3F); // 6-bit Green
                        tempData[r * 2 + 2] = (byte)(((temp >> 11) & 0x1F) * 255 / 0x1F); // 5-bit Red
                        tempData[r * 2 + 3] = 255; // Alpha always 255
                    }

                    fart = tempData;
                    break;

                    #endregion

                    #region A8Y8

                case (BitmapFormat)3:
                    poolength = fart.Length;
                    tempData = new byte[poolength / 2 * 4];
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        tempData[r * 2 + 0] = fart[r + 1]; // B
                        tempData[r * 2 + 1] = fart[r + 1]; // G
                        tempData[r * 2 + 2] = fart[r + 1]; // R
                        tempData[r * 2 + 3] = fart[r + 0]; // A
                    }

                    fart = tempData;
                    stride *= 4;
                    break;

                    #endregion

                    #region Unknown - Very similar (exactly?) to G8B8

                case (BitmapFormat)23:
                    stride *= 4;
                    poolength = fart.Length;
                    tempData = new byte[poolength / 2 * 4];

                    // These are actually signed (+/-128), so convert to unsigned
                    for (int e = 0; e < poolength / 2; e++)
                    {
                        int r = e * 2;
                        tempData[r * 2 + 0] = (byte)(fart[r + 1] + 128); // Blue
                        tempData[r * 2 + 1] = (byte)(fart[r + 1] + 128); // Green
                        tempData[r * 2 + 2] = (byte)(fart[r + 0] + 128); // Red
                        tempData[r * 2 + 3] = (byte)(fart[r + 0] + 128); // Alpha
                    }

                    fart = tempData;
                    break;

                    #endregion

                    #endregion

                    #region // 8 bit \\

                    #region P8

                case BitmapFormat.BITM_FORMAT_P8:
                    poolength = fart.Length;
                    tempData = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        tempData[r + 0] = fart[e];
                        tempData[r + 1] = fart[e];
                        tempData[r + 2] = fart[e];
                        tempData[r + 3] = 255;
                    }

                    fart = tempData;
                    stride *= 4;
                    break;

                    #endregion

                    #region A8

                case BitmapFormat.BITM_FORMAT_A8:
                    poolength = fart.Length;
                    tempData = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        tempData[r + 0] = fart[e];
                        tempData[r + 1] = fart[e];
                        tempData[r + 2] = fart[e];
                        tempData[r + 3] = 255;
                    }

                    fart = tempData;
                    stride *= 4;
                    break;

                    #endregion

                    #region AY8

                case BitmapFormat.BITM_FORMAT_AY8:
                    poolength = fart.Length;
                    tempData = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        tempData[r + 0] = (byte)(((fart[e] & 0xF0) >> 4) * 255 / 15);
                        tempData[r + 1] = (byte)(((fart[e] & 0xF0) >> 4) * 255 / 15);
                        tempData[r + 2] = (byte)(((fart[e] & 0xF0) >> 4) * 255 / 15);
                        tempData[r + 3] = (byte)((fart[e] & 0x0F) * 255 / 15);

                        /*
                        // I think this is the order, looking at rasterizer\distance_attenuation
                        tempData[r + 0] = (byte)((fart[e] & 0x0F) * 255 / 15);
                        tempData[r + 1] = (byte)((fart[e] & 0x0F) * 255 / 15);
                        tempData[r + 2] = (byte)((fart[e] & 0x0F) * 255 / 15);
                        tempData[r + 3] = (byte)(((fart[e] & 0xF0) >> 4) * 255 / 15);
                        /*
                        if (fart[e] == 0)
                            tempData[r + 3] = 0;
                        else
                            tempData[r + 3] = 255;
                        */
                    }

                    fart = tempData;
                    stride *= 4;
                    break;

                    #endregion

                    #region Y8

                case (BitmapFormat)1:
                    poolength = fart.Length;
                    tempData = new byte[poolength * 4];
                    for (int e = 0; e < poolength; e++)
                    {
                        int r = e * 4;
                        tempData[r + 0] = fart[e];
                        tempData[r + 1] = fart[e];
                        tempData[r + 2] = fart[e];
                        tempData[r + 3] = 255;
                    }

                    fart = tempData;
                    stride *= 4;
                    break;

                    #endregion

                    #region LightMap

                case BitmapFormat.BITM_FORMAT_LIGHTMAP:
                    poolength = fart.Length;
                    tempData = new byte[poolength * 4];
                    int bspnumber = ident;
                    int paletteindex = -1;

                    if (visualchunkindex < 0)
                    {
                        int wtf = 0 - (visualchunkindex + 1);
                        paletteindex = map.BSP.sbsp[bspnumber].SceneryChunk_LightMap_Index[wtf];
                    }

                    if (paletteindex == -1)
                    {
                        for (int i = 0; i < map.BSP.sbsp[bspnumber].VisualChunk_Bitmap_Index.Length; i++)
                        {
                            if (map.BSP.sbsp[bspnumber].VisualChunk_Bitmap_Index[i] == visualchunkindex)
                            {
                                paletteindex = map.BSP.sbsp[bspnumber].VisualChunk_LightMap_Index[visualchunkindex];
                                break;
                            }
                        }
                    }

                    if (paletteindex == -1)
                    {
                        for (int i = 0; i < map.BSP.sbsp[bspnumber].SceneryChunk_Bitmap_Index.Length; i++)
                        {
                            if (map.BSP.sbsp[bspnumber].SceneryChunk_Bitmap_Index[i] == visualchunkindex)
                            {
                                paletteindex = map.BSP.sbsp[bspnumber].SceneryChunk_LightMap_Index[i];
                                break;
                            }
                        }
                    }

                    if (paletteindex != 255)
                    {
                        for (int e = 0; e < poolength; e++)
                        {
                            int r = e * 4;
                            tempData[r + 0] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][fart[e]].r;
                            tempData[r + 1] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][fart[e]].g;
                            tempData[r + 2] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][fart[e]].b;
                            tempData[r + 3] = (byte)map.BSP.sbsp[bspnumber].LightMap_Palettes[paletteindex][fart[e]].a;
                        }

                        fart = tempData;
                        stride *= 4;
                    }

                    break;

                    #endregion

                    #endregion
            }

            return stride;
        }

        /// <summary>
        /// The rtl move memory.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="src">The src.</param>
        /// <param name="cb">The cb.</param>
        /// <remarks></remarks>
        [DllImport("kernel32.dll")]
        private static extern void RtlMoveMemory(IntPtr dest, byte[] src, int cb);

        /// <summary>
        /// The bi get raw total size.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="ignoreInternal">The ignore internal.</param>
        /// <returns>The bi get raw total size.</returns>
        /// <remarks></remarks>
        private static long biGetRawTotalSize(RawDataContainer raw, bool ignoreInternal)
        {
            long rawSize = 0;
            for (int i = 0; i < raw.rawChunks.Count; i++)
            {
                if (raw.rawChunks[i].rawLocation != MapTypes.Internal || !ignoreInternal)
                {
                    {
                        rawSize += raw.rawChunks[i].size;
                        rawSize += genPaddingSize(raw.rawChunks[i].size /*.lods.get(k).rawSize*/, 512);
                    }
                }
            }

            return rawSize;
        }

        /// <summary>
        /// The bi update map info.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="change">The change.</param>
        /// <remarks></remarks>
        private static void biUpdateMapInfo(Map map, int change)
        {
            // Write back all changed values
            map.MapHeader.fileSize += change;

            map.MapHeader.indexOffset += change;
            map.BW.BaseStream.Position = 8;
            map.BW.Write(map.MapHeader.fileSize);
            map.BW.BaseStream.Position = 16;
            map.BW.Write(map.MapHeader.indexOffset);

            // Start of primary magic finder
            map.BR.BaseStream.Position = map.MapHeader.indexOffset;
            map.PrimaryMagic = map.BR.ReadInt32() - (map.MapHeader.indexOffset + 32);

            // End of primary magic finder

            // Start of Secondary Magic
            map.BR.BaseStream.Position = map.MapHeader.indexOffset + 8;
            map.BR.BaseStream.Position = (map.BR.ReadInt32() - map.PrimaryMagic) + 8;
            map.SecondaryMagic = map.BR.ReadInt32() - (map.MapHeader.indexOffset + map.MapHeader.metaStart);

            // End of Secondary Magic

            // Find new tagsOffset
            map.BR.BaseStream.Position = map.MapHeader.indexOffset + 4;
            map.IndexHeader.tagsOffset = map.MapHeader.indexOffset + 32 + (map.BR.ReadInt32() * 12);

            for (int i = 0; i < map.IndexHeader.metaCount; i++)
            {
                if (map.MetaInfo.TagType[i] != "sbsp" && map.MetaInfo.TagType[i] != "ltmp")
                {
                    map.MetaInfo.Offset[i] += change;
                }
            }
        }

        /// <summary>
        /// The gen padding.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static byte[] genPadding(int amount, byte type)
        {
            byte[] padding = new byte[amount];
            for (int i = 0; i < amount; i++)
            {
                padding[i] = type;
            }

            return padding;
        }

        /// <summary>
        /// The gen padding size.
        /// </summary>
        /// <param name="totRawSize">The tot raw size.</param>
        /// <param name="padToSize">The pad to size.</param>
        /// <returns>The gen padding size.</returns>
        /// <remarks></remarks>
        private static int genPaddingSize(long totRawSize, int padToSize)
        {
            long paddingSize = padToSize - (totRawSize % padToSize);
            if (paddingSize == padToSize)
            {
                paddingSize = 0;
            }

            return (int)paddingSize;
        }

        /// <summary>
        /// The shift.
        /// </summary>
        /// <param name="FS">The fs.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <remarks></remarks>
        private static void shift(FileStream FS, long offset, long size)
        {
            GC.Collect();
            long freeMemory = GC.GetTotalMemory(false) - 1000000;
            long oldLength = FS.Length;
            long newLength = oldLength + size;
            int shiftSize = (int)(oldLength - offset);
            bool shortWay = shiftSize < freeMemory;
            if (size < 0)
            {
                shortWay = true;
            }

            if (shortWay)
            {
                byte[] data = new byte[shiftSize];
                FS.Position = offset;
                FS.Read(data, 0, shiftSize);
                FS.Position = offset + size;
                FS.Write(data, 0, shiftSize);
                data = null;
            }
            else
            {
                int amountLeft = shiftSize;
                int defaultChunkSize = (int)freeMemory;
                long fileReadPos = oldLength;
                long fileWritePos = newLength;
                while (amountLeft != 0)
                {
                    int chunkSize;
                    if (amountLeft > defaultChunkSize)
                    {
                        chunkSize = defaultChunkSize;
                    }
                    else
                    {
                        chunkSize = amountLeft;
                    }

                    fileReadPos -= chunkSize;
                    fileWritePos -= chunkSize;
                    byte[] temp = new byte[chunkSize];
                    FS.Position = fileReadPos;
                    FS.Read(temp, 0, chunkSize);
                    FS.Position = fileWritePos;
                    FS.Write(temp, 0, chunkSize);
                    amountLeft -= chunkSize;
                }
            }

            if (size > 0)
            {
                byte[] temp = new byte[(int)size];
                FS.Position = offset;
                FS.Write(temp, 0, temp.Length);
            }

            FS.SetLength(newLength);
        }

        #endregion

        /// <summary>
        /// The bitmap info.
        /// </summary>
        /// <remarks></remarks>
        public class BitmapInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The bits per pixel.
            /// </summary>
            public ushort bitsPerPixel;

            /// <summary>
            /// The depth.
            /// </summary>
            public ushort depth;

            /// <summary>
            /// The flags.
            /// </summary>
            public ushort flags;

            /// <summary>
            /// The format.
            /// </summary>
            public ushort format; // Unused

            /// <summary>
            /// The formatname.
            /// </summary>
            public BitmapFormat formatname;

            /// <summary>
            /// The height.
            /// </summary>
            public ushort height;

            /// <summary>
            /// The mip map count.
            /// </summary>
            public ushort mipMapCount;

            /// <summary>
            /// The pixel offset.
            /// </summary>
            public ushort pixelOffset;

            /// <summary>
            /// The reg point x.
            /// </summary>
            public ushort regPointX;

            /// <summary>
            /// The reg point y.
            /// </summary>
            public ushort regPointY;

            /// <summary>
            /// The swizzle.
            /// </summary>
            public bool swizzle;

            /// <summary>
            /// The tagtype.
            /// </summary>
            public char[] tagtype;

            /// <summary>
            /// The type.
            /// </summary>
            public ushort type;

            /// <summary>
            /// The typename.
            /// </summary>
            public BitmapType typename;

            /// <summary>
            /// The width.
            /// </summary>
            public ushort width;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BitmapInfo"/> class.
            /// </summary>
            /// <param name="FormatName">Name of the format.</param>
            /// <param name="Swizzle">if set to <c>true</c> [swizzle].</param>
            /// <remarks></remarks>
            public BitmapInfo(BitmapFormat FormatName, bool Swizzle)
            {
                formatname = FormatName;
                swizzle = Swizzle;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BitmapInfo"/> class.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public BitmapInfo(int offset, ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset;
                tagtype = BR.ReadChars(4);
                width = BR.ReadUInt16();
                height = BR.ReadUInt16();
                depth = BR.ReadUInt16();
                type = BR.ReadUInt16();
                typename = (BitmapType)type;

                // case BitmapFormat.BITM_TYPE_LIGHTMAP:
                format = BR.ReadUInt16();
                formatname = (BitmapFormat)format;
                switch (formatname)
                {
                    case BitmapFormat.BITM_FORMAT_A8:
                    case BitmapFormat.BITM_FORMAT_P8:
                    case BitmapFormat.BITM_FORMAT_Y8:
                    case BitmapFormat.BITM_FORMAT_AY8:
                    case BitmapFormat.BITM_FORMAT_LIGHTMAP:
                        bitsPerPixel = 8;
                        break;
                    case BitmapFormat.BITM_FORMAT_A1R5G5B5:
                    case BitmapFormat.BITM_FORMAT_A4R4G4B4:
                    case BitmapFormat.BITM_FORMAT_A8Y8:
                    case BitmapFormat.BITM_FORMAT_R5G6B5:
                    case BitmapFormat.BITM_FORMAT_G8B8:
              case BitmapFormat.BITM_FORMAT_UNKNOWN:
                        bitsPerPixel = 16;
                        break;
                    case BitmapFormat.BITM_FORMAT_X8R8G8B8:
                    case BitmapFormat.BITM_FORMAT_A8R8G8B8:
                    case BitmapFormat.BITM_FORMAT_DXT1:
                    case BitmapFormat.BITM_FORMAT_DXT2AND3:
                    case BitmapFormat.BITM_FORMAT_DXT4AND5:
                        bitsPerPixel = 32;
                        break;
                    default:
                        bitsPerPixel = 0;
                        break;
                }

                flags = BR.ReadUInt16();

                // if ((flags & 0x1000) == 0x1000) { swizzle = true; }
                if ((flags & 0x8) == 0x8)
                {
                    swizzle = true;
                }

                regPointX = BR.ReadUInt16();
                regPointY = BR.ReadUInt16();
                mipMapCount = BR.ReadUInt16();
                pixelOffset = BR.ReadUInt16();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The generate.
            /// </summary>
            /// <param name="ddsd">The ddsd.</param>
            /// <remarks></remarks>
            public void Generate(DDS.DDSURFACEDESC2 ddsd)
            {
                string bitmstring = "mtib";
                tagtype = bitmstring.ToCharArray();

                // width = ddsd.width; ;
                // height = ddsd.height; ;
                // depth=ddsd.depth;
                /*
                    type=ddsd.;
                    format;
                    formatname;
                    swizzle;
                    flags;
                    regPointX;
                    regPointY;
                    mipMapCount=ddsd.MipMapCount;
                    pixelOffset=d;
                */
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <param name="BW">The bw.</param>
            /// <remarks></remarks>
            public void Write(ref BinaryWriter BW)
            {
                BW.Write(tagtype);
                BW.Write(width);
                BW.Write(height);
                BW.Write(depth);
                BW.Write(type);
                BW.Write((short)formatname);
                BW.Write(flags);

                BW.Write(regPointX);
                BW.Write(regPointY);
                BW.Write(mipMapCount);
                BW.Write(pixelOffset);
            }

            #endregion
        }

        /// <summary>
        /// The halo bitmap header.
        /// </summary>
        /// <remarks></remarks>
        public class HaloBitmapHeader
        {
            #region Constants and Fields

            /// <summary>
            /// The bumpheight.
            /// </summary>
            public float bumpheight;

            /// <summary>
            /// The colorplateheight.
            /// </summary>
            public short colorplateheight;

            /// <summary>
            /// The colorplatewidth.
            /// </summary>
            public short colorplatewidth;

            /// <summary>
            /// The compressedcolorplatedata.
            /// </summary>
            public short compressedcolorplatedata;

            /// <summary>
            /// The detailfadefactor.
            /// </summary>
            public float detailfadefactor;

            /// <summary>
            /// The format.
            /// </summary>
            public short format;

            /// <summary>
            /// The sharpenamount.
            /// </summary>
            public float sharpenamount;

            /// <summary>
            /// The spritebudgesize.
            /// </summary>
            public short spritebudgesize;

            /// <summary>
            /// The spritebudgetcount.
            /// </summary>
            public short spritebudgetcount;

            /// <summary>
            /// The type.
            /// </summary>
            public short type;

            /// <summary>
            /// The usage.
            /// </summary>
            public short usage;

            /// <summary>
            /// The usageflags.
            /// </summary>
            public short usageflags;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="HaloBitmapHeader"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public HaloBitmapHeader(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 0;
                type = BR.ReadInt16();
                format = BR.ReadInt16();
                usage = BR.ReadInt16();
                usageflags = BR.ReadInt16();
                detailfadefactor = BR.ReadSingle();
                sharpenamount = BR.ReadSingle();
                bumpheight = BR.ReadSingle();
                spritebudgesize = BR.ReadInt16();
                spritebudgetcount = BR.ReadInt16();
                colorplatewidth = BR.ReadInt16();
                colorplateheight = BR.ReadInt16();
                compressedcolorplatedata = BR.ReadInt16();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The write header halo bitmap header.
            /// </summary>
            /// <param name="BW">The bw.</param>
            /// <remarks></remarks>
            public void WriteHeaderHaloBitmapHeader(ref BinaryWriter BW)
            {
                BW.BaseStream.Position = 0;
                BW.Write(type);
                BW.Write(format);
                BW.Write(usage);
                BW.Write(usageflags);
                BW.Write(detailfadefactor);
                BW.Write(sharpenamount);
                BW.Write(bumpheight);
                BW.Write(spritebudgesize);
                BW.Write(spritebudgetcount);
                BW.Write(colorplatewidth);
                BW.Write(colorplateheight);
                BW.Write(compressedcolorplatedata);
            }

            #endregion
        }
    }
}