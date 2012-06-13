// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="">
//   
// </copyright>
// <summary>
//   The plugin manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity // changing this namespace will break libraries...
{
    using System;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The entity plugin.
    /// </summary>
    /// <remarks></remarks>
    public partial class EntityPlugin : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The author.
        /// </summary>
        public string Author;

        /// <summary>
        /// The map number.
        /// </summary>
        public int map;

        /// <summary>
        /// The plugin name.
        /// </summary>
        public string PluginName;

        /// <summary>
        /// The tagtype.
        /// </summary>
        public string tagtype = "*.*";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPlugin"/> class.
        /// </summary>
        /// <remarks></remarks>
        public EntityPlugin()
        {
            this.Dock = DockStyle.Fill;
            this.PluginName = "Default Plugin";
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get name.
        /// </summary>
        /// <returns>The get name.</returns>
        /// <remarks></remarks>
        public string GetName()
        {
            return this.PluginName;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="progressbar">The progressbar.</param>
        /// <remarks></remarks>
        public virtual void Run(Map map, ref ToolStripProgressBar progressbar)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The entity plugin_ load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void EntityPlugin_Load(object sender, EventArgs e)
        {
        }

        #endregion
    }
}