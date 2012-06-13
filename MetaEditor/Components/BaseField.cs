// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseField.cs" company="">
//   
// </copyright>
// <summary>
//   The base field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The base field.
    /// </summary>
    partial class BaseField : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The ent name.
        /// </summary>
        public string EntName = "Error in getting plugin element name";

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber;

        /// <summary>
        /// The chunk offset.
        /// </summary>
        public int chunkOffset;

        /// <summary>
        /// The map index.
        /// </summary>
        public Map map;

        /// <summary>
        /// The offset in map.
        /// </summary>
        public int offsetInMap;

        #endregion
    }
}