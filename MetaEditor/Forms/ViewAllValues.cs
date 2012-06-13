// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewAllValues.cs" company="">
//   
// </copyright>
// <summary>
//   The custom tree node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Forms
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The custom tree node.
    /// </summary>
    /// <remarks></remarks>
    public class CustomTreeNode : TreeNode
    {
        #region Constants and Fields

        /// <summary>
        /// The vi.
        /// </summary>
        public ValueInfo VI;

        #endregion
    }

    /// <summary>
    /// The value info.
    /// </summary>
    /// <remarks></remarks>
    public class ValueInfo
    {
    }

    /// <summary>
    /// The view all values.
    /// </summary>
    /// <remarks></remarks>
    public partial class ViewAllValues : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The line number.
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAllValues"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ViewAllValues()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load variables.
        /// </summary>
        /// <param name="map">The map index.</param>
        /// <param name="entlineNumber">The entline number.</param>
        /// <remarks></remarks>
        public void LoadVariables(Map map, int entlineNumber)
        {
            this.map = map;
            this.lineNumber = entlineNumber;

            // FilterEntItems();
        }

        #endregion

        // private void FilterEntItems()
        // {
        // IFPIO ifp = IFP.IFPHashMap.GetIfp(map.SelectedMeta.type);
        // ifp.items = this.RecursiveFilterEntItems(ifp.items);
        // }
        #region Methods

        /// <summary>
        /// The recursive filter ent items.
        /// </summary>
        /// <param name="entItems">The ent items.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private object[] RecursiveFilterEntItems(object[] entItems)
        {
            List<object> newItems = new List<object>(0);
            for (int counter = 0; counter < entItems.Length; counter++)
            {
                if (((IFPIO.BaseObject)entItems[counter]).ObjectType == IFPIO.ObjectEnum.Struct)
                {
                    object[] temp = RecursiveFilterEntItems(((IFPIO.Reflexive)entItems[counter]).items);
                    if (temp != null && temp.Length != 0)
                    {
                        newItems.Add(temp);
                    }
                }
                else if (((IFPIO.BaseObject)entItems[counter]).lineNumber == this.lineNumber)
                {
                    newItems.Add(entItems[counter]);
                    break;
                }
            }

            return newItems.Count == 0 ? null : newItems.ToArray();
        }

        #endregion

        // this.this.map = map;
        // this.EntIndexer = entIndexer;
        // FilterEntItems();
        // }
        // private void FilterEntItems()
        // {
        // Ent = IFP.IFPHashMap.GetIfp(map.SelectedMeta.type);
        // this.RecursiveFilterEntItems(0);
        // }
        // private void RecursiveFilterEntItems(int ItemIndexer)
        // {
        // while (ItemIndexer != -1)
        // {
        // switch (this.Ent.ENTElements[ItemIndexer].ObjectType)
        // {
        // case IFPIO.ObjectEnum.Reflexive:
        // {
        // RecursiveFilterEntItems(this.Ent.ENTElements[ItemIndexer].child);
        // }
        // default:
        // {
        // }
        // }
        // ItemIndexer = this.Ent.ENTElements[ItemIndexer].siblingNext;
        // }
        // }
        // void tt()
        // {
        // TreeView tv = new TreeView();
        // tv
        // }
        // Changed ifpio
        // Changed MetaEditor
    }
}