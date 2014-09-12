using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HaloMap.Plugins;

namespace entity.MetaEditor2
{
    public class reflexiveData
    {
        #region Constants and Fields
        public int baseOffset;
        public int chunkCount;
        public int chunkSelected;
        public int inTagNumber;
        public TreeNode node;
        public reflexiveData parent;
        public reflexiveData[] children;
        public IFPIO.Reflexive reflexive;
        public ToolStripItem[] tsItems;
        #endregion

        #region Constructors and Destructors
        public reflexiveData(reflexiveData parentReflexive)
        {
            this.node = null;
            this.reflexive = null;
            this.baseOffset = -1;
            this.chunkCount = 0;
            this.chunkSelected = -1;
            this.inTagNumber = -1;
            this.parent = parentReflexive;
            this.children = null;
        }
        #endregion
    }

}
