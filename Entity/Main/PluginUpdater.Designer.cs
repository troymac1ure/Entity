namespace entity.Main
{
    partial class PluginUpdater
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
            this.dgvPluginData = new System.Windows.Forms.DataGridView();
            this.cbUpdate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.metaType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionNew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.md5Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblUpdateCountLabel = new System.Windows.Forms.Label();
            this.lblUpdateCount = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.lblPluginSetLabel = new System.Windows.Forms.Label();
            this.lblPluginSet = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPluginData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPluginData
            // 
            this.dgvPluginData.AllowUserToAddRows = false;
            this.dgvPluginData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPluginData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPluginData.ColumnHeadersHeight = 20;
            this.dgvPluginData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPluginData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cbUpdate,
            this.metaType,
            this.versionCurrent,
            this.versionNew,
            this.md5Hash});
            this.dgvPluginData.Location = new System.Drawing.Point(12, 12);
            this.dgvPluginData.Name = "dgvPluginData";
            this.dgvPluginData.RowHeadersVisible = false;
            this.dgvPluginData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPluginData.Size = new System.Drawing.Size(410, 434);
            this.dgvPluginData.TabIndex = 0;
            this.dgvPluginData.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPluginData_CellBeginEdit);
            // 
            // cbUpdate
            // 
            this.cbUpdate.FillWeight = 73.3026F;
            this.cbUpdate.HeaderText = "Update?";
            this.cbUpdate.Name = "cbUpdate";
            this.cbUpdate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // metaType
            // 
            this.metaType.FillWeight = 87.98267F;
            this.metaType.HeaderText = "Meta Type";
            this.metaType.Name = "metaType";
            this.metaType.ReadOnly = true;
            // 
            // versionCurrent
            // 
            this.versionCurrent.FillWeight = 144.9919F;
            this.versionCurrent.HeaderText = "Current Version";
            this.versionCurrent.Name = "versionCurrent";
            this.versionCurrent.ReadOnly = true;
            // 
            // versionNew
            // 
            this.versionNew.FillWeight = 144.9919F;
            this.versionNew.HeaderText = "New Version";
            this.versionNew.Name = "versionNew";
            this.versionNew.ReadOnly = true;
            // 
            // md5Hash
            // 
            this.md5Hash.FillWeight = 1F;
            this.md5Hash.HeaderText = "MD5";
            this.md5Hash.Name = "md5Hash";
            this.md5Hash.ReadOnly = true;
            // 
            // lblUpdateCountLabel
            // 
            this.lblUpdateCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateCountLabel.AutoSize = true;
            this.lblUpdateCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdateCountLabel.Location = new System.Drawing.Point(12, 451);
            this.lblUpdateCountLabel.Name = "lblUpdateCountLabel";
            this.lblUpdateCountLabel.Size = new System.Drawing.Size(116, 13);
            this.lblUpdateCountLabel.TabIndex = 1;
            this.lblUpdateCountLabel.Text = "# of Plugins to Update:";
            // 
            // lblUpdateCount
            // 
            this.lblUpdateCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateCount.AutoSize = true;
            this.lblUpdateCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdateCount.Location = new System.Drawing.Point(134, 449);
            this.lblUpdateCount.Name = "lblUpdateCount";
            this.lblUpdateCount.Size = new System.Drawing.Size(16, 16);
            this.lblUpdateCount.TabIndex = 2;
            this.lblUpdateCount.Text = "0";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpdate.Location = new System.Drawing.Point(322, 469);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 29);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "&Update Plugins";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(230, 469);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 29);
            this.button1.TabIndex = 4;
            this.button1.Text = "&Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNone.Location = new System.Drawing.Point(12, 469);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(79, 29);
            this.btnSelectNone.TabIndex = 5;
            this.btnSelectNone.Text = "Select &None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(97, 469);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(79, 29);
            this.btnSelectAll.TabIndex = 5;
            this.btnSelectAll.Text = "Select &All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // lblPluginSetLabel
            // 
            this.lblPluginSetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginSetLabel.AutoSize = true;
            this.lblPluginSetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPluginSetLabel.Location = new System.Drawing.Point(208, 450);
            this.lblPluginSetLabel.Name = "lblPluginSetLabel";
            this.lblPluginSetLabel.Size = new System.Drawing.Size(95, 13);
            this.lblPluginSetLabel.TabIndex = 6;
            this.lblPluginSetLabel.Text = "Current Plugin Set:";
            // 
            // lblPluginSet
            // 
            this.lblPluginSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPluginSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPluginSet.Location = new System.Drawing.Point(302, 450);
            this.lblPluginSet.Name = "lblPluginSet";
            this.lblPluginSet.Size = new System.Drawing.Size(120, 13);
            this.lblPluginSet.TabIndex = 6;
            this.lblPluginSet.Text = "---";
            this.lblPluginSet.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PluginUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 507);
            this.Controls.Add(this.lblPluginSet);
            this.Controls.Add(this.lblPluginSetLabel);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblUpdateCount);
            this.Controls.Add(this.lblUpdateCountLabel);
            this.Controls.Add(this.dgvPluginData);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PluginUpdater";
            this.ShowInTaskbar = false;
            this.Text = "PluginUpdater";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPluginData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPluginData;
        private System.Windows.Forms.Label lblUpdateCountLabel;
        private System.Windows.Forms.Label lblUpdateCount;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cbUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn metaType;
        private System.Windows.Forms.DataGridViewTextBoxColumn versionCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn versionNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn md5Hash;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Label lblPluginSetLabel;
        private System.Windows.Forms.Label lblPluginSet;
    }
}