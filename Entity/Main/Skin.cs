using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using HaloMap.Map;
using HaloMap.Meta;
using HaloMap.RawData;

using entity.Main.MenuClasses;

namespace entity.Main
{
    public class Skin
    {
        bool isLoaded = false;
        Map map;
        Meta meta;

        List<bitmapData> bitmapBlocks;
        List<textBlockData> textBlocks;

        public Skin(Map map, int tagIdent)
        {
            this.map = map;
            meta = Map.GetMetaFromTagIndex(map.Functions.ForMeta.FindMetaByID(tagIdent), map, false, false);
        }

        public Skin(Map map, Meta meta)
        {
            this.map = map;
            this.meta = meta;
        }

        private void loadSkin()
        {
            bitmapBlocks = new List<bitmapData>();
            textBlocks = new List<textBlockData>();
            BinaryReader br = new BinaryReader(meta.MS);
            BinaryReader br2 = br;

            #region Bitmap block reflexive
            br.BaseStream.Position = 36;
            int bitmapBlockCount = br.ReadInt32();
            int bitmapBlockOffset = br.ReadInt32() - map.SecondaryMagic;

            // Handles reflexives in other tags (pointers)
            int bitmBlockTag = map.Functions.ForMeta.FindMetaByOffset(bitmapBlockOffset);
            if (bitmapBlockCount > 0 &&  bitmBlockTag != meta.TagIndex)
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
                bd.meta = Map.GetMetaFromTagIndex(map.Functions.ForMeta.FindMetaByID(bd.bitmIdent), map, false, false);
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
                bitmapData bd = this.bitmapBlocks[i];
                bitBlocks.Add(bd);
            }
            for (int i = 0; i < this.textBlocks.Count; i++)
            {
                textBlockData td = textBlocks[i];
                bitmapData bdt = new bitmapData(td.order);
                int width = (td.right - td.left);
                Bitmap textBitm = new Bitmap(width, td.top - td.bottom);
                using (Graphics g = Graphics.FromImage(textBitm))
                {
                    Font font = new Font(FontFamily.GenericMonospace, 9.0f);
                    switch (td.customFont)
                    {
                        case 0:     // fixedsys-9                            
                            break;
                        case 1:     // conduit-12
                            font = new Font(FontFamily.GenericMonospace, 12.0f);
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

                    SizeF textInfo = g.MeasureString(Text,font);
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
                            (byte)(td.h2color.A * byte.MaxValue),
                            (byte)(td.h2color.R * byte.MaxValue),
                            (byte)(td.h2color.G * byte.MaxValue),
                            (byte)(td.h2color.B * byte.MaxValue)
                        ));
                    #endregion

                    g.DrawString(Text, font, brush, textLeft, 0);

                }
                bdt.left = td.left;
                bdt.top = (short)td.bottom;
                bdt.link = textBitm;
                bdt.meta = new Meta(map); // We use a null meta for drawing checks, so  make sure we have something.
                bdt.renderDepth = td.renderDepth;
                bitBlocks.Add(bdt);
            }

            return bitBlocks;
        }
    }
}
