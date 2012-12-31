namespace entity
{
    partial class HelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            this.browser = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsIndex = new System.Windows.Forms.ToolStripButton();
            this.tsBack = new System.Windows.Forms.ToolStripButton();
            this.tsForward = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Location = new System.Drawing.Point(0, 25);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(534, 317);
            this.browser.TabIndex = 0;
            this.browser.CanGoForwardChanged += new System.EventHandler(this.browser_CanGoForwardChanged);
            this.browser.CanGoBackChanged += new System.EventHandler(this.browser_CanGoBackChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsIndex,
            this.tsBack,
            this.tsForward});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(534, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsIndex
            // 
            this.tsIndex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsIndex.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tsIndex.Image = ((System.Drawing.Image)(resources.GetObject("tsIndex.Image")));
            this.tsIndex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsIndex.Name = "tsIndex";
            this.tsIndex.Size = new System.Drawing.Size(54, 22);
            this.tsIndex.Text = "[Index]";
            this.tsIndex.Click += new System.EventHandler(this.tsIndex_Click);
            // 
            // tsBack
            // 
            this.tsBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBack.Enabled = false;
            this.tsBack.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tsBack.Image = ((System.Drawing.Image)(resources.GetObject("tsBack.Image")));
            this.tsBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBack.Name = "tsBack";
            this.tsBack.Size = new System.Drawing.Size(48, 22);
            this.tsBack.Text = "[Back]";
            this.tsBack.Click += new System.EventHandler(this.tsBack_Click);
            // 
            // tsForward
            // 
            this.tsForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsForward.Enabled = false;
            this.tsForward.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tsForward.Image = ((System.Drawing.Image)(resources.GetObject("tsForward.Image")));
            this.tsForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsForward.Name = "tsForward";
            this.tsForward.Size = new System.Drawing.Size(67, 22);
            this.tsForward.Text = "[Forward]";
            this.tsForward.Click += new System.EventHandler(this.tsForward_Click);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 342);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.toolStrip1);
            this.Name = "HelpForm";
            this.Text = "Help Contents";
            this.Activated += new System.EventHandler(this.HelpForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HelpForm_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsIndex;
        private System.Windows.Forms.ToolStripButton tsBack;
        private System.Windows.Forms.ToolStripButton tsForward;
    }
}