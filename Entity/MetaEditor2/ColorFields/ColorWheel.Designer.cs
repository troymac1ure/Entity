namespace entity.MetaEditor2
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    partial class ColorWheel
    {

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
            this.components = new System.ComponentModel.Container();
            this.wheelPictureBox = new System.Windows.Forms.PictureBox();
            this.brightnessPictureBox = new System.Windows.Forms.PictureBox();
            this.colorBox = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.wheelPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // wheelPictureBox
            // 
            this.wheelPictureBox.Location = new System.Drawing.Point(3, 3);
            this.wheelPictureBox.Name = "wheelPictureBox";
            this.wheelPictureBox.Padding = new System.Windows.Forms.Padding(4);
            this.wheelPictureBox.Size = new System.Drawing.Size(160, 160);
            this.wheelPictureBox.TabIndex = 0;
            this.wheelPictureBox.TabStop = false;
            this.wheelPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseMove);
            this.wheelPictureBox.Click += new System.EventHandler(this.wheelPictureBox_Click);
            this.wheelPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseDown);
            this.wheelPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.wheelPictureBox_Paint);
            this.wheelPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseUp);
            // 
            // brightnessPictureBox
            // 
            this.brightnessPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.brightnessPictureBox.Location = new System.Drawing.Point(180, 11);
            this.brightnessPictureBox.Name = "brightnessPictureBox";
            this.brightnessPictureBox.Size = new System.Drawing.Size(20, 128);
            this.brightnessPictureBox.TabIndex = 0;
            this.brightnessPictureBox.TabStop = false;
            this.brightnessPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseMove);
            this.brightnessPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseDown);
            this.brightnessPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.brightnessPictureBox_Paint);
            this.brightnessPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.wheelPictureBox_MouseUp);
            // 
            // colorBox
            // 
            this.colorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox.Location = new System.Drawing.Point(230, 21);
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new System.Drawing.Size(100, 100);
            this.colorBox.TabIndex = 0;
            this.colorBox.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 800;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ColorWheel
            // 
            this.Controls.Add(this.wheelPictureBox);
            this.Controls.Add(this.brightnessPictureBox);
            this.Controls.Add(this.colorBox);
            this.Name = "ColorWheel";
            this.Size = new System.Drawing.Size(352, 156);
            ((System.ComponentModel.ISupportInitialize)(this.wheelPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Timer timer1;
        private System.ComponentModel.IContainer components;
    }
}