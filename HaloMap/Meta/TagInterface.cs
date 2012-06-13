// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagInterface.cs" company="">
//   
// </copyright>
// <summary>
//   The tag interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Meta
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    using HaloMap.Map;

    /// <summary>
    /// The tag interface.
    /// </summary>
    /// <remarks></remarks>
    public class TagInterface
    {
        #region Constants and Fields

        /// <summary>
        /// The bsp number.
        /// </summary>
        private static int BSPNumber = -1;

        /// <summary>
        /// The ident.
        /// </summary>
        private static int Ident;

        /// <summary>
        /// The magic.
        /// </summary>
        private static int Magic;

        /// <summary>
        /// The map.
        /// </summary>
        private static Map Map;

        /// <summary>
        /// The offset.
        /// </summary>
        private static int Offset;

        /// <summary>
        /// The tag name.
        /// </summary>
        private static string TagName;

        /// <summary>
        /// The tag number.
        /// </summary>
        private static int TagNumber;

        /// <summary>
        /// The tag type.
        /// </summary>
        private static string TagType;

        /// <summary>
        /// The header.
        /// </summary>
        private TagBlock header;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TagInterface"/> class.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public TagInterface(int tagIndex, Map map)
        {
            Header = new TagBlock("Header", null, map.HaloVersion);
            SeekInMeta(0);
            Map = map;
            TagNumber = tagIndex;
            Ident = map.MetaInfo.Ident[TagNumber];
            TagType = map.MetaInfo.TagType[TagNumber];
            TagName = map.FileNames.Name[TagNumber];
            Offset = map.MetaInfo.Offset[TagNumber];
            if (TagType == "sbsp")
            {
                BSPNumber = map.BSP.FindBSPNumberByBSPIdent(Ident);
                Magic = map.BSP.sbsp[BSPNumber].magic;
            }
            else
            {
                switch (map.HaloVersion)
                {
                    case HaloVersionEnum.Halo2:
                    case HaloVersionEnum.Halo2Vista:
                        Magic = map.SecondaryMagic;
                        break;
                    case HaloVersionEnum.Halo1:
                    case HaloVersionEnum.HaloCE:
                        Magic = map.PrimaryMagic;
                        break;
                }
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// The meta object type.
        /// </summary>
        /// <remarks></remarks>
        public enum MetaObjectType
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Header.
        /// </summary>
        /// <value>The header.</value>
        /// <remarks></remarks>
        public TagBlock Header
        {
            get
            {
                return header;
            }

            set
            {
                header = value;
                value.ChunkCount = 1;
                value.MapOffset = Offset;

                // value.Translation.MapTranslation = Offset;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The seek in map.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <remarks></remarks>
        public void SeekInMap(int offset)
        {
            Map.BR.BaseStream.Position = offset;
        }

        /// <summary>
        /// The seek in meta.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <remarks></remarks>
        public void SeekInMeta(int offset)
        {
            Map.BR.BaseStream.Position = Offset + offset;
        }

        #endregion

        /// <summary>
        /// The angle.
        /// </summary>
        /// <remarks></remarks>
        public class Angle : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public float Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Angle"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public Angle(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The convert from radian.
            /// </summary>
            /// <param name="angle">The angle.</param>
            /// <returns>The convert from radian.</returns>
            /// <remarks></remarks>
            public float ConvertFromRadian(float angle)
            {
                return (float)((angle * 180.0) / Math.PI);
            }

            /// <summary>
            /// The convert to radian.
            /// </summary>
            /// <param name="angle">The angle.</param>
            /// <returns>The convert to radian.</returns>
            /// <remarks></remarks>
            public float ConvertToRadian(float angle)
            {
                double dbl;
                if (angle != 0F)
                {
                    dbl = 0.017453292519943295 * (angle);
                    return (float)(dbl);
                }
                else
                {
                    return 0F;
                }

                //return (float)(dbl);
            }

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadSingle();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(ConvertToRadian(Value));
            }

            #endregion
        }

        /// <summary>
        /// The base object.
        /// </summary>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The map offset.
            /// </summary>
            internal int mapOffset;

            /// <summary>
            /// The meta offset.
            /// </summary>
            internal int metaOffset;

            /// <summary>
            /// The name.
            /// </summary>
            internal string name;

            /// <summary>
            /// The offset.
            /// </summary>
            internal int offset;

            /// <summary>
            /// The parent.
            /// </summary>
            internal TagBlock parent;

            /// <summary>
            /// The size.
            /// </summary>
            internal int size;

            #endregion

            // public int ParentIndex = -1;
            // public int SiblingIndex = -1;
            // public int ChildIndex = -1;
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BaseObject"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public BaseObject(string name)
            {
                if (name != null)
                {
                    this.name = name;
                }
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets MapOffset.
            /// </summary>
            /// <value>The map offset.</value>
            /// <remarks></remarks>
            [Category("Base Properties")]
            public int MapOffset
            {
                get
                {
                    return mapOffset;
                }

                set
                {
                    mapOffset = value;
                }
            }

            /// <summary>
            /// Gets MetaOffset.
            /// </summary>
            /// <remarks></remarks>
            [Category("Base Properties")]
            public int MetaOffset
            {
                get
                {
                    return metaOffset;
                }
 // set { metaOffset = value; }
            }

            /// <summary>
            /// Gets Name.
            /// </summary>
            /// <remarks></remarks>
            [Category("Base Properties")]
            public string Name
            {
                get
                {
                    return name;
                }
 // set { name = value; }
            }

            /// <summary>
            /// Gets Offset.
            /// </summary>
            /// <remarks></remarks>
            [Category("Base Properties")]
            public int Offset
            {
                get
                {
                    return offset;
                }
 // set { offset = value; }
            }

            /// <summary>
            /// Gets Parent.
            /// </summary>
            /// <remarks></remarks>
            [Category("Base Properties")]
            [TypeConverter(typeof(BaseObjectConverter))]
            public TagBlock Parent
            {
                get
                {
                    return parent;
                }
            }

            /// <summary>
            /// Gets Size.
            /// </summary>
            /// <remarks></remarks>
            [Category("Base Properties")]
            public int Size
            {
                get
                {
                    return size;
                }
 // set { size = value; }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public virtual void Read()
            {
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public virtual void Write()
            {
            }

            #endregion
        }

        /// <summary>
        /// The base object converter.
        /// </summary>
        /// <remarks></remarks>
        public class BaseObjectConverter : ExpandableObjectConverter
        {
            #region Public Methods

            /// <summary>
            /// The convert to.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="culture">The culture.</param>
            /// <param name="value">The value.</param>
            /// <param name="destType">The dest type.</param>
            /// <returns>The convert to.</returns>
            /// <remarks></remarks>
            public override object ConvertTo(
                ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
            {
                if (destType == typeof(string))
                {
                    if (value is Pointer)
                    {
                        Pointer emp = (Pointer)value;
                        return "Offsets";
                    }
                    else if (value is TagBlockLayout)
                    {
                        TagBlockLayout tag = (TagBlockLayout)value;
                        return tag.Parent.Name;
                    }
                    else if (value is TagBlock)
                    {
                        TagBlock tag = (TagBlock)value;
                        return tag.Name;
                    }
                }

                return string.Empty;
            }

            #endregion
        }

        /// <summary>
        /// The bit mask flags.
        /// </summary>
        /// <remarks></remarks>
        public class BitMaskFlags : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The bit count.
            /// </summary>
            public int BitCount;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BitMaskFlags"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public BitMaskFlags(string name)
                : base(name)
            {
            }

            #endregion
        }

        /// <summary>
        /// The byte flags.
        /// </summary>
        /// <remarks></remarks>
        public class ByteFlags : BitMaskFlags
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public byte Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ByteFlags"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public ByteFlags(string name)
                : base(name)
            {
                this.BitCount = 8;
                this.size = 1;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadByte();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The byte integer.
        /// </summary>
        /// <remarks></remarks>
        public class ByteInteger : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public short Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ByteInteger"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public ByteInteger(string name)
                : base(name)
            {
                this.size = 1;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadByte();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The chunk collection.
        /// </summary>
        /// <remarks></remarks>
        public class ChunkCollection : List<TagBlockLayout>
        {
            #region Indexers

            /// <summary>
            /// The this.
            /// </summary>
            /// <returns>
            /// The element at the specified index.
            ///   </returns>
            ///   
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///   <paramref name="index"/> is less than 0.
            /// -or-
            ///   <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.
            ///   </exception>
            /// <remarks></remarks>
            public new TagBlockLayout this[int indexer]
            {
                get
                {
                    return base[indexer];
                }

                set
                {
                    base[indexer] = value;
                }
            }

            #endregion
        }

        /// <summary>
        /// The enumerator.
        /// </summary>
        /// <remarks></remarks>
        public class Enumerator : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public short Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public Enumerator(string name)
                : base(name)
            {
                this.size = 2;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadInt16();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The h 1 tag reference.
        /// </summary>
        /// <remarks></remarks>
        public class H1TagReference : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The ident.
            /// </summary>
            public TagReference Ident;

            /// <summary>
            /// The string translation.
            /// </summary>
            public Pointer StringTranslation;

            /// <summary>
            /// The tag type.
            /// </summary>
            public HaloTagType TagType;

            /// <summary>
            /// The zero.
            /// </summary>
            public int Zero;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="H1TagReference"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public H1TagReference(string name)
                : base(name)
            {
                TagType = new HaloTagType(name);
                StringTranslation = new Pointer(name);
                this.size = 16;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                TagType.Read();
                StringTranslation.Read();
                Zero = Map.BR.ReadInt32();
                Ident.Read();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                TagType.Write();
                StringTranslation.Write();
                Map.BW.Write(Zero);
                Ident.Write();
            }

            #endregion
        }

        /// <summary>
        /// The halo tag type.
        /// </summary>
        /// <remarks></remarks>
        public class HaloTagType : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The tag type.
            /// </summary>
            public string TagType;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="HaloTagType"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public HaloTagType(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                char[] temp = Map.BR.ReadChars(4);
                Array.Reverse(temp);
                TagType = new string(temp);
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                char[] temp = TagType.ToCharArray();
                Array.Reverse(temp);
                Map.BW.Write(temp);
            }

            #endregion
        }

        /// <summary>
        /// The long integer.
        /// </summary>
        /// <remarks></remarks>
        public class LongInteger : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public int Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LongInteger"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public LongInteger(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets Long.
            /// </summary>
            /// <value>The long.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public int Long
            {
                get
                {
                    return Value;
                }

                set
                {
                    Value = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadInt32();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The padding x.
        /// </summary>
        /// <remarks></remarks>
        public class PaddingX : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PaddingX"/> class.
            /// </summary>
            /// <param name="size">The size.</param>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public PaddingX(int size, string name)
                : base(name)
            {
                this.size = size;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Map.BR.BaseStream.Position += this.Size;
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                byte zero = 0;
                for (int x = 0; x < this.Size; x++)
                {
                    Map.BW.Write(zero);
                }
            }

            #endregion
        }

        /// <summary>
        /// The pointer.
        /// </summary>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Pointer : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The map translation.
            /// </summary>
            private int mapTranslation;

            /// <summary>
            /// The meta translation.
            /// </summary>
            private int metaTranslation;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Pointer"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public Pointer(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets MapTranslation.
            /// </summary>
            /// <remarks></remarks>
            public int MapTranslation
            {
                get
                {
                    return mapTranslation;
                }
            }

            /// <summary>
            /// Gets MetaTranslation.
            /// </summary>
            /// <remarks></remarks>
            public int MetaTranslation
            {
                get
                {
                    return metaTranslation;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                mapTranslation = Map.BR.ReadInt32() - Magic;
                metaTranslation = MapTranslation - TagInterface.Offset;
            }

            // public void Write()
            // {
            // int temp = metaoffset + MetaTranslation + Magic;
            // map.BW.Write(temp);
            // }
            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                int temp = MapTranslation + Magic;
                Map.BW.Write(temp);
            }

            #endregion
        }

        /// <summary>
        /// The real argb color.
        /// </summary>
        /// <remarks></remarks>
        public class RealARGBColor : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The a.
            /// </summary>
            public float a;

            /// <summary>
            /// The b.
            /// </summary>
            public float b;

            /// <summary>
            /// The g.
            /// </summary>
            public float g;

            /// <summary>
            /// The r.
            /// </summary>
            public float r;

            /// <summary>
            /// The color.
            /// </summary>
            private Color color;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealARGBColor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealARGBColor(string name)
                : base(name)
            {
                this.size = 16;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets A.
            /// </summary>
            /// <value>The A.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float A
            {
                get
                {
                    return a;
                }

                set
                {
                    UpdateColor();
                    a = value;
                }
            }

            /// <summary>
            /// Gets or sets B.
            /// </summary>
            /// <value>The B.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float B
            {
                get
                {
                    return b;
                }

                set
                {
                    UpdateColor();
                    b = value;
                }
            }

            /// <summary>
            /// Gets or sets Color.
            /// </summary>
            /// <value>The color.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public Color Color
            {
                get
                {
                    return color;
                }

                set
                {
                    ConvertColor();
                    color = value;
                }
            }

            /// <summary>
            /// Gets or sets G.
            /// </summary>
            /// <value>The G.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float G
            {
                get
                {
                    return g;
                }

                set
                {
                    UpdateColor();
                    g = value;
                }
            }

            /// <summary>
            /// Gets or sets R.
            /// </summary>
            /// <value>The R.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float R
            {
                get
                {
                    return r;
                }

                set
                {
                    UpdateColor();
                    r = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                a = Map.BR.ReadSingle();
                r = Map.BR.ReadSingle();
                g = Map.BR.ReadSingle();
                b = Map.BR.ReadSingle();
                UpdateColor();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(a);
                Map.BW.Write(r);
                Map.BW.Write(g);
                Map.BW.Write(b);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The convert color.
            /// </summary>
            /// <remarks></remarks>
            private void ConvertColor()
            {
                a = color.A / (float)255;
                r = color.R / (float)255;
                g = color.G / (float)255;
                b = color.B / (float)255;
            }

            /// <summary>
            /// The update color.
            /// </summary>
            /// <remarks></remarks>
            private void UpdateColor()
            {
                if (r > 1)
                {
                    r = 1;
                }

                if (g > 1)
                {
                    g = 1;
                }

                if (b > 1)
                {
                    b = 1;
                }

                if (a > 1)
                {
                    a = 1;
                }

                if (r < 0)
                {
                    r = 0;
                }

                if (g < 0)
                {
                    g = 0;
                }

                if (b < 0)
                {
                    b = 0;
                }

                if (a < 0)
                {
                    b = 0;
                }

                color = Color.FromArgb((int)(a * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));
            }

            #endregion
        }

        /// <summary>
        /// The real euler angle 3 d.
        /// </summary>
        /// <remarks></remarks>
        public class RealEulerAngle3D : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The p.
            /// </summary>
            public Angle p;

            /// <summary>
            /// The r.
            /// </summary>
            public Angle r;

            /// <summary>
            /// The y.
            /// </summary>
            public Angle y;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealEulerAngle3D"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealEulerAngle3D(string name)
                : base(name)
            {
                y = new Angle(name);
                p = new Angle(name);
                r = new Angle(name);
                this.size = 12;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                y.Read();
                p.Read();
                r.Read();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                y.Write();
                p.Write();
                r.Write();
            }

            #endregion
        }

        /// <summary>
        /// The real float.
        /// </summary>
        /// <remarks></remarks>
        public class RealFloat : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public float Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealFloat"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealFloat(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets Float.
            /// </summary>
            /// <value>The float.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float Float
            {
                get
                {
                    return Value;
                }

                set
                {
                    Value = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadSingle();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The real plane 3 d.
        /// </summary>
        /// <remarks></remarks>
        public class RealPlane3D : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The d.
            /// </summary>
            private float d;

            /// <summary>
            /// The i.
            /// </summary>
            private float i;

            /// <summary>
            /// The j.
            /// </summary>
            private float j;

            /// <summary>
            /// The k.
            /// </summary>
            private float k;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealPlane3D"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealPlane3D(string name)
                : base(name)
            {
                this.size = 16;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets D.
            /// </summary>
            /// <value>The D.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float D
            {
                get
                {
                    return d;
                }

                set
                {
                    d = value;
                }
            }

            /// <summary>
            /// Gets or sets I.
            /// </summary>
            /// <value>The I.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float I
            {
                get
                {
                    return i;
                }

                set
                {
                    i = value;
                }
            }

            /// <summary>
            /// Gets or sets J.
            /// </summary>
            /// <value>The J.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float J
            {
                get
                {
                    return j;
                }

                set
                {
                    j = value;
                }
            }

            /// <summary>
            /// Gets or sets K.
            /// </summary>
            /// <value>The K.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float K
            {
                get
                {
                    return k;
                }

                set
                {
                    k = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                i = Map.BR.ReadSingle();
                j = Map.BR.ReadSingle();
                k = Map.BR.ReadSingle();
                d = Map.BR.ReadSingle();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(i);
                Map.BW.Write(j);
                Map.BW.Write(k);
                Map.BW.Write(d);
            }

            #endregion
        }

        /// <summary>
        /// The real point 3 d.
        /// </summary>
        /// <remarks></remarks>
        public class RealPoint3D : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The x.
            /// </summary>
            public float x;

            /// <summary>
            /// The y.
            /// </summary>
            public float y;

            /// <summary>
            /// The z.
            /// </summary>
            public float z;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealPoint3D"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealPoint3D(string name)
                : base(name)
            {
                this.size = 12;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                x = Map.BR.ReadSingle();
                y = Map.BR.ReadSingle();
                z = Map.BR.ReadSingle();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(x);
                Map.BW.Write(y);
                Map.BW.Write(z);
            }

            #endregion
        }

        /// <summary>
        /// The real rgb color.
        /// </summary>
        /// <remarks></remarks>
        public class RealRGBColor : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The b.
            /// </summary>
            private float b;

            /// <summary>
            /// The color.
            /// </summary>
            private Color color;

            /// <summary>
            /// The g.
            /// </summary>
            private float g;

            /// <summary>
            /// The r.
            /// </summary>
            private float r;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RealRGBColor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public RealRGBColor(string name)
                : base(name)
            {
                this.size = 12;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets B.
            /// </summary>
            /// <value>The B.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float B
            {
                get
                {
                    return b;
                }

                set
                {
                    UpdateColor();
                    b = value;
                }
            }

            /// <summary>
            /// Gets or sets Color.
            /// </summary>
            /// <value>The color.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public Color Color
            {
                get
                {
                    return color;
                }

                set
                {
                    ConvertColor();
                    color = value;
                }
            }

            /// <summary>
            /// Gets or sets G.
            /// </summary>
            /// <value>The G.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float G
            {
                get
                {
                    return g;
                }

                set
                {
                    UpdateColor();
                    g = value;
                }
            }

            /// <summary>
            /// Gets or sets R.
            /// </summary>
            /// <value>The R.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public float R
            {
                get
                {
                    return r;
                }

                set
                {
                    UpdateColor();
                    r = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                r = Map.BR.ReadSingle();
                g = Map.BR.ReadSingle();
                b = Map.BR.ReadSingle();
                UpdateColor();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(r);
                Map.BW.Write(g);
                Map.BW.Write(b);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The convert color.
            /// </summary>
            /// <remarks></remarks>
            private void ConvertColor()
            {
                r = color.R / (float)255;
                g = color.G / (float)255;
                b = color.B / (float)255;
            }

            /// <summary>
            /// The update color.
            /// </summary>
            /// <remarks></remarks>
            private void UpdateColor()
            {
                if (r > 1)
                {
                    r = 1;
                }

                if (g > 1)
                {
                    g = 1;
                }

                if (b > 1)
                {
                    b = 1;
                }

                if (r < 0)
                {
                    r = 0;
                }

                if (g < 0)
                {
                    g = 0;
                }

                if (b < 0)
                {
                    b = 0;
                }

                color = Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
            }

            #endregion
        }

        /// <summary>
        /// The short integer.
        /// </summary>
        /// <remarks></remarks>
        public class ShortInteger : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public short Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ShortInteger"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public ShortInteger(string name)
                : base(name)
            {
                this.size = 2;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets Short.
            /// </summary>
            /// <value>The short.</value>
            /// <remarks></remarks>
            [Category("Values")]
            public short Short
            {
                get
                {
                    return Value;
                }

                set
                {
                    Value = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadInt16();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }

        /// <summary>
        /// The tag block.
        /// </summary>
        /// <remarks></remarks>
        [DefaultPropertyAttribute("Name")]
        public class TagBlock : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The halo version.
            /// </summary>
            private readonly HaloVersionEnum HaloVersion;

            /// <summary>
            /// The layout.
            /// </summary>
            private readonly TagBlockLayout Layout = new TagBlockLayout();

            /// <summary>
            /// The chunks.
            /// </summary>
            private readonly ChunkCollection chunks = new ChunkCollection();

            /// <summary>
            /// The translation.
            /// </summary>
            private readonly Pointer translation = new Pointer("Translation");

            /// <summary>
            /// The chunk count.
            /// </summary>
            private int chunkCount;

            /// <summary>
            /// The chunknumber.
            /// </summary>
            private int chunknumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TagBlock"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="tagblocklayout">The tagblocklayout.</param>
            /// <param name="haloversion">The haloversion.</param>
            /// <remarks></remarks>
            public TagBlock(string name, TagBlockLayout tagblocklayout, HaloVersionEnum haloversion)
                : base(name)
            {
                if (tagblocklayout != null)
                {
                    Layout = tagblocklayout;
                }

                Layout.Parent = this;
                this.size = 8;
                if (haloversion != HaloVersionEnum.Halo2)
                {
                    this.size += 4;
                }

                HaloVersion = haloversion;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets ChunkCount.
            /// </summary>
            /// <value>The chunk count.</value>
            /// <remarks></remarks>
            [Category("Chunk Info")]
            public int ChunkCount
            {
                get
                {
                    return chunkCount;
                }

                set
                {
                    chunkCount = value;
                }
            }

            /// <summary>
            /// Gets ChunkInfo.
            /// </summary>
            /// <remarks></remarks>
            [Category("Chunk Info")]
            [TypeConverter(typeof(BaseObjectConverter))]
            public TagBlockLayout ChunkInfo
            {
                get
                {
                    return chunks[chunknumber];
                }
            }

            /// <summary>
            /// Gets or sets ChunkNumber.
            /// </summary>
            /// <value>The chunk number.</value>
            /// <remarks></remarks>
            [Category("Chunk Info")]
            public int ChunkNumber
            {
                get
                {
                    return chunknumber;
                }

                set
                {
                    chunknumber = value;
                }
            }

            /// <summary>
            /// Gets Translation.
            /// </summary>
            /// <remarks></remarks>
            [Category("Chunk Info")]
            [TypeConverter(typeof(BaseObjectConverter))]
            public Pointer Translation
            {
                get
                {
                    return translation;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                chunkCount = Map.BR.ReadInt32();
                translation.Read();
                if (HaloVersion != HaloVersionEnum.Halo2 || 
                    HaloVersion != HaloVersionEnum.Halo2Vista)
                {
                    Map.BR.ReadInt32();
                }
            }

            /// <summary>
            /// The read elements.
            /// </summary>
            /// <remarks></remarks>
            public void ReadElements()
            {
                for (int x = 0; x < ChunkCount; x++)
                {
                    chunks.Add(new TagBlockLayout());

                    // elements[x] = new TagBlockLayout();
                    chunks[x] = Layout;
                    chunks[x].Parent = this;
                }

                Map.BR.BaseStream.Position = TagInterface.Offset + Translation.MetaTranslation;
                for (int x = 0; x < ChunkCount; x++)
                {
                    chunks[x].ReadFields();
                }
            }

            #endregion
        }

        /// <summary>
        /// The tag block layout.
        /// </summary>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class TagBlockLayout : List<BaseObject>
        {
            #region Constants and Fields

            /// <summary>
            /// The chunk size.
            /// </summary>
            public int ChunkSize;

            /// <summary>
            /// The parent.
            /// </summary>
            public TagBlock Parent;

            #endregion

            #region Indexers

            /// <summary>
            /// The this.
            /// </summary>
            /// <returns>
            /// The element at the specified index.
            ///   </returns>
            ///   
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///   <paramref name="index"/> is less than 0.
            /// -or-
            ///   <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.
            ///   </exception>
            /// <remarks></remarks>
            public new BaseObject this[int indexer]
            {
                get
                {
                    return base[indexer];
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The add.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <remarks></remarks>
            public new void Add(BaseObject obj)
            {
                obj.offset = this.ChunkSize;
                obj.parent = Parent;
                base.Add(obj);
                this.ChunkSize += obj.Size;
            }

            /// <summary>
            /// The read fields.
            /// </summary>
            /// <remarks></remarks>
            public void ReadFields()
            {
                foreach (BaseObject temp in this)
                {
                    temp.Read();
                    temp.parent = this.Parent;
                }

                foreach (BaseObject temp in this)
                {
                    if (temp is TagBlock)
                    {
                        ((TagBlock)temp).ReadElements();
                    }
                }
            }

            /// <summary>
            /// The remove at.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///   <paramref name="index"/> is less than 0.
            /// -or-
            ///   <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.
            ///   </exception>
            /// <remarks></remarks>
            public new void RemoveAt(int index)
            {
                this.ChunkSize -= base[index].Size;
                base.RemoveAt(index);
            }

            #endregion
        }

        /// <summary>
        /// The tag reference.
        /// </summary>
        /// <remarks></remarks>
        public class TagReference : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The ident.
            /// </summary>
            public int Ident;

            /// <summary>
            /// The tag name.
            /// </summary>
            public string TagName;

            /// <summary>
            /// The tag number.
            /// </summary>
            public int TagNumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TagReference"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public TagReference(string name)
                : base(name)
            {
                this.size = 4;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Ident = Map.BR.ReadInt32();
                TagNumber = Map.Functions.ForMeta.FindMetaByID(Ident);
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Ident);
            }

            #endregion
        }

        /// <summary>
        /// The word flags.
        /// </summary>
        /// <remarks></remarks>
        public class WordFlags : BitMaskFlags
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public short Value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WordFlags"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public WordFlags(string name)
                : base(name)
            {
                this.BitCount = 16;
                this.size = 2;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The read.
            /// </summary>
            /// <remarks></remarks>
            public override void Read()
            {
                Value = Map.BR.ReadInt16();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <remarks></remarks>
            public override void Write()
            {
                Map.BW.Write(Value);
            }

            #endregion
        }
    }
}