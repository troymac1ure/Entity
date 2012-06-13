using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace entity.MetaEditorPlus
{
    partial class myTab : UserControl
    {
        Color outlineColor = Color.Black;
        // BackColor not used!
        Color backColor = Color.FromArgb(255, 170, 170, 170);
        Color tabColor1 = Color.FromArgb(255, 220,220,235);
        Color tabColor2 = Color.FromArgb(255, 140, 140, 140);


        public myTab()
        {
            InitializeComponent();
            this.tabLeft.Visible = false;
            this.tabCenter.Visible = false;
            this.tabRight.Visible = false;
            this.AutoSize = false;
            this.Size = new Size(120, 40);
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        [Description("Text which appears in control")]
        public override string Text { get { return label.Text; } set { label.Text = value; } }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Drawing.ContentAlignment TextAlign { get { return this.label.TextAlign; } set { this.label.TextAlign = value; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.Graphics graphics = this.CreateGraphics();
            GraphicsPath gp = new GraphicsPath();
            GraphicsPath gpActive = new GraphicsPath();
            System.Drawing.Pen pen;

            Rectangle tabBounds = new Rectangle(0, 0, this.Width, this.Height);
            float tenthWidth = tabBounds.Width / 10.0f;

            pen = new System.Drawing.Pen(outlineColor);
            Rectangle arc1 = new Rectangle(tabBounds.X, tabBounds.Y , (int)(2 * tenthWidth), tabBounds.Height * 2);
            Rectangle arc2 = new Rectangle(tabBounds.X + (int)(8 * tenthWidth), tabBounds.Y, (int)(2 * tenthWidth), tabBounds.Height * 2);


            gp.AddArc(arc1, 180.0f, 90.0f);
            gp.AddLine(tabBounds.X + (int)(1 * tenthWidth), tabBounds.Y, tabBounds.X + (int)(9 * tenthWidth), tabBounds.Y);
            gp.AddArc(arc2, 270.0f, 90.0f);
            gpActive = new GraphicsPath(gp.PathPoints, gp.PathTypes);
            gp.AddLine(tabBounds.X, tabBounds.Y + tabBounds.Height - 1, tabBounds.X + tabBounds.Width, tabBounds.Y + tabBounds.Height - 1);
            gp.CloseFigure();

            PathGradientBrush pgb = new PathGradientBrush(gp);
            pgb.CenterPoint = new PointF(tabBounds.X / 2, tabBounds.Y / 2);
            pgb.SurroundColors = new Color[] {
                tabColor1,
                tabColor2
            };
            graphics.FillPath(pgb, gp);
            if (this.ParentForm.ActiveControl == this)
            {
                pen.Width = 2;
                graphics.DrawPath(pen, gpActive);
                pen.Brush = pgb;
                graphics.DrawLine(pen, tabBounds.X, tabBounds.Y + tabBounds.Height - 1, tabBounds.X + tabBounds.Width, tabBounds.Y + tabBounds.Height - 1);
            }
            else
            {
                pen.Width = 1;
                graphics.DrawPath(pen, gp);
            }
            pen.Dispose();
            graphics.Dispose();
        }
    }

}
