// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Float.cs" company="">
//   
// </copyright>
// <summary>
//   The float.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Components
{
    using System;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The float.
    /// </summary>
    public partial class Float : BaseField
    {
        //// public int LineNumber;
        // private Map mapindex;
        // public int chunkOffset;
        // public int offsetInMap;
        // public string EntName = "Error in getting plugin element name";
        #region Constants and Fields

        /// <summary>
        /// The value.
        /// </summary>
        public float value;

        /// <summary>
        /// The is nulled out reflexive.
        /// </summary>
        private bool isNulledOutReflexive = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Float"/> class.
        /// </summary>
        /// <param name="iEntName">
        /// The i ent name.
        /// </param>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="iOffsetInChunk">
        /// The i offset in chunk.
        /// </param>
        public Float(string iEntName, Map map, int iOffsetInChunk)
        {
            this.chunkOffset = iOffsetInChunk;
            this.map = map;
            this.EntName = iEntName;
            InitializeComponent();
            this.Size = this.PreferredSize;
            this.Dock = DockStyle.Top;
            this.Controls[0].Text = EntName;
            this.AutoSize = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The populate.
        /// </summary>
        /// <param name="iOffset">
        /// The i offset.
        /// </param>
        public void Populate(int iOffset)
        {
            this.isNulledOutReflexive = false;
            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BR.BaseStream.Position = iOffset + this.chunkOffset;
            this.value = map.BR.ReadSingle();
            this.Controls[2].Text = value.ToString();
            this.offsetInMap = iOffset + this.chunkOffset;
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            if (this.isNulledOutReflexive)
            {
                return;
            }

            bool openedMap = false;
            if (map.isOpen == false)
            {
                map.OpenMap(MapTypes.Internal);
                openedMap = true;
            }

            map.BW.BaseStream.Position = this.offsetInMap;
            map.BW.Write(this.value);
            if (openedMap)
            {
                map.CloseMap();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The text box 1_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).TextChanged -= textBox1_TextChanged;
            try
            {
                this.value = Convert.ToSingle(((TextBox)sender).Text);
            }
            catch
            {
                MessageBox.Show(((TextBox)sender).Text + " Is not a float");
                ((TextBox)sender).Text = this.value.ToString();
            }

            ((TextBox)sender).TextChanged += textBox1_TextChanged;
        }

        #endregion
    }
}