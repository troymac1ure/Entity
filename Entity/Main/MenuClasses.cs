using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using HaloMap.Meta;
using HaloMap.Map;

namespace entity.Main.MenuClasses
{
    public class stringID
    {
        public short sidIndexer { get; set; }
        public byte unused { get; set; }
        public byte length { get; set; }
        
        public stringID()
        {
        }

        public void Read(BinaryReader br)
        {
            this.sidIndexer = br.ReadInt16();
            this.unused = br.ReadByte();
            this.length = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.sidIndexer);
            bw.Write(this.unused);
            bw.Write(this.length);
        }
    }

    public class H2Color
    {
        public float A;
        public float R;
        public float G;
        public float B;

        public static H2Color fromARGB(float alpha, float red, float green, float blue)
        {
            H2Color h2c = new H2Color();
            h2c.A = alpha;
            h2c.R = red;
            h2c.G = green;
            h2c.B = blue;
            return h2c;
        }
    }

    public class baseData
    {
        public string title;
        public int offset;
        public Meta meta;
        public object link;
        public bool isPopulated = false;
        public int order;

        /// <summary>
        /// Creates a new baseData object
        /// </summary>
        /// <param name="Order">The original order placement (used for "unsorting")</param>
        public baseData(int Order)
        {
            this.order = Order;
            this.title = string.Empty;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return title;
        }
    }

    public class bitmapData : baseData
    {
        public bool ignoreForMenuSpacing;
        public bool useSubMaps;
        public bool pulsingText;
        public bool autoTypingText;
        public short animationIndex;
        public short introAnimationDelay;
        public short blendMethod;
        public short bitmNumber;
        public short left;
        public short top;
        public float horizontalWrapsPerSec;
        public float verticalWrapsPerSec;
        public char[] bitmTag;
        public int bitmIdent;
        public short renderDepth;
        private short unused;
        public float spriteAnimationFPS;
        public short progressLeft;
        public short progressBottom;
        public stringID stringID = new stringID();
        public short progressScaleX;
        public short progressScaleY;

        public bitmapData(int order)
            : base(order)
        {
        }

        public override object Clone()
        {            
            return (bitmapData) base.Clone();
        }

        public void Read(BinaryReader br)
        {
            br.BaseStream.Position = offset;
            int flags = br.ReadInt32();
            ignoreForMenuSpacing = ((flags & 1) != 0); // bit 0 // Used to ignore menu item spacing (overlap bitmaps)
            useSubMaps = ((flags & 2) != 0); // bit 1           // Used on [skin] tag
            pulsingText = ((flags & 4) != 0); // bit 2          // Also "Hide Bitmap" on some tags (eg. [skin])
            autoTypingText = ((flags & 8) != 0); // bit 3       // used on [wgit] tag 

            animationIndex = br.ReadInt16();
            introAnimationDelay = br.ReadInt16();
            blendMethod = br.ReadInt16();
            bitmNumber = br.ReadInt16();
            left = br.ReadInt16();
            top = br.ReadInt16();
            horizontalWrapsPerSec = br.ReadSingle();
            verticalWrapsPerSec = br.ReadSingle();
            bitmTag = br.ReadChars(4);
            bitmIdent = br.ReadInt32();
            renderDepth = br.ReadInt16();
            unused = br.ReadInt16();
            spriteAnimationFPS = br.ReadSingle();
            progressLeft = br.ReadInt16();
            progressBottom = br.ReadInt16();
            stringID.Read(br);
            progressScaleX = br.ReadInt16();
            progressScaleY = br.ReadInt16();
            isPopulated = true;
        }

        public void Write(BinaryWriter bw)
        {
            bw.BaseStream.Position = offset;

            int flags = int.MaxValue - 8 - 4 - 2 - 1;
            flags += flags | (ignoreForMenuSpacing ? 1 : 0); // bit 0
            flags += flags | (useSubMaps ? 2 : 0); // bit 1
            flags += flags | (pulsingText ? 4 : 0); // bit 2
            flags += flags | (autoTypingText ? 8 : 0); // bit 3
            bw.Write(flags);

            bw.Write(animationIndex);
            bw.Write(introAnimationDelay);
            bw.Write(blendMethod);
            bw.Write(bitmNumber);
            bw.Write(left);
            bw.Write(top);
            bw.Write(horizontalWrapsPerSec);
            bw.Write(verticalWrapsPerSec);

            if (bitmIdent == 0)  // <null>
            {
                bw.Write((int)-1);  // Tag 
                bw.Write((int)-1);  // Ident
            }
            else
            {
                char[] bitmTag = "mtib".ToCharArray();
                bw.Write(bitmTag);
                bw.Write(bitmIdent);
            }
            bw.Write(renderDepth);
            bw.Write(unused);
            bw.Write(spriteAnimationFPS);
            bw.Write(progressLeft);
            bw.Write(progressBottom);
            stringID.Write(bw);
            bw.Write(progressScaleX);
            bw.Write(progressScaleY);
        }
    }

    public class bitmapInfo
    {
        public short BitmapIndex;
        public short BitmapIndexMax;
        public Bitmap Image;
    }

    public class listBlockData : baseData
    {
        public bool buttonsLoop;
        public short skinIndex;
        public short visibleItemCount;
        public short left;
        public short bottom;
        public short animationIndex;
        public short introAnimationDelay;

        public listBlockData(int Order)
            : base(Order)
        {
        }

        public void Read(BinaryReader br)
        {
            int flags = br.ReadInt32();
            buttonsLoop = ((flags & 1) != 0); // bit 0

            skinIndex = br.ReadInt16();
            visibleItemCount = br.ReadInt16();
            left = br.ReadInt16();
            bottom = br.ReadInt16();
            animationIndex = br.ReadInt16();
            introAnimationDelay = br.ReadInt16();
        }
    }

    public class paneData : baseData
    {
        private short unused;
        public short animationIndex;
        public List<bitmapData> bitmaps = new List<bitmapData>();
        public List<listBlockData> listBlocks = new List<listBlockData>();
        public List<textBlockData> textBlocks = new List<textBlockData>();

        public paneData(int order)
            : base(order)
        {
        }

        public void Read(BinaryReader br)
        {
            unused = br.ReadInt16();
            animationIndex = br.ReadInt16();
            // ...
        }
    }

    public class screenData : baseData
    {
        public List<string> strings = new List<string>();

        public bool flag0;
        public bool flag1;
        public bool flag2;
        public bool flag3;
        public bool flag4;
        public bool flag5;
        public bool flag6;
        public short screenID;
        public short buttonKeyType;
        public H2Color textColor;
        public char[] stringlistTag;
        public int stringsListIdent;
        public List<paneData> panes = new List<paneData>();
        public short shapeGroup;
        private short unused;
        public stringID headerStringID = new stringID();
        public H2Color sourceColor;
        public H2Color destColor;
        public float zoomScaleX;
        public float zoomScaleY;
        public float refractionScaleX;
        public float refractionScaleY;

        public screenData(int order)
            : base(order)
        {
        }

        /// <summary>
        /// Loads the strings from "stringListIdent" into "strings"
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool getStrings(Map map)
        {
            if (!isPopulated | stringsListIdent == -1)
                return false;
            Meta m = Map.GetMetaFromTagIndex(map.Functions.ForMeta.FindMetaByID(stringsListIdent), map, false, false);
            BinaryReader br = new BinaryReader(m.MS);
            br.BaseStream.Position = 16;    // English only...
            int offset = br.ReadUInt16();
            int count = br.ReadUInt16();

            int uiOffset = map.Unicode.ut[0].indexOffset;
            int utOffset = map.Unicode.ut[0].tableOffset;
            if (map.Unicode.ut[0].SIDs == null) 
                map.Unicode.ut[0].Read();
            
            map.OpenMap(MapTypes.Internal);
            br = new BinaryReader(map.FS);
            for (int i = 0; i < count; i++)
            {
                br.BaseStream.Position = utOffset + map.Unicode.ut[0].US[offset + i].offset;
                strings.Add(new string(br.ReadChars(map.Unicode.ut[0].US[offset + i].size)));
            }
            map.CloseMap();
            return true;
        }

        public void Read(BinaryReader br)
        {
            int flags = br.ReadInt32();
            flag0 = ((flags & 1) != 0); // 0
            flag1 = ((flags & 2) != 0); // 1
            flag2 = ((flags & 4) != 0); // 2
            flag3 = ((flags & 8) != 0); // 3
            flag4 = ((flags & 16) != 0); // 4
            flag5 = ((flags & 32) != 0); // 5
            flag6 = ((flags & 64) != 0); // 6
            screenID = br.ReadInt16();
            buttonKeyType = br.ReadInt16();
            textColor = H2Color.fromARGB(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
            stringlistTag = br.ReadChars(4);
            stringsListIdent = br.ReadInt32();
            br.BaseStream.Position += 8;
            shapeGroup = br.ReadInt16();
            unused = br.ReadInt16();
            headerStringID.Read(br);
            sourceColor = H2Color.fromARGB(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());
            destColor = H2Color.fromARGB(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle());

            zoomScaleX = br.ReadSingle();
            zoomScaleY = br.ReadSingle();
            refractionScaleX = br.ReadSingle();
            refractionScaleY = br.ReadSingle();            
            isPopulated = true;
        }

        public override string ToString()
        {
            return "[" + screenID.ToString("000") + "] " + title;
        }
    }

    public class skinData : baseData 
    {
        public Skin skin;

        public skinData()
            : base(0)
        {
        }
    }

    public class textBlockData : baseData
    {
        public bool leftAligned;
        public bool rightAligned;
        public bool pulsing;
        public bool tinyText;
        public short animationIndex;
        public short introAnimationDelay;
        public short unused;
        public short customFont;
        public H2Color h2color;
        public short top;
        public short left;
        public short bottom;
        public short right;
        public stringID stringID = new stringID();
        public short renderDepth;
        public short unused2;

        public textBlockData(int Order)
            : base(Order)
        {
        }

        public void Read(BinaryReader br)
        {
            br.BaseStream.Position = offset;
            int flags = br.ReadInt32();
            leftAligned = ((flags & 1) != 0); // bit 0
            rightAligned = ((flags & 2) != 0); // bit 1
            pulsing = ((flags & 4) != 0); // bit 2
            tinyText = ((flags & 8) != 0); // bit 3

            animationIndex = br.ReadInt16();
            introAnimationDelay = br.ReadInt16();
            unused = br.ReadInt16();
            customFont = br.ReadInt16();
            h2color = new H2Color();
            h2color.A = br.ReadSingle();
            h2color.R = br.ReadSingle();
            h2color.G = br.ReadSingle();
            h2color.B = br.ReadSingle();
            
            top = br.ReadInt16();
            left = br.ReadInt16();
            bottom = br.ReadInt16();
            right = br.ReadInt16();
            stringID.Read(br);
            renderDepth = br.ReadInt16();
            unused2 = br.ReadInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.BaseStream.Position = offset;
            int flags = int.MaxValue - 8 - 4 - 2 - 1;
            flags += flags | (leftAligned ? 1 : 0); // bit 0
            flags += flags | (rightAligned ? 2 : 0); // bit 1
            flags += flags | (pulsing ? 4 : 0); // bit 2
            flags += flags | (tinyText ? 8 : 0); // bit 3
            bw.Write(flags);

            bw.Write(animationIndex);
            bw.Write(introAnimationDelay);;
            bw.Write(unused);
            bw.Write(customFont);
            bw.Write(h2color.A);
            bw.Write(h2color.R);
            bw.Write(h2color.G);
            bw.Write(h2color.B);

            bw.Write(top);
            bw.Write(left);
            bw.Write(bottom);
            bw.Write(right);
            stringID.Write(bw);
            bw.Write(renderDepth);
            bw.Write(unused2);
        }
    }
}
