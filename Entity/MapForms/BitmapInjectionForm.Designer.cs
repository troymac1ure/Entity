namespace entity.MapForms
{
    partial class BitmapInjectionForm
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
            this.lblInjectionBitmap = new System.Windows.Forms.Label();
            this.lbInjectionBitmap = new System.Windows.Forms.ListBox();
            this.btnInject = new System.Windows.Forms.Button();
            this.lbSourceDDS = new System.Windows.Forms.ListBox();
            this.pbSourceDDS = new System.Windows.Forms.PictureBox();
            this.lbDestBitmap = new System.Windows.Forms.ListBox();
            this.pbDestBitmap = new System.Windows.Forms.PictureBox();
            this.lblDDSFile = new System.Windows.Forms.Label();
            this.lblInternalBitmap = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbSourceDDS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDestBitmap)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInjectionBitmap
            // 
            this.lblInjectionBitmap.Location = new System.Drawing.Point(259, 83);
            this.lblInjectionBitmap.Name = "lblInjectionBitmap";
            this.lblInjectionBitmap.Size = new System.Drawing.Size(140, 20);
            this.lblInjectionBitmap.TabIndex = 0;
            this.lblInjectionBitmap.Text = "Select Bitmap # to inject to:";
            // 
            // lbInjectionBitmap
            // 
            this.lbInjectionBitmap.Location = new System.Drawing.Point(262, 106);
            this.lbInjectionBitmap.Name = "lbInjectionBitmap";
            this.lbInjectionBitmap.Size = new System.Drawing.Size(127, 82);
            this.lbInjectionBitmap.TabIndex = 1;
            this.lbInjectionBitmap.SelectedIndexChanged += new System.EventHandler(this.lbInjectionBitmap_SelectedIndexChanged);
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(289, 38);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(75, 23);
            this.btnInject.TabIndex = 2;
            this.btnInject.Text = "&Inject";
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // lbSourceDDS
            // 
            this.lbSourceDDS.Location = new System.Drawing.Point(12, 26);
            this.lbSourceDDS.Name = "lbSourceDDS";
            this.lbSourceDDS.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbSourceDDS.Size = new System.Drawing.Size(149, 69);
            this.lbSourceDDS.TabIndex = 3;
            // 
            // pbSourceDDS
            // 
            this.pbSourceDDS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSourceDDS.Location = new System.Drawing.Point(176, 26);
            this.pbSourceDDS.Name = "pbSourceDDS";
            this.pbSourceDDS.Size = new System.Drawing.Size(70, 70);
            this.pbSourceDDS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSourceDDS.TabIndex = 4;
            this.pbSourceDDS.TabStop = false;
            // 
            // lbDestBitmap
            // 
            this.lbDestBitmap.Location = new System.Drawing.Point(12, 118);
            this.lbDestBitmap.Name = "lbDestBitmap";
            this.lbDestBitmap.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbDestBitmap.Size = new System.Drawing.Size(149, 69);
            this.lbDestBitmap.TabIndex = 5;
            // 
            // pbDestBitmap
            // 
            this.pbDestBitmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbDestBitmap.Location = new System.Drawing.Point(176, 118);
            this.pbDestBitmap.Name = "pbDestBitmap";
            this.pbDestBitmap.Size = new System.Drawing.Size(70, 70);
            this.pbDestBitmap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbDestBitmap.TabIndex = 6;
            this.pbDestBitmap.TabStop = false;
            // 
            // lblDDSFile
            // 
            this.lblDDSFile.AutoSize = true;
            this.lblDDSFile.Location = new System.Drawing.Point(12, 10);
            this.lblDDSFile.Name = "lblDDSFile";
            this.lblDDSFile.Size = new System.Drawing.Size(52, 13);
            this.lblDDSFile.TabIndex = 7;
            this.lblDDSFile.Text = "DDS File:";
            // 
            // lblInternalBitmap
            // 
            this.lblInternalBitmap.AutoSize = true;
            this.lblInternalBitmap.Location = new System.Drawing.Point(12, 102);
            this.lblInternalBitmap.Name = "lblInternalBitmap";
            this.lblInternalBitmap.Size = new System.Drawing.Size(80, 13);
            this.lblInternalBitmap.TabIndex = 8;
            this.lblInternalBitmap.Text = "Internal Bitmap:";
            // 
            // BitmapInjectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 200);
            this.Controls.Add(this.lblInternalBitmap);
            this.Controls.Add(this.lblDDSFile);
            this.Controls.Add(this.lblInjectionBitmap);
            this.Controls.Add(this.lbInjectionBitmap);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.lbSourceDDS);
            this.Controls.Add(this.pbSourceDDS);
            this.Controls.Add(this.lbDestBitmap);
            this.Controls.Add(this.pbDestBitmap);
            this.Name = "BitmapInjectionForm";
            this.Text = "BitmapInjectionForm";
            ((System.ComponentModel.ISupportInitialize)(this.pbSourceDDS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDestBitmap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInjectionBitmap;
        private System.Windows.Forms.ListBox lbInjectionBitmap;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.ListBox lbSourceDDS;
        private System.Windows.Forms.PictureBox pbSourceDDS;
        private System.Windows.Forms.ListBox lbDestBitmap;
        private System.Windows.Forms.PictureBox pbDestBitmap;
        private System.Windows.Forms.Label lblDDSFile;
        private System.Windows.Forms.Label lblInternalBitmap;
    }
}