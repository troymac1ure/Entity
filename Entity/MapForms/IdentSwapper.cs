// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentSwapper.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for IdentSwapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MapForms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// Summary description for IdentSwapper.
    /// </summary>
    /// <remarks></remarks>
    public partial class IdentSwapper : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentSwapper"/> class.
        /// </summary>
        /// <remarks></remarks>
        public IdentSwapper()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // TODO: Add any constructor code after InitializeComponent call
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load stuff.
        /// </summary>
        /// <param name="idx">The idx.</param>
        /// <param name="offsetx">The offsetx.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void LoadStuff(List<int> idx, List<int> offsetx, Map map)
        {
            this.map = map;
            for (int count = 0; count < idx.Count; count++)
            {
                identSwapLayout idSwap = new identSwapLayout(map);
                IdentSwaps.Add(idSwap);

                idSwap.offset = offsetx[count];
                idSwap.id = idx[count];

                idSwap.tagtype.Sorted = false;

                for (int x = 0; x < map.MetaInfo.TagTypesCount; x++)
                {
                    idSwap.tagtype.Items.Add(map.MetaInfo.TagTypes[x]);
                }

                IEnumerator i = map.MetaInfo.TagTypes.Keys.GetEnumerator();
                while (i.MoveNext())
                {
                    idSwap.tagtype.Items.Add(i.Current);
                }

                idSwap.tagtype.Sorted = true;

                if (idSwap.id != -1)
                {
                    int temp = idSwap.tagtype.Items.IndexOf(map.MetaInfo.TagType[idSwap.id]);
                    idSwap.tagtype.SelectedIndex = temp;
                    idSwap.oldident.Text = map.MetaInfo.TagType[idSwap.id] + " - " + map.FileNames.Name[idSwap.id];
                }
                else
                {
                    idSwap.tagtype.SelectedIndex = 0;
                    idSwap.oldident.Text = "Nulled Out";
                }

                idSwap.Location = new Point(5, 5 + (IdentSwaps.Count - 1) * (idSwap.Height + 5));
                this.Controls.Add(idSwap);
                this.Height += idSwap.Height + 10;
            }
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
            this.button1 = new Button();
            this.SuspendLayout();

            // button1
            this.button1.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left));
            this.button1.BackColor = Color.LightSkyBlue;
            this.button1.DialogResult = DialogResult.OK;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Location = new Point(8, 6);
            this.button1.Name = "button1";
            this.button1.Size = new Size(448, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Swap Ident";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += this.button1_Click;

            // IdentSwapper
            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(462, 32);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Name = "IdentSwapper";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Ident Swapper";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            map.OpenMap(MapTypes.Internal);
            for (int i = 0; i < IdentSwaps.Count; i++)
            {
                map.BW.BaseStream.Position = IdentSwaps[i].offset;
                int temp = map.Functions.ForMeta.FindByNameAndTagType(
                    IdentSwaps[i].tagtype.Text, IdentSwaps[i].tagname.Text);
                if (temp != -1)
                {
                    map.BW.Write(map.MetaInfo.Ident[temp]);
                }
                else
                {
                    map.BW.Write(temp);
                }
            }

            map.CloseMap();
        }

        #endregion
    }
}