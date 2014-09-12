using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HaloMap.Map;
using HaloMap.Meta;

namespace entity.MetaEditor2
{
    public partial class BaseField : UserControl
    {
        #region Fields
        public int LineNumber;
        public Map map;
        public int chunkOffset;
        public int offsetInMap;
        /// <summary>
        /// Size of control in bytes
        /// </summary>
        public int size;
        public string EntName = "Error in getting plugin element name";
        public Meta meta;
        public string description = string.Empty;
        #endregion

        public BaseField()
        {
            this.Leave += new EventHandler(BaseField_Leave);
            size = 0;
        }

        public virtual void BaseField_Leave(object sender, EventArgs e)
        {
        }

        public virtual void Save()
        {
            throw new NotImplementedException("Error Saving " + this.GetType().ToString());
        }
    }
}
