namespace MetaEditor.Components
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using global::MetaEditor.ColorShifting;

    partial class ColorWheel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private Bitmap renderBitmap = null;
        private Bitmap brightnessBitmap = null;
        private bool tracking = false;
        private Point lastMouseXY;

        private PictureBox wheelPictureBox;
        private PictureBox brightnessPictureBox;
        private PictureBox colorBox;

        //private int brightness = 100;

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wheelPictureBox = new System.Windows.Forms.PictureBox();
            this.brightnessPictureBox = new System.Windows.Forms.PictureBox();
            this.colorBox = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // wheelPictureBox
            // 
            this.wheelPictureBox.Location = new System.Drawing.Point(0, 0);
            this.wheelPictureBox.Name = "wheelPictureBox";
            this.wheelPictureBox.TabIndex = 0;
            this.wheelPictureBox.TabStop = false;
            this.wheelPictureBox.Click += new System.EventHandler(this.wheelPictureBox_Click);
            this.wheelPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.wheelPictureBox_Paint);
            this.wheelPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseUp);
            this.wheelPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseMove);
            this.wheelPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseDown);
            // 
            // brightnessPictureBox
            // 
            this.brightnessPictureBox.Location = new System.Drawing.Point(180, 11);
            this.brightnessPictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.brightnessPictureBox.Name = "brightnessPictureBox";
            this.brightnessPictureBox.Size = new Size(20, 128);
            this.brightnessPictureBox.TabIndex = 0;
            this.brightnessPictureBox.TabStop = false;
            this.brightnessPictureBox.Paint += new PaintEventHandler(brightnessPictureBox_Paint);
            this.brightnessPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseUp);
            this.brightnessPictureBox.MouseMove += new MouseEventHandler(wheelPictureBox_MouseMove);
            this.brightnessPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseDown);
            // 
            // saturationPictureBox
            // 
            this.colorBox.Location = new System.Drawing.Point(230, 21);
            this.colorBox.BorderStyle = BorderStyle.FixedSingle;
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new Size(100, 100);
            this.colorBox.TabIndex = 0;
            this.colorBox.TabStop = false;
            //this.colorBox.Click += new System.EventHandler(this.colorBox_Click);
            //this.colorBox.Paint += new PaintEventHandler(colorBox_Paint);
            //this.colorBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.colorBox_MouseUp);
            //this.colorBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.colorBox_MouseMove);
            //his.colorBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.colorBox_MouseDown);
            // 
            // ColorWheel
            // 
            this.Controls.Add(this.wheelPictureBox);
            this.Controls.Add(this.brightnessPictureBox);
            this.Controls.Add(this.colorBox);
            this.Name = "ColorWheel";
            this.ResumeLayout(false);

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
        
        
        void brightnessPictureBox_Paint(object sender, PaintEventArgs e)
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
                    cx -=  (this.Controls[cntlNum].Width / 2);
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
                        hsvColor.value = Math.Max(0, Math.Min(100,100 - (cy * 100 / this.Controls[cntlNum].Height)));
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
        #endregion
    }
}