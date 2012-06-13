namespace entity.Renderers
{
    using System.Windows.Forms;

    public partial class LightmapViewer : Form
	{

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // ModelViewer
            //
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "ModelViewer";

            this.ResumeLayout(false);
        }
	}
}

