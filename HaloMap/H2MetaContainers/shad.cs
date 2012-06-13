// --------------------------------------------------------------------------------------------------------------------
// <copyright file="shad.cs" company="">
//   
// </copyright>
// <summary>
//   The dx shader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HaloMap.H2MetaContainers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using HaloMap.DDSFunctions;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The dx shader.
    /// </summary>
    /// <remarks></remarks>
    public class DXShader
    {
        #region Constants and Fields

        /// <summary>
        /// The my effect.
        /// </summary>
        private readonly Effect myEffect;

        /// <summary>
        /// The my title.
        /// </summary>
        private readonly string myTitle;

        /// <summary>
        /// The my file.
        /// </summary>
        private string myFile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DXShader"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="myDevice">My device.</param>
        /// <remarks></remarks>
        public DXShader(string title, string filename, ref Device myDevice)
        {
            myTitle = title;
            myFile = filename;
            string error = string.Empty;
            myEffect = Effect.FromFile(myDevice, filename, null, null, ShaderFlags.PartialPrecision, null, out error);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets MyEffect.
        /// </summary>
        /// <remarks></remarks>
        public Effect MyEffect
        {
            get
            {
                return myEffect;
            }
        }

        /// <summary>
        /// Gets MyTitle.
        /// </summary>
        /// <remarks></remarks>
        public string MyTitle
        {
            get
            {
                return myTitle;
            }
        }

        #endregion
    }

    /// <summary>
    /// The shader info.
    /// </summary>
    /// <remarks></remarks>
    public class ShaderInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The alpha.
        /// </summary>
        public AlphaType Alpha;

        /// <summary>
        /// The bitmap names.
        /// </summary>
        public List<string> BitmapNames = new List<string>();

        /// <summary>
        /// The bitmap textures.
        /// </summary>
        public Texture[] BitmapTextures;

        /// <summary>
        /// The bitmaps.
        /// </summary>
        public List<Bitmap> Bitmaps = new List<Bitmap>();

        /// <summary>
        /// The bump map bitmap.
        /// </summary>
        public Bitmap BumpMapBitmap;

        /// <summary>
        /// The bump map name.
        /// </summary>
        public string BumpMapName;

        /// <summary>
        /// The bump map texture.
        /// </summary>
        public Texture BumpMapTexture;

        /// <summary>
        /// The cube map bitmap.
        /// </summary>
        public Bitmap CubeMapBitmap;

        /// <summary>
        /// The cube map name.
        /// </summary>
        public string CubeMapName;

        /// <summary>
        /// The cube map texture.
        /// </summary>
        public Texture CubeMapTexture;

        /// <summary>
        /// The main bitmap.
        /// </summary>
        public Bitmap MainBitmap;

        /// <summary>
        /// The main name.
        /// </summary>
        public string MainName = string.Empty;

        /// <summary>
        /// The main texture.
        /// </summary>
        public Texture MainTexture;

        /// <summary>
        /// The normal map.
        /// </summary>
        public Texture NormalMap;

        /// <summary>
        /// The microdetail bitmap.
        /// </summary>
        public Bitmap microdetailBitmap;

        /// <summary>
        /// The microdetail name.
        /// </summary>
        public string microdetailName;

        /// <summary>
        /// The microdetail texture.
        /// </summary>
        public Texture microdetailTexture;

        /// <summary>
        /// The microdetailuscale.
        /// </summary>
        public float microdetailuscale;

        /// <summary>
        /// The microdetailvscale.
        /// </summary>
        public float microdetailvscale;

        /// <summary>
        /// The microdetailwscale.
        /// </summary>
        public float microdetailwscale;

        /// <summary>
        /// The primarydetail bitmap.
        /// </summary>
        public Bitmap primarydetailBitmap;

        /// <summary>
        /// The primarydetail name.
        /// </summary>
        public string primarydetailName;

        /// <summary>
        /// The primarydetail texture.
        /// </summary>
        public Texture primarydetailTexture;

        /// <summary>
        /// The primarydetailuscale.
        /// </summary>
        public float primarydetailuscale;

        /// <summary>
        /// The primarydetailvscale.
        /// </summary>
        public float primarydetailvscale;

        /// <summary>
        /// The primarydetailwscale.
        /// </summary>
        public float primarydetailwscale;

        /// <summary>
        /// The secondarydetail bitmap.
        /// </summary>
        public Bitmap secondarydetailBitmap;

        /// <summary>
        /// The secondarydetail name.
        /// </summary>
        public string secondarydetailName;

        /// <summary>
        /// The secondarydetail texture.
        /// </summary>
        public Texture secondarydetailTexture;

        /// <summary>
        /// The secondarydetailuscale.
        /// </summary>
        public float secondarydetailuscale;

        /// <summary>
        /// The secondarydetailvscale.
        /// </summary>
        public float secondarydetailvscale;

        /// <summary>
        /// The secondarydetailwscale.
        /// </summary>
        public float secondarydetailwscale;

        /// <summary>
        /// The shader name.
        /// </summary>
        public string shaderName;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        public int TagIndex;

        /// <summary>
        /// The levels.
        /// </summary>
        private int levels = 1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderInfo"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ShaderInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderInfo"/> class.
        /// </summary>
        /// <param name="TagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public ShaderInfo(int TagIndex, Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    H2ShaderInfo(TagIndex, map);
                    break;
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    CEShaderInfo(TagIndex, map);
                    break;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// The alpha type.
        /// </summary>
        /// <remarks></remarks>
        public enum AlphaType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None, 

            /// <summary>
            /// The alpha test.
            /// </summary>
            AlphaTest, 

            /// <summary>
            /// The alpha blend.
            /// </summary>
            AlphaBlend, 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The create texture.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Texture CreateTexture(ref Device device, Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            Texture texture = new Texture(
                device, bitmap.Width, bitmap.Height, 1, Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int pitch;
            GraphicsStream textureData = texture.LockRectangle(0, LockFlags.None, out pitch);

            // Debug.Assert(bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Debug.Assert(sizeof(int) == 4 && (bitmapData.Stride & 3) == 0 && (pitch & 3) == 0);
            unsafe
            {
                int* texturePointer = (int*)textureData.InternalDataPointer;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int* bitmapLinePointer = (int*)bitmapData.Scan0 + y * (bitmapData.Stride / sizeof(int));
                    int* textureLinePointer = texturePointer + y * (pitch / sizeof(int));
                    int length = bitmap.Width;
                    while (--length >= 0)
                    {
                        *textureLinePointer++ = *bitmapLinePointer++;
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
            texture.UnlockRectangle(0);
            texture.GenerateMipSubLevels();
            texture.AutoGenerateFilterType = TextureFilter.Linear;
            return texture;
        }

        /// <summary>
        /// The ce shader info.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void CEShaderInfo(int TagIndex, Map map)
        {
            this.TagIndex = TagIndex;
            if (this.TagIndex == -1)
            {
                return;
            }

            this.shaderName = map.FileNames.Name[this.TagIndex];

            map.OpenMap(MapTypes.Internal);
            int mainid = 0;
            int primarydetail = -1;
            int secondarydetail = -1;
            int micro = -1;
            switch (map.MetaInfo.TagType[TagIndex])
            {
                case "schi":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 228;
                    mainid = map.BR.ReadInt32();
                    break;
                case "soso":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 176;
                    mainid = map.BR.ReadInt32();
                    break;
                case "sgla":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 356;
                    mainid = map.BR.ReadInt32();
                    break;
                case "scex":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 900;
                    mainid = map.BR.ReadInt32();
                    break;
                case "senv":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 148;
                    mainid = map.BR.ReadInt32();
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 180;
                    this.primarydetailuscale = map.BR.ReadSingle();
                    this.primarydetailvscale = this.primarydetailuscale;
                    this.primarydetailwscale = 1;
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 196;
                    primarydetail = map.BR.ReadInt32();

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 200;
                    this.secondarydetailuscale = map.BR.ReadSingle();
                    this.secondarydetailvscale = this.secondarydetailuscale;
                    this.secondarydetailwscale = 1;
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 216;
                    secondarydetail = map.BR.ReadInt32();

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 248;
                    this.microdetailuscale = map.BR.ReadSingle();
                    this.microdetailvscale = this.secondarydetailuscale;
                    this.microdetailwscale = 1;
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 264;
                    micro = map.BR.ReadInt32();

                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 40;
                    byte alphatest = map.BR.ReadByte();
                    int test = alphatest & 1;
                    if (test != 0)
                    {
                        this.Alpha = AlphaType.AlphaTest;
                    }

                    break;
                case "swat":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 88;
                    mainid = map.BR.ReadInt32();
                    break;
                case "smet":
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 88;
                    mainid = map.BR.ReadInt32();
                    break;
            }

            map.CloseMap();

            mainid = map.Functions.ForMeta.FindMetaByID(mainid);
            primarydetail = map.Functions.ForMeta.FindMetaByID(primarydetail);
            secondarydetail = map.Functions.ForMeta.FindMetaByID(secondarydetail);
            micro = map.Functions.ForMeta.FindMetaByID(micro);
            if (mainid == -1)
            {
                return;
            }

            if (map.MetaInfo.external[mainid])
            {
                map.OpenMap(MapTypes.Bitmaps);
            }
            else
            {
                map.OpenMap(MapTypes.Internal);
            }

            Meta tempmeta = new Meta(map);
            tempmeta.ReadMetaFromMap(mainid, false);

            map.CloseMap();
            ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
            // Attempt to load LOD2, if that fails, load LOD0
            try
            {
                this.MainBitmap = pm.FindChunkAndDecode(0, 2, 0, ref tempmeta, map, 0, 0);
            }
            catch
            {
                this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
            }
            this.MainName = map.FileNames.Name[mainid];

            if (primarydetail != -1)
            {
                if (map.MetaInfo.external[primarydetail])
                {
                    map.OpenMap(MapTypes.Bitmaps);
                }
                else
                {
                    map.OpenMap(MapTypes.Internal);
                }

                tempmeta = new Meta(map);
                tempmeta.ReadMetaFromMap(primarydetail, false);

                map.CloseMap();
                pm = new ParsedBitmap(ref tempmeta, map);
                this.primarydetailBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                this.primarydetailName = map.FileNames.Name[primarydetail];
            }

            if (secondarydetail != -1)
            {
                if (map.MetaInfo.external[secondarydetail])
                {
                    map.OpenMap(MapTypes.Bitmaps);
                }
                else
                {
                    map.OpenMap(MapTypes.Internal);
                }

                tempmeta = new Meta(map);
                tempmeta.ReadMetaFromMap(secondarydetail, false);

                map.CloseMap();
                pm = new ParsedBitmap(ref tempmeta, map);
                this.secondarydetailBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                this.secondarydetailName = map.FileNames.Name[secondarydetail];
            }

            if (micro != -1)
            {
                if (map.MetaInfo.external[micro])
                {
                    map.OpenMap(MapTypes.Bitmaps);
                }
                else
                {
                    map.OpenMap(MapTypes.Internal);
                }

                tempmeta = new Meta(map);
                tempmeta.ReadMetaFromMap(micro, false);

                map.CloseMap();
                pm = new ParsedBitmap(ref tempmeta, map);
                this.microdetailBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                this.microdetailName = map.FileNames.Name[micro];
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            foreach (Bitmap b in Bitmaps)
            {
                b.Dispose();
            }
            if (BitmapTextures != null)
                foreach (Texture t in BitmapTextures)
                {
                    t.Dispose();
                }
            if (BumpMapBitmap != null) BumpMapBitmap.Dispose();
            if (BumpMapTexture != null) BumpMapTexture.Dispose();
            if (CubeMapBitmap != null) CubeMapBitmap.Dispose();
            if (CubeMapTexture != null) CubeMapTexture.Dispose();
            if (MainBitmap != null) MainBitmap.Dispose();
            if (MainTexture != null) MainTexture.Dispose();
            if (NormalMap != null) NormalMap.Dispose();
            if (microdetailBitmap != null) microdetailBitmap.Dispose();
            if (microdetailTexture != null) microdetailTexture.Dispose();
            if (primarydetailBitmap != null) primarydetailBitmap.Dispose();
            if (primarydetailTexture != null) primarydetailTexture.Dispose();
            if (secondarydetailBitmap != null) secondarydetailBitmap.Dispose();
            if (secondarydetailTexture != null) secondarydetailTexture.Dispose();
        }

        /// <summary>
        /// The h 2 shader info.
        /// </summary>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void H2ShaderInfo(int TagIndex, Map map)
        {
            bool alreadyOpen = true;
            if (!(map.isOpen && map.openMapType == MapTypes.Internal))
            {
                map.OpenMap(MapTypes.Internal);
                alreadyOpen = false;
            }

            this.TagIndex = TagIndex;
            if (this.TagIndex == -1)
            {
                return;
            }

            this.shaderName = map.FileNames.Name[this.TagIndex];
            map.BR.BaseStream.Position = map.MetaInfo.Offset[this.TagIndex] + 4;
            int tempstem = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
            if (tempstem != -1)
            {
                if (map.FileNames.Name[tempstem].IndexOf("alphatest") != -1)
                {
                    this.Alpha = AlphaType.AlphaTest;
                }
                else if (map.FileNames.Name[tempstem].IndexOf("alpha") != -1)
                {
                    this.Alpha = AlphaType.AlphaBlend;
                }
                else if (map.FileNames.Name[tempstem].IndexOf("water") != -1)
                {
                    this.Alpha = AlphaType.AlphaBlend;
                }
                else
                {
                    this.Alpha = AlphaType.None;
                }
            }
            else
            {
                this.Alpha = AlphaType.None;
            }

            map.BR.BaseStream.Position = map.MetaInfo.Offset[this.TagIndex] + 12;
            int tempc2 = map.BR.ReadInt32();
            int tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;
            if (tempc2 != 0)
            {
                map.BR.BaseStream.Position = tempr2 + 4;
                int tempcrap = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                if (tempcrap != -1)
                {
                    Meta tempmeta = new Meta(map);
                    tempmeta.ReadMetaFromMap(tempcrap, false);
                    ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                    this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                    this.MainName = map.FileNames.Name[tempcrap];
                    this.levels = pm.Properties[0].mipMapCount;
                }
            }

            //map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[this.TagIndex] + 32;
            tempc2 = map.BR.ReadInt32();
            tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;

            map.BR.BaseStream.Position = tempr2 + 24;
            int fuckr = map.BR.ReadInt32();
            if (fuckr != 0)
            {
                fuckr -= map.SecondaryMagic;
                map.BR.BaseStream.Position = fuckr;
                this.primarydetailuscale = map.BR.ReadSingle();
                this.primarydetailvscale = map.BR.ReadSingle();
                this.primarydetailwscale = map.BR.ReadSingle();
                map.BR.ReadSingle();
                this.secondarydetailuscale = map.BR.ReadSingle();
                this.secondarydetailvscale = map.BR.ReadSingle();
                this.secondarydetailwscale = map.BR.ReadSingle();
            }

            map.BR.BaseStream.Position = tempr2 + 4;
            tempc2 = map.BR.ReadInt32();
            tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;

            /*

            for (int x = 0; x < tempc2; x++)
            {
                map.BR.BaseStream.Position = tempr2 + (x * 12);
                int tempcrap = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                if (tempcrap == -1) { continue; }
                Meta tempmeta = new Meta(map);
                tempmeta.ReadMetaFromMap(tempcrap, map, false);
                Raw.ParsedBitmap pm = new Raw.ParsedBitmap(ref tempmeta, map);
                Bitmaps.Add(pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0));
               BitmapNames.Add(map.FileNames.Name[tempcrap]);
            }

            */
            // map.BR.BaseStream.Position = tempr + 20;
            // tempc2 = map.BR.ReadInt32();
            // tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;

            // tempc2 = tempc;
            // tempr2 = tempr;
            if (tempc2 != 0)
            {
                map.BR.BaseStream.Position = tempr2;
                int tempcrap = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                if (tempcrap != -1)
                {
                    int test = map.FileNames.Name[tempcrap].IndexOf("reflection_maps");
                    if (map.FileNames.Name[tempcrap].IndexOf("_bump") != -1)
                    {
                        Meta tempmeta = new Meta(map);
                        tempmeta.ReadMetaFromMap(tempcrap, false);
                        ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                      //  this.BumpMapBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                        this.BumpMapName = map.FileNames.Name[tempcrap];
                    }
                    else if (map.FileNames.Name[tempcrap].IndexOf("_cube_map") != -1)
                    {
                        Meta tempmeta = new Meta(map);
                        tempmeta.ReadMetaFromMap(tempcrap, false);
                        ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                      //  this.CubeMapBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                        this.CubeMapName = map.FileNames.Name[tempcrap];
                    }
                    else if (map.FileNames.Name[tempcrap].IndexOf("default_") == -1 && this.MainBitmap == null &&
                             test == -1)
                    {
                        Meta tempmeta = new Meta(map);
                        tempmeta.ReadMetaFromMap(tempcrap, false);
                        ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                        
                        // Try to load LOD2-MIP3 if that fails, load LOD2-MIP0, otherwise LOD0-MIP0
                        this.MainBitmap = pm.FindChunkAndDecode(0, 2, 3, ref tempmeta, map, 0, 0);
                        if (this.MainBitmap == null)
                        {
                            this.MainBitmap = pm.FindChunkAndDecode(0, 2, 0, ref tempmeta, map, 0, 0);
                            if (this.MainBitmap == null)
                                this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                        }

                        this.MainName = map.FileNames.Name[tempcrap];
                        this.levels = pm.Properties[0].mipMapCount;
                    }
                    else if (test != -1)
                    {
                        map.BR.BaseStream.Position += 8;
                        tempcrap = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                        Meta tempmeta = new Meta(map);
                        tempmeta.ReadMetaFromMap(tempcrap, false);
                        ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);

                        // this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map);
                        // this.MainName = map.FileNames.Name[tempcrap];
                    }
                }

                map.BR.BaseStream.Position = tempr2 + 24;
                tempcrap = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                if (tempcrap != -1 && this.MainBitmap == null)
                {
                    Meta tempmeta = new Meta(map);
                    tempmeta.ReadMetaFromMap(tempcrap, false);
                    ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                    this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                    this.MainName = map.FileNames.Name[tempcrap];
                    this.levels = pm.Properties[0].mipMapCount;
                }
                else if (this.MainBitmap == null)
                {
                    map.BR.BaseStream.Position = tempr2 + 12;
                    tempcrap = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (tempcrap != -1)
                    {
                        Meta tempmeta = new Meta(map);
                        tempmeta.ReadMetaFromMap(tempcrap, false);
                        ParsedBitmap pm = new ParsedBitmap(ref tempmeta, map);
                        this.MainBitmap = pm.FindChunkAndDecode(0, 0, 0, ref tempmeta, map, 0, 0);
                        this.MainName = map.FileNames.Name[tempcrap];
                        this.levels = pm.Properties[0].mipMapCount;
                    }
                }
            }

            if (!alreadyOpen)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The kill bitmaps.
        /// </summary>
        /// <remarks></remarks>
        public void KillBitmaps()
        {
            BumpMapBitmap = null;
            CubeMapBitmap = null;
            MainBitmap = null;
        }

        /// <summary>
        /// The make textures.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public void MakeTextures(ref Device device)
        {
            /*
            BitmapTextures = new Texture[Bitmaps.Count];
            for (int x = 0; x < Bitmaps.Count; x++) BitmapTextures[x] = CreateTexture(ref device, Bitmaps[x]);
             */
            if (BumpMapBitmap != null)
            {
                BumpMapTexture = CreateTexture(ref device, BumpMapBitmap);
                SurfaceDescription desc = BumpMapTexture.GetLevelDescription(0);
                NormalMap = new Texture(
                    device, desc.Width, desc.Height, BumpMapTexture.LevelCount, 0, Format.A8R8G8B8, Pool.Managed);
                TextureLoader.ComputeNormalMap(NormalMap, BumpMapTexture, 0, Channel.Alpha, 1.0f);
            }

            if (primarydetailBitmap != null)
            {
                primarydetailTexture = CreateTexture(ref device, primarydetailBitmap);
            }

            if (secondarydetailBitmap != null)
            {
                secondarydetailTexture = CreateTexture(ref device, secondarydetailBitmap);
            }

            if (microdetailBitmap != null)
            {
                microdetailTexture = CreateTexture(ref device, microdetailBitmap);
            }

            if (CubeMapBitmap != null)
            {
                // CubeMapTexture = Texture.FromBitmap(device, CubeMapBitmap, Usage.AutoGenerateMipMap, Pool.Managed);
            }

            if (MainBitmap != null)
            {
                MainTexture = CreateTexture(ref device, MainBitmap);

                // try
                // {
                // }
                // catch
                // {
                // MainTexture = Texture.FromBitmap(device, MainBitmap, Usage.AutoGenerateMipMap, Pool.Managed);
                // }
            }
        }

        #endregion
    }
}