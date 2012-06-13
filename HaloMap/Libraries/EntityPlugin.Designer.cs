namespace entity
{
    using System.Windows.Forms;

    public partial class EntityPlugin : UserControl
	{

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // EntityPlugin
            //
            this.Name = "EntityPlugin";
            this.Load += new System.EventHandler(this.EntityPlugin_Load);
            this.ResumeLayout(false);
        }

	}
}

