namespace entity.HexEditor
{
    partial class GoToDialogBox
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
            this.label1 = new System.Windows.Forms.Label();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.decimalbutton = new System.Windows.Forms.RadioButton();
            this.hexadecimalbutton = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-9, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Go To:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maskedTextBox1.Location = new System.Drawing.Point(43, 3);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(139, 20);
            this.maskedTextBox1.TabIndex = 2;
            // 
            // decimalbutton
            // 
            this.decimalbutton.AutoSize = true;
            this.decimalbutton.Checked = true;
            this.decimalbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.decimalbutton.Location = new System.Drawing.Point(12, 29);
            this.decimalbutton.Name = "decimalbutton";
            this.decimalbutton.Size = new System.Drawing.Size(62, 17);
            this.decimalbutton.TabIndex = 3;
            this.decimalbutton.TabStop = true;
            this.decimalbutton.Text = "Decimal";
            this.decimalbutton.UseVisualStyleBackColor = true;
            this.decimalbutton.Click += new System.EventHandler(this.decimalbutton_CheckedChanged);
            // 
            // hexadecimalbutton
            // 
            this.hexadecimalbutton.AutoSize = true;
            this.hexadecimalbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hexadecimalbutton.Location = new System.Drawing.Point(101, 29);
            this.hexadecimalbutton.Name = "hexadecimalbutton";
            this.hexadecimalbutton.Size = new System.Drawing.Size(85, 17);
            this.hexadecimalbutton.TabIndex = 4;
            this.hexadecimalbutton.Text = "Hexadecimal";
            this.hexadecimalbutton.UseVisualStyleBackColor = true;
            this.hexadecimalbutton.Click += new System.EventHandler(this.hexadecimalbutton_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Go To Offset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GoToDialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(188, 73);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.hexadecimalbutton);
            this.Controls.Add(this.decimalbutton);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GoToDialogBox";
            this.Text = "GoToDialogBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.RadioButton decimalbutton;
        private System.Windows.Forms.RadioButton hexadecimalbutton;
        private System.Windows.Forms.Button button1;
    }
}