using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.IO;

namespace entity
{
    public partial class HelpForm : Form
    {
        string defaultPath;
        /// <summary>
        /// The update ftp name.
        /// </summary>
        private static string updateFTPName = "h2misc@acemods.org";

        /// <summary>
        /// The update ftp pass.
        /// </summary>
        private static string updateFTPPass = "y,Nk+,G/ur5i";

        /// <summary>
        /// The update ftp server.
        /// </summary>
        private static string updateFTPServer = "ftp://ftp.acemods.org/Troy Mac1ures Folder (Reaper approved)/Help/";
        /// <summary>
        /// The information file containing all the file versions.
        /// </summary>
        private static string updateNFOFile = "database.xml";        
        /// <summary>
        /// Check for updates only once per opening
        /// </summary>
        private bool CheckedUpdates = false;

        private class FileInfo
        {
            public string name;
            public string md5;
            public XmlNode node;

            public FileInfo(string name, string md5, XmlNode node)
            {
                this.name = name;
                this.md5 = md5;
                this.node = node;
            }
        }


        public HelpForm()
        {
            defaultPath = Assembly.GetExecutingAssembly().Location;
            defaultPath = defaultPath.Remove(defaultPath.LastIndexOf("\\")) + "\\Help\\";
            
            // Getting lazy...
            Directory.CreateDirectory(defaultPath);
            Directory.CreateDirectory(defaultPath + "images\\");

            InitializeComponent();
            
            this.Text = "Entity " 
                + Assembly.GetExecutingAssembly().GetName().Version.Major + "."
                + Assembly.GetExecutingAssembly().GetName().Version.Minor + "."
                + Assembly.GetExecutingAssembly().GetName().Version.Build
                + " Help File";            
        }

        private void HelpForm_Activated(object sender, EventArgs e)
        {
            if (!CheckedUpdates)
            {
                checkForUpdates();
                CheckedUpdates = true;
            }

            tsIndex_Click(this, null);

            //Used to handle missing help pages (must come after a page has been loaded)
            var axWebBrowser = (SHDocVw.WebBrowser)browser.ActiveXInstance;
            axWebBrowser.NavigateError += axWebBrowser_NavigateError;
        }

        private bool checkForUpdates()
        {
            // Create a custom error page
            // Known bug is pressing the back button goes to the original DNS error page
            string updatePage =
                "<html>" +
                "<head>" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\">" +
                "<title>Checking For Help Updates</title>" +
                "</head>" +
                "<body text=\"#ffffff\" link=\"#0000ff\" vlink=\"#0000ff\" bgcolor=\"#02020a\">" +
                "<p><h1>Updating Entity Help Pages ...</h1></p>" +
                "<p></p>" +
                "<div id=\"info\" style=\"display: none\">Updating file ________.htm</div>" +
                "</body></html>";
            browser.DocumentText = updatePage;
            //"javascript:document.getElementById('info').style.display='block';";
            Application.DoEvents();

            WebClient wc = new WebClient();

            // Set our FTP Username & Password
            wc.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);

            // Download our Information File
            // Contains a list of all the latest versions of each help file
            // FORMAT: (ex.)
            //   <entityhelp>
            //      <file name="index.htm" version="1.0" size="1622" />
            //   </entityhelp>
            int count = 0;
            string netVerInfo = null;
            while (netVerInfo == null)
            {
                try
                {
                    netVerInfo = wc.DownloadString(updateFTPServer + updateNFOFile);
                }
                catch (WebException e)
                {
                    // if failure due to missing directory / file, just abort on first try
                    if (((FtpWebResponse)e.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        return false;
                    
                    // If error due to something else, give 5 chances to connect
                    count++;
                    if (count < 5)
                    {
                        System.Threading.Thread.Sleep(300);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            string updtVersion = string.Empty;
            string updtDate = string.Empty;
            string updtName = string.Empty;

            XmlDocument xd = new XmlDocument();
            try
            {
                xd.LoadXml(netVerInfo);
            }
            catch
            {
                throw new Exception("Error reading help database file on server. Updates unavailable.");
            }
            if (xd.FirstChild.Name == "entityhelp")
            {
                List<FileInfo> files = new List<FileInfo>();
                
                XmlNode xn = xd.FirstChild.FirstChild;
                while (xn != null)
                {
                    switch (xn.Name)
                    {
                        case "file":                            
                            string fname;
                            string fmd5;

                            // Must have filename
                            try
                            { fname = xn.Attributes.GetNamedItem("name").Value; }
                            catch
                            { break; }
                            
                            // md5 can be calculated and then updated for us
                            try
                            { fmd5 = xn.Attributes.GetNamedItem("md5").Value; }
                            catch
                            { fmd5 = string.Empty; }

                            files.Add(new FileInfo(fname, fmd5, xn));
                            break;
                    }

                    xn = xn.NextSibling;
                }

                bool xDocChange = false;
                foreach (FileInfo fi in files)
                {
                    if (File.Exists(defaultPath + fi.name))
                    {
                        FileStream fs = new FileStream(defaultPath + fi.name, FileMode.Open);
                        long fsize = fs.Length;
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                        string md5 = GetChecksum(data);

                        //DateTime dt = File.GetLastWriteTime(defaultPath + fi.name);
                        if (md5 != fi.md5)
                        {
                            fs.Close();
                            wc.DownloadFile(updateFTPServer + fi.name, defaultPath + fi.name);
                            
                            // Recheck md5
                            fs = new FileStream(defaultPath + fi.name, FileMode.Open);
                            data = new byte[fs.Length];
                            fs.Read(data, 0, data.Length);
                            md5 = GetChecksum(data);

                            if (fi.md5 != md5)
                            {
                                xDocChange = true;
                                XmlNode xmn = fi.node.Attributes.GetNamedItem("md5");
                                if (xmn != null)
                                    xmn.Value = md5;
                                else
                                {
                                    XmlAttribute xAttr = xd.CreateAttribute("md5");
                                    xAttr.InnerText = md5;
                                    fi.node.Attributes.Append(xAttr);
                                }
                                fi.md5 = md5;
                            }
                        }
                        fs.Close();

                    }
                }

                if (xDocChange)
                {
                    // WriteXDoc back out
                    wc.UploadString( updateFTPServer + updateNFOFile, xd.InnerXml);
                }

                return true;
            }
            return false;
        }

        private static string GetChecksum(byte[] buffer)
        {
            MD5 md5 = MD5.Create();
            byte[] checksum = md5.ComputeHash(buffer);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }

        void axWebBrowser_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            // Create a custom error page
            // Known bug is pressing the back button goes to the original DNS error page
            string errorPage =
                "<html>" +
                "<head>" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\">" +
                "<title>Missing Help Page</title>" +
                "</head>" +
                "<body text=\"#ffffff\" link=\"#0000ff\" vlink=\"#0000ff\" bgcolor=\"#02020a\">" +
                "<dir><p><h1>Error - Page not found</h1></p>" +
                "<p>The page you were directed to does not exist</p>" +
                "<p></p>" +
                "<p>" + ((string)URL) + "</p>" +
                "<p></p>" +
                "<hr align=\"right\" size=\"4\">" +
                "<p><b><font face=\"Arial\" size=\"2\" color=\"#ffffff\">[<a href=\"javascript: window.history.go(-2)\">Back</a>]</font></b></p>" +
                "</dir>" +
                "</body></html>";
            browser.DocumentText = errorPage;
        } 

        #region events 

        void browser_CanGoBackChanged(object sender, System.EventArgs e)
        {
            this.tsBack.Enabled = browser.CanGoBack;
        }

        void browser_CanGoForwardChanged(object sender, System.EventArgs e)
        {
            this.tsForward.Enabled = browser.CanGoForward;
        }

        private void tsIndex_Click(object sender, EventArgs e)
        {
            browser.Navigate(defaultPath + "index.htm");
        }

        private void HelpForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void tsBack_Click(object sender, EventArgs e)
        {
            // Due to our custom page, we need to go back twice...
            if (browser.Document.Title == "Missing Help Page")
                browser.GoBack();
            browser.GoBack();
        }

        private void tsForward_Click(object sender, EventArgs e)
        {
            browser.GoForward();
        }

        #endregion


    }
}
