using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Globals
{
    public partial class PluginSetSelector : Form
    {
        public class pluginInfoClass
        {
            public List<string> Names;
            public List<string> Paths;

            public pluginInfoClass()
            {
                this.Names = new List<string>();
                this.Paths = new List<string>();
            }
        }

        #region fields
        public static pluginInfoClass pluginInfo = new pluginInfoClass();
        #endregion

        public PluginSetSelector()
        {
            InitializeComponent();
        }

        private static string Convert(object o) // : System.Converter<object, string>()
        {
            return (string)(o);
        }

        public static void populate()
        {
            if (pluginInfo.Names.Count < 1)
            {
                RegistryAccess ra = new RegistryAccess(Microsoft.Win32.Registry.CurrentUser,
                                         RegistryAccess.RegPaths.Halo2 + "PluginSets\\");
                if (ra.isOpen)
                {
                    pluginInfo.Names = new List<string>(ra.getNames());
                    pluginInfo.Paths = new List<string>(Array.ConvertAll(ra.getValues(),new Converter<object,string>(Convert)));
                    int i = pluginInfo.Names.IndexOf("");
                    if (i != -1)
                    {
                        pluginInfo.Names.RemoveAt(i);
                        pluginInfo.Paths.RemoveAt(i);
                    }
                }
                else
                {
                    pluginInfo.Names.Add("Default");
                    pluginInfo.Paths.Add(Prefs.pathPluginsFolder);
                }
                ra.CloseReg();
            }
        }

        public static string[] getNames()
        {
            return pluginInfo.Names.ToArray();
        }

        public static string[] getPaths()
        {
            return pluginInfo.Paths.ToArray();
        }

        public static string getName(string Path)
        {
            for (int i = 0; i < pluginInfo.Paths.Count; i++)
                if (pluginInfo.Paths[i] == Path)
                    return pluginInfo.Names[i];
            return string.Empty;
        }

        public static string getPath(string Name)
        {
            for (int i = 0; i < pluginInfo.Names.Count; i++)
                if (pluginInfo.Names[i] == Name)
                    return pluginInfo.Paths[i];
            return string.Empty;
        }

        public static string getActivePlugin()
        {
            RegistryAccess ra = new RegistryAccess(Microsoft.Win32.Registry.CurrentUser,
                                     RegistryAccess.RegPaths.Halo2 + "PluginSets\\");
            string s = ra.getValue("");
            ra.CloseReg();
            return s;
        }

        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = tbPluginDirectory.Text;
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                tbPluginDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void PluginSetSelector_Load(object sender, EventArgs e)
        {
            populate();
            listBox1.Items.AddRange(pluginInfo.Names.ToArray());
            listBox1.Items.Add("[New]");
        }

        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            // If [New] selected
            if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
            {
                pluginInfo.Names.Add(tbPluginName.Text);
                pluginInfo.Paths.Add(tbPluginDirectory.Text);
                listBox1.Items.Insert(listBox1.Items.Count - 1, tbPluginName.Text);
                listBox1.SelectedIndex--;
            }
            else
            {
                pluginInfo.Names[listBox1.SelectedIndex] = tbPluginName.Text;
                pluginInfo.Paths[listBox1.SelectedIndex] = tbPluginDirectory.Text;
                listBox1.Items[listBox1.SelectedIndex] = tbPluginName.Text;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == listBox1.Items.Count - 1 ||
                listBox1.SelectedIndex == -1)
            {
                tbPluginName.Text = string.Empty;
                tbPluginDirectory.Text = Prefs.pathPluginsFolder;
            }
            else
            {
                tbPluginName.Text = pluginInfo.Names[listBox1.SelectedIndex];
                tbPluginDirectory.Text = pluginInfo.Paths[listBox1.SelectedIndex];
            }
        }

        private void PluginSetSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Clear any old plugin listings
            RegistryAccess.removeKey(Microsoft.Win32.Registry.CurrentUser,
                                         RegistryAccess.RegPaths.Halo2 + "PluginSets\\");
            // We must always have at least a Default plugin
            if (pluginInfo.Names.Count == 0)
            {
                pluginInfo.Names.Add("Default");
                pluginInfo.Paths.Add(Prefs.pathPluginsFolder);
            }
            // Write all the plugins back to the registry
            for (int i = 0; i < pluginInfo.Names.Count; i++)
            {
                RegistryAccess.setValue(Microsoft.Win32.Registry.CurrentUser,
                                         RegistryAccess.RegPaths.Halo2 + "PluginSets\\",
                                         pluginInfo.Names[i],
                                         pluginInfo.Paths[i]);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                pluginInfo.Names.RemoveAt(listBox1.SelectedIndex);
                pluginInfo.Paths.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

    }
}
