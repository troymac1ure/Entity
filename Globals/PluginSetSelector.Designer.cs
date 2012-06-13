namespace Globals
{
    partial class PluginSetSelector
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tbPluginName = new System.Windows.Forms.TextBox();
            this.tbPluginDirectory = new System.Windows.Forms.TextBox();
            this.lblPluginName = new System.Windows.Forms.Label();
            this.lblPluginDirectory = new System.Windows.Forms.Label();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnAddUpdate = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(155, 134);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tbPluginName
            // 
            this.tbPluginName.Location = new System.Drawing.Point(201, 48);
            this.tbPluginName.Name = "tbPluginName";
            this.tbPluginName.Size = new System.Drawing.Size(202, 20);
            this.tbPluginName.TabIndex = 1;
            // 
            // tbPluginDirectory
            // 
            this.tbPluginDirectory.Location = new System.Drawing.Point(201, 102);
            this.tbPluginDirectory.Name = "tbPluginDirectory";
            this.tbPluginDirectory.Size = new System.Drawing.Size(348, 20);
            this.tbPluginDirectory.TabIndex = 2;
            // 
            // lblPluginName
            // 
            this.lblPluginName.AutoSize = true;
            this.lblPluginName.Location = new System.Drawing.Point(198, 32);
            this.lblPluginName.Name = "lblPluginName";
            this.lblPluginName.Size = new System.Drawing.Size(86, 13);
            this.lblPluginName.TabIndex = 3;
            this.lblPluginName.Text = "Plugin Set Name";
            // 
            // lblPluginDirectory
            // 
            this.lblPluginDirectory.AutoSize = true;
            this.lblPluginDirectory.Location = new System.Drawing.Point(198, 86);
            this.lblPluginDirectory.Name = "lblPluginDirectory";
            this.lblPluginDirectory.Size = new System.Drawing.Size(81, 13);
            this.lblPluginDirectory.TabIndex = 4;
            this.lblPluginDirectory.Text = "Plugin Directory";
            // 
            // btnSelectDirectory
            // 
            this.btnSelectDirectory.Location = new System.Drawing.Point(552, 102);
            this.btnSelectDirectory.Name = "btnSelectDirectory";
            this.btnSelectDirectory.Size = new System.Drawing.Size(29, 19);
            this.btnSelectDirectory.TabIndex = 5;
            this.btnSelectDirectory.Text = "...";
            this.btnSelectDirectory.UseVisualStyleBackColor = true;
            this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
            // 
            // btnAddUpdate
            // 
            this.btnAddUpdate.Location = new System.Drawing.Point(201, 141);
            this.btnAddUpdate.Name = "btnAddUpdate";
            this.btnAddUpdate.Size = new System.Drawing.Size(107, 24);
            this.btnAddUpdate.TabIndex = 6;
            this.btnAddUpdate.Text = "&Add / Update";
            this.btnAddUpdate.UseVisualStyleBackColor = true;
            this.btnAddUpdate.Click += new System.EventHandler(this.btnAddUpdate_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(12, 146);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(156, 23);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // PluginSetSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 177);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddUpdate);
            this.Controls.Add(this.btnSelectDirectory);
            this.Controls.Add(this.tbPluginDirectory);
            this.Controls.Add(this.tbPluginName);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblPluginName);
            this.Controls.Add(this.lblPluginDirectory);
            this.Name = "PluginSetSelector";
            this.Text = "PluginSetSelector";
            this.Load += new System.EventHandler(this.PluginSetSelector_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PluginSetSelector_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tbPluginName;
        private System.Windows.Forms.TextBox tbPluginDirectory;
        private System.Windows.Forms.Label lblPluginName;
        private System.Windows.Forms.Label lblPluginDirectory;
        private System.Windows.Forms.Button btnSelectDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnAddUpdate;
        private System.Windows.Forms.Button btnRemove;
    }
}