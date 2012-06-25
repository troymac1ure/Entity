using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using entity;


namespace TestLibrary
{
    public class Class1: entity.EntityPlugin
    {
        private CheckBox checkBox1;

        public int MapNumber;

        public Class1()
        {
            this.Dock = DockStyle.Fill;
            this.PluginName = "test";
            InitializeComponent();
        }
        public override void Run(int mapnum,ref ToolStripProgressBar progress,ref ListView lv)
        {
            MapNumber = mapnum;
            MessageBox.Show("Refresh Map");

            Maps.Refresh(MapNumber);
            
      
        }

        private void InitializeComponent()
        {
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(94, 83);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // Class1
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.checkBox1);
            this.Name = "Class1";
            this.Size = new System.Drawing.Size(787, 337);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
