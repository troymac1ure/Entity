namespace entity.Main
{
    using HaloMap.Map;

    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMainMenuFile = new System.Windows.Forms.Label();
            this.gbExternalPaths = new System.Windows.Forms.GroupBox();
            this.btnBitmapsFile = new System.Windows.Forms.Button();
            this.btnSPSharedFile = new System.Windows.Forms.Button();
            this.btnSharedFile = new System.Windows.Forms.Button();
            this.btnMainmenuFile = new System.Windows.Forms.Button();
            this.tbBitmapsFile = new System.Windows.Forms.TextBox();
            this.tbSPSharedFile = new System.Windows.Forms.TextBox();
            this.tbSharedFile = new System.Windows.Forms.TextBox();
            this.tbMainmenuFile = new System.Windows.Forms.TextBox();
            this.lblBitmapsFile = new System.Windows.Forms.Label();
            this.lblSPSharedFile = new System.Windows.Forms.Label();
            this.lblSharedFile = new System.Windows.Forms.Label();
            this.gbOther = new System.Windows.Forms.GroupBox();
            this.lblBitmapsFolder = new System.Windows.Forms.Label();
            this.btnBitmapFolder = new System.Windows.Forms.Button();
            this.tbBitmapsFolder = new System.Windows.Forms.TextBox();
            this.lblExtractsFolder = new System.Windows.Forms.Label();
            this.btnExtractFolder = new System.Windows.Forms.Button();
            this.tbExtractsFolder = new System.Windows.Forms.TextBox();
            this.btnPluginFolder = new System.Windows.Forms.Button();
            this.tbPluginFolder = new System.Windows.Forms.TextBox();
            this.lblPluginFolder = new System.Windows.Forms.Label();
            this.btnCleanMapsFolder = new System.Windows.Forms.Button();
            this.tbCleanMapsFolder = new System.Windows.Forms.TextBox();
            this.lblCleanMapsFolder = new System.Windows.Forms.Label();
            this.lblPatchFolder = new System.Windows.Forms.Label();
            this.btnPatchFolder = new System.Windows.Forms.Button();
            this.tbPatchFolder = new System.Windows.Forms.TextBox();
            this.btnMapsFolder = new System.Windows.Forms.Button();
            this.tbMapsFolder = new System.Windows.Forms.TextBox();
            this.lblMapFolder = new System.Windows.Forms.Label();
            this.useDefaultMaps = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.btnClearRegistry = new System.Windows.Forms.Button();
            this.cbUseRegistry = new System.Windows.Forms.CheckBox();
            this.lblAutoUpdate = new System.Windows.Forms.Label();
            this.checkUpdates = new System.Windows.Forms.ComboBox();
            this.updateFrequencyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbExternalPaths.SuspendLayout();
            this.gbOther.SuspendLayout();
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateFrequencyBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMainMenuFile
            // 
            this.lblMainMenuFile.AutoSize = true;
            this.lblMainMenuFile.BackColor = System.Drawing.Color.Transparent;
            this.lblMainMenuFile.Location = new System.Drawing.Point(8, 26);
            this.lblMainMenuFile.Name = "lblMainMenuFile";
            this.lblMainMenuFile.Size = new System.Drawing.Size(60, 13);
            this.lblMainMenuFile.TabIndex = 1;
            this.lblMainMenuFile.Text = "Main Menu";
            // 
            // gbExternalPaths
            // 
            this.gbExternalPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbExternalPaths.Controls.Add(this.btnBitmapsFile);
            this.gbExternalPaths.Controls.Add(this.btnSPSharedFile);
            this.gbExternalPaths.Controls.Add(this.btnSharedFile);
            this.gbExternalPaths.Controls.Add(this.btnMainmenuFile);
            this.gbExternalPaths.Controls.Add(this.tbBitmapsFile);
            this.gbExternalPaths.Controls.Add(this.tbSPSharedFile);
            this.gbExternalPaths.Controls.Add(this.tbSharedFile);
            this.gbExternalPaths.Controls.Add(this.tbMainmenuFile);
            this.gbExternalPaths.Controls.Add(this.lblBitmapsFile);
            this.gbExternalPaths.Controls.Add(this.lblSPSharedFile);
            this.gbExternalPaths.Controls.Add(this.lblSharedFile);
            this.gbExternalPaths.Controls.Add(this.lblMainMenuFile);
            this.gbExternalPaths.Location = new System.Drawing.Point(2, 2);
            this.gbExternalPaths.Name = "gbExternalPaths";
            this.gbExternalPaths.Size = new System.Drawing.Size(416, 121);
            this.gbExternalPaths.TabIndex = 0;
            this.gbExternalPaths.TabStop = false;
            this.gbExternalPaths.Text = "External Map Paths";
            // 
            // btnBitmapsFile
            // 
            this.btnBitmapsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBitmapsFile.Location = new System.Drawing.Point(376, 93);
            this.btnBitmapsFile.Name = "btnBitmapsFile";
            this.btnBitmapsFile.Size = new System.Drawing.Size(33, 22);
            this.btnBitmapsFile.TabIndex = 12;
            this.btnBitmapsFile.Text = "...";
            this.btnBitmapsFile.UseVisualStyleBackColor = true;
            this.btnBitmapsFile.Click += new System.EventHandler(this.btnBitmapsFile_Click);
            // 
            // btnSPSharedFile
            // 
            this.btnSPSharedFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSPSharedFile.Location = new System.Drawing.Point(376, 69);
            this.btnSPSharedFile.Name = "btnSPSharedFile";
            this.btnSPSharedFile.Size = new System.Drawing.Size(33, 22);
            this.btnSPSharedFile.TabIndex = 9;
            this.btnSPSharedFile.Text = "...";
            this.btnSPSharedFile.UseVisualStyleBackColor = true;
            this.btnSPSharedFile.Click += new System.EventHandler(this.btnSPSharedFile_Click);
            // 
            // btnSharedFile
            // 
            this.btnSharedFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSharedFile.Location = new System.Drawing.Point(376, 45);
            this.btnSharedFile.Name = "btnSharedFile";
            this.btnSharedFile.Size = new System.Drawing.Size(33, 22);
            this.btnSharedFile.TabIndex = 6;
            this.btnSharedFile.Text = "...";
            this.btnSharedFile.UseVisualStyleBackColor = true;
            this.btnSharedFile.Click += new System.EventHandler(this.btnSharedFile_Click);
            // 
            // btnMainmenuFile
            // 
            this.btnMainmenuFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMainmenuFile.Location = new System.Drawing.Point(376, 21);
            this.btnMainmenuFile.Name = "btnMainmenuFile";
            this.btnMainmenuFile.Size = new System.Drawing.Size(33, 22);
            this.btnMainmenuFile.TabIndex = 3;
            this.btnMainmenuFile.Text = "...";
            this.btnMainmenuFile.UseVisualStyleBackColor = true;
            this.btnMainmenuFile.Click += new System.EventHandler(this.btnMainmenuFile_Click);
            // 
            // tbBitmapsFile
            // 
            this.tbBitmapsFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBitmapsFile.Location = new System.Drawing.Point(74, 94);
            this.tbBitmapsFile.Name = "tbBitmapsFile";
            this.tbBitmapsFile.Size = new System.Drawing.Size(296, 20);
            this.tbBitmapsFile.TabIndex = 11;
            // 
            // tbSPSharedFile
            // 
            this.tbSPSharedFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSPSharedFile.Location = new System.Drawing.Point(74, 71);
            this.tbSPSharedFile.Name = "tbSPSharedFile";
            this.tbSPSharedFile.Size = new System.Drawing.Size(296, 20);
            this.tbSPSharedFile.TabIndex = 8;
            // 
            // tbSharedFile
            // 
            this.tbSharedFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSharedFile.Location = new System.Drawing.Point(74, 47);
            this.tbSharedFile.Name = "tbSharedFile";
            this.tbSharedFile.Size = new System.Drawing.Size(296, 20);
            this.tbSharedFile.TabIndex = 5;
            // 
            // tbMainmenuFile
            // 
            this.tbMainmenuFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMainmenuFile.Location = new System.Drawing.Point(74, 23);
            this.tbMainmenuFile.Name = "tbMainmenuFile";
            this.tbMainmenuFile.Size = new System.Drawing.Size(296, 20);
            this.tbMainmenuFile.TabIndex = 2;
            // 
            // lblBitmapsFile
            // 
            this.lblBitmapsFile.AutoSize = true;
            this.lblBitmapsFile.BackColor = System.Drawing.Color.Transparent;
            this.lblBitmapsFile.Location = new System.Drawing.Point(8, 98);
            this.lblBitmapsFile.Name = "lblBitmapsFile";
            this.lblBitmapsFile.Size = new System.Drawing.Size(44, 13);
            this.lblBitmapsFile.TabIndex = 10;
            this.lblBitmapsFile.Text = "Bitmaps";
            // 
            // lblSPSharedFile
            // 
            this.lblSPSharedFile.AutoSize = true;
            this.lblSPSharedFile.BackColor = System.Drawing.Color.Transparent;
            this.lblSPSharedFile.Location = new System.Drawing.Point(8, 74);
            this.lblSPSharedFile.Name = "lblSPSharedFile";
            this.lblSPSharedFile.Size = new System.Drawing.Size(58, 13);
            this.lblSPSharedFile.TabIndex = 7;
            this.lblSPSharedFile.Text = "SP Shared";
            // 
            // lblSharedFile
            // 
            this.lblSharedFile.AutoSize = true;
            this.lblSharedFile.BackColor = System.Drawing.Color.Transparent;
            this.lblSharedFile.Location = new System.Drawing.Point(8, 50);
            this.lblSharedFile.Name = "lblSharedFile";
            this.lblSharedFile.Size = new System.Drawing.Size(41, 13);
            this.lblSharedFile.TabIndex = 4;
            this.lblSharedFile.Text = "Shared";
            // 
            // gbOther
            // 
            this.gbOther.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOther.Controls.Add(this.lblBitmapsFolder);
            this.gbOther.Controls.Add(this.btnBitmapFolder);
            this.gbOther.Controls.Add(this.tbBitmapsFolder);
            this.gbOther.Controls.Add(this.lblExtractsFolder);
            this.gbOther.Controls.Add(this.btnExtractFolder);
            this.gbOther.Controls.Add(this.tbExtractsFolder);
            this.gbOther.Controls.Add(this.btnPluginFolder);
            this.gbOther.Controls.Add(this.tbPluginFolder);
            this.gbOther.Controls.Add(this.lblPluginFolder);
            this.gbOther.Controls.Add(this.btnCleanMapsFolder);
            this.gbOther.Controls.Add(this.tbCleanMapsFolder);
            this.gbOther.Controls.Add(this.lblCleanMapsFolder);
            this.gbOther.Controls.Add(this.lblPatchFolder);
            this.gbOther.Controls.Add(this.btnPatchFolder);
            this.gbOther.Controls.Add(this.tbPatchFolder);
            this.gbOther.Controls.Add(this.btnMapsFolder);
            this.gbOther.Controls.Add(this.tbMapsFolder);
            this.gbOther.Controls.Add(this.lblMapFolder);
            this.gbOther.Location = new System.Drawing.Point(2, 126);
            this.gbOther.Name = "gbOther";
            this.gbOther.Size = new System.Drawing.Size(416, 171);
            this.gbOther.TabIndex = 13;
            this.gbOther.TabStop = false;
            this.gbOther.Text = "Other Folders";
            // 
            // lblBitmapsFolder
            // 
            this.lblBitmapsFolder.AutoSize = true;
            this.lblBitmapsFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblBitmapsFolder.Location = new System.Drawing.Point(0, 98);
            this.lblBitmapsFolder.Name = "lblBitmapsFolder";
            this.lblBitmapsFolder.Size = new System.Drawing.Size(71, 13);
            this.lblBitmapsFolder.TabIndex = 23;
            this.lblBitmapsFolder.Text = "Bitmap Folder";
            // 
            // btnBitmapFolder
            // 
            this.btnBitmapFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBitmapFolder.Location = new System.Drawing.Point(376, 93);
            this.btnBitmapFolder.Name = "btnBitmapFolder";
            this.btnBitmapFolder.Size = new System.Drawing.Size(33, 22);
            this.btnBitmapFolder.TabIndex = 25;
            this.btnBitmapFolder.Text = "...";
            this.btnBitmapFolder.UseVisualStyleBackColor = true;
            this.btnBitmapFolder.Click += new System.EventHandler(this.btnBitmapFolder_Click);
            // 
            // tbBitmapsFolder
            // 
            this.tbBitmapsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBitmapsFolder.Location = new System.Drawing.Point(74, 95);
            this.tbBitmapsFolder.Name = "tbBitmapsFolder";
            this.tbBitmapsFolder.Size = new System.Drawing.Size(296, 20);
            this.tbBitmapsFolder.TabIndex = 24;
            // 
            // lblExtractsFolder
            // 
            this.lblExtractsFolder.AutoSize = true;
            this.lblExtractsFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblExtractsFolder.Location = new System.Drawing.Point(-1, 124);
            this.lblExtractsFolder.Name = "lblExtractsFolder";
            this.lblExtractsFolder.Size = new System.Drawing.Size(72, 13);
            this.lblExtractsFolder.TabIndex = 26;
            this.lblExtractsFolder.Text = "Extract Folder";
            // 
            // btnExtractFolder
            // 
            this.btnExtractFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractFolder.Location = new System.Drawing.Point(376, 119);
            this.btnExtractFolder.Name = "btnExtractFolder";
            this.btnExtractFolder.Size = new System.Drawing.Size(33, 22);
            this.btnExtractFolder.TabIndex = 28;
            this.btnExtractFolder.Text = "...";
            this.btnExtractFolder.UseVisualStyleBackColor = true;
            this.btnExtractFolder.Click += new System.EventHandler(this.btnExtractFolder_Click);
            // 
            // tbExtractsFolder
            // 
            this.tbExtractsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExtractsFolder.Location = new System.Drawing.Point(74, 121);
            this.tbExtractsFolder.Name = "tbExtractsFolder";
            this.tbExtractsFolder.Size = new System.Drawing.Size(296, 20);
            this.tbExtractsFolder.TabIndex = 27;
            // 
            // btnPluginFolder
            // 
            this.btnPluginFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginFolder.Location = new System.Drawing.Point(376, 67);
            this.btnPluginFolder.Name = "btnPluginFolder";
            this.btnPluginFolder.Size = new System.Drawing.Size(33, 22);
            this.btnPluginFolder.TabIndex = 22;
            this.btnPluginFolder.Text = "...";
            this.btnPluginFolder.UseVisualStyleBackColor = true;
            this.btnPluginFolder.Click += new System.EventHandler(this.btnPluginFolder_Click);
            // 
            // tbPluginFolder
            // 
            this.tbPluginFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPluginFolder.Location = new System.Drawing.Point(74, 69);
            this.tbPluginFolder.Name = "tbPluginFolder";
            this.tbPluginFolder.Size = new System.Drawing.Size(296, 20);
            this.tbPluginFolder.TabIndex = 21;
            // 
            // lblPluginFolder
            // 
            this.lblPluginFolder.AutoSize = true;
            this.lblPluginFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblPluginFolder.Location = new System.Drawing.Point(3, 72);
            this.lblPluginFolder.Name = "lblPluginFolder";
            this.lblPluginFolder.Size = new System.Drawing.Size(68, 13);
            this.lblPluginFolder.TabIndex = 20;
            this.lblPluginFolder.Text = "Plugin Folder";
            // 
            // btnCleanMapsFolder
            // 
            this.btnCleanMapsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCleanMapsFolder.Location = new System.Drawing.Point(376, 41);
            this.btnCleanMapsFolder.Name = "btnCleanMapsFolder";
            this.btnCleanMapsFolder.Size = new System.Drawing.Size(33, 22);
            this.btnCleanMapsFolder.TabIndex = 19;
            this.btnCleanMapsFolder.Text = "...";
            this.btnCleanMapsFolder.UseVisualStyleBackColor = true;
            this.btnCleanMapsFolder.Click += new System.EventHandler(this.btnCleanMapsFolder_Click);
            // 
            // tbCleanMapsFolder
            // 
            this.tbCleanMapsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCleanMapsFolder.Location = new System.Drawing.Point(74, 43);
            this.tbCleanMapsFolder.Name = "tbCleanMapsFolder";
            this.tbCleanMapsFolder.Size = new System.Drawing.Size(296, 20);
            this.tbCleanMapsFolder.TabIndex = 18;
            // 
            // lblCleanMapsFolder
            // 
            this.lblCleanMapsFolder.AutoSize = true;
            this.lblCleanMapsFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblCleanMapsFolder.Location = new System.Drawing.Point(8, 46);
            this.lblCleanMapsFolder.Name = "lblCleanMapsFolder";
            this.lblCleanMapsFolder.Size = new System.Drawing.Size(63, 13);
            this.lblCleanMapsFolder.TabIndex = 17;
            this.lblCleanMapsFolder.Text = "Clean Maps";
            // 
            // lblPatchFolder
            // 
            this.lblPatchFolder.AutoSize = true;
            this.lblPatchFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblPatchFolder.Enabled = false;
            this.lblPatchFolder.Location = new System.Drawing.Point(6, 148);
            this.lblPatchFolder.Name = "lblPatchFolder";
            this.lblPatchFolder.Size = new System.Drawing.Size(67, 13);
            this.lblPatchFolder.TabIndex = 29;
            this.lblPatchFolder.Text = "Patch Folder";
            // 
            // btnPatchFolder
            // 
            this.btnPatchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPatchFolder.Enabled = false;
            this.btnPatchFolder.Location = new System.Drawing.Point(376, 143);
            this.btnPatchFolder.Name = "btnPatchFolder";
            this.btnPatchFolder.Size = new System.Drawing.Size(33, 22);
            this.btnPatchFolder.TabIndex = 31;
            this.btnPatchFolder.Text = "...";
            this.btnPatchFolder.UseVisualStyleBackColor = true;
            this.btnPatchFolder.Click += new System.EventHandler(this.btnPatchFolder_Click);
            // 
            // tbPatchFolder
            // 
            this.tbPatchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPatchFolder.Enabled = false;
            this.tbPatchFolder.Location = new System.Drawing.Point(74, 145);
            this.tbPatchFolder.Name = "tbPatchFolder";
            this.tbPatchFolder.Size = new System.Drawing.Size(296, 20);
            this.tbPatchFolder.TabIndex = 30;
            // 
            // btnMapsFolder
            // 
            this.btnMapsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapsFolder.Location = new System.Drawing.Point(376, 15);
            this.btnMapsFolder.Name = "btnMapsFolder";
            this.btnMapsFolder.Size = new System.Drawing.Size(33, 22);
            this.btnMapsFolder.TabIndex = 16;
            this.btnMapsFolder.Text = "...";
            this.btnMapsFolder.UseVisualStyleBackColor = true;
            this.btnMapsFolder.Click += new System.EventHandler(this.btnMapsFolder_Click);
            // 
            // tbMapsFolder
            // 
            this.tbMapsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMapsFolder.Location = new System.Drawing.Point(74, 17);
            this.tbMapsFolder.Name = "tbMapsFolder";
            this.tbMapsFolder.Size = new System.Drawing.Size(296, 20);
            this.tbMapsFolder.TabIndex = 15;
            // 
            // lblMapFolder
            // 
            this.lblMapFolder.AutoSize = true;
            this.lblMapFolder.BackColor = System.Drawing.Color.Transparent;
            this.lblMapFolder.Location = new System.Drawing.Point(8, 20);
            this.lblMapFolder.Name = "lblMapFolder";
            this.lblMapFolder.Size = new System.Drawing.Size(65, 13);
            this.lblMapFolder.TabIndex = 14;
            this.lblMapFolder.Text = "Maps Folder";
            // 
            // useDefaultMaps
            // 
            this.useDefaultMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.useDefaultMaps.BackColor = System.Drawing.Color.Transparent;
            this.useDefaultMaps.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.useDefaultMaps.Checked = true;
            this.useDefaultMaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useDefaultMaps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useDefaultMaps.Location = new System.Drawing.Point(69, 19);
            this.useDefaultMaps.Name = "useDefaultMaps";
            this.useDefaultMaps.Size = new System.Drawing.Size(332, 17);
            this.useDefaultMaps.TabIndex = 32;
            this.useDefaultMaps.Text = "Always use default maps (otherwise look in current map path)";
            this.useDefaultMaps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.useDefaultMaps.UseVisualStyleBackColor = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // openFolderDialog
            // 
            this.openFolderDialog.Description = "Select MAP files directory";
            // 
            // gbOptions
            // 
            this.gbOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOptions.Controls.Add(this.btnClearRegistry);
            this.gbOptions.Controls.Add(this.cbUseRegistry);
            this.gbOptions.Controls.Add(this.lblAutoUpdate);
            this.gbOptions.Controls.Add(this.checkUpdates);
            this.gbOptions.Controls.Add(this.useDefaultMaps);
            this.gbOptions.Location = new System.Drawing.Point(2, 303);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(416, 119);
            this.gbOptions.TabIndex = 14;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // btnClearRegistry
            // 
            this.btnClearRegistry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearRegistry.Enabled = false;
            this.btnClearRegistry.Location = new System.Drawing.Point(115, 37);
            this.btnClearRegistry.Name = "btnClearRegistry";
            this.btnClearRegistry.Size = new System.Drawing.Size(145, 22);
            this.btnClearRegistry.TabIndex = 34;
            this.btnClearRegistry.Text = "&Remove registry entries";
            this.btnClearRegistry.UseVisualStyleBackColor = true;
            this.btnClearRegistry.Click += new System.EventHandler(this.btnClearRegistry_Click);
            // 
            // cbUseRegistry
            // 
            this.cbUseRegistry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUseRegistry.AutoSize = true;
            this.cbUseRegistry.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbUseRegistry.Checked = true;
            this.cbUseRegistry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseRegistry.Location = new System.Drawing.Point(266, 42);
            this.cbUseRegistry.Name = "cbUseRegistry";
            this.cbUseRegistry.Size = new System.Drawing.Size(135, 17);
            this.cbUseRegistry.TabIndex = 33;
            this.cbUseRegistry.Text = "Use registry for settings";
            this.cbUseRegistry.UseVisualStyleBackColor = true;
            this.cbUseRegistry.CheckedChanged += new System.EventHandler(this.cbUseRegistry_CheckedChanged);
            // 
            // lblAutoUpdate
            // 
            this.lblAutoUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAutoUpdate.AutoSize = true;
            this.lblAutoUpdate.Location = new System.Drawing.Point(168, 90);
            this.lblAutoUpdate.Name = "lblAutoUpdate";
            this.lblAutoUpdate.Size = new System.Drawing.Size(106, 13);
            this.lblAutoUpdate.TabIndex = 35;
            this.lblAutoUpdate.Text = "Perform Auto Update";
            // 
            // checkUpdates
            // 
            this.checkUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkUpdates.DataSource = this.updateFrequencyBindingSource;
            this.checkUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkUpdates.FormattingEnabled = true;
            this.checkUpdates.Location = new System.Drawing.Point(280, 87);
            this.checkUpdates.Name = "checkUpdates";
            this.checkUpdates.Size = new System.Drawing.Size(121, 21);
            this.checkUpdates.TabIndex = 36;
            // 
            // updateFrequencyBindingSource
            // 
            this.updateFrequencyBindingSource.DataSource = new Globals.Prefs.updateFrequency[] {
        Globals.Prefs.updateFrequency.Always,
        Globals.Prefs.updateFrequency.Daily,
        Globals.Prefs.updateFrequency.Weekly,
        Globals.Prefs.updateFrequency.Monthly,
        Globals.Prefs.updateFrequency.Never};
            this.updateFrequencyBindingSource.Position = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(324, 428);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 25);
            this.btnSave.TabIndex = 38;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(224, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 25);
            this.btnCancel.TabIndex = 37;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 457);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbOptions);
            this.Controls.Add(this.gbOther);
            this.Controls.Add(this.gbExternalPaths);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Settings";
            this.Text = "Settings";
            this.gbExternalPaths.ResumeLayout(false);
            this.gbExternalPaths.PerformLayout();
            this.gbOther.ResumeLayout(false);
            this.gbOther.PerformLayout();
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateFrequencyBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMainMenuFile;
        private System.Windows.Forms.GroupBox gbExternalPaths;
        private System.Windows.Forms.GroupBox gbOther;
        private System.Windows.Forms.Label lblBitmapsFile;
        private System.Windows.Forms.Label lblSPSharedFile;
        private System.Windows.Forms.Label lblSharedFile;
        private System.Windows.Forms.TextBox tbBitmapsFile;
        private System.Windows.Forms.TextBox tbSPSharedFile;
        private System.Windows.Forms.TextBox tbSharedFile;
        private System.Windows.Forms.TextBox tbMainmenuFile;
        private System.Windows.Forms.Button btnMainmenuFile;
        private System.Windows.Forms.Button btnBitmapsFile;
        private System.Windows.Forms.Button btnSPSharedFile;
        private System.Windows.Forms.Button btnSharedFile;
        private System.Windows.Forms.Label lblMapFolder;
        private System.Windows.Forms.Label lblCleanMapsFolder;
        private System.Windows.Forms.Label lblPatchFolder;
        private System.Windows.Forms.TextBox tbMapsFolder;
        private System.Windows.Forms.TextBox tbCleanMapsFolder;
        private System.Windows.Forms.TextBox tbPatchFolder;
        private System.Windows.Forms.Button btnMapsFolder;
        private System.Windows.Forms.Button btnCleanMapsFolder;
        private System.Windows.Forms.Button btnPatchFolder;
        private System.Windows.Forms.CheckBox useDefaultMaps;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog openFolderDialog;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.Label lblAutoUpdate;
        private System.Windows.Forms.ComboBox checkUpdates;
        private System.Windows.Forms.BindingSource updateFrequencyBindingSource;
        private System.Windows.Forms.Button btnPluginFolder;
        private System.Windows.Forms.TextBox tbPluginFolder;
        private System.Windows.Forms.Label lblPluginFolder;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClearRegistry;
        private System.Windows.Forms.CheckBox cbUseRegistry;
        private System.Windows.Forms.Label lblBitmapsFolder;
        private System.Windows.Forms.Button btnBitmapFolder;
        private System.Windows.Forms.TextBox tbBitmapsFolder;
        private System.Windows.Forms.Label lblExtractsFolder;
        private System.Windows.Forms.Button btnExtractFolder;
        private System.Windows.Forms.TextBox tbExtractsFolder;
    }
}