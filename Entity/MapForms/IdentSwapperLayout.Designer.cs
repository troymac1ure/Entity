namespace entity.MapForms
{
    using System.Windows.Forms;

    public partial class identSwapLayout : UserControl
	{
        public System.Windows.Forms.Label oldident;
        public System.Windows.Forms.ComboBox tagname;
        public System.Windows.Forms.ComboBox tagtype;

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
	}
}

