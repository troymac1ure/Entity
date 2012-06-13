namespace entity.MapForms
{
    partial class ChunkClonerWindow
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.expand = new System.Windows.Forms.Button();
            this.collapse = new System.Windows.Forms.Button();
            this.savemeta = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.copybutton = new System.Windows.Forms.Button();
            this.chunkamount = new System.Windows.Forms.TextBox();
            this.addchunks = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(761, 408);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.expand);
            this.panel2.Controls.Add(this.collapse);
            this.panel2.Controls.Add(this.savemeta);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.treeView1);
            this.panel2.Controls.Add(this.copybutton);
            this.panel2.Controls.Add(this.chunkamount);
            this.panel2.Controls.Add(this.addchunks);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(10, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(739, 386);
            this.panel2.TabIndex = 0;
            // 
            // expand
            // 
            this.expand.Location = new System.Drawing.Point(514, 249);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(217, 24);
            this.expand.TabIndex = 12;
            this.expand.Text = "Expand All Nodes";
            this.expand.UseVisualStyleBackColor = true;
            this.expand.Click += new System.EventHandler(this.expand_Click);
            // 
            // collapse
            // 
            this.collapse.Location = new System.Drawing.Point(514, 219);
            this.collapse.Name = "collapse";
            this.collapse.Size = new System.Drawing.Size(217, 24);
            this.collapse.TabIndex = 11;
            this.collapse.Text = "Collapse All Nodes";
            this.collapse.UseVisualStyleBackColor = true;
            this.collapse.Click += new System.EventHandler(this.collapse_Click);
            // 
            // savemeta
            // 
            this.savemeta.Location = new System.Drawing.Point(515, 354);
            this.savemeta.Name = "savemeta";
            this.savemeta.Size = new System.Drawing.Size(217, 24);
            this.savemeta.TabIndex = 10;
            this.savemeta.Text = "Save Meta";
            this.savemeta.UseVisualStyleBackColor = true;
            this.savemeta.Click += new System.EventHandler(this.savemeta_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(514, 151);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(217, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Delete Reflex/Chunk";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(514, 122);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(217, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Clone";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(514, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(217, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Add To Selected Reflex/Chunk";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(514, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(217, 24);
            this.button1.TabIndex = 6;
            this.button1.Text = "Add Meta To Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView1
            // 
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(6, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(502, 376);
            this.treeView1.TabIndex = 5;
            // 
            // copybutton
            // 
            this.copybutton.Location = new System.Drawing.Point(515, 3);
            this.copybutton.Name = "copybutton";
            this.copybutton.Size = new System.Drawing.Size(217, 24);
            this.copybutton.TabIndex = 4;
            this.copybutton.Text = "Copy To Clip Board";
            this.copybutton.UseVisualStyleBackColor = true;
            this.copybutton.Click += new System.EventHandler(this.copybutton_Click);
            // 
            // chunkamount
            // 
            this.chunkamount.Location = new System.Drawing.Point(620, 99);
            this.chunkamount.Name = "chunkamount";
            this.chunkamount.Size = new System.Drawing.Size(100, 20);
            this.chunkamount.TabIndex = 3;
            this.chunkamount.Text = "1";
            this.chunkamount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chunkamount.TextChanged += new System.EventHandler(this.chunkamount_TextChanged);
            // 
            // addchunks
            // 
            this.addchunks.Location = new System.Drawing.Point(514, 33);
            this.addchunks.Name = "addchunks";
            this.addchunks.Size = new System.Drawing.Size(217, 23);
            this.addchunks.TabIndex = 2;
            this.addchunks.Text = "Overwrite Selected Chunk/Reflexive";
            this.addchunks.UseVisualStyleBackColor = true;
            this.addchunks.Click += new System.EventHandler(this.addchunks_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(512, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Amount Of Chunks:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChunkClonerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(761, 408);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChunkClonerWindow";
            this.Text = "Chunk Cloner";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button addchunks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox chunkamount;
        private System.Windows.Forms.Button copybutton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button savemeta;
        private System.Windows.Forms.Button expand;
        private System.Windows.Forms.Button collapse;
    }
}