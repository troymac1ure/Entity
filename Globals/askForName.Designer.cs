namespace Globals
{
    partial class askForName
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
            this.tbNewName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbOldName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbNewName
            // 
            this.tbNewName.Location = new System.Drawing.Point(90, 34);
            this.tbNewName.Name = "tbNewName";
            this.tbNewName.Size = new System.Drawing.Size(444, 20);
            this.tbNewName.TabIndex = 0;
            this.tbNewName.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "New Name";
            // 
            // createButton
            // 
            this.createButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.createButton.Location = new System.Drawing.Point(190, 60);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(204, 23);
            this.createButton.TabIndex = 2;
            this.createButton.Text = "Create New Tag";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Old Name";
            // 
            // tbOldName
            // 
            this.tbOldName.Enabled = false;
            this.tbOldName.Location = new System.Drawing.Point(90, 6);
            this.tbOldName.Name = "tbOldName";
            this.tbOldName.Size = new System.Drawing.Size(444, 20);
            this.tbOldName.TabIndex = 4;
            // 
            // askForName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 90);
            this.Controls.Add(this.tbOldName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbNewName);
            this.Name = "askForName";
            this.Text = "askForName";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.askForName_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox tbNewName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tbOldName;
    }
}