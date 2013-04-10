/*
 * lbScreensList.Item cast as screenData
 *      offset = lbPanes.SelectedIndex;       // The last selected pane for that screen
 *      link = new int[lbPanes.Items.Count];  // Keeps track of which bitmap # is selected in each pane
 *      
 * lbPanes.Item cast as baseData
 *      offset = panesOffset + panes * 76;    // Store panes offset for quicker access
 *      link = ***  ***
 *
 * lbBitmaps.Item cast as bitmapData
 *      offset = bitmapOffset + bitmaps * 56;
 *      link = Bitmap Image
 *      
 * cbBitmapIdent cast as baseData
 *      offset = panesOffset + panes * 76;    // Store panes offset for quicker access
 *      link = Bitmap Image?
 *      
 */
namespace entity.Main
{
    using Microsoft.DirectX;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using entity.MapForms;
    using entity.Main.MenuClasses;

    using HaloMap;
    using HaloMap.DDSFunctions;
    using HaloMap.Map;
    using HaloMap.Meta;

    using Globals;

    using HaloMap.RawData;

    public partial class MainmenuVisualEdit : Form
    {
        #region classes


        #endregion

        #region enums
        /*
        public enum buttonKeyType
        {
          "[none]",
          "A Select B Back",
          "A Select B Cancel",
          "A Enter B Cancel",
          "X Party Privacy A Select B Cancel",
          "A Select B Done",
          "X Clan Member Options",
          "A Select B Resume",
          "X Options",
          "A Select",
          "X Settings A Select B Back",
          "A Select B Done",
          "A Accept",
          "B Cancel",
          "Y Friends A Select B Back",
          "Y Friends A Select B Cancel",
          "Y Friends A Enter B Cancel",
          "Y Friends A Select",
          "Y Friends A Select B Done",
          "Y Friends A Accept",
          "Y Friends A Cancel",
          "A Select B Back",
          "A Continue"

        }
        */
        #endregion

        #region Constants and Fields

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        /// <summary>
        /// The br.
        /// </summary>
        private BinaryReader br;

        /// <summary>
        /// The bw.
        /// </summary>
        private BinaryWriter bw;

        /// <summary>
        /// The currsntly selected screen
        /// </summary>
        screenData currentScreen;

        /// <summary>
        /// Used for motion bitmaps
        /// </summary>
        long tickTimer;

        /// <summary>
        /// The index of the bitmap being dragged. (-1 for none)
        /// </summary>
        int currentDragBitmap = -1;
        /// <summary>
        /// Keeps track of the mouse start position for the drag
        /// </summary>
        Point currentDragStart;

        /// <summary>
        /// A list of all screens available
        /// </summary>
        List<screenData> screens = new List<screenData>();

        /// <summary>
        /// For StringID editing/selection
        /// </summary>
        entity.MetaFuncs.MEStringsSelector sSwap;

        #region Default Header Font Data
        /// <summary>
        /// The default font used for headers text (ui\\shared_globals, offset #352)
        /// </summary>
        short defaultHeaderFont;
        /// <summary>
        /// The default location of the header fonr text
        /// </summary>
        Rectangle defaultHeaderPos;
        #endregion
        #endregion

        #region Constructors and Destructors
        public MainmenuVisualEdit()
        {
            InitializeComponent();

            #region If H2 fonts have not been setup, ask to locate now
            if (!Directory.Exists(Prefs.pathFontsFolder))
            {
                if (MessageBox.Show("H2 fonts directory has not been setup yet.\nIf you cancel, a default set of windows fonts will be used.\nDo you wish to locate the fonts now?", "Locate H2 Fonts", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.Description = "Select location of Halo 2 Fonts to display actual fonts";
                    fbd.SelectedPath = Prefs.pathMapsFolder;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        Prefs.pathFontsFolder = fbd.SelectedPath;
                        Prefs.Save();
                    }
                }

            }
            if (!Prefs.pathFontsFolder.EndsWith("\\"))
                Prefs.pathFontsFolder += "\\";
            #endregion

            btnMainmenuFile.Text = Prefs.pathMainmenu;
            if (!loadMainMenuData(btnMainmenuFile.Text))
            {
                btnMainmenuFile_Click(this, null);
            }
        }
        #endregion

        #region Methods

        private void btnMainmenuFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = btnMainmenuFile.Text;
            do
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    btnMainmenuFile.Text = openFileDialog1.FileName;
                    loadMainMenuData(btnMainmenuFile.Text);
                }
                else
                    btnMainmenuFile.Text = string.Empty;
            } while (map == null);
        }

        private void btnRestoreAll_Click(object sender, EventArgs e)
        {
            loadMainMenuData(btnMainmenuFile.Text);
        }

        private void btnRestorePane_Click(object sender, EventArgs e)
        {
            int psi = lbPanes.SelectedIndex;
            int bsi = lbBitmaps.SelectedIndex;

            /*
            baseData sd = (baseData)lbPanes.SelectedItem;
            if (sd == null)
                return;
            map.OpenMap(MapTypes.Internal);
            BinaryReader BR = new BinaryReader(map.FS);
            BR.BaseStream.Position = sd.meta.offset + sd.offset;
            sd.meta.MS.Write(BR.ReadBytes(sd.meta.size), sd.offset, sd.meta.size);
            map.CloseMap();
            */

            lbBitmaps.SelectedIndex = -1;
            lbPanes_SelectedIndexChanged(lbPanes, null);
            lbBitmaps.SelectedIndex = bsi;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            writePaneToMemory();

            baseData sd = (baseData)lbScreensList.SelectedItem;
            map.OpenMap(MapTypes.MainMenu, false);
            if (!map.isOpen)
                return;
            map.FS.Position = sd.meta.offset;
            map.FS.Write(sd.meta.MS.ToArray(), 0, sd.meta.size);
            map.CloseMap();
        }

        private void btnScrollDown_Click(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            switch (c.Name)
            {
                case "btnXScrollDown":
                    c = tbXScroll;
                    break;
                case "btnYScrollDown":
                    c = tbYScroll;
                    break;
            }
            float f = (float)Math.Round(float.Parse(c.Text), 2);
            c.Text = (f - 0.01f).ToString("0.00");
        }

        private void btnScrollUp_Click(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            switch (c.Name)
            {
                case "btnXScrollUp":
                    c = tbXScroll;
                    break;
                case "btnYScrollUp":
                    c = tbYScroll;
                    break;
            }
            float f = (float)Math.Round(float.Parse(c.Text),2);
            c.Text = (f + 0.01f).ToString("0.00");
        }

        private void btnTBStringID_Click(object sender, EventArgs e)
        {
            if (this.sSwap == null)
            {
                this.sSwap = new entity.MetaFuncs.MEStringsSelector(map, this);
                //this.sSwap.cbSelectStringIDs.SelectedIndex = 1;  // Unicodes only
            }

            textBlockData tbd = (textBlockData)lbTBTextBlocks.SelectedItem;
            this.sSwap.SelectedID = tbd.stringID.sidIndexer;
            if (this.sSwap.ShowDialog() == DialogResult.OK)
            {
                //this.Enabled = true;

                short id = (short)this.sSwap.SelectedID;
                if (tbd.stringID.sidIndexer != id)
                {
                    tbd.stringID.sidIndexer = id;
                    tbd.stringID.length = (byte)map.Strings.Length[id];
                }
            }
        }

        private void cbBitmapIdent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbBitmapIdent.Focused)
                return;
            baseData sd = (baseData)cbBitmapIdent.SelectedItem;
            bitmapInfo bi = (bitmapInfo)sd.link;
            bitmapData bsd = (bitmapData)lbBitmaps.SelectedItem;
            if (sd.meta == null)
            {
                sd.meta = new Meta(map);
                map.OpenMap(MapTypes.MainMenu);
                sd.meta.ReadMetaFromMap(sd.offset, false);
                map.CloseMap();
                sd.title = sd.meta.name;

                sd.link = bi = new bitmapInfo();
                ParsedBitmap pm = new ParsedBitmap(ref sd.meta, map);
                bi.BitmapIndexMax = (short)pm.Properties.Length;
                bi.BitmapIndex = 0;
                bi.Image = pm.FindChunkAndDecode(bi.BitmapIndex, 0, 0, ref sd.meta, map, 0, 0);
                sd.link = bi;
            }
            bsd.bitmNumber = bi.BitmapIndex;
            bsd.link = bi.Image;
            bsd.meta = sd.meta;
            bsd.bitmIdent = bsd.meta.ident;
            bsd.title = sd.title.Substring(sd.title.LastIndexOf('\\') + 1);

            #region do this to update text in listbox, otherwise although changed, it doesn't show
            Control control = this.ActiveControl;
            while (control is ContainerControl)
            {
                control = ((ContainerControl)control).ActiveControl;
            }

            int selIndex = lbBitmaps.SelectedIndex;
            ((bitmapData)lbBitmaps.SelectedItem).title = bsd.title;
            // Actually update ListBox with data source
            ((CurrencyManager)lbBitmaps.BindingContext[lbBitmaps.DataSource]).Refresh();
            #endregion
        }

        private void cbBitmapIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbBitmapIndex.Focused)
                return;
            bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
            bitmapInfo bi = (bitmapInfo)((baseData)cbBitmapIdent.SelectedItem).link;
            bd.bitmNumber = bi.BitmapIndex = (short)cbBitmapIndex.SelectedIndex;
            ParsedBitmap pm = new ParsedBitmap(ref bd.meta, map);
            bd.link = bi.Image = pm.FindChunkAndDecode(bd.bitmNumber, 0, 0, ref bd.meta, map, 0, 0);
        }

        private void cbLBSkinIdent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLBSkinIdent.SelectedIndex == -1)
                return;

            int i = Math.Min(((paneData)lbPanes.SelectedItem).listBlocks[0].visibleItemCount, currentScreen.strings.Count);
            int selection = 1;
            if (lbPanes.Items.Count > 0)
                selection = lbPanes.SelectedIndex;
            //createSkin(i > 0 ? (i < 2 ? 0 : 2) : 1);
            createSkin( selection );
        }

        private void chSort_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lbBitmaps_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbBitmapIndex.Enabled = false;
            cbBitmapIndex.Items.Clear();
            
            screenData paneBitmapSelections = (screenData)lbScreensList.SelectedItem;
            if (paneBitmapSelections.link == null)
                paneBitmapSelections.link = new int[lbPanes.Items.Count];
            ((int[])paneBitmapSelections.link)[lbPanes.SelectedIndex] = lbBitmaps.SelectedIndex;

            if (lbBitmaps.SelectedIndex < 1 || lbBitmaps.SelectedItem == null)
            {
                tbXPos.Enabled = false;
                tbYPos.Enabled = false;
                tbXScroll.Enabled = false;
                tbYScroll.Enabled = false;
                tbRenderDepth.Enabled = false;
                cbBitmapIdent.Enabled = false;
                cbBitmapIdent.SelectedIndex = 0;
                return;
            }
            else
            {
                tbXPos.Enabled = true;
                tbYPos.Enabled = true;
                tbXScroll.Enabled = true;
                tbYScroll.Enabled = true;
                tbRenderDepth.Enabled = true;
                cbBitmapIdent.Enabled = true;
            }

            // Actually update ListBox with data source
            ((CurrencyManager)lbBitmaps.BindingContext[lbBitmaps.DataSource]).Refresh();
            bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
            //bitmapData bd = ((List<bitmapData>)lbBitmaps.DataSource)[lbBitmaps.SelectedIndex];
            
            tbXPos.Text = bd.left.ToString();
            tbYPos.Text = bd.top.ToString();
            tbXScroll.Text = bd.horizontalWrapsPerSec.ToString();
            tbYScroll.Text = bd.verticalWrapsPerSec.ToString();
            tbRenderDepth.Text = bd.renderDepth.ToString();
            if (bd.bitmIdent != -1)
            {
                cbBitmapIdent.Text = map.FileNames.Name[(int)map.MetaInfo.identHT[bd.bitmIdent]];
            }
            else
                cbBitmapIdent.Text = "null";
            if (bd.link != null)
            {
                baseData sd = (baseData)cbBitmapIdent.SelectedItem;
                ParsedBitmap pm = new ParsedBitmap(ref bd.meta, map);
                bitmapInfo bi = (bitmapInfo)sd.link;
                if (bi == null)
                {
                    bi = new bitmapInfo();
                    bi.BitmapIndex = bd.bitmNumber;
                    bi.BitmapIndexMax = (short)pm.Properties.Length;
                    bi.Image = (Bitmap)bd.link;
                    sd.link = bi;
                }
                if (bi.BitmapIndex != bd.bitmNumber)
                {
                    bi.BitmapIndex = bd.bitmNumber;
                    bi.Image = (Bitmap)bd.link;
                }
                if (bi.BitmapIndexMax > 1) 
                    cbBitmapIndex.Enabled = true;
                for (int i = 0; i < bi.BitmapIndexMax; i++)
                    cbBitmapIndex.Items.Add(i.ToString());
                cbBitmapIndex.SelectedIndex = Math.Min(bd.bitmNumber, bi.BitmapIndexMax - 1);
            }
        }

        private void lbPanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPanes.Focused)
                writePaneToMemory();

            if (lbPanes.SelectedIndex == -1)
                return;

            paneData pd = (paneData)lbPanes.SelectedItem;
            br = new BinaryReader(pd.meta.MS);

            #region Bitmaps
            // Clear the list of bitmaps
            pd.bitmaps.Clear();
            // Add a default "no selection" to the top of the list
            pd.bitmaps.Add(new bitmapData(0));
            pd.bitmaps[0].title = "<None>";

            br.BaseStream.Position = pd.offset + 36;
            int bitmapCount = br.ReadInt32();
            int bitmapOffset = br.ReadInt32() - map.SecondaryMagic;

            if (bitmapCount > 0)
            {
                BinaryReader br2 = br;

                // Handle pointers to other tags
                int bitmTag = map.Functions.ForMeta.FindMetaByOffset(bitmapOffset);
                if (bitmTag != currentScreen.meta.TagIndex)
                {
                    Meta newMeta = Map.GetMetaFromTagIndex(bitmTag, map, false, true);
                    br2 = new BinaryReader(newMeta.MS);
                    bitmapOffset -= newMeta.offset;
                }
                else
                    bitmapOffset -= currentScreen.meta.offset;

                for (int bitmaps = 0; bitmaps < bitmapCount; bitmaps++)
                {
                    bitmapData bitmSD = new bitmapData(bitmaps);
                    bitmSD.offset = bitmapOffset + bitmaps * 56;
                    bitmSD.Read(br2);

                    int tagNum = bitmSD.bitmIdent != -1 ? (int)map.MetaInfo.identHT[bitmSD.bitmIdent] : -1;

                    if (tagNum != -1)
                    {
                        bitmSD.meta = Map.GetMetaFromTagIndex(tagNum, map, false, true);
                        bitmSD.title = map.FileNames.Name[tagNum].Substring(map.FileNames.Name[tagNum].LastIndexOf('\\') + 1);
                    }
                    else
                    {
                        bitmSD.meta = null;
                        bitmSD.title = string.Empty;
                    }

                    if (bitmSD.meta == null)
                    {
                        bitmSD.link = null;
                        continue;
                    }

                    ParsedBitmap pm = new ParsedBitmap(ref bitmSD.meta, map);
                    Bitmap b = pm.FindChunkAndDecode(bitmSD.bitmNumber, 0, 0, ref bitmSD.meta, map, 0, 0);

                    System.Drawing.Imaging.ImageAttributes imgAttr = new System.Drawing.Imaging.ImageAttributes();

                    // If alpha-blended = 1, invert alpha channel now (way faster to do once!)
                    if (bitmSD.blendMethod == 1)
                    {
                        Bitmap bTemp = new Bitmap(b.Width, b.Height);
                        using (Graphics g = Graphics.FromImage(bTemp))
                        {

                            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
                            cm.Matrix00 = 1.0f;
                            cm.Matrix03 = -1.0f;    // Reverse red alpha
                            cm.Matrix11 = 1.0f;
                            cm.Matrix13 = -1.0f;    // Reverse green alpha
                            cm.Matrix22 = 1.0f;
                            cm.Matrix23 = -1.0f;    // REverse blue alpha
                            cm.Matrix33 = 1.0f;
                            imgAttr.SetColorMatrix(cm);

                            g.Clear(Color.Empty);

                            g.DrawImage(
                                b,
                                new Rectangle(
                                    0,
                                    0,
                                    b.Width,
                                    b.Height),
                                0,
                                0,
                                b.Width,
                                b.Height,
                                GraphicsUnit.Pixel,
                                imgAttr
                                );
                            g.Dispose();
                        }
                        b.Dispose();
                        b = bTemp;
                    }
                    bitmSD.link = b;

                    pd.bitmaps.Add(bitmSD);
                }

                drawBitmap();
            }
            tabControl1.TabPages[0].Text = "Bitmaps [" + bitmapCount.ToString() + "]";
            tabControl1.TabPages[0].Enabled = (bitmapCount > 0);
            
            #endregion

            #region List Blocks
            // Clear the list of List Blocks
            pd.listBlocks.Clear();
            // Always use the first panes Data for list blocks
            br.BaseStream.Position = ((paneData)lbPanes.Items[0]).offset + 12;
            int listBlockCount = br.ReadInt32();
            int listBlockOffset = br.ReadInt32() - map.SecondaryMagic;

            if (listBlockCount > 0)
            {
                BinaryReader br2 = br;

                // Handle pointers to other tags
                int listBlockTag = map.Functions.ForMeta.FindMetaByOffset(listBlockOffset);
                if (listBlockTag != currentScreen.meta.TagIndex)
                {
                    Meta newMeta = Map.GetMetaFromTagIndex(listBlockTag, map, false, true);
                    br2 = new BinaryReader(newMeta.MS);
                    listBlockOffset -= newMeta.offset;
                }
                else
                    listBlockOffset -= currentScreen.meta.offset;

                for (int listBlock = 0; listBlock < listBlockCount; listBlock++)
                {
                    listBlockData ld = new listBlockData(listBlock);
                    br2.BaseStream.Position = ld.offset = listBlockOffset + listBlock * 24;
                    ld.Read(br2);

                    //bitmSD.link = b;

                    pd.listBlocks.Add(ld);
                }
            }
            tabControl1.TabPages[1].Text = "List Boxes [" + listBlockCount.ToString() + "]";
            tabControl1.TabPages[1].Enabled = (listBlockCount > 0);
            #endregion

            #region Text Blocks
            br.BaseStream.Position = pd.offset + 28;
            int textBlockCount = br.ReadInt32();
            int textBlockOffset = br.ReadInt32() - map.SecondaryMagic;

            if (textBlockCount > 0)
            {
                BinaryReader br2 = br;

                // Handle pointers to other tags
                int textBlockTag = map.Functions.ForMeta.FindMetaByOffset(textBlockOffset);
                if (textBlockTag != currentScreen.meta.TagIndex)
                {
                    Meta newMeta = Map.GetMetaFromTagIndex(textBlockTag, map, false, true);
                    br2 = new BinaryReader(newMeta.MS);
                    textBlockOffset -= newMeta.offset;
                }
                else
                    textBlockOffset -= currentScreen.meta.offset;

                pd.textBlocks.Clear();
                for (int textBlock = 0; textBlock < textBlockCount; textBlock++)
                {
                    textBlockData tb = new textBlockData(textBlock);
                    br2.BaseStream.Position = tb.offset = textBlockOffset + textBlock * 44;
                    tb.Read(br2);
                    tb.title = textBlock.ToString("0: ") + sSwap.getStringFromID(tb.stringID.sidIndexer);

                    pd.textBlocks.Add(tb);
                }
            }

            tabControl1.TabPages[2].Text = "Text Blocks [" + textBlockCount.ToString() + "]";
            tabControl1.TabPages[2].Enabled = (textBlockCount > 0);
            #endregion

            #region Model Scenes
            br.BaseStream.Position = pd.offset + 44;
            int modelScenesCount = br.ReadInt32();
            int modelScenesOffset = br.ReadInt32() - map.SecondaryMagic;

            if (modelScenesCount > 0)
            {
            }

            tabControl1.TabPages[3].Text = "Model Scenes [" + modelScenesCount.ToString() + "]";
            tabControl1.TabPages[3].Enabled = (modelScenesCount > 0);
            #endregion

            #region Variable Buttons
            br.BaseStream.Position = pd.offset + 68;
            int variableButtonsCount = br.ReadInt32();
            int variableButtonsOffset = br.ReadInt32() - map.SecondaryMagic;

            if (variableButtonsCount > 0)
            {
            }

            tabControl1.TabPages[4].Text = "Variable Buttons [" + variableButtonsCount.ToString() + "]";
            tabControl1.TabPages[4].Enabled = (variableButtonsCount > 0);
            #endregion

            #region Buttons
            br.BaseStream.Position = pd.offset + 4;
            int buttonsCount = br.ReadInt32();
            int buttonsOffset = br.ReadInt32() - map.SecondaryMagic;

            if (buttonsCount > 0)
            {
            }

            tabControl1.TabPages[5].Text = "Buttons [" + buttonsCount.ToString() + "]";
            tabControl1.TabPages[5].Enabled = (buttonsCount > 0);
            #endregion

            screenData screenSelected = (screenData)lbScreensList.SelectedItem;
            // offset stores the last currently selected pane for that screen
            screenSelected.offset = lbPanes.SelectedIndex;

            #region Set the bitmap list box to display our data in pd.bitmaps
            lbBitmaps.DataSource = pd.bitmaps;
            #endregion
            
            #region Set the skin combo box to display the current skin
            if (pd.listBlocks.Count > 0)
            {
                cbLBSkinIdent.Enabled = true;
                // Change value to be sure skin is created
                cbLBSkinIdent.SelectedIndex = -1;
                cbLBSkinIdent.SelectedIndex = pd.listBlocks[0].skinIndex;
            }
            else
            {
                cbLBSkinIdent.Enabled = false;
                cbLBSkinIdent.SelectedIndex = -1;
            }
            #endregion

            #region Set the text block list box to display our data in pd.textBlocks
            lbTBTextBlocks.DataSource = pd.textBlocks;

            foreach (textBlockData td in pd.textBlocks)
            {
                List<bitmapData> lbd = new List<bitmapData>();

                entity.MetaFuncs.MEStringsSelector.Unicode[] ul = sSwap.getUnicodesFromID(td.stringID.sidIndexer);

                screenData sd = ((screenData)lbScreensList.SelectedItem);
                // Default to first
                string s = ul.Length > 0 ? ul[0].unicode : string.Empty;
                for (int i = 0; i < ul.Length; i++)
                {
                    foreach (entity.MetaFuncs.MEStringsSelector.Unicode uni in sd.strings)
                        if (ul[i].position == uni.position)
                        {
                            s = ul[i].unicode;
                            i = ul.Length;
                            break;
                        }
                }


                // Create bitmap from Text
                if (s != string.Empty)
                {
                    bitmapData bd = Skin.createText(map, td, s, true);
                    if (bd != null)
                        lbd.Add(bd);
                }
                
                td.link = lbd;
            }

            #endregion

            // Transfer List Box Data to text boxes, etc
            showListBoxData();

            // link holds an array of last selected bitmap #s for each pane
            if (screenSelected.link != null)
            {
                int[] paneBitmapSelections = (int[])screenSelected.link;
                lbBitmaps.SelectedIndex = paneBitmapSelections[screenSelected.offset];
            }
            else
                lbBitmaps.SelectedIndex = 0;
            
        }

        private void lbScreensList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbScreensList.Focused)
                writePaneToMemory();

            currentScreen = (screenData)lbScreensList.SelectedItem;

            currentScreen.panes.Clear();

            br = new BinaryReader(currentScreen.meta.MS);

            br.BaseStream.Position = 32;    // Panes reflexive
            int panesCount = br.ReadInt32();
            int panesOffset = br.ReadInt32() - map.SecondaryMagic;
            int metaOffset = currentScreen.meta.offset;
            Meta m = currentScreen.meta;

            BinaryReader br2 = br;
            #region Handle pointers to other tags
            int panesTag = map.Functions.ForMeta.FindMetaByOffset(panesOffset);
            if (panesTag != -1 && panesTag != currentScreen.meta.TagIndex)
            {
                m = Map.GetMetaFromTagIndex(panesTag, map, false, true);
                br2 = new BinaryReader(m.MS);
                metaOffset = m.offset;
            }
            #endregion

            for (int panes = 0; panes < panesCount; panes++)
            {
                paneData pd = new paneData(panes);
                pd.meta = m;
                pd.offset = panesOffset - metaOffset + panes * 76;  // Store panes offset for quicker access
                pd.title = panes.ToString() + ": ";

                br2.BaseStream.Position = panesOffset - metaOffset + panes * 76 + 28;  // Text Block
                int textCount = br2.ReadInt32();
                int textOffset = br2.ReadInt32() - map.SecondaryMagic;

                if (textCount > 0)
                {
                    BinaryReader br3 = br2;

                    // Handle pointers to other tags
                    int textTag = map.Functions.ForMeta.FindMetaByOffset(textOffset);
                    if (textTag != currentScreen.meta.TagIndex)
                    {
                        Meta newMeta = Map.GetMetaFromTagIndex(textTag, map, false, true);
                        br3 = new BinaryReader(newMeta.MS);
                        textOffset -= newMeta.offset;
                    }
                    else
                        textOffset -= currentScreen.meta.offset;

                    br3.BaseStream.Position = textOffset + 36;  // Text Block
                    short SIDNum = br3.ReadInt16();
                    byte unknown = br3.ReadByte();
                    byte SIDLength = br3.ReadByte();
                    string name = map.Strings.Name[SIDNum];
                    if (name.Length == SIDLength && name.Length != 0)
                        pd.title += name;
                }
                else
                    pd.title += lbScreensList.SelectedItem.ToString();
                currentScreen.panes.Add(pd);
            }
            
            lbPanes.DataSource = currentScreen.panes;
            // Actually update ListBox with data source
            ((CurrencyManager)lbPanes.BindingContext[lbPanes.DataSource]).Refresh();

            if (lbPanes.Items.Count > 0)
            {
                lbPanes.SelectedIndex = -1; // force it to reload, even if same pane is selected
                lbPanes.SelectedIndex = ((baseData)lbScreensList.SelectedItem).offset;
            }
        }

        private void lbTBTextBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbTBTextBlocks.SelectedItem == null)
            {
                tbTBLeft.Enabled = false;
                tbTBRight.Enabled = false;
                tbTBTop.Enabled = false;
                tbTBBottom.Enabled = false;
                gbTBFlags.Enabled = false;
                return;
            }
            else
            {
                tbTBLeft.Enabled = true;
                tbTBRight.Enabled = true;
                tbTBTop.Enabled = true;
                tbTBBottom.Enabled = true;
                gbTBFlags.Enabled = true;
            }

            textBlockData td = (textBlockData)lbTBTextBlocks.SelectedItem;

            tbTBLeft.Text = td.left.ToString();
            tbTBRight.Text = td.right.ToString();
            tbTBTop.Text = td.top.ToString();
            tbTBBottom.Text = td.bottom.ToString();
            cbTBLeftAlign.Checked = td.leftAligned;
            cbTBRightAlign.Checked = td.rightAligned;
            cbTBPulsing.Checked = td.pulsing;
            cbTBTinyText.Checked = td.tinyText;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    // Grab selected bitmap
                    if (lbBitmaps.SelectedIndex > 0)
                    {
                        bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
                        currentDragBitmap = lbBitmaps.SelectedIndex;
                        currentDragStart = new Point(e.X - int.Parse(tbXPos.Text), (e.Y + int.Parse(tbYPos.Text)));
                    }
                    break;
                case 1:                    
                    if (((paneData)lbPanes.SelectedItem).listBlocks.Count > 0)
                    {
                        listBlockData lbd = ((paneData)lbPanes.SelectedItem).listBlocks[0];
                        currentDragBitmap = 0;
                        currentDragStart = new Point(e.X - lbd.left, (e.Y + lbd.bottom));
                    }
                    break;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentDragBitmap >= 0)
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        if (currentDragBitmap > 0)  // Ignore <Null> entry
                        {
                            // If selected bitmap is "grabbed" move it
                            bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
                            tbXPos.Text = (e.X - currentDragStart.X).ToString();
                            tbYPos.Text = (currentDragStart.Y - e.Y).ToString();
                        }
                        break;
                    case 1:
                        if (((paneData)lbPanes.SelectedItem).listBlocks.Count > 0)
                        {
                            tbLBLeft.Text = (e.X - currentDragStart.X).ToString();
                            tbLBBottom.Text = (currentDragStart.Y - e.Y).ToString();
                        }
                        break;
                }
            }
            else
            {
                int mx = e.X - pictureBox1.Image.Width / 2;
                int my = pictureBox1.Image.Height / 2 - e.Y;
                paneData pd = (paneData)lbPanes.SelectedItem;
                for (int i = 0; i < pd.listBlocks.Count; i++)
                {
                    int foundActive = lbPanes.SelectedIndex;
                    List<bitmapData> bds = (List<bitmapData>)pd.listBlocks[i].link;
                    for (int ii = 0; ii < bds.Count; ii++)
                    {    
                        bitmapData bd = bds[ii];
                        Bitmap b = ((Bitmap)bd.link);
                        if (mx >= bd.left && my <= bd.top &&
                            mx <= (bd.left + b.Width) &&
                            my >= (bd.top - b.Height))
                        {
                            int count = Math.Min(pd.listBlocks[0].visibleItemCount, currentScreen.strings.Count);
                            foundActive = ii * count / bds.Count;
                            break;
                        }
                    }
                    createSkin(foundActive);
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Release selected bitmap
            currentDragBitmap = -1;
        }

        private void tbPos_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int value;
            if (int.TryParse(tb.Text, out value))
            {
                bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
                if (tb == tbXPos)
                    bd.left = (short)value;
                if (tb == tbYPos)
                    bd.top = (short)value;
                if (tb == tbRenderDepth)
                    bd.renderDepth = (short)value;

                // Do not try to access list blocks if they don't exist
                if (((paneData)lbPanes.SelectedItem).listBlocks.Count > 0)
                {
                    listBlockData ld = (listBlockData)((paneData)lbPanes.SelectedItem).listBlocks[0];
                    if (tb == tbLBLeft)
                        ld.left = (short)value;
                    if (tb == tbLBBottom)
                        ld.bottom = (short)value;
                    if (tb == tbLBVisibleItemsCount)
                        ld.visibleItemCount = (short)value;
                    if (tb == tbLBAnimationIndex)
                        ld.animationIndex = (short)value;
                    if (tb == tbLBAnimationDelay)
                        ld.introAnimationDelay = (short)value;
                }
            }
        }

        private void tbScroll_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            float value;
            if (float.TryParse(tb.Text, out value))
            {
                bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
                if (tb == tbXScroll)
                    bd.horizontalWrapsPerSec = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);
                if (tb == tbYScroll)
                    bd.verticalWrapsPerSec = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tickTimer += timer1.Interval;
            if (this.Visible && map == null)
                this.Dispose();
            drawBitmap();
        }

        private void MainmenuVisualEdit_Activated(object sender, EventArgs e)
        {
            if (map == null && btnMainmenuFile.Text == string.Empty)
                this.Dispose();
            timer1.Enabled = true;
        }

        private void MainmenuVisualEdit_Deactivate(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void MainmenuVisualEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Sure you want to blah... save... blah

            // Reclaim memory
            foreach (object o in cbBitmapIdent.Items)
            {
                baseData sd = (baseData)o;
                if (sd.link != null && ((bitmapInfo)sd.link).Image != null)
                    ((Bitmap)((bitmapInfo)sd.link).Image).Dispose();
                if (sd.meta != null)
                    sd.meta.Dispose();
            }
        }

        private void MainmenuVisualEdit_Load(object sender, EventArgs e)
        {
            // Load all unicodes
            this.sSwap = new entity.MetaFuncs.MEStringsSelector(map, this);
        }
        
        #endregion

        #region Functions

        private void createSkin(int selection)
        {
            // Create the menu and save it as a Bitmap to be drawn in the drawBitmap() function

            skinData sd = (skinData)cbLBSkinIdent.SelectedItem;
            paneData pd = ((paneData)lbPanes.SelectedItem);
            screenData scrData = ((screenData)lbScreensList.SelectedItem);

            // Get trial bitmap for sizing purposes
            //Rectangle bTrial = sd.skin.getMenuSize();
            //Bitmap finalImage = new Bitmap(bTrial.Width, bTrial.Height * pd.listBlocks[0].visibleItemCount);

            List<bitmapData> lbd = new List<bitmapData>(); 

            // Skip first entry as it is used for headers (always?)
            int start = scrData.headerStringID.sidIndexer + scrData.headerStringID.length > 0 ? 1 : 0;
            if (start > 0 && currentScreen.strings.Count > 0 && pd.textBlocks.Count > 0)
            {
                string header = currentScreen.strings[0].unicode;
                textBlockData tbd = (textBlockData)pd.textBlocks[0].Clone();
                tbd.customFont = defaultHeaderFont;
                int height = 40;
                int width = 300;
                switch (scrData.shapeGroup)
                {
                    case 0:
                        //tbd.bottom = 93;
                        //tbd.left = -150;
                        tbd.left = (short)(defaultHeaderPos.Left + 18);
                        tbd.bottom = (short)(defaultHeaderPos.Bottom - 25);
                        tbd.right = (short)(tbd.left + width);
                        tbd.top = (short)(tbd.bottom + height);
                        tbd.leftAligned = true;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    default:
                        tbd.top = (short)defaultHeaderPos.Top;
                        tbd.left = (short)defaultHeaderPos.Left;
                        tbd.bottom = (short)defaultHeaderPos.Bottom;
                        tbd.right = (short)defaultHeaderPos.Right;
                        tbd.leftAligned = true;
                        break;
                }
                // Wrap text draw the text from the top down instead of bottom down (menu lists)
                lbd.Add(entity.Main.Skin.createText(map, tbd, header, true));
            }
            for (int i = 0; i < Math.Min(pd.listBlocks[0].visibleItemCount, currentScreen.strings.Count - start); i++)
            {
                // *** List by SID index, not unicode index
                List<bitmapData> bds = sd.skin.getMenuSelection(currentScreen.strings[i + start].unicode, i < selection ? 1 : i == selection ? 0 : 2);
                int height = 0;
                for (int ii = 0; ii < bds.Count; ii++)
                    if (((Bitmap)bds[ii].link).Height > height && !bds[ii].ignoreForMenuSpacing)
                        height = ((Bitmap)bds[ii].link).Height;
                foreach (bitmapData bd in bds)
                {
                    // Don't want to change original offsets!
                    bitmapData bd2 = (bitmapData)bd.Clone();
                    bd2.left += pd.listBlocks[0].left;
                    bd2.top += (short)(pd.listBlocks[0].bottom + ((Bitmap)bd2.link).Height - (height * i));
                    //bd.renderDepth = 10;
                    //bd.title = "listBlock";
                    lbd.Add(bd2);
                }
                
            }
            pd.listBlocks[0].link = lbd;
        }

        private void drawBitmap()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            if (!splitContainer1.Panel2.Enabled || map == null)
                return;
            // List used to hold all bitmaps to be processed
            List<bitmapData> bitmaps = new List<bitmapData>();
            
            #region Add backgrounds & menu items to list & sort into depth rendering order
            #region add lbBitmaps to list (background images)
            if (lbBitmaps.Items.Count > 1)
            {
                for (int i = 1; i < lbBitmaps.Items.Count; i++)
                {
                    bitmapData bd = (bitmapData)lbBitmaps.Items[i];
                    int step = 0;
                    while (step < bitmaps.Count && bd.renderDepth > bitmaps[step].renderDepth)
                        step++;
                    bitmaps.Insert(step, bd);
                }
            }
            #endregion
            paneData pd = (paneData)lbPanes.SelectedItem;
            #region add List Blocks to list (Menu Items & backdrops)
            if (pd.listBlocks.Count > 0)
                for (int i = 0; i < pd.listBlocks.Count; i++)
                {
                    List<bitmapData> bds = (List<bitmapData>)pd.listBlocks[i].link;
                    foreach (bitmapData bd in bds)
                    {                        
                        int step = 0;
                        //  renderDepth >= ( as opposed to just > ) so text takes upper priority
                        while (step < bitmaps.Count && bd.renderDepth >= bitmaps[step].renderDepth)
                            step++;
                        bitmaps.Insert(step, bd);                        
                    }
                }
            #endregion
            #region add Text Blocks to list (Information Text)
            if (pd.textBlocks.Count > 0)
                for (int i = 0; i < pd.textBlocks.Count; i++)
                {
                    List<bitmapData> bds = (List<bitmapData>)pd.textBlocks[i].link;
                    foreach (bitmapData bd in bds)
                    {
                        int step = 0;
                        //  renderDepth >= ( as opposed to just > ) so text takes upper priority
                        while (step < bitmaps.Count && bd.renderDepth >= bitmaps[step].renderDepth)
                            step++;
                        bitmaps.Insert(step, bd);
                    }
                }
            #endregion
            #endregion

            // create a bitmap to hold the combined image
            Bitmap finalImage = new Bitmap(pictureBox1.Width, pictureBox1.Height); // (1024, 512) (602, 480) ???
            
            // get a graphics object from the image so we can draw on it
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                #region Background bitmap drawing
                // Some menus have no background bitmaps
                if (bitmaps.Count > 0)
                {
                    // set background color
                    g.Clear(Color.Empty);

                    // go through each image and draw it on the final image
                    #region LoadBitmap(Meta meta);
                    foreach (bitmapData bd in bitmaps)
                    {
                        if (bd.meta == null)
                            continue;

                        Bitmap b = (Bitmap)bd.link;
                        if (b == null)
                            continue;
                        int xOff = (int)(-bd.horizontalWrapsPerSec * tickTimer) % b.Width;
                        int yOff = (int)(bd.verticalWrapsPerSec * tickTimer) % b.Height;
                        xOff = xOff >= 0 ? xOff : xOff + b.Width;
                        yOff = yOff >= 0 ? yOff : yOff + b.Height;

                        
                        System.Drawing.Imaging.ImageAttributes imgAttr = new System.Drawing.Imaging.ImageAttributes();
                        imgAttr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

                        g.DrawImage(
                            b,
                            new Rectangle(
                                finalImage.Width / 2 + bd.left,
                                finalImage.Height / 2 - bd.top,
                                b.Width,
                                b.Height),
                            xOff,
                            yOff,
                            b.Width,
                            b.Height,
                            GraphicsUnit.Pixel,
                            imgAttr
                            );


                    }
                    #endregion
                }
                #endregion

                #region Bitmap outlining
                if (tabControl1.SelectedTab == tpBitmaps)
                {
                    if (lbBitmaps.SelectedIndex > 0)
                    {
                        bitmapData bd = (bitmapData)lbBitmaps.SelectedItem;
                        if (bd.link != null)
                        {
                            int origLeft = finalImage.Width / 2 + bd.left;
                            int left = origLeft > 0 ? origLeft : 1;
                            int origTop = finalImage.Height / 2 - bd.top;
                            int top = origTop > 0 ? origTop : 1;
                            int width = ((Bitmap)bd.link).Width - (left - origLeft);
                            int height = ((Bitmap)bd.link).Height - (top - origTop);
                            g.DrawRectangle(
                                new Pen(Color.Red, 2.0f),
                                new Rectangle(
                                    left,
                                    top,
                                    (width + left) < finalImage.Width ? width : finalImage.Width - left - 1,
                                    (height + top) < finalImage.Height ? height : finalImage.Height - top - 1
                                ));
                        }
                    }
                }
                #endregion

                #region List Box lines
                if (tabControl1.SelectedTab == tpListBlocks)
                {
                    if (((paneData)lbPanes.SelectedItem).listBlocks.Count > 0)
                    {
                        listBlockData ld = (listBlockData)((paneData)lbPanes.SelectedItem).listBlocks[0];
                        int startX = finalImage.Width / 2;
                        int startY = finalImage.Height / 2;
                        g.DrawLines(
                                new Pen(Color.Red, 2.0f),
                                new Point[] { 
                                    new Point(startX + ld.left, startY - ld.bottom - 100),
                                    new Point(startX + ld.left, startY - ld.bottom),
                                    new Point(startX + ld.left + 100, startY - ld.bottom)
                                });
                        g.DrawLines(
                                new Pen(Color.Red, 1.5f),
                                new Point[] { 
                                    new Point(startX + ld.left - 4, startY - ld.bottom - 40),
                                    new Point(startX + ld.left - 4, startY - ld.bottom + 4),
                                    new Point(startX + ld.left + 40, startY - ld.bottom + 4)
                                });
                    }
                }
                #endregion

                #region Text Block outlining
                if (tabControl1.SelectedTab == tpTextBlocks)
                {
                    if (lbTBTextBlocks.SelectedIndex >= 0)
                    {
                        textBlockData td = (textBlockData)lbTBTextBlocks.SelectedItem;
                        int origLeft = finalImage.Width / 2 + td.left - 1;
                        int left = origLeft > 0 ? origLeft : 1;
                        int origTop = finalImage.Height / 2 - td.top - 1;
                        int top = origTop > 0 ? origTop : 1;
                        int width = finalImage.Width / 2 + td.right - (left) + 2;
                        int height = finalImage.Height / 2 - td.bottom - (top - origTop) + 2;
                        g.DrawRectangle(
                            new Pen(Color.Red, 2.0f),
                            new Rectangle(
                                left,
                                top,
                                (width + left) < finalImage.Width ? width : finalImage.Width - left - 1,
                                (height + top) < finalImage.Height ? height : finalImage.Height - top - 1
                            ));
                    }
                }
                #endregion
                g.Dispose();
            }
            pictureBox1.Image = finalImage;

            // Perform garbage collection or memory usage can quickly raise to > 1Gb until collection is performed
            GC.Collect();
        }

        private bool loadMainMenuData(string mainmenuFileName)
        {
            this.Cursor = Cursors.WaitCursor;
            if (map != null)
            {
                map.CloseMap();
                map = null;
            }
            map = Map.LoadFromFile(mainmenuFileName);
            if (map == null)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("Load failed! Map not found or inaccessible.\n" + mainmenuFileName);
                return false;
            }

            cbBitmapIdent.DataSource = null;
            cbBitmapIdent.Items.Clear();
            cbBitmapIdent.Items.Add(new baseData(0));
            ((baseData)cbBitmapIdent.Items[0]).title = "<null>";
            int orderCount = 0;
            for (int i = 0; i < map.FileNames.Name.Length; i++)
            {
                if (map.MetaInfo.TagType[i] == "bitm")
                {
                    baseData sd = new baseData(++orderCount);
                    sd.offset = i;
                    sd.title = map.FileNames.Name[i];
                    cbBitmapIdent.Items.Add(sd);
                }
            }

            Meta meta;
            
            // Get the tag index for [matg] globals\\globals
            int matgIndex = map.Functions.ForMeta.FindByNameAndTagType("matg", "globals\\globals");
            meta = Map.GetMetaFromTagIndex(matgIndex, map, false, true);
            br = new BinaryReader(meta.MS);

            br.BaseStream.Position = 272;
            int interfaceItemCount = br.ReadInt32();
            int interfaceItemOffset = br.ReadInt32() - map.SecondaryMagic - meta.offset;

            br.BaseStream.Position = interfaceItemOffset + 128;
            // Mainmenu Menus (Exists in Mainmenu.map)
            char[] wgtzMMTag = br.ReadChars(4);
            int wgtzMMIdent = br.ReadInt32();
            // Single Player Menus (Exists in Shared / SPShared.map)
            char[] wgtzSPTag = br.ReadChars(4);
            int wgtzSPIdent = br.ReadInt32();
            // Multiplayer Menus (Exists in Shared / SPShared.map)
            char[] wgtzMPTag = br.ReadChars(4);
            int wgtzMPIdent = br.ReadInt32();

            int tagIndex = -1;
            // Get the tag index for [wgtz] ui\\main_menu
            if (wgtzMMIdent != -1)
                tagIndex = (int)map.MetaInfo.identHT[wgtzMMIdent];
            else if (wgtzSPIdent != -1)
                tagIndex = (int)map.MetaInfo.identHT[wgtzSPIdent];
            else if (wgtzMPIdent != -1)
                tagIndex = (int)map.MetaInfo.identHT[wgtzMPIdent];
            //int tagIndex = map.Functions.ForMeta.FindByNameAndTagType("wgtz", "ui\\main_menu");
            if (tagIndex == -1)
            {
                map.CloseMap();
                map = null;
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("Load failed! Not a MAINMENU.MAP / SHARED.MAP / SPSHARED.MAP file.\n" + mainmenuFileName);
                return false;
            }

            // [wgzt] ui\\main_menu meta
            meta = Map.GetMetaFromTagIndex(tagIndex, map, false, true);
            br = new BinaryReader(meta.MS);

            #region Skins List Loading Section
            br.BaseStream.Position = 0;
            char[] wiglTag = br.ReadChars(4); 
            int wiglIdent = br.ReadInt32();

            // Should be "ui\ui_shared_globals" by default
            int tagNum = (int)map.MetaInfo.identHT[wiglIdent];
            Meta metaSG = Map.GetMetaFromTagIndex(tagNum, map, false, true);
            BinaryReader brSG = new BinaryReader(metaSG.MS);

            // Get the default header font number
            brSG.BaseStream.Position = 352;
            defaultHeaderFont = brSG.ReadInt16();
            
            // Get the default position of the header text
            brSG.BaseStream.Position = 376;
            short dhTop = brSG.ReadInt16();
            short dhLeft = brSG.ReadInt16();
            short dhBottom = brSG.ReadInt16();
            short dhRight = brSG.ReadInt16();
            defaultHeaderPos = new Rectangle(dhLeft, dhTop, dhRight-dhLeft, dhBottom-dhTop);

            // Get the skin data
            brSG.BaseStream.Position = 312;
            int listItemCount = brSG.ReadInt32();
            int listItemOffset = brSG.ReadInt32() - map.SecondaryMagic - metaSG.offset;

            for (int list = 0; list < listItemCount; list++)
            {
                brSG.BaseStream.Position = listItemOffset + list * 8;
                char[] skinTag = brSG.ReadChars(4);
                int skinIdent = brSG.ReadInt32();

                tagNum = (int)map.MetaInfo.identHT[skinIdent];
                string[] tagData = map.FileNames.Name[tagNum].Split('\\');

                skinData sd = new skinData();
                sd.offset = 0;
                sd.title = "[" + list.ToString("00") + "] " + tagData[tagData.Length - 1];
                sd.meta = Map.GetMetaFromTagIndex(tagNum, map, false, true);
                
                sd.skin  = new Skin(map, skinIdent);

                cbLBSkinIdent.Items.Add(sd);
            }

            #endregion

            #region Graphics Loading Section

            // Load Mainmenu screens tag
            // Offset 8 = Screen Widgets refelxive
            br.BaseStream.Position = 8;
            int screenCount = br.ReadInt32(); // Number of screen widgets
            int screenOffset = br.ReadInt32() - map.SecondaryMagic - meta.offset;

            // Release any memory from previously loaded menus (if any)
            //foreach(screenData sd in lbScreensList.Items)
            //    sd.meta.Dispose();
            //lbScreensList.Items.Clear();

            foreach (screenData sd in screens)
                sd.meta.Dispose();
            screens.Clear();

            // For each tag/ident screen widget entry in [wgzt] ui\\main_menu...
            for (int screen = 0; screen < screenCount; screen++)
            {
                // Offset 0 in each reflexive listing points to screen widget
                br.BaseStream.Position = screenOffset + screen * 8;
                char[] wgitTag = br.ReadChars(4);
                int wgitIdent = br.ReadInt32();

                // Retrieve the screen widget Tag Index & name and read in the [wigt] meta
                // These are all stored into the lbScreenList list box items
                tagNum = (int)map.MetaInfo.identHT[wgitIdent];
                string[] tagData = map.FileNames.Name[tagNum].Split('\\');
                screenData sd = new screenData(screen);
                sd.offset = 0;
                sd.title = tagData[tagData.Length - 1];
                sd.meta = Map.GetMetaFromTagIndex(tagNum, map, false, true);
                sd.meta.MS.Position = 0;
                sd.Read(new BinaryReader(sd.meta.MS));
                sd.getStrings(map);

                //lbScreensList.Items.Add(sd);
                screens.Add(sd);
            }

            //screens.Sort();
            
            lbScreensList.DataSource = screens;
            // Actually update ListBox with data source
            ((CurrencyManager)lbScreensList.BindingContext[lbScreensList.DataSource]).Refresh();
            if (lbScreensList.Items.Count > 0)
            {
                lbScreensList.SelectedIndex = 0;
                // Make sure this is called to update the Panes ListBox on reload
                lbScreensList_SelectedIndexChanged(this, null);
            }
            #endregion

            //br.Close();        

            this.Cursor = Cursors.Arrow;
            return true;
        }

        private void showListBoxData()
        {
            if (((paneData)lbPanes.SelectedItem).listBlocks.Count == 0)
            {

                return;
            }
            
            listBlockData ld = (listBlockData)((paneData)lbPanes.SelectedItem).listBlocks[0];
            // Option Flags
            cbLBButtonsLoop.Checked = ld.buttonsLoop;

            tbLBLeft.Text = ld.left.ToString();
            tbLBBottom.Text = ld.bottom.ToString();
            tbLBVisibleItemsCount.Text = ld.visibleItemCount.ToString();
            tbLBAnimationIndex.Text = ld.animationIndex.ToString();
            tbLBAnimationDelay.Text = ld.introAnimationDelay.ToString();
        }

        private void writePaneToMemory()
        {
            baseData sd = (baseData)lbPanes.SelectedItem;
            if (sd == null)
                return;

            bw = new BinaryWriter(sd.meta.MS);
            // Skip first entry, due to it being "<None>" (used for viewing screen without red bitmap outline)
            for (int bitmaps = 1; bitmaps < lbBitmaps.Items.Count; bitmaps++)
            {
                bitmapData bitmSD = (bitmapData)lbBitmaps.Items[bitmaps];
                if (bitmSD == null)  // Never should happen I think, but for safety
                    continue;

                bitmSD.Write(new BinaryWriter(sd.meta.MS));
            }
        }

        #endregion

    }
}
