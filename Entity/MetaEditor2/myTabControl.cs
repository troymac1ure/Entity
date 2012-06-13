using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace entity.MetaEditorPlus
{
    public partial class myTabControl : Form
    {
        public myTabControl()
        {
            InitializeComponent();
        }

        /*
        public void Add(myTab tab)
        {
            panel2.Controls.Add(tab);
        }
        */

        private void myTabControl_Load(object sender, EventArgs e)
        {
            myTab mt = new myTab();
            mt.Dock = DockStyle.Right;
            mt.Text = "DynamicTab";
            panel2.Controls.Add(mt);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // base.OnPaint(e);
            
            // Paint each control, leaving the active tab till last            
            Control topControl = null;
            foreach (Control c in panel2.Controls)
            {
                if (c is myTab)
                    if (this.ActiveControl != c)
                        c.Update();
                    else
                        topControl = c;

            }
            if (topControl != null)
                topControl.Update();

            /*
            System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Pen pen;

            Rectangle tabBounds = new Rectangle(20, 100, 140, 60);
            float tenthWidth = tabBounds.Width / 10.0f;

            pen = new System.Drawing.Pen(System.Drawing.Color.Yellow);
            Rectangle arc1 = new Rectangle(tabBounds.X, tabBounds.Y, (int)(2 * tenthWidth), tabBounds.Height);
            Rectangle arc2 = new Rectangle(tabBounds.X + (int)(8 * tenthWidth), tabBounds.Y, (int)(2 * tenthWidth), tabBounds.Height);
            graphics.DrawRectangle(pen, arc1);
            graphics.DrawRectangle(pen, arc2);
            //graphics.DrawRectangle(pen, 179, 100, 40, 120);

            pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            pen.Width = 1;
            //pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

            gp.AddArc(arc1, 180.0f, 90.0f);
            gp.AddLine(tabBounds.X + (int)(1 * tenthWidth), tabBounds.Y, tabBounds.X + (int)(9 * tenthWidth), tabBounds.Y);
            gp.AddArc(arc2, 270.0f, 90.0f);
            gp.AddLine(tabBounds.X, tabBounds.Y + tabBounds.Height / 2, tabBounds.X + tabBounds.Width, tabBounds.Y + tabBounds.Height / 2);
            gp.CloseFigure();
            System.Drawing.Drawing2D.PathGradientBrush pgb = new System.Drawing.Drawing2D.PathGradientBrush(gp);
            //pgb.CenterColor = Color.FromArgb(255, 220, 220, 255);
            pgb.CenterPoint = new PointF(tabBounds.X / 2, tabBounds.Y / 2);
            pgb.SurroundColors = new Color[] {
                Color.FromArgb(255, 220,220,235),
                Color.FromArgb(255, 140,140,140)
            };
            graphics.FillPath(pgb, gp);
            graphics.DrawPath(pen, gp);
            //                                                   179, 100, 40, 120)
            //graphics.FillRegion(Brushes.DarkGreen, new Region(new Rectangle(20,40,120,80)));
            pen.Dispose();
            graphics.Dispose();
            */
        }
    }
}
