// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MEStringsSelector.cs" company="">
//   
// </copyright>
// <summary>
//   The me strings selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Forms
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// The me strings selector.
    /// </summary>
    /// <remarks></remarks>
    public partial class MEStringsSelector : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The _selected id.
        /// </summary>
        private int _selectedID;

        /// <summary>
        /// The _selected index.
        /// </summary>
        private int _selectedIndex;

        /// <summary>
        /// The list box update.
        /// </summary>
        private bool listBoxUpdate = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MEStringsSelector"/> class.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <remarks></remarks>
        public MEStringsSelector(string[] names)
        {
            InitializeComponent();

            // KeyPreview = true;
            for (int i = 0; i < names.Length; i++)
            {
                dataGridView1.Rows.Add(new object[] { names[i], i });
            }

            dataGridView1.Rows[dataGridView1.RowCount - 1].Height = 0;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        #endregion

        #region Properties

        /// <summary>
        /// SelectedIndex
        /// </summary>
        /// <value>The selected ID.</value>
        /// <remarks></remarks>
        public int SelectedID
        {
            get
            {
                return _selectedID;
            }

            set
            {
                setPosFromID(value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get id from string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The get id from string.</returns>
        /// <remarks></remarks>
        public string getIDFromString(string s)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1[0, i].Value.ToString() == s)
                {
                    return dataGridView1[1, i].Value.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// The get string from id.
        /// </summary>
        /// <param name="ID">The id.</param>
        /// <returns>The get string from id.</returns>
        /// <remarks></remarks>
        public string getStringFromID(int ID)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1[1, i].Value.ToString() == ID.ToString())
                {
                    return dataGridView1[0, ID].ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// The set pos from id.
        /// </summary>
        /// <param name="ID">The id.</param>
        /// <remarks></remarks>
        public void setPosFromID(int ID)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1[1, i].Value.ToString() == ID.ToString())
                {
                    dataGridView1.CurrentCell = dataGridView1[0, i];
                    _selectedID = int.Parse(dataGridView1[1, i].Value.ToString());
                    _selectedIndex = i;
                    return;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The me strings selector_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MEStringsSelector_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            listBox1.Items.Clear();
            for (int i = 0; i < dataGridView1.RowCount - 2; i++)
            {
                listBox1.Items.Add(dataGridView1[0, i].Value.ToString());
            }

            listBox1.SelectedIndex = _selectedIndex;
            listBox1.TopIndex = _selectedIndex;
            textBox1.Text = string.Empty;
            textBox1.Focus();
        }

        /// <summary>
        /// The check box 1_ check state changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            listBoxUpdate = true;
        }

        /// <summary>
        /// The check string contains.
        /// </summary>
        /// <param name="newSel">The new sel.</param>
        /// <remarks></remarks>
        private void checkStringContains(string newSel)
        {
            listBox1.SuspendLayout();
            if (textBox1.Text == string.Empty)
            {
                listBox1.Items.Clear();
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    listBox1.Items.Add(dataGridView1[0, i].Value.ToString());
                    if (listBox1.Items[listBox1.Items.Count - 1].ToString() == newSel)
                    {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }
                }
            }
            else
            {
                listBox1.Items.Clear();
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    string s = dataGridView1[0, i].Value.ToString();
                    if (s.Contains(textBox1.Text))
                    {
                        listBox1.Items.Add(s);
                        if (s == newSel)
                        {
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }
                    }
                }
            }

            listBox1.ResumeLayout();
        }

        /// <summary>
        /// The check string start.
        /// </summary>
        /// <param name="newSel">The new sel.</param>
        /// <remarks></remarks>
        private void checkStringStart(string newSel)
        {
            listBox1.SuspendLayout();
            if (textBox1.Text == string.Empty)
            {
                listBox1.Items.Clear();
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    listBox1.Items.Add(dataGridView1[0, i].Value.ToString());
                    if (listBox1.Items[listBox1.Items.Count - 1].ToString() == newSel)
                    {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }
                }
            }
            else
            {
                listBox1.Items.Clear();
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    string s = dataGridView1[0, i].Value.ToString();
                    if (s.StartsWith(textBox1.Text))
                    {
                        listBox1.Items.Add(s);
                        if (s == newSel)
                        {
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }
                    }
                }
            }

            listBox1.ResumeLayout();
        }

        /// <summary>
        /// The list box 1_ double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string s = listBox1.Items[listBox1.SelectedIndex].ToString();
            _selectedID = int.Parse(getIDFromString(s));
            this.Close();
        }

        /// <summary>
        /// The list box 1_ key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox1.Focus();
        }

        /// <summary>
        /// The text box 1_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int listHeight = listBox1.Height / listBox1.ItemHeight;
            switch (e.KeyCode)
            {
                case Keys.Home:
                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.SelectedIndex = 0;
                    }

                    break;
                case Keys.PageUp:
                    if (listBox1.Items.Count > 0)
                    {
                        if (listBox1.SelectedIndex > listHeight)
                        {
                            listBox1.SelectedIndex -= listHeight;
                        }
                        else
                        {
                            listBox1.SelectedIndex = 0;
                        }
                    }

                    break;
                case Keys.Up:
                    if (listBox1.SelectedIndex > 0)
                    {
                        listBox1.SelectedIndex -= 1;
                    }

                    break;
                case Keys.Down:

                    // if (listBox1.Items.Count > 0)
                    if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                    {
                        listBox1.SelectedIndex += 1;
                    }

                    break;
                case Keys.PageDown:

                    // if (listBox1.Items.Count > 0)
                    if (listBox1.SelectedIndex + listHeight < listBox1.Items.Count - 1)
                    {
                        listBox1.SelectedIndex += listHeight;
                    }
                    else
                    {
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }

                    break;
                case Keys.End:

                    // if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    break;
                case Keys.Enter:
                    listBox1_DoubleClick(sender, new EventArgs());
                    break;
            }
        }

        /// <summary>
        /// The text box 1_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            listBoxUpdate = true;
            timer1.Interval = 700;
            timer1.Enabled = true;
        }

        /// <summary>
        /// The timer 1_ tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (listBox1.Focused)
            {
                textBox1.Focus();
            }

            if (!listBoxUpdate)
            {
                return;
            }

            timer1.Stop();
            listBoxUpdate = false;
            string oldSel = null;
            if (listBox1.SelectedIndex != -1)
            {
                oldSel = listBox1.Items[listBox1.SelectedIndex].ToString();
            }

            if (checkBox1.CheckState == CheckState.Checked)
            {
                checkStringContains(oldSel);
            }
            else
            {
                checkStringStart(oldSel);
            }

            if (listBox1.SelectedIndex == -1 && listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }

            timer1.Enabled = true;
        }

        #endregion
    }
}