namespace entity.Renderers
{
    using System.Windows.Forms;
    using System.Drawing;

    public partial class BSPCollisionViewer : Form
	{
        /// <summary>
        /// The initialize component.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTabChange = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblZLabel = new System.Windows.Forms.Label();
            this.lblYLabel = new System.Windows.Forms.Label();
            this.lblXLabel = new System.Windows.Forms.Label();
            this.lblEditMode = new System.Windows.Forms.Label();
            this.lblEditModeLabel = new System.Windows.Forms.Label();
            this.gbSurfaceFlags = new System.Windows.Forms.GroupBox();
            this.cbConveyor = new System.Windows.Forms.CheckBox();
            this.cbInvalid = new System.Windows.Forms.CheckBox();
            this.cbBreakable = new System.Windows.Forms.CheckBox();
            this.cbClimable = new System.Windows.Forms.CheckBox();
            this.cbInvisible = new System.Windows.Forms.CheckBox();
            this.cbTwoSided = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.gbSurfaceFlags.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.lblTabChange);
            this.panel1.Controls.Add(this.lblZ);
            this.panel1.Controls.Add(this.lblY);
            this.panel1.Controls.Add(this.lblX);
            this.panel1.Controls.Add(this.lblZLabel);
            this.panel1.Controls.Add(this.lblYLabel);
            this.panel1.Controls.Add(this.lblXLabel);
            this.panel1.Controls.Add(this.lblEditMode);
            this.panel1.Controls.Add(this.lblEditModeLabel);
            this.panel1.Controls.Add(this.gbSurfaceFlags);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 180);
            this.panel1.TabIndex = 0;
            // 
            // lblTabChange
            // 
            this.lblTabChange.AutoSize = true;
            this.lblTabChange.Location = new System.Drawing.Point(3, 20);
            this.lblTabChange.Name = "lblTabChange";
            this.lblTabChange.Size = new System.Drawing.Size(86, 13);
            this.lblTabChange.TabIndex = 9;
            this.lblTabChange.Text = "(TAB to Change)";
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblZ.Location = new System.Drawing.Point(28, 67);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(13, 13);
            this.lblZ.TabIndex = 7;
            this.lblZ.Text = "0";
            this.lblZ.Visible = false;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblY.Location = new System.Drawing.Point(28, 51);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(13, 13);
            this.lblY.TabIndex = 6;
            this.lblY.Text = "0";
            this.lblY.Visible = false;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblX.Location = new System.Drawing.Point(28, 35);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(13, 13);
            this.lblX.TabIndex = 5;
            this.lblX.Text = "0";
            this.lblX.Visible = false;
            // 
            // lblZLabel
            // 
            this.lblZLabel.AutoSize = true;
            this.lblZLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZLabel.Location = new System.Drawing.Point(3, 67);
            this.lblZLabel.Name = "lblZLabel";
            this.lblZLabel.Size = new System.Drawing.Size(19, 13);
            this.lblZLabel.TabIndex = 4;
            this.lblZLabel.Text = "Z:";
            this.lblZLabel.Visible = false;
            // 
            // lblYLabel
            // 
            this.lblYLabel.AutoSize = true;
            this.lblYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYLabel.Location = new System.Drawing.Point(3, 51);
            this.lblYLabel.Name = "lblYLabel";
            this.lblYLabel.Size = new System.Drawing.Size(19, 13);
            this.lblYLabel.TabIndex = 3;
            this.lblYLabel.Text = "Y:";
            this.lblYLabel.Visible = false;
            // 
            // lblXLabel
            // 
            this.lblXLabel.AutoSize = true;
            this.lblXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXLabel.Location = new System.Drawing.Point(3, 35);
            this.lblXLabel.Name = "lblXLabel";
            this.lblXLabel.Size = new System.Drawing.Size(19, 13);
            this.lblXLabel.TabIndex = 2;
            this.lblXLabel.Text = "X:";
            this.lblXLabel.Visible = false;
            // 
            // lblEditMode
            // 
            this.lblEditMode.AutoSize = true;
            this.lblEditMode.Location = new System.Drawing.Point(67, 7);
            this.lblEditMode.Name = "lblEditMode";
            this.lblEditMode.Size = new System.Drawing.Size(33, 13);
            this.lblEditMode.TabIndex = 1;
            this.lblEditMode.Text = "None";
            // 
            // lblEditModeLabel
            // 
            this.lblEditModeLabel.AutoSize = true;
            this.lblEditModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditModeLabel.Location = new System.Drawing.Point(3, 7);
            this.lblEditModeLabel.Name = "lblEditModeLabel";
            this.lblEditModeLabel.Size = new System.Drawing.Size(68, 13);
            this.lblEditModeLabel.TabIndex = 0;
            this.lblEditModeLabel.Text = "Edit Mode:";
            // 
            // gbSurfaceFlags
            // 
            this.gbSurfaceFlags.Controls.Add(this.cbConveyor);
            this.gbSurfaceFlags.Controls.Add(this.cbInvalid);
            this.gbSurfaceFlags.Controls.Add(this.cbBreakable);
            this.gbSurfaceFlags.Controls.Add(this.cbClimable);
            this.gbSurfaceFlags.Controls.Add(this.cbInvisible);
            this.gbSurfaceFlags.Controls.Add(this.cbTwoSided);
            this.gbSurfaceFlags.Location = new System.Drawing.Point(6, 67);
            this.gbSurfaceFlags.Name = "gbSurfaceFlags";
            this.gbSurfaceFlags.Size = new System.Drawing.Size(104, 110);
            this.gbSurfaceFlags.TabIndex = 8;
            this.gbSurfaceFlags.TabStop = false;
            this.gbSurfaceFlags.Text = "Surface Flags";
            this.gbSurfaceFlags.Visible = false;
            // 
            // cbConveyor
            // 
            this.cbConveyor.AutoSize = true;
            this.cbConveyor.Location = new System.Drawing.Point(6, 90);
            this.cbConveyor.Name = "cbConveyor";
            this.cbConveyor.Size = new System.Drawing.Size(71, 17);
            this.cbConveyor.TabIndex = 5;
            this.cbConveyor.Text = "Conveyor";
            this.cbConveyor.UseVisualStyleBackColor = true;
            this.cbConveyor.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbInvalid
            // 
            this.cbInvalid.AutoSize = true;
            this.cbInvalid.Location = new System.Drawing.Point(6, 75);
            this.cbInvalid.Name = "cbInvalid";
            this.cbInvalid.Size = new System.Drawing.Size(57, 17);
            this.cbInvalid.TabIndex = 4;
            this.cbInvalid.Text = "Invalid";
            this.cbInvalid.UseVisualStyleBackColor = true;
            this.cbInvalid.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbBreakable
            // 
            this.cbBreakable.AutoSize = true;
            this.cbBreakable.Location = new System.Drawing.Point(6, 60);
            this.cbBreakable.Name = "cbBreakable";
            this.cbBreakable.Size = new System.Drawing.Size(74, 17);
            this.cbBreakable.TabIndex = 3;
            this.cbBreakable.Text = "Breakable";
            this.cbBreakable.UseVisualStyleBackColor = true;
            this.cbBreakable.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbClimable
            // 
            this.cbClimable.AutoSize = true;
            this.cbClimable.Location = new System.Drawing.Point(6, 45);
            this.cbClimable.Name = "cbClimable";
            this.cbClimable.Size = new System.Drawing.Size(65, 17);
            this.cbClimable.TabIndex = 2;
            this.cbClimable.Text = "Climable";
            this.cbClimable.UseVisualStyleBackColor = true;
            this.cbClimable.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbInvisible
            // 
            this.cbInvisible.AutoSize = true;
            this.cbInvisible.Location = new System.Drawing.Point(6, 30);
            this.cbInvisible.Name = "cbInvisible";
            this.cbInvisible.Size = new System.Drawing.Size(64, 17);
            this.cbInvisible.TabIndex = 1;
            this.cbInvisible.Text = "Invisible";
            this.cbInvisible.UseVisualStyleBackColor = true;
            this.cbInvisible.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbTwoSided
            // 
            this.cbTwoSided.AutoSize = true;
            this.cbTwoSided.Location = new System.Drawing.Point(6, 15);
            this.cbTwoSided.Name = "cbTwoSided";
            this.cbTwoSided.Size = new System.Drawing.Size(77, 17);
            this.cbTwoSided.TabIndex = 0;
            this.cbTwoSided.Text = "Two Sided";
            this.cbTwoSided.UseVisualStyleBackColor = true;
            this.cbTwoSided.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // BSPCollisionViewer
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panel1);
            this.Name = "BSPCollisionViewer";
            this.Text = "BSP Viewer";
            this.MouseEnter += new System.EventHandler(this.BSPCollisionViewer_MouseEnter);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BSPCollisionViewer_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BSPCollisionViewer_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ModelViewer_MouseDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbSurfaceFlags.ResumeLayout(false);
            this.gbSurfaceFlags.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}

