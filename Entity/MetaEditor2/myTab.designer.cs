using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace entity.MetaEditorPlus
{
    partial class myTab : UserControl
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
            this.tabRight = new System.Windows.Forms.PictureBox();
            this.tabLeft = new System.Windows.Forms.PictureBox();
            this.tabCenter = new System.Windows.Forms.PictureBox();
            this.label = new entity.MetaEditorPlus.TransparentText();
            ((System.ComponentModel.ISupportInitialize)(this.tabRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabCenter)).BeginInit();
            this.SuspendLayout();
            // 
            // tabRight
            // 
            this.tabRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabRight.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tabRight.BackgroundImage = global::entity.Properties.Resources.TabRight;
            this.tabRight.Location = new System.Drawing.Point(139, 0);
            this.tabRight.Margin = new System.Windows.Forms.Padding(0);
            this.tabRight.Name = "tabRight";
            this.tabRight.Size = new System.Drawing.Size(26, 25);
            this.tabRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tabRight.TabIndex = 1;
            this.tabRight.TabStop = false;
            // 
            // tabLeft
            // 
            this.tabLeft.BackgroundImage = global::entity.Properties.Resources.TabLeft;
            this.tabLeft.InitialImage = global::entity.Properties.Resources.TabLeft;
            this.tabLeft.Location = new System.Drawing.Point(0, 0);
            this.tabLeft.Margin = new System.Windows.Forms.Padding(0);
            this.tabLeft.Name = "tabLeft";
            this.tabLeft.Size = new System.Drawing.Size(18, 25);
            this.tabLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tabLeft.TabIndex = 1;
            this.tabLeft.TabStop = false;
            // 
            // tabCenter
            // 
            this.tabCenter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCenter.BackgroundImage = global::entity.Properties.Resources.TabCenter;
            this.tabCenter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabCenter.Location = new System.Drawing.Point(18, 0);
            this.tabCenter.Margin = new System.Windows.Forms.Padding(0);
            this.tabCenter.Name = "tabCenter";
            this.tabCenter.Size = new System.Drawing.Size(121, 25);
            this.tabCenter.TabIndex = 1;
            this.tabCenter.TabStop = false;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Location = new System.Drawing.Point(18, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(121, 25);
            this.label.TabIndex = 2;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myTab
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tabRight);
            this.Controls.Add(this.tabLeft);
            this.Controls.Add(this.label);
            this.Controls.Add(this.tabCenter);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "myTab";
            this.Size = new System.Drawing.Size(168, 24);
            ((System.ComponentModel.ISupportInitialize)(this.tabRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabCenter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox tabLeft;
        private System.Windows.Forms.PictureBox tabCenter;
        private System.Windows.Forms.PictureBox tabRight;
        private entity.MetaEditorPlus.TransparentText label;

    }
}
