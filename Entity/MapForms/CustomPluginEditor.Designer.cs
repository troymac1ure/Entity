namespace entity.MapForms
{
    partial class CustomPluginEditor
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.comboBoxPluginName = new System.Windows.Forms.ComboBox();
            this.saveTagBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.treeViewTags = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(102, 69);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(397, 406);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // comboBoxPluginName
            // 
            this.comboBoxPluginName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPluginName.FormattingEnabled = true;
            this.comboBoxPluginName.Location = new System.Drawing.Point(12, 10);
            this.comboBoxPluginName.Name = "comboBoxPluginName";
            this.comboBoxPluginName.Size = new System.Drawing.Size(172, 21);
            this.comboBoxPluginName.TabIndex = 2;
            this.comboBoxPluginName.SelectedIndexChanged += new System.EventHandler(this.comboBoxPluginName_SelectedIndexChanged);
            // 
            // saveTagBtn
            // 
            this.saveTagBtn.Location = new System.Drawing.Point(428, 45);
            this.saveTagBtn.Name = "saveTagBtn";
            this.saveTagBtn.Size = new System.Drawing.Size(71, 25);
            this.saveTagBtn.TabIndex = 3;
            this.saveTagBtn.Text = "&Save Tag";
            this.saveTagBtn.UseVisualStyleBackColor = true;
            this.saveTagBtn.Click += new System.EventHandler(this.savetagTypeBtn_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(199, 7);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 25);
            this.button2.TabIndex = 4;
            this.button2.Text = "&Rename";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.renameBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Visible Tags";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(271, 7);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(71, 25);
            this.button3.TabIndex = 6;
            this.button3.Text = "D&uplicate";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.dupBtn_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(348, 7);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(71, 25);
            this.button4.TabIndex = 7;
            this.button4.Text = "&Delete";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.delBtn_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(203, 45);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(74, 21);
            this.button5.TabIndex = 8;
            this.button5.Text = "Select &All";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(283, 45);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(74, 21);
            this.button6.TabIndex = 9;
            this.button6.Text = "Select &None";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // treeViewTags
            // 
            this.treeViewTags.CheckBoxes = true;
            this.treeViewTags.FullRowSelect = true;
            this.treeViewTags.HideSelection = false;
            this.treeViewTags.Location = new System.Drawing.Point(6, 69);
            this.treeViewTags.Name = "treeViewTags";
            this.treeViewTags.ShowRootLines = false;
            this.treeViewTags.Size = new System.Drawing.Size(90, 406);
            this.treeViewTags.TabIndex = 10;
            this.treeViewTags.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTags_AfterCheck);
            this.treeViewTags.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTags_AfterSelect);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Tag Listing";
            // 
            // CustomPluginEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 482);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.treeViewTags);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.saveTagBtn);
            this.Controls.Add(this.comboBoxPluginName);
            this.Controls.Add(this.treeView1);
            this.Name = "CustomPluginEditor";
            this.Text = "CustomPluginEditor";
            this.Load += new System.EventHandler(this.CustomPluginEditor_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CustomPluginEditor_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        public  System.Windows.Forms.ComboBox comboBoxPluginName;
        private System.Windows.Forms.Button saveTagBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TreeView treeViewTags;
        private System.Windows.Forms.Label label2;
    }
}