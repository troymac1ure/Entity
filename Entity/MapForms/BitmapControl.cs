// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapControl.cs" company="">
//   
// </copyright>
// <summary>
//   The bitmap control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using entity.Main;

    using HaloMap;
    using HaloMap.DDSFunctions;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    /// <summary>
    /// The bitmap control.
    /// </summary>
    /// <remarks></remarks>
    public partial class BitmapControl : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The map number.
        /// </summary>
        public Map map;

        /// <summary>
        /// The pm.
        /// </summary>
        public ParsedBitmap pm;

        /// <summary>
        /// The bitm ptr.
        /// </summary>
        private IntPtr bitmPtr = IntPtr.Zero; // Data pointer for the "editor" bitmap

        /// <summary>
        /// The currently selected bitmap.
        /// </summary>
        private int currBitm;

        /// <summary>
        /// The currently selected chunk.
        /// </summary>
        private int currChunk = 0;

        /// <summary>
        /// The currently selected mipmap.
        /// </summary>
        private int currMipMap;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapControl"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public BitmapControl(Map map)
        {
            InitializeComponent();
            this.map = map;

            map.OpenMap(MapTypes.Internal);

            pm = new ParsedBitmap(ref map.SelectedMeta, map);

            for (int x = 0; x < pm.Properties.Length; x++)
            {
                TreeNode tn = new TreeNode("Bitmap #" + x);
                tn.Tag = "BITMAP";

                for (int i = 0; i < map.SelectedMeta.raw.rawChunks.Count; i++)
                {
                    int count = pm.Properties[x].mipMapCount;
                    BitmapRawDataChunk bmRaw = (BitmapRawDataChunk)map.SelectedMeta.raw.rawChunks[i];
                    if (bmRaw.inchunk == x)
                    {
                        TreeNode chunknode = new TreeNode("Chunk #" + bmRaw.num);
                        chunknode.Tag = "CHUNK";

                        int chunkWidth = pm.Properties[x].width >> bmRaw.num;
                        int mipcount = 0;
                        while (chunkWidth > 1 && count > 0)
                        {
                            TreeNode mipnode = new TreeNode("Mipmap #" + (mipcount++));
                            mipnode.Tag = "MIPMAP";
                            chunknode.Nodes.Add(mipnode);
                            chunkWidth >>= 1;
                            count--;
                        }

                        tn.Nodes.Add(chunknode);
                    }
                }

                treeView1.Nodes.Add(tn);
            }

            map.CloseMap();

            DisplayBitmap(0, 0, 0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The display bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="mipmap">The mipmap.</param>
        /// <remarks></remarks>
        public void DisplayBitmap(int bitmap, int chunk, int mipmap)
        {
            if (bitmPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(bitmPtr);
            }

            pictureBox1.Image = pm.FindChunkAndDecode(
                bitmap, chunk, mipmap, ref map.SelectedMeta, map, chunk, 0);

            ParsedBitmap.BitmapInfo bmInfo = new ParsedBitmap.BitmapInfo(
                pm.Properties[0].formatname, pm.Properties[0].swizzle);

            ////////////// Remove this. For testing Decoding/Encoding ///////////////
            // if (!pm.Properties[0].formatname.ToString().ToUpper().Contains("DXT"))
            // pictureBox1.Image = DDS_Convert.DecodeDDS(DDS_Convert.EncodeDDS((Bitmap)pictureBox1.Image, ref bmInfo), bmInfo);
            pictureBox1.Width = (pictureBox1.Image.Width * trackBar1.Value) / 2;
            pictureBox1.Height = (pictureBox1.Image.Height * trackBar1.Value) / 2;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// The refresh.
        /// </summary>
        /// <remarks></remarks>
        public void RefreshDisplay()
        {
            DisplayBitmap(currBitm, currChunk, currMipMap);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes panel background color when left / right clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void panel1_Click(object sender, EventArgs e)
        {
            Color[] colors = new Color[7] { Color.Black, Color.Red, Color.Blue, Color.SteelBlue, 
                                            Color.Yellow, Color.White, Color.Gray};
            for (int i = 0; i < colors.Length; i++)
                if (this.BackColor == colors[i])
                {
                    if (((MouseEventArgs)e).Button == MouseButtons.Left)
                    {
                        i = i == 0 ? colors.Length - 1 : i-1;
                        this.BackColor = colors[i];
                        break;
                    }
                    else if (((MouseEventArgs)e).Button == MouseButtons.Right)
                    {
                        i++;
                        i %= colors.Length;
                        this.BackColor = colors[i];
                        break;
                    }                        
                }
        }

        /// <summary>
        /// The picture box 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Use a temp or it can get an oversize error.
            int temp = trackBar1.Value;
            temp += 2;
            if (temp > 6)
            {
                temp = 2;
            }

            trackBar1.Value = temp;
            DisplayBitmap(currBitm, currChunk, currMipMap);
        }

        /// <summary>
        /// The track bar 1_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.label1.Text = (trackBar1.Value / 2.0f).ToString("0.0'x'");
            DisplayBitmap(currBitm, currChunk, currMipMap);
        }

        /// <summary>
        /// The tree view 1_ after select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Level == 2)
            {
                currBitm = treeView1.SelectedNode.Parent.Parent.Index;
                currChunk = treeView1.SelectedNode.Parent.Index;
                currMipMap = treeView1.SelectedNode.Index;
            }
            else if (treeView1.SelectedNode.Level == 1)
            {
                currBitm = treeView1.SelectedNode.Parent.Index;
                currChunk = treeView1.SelectedNode.Index;
                currMipMap = 0;
            }
            else
            {
                currBitm = treeView1.SelectedNode.Index;
                currChunk = 0;
                currMipMap = 0;
            }

            DisplayBitmap(currBitm, currChunk, currMipMap);

            // Wow. That's alot of parents. o SHOULD be type MapForm
            // object o = this.Parent.Parent.Parent.Parent.Parent.Parent;
            object o = ((Form)this.TopLevelControl).ActiveMdiChild;
            if (o is MapForm)
            {
                MapForm mf = (MapForm)o;
                Meta meta = map.SelectedMeta;
                ParsedBitmap pm = new ParsedBitmap(ref meta, map);
                //if (mf.bitmMainPtr != IntPtr.Zero)
                //{
                //    Marshal.FreeHGlobal(mf.bitmMainPtr);
                //}

                Bitmap b = pm.FindChunkAndDecode(currBitm, currChunk, currMipMap, ref meta, map, 0, 0);
                mf.pictureBox = (Bitmap)pictureBox1.Image;
                mf.statusBarText = (Math.Max(pm.Properties[currBitm].width >> currChunk >> currMipMap,1)).ToString().PadLeft(4) + " X " +
                                   (Math.Max(pm.Properties[currBitm].height >> currChunk >> currMipMap,1)).ToString().PadRight(4) + "X " +
                                   (Math.Max(pm.Properties[currBitm].depth >> currChunk >> currMipMap,1)).ToString().PadRight(4) + " " +
                                   ("(" + pm.Properties[currBitm].typename.ToString().Remove(0, 10) + ") ").PadRight(10) +
                                   pm.Properties[currBitm].formatname.ToString().Remove(0, 12).PadRight(10) + " - Swizzle:" +
                                   pm.Properties[0].swizzle + "- Location: " + meta.raw.rawChunks[0].rawLocation;
            }
        }

        #endregion
    }
}