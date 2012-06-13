// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsToShow.cs" company="">
//   
// </copyright>
// <summary>
//   The items to show.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Forms
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The items to show.
    /// </summary>
    /// <remarks></remarks>
    public partial class ItemsToShow : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsToShow"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ItemsToShow()
        {
            InitializeComponent();
            this.checkbReflexives.Checked = MetaEditor.ShowReflexives;
            this.checkBIdents.Checked = MetaEditor.ShowIdents;
            this.checkBSIDS.Checked = MetaEditor.ShowSIDs;
            this.checkBFloats.Checked = MetaEditor.ShowFloats;
            this.checkBString32s.Checked = MetaEditor.ShowString32s;
            this.checkBString256s.Checked = MetaEditor.ShowString256s;
            this.checkBUnicodeString64s.Checked = MetaEditor.ShowUnicodeString64s;
            this.checkBUnicodeString256s.Checked = MetaEditor.ShowUnicodeString256s;
            this.checkBInts.Checked = MetaEditor.ShowInts;
            this.checkBUints.Checked = MetaEditor.ShowUints;
            this.checkBShorts.Checked = MetaEditor.ShowShorts;
            this.checkBUshorts.Checked = MetaEditor.ShowUshorts;
            this.checkBBitmask16s.Checked = MetaEditor.ShowBitmask16s;
            this.checkBBitmask8s.Checked = MetaEditor.ShowBitmask8s;
            this.checkBBitmask32s.Checked = MetaEditor.ShowBitmask32s;
            this.checkBEnum8s.Checked = MetaEditor.ShowEnum8s;
            this.checkBEnum16s.Checked = MetaEditor.ShowEnum16s;
            this.checkBEnum32s.Checked = MetaEditor.ShowEnum32s;
            this.checkBBlockIndex8s.Checked = MetaEditor.ShowBlockIndex8s;
            this.checkBBlockIndex16s.Checked = MetaEditor.ShowBlockIndex16s;
            this.checkBBlockIndex32s.Checked = MetaEditor.ShowBlockIndex32s;
            this.checkBBytes.Checked = MetaEditor.ShowBytes;
            this.checkBInvisibles.Checked = MetaEditor.ShowInvisibles;
            this.checkBUndefined.Checked = MetaEditor.ShowUndefineds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The butt cancel_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The butt check all_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttCheckAll_Click(object sender, EventArgs e)
        {
            this.checkbReflexives.Checked = true;
            this.checkBIdents.Checked = true;
            this.checkBSIDS.Checked = true;
            this.checkBFloats.Checked = true;
            this.checkBString32s.Checked = true;
            this.checkBString256s.Checked = true;
            this.checkBUnicodeString64s.Checked = true;
            this.checkBUnicodeString256s.Checked = true;
            this.checkBInts.Checked = true;
            this.checkBUints.Checked = true;
            this.checkBShorts.Checked = true;
            this.checkBUshorts.Checked = true;
            this.checkBBitmask16s.Checked = true;
            this.checkBBitmask8s.Checked = true;
            this.checkBBitmask32s.Checked = true;
            this.checkBEnum8s.Checked = true;
            this.checkBEnum16s.Checked = true;
            this.checkBEnum32s.Checked = true;
            this.checkBBlockIndex8s.Checked = true;
            this.checkBBlockIndex16s.Checked = true;
            this.checkBBlockIndex32s.Checked = true;
            this.checkBBytes.Checked = true;
            this.checkBInvisibles.Checked = true;
            this.checkBUndefined.Checked = true;
        }

        /// <summary>
        /// The butt o k_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttOK_Click(object sender, EventArgs e)
        {
            MetaEditor.ShowReflexives = this.checkbReflexives.Checked;
            MetaEditor.ShowIdents = this.checkBIdents.Checked;
            MetaEditor.ShowSIDs = this.checkBSIDS.Checked;
            MetaEditor.ShowFloats = this.checkBFloats.Checked;
            MetaEditor.ShowString32s = this.checkBString32s.Checked;
            MetaEditor.ShowString256s = this.checkBString256s.Checked;
            MetaEditor.ShowUnicodeString64s = this.checkBUnicodeString64s.Checked;
            MetaEditor.ShowUnicodeString256s = this.checkBUnicodeString256s.Checked;
            MetaEditor.ShowInts = this.checkBInts.Checked;
            MetaEditor.ShowUints = this.checkBUints.Checked;
            MetaEditor.ShowShorts = this.checkBShorts.Checked;
            MetaEditor.ShowUshorts = this.checkBUshorts.Checked;
            MetaEditor.ShowBitmask16s = this.checkBBitmask16s.Checked;
            MetaEditor.ShowBitmask8s = this.checkBBitmask8s.Checked;
            MetaEditor.ShowBitmask32s = this.checkBBitmask32s.Checked;
            MetaEditor.ShowEnum8s = this.checkBEnum8s.Checked;
            MetaEditor.ShowEnum16s = this.checkBEnum16s.Checked;
            MetaEditor.ShowEnum32s = this.checkBEnum32s.Checked;
            MetaEditor.ShowBlockIndex8s = this.checkBBlockIndex8s.Checked;
            MetaEditor.ShowBlockIndex16s = this.checkBBlockIndex16s.Checked;
            MetaEditor.ShowBlockIndex32s = this.checkBBlockIndex32s.Checked;
            MetaEditor.ShowBytes = this.checkBBytes.Checked;
            MetaEditor.ShowInvisibles = this.checkBInvisibles.Checked;
            MetaEditor.ShowUndefineds = this.checkBUndefined.Checked;
            this.Close();
        }

        /// <summary>
        /// The butt uncheck all_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void buttUncheckAll_Click(object sender, EventArgs e)
        {
            this.checkbReflexives.Checked = false;
            this.checkBIdents.Checked = false;
            this.checkBSIDS.Checked = false;
            this.checkBFloats.Checked = false;
            this.checkBString32s.Checked = false;
            this.checkBString256s.Checked = false;
            this.checkBUnicodeString64s.Checked = false;
            this.checkBUnicodeString256s.Checked = false;
            this.checkBInts.Checked = false;
            this.checkBUints.Checked = false;
            this.checkBShorts.Checked = false;
            this.checkBUshorts.Checked = false;
            this.checkBBitmask16s.Checked = false;
            this.checkBBitmask8s.Checked = false;
            this.checkBBitmask32s.Checked = false;
            this.checkBEnum8s.Checked = false;
            this.checkBEnum16s.Checked = false;
            this.checkBEnum32s.Checked = false;
            this.checkBBlockIndex8s.Checked = false;
            this.checkBBlockIndex16s.Checked = false;
            this.checkBBlockIndex32s.Checked = false;
            this.checkBBytes.Checked = false;
            this.checkBInvisibles.Checked = false;
            this.checkBUndefined.Checked = false;
        }

        #endregion
    }
}