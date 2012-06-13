// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaEditor.cs" company="">
//   
// </copyright>
// <summary>
//   The meta editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaFuncs
{
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms;

    using HaloMap.Map;
    using HaloMap.Plugins;

    /// <summary>
    /// The meta editor.
    /// </summary>
    /// <remarks></remarks>
    public class MetaEditor
    {
        #region Constants and Fields

        /// <summary>
        /// The map.
        /// </summary>
        public static Map map;

        /// <summary>
        /// The controls.
        /// </summary>
        private ReflexiveContainer Controls;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaEditor"/> class.
        /// </summary>
        /// <param name="ifp">The ifp.</param>
        /// <param name="split">The split.</param>
        /// <param name="mapx">The mapx.</param>
        /// <remarks></remarks>
        public MetaEditor(IFPIO ifp, SplitContainer split, Map mapx)
        {
            map = mapx;

            ReflexiveContainer tempr = new ReflexiveContainer("Header");
            MakeControls(0, ifp.items, ref tempr);
            Controls = tempr;
            DrawControls(ref split);
        }

        #endregion

        #region Enums

        /// <summary>
        /// The container type.
        /// </summary>
        /// <remarks></remarks>
        public enum ContainerType
        {
            /// <summary>
            /// The reflexive.
            /// </summary>
            Reflexive, 

            /// <summary>
            /// The ident.
            /// </summary>
            Ident, 

            /// <summary>
            /// The string id.
            /// </summary>
            StringID, 

            /// <summary>
            /// The int 32.
            /// </summary>
            Int32, 

            /// <summary>
            /// The int 16.
            /// </summary>
            Int16, 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The generate ident type combobox.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <remarks></remarks>
        public static void GenerateIdentTypeCombobox(ComboBox c)
        {
            // c = new ComboBox();
            IEnumerator i = map.MetaInfo.TagTypes.Keys.GetEnumerator();

            while (i.MoveNext())
            {
                c.Items.Add(i.Current);
            }

            c.Sorted = true;

            // return c;
        }

        /// <summary>
        /// The draw controls.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <remarks></remarks>
        public void DrawControls(ref SplitContainer s)
        {
            s.Panel2Collapsed = true;
            foreach (Control c in s.Panel2.Controls)
            {
                c.Dispose();
            }

            s.Panel2.Controls.Clear();
            DrawControlsRecursive(ref s, ref Controls);
            s.Panel2Collapsed = false;
        }

        /// <summary>
        /// The make controls.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="ifpobjects">The ifpobjects.</param>
        /// <param name="reflex">The reflex.</param>
        /// <remarks></remarks>
        public void MakeControls(int y, object[] ifpobjects, ref ReflexiveContainer reflex)
        {
            foreach (object o in ifpobjects)
            {
                IFPIO.BaseObject tempbase = (IFPIO.BaseObject)o;

                switch (tempbase.ObjectType)
                {
                    case IFPIO.ObjectEnum.Struct:
                        IFPIO.Reflexive r = (IFPIO.Reflexive)tempbase;

                        // if (r.visible == false) { break; }
                        ReflexiveContainer tempr = new ReflexiveContainer(r);
                        MakeControls(0, r.items, ref tempr);
                        reflex.Controls.Add(tempr);

                        break;
                    case IFPIO.ObjectEnum.Ident:
                        IFPIO.Ident id = (IFPIO.Ident)tempbase;

                        // if (id.visible == false) { break; }
                        IdentContainer tempid = new IdentContainer(id);
                        reflex.Controls.Add(tempid);
                        break;
                    case IFPIO.ObjectEnum.Int:
                        IFPIO.IFPInt int32 = (IFPIO.IFPInt)tempbase;

                        // if (int32.visible == false) { break; }
                        Int32Container tempint = new Int32Container(int32);
                        reflex.Controls.Add(tempint);
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The draw controls recursive.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="reflex">The reflex.</param>
        /// <remarks></remarks>
        private void DrawControlsRecursive(ref SplitContainer s, ref ReflexiveContainer reflex)
        {
            foreach (object o in reflex.Controls)
            {
                PluginControl tempbase = (PluginControl)o;
                bool nospacer = true;
                switch (tempbase.type)
                {
                    case ContainerType.Reflexive:
                        ReflexiveContainer r = (ReflexiveContainer)tempbase;

                        DrawControlsRecursive(ref r.Container, ref r);
                        reflex.height += 25;

                        s.Panel2.Controls.Add(r.Container);
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].BringToFront();
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].Update();
                        break;
                    case ContainerType.Ident:
                        IdentContainer id = (IdentContainer)tempbase;
                        s.Panel2.Controls.Add(id.panel);
                        reflex.height += id.panel.Height;
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].BringToFront();
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].Update();
                        break;
                    case ContainerType.Int32:
                        Int32Container int32 = (Int32Container)tempbase;
                        s.Panel2.Controls.Add(int32.panel);
                        reflex.height += int32.panel.Height;
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].BringToFront();
                        s.Panel2.Controls[s.Panel2.Controls.Count - 1].Update();
                        break;
                    default:
                        nospacer = true;
                        break;
                }

                if (nospacer)
                {
                    continue;
                }

                Panel Spacerx = new Panel();
                Spacerx.Height = 10;
                Spacerx.BackColor = Color.Transparent;
                Spacerx.Dock = DockStyle.Top;
                s.Panel2.Controls.Add(Spacerx);
                s.Panel2.Controls[s.Panel2.Controls.Count - 1].BringToFront();
                s.Panel2.Controls[s.Panel2.Controls.Count - 1].Update();
                reflex.height += 10;
            }
        }

        #endregion

        /// <summary>
        /// The ident container.
        /// </summary>
        /// <remarks></remarks>
        public class IdentContainer : PluginControl
        {
            #region Constants and Fields

            /// <summary>
            /// The description label.
            /// </summary>
            public Label DescriptionLabel;

            /// <summary>
            /// The name combo.
            /// </summary>
            public ComboBox NameCombo;

            /// <summary>
            /// The tag type combo.
            /// </summary>
            public ComboBox TagTypeCombo;

            /// <summary>
            /// The panel.
            /// </summary>
            public Panel panel;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IdentContainer"/> class.
            /// </summary>
            /// <param name="ident">The ident.</param>
            /// <remarks></remarks>
            public IdentContainer(IFPIO.Ident ident)
            {
                type = ContainerType.Ident;
                Label templabel = new Label();
                templabel.BorderStyle = BorderStyle.FixedSingle;
                templabel.BackColor = Color.Silver;
                templabel.Text = "Ident (" + ident.name + ") • Offset: " + ident.offset.ToString("X");
                templabel.TextAlign = ContentAlignment.MiddleCenter;
                templabel.Dock = DockStyle.Top;
                templabel.Height = 20;
                templabel.Width = 125;
                templabel.Top = 0;

                DescriptionLabel = templabel;
                TagTypeCombo = new ComboBox();
                TagTypeCombo.Width = 75;
                TagTypeCombo.Dock = DockStyle.Right;
                TagTypeCombo.Top = 20;
                TagTypeCombo.Left = 0;

                // TagTypeCombo=
                GenerateIdentTypeCombobox(TagTypeCombo);

                NameCombo = new ComboBox();
                NameCombo.Width = 250;
                NameCombo.Dock = DockStyle.Bottom;
                NameCombo.Top = 20;
                NameCombo.Left = 200;

                // NameCombo.Dock = DockStyle.Top;
                panel = new Panel();
                panel.Dock = DockStyle.Top;
                panel.Height = 40;

                // panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Controls.Add(DescriptionLabel);
                panel.Controls.Add(TagTypeCombo);
                panel.Controls.Add(NameCombo);
            }

            #endregion
        }

        /// <summary>
        /// The int 32 container.
        /// </summary>
        /// <remarks></remarks>
        public class Int32Container : PluginControl
        {
            #region Constants and Fields

            /// <summary>
            /// The description label.
            /// </summary>
            public Label DescriptionLabel;

            /// <summary>
            /// The int text box.
            /// </summary>
            public TextBox IntTextBox;

            /// <summary>
            /// The panel.
            /// </summary>
            public Panel panel;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Int32Container"/> class.
            /// </summary>
            /// <param name="intx">The intx.</param>
            /// <remarks></remarks>
            public Int32Container(IFPIO.IFPInt intx)
            {
                type = ContainerType.Int32;
                Label templabel = new Label();
                templabel.BorderStyle = BorderStyle.FixedSingle;
                templabel.BackColor = Color.Silver;
                templabel.Text = "Int32 (" + intx.name + ") • Offset: " + intx.offset.ToString("X");
                templabel.TextAlign = ContentAlignment.MiddleCenter;
                templabel.Dock = DockStyle.Left;
                templabel.Height = 20;
                templabel.AutoSize = true;
                DescriptionLabel = templabel;

                IntTextBox = new TextBox();
                IntTextBox.Dock = DockStyle.Fill;
                IntTextBox.Height = 20;

                // IntTextBox.Text=

                // NameCombo.Dock = DockStyle.Top;
                panel = new Panel();
                panel.Dock = DockStyle.Top;
                panel.Height = 40;

                // panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Controls.Add(DescriptionLabel);
                panel.Controls.Add(IntTextBox);
            }

            #endregion
        }

        /// <summary>
        /// The plugin control.
        /// </summary>
        /// <remarks></remarks>
        public class PluginControl
        {
            #region Constants and Fields

            /// <summary>
            /// The height.
            /// </summary>
            public int height;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The type.
            /// </summary>
            public ContainerType type;

            /// <summary>
            /// The width.
            /// </summary>
            public int width;

            #endregion
        }

        /// <summary>
        /// The reflexive container.
        /// </summary>
        /// <remarks></remarks>
        public class ReflexiveContainer : PluginControl
        {
            #region Constants and Fields

            /// <summary>
            /// The container.
            /// </summary>
            public SplitContainer Container;

            /// <summary>
            /// The controls.
            /// </summary>
            public ArrayList Controls = new ArrayList(0);

            /// <summary>
            /// The selected chunk combo box.
            /// </summary>
            public ComboBox SelectedChunkComboBox;

            /// <summary>
            /// The max size.
            /// </summary>
            protected Size maxSize;

            /// <summary>
            /// The min size.
            /// </summary>
            protected Size minSize;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflexiveContainer"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <remarks></remarks>
            public ReflexiveContainer(string name)
            {
                base.height = 25;
                type = ContainerType.Reflexive;
                Label templabel = new Label();
                templabel.BorderStyle = BorderStyle.FixedSingle;
                templabel.BackColor = Color.WhiteSmoke;
                templabel.Text = name;
                templabel.TextAlign = ContentAlignment.MiddleCenter;
                templabel.Dock = DockStyle.Left;

                templabel.MouseDown += ReflexiveHeader_MouseDown;

                SelectedChunkComboBox = new ComboBox();
                SelectedChunkComboBox.Dock = DockStyle.Right;
                SelectedChunkComboBox.Width = 50;
                SelectedChunkComboBox.FlatStyle = FlatStyle.Popup;

                // container
                SplitContainer tempsplit = new SplitContainer();
                tempsplit.Orientation = Orientation.Horizontal;
                tempsplit.Panel1MinSize = 0;
                tempsplit.SplitterDistance = 25;
                tempsplit.IsSplitterFixed = true;
                tempsplit.SplitterWidth = 1;
                tempsplit.Dock = DockStyle.Fill;

                // panel 1
                tempsplit.Panel1.Controls.Add(SelectedChunkComboBox);
                tempsplit.Panel1.Controls.Add(templabel);

                // panel 2
                tempsplit.Panel2.AutoScroll = true;
                tempsplit.Panel2.BackColor = Color.Honeydew;

                // Padding prop = new Padding();
                // prop.All = 10;
                // tempsplit.Panel2.Margin = prop;
                tempsplit.Panel2Collapsed = true;

                Container = tempsplit;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflexiveContainer"/> class.
            /// </summary>
            /// <param name="reflexive">The reflexive.</param>
            /// <remarks></remarks>
            public ReflexiveContainer(IFPIO.Reflexive reflexive)
            {
                type = ContainerType.Reflexive;

                // Controls.
                Label templabel = new Label();
                templabel.BorderStyle = BorderStyle.FixedSingle;
                templabel.BackColor = Color.Silver;
                templabel.Text = "Reflexive (" + reflexive.name + ") • Offset: " + reflexive.offset.ToString("X");
                templabel.TextAlign = ContentAlignment.MiddleCenter;
                templabel.Dock = DockStyle.Fill;
                templabel.MouseDown += ReflexiveHeader_MouseDown;

                SelectedChunkComboBox = new ComboBox();
                SelectedChunkComboBox.Dock = DockStyle.Right;
                SelectedChunkComboBox.Width = 50;
                SelectedChunkComboBox.Height = 25;
                SelectedChunkComboBox.FlatStyle = FlatStyle.Popup;

                // container
                SplitContainer tempsplit = new SplitContainer();
                tempsplit.Orientation = Orientation.Horizontal;
                tempsplit.Panel1MinSize = 0;
                tempsplit.SplitterDistance = 25;
                tempsplit.IsSplitterFixed = true;
                tempsplit.SplitterWidth = 1;
                tempsplit.Dock = DockStyle.Top;
                SetMaxSize(0, 25);
                tempsplit.MaximumSize = this.MaxSize;

                // panel 1
                tempsplit.Panel1.Controls.Add(templabel);
                tempsplit.Panel1.Controls.Add(SelectedChunkComboBox);

                // panel 2
                tempsplit.Panel2.AutoScroll = true;
                tempsplit.Panel2.BackColor = Color.Honeydew;
                Padding prop = new Padding();
                prop.All = 10;

                // prop.Right = 0;
                tempsplit.Panel2.Padding = prop;
                tempsplit.Panel2Collapsed = true;
                tempsplit.BorderStyle = BorderStyle.None;

                // tempsplit.Visible = true;
                this.Container = tempsplit;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets MaxSize.
            /// </summary>
            /// <value>The size of the max.</value>
            /// <remarks></remarks>
            public Size MaxSize
            {
                get
                {
                    return maxSize;
                }

                set
                {
                    maxSize = value;
                }
            }

            /// <summary>
            /// Gets or sets MinSize.
            /// </summary>
            /// <value>The size of the min.</value>
            /// <remarks></remarks>
            public Size MinSize
            {
                get
                {
                    return minSize;
                }

                set
                {
                    minSize = value;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The set max size.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <remarks></remarks>
            public void SetMaxSize(int width, int height)
            {
                maxSize = new Size(width, height);
            }

            /// <summary>
            /// The set min size.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <remarks></remarks>
            public void SetMinSize(int width, int height)
            {
                minSize = new Size(width, height);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The reflexive header_ mouse down.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            private void ReflexiveHeader_MouseDown(object sender, MouseEventArgs e)
            {
                Label x = (Label)sender;
                SplitterPanel s = (SplitterPanel)x.Parent;
                SplitContainer sc = (SplitContainer)s.Parent;
                if (sc.Panel2Collapsed == false)
                {
                    SetMaxSize(0, 25);
                    SetMinSize(0, 25);
                    sc.MaximumSize = this.MaxSize;
                    sc.MinimumSize = this.MinSize;
                    sc.Height = 25;
                    sc.BorderStyle = BorderStyle.None;

                    sc.Panel2Collapsed = true;
                    sc.SplitterDistance = 25;
                    sc.Update();

                    x.BackColor = Color.Silver;
                }
                else
                {
                    SetMaxSize(0, height + 50);
                    SetMinSize(0, 25);

                    sc.MaximumSize = this.MaxSize;
                    sc.MinimumSize = this.MinSize;
                    sc.Height = height + 50;
                    sc.BorderStyle = BorderStyle.FixedSingle;
                    sc.Panel2Collapsed = false;
                    sc.SplitterDistance = 25;
                    sc.Update();
                    x.BackColor = Color.CornflowerBlue;
                }
            }

            #endregion
        }
    }
}