// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkinOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The skin options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Globals;

    /// <summary>
    /// The skin options.
    /// </summary>
    /// <remarks></remarks>
    public partial class SkinOptions : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The mainform.
        /// </summary>
        public Form1 mainform;

        /// <summary>
        /// The skinshit.
        /// </summary>
        public SkinInfo[] skinshit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinOptions"/> class.
        /// </summary>
        /// <remarks></remarks>
        public SkinOptions()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The generate skin sets.
        /// </summary>
        /// <remarks></remarks>
        public void GenerateSkinSets()
        {
            skinshit = new SkinInfo[100];

            DirectoryInfo di = new DirectoryInfo(Global.StartupPath + "\\Skins");
            try
            {
                FileInfo[] diar1 = di.GetFiles("*.esf");
                int i = 0;
                foreach (FileInfo dra in diar1)
                {
                    skinshit[i] = teststuff(dra.ToString());
                    i = i + 1;
                }

                for (int j = 0; j < i; j++)
                {
                    comboBox1.Items.Add(skinshit[j].skin_name);
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Error finding skins or skin directory.", ex);
                this.Close();
            }
        }

        /// <summary>
        /// The get settings.
        /// </summary>
        /// <remarks></remarks>
        public void GetSettings()
        {
            if (ReadSkinInfo().Use_Skin != "true")
            {
                comboBox1.Enabled = false;
                checkBox1.Checked = false;
            }
            else
            {
                comboBox1.Enabled = true;
                checkBox1.Checked = true;
                int index = -1;
                for (int i = 0; i < skinshit.Length; i++)
                {
                    if (skinshit[i].skin_Path == ReadSkinInfo().Skin_Path)
                    {
                        index = i;
                        break;
                    }
                }

                comboBox1.SelectedIndex = index;
            }
        }

        /// <summary>
        /// The read skin info.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public SkinINFO2User ReadSkinInfo()
        {
            SkinINFO2User si;

            StreamReader sr = new StreamReader(Global.StartupPath + "\\Skins\\Settings.xml");
            XmlTextReader xr = new XmlTextReader(sr);
            XmlDocument settingsxml = new XmlDocument();
            settingsxml.Load(xr);
            XmlNodeList FormSettingsXmlNode = settingsxml.SelectNodes("skin/settings");

            // Use Skin
            XmlNode SkinInfo = FormSettingsXmlNode.Item(0).SelectSingleNode("Use_Skin");
            si.Use_Skin = SkinInfo.InnerText;

            // Skin Path
            SkinInfo = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Path");
            si.Skin_Path = SkinInfo.InnerText;
            sr.Close();

            return si;
        }

        /// <summary>
        /// The teststuff.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SkinInfo teststuff(string filepath)
        {
            SkinInfo si;

            StreamReader sr = new StreamReader(Global.StartupPath + "\\Skins\\" + filepath);
            si.skin_Path = filepath;
            XmlTextReader xr = new XmlTextReader(sr);
            XmlDocument settingsxml = new XmlDocument();
            settingsxml.Load(xr);
            XmlNodeList FormSettingsXmlNode = settingsxml.SelectNodes("skin/settings");

            // Name
            XmlNode SkinInfo = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Name");
            si.skin_name = SkinInfo.InnerText;

            // Author
            SkinInfo = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Author");
            si.skin_Author = SkinInfo.InnerText;

            // Version
            SkinInfo = FormSettingsXmlNode.Item(0).SelectSingleNode("Skin_Version");
            si.skin_Version = SkinInfo.InnerText;
            sr.Close();

            return si;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The skin options_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SkinOptions_Load(object sender, EventArgs e)
        {
            GenerateSkinSets();
            if (comboBox1.Items.Count > 0)
                GetSettings();
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            string path = Global.StartupPath + "\\skins\\Settings.xml";
            TextWriter tw;
            tw = new StreamWriter(path, false);
            tw.WriteLine("<skin>");
            tw.WriteLine("  <settings>");
            if (checkBox1.Checked)
            {
                int index = -1;
                for (int i = 0; i < skinshit.Length; i++)
                {
                    if (skinshit[i].skin_name == comboBox1.Text)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    tw.WriteLine("    <Use_Skin>true</Use_Skin>");
                    tw.WriteLine("    <Skin_Path>" + skinshit[index].skin_Path + "</Skin_Path>");
                }
            }
            else
            {
                tw.WriteLine("    <Use_Skin>false</Use_Skin>");
                tw.WriteLine("    <Skin_Path>Default</Skin_Path>");
            }

            tw.WriteLine("  </settings>");
            tw.WriteLine("</skin>");

            tw.Close();

            mainform.ReloadSkins();

            MessageBox.Show("Saved!");
            this.Close();
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The check box 1_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
            }
        }

        /// <summary>
        /// The combo box 1_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = -1;
            for (int i = 0; i < skinshit.Length; i++)
            {
                if (skinshit[i].skin_name == comboBox1.Text)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                label4.Text = skinshit[index].skin_name;
                label5.Text = skinshit[index].skin_Author;
                label6.Text = skinshit[index].skin_Version;
            }
        }

        #endregion

        /// <summary>
        /// The skin inf o 2 user.
        /// </summary>
        /// <remarks></remarks>
        public struct SkinINFO2User
        {
            #region Constants and Fields

            /// <summary>
            /// The skin_ path.
            /// </summary>
            public string Skin_Path;

            /// <summary>
            /// The use_ skin.
            /// </summary>
            public string Use_Skin;

            #endregion
        }

        /// <summary>
        /// The skin info.
        /// </summary>
        /// <remarks></remarks>
        public struct SkinInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The skin_ author.
            /// </summary>
            public string skin_Author;

            /// <summary>
            /// The skin_ path.
            /// </summary>
            public string skin_Path;

            /// <summary>
            /// The skin_ version.
            /// </summary>
            public string skin_Version;

            /// <summary>
            /// The skin_name.
            /// </summary>
            public string skin_name;

            #endregion
        }
    }
}