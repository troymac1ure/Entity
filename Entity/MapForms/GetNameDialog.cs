// --------------------------------------------------------------------------------------------------------------------
// <copyright file="getNameDialog.cs" company="">
//   
// </copyright>
// <summary>
//   The get name dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The get name dialog.
    /// </summary>
    /// <remarks></remarks>
    internal partial class GetNameDialog : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="getNameDialog"/> class.
        /// </summary>
        /// <param name="formName">Name of the form.</param>
        /// <param name="label">The label.</param>
        /// <param name="name">The name.</param>
        /// <param name="buttonName">Name of the button.</param>
        /// <remarks></remarks>
        public GetNameDialog(string formName, string label, string name, string buttonName)
        {
            InitializeComponent();
            this.Text = formName;
            label1.Text = label;
            textBox1.Text = name;
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = textBox1.Text.Length;
            button1.Text = buttonName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="formName">The form name.</param>
        /// <param name="label">The label.</param>
        /// <param name="name">The name.</param>
        /// <param name="buttonName">The button name.</param>
        /// <returns>The show.</returns>
        /// <remarks></remarks>
        public static string Show(string formName, string label, string name, string buttonName)
        {
            GetNameDialog temp = new GetNameDialog(formName, label, name, buttonName);
            temp.ShowDialog();
            name = temp.textBox1.Text;
            temp.Dispose();
            return name;
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
            this.Close();
        }

        #endregion
    }
}