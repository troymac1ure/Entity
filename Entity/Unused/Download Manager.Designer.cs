namespace entity.Unused
{
    partial class Download_Manager
    {
        #region Fields

        private System.Windows.Forms.Button Cancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ProgressBar fileProgress;
        private System.Windows.Forms.Label lblDest;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label txtDest;
        private System.Windows.Forms.Label txtSource;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblDest = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.fileProgress = new System.Windows.Forms.ProgressBar();
            this.txtSource = new System.Windows.Forms.Label();
            this.txtDest = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // lblSource
            //
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(20, 23);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(47, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Source :";
            //
            // lblDest
            //
            this.lblDest.AutoSize = true;
            this.lblDest.Location = new System.Drawing.Point(20, 48);
            this.lblDest.Name = "lblDest";
            this.lblDest.Size = new System.Drawing.Size(66, 13);
            this.lblDest.TabIndex = 1;
            this.lblDest.Text = "Destination :";
            //
            // lblProgress
            //
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(40, 79);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(57, 20);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "label3";
            //
            // Cancel
            //
            this.Cancel.Location = new System.Drawing.Point(297, 154);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "&Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click_1);
            //
            // fileProgress
            //
            this.fileProgress.Location = new System.Drawing.Point(44, 102);
            this.fileProgress.Name = "fileProgress";
            this.fileProgress.Size = new System.Drawing.Size(581, 32);
            this.fileProgress.TabIndex = 7;
            //
            // txtSource
            //
            this.txtSource.AutoSize = true;
            this.txtSource.Location = new System.Drawing.Point(93, 23);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(82, 13);
            this.txtSource.TabIndex = 8;
            this.txtSource.Text = ".: Please Wait :.";
            //
            // txtDest
            //
            this.txtDest.AutoSize = true;
            this.txtDest.Location = new System.Drawing.Point(93, 48);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(82, 13);
            this.txtDest.TabIndex = 9;
            this.txtDest.Text = ".: Please Wait :.";
            //
            // timer
            //
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            //
            // Download_Manager
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 189);
            this.Controls.Add(this.txtDest);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.fileProgress);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblDest);
            this.Controls.Add(this.lblSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Download_Manager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download_Manager";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Download_Manager_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Methods
    }
}