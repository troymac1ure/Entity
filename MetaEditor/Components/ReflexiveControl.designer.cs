namespace MetaEditor.Components
{
    partial class ReflexiveControl
    {
        #region Fields

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Panel bottomPanel;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStripMenuItem copyToAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.Button deleteAllButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button duplicateButton;
        private System.Windows.Forms.ToolStripMenuItem gotoToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip identContext;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.Button tempButton;
        private System.Windows.Forms.ComboBox tempComboBox;
        private System.Windows.Forms.Label tempLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.Panel topPanel;

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
            this.identContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.tempButton = new System.Windows.Forms.Button();
            this.tempLabel = new System.Windows.Forms.Label();
            this.tempComboBox = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.insertButton = new System.Windows.Forms.Button();
            this.duplicateButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.deleteAllButton = new System.Windows.Forms.Button();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.identContext.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // identContext
            //
            this.identContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToolStripMenuItem,
            this.copyToAllToolStripMenuItem,
            this.toolStripSeparator,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.identContext.Name = "identContext";
            this.identContext.Size = new System.Drawing.Size(117, 120);
            this.identContext.Opening += new System.ComponentModel.CancelEventHandler(this.identContext_Opening);
            //
            // gotoToolStripMenuItem
            //
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.gotoToolStripMenuItem.Text = "Go to";
            this.gotoToolStripMenuItem.Visible = false;
            this.gotoToolStripMenuItem.Click += new System.EventHandler(this.gotoToolStripMenuItem_Click);
            //
            // copyToAllToolStripMenuItem
            //
            this.copyToAllToolStripMenuItem.Name = "copyToAllToolStripMenuItem";
            this.copyToAllToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.copyToAllToolStripMenuItem.Text = "Copy to all reflexives";
            this.copyToAllToolStripMenuItem.Click += new System.EventHandler(copyToAllToolStripMenuItem_Click);
            //
            // toolStripSeparator
            //
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(113, 6);
            //
            // cutToolStripMenuItem
            //
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            //
            // copyToolStripMenuItem
            //
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            //
            // pasteToolStripMenuItem
            //
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            //
            // deleteToolStripMenuItem
            //
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            //
            // topPanel
            //
            this.topPanel.BackColor = System.Drawing.Color.DarkGray;
            this.topPanel.Controls.Add(this.tempButton);
            this.topPanel.Controls.Add(this.tempLabel);
            this.topPanel.Controls.Add(this.tempComboBox);
            /*
            this.topPanel.Controls.Add(this.addButton);
            this.topPanel.Controls.Add(this.insertButton);
            this.topPanel.Controls.Add(this.duplicateButton);
            this.topPanel.Controls.Add(this.deleteButton);
            this.topPanel.Controls.Add(this.deleteAllButton);
            */
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.MinimumSize = new System.Drawing.Size(0, 28);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(713, 28);
            this.topPanel.TabIndex = 0;
            //
            // tempButton
            //
            this.tempButton.BackColor = System.Drawing.Color.LightGray;
            this.tempButton.Location = new System.Drawing.Point(5, 1);
            this.tempButton.Name = "tempButton";
            this.tempButton.Size = new System.Drawing.Size(28, 22);
            this.tempButton.TabIndex = 0;
            this.tempButton.Text = "-";
            this.tempButton.UseVisualStyleBackColor = false;
            this.tempButton.Click += new System.EventHandler(this.ReflexiveButton_Click);
            //
            // tempLabel
            //
            this.tempLabel.Location = new System.Drawing.Point(34, 6);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(160, 13);
            this.tempLabel.TabIndex = 1;
            this.tempLabel.Text = "reflexiveLabel";
            //
            // tempComboBox
            //
            this.tempComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tempComboBox.FormattingEnabled = true;
            this.tempComboBox.Location = new System.Drawing.Point(200, 2);
            this.tempComboBox.Name = "tempComboBox";
            this.tempComboBox.Size = new System.Drawing.Size(140, 21);
            this.tempComboBox.TabIndex = 2;
            this.tempComboBox.SelectedIndexChanged += new System.EventHandler(this.ReflexiveLoader_Close);
            this.tempComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tempComboBox_KeyPress);
            this.tempComboBox.DropDown += new System.EventHandler(this.ReflexiveLoader_DropDown);
            //
            // addButton
            //
            this.addButton.BackColor = System.Drawing.Color.LightGray;
            this.addButton.Location = new System.Drawing.Point(350, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(40, 22);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            //
            // insertButton
            //
            this.insertButton.BackColor = System.Drawing.Color.LightGray;
            this.insertButton.Location = new System.Drawing.Point(396, 3);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(55, 22);
            this.insertButton.TabIndex = 4;
            this.insertButton.Text = "Insert";
            this.insertButton.UseVisualStyleBackColor = false;
            //
            // duplicateButton
            //
            this.duplicateButton.BackColor = System.Drawing.Color.LightGray;
            this.duplicateButton.Location = new System.Drawing.Point(457, 3);
            this.duplicateButton.Name = "duplicateButton";
            this.duplicateButton.Size = new System.Drawing.Size(60, 22);
            this.duplicateButton.TabIndex = 5;
            this.duplicateButton.Text = "Duplicate";
            this.duplicateButton.UseVisualStyleBackColor = false;
            //
            // deleteButton
            //
            this.deleteButton.BackColor = System.Drawing.Color.LightGray;
            this.deleteButton.Location = new System.Drawing.Point(523, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(55, 22);
            this.deleteButton.TabIndex = 6;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = false;
            //
            // deleteAllButton
            //
            this.deleteAllButton.BackColor = System.Drawing.Color.LightGray;
            this.deleteAllButton.Location = new System.Drawing.Point(584, 3);
            this.deleteAllButton.Name = "deleteAllButton";
            this.deleteAllButton.Size = new System.Drawing.Size(65, 22);
            this.deleteAllButton.TabIndex = 7;
            this.deleteAllButton.Text = "Delete All";
            this.deleteAllButton.UseVisualStyleBackColor = false;
            //
            // bottomPanel
            //
            this.bottomPanel.AutoSize = true;
            this.bottomPanel.BackColor = System.Drawing.Color.LightGray;
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.bottomPanel.Location = new System.Drawing.Point(0, 28);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(713, 0);
            this.bottomPanel.TabIndex = 1;
            //
            // ReflexiveControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.MinimumSize = new System.Drawing.Size(4, 28);
            this.Name = "ReflexiveControl";
            this.Size = new System.Drawing.Size(713, 75);
            this.identContext.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Methods
    }
}