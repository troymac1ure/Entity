// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RawData.cs" company="">
//   
// </copyright>
// <summary>
//   The raw data container type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RawData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;

    #region Enumerations

    /// <summary>
    /// The raw data container type.
    /// </summary>
    /// <remarks></remarks>
    public enum RawDataContainerType
    {
        /// <summary>
        /// The bitmap.
        /// </summary>
        Bitmap, 

        /// <summary>
        /// The header.
        /// </summary>
        Header, 

        /// <summary>
        /// The model.
        /// </summary>
        Model, 

        /// <summary>
        /// The animation.
        /// </summary>
        Animation, 

        /// <summary>
        /// The sound.
        /// </summary>
        Sound, 

        /// <summary>
        /// The coconuts model.
        /// </summary>
        CoconutsModel, 

        /// <summary>
        /// The prtm.
        /// </summary>
        PRTM, 

        /// <summary>
        /// The decr.
        /// </summary>
        DECR, 

        /// <summary>
        /// The bsp.
        /// </summary>
        BSP, 

        /// <summary>
        /// The light map.
        /// </summary>
        LightMap, 

        /// <summary>
        /// The empty.
        /// </summary>
        Empty, 

        /// <summary>
        /// The weather.
        /// </summary>
        Weather, 

        /// <summary>
        /// The bsp meta.
        /// </summary>
        BSPMeta, 

        /// <summary>
        /// The unicode names index.
        /// </summary>
        UnicodeNamesIndex, 

        /// <summary>
        /// The unicode names.
        /// </summary>
        UnicodeNames, 

        /// <summary>
        /// The strings index.
        /// </summary>
        StringsIndex, 

        /// <summary>
        /// The strings 1.
        /// </summary>
        Strings1, 

        /// <summary>
        /// The strings 2.
        /// </summary>
        Strings2, 

        /// <summary>
        /// The file names index.
        /// </summary>
        FileNamesIndex, 

        /// <summary>
        /// The file names.
        /// </summary>
        FileNames, 

        /// <summary>
        /// The crazy.
        /// </summary>
        Crazy, 

        /// <summary>
        /// The meta index.
        /// </summary>
        MetaIndex, 

        /// <summary>
        /// The meta data.
        /// </summary>
        MetaData
    }

    /// <summary>
    /// The raw data type.
    /// </summary>
    /// <remarks></remarks>
    public enum RawDataType
    {
        /// <summary>
        /// The bitm.
        /// </summary>
        bitm, 

        /// <summary>
        /// The bsp 1.
        /// </summary>
        bsp1, 

        /// <summary>
        /// The bsp 2.
        /// </summary>
        bsp2, 

        /// <summary>
        /// The bsp 3.
        /// </summary>
        bsp3, 

        /// <summary>
        /// The bsp 4.
        /// </summary>
        bsp4, 

        /// <summary>
        /// The decr.
        /// </summary>
        DECR, 

        /// <summary>
        /// The jmad.
        /// </summary>
        jmad, 

        /// <summary>
        /// The ltmp.
        /// </summary>
        ltmp, 

        /// <summary>
        /// The mode 1.
        /// </summary>
        mode1, 

        /// <summary>
        /// The mode 2.
        /// </summary>
        mode2, 

        /// <summary>
        /// The prtm.
        /// </summary>
        PRTM, 

        /// <summary>
        /// The snd 1.
        /// </summary>
        snd1, 

        /// <summary>
        /// The snd 2.
        /// </summary>
        snd2, 

        /// <summary>
        /// The weat.
        /// </summary>
        weat, 

        /// <summary>
        /// The halo ce vertices.
        /// </summary>
        HaloCEVertices, 

        /// <summary>
        /// The halo ce indices.
        /// </summary>
        HaloCEIndices
    }

    #endregion Enumerations

    /// <summary>
    /// The animation.
    /// </summary>
    /// <remarks></remarks>
    public class Animation : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Animation()
        {
            containerType = RawDataContainerType.Animation;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 172;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.jmad;
                Raw.pointerMetaOffset = tempr + (x * 20) + 8 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset - 4;
                Raw.size = map.BR.ReadInt32();
                Raw.offset = map.BR.ReadInt32();

                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }
        }

        #endregion
    }

    // raw data containers
    /// <summary>
    /// The bitmap raw.
    /// </summary>
    /// <remarks></remarks>
    public class BitmapRaw : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRaw"/> class.
        /// </summary>
        /// <remarks></remarks>
        public BitmapRaw()
        {
            containerType = RawDataContainerType.Bitmap;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    ReadH2BitmapRaw(TagIndex, map, dontreadraw);
                    break;
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    ReadCEBitmapRaw(TagIndex, map, dontreadraw);
                    break;
            }
        }

        /// <summary>
        /// The read ce bitmap raw.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadCEBitmapRaw(int TagIndex, Map map, bool dontreadraw)
        {
            if (map.MetaInfo.external[TagIndex])
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 84;
                int rawcount = map.BR.ReadInt32();
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 100;
                int translation = map.BR.ReadInt32();
                for (int x = 0; x < rawcount; x++)
                {
                    BitmapRawDataChunk Raw = new BitmapRawDataChunk();
                    Raw.inchunk = x;
                    Raw.num = 0;
                    Raw.rawDataType = RawDataType.bitm;
                    Raw.pointerMetaOffset = (x * 48) + translation + 24;
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                    Raw.offset = map.BR.ReadInt32();
                    Raw.size = map.BR.ReadInt32();
                    Raw.rawLocation = MapTypes.Bitmaps;

                    try
                    {
                        if (dontreadraw == false)
                        {
                            // map.OpenMap(Raw.rawLocation);
                            map.BR.BaseStream.Position = Raw.offset;
                            Raw.MS = new MemoryStream(Raw.size);
                            Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);

                            // map.OpenMap(MapTypes.Internal);
                        }

                        this.rawChunks.Add(Raw);
                    }
                    catch
                    {
                    }
                }

                // m.CloseMap();
            }
            else
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 96;
                int tempc = map.BR.ReadInt32();
                int tempr = map.BR.ReadInt32() - map.PrimaryMagic - map.MetaInfo.Offset[TagIndex];
                for (int x = 0; x < tempc; x++)
                {
                    BitmapRawDataChunk Raw = new BitmapRawDataChunk();
                    Raw.inchunk = x;
                    Raw.num = 0;
                    Raw.pointerMetaOffset = tempr + (x * 48) + 24;
                    map.BR.BaseStream.Position = Raw.pointerMetaOffset + map.MetaInfo.Offset[TagIndex];
                    Raw.rawDataType = RawDataType.bitm;
                    Raw.offset = map.BR.ReadInt32();
                    Raw.size = map.BR.ReadInt32();
                    Raw.rawLocation = MapTypes.Internal;

                    if (dontreadraw == false)
                    {
                        // map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);

                        // map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }
        }

        /// <summary>
        /// The read h 2 bitmap raw.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadH2BitmapRaw(int TagIndex, Map map, bool dontreadraw)
        {
            // Offset 68 = Bitmap Data reflexive
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 68;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;

            for (int x = 0; x < tempc; x++)
            {
                // Bitmap Data reflexive is 116 bytes large
                // Offset 4 in BD reflexive is (short) width
                map.BR.BaseStream.Position = tempr + (x * 116) + 4;
                int width = map.BR.ReadInt16();

                // Offset 6 in BD reflexive is (short) width
                int height = map.BR.ReadInt16();

                // Offset 8 in BD reflexive is (short) width
                int depth = map.BR.ReadInt16();

                // Offset 10 in BD reflexive is (short) type
                int type = map.BR.ReadInt16();

                // Offset 12 in BD reflexive is (short) format
                int format = map.BR.ReadInt16();

                // Offset 20 in BD reflexive is (short) mipmap Count
                map.BR.BaseStream.Position = tempr + (x * 116) + 20;
                int mipmapCount = map.BR.ReadInt16();

                for (int xx = 0; xx < 6; xx++)
                {
                    BitmapRawDataChunk Raw = new BitmapRawDataChunk();
                    Raw.inchunk = x;
                    Raw.num = xx;
                    Raw.pointerMetaOffset = tempr + (x * 116) + 28 + (xx * 4) - map.MetaInfo.Offset[TagIndex];
                    Raw.rawDataType = RawDataType.bitm;
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                    Raw.offset = map.BR.ReadInt32();
                    if (Raw.offset == -1)
                    {
                        break;
                    }

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset + 24;
                    Raw.size = map.BR.ReadInt32();

                    map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);

                        #region mipmap loading section

                        int offset = 0;
                        int Bpp = 4; // 32 bit
                        switch (format)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 17:
                                Bpp = 1; // 8 bit
                                break;
                            case 3:
                            case 6:
                            case 8:
                            case 9:
                            case 22:
                                Bpp = 2; // 16 bit
                                break;
                        }

                        for (int xxx = xx; xxx < mipmapCount; xxx++)
                        {
                            mipmapDataChunk mipmapDC = new mipmapDataChunk();
                            mipmapDC.width = width >> xxx;
                            mipmapDC.height = height >> xxx;
                            mipmapDC.depth = depth >> xxx != 0 ? depth >> xxx : 1;
                            mipmapDC.offset = offset;
                            mipmapDC.size = mipmapDC.width * mipmapDC.height * mipmapDC.depth * Bpp;

                            // if (type == 2)  // Cubemap, 6 sides
                            // mipmapDC.size *= 6;
                            switch (format)
                            {
                                case 14: // DXT1
                                    mipmapDC.size /= 8;
                                    break;
                                case 15: // DXT3
                                case 16: // DXT5
                                    mipmapDC.size /= 4;
                                    break;
                            }

                            offset += mipmapDC.size;
                            Raw.mipmaps.Add(mipmapDC);
                        }

                        #endregion

                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The bitmap raw data chunk.
    /// </summary>
    /// <remarks></remarks>
    public class BitmapRawDataChunk : RawDataChunk
    {
        #region Constants and Fields

        /// <summary>
        /// The inchunk.
        /// </summary>
        public int inchunk;

        /// <summary>
        /// The mipmaps.
        /// </summary>
        public List<mipmapDataChunk> mipmaps;

        /// <summary>
        /// The num.
        /// </summary>
        public int num;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRawDataChunk"/> class.
        /// </summary>
        /// <remarks></remarks>
        public BitmapRawDataChunk()
        {
            mipmaps = new List<mipmapDataChunk>();
        }

        #endregion
    }

    /// <summary>
    /// The bsp raw.
    /// </summary>
    /// <remarks></remarks>
    public class BSPRaw : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPRaw"/> class.
        /// </summary>
        /// <remarks></remarks>
        public BSPRaw()
        {
            containerType = RawDataContainerType.BSP;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 2 read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void H2Read(int TagIndex, Map map, bool dontreadraw)
        {
            int w = map.BSP.FindBSPNumberByBSPIdent(map.MetaInfo.Ident[TagIndex]);

            // bsp model 1
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 172;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.bsp1;
                Raw.pointerMetaOffset = tempr + (x * 176) + 40 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = tempr + (x * 176) + 40;
                Raw.offset = map.BR.ReadInt32();

                Raw.size = map.BR.ReadInt32();
                if (Raw.offset == -1)
                {
                    this.rawChunks.Add(Raw);
                    continue;
                }

                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }

            // bsp model 2
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 328;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.bsp2;
                Raw.pointerMetaOffset = tempr + (x * 200) + 40 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = tempr + (x * 200) + 40;
                Raw.offset = map.BR.ReadInt32();

                Raw.size = map.BR.ReadInt32();
                if (Raw.offset == -1)
                {
                    this.rawChunks.Add(Raw);
                    continue;
                }

                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }

            // light map
            if (map.BSP.sbsp[w].lightmapident != -1)
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 8;
                tempr = map.BR.ReadInt32();
                if (tempr == 0)
                {
                    goto skiplightmap;
                }

                tempr -= map.BSP.sbsp[w].magic;
                map.BR.BaseStream.Position = tempr + 128;
                tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
                map.BR.BaseStream.Position = tempr + 64;
                tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
                for (int x = 0; x < tempc; x++)
                {
                    RawDataChunk Raw = new RawDataChunk();
                    Raw.rawDataType = RawDataType.ltmp;
                    Raw.pointerMetaOffset = tempr + (x * 56) + 12 - map.MetaInfo.Offset[TagIndex];
                    map.BR.BaseStream.Position = tempr + (x * 56) + 12;
                    Raw.offset = map.BR.ReadInt32();

                    Raw.size = map.BR.ReadInt32();
                    if (Raw.offset == -1)
                    {
                        this.rawChunks.Add(Raw);
                        continue;
                    }

                    map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }

            skiplightmap:

            // bsp model 3
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 580;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;

            // if (tempc==0){return;}
            map.BR.BaseStream.Position = tempr + 16;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.bsp3;
                Raw.pointerMetaOffset = tempr + (x * 44) - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = tempr + (x * 44);
                Raw.offset = map.BR.ReadInt32();
                Raw.size = map.BR.ReadInt32();
                if (Raw.offset == -1)
                {
                    this.rawChunks.Add(Raw);
                    continue;
                }

                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }

            // bsp model 4
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 548;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;

            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.bsp4;
                Raw.pointerMetaOffset = tempr + (x * 172) + 16 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = tempr + (x * 172) + 16;
                Raw.offset = map.BR.ReadInt32();
                if (Raw.offset == -1)
                {
                    continue;
                }

                Raw.size = map.BR.ReadInt32();
                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                    H2Read(TagIndex, map, dontreadraw);
                    break;
                case HaloVersionEnum.HaloCE:
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// The decr.
    /// </summary>
    /// <remarks></remarks>
    public class DECR : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DECR"/> class.
        /// </summary>
        /// <remarks></remarks>
        public DECR()
        {
            containerType = RawDataContainerType.DECR;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            RawDataChunk Raw = new RawDataChunk();
            Raw.rawDataType = RawDataType.DECR;
            Raw.pointerMetaOffset = 56;
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 56;
            Raw.offset = map.BR.ReadInt32();
            Raw.size = map.BR.ReadInt32();
            map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
            if (dontreadraw == false)
            {
                map.OpenMap(Raw.rawLocation);
                map.BR.BaseStream.Position = Raw.offset;
                Raw.MS = new MemoryStream(Raw.size);
                Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                map.OpenMap(MapTypes.Internal);
            }

            this.rawChunks.Add(Raw);
        }

        #endregion
    }

    /// <summary>
    /// The lightmap raw.
    /// </summary>
    /// <remarks></remarks>
    public class LightmapRaw : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LightmapRaw"/> class.
        /// </summary>
        /// <remarks></remarks>
        public LightmapRaw()
        {
            containerType = RawDataContainerType.LightMap;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 2 read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void H2Read(int TagIndex, Map map, bool dontreadraw)
        {
            int w = map.BSP.FindBSPNumberByLightMapIdent(map.MetaInfo.Ident[TagIndex]);

            // light map
            if (map.BSP.sbsp[w].lightmapident != -1)
            {
                int tempr = map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = tempr + 128;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
                map.BR.BaseStream.Position = tempr + 64;
                tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.BSP.sbsp[w].magic;
                for (int x = 0; x < tempc; x++)
                {
                    RawDataChunk Raw = new RawDataChunk();
                    Raw.rawDataType = RawDataType.ltmp;
                    Raw.pointerMetaOffset = tempr + (x * 56) + 12 - map.MetaInfo.Offset[TagIndex];
                    map.BR.BaseStream.Position = tempr + (x * 56) + 12;
                    Raw.offset = map.BR.ReadInt32();

                    Raw.size = map.BR.ReadInt32();
                    if (Raw.offset == -1)
                    {
                        this.rawChunks.Add(Raw);
                        continue;
                    }

                    map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    H2Read(TagIndex, map, dontreadraw);
                    break;
                case HaloVersionEnum.HaloCE:
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// The mipmap data chunk.
    /// </summary>
    /// <remarks></remarks>
    public class mipmapDataChunk
    {
        #region Constants and Fields

        /// <summary>
        /// The ms.
        /// </summary>
        public MemoryStream MS;

        /// <summary>
        /// The depth.
        /// </summary>
        public int depth;

        /// <summary>
        /// The height.
        /// </summary>
        public int height;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        /// <summary>
        /// The width.
        /// </summary>
        public int width;

        #endregion
    }

    /// <summary>
    /// The model.
    /// </summary>
    /// <remarks></remarks>
    public class Model : RawDataContainer
    {
        #region Constants and Fields

        /// <summary>
        /// The intermediate pointer offset.
        /// </summary>
        public int IntermediatePointerOffset; // for halo 1

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Model()
        {
            containerType = RawDataContainerType.Model;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    ReadH2ModelRaw(TagIndex, map, dontreadraw);
                    break;
                case HaloVersionEnum.HaloCE:
                    ReadCEModelRaw(TagIndex, map, dontreadraw);
                    break;
                case HaloVersionEnum.Halo1:
                    ReadH1ModelRaw(TagIndex, map, dontreadraw);
                    break;
            }
        }

        /// <summary>
        /// The read ce model raw.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadCEModelRaw(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 208;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            for (int y = 0; y < tempc; y++)
            {
                map.BR.BaseStream.Position = tempr + (y * 48) + 36;
                int tempcc = map.BR.ReadInt32();
                int temprc = map.BR.ReadInt32() - map.PrimaryMagic;
                for (int x = 0; x < tempcc; x++)
                {
                    RawDataChunk Raw = new RawDataChunk();

                    map.BR.BaseStream.Position = temprc + (x * 132) + 4;
                    Raw.shadernumber = map.BR.ReadInt16();
                    Raw.pointerMetaOffset = temprc + (x * 132) + 76 - map.MetaInfo.Offset[TagIndex];

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset - 4;
                    Raw.rawDataType = RawDataType.HaloCEIndices;
                    Raw.size = (map.BR.ReadInt32() + 2) * 2;
                    Raw.offset = map.IndexHeader.ModelRawDataOffset + map.IndexHeader.ModelIndicesOffset +
                                 map.BR.ReadInt32();

                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);

                    Raw = new RawDataChunk();
                    Raw.rawDataType = RawDataType.HaloCEVertices;
                    Raw.pointerMetaOffset = temprc + (x * 132) + 100 - map.MetaInfo.Offset[TagIndex];
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset - 12;
                    Raw.size = map.BR.ReadInt32();
                    Raw.size *= 68;

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;

                    Raw.offset = map.IndexHeader.ModelRawDataOffset + map.BR.ReadInt32();

                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }
        }

        /// <summary>
        /// The read h 1 model raw.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadH1ModelRaw(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 208;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            for (int y = 0; y < tempc; y++)
            {
                map.BR.BaseStream.Position = tempr + (y * 48) + 36;
                int tempcc = map.BR.ReadInt32();
                int temprc = map.BR.ReadInt32() - map.PrimaryMagic;
                for (int x = 0; x < tempcc; x++)
                {
                    RawDataChunk Raw = new RawDataChunk();

                    map.BR.BaseStream.Position = temprc + (x * 104) + 4;
                    Raw.shadernumber = map.BR.ReadInt16();
                    Raw.pointerMetaOffset = temprc + (x * 104) + 76 - map.MetaInfo.Offset[TagIndex];

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset - 4;
                    Raw.rawDataType = RawDataType.HaloCEIndices;
                    Raw.size = (map.BR.ReadInt32() + 2) * 2;
                    Raw.offset = map.BR.ReadInt32() - map.PrimaryMagic;
                    

                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);

                    Raw = new RawDataChunk();
                    Raw.rawDataType = RawDataType.HaloCEVertices;
                    Raw.pointerMetaOffset = temprc + (x * 104) + 100 - map.MetaInfo.Offset[TagIndex];
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset - 12;
                    Raw.size = map.BR.ReadInt32();
                    Raw.size *= 32;

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;

                    this.IntermediatePointerOffset = map.BR.ReadInt32() - map.PrimaryMagic;
                    
                    map.BR.BaseStream.Position = this.IntermediatePointerOffset + 4;
                    Raw.offset = map.BR.ReadInt32() - map.PrimaryMagic;

                    if (dontreadraw == false)
                    {
                        map.OpenMap(Raw.rawLocation);
                        map.BR.BaseStream.Position = Raw.offset;
                        Raw.MS = new MemoryStream(Raw.size);
                        Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                        map.OpenMap(MapTypes.Internal);
                    }

                    this.rawChunks.Add(Raw);
                }
            }
        }

        /// <summary>
        /// The read h 2 model raw.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadH2ModelRaw(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 36;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.pointerMetaOffset = tempr + (x * 92) + 56 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                Raw.rawDataType = RawDataType.mode1;
                Raw.offset = map.BR.ReadInt32();
                Raw.size = map.BR.ReadInt32();
                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }

            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 116;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.pointerMetaOffset = tempr + (x * 88) + 52 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                Raw.rawDataType = RawDataType.mode2;
                Raw.offset = map.BR.ReadInt32();
                Raw.size = map.BR.ReadInt32();
                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }
        }

        #endregion
    }

    /// <summary>
    /// The prtm.
    /// </summary>
    /// <remarks></remarks>
    public class PRTM : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PRTM"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PRTM()
        {
            containerType = RawDataContainerType.PRTM;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            RawDataChunk Raw = new RawDataChunk();
            Raw.rawDataType = RawDataType.PRTM;
            Raw.pointerMetaOffset = 160;
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 160;
            Raw.offset = map.BR.ReadInt32();
            Raw.size = map.BR.ReadInt32();
            map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
            if (dontreadraw == false)
            {
                map.OpenMap(Raw.rawLocation);
                map.BR.BaseStream.Position = Raw.offset;
                Raw.MS = new MemoryStream(Raw.size);
                Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                map.OpenMap(MapTypes.Internal);
            }

            this.rawChunks.Add(Raw);
        }

        #endregion
    }

    /// <summary>
    /// The raw data chunk.
    /// </summary>
    /// <remarks></remarks>
    public class RawDataChunk
    {
        #region Constants and Fields

        /// <summary>
        /// The ms.
        /// </summary>
        public MemoryStream MS;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The pointer meta offset.
        /// </summary>
        public int pointerMetaOffset;

        /// <summary>
        /// The raw data type.
        /// </summary>
        public RawDataType rawDataType;

        /// <summary>
        /// The raw location.
        /// </summary>
        public MapTypes rawLocation;

        /// <summary>
        /// The shadernumber.
        /// </summary>
        public int shadernumber;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        #endregion
    }

    /// <summary>
    /// The raw data container.
    /// </summary>
    /// <remarks></remarks>
    public class RawDataContainer
    {
        #region Constants and Fields

        /// <summary>
        /// The container type.
        /// </summary>
        public RawDataContainerType containerType;

        /// <summary>
        /// The raw chunks.
        /// </summary>
        public List<RawDataChunk> rawChunks = new List<RawDataChunk>();

        #endregion

        #region Public Methods

        /// <summary>
        /// The load raw from file.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public RawDataContainer LoadRawFromFile(string inputFilePath, Meta.Meta meta)
        {
            RawDataContainer raw = new RawDataContainer();
            int x = inputFilePath.LastIndexOf('.');
            string temp = inputFilePath.Substring(0, x + 1) + meta.type + "raw";

            XmlTextReader xtr = new XmlTextReader(temp + ".xml");
            xtr.WhitespaceHandling = WhitespaceHandling.None;

            FileStream FS = new FileStream(temp, FileMode.Open);
            BinaryReader BR = new BinaryReader(FS);

            while (xtr.Read())
            {
                switch (xtr.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xtr.Name == "RawData")
                        {
                            string oi = xtr.GetAttribute("RawType");

                            switch (oi)
                            {
                                case "Model":
                                    raw = new Model();
                                    break;
                                case "Bitmap":
                                    raw = new BitmapRaw();
                                    break;
                                case "Animation":
                                    raw = new Animation();
                                    break;
                                case "DECR":
                                    raw = new DECR();
                                    break;
                                case "PRTM":
                                    raw = new PRTM();
                                    break;
                                case "Weather":
                                    raw = new Weather();
                                    break;
                                case "Sound":
                                    raw = new Sound();
                                    break;
                                case "BSP":
                                    raw = new BSPRaw();
                                    break;
                            }
                        }
                        else if (xtr.Name == "RawChunk")
                        {
                            RawDataChunk r = new RawDataChunk();
                            string temps = xtr.GetAttribute("RawDataType");
                            switch (temps)
                            {
                                case "bitm":
                                    r.rawDataType = RawDataType.bitm;
                                    break;
                                case "bsp1":
                                    r.rawDataType = RawDataType.bsp1;
                                    break;
                                case "bsp2":
                                    r.rawDataType = RawDataType.bsp2;
                                    break;
                                case "bsp3":
                                    r.rawDataType = RawDataType.bsp3;
                                    break;
                                case "bsp4":
                                    r.rawDataType = RawDataType.bsp4;
                                    break;
                                case "DECR":
                                    r.rawDataType = RawDataType.DECR;
                                    break;
                                case "jmad":
                                    r.rawDataType = RawDataType.jmad;
                                    break;
                                case "ltmp":
                                    r.rawDataType = RawDataType.ltmp;
                                    break;
                                case "mode1":
                                    r.rawDataType = RawDataType.mode1;
                                    break;
                                case "mode2":
                                    r.rawDataType = RawDataType.mode2;
                                    break;
                                case "PRTM":
                                    r.rawDataType = RawDataType.PRTM;
                                    break;
                                case "snd1":
                                    r.rawDataType = RawDataType.snd1;
                                    break;
                                case "snd2":
                                    r.rawDataType = RawDataType.snd2;
                                    break;
                            }

                            r.offset = Convert.ToInt32(xtr.GetAttribute("PointsToOffset"));
                            r.pointerMetaOffset = Convert.ToInt32(xtr.GetAttribute("PointerMetaOffset"));
                            r.size = Convert.ToInt32(xtr.GetAttribute("ChunkSize"));
                            int rawdataspot = Convert.ToInt32(xtr.GetAttribute("RawDataOffset"));
                            BR.BaseStream.Position = rawdataspot;
                            r.MS = new MemoryStream(r.size);
                            r.MS.Write(BR.ReadBytes(r.size), 0, r.size);

                            raw.rawChunks.Add(r);
                        }

                        break;
                }
            }

            BR.Close();
            FS.Close();
            xtr.Close();

            if (meta.type == "snd!")
            {
                Stream s = File.Open(temp + "layout", FileMode.Open);

                #region attempt to convert v1.0.0.0 files to v1.1.0.0 format
                {
                    StreamReader sr = new StreamReader(s);
                    string convertOldVersion = sr.ReadToEnd();
                    if (convertOldVersion.Contains("Version=1.0.0.0"))
                    {
                        int xx = -1;
                        while ((xx = convertOldVersion.IndexOf("entity.MetaContainers")) > -1)
                        {
                            if (convertOldVersion[xx] == '[')
                                convertOldVersion = convertOldVersion.Substring(0, xx) +
                                                "HaloMap.H2MetaContainers" +
                                                convertOldVersion.Substring(xx + "entity.MetaContainers".Length);
                            else
                                convertOldVersion = convertOldVersion.Substring(0, xx - 1) +
                                                (char)((byte)convertOldVersion[xx - 1] + 3) +   // string is 3 bytes longer
                                                "HaloMap.H2MetaContainers" +
                                                convertOldVersion.Substring(xx + "entity.MetaContainers".Length);
                        }
                        while ((xx = convertOldVersion.IndexOf("entity.MapTypes")) > -1)
                        {
                            convertOldVersion = convertOldVersion.Substring(0, xx - 1) +
                                                (char)((byte)convertOldVersion[xx - 1] + 5) +  // string is 5 bytes longer
                                                "HaloMap.Map.MapTypes" +
                                                convertOldVersion.Substring(xx + "entity.MapTypes".Length);
                        }
                        
                        // Convert the modified string into a stream
                        StreamWriter sw = new StreamWriter(s);
                        sw.BaseStream.Position = 0;
                        char[] ca = convertOldVersion.ToCharArray();
                        byte[] ba = Encoding.Default.GetBytes(ca);
                        sw.BaseStream.Write(ba, 0, ba.Length);

#if DEBUG
                        // Write an output file for testing
                        Stream s2 = File.Create(temp + "layout_test");
                        sw = new StreamWriter(s2);
                        sw.BaseStream.Write(ba, 0, ba.Length); 
                        s2.Close();
#endif
                    }

                    s.Position = 0;
                }
                #endregion
                BinaryFormatter b = new BinaryFormatter();
                Sound temps = (Sound)raw;
                temps.Permutations = (ugh_.SoundPermutationChunk[])b.Deserialize(s);
                meta.raw = temps;
                s.Close();
            }

            return raw;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public virtual void Read(int TagIndex, Map map, bool dontreadraw)
        {
        }

        /// <summary>
        /// The save raw to file.
        /// </summary>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void SaveRawToFile(string outputFilePath, Meta.Meta meta)
        {
            int x = outputFilePath.LastIndexOf('.');
            string temp = outputFilePath.Substring(0, x + 1) + meta.type + "raw";

            if (meta.type == "snd!" && 
                (meta.Map.HaloVersion == HaloVersionEnum.Halo2 ||
                 meta.Map.HaloVersion == HaloVersionEnum.Halo2Vista))
            {
                Stream s = File.Open(temp + "layout", FileMode.Create);
                BinaryFormatter b = new BinaryFormatter();
                Sound temps = (Sound)meta.raw;
                b.Serialize(s, temps.Permutations);
                s.Flush();
                s.Close();
            }

            XmlTextWriter xtw = new XmlTextWriter(temp + ".xml", Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("RawData");
            xtw.WriteAttributeString("TagType", meta.type);
            xtw.WriteAttributeString("TagName", meta.name);
            xtw.WriteAttributeString("RawType", meta.raw.containerType.ToString());
            xtw.WriteAttributeString("RawChunkCount", this.rawChunks.Count.ToString());
            xtw.WriteAttributeString("Date", DateTime.Today.ToShortDateString());
            xtw.WriteAttributeString("Time", DateTime.Now.ToShortTimeString());
            xtw.WriteAttributeString("EntityVersion", "0.1");

            BinaryWriter BW = new BinaryWriter(new FileStream(temp, FileMode.Create));
            int loc = 0;
            for (x = 0; x < this.rawChunks.Count; x++)
            {
                RawDataChunk r = this.rawChunks[x];
                if (r.offset == -1)
                {
                    continue;
                }

                xtw.WriteStartElement("RawChunk");
                xtw.WriteAttributeString("RawDataType", r.rawDataType.ToString());
                xtw.WriteAttributeString("PointerMetaOffset", r.pointerMetaOffset.ToString());
                xtw.WriteAttributeString("ChunkSize", r.size.ToString());
                xtw.WriteAttributeString("RawDataOffset", loc.ToString());
                xtw.WriteAttributeString("PointsToOffset", r.offset.ToString());
                xtw.WriteAttributeString("RawLocation", r.rawLocation.ToString());
                xtw.WriteEndElement();
                BW.BaseStream.Write(r.MS.ToArray(), 0, r.size);
                loc += r.size;
            }

            BW.Flush();
            BW.Close();
            xtw.WriteEndElement();
            xtw.Flush();
            xtw.Close();
        }

        #endregion
    }

    /// <summary>
    /// The sound.
    /// </summary>
    /// <remarks></remarks>
    public class Sound : RawDataContainer
    {
        #region Constants and Fields

        /// <summary>
        /// The permutations.
        /// </summary>
        public ugh_.SoundPermutationChunk[] Permutations;

        /// <summary>
        /// The count.
        /// </summary>
        public int count;

        /// <summary>
        /// The index.
        /// </summary>
        public int index;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Sound()
        {
            containerType = RawDataContainerType.Sound;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    ReadH2SoundData(TagIndex, map, dontreadraw);
                    break;

                    // case Map.HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    ReadH1SoundData(TagIndex, map, dontreadraw);
                    break;
            }
        }

        /// <summary>
        /// The read h 1 sound data.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadH1SoundData(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 152;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            map.BR.BaseStream.Position = tempr + 60;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.pointerMetaOffset = tempr + (x * 124) + 64 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                Raw.rawDataType = RawDataType.snd1;
                Raw.size = map.BR.ReadInt32();
                map.BR.ReadInt32();
                Raw.offset = map.BR.ReadInt32();

                if (dontreadraw == false)
                {
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                }

                this.rawChunks.Add(Raw);
            }
        }

        /// <summary>
        /// The read h 2 sound data.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadH2SoundData(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 8;

            index = map.BR.ReadInt16();
            count = 1; // map.BR.ReadInt16();
            Permutations = new ugh_.SoundPermutationChunk[count];
            for (int x = 0; x < count; x++)
            {
                int currentindex = index + x;
                Permutations[x] = map.ugh.Permutations[currentindex];
                for (int xx = 0; xx < map.ugh.Permutations[currentindex].choicecount; xx++)
                {
                    int choiceindexx = map.ugh.Permutations[currentindex].choiceindex + xx;
                    Permutations[x].Choices.Add(map.ugh.Choices[choiceindexx]);
                    for (int xxx = 0; xxx < map.ugh.Choices[choiceindexx].soundcount; xxx++)
                    {
                        int soundindex = map.ugh.Choices[choiceindexx].soundindex + xxx;
                        Permutations[x].Choices[xx].SoundChunks1.Add(map.ugh.SoundChunks1[soundindex]);

                        RawDataChunk Raw = new RawDataChunk();

                        Raw.rawDataType = RawDataType.snd1;
                        Raw.offset = map.ugh.SoundChunks1[soundindex].offset;
                        Raw.size = map.ugh.SoundChunks1[soundindex].size & 0x3FFFFFFF;
                        Raw.rawLocation = map.ugh.SoundChunks1[soundindex].rawLocation;
                        if (dontreadraw == false)
                        {
                            map.OpenMap(Raw.rawLocation);
                            map.BR.BaseStream.Position = Raw.offset;
                            Raw.MS = new MemoryStream(Raw.size);
                            Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                            map.OpenMap(MapTypes.Internal);
                        }

                        this.rawChunks.Add(Raw);
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The ugh raw container.
    /// </summary>
    /// <remarks></remarks>
    public class UghRawContainer : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UghRawContainer"/> class.
        /// </summary>
        /// <remarks></remarks>
        public UghRawContainer()
        {
            containerType = RawDataContainerType.CoconutsModel;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 80;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.rawDataType = RawDataType.snd2;
                Raw.pointerMetaOffset = tempr + (x * 44) + 8 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;

                Raw.offset = map.BR.ReadInt32();
                Raw.size = map.BR.ReadInt32();
                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }
        }

        #endregion
    }

    /// <summary>
    /// The weather.
    /// </summary>
    /// <remarks></remarks>
    public class Weather : RawDataContainer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Weather"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Weather()
        {
            containerType = RawDataContainerType.Weather;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public override void Read(int TagIndex, Map map, bool dontreadraw)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex];
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            for (int x = 0; x < tempc; x++)
            {
                RawDataChunk Raw = new RawDataChunk();
                Raw.pointerMetaOffset = tempr + (x * 140) + 64 - map.MetaInfo.Offset[TagIndex];
                map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + Raw.pointerMetaOffset;
                Raw.rawDataType = RawDataType.weat;
                Raw.offset = map.BR.ReadInt32();
                Raw.size = map.BR.ReadInt32();
                map.Functions.ParsePointer(ref Raw.offset, ref Raw.rawLocation);
                if (dontreadraw == false)
                {
                    map.OpenMap(Raw.rawLocation);
                    map.BR.BaseStream.Position = Raw.offset;
                    Raw.MS = new MemoryStream(Raw.size);
                    Raw.MS.Write(map.BR.ReadBytes(Raw.size), 0, Raw.size);
                    map.OpenMap(MapTypes.Internal);
                }

                this.rawChunks.Add(Raw);
            }
        }

        #endregion
    }
}