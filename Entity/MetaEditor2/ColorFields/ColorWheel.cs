// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorWheel.cs" company="">
//   
// </copyright>
// <summary>
//   The color wheel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaEditor2
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The color wheel.
    /// </summary>
    public partial class ColorWheel : BaseField
    {
        #region Constants and Fields

        /// <summary>
        /// The ent name.
        /// </summary>
        public string EntName = "Error in getting plugin element name";

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber;

        /// <summary>
        /// The value.
        /// </summary>
        public object Value;

        /// <summary>
        /// The alpha text.
        /// </summary>
        public TextBox alphaText;

        /// <summary>
        /// The blue text.
        /// </summary>
        public TextBox blueText;

        /// <summary>
        /// The chunk offset.
        /// </summary>
        public int chunkOffset;

        /// <summary>
        /// The color.
        /// </summary>
        public ColorHandler.RGB color = new ColorHandler.RGB(0, 0, 0);

        /// <summary>
        /// The green text.
        /// </summary>
        public TextBox greenText;

        /// <summary>
        /// The offset in map.
        /// </summary>
        public int offsetInMap;

        /// <summary>
        /// The red text.
        /// </summary>
        public TextBox redText;

        /// <summary>
        /// The color tesselation.
        /// </summary>
        private const int colorTesselation = 60;

        /// <summary>
        /// The hsv brightness.
        /// </summary>
        //private ColorHandler.HSV hsvBrightness;

        /// <summary>
        /// The hsv color.
        /// </summary>
        private ColorHandler.HSV hsvColor;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        /// <summary>
        /// The mapindex.
        /// </summary>
        private Map mapindex;

        /// <summary>
        /// Set to true when the color wheel is updating the text boxes so that the text boxes
        /// do not try to update the color wheel
        /// </summary>
        private bool updatingColors;

        #endregion

        /*
        private HsvColor hsvColor;
        public HsvColor HsvColor
        {
            get
            {
                return hsvColor;
            }

            set
            {
                if (hsvColor != value)
                {
                    HsvColor oldColor = hsvColor;
                    hsvColor = value;
                    this.OnColorChanged();
                    Refresh();
                }
            }
        }
        */
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWheel"/> class.
        /// </summary>
        public ColorWheel()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.AutoSize = false;

            // hsvColor = new ColorHandler.HSV(0, 0, brightness);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWheel"/> class.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        public ColorWheel(int r, int g, int b)
        {
            color = new ColorHandler.RGB(r, g, b);
            InitializeComponent();
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.AutoSize = false;

            // hsvColor = new ColorHandler.HSV(0, 0, brightness);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets HSVColor.
        /// </summary>
        public ColorHandler.HSV HSVColor
        {
            get
            {
                return hsvColor;
            }

            set
            {
                if (hsvColor.Hue != value.Hue & hsvColor.Saturation != value.Saturation & hsvColor.value != value.value)
                {
                    ColorHandler.HSV oldColor = hsvColor;
                    hsvColor = value;
                    this.OnColorChanged();

                    // Refresh();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The check for color.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="cntl">
        /// The cntl.
        /// </param>
        /// <param name="search1">
        /// The search 1.
        /// </param>
        /// <param name="search2">
        /// The search 2.
        /// </param>
        /// <returns>
        /// The check for color.
        /// </returns>
        public static bool checkForColor(string name, DataValues cntl, string search1, string search2)
        {
            if (name == null)
            {
                return false;
            }

            name = name.ToLower();
            search1 = search1.ToLower();
            int i = name.IndexOf(search1) + search1.Length;
            if (((name == search1.Replace(" ", string.Empty)) || name.Contains(search2.ToLower())) & (cntl == null))
            {
                return true;
            }

            int start = -1;
            if (name.Contains(search1))
            {
                do
                {
                    start = name.IndexOf(search1, start + 1);
                    if (name.Length == (start + search1.Length) || name[start + search1.Length] == ' ')
                    {
                        return true;
                    }
                }
                while (start != -1);
            }

            return false;
        }

        /// <summary>
        /// The set text box.
        /// </summary>
        /// <param name="tb">
        /// The tb.
        /// </param>
        /// <param name="col">
        /// The col.
        /// </param>
        public void setTextBox(TextBox tb, Color col)
        {
            if (col == Color.White)
            {
                alphaText = tb;
                alphaText.Tag = Color.White;
                alphaText.TextChanged += Control_TextChanged;
                alphaText.LostFocus += Control_LostFocus;
            }
            else if (col == Color.Red)
            {
                redText = tb;
                redText.Tag = Color.Red;
                redText.TextChanged += Control_TextChanged;
                redText.LostFocus += Control_LostFocus;
            }
            else if (col == Color.Green)
            {
                greenText = tb;
                greenText.Tag = Color.Green;
                greenText.TextChanged += Control_TextChanged;
                greenText.LostFocus += Control_LostFocus;
            }
            else if (col == Color.Blue)
            {
                blueText = tb;
                blueText.Tag = Color.Blue;
                blueText.TextChanged += Control_TextChanged;
                blueText.LostFocus += Control_LostFocus;
            }
        }

        /// <summary>
        /// The update bar.
        /// </summary>
        public void updateBar()
        {
            if (brightnessBitmap == null)
            {
                return;
            }

            using (Graphics g1 = Graphics.FromImage(brightnessBitmap))
            {
                g1.Clear(this.BackColor);
                DrawBar(g1, brightnessBitmap.Width, brightnessBitmap.Height);
            }

            updateBox();
        }

        /// <summary>
        /// The update box.
        /// </summary>
        public void updateBox()
        {
            colorBox.BackColor = ColorHandler.HSVtoColor(hsvColor);
            ColorHandler.RGB tempRGB = ColorHandler.HSVtoRGB(hsvColor);
            updatingColors = true;
            if (alphaText != null)
            {
                alphaText.Text = (this.color.Alpha / 255f).ToString();
            }

            redText.Text = (tempRGB.Red / 255f).ToString();
            greenText.Text = (tempRGB.Green / 255f).ToString();
            blueText.Text = (tempRGB.Blue / 255f).ToString();
            updatingColors = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get circle points.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="center">
        /// The center.
        /// </param>
        /// <returns>
        /// </returns>
        private static PointF[] GetCirclePoints(float r, PointF center)
        {
            PointF[] points = new PointF[colorTesselation];

            for (int i = 0; i < colorTesselation; i++)
            {
                float theta = (i / (float)colorTesselation) * 2 * (float)Math.PI;
                points[i] = SphericalToCartesian(r, theta);
                points[i].X += center.X;
                points[i].Y += center.Y;
            }

            return points;
        }

        /// <summary>
        /// The spherical to cartesian.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="theta">
        /// The theta.
        /// </param>
        /// <returns>
        /// </returns>
        private static PointF SphericalToCartesian(float r, float theta)
        {
            float x;
            float y;

            x = r * (float)Math.Cos(theta);
            y = r * (float)Math.Sin(theta);

            return new PointF(x, y);
        }

        /// <summary>
        /// The control_ lost focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Control_LostFocus(object sender, EventArgs e)
        {
            // In case they leave the text box before the timer has expired, call the
            // timer1_tick to update the color wheel with the new data.
            if (timer1.Enabled) 
                timer1_Tick(sender, e);
            hsvColor = ColorHandler.RGBtoHSV(this.color);
            updateBar();
            this.Refresh();
        }

        /// <summary>
        /// Called when a (ARGB) textbox is changed. Starts a timer, then updates the
        /// colorwheel after the timer expires (allowing for typing)
        /// </summary>
        /// <param name="sender">The (ARGB) textbox that sent the request</param>
        /// <param name="e"></param>
        private void Control_TextChanged(object sender, EventArgs e)
        {
            if (updatingColors)
            {
                return;
            }
            timer1.Tag = sender;
            if (((Control)sender).Focused)
            {
                timer1.Start();
            }
            else
            {
                timer1_Tick(null, null);
            }
        }

        /// <summary>
        /// The draw bar.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        private void DrawBar(Graphics g, int width, int height)
        {
            PointF[] points = new PointF[2];

            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = width / 2;
                points[i].Y = i * height;
            }

            ColorHandler.HSV col = new ColorHandler.HSV(hsvColor.Hue, hsvColor.Saturation, 100);

            using (
                LinearGradientBrush lgb = new LinearGradientBrush(
                    points[0], points[1], ColorHandler.HSVtoColor(col), Color.Black))
            {
                g.FillRectangle(lgb, 0, 0, width, height);
            }
        }

        /// <summary>
        /// The draw wheel.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        private void DrawWheel(Graphics g, int width, int height)
        {
            float radius = ComputeRadius(new Size(width, height));
            PointF[] points = GetCirclePoints(Math.Max(1.0f, radius - 1), new PointF(radius, radius));

            using (PathGradientBrush pgb = new PathGradientBrush(points))
            {
                pgb.CenterColor = Color.White;
                pgb.CenterPoint = new PointF(radius, radius);
                pgb.SurroundColors = GetColors();

                g.FillEllipse(pgb, 0, 0, radius * 2, radius * 2);
            }
        }

        /// <summary>
        /// The get colors.
        /// </summary>
        /// <returns>
        /// </returns>
        private Color[] GetColors()
        {
            Color[] colors = new Color[colorTesselation];

            for (int i = 0; i < colorTesselation; i++)
            {
                int hue = (i * 360) / colorTesselation;
                colors[i] = ColorHandler.HSVtoColor(new ColorHandler.HSV(hue, 100, 100));
            }

            return colors;
        }

        /// <summary>
        /// The init render surface.
        /// </summary>
        private void InitRenderSurface()
        {
            if (renderBitmap != null)
            {
                renderBitmap.Dispose();
            }

            int wheelDiameter = (int)ComputeDiameter(Size);

            renderBitmap = new Bitmap(
                Math.Max(1, (wheelDiameter * 4) / 3), Math.Max(1, (wheelDiameter * 4) / 3), PixelFormat.Format24bppRgb);

            using (Graphics g1 = Graphics.FromImage(renderBitmap))
            {
                g1.Clear(this.BackColor);
                DrawWheel(g1, renderBitmap.Width, renderBitmap.Height);
            }

            if (brightnessBitmap != null)
            {
                brightnessBitmap.Dispose();
            }

            brightnessBitmap = new Bitmap(
                Math.Max(1, brightnessPictureBox.Width), 
                Math.Max(1, brightnessPictureBox.Height), 
                PixelFormat.Format24bppRgb);

            updateBar();
        }

        /// <summary>
        /// The init rendering.
        /// </summary>
        private void InitRendering()
        {
            if (this.renderBitmap == null)
            {
                InitRenderSurface();
                this.wheelPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                int size = (int)Math.Ceiling(ComputeDiameter(this.Size));
                this.wheelPictureBox.Size = new Size(size, size);
                this.wheelPictureBox.Image = this.renderBitmap;

                this.brightnessPictureBox.Image = this.brightnessBitmap;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            InitRendering();
            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            InitRendering();
            base.OnPaint(e);
        }

        private void wheelPictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            float radius = ComputeRadius(Size);
            float theta = ((float)HSVColor.Hue / 360.0f) * 2.0f * (float)Math.PI;
            float alpha = ((float)HSVColor.Saturation / 100.0f);
            float x = (alpha * (radius - 1) * (float)Math.Cos(theta)) + radius;
            float y = (alpha * (radius - 1) * (float)Math.Sin(theta)) + radius;
            int ix = (int)x;
            int iy = (int)y;

            // Draw the 'target rectangle'
            GraphicsContainer container = e.Graphics.BeginContainer();
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(Pens.Black, ix - 1, iy - 1, 3, 3);
            e.Graphics.DrawRectangle(Pens.White, ix, iy, 1, 1);
            e.Graphics.EndContainer(container);
        }

        private void wheelPictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //OnMouseMove(sender, e);
            base.OnMouseMove(e);

            lastMouseXY = new Point(e.X, e.Y);

            if (tracking)
            {
                GrabColor(sender, new Point(e.X, e.Y));
            }
        }

        private void wheelPictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //OnMouseUp(sender, e);
            base.OnMouseUp(e);

            if (tracking)
            {
                GrabColor(sender, new Point(e.X, e.Y));
            }

            tracking = false;
        }

        private void wheelPictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void wheelPictureBox_Click(object sender, System.EventArgs e)
        {
            OnClick(e);
        }


        private void brightnessPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // Draw the 'target rectangle'
            GraphicsContainer container = e.Graphics.BeginContainer();
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(Pens.White, 0, e.Graphics.VisibleClipBounds.Height - (hsvColor.value * e.Graphics.VisibleClipBounds.Height / 100) - 1, brightnessBitmap.Width - 3, 3);
            e.Graphics.EndContainer(container);
        }

        private static float ComputeRadius(Size size)
        {
            return Math.Min((float)size.Width / 2, (float)size.Height / 2);
        }

        private static float ComputeDiameter(Size size)
        {
            return Math.Min((float)size.Width, (float)size.Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (renderBitmap != null && (ComputeRadius(Size) != ComputeRadius(renderBitmap.Size)))
            {
                renderBitmap.Dispose();
                renderBitmap = null;
            }

            Invalidate();
        }

        public event EventHandler ColorChanged;
        protected virtual void OnColorChanged()
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, EventArgs.Empty);
            }
        }

        private void GrabColor(object sender, Point mouseXY)
        {
            int cntlNum = this.Controls.IndexOf((Control)sender);
            int cx = mouseXY.X;
            int cy = mouseXY.Y;

            switch (cntlNum)
            {
                case 0:
                    // center our coordinate system so the middle is (0,0), and positive Y is facing up
                    cx -= (this.Controls[cntlNum].Width / 2);
                    cy -= (this.Controls[cntlNum].Height / 2);
                    if (cx < this.Controls[cntlNum].Width / 2)
                    {
                        double theta = Math.Atan2(cy, cx);

                        if (theta < 0)
                        {
                            theta += 2 * Math.PI;
                        }

                        double alpha = Math.Sqrt((cx * cx) + (cy * cy));

                        int h = (int)((theta / (Math.PI * 2)) * 360.0);
                        int s = (int)Math.Min(100.0, (alpha / (double)(this.Controls[0].Width / 2)) * 100);
                        int v = hsvColor.value;

                        hsvColor = new ColorHandler.HSV(h, s, v);

                        OnColorChanged();
                        updateBar();
                    }
                    break;
                case 1:
                    if (cx < this.Controls[cntlNum].Width)
                    {
                        hsvColor.value = Math.Max(0, Math.Min(100, 100 - (cy * 100 / this.Controls[cntlNum].Height)));
                        updateBox();
                    }
                    break;
            }
            this.color = ColorHandler.HSVtoRGB(hsvColor);
            Invalidate(true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                tracking = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (tracking)
            {
                //GrabColor(sender, new Point(e.X, e.Y));
            }

            tracking = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            lastMouseXY = new Point(e.X, e.Y);

            if (tracking)
            {
                //GrabColor(sender, new Point(e.X, e.Y));
            }
        }

        /// <summary>
        /// The timer is used to control the color wheel update from the text boxes.
        /// If we update instantly, we are unable to type, therefore allow a delay for typing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // The tag contains the control that started the timer
            Control c = (Control)timer1.Tag;
            try
            {
                if ((Color)c.Tag == Color.White)
                {
                    this.color.Alpha = Math.Max(0, Math.Min(255, (int)(float.Parse(c.Text) * 255)));
                }

                if ((Color)c.Tag == Color.Red)
                {
                    this.color.Red = Math.Max(0, Math.Min(255, (int)(float.Parse(c.Text) * 255)));
                }

                if ((Color)c.Tag == Color.Green)
                {
                    this.color.Green = Math.Max(0, Math.Min(255, (int)(float.Parse(c.Text) * 255)));
                }

                if ((Color)c.Tag == Color.Blue)
                {
                    this.color.Blue = Math.Max(0, Math.Min(255, (int)(float.Parse(c.Text) * 255)));
                }

                this.hsvColor = ColorHandler.RGBtoHSV(this.color);
                updateBar();
            }
            catch
            {
            }
            timer1.Stop();
        }
        #endregion

    }
}