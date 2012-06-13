namespace entity.MetaEditor2
{
    partial class Ident
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbTagIdent = new System.Windows.Forms.ComboBox();
            this.cbTagType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(641, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tag / Ident";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 7);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label4.Size = new System.Drawing.Size(171, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "error in getting plugin element name";
            // 
            // cbTagIdent
            // 
            this.cbTagIdent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTagIdent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTagIdent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTagIdent.FormattingEnabled = true;
            this.cbTagIdent.Location = new System.Drawing.Point(243, 4);
            this.cbTagIdent.MinimumSize = new System.Drawing.Size(100,21);
            this.cbTagIdent.Name = "cbTagIdent";
            this.cbTagIdent.Size = new System.Drawing.Size(348, 21);
            this.cbTagIdent.TabIndex = 3;
            this.cbTagIdent.Text = "Ident";
            this.cbTagIdent.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbTagIdent_MouseClick);
            this.cbTagIdent.DropDownClosed += new System.EventHandler(this.cbTagIdent_DropDownClose);
            this.cbTagIdent.DropDown += new System.EventHandler(this.cbTagIdent_DropDown);
            this.cbTagIdent.TextChanged += new System.EventHandler(this.cbTagIdent_TextChanged);
            // 
            // cbTagType
            // 
            this.cbTagType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTagType.FormattingEnabled = true;
            this.cbTagType.Location = new System.Drawing.Point(180, 4);
            this.cbTagType.Name = "cbTagType";
            this.cbTagType.Size = new System.Drawing.Size(57, 21);
            this.cbTagType.TabIndex = 2;
            this.cbTagType.Text = "Type";
            this.cbTagType.SelectedIndexChanged += new System.EventHandler(this.cbTagType_SelectedIndexChanged);
            this.cbTagType.DropDown += new System.EventHandler(this.cbTagType_DropDown);
            // 
            // Ident
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbTagIdent);
            this.Controls.Add(this.cbTagType);
            this.Controls.Add(this.label1);
            this.Name = "Ident";
            this.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.Size = new System.Drawing.Size(701, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox TagType;
        private System.Windows.Forms.ComboBox cbTagIdent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbTagType;
        private System.Windows.Forms.Label label1;
    }
}
