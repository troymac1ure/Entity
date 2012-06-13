// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentSwapperLayout.cs" company="">
//   
// </copyright>
// <summary>
//   The ident swap layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The ident swap layout.
    /// </summary>
    /// <remarks></remarks>
    partial class identSwapLayout : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The id.
        /// </summary>
        public int id;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="identSwapLayout"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public identSwapLayout(Map map)
        {
            this.map = map;
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeComponent()
        {
            this.oldident = new Label();
            this.tagname = new ComboBox();
            this.tagtype = new ComboBox();
            this.SuspendLayout();

            // oldident
            this.oldident.BackColor = Color.LightGoldenrodYellow;
            this.oldident.BorderStyle = BorderStyle.FixedSingle;
            this.oldident.Location = new Point(0, 0);
            this.oldident.Name = "oldident";
            this.oldident.Size = new Size(447, 23);
            this.oldident.TabIndex = 1;
            this.oldident.TextAlign = ContentAlignment.MiddleCenter;

            // tagname
            this.tagname.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.tagname.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.tagname.BackColor = Color.White;
            this.tagname.DropDownStyle = ComboBoxStyle.DropDownList;
            this.tagname.FlatStyle = FlatStyle.System;
            this.tagname.Location = new Point(85, 25);
            this.tagname.Name = "tagname";
            this.tagname.Size = new Size(362, 21);
            this.tagname.TabIndex = 3;

            // tagtype
            this.tagtype.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.tagtype.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.tagtype.BackColor = Color.White;
            this.tagtype.DropDownStyle = ComboBoxStyle.DropDownList;
            this.tagtype.FlatStyle = FlatStyle.System;
            this.tagtype.Location = new Point(1, 25);
            this.tagtype.Name = "tagtype";
            this.tagtype.Size = new Size(80, 21);
            this.tagtype.TabIndex = 2;
            this.tagtype.SelectedIndexChanged += this.tagtype_SelectedIndexChanged;

            // identSwapLayout
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.oldident);
            this.Controls.Add(this.tagname);
            this.Controls.Add(this.tagtype);
            this.Name = "identSwapLayout";
            this.Size = new Size(450, 50);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// The tagtype_ selected index changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tagtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            tagname.Items.Clear();
            tagname.Sorted = false;
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                if (map.MetaInfo.TagType[x] == tagtype.Items[tagtype.SelectedIndex].ToString())
                {
                    tagname.Items.Add(map.FileNames.Name[x]);
                }
            }

            tagname.Sorted = true;
            tagname.Sorted = false;
            tagname.Items.Add("Nulled Out");
            if (id != -1)
            {
                int temp = tagname.Items.IndexOf(map.FileNames.Name[id]);
                tagname.SelectedIndex = temp;
            }
            else
            {
                tagname.SelectedIndex = tagname.Items.Count - 1;
            }
        }

        #endregion
    }
}