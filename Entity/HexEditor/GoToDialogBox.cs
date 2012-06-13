// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoToDialogBox.cs" company="">
//   
// </copyright>
// <summary>
//   The go to dialog box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.HexEditor
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Globals;

    /// <summary>
    /// The go to dialog box.
    /// </summary>
    /// <remarks></remarks>
    public partial class GoToDialogBox : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The offset.
        /// </summary>
        private int offset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GoToDialogBox"/> class.
        /// </summary>
        /// <remarks></remarks>
        public GoToDialogBox()
        {
            InitializeComponent();
            maskedTextBox1.Text = Offset.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Offset.
        /// </summary>
        /// <remarks></remarks>
        public int Offset
        {
            get
            {
                return offset;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (decimalbutton.Checked)
                {
                    this.offset = int.Parse(maskedTextBox1.Text);
                }
                else
                {
                    this.offset = int.Parse(maskedTextBox1.Text, NumberStyles.HexNumber);
                }

                this.offset = this.offset / 10 * 10;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Invalid Entry", ex);
                return;
            }

            this.Hide();
        }

        /// <summary>
        /// The decimalbutton_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void decimalbutton_CheckedChanged(object sender, EventArgs e)
        {
            decimalbutton.Checked = true;
            hexadecimalbutton.Checked = false;
            try
            {
                maskedTextBox1.Text = int.Parse(maskedTextBox1.Text, NumberStyles.HexNumber).ToString();
            }
            catch
            {
            }
        }

        /// <summary>
        /// The hexadecimalbutton_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void hexadecimalbutton_CheckedChanged(object sender, EventArgs e)
        {
            decimalbutton.Checked = false;
            hexadecimalbutton.Checked = true;
            try
            {
                maskedTextBox1.Text = int.Parse(maskedTextBox1.Text).ToString("X");
            }
            catch
            {
            }
        }

        #endregion
    }
}