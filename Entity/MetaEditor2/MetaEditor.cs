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
using DevComponents.DotNetBar;

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

        /// <summary>
        /// Keeps track of externally references reflexives
        /// </summary>
        public List<references> reflexiveReferences = new List<references>();

        public class references
        {
            public int chunkCount;
            public int ident;
            public string name;
            public int offset;
            public int size;
            public int tagIndex;
            public string tagType;
            public string tagName;

            public override string ToString()
            {
                return "\"" + name + "\", count=" + chunkCount + "  =>  [" + tagType + "] " + tagName;
            }
        }

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
            string typeAndTag = "[" + meta.type.ToLower() + "] " + meta.name.Substring(meta.name.LastIndexOf('\\') + 1);

            if (!allowDuplicate)
                for (int i = 0; i < this.tabs.Tabs.Count; i++)
                    if (this.tabs.Tabs[i].Text == typeAndTag)
                    {
                        this.tabs.SelectedTabIndex = i;
                        //this.tabs.SelectedTab.Focus();
                        return i;
                    }

            MetaEditorControlPage mecp = new MetaEditorControlPage(meta, mapForm);            
            this.BackColor = bgColor;
            this.ForeColor = fgColor;
            mecp.setFormColors(fgColor, bgColor);
            
            // mapName = map.filePath.Substring(map.filePath.LastIndexOf('\\') + 1).ToUpper() +
            TabItem tp = tabs.CreateTab(typeAndTag);
            mecp.Parent = tp.AttachedControl;
            mecp.Dock = DockStyle.Fill;
            #region tooltip Information
            tp.Tooltip = "[" + @meta.type.ToLower() + "] " + @meta.name;
            try
            {
                string PluginSet = Globals.PluginSetSelector.getActivePlugin();
                tp.Tooltip += "\nPlugin set: " + PluginSet;
            } 
            catch {}
            string PluginInfo = string.Empty;
            try
            {
                string PluginAuthor = ((HaloMap.Plugins.IFPIO)HaloMap.Plugins.IFPHashMap.H2IFPHash[meta.type]).author;
                if (PluginAuthor != null)
                    PluginInfo += "Author: " + PluginAuthor + "  ";
            } 
            catch { }
            try
            {
                string PluginVersion = ((HaloMap.Plugins.IFPIO)HaloMap.Plugins.IFPHashMap.H2IFPHash[meta.type]).version;
                if (PluginVersion != null)
                    PluginInfo += "Version: " + PluginVersion + "  ";
            }
            catch { }
            if (PluginInfo != string.Empty)
                tp.Tooltip += "\nPlugin " + PluginInfo;
            #endregion

            this.tabs.AllowDrop = true;
            //this.tabs.DragDrop += new DragEventHandler(tp_DragDrop);
            //this.tabs.DragEnter += new DragEventHandler(tp_DragEnter);
            //this.tabs.DragLeave += new EventHandler(tp_DragLeave);
            //// These tabs don't change selected index till MouseUp, so do it on MouseDown!
            //this.tabs.MouseDown += new MouseEventHandler(tabControl1_MouseDown);
            //this.tabs.MouseMove += new MouseEventHandler(tabControl1_MouseMove);

            this.tabs.SelectedTabIndex = tabs.Tabs.IndexOf(tp);

            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Focus();

            return tabs.Tabs.IndexOf(tp);
        }

        // These tabs don't change selected index till MouseUp, so we do it on MouseDown instead for dragging purposes!
        //void tabControl1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    for (int i = 0; i < tabs.Tabs.Count; i++)
        //    {
        //        TabItem tpTemp = tabs.Tabs[i];
        //        Rectangle r = tabs.GetTabRect(i);
        //        int change = (i < tabs.SelectedIndex ? 1 : -1);
        //        if ((e.X >= r.Left + change * -10) && (e.X <= r.Left + r.Width + change * -10) &&
        //            (e.Y >= r.Top) && (e.Y < r.Top + r.Height))
        //        {
        //            {                        
        //                tabs.SelectedTab = tpTemp;
        //                this.Text = map.filePath.Substring(map.filePath.LastIndexOf('\\') + 1).ToUpper() + ": " +
        //                        tpTemp.ToolTipText;
        //            }
        //        }
        //    }
        //}

        //void tabControl1_MouseMove(object sender, MouseEventArgs e)
        //{
            /*
            if (e.Button == MouseButtons.Left)
            {
                //this.tabControl1.DoDragDrop(this.tabControl1.SelectedTab, DragDropEffects.All);

                int mX = e.X;
                int mY = e.Y;
                TabItem tp = tabControl1.SelectedTab;
                for (int i = 0; i < tabControl1.Tabs.Count; i++)
                {
                    TabItem tpTemp = tabControl1.Tabs[i];
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
                        tabControl1.Tabs.Remove(tpTemp);
                        tabControl1.Tabs.Insert(tabControl1.SelectedIndex + (change < 0 ? 0 : 1), tpTemp);
                        break;
                    }
                }
            }
            else
                isDragging = false;
            */

        //}

        //void tp_DragDrop(object sender, DragEventArgs e)
        //{
            /*
            TabItem DropTab = (TabItem)(e.Data.GetData(typeof(TabItem)));
            int mX = e.X - this.Left;
            int mY = e.Y - this.Top - (this.FontHeight+17);
            for (int i = 0; i < tabControl1.Tabs.Count; i++)
            {
                TabItem tp = tabControl1.Tabs[i];
                Rectangle r = tabControl1.GetTabRect(i);
                if ((mX >= r.Left) && (mX <= r.Left + r.Width + 30 ) &&
                    (mY >= r.Top) && (mY <= r.Top + r.Height))
                {
                    tabControl1.Tabs.RemoveAt(i);
                    tabControl1.Tabs.Insert(i, tp);
                    break;
                }
            }
            */
            //if (tabControl1.SelectedTab != DropTab)
            //    this.tabControl1.Tabs.Add(DropTab);
        //}

        //void tp_DragLeave(object sender, EventArgs e)
        //{
            //
        //}

        //void tp_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(typeof(TabItem)))
        //        e.Effect = DragDropEffects.Move;
        //    else 
        //        e.Effect = DragDropEffects.None;
        //}


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
            foreach (TabItem tp in this.tabs.Tabs)
            {
                // Dispose of each Meta Editor Control Page (causes change save validation
                tp.AttachedControl.Controls[0].Dispose();
            }
        }

        public bool checkSelectionInCurrentTag()
        {
//            return (meta.TagIndex == ((reflexiveData)treeViewTagReflexives.SelectedNode.Tag).inTagNumber);
            return true;
        }

        private void customTabControl1_TabClosing(object sender, TabControlCancelEventArgs e)
        {

            if (tabs.Tabs.Count <= 1)
                this.Close();
        }

        private void tabs_TabItemClose(object sender, TabStripActionEventArgs e)
        {
            ((DevComponents.DotNetBar.TabControl)sender).SelectedTab.AttachedControl.Controls[0].Dispose();
        }

    }
}
