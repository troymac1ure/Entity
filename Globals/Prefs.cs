using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace Globals
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public static class Prefs
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <remarks></remarks>
        public static string FilePath { get; private set; }

        /// <summary>
        /// The check update.
        /// </summary>
        public static updateFrequency checkUpdate = updateFrequency.Daily;

        /// <summary>
        /// The last check.
        /// </summary>
        public static DateTime lastCheck = DateTime.MinValue;

        /// <summary>
        /// The path bitmaps.
        /// </summary>
        public static string pathBitmaps = "bitmap.map";

        /// <summary>
        /// The path clean maps.
        /// </summary>
        public static string pathCleanMaps = Global.StartupPath + "\\CleanMaps";

        /// <summary>
        /// The path last file.
        /// </summary>
        public static List<RecentFile> RecentOpenedMaps = new List<RecentFile>();

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public class RecentFile
        {
            /// <summary>
            /// The path to the recently opened file
            /// </summary>
            public string Path;

            /// <summary>
            /// The corresponding MenuItem attached to the recently opened file
            /// </summary>
            public ToolStripMenuItem MenuItem;
        }

        /// <summary>
        /// The maximum # of recently used maps allowed in out list
        /// </summary>
        public const int MaxRecentFiles = 4;

        /// <summary>
        /// The path mainmenu.
        /// </summary>
        public static string pathMainmenu = "mainmenu.map";

        /// <summary>
        /// The path map folder.
        /// </summary>
        public static string pathMapFolder = Global.StartupPath;

        /// <summary>
        /// The path patch folder.
        /// </summary>
        public static string pathPatchFolder = Global.StartupPath;

        /// <summary>
        /// Folder for the plugins
        /// </summary>
        public static string pathPluginsFolder = Global.StartupPath + @"\Plugins\Halo 2\ent\";
        /// <summary>
        /// Folder for Bitmap Extraction / Injection
        /// </summary>
        public static string pathBitmapsFolder = Global.StartupPath + @"\Extracts\Bitmaps\";
        /// <summary>
        /// Folder for Meta Extraction / Injection
        /// </summary>
        public static string pathExtractsFolder = Global.StartupPath + @"\Extracts\";

        /// <summary>
        /// The path sp shared.
        /// </summary>
        public static string pathSPShared = "single_player_shared.map";

        /// <summary>
        /// The path shared.
        /// </summary>
        public static string pathShared = "shared.map";

        /// <summary>
        /// Option to always use default maps or if shared maps exist in the directory with the
        /// loaded map, then use those.
        /// </summary>
        public static bool useDefaultMaps = true;
        
        /// <summary>
        /// Whether to save / load settings from file or registry by default
        /// </summary>
        public static bool useRegistryEntries = true;

        #region Enums

        /// <summary>
        /// The update frequency.
        /// </summary>
        /// <remarks></remarks>
        public enum updateFrequency
        {
            /// <summary>
            /// The always.
            /// </summary>
            Always,

            /// <summary>
            /// The daily.
            /// </summary>
            Daily,

            /// <summary>
            /// The weekly.
            /// </summary>
            Weekly,

            /// <summary>
            /// The monthly.
            /// </summary>
            Monthly,

            /// <summary>
            /// The never.
            /// </summary>
            Never
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public class CustomPluginMask
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            /// <remarks></remarks>
            public string Name { get; set; }

            /// <summary>
            /// Gets the visible tag types.
            /// </summary>
            /// <remarks></remarks>
            public List<string> VisibleTagTypes { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            /// <remarks></remarks>
            public CustomPluginMask ()
            {
                VisibleTagTypes = new List<string>();
            }
        }

        /// <summary>
        /// Gets the custom plugin masks.
        /// </summary>
        /// <remarks></remarks>
        public static List<CustomPluginMask> CustomPluginMasks { get; private set; }

        /// <summary>
        /// Gets the quick access tag types.
        /// </summary>
        /// <remarks></remarks>
        public static List<QuickAccessTagType> QuickAccessTagTypes { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public class QuickAccessTagType
        {
            /// <summary>
            /// 
            /// </summary>
            public string TagType;

            /// <summary>
            /// 
            /// </summary>
            public List<string> TagPaths;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            /// <remarks></remarks>
            public QuickAccessTagType()
            {
                TagPaths = new List<string>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_MAINMENUFILE = "mainmenu";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_MPSHAREDFILE = "mpshared";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_SPSHAREDFILE = "spshared";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_BITMAPSFILE = "bitmaps";

        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_MAPSFOLDER = "mapsDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_CLEANMAPSFOLDER = "cleanmapsDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_PLUGINSFOLDER = "pluginsDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_PATCHFOLDER = "patchDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_BITMAPSFOLDER = "bitmapsDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_EXTRACTSFOLDER = "extractsDir";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_USEREGISTRY = "useRegistry";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_USEDEFMAPS = "useDefaultMaps";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_CHECKUPDATE = "checkUpdate";
        /// <summary>
        /// The date & time of the last update check
        /// </summary>
        private const string XML_NODE_LASTUPDATECHECK = "lastUpdateCheck";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_RECENTMAPSLIST = "recentMapsList";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_RECENTMAP = "recentMap";

        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_QUICKACCESSTAGSLIST = "quickAccessTagList";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_QUICKACCESSTAG = "quickAccessTag";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_QUICKACCESSTAGPATH = "tagPath";

        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_USERPLUGINMASKS = "userPluginMasks";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_PLUGINMASK = "pluginMask";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_ATTR_PLUGINMASKNAME = "name";
        /// <summary>
        /// 
        /// </summary>
        private const string XML_NODE_PLUGINMASK_VISIBLETAGTYPE = "visibleTagType";

        /// <summary>
        /// 
        /// </summary>
        private const string XML_ATTR_TAGTYPENAME = "name";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        static Prefs()
        {
            FilePath = Global.StartupPath + "\\User Settings.xml";
            CustomPluginMasks = new List<CustomPluginMask>();
            QuickAccessTagTypes = new List<QuickAccessTagType>();
        }

        /// <summary>
        /// Gets the type of the quick access tag.
        /// </summary>
        /// <param name="tagType">Type of the tag.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static QuickAccessTagType GetQuickAccessTagType(string tagType)
        {
            for (int i = 0 ; i<QuickAccessTagTypes.Count;i++)
            {
                if (QuickAccessTagTypes[i].TagType == tagType) return QuickAccessTagTypes[i];
            }
            return null;
        }

        /// <summary>
        /// Returns true if file was loaded
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool Load()
        {
            // check for settings file
            bool settingsFileExists = File.Exists(FilePath);

                // Parse the settings file
            if (settingsFileExists)
            {
                XmlDocument doc = new XmlDocument();

                // load settings into document
                doc.Load(FilePath);

                // loop rootnode's nodes, and depending on what node we find
                // do a different action.
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case XML_NODE_MAINMENUFILE:
                            {
                                pathMainmenu = node.InnerText;
                                break;
                            }
                        case XML_NODE_MPSHAREDFILE:
                            {
                                pathShared = node.InnerText;
                                break;
                            }
                        case XML_NODE_SPSHAREDFILE:
                            {
                                pathSPShared = node.InnerText;
                                break;
                            }
                        case XML_NODE_BITMAPSFILE:
                            {
                                pathBitmaps = node.InnerText;
                                break;
                            }

                        case XML_NODE_MAPSFOLDER:
                            {
                                pathMapFolder = node.InnerText;
                                break;
                            }
                        case XML_NODE_CLEANMAPSFOLDER:
                            {
                                pathCleanMaps = node.InnerText;
                                break;
                            }
                        case XML_NODE_PLUGINSFOLDER:
                            {
                                pathPluginsFolder = node.InnerText;
                                break;
                            }
                        case XML_NODE_BITMAPSFOLDER:
                            {
                                pathBitmapsFolder = node.InnerText;
                                break;
                            }
                        case XML_NODE_EXTRACTSFOLDER:
                            {
                                pathExtractsFolder = node.InnerText;
                                break;
                            }
                        case XML_NODE_PATCHFOLDER:
                            {
                                pathPatchFolder = node.InnerText;
                                break;
                            }

                        case XML_NODE_USEDEFMAPS:
                            {
                                useDefaultMaps = bool.Parse(node.InnerText);
                                break;
                            }
                        case XML_NODE_USEREGISTRY:
                            {
                                useRegistryEntries = bool.Parse(node.InnerText);
                                break;
                            }
                        case XML_NODE_CHECKUPDATE:
                            {
                                checkUpdate = (updateFrequency)Enum.Parse(typeof(updateFrequency), node.InnerText);
                                break;
                            }
                        case XML_NODE_LASTUPDATECHECK:
                            {
                                lastCheck = DateTime.Parse(node.InnerText);
                                break;
                            }


                        case XML_NODE_RECENTMAPSLIST:
                            {
                                foreach (XmlNode recentMapNode in node)
                                {
                                    RecentFile rf = new RecentFile();
                                    rf.Path = recentMapNode.InnerText;
                                    RecentOpenedMaps.Add(rf);
                                }
                                break;
                            }
                        case XML_NODE_QUICKACCESSTAGSLIST:
                            {
                                foreach (XmlNode quickAccessNode in node)
                                {
                                    QuickAccessTagType quickAcess = new QuickAccessTagType();
                                    quickAcess.TagType = quickAccessNode.Attributes[XML_ATTR_TAGTYPENAME].InnerText;
                                    foreach (XmlNode tagPathNode in quickAccessNode)
                                    {
                                        //switch (tagPathNode.Name)
                                        //{
                                        //    case XML_NODE_QUICKACCESSTAGPATH:
                                        //        {
                                        quickAcess.TagPaths.Add(tagPathNode.InnerText);
                                        //            break;
                                        //        }

                                        //}
                                    }
                                    QuickAccessTagTypes.Add(quickAcess);
                                }
                                break;
                            }
                        case XML_NODE_USERPLUGINMASKS:
                            {
                                foreach (XmlNode pluginMaskNode in node)
                                {
                                    CustomPluginMask mask = new CustomPluginMask();
                                    mask.Name = pluginMaskNode.Attributes[XML_ATTR_PLUGINMASKNAME].InnerText;

                                    foreach (XmlNode maskNode in pluginMaskNode)
                                    {
                                        //switch (maskNode.Name)
                                        //{
                                        //    case XML_NODE_PLUGINMASK_VISIBLETAGTYPE:
                                        //        {
                                        mask.VisibleTagTypes.Add(
                                            maskNode.Attributes[XML_ATTR_TAGTYPENAME].InnerText);
                                        //            break;
                                        //        }

                                        //}
                                    }

                                    CustomPluginMasks.Add(mask);
                                }
                                break;
                            }
                    }
                }
            }

            // If useRegistryEntires is set to false & the settings file exists, then the
            // user doesn't wish to use the registry, so we are good to exit
            if (settingsFileExists && !useRegistryEntries)
                return true;

            // If enabled, load the settings from the registry
            if (useRegistryEntries)
            {
                // Used for accessing the registry
                RegistryAccess Reg;

                try
                {
                    // UseSettingsFile contains a list of Entity directories that wish to use the settings file instead of
                    // the registry. Check if our path is in the list for using the settings file.
                    Reg = new RegistryAccess(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\UseSettingsFile\");

                    // If Entry exists in list, use settings file if it exists
                    if (Reg.getValue(Globals.Global.StartupPath) != null)
                        return settingsFileExists;

                    // Try to open the settings for Halo 2, if it doesn't exist, Reg.isOpen == false
                    Reg = new RegistryAccess(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths);
                    if (Reg.isOpen)
                    {
                        string tempS = string.Empty;
                        #region General Halo2 paths in registry
                        tempS = Reg.getValue(RegistryAccess.RegNames.MainMenuFile);
                        if (tempS != null) Prefs.pathMainmenu = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.SharedFile);
                        if (tempS != null) Prefs.pathShared = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.SinglePlayerSharedFile);
                        if (tempS != null) Prefs.pathSPShared = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.BitmapsFile);
                        if (tempS != null) Prefs.pathBitmaps = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.MapsPath);
                        if (tempS != null) Prefs.pathMapFolder = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.BitmapsPath);
                        if (tempS != null) Prefs.pathBitmapsFolder = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.ExtractsPath);
                        if (tempS != null) Prefs.pathExtractsFolder = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.PluginsPath);
                        if (tempS != null) Prefs.pathPluginsFolder = tempS;
                        tempS = Reg.getValue(RegistryAccess.RegNames.CleanMapsPath);
                        if (tempS != null) Prefs.pathCleanMaps = tempS;
                        #endregion
                        #region Entity specific paths & settings
                        Reg = new RegistryAccess(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\");
                        tempS = Reg.getValue("PatchFolder");
                        if (tempS != null) Prefs.pathPatchFolder = tempS;
                        tempS = Reg.getValue("UseDefaultMaps");
                        if (tempS != null) Prefs.useDefaultMaps = bool.Parse(tempS);
                        Prefs.useRegistryEntries = true;
                        #region Automatic Update
                        tempS = Reg.getValue("lastCheck");
                        if (tempS != null)
                            try
                            {
                                Prefs.lastCheck = DateTime.Parse(tempS);
                            }
                            catch
                            {
                                Prefs.lastCheck = DateTime.MinValue;
                            }
                        tempS = Reg.getValue("checkUpdate");
                        try
                        {
                            Prefs.updateFrequency updateFreq = (Prefs.updateFrequency)Enum.Parse(typeof(Prefs.updateFrequency), tempS);
                            Prefs.checkUpdate = updateFreq;
                        }
                        catch
                        {
                            Prefs.checkUpdate = Prefs.updateFrequency.Daily;
                        }
                        #endregion;

                        // Check for recent files in the registry
                        Reg = new RegistryAccess(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2RecentFiles);
                        if (Reg.isOpen)
                        {
                            for (int count = Prefs.MaxRecentFiles - 1; count >= 0; count--)
                            {
                                tempS = Reg.getValue(count.ToString());
                                if (tempS != null)
                                {
                                    RecentFile rf = new RecentFile();
                                    rf.Path = tempS;
                                    RecentOpenedMaps.Insert(0, rf);
                                }
                            }
                        }

                        #endregion
                        #region Load Quick Access Tags
                        if (Prefs.useRegistryEntries)
                        {
                            try
                            {
                                RegistryAccess ra = new RegistryAccess(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\");
                                string[] tags = ra.getKeys();
                                foreach (string tagType in tags)
                                {
                                    QuickAccessTagType qatt = new QuickAccessTagType();
                                    qatt.TagType = tagType;
                                    ra.setKey(Microsoft.Win32.Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\ME\Tags\" + tagType + @"\");
                                    foreach (string tagName in ra.getNames())
                                    {
                                        if (ra.getValue(tagName).ToLower() == "true")
                                            qatt.TagPaths.Add(tagName);
                                    }
                                    if (qatt.TagPaths.Count > 0)
                                        QuickAccessTagTypes.Add(qatt);
                                }
                            }
                            catch
                            {
                                // Ignore errors regarding to Quick keys
                            }
                        }
                        #endregion
                        Reg.CloseReg();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Global.ShowErrorMsg("Prefs Load Exception", e);
                    return settingsFileExists;
                }

            }

            return false;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <remarks></remarks>
        public static void Save()
        {
            #region Settings File
            XmlTextWriter writer = new XmlTextWriter(FilePath, Encoding.ASCII);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            writer.WriteStartElement("entityUserSettings");
            writer.WriteElementString(XML_NODE_MAINMENUFILE, pathMainmenu);
            writer.WriteElementString(XML_NODE_MPSHAREDFILE, pathShared);
            writer.WriteElementString(XML_NODE_SPSHAREDFILE, pathSPShared);
            writer.WriteElementString(XML_NODE_BITMAPSFILE, pathBitmaps);

            writer.WriteElementString(XML_NODE_MAPSFOLDER, pathMapFolder);
            writer.WriteElementString(XML_NODE_CLEANMAPSFOLDER, pathCleanMaps);
            writer.WriteElementString(XML_NODE_PLUGINSFOLDER, pathPluginsFolder);
            writer.WriteElementString(XML_NODE_BITMAPSFOLDER, pathBitmapsFolder);
            writer.WriteElementString(XML_NODE_EXTRACTSFOLDER, pathExtractsFolder);
            writer.WriteElementString(XML_NODE_PATCHFOLDER, pathPatchFolder);

            writer.WriteElementString(XML_NODE_USEDEFMAPS, useDefaultMaps.ToString());
            writer.WriteElementString(XML_NODE_USEREGISTRY, useRegistryEntries.ToString());
            writer.WriteElementString(XML_NODE_CHECKUPDATE, checkUpdate.ToString());
            writer.WriteElementString(XML_NODE_LASTUPDATECHECK, lastCheck.ToString());

            writer.WriteStartElement(XML_NODE_RECENTMAPSLIST);
            // Only keep a list of the last 10 used maps (Entity only uses 4)
            for (int i = 0; i < Math.Min(RecentOpenedMaps.Count, 10); i++)
            {
                writer.WriteElementString(XML_NODE_RECENTMAP, RecentOpenedMaps[i].Path);
            }
            writer.WriteEndElement();

            writer.WriteStartElement(XML_NODE_QUICKACCESSTAGSLIST);
            for (int i = 0; i < QuickAccessTagTypes.Count; i++)
            {
                writer.WriteStartElement(XML_NODE_QUICKACCESSTAG);
                writer.WriteAttributeString(XML_ATTR_TAGTYPENAME, QuickAccessTagTypes[i].TagType);
                foreach (string tagPath in QuickAccessTagTypes[i].TagPaths)
                {
                    writer.WriteElementString(XML_NODE_QUICKACCESSTAGPATH, tagPath);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement(XML_NODE_USERPLUGINMASKS);
            foreach (CustomPluginMask mask in CustomPluginMasks)
            {
                writer.WriteStartElement(XML_NODE_PLUGINMASK);
                writer.WriteAttributeString(XML_ATTR_PLUGINMASKNAME, mask.Name);
                foreach (string tagType in mask.VisibleTagTypes)
                {
                    writer.WriteStartElement(XML_NODE_PLUGINMASK_VISIBLETAGTYPE);
                    writer.WriteAttributeString(XML_ATTR_TAGTYPENAME, tagType);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Close();
            #endregion

            #region Registry Entries
            if (useRegistryEntries)
            {
                //RegistryAccess.removeValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\UseSettingsFile\", Application.StartupPath);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, "", "");

                // General Halo2 program settings
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.MainMenuFile, Prefs.pathMainmenu);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.SharedFile, Prefs.pathShared);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.SinglePlayerSharedFile, Prefs.pathSPShared);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.BitmapsFile, Prefs.pathBitmaps);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.MapsPath, Prefs.pathMapFolder);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.CleanMapsPath, Prefs.pathCleanMaps);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.PluginsPath, Prefs.pathPluginsFolder);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.BitmapsPath, Prefs.pathBitmapsFolder);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2Paths, RegistryAccess.RegNames.ExtractsPath, Prefs.pathExtractsFolder);

                // Entity specific settings
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\", "PatchFolder", Prefs.pathPatchFolder);
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\", "UseDefaultMaps", Prefs.useDefaultMaps.ToString());
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\", "checkUpdate", Prefs.checkUpdate.ToString());
                RegistryAccess.setValue(Registry.CurrentUser, RegistryAccess.RegPaths.Halo2 + @"Entity\", "lastCheck", Prefs.lastCheck.ToString());

                // Save recently used maps list in the registry (max 10)
                for (int count = 0; count < Math.Min(RecentOpenedMaps.Count, 10); count++)
                {
                    RegistryAccess.setValue(
                        Registry.CurrentUser,
                        RegistryAccess.RegPaths.Halo2RecentFiles,
                        count.ToString(),
                        RecentOpenedMaps[count].Path);
                }
            }
            #endregion
        }
    }
}
