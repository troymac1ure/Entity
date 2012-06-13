namespace entity.Forms.Meta_Editor
{
    partial class MetaEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaEditor));
            this.identContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripButton();
            this.ButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripShowAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripColorShifting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripTagType = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTagName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripReflexiveNumber = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripInformation = new System.Windows.Forms.ToolStripStatusLabel();
            this.identContext.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // identContext
            // 
            this.identContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToolStripMenuItem,
            this.toolStripSeparator,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.identContext.Name = "identContext";
            this.identContext.Size = new System.Drawing.Size(187, 164);
            this.identContext.Opening += new System.ComponentModel.CancelEventHandler(this.identContext_Opening);
            // 
            // gotoToolStripMenuItem
            // 
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.gotoToolStripMenuItem.Text = "Go to";
            this.gotoToolStripMenuItem.Visible = false;
            this.gotoToolStripMenuItem.Click += new System.EventHandler(this.gotoToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(176, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.DarkGray;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.ButtonSave,
            this.toolStripSplitButton1,
            this.toolStripButton1,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 2);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(581, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLabel1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabel1.Image")));
            this.toolStripLabel1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "[ Manual]";
            this.toolStripLabel1.Click += new System.EventHandler(this.toolStripLabel1_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ButtonSave.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.ButtonSave.Size = new System.Drawing.Size(45, 22);
            this.ButtonSave.Text = "Save";
            this.ButtonSave.Click += new System.EventHandler(this.SaveButton_DropDown);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripShowAll,
            this.toolStripColorShifting,
            this.toolStripMenuItem3});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(48, 22);
            this.toolStripSplitButton1.Text = "Tools";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(200, 22);
            this.toolStripMenuItem2.Text = "Item Selector/Ent Editor";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripShowAll
            // 
            this.toolStripShowAll.Name = "toolStripShowAll";
            this.toolStripShowAll.Size = new System.Drawing.Size(200, 22);
            this.toolStripShowAll.Text = "Show.....";
            this.toolStripShowAll.Click += new System.EventHandler(this.toolStripShowAll_Click);
            // 
            // toolStripColorShifting
            // 
            this.toolStripColorShifting.Name = "toolStripColorShifting";
            this.toolStripColorShifting.Size = new System.Drawing.Size(200, 22);
            this.toolStripColorShifting.Text = "Color Shifting";
            this.toolStripColorShifting.Click += new System.EventHandler(this.toolStripColorShifting_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(200, 22);
            this.toolStripMenuItem3.Text = "About";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripButton1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripButton1.Size = new System.Drawing.Size(71, 22);
            this.toolStripButton1.Text = "Sync Xbox";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(10, 35);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panel1.Size = new System.Drawing.Size(561, 524);
            this.panel1.TabIndex = 10;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTagType,
            this.toolStripTagName,
            this.toolStripReflexiveNumber,
            this.toolStripInformation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 710);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(581, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripTagType
            // 
            this.toolStripTagType.AutoSize = false;
            this.toolStripTagType.BackColor = System.Drawing.Color.Transparent;
            this.toolStripTagType.Name = "toolStripTagType";
            this.toolStripTagType.Size = new System.Drawing.Size(36, 17);
            this.toolStripTagType.Text = "[TYPE]";
            // 
            // toolStripTagName
            // 
            this.toolStripTagName.BackColor = System.Drawing.Color.Transparent;
            this.toolStripTagName.Name = "toolStripTagName";
            this.toolStripTagName.Size = new System.Drawing.Size(427, 17);
            this.toolStripTagName.Spring = true;
            this.toolStripTagName.Text = "[TAG]";
            this.toolStripTagName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripReflexiveNumber
            // 
            this.toolStripReflexiveNumber.BackColor = System.Drawing.Color.Transparent;
            this.toolStripReflexiveNumber.Name = "toolStripReflexiveNumber";
            this.toolStripReflexiveNumber.Size = new System.Drawing.Size(56, 17);
            this.toolStripReflexiveNumber.Text = "Main (0/0)";
            this.toolStripReflexiveNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripInformation
            // 
            this.toolStripInformation.BackColor = System.Drawing.Color.Transparent;
            this.toolStripInformation.ForeColor = System.Drawing.SystemColors.GrayText;
            this.toolStripInformation.Name = "toolStripInformation";
            this.toolStripInformation.Size = new System.Drawing.Size(47, 17);
            this.toolStripInformation.Text = ".: Idle :.";
            this.toolStripInformation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MetaEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "MetaEditor";
            this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.Size = new System.Drawing.Size(581, 732);
            this.SizeChanged += new System.EventHandler(this.MetaEditor_SizeChanged);
            this.identContext.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ContextMenuStrip identContext;
        private System.Windows.Forms.ToolStripMenuItem gotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ButtonSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripShowAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripColorShifting;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTagName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripInformation;
        private System.Windows.Forms.ToolStripStatusLabel toolStripReflexiveNumber;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTagType;
    }
}
