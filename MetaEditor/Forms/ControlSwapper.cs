// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlSwapper.cs" company="">
//   
// </copyright>
// <summary>
//   The control swapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetaEditor.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using global::MetaEditor.Components;

    using Globals;

    using HaloMap.Plugins;

    /// <summary>
    /// The control swapper.
    /// </summary>
    /// <remarks></remarks>
    public partial class ControlSwapper : Form
    {
        // private int offset = -1;
        #region Constants and Fields

        /// <summary>
        /// The chunkoffset.
        /// </summary>
        private readonly int chunkoffset = -1;

        /// <summary>
        /// The control size.
        /// </summary>
        private readonly int controlSize;

        /// <summary>
        /// The data types.
        /// </summary>
        private readonly List<dataTypeStruct> dataTypes = new List<dataTypeStruct>();

        /// <summary>
        /// The line num.
        /// </summary>
        private readonly int lineNum = -1;

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name = string.Empty;

        /// <summary>
        /// The s box.
        /// </summary>
        private readonly selectionBox sBox;

        /// <summary>
        /// The new control size.
        /// </summary>
        private int newControlSize;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlSwapper"/> class.
        /// </summary>
        /// <param name="orig">The orig.</param>
        /// <remarks></remarks>
        public ControlSwapper(Control orig)
        {
            InitializeComponent();

            labelOrigType.Dock = DockStyle.Top;
            labelOrigType.Text = orig.GetType().ToString();
            if (orig is DataValues)
            {
                labelOrigType.Text += "." + ((DataValues)orig).ValueType;
            }

            orig.Enabled = false;
            this.splitContainer1.Panel1.Controls.Add(orig);
            this.splitContainer1.Panel1.Controls[this.splitContainer1.Panel1.Controls.Count - 1].BringToFront();
            this.splitContainer1.Panel2.Controls.Add(sBox = new selectionBox());

            foreach (string o in Enum.GetNames(typeof(IFPIO.ObjectEnum)))
            {
                dataTypeStruct dts = new dataTypeStruct();
                dts.name = o;
                dts.size = getSizeOf(o);
                dataTypes.Add(dts);
            }

            getDataOf(orig, ref name, ref controlSize, ref chunkoffset, ref lineNum);
            if (controlSize == -1)
            {
                MessageBox.Show("Unknown control size. Make sure all changes are of equal size!");
                controlSize = Int32.MinValue;
            }

            this.label1.Text = "Original Control Size: " + controlSize;
            try
            {
                this.sBox.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                this.sBox.comboBox1.DataSource = dataTypes;
                this.sBox.comboBox1.DisplayMember = "Name";
                this.sBox.comboBox1.ValueMember = "Size";
            }
            catch (Exception e)
            {
                Global.ShowErrorMsg(e.Message, e);
            }

            // this.sBox.comboBox1.Items.AddRange(dataTypes.ToArray());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get data of.
        /// </summary>
        /// <param name="cntl">The cntl.</param>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="chunkoffset">The chunkoffset.</param>
        /// <param name="lineNum">The line num.</param>
        /// <remarks></remarks>
        private void getDataOf(Control cntl, ref string name, ref int size, ref int chunkoffset, ref int lineNum)
        {
            string ctrlName = name;
            chunkoffset = ((BaseField)cntl).chunkOffset;
            lineNum = ((BaseField)cntl).LineNumber;
            name = ((BaseField)cntl).EntName;

            switch (cntl.GetType().ToString())
            {
                case "Entity.MetaEditor.DataValues":
                    ctrlName = ((DataValues)cntl).ValueType.ToString();

                    // int iii = (int)((DataValues)cntl).ValueType;
                    break;
                case "Entity.MetaEditor.Indices":
                    ctrlName = ((Indices)cntl).ValueType.ToString();

                    // int ii = (int)((Indices)cntl).ValueType;
                    break;
                default:
                    ctrlName = cntl.GetType().ToString();
                    if (ctrlName.IndexOf('.') > 0)
                    {
                        ctrlName = ctrlName.Substring(ctrlName.LastIndexOf('.') + 1);
                    }

                    break;
            }

            size = getSizeOf(ctrlName);
        }

        /// <summary>
        /// The get size of.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The get size of.</returns>
        /// <remarks></remarks>
        private int getSizeOf(string name)
        {
            switch (name.ToLower())
            {
                case "byte":
                case "byte_flags":
                case "char_enum":
                    return 1;
                case "enum":
                case "int16":
                case "short":
                case "word_flags":
                    return 2;
                case "int":
                case "float":
                case "long_enum":
                case "long_flags":
                case "sid":
                    return 4;
                case "ident":
                case "tagblock":
                    return 8;
                case "string32":
                    return 32;
                case "unicode64":
                    return 64;
                case "string":
                case "string256":
                case "unicode256":
                    return 256;
            }

            return -1;
        }

        #endregion

        /// <summary>
        /// The data type struct.
        /// </summary>
        /// <remarks></remarks>
        public class dataTypeStruct
        {
            #region Properties

            /// <summary>
            /// Gets or sets name.
            /// </summary>
            /// <value>The name.</value>
            /// <remarks></remarks>
            public string name { get; set; }

            /// <summary>
            /// Gets or sets size.
            /// </summary>
            /// <value>The size.</value>
            /// <remarks></remarks>
            public int size { get; set; }

            #endregion
        }

        /// <summary>
        /// The selection box.
        /// </summary>
        /// <remarks></remarks>
        private class selectionBox : UserControl
        {
            #region Constants and Fields

            /// <summary>
            /// The button 1.
            /// </summary>
            public readonly Button button1 = new Button();

            /// <summary>
            /// The combo box 1.
            /// </summary>
            public readonly ComboBox comboBox1 = new ComboBox();

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="selectionBox"/> class.
            /// </summary>
            /// <remarks></remarks>
            public selectionBox()
            {
                // button1
                this.button1.Dock = DockStyle.Right;
                this.button1.Name = "button1";
                this.button1.Size = new Size(27, 22);
                this.button1.TabIndex = 0;
                this.button1.Text = "+";
                this.button1.UseVisualStyleBackColor = true;
                this.button1.Click += this.button1_Click;

                // comboBox1
                this.comboBox1.Dock = DockStyle.Right;
                this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                this.comboBox1.FormattingEnabled = true;
                this.comboBox1.Name = "comboBox1";
                this.comboBox1.Size = new Size(126, 21);
                this.comboBox1.TabIndex = 1;

                // selectionBox
                this.Dock = DockStyle.Top;
                this.Size = new Size(400, 24);
                this.Controls.Add(comboBox1);
                this.Controls.Add(button1);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The but_ click.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            private void but_Click(object sender, EventArgs e)
            {
                // Remove control object
                ControlSwapper cs = (ControlSwapper)((Control)sender).TopLevelControl;
                int conNum = cs.splitContainer1.Panel2.Controls.IndexOf(((Control)sender).Parent);
                string s = string.Empty;
                int j = 0;
                int size = 0;
                cs.getDataOf(((Control)sender).Parent, ref s, ref size, ref j, ref j);
                cs.newControlSize -= size;
                if (((ControlSwapper)this.TopLevelControl).label1.Text.Contains(" :: "))
                {
                    ((ControlSwapper)this.TopLevelControl).label1.Text =
                        ((ControlSwapper)this.TopLevelControl).label1.Text.Remove(
                            ((ControlSwapper)this.TopLevelControl).label1.Text.IndexOf(" :: "));
                }

                ((ControlSwapper)this.TopLevelControl).label1.Text += " :: New Control Size : " +
                                                                      ((ControlSwapper)this.TopLevelControl).
                                                                          newControlSize;
                cs.splitContainer1.Panel2.Controls.RemoveAt(conNum);

                // throw new NotImplementedException();
            }

            /// <summary>
            /// The button 1_ click.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            private void button1_Click(object sender, EventArgs e)
            {
                ControlSwapper parent = (ControlSwapper)this.Parent.Parent.Parent;

                // Add selected control
                IFPIO.Option[] options = null;
                //int strLength = 0;
                Control con = null;
                switch (((dataTypeStruct)comboBox1.SelectedItem).name.ToLower())
                {
                    /*
                    parent.splitContainer1.Panel2.Controls.Add(new DataValues(parent.name, null, parent.chunkoffset, IFPIO.ObjectEnum.Byte, parent.lineNum));
                    parent.splitContainer1.Panel2.Controls.Add(new DataValues(parent.name, null, parent.chunkoffset, IFPIO.ObjectEnum.Short, parent.lineNum));
                    parent.splitContainer1.Panel2.Controls.Add(new DataValues(parent.name, null, parent.chunkoffset, IFPIO.ObjectEnum.Int, parent.lineNum));
                    parent.splitContainer1.Panel2.Controls.Add(new DataValues(parent.name, null, parent.chunkoffset, IFPIO.ObjectEnum.Float, parent.lineNum));
                */
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "uint":
                    case "float":
                    case "unknown":
                    case "unused":
                        con = new DataValues(
                            parent.name,
                            null,
                            parent.chunkoffset,
                            (IFPIO.ObjectEnum)
                            Enum.Parse(typeof(IFPIO.ObjectEnum), ((dataTypeStruct)comboBox1.SelectedItem).name, true),
                            parent.lineNum);
                        break;
                    case "char_enum":
                    case "enum":
                    case "long_enum":
                        options = new IFPIO.Option[((dataTypeStruct)comboBox1.SelectedItem).size << 3];

                        // Size * 8 bits
                        for (int x = 0; x < options.Length; x++)
                        {
                            options[x] = new IFPIO.Option("Bit " + x, "", x.ToString(), parent.lineNum);
                        }

                        if (parent.splitContainer1.Panel1.Controls[0] is Bitmask)
                        {
                            Bitmask b = (Bitmask)parent.splitContainer1.Panel1.Controls[0];
                            foreach (IFPIO.Option o in b.Options)
                            {
                                if (o.value < options.Length)
                                {
                                    options[o.value].name = o.name;
                                }
                            }
                        }

                        ;
                        if (parent.splitContainer1.Panel1.Controls[0] is Enums)
                        {
                            Enums en = (Enums)parent.splitContainer1.Panel1.Controls[0];
                            foreach (IFPIO.Option o in en.Options)
                            {
                                if (o.value < options.Length)
                                {
                                    options[o.value].name = o.name;
                                }
                            }
                        }

                        ;
                        con = new Enums(parent.name, null, parent.chunkoffset, options.Length, options, parent.lineNum);
                        break;
                    case "byte_flags":
                    case "word_flags":
                    case "long_flags":
                        options = new IFPIO.Option[((dataTypeStruct)comboBox1.SelectedItem).size << 3];

                        // Size * 8 bits
                        for (int x = 0; x < options.Length; x++)
                        {
                            options[x] = new IFPIO.Option("Bit " + x, "", x.ToString(), parent.lineNum);
                        }

                        if (parent.splitContainer1.Panel1.Controls[0] is Bitmask)
                        {
                            Bitmask b = (Bitmask)parent.splitContainer1.Panel1.Controls[0];
                            foreach (IFPIO.Option o in b.Options)
                            {
                                options[o.value].name = o.name;
                            }
                        }

                        ;
                        if (parent.splitContainer1.Panel1.Controls[0] is Enums)
                        {
                            Enums en = (Enums)parent.splitContainer1.Panel1.Controls[0];
                            foreach (IFPIO.Option o in en.Options)
                            {
                                options[o.value].name = o.name;
                            }
                        }

                        ;
                        con = new Bitmask(
                            parent.name, null, parent.chunkoffset, options.Length, options, parent.lineNum);
                        break;
                    case "stringid":
                        con = new SID(parent.name, null, parent.chunkoffset, parent.lineNum);
                        break;
                    case "string":
                        con = new EntStrings(
                            parent.name,
                            null,
                            parent.chunkoffset,
                            ((dataTypeStruct)comboBox1.SelectedItem).size,
                            false,
                            parent.lineNum);
                        break;
                    case "unicodestring":
                        con = new EntStrings(
                            parent.name,
                            null,
                            parent.chunkoffset,
                            ((dataTypeStruct)comboBox1.SelectedItem).size,
                            true,
                            parent.lineNum);
                        break;
                    case "block":
                        con = new TagBlock(parent.name, null, parent.chunkoffset, parent.lineNum);
                        break;
                    case "ident":
                        con = new Ident(parent.name, null, parent.chunkoffset, true, parent.lineNum);
                        break;
                    case "struct":

                        // Unhandled
                        //int ifkdn = 0;
                        break;
                    default:
                        {
                            return;
                        }
                }

                Button but = new Button();
                but.Dock = DockStyle.Right;
                but.Size = new Size(30, 30);
                but.Text = "-";
                but.Click += but_Click;

                if (con != null)
                {
                    con.Controls.Add(but);

                    // con.Enabled = false;
                    con.Dock = DockStyle.Top;
                    Point loc = con.Controls[con.Controls.Count - 2].Location;
                    loc.X -= 50;
                    con.Controls[con.Controls.Count - 2].Location = loc;

                    // con.TabIndex--;
                    parent.splitContainer1.Panel2.Controls.Add(con);
                    con.BringToFront();
                    ((ControlSwapper)this.TopLevelControl).newControlSize +=
                        ((ControlSwapper)this.TopLevelControl).getSizeOf(((dataTypeStruct)comboBox1.SelectedItem).name);
                    if (((ControlSwapper)this.TopLevelControl).label1.Text.Contains(" :: "))
                    {
                        ((ControlSwapper)this.TopLevelControl).label1.Text =
                            ((ControlSwapper)this.TopLevelControl).label1.Text.Remove(
                                ((ControlSwapper)this.TopLevelControl).label1.Text.IndexOf(" :: "));
                    }

                    ((ControlSwapper)this.TopLevelControl).label1.Text += " :: New Control Size : " +
                                                                          ((ControlSwapper)this.TopLevelControl).
                                                                              newControlSize;
                }

                parent.splitContainer1.Panel2.Controls[parent.splitContainer1.Panel2.Controls.IndexOf(this)].
                    BringToFront();
                parent.splitContainer1.Panel2.Controls[parent.splitContainer1.Panel2.Controls.IndexOf(this)].TabIndex++;
            }

            #endregion
        }
    }
}