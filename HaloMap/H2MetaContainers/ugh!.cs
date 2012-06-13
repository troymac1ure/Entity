// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ugh!.cs" company="">
//   
// </copyright>
// <summary>
//   The ugh_.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using HaloMap.Map;

    /// <summary>
    /// The ugh_.
    /// </summary>
    /// <remarks></remarks>
    public class ugh_
    {
        #region Constants and Fields

        /// <summary>
        /// The choices.
        /// </summary>
        public List<SoundChoiceChunk> Choices = new List<SoundChoiceChunk>(0);

        /// <summary>
        /// The choices translation.
        /// </summary>
        public int ChoicesTranslation;

        /// <summary>
        /// The permutations.
        /// </summary>
        public List<SoundPermutationChunk> Permutations = new List<SoundPermutationChunk>(0);

        /// <summary>
        /// The permutations translation.
        /// </summary>
        public int PermutationsTranslation;

        /// <summary>
        /// The sound chunk 1 translation.
        /// </summary>
        public int SoundChunk1Translation;

        /// <summary>
        /// The sound chunks 1.
        /// </summary>
        public List<SoundType1Chunk> SoundChunks1 = new List<SoundType1Chunk>(0);

        /// <summary>
        /// The sound chunks 2.
        /// </summary>
        public List<SoundType2Chunk> SoundChunks2 = new List<SoundType2Chunk>(0);

        /// <summary>
        /// The sound names.
        /// </summary>
        public List<string> SoundNames = new List<string>(0);

        /// <summary>
        /// The sound names translation.
        /// </summary>
        public int SoundNamesTranslation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ugh_"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public ugh_(ref Meta.Meta meta)
        {
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 16;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BR.BaseStream.Position = tempr;
            SoundNamesTranslation = tempr;
            for (int x = 0; x < tempc; x++)
            {
                SoundNames.Add(meta.Map.Strings.Name[BR.ReadUInt16()]);
                BR.ReadInt16();
            }

            BR.BaseStream.Position = 32;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BR.BaseStream.Position = tempr;
            PermutationsTranslation = tempr;
            for (int x = 0; x < tempc; x++)
            {
                SoundPermutationChunk tempp = new SoundPermutationChunk();
                tempp.unknown1 = BR.ReadInt32();
                tempp.unknown2 = BR.ReadInt32();
                tempp.choiceindex = BR.ReadUInt16();
                tempp.choicecount = BR.ReadUInt16();

                Permutations.Add(tempp);
            }

            BR.BaseStream.Position = 40;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BR.BaseStream.Position = tempr;
            ChoicesTranslation = tempr;
            for (int x = 0; x < tempc; x++)
            {
                SoundChoiceChunk tempp = new SoundChoiceChunk();
                tempp.NameIndex = BR.ReadUInt16();
                tempp.Name = meta.Map.Strings.Name[tempp.NameIndex];
                tempp.unknown1 = BR.ReadInt16();
                tempp.unknown2 = BR.ReadInt32();
                tempp.unknown3 = BR.ReadInt32();
                tempp.soundindex = BR.ReadUInt16();
                tempp.soundcount = BR.ReadUInt16();

                Choices.Add(tempp);
            }

            BR.BaseStream.Position = 64;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            SoundChunk1Translation = tempr;
            for (int x = 0; x < tempc; x++)
            {
                BR.BaseStream.Position = tempr + (x * 12);
                SoundType1Chunk tempp = new SoundType1Chunk();
                tempp.offset = BR.ReadInt32();
                tempp.size = BR.ReadUInt16();
                meta.Map.Functions.ParsePointer(ref tempp.offset, ref tempp.rawLocation);
                tempp.unknown1 = BR.ReadUInt16();
                tempp.unknown2 = BR.ReadInt32();
                SoundChunks1.Add(tempp);
            }

            BR.BaseStream.Position = 80;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;

            for (int x = 0; x < tempc; x++)
            {
                BR.BaseStream.Position = tempr + (x * 12) + 8;
                SoundType2Chunk tempp = new SoundType2Chunk();
                tempp.offset = BR.ReadInt32();
                tempp.size = BR.ReadInt32();
                meta.Map.Functions.ParsePointer(ref tempp.offset, ref tempp.rawLocation);
                SoundChunks2.Add(tempp);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get ugh container info.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public static void GetUghContainerInfo(Map map)
        {
            Meta.Meta tempughmeta = new Meta.Meta(map);
            map.OpenMap(MapTypes.Internal);
            tempughmeta.ReadMetaFromMap(map.IndexHeader.metaCount - 1, true);
            map.ugh = new ugh_(ref tempughmeta);
            map.CloseMap();
            
        }

        #endregion

        /// <summary>
        /// The sound choice chunk.
        /// </summary>
        /// <remarks></remarks>
        [Serializable]
        public class SoundChoiceChunk
        {
            #region Constants and Fields

            /// <summary>
            /// The name index.
            /// </summary>
            public ushort NameIndex;

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The unknown 1.
            /// </summary>
            public short unknown1;

            /// <summary>
            /// The unknown 2.
            /// </summary>
            public int unknown2;

            /// <summary>
            /// The unknown 3.
            /// </summary>
            public int unknown3;

            /// <summary>
            /// The soundindex.
            /// </summary>
            public ushort soundindex;

            /// <summary>
            /// The soundcount.
            /// </summary>
            public ushort soundcount;

            /// <summary>
            /// The sound chunks 1.
            /// </summary>
            public List<SoundType1Chunk> SoundChunks1 = new List<SoundType1Chunk>(0);

            #endregion
        }

        /// <summary>
        /// The sound permutation chunk.
        /// </summary>
        /// <remarks></remarks>
        [Serializable]
        public class SoundPermutationChunk
        {
            #region Constants and Fields

            /// <summary>
            /// The unknown 1.
            /// </summary>
            public int unknown1;

            /// <summary>
            /// The unknown 2.
            /// </summary>
            public int unknown2;

            /// <summary>
            /// The choiceindex.
            /// </summary>
            public ushort choiceindex;

            /// <summary>
            /// The choicecount.
            /// </summary>
            public ushort choicecount;

            /// <summary>
            /// The choices.
            /// </summary>
            public List<SoundChoiceChunk> Choices = new List<SoundChoiceChunk>(0);

            #endregion
        }

        /// <summary>
        /// The sound type 1 chunk.
        /// </summary>
        /// <remarks></remarks>
        [Serializable]
        public class SoundType1Chunk
        {
            #region Constants and Fields

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The raw location.
            /// </summary>
            public MapTypes rawLocation;

            /// <summary>
            /// The size.
            /// </summary>
            public ushort size;

            /// <summary>
            /// The unknown 1.
            /// </summary>
            public ushort unknown1;

            /// <summary>
            /// The unknown 2.
            /// </summary>
            public int unknown2;

            #endregion
        }

        /// <summary>
        /// The sound type 2 chunk.
        /// </summary>
        /// <remarks></remarks>
        public class SoundType2Chunk
        {
            #region Constants and Fields

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The raw location.
            /// </summary>
            public MapTypes rawLocation;

            /// <summary>
            /// The size.
            /// </summary>
            public int size;

            #endregion
        }
    }
}