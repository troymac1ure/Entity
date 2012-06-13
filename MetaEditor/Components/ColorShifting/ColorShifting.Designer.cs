namespace MetaEditor.ColorShifting
{
    partial class ColorShifting
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
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.hScrlBRed = new System.Windows.Forms.HScrollBar();
            this.hScrlBBlue = new System.Windows.Forms.HScrollBar();
            this.hScrlBGreen = new System.Windows.Forms.HScrollBar();
            this.txtBRed = new System.Windows.Forms.TextBox();
            this.txtBGreen = new System.Windows.Forms.TextBox();
            this.txtBBlue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labStartingColorPreview = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtbBlueToShift = new System.Windows.Forms.TextBox();
            this.txtbGreenToShift = new System.Windows.Forms.TextBox();
            this.txtbRedToShift = new System.Windows.Forms.TextBox();
            this.hScrlBGreenToShift = new System.Windows.Forms.HScrollBar();
            this.hScrlBBlueToShift = new System.Windows.Forms.HScrollBar();
            this.hScrlBRedToShift = new System.Windows.Forms.HScrollBar();
            this.labLayers1 = new System.Windows.Forms.Label();
            this.labLayers2 = new System.Windows.Forms.Label();
            this.labLayers3 = new System.Windows.Forms.Label();
            this.labLayers4 = new System.Windows.Forms.Label();
            this.labLayers5 = new System.Windows.Forms.Label();
            this.labLayers6 = new System.Windows.Forms.Label();
            this.labLayers7 = new System.Windows.Forms.Label();
            this.labLayers8 = new System.Windows.Forms.Label();
            this.labLayers9 = new System.Windows.Forms.Label();
            this.labLayers10 = new System.Windows.Forms.Label();
            this.labLayers11 = new System.Windows.Forms.Label();
            this.labLayers12 = new System.Windows.Forms.Label();
            this.labLayers13 = new System.Windows.Forms.Label();
            this.labLayers14 = new System.Windows.Forms.Label();
            this.labLayers15 = new System.Windows.Forms.Label();
            this.labLayers16 = new System.Windows.Forms.Label();
            this.labLayers17 = new System.Windows.Forms.Label();
            this.labLayers18 = new System.Windows.Forms.Label();
            this.labLayers19 = new System.Windows.Forms.Label();
            this.buttSave = new System.Windows.Forms.Button();
            this.buttCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // hScrlBRed
            // 
            this.hScrlBRed.Location = new System.Drawing.Point(105, 29);
            this.hScrlBRed.Maximum = 255;
            this.hScrlBRed.Name = "hScrlBRed";
            this.hScrlBRed.Size = new System.Drawing.Size(109, 17);
            this.hScrlBRed.TabIndex = 0;
            this.hScrlBRed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBRed_Scroll);
            // 
            // hScrlBBlue
            // 
            this.hScrlBBlue.Location = new System.Drawing.Point(105, 81);
            this.hScrlBBlue.Maximum = 255;
            this.hScrlBBlue.Name = "hScrlBBlue";
            this.hScrlBBlue.Size = new System.Drawing.Size(109, 17);
            this.hScrlBBlue.TabIndex = 1;
            this.hScrlBBlue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBBlue_Scroll);
            // 
            // hScrlBGreen
            // 
            this.hScrlBGreen.Location = new System.Drawing.Point(105, 55);
            this.hScrlBGreen.Maximum = 255;
            this.hScrlBGreen.Name = "hScrlBGreen";
            this.hScrlBGreen.Size = new System.Drawing.Size(109, 17);
            this.hScrlBGreen.TabIndex = 2;
            this.hScrlBGreen.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBGreen_Scroll);
            // 
            // txtBRed
            // 
            this.txtBRed.BackColor = System.Drawing.SystemColors.Window;
            this.txtBRed.Location = new System.Drawing.Point(217, 26);
            this.txtBRed.Name = "txtBRed";
            this.txtBRed.Size = new System.Drawing.Size(44, 20);
            this.txtBRed.TabIndex = 3;
            this.txtBRed.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // txtBGreen
            // 
            this.txtBGreen.BackColor = System.Drawing.SystemColors.Window;
            this.txtBGreen.Location = new System.Drawing.Point(217, 52);
            this.txtBGreen.Name = "txtBGreen";
            this.txtBGreen.Size = new System.Drawing.Size(44, 20);
            this.txtBGreen.TabIndex = 4;
            this.txtBGreen.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // txtBBlue
            // 
            this.txtBBlue.BackColor = System.Drawing.SystemColors.Window;
            this.txtBBlue.Location = new System.Drawing.Point(217, 78);
            this.txtBBlue.Name = "txtBBlue";
            this.txtBBlue.Size = new System.Drawing.Size(44, 20);
            this.txtBBlue.TabIndex = 5;
            this.txtBBlue.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Red To Start At";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Blue To Start At";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Green To Start At";
            // 
            // labStartingColorPreview
            // 
            this.labStartingColorPreview.BackColor = System.Drawing.SystemColors.Control;
            this.labStartingColorPreview.Location = new System.Drawing.Point(267, 26);
            this.labStartingColorPreview.Name = "labStartingColorPreview";
            this.labStartingColorPreview.Size = new System.Drawing.Size(22, 72);
            this.labStartingColorPreview.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(295, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Green To Shift";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Blue To Shift";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(295, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Red To Shift";
            // 
            // txtbBlueToShift
            // 
            this.txtbBlueToShift.BackColor = System.Drawing.SystemColors.Window;
            this.txtbBlueToShift.Location = new System.Drawing.Point(486, 78);
            this.txtbBlueToShift.Name = "txtbBlueToShift";
            this.txtbBlueToShift.Size = new System.Drawing.Size(44, 20);
            this.txtbBlueToShift.TabIndex = 15;
            this.txtbBlueToShift.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // txtbGreenToShift
            // 
            this.txtbGreenToShift.BackColor = System.Drawing.SystemColors.Window;
            this.txtbGreenToShift.Location = new System.Drawing.Point(486, 52);
            this.txtbGreenToShift.Name = "txtbGreenToShift";
            this.txtbGreenToShift.Size = new System.Drawing.Size(44, 20);
            this.txtbGreenToShift.TabIndex = 14;
            this.txtbGreenToShift.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // txtbRedToShift
            // 
            this.txtbRedToShift.BackColor = System.Drawing.SystemColors.Window;
            this.txtbRedToShift.Location = new System.Drawing.Point(486, 26);
            this.txtbRedToShift.Name = "txtbRedToShift";
            this.txtbRedToShift.Size = new System.Drawing.Size(44, 20);
            this.txtbRedToShift.TabIndex = 13;
            this.txtbRedToShift.TextChanged += new System.EventHandler(this.txtBRed_TextChanged);
            // 
            // hScrlBGreenToShift
            // 
            this.hScrlBGreenToShift.Location = new System.Drawing.Point(374, 51);
            this.hScrlBGreenToShift.Maximum = 255;
            this.hScrlBGreenToShift.Minimum = -255;
            this.hScrlBGreenToShift.Name = "hScrlBGreenToShift";
            this.hScrlBGreenToShift.Size = new System.Drawing.Size(109, 17);
            this.hScrlBGreenToShift.TabIndex = 12;
            this.hScrlBGreenToShift.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBGreenToShift_Scroll);
            // 
            // hScrlBBlueToShift
            // 
            this.hScrlBBlueToShift.Location = new System.Drawing.Point(374, 77);
            this.hScrlBBlueToShift.Maximum = 255;
            this.hScrlBBlueToShift.Minimum = -255;
            this.hScrlBBlueToShift.Name = "hScrlBBlueToShift";
            this.hScrlBBlueToShift.Size = new System.Drawing.Size(109, 17);
            this.hScrlBBlueToShift.TabIndex = 11;
            this.hScrlBBlueToShift.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBBlueToShift_Scroll);
            // 
            // hScrlBRedToShift
            // 
            this.hScrlBRedToShift.Location = new System.Drawing.Point(374, 25);
            this.hScrlBRedToShift.Maximum = 255;
            this.hScrlBRedToShift.Minimum = -255;
            this.hScrlBRedToShift.Name = "hScrlBRedToShift";
            this.hScrlBRedToShift.Size = new System.Drawing.Size(109, 17);
            this.hScrlBRedToShift.TabIndex = 10;
            this.hScrlBRedToShift.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrlBRedToShift_Scroll);
            // 
            // labLayers1
            // 
            this.labLayers1.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers1.Location = new System.Drawing.Point(12, 124);
            this.labLayers1.Name = "labLayers1";
            this.labLayers1.Size = new System.Drawing.Size(22, 72);
            this.labLayers1.TabIndex = 19;
            this.labLayers1.Text = "1";
            // 
            // labLayers2
            // 
            this.labLayers2.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers2.Location = new System.Drawing.Point(40, 124);
            this.labLayers2.Name = "labLayers2";
            this.labLayers2.Size = new System.Drawing.Size(22, 72);
            this.labLayers2.TabIndex = 20;
            this.labLayers2.Text = "2";
            // 
            // labLayers3
            // 
            this.labLayers3.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers3.Location = new System.Drawing.Point(68, 124);
            this.labLayers3.Name = "labLayers3";
            this.labLayers3.Size = new System.Drawing.Size(22, 72);
            this.labLayers3.TabIndex = 21;
            this.labLayers3.Text = "3";
            // 
            // labLayers4
            // 
            this.labLayers4.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers4.Location = new System.Drawing.Point(96, 124);
            this.labLayers4.Name = "labLayers4";
            this.labLayers4.Size = new System.Drawing.Size(22, 72);
            this.labLayers4.TabIndex = 22;
            this.labLayers4.Text = "4";
            // 
            // labLayers5
            // 
            this.labLayers5.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers5.Location = new System.Drawing.Point(124, 124);
            this.labLayers5.Name = "labLayers5";
            this.labLayers5.Size = new System.Drawing.Size(22, 72);
            this.labLayers5.TabIndex = 23;
            this.labLayers5.Text = "5";
            // 
            // labLayers6
            // 
            this.labLayers6.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers6.Location = new System.Drawing.Point(152, 124);
            this.labLayers6.Name = "labLayers6";
            this.labLayers6.Size = new System.Drawing.Size(22, 72);
            this.labLayers6.TabIndex = 24;
            this.labLayers6.Text = "6";
            // 
            // labLayers7
            // 
            this.labLayers7.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers7.Location = new System.Drawing.Point(180, 124);
            this.labLayers7.Name = "labLayers7";
            this.labLayers7.Size = new System.Drawing.Size(22, 72);
            this.labLayers7.TabIndex = 25;
            this.labLayers7.Text = "7";
            // 
            // labLayers8
            // 
            this.labLayers8.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers8.Location = new System.Drawing.Point(208, 124);
            this.labLayers8.Name = "labLayers8";
            this.labLayers8.Size = new System.Drawing.Size(22, 72);
            this.labLayers8.TabIndex = 26;
            this.labLayers8.Text = "8";
            // 
            // labLayers9
            // 
            this.labLayers9.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers9.Location = new System.Drawing.Point(236, 124);
            this.labLayers9.Name = "labLayers9";
            this.labLayers9.Size = new System.Drawing.Size(22, 72);
            this.labLayers9.TabIndex = 27;
            this.labLayers9.Text = "9";
            // 
            // labLayers10
            // 
            this.labLayers10.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers10.Location = new System.Drawing.Point(264, 124);
            this.labLayers10.Name = "labLayers10";
            this.labLayers10.Size = new System.Drawing.Size(22, 72);
            this.labLayers10.TabIndex = 28;
            this.labLayers10.Text = "10";
            // 
            // labLayers11
            // 
            this.labLayers11.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers11.Location = new System.Drawing.Point(292, 124);
            this.labLayers11.Name = "labLayers11";
            this.labLayers11.Size = new System.Drawing.Size(22, 72);
            this.labLayers11.TabIndex = 29;
            this.labLayers11.Text = "11";
            // 
            // labLayers12
            // 
            this.labLayers12.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers12.Location = new System.Drawing.Point(320, 124);
            this.labLayers12.Name = "labLayers12";
            this.labLayers12.Size = new System.Drawing.Size(22, 72);
            this.labLayers12.TabIndex = 30;
            this.labLayers12.Text = "12";
            // 
            // labLayers13
            // 
            this.labLayers13.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers13.Location = new System.Drawing.Point(348, 124);
            this.labLayers13.Name = "labLayers13";
            this.labLayers13.Size = new System.Drawing.Size(22, 72);
            this.labLayers13.TabIndex = 31;
            this.labLayers13.Text = "13";
            // 
            // labLayers14
            // 
            this.labLayers14.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers14.Location = new System.Drawing.Point(376, 124);
            this.labLayers14.Name = "labLayers14";
            this.labLayers14.Size = new System.Drawing.Size(22, 72);
            this.labLayers14.TabIndex = 32;
            this.labLayers14.Text = "14";
            // 
            // labLayers15
            // 
            this.labLayers15.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers15.Location = new System.Drawing.Point(404, 124);
            this.labLayers15.Name = "labLayers15";
            this.labLayers15.Size = new System.Drawing.Size(22, 72);
            this.labLayers15.TabIndex = 33;
            this.labLayers15.Text = "15";
            // 
            // labLayers16
            // 
            this.labLayers16.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers16.Location = new System.Drawing.Point(432, 124);
            this.labLayers16.Name = "labLayers16";
            this.labLayers16.Size = new System.Drawing.Size(22, 72);
            this.labLayers16.TabIndex = 34;
            this.labLayers16.Text = "16";
            // 
            // labLayers17
            // 
            this.labLayers17.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers17.Location = new System.Drawing.Point(460, 124);
            this.labLayers17.Name = "labLayers17";
            this.labLayers17.Size = new System.Drawing.Size(22, 72);
            this.labLayers17.TabIndex = 35;
            this.labLayers17.Text = "17";
            // 
            // labLayers18
            // 
            this.labLayers18.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers18.Location = new System.Drawing.Point(488, 124);
            this.labLayers18.Name = "labLayers18";
            this.labLayers18.Size = new System.Drawing.Size(22, 72);
            this.labLayers18.TabIndex = 36;
            this.labLayers18.Text = "18";
            // 
            // labLayers19
            // 
            this.labLayers19.BackColor = System.Drawing.SystemColors.Control;
            this.labLayers19.Location = new System.Drawing.Point(516, 124);
            this.labLayers19.Name = "labLayers19";
            this.labLayers19.Size = new System.Drawing.Size(22, 72);
            this.labLayers19.TabIndex = 37;
            this.labLayers19.Text = "19";
            // 
            // buttSave
            // 
            this.buttSave.Location = new System.Drawing.Point(374, 207);
            this.buttSave.Name = "buttSave";
            this.buttSave.Size = new System.Drawing.Size(75, 23);
            this.buttSave.TabIndex = 38;
            this.buttSave.Text = "Save";
            this.buttSave.UseVisualStyleBackColor = true;
            this.buttSave.Click += new System.EventHandler(this.buttSave_Click);
            // 
            // buttCancel
            // 
            this.buttCancel.Location = new System.Drawing.Point(463, 207);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.Size = new System.Drawing.Size(75, 23);
            this.buttCancel.TabIndex = 39;
            this.buttCancel.Text = "Cancel";
            this.buttCancel.UseVisualStyleBackColor = true;
            this.buttCancel.Click += new System.EventHandler(this.buttCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(176, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(186, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Preview Reflexive Layer Color Shifting";
            // 
            // ColorShifting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 242);
            this.Controls.Add(this.labLayers19);
            this.Controls.Add(this.labLayers18);
            this.Controls.Add(this.labLayers17);
            this.Controls.Add(this.labLayers16);
            this.Controls.Add(this.labLayers15);
            this.Controls.Add(this.labLayers14);
            this.Controls.Add(this.labLayers13);
            this.Controls.Add(this.labLayers12);
            this.Controls.Add(this.labLayers11);
            this.Controls.Add(this.labLayers10);
            this.Controls.Add(this.labLayers9);
            this.Controls.Add(this.labLayers8);
            this.Controls.Add(this.labLayers7);
            this.Controls.Add(this.labLayers6);
            this.Controls.Add(this.labLayers5);
            this.Controls.Add(this.labLayers4);
            this.Controls.Add(this.labLayers3);
            this.Controls.Add(this.labLayers2);
            this.Controls.Add(this.labLayers1);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtbBlueToShift);
            this.Controls.Add(this.txtbGreenToShift);
            this.Controls.Add(this.txtbRedToShift);
            this.Controls.Add(this.hScrlBGreenToShift);
            this.Controls.Add(this.hScrlBBlueToShift);
            this.Controls.Add(this.hScrlBRedToShift);
            this.Controls.Add(this.labStartingColorPreview);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBBlue);
            this.Controls.Add(this.txtBGreen);
            this.Controls.Add(this.txtBRed);
            this.Controls.Add(this.hScrlBGreen);
            this.Controls.Add(this.hScrlBBlue);
            this.Controls.Add(this.hScrlBRed);
            this.Name = "ColorShifting";
            this.Text = "Customize Color Shifting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.HScrollBar hScrlBRed;
        private System.Windows.Forms.HScrollBar hScrlBBlue;
        private System.Windows.Forms.HScrollBar hScrlBGreen;
        private System.Windows.Forms.TextBox txtBRed;
        private System.Windows.Forms.TextBox txtBGreen;
        private System.Windows.Forms.TextBox txtBBlue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labStartingColorPreview;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtbBlueToShift;
        private System.Windows.Forms.TextBox txtbGreenToShift;
        private System.Windows.Forms.TextBox txtbRedToShift;
        private System.Windows.Forms.HScrollBar hScrlBGreenToShift;
        private System.Windows.Forms.HScrollBar hScrlBBlueToShift;
        private System.Windows.Forms.HScrollBar hScrlBRedToShift;
        private System.Windows.Forms.Label labLayers1;
        private System.Windows.Forms.Label labLayers2;
        private System.Windows.Forms.Label labLayers3;
        private System.Windows.Forms.Label labLayers4;
        private System.Windows.Forms.Label labLayers5;
        private System.Windows.Forms.Label labLayers6;
        private System.Windows.Forms.Label labLayers7;
        private System.Windows.Forms.Label labLayers8;
        private System.Windows.Forms.Label labLayers9;
        private System.Windows.Forms.Label labLayers10;
        private System.Windows.Forms.Label labLayers11;
        private System.Windows.Forms.Label labLayers12;
        private System.Windows.Forms.Label labLayers13;
        private System.Windows.Forms.Label labLayers14;
        private System.Windows.Forms.Label labLayers15;
        private System.Windows.Forms.Label labLayers16;
        private System.Windows.Forms.Label labLayers17;
        private System.Windows.Forms.Label labLayers18;
        private System.Windows.Forms.Label labLayers19;
        public System.Windows.Forms.Button buttSave;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.Label label4;
    }
}