// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Download Manager.cs" company="">
//   
// </copyright>
// <summary>
//   The download_ manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Unused
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows.Forms;

    using entity.Main;
    using entity.MapForms;

    using Globals;

    // For Update
    // For Net Access & Threading

    /// <summary>
    /// The download_ manager.
    /// </summary>
    /// <remarks></remarks>
    public partial class Download_Manager : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The is busy.
        /// </summary>
        private static readonly object isBusy = new object();

        // The progress of the download in percentage

        /// <summary>
        /// The _main form.
        /// </summary>
        private readonly Form1 _mainForm;

        /// <summary>
        /// The dest file.
        /// </summary>
        private readonly List<string> destFile = new List<string>();

        /// <summary>
        /// The file done.
        /// </summary>
        private readonly List<int> fileDone = new List<int>();

        /// <summary>
        /// The source url.
        /// </summary>
        private readonly List<string> sourceURL = new List<string>();

        /// <summary>
        /// The percent progress.
        /// </summary>
        private static int PercentProgress;

        /// <summary>
        /// The access credentials.
        /// </summary>
        private NetworkCredential accessCredentials = new NetworkCredential();

        /// <summary>
        /// The current file.
        /// </summary>
        private int currentFile;

        // The request to the ftp server for file information
        /// <summary>
        /// The ftp request.
        /// </summary>
        private FtpWebRequest ftpRequest;

        // The response from the ftp server containing information about the file
        /// <summary>
        /// The ftp response.
        /// </summary>
        private FtpWebResponse ftpResponse;

        /// <summary>
        /// The set cancel.
        /// </summary>
        private bool setCancel;

        // The stream of data that we write to the harddrive
        /// <summary>
        /// The str local.
        /// </summary>
        private Stream strLocal;

        // The stream of data retrieved from the web server
        /// <summary>
        /// The str response.
        /// </summary>
        private Stream strResponse;

        // The thread inside which the download happens
        /// <summary>
        /// The thr download.
        /// </summary>
        private Thread thrDownload;

        // The request to the web server for file information
        /// <summary>
        /// The web request.
        /// </summary>
        private HttpWebRequest webRequest;

        // The response from the web server containing information about the file
        /// <summary>
        /// The web response.
        /// </summary>
        private HttpWebResponse webResponse;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Download_Manager"/> class.
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        /// <remarks></remarks>
        public Download_Manager(Form1 mainForm)
        {
            InitializeComponent();
            this._mainForm = mainForm;
            timer.Start();
            this.Show();
        }

        #endregion

        // The delegate which we will call from the thread to update the form
        #region Delegates

        /// <summary>
        /// The update progess callback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="BytesRead">The bytes read.</param>
        /// <param name="TotalBytes">The total bytes.</param>
        /// <remarks></remarks>
        private delegate void UpdateProgessCallback(string source, string dest, long BytesRead, long TotalBytes);

        #endregion

        #region Public Methods

        /// <summary>
        /// The download file.
        /// </summary>
        /// <param name="URL">The url.</param>
        /// <param name="DestFile">The dest file.</param>
        /// <remarks></remarks>
        public void DownloadFile(string URL, string DestFile)
        {
            this.sourceURL.Add(URL);
            this.destFile.Add(DestFile);
            this.fileDone.Add(0); // 0 = File not finished

            // Let the user know we are connecting to the server
            lblProgress.Text = "Download Starting";
            string locType = "http";
            int typeEnd = URL.IndexOf(":");
            if (typeEnd > 0)
            {
                locType = URL.Substring(0, typeEnd);
            }

            // Create a new thread that calls the Download() method
            switch (locType.ToLower())
            {
                case "ftp":
                    thrDownload = new Thread(DownloadFTP);
                    break;
                case "http":
                    thrDownload = new Thread(DownloadHTTP);
                    break;
            }

            // Start the thread, and thus call Download()
            thrDownload.Start();
        }

        /// <summary>
        /// The reset credentials.
        /// </summary>
        /// <remarks></remarks>
        public void ResetCredentials()
        {
            accessCredentials = (NetworkCredential)CredentialCache.DefaultCredentials;
        }

        /// <summary>
        /// The set credentials.
        /// </summary>
        /// <param name="UserName">The user name.</param>
        /// <param name="Password">The password.</param>
        /// <remarks></remarks>
        public void SetCredentials(string UserName, string Password)
        {
            accessCredentials.UserName = UserName;
            accessCredentials.Password = Password;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void Cancel_Click(object sender, EventArgs e)
        {
            // Close the web response and the streams
            webResponse.Close();
            strResponse.Close();
            strLocal.Close();

            // Abort the thread that's downloading
            thrDownload.Abort();

            // Set the progress bar back to 0 and the label
            fileProgress.Value = 0;
            lblProgress.Text = "Download Cancelled";
        }

        /// <summary>
        /// The cancel_ click_1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void Cancel_Click_1(object sender, EventArgs e)
        {
            if (currentFile < destFile.Count)
            {
                setCancel = true;
                fileDone[currentFile] = 2; // error code 2
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// The download ftp.
        /// </summary>
        /// <remarks></remarks>
        private void DownloadFTP()
        {
            lock (isBusy)
            {
                if (setCancel)
                {
                    return;
                }

                using (WebClient wcDownload = new WebClient())
                {
                    try
                    {
                        string sourceShortName =
                            sourceURL[currentFile].Substring(sourceURL[currentFile].LastIndexOf('/') + 1);
                        this.Invoke(
                            new UpdateProgessCallback(this.UpdateProgress), 
                            new object[] { sourceShortName, destFile[currentFile], 0, 0 });

                        // Create a request to the file we are downloading
                        ftpRequest = (FtpWebRequest)WebRequest.Create(sourceURL[currentFile]);

                        // Set default authentication for retrieving the file info
                        ftpRequest.Credentials = accessCredentials;

                        // Retrieve the response from the server
                        try
                        {
                            ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                            // Ask the server for the file size and store it
                            long fileSize = ftpResponse.ContentLength;

                            if (ftpResponse != null)
                            {
                                ftpResponse.Close();
                            }

                            // Set default authentication for retrieving the file
                            wcDownload.Credentials = accessCredentials;

                            // Open the URL for download
                            strResponse = wcDownload.OpenRead(sourceURL[currentFile]);

                            // Create a new file stream where we will be saving the data (local drive)
                            strLocal = new FileStream(
                                destFile[currentFile], FileMode.Create, FileAccess.Write, FileShare.None);

                            // It will store the current number of bytes we retrieved from the server
                            int bytesSize = 0;

                            // A buffer for storing and writing the data retrieved from the server
                            byte[] downBuffer = new byte[2048];

                            // Loop through the buffer until the buffer is empty
                            while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0 && !setCancel)
                            {
                                // Write the data from the buffer to the local hard drive
                                strLocal.Write(downBuffer, 0, bytesSize);

                                // Invoke the method that updates the form's label and progress bar
                                this.Invoke(
                                    new UpdateProgessCallback(this.UpdateProgress), 
                                    new object[] { sourceShortName, destFile[currentFile], strLocal.Length, fileSize });
                            }

                            fileDone[currentFile] = 1;
                        }
                        catch (WebException ex)
                        {
                            fileDone[currentFile] = 2;
                            Global.ShowErrorMsg(sourceURL[currentFile] + "\n\n" + ex.Message, ex);
                        }
                    }
                    finally
                    {
                        // When the above code has ended, close the streams
                        try
                        {
                            if (strLocal != null)
                            {
                                strLocal.Close();
                                if (setCancel)
                                {
                                    File.Delete(destFile[currentFile]);
                                }
                            }

                            if (strResponse != null)
                            {
                                strResponse.Flush();
                                strResponse.Close();
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                currentFile++;
            }
        }

        /// <summary>
        /// The download http.
        /// </summary>
        /// <remarks></remarks>
        private void DownloadHTTP()
        {
            lock (isBusy)
            {
                using (WebClient wcDownload = new WebClient())
                {
                    try
                    {
                        // Create a request to the file we are downloading
                        webRequest = (HttpWebRequest)WebRequest.Create(sourceURL[currentFile]);

                        // Set default authentication for retrieving the file
                        webRequest.Credentials = CredentialCache.DefaultCredentials;

                        // Retrieve the response from the server
                        webResponse = (HttpWebResponse)webRequest.GetResponse();

                        // Ask the server for the file size and store it
                        long fileSize = webResponse.ContentLength;

                        // Open the URL for download
                        wcDownload.Credentials = webRequest.Credentials;
                        strResponse = wcDownload.OpenRead(sourceURL[currentFile]);

                        // Create a new file stream where we will be saving the data (local drive)
                        strLocal = new FileStream(
                            destFile[currentFile], FileMode.Create, FileAccess.Write, FileShare.None);

                        // It will store the current number of bytes we retrieved from the server
                        int bytesSize = 0;

                        // A buffer for storing and writing the data retrieved from the server
                        byte[] downBuffer = new byte[2048];

                        // Loop through the buffer until the buffer is empty
                        while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                        {
                            // Write the data from the buffer to the local hard drive
                            strLocal.Write(downBuffer, 0, bytesSize);

                            // Invoke the method that updates the form's label and progress bar
                            this.Invoke(
                                new UpdateProgessCallback(this.UpdateProgress), 
                                new object[]
                                    {
                                       sourceURL[currentFile], destFile[currentFile], strLocal.Length, fileSize 
                                    });
                        }
                    }
                    finally
                    {
                        // When the above code has ended, close the streams
                        webResponse.Close();
                        strResponse.Close();
                        strLocal.Close();
                    }
                }

                fileDone[currentFile] = 1;
                currentFile++;
            }
        }

        /// <summary>
        /// The download_ manager_ form closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void Download_Manager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (thrDownload != null)
            {
                thrDownload.Abort();
            }

            if (!setCancel && destFile.Count > 0)
            {
                string Updater = destFile[0].Substring(0, destFile[0].LastIndexOf('\\')) + "\\UpdateEnt.exe";
                if (File.Exists(destFile[0]) && File.Exists(Updater))
                {
                    string args = Process.GetCurrentProcess().Id.ToString();
                    args += " \"" + destFile[0] + "\"";
                    if (File.Exists(destFile[1]))
                    {
                        args += " \"" + destFile[1] + "\"";
                    }
                    else
                    {
                        args += " \"\"";
                    }

                    args += " http://forums.remnantmods.com/viewtopic.php?f=10&t=1325,http://forums.remnantmods.com";

                    // Attach any open maps to be reopened
                    foreach (MapForm mapForm in _mainForm.MdiChildren)
                    {
                        args += " \"" + mapForm.map.filePath + "\"";
                    }

                    Process.Start(Updater, args);
                }
            }

            this.Dispose();
        }

        /// <summary>
        /// The update progress.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="BytesRead">The bytes read.</param>
        /// <param name="TotalBytes">The total bytes.</param>
        /// <remarks></remarks>
        private void UpdateProgress(string source, string dest, long BytesRead, long TotalBytes)
        {
            txtSource.Text = source;
            txtDest.Text = dest;

            // Calculate the download progress in percentages
            if (TotalBytes != 0)
            {
                PercentProgress = Convert.ToInt32((BytesRead * 100) / TotalBytes);
            }
            else
            {
                PercentProgress = 0;
            }

            // Make progress on the progress bar
            fileProgress.Value = PercentProgress;

            // Display the current progress on the form
            lblProgress.Text = "Downloaded " + BytesRead + " / " + TotalBytes + " (" + PercentProgress + "%)";
        }

        /// <summary>
        /// The timer_ tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (currentFile == sourceURL.Count)
            {
                Cancel.Text = "&Install";
                int goodFiles = 0;
                for (int i = 0; i < fileDone.Count; i++)
                {
                    if (fileDone[i] == 1)
                    {
                        goodFiles++;
                    }
                }

                UpdateProgress(string.Empty, string.Empty, goodFiles, fileDone.Count);
            }
            else
            {
                Cancel.Text = "&Cancel";
            }

            if (this.setCancel)
            {
                this.Close();
            }
        }

        #endregion
    }
}