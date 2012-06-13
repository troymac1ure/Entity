// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The bsp info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.Collections.Generic;

    /// <summary>
    /// The bsp info.
    /// </summary>
    /// <remarks></remarks>
    public class BSPInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The halo 1 vertice count.
        /// </summary>
        public int Halo1VerticeCount;

        /// <summary>
        /// The light map_ palettes.
        /// </summary>
        public List<BSPContainer.Palette_Color[]> LightMap_Palettes = new List<BSPContainer.Palette_Color[]>();

        /// <summary>
        /// The light map_ tag number.
        /// </summary>
        public int LightMap_TagNumber = -1;

        /// <summary>
        /// The scenery chunk_ bitmap_ index.
        /// </summary>
        public int[] SceneryChunk_Bitmap_Index;

        /// <summary>
        /// The scenery chunk_ light map_ index.
        /// </summary>
        public int[] SceneryChunk_LightMap_Index;

        /// <summary>
        /// The visual chunk_ bitmap_ index.
        /// </summary>
        public int[] VisualChunk_Bitmap_Index;

        /// <summary>
        /// The visual chunk_ light map_ index.
        /// </summary>
        public int[] VisualChunk_LightMap_Index;

        /// <summary>
        /// The ident.
        /// </summary>
        public int ident;

        /// <summary>
        /// The lightmapident.
        /// </summary>
        public int lightmapident;

        /// <summary>
        /// The lightmapoffset.
        /// </summary>
        public int lightmapoffset;

        /// <summary>
        /// The lightmapsize.
        /// </summary>
        public int lightmapsize;

        /// <summary>
        /// The lightmapTagIndex.
        /// </summary>
        public int lightmapTagIndex;

        /// <summary>
        /// The location.
        /// </summary>
        public MapTypes location;

        /// <summary>
        /// The magic.
        /// </summary>
        public int magic;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The palettesoffset.
        /// </summary>
        public int palettesoffset;

        /// <summary>
        /// The pointer offset.
        /// </summary>
        public int pointerOffset;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        public int TagIndex;

        #endregion

        #region Public Methods

        /// <summary>
        /// The write palettes.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void WritePalettes(Map map)
        {
            map.OpenMap(MapTypes.Internal);
            map.BW.BaseStream.Position = palettesoffset;
            foreach (BSPContainer.Palette_Color[] palette in LightMap_Palettes)
            {
                for (int x = 0; x < 256; x++)
                {
                    map.BW.Write((byte)palette[x].r);
                    map.BW.Write((byte)palette[x].g);
                    map.BW.Write((byte)palette[x].b);
                    map.BW.Write((byte)palette[x].a);
                }
            }

            map.CloseMap();
        }

        #endregion
    }
}