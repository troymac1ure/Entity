// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaScanner.cs" company="">
//   
// </copyright>
// <summary>
//   The meta scanner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Meta
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The meta scanner.
    /// </summary>
    /// <remarks></remarks>
    public class MetaScanner
    {
        /// <summary>
        /// 
        /// </summary>
        private Meta meta;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaScanner"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public MetaScanner(Meta meta)
        {
            this.meta = meta;
        }

        #region Public Methods

        /// <summary>
        /// The scan manually.
        /// </summary>
        /// <remarks></remarks>
        public void ScanManually()
        {
            BinaryReader BR = new BinaryReader(meta.MS);

            byte tempbyte, tempbyte2;
            short tempshort;
            int tempint, tempint2, tempintx;
            int oldint = 0;
            //int lastReflex;

            int tempindex;
            int offset = 0;
            int size = meta.size;
            int bspindex = -1;
            int bspstringidminoffset = 0;
            if (meta.type == "sbsp")
            {
                bspindex = meta.Map.BSP.FindBSPNumberByBSPIdent(meta.ident);
                BR.BaseStream.Position = 340;
                bspstringidminoffset = BR.ReadInt32() - meta.magic - meta.offset;
            }

            for (int y = 0; y < 2; y++)
            {
                BR.BaseStream.Position = offset;
                for (int x = 0; x < size; x += 4)
                {
                    if (x + 4 > size)
                    {
                        continue;
                    }

                    tempint = BR.ReadInt32();
                    tempint2 = tempint - meta.magic;
                    tempintx = (oldint >> 16) & 0xFFFF;
                    if (y == 1)
                    {
                        goto identstuff;
                    }

                    if (x == 0 && meta.Map.HaloVersion == HaloVersionEnum.HaloCE && bspindex >= 0)
                    {
                        goto crap;
                    }

                    if (tempintx == 51914 && meta.Map.HaloVersion == HaloVersionEnum.Halo1 && bspindex >= 0)
                    {
                        goto crap;
                    }

                    if (tempintx != 0)
                    {
                        if (bspindex >= 0)
                        {
                            if (x > 11)
                            {
                                oldint = tempint;
                                goto identstuff;
                            }
                        }
                        else
                        {
                            oldint = tempint;
                            goto identstuff;
                        }
                    }

                crap:
                    if (bspindex >= 0)
                    {
                        if (tempint2 < meta.Map.BSP.sbsp[bspindex].offset ||
                            tempint2 >= meta.Map.BSP.sbsp[bspindex].size + meta.Map.BSP.sbsp[bspindex].offset)
                        {
                            oldint = tempint;
                            goto identstuff;
                        }
                    }
                    else
                    {
                        switch (meta.Map.HaloVersion)
                        {
                            case HaloVersionEnum.Halo2:
                                if (tempint2 < meta.Map.MapHeader.indexOffset + meta.Map.MapHeader.metaStart ||
                                    tempint2 >=
                                    meta.Map.MapHeader.indexOffset + meta.Map.MapHeader.metaStart + meta.Map.MapHeader.metaSize)
                                {
                                    oldint = tempint;
                                    goto identstuff;
                                }

                                break;
                            case HaloVersionEnum.HaloCE:
                            case HaloVersionEnum.Halo1:
                                if (tempint2 < meta.Map.MetaInfo.Offset[meta.TagIndex] ||
                                    tempint2 >= meta.Map.MetaInfo.Offset[meta.TagIndex] + meta.Map.MetaInfo.Size[meta.TagIndex])
                                {
                                    oldint = tempint;
                                    goto identstuff;
                                }

                                break;
                        }
                    }

                    int tempint3 = 0;
                    if (bspindex >= 0)
                    {
                        tempint3 = meta.Map.BSP.sbsp[bspindex].TagIndex;
                        if (tempintx != 0)
                        {
                            oldint = 1;
                        }

                        if (x == 0 && meta.Map.HaloVersion == HaloVersionEnum.HaloCE)
                        {
                            oldint = 1;
                        }
                    }
                    else
                    {
                        if (meta.Map.HaloVersion == HaloVersionEnum.Halo2 ||
                            meta.Map.HaloVersion == HaloVersionEnum.Halo2Vista)
                        {
                            tempint3 = meta.Map.Functions.ForMeta.FindMetaByOffset(tempint2);
                        }
                        else if (meta.Map.HaloVersion == HaloVersionEnum.HaloCE ||
                                 meta.Map.HaloVersion == HaloVersionEnum.Halo1)
                        {
                            tempint3 = meta.Map.Functions.ForMeta.FindMetaByOffset(tempint2);
                            if (tempint3 != meta.TagIndex)
                            {
                                oldint = tempint;
                                goto identstuff;
                            }
                        }
                    }

                    if (tempint3 != -1)
                    {
                        int tempint4 = tempint2 - meta.Map.MetaInfo.Offset[tempint3];
                        XReflex r = new XReflex();

                        r.chunkcount = oldint;
                        if (r.chunkcount == 0)
                        {
                            continue;
                        }

                        r.chunksize = -1;
                        r.translation = tempint4;
                        r.pointstoTagIndex = tempint3;

                        r.pointstotagtype = meta.Map.MetaInfo.TagType[tempint3];
                        r.pointstotagname = meta.Map.FileNames.Name[tempint3];
                        r.offset = x - 4;
                        r.intag = meta.TagIndex;

                        r.intagtype = meta.type;
                        r.intagname = meta.name;
                        r.mapOffset = meta.Map.MetaInfo.Offset[r.intag] + (x - 4);
                        r.description = "Reflexive";

                        meta.items.Add(r);
                    }

                    oldint = tempint;
                    continue;
                identstuff:

                    // if (tempint < meta.Map.MetaInfo.lowident | tempint > meta.Map.MetaInfo.highident) { continue; }
                    tempindex = meta.Map.Functions.ForMeta.FindMetaByID(tempint);
                    if (tempindex != -1)
                    {
                        Meta.Ident i = new Meta.Ident();
                        i.ident = tempint;
                        i.pointstoTagIndex = tempindex;
                        i.pointstotagtype = meta.Map.MetaInfo.TagType[tempindex];
                        i.pointstotagname = meta.Map.FileNames.Name[tempindex];
                        i.offset = x;
                        if (y == 1)
                        {
                            i.offset += 2;
                        }

                        i.intag = meta.TagIndex;
                        i.intagtype = meta.Map.MetaInfo.TagType[i.intag];
                        i.intagname = meta.Map.FileNames.Name[i.intag];
                        i.description = "Ident";
                        i.mapOffset = meta.Map.MetaInfo.Offset[i.intag] + x;
                        if (y == 1)
                        {
                            i.mapOffset += 2;
                        }

                        meta.items.Add(i);
                    }
                }

                if (y == 1 || meta.Map.HaloVersion != HaloVersionEnum.Halo2)
                {
                    offset += 2;
                    size -= 4;
                    continue;
                }

                BR.BaseStream.Position = offset;
                for (int x = 0; x < size; x += 4)
                {
                    if (x + 4 > size)
                    {
                        continue;
                    }

                    tempshort = BR.ReadInt16();
                    tempbyte2 = BR.ReadByte();
                    tempbyte = BR.ReadByte();
                    if (x < bspstringidminoffset)
                    {
                        continue;
                    }

                    if (tempbyte2 == 0 && tempshort > 0 && tempshort < meta.Map.MapHeader.scriptReferenceCount &&
                        tempbyte == meta.Map.Strings.Length[tempshort])
                    {
                        Meta.String i = new Meta.String();
                        i.offset = x;
                        i.name = meta.Map.Strings.Name[tempshort];
                        i.intag = meta.TagIndex;
                        i.intagtype = meta.Map.MetaInfo.TagType[i.intag];
                        i.intagname = meta.Map.FileNames.Name[i.intag];
                        i.description = "String";
                        i.mapOffset = meta.Map.MetaInfo.Offset[i.intag] + x;

                        meta.items.Add(i);
                    }
                }

                offset += 2;
                size -= 4;
            }

            /********
             * Try to calculate reflex sizes. Mostly works, except for last reflexives and external pointers
            lastReflex = -1;
            for (int x = 0; x < meta.items.Count; x++)
            {
                if (!(meta.items[x] is XReflex)) { continue; }
                XReflex x1 = (XReflex)meta.items[x];
                int nextReflex = -1;
                int nextTranslation = int.MaxValue;
                for (int y = x + 1; y <= meta.items.Count; y++)
                {
                    if (y == meta.items.Count)
                    {
                        if (nextReflex == -1)
                            x1.chunksize = meta.size - x1.translation;
                        continue;
                    }
                    if (meta.items[y] is XReflex)
                    {
                        XReflex x2 = (XReflex)meta.items[y];
                        if (x2.translation > x1.translation && (x2.translation - x1.translation) < nextTranslation)
                        {
                            nextTranslation = x2.translation - x1.translation;
                            nextReflex = y;
                            x1.chunksize = nextTranslation / x1.chunkcount;
                            if (x2.offset - (x1.translation + x1.chunksize * x1.chunkcount) >= 0)
                                x2.parent = x;
                        }
                    }
                }
                if (x1.parent != -1)
                {
                    XReflex x2 = (XReflex)meta.items[x1.parent];
                    if (x1.offset > (x2.translation + x2.chunksize * x2.chunkcount))
                        x1.chunksize = (x2.translation + x2.chunksize);
                }
            }
            */
        }

        // Clears all meta data & Transfers ifp to meta
        /// <summary>
        /// The scan with ifp.
        /// </summary>
        /// <param name="ifp">The ifp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] ScanWithIFP(ref IFPIO ifp)
        {
            string[] errors = new string[0];
            meta.items.Clear();
            if (ifp.items != null)
            {
                if (meta.parsed)
                {
                    meta.size = 0;
                    meta.MS = new MemoryStream();
                    meta.Map.BR.BaseStream.Position = meta.offset;
                    meta.MS.Write(meta.Map.BR.ReadBytes(ifp.headerSize), 0, ifp.headerSize);
                    meta.size += ifp.headerSize;
                }

                if (meta.type == "sbsp")
                {
                    int p = meta.Map.BSP.FindBSPNumberByBSPIdent(meta.ident);
                    errors = CycleElements(
                        ifp.items, meta.offset, 1, meta.TagIndex, 0, meta.Map.BSP.sbsp[p].magic, 0);
                }
                else if (meta.type == "ltmp")
                {
                    int p = meta.Map.BSP.FindBSPNumberByLightMapIdent(meta.ident);
                    errors = CycleElements(
                        ifp.items, meta.offset, 1, meta.TagIndex, 0, meta.Map.BSP.sbsp[p].magic, 0);
                }
                else
                {
                    // not "sbsp" or "ltmp"
                    // Returns any IDENTS, REFLEXIVES & STRINGS for Reference Listing
                    errors = CycleElements(
                        ifp.items, meta.offset, 1, meta.TagIndex, 0, meta.Map.SecondaryMagic, 0);
                }
            }

            return errors;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cycle elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="TagIndex">The TagIndex.</param>
        /// <param name="chunksize">The chunksize.</param>
        /// <param name="magic">The magic.</param>
        /// <param name="previousparsedtranslation">The previousparsedtranslation.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string[] CycleElements(
            object[] elements,
            int offset,
            int count,
            int TagIndex,
            int chunksize,
            int magic,
            int previousparsedtranslation)
        {
            List<string> tempS = new List<string>();

            // elements = all the listings in the .ENT file
            foreach (IFPIO.BaseObject tempbase in elements.Cast<IFPIO.BaseObject>())
            {
                for (int x = 0; x < count; x++)
                {
                    switch (tempbase.ObjectType)
                    {
                        // AKA Structs
                        case IFPIO.ObjectEnum.Struct:
                            IFPIO.Reflexive tempreflex = (IFPIO.Reflexive)tempbase;

                            XReflex r = new XReflex();
                            meta.Map.BR.BaseStream.Position = offset + tempreflex.offset + (x * chunksize);

                            r.mapOffset = (int)meta.Map.BR.BaseStream.Position;

                            // Contains how many Reflexives are listed
                            r.chunkcount = meta.Map.BR.ReadInt32();
                            if (r.chunkcount == 0)
                            {
                                continue;
                            }

                            r.chunksize = tempreflex.chunkSize;
                            int tempshit = meta.Map.BR.ReadInt32();
                            r.translation = tempshit - meta.magic;

                            r.pointstoTagIndex = meta.Map.Functions.ForMeta.FindMetaByOffset(r.translation);
                            if (meta.type == "sbsp" && r.pointstoTagIndex != -1)
                            {
                                // r.translation = tempshit - magic;
                                r.pointstoTagIndex = meta.TagIndex;
                            }

                            if (tempreflex.HasCount == false)
                            {
                                r.translation = r.chunkcount - magic;
                                r.chunkcount = 1;
                                r.pointstoTagIndex = meta.TagIndex;
                            }

                            r.description = tempreflex.name + " [" + x + "]";
                            if (r.pointstoTagIndex == -1)
                            {
                                continue;
                            }

                            if (meta.parsed)
                            {
                                meta.Map.BR.BaseStream.Position = r.translation;
                                r.RealTranslation = r.translation - meta.Map.MetaInfo.Offset[r.pointstoTagIndex];
                                r.RealTagNumber = r.pointstoTagIndex;

                                int messed = 0; // NEVER USED, ONLY INCREMENTED??
                                bool found = false;
                                for (int h = 0; h < meta.items.Count; h++)
                                {
                                    Meta.Item item = meta.items[h];
                                    if (item.type != Meta.ItemType.Reflexive)
                                    {
                                        continue;
                                    }

                                    XReflex rr = (XReflex)meta.items[h];

                                    if (r.RealTranslation == rr.RealTranslation && r.RealTagNumber == rr.RealTagNumber)
                                    {
                                        r.translation = rr.translation;
                                        r.pointstoTagIndex = meta.TagIndex;
                                        r.pointstotagtype = meta.type;
                                        r.pointstotagname = meta.name;
                                        r.offset = previousparsedtranslation + tempreflex.offset + (x * chunksize);
                                        r.intag = meta.TagIndex;
                                        r.intagtype = meta.type;
                                        r.intagname = meta.name;

                                        meta.items.Add(r);
                                        meta.reflexivecount++;
                                        found = true;
                                        break;
                                    }

                                    messed++;
                                }

                                if (found == false)
                                {
                                    r.translation = meta.size;
                                    meta.MS.SetLength(meta.size + (r.chunksize * r.chunkcount));
                                    meta.MS.Write(
                                        meta.Map.BR.ReadBytes(r.chunksize * r.chunkcount), 0, r.chunksize * r.chunkcount);

                                    /************************************************************/
                                    // Shouldn't this just add the number of added chunks??
                                    meta.size += r.chunksize * r.chunkcount;

                                    // if (tempreflex.adding != 0)
                                    // {
                                    // int temppadsize = meta.Map.Functions.Padding(r.chunksize * r.chunkcount, (int)s.Padding);
                                    // meta.MS.SetLength((long)(meta.size + (temppadsize)));
                                    // byte[] tempbytes = new byte[temppadsize];
                                    // meta.MS.Write(tempbytes, 0, temppadsize);
                                    // meta.size += temppadsize;
                                    // }
                                    r.pointstoTagIndex = meta.TagIndex;
                                    r.pointstotagtype = meta.type;
                                    r.pointstotagname = meta.name;
                                    r.offset = previousparsedtranslation + tempreflex.offset + (x * chunksize);
                                    r.intag = meta.TagIndex;
                                    r.intagtype = meta.type;
                                    r.intagname = meta.name;

                                    meta.items.Add(r);
                                    meta.reflexivecount++;

                                    this.CycleElements(
                                        tempreflex.items,
                                        r.RealTranslation + meta.Map.MetaInfo.Offset[r.RealTagNumber],
                                        r.chunkcount,
                                        r.pointstoTagIndex,
                                        tempreflex.chunkSize,
                                        magic,
                                        r.translation);
                                }
                                else
                                {
                                    found = false;
                                }
                            }


                            #region unparsedReflexive
                            else
                            {
                                r.pointstotagtype = meta.Map.MetaInfo.TagType[r.pointstoTagIndex];
                                r.pointstotagname = meta.Map.FileNames.Name[r.pointstoTagIndex];
                                r.translation = r.translation - meta.Map.MetaInfo.Offset[r.pointstoTagIndex];

                                r.offset = offset + tempreflex.offset + (x * chunksize);
                                r.mapOffset = r.offset;
                                r.offset -= meta.Map.MetaInfo.Offset[TagIndex];
                                r.intag = TagIndex;
                                r.intagtype = meta.Map.MetaInfo.TagType[r.intag];
                                r.intagname = meta.Map.FileNames.Name[r.intag];

                                meta.items.Add(r);
                                meta.reflexivecount++;
                                this.CycleElements(
                                    tempreflex.items,
                                    meta.Map.MetaInfo.Offset[r.pointstoTagIndex] + r.translation,
                                    r.chunkcount,
                                    r.pointstoTagIndex,
                                    tempreflex.chunkSize,
                                    magic,
                                    0);
                            }

                            #endregion

                            break;

                        case IFPIO.ObjectEnum.Ident:
                            IFPIO.Ident tempident = (IFPIO.Ident)tempbase;
                            Meta.Ident i = new Meta.Ident();
                            meta.Map.BR.BaseStream.Position = offset + tempident.offset + (x * chunksize);
                            i.mapOffset = (int)meta.Map.BR.BaseStream.Position;
                            i.ident = meta.Map.BR.ReadInt32();
                            i.intag = TagIndex;
                            i.intagtype = meta.Map.MetaInfo.TagType[i.intag];
                            i.intagname = meta.Map.FileNames.Name[i.intag];
                            i.pointstoTagIndex = meta.Map.Functions.ForMeta.FindMetaByID(i.ident);
                            if (i.pointstoTagIndex != -1)
                            {
                                // i.pointstoTagIndex = meta.Map.Functions.Meta.FindMetaByID(i.ident, meta.Map);
                                i.pointstotagtype = meta.Map.MetaInfo.TagType[i.pointstoTagIndex];
                                i.pointstotagname = meta.Map.FileNames.Name[i.pointstoTagIndex];
                            }
                            else
                            {
                                if (i.ident != -1)
                                {
                                    int off = i.mapOffset - meta.Map.MetaInfo.Offset[TagIndex];

                                    /*********************************************************************
                                     * Save errors to be displayed all at once.
                                     *********************************************************************/
                                    tempS.Add(
                                        "Tag: [" + meta.Map.MetaInfo.TagType[TagIndex] + "] " +
                                        meta.Map.FileNames.Name[TagIndex] + "    @ Offset: " + off.ToString().PadLeft(10) +
                                        " ,  Name: \"" + tempident.name + "\"");
                                    i.ident = -1;
                                }

                                i.pointstoTagIndex = -1;
                                i.pointstotagname = "Null";
                            }

                            //i.mapOffset = offset + tempident.offset + (x * chunksize);
                            i.offset = i.mapOffset - meta.Map.MetaInfo.Offset[TagIndex];
                            if (meta.parsed)
                            {
                                i.offset = previousparsedtranslation + tempident.offset + (x * chunksize);
                            }

                            i.description = tempident.name + " [" + x + "]";
                            meta.items.Add(i);
                            break;



                        // If our object is a STRING, read the string info from the file, not just offset values
                        case IFPIO.ObjectEnum.StringID:
                            IFPIO.SID tempstringid = (IFPIO.SID)tempbase;
                            Meta.String si = new Meta.String();
                            meta.Map.BR.BaseStream.Position = offset + tempstringid.offset + (x * chunksize);
                            si.mapOffset = (int)meta.Map.BR.BaseStream.Position;
                            si.id = meta.Map.BR.ReadUInt16();
                            if (si.id == 0 | si.id >= meta.Map.MapHeader.scriptReferenceCount)
                            {
                                continue;
                            }

                            meta.Map.BR.ReadByte();
                            int temp = meta.Map.BR.ReadByte();
                            if (temp != meta.Map.Strings.Length[si.id])
                            {
                                continue;
                            }

                            // Finds offset for each subsection -> Chunksize = Size of whole Subsection
                            si.mapOffset = offset + tempstringid.offset + (x * chunksize);

                            si.offset = si.mapOffset - meta.Map.MetaInfo.Offset[TagIndex];
                            if (meta.parsed)
                            {
                                si.offset = previousparsedtranslation + tempstringid.offset + (x * chunksize);
                            }

                            si.name = meta.Map.Strings.Name[si.id];
                            si.intag = TagIndex;
                            si.intagtype = meta.Map.MetaInfo.TagType[si.intag];
                            si.intagname = meta.Map.FileNames.Name[si.intag];
                            si.description = tempstringid.name + " [" + x + "]";
                            meta.items.Add(si);
                            break;


                    }
                }
            }

            return tempS.ToArray();
        }

        #endregion

        /// <summary>
        /// The x reflex.
        /// </summary>
        /// <remarks></remarks>
        public class XReflex : Meta.Reflexive
        {
            #region Constants and Fields

            /// <summary>
            /// The realTagIndex.
            /// </summary>
            public int RealTagNumber;

            /// <summary>
            /// The realtranslation.
            /// </summary>
            public int RealTranslation;

            #endregion
        }
    }
}