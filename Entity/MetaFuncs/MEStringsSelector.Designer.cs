namespace entity.MetaFuncs
{
    partial class MEStringsSelector
    {
        #region Fields

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer tmr_MEStrings;

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
            this.tmr_MEStrings = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbSelectStringIDs = new System.Windows.Forms.ComboBox();
            this.lblStringID = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lbStringIDs = new System.Windows.Forms.ListBox();
            this.lblSelectStringIDs = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblUnicode = new System.Windows.Forms.Label();
            this.cbShowAllUnicodes = new System.Windows.Forms.CheckBox();
            this.lbUnicodes = new System.Windows.Forms.ListBox();
            this.cbUnicodeSort = new System.Windows.Forms.ComboBox();
            this.lblUnicodeSort = new System.Windows.Forms.Label();
            this.lblUnicodePosition = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmr_MEStrings
            // 
            this.tmr_MEStrings.Interval = 700;
            this.tmr_MEStrings.Tick += new System.EventHandler(this.tmr_MEStrings_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(6, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbSelectStringIDs);
            this.splitContainer1.Panel1.Controls.Add(this.lblStringID);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelect);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox1);
            this.splitContainer1.Panel1.Controls.Add(this.lbStringIDs);
            this.splitContainer1.Panel1.Controls.Add(this.lblSelectStringIDs);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblUnicodePosition);
            this.splitContainer1.Panel2.Controls.Add(this.lblUnicodeSort);
            this.splitContainer1.Panel2.Controls.Add(this.cbUnicodeSort);
            this.splitContainer1.Panel2.Controls.Add(this.lblUnicode);
            this.splitContainer1.Panel2.Controls.Add(this.cbShowAllUnicodes);
            this.splitContainer1.Panel2.Controls.Add(this.lbUnicodes);
            this.splitContainer1.Size = new System.Drawing.Size(554, 429);
            this.splitContainer1.SplitterDistance = 233;
            this.splitContainer1.TabIndex = 7;
            // 
            // cbSelectStringIDs
            // 
            this.cbSelectStringIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSelectStringIDs.DisplayMember = "ALl";
            this.cbSelectStringIDs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectStringIDs.FormattingEnabled = true;
            this.cbSelectStringIDs.Items.AddRange(new object[] {
            "All",
            "Unicode Strings",
            "No Unicode Strings"});
            this.cbSelectStringIDs.Location = new System.Drawing.Point(387, 200);
            this.cbSelectStringIDs.Name = "cbSelectStringIDs";
            this.cbSelectStringIDs.Size = new System.Drawing.Size(159, 21);
            this.cbSelectStringIDs.TabIndex = 15;
            this.cbSelectStringIDs.SelectedIndexChanged += new System.EventHandler(this.cbSelectStringIDs_SelectedIndexChanged);
            // 
            // lblStringID
            // 
            this.lblStringID.AutoSize = true;
            this.lblStringID.Location = new System.Drawing.Point(5, 7);
            this.lblStringID.Name = "lblStringID";
            this.lblStringID.Size = new System.Drawing.Size(51, 13);
            this.lblStringID.TabIndex = 14;
            this.lblStringID.Text = "String ID:";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(445, 2);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(99, 22);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(240, 183);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(97, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Starts with only";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // lbStringIDs
            // 
            this.lbStringIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbStringIDs.FormattingEnabled = true;
            this.lbStringIDs.Location = new System.Drawing.Point(7, 27);
            this.lbStringIDs.Name = "lbStringIDs";
            this.lbStringIDs.Size = new System.Drawing.Size(537, 147);
            this.lbStringIDs.TabIndex = 7;
            this.lbStringIDs.SelectedIndexChanged += new System.EventHandler(this.lbStringIDs_SelectedIndexChanged);
            // 
            // lblSelectStringIDs
            // 
            this.lblSelectStringIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectStringIDs.AutoSize = true;
            this.lblSelectStringIDs.Location = new System.Drawing.Point(384, 183);
            this.lblSelectStringIDs.Name = "lblSelectStringIDs";
            this.lblSelectStringIDs.Size = new System.Drawing.Size(105, 13);
            this.lblSelectStringIDs.TabIndex = 9;
            this.lblSelectStringIDs.Text = "Show StringIDs with:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.Location = new System.Drawing.Point(8, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(396, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Type all or part of the string to narrow list";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(8, 201);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(329, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(221, 5);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.label1.Size = new System.Drawing.Size(223, 19);
            this.label1.TabIndex = 11;
            this.label1.Text = "Double click the new string to select it or click";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUnicode
            // 
            this.lblUnicode.AutoSize = true;
            this.lblUnicode.Location = new System.Drawing.Point(5, 11);
            this.lblUnicode.Name = "lblUnicode";
            this.lblUnicode.Size = new System.Drawing.Size(50, 13);
            this.lblUnicode.TabIndex = 8;
            this.lblUnicode.Text = "Unicode:";
            // 
            // cbShowAllUnicodes
            // 
            this.cbShowAllUnicodes.AutoSize = true;
            this.cbShowAllUnicodes.Location = new System.Drawing.Point(67, 10);
            this.cbShowAllUnicodes.Name = "cbShowAllUnicodes";
            this.cbShowAllUnicodes.Size = new System.Drawing.Size(114, 17);
            this.cbShowAllUnicodes.TabIndex = 7;
            this.cbShowAllUnicodes.Text = "Show all Unicodes";
            this.cbShowAllUnicodes.UseVisualStyleBackColor = true;
            this.cbShowAllUnicodes.Click += new System.EventHandler(this.cbShowAllUnicodes_CheckedChanged);
            // 
            // lbUnicodes
            // 
            this.lbUnicodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbUnicodes.FormattingEnabled = true;
            this.lbUnicodes.Location = new System.Drawing.Point(8, 30);
            this.lbUnicodes.Name = "lbUnicodes";
            this.lbUnicodes.Size = new System.Drawing.Size(534, 147);
            this.lbUnicodes.TabIndex = 6;
            this.lbUnicodes.SelectedIndexChanged += new System.EventHandler(this.lbUnicodes_SelectedIndexChanged);
            // 
            // cbUnicodeSort
            // 
            this.cbUnicodeSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnicodeSort.FormattingEnabled = true;
            this.cbUnicodeSort.Items.AddRange(new object[] {
            "Name",
            "Offset"});
            this.cbUnicodeSort.Location = new System.Drawing.Point(445, 6);
            this.cbUnicodeSort.Name = "cbUnicodeSort";
            this.cbUnicodeSort.Size = new System.Drawing.Size(97, 21);
            this.cbUnicodeSort.TabIndex = 9;
            this.cbUnicodeSort.SelectedIndexChanged += new System.EventHandler(this.cbUnicodeSort_SelectedIndexChanged);
            // 
            // lblUnicodeSort
            // 
            this.lblUnicodeSort.AutoSize = true;
            this.lblUnicodeSort.Location = new System.Drawing.Point(381, 9);
            this.lblUnicodeSort.Name = "lblUnicodeSort";
            this.lblUnicodeSort.Size = new System.Drawing.Size(58, 13);
            this.lblUnicodeSort.TabIndex = 10;
            this.lblUnicodeSort.Text = "Sort Order:";
            // 
            // lblUnicodePosition
            // 
            this.lblUnicodePosition.AutoSize = true;
            this.lblUnicodePosition.Location = new System.Drawing.Point(237, 11);
            this.lblUnicodePosition.Name = "lblUnicodePosition";
            this.lblUnicodePosition.Size = new System.Drawing.Size(57, 13);
            this.lblUnicodePosition.TabIndex = 11;
            this.lblUnicodePosition.Text = "Unicode #";
            // 
            // MEStringsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 441);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MEStringsSelector";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "String Swap";
            this.Load += new System.EventHandler(this.MEStringsSelector_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Methods

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbSelectStringIDs;
        private System.Windows.Forms.Label lblStringID;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ListBox lbStringIDs;
        private System.Windows.Forms.Label lblSelectStringIDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUnicode;
        private System.Windows.Forms.CheckBox cbShowAllUnicodes;
        private System.Windows.Forms.ListBox lbUnicodes;
        private System.Windows.Forms.Label lblUnicodeSort;
        private System.Windows.Forms.ComboBox cbUnicodeSort;
        private System.Windows.Forms.Label lblUnicodePosition;

    }
}