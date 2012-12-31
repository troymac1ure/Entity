using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using HaloMap.Map;
using System.Xml;

namespace entity.Main
{
    public partial class PluginUpdater : Form
    {        
        #region Classes and Types
        class FileData
        {
            // [Attribs] Link#    Owner      Group   Size(Byte)     Last mod Name
            // drwxr-xr-x   11 acemodso   acemodso         4096 Oct 18 02:04 ..

            /// <summary>
            /// The attributes of the file; we only care about the directory attribute though.
            /// </summary>
            FileAttributes _attrib;
            public FileAttributes attrib { get { return _attrib; } set { _attrib = value; } }

            /// <summary>
            /// The size of the file.
            /// </summary>
            long _size;
            public long size { get { return _size; } set { _size = value; } }

            /// <summary>
            /// The date & time stamp of the file.
            /// </summary>
            DateTime _date;
            public DateTime date { get { return _date; } set { _date = value; } }

            /// <summary>
            /// The file name.
            /// </summary>
            string _name;
            public string name { get { return _name; } set { _name = value; } }

            /// <summary>
            /// Reads the UNIX formatted data in the format:
            /// "drwxr-xr-x    1 [ owner]   [ group]        24096 Jan 01 01:59 filename.txt"
            /// </summary>
            /// <param name="data"></param>
            public FileData(string data)
            {
                Read(data);
            }

            public void Read(string data)
            {
                if (data[0] == 'd') 
                    _attrib = FileAttributes.Directory;
                else 
                    _attrib = FileAttributes.Normal;                
                _size = long.Parse(data.Substring(36, 12));
                string s = data.Substring(49, 12);
                // We need to insert the year so that the time & date are decoded properly
                _date = DateTime.Parse(data.Substring(49, 12).Insert(6, ", " + DateTime.Now.Year.ToString()));
                _name = data.Substring(62);
            }

            public override int GetHashCode()
            {
                return (int)((date.Month + date.Day + date.Hour + date.Minute) ^ size);
            }

        }

        class PluginData
        {
            public FileData fileData;
            public XmlNode xmlNode;
            public string author;
            public string version;
            public string md5;
            public string tagType;

            public PluginData(FileData filedata, XmlNode xmlnode)
            {
                fileData = filedata;                
                xmlNode = xmlnode;
                try
                {
                    this.md5 = xmlNode.Attributes.GetNamedItem("md5").Value;
                }
                catch { this.md5 = ""; }
                try
                {
                    this.tagType = xmlNode.Attributes.GetNamedItem("tagtype").Value;
                }
                catch { this.tagType = filedata.name.Split('-')[0]; }
                try
                {
                    this.author = xmlNode.Attributes.GetNamedItem("author").Value;
                }
                catch { this.author = "unknown"; }
                try
                {
                    this.version = xmlNode.Attributes.GetNamedItem("version").Value;
                }
                catch { this.version = "0.0"; }
            }

            public override int GetHashCode()
            {
                return (int)Math.Abs((fileData.GetHashCode() + author.GetHashCode()) ^ version.GetHashCode()) % 99999999;
            }

        }

        private class ProgressForm
        {
            Form formPB = new Form();
            ProgressBar pb = new ProgressBar();
            Label lb = new Label();
            public string Text { get { return lb.Text; } set { lb.Text = value; } }
            public int Value { get { return pb.Value; } set { pb.Value = value; } }

            public ProgressForm()
            {
                formPB.FormBorderStyle = FormBorderStyle.FixedSingle;
                formPB.ControlBox = false;
                formPB.ShowInTaskbar = false;
                formPB.Size = new Size(150, 80);
                formPB.TopMost = true;
                pb.Dock = DockStyle.Fill;
                lb.Dock = DockStyle.Top;
                lb.Size = new Size(150, 30);
                lb.Text = string.Empty;
                formPB.Controls.Add(pb);
                formPB.Controls.Add(lb);
            }

            public void Show()
            {
                formPB.Show();
            }

            public void Dispose()
            {
                formPB.Dispose();
            }
        }

        #endregion

        #region Constants and Fields
        /// <summary>
        /// The update ftp name.
        /// </summary>
        private static string updateFTPName = "h2misc@acemods.org";

        /// <summary>
        /// The update ftp pass.
        /// </summary>
        private static string updateFTPPass = "y,Nk+,G/ur5i";

        /// <summary>
        /// For updating plugins
        /// </summary>
        private static string pluginsFTPServer = "ftp://ftp.acemods.org/Plugins/";

        /// <summary>
        /// The Database file located on the server
        /// </summary>
        private static string pluginsFTPdbFile = pluginsFTPServer + "plugins.xml";
        /// <summary>
        /// Holds a listing of all the files on the FTP server
        /// </summary>
        List<FileData> listing = new List<FileData>();

        /// <summary>
        /// Holds the data from the FTP database file
        /// </summary>
        XmlDocument xDoc = new XmlDocument();
        
        /// <summary>
        /// Keeps track of the total number of checked boxes
        /// </summary>
        private int totalChecked = 0;        

        #endregion

        #region Constructors and Destructors
        public PluginUpdater()
        {
            InitializeComponent();

            // Show selected plugin set
            lblPluginSet.Text = Globals.PluginSetSelector.getName(Globals.Prefs.pathPluginsFolder);

            this.dgvPluginData.CurrentCellDirtyStateChanged += new EventHandler(dgvPluginData_CurrentCellDirtyStateChanged);
            this.dgvPluginData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPluginData_CellValueChanged);            

            // Download the database file & file listing from the server
            RetrieveDataFromFTPServer();

            totalChecked = 0;
            for (int i = 0; i < dgvPluginData.Rows.Count; i++)
                if (((bool)((DataGridViewCheckBoxCell)dgvPluginData.Rows[i].Cells[0]).Value) == true)
                    totalChecked++;
            lblUpdateCount.Text = totalChecked.ToString();
            // Sort by selected tags?
            //dgvPluginData.Sort(dgvPluginData.Columns[0], ListSortDirection.Descending);
        }
        #endregion

        #region Methods
        public static XmlNode RenameNode(XmlNode node, string namespaceURI, string qualifiedName)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement oldElement = (XmlElement)node;
                XmlElement newElement = node.OwnerDocument.CreateElement(qualifiedName, namespaceURI);

                while (oldElement.HasAttributes)
                {
                    newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
                }

                newElement.InnerXml = oldElement.InnerXml;
                //while (oldElement.HasChildNodes)
                //{
                //    newElement.AppendChild(oldElement.FirstChild);
                //}

                if (oldElement.ParentNode != null)
                {
                    oldElement.ParentNode.ReplaceChild(newElement, oldElement);
                }

                return newElement;
            }
            else
            {
                return null;
            }
        }

        private void RetrieveDataFromFTPServer()
        {
            FtpWebRequest ftpRequest;
            FtpWebResponse ftpResponse;

            xDoc = new XmlDocument();

            #region Attempt to download plugin database file
            try
            {
                // Create a request for the database file
                ftpRequest = (FtpWebRequest)WebRequest.Create(pluginsFTPdbFile);
                // Set default authentication for retrieving the file info
                ftpRequest.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);

                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                // Ask the server for the file size and store it
                long fileSize = ftpResponse.ContentLength;
                ftpResponse.Close();

                WebClient wc = new WebClient();
                wc.Credentials = ftpRequest.Credentials;
                xDoc.LoadXml(wc.DownloadString(pluginsFTPdbFile));
            }
            catch
            {
                xDoc.AppendChild(xDoc.CreateElement("Plugins"));
            }
            #endregion

            #region Retrieve list of all files & directories from FTP server
            try
            {
                // Create a request to the directory we are working in
                ftpRequest = (FtpWebRequest)WebRequest.Create(pluginsFTPServer);
                // Set default authentication for retrieving the file info
                ftpRequest.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(ftpResponse.GetResponseStream());
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    listing.Add(new FileData(line));
                    line = streamReader.ReadLine();
                }
                ftpResponse.Close();
            }
            catch (Exception ex)
            {
            }
            #endregion

            ProgressForm formPB = new ProgressForm();
            formPB.Show();

            HashSet<string> hashTable = new HashSet<string>();            
            bool xDocChange = false;
            foreach (FileData fd in listing)
            {
                // We only want files (non-directories)
                if ((fd.attrib | FileAttributes.Directory) != FileAttributes.Directory)
                {
                    string[] fName = fd.name.ToLower().Split('.','-');
                    string modifiedName = fName[0].Replace('!', '_').Replace('+', '_');

                    #region Update information box
                    formPB.Text = "Analyzing: \n" + fd.name;
                    formPB.Value = listing.IndexOf(fd) * 100 / listing.Count;
                    Application.DoEvents();
                    #endregion

                    if (fName[fName.Length-1] != "ent")
                        continue;

                    /*
                    if (fName.Length > 3)
                    {
                        string ver = fName[1];
                        for (int i = 2; i < fName.Length - 1; i++)
                            ver += '.' + fName[i];
                        fVer = new Version(ver);
                    }
                    */
                    
                    // Check for the matching tag HASHCODE name
                    XmlNodeList xnl = null;
                    XmlNode xNode = null;

                    if (fName.Length > 2)
                        xnl = xDoc.GetElementsByTagName("_" + fName[1]);
                    

                    // Search through each return value for the one in the desired parent TAG TYPE
                    if (xnl != null && xnl.Count > 0)
                        foreach (XmlNode xn in xnl)
                        {
                            if (xn.ParentNode.Name == modifiedName)
                            {
                                xNode = xn;
                                break;
                            }
                        }

                @updatePlugin:
                    // If we do not get any results for the HASHCODE, check for parent TAG TYPE
                    if (xNode == null)
                    {
                        // Search for a matching TAG TYPE
                        XmlNodeList xml = xDoc.GetElementsByTagName(modifiedName);

                        // No matching TAG TYPE was found, so create a tag placeholder (eg. SCNR)
                        if (xml.Count == 0)
                        {
                            xNode = xDoc.CreateElement(modifiedName);
                            xDoc.FirstChild.AppendChild(xNode);
                        }
                        // We found a matching TAG TYPE, so select it
                        else
                        {
                            xNode = xml[0];
                        }
                    
                        // Retrieve the plugin from the server & parse for needed info
                        WebClient wc = new WebClient();
                        wc.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);
                        try
                        {
                            string tempPlugin = wc.DownloadString(pluginsFTPServer + fd.name);
                            XmlDocument xd = new XmlDocument();
                            xd.LoadXml(tempPlugin);
                            XmlNodeList XList;
                            XmlNode xnA;    // Holds author attribute info
                            XmlNode xnV;    // Holds version attribute info
                            string tag;

                            try
                            {
                                XList = xd.GetElementsByTagName("revision");
                                xnA = XList[0].Attributes.GetNamedItem("author");
                                xnV = XList[0].Attributes.GetNamedItem("version");
                                try
                                {
                                    tag = XList[0].Attributes.GetNamedItem("class").Value;
                                }
                                catch
                                {
                                    tag = fName[1].ToLower();
                                }
                            }
                            catch
                            {
                                try
                                {
                                    XList = xd.GetElementsByTagName("plugin");
                                    xnA = XList[0].Attributes.GetNamedItem("author");
                                    xnV = XList[0].Attributes.GetNamedItem("version");
                                    tag = XList[0].Attributes.GetNamedItem("class").Value;
                                }
                                catch
                                {
                                    // Bad plugin?
                                    continue;
                                }
                            }

                            //
                            // Need to generate name first & will rename later
                            //
                            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();                            
                            string md5 = GetChecksum(encoding.GetBytes(tempPlugin));

                            if (fName.Length > 2)
                                xNode = xNode.AppendChild(xDoc.CreateElement("_" + fName[1]));
                            else
                                xNode = xNode.AppendChild(xDoc.CreateElement("_---"));

                            // Append author & version to xNode
                            // (This needs to be after the xNode is possibly set to xNode.Child)

                            // MD5 Hashcode
                            XmlAttribute xAttr = xDoc.CreateAttribute("md5");
                            xAttr.InnerText = md5;
                            xNode.Attributes.Append(xAttr);

                            // Author info
                            xAttr = xDoc.CreateAttribute("tagtype");
                            xAttr.InnerText = tag;
                            xNode.Attributes.Append(xAttr);

                            // Author info
                            xAttr = xDoc.CreateAttribute("author");
                            xAttr.InnerText = xnA.Value;
                            xNode.Attributes.Append(xAttr);

                            // Version info
                            xAttr = xDoc.CreateAttribute("version");
                            xAttr.InnerText = xnV.Value;
                            xNode.Attributes.Append(xAttr);

                            // States that changes need to be made to database file
                            xDocChange = true;
                        }
                        catch (Exception ex)
                        {
                            Globals.Global.ShowErrorMsg("Error from plugin server: ", ex);
                        }
                    }
                    PluginData pd = new PluginData(fd, xNode);
                    if (pd.md5 == string.Empty)
                    {
                        xNode = null;
                        goto @updatePlugin;
                    }
                    if (hashTable.Contains(pd.md5))
                    {
                        // Duplicate file found, erase file & remove entry
                        try
                        {
                            ftpRequest = (FtpWebRequest)WebRequest.Create(pluginsFTPServer + fd.name);
                            // Set default authentication for retrieving the file info
                            ftpRequest.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);

                            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                            // Ask the server for the file size and store it
                            ftpResponse.Close();
                            xDocChange = true;
                        }
                        catch
                        { }
                        continue;
                    }
                    
                    hashTable.Add(pd.md5);

                    // 1827
                    // "_fx_-87943969.ent"
                    // {27/10/2012 1:49:00 AM}
                    // "Grimdoomer"
                    // "0.99"

                    // Create a unique short hash code for this plugin using author, version & file date/time stamp
                    string hashcode = pd.GetHashCode().ToString();
                    string fileName = fName[0] + '-' + hashcode + ".ent";
                    if (fd.name.ToLower() != fileName)
                    {
                        // Rename the node to reflect the [new] hashcode
                        if (xNode.Name != "_" + hashcode)
                            xNode = RenameNode(xNode, string.Empty, '_' + hashcode);
                        #region Rename file to naming sequence
                        // Create a request to the directory we are working in
                        ftpRequest = (FtpWebRequest)WebRequest.Create(pluginsFTPServer + fd.name);
                        // Set default authentication for retrieving the file info
                        ftpRequest.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);
                        ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                        ftpRequest.RenameTo = fileName;
                        ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                        ftpResponse.Close();
                        #endregion
                        fd.name = fileName;
                    }

                    if (pd.version == "0.0")
                    {
                        int a = 0;
                    }
                    this.Add( fName[0], pd);
                }

            }
            // Clean up our loading form
            formPB.Dispose();

            if (xDocChange)
            {
                // WriteXDoc back out
                WebClient wc = new WebClient();
                wc.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);
                wc.UploadString(pluginsFTPdbFile, xDoc.InnerXml);
            }
        }

        private void Add(string name, PluginData pluginData)
        {
            string info = "<missing>";
            Version currentVer = new Version(0,0);
            try
            {
                HaloMap.Plugins.IFPIO ifpio = HaloMap.Plugins.IFPHashMap.GetIfp(name, HaloVersionEnum.Halo2);
                if (ifpio.revisions.Length > 2)
                {
                    currentVer = new Version(ifpio.revisions[1]);
                    info = currentVer + " (" + ifpio.revisions[0] + ")";
                }
                else
                {
                    currentVer = new Version(ifpio.version);
                    info = currentVer + " (" + ifpio.author + ")";
                }
            }
            catch
            {                
            }
            dgvPluginData.Rows.Add(
                currentVer < new Version(pluginData.version),
                name, 
                info, 
                pluginData.version + " (" + pluginData.author + ")", 
                pluginData.md5);
            dgvPluginData.Rows[dgvPluginData.Rows.Count - 1].Tag = pluginData;
        }

        private static string GetChecksum(byte[] buffer)
        {
            MD5 md5 = MD5.Create();
            byte[] checksum = md5.ComputeHash(buffer);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
        
        private void dgvPluginData_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0) e.Cancel = true;
        }

        private void dgvPluginData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > 0)
            {
                DataGridViewCheckBoxCell curCell = (DataGridViewCheckBoxCell)dgvPluginData[e.ColumnIndex, e.RowIndex];
                this.totalChecked += ((bool)curCell.Value ? 1 : -1);
                lblUpdateCount.Text = totalChecked.ToString();
                if ((bool)curCell.Value == true)
                {
                    // For nested looping, to not deselect our desired selection
                    dgvPluginData[0, e.RowIndex].Tag = "---";
                    string tagtype = (string)dgvPluginData[1, e.RowIndex].Value;
                    for (int i = 0; i < dgvPluginData.Rows.Count; i++)
                        if ((string)dgvPluginData[1, i].Value == tagtype && dgvPluginData[0, i].Tag == null)
                            dgvPluginData[0, i].Value = false;
                    dgvPluginData[0, e.RowIndex].Tag = null;
                }
            }
        }

        private void dgvPluginData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPluginData.IsCurrentCellDirty)
            {
                dgvPluginData.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        #endregion

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvPluginData.Rows.Count; i++)
                dgvPluginData[0, i].Value = false;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvPluginData.Rows.Count; i++)
                dgvPluginData[0, i].Value = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ProgressForm pForm = new ProgressForm();
            pForm.Show();
            dgvPluginData.Sort( dgvPluginData.Columns[0], ListSortDirection.Descending);
            int i = -1; int count = 0;
            while ((bool)dgvPluginData[0, ++i].Value == true)
            {
                PluginData pd = (PluginData)dgvPluginData.Rows[i].Tag;
                string localName = pd.tagType + ".ent";
                pForm.Text = "updating " + localName;
                pForm.Value = count++ * 100 / totalChecked;
                Application.DoEvents();

                WebClient wc = new WebClient();
                wc.Credentials = new NetworkCredential(updateFTPName, updateFTPPass);
                try
                {
                    wc.DownloadFile(
                        pluginsFTPServer + pd.fileData.name, 
                        Globals.Prefs.pathPluginsFolder + "\\" + localName
                        );
                }
                catch (Exception ex)
                {
                    Globals.Global.ShowErrorMsg("Error downloading plugin", ex);
                }
            }
            pForm.Dispose();

            HaloMap.Plugins.IFPHashMap.H1IFPHash.Clear();
            HaloMap.Plugins.IFPHashMap.H2IFPHash.Clear();
        }


    }
}
