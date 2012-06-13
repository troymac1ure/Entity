// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HexRichTextBox.cs" company="">
//   
// </copyright>
// <summary>
//   The hex rich text box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.HexEditor
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    /// <summary>
    /// The hex rich text box.
    /// </summary>
    /// <remarks></remarks>
    public class HexRichTextBox : RichTextBox
    {
        #region Constants and Fields

        /// <summary>
        /// The _ paint.
        /// </summary>
        public static bool _Paint = true;

        /// <summary>
        /// The columns.
        /// </summary>
        public int Columns;

        /// <summary>
        /// The offset.
        /// </summary>
        public int Offset;

        /// <summary>
        /// The rows.
        /// </summary>
        public int Rows;

        /// <summary>
        /// The w m_ paint.
        /// </summary>
        private const short WM_PAINT = 0x00f;

        /// <summary>
        /// The hexbytes.
        /// </summary>
        private byte[] hexbytes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HexRichTextBox"/> class.
        /// </summary>
        /// <remarks></remarks>
        public HexRichTextBox()
        {
            this.Rows = 19;
            this.Columns = 16;

            this.ReadOnly = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ByteArray.
        /// </summary>
        /// <value>The byte array.</value>
        /// <remarks></remarks>
        public byte[] ByteArray
        {
            get
            {
                return hexbytes;
            }

            set
            {
                hexbytes = value;
                LoadHex(0);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load hex.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <remarks></remarks>
        public void LoadHex(int offset)
        {
            _Paint = false;
            this.Offset = offset;
            int tempstart = this.SelectionStart;
            int templength = this.SelectionLength;
            Color c = this.SelectionColor;
            this.SelectAll();
            
            this.SelectionColor = Color.Black;

            string temp = string.Empty;
            try
            {
                for (int counter = 0; counter < this.Rows; counter++)
                {
                    for (int counter2 = 0; counter2 < this.Columns; counter2++)
                    {
                        temp += this.ByteArray[offset + counter2].ToString("X").PadLeft(2, '0') + " ";
                    }

                    offset += this.Columns;
                }
            }
            catch
            {
            }

            this.Text = temp;
            this.Select(tempstart, templength);
            this.SelectionColor = c;
            _Paint = true;
            Application.DoEvents();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on key down.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            int val = e.KeyValue;
            string tempchar = string.Empty;
            try
            {
                tempchar = this.Text.Substring(this.SelectionStart, 1);
            }
            catch
            {
            }

            if (tempchar == " ")
            {
                // this.SelectionLength += 1;
                this.SelectionStart += 1;
                tempchar = this.Text.Substring(this.SelectionStart, 1);
                int keyindex = (int)Enum.Parse(typeof(Keys), tempchar);
                val = keyindex;
            }

            int zeroindex = (int)Keys.D0;
            int nineindex = (int)Keys.D9;
            int aindex = (int)Keys.A;
            int findex = (int)Keys.F;
            if ((e.KeyValue >= zeroindex && e.KeyValue <= nineindex) || (e.KeyValue >= aindex && e.KeyValue <= findex))
            {
                int index = this.Offset + (this.SelectionStart / 3);
                Keys k = (Keys)e.KeyValue;
                string s = Enum.GetName(typeof(Keys), k);
                byte b;
                if (s.Length == 2)
                {
                    s = s[1].ToString();
                }

                int o;
                int r = Math.DivRem(this.SelectionStart, 3, out o);
                if (o == 0)
                {
                    string tempchar2 = this.Text.Substring(this.SelectionStart + 1, 1);
                    s = s + tempchar2;
                }
                else
                {
                    string tempchar2 = this.Text.Substring(this.SelectionStart - 1, 1);
                    s = tempchar2 + s;
                }

                b = byte.Parse(s, NumberStyles.HexNumber);
                hexbytes[index] = b;
                this.SelectionStart += 1;
                this.SelectionLength = 0;
                LoadHex(this.Offset);
            }
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        protected override void WndProc(ref Message m)
        {
            // Code courtesy of Mark Mihevc
            // sometimes we want to eat the paint message so we don't have to see all the
            // flicker from when we select the text to change the color.
            if (m.Msg == WM_PAINT)
            {
                if (_Paint)
                {
                    base.WndProc(ref m); // if we decided to paint this control, just call the RichTextBox WndProc
                }
                else
                {
                    m.Result = IntPtr.Zero;
                }

                // not painting, must set this to IntPtr.Zero if not painting otherwise serious problems.
            }
            else
            {
                base.WndProc(ref m); // message other than WM_PAINT, jsut do what you normally do.
            }
        }

        #endregion
    }
}