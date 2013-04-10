using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using H2Font;
using HaloMap.Map;
using HaloMap.Meta;
using HaloMap.RawData;

using entity.Main.MenuClasses;
using Globals;

namespace entity.Main
{
    public class Skin
    {
        #region Constants and Fields
        bool isLoaded = false;
        Map map;
        Meta meta;

        /// <summary>
        /// Access to the original Halo 2 fonts
        /// </summary>
        static private List<H2FontClass.H2Font> H2fonts = new List<H2FontClass.H2Font>();

        List<animationData> animation;
        List<bitmapData> bitmapBlocks;
        List<textBlockData> textBlocks;
        /// <summary>
        /// Used to keep track of the fonts listed in the "font_table.txt" file
        /// </summary>
        static List<string> fontNames = new List<string>();
        #endregion

        #region Constructors and Destructors
        public Skin(Map map, int tagIdent)
        {
            this.map = map;
            meta = Map.GetMetaFromTagIndex(map.Functions.ForMeta.FindMetaByID(tagIdent), map, false, false);
            loadFontNames();
        }

        public Skin(Map map, Meta meta)
        {
            this.map = map;
            this.meta = meta;
            loadFontNames();
        }
        #endregion

        static private void loadFontNames()
        {
            // Don't reload font list from file if it is already loaded
            if (fontNames.Count > 0)
                return;

            if (!File.Exists(Prefs.pathFontsFolder + "font_table.txt"))
            {
                #region default font_table.txt listing
                fontNames.Add("fixedsys-9");
                fontNames.Add("conduit-12");
                fontNames.Add("handel_gothic-11");
                fontNames.Add("handel_gothic-24");
                fontNames.Add("conduit-13");
                fontNames.Add("conduit-12");
                fontNames.Add("conduit-13");
                fontNames.Add("conduit-12");
                fontNames.Add("MSLCD-14");
                fontNames.Add("conduit-16");
                fontNames.Add("handel_gothic-13");
                #endregion
            }
            else
            {
                StreamReader sr = new StreamReader(Prefs.pathFontsFolder + "font_table.txt");
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s != string.Empty)
                        fontNames.Add(s);
                }
            }
        }

        private void loadSkin()
        {
            animation = new List<animationData>();
            bitmapBlocks = new List<bitmapData>();
            textBlocks = new List<textBlockData>();
            BinaryReader br = new BinaryReader(meta.MS);
            BinaryReader br2 = br;

            #region Animation Frames reflexive
            br2 = br;
            br.BaseStream.Position = 20;
            int animationCount = br.ReadInt32();
            int animationOffset = br.ReadInt32() - map.SecondaryMagic;

            // Handles reflexives in other tags (pointers)
            int animationTag = map.Functions.ForMeta.FindMetaByOffset(animationOffset);
            if (animationCount > 0 && animationTag != meta.TagIndex)
            {
                Meta newMeta = Map.GetMetaFromTagIndex(animationTag, meta.Map, false, true);
                br2 = new BinaryReader(newMeta.MS);
                animationOffset -= newMeta.offset;
            }
            else
                animationOffset -= meta.offset;

            for (int list = 0; list < animationCount; list++)
            {               
                animationData ad = new animationData(list, meta);
                ad.offset = animationOffset + list * 16;
                ad.Read(br2);
                this.animation.Add(ad);
            }
            #endregion

            #region Text blocks reflexive
            br2 = br;
            br.BaseStream.Position = 28;
            int textBlockCount = br.ReadInt32();
            int textBlockOffset = br.ReadInt32() - map.SecondaryMagic;

            // Handles reflexives in other tags (pointers)
            int textBlockTag = map.Functions.ForMeta.FindMetaByOffset(textBlockOffset);
            if (textBlockCount > 0 && textBlockTag != meta.TagIndex)
            {
                Meta newMeta = Map.GetMetaFromTagIndex(textBlockTag, map, false, true);
                br2 = new BinaryReader(newMeta.MS);
                textBlockOffset -= newMeta.offset;
            }
            else
                textBlockOffset -= meta.offset;

            for (int list = 0; list < textBlockCount; list++)
            {
                textBlockData tbd = new textBlockData(list);
                tbd.offset = textBlockOffset + list * 44;
                tbd.Read(br2);
                this.textBlocks.Add(tbd);
            }
            #endregion

            #region Bitmap block reflexive
            br.BaseStream.Position = 36;
            int bitmapBlockCount = br.ReadInt32();
            int bitmapBlockOffset = br.ReadInt32() - map.SecondaryMagic;

            // Handles reflexives in other tags (pointers)
            int bitmBlockTag = map.Functions.ForMeta.FindMetaByOffset(bitmapBlockOffset);
            if (bitmapBlockCount > 0 && bitmBlockTag != meta.TagIndex)
            {
                Meta newMeta = Map.GetMetaFromTagIndex(bitmBlockTag, map, false, true);
                br2 = new BinaryReader(newMeta.MS);
                bitmapBlockOffset -= newMeta.offset;
            }
            else
                bitmapBlockOffset -= meta.offset;

            // Always add 3 in case of using sub bitmaps
            for (int list = 0; list < bitmapBlockCount; list++)
            {
                bitmapData bd = new bitmapData(list);
                bd.offset = bitmapBlockOffset + list * 56;
                bd.Read(br2);
                int bitmID = map.Functions.ForMeta.FindMetaByID(bd.bitmIdent);
                if (bitmID != -1)
                {
                    bd.meta = Map.GetMetaFromTagIndex(bitmID, map, false, false);
                    ParsedBitmap pm = new ParsedBitmap(ref bd.meta, map);
                    bd.link = pm.FindChunkAndDecode(0, 0, 0, ref bd.meta, map, 0, 0);
                    this.bitmapBlocks.Add(bd);

                    if (pm.Properties.Length > 1)
                    {
                        bd = (bitmapData)bd.Clone();
                        bd.link = pm.FindChunkAndDecode(1, 0, 0, ref bd.meta, map, 0, 0);
                    }
                    this.bitmapBlocks.Add(bd);

                    if (pm.Properties.Length > 2)
                    {
                        bd = (bitmapData)bd.Clone();
                        bd.link = pm.FindChunkAndDecode(2, 0, 0, ref bd.meta, map, 0, 0);
                    }
                    this.bitmapBlocks.Add(bd);
                }
            }
            #endregion

            isLoaded = true;
        }

        public Rectangle getMenuSize()
        {
            if (!isLoaded)
                loadSkin();

            // Find extremes of all bitmaps
            int left = int.MaxValue;
            int right = int.MinValue;
            int top = int.MaxValue;
            int bottom = int.MinValue;

            // Need to take into account render depth for these still!!!
            foreach (bitmapData bd in this.bitmapBlocks)
            {
                top = bd.top < top ? bd.top : top;
                left = bd.left < left ? bd.left : left;
                Bitmap b = (Bitmap)bd.link;
                right = (b.Width + bd.left) > right ? (b.Width + bd.left) : right;
                bottom = (b.Height + bd.top) > bottom ? (b.Height + bd.top) : bottom;
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }

        public List<bitmapData> getMenuSelection(string Text, int selection)
        {
            if (!isLoaded)
                loadSkin();

            List<bitmapData> bitBlocks = new List<bitmapData>();

            for (int i = 0; i < this.bitmapBlocks.Count; i++)
            {
                // Each submap is saved togther, so skip 3 and offset depending on selection style
                if (i % 3 != selection)
                    continue;
                bitmapData bd = (bitmapData)this.bitmapBlocks[i].Clone();
                if (selection == 0)
                {
                    bd.left += (short)animation[0].keyFrames[1].posX;
                    bd.top -= (short)animation[0].keyFrames[1].posY;
                }
                bitBlocks.Add(bd);
            }
            for (int i = 0; i < this.textBlocks.Count; i++)
            {
                textBlockData tbd = (textBlockData)textBlocks[i].Clone();
                if (selection == i)
                {
                    tbd.left += (short)animation[0].keyFrames[1].posX;
                    tbd.top -= (short)animation[0].keyFrames[1].posY;
                }
                bitBlocks.Add(createText(map, tbd, Text, false));
            }

            return bitBlocks;
        }

        private static H2FontClass.H2Font getFont(string fontName)
        {
            foreach (H2FontClass.H2Font H2f in H2fonts)
                if (H2f.ToString() == fontName)
                {
                    return H2f;
                }
            H2FontClass.H2Font H2font = new H2FontClass.H2Font(Prefs.pathFontsFolder + fontName);
            H2fonts.Add(H2font);
            return H2font;
        }

        public static bitmapData createText(Map map, textBlockData td, string Text, bool wrapText)
        {
            if (Text == string.Empty || td.customFont == -1)
                return null;
            bitmapData bdt = new bitmapData(td.order);
            int width = (td.right - td.left);
            Bitmap textBitm = new Bitmap( width, Math.Abs(td.top - td.bottom));
            H2FontClass.H2Font H2font = getFont(fontNames[td.customFont]);
            
            // For Tiny text, just use our default font as it doens't really matter
            if (H2font.isLoaded && !td.tinyText)
            {
                // Replace codes such as <GamerTag>, but not "A button" and medals, etc
                Text = entity.MetaFuncs.MEStringsSelector.ReplaceCodes(Text, false);
                using (Graphics g = Graphics.FromImage(textBitm))
                {
                    Bitmap fontBitm = wrapText ? 
                        H2font.writeString(Text, width) : 
                        H2font.writeString(Text);
                    if (fontBitm != null)
                    {

                        #region Text alignment

                        int textLeft;
                        if (td.leftAligned)
                            textLeft = 0;
                        else if (td.rightAligned)
                            textLeft = (int)(width - fontBitm.Width);
                        else  // center aligned
                            textLeft = (int)(width / 2 - fontBitm.Width / 2);

                        #endregion

                        if (!wrapText)
                        {
                            g.DrawImage(fontBitm, new Point(textLeft, 0));
                            bdt.top = (short)(td.bottom);
                        }
                        else
                        {
                            g.DrawImage(fontBitm, new Point(textLeft, 0));
                            bdt.top = (short)td.top;
                        }
                        fontBitm.Dispose();
                    }
                }
            }
            else
            {
                // Replace all codes internal and font contained as windows
                // fonts will not contain the special H2 codes
                Text = entity.MetaFuncs.MEStringsSelector.ReplaceCodes(Text, true);
                using (Graphics g = Graphics.FromImage(textBitm))
                {
                    Font font = new Font(FontFamily.GenericMonospace, 5.0f);
                    if (!td.tinyText)
                        switch (td.customFont)
                        {
                            case 0:     // fixedsys-9                            
                                font = new Font(FontFamily.GenericMonospace, 9.0f);
                                break;
                            case 1:     // conduit-12
                                font = new Font("Arial Narrow", 12.0f);
                                break;
                            case 2:     // handel_gothic-11
                                font = new Font(FontFamily.GenericSerif, 11.0f);
                                break;
                            case 3:     // handel_gothic-24
                                font = new Font(FontFamily.GenericSerif, 24.0f);
                                break;
                            case 4:     // conduit-13
                                font = new Font(FontFamily.GenericMonospace, 13.0f);
                                break;
                            case 5:     // conduit-12
                                font = new Font(FontFamily.GenericMonospace, 12.0f);
                                break;
                            case 6:     // conduit-13
                                font = new Font(FontFamily.GenericMonospace, 13.0f);
                                break;
                            case 7:     // conduit-12
                                font = new Font(FontFamily.GenericMonospace, 12.0f);
                                break;
                            case 8:     // MSLCD-14
                                font = new Font(FontFamily.GenericSerif, 14.0f);
                                break;
                            case 9:     // conduit-16
                                font = new Font(FontFamily.GenericMonospace, 16.0f);
                                break;
                            case 10:     // handel_gothic-13
                                font = new Font(FontFamily.GenericSerif, 13.0f);
                                break;
                        }

                    #region Text alignment

                    SizeF textInfo = g.MeasureString(Text, font);
                    int textLeft;
                    if (td.leftAligned)
                        textLeft = 0;
                    else if (td.rightAligned)
                        textLeft = (int)(width - textInfo.Width);
                    else  // center aligned
                        textLeft = (int)(width / 2 - textInfo.Width / 2);

                    #endregion

                    #region Text color
                    Brush brush = new SolidBrush(
                        Color.FromArgb(
                            (byte)(td.h2color.A == 0 ? 255 : td.h2color.A * byte.MaxValue),
                            (byte)(td.h2color.R * byte.MaxValue),
                            (byte)(td.h2color.G * byte.MaxValue),
                            (byte)(td.h2color.B * byte.MaxValue)
                        ));
                    #endregion

                    if (!wrapText)
                    {
                        g.DrawString(Text, font, brush, textLeft, 0);
                        bdt.top = (short)(td.bottom);// + textInfo.Height);
                    }
                    else
                    {
                        g.DrawString(Text, font, brush, new RectangleF(0, 0, textBitm.Width, textBitm.Height));
                        bdt.top = (short)td.top;
                    }
                }
            }
            bdt.left = td.left;
            bdt.link = textBitm;
            bdt.meta = new Meta(map); // We use a null meta for drawing checks, so  make sure we have something.
            bdt.renderDepth = td.renderDepth;
            return bdt;

        }
    }

}
