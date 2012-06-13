// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaSplitter.cs" company="">
//   
// </copyright>
// <summary>
//   The meta splitter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.ChunkCloning
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.RawData;

    /// <summary>
    /// The meta splitter.
    /// </summary>
    /// <remarks></remarks>
    public class MetaSplitter
    {
        #region Constants and Fields

        /// <summary>
        /// The header.
        /// </summary>
        public SplitReflexive Header;

        /// <summary>
        /// The magic.
        /// </summary>
        public int magic;

        // public List<Meta.Item> Items;
        /// <summary>
        /// The name.
        /// </summary>
        public string name;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The raw.
        /// </summary>
        public RawDataContainer raw;

        /// <summary>
        /// The rawtype.
        /// </summary>
        public RawDataContainerType rawtype;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        public int TagIndex;

        /// <summary>
        /// The type.
        /// </summary>
        public string type;

        #endregion

        #region Public Methods

        /// <summary>
        /// The split with ifp.
        /// </summary>
        /// <param name="ifp">The ifp.</param>
        /// <param name="meta">The meta.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void SplitWithIFP(ref IFPIO ifp, ref Meta meta, Map map)
        {
            this.type = meta.type;
            this.TagIndex = meta.TagIndex;
            this.name = meta.name;
            this.offset = meta.offset;
            this.magic = meta.magic;
            this.raw = meta.raw;
            this.rawtype = meta.rawType;
            map.OpenMap(MapTypes.Internal);
            if (ifp.items != null)
            {
                map.BR.BaseStream.Position = meta.offset;
                Header = new SplitReflexive();
                Header.offset = 0;
                Header.Chunks = new List<SplitReflexive>();
                Header.translation = 0;

                // Header.MS = new MemoryStream(ifp.headerSize);
                // Header.MS.Write(map.BR.ReadBytes(ifp.headerSize), 0, ifp.headerSize);
                Header.chunksize = ifp.headerSize;
                Header.chunkcount = 1;
                Header.splitReflexiveType = SplitReflexive.SplitReflexiveType.Container;
                Header.realtranslation = meta.offset;
                if (meta.type == "sbsp")
                {
                    int p = map.BSP.FindBSPNumberByBSPIdent(meta.ident);
                    CycleElements(
                        ref Header, ifp.items, ref meta, meta.offset, map, meta.TagIndex, map.BSP.sbsp[p].magic);
                }
                else if (meta.type == "ltmp")
                {
                    int p = map.BSP.FindBSPNumberByLightMapIdent(meta.ident);
                    CycleElements(
                        ref Header, ifp.items, ref meta, meta.offset, map, meta.TagIndex, map.BSP.sbsp[p].magic);
                }
                else
                {
                    // not "sbsp" or "ltmp"
                    CycleElements(ref Header, ifp.items, ref meta, meta.offset, map, meta.TagIndex, map.SecondaryMagic);
                }
            }

            map.CloseMap();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get base string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The get base string.</returns>
        /// <remarks></remarks>
        private static string GetBaseString(string name)
        {
            string[] split = name.Split('[');
            string tempstring = split[0].Trim();
            return tempstring;
        }

        /// <summary>
        /// The cycle elements.
        /// </summary>
        /// <param name="reflex">The reflex.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="meta">The meta.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="map">The map.</param>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="magic">The magic.</param>
        /// <remarks></remarks>
        private void CycleElements(
            ref SplitReflexive reflex, object[] elements, ref Meta meta, int offset, Map map, int TagIndex, int magic)
        {
            for (int x = 0; x < reflex.chunkcount; x++)
            {
                SplitReflexive chunkreflexive = new SplitReflexive();
                map.BR.BaseStream.Position = reflex.realtranslation + (x * reflex.chunksize);
                chunkreflexive.MS = new MemoryStream(reflex.chunksize);
                chunkreflexive.MS.Write(map.BR.ReadBytes(reflex.chunksize), 0, reflex.chunksize);
                chunkreflexive.chunksize = reflex.chunksize;
                chunkreflexive.splitReflexiveType = SplitReflexive.SplitReflexiveType.Chunk;

                reflex.Chunks.Add(chunkreflexive);

                for (int xx = 0; xx < elements.Length; xx++)
                {
                    IFPIO.BaseObject tempbase = (IFPIO.BaseObject)elements[xx];

                    switch (tempbase.ObjectType)
                    {
                        case IFPIO.ObjectEnum.Struct:
                            IFPIO.Reflexive tempreflex = (IFPIO.Reflexive)tempbase;

                            SplitReflexive r = new SplitReflexive();
                            map.BR.BaseStream.Position = offset + tempreflex.offset + (x * reflex.chunksize);
                            r.mapOffset = (int)map.BR.BaseStream.Position;
                            r.chunkcount = map.BR.ReadInt32();
                            if (r.chunkcount == 0)
                            {
                                continue;
                            }

                            r.chunksize = tempreflex.chunkSize;
                            r.translation = map.BR.ReadInt32() - magic;
                            r.pointstoTagIndex = map.Functions.ForMeta.FindMetaByOffset(r.translation);
                            r.description = tempreflex.name;
                            if (r.pointstoTagIndex == -1)
                            {
                                continue;
                            }

                            // r.parent = reflex.description;// parentname;
                            r.realtranslation = r.translation;
                            r.realTagIndex = r.pointstoTagIndex;

                            r.label = tempreflex.label;

                            r.pointstoTagIndex = meta.TagIndex;
                            r.pointstotagtype = meta.type;
                            r.pointstotagname = meta.name;
                            r.offset = tempreflex.offset;
                            r.intag = meta.TagIndex;
                            r.intagtype = meta.type;
                            r.intagname = meta.name;
                            r.translation -= map.MetaInfo.Offset[r.realTagIndex];

                            r.inchunknumber = x;
                            r.splitReflexiveType = SplitReflexive.SplitReflexiveType.Container;
                            r.Chunks = new List<SplitReflexive>();
                            CycleElements(
                                ref r, tempreflex.items, ref meta, r.realtranslation, map, r.pointstoTagIndex, magic);
                            reflex.Chunks[x].ChunkResources.Add(r);
                            meta.reflexivecount++;

                            break;
                        case IFPIO.ObjectEnum.Ident:
                            IFPIO.Ident tempident = (IFPIO.Ident)tempbase;
                            SplitIdent i = new SplitIdent();
                            map.BR.BaseStream.Position = offset + tempident.offset + (x * reflex.chunksize);
                            i.mapOffset = (int)map.BR.BaseStream.Position;
                            i.ident = map.BR.ReadInt32();
                            if (i.ident != -1)
                            {
                                try
                                {
                                    i.pointstoTagIndex = map.Functions.ForMeta.FindMetaByID(i.ident);
                                    i.pointstotagtype = map.MetaInfo.TagType[i.pointstoTagIndex];
                                    i.pointstotagname = map.FileNames.Name[i.pointstoTagIndex];
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                            i.mapOffset = offset + tempident.offset + (x * reflex.chunksize);

                            i.offset = tempident.offset;
                            i.inchunknumber = x;
                            i.intag = TagIndex;
                            i.intagtype = map.MetaInfo.TagType[i.intag];
                            i.intagname = map.FileNames.Name[i.intag];
                            i.description = tempident.name;

                            reflex.Chunks[x].ChunkResources.Add(i);
                            break;
                        case IFPIO.ObjectEnum.StringID:
                            IFPIO.SID tempstringid = (IFPIO.SID)tempbase;
                            SplitString si = new SplitString();
                            map.BR.BaseStream.Position = offset + tempstringid.offset + (x * reflex.chunksize);
                            si.mapOffset = (int)map.BR.BaseStream.Position;
                            si.id = map.BR.ReadUInt16();
                            if (si.id == 0 | si.id >= map.MapHeader.scriptReferenceCount)
                            {
                                continue;
                            }

                            map.BR.ReadByte();
                            int temp = map.BR.ReadByte();
                            if (temp != map.Strings.Length[si.id])
                            {
                                continue;
                            }

                            si.mapOffset = offset + tempstringid.offset + (x * reflex.chunksize);
                            si.offset = si.mapOffset - map.MetaInfo.Offset[TagIndex];

                            si.offset = tempstringid.offset;
                            si.inchunknumber = x;
                            si.name = map.Strings.Name[si.id];
                            si.intag = TagIndex;
                            si.intagtype = map.MetaInfo.TagType[si.intag];
                            si.intagname = map.FileNames.Name[si.intag];
                            si.description = tempstringid.name;
                            reflex.Chunks[x].ChunkResources.Add(si);
                            break;
                    }
                }
            }

            return;
        }

        #endregion

        /// <summary>
        /// The split ident.
        /// </summary>
        /// <remarks></remarks>
        public class SplitIdent : Meta.Ident
        {
            #region Constants and Fields

            /// <summary>
            /// The inchunknumber.
            /// </summary>
            public int inchunknumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SplitIdent"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SplitIdent()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SplitIdent"/> class.
            /// </summary>
            /// <param name="si">The si.</param>
            /// <remarks></remarks>
            public SplitIdent(SplitIdent si)
            {
                inchunknumber = si.inchunknumber;
                this.pointstotagname = si.pointstotagname;
                this.pointstoTagIndex = si.pointstoTagIndex;
                this.pointstotagtype = si.pointstotagtype;
                this.child = si.child;
                this.description = si.description;
                this.intag = si.intag;
                this.intagname = si.intagname;
                this.intagtype = si.intagtype;
                this.mapOffset = si.mapOffset;
                this.offset = si.offset;
                this.parent = si.parent;
                this.sibling = si.sibling;
                this.type = si.type;
            }

            #endregion
        }

        /// <summary>
        /// The split reflexive.
        /// </summary>
        /// <remarks></remarks>
        public class SplitReflexive : Meta.Reflexive
        {
            #region Constants and Fields

            /// <summary>
            /// The chunk resources.
            /// </summary>
            public List<Meta.Item> ChunkResources = new List<Meta.Item>();

            /// <summary>
            /// The chunks.
            /// </summary>
            public List<SplitReflexive> Chunks = new List<SplitReflexive>();

            /// <summary>
            /// The ms.
            /// </summary>
            public MemoryStream MS;

            /// <summary>
            /// The inchunknumber.
            /// </summary>
            public int inchunknumber;

            /// <summary>
            /// The label.
            /// </summary>
            public string label;

            /// <summary>
            /// The realTagIndex.
            /// </summary>
            public int realTagIndex;

            /// <summary>
            /// The realtranslation.
            /// </summary>
            public int realtranslation;

            /// <summary>
            /// The split reflexive type.
            /// </summary>
            public SplitReflexiveType splitReflexiveType;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SplitReflexive"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SplitReflexive()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SplitReflexive"/> class.
            /// </summary>
            /// <param name="sr">The sr.</param>
            /// <remarks></remarks>
            public SplitReflexive(SplitReflexive sr)
            {
                inchunknumber = sr.inchunknumber;
                ChunkResources = new List<Meta.Item>(sr.ChunkResources.Count);
                for (int count = 0; count < ChunkResources.Capacity; count++)
                {
                    ChunkResources.Add(new SplitIdent());
                    this.chunkcount = sr.chunkcount;
                    this.chunksize = sr.chunksize;
                    this.pointstotagname = sr.pointstotagname;
                    this.pointstoTagIndex = sr.pointstoTagIndex;
                    this.pointstotagtype = sr.pointstotagtype;
                    ChunkResources[count].child = sr.ChunkResources[count].child;
                    ChunkResources[count].description = sr.ChunkResources[count].description;
                    ChunkResources[count].intag = sr.ChunkResources[count].intag;
                    ChunkResources[count].intagname = sr.ChunkResources[count].intagname;
                    ChunkResources[count].intagtype = sr.ChunkResources[count].intagtype;
                    ChunkResources[count].mapOffset = sr.ChunkResources[count].mapOffset;
                    ChunkResources[count].offset = sr.ChunkResources[count].offset;
                    ChunkResources[count].parent = sr.ChunkResources[count].parent;
                    ChunkResources[count].sibling = sr.ChunkResources[count].sibling;
                    ChunkResources[count].type = sr.ChunkResources[count].type;
                }

                if (sr.Chunks.Count != 0)
                {
                    MessageBox.Show("sr.Chunks == " + sr.Chunks.Count + " -> location METASPLITTER.CS: Line 65");
                }

                Chunks = new List<SplitReflexive>(sr.Chunks);
                realtranslation = sr.realtranslation;
                realTagIndex = sr.realTagIndex;
                MS = sr.MS;
                splitReflexiveType = sr.splitReflexiveType;
                label = sr.label;
            }

            #endregion

            #region Enums

            /// <summary>
            /// The split reflexive type.
            /// </summary>
            /// <remarks></remarks>
            public enum SplitReflexiveType
            {
                /// <summary>
                /// The chunk.
                /// </summary>
                Chunk, 

                /// <summary>
                /// The container.
                /// </summary>
                Container
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The find.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public Meta.Item Find(int offset)
            {
                for (int i = 0; i < ChunkResources.Count; i++)
                {
                    if (ChunkResources[i].offset == offset)
                    {
                        return ChunkResources[i];
                    }
                }

                return null;
            }

            #endregion
        }

        /// <summary>
        /// The split string.
        /// </summary>
        /// <remarks></remarks>
        public class SplitString : Meta.String
        {
            #region Constants and Fields

            /// <summary>
            /// The inchunknumber.
            /// </summary>
            public int inchunknumber;

            #endregion
        }
    }
}