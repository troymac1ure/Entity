namespace entity.BitmapOps
{
    partial class BitmapOperations
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
            this.comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.radioButton32Bit = new System.Windows.Forms.RadioButton();
            this.groupBoxBPP = new System.Windows.Forms.GroupBox();
            this.radioButton8Bit = new System.Windows.Forms.RadioButton();
            this.radioButton16Bit = new System.Windows.Forms.RadioButton();
            this.labelFormat = new System.Windows.Forms.Label();
            this.groupBoxFileFormat = new System.Windows.Forms.GroupBox();
            this.radioButtonBitmap = new System.Windows.Forms.RadioButton();
            this.radioButtonJpeg = new System.Windows.Forms.RadioButton();
            this.radioButtonDDS = new System.Windows.Forms.RadioButton();
            this.labelMipmaps = new System.Windows.Forms.Label();
            this.checkBoxMipMaps = new System.Windows.Forms.CheckBox();
            this.comboBoxMipmaps = new System.Windows.Forms.ComboBox();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.dataOrigSwizzle = new System.Windows.Forms.Label();
            this.labelOrigSwizzle = new System.Windows.Forms.Label();
            this.dataOrigHeight = new System.Windows.Forms.Label();
            this.dataOrigWidth = new System.Windows.Forms.Label();
            this.dataOrigFormat = new System.Windows.Forms.Label();
            this.dataOrigBPP = new System.Windows.Forms.Label();
            this.dataOrigType = new System.Windows.Forms.Label();
            this.labelOrigHeight = new System.Windows.Forms.Label();
            this.labelOrigWidth = new System.Windows.Forms.Label();
            this.labelOrigFormat = new System.Windows.Forms.Label();
            this.labelOrigBPP = new System.Windows.Forms.Label();
            this.labelOrigType = new System.Windows.Forms.Label();
            this.groupBoxCompression = new System.Windows.Forms.GroupBox();
            this.radioButtonCompressed = new System.Windows.Forms.RadioButton();
            this.radioButtonUncompressed = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxBPP.SuspendLayout();
            this.groupBoxFileFormat.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            this.groupBoxCompression.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxFormat
            // 
            this.comboBoxFormat.FormattingEnabled = true;
            this.comboBoxFormat.Location = new System.Drawing.Point(113, 154);
            this.comboBoxFormat.Name = "comboBoxFormat";
            this.comboBoxFormat.Size = new System.Drawing.Size(152, 21);
            this.comboBoxFormat.TabIndex = 0;
            // 
            // radioButton32Bit
            // 
            this.radioButton32Bit.AutoSize = true;
            this.radioButton32Bit.Location = new System.Drawing.Point(23, 14);
            this.radioButton32Bit.Name = "radioButton32Bit";
            this.radioButton32Bit.Size = new System.Drawing.Size(52, 17);
            this.radioButton32Bit.TabIndex = 1;
            this.radioButton32Bit.TabStop = true;
            this.radioButton32Bit.Text = "32 Bit";
            this.radioButton32Bit.UseVisualStyleBackColor = true;
            this.radioButton32Bit.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBoxBPP
            // 
            this.groupBoxBPP.Controls.Add(this.radioButton8Bit);
            this.groupBoxBPP.Controls.Add(this.radioButton16Bit);
            this.groupBoxBPP.Controls.Add(this.radioButton32Bit);
            this.groupBoxBPP.Location = new System.Drawing.Point(22, 98);
            this.groupBoxBPP.Name = "groupBoxBPP";
            this.groupBoxBPP.Size = new System.Drawing.Size(276, 37);
            this.groupBoxBPP.TabIndex = 2;
            this.groupBoxBPP.TabStop = false;
            this.groupBoxBPP.Text = "Bits Per Pixel";
            // 
            // radioButton8Bit
            // 
            this.radioButton8Bit.AutoSize = true;
            this.radioButton8Bit.Location = new System.Drawing.Point(197, 14);
            this.radioButton8Bit.Name = "radioButton8Bit";
            this.radioButton8Bit.Size = new System.Drawing.Size(46, 17);
            this.radioButton8Bit.TabIndex = 3;
            this.radioButton8Bit.TabStop = true;
            this.radioButton8Bit.Text = "8 Bit";
            this.radioButton8Bit.UseVisualStyleBackColor = true;
            this.radioButton8Bit.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton16Bit
            // 
            this.radioButton16Bit.AutoSize = true;
            this.radioButton16Bit.Location = new System.Drawing.Point(107, 14);
            this.radioButton16Bit.Name = "radioButton16Bit";
            this.radioButton16Bit.Size = new System.Drawing.Size(52, 17);
            this.radioButton16Bit.TabIndex = 2;
            this.radioButton16Bit.TabStop = true;
            this.radioButton16Bit.Text = "16 Bit";
            this.radioButton16Bit.UseVisualStyleBackColor = true;
            this.radioButton16Bit.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(30, 157);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(67, 13);
            this.labelFormat.TabIndex = 3;
            this.labelFormat.Text = "Save Format";
            // 
            // groupBoxFileFormat
            // 
            this.groupBoxFileFormat.Controls.Add(this.radioButtonBitmap);
            this.groupBoxFileFormat.Controls.Add(this.radioButtonJpeg);
            this.groupBoxFileFormat.Controls.Add(this.radioButtonDDS);
            this.groupBoxFileFormat.Location = new System.Drawing.Point(22, 15);
            this.groupBoxFileFormat.Name = "groupBoxFileFormat";
            this.groupBoxFileFormat.Size = new System.Drawing.Size(276, 37);
            this.groupBoxFileFormat.TabIndex = 4;
            this.groupBoxFileFormat.TabStop = false;
            this.groupBoxFileFormat.Text = "File Format";
            // 
            // radioButtonBitmap
            // 
            this.radioButtonBitmap.AutoSize = true;
            this.radioButtonBitmap.Location = new System.Drawing.Point(197, 14);
            this.radioButtonBitmap.Name = "radioButtonBitmap";
            this.radioButtonBitmap.Size = new System.Drawing.Size(57, 17);
            this.radioButtonBitmap.TabIndex = 3;
            this.radioButtonBitmap.TabStop = true;
            this.radioButtonBitmap.Text = "Bitmap";
            this.radioButtonBitmap.UseVisualStyleBackColor = true;
            this.radioButtonBitmap.CheckedChanged += new System.EventHandler(this.radioButtonBitmap_CheckedChanged);
            // 
            // radioButtonJpeg
            // 
            this.radioButtonJpeg.AutoSize = true;
            this.radioButtonJpeg.Location = new System.Drawing.Point(107, 14);
            this.radioButtonJpeg.Name = "radioButtonJpeg";
            this.radioButtonJpeg.Size = new System.Drawing.Size(48, 17);
            this.radioButtonJpeg.TabIndex = 2;
            this.radioButtonJpeg.TabStop = true;
            this.radioButtonJpeg.Text = "Jpeg";
            this.radioButtonJpeg.UseVisualStyleBackColor = true;
            this.radioButtonJpeg.CheckedChanged += new System.EventHandler(this.radioButtonJpeg_CheckedChanged);
            // 
            // radioButtonDDS
            // 
            this.radioButtonDDS.AutoSize = true;
            this.radioButtonDDS.Location = new System.Drawing.Point(23, 14);
            this.radioButtonDDS.Name = "radioButtonDDS";
            this.radioButtonDDS.Size = new System.Drawing.Size(48, 17);
            this.radioButtonDDS.TabIndex = 1;
            this.radioButtonDDS.TabStop = true;
            this.radioButtonDDS.Text = "DDS";
            this.radioButtonDDS.UseVisualStyleBackColor = true;
            this.radioButtonDDS.CheckedChanged += new System.EventHandler(this.radioButtonDDS_CheckedChanged);
            // 
            // labelMipmaps
            // 
            this.labelMipmaps.AutoSize = true;
            this.labelMipmaps.Location = new System.Drawing.Point(53, 204);
            this.labelMipmaps.Name = "labelMipmaps";
            this.labelMipmaps.Size = new System.Drawing.Size(108, 13);
            this.labelMipmaps.TabIndex = 5;
            this.labelMipmaps.Text = "# of mipmaps to save";
            // 
            // checkBoxMipMaps
            // 
            this.checkBoxMipMaps.AutoSize = true;
            this.checkBoxMipMaps.Location = new System.Drawing.Point(33, 181);
            this.checkBoxMipMaps.Name = "checkBoxMipMaps";
            this.checkBoxMipMaps.Size = new System.Drawing.Size(132, 17);
            this.checkBoxMipMaps.TabIndex = 6;
            this.checkBoxMipMaps.Text = "Save Mipmaps (LODs)";
            this.checkBoxMipMaps.UseVisualStyleBackColor = true;
            this.checkBoxMipMaps.CheckedChanged += new System.EventHandler(this.checkBoxMipMaps_CheckedChanged);
            // 
            // comboBoxMipmaps
            // 
            this.comboBoxMipmaps.FormattingEnabled = true;
            this.comboBoxMipmaps.Location = new System.Drawing.Point(167, 201);
            this.comboBoxMipmaps.Name = "comboBoxMipmaps";
            this.comboBoxMipmaps.Size = new System.Drawing.Size(67, 21);
            this.comboBoxMipmaps.TabIndex = 7;
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Controls.Add(this.dataOrigSwizzle);
            this.groupBoxInfo.Controls.Add(this.labelOrigSwizzle);
            this.groupBoxInfo.Controls.Add(this.dataOrigHeight);
            this.groupBoxInfo.Controls.Add(this.dataOrigWidth);
            this.groupBoxInfo.Controls.Add(this.dataOrigFormat);
            this.groupBoxInfo.Controls.Add(this.dataOrigBPP);
            this.groupBoxInfo.Controls.Add(this.dataOrigType);
            this.groupBoxInfo.Controls.Add(this.labelOrigHeight);
            this.groupBoxInfo.Controls.Add(this.labelOrigWidth);
            this.groupBoxInfo.Controls.Add(this.labelOrigFormat);
            this.groupBoxInfo.Controls.Add(this.labelOrigBPP);
            this.groupBoxInfo.Controls.Add(this.labelOrigType);
            this.groupBoxInfo.Location = new System.Drawing.Point(315, 15);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(215, 114);
            this.groupBoxInfo.TabIndex = 8;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Image Info";
            // 
            // dataOrigSwizzle
            // 
            this.dataOrigSwizzle.AutoSize = true;
            this.dataOrigSwizzle.Location = new System.Drawing.Point(63, 86);
            this.dataOrigSwizzle.Name = "dataOrigSwizzle";
            this.dataOrigSwizzle.Size = new System.Drawing.Size(57, 13);
            this.dataOrigSwizzle.TabIndex = 11;
            this.dataOrigSwizzle.Text = "<unkown>";
            // 
            // labelOrigSwizzle
            // 
            this.labelOrigSwizzle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigSwizzle.AutoSize = true;
            this.labelOrigSwizzle.Location = new System.Drawing.Point(8, 86);
            this.labelOrigSwizzle.Name = "labelOrigSwizzle";
            this.labelOrigSwizzle.Size = new System.Drawing.Size(48, 13);
            this.labelOrigSwizzle.TabIndex = 10;
            this.labelOrigSwizzle.Text = "Swizzled";
            // 
            // dataOrigHeight
            // 
            this.dataOrigHeight.AutoSize = true;
            this.dataOrigHeight.Location = new System.Drawing.Point(63, 72);
            this.dataOrigHeight.Name = "dataOrigHeight";
            this.dataOrigHeight.Size = new System.Drawing.Size(57, 13);
            this.dataOrigHeight.TabIndex = 9;
            this.dataOrigHeight.Text = "<unkown>";
            // 
            // dataOrigWidth
            // 
            this.dataOrigWidth.AutoSize = true;
            this.dataOrigWidth.Location = new System.Drawing.Point(63, 58);
            this.dataOrigWidth.Name = "dataOrigWidth";
            this.dataOrigWidth.Size = new System.Drawing.Size(57, 13);
            this.dataOrigWidth.TabIndex = 8;
            this.dataOrigWidth.Text = "<unkown>";
            // 
            // dataOrigFormat
            // 
            this.dataOrigFormat.AutoSize = true;
            this.dataOrigFormat.Location = new System.Drawing.Point(63, 44);
            this.dataOrigFormat.Name = "dataOrigFormat";
            this.dataOrigFormat.Size = new System.Drawing.Size(57, 13);
            this.dataOrigFormat.TabIndex = 7;
            this.dataOrigFormat.Text = "<unkown>";
            // 
            // dataOrigBPP
            // 
            this.dataOrigBPP.AutoSize = true;
            this.dataOrigBPP.Location = new System.Drawing.Point(63, 30);
            this.dataOrigBPP.Name = "dataOrigBPP";
            this.dataOrigBPP.Size = new System.Drawing.Size(57, 13);
            this.dataOrigBPP.TabIndex = 6;
            this.dataOrigBPP.Text = "<unkown>";
            // 
            // dataOrigType
            // 
            this.dataOrigType.AutoSize = true;
            this.dataOrigType.Location = new System.Drawing.Point(63, 16);
            this.dataOrigType.Name = "dataOrigType";
            this.dataOrigType.Size = new System.Drawing.Size(57, 13);
            this.dataOrigType.TabIndex = 5;
            this.dataOrigType.Text = "<unkown>";
            // 
            // labelOrigHeight
            // 
            this.labelOrigHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigHeight.AutoSize = true;
            this.labelOrigHeight.Location = new System.Drawing.Point(18, 72);
            this.labelOrigHeight.Name = "labelOrigHeight";
            this.labelOrigHeight.Size = new System.Drawing.Size(38, 13);
            this.labelOrigHeight.TabIndex = 4;
            this.labelOrigHeight.Text = "Height";
            // 
            // labelOrigWidth
            // 
            this.labelOrigWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigWidth.AutoSize = true;
            this.labelOrigWidth.Location = new System.Drawing.Point(21, 58);
            this.labelOrigWidth.Name = "labelOrigWidth";
            this.labelOrigWidth.Size = new System.Drawing.Size(35, 13);
            this.labelOrigWidth.TabIndex = 3;
            this.labelOrigWidth.Text = "Width";
            // 
            // labelOrigFormat
            // 
            this.labelOrigFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigFormat.AutoSize = true;
            this.labelOrigFormat.Location = new System.Drawing.Point(18, 44);
            this.labelOrigFormat.Name = "labelOrigFormat";
            this.labelOrigFormat.Size = new System.Drawing.Size(39, 13);
            this.labelOrigFormat.TabIndex = 2;
            this.labelOrigFormat.Text = "Format";
            // 
            // labelOrigBPP
            // 
            this.labelOrigBPP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigBPP.AutoSize = true;
            this.labelOrigBPP.Location = new System.Drawing.Point(28, 30);
            this.labelOrigBPP.Name = "labelOrigBPP";
            this.labelOrigBPP.Size = new System.Drawing.Size(28, 13);
            this.labelOrigBPP.TabIndex = 1;
            this.labelOrigBPP.Text = "BPP";
            // 
            // labelOrigType
            // 
            this.labelOrigType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOrigType.AutoSize = true;
            this.labelOrigType.Location = new System.Drawing.Point(26, 16);
            this.labelOrigType.Name = "labelOrigType";
            this.labelOrigType.Size = new System.Drawing.Size(31, 13);
            this.labelOrigType.TabIndex = 0;
            this.labelOrigType.Text = "Type";
            // 
            // groupBoxCompression
            // 
            this.groupBoxCompression.Controls.Add(this.radioButtonCompressed);
            this.groupBoxCompression.Controls.Add(this.radioButtonUncompressed);
            this.groupBoxCompression.Location = new System.Drawing.Point(22, 58);
            this.groupBoxCompression.Name = "groupBoxCompression";
            this.groupBoxCompression.Size = new System.Drawing.Size(276, 34);
            this.groupBoxCompression.TabIndex = 9;
            this.groupBoxCompression.TabStop = false;
            this.groupBoxCompression.Text = "Compression";
            // 
            // radioButtonCompressed
            // 
            this.radioButtonCompressed.AutoSize = true;
            this.radioButtonCompressed.Location = new System.Drawing.Point(167, 11);
            this.radioButtonCompressed.Name = "radioButtonCompressed";
            this.radioButtonCompressed.Size = new System.Drawing.Size(83, 17);
            this.radioButtonCompressed.TabIndex = 3;
            this.radioButtonCompressed.TabStop = true;
            this.radioButtonCompressed.Text = "Compressed";
            this.radioButtonCompressed.UseVisualStyleBackColor = true;
            this.radioButtonCompressed.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonUncompressed
            // 
            this.radioButtonUncompressed.AutoSize = true;
            this.radioButtonUncompressed.Location = new System.Drawing.Point(18, 11);
            this.radioButtonUncompressed.Name = "radioButtonUncompressed";
            this.radioButtonUncompressed.Size = new System.Drawing.Size(96, 17);
            this.radioButtonUncompressed.TabIndex = 2;
            this.radioButtonUncompressed.TabStop = true;
            this.radioButtonUncompressed.Text = "Uncompressed";
            this.radioButtonUncompressed.UseVisualStyleBackColor = true;
            this.radioButtonUncompressed.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(189, 259);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(192, 28);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // BitmapOperations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 299);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBoxCompression);
            this.Controls.Add(this.groupBoxInfo);
            this.Controls.Add(this.comboBoxMipmaps);
            this.Controls.Add(this.checkBoxMipMaps);
            this.Controls.Add(this.labelMipmaps);
            this.Controls.Add(this.groupBoxFileFormat);
            this.Controls.Add(this.labelFormat);
            this.Controls.Add(this.groupBoxBPP);
            this.Controls.Add(this.comboBoxFormat);
            this.Name = "BitmapOperations";
            this.Text = "BitmapOperations";
            this.groupBoxBPP.ResumeLayout(false);
            this.groupBoxBPP.PerformLayout();
            this.groupBoxFileFormat.ResumeLayout(false);
            this.groupBoxFileFormat.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            this.groupBoxCompression.ResumeLayout(false);
            this.groupBoxCompression.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFormat;
        private System.Windows.Forms.RadioButton radioButton32Bit;
        private System.Windows.Forms.GroupBox groupBoxBPP;
        private System.Windows.Forms.RadioButton radioButton8Bit;
        private System.Windows.Forms.RadioButton radioButton16Bit;
        private System.Windows.Forms.Label labelFormat;
        private System.Windows.Forms.GroupBox groupBoxFileFormat;
        private System.Windows.Forms.RadioButton radioButtonBitmap;
        private System.Windows.Forms.RadioButton radioButtonJpeg;
        private System.Windows.Forms.RadioButton radioButtonDDS;
        private System.Windows.Forms.Label labelMipmaps;
        private System.Windows.Forms.CheckBox checkBoxMipMaps;
        private System.Windows.Forms.ComboBox comboBoxMipmaps;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.Label dataOrigType;
        private System.Windows.Forms.Label labelOrigHeight;
        private System.Windows.Forms.Label labelOrigWidth;
        private System.Windows.Forms.Label labelOrigFormat;
        private System.Windows.Forms.Label labelOrigBPP;
        private System.Windows.Forms.Label labelOrigType;
        private System.Windows.Forms.Label dataOrigHeight;
        private System.Windows.Forms.Label dataOrigWidth;
        private System.Windows.Forms.Label dataOrigFormat;
        private System.Windows.Forms.Label dataOrigBPP;
        private System.Windows.Forms.GroupBox groupBoxCompression;
        private System.Windows.Forms.RadioButton radioButtonCompressed;
        private System.Windows.Forms.RadioButton radioButtonUncompressed;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label dataOrigSwizzle;
        private System.Windows.Forms.Label labelOrigSwizzle;
    }
}