// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MEStringsSelector.cs" company="">
//   
// </copyright>
// <summary>
//   The me strings selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaFuncs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using HaloMap.Map;
    using System.IO;

    /// <summary>
    /// The me strings selector.
    /// </summary>
    public partial class MEStringsSelector : Form
    {
        #region private classes
        public class StringID
        {
            public string name { get; set; }
            public int id { get; set; }
            /// <summary>
            /// A List of Unicodes that point to this StringID
            /// </summary>
            public List<Unicode> unicodes = new List<Unicode>();

            public StringID(string Name, int ID)
            {
                this.name = Name;
                this.id = ID;
            }

            public override string ToString()
            {
                return this.name;
            }
        
        }

        class Encode
        {
            public byte[] codes { get; set; }
            public string definition { get; set; }

            public Encode(byte[] Codes, string Definition)
            {
                codes = Codes;
                definition = Definition;
            }
        }

        public class Unicode
        {
            public int offset { get; set; }
            public int position { get; set; }
            public string text { get; set; }
            public int textLength { get; set; }
            public string unicode { get; set; }
            /// <summary>
            /// The StringID that this Unicode is connected to
            /// </summary>
            public StringID stringID { get; set; }

            public Unicode(int Position, string Unicode, int TextLength, int Offset, StringID StringID)
            {
                this.position = Position;
                this.unicode = Unicode;
                this.textLength = TextLength;
                this.offset = Offset;
                this.stringID = StringID;

                byte[] tempbytes = System.Text.Encoding.UTF8.GetBytes(this.unicode);
                int change = tempbytes.Length;
                tempbytes = replaceCodes(tempbytes, true);
                change = tempbytes.Length - change;     // Account for string length changes
                this.text = System.Text.Encoding.ASCII.GetString(tempbytes).Substring(0, Math.Min(this.textLength + change, tempbytes.Length));
            }

            public override string ToString()
            {
                return ("[" + this.offset.ToString() + "] " + this.text);
            }

        }
        #endregion

        #region Constants and Fields

        /// <summary>
        /// The location of the Unicode decoding file
        /// </summary>
        static string unicodeFile;

        /// <summary>
        /// The _selected id.
        /// </summary>
        private int _selectedID;

        /// <summary>
        /// The _selected index.
        /// </summary>
        private int _selectedIndex;

        /// <summary>
        /// The list box update.
        /// </summary>
        private bool listBoxUpdate = true;

        List<StringID> stringIDs = new List<StringID>();            // List of all StringIDs
        List<StringID> stringIDsNoUnicode = new List<StringID>();   // List of all StringIDs without Unicodes
        List<StringID> stringIDsUnicode = new List<StringID>();     // List of all StringIDs with Unicodes
        List<StringID> searchStringIDs = new List<StringID>();      // List of searched StringIDs
        List<Unicode> unicodes = new List<Unicode>();               // List of all Unicodes
        List<Unicode> searchUnicodes = new List<Unicode>();         // List of searched Unicodes

        /// <summary>
        /// Holds a list of all loaded unicode codes
        /// </summary>
        static List<Encode> encodes { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MEStringsSelector"/> class.
        /// </summary>
        /// <param name="names">
        /// The names.
        /// </param>
        public MEStringsSelector(Map map, object sender)
        {
            InitializeComponent();

            if (encodes == null)
            {
                // load any listed unicode codes
                try
                {
                    encodes = loadUnicodes();
                }
                catch
                {
                    encodes = new List<Encode>();
                }
            }

            // Select ALL
            this.cbSelectStringIDs.SelectedIndex = 0;
            this.Owner = (Form)sender;

            string[] names = map.Strings.Name;
            // KeyPreview = true;
            for (int i = 0; i < names.Length; i++)
            {
                //dgvStringIDs.Rows.Add(new object[] { names[i], i, -1 });
                StringID SID = new StringID(names[i], i);
                // Create 2 duplicate lists
                stringIDs.Add(SID);
                stringIDsNoUnicode.Add(SID);
            }

            UnicodeTableReader.UnicodeTable ut = map.Unicode.ut[0];
            if (ut.SIDs == null)
                ut.Read();
            for (int i = 0; i < ut.US.Length; i++)
            {
                Unicode uni = new Unicode(i, ut.US[i].uString, ut.US[i].size, ut.US[i].offset, stringIDs[ut.SIDs[i].id]);
                unicodes.Add(uni);
                StringID SID = stringIDs[ut.SIDs[i].id];
                SID.unicodes.Add(uni);
                if (stringIDsNoUnicode.Contains(SID))
                {
                    stringIDsNoUnicode.Remove(SID); // Switch it from NO UNICODE list
                    stringIDsUnicode.Add(SID);      // ...to UNICODE list
                }
            }

            lbStringIDs.DataSource = stringIDs;
            lbUnicodes.DataSource = unicodes;

            // Set default sort method and apply sort to lists
            cbSIDSort.SelectedIndex = 0;
            cbUnicodeSort.SelectedIndex = 0;
            
            List<string> ss = new List<string>();
            for (int i = 0; i < unicodes[1].stringID.unicodes.Count; i++)
            {
                string s = unicodes[0].stringID.unicodes[i].ToString();
                byte[] tempbytes = System.Text.Encoding.Unicode.GetBytes(s);
                System.Text.Encoding decode = System.Text.Encoding.ASCII;
                ss.Add(decode.GetString(tempbytes));
            }

            setlbStringsIDDataSource();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   SelectedIndex
        /// </summary>
        public int SelectedID
        {
            get
            {
                return _selectedID;
            }

            set
            {
                setPosFromID(value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get id from string.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The get id from string.
        /// </returns>
        public string getIDFromString(string s)
        {
            for (int i = 0; i < lbStringIDs.Items.Count; i++)
            {
                if (lbStringIDs.Items[i].ToString() == s)
                {
                    return ((StringID)lbStringIDs.Items[i]).id.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// The get string from id.
        /// </summary>
        /// <param name="ID">
        /// The id.
        /// </param>
        /// <returns>
        /// The get string from id.
        /// </returns>
        public string getStringFromID(int ID)
        {
            for (int i = 0; i < lbStringIDs.Items.Count; i++)
            {
                if (((StringID)lbStringIDs.Items[i]).id == ID)
                {
                    return lbStringIDs.Items[i].ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// The set pos from id.
        /// </summary>
        /// <param name="ID">
        /// The id.
        /// </param>
        public void setPosFromID(int ID)
        {

            for (int i = 0; i < stringIDs.Count; i++)
            {
                if (stringIDs[i].id == ID)
                {
                    // We need to disable as it will randomly throw faults if the unicode is updated before the selectedItem is fully changed
                    lbStringIDs.Enabled = false;
                    lbStringIDs.SelectedItem = stringIDs[i];
                    lbStringIDs.Enabled = true;
                    updateUnicodeFromStringID();
                    _selectedID = stringIDs[i].id;
                    _selectedIndex = lbStringIDs.SelectedIndex;
                    return;
                }
            }
            /*
            for (int i = 0; i < dgvStringIDs.RowCount; i++)
            {
                if (dgvStringIDs[1, i].Value.ToString() == ID.ToString())
                {
                    dgvStringIDs.CurrentCell = dgvStringIDs[0, i];
                    _selectedID = int.Parse(dgvStringIDs[1, i].Value.ToString());
                    _selectedIndex = i;
                    return;
                }
            }
            */
        }

        public Unicode[] getUnicodesFromID(int ID)
        {
            List<Unicode> ul = new List<Unicode>();
            for (int i = 0; i < stringIDs.Count; i++)
            {
                if (stringIDs[i].id == ID)
                {
                    ul = stringIDs[i].unicodes;
                    break;
                }
            }
            return ul.ToArray();
        }

        /// <summary>
        /// Replaces special unicode entries to a ASCII readable version
        /// </summary>
        /// <param name="text">The unicode text to replace</param>
        /// <param name="allCodes">If true does all codes. If false does just the internal codes (such as "GamerTag")</param>
        /// <returns></returns>
        public static string ReplaceCodes(string text, bool allCodes)
        {
            byte[] tempbytes = System.Text.Encoding.UTF8.GetBytes(text);
            tempbytes = replaceCodes(tempbytes, allCodes);
            text = System.Text.Encoding.UTF8.GetString(tempbytes);
                
            return text;
        }

        #endregion

        #region Methods

        private static byte[] replaceCode(byte[] bytes, byte[] code, string decode)
        {
            List<byte> bs = new List<byte>(bytes);
            int change = 0;
            for (int i = 0; i < bytes.Length; i++)
                for (int ii = 0; ii < code.Length; ii++)
                {
                    if (bytes[i + ii] != code[ii])
                        break;
                    // Found
                    if (ii == code.Length - 1)
                    {
                        bs.RemoveRange(i - change, code.Length);
                        bs.InsertRange(i - change, System.Text.Encoding.ASCII.GetBytes(decode));
                        change += (code.Length - System.Text.Encoding.ASCII.GetBytes(decode).Length);
                    }
                }


            return (bs.ToArray());
        }

        private static byte[] replaceCodes(byte[] bytes, bool allCodes)
        {
            if (encodes == null)
            {
                try
                {
                    encodes = loadUnicodes();
                }
                catch
                {
                    encodes = new List<Encode>();
                }
            }

            byte[] temp = bytes;
            temp = replaceCode(temp, new byte[] { 13, 10 }, "\n");

            for (int i = 0; i < encodes.Count; i++)
            {
                if (!allCodes)  // The following are codes contained in the font files
                    if ((encodes[i].codes[0] == 238 && encodes[i].codes[1] == 128) ||
                        (encodes[i].codes[0] == 238 && encodes[i].codes[1] == 132) ||
                        (encodes[i].codes[0] == 238 && encodes[i].codes[1] == 145))
                        continue;
                temp = replaceCode(temp, encodes[i].codes, encodes[i].definition);
            }

            #region old code listing (now stored externally)
            /*
                temp = replaceCode(temp, new byte[] { 226, 128, 153 }, "'");

                temp = replaceCode(temp, new byte[] { 238, 128, 144 }, "??");    // ??
                temp = replaceCode(temp, new byte[] { 238, 128, 253 }, "(WHITE)");    // ??
                temp = replaceCode(temp, new byte[] { 238, 128, 254 }, "(BLACK)");    // ??

                temp = replaceCode(temp, new byte[] { 238, 132, 128 }, "(A)"); // ??
                temp = replaceCode(temp, new byte[] { 238, 132, 129 }, "(B)"); // ??
                temp = replaceCode(temp, new byte[] { 238, 132, 130 }, "(A)"); // ??
                temp = replaceCode(temp, new byte[] { 238, 132, 154 }, "[MP]");         // mp
                temp = replaceCode(temp, new byte[] { 238, 132, 155 }, "[NEEDLER]");    // nd
                temp = replaceCode(temp, new byte[] { 238, 132, 156 }, "[]");           //
                temp = replaceCode(temp, new byte[] { 238, 132, 157 }, "[PP]");         // pp
                temp = replaceCode(temp, new byte[] { 238, 132, 158 }, "[PR]");         // pr
                temp = replaceCode(temp, new byte[] { 238, 132, 159 }, "[]");
                temp = replaceCode(temp, new byte[] { 238, 132, 160 }, "[]");
                temp = replaceCode(temp, new byte[] { 238, 132, 161 }, "[]");
                temp = replaceCode(temp, new byte[] { 238, 132, 162 }, "[SMG]");        // smg
                temp = replaceCode(temp, new byte[] { 238, 132, 170 }, "[BPR]");        // bpr                

                temp = replaceCode(temp, new byte[] { 238, 144, 136 }, "<map name>");
                temp = replaceCode(temp, new byte[] { 238, 144, 138 }, "<game type>");
                temp = replaceCode(temp, new byte[] { 238, 144, 147 }, "<timer>");
                temp = replaceCode(temp, new byte[] { 238, 144, 148 }, "<other player>");
                temp = replaceCode(temp, new byte[] { 238, 144, 150 }, "<leader>");
                temp = replaceCode(temp, new byte[] { 238, 144, 164 }, "<gamertag>");
                temp = replaceCode(temp, new byte[] { 238, 144, 177 }, "<profile>");
                // In-game ??
                temp = replaceCode(temp, new byte[] { 238, 145, 135 }, "(A)");          // ?? 
                temp = replaceCode(temp, new byte[] { 238, 145, 136 }, "(X)");
                temp = replaceCode(temp, new byte[] { 238, 145, 137 }, "(Y)");
                temp = replaceCode(temp, new byte[] { 238, 145, 138 }, "(B)");
                temp = replaceCode(temp, new byte[] { 238, 145, 139 }, "(BACK)");      // ?? eg. hold to talk to entire team
                temp = replaceCode(temp, new byte[] { 238, 145, 141 }, "(RT)");        // ?? eg. hold to lock on rocket

                temp = replaceCode(temp, new byte[] { 238, 145, 157 }, "(percent)");        // ?? eg. hold to lock on rocket
                */
            #endregion

            return temp;
        }

        private static List<Encode> loadUnicodes()
        {
            List<Encode> encodes = new List<Encode>();
            unicodeFile = Application.StartupPath + "\\unicodes.txt";
            StreamReader sr = new StreamReader(unicodeFile);            
            TextReader tr = sr;
            while (!sr.EndOfStream)
            {
                string line = tr.ReadLine();
                // ignore comments and blank lines
                if (!line.Trim().StartsWith(";") && line.Trim().Length > 0)
                {
                    string[] ss = line.Split(' ');
                    if (line.Contains('"'))
                    {
                        int strStart = line.IndexOf('"');
                        ss[3] = line.Substring(
                            strStart, 
                            (int)Math.Min( 
                                (uint)line.IndexOf('"', strStart + 1) - strStart + 1,
                                (uint)line.Length - strStart
                            ));
                    }
                    try
                    {
                        Encode enc = new Encode(
                            new byte[] { 
                                byte.Parse(ss[0].Replace(",", "").Trim()),
                                byte.Parse(ss[1].Replace(",", "").Trim()),
                                byte.Parse(ss[2].Replace(",", "").Trim())
                            }, ss[3]);
                        encodes.Add(enc);
                    }
                    catch
                    {
                    }
                }
            }
            sr.Close();

            return encodes;
        }

        private void sortStringIDLists()
        {
            object o = lbStringIDs.SelectedItem;
            List<StringID> list = (List<StringID>)lbStringIDs.DataSource;
            switch (cbSIDSort.SelectedIndex)
            {
                case 0:
                    list = stringIDs.OrderBy(StringID => StringID.name).ToList();
                    break;
                case 1:
                    list = stringIDs.OrderBy(StringID => StringID.id).ToList();
                    break;
            }
            lbStringIDs.DataSource = list;
            ((CurrencyManager)lbStringIDs.BindingContext[lbStringIDs.DataSource]).Refresh();
            try
            {
                lbStringIDs.SelectedItem = o;
            }
            catch
            {
            }
        }

        private void sortUnicodeLists()
        {
            object o = lbUnicodes.SelectedItem;
            List<Unicode> list = (List<Unicode>)lbUnicodes.DataSource;
            switch (cbUnicodeSort.SelectedIndex)
            {
                case 0:
                    list = list.OrderBy(Unicode => Unicode.text).ToList();
                    break;
                case 1:
                    list = list.OrderBy(Unicode => Unicode.offset).ToList();
                    break;
            }
            lbUnicodes.DataSource = list;
            ((CurrencyManager)lbUnicodes.BindingContext[lbUnicodes.DataSource]).Refresh();
            try
            {
                lbUnicodes.SelectedItem = o;
            }
            catch
            {
            }
        }

        /// <summary>
        /// The me strings selector_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MEStringsSelector_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            lbStringIDs.SelectedIndex = _selectedIndex;
            lbStringIDs.TopIndex = _selectedIndex;

            textBox1.Text = string.Empty;
            textBox1.Focus();
            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            listBox1_DoubleClick(this, null);
        }

        private void cbSIDSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            sortStringIDLists();
        }

        private void cbUnicodeSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            sortUnicodeLists();
        }

        /// <summary>
        /// The check box 1_ check state changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            listBoxUpdate = true;
        }

        /// <summary>
        /// The check string contains.
        /// </summary>
        /// <param name="newSel">
        /// The new sel.
        /// </param>
        private void checkStringContains(string newSel)
        {
            lbStringIDs.SuspendLayout();
            if (textBox1.Text == string.Empty)
            {
                setlbStringsIDDataSource();
                for (int i = 0; i < stringIDs.Count; i++)
                {
                    if (stringIDs[i].ToString() == newSel)
                    {
                        lbStringIDs.SelectedItem = stringIDs[i];
                    }
                }
            }
            else
            {
                searchStringIDs.Clear();
                int selection = -1;
                lbStringIDs.DataSource = null;
                List<StringID> SIDs = getlbStringsIDDataSource();                
                for (int i = 0; i < SIDs.Count; i++)
                {
                    string s = SIDs[i].ToString();
                    if (s.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        searchStringIDs.Add(SIDs[i]);
                        if (s == newSel)
                        {
                            selection = i;
                        }
                    }
                }
                //searchStringIDs = searchStringIDs.OrderBy(StringID => StringID.id).ToList();
                lbStringIDs.DataSource = searchStringIDs;

                
                if (selection != -1)
                    lbStringIDs.SelectedItem = SIDs[selection];
            }

            lbStringIDs.ResumeLayout();
        }

        /// <summary>
        /// The check string start.
        /// </summary>
        /// <param name="newSel">
        /// The new sel.
        /// </param>
        private void checkStringStart(string newSel)
        {
            lbStringIDs.SuspendLayout();
            if (textBox1.Text == string.Empty)
            {
                setlbStringsIDDataSource();
                List<StringID> SIDs = getlbStringsIDDataSource();
                for (int i = 0; i < SIDs.Count; i++)
                {
                    if (SIDs[i].ToString() == newSel)
                    {
                        lbStringIDs.SelectedItem = SIDs[i];
                    }
                }
            }
            else
            {
                searchStringIDs.Clear();
                int selection = -1;
                lbStringIDs.DataSource = null;
                List<StringID> SIDs = getlbStringsIDDataSource();
                for (int i = 0; i < SIDs.Count; i++)
                {
                    string s = SIDs[i].ToString();
                    if (s.StartsWith(textBox1.Text))
                    {
                        searchStringIDs.Add(SIDs[i]);
                        if (s == newSel)
                        {
                            selection = i;
                        }
                    }
                }
                lbStringIDs.DataSource = searchStringIDs;
                if (selection != -1)
                    lbStringIDs.SelectedItem = SIDs[selection];
            }

            lbStringIDs.ResumeLayout();
        }

        private void lbStringIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lbStringIDs.Enabled)
                return;
            updateUnicodeFromStringID();
            if (lbUnicodes.SelectedIndex != -1)
                lblUnicodePosition.Text = "Unicode #" + ((Unicode)lbUnicodes.SelectedItem).position + " / " + unicodes.Count;
            else
                lblUnicodePosition.Text = string.Empty;
            try
            {
                lblStringIDNumber.Text = "String ID #" + ((StringID)lbStringIDs.SelectedItem).id.ToString() + " (Total: " + lbStringIDs.Items.Count + ")";
            }
            catch
            {}

        }

        private void lbUnicodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lbUnicodes.Enabled)
                return;
            updateStringIDFromUnicode();
            if (lbUnicodes.SelectedIndex == -1 && lbUnicodes.Items.Count > 0)
            {
                lbUnicodes.SelectedIndex = 0;
            }
            if (lbUnicodes.SelectedItem != null)
                lblUnicodePosition.Text = "Unicode #" + ((Unicode)lbUnicodes.SelectedItem).position + " / " + unicodes.Count;
            
        }

        /// <summary>
        /// The list box 1_ double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string s = lbStringIDs.SelectedItem.ToString();
            _selectedID = ((StringID)lbStringIDs.SelectedItem).id;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// The list box 1_ key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox1.Focus();
        }

        private void cbSelectStringIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            setlbStringsIDDataSource();
        }

        private void cbShowAllUnicodes_CheckedChanged(object sender, EventArgs e)
        {
            object o = lbUnicodes.SelectedItem;
            if (cbShowAllUnicodes.Checked)
                lbUnicodes.DataSource = unicodes;
            else
            {
                StringID SID = (StringID)lbStringIDs.SelectedItem;
                lbUnicodes.DataSource = SID.unicodes;
            }
            try
            {
                lbUnicodes.SelectedItem = o;
            }
            catch
            {
            }
            sortUnicodeLists();
        }

        private List<StringID> getlbStringsIDDataSource()
        {
            switch (cbSelectStringIDs.SelectedIndex)
            {
                case 0:
                    return(stringIDs);
                case 1:
                    return(stringIDsUnicode);
                case 2:
                    return(stringIDsNoUnicode);
            }
            return new List<StringID>();
        }

        private void setlbStringsIDDataSource()
        {
            object o = lbStringIDs.SelectedItem;
            switch (cbSelectStringIDs.SelectedIndex)
            {
                case 0:
                    lbStringIDs.DataSource = stringIDs;
                    break;
                case 1:
                    lbStringIDs.DataSource = stringIDsUnicode;
                    break;
                case 2:
                    lbStringIDs.DataSource = stringIDsNoUnicode;
                    break;
            }
            try
            {
                if (o != null)
                    lbStringIDs.SelectedItem = o;
                // Make sure selection # & count is up to date
                lblStringIDNumber.Text = "String ID #" + ((StringID)lbStringIDs.SelectedItem).id.ToString() + " (Total: " + lbStringIDs.Items.Count + ")";
            }
            catch
            { }
            listBoxUpdate = true;
        }

        /// <summary>
        /// The text box 1_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int listHeight = lbStringIDs.Height / lbStringIDs.ItemHeight;
            switch (e.KeyCode)
            {
                /*
                case Keys.Home:
                    if (lbStringIDs.Items.Count > 0)
                    {
                        lbStringIDs.SelectedIndex = 0;
                    }
                    break;

                case Keys.End:

                    // if (listBox1.Items.Count > 0)
                    lbStringIDs.SelectedIndex = lbStringIDs.Items.Count - 1;
                    break;
                */
                case Keys.PageUp:
                    if (lbStringIDs.Items.Count > 0)
                    {
                        if (lbStringIDs.SelectedIndex > listHeight)
                        {
                            lbStringIDs.SelectedIndex -= listHeight;
                        }
                        else
                        {
                            lbStringIDs.SelectedIndex = 0;
                        }
                    }

                    break;
                case Keys.Up:
                    if (lbStringIDs.SelectedIndex > 0)
                    {
                        lbStringIDs.SelectedIndex -= 1;
                    }

                    break;
                case Keys.Down:

                    // if (listBox1.Items.Count > 0)
                    if (lbStringIDs.SelectedIndex < lbStringIDs.Items.Count - 1)
                    {
                        lbStringIDs.SelectedIndex += 1;
                    }

                    break;
                case Keys.PageDown:

                    // if (listBox1.Items.Count > 0)
                    if (lbStringIDs.SelectedIndex + listHeight < lbStringIDs.Items.Count - 1)
                    {
                        lbStringIDs.SelectedIndex += listHeight;
                    }
                    else
                    {
                        lbStringIDs.SelectedIndex = lbStringIDs.Items.Count - 1;
                    }

                    break;
                case Keys.Enter:
                    listBox1_DoubleClick(sender, new EventArgs());
                    break;
            }
        }

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
            tmr_MEStrings.Stop();
            listBoxUpdate = true;
            tmr_MEStrings.Interval = 700;
            tmr_MEStrings.Enabled = true;
            tmr_MEStrings.Start();
        }

        /// <summary>
        /// The timer 1_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void tmr_MEStrings_Tick(object sender, EventArgs e)
        {
            if (lbStringIDs.Focused)
            {
                textBox1.Focus();
            }

            if (!listBoxUpdate)
            {
                return;
            }

            tmr_MEStrings.Stop();
            listBoxUpdate = false;
            string oldSel = null;
            if (lbStringIDs.SelectedIndex != -1)
            {
                oldSel = lbStringIDs.Items[lbStringIDs.SelectedIndex].ToString();
            }

            if (checkBox1.CheckState != CheckState.Checked)
            {
                checkStringContains(oldSel);
            }
            else
            {
                checkStringStart(oldSel);
            }

            if (lbStringIDs.SelectedIndex == -1 && lbStringIDs.Items.Count > 0)
            {
                lbStringIDs.SelectedIndex = 0;
            }

            tmr_MEStrings.Enabled = true;
        }

        private void updateUnicodeFromStringID()
        {
            StringID SID = (StringID)lbStringIDs.SelectedItem;
            if (SID == null)
                return;
            lbUnicodes.Enabled = false;
            if (cbShowAllUnicodes.Checked)
            {
                lbUnicodes.DataSource = unicodes;
                if (SID.unicodes.Count > 0)
                    lbUnicodes.SelectedItem = SID.unicodes[0];
            }
            else
            {
                lbUnicodes.DataSource = SID.unicodes;
                // Sometimes it doesn't properly select a default item and throws errors, so refresh to fix
                ((CurrencyManager)lbUnicodes.BindingContext[lbUnicodes.DataSource]).Refresh();

#if DEBUG
                /*
                 * This is for debugging of unknown unicode codes only
                 */
                if (SID.unicodes.Count > 0 && SID.unicodes[0].ToString().Contains("???"))
                {
                    string s = SID.unicodes[0].unicode.ToString();
                    byte[] tempbytes = System.Text.Encoding.Unicode.GetBytes(s);
                    string temps = "Unknown Codes:\n";
                    for(int i = 0; i < tempbytes.Length; i++)
                        if (tempbytes[i] == 238)
                        {
                            temps += tempbytes[i].ToString() + " " + tempbytes[i + 1].ToString() + " " + tempbytes[i + 2].ToString() + "\n";
                        }
                    MessageBox.Show(temps);
                }
#endif
            }
            lbUnicodes.Enabled = true;
        }

        private void updateStringIDFromUnicode()
        {
            Unicode uni = (Unicode)lbUnicodes.SelectedItem;
            if (uni == null)
                return;
            lbStringIDs.Enabled = false;
            if (uni.stringID != null)
                lbStringIDs.SelectedItem = uni.stringID;
            lbStringIDs.Enabled = true;
        }

        #endregion

    }
}