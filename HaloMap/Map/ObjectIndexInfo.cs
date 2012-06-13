// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectIndexInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The object index info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System;
    using System.Collections;
    using System.IO;

    /// <summary>
    /// The object index info.
    /// </summary>
    /// <remarks></remarks>
    public class ObjectIndexInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The ident.
        /// </summary>
        public int[] Ident;

        /// <summary>
        /// The offset.
        /// </summary>
        public int[] Offset;

        /// <summary>
        /// The size.
        /// </summary>
        public int[] Size;

        /// <summary>
        /// The tag type.
        /// </summary>
        public string[] TagType;

        /// <summary>
        /// The tag type 2.
        /// </summary>
        public string[] TagType2;

        /// <summary>
        /// The tag type 3.
        /// </summary>
        public string[] TagType3;

        /// <summary>
        /// The tag types.
        /// </summary>
        public Hashtable TagTypes;

        /// <summary>
        /// The tag types count.
        /// </summary>
        public int TagTypesCount;

        /// <summary>
        /// The bitmapindex.
        /// </summary>
        public int[] bitmapindex;

        /// <summary>
        /// The external.
        /// </summary>
        public bool[] external;

        /// <summary>
        /// The highident.
        /// </summary>
        public int highident;

        /// <summary>
        /// The ident ht.
        /// </summary>
        public Hashtable identHT;

        /// <summary>
        /// The lowident.
        /// </summary>
        public int lowident;

        /// <summary>
        /// The stringoffset.
        /// </summary>
        public int[] stringoffset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIndexInfo"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public ObjectIndexInfo(ref BinaryReader BR, Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                    LoadHalo2ObjectIndexInfo(ref BR, map);
                    break;
                case HaloVersionEnum.Halo2Vista:
                    LoadHalo2ObjectIndexInfo(ref BR, map);
                    break;
                case HaloVersionEnum.HaloCE:
                    LoadHaloCEObjectIndexInfo(ref BR, map);
                    break;
                case HaloVersionEnum.Halo1:
                    LoadHalo1ObjectIndexInfo(ref BR, map);
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load halo 1 object index info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHalo1ObjectIndexInfo(ref BinaryReader BR, Map map)
        {
            TagTypes = new Hashtable();
            identHT = new Hashtable();
            TagType = new string[map.IndexHeader.metaCount];
            TagType2 = new string[map.IndexHeader.metaCount];
            TagType3 = new string[map.IndexHeader.metaCount];
            Ident = new int[map.IndexHeader.metaCount];
            Offset = new int[map.IndexHeader.metaCount];
            Size = new int[map.IndexHeader.metaCount];
            stringoffset = new int[map.IndexHeader.metaCount];
            string[] temptagtypes = new string[500];
            external = new bool[map.IndexHeader.metaCount];
            bitmapindex = new int[map.IndexHeader.metaCount];
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32);
                char[] tempchar = BR.ReadChars(4);

                Array.Reverse(tempchar);

                // BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 4;
                // char[] tempchar2 = BR.ReadChars(4);
                // Array.Reverse(tempchar2);
                // BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 8;
                // char[] tempchar3 = BR.ReadChars(4);
                // Array.Reverse(tempchar3);
                string tempstring = new string(tempchar);

                // string tempstring2 = new string(tempchar2);
                // string tempstring3 = new string(tempchar3);
                object tempobj = TagTypes[tempstring];
                if (tempobj == null)
                {
                    TagTypes.Add(tempstring, tempstring);
                }

                TagType[x] = tempstring;

                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 12;
                Ident[x] = BR.ReadInt32();
                identHT.Add(Ident[x], x);
                if (x == 0)
                {
                    lowident = Ident[x];
                }

                if (Ident[x] > highident)
                {
                    highident = Ident[x];
                }

                if (Ident[x] < lowident)
                {
                    lowident = Ident[x];
                }

                stringoffset[x] = BR.ReadInt32() - map.PrimaryMagic;
                Offset[x] = BR.ReadInt32();

                Offset[x] -= map.PrimaryMagic;
            }

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                int end = map.MapHeader.fileSize;

                for (int xx = 0; xx < map.IndexHeader.metaCount; xx++)
                {
                    if (Offset[xx] < end && Offset[xx] > Offset[x] && external[xx] != true && Offset[xx] > 0)
                    {
                        end = Offset[xx];
                    }
                }

                Size[x] = end - Offset[x];
                if (Size[x] < 0)
                {
                    Size[x] = 0;
                }
            }
        }

        /// <summary>
        /// The load halo 2 object index info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHalo2ObjectIndexInfo(ref BinaryReader BR, Map map)
        {
            TagTypes = new Hashtable();
            identHT = new Hashtable();
            TagType = new string[map.IndexHeader.metaCount];
            Ident = new int[map.IndexHeader.metaCount];
            Offset = new int[map.IndexHeader.metaCount];
            Size = new int[map.IndexHeader.metaCount];
            string[] temptagtypes = new string[500];
            BR.BaseStream.Position = map.IndexHeader.tagsOffset;
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                char[] tempchar = BR.ReadChars(4);
                Array.Reverse(tempchar);
                string tempstring = new string(tempchar);
                object tempobj = TagTypes[tempstring];
                if (tempobj == null)
                {
                    TagTypes.Add(tempstring, tempstring);
                }

                TagType[x] = tempstring;

                Ident[x] = BR.ReadInt32();
                if (Ident[x] == -1)
                {
                    map.IndexHeader.metaCount = x;
                    map.HaloVersion = HaloVersionEnum.Halo2Vista;
                    break;
                }
                identHT.Add(Ident[x], x);
                if (x == 0)
                {
                    lowident = Ident[x];
                }

                if (Ident[x] > highident)
                {
                    highident = Ident[x];
                }

                if (Ident[x] < lowident)
                {
                    lowident = Ident[x];
                }

                Offset[x] = BR.ReadInt32() - map.SecondaryMagic;
                Size[x] = BR.ReadInt32();
            }

            // TagTypes=new string[TagTypesCount];
            // Array.Copy(temptagtypes,0,TagTypes,0,TagTypesCount);
        }

        /// <summary>
        /// The load halo ce object index info.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadHaloCEObjectIndexInfo(ref BinaryReader BR, Map map)
        {
            TagTypes = new Hashtable();
            identHT = new Hashtable();
            TagType = new string[map.IndexHeader.metaCount];
            TagType2 = new string[map.IndexHeader.metaCount];
            TagType3 = new string[map.IndexHeader.metaCount];
            Ident = new int[map.IndexHeader.metaCount];
            Offset = new int[map.IndexHeader.metaCount];
            Size = new int[map.IndexHeader.metaCount];
            stringoffset = new int[map.IndexHeader.metaCount];
            string[] temptagtypes = new string[500];
            external = new bool[map.IndexHeader.metaCount];
            bitmapindex = new int[map.IndexHeader.metaCount];
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32);
                char[] tempchar = BR.ReadChars(4);
                Array.Reverse(tempchar);
                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 4;
                char[] tempchar2 = BR.ReadChars(4);
                Array.Reverse(tempchar2);
                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 8;
                char[] tempchar3 = BR.ReadChars(4);
                Array.Reverse(tempchar3);
                string tempstring = new string(tempchar);
                string tempstring2 = new string(tempchar2);
                string tempstring3 = new string(tempchar3);

                object tempobj = TagTypes[tempstring];
                if (tempobj == null)
                {
                    TagTypes.Add(tempstring, tempstring);
                }

                TagType[x] = tempstring;

                BR.BaseStream.Position = map.IndexHeader.tagsOffset + (x * 32) + 12;
                Ident[x] = BR.ReadInt32();
                identHT.Add(Ident[x], x);
                if (x == 0)
                {
                    lowident = Ident[x];
                }

                if (Ident[x] > highident)
                {
                    highident = Ident[x];
                }

                if (Ident[x] < lowident)
                {
                    lowident = Ident[x];
                }

                stringoffset[x] = BR.ReadInt32() - map.PrimaryMagic;
                Offset[x] = BR.ReadInt32();

                if (TagType[x] == "bitm" && map.BitmapLibary.error == false)
                {
                    if (Offset[x] < map.PrimaryMagic)
                    {
                        int tempindex = Offset[x];
                        bitmapindex[x] = tempindex;
                        Size[x] = map.BitmapLibary.RawSize[tempindex];
                        Offset[x] = map.BitmapLibary.RawOffset[tempindex];
                        external[x] = true;
                    }
                    else
                    {
                        Offset[x] -= map.PrimaryMagic;
                    }
                }
                else if (TagType[x] == "bitm" && map.BitmapLibary.error)
                {
                    if (Offset[x] < map.PrimaryMagic)
                    {
                        Size[x] = 0;
                        Offset[x] = 0;
                    }
                    else
                    {
                        Offset[x] -= map.PrimaryMagic;
                    }
                }
                else
                {
                    Offset[x] -= map.PrimaryMagic;
                }
            }

            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                int end = map.MapHeader.fileSize;
                if (external[x] != true)
                {
                    for (int xx = 0; xx < map.IndexHeader.metaCount; xx++)
                    {
                        if (Offset[xx] < end && Offset[xx] > Offset[x] && external[xx] != true && Offset[xx] > 0)
                        {
                            end = Offset[xx];
                        }
                    }

                    Size[x] = end - Offset[x];
                    if (Size[x] < 0)
                    {
                        Size[x] = 0;
                    }
                }
            }
        }

        #endregion
    }
}