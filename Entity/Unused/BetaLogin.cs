// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BetaLogin.cs" company="">
//   
// </copyright>
// <summary>
//   The beta login.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Unused
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The beta login.
    /// </summary>
    /// <remarks></remarks>
    partial class BetaLogin : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BetaLogin"/> class.
        /// </summary>
        /// <remarks></remarks>
        public BetaLogin()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The login site.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="pass">The pass.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public StreamReader LoginSite(string username, string pass)
        {
            string text1 = "http://haloplugins.com/betalogin.php";
            HttpWebRequest request1 = (HttpWebRequest)(WebRequest.Create(text1));
            request1.Method = "POST";
            request1.ContentType = "application/x-www-form-urlencoded";
            string[] textArray3 = new[]
                {
                   "username=" + username + "&password=" + pass + "&macaddress=" + FillNetworkAdapters() 
                };
            string text2 = string.Concat(textArray3);
            ASCIIEncoding encoding1 = new ASCIIEncoding();
            byte[] buffer1 = encoding1.GetBytes(text2);
            Stream stream1 = request1.GetRequestStream();
            stream1.Write(buffer1, 0, buffer1.Length);
            stream1.Close();
            HttpWebResponse response1 = (HttpWebResponse)(request1.GetResponse());
            StreamReader reader1 = new StreamReader(response1.GetResponseStream());
            return reader1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The fill network adapters.
        /// </summary>
        /// <returns>The fill network adapters.</returns>
        /// <remarks></remarks>
        private string FillNetworkAdapters()
        {
            string returnstring = string.Empty;
            ManagementObjectCollection collection1 =
                new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
            foreach (ManagementObject obj1 in collection1)
            {
                if ((bool)obj1["IPEnabled"])
                {
                    string text1 = obj1["Caption"].ToString().Substring(11);
                    returnstring = GetMACAddress(text1);
                    break;
                }
            }

            return returnstring;
        }

        /// <summary>
        /// The get mac address.
        /// </summary>
        /// <param name="Adapter">The adapter.</param>
        /// <returns>The get mac address.</returns>
        /// <remarks></remarks>
        private string GetMACAddress(string Adapter)
        {
            string text1 = string.Empty;
            ManagementObjectCollection collection1 =
                new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
            foreach (ManagementObject obj1 in collection1)
            {
                if ((bool)obj1["IPEnabled"])
                {
                    string text2 = obj1["Caption"].ToString().Substring(11);
                    if (text2 == Adapter)
                    {
                        text1 = obj1["MacAddress"].ToString();
                    }
                }
            }

            return text1;
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeComponent()
        {
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.textBox2 = new TextBox();
            this.textBox1 = new TextBox();
            this.button1 = new Button();
            this.SuspendLayout();

            // label3
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, (0));
            this.label3.Location = new Point(26, 20);
            this.label3.Name = "label3";
            this.label3.Size = new Size(279, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Please login with your entity beta user information.";

            // label2
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, (0));
            this.label2.Location = new Point(58, 84);
            this.label2.Name = "label2";
            this.label2.Size = new Size(73, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Password:";

            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, (0));
            this.label1.Location = new Point(58, 58);
            this.label1.Name = "label1";
            this.label1.Size = new Size(77, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Username:";

            // textBox2
            this.textBox2.Location = new Point(136, 83);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new Size(136, 20);
            this.textBox2.TabIndex = 8;

            // textBox1
            this.textBox1.Location = new Point(136, 57);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(136, 20);
            this.textBox1.TabIndex = 7;

            // button1
            this.button1.Location = new Point(109, 123);
            this.button1.Name = "button1";
            this.button1.Size = new Size(113, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += this.button1_Click_1;

            // BetaLogin
            this.BackColor = Color.SteelBlue;
            this.ClientSize = new Size(331, 164);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "BetaLogin";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// The button 1_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click_1(object sender, EventArgs e)
        {
            string specialstring = LoginSite(textBox1.Text, textBox2.Text).ReadToEnd();
            if (specialstring == "0")
            {
                MessageBox.Show("Sorry but you could not be logged in.");
                Application.Exit();
            }
            else if (specialstring == "1")
            {
                MessageBox.Show("You are now logged in.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Sorry but there was a problem with the login system.");

                // MessageBox.Show(specialstring);
                Application.Exit();
            }
        }

        #endregion
    }
}