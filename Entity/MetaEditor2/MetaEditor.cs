using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using entity.MapForms;
using HaloMap.Meta;

namespace entity.MetaEditor2
{
    public partial class WinMetaEditor : Form
    {
        //Meta meta = null;
        //Meta oldMeta = null;
        HaloMap.Map.Map map = null;
        MapForm mapForm;
        bool isDragging = false;

        private Color bgColor;
        private Color fgColor;
        public Color BackgroundColor { get { return bgColor; } set { bgColor = value; } }
        public Color ForegroundColor { get { return fgColor; } set { fgColor = value; } }

        public WinMetaEditor(MapForm sender, HaloMap.Map.Map map)
        {
            InitializeComponent();

            this.map = map;
            this.Owner = sender;
            mapForm = sender;
            this.Text = map.filePath.Substring(map.filePath.LastIndexOf('\\') + 1).ToUpper();
        }

        /// <summary>
        /// Opens a new tab and loads the meta into it
        /// </summary>
        /// <param name="meta">The meta to load into the new tab</param>
        /// <param name="allowDuplicate">if false, will look for a tab with the same meta and activate.
        /// if true, will open a new tab with a duplicated meta</param>
        /// <returns>Page Number of active tab</returns>
        public int addNewTab( Meta meta, bool allowDuplicate)
        {
            string typeAndTag = "[" + meta.type.ToLower() + "] " + map.SelectedMeta.name.Substring(meta.name.LastIndexOf('\\') + 1);

            if (!allowDuplicate)
                for (int i = 0; i < this.customTabControl1.TabPages.Count; i++)
                    if (this.customTabControl1.TabPages[i].Text == typeAndTag)
                    {
                        this.customTabControl1.SelectedIndex = i;
                        this.customTabControl1.SelectedTab.Focus();
                        return i;
                    }

            MetaEditorControlPage mecp = new MetaEditorControlPage(meta, mapForm);
            this.BackColor = bgColor;
            this.ForeColor = fgColor;
            mecp.setFormColors(fgColor, bgColor);

            // mapName = map.filePath.Substring(map.filePath.LastIndexOf('\\') + 1).ToUpper() +
            TabPage tp = new TabPage();
            tp.Controls.Add(mecp);
            mecp.Dock = DockStyle.Fill;
            tp.Text = typeAndTag;
            tp.ToolTipText = "[" + meta.type.ToLower() + "] " + meta.name;

            this.customTabControl1.AllowDrop = true;
            this.customTabControl1.DragDrop += new DragEventHandler(tp_DragDrop);
            this.customTabControl1.DragEnter += new DragEventHandler(tp_DragEnter);
            this.customTabControl1.DragLeave += new EventHandler(tp_DragLeave);
            // These tabs don't change selected index till MouseUp, so do it on MouseDown!
            this.customTabControl1.MouseDown += new MouseEventHandler(tabControl1_MouseDown);
            this.customTabControl1.MouseMove += new MouseEventHandler(tabControl1_MouseMove);

            this.customTabControl1.TabPages.Add(tp);
            this.customTabControl1.SelectedIndex = customTabControl1.TabPages.IndexOf(tp);

            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Focus();

            return customTabControl1.TabPages.IndexOf(tp);
        }

        // These tabs don't change selected index till MouseUp, so we do it on MouseDown instead for dragging purposes!
        void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                TabPage tpTemp = tabControl1.TabPages[i];
                Rectangle r = tabControl1.GetTabRect(i);
                int change = (i < tabControl1.SelectedIndex ? 1 : -1);
                if ((e.X >= r.Left + change * -10) && (e.X <= r.Left + r.Width + change * -10) &&
                    (e.Y >= r.Top) && (e.Y < r.Top + r.Height))
                {
                    {                        
                        tabControl1.SelectedTab = tpTemp;
                        this.Text = map.filePath.Substring(map.filePath.LastIndexOf('\\') + 1).ToUpper() + ": " +
                                tpTemp.ToolTipText;
                    }
                }
            }
        }

        void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (e.Button == MouseButtons.Left)
            {
                //this.tabControl1.DoDragDrop(this.tabControl1.SelectedTab, DragDropEffects.All);

                int mX = e.X;
                int mY = e.Y;
                TabPage tp = tabControl1.SelectedTab;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    TabPage tpTemp = tabControl1.TabPages[i];
                    if (tp == tpTemp)
                        continue;
                    Rectangle r = tabControl1.GetTabRect(i);
                    int change = (i < tabControl1.SelectedIndex ? 1 : -1);
                    if (( mX >= r.Left + change * -10) && (mX <= r.Left + r.Width + change * -10) &&
                        (mY >= r.Top) && (mY < r.Top + r.Height))
                    {
                        //
                        if (!isDragging)
                        {
                            tabControl1.SelectedIndex = i;
                            isDragging = true;
                            return;
                        }
                        //
                        tabControl1.TabPages.Remove(tpTemp);
                        tabControl1.TabPages.Insert(tabControl1.SelectedIndex + (change < 0 ? 0 : 1), tpTemp);
                        break;
                    }
                }
            }
            else
                isDragging = false;
            */

        }

        void tp_DragDrop(object sender, DragEventArgs e)
        {
            /*
            TabPage DropTab = (TabPage)(e.Data.GetData(typeof(TabPage)));
            int mX = e.X - this.Left;
            int mY = e.Y - this.Top - (this.FontHeight+17);
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                TabPage tp = tabControl1.TabPages[i];
                Rectangle r = tabControl1.GetTabRect(i);
                if ((mX >= r.Left) && (mX <= r.Left + r.Width + 30 ) &&
                    (mY >= r.Top) && (mY <= r.Top + r.Height))
                {
                    tabControl1.TabPages.RemoveAt(i);
                    tabControl1.TabPages.Insert(i, tp);
                    break;
                }
            }
            */
            //if (tabControl1.SelectedTab != DropTab)
            //    this.tabControl1.TabPages.Add(DropTab);
        }

        void tp_DragLeave(object sender, EventArgs e)
        {
            //
        }

        void tp_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TabPage)))
                e.Effect = DragDropEffects.Move;
            else 
                e.Effect = DragDropEffects.None;
        }


        /// <summary>
        /// This is used to update the reflexive label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WinMEControl_LostFocus(object sender, EventArgs e)
        {
            
        }

        private void WinMetaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close all open tabs??
            foreach (TabPage tp in this.customTabControl1.TabPages)
            {
                // Dispose of each Meta Editor Control Page (causes change save validation
                tp.Controls[0].Dispose();
            }
        }

        public bool checkSelectionInCurrentTag()
        {
//            return (meta.TagIndex == ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber);
            return true;
        }

        private void customTabControl1_TabClosing(object sender, TabControlCancelEventArgs e)
        {

            if (customTabControl1.TabPages.Count <= 1)
                this.Close();
        }

    }
}
