namespace entity.Main
{
    partial class MainmenuEdit
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
            this.lbMapListing = new System.Windows.Forms.ListBox();
            this.rbCampaign = new System.Windows.Forms.RadioButton();
            this.rbMultiplayer = new System.Windows.Forms.RadioButton();
            this.tbScenarioName = new System.Windows.Forms.TextBox();
            this.lblScenarioName = new System.Windows.Forms.Label();
            this.lblMapName = new System.Windows.Forms.Label();
            this.tbMapName = new System.Windows.Forms.TextBox();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblMapDescription = new System.Windows.Forms.Label();
            this.tbMapDescription = new System.Windows.Forms.TextBox();
            this.btnCopyToAll = new System.Windows.Forms.Button();
            this.pbMapBitmap = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblNone = new System.Windows.Forms.Label();
            this.lblCTF = new System.Windows.Forms.Label();
            this.lblSlayer = new System.Windows.Forms.Label();
            this.lblOddball = new System.Windows.Forms.Label();
            this.lblKOTH = new System.Windows.Forms.Label();
            this.lblRace = new System.Windows.Forms.Label();
            this.lblHeadhunter = new System.Windows.Forms.Label();
            this.lblJuggernaught = new System.Windows.Forms.Label();
            this.lblTerritories = new System.Windows.Forms.Label();
            this.lblAssault = new System.Windows.Forms.Label();
            this.nbNone = new System.Windows.Forms.NumericUpDown();
            this.nbCTF = new System.Windows.Forms.NumericUpDown();
            this.nbSlayer = new System.Windows.Forms.NumericUpDown();
            this.nbOddball = new System.Windows.Forms.NumericUpDown();
            this.nbKOTH = new System.Windows.Forms.NumericUpDown();
            this.nbRace = new System.Windows.Forms.NumericUpDown();
            this.nbHeadhunter = new System.Windows.Forms.NumericUpDown();
            this.nbJuggernaught = new System.Windows.Forms.NumericUpDown();
            this.nbTerritories = new System.Windows.Forms.NumericUpDown();
            this.nbAssault = new System.Windows.Forms.NumericUpDown();
            this.btnSortAlphabetically = new System.Windows.Forms.Button();
            this.btnAddMap = new System.Windows.Forms.Button();
            this.btnRemoveMap = new System.Windows.Forms.Button();
            this.btnSaveMainMenu = new System.Windows.Forms.Button();
            this.ofdLoadMap = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbMapBitmap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbNone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbCTF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbSlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbOddball)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbKOTH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbRace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbHeadhunter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbJuggernaught)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbTerritories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbAssault)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMapListing
            // 
            this.lbMapListing.FormattingEnabled = true;
            this.lbMapListing.Location = new System.Drawing.Point(28, 86);
            this.lbMapListing.Name = "lbMapListing";
            this.lbMapListing.Size = new System.Drawing.Size(208, 342);
            this.lbMapListing.TabIndex = 6;
            this.lbMapListing.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbMapListing_MouseUp);
            this.lbMapListing.SelectedIndexChanged += new System.EventHandler(this.lbMapListing_SelectedIndexChanged);
            this.lbMapListing.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbMapListing_MouseMove);
            // 
            // rbCampaign
            // 
            this.rbCampaign.AutoSize = true;
            this.rbCampaign.Location = new System.Drawing.Point(28, 39);
            this.rbCampaign.Name = "rbCampaign";
            this.rbCampaign.Size = new System.Drawing.Size(72, 17);
            this.rbCampaign.TabIndex = 3;
            this.rbCampaign.Text = "Campaign";
            this.rbCampaign.UseVisualStyleBackColor = true;
            this.rbCampaign.CheckedChanged += new System.EventHandler(this.rbCampaign_CheckedChanged);
            // 
            // rbMultiplayer
            // 
            this.rbMultiplayer.AutoSize = true;
            this.rbMultiplayer.Checked = true;
            this.rbMultiplayer.Location = new System.Drawing.Point(123, 39);
            this.rbMultiplayer.Name = "rbMultiplayer";
            this.rbMultiplayer.Size = new System.Drawing.Size(75, 17);
            this.rbMultiplayer.TabIndex = 4;
            this.rbMultiplayer.TabStop = true;
            this.rbMultiplayer.Text = "Multiplayer";
            this.rbMultiplayer.UseVisualStyleBackColor = true;
            this.rbMultiplayer.CheckedChanged += new System.EventHandler(this.rbMultiplayer_CheckedChanged);
            // 
            // tbScenarioName
            // 
            this.tbScenarioName.Location = new System.Drawing.Point(267, 25);
            this.tbScenarioName.MaxLength = 255;
            this.tbScenarioName.Name = "tbScenarioName";
            this.tbScenarioName.Size = new System.Drawing.Size(368, 20);
            this.tbScenarioName.TabIndex = 11;
            this.tbScenarioName.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // lblScenarioName
            // 
            this.lblScenarioName.AutoSize = true;
            this.lblScenarioName.Location = new System.Drawing.Point(264, 9);
            this.lblScenarioName.Name = "lblScenarioName";
            this.lblScenarioName.Size = new System.Drawing.Size(80, 13);
            this.lblScenarioName.TabIndex = 4;
            this.lblScenarioName.Text = "Scenario Name";
            // 
            // lblMapName
            // 
            this.lblMapName.AutoSize = true;
            this.lblMapName.Location = new System.Drawing.Point(264, 290);
            this.lblMapName.Name = "lblMapName";
            this.lblMapName.Size = new System.Drawing.Size(59, 13);
            this.lblMapName.TabIndex = 12;
            this.lblMapName.Text = "Map Name";
            // 
            // tbMapName
            // 
            this.tbMapName.Location = new System.Drawing.Point(267, 306);
            this.tbMapName.MaxLength = 31;
            this.tbMapName.Name = "tbMapName";
            this.tbMapName.Size = new System.Drawing.Size(179, 20);
            this.tbMapName.TabIndex = 13;
            this.tbMapName.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // cbLanguage
            // 
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Location = new System.Drawing.Point(90, 6);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(123, 21);
            this.cbLanguage.TabIndex = 2;
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbLanguage_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(25, 9);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(55, 13);
            this.lblLanguage.TabIndex = 1;
            this.lblLanguage.Text = "Language";
            // 
            // lblMapDescription
            // 
            this.lblMapDescription.AutoSize = true;
            this.lblMapDescription.Location = new System.Drawing.Point(264, 332);
            this.lblMapDescription.Name = "lblMapDescription";
            this.lblMapDescription.Size = new System.Drawing.Size(84, 13);
            this.lblMapDescription.TabIndex = 14;
            this.lblMapDescription.Text = "Map Description";
            // 
            // tbMapDescription
            // 
            this.tbMapDescription.Location = new System.Drawing.Point(267, 348);
            this.tbMapDescription.MaxLength = 127;
            this.tbMapDescription.Multiline = true;
            this.tbMapDescription.Name = "tbMapDescription";
            this.tbMapDescription.Size = new System.Drawing.Size(338, 38);
            this.tbMapDescription.TabIndex = 15;
            this.tbMapDescription.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // btnCopyToAll
            // 
            this.btnCopyToAll.Location = new System.Drawing.Point(267, 392);
            this.btnCopyToAll.Name = "btnCopyToAll";
            this.btnCopyToAll.Size = new System.Drawing.Size(338, 29);
            this.btnCopyToAll.TabIndex = 16;
            this.btnCopyToAll.Text = "Copy name && description to all languages";
            this.btnCopyToAll.UseVisualStyleBackColor = true;
            this.btnCopyToAll.Click += new System.EventHandler(this.btnCopyToAll_Click);
            // 
            // pbMapBitmap
            // 
            this.pbMapBitmap.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pbMapBitmap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbMapBitmap.Location = new System.Drawing.Point(267, 51);
            this.pbMapBitmap.Name = "pbMapBitmap";
            this.pbMapBitmap.Size = new System.Drawing.Size(220, 207);
            this.pbMapBitmap.TabIndex = 12;
            this.pbMapBitmap.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(493, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Max number of teams:";
            // 
            // lblNone
            // 
            this.lblNone.AutoSize = true;
            this.lblNone.Location = new System.Drawing.Point(509, 74);
            this.lblNone.Name = "lblNone";
            this.lblNone.Size = new System.Drawing.Size(33, 13);
            this.lblNone.TabIndex = 21;
            this.lblNone.Text = "None";
            // 
            // lblCTF
            // 
            this.lblCTF.AutoSize = true;
            this.lblCTF.Location = new System.Drawing.Point(509, 95);
            this.lblCTF.Name = "lblCTF";
            this.lblCTF.Size = new System.Drawing.Size(27, 13);
            this.lblCTF.TabIndex = 22;
            this.lblCTF.Text = "CTF";
            // 
            // lblSlayer
            // 
            this.lblSlayer.AutoSize = true;
            this.lblSlayer.Location = new System.Drawing.Point(509, 116);
            this.lblSlayer.Name = "lblSlayer";
            this.lblSlayer.Size = new System.Drawing.Size(36, 13);
            this.lblSlayer.TabIndex = 23;
            this.lblSlayer.Text = "Slayer";
            // 
            // lblOddball
            // 
            this.lblOddball.AutoSize = true;
            this.lblOddball.Location = new System.Drawing.Point(509, 137);
            this.lblOddball.Name = "lblOddball";
            this.lblOddball.Size = new System.Drawing.Size(43, 13);
            this.lblOddball.TabIndex = 24;
            this.lblOddball.Text = "Oddball";
            // 
            // lblKOTH
            // 
            this.lblKOTH.AutoSize = true;
            this.lblKOTH.Location = new System.Drawing.Point(509, 158);
            this.lblKOTH.Name = "lblKOTH";
            this.lblKOTH.Size = new System.Drawing.Size(37, 13);
            this.lblKOTH.TabIndex = 25;
            this.lblKOTH.Text = "KOTH";
            // 
            // lblRace
            // 
            this.lblRace.AutoSize = true;
            this.lblRace.Location = new System.Drawing.Point(509, 179);
            this.lblRace.Name = "lblRace";
            this.lblRace.Size = new System.Drawing.Size(33, 13);
            this.lblRace.TabIndex = 26;
            this.lblRace.Text = "Race";
            // 
            // lblHeadhunter
            // 
            this.lblHeadhunter.AutoSize = true;
            this.lblHeadhunter.Location = new System.Drawing.Point(509, 200);
            this.lblHeadhunter.Name = "lblHeadhunter";
            this.lblHeadhunter.Size = new System.Drawing.Size(63, 13);
            this.lblHeadhunter.TabIndex = 27;
            this.lblHeadhunter.Text = "Headhunter";
            // 
            // lblJuggernaught
            // 
            this.lblJuggernaught.AutoSize = true;
            this.lblJuggernaught.Location = new System.Drawing.Point(509, 221);
            this.lblJuggernaught.Name = "lblJuggernaught";
            this.lblJuggernaught.Size = new System.Drawing.Size(72, 13);
            this.lblJuggernaught.TabIndex = 28;
            this.lblJuggernaught.Text = "Juggernaught";
            // 
            // lblTerritories
            // 
            this.lblTerritories.AutoSize = true;
            this.lblTerritories.Location = new System.Drawing.Point(509, 242);
            this.lblTerritories.Name = "lblTerritories";
            this.lblTerritories.Size = new System.Drawing.Size(53, 13);
            this.lblTerritories.TabIndex = 29;
            this.lblTerritories.Text = "Territories";
            // 
            // lblAssault
            // 
            this.lblAssault.AutoSize = true;
            this.lblAssault.Location = new System.Drawing.Point(509, 263);
            this.lblAssault.Name = "lblAssault";
            this.lblAssault.Size = new System.Drawing.Size(41, 13);
            this.lblAssault.TabIndex = 30;
            this.lblAssault.Text = "Assault";
            // 
            // nbNone
            // 
            this.nbNone.Location = new System.Drawing.Point(589, 72);
            this.nbNone.Name = "nbNone";
            this.nbNone.Size = new System.Drawing.Size(46, 20);
            this.nbNone.TabIndex = 30;
            this.nbNone.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbCTF
            // 
            this.nbCTF.Location = new System.Drawing.Point(589, 93);
            this.nbCTF.Name = "nbCTF";
            this.nbCTF.Size = new System.Drawing.Size(46, 20);
            this.nbCTF.TabIndex = 31;
            this.nbCTF.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbSlayer
            // 
            this.nbSlayer.Location = new System.Drawing.Point(589, 114);
            this.nbSlayer.Name = "nbSlayer";
            this.nbSlayer.Size = new System.Drawing.Size(46, 20);
            this.nbSlayer.TabIndex = 32;
            this.nbSlayer.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbOddball
            // 
            this.nbOddball.Location = new System.Drawing.Point(589, 135);
            this.nbOddball.Name = "nbOddball";
            this.nbOddball.Size = new System.Drawing.Size(46, 20);
            this.nbOddball.TabIndex = 33;
            this.nbOddball.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbKOTH
            // 
            this.nbKOTH.Location = new System.Drawing.Point(589, 156);
            this.nbKOTH.Name = "nbKOTH";
            this.nbKOTH.Size = new System.Drawing.Size(46, 20);
            this.nbKOTH.TabIndex = 34;
            this.nbKOTH.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbRace
            // 
            this.nbRace.Location = new System.Drawing.Point(589, 177);
            this.nbRace.Name = "nbRace";
            this.nbRace.Size = new System.Drawing.Size(46, 20);
            this.nbRace.TabIndex = 35;
            this.nbRace.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbHeadhunter
            // 
            this.nbHeadhunter.Location = new System.Drawing.Point(589, 198);
            this.nbHeadhunter.Name = "nbHeadhunter";
            this.nbHeadhunter.Size = new System.Drawing.Size(46, 20);
            this.nbHeadhunter.TabIndex = 36;
            this.nbHeadhunter.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbJuggernaught
            // 
            this.nbJuggernaught.Location = new System.Drawing.Point(589, 219);
            this.nbJuggernaught.Name = "nbJuggernaught";
            this.nbJuggernaught.Size = new System.Drawing.Size(46, 20);
            this.nbJuggernaught.TabIndex = 37;
            this.nbJuggernaught.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbTerritories
            // 
            this.nbTerritories.Location = new System.Drawing.Point(589, 240);
            this.nbTerritories.Name = "nbTerritories";
            this.nbTerritories.Size = new System.Drawing.Size(46, 20);
            this.nbTerritories.TabIndex = 38;
            this.nbTerritories.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // nbAssault
            // 
            this.nbAssault.Location = new System.Drawing.Point(589, 261);
            this.nbAssault.Name = "nbAssault";
            this.nbAssault.Size = new System.Drawing.Size(46, 20);
            this.nbAssault.TabIndex = 39;
            this.nbAssault.Leave += new System.EventHandler(this.editableBox_Leave);
            // 
            // btnSortAlphabetically
            // 
            this.btnSortAlphabetically.Location = new System.Drawing.Point(28, 62);
            this.btnSortAlphabetically.Name = "btnSortAlphabetically";
            this.btnSortAlphabetically.Size = new System.Drawing.Size(208, 23);
            this.btnSortAlphabetically.TabIndex = 5;
            this.btnSortAlphabetically.Text = "Sort Alphabetically";
            this.btnSortAlphabetically.UseVisualStyleBackColor = true;
            this.btnSortAlphabetically.Click += new System.EventHandler(this.btnSortAlphabetically_Click);
            // 
            // btnAddMap
            // 
            this.btnAddMap.Location = new System.Drawing.Point(28, 434);
            this.btnAddMap.Name = "btnAddMap";
            this.btnAddMap.Size = new System.Drawing.Size(72, 20);
            this.btnAddMap.TabIndex = 7;
            this.btnAddMap.Text = "&Add";
            this.btnAddMap.UseVisualStyleBackColor = true;
            this.btnAddMap.Click += new System.EventHandler(this.btnAddMap_Click);
            // 
            // btnRemoveMap
            // 
            this.btnRemoveMap.Location = new System.Drawing.Point(164, 434);
            this.btnRemoveMap.Name = "btnRemoveMap";
            this.btnRemoveMap.Size = new System.Drawing.Size(72, 20);
            this.btnRemoveMap.TabIndex = 8;
            this.btnRemoveMap.Text = "&Delete";
            this.btnRemoveMap.UseVisualStyleBackColor = true;
            this.btnRemoveMap.Click += new System.EventHandler(this.btnRemoveMap_Click);
            // 
            // btnSaveMainMenu
            // 
            this.btnSaveMainMenu.Location = new System.Drawing.Point(521, 434);
            this.btnSaveMainMenu.Name = "btnSaveMainMenu";
            this.btnSaveMainMenu.Size = new System.Drawing.Size(134, 29);
            this.btnSaveMainMenu.TabIndex = 40;
            this.btnSaveMainMenu.Text = "&Save Changes";
            this.btnSaveMainMenu.UseVisualStyleBackColor = true;
            this.btnSaveMainMenu.Click += new System.EventHandler(this.btnSaveMainMenu_Click);
            // 
            // ofdLoadMap
            // 
            this.ofdLoadMap.FileName = "openFileDialog1";
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Location = new System.Drawing.Point(349, 259);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(66, 21);
            this.btnLoadImage.TabIndex = 41;
            this.btnLoadImage.Text = "&Import";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(421, 259);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(66, 22);
            this.btnSaveImage.TabIndex = 42;
            this.btnSaveImage.Text = "&Export";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // MainmenuEdit
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 474);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.btnSaveMainMenu);
            this.Controls.Add(this.btnRemoveMap);
            this.Controls.Add(this.btnAddMap);
            this.Controls.Add(this.btnSortAlphabetically);
            this.Controls.Add(this.nbAssault);
            this.Controls.Add(this.nbTerritories);
            this.Controls.Add(this.nbJuggernaught);
            this.Controls.Add(this.nbHeadhunter);
            this.Controls.Add(this.nbRace);
            this.Controls.Add(this.nbKOTH);
            this.Controls.Add(this.nbOddball);
            this.Controls.Add(this.nbSlayer);
            this.Controls.Add(this.nbCTF);
            this.Controls.Add(this.nbNone);
            this.Controls.Add(this.lblAssault);
            this.Controls.Add(this.lblTerritories);
            this.Controls.Add(this.lblJuggernaught);
            this.Controls.Add(this.lblHeadhunter);
            this.Controls.Add(this.lblRace);
            this.Controls.Add(this.lblKOTH);
            this.Controls.Add(this.lblOddball);
            this.Controls.Add(this.lblSlayer);
            this.Controls.Add(this.lblCTF);
            this.Controls.Add(this.lblNone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbMapBitmap);
            this.Controls.Add(this.btnCopyToAll);
            this.Controls.Add(this.tbMapDescription);
            this.Controls.Add(this.lblMapDescription);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.cbLanguage);
            this.Controls.Add(this.tbMapName);
            this.Controls.Add(this.lblMapName);
            this.Controls.Add(this.tbScenarioName);
            this.Controls.Add(this.rbMultiplayer);
            this.Controls.Add(this.rbCampaign);
            this.Controls.Add(this.lbMapListing);
            this.Controls.Add(this.lblScenarioName);
            this.Name = "MainmenuEdit";
            this.Text = "MainMenu Editor";
            this.Load += new System.EventHandler(this.MainmenuEdit_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainmenuEdit_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainmenuEdit_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pbMapBitmap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbNone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbCTF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbSlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbOddball)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbKOTH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbRace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbHeadhunter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbJuggernaught)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbTerritories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbAssault)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbMapListing;
        private System.Windows.Forms.RadioButton rbCampaign;
        private System.Windows.Forms.RadioButton rbMultiplayer;
        private System.Windows.Forms.TextBox tbScenarioName;
        private System.Windows.Forms.Label lblScenarioName;
        private System.Windows.Forms.Label lblMapName;
        private System.Windows.Forms.TextBox tbMapName;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Label lblMapDescription;
        private System.Windows.Forms.TextBox tbMapDescription;
        private System.Windows.Forms.Button btnCopyToAll;
        private System.Windows.Forms.PictureBox pbMapBitmap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNone;
        private System.Windows.Forms.Label lblCTF;
        private System.Windows.Forms.Label lblSlayer;
        private System.Windows.Forms.Label lblOddball;
        private System.Windows.Forms.Label lblKOTH;
        private System.Windows.Forms.Label lblRace;
        private System.Windows.Forms.Label lblHeadhunter;
        private System.Windows.Forms.Label lblJuggernaught;
        private System.Windows.Forms.Label lblTerritories;
        private System.Windows.Forms.Label lblAssault;
        private System.Windows.Forms.NumericUpDown nbNone;
        private System.Windows.Forms.NumericUpDown nbCTF;
        private System.Windows.Forms.NumericUpDown nbSlayer;
        private System.Windows.Forms.NumericUpDown nbOddball;
        private System.Windows.Forms.NumericUpDown nbKOTH;
        private System.Windows.Forms.NumericUpDown nbRace;
        private System.Windows.Forms.NumericUpDown nbHeadhunter;
        private System.Windows.Forms.NumericUpDown nbJuggernaught;
        private System.Windows.Forms.NumericUpDown nbTerritories;
        private System.Windows.Forms.NumericUpDown nbAssault;
        private System.Windows.Forms.Button btnSortAlphabetically;
        private System.Windows.Forms.Button btnAddMap;
        private System.Windows.Forms.Button btnRemoveMap;
        private System.Windows.Forms.Button btnSaveMainMenu;
        private System.Windows.Forms.OpenFileDialog ofdLoadMap;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnSaveImage;
    }
}