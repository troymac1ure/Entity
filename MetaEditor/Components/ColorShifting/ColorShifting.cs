// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorShifting.cs" company="">
//   
// </copyright>
// <summary>
//   The color shifting.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.ColorShifting
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The color shifting.
    /// </summary>
    public partial class ColorShifting : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The change text.
        /// </summary>
        private bool changeText;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorShifting"/> class.
        /// </summary>
        public ColorShifting()
        {
            InitializeComponent();
            hScrlBRed.Value = ShiftColors.StartingRed;
            hScrlBGreen.Value = ShiftColors.StartingGreen;
            hScrlBBlue.Value = ShiftColors.StartingBlue;
            txtBBlue.Text = ShiftColors.StartingBlue.ToString();
            txtBGreen.Text = ShiftColors.StartingGreen.ToString();
            txtBRed.Text = ShiftColors.StartingRed.ToString();
            hScrlBBlueToShift.Value = ShiftColors.BlueToShift;
            hScrlBGreenToShift.Value = ShiftColors.GreenToShift;
            hScrlBRedToShift.Value = ShiftColors.RedToShift;
            txtbBlueToShift.Text = ShiftColors.BlueToShift.ToString();
            txtbGreenToShift.Text = ShiftColors.GreenToShift.ToString();
            txtbRedToShift.Text = ShiftColors.RedToShift.ToString();
            SetStartingColorPreview();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set starting color preview.
        /// </summary>
        private void SetStartingColorPreview()
        {
            this.labStartingColorPreview.BackColor = Color.FromArgb(
                255, hScrlBRed.Value, hScrlBGreen.Value, hScrlBBlue.Value);
        }

        /// <summary>
        /// The update layers preview.
        /// </summary>
        private void UpdateLayersPreview()
        {
            int incrementer = 0;
            for (int counter = 18; counter > -1; counter--)
            {
                this.Controls[counter].BackColor = Color.FromArgb(
                    255,
                    ShiftColors.Shifter(incrementer, hScrlBRedToShift.Value, hScrlBRed.Value),
                    ShiftColors.Shifter(incrementer, hScrlBGreenToShift.Value, hScrlBGreen.Value),
                    ShiftColors.Shifter(incrementer, hScrlBBlueToShift.Value, hScrlBBlue.Value));
                incrementer++;
            }
        }

        /// <summary>
        /// The butt cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The butt save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttSave_Click(object sender, EventArgs e)
        {
            ShiftColors.StartingRed = this.hScrlBRed.Value;
            ShiftColors.StartingGreen = this.hScrlBGreen.Value;
            ShiftColors.StartingBlue = this.hScrlBBlue.Value;
            ShiftColors.RedToShift = this.hScrlBRedToShift.Value;
            ShiftColors.GreenToShift = this.hScrlBGreenToShift.Value;
            ShiftColors.BlueToShift = this.hScrlBBlueToShift.Value;
            ShiftColors.WritePlugin();
            this.Close();
        }

        /// <summary>
        /// The h scrl b blue to shift_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBBlueToShift_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtbBlueToShift.Text = hScrlBBlueToShift.Value.ToString();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The h scrl b blue_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBBlue_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtBBlue.Text = hScrlBBlue.Value.ToString();
            SetStartingColorPreview();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The h scrl b green to shift_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBGreenToShift_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtbGreenToShift.Text = hScrlBGreenToShift.Value.ToString();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The h scrl b green_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBGreen_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtBGreen.Text = hScrlBGreen.Value.ToString();
            SetStartingColorPreview();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The h scrl b red to shift_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBRedToShift_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtbRedToShift.Text = hScrlBRedToShift.Value.ToString();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The h scrl b red_ scroll.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void hScrlBRed_Scroll(object sender, ScrollEventArgs e)
        {
            this.changeText = false;
            txtBRed.Text = hScrlBRed.Value.ToString();
            SetStartingColorPreview();
            this.UpdateLayersPreview();
            this.changeText = true;
        }

        /// <summary>
        /// The txt b red_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void txtBRed_TextChanged(object sender, EventArgs e)
        {
            if (this.changeText == false)
            {
                return;
            }

            try
            {
                this.hScrlBRed.Value = Convert.ToInt32(txtBRed.Text);
                this.hScrlBGreen.Value = Convert.ToInt32(txtBGreen.Text);
                this.hScrlBBlue.Value = Convert.ToInt32(txtBBlue.Text);
                this.hScrlBRedToShift.Value = Convert.ToInt32(txtbRedToShift.Text);
                this.hScrlBGreenToShift.Value = Convert.ToInt32(txtbGreenToShift.Text);
                this.hScrlBBlueToShift.Value = Convert.ToInt32(txtbBlueToShift.Text);
                SetStartingColorPreview();
                this.UpdateLayersPreview();
            }
            catch
            {
            }
        }

        #endregion
    }
}