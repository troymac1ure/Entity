namespace entity.MapForms
{
    using System.Collections.Generic;

    public partial class IdentSwapper : System.Windows.Forms.Form
	{

        private System.Windows.Forms.Button button1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private List<identSwapLayout> IdentSwaps = new List<identSwapLayout>();

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
	}
}

