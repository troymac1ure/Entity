namespace entity.Main
{
    partial class MapWizard
    {
        #region Fields

        private System.Windows.Forms.ComboBox baseMapComboBox;
        private System.Windows.Forms.Label baseMapLabel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button createMapButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label newFileNameLabel;
        private System.Windows.Forms.TextBox newMapFileTextBox;
        private System.Windows.Forms.Label newMapLabel;
        private System.Windows.Forms.TextBox newMapTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapWizard));
            this.baseMapComboBox = new System.Windows.Forms.ComboBox();
            this.baseMapLabel = new System.Windows.Forms.Label();
            this.newMapFileTextBox = new System.Windows.Forms.TextBox();
            this.newFileNameLabel = new System.Windows.Forms.Label();
            this.createMapButton = new System.Windows.Forms.Button();
            this.newMapLabel = new System.Windows.Forms.Label();
            this.newMapTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            //
            // baseMapComboBox
            //
            this.baseMapComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.baseMapComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.baseMapComboBox.FormattingEnabled = true;
            this.baseMapComboBox.Location = new System.Drawing.Point(121, 33);
            this.baseMapComboBox.Name = "baseMapComboBox";
            this.baseMapComboBox.Size = new System.Drawing.Size(247, 21);
            this.baseMapComboBox.TabIndex = 1;
            this.baseMapComboBox.TextChanged += new System.EventHandler(this.baseMapComboBox_TextChanged);
            //
            // baseMapLabel
            //
            this.baseMapLabel.AutoSize = true;
            this.baseMapLabel.Location = new System.Drawing.Point(60, 36);
            this.baseMapLabel.Name = "baseMapLabel";
            this.baseMapLabel.Size = new System.Drawing.Size(55, 13);
            this.baseMapLabel.TabIndex = 0;
            this.baseMapLabel.Text = "Base Map";
            //
            // newMapFileTextBox
            //
            this.newMapFileTextBox.Location = new System.Drawing.Point(121, 80);
            this.newMapFileTextBox.Name = "newMapFileTextBox";
            this.newMapFileTextBox.Size = new System.Drawing.Size(247, 20);
            this.newMapFileTextBox.TabIndex = 3;
            this.newMapFileTextBox.TextChanged += new System.EventHandler(this.newMapFileTextBox_TextChanged);
            this.newMapFileTextBox.Leave += new System.EventHandler(this.newMapFileTextBox_Leave);
            this.newMapFileTextBox.Enter += new System.EventHandler(this.newMapFileTextBox_Enter);
            //
            // newFileNameLabel
            //
            this.newFileNameLabel.AutoSize = true;
            this.newFileNameLabel.Location = new System.Drawing.Point(12, 83);
            this.newFileNameLabel.Name = "newFileNameLabel";
            this.newFileNameLabel.Size = new System.Drawing.Size(103, 13);
            this.newFileNameLabel.TabIndex = 2;
            this.newFileNameLabel.Text = "New Map File Name";
            //
            // createMapButton
            //
            this.createMapButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.createMapButton.Location = new System.Drawing.Point(121, 172);
            this.createMapButton.Name = "createMapButton";
            this.createMapButton.Size = new System.Drawing.Size(182, 41);
            this.createMapButton.TabIndex = 6;
            this.createMapButton.Text = "Create Map";
            this.createMapButton.UseVisualStyleBackColor = true;
            this.createMapButton.Click += new System.EventHandler(this.createMapButton_Click);
            //
            // newMapLabel
            //
            this.newMapLabel.AutoSize = true;
            this.newMapLabel.Location = new System.Drawing.Point(30, 131);
            this.newMapLabel.Name = "newMapLabel";
            this.newMapLabel.Size = new System.Drawing.Size(84, 13);
            this.newMapLabel.TabIndex = 4;
            this.newMapLabel.Text = "New Map Name";
            //
            // newMapTextBox
            //
            this.newMapTextBox.Location = new System.Drawing.Point(121, 128);
            this.newMapTextBox.Name = "newMapTextBox";
            this.newMapTextBox.Size = new System.Drawing.Size(247, 20);
            this.newMapTextBox.TabIndex = 5;
            this.newMapTextBox.TextChanged += new System.EventHandler(this.newMapTextBox_TextChanged);
            this.newMapTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newMapTextBox_KeyDown);
            this.newMapTextBox.Leave += new System.EventHandler(this.newMapTextBox_Leave);
            this.newMapTextBox.Enter += new System.EventHandler(this.newMapTextBox_Enter);
            //
            // pictureBox1
            //
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(447, -7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "label2";
            //
            // MapWizard
            //
            this.AcceptButton = this.createMapButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 242);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.newMapTextBox);
            this.Controls.Add(this.newMapLabel);
            this.Controls.Add(this.createMapButton);
            this.Controls.Add(this.newFileNameLabel);
            this.Controls.Add(this.newMapFileTextBox);
            this.Controls.Add(this.baseMapLabel);
            this.Controls.Add(this.baseMapComboBox);
            this.Name = "MapWizard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MapWizard";
            this.Load += new System.EventHandler(this.MapWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Methods
    }
}