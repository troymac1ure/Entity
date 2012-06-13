namespace entity.MetaEditorPlus
{
    partial class myTabControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.myTab1 = new entity.MetaEditorPlus.myTab();
            this.myTab2 = new entity.MetaEditorPlus.myTab();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(772, 368);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(28, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.myTab1);
            this.panel2.Controls.Add(this.myTab2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(772, 25);
            this.panel2.TabIndex = 3;
            // 
            // myTab1
            // 
            this.myTab1.BackColor = System.Drawing.Color.Transparent;
            this.myTab1.Location = new System.Drawing.Point(0, 0);
            this.myTab1.Margin = new System.Windows.Forms.Padding(0);
            this.myTab1.Name = "myTab1";
            this.myTab1.Size = new System.Drawing.Size(146, 25);
            this.myTab1.TabIndex = 3;
            this.myTab1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // myTab2
            // 
            this.myTab2.BackColor = System.Drawing.Color.Transparent;
            this.myTab2.Location = new System.Drawing.Point(136, 0);
            this.myTab2.Margin = new System.Windows.Forms.Padding(0);
            this.myTab2.Name = "myTab2";
            this.myTab2.Size = new System.Drawing.Size(147, 25);
            this.myTab2.TabIndex = 4;
            this.myTab2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // myTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 393);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "myTabControl";
            this.Text = "Temp";
            this.Load += new System.EventHandler(this.myTabControl_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private myTab myTab1;
        private myTab myTab2;
    }
}