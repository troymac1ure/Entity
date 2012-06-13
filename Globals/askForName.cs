// --------------------------------------------------------------------------------------------------------------------
// <copyright file="askForName.cs" company="">
//   
// </copyright>
// <summary>
//   The ask for name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Globals
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The ask for name.
    /// </summary>
    /// <remarks></remarks>
    public partial class askForName : Form
    {
        // public delegate void giveButtonState(TextBox text);
        #region Constants and Fields

        /// <summary>
        /// The tag.
        /// </summary>
        private string tag;
        /// <summary>
        /// Keeps track of the new tag name before changes for allowing of auto-tag changing
        /// </summary>
        private string previousTagName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="askForName"/> class.
        /// </summary>
        /// <param name="titleText">The text to display on the title bar.</param>
        /// <param name="labelText">The text to display on the label.</param>
        /// <param name="buttonText">The text to display on the button.</param>
        /// <param name="oldTagName">The tag's original name (path included).</param>
        /// <param name="newTagName">The new name of the tag (without path)</param>
        /// <param name="smartChange">if set to True, will update the top label as a tag when the bottom label is changed</param>
        /// <remarks></remarks>
        public askForName(string titleText, string labelText, string buttonText, string oldTagName, string newTagName, bool smartChange)
        {
            InitializeComponent();
            this.Text = titleText;
            this.label1.Text = labelText;
            this.createButton.Text = buttonText;
            this.tbOldName.Text = oldTagName;
            this.tbNewName.Text = newTagName;
            tag = newTagName;
            this.tbNewName.Left = this.label1.Width + 20;
            this.tbNewName.Width = this.Width - (this.tbNewName.Left + 20);
            if (smartChange)
            {
                tbOldName.Enabled = true;
                tbNewName.TextChanged += new EventHandler(tbNewName_TextChanged);
            }
            this.previousTagName = newTagName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get text box value.
        /// </summary>
        /// <returns>The get text box value.</returns>
        /// <remarks></remarks>
        public string getTextBoxValue()
        {
            if (tbOldName.Enabled)
                return tbOldName.Text;
            else
                return tbNewName.Text;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ask for name_ form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void askForName_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// The create button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void createButton_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The name box_ leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void nameBox_Leave(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The name box_ text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void nameBox_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// When the askForMapName dialog text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbNewName_TextChanged(object sender, EventArgs e)
        {
            string[] sa = tbOldName.Text.Split('\\');
            string temp = string.Empty;
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i] == previousTagName)
                    temp += tbNewName.Text;
                else
                    temp += sa[i];
                if (i < sa.Length - 1) temp += '\\';
            }
            tbOldName.Text = temp;
            previousTagName = tbNewName.Text;
        }
        #endregion
    }
}