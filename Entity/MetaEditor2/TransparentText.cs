using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace entity.MetaEditorPlus
{
    class TransparentText : UserControl
    {
        // An event that clients can use to be notified whenever the
        // Text Alignment proprty changes.
        public event EventHandler TextAlignChanged;


        #region Fields
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private Point textPosition;
        private Font font = new Font("Ariel", 10, FontStyle.Bold);
        private string text;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TransparentText()
        {
        }

        #region Text paramter in design mode
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        [Description("Text which appears in control")]
        public override string Text { get { return text; } set { text = value; } }
        #endregion
        
        #region TextAlign paramter in design mode
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("TextAlign")]
        [Description("Determines the position of the text within the transparent text control")]
        public ContentAlignment TextAlign
        { 
            get { return textAlign; } 
            set { 
                this.textAlign = value;
                if (this.TextAlignChanged != null) 
                    this.TextAlignChanged(this, null);
                updateTextPosition();
            }
        }
        #endregion

        private void updateTextPosition()
        {
            Graphics graphics = this.CreateGraphics(); 
            SizeF textSize = graphics.MeasureString(this.text, this.font);
            graphics.Dispose();
            switch (this.textAlign)
            {
                case ContentAlignment.TopLeft:
                    textPosition.X = 0;
                    textPosition.Y = 0;
                    break;
                case ContentAlignment.TopCenter:
                    textPosition.X = (int)((this.Width / 2) - (textSize.Width / 2));
                    textPosition.Y = 0;
                    break;
                case ContentAlignment.TopRight:
                    textPosition.X = (int)(this.Width - textSize.Width);
                    textPosition.Y = 0;
                    break;

                case ContentAlignment.MiddleLeft:
                    textPosition.X = 0;
                    textPosition.Y = (int)((this.Height / 2) - (textSize.Height / 2));
                    break;
                case ContentAlignment.MiddleCenter:
                    textPosition.X = (int)((this.Width / 2) - (textSize.Width / 2));
                    textPosition.Y = (int)((this.Height / 2) - (textSize.Height / 2));
                    break;
                case ContentAlignment.MiddleRight:
                    textPosition.X = (int)(this.Width - textSize.Width);
                    textPosition.Y = (int)((this.Height / 2) - (textSize.Height / 2));
                    break;

                case ContentAlignment.BottomLeft:
                    textPosition.X = 0;
                    textPosition.Y = (int)(this.Height - textSize.Height);
                    break;
                case ContentAlignment.BottomCenter:
                    textPosition.X = (int)((this.Width / 2) - (textSize.Width / 2));
                    textPosition.Y = (int)(this.Height - textSize.Height);
                    break;
                case ContentAlignment.BottomRight:
                    textPosition.X = (int)(this.Width - textSize.Width);
                    textPosition.Y = (int)(this.Height - textSize.Height);
                    break;
            }
        }


        #region Transparent background
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        private void InvalidateEx()
        {
            if (Parent != null)
                Parent.Invalidate(Bounds, false);
            else
                Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // base.OnPaintBackground(e);
        }
        #endregion


        protected override void OnPaint(PaintEventArgs e)
        {
            updateTextPosition();
            /*
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            */
            e.Graphics.DrawString(Text, font, Brushes.DarkGray, textPosition.X + 1, textPosition.Y + 2); //Shadow
            e.Graphics.DrawString(Text, font, Brushes.Black, textPosition.X, textPosition.Y);
        }
    }
}
