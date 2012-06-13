namespace entity.HexEditor
{
    partial class HexView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtbInfo = new System.Windows.Forms.RichTextBox();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbRawNumber = new System.Windows.Forms.ComboBox();
            this.lblRawView = new System.Windows.Forms.Label();
            this.lblMetaView = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.cbGoto = new System.Windows.Forms.ComboBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.btnGoto = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(10, 10);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(577, 24);
            this.menuStrip1.TabIndex = 38;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.goToToolStripMenuItem.Text = "Go To";
            this.goToToolStripMenuItem.Click += new System.EventHandler(this.goToToolStripMenuItem_Click);
            // 
            // txtbInfo
            // 
            this.txtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtbInfo.Font = new System.Drawing.Font("Courier New", 8F);
            this.txtbInfo.Location = new System.Drawing.Point(0, 0);
            this.txtbInfo.MaximumSize = new System.Drawing.Size(3000, 220);
            this.txtbInfo.Name = "txtbInfo";
            this.txtbInfo.ReadOnly = true;
            this.txtbInfo.Size = new System.Drawing.Size(678, 207);
            this.txtbInfo.TabIndex = 39;
            this.txtbInfo.Text = "";
            // 
            // hexBox1
            // 
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.HexCasing = Be.Windows.Forms.HexCasing.Lower;
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 0);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(678, 340);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 37;
            this.hexBox1.UseFixedBytesPerLine = true;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.SelectionStartChanged += new System.EventHandler(this.hexBox1_SelectionStartChanged);
            this.hexBox1.SelectionLengthChanged += new System.EventHandler(this.hexBox1_SelectionLengthChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 36);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.hexBox1);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtbInfo);
            this.splitContainer1.Size = new System.Drawing.Size(678, 551);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 41;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.cbRawNumber);
            this.panel1.Controls.Add(this.lblRawView);
            this.panel1.Controls.Add(this.lblMetaView);
            this.panel1.Controls.Add(this.lblPosition);
            this.panel1.Controls.Add(this.cbGoto);
            this.panel1.Controls.Add(this.lblLocation);
            this.panel1.Controls.Add(this.btnGoto);
            this.panel1.Controls.Add(this.btnSaveChanges);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(678, 31);
            this.panel1.TabIndex = 42;
            // 
            // cbRawNumber
            // 
            this.cbRawNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRawNumber.FormattingEnabled = true;
            this.cbRawNumber.Location = new System.Drawing.Point(470, 3);
            this.cbRawNumber.Name = "cbRawNumber";
            this.cbRawNumber.Size = new System.Drawing.Size(103, 21);
            this.cbRawNumber.TabIndex = 8;
            this.cbRawNumber.Visible = false;
            this.cbRawNumber.SelectedIndexChanged += new System.EventHandler(this.cbRawNumber_SelectedIndexChanged);
            // 
            // lblRawView
            // 
            this.lblRawView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRawView.Location = new System.Drawing.Point(411, 5);
            this.lblRawView.Name = "lblRawView";
            this.lblRawView.Size = new System.Drawing.Size(53, 18);
            this.lblRawView.TabIndex = 7;
            this.lblRawView.Text = "Raw";
            this.lblRawView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRawView.Visible = false;
            this.lblRawView.Click += new System.EventHandler(this.lblRawView_Click);
            // 
            // lblMetaView
            // 
            this.lblMetaView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMetaView.Location = new System.Drawing.Point(352, 6);
            this.lblMetaView.Name = "lblMetaView";
            this.lblMetaView.Size = new System.Drawing.Size(53, 18);
            this.lblMetaView.TabIndex = 6;
            this.lblMetaView.Text = "Meta";
            this.lblMetaView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMetaView.Click += new System.EventHandler(this.lblMetaView_Click);
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(272, 9);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(30, 13);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "0 / 0";
            // 
            // cbGoto
            // 
            this.cbGoto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cbGoto.FormattingEnabled = true;
            this.cbGoto.Location = new System.Drawing.Point(128, 5);
            this.cbGoto.Name = "cbGoto";
            this.cbGoto.Size = new System.Drawing.Size(78, 16);
            this.cbGoto.TabIndex = 4;
            // 
            // lblLocation
            // 
            this.lblLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocation.Location = new System.Drawing.Point(564, 2);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(107, 26);
            this.lblLocation.TabIndex = 3;
            this.lblLocation.Text = "Ln 0    Col 0";
            this.lblLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGoto
            // 
            this.btnGoto.Location = new System.Drawing.Point(212, 0);
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(54, 25);
            this.btnGoto.TabIndex = 1;
            this.btnGoto.Text = "Goto";
            this.btnGoto.UseVisualStyleBackColor = true;
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(-2, 0);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(92, 25);
            this.btnSaveChanges.TabIndex = 0;
            this.btnSaveChanges.Text = "Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // HexView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "HexView";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(683, 591);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HexView_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtbInfo;
        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnGoto;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.ComboBox cbGoto;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblMetaView;
        private System.Windows.Forms.Label lblRawView;
        private System.Windows.Forms.ComboBox cbRawNumber;
    }
}
