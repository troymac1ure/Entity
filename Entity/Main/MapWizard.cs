// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapWizard.cs" company="">
//   
// </copyright>
// <summary>
//   The map wizard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using entity.MapForms;

    using HaloMap.Map;
    using Globals;

    /// <summary>
    /// The map wizard.
    /// </summary>
    /// <remarks></remarks>
    public partial class MapWizard : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The old name.
        /// </summary>
        private string oldName = string.Empty;

        /// <summary>
        /// The update file new map.
        /// </summary>
        //private bool updateFileNewMap;

        /// <summary>
        /// The update new map.
        /// </summary>
        private bool updateNewMap;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapWizard"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MapWizard()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The map wizard_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void MapWizard_Load(object sender, EventArgs e)
        {
            string[] fileNames;
            label1.Text = Prefs.pathCleanMaps;
            label2.Text = Prefs.pathMapFolder;
            try
            {
                fileNames = Directory.GetFiles(Prefs.pathCleanMaps, "*.map");
            }
            catch
            {
                fileNames = Directory.GetFiles(Prefs.pathMapFolder, "*.map");
            }

            for (int i = 0; i < fileNames.Length; i++)
            {
                string[] s = fileNames[i].Split('\\');
                baseMapComboBox.Items.Add(s[s.Length - 1].ToLower().Replace(".map", string.Empty));
            }

            if (baseMapComboBox.Items.Count > 0)
            {
                baseMapComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The base map combo box_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void baseMapComboBox_TextChanged(object sender, EventArgs e)
        {
            newMapFileTextBox.Text = baseMapComboBox.Text + ".map";
            newMapTextBox.Text = baseMapComboBox.Text;
        }

        /// <summary>
        /// The create map button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void createMapButton_Click(object sender, EventArgs e)
        {
            // Makes sure the filename gets the .map added to the end
            if (!this.createMapButton.Focused)
                this.createMapButton.Focus();

            Button butt = (Button)sender;
            string oldText = butt.Text;
            butt.Text = ".: Copying Map :.";
            Application.DoEvents();
            string newName = Prefs.pathMapFolder + "\\" + newMapFileTextBox.Text;
            try
            {
                File.Copy(Prefs.pathCleanMaps + "\\" + baseMapComboBox.Text + ".map", newName);
            }
            catch
            {
                if (MessageBox.Show("File exists! Overwrite?", "File Exists", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    File.Copy(Prefs.pathCleanMaps + "\\" + baseMapComboBox.Text + ".map", newName, true);
                }
                else
                {
                    butt.Text = oldText;
                    return;
                }
            }

            FileAttributes fa = File.GetAttributes(newName);
            if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(newName, fa & ~FileAttributes.ReadOnly);
            }

            // Load map into editor
            MapForm createdmapForm = (Owner as Form1).TryLoadMapForm(newName);

            // Set Map name in File
            createdmapForm.setMapName(newMapTextBox.Text);

            /*
            string s = Maps.map[((Entity.Form1)this.Owner).MapForms.Count - 1].MapHeader.scenarioPath;
            newMapTextBox.Text += '\\' + newMapTextBox.Text;
            string[] s2 = s.Split('\\');
            for (int i = s2.Length - 3; i >= 0; i--)
                newMapTextBox.Text = s2[i] + "\\" + newMapTextBox.Text;
            Maps.map[((Entity.Form1)this.Owner).MapForms.Count - 1].MapHeader.scenarioPath = newMapTextBox.Text.PadRight(64, '\0');
            */
            this.Close();
        }

        /// <summary>
        /// The new map file text box_ enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapFileTextBox_Enter(object sender, EventArgs e)
        {
            newMapFileTextBox.Text = newMapFileTextBox.Text.Replace(".map", string.Empty);
            if (newMapTextBox.Text.Replace(' ', '_') == newMapFileTextBox.Text)
            {
                updateNewMap = true;
            }
            else
            {
                updateNewMap = false;
            }
        }

        /// <summary>
        /// The new map file text box_ leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapFileTextBox_Leave(object sender, EventArgs e)
        {
            updateNewMap = false;
            newMapFileTextBox.Text += ".map";
        }

        /// <summary>
        /// The new map file text box_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapFileTextBox_TextChanged(object sender, EventArgs e)
        {
            if (updateNewMap && newMapFileTextBox.Focused)
            {
                newMapTextBox.Text = newMapFileTextBox.Text.Replace('_', ' ');
            }
        }

        /// <summary>
        /// The new map text box_ enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapTextBox_Enter(object sender, EventArgs e)
        {
            /*
            oldName = newMapTextBox.Text;
            if (newMapTextBox.Text == newMapFileTextBox.Text.Replace(".map","") || newMapFileTextBox.Text == ".map")
                updateFileNewMap = true;
            else
                updateFileNewMap = false;
            */
        }

        /// <summary>
        /// The new map text box_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The new map text box_ leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapTextBox_Leave(object sender, EventArgs e)
        {
            /*
            updateFileNewMap = false;
            if (oldName == newMapFileTextBox.Text.Replace(".map", ""))
                newMapFileTextBox.Text = newMapTextBox.Text.Substring(0,newMapFileTextBox.Text.Length-4) + ".map";
            */
        }

        /// <summary>
        /// The new map text box_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void newMapTextBox_TextChanged(object sender, EventArgs e)
        {
            // ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(" ","");
            /*
            if (updateFileNewMap && newMapTextBox.Focused)
                newMapFileTextBox.Text = newMapTextBox.Text.Replace(' ','_') + ".map";
            */
        }

        #endregion
    }
}