namespace entity.MetaEditor2
{
    partial class RestoreSelection
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
            this.btnRestoreValues = new System.Windows.Forms.Button();
            this.lblSourceTag = new System.Windows.Forms.Label();
            this.lbSourceIndices = new System.Windows.Forms.ListBox();
            this.lbDestIndices = new System.Windows.Forms.ListBox();
            this.lblSourceIndices = new System.Windows.Forms.Label();
            this.lblDestIndices = new System.Windows.Forms.Label();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.cbSourceNoClear = new System.Windows.Forms.CheckBox();
            this.cbDestNoClear = new System.Windows.Forms.CheckBox();
            this.lblSourceDestDiff = new System.Windows.Forms.Label();
            this.cbSourceReflexiveNumber = new System.Windows.Forms.ComboBox();
            this.lblSourceReflexiveNumber = new System.Windows.Forms.Label();
            this.lblDestTag = new System.Windows.Forms.Label();
            this.lblDestReflexiveNumber = new System.Windows.Forms.Label();
            this.cbDestReflexiveNumber = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.scAll = new System.Windows.Forms.SplitContainer();
            this.scSource = new System.Windows.Forms.SplitContainer();
            this.tvSourceTags = new entity.MetaEditor2.TagTreeView();
            this.scDest = new System.Windows.Forms.SplitContainer();
            this.tvDestTags = new entity.MetaEditor2.TagTreeView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.scAll.Panel1.SuspendLayout();
            this.scAll.Panel2.SuspendLayout();
            this.scAll.SuspendLayout();
            this.scSource.Panel1.SuspendLayout();
            this.scSource.Panel2.SuspendLayout();
            this.scSource.SuspendLayout();
            this.scDest.Panel1.SuspendLayout();
            this.scDest.Panel2.SuspendLayout();
            this.scDest.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRestoreValues
            // 
            this.btnRestoreValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestoreValues.Enabled = false;
            this.btnRestoreValues.Location = new System.Drawing.Point(692, 12);
            this.btnRestoreValues.Name = "btnRestoreValues";
            this.btnRestoreValues.Size = new System.Drawing.Size(88, 25);
            this.btnRestoreValues.TabIndex = 3;
            this.btnRestoreValues.Text = "&Restore Values";
            this.btnRestoreValues.UseVisualStyleBackColor = true;
            this.btnRestoreValues.Click += new System.EventHandler(this.savetagTypeBtn_Click);
            // 
            // lblSourceTag
            // 
            this.lblSourceTag.AutoSize = true;
            this.lblSourceTag.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSourceTag.Location = new System.Drawing.Point(0, 0);
            this.lblSourceTag.Name = "lblSourceTag";
            this.lblSourceTag.Size = new System.Drawing.Size(96, 13);
            this.lblSourceTag.TabIndex = 11;
            this.lblSourceTag.Text = "Source Tag Listing";
            // 
            // lbSourceIndices
            // 
            this.lbSourceIndices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSourceIndices.FormattingEnabled = true;
            this.lbSourceIndices.Location = new System.Drawing.Point(0, 13);
            this.lbSourceIndices.Name = "lbSourceIndices";
            this.lbSourceIndices.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbSourceIndices.Size = new System.Drawing.Size(199, 264);
            this.lbSourceIndices.TabIndex = 12;
            this.lbSourceIndices.SelectedIndexChanged += new System.EventHandler(this.lbIndices_SelectedIndexChanged);
            this.lbSourceIndices.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbIndices_MouseMove);
            // 
            // lbDestIndices
            // 
            this.lbDestIndices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDestIndices.FormattingEnabled = true;
            this.lbDestIndices.Location = new System.Drawing.Point(0, 13);
            this.lbDestIndices.Name = "lbDestIndices";
            this.lbDestIndices.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbDestIndices.Size = new System.Drawing.Size(199, 264);
            this.lbDestIndices.TabIndex = 13;
            this.lbDestIndices.SelectedIndexChanged += new System.EventHandler(this.lbIndices_SelectedIndexChanged);
            this.lbDestIndices.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbIndices_MouseMove);
            // 
            // lblSourceIndices
            // 
            this.lblSourceIndices.AutoSize = true;
            this.lblSourceIndices.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSourceIndices.Location = new System.Drawing.Point(0, 0);
            this.lblSourceIndices.Name = "lblSourceIndices";
            this.lblSourceIndices.Size = new System.Drawing.Size(52, 13);
            this.lblSourceIndices.TabIndex = 14;
            this.lblSourceIndices.Text = "From File:";
            // 
            // lblDestIndices
            // 
            this.lblDestIndices.AutoSize = true;
            this.lblDestIndices.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDestIndices.Location = new System.Drawing.Point(0, 0);
            this.lblDestIndices.Name = "lblDestIndices";
            this.lblDestIndices.Size = new System.Drawing.Size(70, 13);
            this.lblDestIndices.TabIndex = 15;
            this.lblDestIndices.Text = "To Reflexive:";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(321, 12);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(86, 25);
            this.btnSelectAll.TabIndex = 16;
            this.btnSelectAll.Text = "Select &All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectNone.Location = new System.Drawing.Point(411, 12);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(86, 25);
            this.btnSelectNone.TabIndex = 17;
            this.btnSelectNone.Text = "Select &None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // cbSourceNoClear
            // 
            this.cbSourceNoClear.AutoSize = true;
            this.cbSourceNoClear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbSourceNoClear.Location = new System.Drawing.Point(0, 277);
            this.cbSourceNoClear.Name = "cbSourceNoClear";
            this.cbSourceNoClear.Size = new System.Drawing.Size(199, 17);
            this.cbSourceNoClear.TabIndex = 18;
            this.cbSourceNoClear.Text = "Keep selections after restore";
            this.cbSourceNoClear.UseVisualStyleBackColor = true;
            // 
            // cbDestNoClear
            // 
            this.cbDestNoClear.AutoSize = true;
            this.cbDestNoClear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbDestNoClear.Location = new System.Drawing.Point(0, 277);
            this.cbDestNoClear.Name = "cbDestNoClear";
            this.cbDestNoClear.Size = new System.Drawing.Size(199, 17);
            this.cbDestNoClear.TabIndex = 19;
            this.cbDestNoClear.Text = "Keep selections after restore";
            this.cbDestNoClear.UseVisualStyleBackColor = true;
            // 
            // lblSourceDestDiff
            // 
            this.lblSourceDestDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSourceDestDiff.AutoSize = true;
            this.lblSourceDestDiff.Location = new System.Drawing.Point(680, 43);
            this.lblSourceDestDiff.Name = "lblSourceDestDiff";
            this.lblSourceDestDiff.Size = new System.Drawing.Size(100, 13);
            this.lblSourceDestDiff.TabIndex = 20;
            this.lblSourceDestDiff.Text = "No selections made";
            this.lblSourceDestDiff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSourceDestDiff.Visible = false;
            // 
            // cbSourceReflexiveNumber
            // 
            this.cbSourceReflexiveNumber.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbSourceReflexiveNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSourceReflexiveNumber.Enabled = false;
            this.cbSourceReflexiveNumber.FormattingEnabled = true;
            this.cbSourceReflexiveNumber.Location = new System.Drawing.Point(335, 0);
            this.cbSourceReflexiveNumber.Name = "cbSourceReflexiveNumber";
            this.cbSourceReflexiveNumber.Size = new System.Drawing.Size(60, 21);
            this.cbSourceReflexiveNumber.TabIndex = 21;
            this.cbSourceReflexiveNumber.SelectedIndexChanged += new System.EventHandler(this.cbSourceReflexiveNumber_SelectedIndexChanged);
            // 
            // lblSourceReflexiveNumber
            // 
            this.lblSourceReflexiveNumber.AutoSize = true;
            this.lblSourceReflexiveNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSourceReflexiveNumber.Enabled = false;
            this.lblSourceReflexiveNumber.Location = new System.Drawing.Point(0, 0);
            this.lblSourceReflexiveNumber.Name = "lblSourceReflexiveNumber";
            this.lblSourceReflexiveNumber.Size = new System.Drawing.Size(98, 13);
            this.lblSourceReflexiveNumber.TabIndex = 22;
            this.lblSourceReflexiveNumber.Text = "Source Reflexive #";
            // 
            // lblDestTag
            // 
            this.lblDestTag.AutoSize = true;
            this.lblDestTag.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDestTag.Location = new System.Drawing.Point(0, 0);
            this.lblDestTag.Name = "lblDestTag";
            this.lblDestTag.Size = new System.Drawing.Size(84, 13);
            this.lblDestTag.TabIndex = 24;
            this.lblDestTag.Text = "Dest Tag Listing";
            // 
            // lblDestReflexiveNumber
            // 
            this.lblDestReflexiveNumber.AutoSize = true;
            this.lblDestReflexiveNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDestReflexiveNumber.Enabled = false;
            this.lblDestReflexiveNumber.Location = new System.Drawing.Point(0, 0);
            this.lblDestReflexiveNumber.Name = "lblDestReflexiveNumber";
            this.lblDestReflexiveNumber.Size = new System.Drawing.Size(86, 13);
            this.lblDestReflexiveNumber.TabIndex = 25;
            this.lblDestReflexiveNumber.Text = "Dest Reflexive #";
            // 
            // cbDestReflexiveNumber
            // 
            this.cbDestReflexiveNumber.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbDestReflexiveNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDestReflexiveNumber.Enabled = false;
            this.cbDestReflexiveNumber.FormattingEnabled = true;
            this.cbDestReflexiveNumber.Location = new System.Drawing.Point(335, 0);
            this.cbDestReflexiveNumber.Name = "cbDestReflexiveNumber";
            this.cbDestReflexiveNumber.Size = new System.Drawing.Size(60, 21);
            this.cbDestReflexiveNumber.TabIndex = 26;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbSourceReflexiveNumber);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 13);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 24);
            this.panel1.TabIndex = 27;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbDestReflexiveNumber);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 13);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(395, 24);
            this.panel2.TabIndex = 28;
            // 
            // scAll
            // 
            this.scAll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scAll.Location = new System.Drawing.Point(12, 59);
            this.scAll.Name = "scAll";
            // 
            // scAll.Panel1
            // 
            this.scAll.Panel1.Controls.Add(this.scSource);
            this.scAll.Panel1.Controls.Add(this.panel1);
            this.scAll.Panel1.Controls.Add(this.lblSourceReflexiveNumber);
            // 
            // scAll.Panel2
            // 
            this.scAll.Panel2.Controls.Add(this.scDest);
            this.scAll.Panel2.Controls.Add(this.panel2);
            this.scAll.Panel2.Controls.Add(this.lblDestReflexiveNumber);
            this.scAll.Size = new System.Drawing.Size(794, 331);
            this.scAll.SplitterDistance = 395;
            this.scAll.TabIndex = 29;
            // 
            // scSource
            // 
            this.scSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scSource.Location = new System.Drawing.Point(0, 37);
            this.scSource.Name = "scSource";
            // 
            // scSource.Panel1
            // 
            this.scSource.Panel1.Controls.Add(this.tvSourceTags);
            this.scSource.Panel1.Controls.Add(this.lblSourceTag);
            // 
            // scSource.Panel2
            // 
            this.scSource.Panel2.Controls.Add(this.lbSourceIndices);
            this.scSource.Panel2.Controls.Add(this.lblSourceIndices);
            this.scSource.Panel2.Controls.Add(this.cbSourceNoClear);
            this.scSource.Size = new System.Drawing.Size(395, 294);
            this.scSource.SplitterDistance = 192;
            this.scSource.TabIndex = 19;
            // 
            // tvSourceTags
            // 
            this.tvSourceTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSourceTags.HideSelection = false;
            this.tvSourceTags.Location = new System.Drawing.Point(0, 13);
            this.tvSourceTags.Name = "tvSourceTags";
            this.tvSourceTags.ShowNodeToolTips = true;
            this.tvSourceTags.Size = new System.Drawing.Size(192, 281);
            this.tvSourceTags.TabIndex = 0;
            this.tvSourceTags.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSourceTags_AfterSelect);
            // 
            // scDest
            // 
            this.scDest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDest.Location = new System.Drawing.Point(0, 37);
            this.scDest.Name = "scDest";
            // 
            // scDest.Panel1
            // 
            this.scDest.Panel1.Controls.Add(this.tvDestTags);
            this.scDest.Panel1.Controls.Add(this.lblDestTag);
            // 
            // scDest.Panel2
            // 
            this.scDest.Panel2.Controls.Add(this.lbDestIndices);
            this.scDest.Panel2.Controls.Add(this.cbDestNoClear);
            this.scDest.Panel2.Controls.Add(this.lblDestIndices);
            this.scDest.Size = new System.Drawing.Size(395, 294);
            this.scDest.SplitterDistance = 192;
            this.scDest.TabIndex = 25;
            // 
            // tvDestTags
            // 
            this.tvDestTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDestTags.HideSelection = false;
            this.tvDestTags.HideUnusedReflexives = false;
            this.tvDestTags.Location = new System.Drawing.Point(0, 13);
            this.tvDestTags.Name = "tvDestTags";
            this.tvDestTags.ShowNodeToolTips = true;
            this.tvDestTags.Size = new System.Drawing.Size(192, 281);
            this.tvDestTags.SortByName = false;
            this.tvDestTags.TabIndex = 23;
            this.tvDestTags.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDestTags_AfterSelect);
            // 
            // RestoreSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 402);
            this.Controls.Add(this.lblSourceDestDiff);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnRestoreValues);
            this.Controls.Add(this.scAll);
            this.Name = "RestoreSelection";
            this.Text = "RestoreSelection";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RestoreSelection_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.scAll.Panel1.ResumeLayout(false);
            this.scAll.Panel1.PerformLayout();
            this.scAll.Panel2.ResumeLayout(false);
            this.scAll.Panel2.PerformLayout();
            this.scAll.ResumeLayout(false);
            this.scSource.Panel1.ResumeLayout(false);
            this.scSource.Panel1.PerformLayout();
            this.scSource.Panel2.ResumeLayout(false);
            this.scSource.Panel2.PerformLayout();
            this.scSource.ResumeLayout(false);
            this.scDest.Panel1.ResumeLayout(false);
            this.scDest.Panel1.PerformLayout();
            this.scDest.Panel2.ResumeLayout(false);
            this.scDest.Panel2.PerformLayout();
            this.scDest.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetaEditor2.TagTreeView tvSourceTags;
        private System.Windows.Forms.Button btnRestoreValues;
        private System.Windows.Forms.Label lblSourceTag;
        private System.Windows.Forms.ListBox lbSourceIndices;
        private System.Windows.Forms.ListBox lbDestIndices;
        private System.Windows.Forms.Label lblSourceIndices;
        private System.Windows.Forms.Label lblDestIndices;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.CheckBox cbSourceNoClear;
        private System.Windows.Forms.CheckBox cbDestNoClear;
        private System.Windows.Forms.Label lblSourceDestDiff;
        private System.Windows.Forms.ComboBox cbSourceReflexiveNumber;
        private System.Windows.Forms.Label lblSourceReflexiveNumber;
        private MetaEditor2.TagTreeView tvDestTags;
        private System.Windows.Forms.Label lblDestTag;
        private System.Windows.Forms.Label lblDestReflexiveNumber;
        private System.Windows.Forms.ComboBox cbDestReflexiveNumber;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer scAll;
        private System.Windows.Forms.SplitContainer scSource;
        private System.Windows.Forms.SplitContainer scDest;
    }
}