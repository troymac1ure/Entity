// --------------------------------------------------------------------------------------------------------------------
// <copyright file="hlmt.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for hlmt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using HaloMap.Map;

    /// <summary>
    /// Summary description for hlmt.
    /// </summary>
    /// <remarks></remarks>
    public class hlmtContainer
    {
        #region Constants and Fields

        /// <summary>
        /// The permutations.
        /// </summary>
        public PermutationInfo Permutations;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        public int TagIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="hlmtContainer"/> class.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public hlmtContainer(int tagIndex, Map map)
        {
            bool alreadyOpen = true;
            if (!(map.isOpen && map.openMapType == MapTypes.Internal))
            {
                map.OpenMap(MapTypes.Internal);
                alreadyOpen = false;
            }

            this.TagIndex = tagIndex;

            Permutations = new PermutationInfo();
            Permutations.Name = map.FileNames.Name[TagIndex];
            map.BR.BaseStream.Position = map.MetaInfo.Offset[TagIndex] + 112;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            Permutations.Piece = new PermutationInfo.PermutationPiece[tempc];
            for (int x = 0; x < tempc; x++)
            {
                Permutations.Piece[x] = new PermutationInfo.PermutationPiece();
                map.BR.BaseStream.Position = tempr + (x * 16);
                Permutations.Piece[x].PieceName = map.Strings.Name[map.BR.ReadInt16()];
                map.BR.BaseStream.Position = tempr + (x * 16) + 8;
                int tempc2 = map.BR.ReadInt32();
                int tempr2 = map.BR.ReadInt32() - map.SecondaryMagic;
                Permutations.Piece[x].Permutation = new PermutationInfo.PermutationPiece.PermutationVariation[tempc2];
                for (int xx = 0; xx < tempc2; xx++)
                {
                    Permutations.Piece[x].Permutation[xx] = new PermutationInfo.PermutationPiece.PermutationVariation();
                    map.BR.BaseStream.Position = tempr2 + (xx * 8);
                    string temps2 = map.Strings.Name[map.BR.ReadInt16()];
                    Permutations.Piece[x].Permutation[xx].PermutationNameX = temps2;
                }
            }

            if (!alreadyOpen)
            {
                map.CloseMap();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The find permutation.
        /// </summary>
        /// <param name="permutationName">The permutation name.</param>
        /// <returns>The find permutation.</returns>
        /// <remarks></remarks>
        public int FindPermutation(string permutationName)
        {
            for (int x = 0; x < this.Permutations.Piece.Length; x++)
            {
                if (this.Permutations.Piece[x].PieceName == permutationName)
                {
                    return x;
                }
            }

            return 0;
        }

        /// <summary>
        /// The find permutation by base class.
        /// </summary>
        /// <param name="TagIndex">
        /// The TagIndex.
        /// </param>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <returns>
        /// The find permutation by base class.
        /// </returns>
        //public int FindPermutationByBaseClass(int tagIndex, ref Map map)
        //{
        //    if (tagIndex == -1)
        //    {
        //        return -1;
        //    }

        //    int tempoff = 0;
        //    switch (map.MetaInfo.TagType[tagIndex])
        //    {
        //        case "itmc":

        //        case "vehc":
        //            tempoff = 24;
        //            break;
        //        case "vehi":

        //        case "bloc":
        //        case "bipd":
        //        case "mach":
        //        case "scen":
        //        case "eqip":
        //        case "weap":
        //            tempoff = 48;
        //            break;
        //        default:
        //            return -1;
        //    }

        //    map.OpenMap(MapTypes.Internal);

        //    map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + tempoff;
        //    string tempname = map.Strings.Name[map.BR.ReadInt16()];

        //    return FindPermutation(tempname);
        //}

        #endregion

        /// <summary>
        /// The permutation info.
        /// </summary>
        /// <remarks></remarks>
        public class PermutationInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The piece.
            /// </summary>
            public PermutationPiece[] Piece;

            #endregion

            /// <summary>
            /// The permutation piece.
            /// </summary>
            /// <remarks></remarks>
            public class PermutationPiece
            {
                #region Constants and Fields

                /// <summary>
                /// The permutation.
                /// </summary>
                public PermutationVariation[] Permutation;

                /// <summary>
                /// The piece name.
                /// </summary>
                public string PieceName;

                #endregion

                /// <summary>
                /// The permutation variation.
                /// </summary>
                /// <remarks></remarks>
                public class PermutationVariation
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The permutation name x.
                    /// </summary>
                    public string PermutationNameX;

                    #endregion
                }
            }
        }
    }
}