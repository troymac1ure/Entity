namespace entity.MetaEditor2
{
    partial class MassFieldChanger
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
            this.components = new System.ComponentModel.Container();
            this.pnlDefaultControls = new System.Windows.Forms.Panel();
            this.lblTo = new System.Windows.Forms.Label();
            this.cbEndChunk = new System.Windows.Forms.ComboBox();
            this.cbStartChunk = new System.Windows.Forms.ComboBox();
            this.lblShowChunks = new System.Windows.Forms.Label();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.btnResetValues = new System.Windows.Forms.Button();
            this.tbInitialValue = new System.Windows.Forms.TextBox();
            this.lblAutoFill = new System.Windows.Forms.Label();
            this.pnlAutoFill = new System.Windows.Forms.Panel();
            this.btnFill = new System.Windows.Forms.Button();
            this.lblChangeValue = new System.Windows.Forms.Label();
            this.lblInitalValue = new System.Windows.Forms.Label();
            this.tbChange = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlFieldControls = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pnlDefaultControls.SuspendLayout();
            this.pnlAutoFill.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDefaultControls
            // 
            this.pnlDefaultControls.Controls.Add(this.progressBar1);
            this.pnlDefaultControls.Controls.Add(this.lblTo);
            this.pnlDefaultControls.Controls.Add(this.cbEndChunk);
            this.pnlDefaultControls.Controls.Add(this.cbStartChunk);
            this.pnlDefaultControls.Controls.Add(this.lblShowChunks);
            this.pnlDefaultControls.Controls.Add(this.btnSaveChanges);
            this.pnlDefaultControls.Controls.Add(this.btnResetValues);
            this.pnlDefaultControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDefaultControls.Location = new System.Drawing.Point(0, 0);
            this.pnlDefaultControls.Name = "pnlDefaultControls";
            this.pnlDefaultControls.Size = new System.Drawing.Size(492, 50);
            this.pnlDefaultControls.TabIndex = 0;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(145, 12);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(16, 13);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "to";
            // 
            // cbEndChunk
            // 
            this.cbEndChunk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEndChunk.FormattingEnabled = true;
            this.cbEndChunk.Location = new System.Drawing.Point(167, 9);
            this.cbEndChunk.Name = "cbEndChunk";
            this.cbEndChunk.Size = new System.Drawing.Size(56, 21);
            this.cbEndChunk.TabIndex = 3;
            this.cbEndChunk.SelectedIndexChanged += new System.EventHandler(this.cbChunk_SelectedIndexChanged);
            this.cbEndChunk.DropDownClosed += new System.EventHandler(this.cbEndChunk_DropDownClosed);
            this.cbEndChunk.DropDown += new System.EventHandler(this.cbEndChunk_DropDown);
            // 
            // cbStartChunk
            // 
            this.cbStartChunk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStartChunk.FormattingEnabled = true;
            this.cbStartChunk.Location = new System.Drawing.Point(82, 9);
            this.cbStartChunk.Name = "cbStartChunk";
            this.cbStartChunk.Size = new System.Drawing.Size(58, 21);
            this.cbStartChunk.TabIndex = 3;
            this.cbStartChunk.SelectedIndexChanged += new System.EventHandler(this.cbChunk_SelectedIndexChanged);
            // 
            // lblShowChunks
            // 
            this.lblShowChunks.AutoSize = true;
            this.lblShowChunks.Location = new System.Drawing.Point(9, 12);
            this.lblShowChunks.Name = "lblShowChunks";
            this.lblShowChunks.Size = new System.Drawing.Size(67, 13);
            this.lblShowChunks.TabIndex = 2;
            this.lblShowChunks.Text = "Edit Chunks:";
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveChanges.Location = new System.Drawing.Point(403, 7);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(79, 23);
            this.btnSaveChanges.TabIndex = 1;
            this.btnSaveChanges.Text = "Save";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // btnResetValues
            // 
            this.btnResetValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetValues.Location = new System.Drawing.Point(318, 7);
            this.btnResetValues.Name = "btnResetValues";
            this.btnResetValues.Size = new System.Drawing.Size(79, 23);
            this.btnResetValues.TabIndex = 0;
            this.btnResetValues.Text = "Reset Values";
            this.btnResetValues.UseVisualStyleBackColor = true;
            this.btnResetValues.Click += new System.EventHandler(this.btnResetValues_Click);
            // 
            // tbInitialValue
            // 
            this.tbInitialValue.Location = new System.Drawing.Point(144, 5);
            this.tbInitialValue.Name = "tbInitialValue";
            this.tbInitialValue.Size = new System.Drawing.Size(69, 20);
            this.tbInitialValue.TabIndex = 5;
            this.tbInitialValue.Text = "0";
            this.tbInitialValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblAutoFill
            // 
            this.lblAutoFill.AutoSize = true;
            this.lblAutoFill.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic)
                            | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoFill.Location = new System.Drawing.Point(7, 8);
            this.lblAutoFill.Name = "lblAutoFill";
            this.lblAutoFill.Size = new System.Drawing.Size(95, 13);
            this.lblAutoFill.TabIndex = 6;
            this.lblAutoFill.Text = "Auto Fill Values";
            // 
            // pnlAutoFill
            // 
            this.pnlAutoFill.Controls.Add(this.btnFill);
            this.pnlAutoFill.Controls.Add(this.lblChangeValue);
            this.pnlAutoFill.Controls.Add(this.lblInitalValue);
            this.pnlAutoFill.Controls.Add(this.lblAutoFill);
            this.pnlAutoFill.Controls.Add(this.tbChange);
            this.pnlAutoFill.Controls.Add(this.tbInitialValue);
            this.pnlAutoFill.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAutoFill.Location = new System.Drawing.Point(0, 50);
            this.pnlAutoFill.Name = "pnlAutoFill";
            this.pnlAutoFill.Size = new System.Drawing.Size(492, 37);
            this.pnlAutoFill.TabIndex = 1;
            this.pnlAutoFill.Visible = false;
            // 
            // btnFill
            // 
            this.btnFill.Location = new System.Drawing.Point(362, 3);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(57, 22);
            this.btnFill.TabIndex = 9;
            this.btnFill.Text = "Fill";
            this.btnFill.UseVisualStyleBackColor = true;
            this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
            // 
            // lblChangeValue
            // 
            this.lblChangeValue.AutoSize = true;
            this.lblChangeValue.Location = new System.Drawing.Point(230, 8);
            this.lblChangeValue.Name = "lblChangeValue";
            this.lblChangeValue.Size = new System.Drawing.Size(47, 13);
            this.lblChangeValue.TabIndex = 8;
            this.lblChangeValue.Text = "Change:";
            this.lblChangeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblInitalValue
            // 
            this.lblInitalValue.AutoSize = true;
            this.lblInitalValue.Location = new System.Drawing.Point(108, 8);
            this.lblInitalValue.Name = "lblInitalValue";
            this.lblInitalValue.Size = new System.Drawing.Size(34, 13);
            this.lblInitalValue.TabIndex = 7;
            this.lblInitalValue.Text = "Initial:";
            this.lblInitalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbChange
            // 
            this.tbChange.Location = new System.Drawing.Point(280, 5);
            this.tbChange.Name = "tbChange";
            this.tbChange.Size = new System.Drawing.Size(69, 20);
            this.tbChange.TabIndex = 5;
            this.tbChange.Text = "1";
            this.tbChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnlFieldControls
            // 
            this.pnlFieldControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlFieldControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFieldControls.Location = new System.Drawing.Point(0, 87);
            this.pnlFieldControls.Name = "pnlFieldControls";
            this.pnlFieldControls.Size = new System.Drawing.Size(492, 279);
            this.pnlFieldControls.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 36);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 11);
            this.progressBar1.TabIndex = 5;
            // 
            // MassFieldChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 366);
            this.Controls.Add(this.pnlFieldControls);
            this.Controls.Add(this.pnlAutoFill);
            this.Controls.Add(this.pnlDefaultControls);
            this.MinimumSize = new System.Drawing.Size(432, 400);
            this.Name = "MassFieldChanger";
            this.Text = "MassFieldChanger";
            this.Shown += new System.EventHandler(this.MassFieldChanger_Shown);
            this.pnlDefaultControls.ResumeLayout(false);
            this.pnlDefaultControls.PerformLayout();
            this.pnlAutoFill.ResumeLayout(false);
            this.pnlAutoFill.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDefaultControls;
        private System.Windows.Forms.Button btnResetValues;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Label lblShowChunks;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.ComboBox cbEndChunk;
        private System.Windows.Forms.ComboBox cbStartChunk;
        private System.Windows.Forms.Label lblAutoFill;
        private System.Windows.Forms.TextBox tbInitialValue;
        private System.Windows.Forms.Panel pnlAutoFill;
        private System.Windows.Forms.TextBox tbChange;
        private System.Windows.Forms.Label lblInitalValue;
        private System.Windows.Forms.Label lblChangeValue;
        private System.Windows.Forms.Button btnFill;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlFieldControls;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}