// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Meta.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for Meta.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Meta
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using HaloMap.Map;
    using HaloMap.Plugins;
    using HaloMap.RawData;

    /// <summary>
    /// Summary description for Meta.
    /// </summary>
    /// <remarks></remarks>
    public class Meta
    {
        #region Constants and Fields

        /// <summary>
        /// The ms.
        /// </summary>
        public MemoryStream MS;

        /// <summary>
        /// The headersize.
        /// </summary>
        public int headersize;

        /// <summary>
        /// The ident.
        /// </summary>
        public int ident;

        /// <summary>
        /// The itemcount.
        /// </summary>
        public int itemcount;

        /// <summary>
        /// The items.
        /// </summary>
        public List<Item> items = new List<Item>();

        /// <summary>
        /// The magic.
        /// </summary>
        public int magic;

        /// <summary>
        /// The map.
        /// </summary>
        /// <remarks></remarks>
        public Map Map { get; private set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string name;

        /// <summary>
        /// The offset.
        /// </summary>
        public int offset;

        /// <summary>
        /// The padding.
        /// </summary>
        public char padding;

        /// <summary>
        /// The parsed.
        /// </summary>
        public bool parsed;

        /// <summary>
        /// The raw.
        /// </summary>
        public RawDataContainer raw;

        /// <summary>
        /// The raw type.
        /// </summary>
        public RawDataContainerType rawType;

        /// <summary>
        /// The reflexivecount.
        /// </summary>
        public int reflexivecount;

        /// <summary>
        /// The scannedwithent.
        /// </summary>
        public bool scannedwithent;

        /// <summary>
        /// The size.
        /// </summary>
        public int size;

        /// <summary>
        /// The TagIndex.
        /// </summary>
        public int TagIndex;

        /// <summary>
        /// The type.
        /// </summary>
        public string type;

        /// <summary>
        /// 
        /// </summary>
        public MetaScanner scanner;

        #endregion

        #region Enums

        /// <summary>
        /// The item type.
        /// </summary>
        /// <remarks></remarks>
        public enum ItemType
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
            /// The string.
            /// </summary>
            String
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Meta"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public Meta(Map map)
        {
            this.Map = map;
            this.scanner = new MetaScanner(this);
        }


        #region Public Methods

        /// <summary>
        /// The scan meta items.
        /// </summary>
        /// <param name="manual">The manual.</param>
        /// <param name="parsed">The parsed.</param>
        /// <remarks></remarks>
        public void ScanMetaItems(bool manual, bool parsed)
        {
            if (parsed)
            {
                this.parsed = true;
            }

            this.items.Clear();
            switch (Map.HaloVersion)
            {
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:

                    

                    this.magic = Map.PrimaryMagic;

                    if (Map.MetaInfo.TagType[this.TagIndex] == "bitm" && Map.MetaInfo.external[this.TagIndex] &&
                        Map.HaloVersion != HaloVersionEnum.Halo1)
                    {
                        Map.CloseMap();
                        Map.OpenMap(MapTypes.Bitmaps);
                    }
                    else
                    {
                        Map.CloseMap();
                        Map.OpenMap(MapTypes.Internal);
                    }

                    this.ReadMetaFromMap(this.TagIndex, false);

                    if (manual && this.type != "sbsp")
                    {
                        this.scannedwithent = true;

                        IFPHashMap.RemoveIfp(this.type, Map);

                        IFPIO io = IFPHashMap.GetIfp(this.type, Map.HaloVersion);

                        this.headersize = io.headerSize;

                        scanner.ScanWithIFP(ref io);
                    }
                    else
                    {
                        scanner.ScanManually();
                    }

                    break;



                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:

                    #region Halo 2

                    Map.OpenMap(MapTypes.Internal);
                    this.ReadMetaFromMap(this.TagIndex, false);

                    if (manual)
                    {
                        // {&&meta.type!="sbsp")
                        IFPHashMap.RemoveIfp(this.type, Map);

                        IFPIO io = IFPHashMap.GetIfp(this.type, Map.HaloVersion);

                        this.headersize = io.headerSize;

                        scanner.ScanWithIFP(ref io);
                    }
                    else
                    {
                        scanner.ScanManually();
                    }

                    break;

                    #endregion
            }
        }

        /// <summary>
        /// The ask for tag name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <returns>The ask for tag name.</returns>
        /// <remarks></remarks>
        public static string askForTagName(string newName)
        {
            Form newForm = new Form();
            newForm.Name = "newForm";
            newForm.Size = new Size(500, 110);
            newForm.Text = "Select exported tag name";
            newForm.FormBorderStyle = FormBorderStyle.FixedDialog;

            

            #region label data

            Label newLabel = new Label();
            newLabel.Location = new Point(10, 14);
            newLabel.Name = "newLabel";
            newLabel.Size = new Size(68, 18);
            newLabel.Text = "Export As";

            #endregion

            #region Textbox Data

            TextBox newTextBox = new TextBox();
            newTextBox.Location = new Point(80, 10);
            newTextBox.Name = "newTextBox";
            newTextBox.Size = new Size(400, 10);
            newTextBox.Text = newName; // Set the name to our predicted name

            #endregion

            #region Button Data

            Button newAddButton = new Button();
            newAddButton.AutoSize = false;
            newAddButton.Name = "newAddButton";
            newAddButton.Size = new Size(160, 30);
            newAddButton.Location = new Point(170, 40);
            newAddButton.Parent = newForm;
            newAddButton.Text = "Save tag";
            newAddButton.DialogResult = DialogResult.OK;

            #endregion

            #region Add Controls

            newForm.Controls.Add(newLabel);
            newForm.Controls.Add(newTextBox);
            newForm.Controls.Add(newAddButton);
            DialogResult result = newForm.ShowDialog();

            #endregion

            if (result == DialogResult.OK)
            {
                return newTextBox.Text;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            this.items.Clear();
            if (this.MS != null)
            {
                this.MS.Dispose();
            }

            if (this.raw != null)
            {
                for (int i = 0; i < this.raw.rawChunks.Count; i++)
                {
                    if (this.raw.rawChunks[i].MS != null)
                        this.raw.rawChunks[i].MS.Dispose();
                }
            }
        }

        /// <summary>
        /// The find items by description.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int[] FindItemsByDescription(string name)
        {
            int[] tempint = new int[this.items.Count];
            int count = 0;
            for (int x = 0; x < this.items.Count; x++)
            {
                Item i = this.items[x];
                string[] tempd = i.description.Split('[');
                string ix = tempd[0];

                if (ix.Trim() == name)
                {
                    tempint[count] = x;
                    count++;
                }
            }

            int[] returnint = new int[count];
            
            Array.Copy(tempint, 0, returnint, 0, count);
            return returnint;
        }

        /// <summary>
        /// The find items by offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The find items by offset.</returns>
        /// <remarks></remarks>
        public int FindItemsByOffset(int offset)
        {
            for (int x = 0; x < this.items.Count; x++)
            {
                Item i = this.items[x];
                if (i.offset == offset)
                {
                    return x;
                }
            }

            return -1;
        }

        /// <summary>
        /// The load meta from file.
        /// </summary>
        /// <param name="inputFileName">The input file name.</param>
        /// <remarks></remarks>
        public void LoadMetaFromFile(string inputFileName)
        {
            // write memorysteam of meta to file
            FileStream FS = new FileStream(inputFileName, FileMode.Open);
            BinaryReader BR = new BinaryReader(FS);
            this.size = (int)FS.Length;
            this.MS = new MemoryStream(this.size);
            BR.BaseStream.Position = 0;
            this.MS.Write(BR.ReadBytes(this.size), 0, this.size);
            BR.Close();
            FS.Close();

            // write idents,strings,reflexives
            XmlTextReader xtr = new XmlTextReader(inputFileName + ".xml");
            xtr.WhitespaceHandling = WhitespaceHandling.None;

            while (xtr.Read())
            {
                // MessageBox.Show(xtr.Name);
                switch (xtr.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xtr.Name == "Meta")
                        {
                            this.type = xtr.GetAttribute("TagType");
                            this.name = xtr.GetAttribute("TagName");
                            this.parsed = xtr.GetAttribute("Parsed") == "True" ? true : false;
                            this.size = Convert.ToInt32(xtr.GetAttribute("Size"));
                            this.magic = Convert.ToInt32(xtr.GetAttribute("Magic"));
                            this.padding = Convert.ToChar(xtr.GetAttribute("Padding"));
                            this.offset = Convert.ToInt32(xtr.GetAttribute("Offset"));
                        }
                        else if (xtr.Name == "Reflexive")
                        {
                            Reflexive r = new Reflexive();
                            r.description = xtr.GetAttribute("Description");
                            r.offset = Convert.ToInt32(xtr.GetAttribute("Offset"));
                            r.chunkcount = Convert.ToInt32(xtr.GetAttribute("ChunkCount"));
                            r.chunksize = Convert.ToInt32(xtr.GetAttribute("ChunkSize"));
                            r.translation = Convert.ToInt32(xtr.GetAttribute("Translation"));
                            r.pointstotagtype = xtr.GetAttribute("PointsToTagType");
                            r.pointstotagname = xtr.GetAttribute("PointsToTagName");
                            r.pointstoTagIndex = Map.Functions.ForMeta.FindByNameAndTagType(
                                r.pointstotagtype, r.pointstotagname);
                            r.intagtype = xtr.GetAttribute("TagType");
                            r.intagname = xtr.GetAttribute("TagName");
                            r.intag = Map.Functions.ForMeta.FindByNameAndTagType(r.intagtype, r.intagname);
                            this.items.Add(r);
                        }
                        else if (xtr.Name == "Ident")
                        {
                            Ident id = new Ident();

                            id.description = xtr.GetAttribute("Description");
                            id.offset = Convert.ToInt32(xtr.GetAttribute("Offset"));
                            id.pointstotagtype = xtr.GetAttribute("PointsToTagType");
                            id.pointstotagname = xtr.GetAttribute("PointsToTagName");
                            id.pointstoTagIndex = Map.Functions.ForMeta.FindByNameAndTagType(
                                id.pointstotagtype, id.pointstotagname);
                            id.intagtype = xtr.GetAttribute("TagType");
                            id.intagname = xtr.GetAttribute("TagName");
                            id.intag = Map.Functions.ForMeta.FindByNameAndTagType(id.intagtype, id.intagname);
                            this.items.Add(id);
                        }
                        else if (xtr.Name == "String")
                        {
                            String s = new String();
                            s.description = xtr.GetAttribute("Description");
                            s.offset = Convert.ToInt32(xtr.GetAttribute("Offset"));
                            s.name = xtr.GetAttribute("StringName");
                            s.intagtype = xtr.GetAttribute("TagType");
                            s.intagname = xtr.GetAttribute("TagName");
                            s.intag = Map.Functions.ForMeta.FindByNameAndTagType(s.intagtype, s.intagname);
                            this.items.Add(s);
                        }

                        break;
                    default:
                        break;
                }
            }

            xtr.Close();

            // 
            ///check for raw
            this.rawType = Map.Functions.ForMeta.CheckForRaw(this.type);
            if (this.rawType != RawDataContainerType.Empty)
            {
                this.raw = new RawDataContainer();
                this.raw = this.raw.LoadRawFromFile(inputFileName, this);
            }
        }

        /// <summary>
        /// The read meta from map.
        /// </summary>
        /// <param name="tagIndex">The tagIndex.</param>
        /// <param name="dontreadraw">The dontreadraw.</param>
        /// <remarks></remarks>
        public void ReadMetaFromMap(int tagIndex, bool dontReadRaw)
        {
            // set meta properties
            this.Map = Map;
            this.TagIndex = tagIndex;
            this.type = Map.MetaInfo.TagType[tagIndex];
            this.name = Map.FileNames.Name[tagIndex];
            this.offset = Map.MetaInfo.Offset[tagIndex];
            this.size = Map.MetaInfo.Size[tagIndex];
            this.ident = Map.MetaInfo.Ident[tagIndex];

            string temps = this.offset.ToString("X");
            char[] tempc = temps.ToCharArray();
            int xxx = tempc.Length;
            this.padding = tempc[xxx - 1];

            // THIS = Currently Selected Tag
            // Find current Tag Meta and read into memory stream (MS)
            this.MS = new MemoryStream(this.size);
            Map.BR.BaseStream.Position = this.offset;
            this.MS.Write(Map.BR.ReadBytes(this.size), 0, this.size);

            // Checks if type has raw data
            this.rawType = Map.Functions.ForMeta.CheckForRaw(this.type);
            if (dontReadRaw == false)
            {
                if (rawType != RawDataContainerType.Empty)
                {
                    this.raw = Map.Functions.ForMeta.ReadRaw(this.TagIndex, dontReadRaw);
                }
            }

            if (this.type == "sbsp")
            {
                int h = Map.BSP.FindBSPNumberByBSPIdent(this.ident);
                this.magic = Map.BSP.sbsp[h].magic;
            }
            else if (this.type == "ltmp")
            {
                int h = Map.BSP.FindBSPNumberByLightMapIdent(this.ident);
                this.magic = Map.BSP.sbsp[h].magic;
            }
            else
            {
                // Not "sbsp" or "ltmp"
                // For Halo 1 or Halo CE
                if (Map.HaloVersion == HaloVersionEnum.HaloCE || Map.HaloVersion == HaloVersionEnum.Halo1)
                {
                    this.magic = Map.PrimaryMagic;
                }
                else
                {
                    // For Halo 2
                    this.magic = Map.SecondaryMagic;
                }
            }
        }

        /// <summary>
        /// The recursively load metas.
        /// </summary>
        /// <param name="parsed">The parsed.</param>
        /// <param name="pb">The pb.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList RecursivelyLoadMetas(bool parsed, ToolStripProgressBar pb)
        {
            ArrayList metas = new ArrayList(0);
            metas.Add(this);
            SaveRecursiveFunction(this, ref metas, parsed, pb, 0, 100);
            return metas;
        }

        /// <summary>
        /// The relink references.
        /// </summary>
        /// <remarks></remarks>
        public void RelinkReferences()
        {
            for (int x = 0; x < this.items.Count; x++)
            {
                Item i = items[x];
                switch (i.type)
                {
                    case ItemType.Ident:
                        Ident id = (Ident)i;
                        id.pointstoTagIndex = Map.Functions.ForMeta.FindByNameAndTagType(
                            id.pointstotagtype, id.pointstotagname);
                        if (id.pointstoTagIndex == -1)
                        {
                            id.ident = -1;
                        }
                        else
                        {
                            id.ident = Map.MetaInfo.Ident[id.pointstoTagIndex];
                        }

                        items[x] = id;
                        break;
                    case ItemType.String:
                        String s = (String)i;
                        if (Array.IndexOf(Map.Strings.Name, s.name) == -1)
                        {
                            s.name = string.Empty;
                            s.id = 0;
                        }

                        items[x] = s;
                        break;
                }
            }
        }

        /// <summary>
        /// Used to export the current meta to a file.
        /// </summary>
        /// <param name="outputFileName">The filename to save the Meta to.</param>
        /// <param name="renameTag">The rename tag.</param>
        /// <remarks></remarks>
        public string SaveMetaToFile(string outputFileName, bool renameTag)
        {
            string[] splitOut = outputFileName.Split('\\');
            // if the filename contains an extension, remove it from the split list
            if (splitOut[splitOut.Length - 1].Contains("."))
            {
                splitOut[splitOut.Length - 1] = splitOut[splitOut.Length - 1].Substring(0, splitOut[splitOut.Length - 1].IndexOf('.'));
            }
            string[] splitOrig = this.name.Split('\\');
            
            // use the starting path for the tag name...
            string tempName = string.Empty;
            for (int i = 0; i < splitOrig.Length; i++)
            {
                if (splitOrig[i] != splitOrig[splitOrig.Length - 1])
                    tempName += splitOrig[i];
                else
                    tempName += splitOut[splitOut.Length - 1];
                if (i < splitOrig.Length - 1)
                    tempName += "\\";
            }

            if (renameTag)
            {
                tempName = askForTagName(tempName);
                if (tempName == string.Empty)
                {
                    return this.name;
                }
            }
            outputFileName = outputFileName.Replace("<", "_");
            outputFileName = outputFileName.Replace(">", "_");

            // write memorysteam of meta to file
            FileStream FS = new FileStream(outputFileName, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS);
            BW.BaseStream.Position = 0;
            BW.BaseStream.Write(this.MS.ToArray(), 0, this.size);
            BW.Close();
            FS.Close();

            XmlTextWriter xtw = new XmlTextWriter(outputFileName + ".xml", Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("Meta");
            xtw.WriteAttributeString("TagType", this.type);
            xtw.WriteAttributeString("TagName", tempName);
            xtw.WriteAttributeString("Offset", this.offset.ToString());
            xtw.WriteAttributeString("Size", this.size.ToString());
            xtw.WriteAttributeString("Magic", this.magic.ToString());
            xtw.WriteAttributeString("Parsed", this.parsed.ToString());
            xtw.WriteAttributeString("Date", DateTime.Today.ToShortDateString());
            xtw.WriteAttributeString("Time", DateTime.Now.ToShortTimeString());
            xtw.WriteAttributeString("EntityVersion", "0.1");
            xtw.WriteAttributeString("Padding", this.padding.ToString());

            // xtw.
            for (int x = 0; x < this.items.Count; x++)
            {
                Item i = this.items[x];
                if (i.intagname == this.name)
                    i.intagname = tempName;
                switch (i.type)
                {
                    case ItemType.Reflexive:
                        Reflexive r = (Reflexive)i;
                        xtw.WriteStartElement("Reflexive");
                        xtw.WriteAttributeString("Description", r.description);
                        xtw.WriteAttributeString("Offset", r.offset.ToString());
                        xtw.WriteAttributeString("ChunkCount", r.chunkcount.ToString());
                        xtw.WriteAttributeString("ChunkSize", r.chunksize.ToString());
                        xtw.WriteAttributeString("Translation", r.translation.ToString());
                        xtw.WriteAttributeString("PointsToTagType", r.pointstotagtype);
                        xtw.WriteAttributeString("PointsToTagName", r.pointstotagname);
                        xtw.WriteAttributeString("TagType", r.intagtype);
                        xtw.WriteAttributeString("TagName", r.intagname);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Ident:
                        Ident id = (Ident)i;
                        xtw.WriteStartElement("Ident");
                        xtw.WriteAttributeString("Description", id.description);
                        xtw.WriteAttributeString("Offset", id.offset.ToString());
                        xtw.WriteAttributeString("PointsToTagType", id.pointstotagtype);
                        xtw.WriteAttributeString("PointsToTagName", id.pointstotagname);
                        xtw.WriteAttributeString("TagType", id.intagtype);
                        xtw.WriteAttributeString("TagName", id.intagname);
                        xtw.WriteEndElement();
                        break;
                    case ItemType.String:
                        String s = (String)i;
                        xtw.WriteStartElement("String");
                        xtw.WriteAttributeString("Description", s.description);
                        xtw.WriteAttributeString("Offset", s.offset.ToString());
                        xtw.WriteAttributeString("StringName", s.name);
                        xtw.WriteAttributeString("TagType", s.intagtype);
                        xtw.WriteAttributeString("TagName", s.intagname);
                        xtw.WriteEndElement();
                        break;
                }
                if (i.intagname == tempName)
                    i.intagname = this.name;
            }

            xtw.WriteEndElement();
            xtw.Close();
            if (this.rawType != RawDataContainerType.Empty)
            {
                this.raw.SaveRawToFile(outputFileName, this);
            }

            return tempName;
        }

        /// <summary>
        /// The save recursive.
        /// </summary>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="parsed">The parsed.</param>
        /// <param name="pb">The pb.</param>
        /// <remarks></remarks>
        public void SaveRecursive(string outputFilePath, bool parsed, ToolStripProgressBar pb)
        {
            ArrayList metas = RecursivelyLoadMetas(parsed, pb);

            #region Get name tag name
            // Contains the orignal meta name
            string origName = ((Meta)metas[0]).name;
            Globals.askForName metaRename = new Globals.askForName("Enter new name for Meta", "Select Exported tag name:", "&Export Recursively", origName, origName.Substring(origName.LastIndexOf('\\') + 1), true);
            if (metaRename.ShowDialog() == DialogResult.Cancel)
            {
                pb.Value = 0;
                return;
            }
            // Contains the changed meta name
            string newName = metaRename.getTextBoxValue();
            metaRename.Dispose();
            #endregion

            // If set to true, will rename any linked tags with the same tag names
            // eg. [eqip] objects\powerups\shotgun_ammo\shotgun_ammo renamed to objects\powerups\shotgun_ammo_max\shotgun_ammo_max
            //    would also have it's name changed in [hltm] & [mode]
            bool recursivelyRenameTags = false;
            if (newName != this.name)
            {
                if (MessageBox.Show("Do you wish to recursively rename similar tags?\n", "Rename recursively?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    recursivelyRenameTags = true;
            }



            int y = newName.LastIndexOf("\\") + 1;
            FileStream FS =
                new FileStream(
                    outputFilePath + "\\" + newName.Substring(y, newName.Length - y) + ".info", FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = (Meta)metas[x];

                string temp1 = m.name;

                #region Recursive Renaming
                List<string> backup = new List<string>();
                // Backup all the items PointsToTagName values &
                // change these to reflect recursive renaming
                // Rename the first Meta
                if (x == 0 && !recursivelyRenameTags)
                    temp1 = newName;
                else if (recursivelyRenameTags && temp1 == origName)
                {
                    temp1 = newName;
                    for (int i = 0; i < m.items.Count; i++)
                    {
                        switch (m.items[i].type)
                        {
                            case ItemType.Ident:
                                backup.Add(((Ident)m.items[i]).pointstotagname);
                                if (((Ident)m.items[i]).pointstotagname == this.name)
                                    ((Ident)m.items[i]).pointstotagname = newName;
                                break;
                            case ItemType.Reflexive:
                                backup.Add(((Reflexive)m.items[i]).pointstotagname);
                                if (((Reflexive)m.items[i]).pointstotagname == this.name)
                                    ((Reflexive)m.items[i]).pointstotagname = newName;
                                break;
                            default:
                                backup.Add(string.Empty);
                                break;
                        }
                    }
                }
                #endregion

                // directory to create
                int loc = temp1.LastIndexOf("\\") + 1;
                string temp2 = outputFilePath + "\\" + temp1.Substring(0, loc);
                Directory.CreateDirectory(temp2);

                // name of meta
                string tempname = temp1.Substring(loc, temp1.Length - loc);
                string temp3 = temp2 + tempname + "." + m.type;
                temp3 = temp3.Replace("<", "_");
                temp3 = temp3.Replace(">", "_");
                m.SortItemsByOffset();               
                m.SaveMetaToFile(temp3, false);

                #region Recursive Renaming Restore
                // Restore Items PointsToTagName values
                if (recursivelyRenameTags)
                {
                    for (int i = 0; i < m.items.Count; i++)
                    {
                        switch (m.items[i].type)
                        {
                            case ItemType.Ident:
                                if (((Ident)m.items[i]).pointstotagname == this.name)
                                    ((Ident)m.items[i]).pointstotagname = backup[i];
                                break;
                            case ItemType.Reflexive:
                                if (((Reflexive)m.items[i]).pointstotagname == this.name)
                                    ((Reflexive)m.items[i]).pointstotagname = backup[i];
                                break;
                        }
                    }
                    backup.Clear();
                }
                #endregion

                SW.WriteLine(temp3);
            }

            SW.Close();
            FS.Close();

            // Reset Progress Bar
            pb.Value = 0;
        }

        /// <summary>
        /// The sort items by offset.
        /// </summary>
        /// <remarks></remarks>
        public void SortItemsByOffset()
        {
            Comparison<Item> compareBySalary;
            compareBySalary = new Comparison<Item>(new ItemSorter().Compare);

            this.items.Sort(compareBySalary);
        }

        /// <summary>
        /// The write references.
        /// </summary>
        /// <remarks></remarks>
        public void WriteReferences()
        {
            BinaryWriter BW = new BinaryWriter(this.MS);

            for (int xx = 0; xx < this.items.Count; xx++)
            {
                Item i = this.items[xx];

                if (i.intag != this.TagIndex)
                {
                    continue;
                }

                switch (i.type)
                {
                    case ItemType.Reflexive:
                        Reflexive reflex = (Reflexive)i;
                        int newreflex = this.offset + reflex.translation + this.magic;

                        if (reflex.pointstoTagIndex != this.TagIndex)
                        {
                            newreflex = this.Map.MetaInfo.Offset[reflex.pointstoTagIndex] + reflex.translation + this.magic;
                        }

                        BW.BaseStream.Position = reflex.offset;
                        BW.Write(reflex.chunkcount);
                        BW.Write(newreflex);
                        break;

                    case ItemType.Ident:
                        Ident id = (Ident)i;

                        BW.BaseStream.Position = id.offset;
                        BW.Write(id.ident);

                        break;

                    case ItemType.String:
                        String ss = (String)i;
                        byte bleh = 0;
                        BW.BaseStream.Position = ss.offset;
                        BW.Write((short)ss.id);
                        BW.Write(bleh);
                        BW.Write((byte)ss.name.Length);
                        break;
                    default:
                        MessageBox.Show(i.type.ToString());
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The save recursive function.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="metas">The metas.</param>
        /// <param name="parsed">The parsed.</param>
        /// <param name="pb">The pb.</param>
        /// <param name="percent">The percent.</param>
        /// <param name="percentsize">The percentsize.</param>
        /// <remarks></remarks>
        private void SaveRecursiveFunction(
            Meta meta, 
            ref ArrayList metas, 
            bool parsed, 
            ToolStripProgressBar pb, 
            float percent, 
            float percentsize)
        {
            int wtf = metas.Count;
            if (meta.items.Count == 0)
            {
                return;
            }

            float test = meta.items.Count;

            float currentpercentsize = percentsize / test;

            for (int x = 0; x < meta.items.Count; x++)
            {
                Item i = meta.items[x];
                if (i.type != ItemType.Ident)
                {
                    continue;
                }

                float currentpercentage = percent + (currentpercentsize * x);
                pb.Value = (int)currentpercentage;
                Application.DoEvents();
                Ident id = (Ident)i;

                if (id.ident == -1)
                {
                    continue;
                }

                if (id.ident == 0)
                {
                    int tagIndex = Map.Functions.ForMeta.FindByNameAndTagType("sbsp", i.intagname);
                    id.ident = Map.MetaInfo.Ident[tagIndex];
                }

                bool exists = false;

                for (int e = 0; e < metas.Count; e++)
                {
                    Meta tempmeta = (Meta)metas[e];

                    // if (id.pointstotagtype  ==tempmeta.type&&id.pointstotagname ==tempmeta.name )
                    if (id.ident == tempmeta.ident)
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    continue;
                }

                // Drag & drop w/ recursive locks up. num == -1
                int num = Map.Functions.ForMeta.FindMetaByID(id.ident);
                if (num < 0)
                {
                    MessageBox.Show("ERROR! Not Found!");
                }

                Meta m = new Meta(Map);
                if (parsed)
                {
                    m.parsed = true;
                }

                m.ReadMetaFromMap(num, false);
                if (m.type == "ltmp" | m.type == "matg")
                {
                    continue;
                }

                if (m.type == "phmo" | m.type == "coll" | m.type == "jmad")
                {
                    m.parsed = false;
                }

                if (m.type != "jmad")
                {
                    IFPIO ifp = IFPHashMap.GetIfp(m.type, Map.HaloVersion);
                    m.headersize = ifp.headerSize;

                    m.scanner.ScanWithIFP(ref ifp);
                }
                else
                {
                    m.scanner.ScanManually();
                }

                m.SortItemsByOffset();

                metas.Add(m);

                Application.DoEvents();
                SaveRecursiveFunction(m, ref metas, parsed, pb, currentpercentage, currentpercentsize);
            }
        }

        #endregion

        /// <summary>
        /// The ident.
        /// </summary>
        /// <remarks></remarks>
        public class Ident : Item
        {
            #region Constants and Fields

            /// <summary>
            /// The ident.
            /// </summary>
            public int ident;

            /// <summary>
            /// The pointstotagname.
            /// </summary>
            public string pointstotagname;

            /// <summary>
            /// The pointstoTagIndex.
            /// </summary>
            public int pointstoTagIndex;

            /// <summary>
            /// The pointstotagtype.
            /// </summary>
            public string pointstotagtype;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Ident"/> class.
            /// </summary>
            /// <remarks></remarks>
            public Ident()
            {
                type = ItemType.Ident;
            }

            #endregion
        }

        /// <summary>
        /// The item.
        /// </summary>
        /// <remarks></remarks>
        public class Item
        {
            #region Constants and Fields

            /// <summary>
            /// The child.
            /// </summary>
            public int child = -1;

            /// <summary>
            /// The description.
            /// </summary>
            public string description;

            /// <summary>
            /// The intag.
            /// </summary>
            public int intag;

            /// <summary>
            /// The intagname.
            /// </summary>
            public string intagname;

            /// <summary>
            /// The intagtype.
            /// </summary>
            public string intagtype;

            /// <summary>
            /// The map offset.
            /// </summary>
            public int mapOffset;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The parent.
            /// </summary>
            public int parent = -1;

            /// <summary>
            /// The sibling.
            /// </summary>
            public int sibling = -1;

            /// <summary>
            /// The type.
            /// </summary>
            public ItemType type;

            #endregion
        }

        /// <summary>
        /// The item sorter.
        /// </summary>
        /// <remarks></remarks>
        public class ItemSorter : IComparer
        {
            #region Implemented Interfaces

            #region IComparer

            /// <summary>
            /// The compare.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>The compare.</returns>
            /// <exception cref="T:System.ArgumentException">
            /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.
            /// -or-
            ///   <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
            ///   </exception>
            /// <remarks></remarks>
            public int Compare(object x, object y)
            {
                return ((Item)x).offset.CompareTo(((Item)y).offset);
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// The reflexive.
        /// </summary>
        /// <remarks></remarks>
        public class Reflexive : Item
        {
            #region Constants and Fields

            /// <summary>
            /// The chunkcount.
            /// </summary>
            public int chunkcount;

            /// <summary>
            /// The chunksize.
            /// </summary>
            public int chunksize;

            /// <summary>
            /// The pointstotagname.
            /// </summary>
            public string pointstotagname;

            /// <summary>
            /// The pointstoTagIndex.
            /// </summary>
            public int pointstoTagIndex;

            /// <summary>
            /// The pointstotagtype.
            /// </summary>
            public string pointstotagtype;

            /// <summary>
            /// The translation.
            /// </summary>
            public int translation;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Reflexive"/> class.
            /// </summary>
            /// <remarks></remarks>
            public Reflexive()
            {
                type = ItemType.Reflexive;
            }

            #endregion
        }

        /// <summary>
        /// The string.
        /// </summary>
        /// <remarks></remarks>
        public class String : Item
        {
            #region Constants and Fields

            /// <summary>
            /// The id.
            /// </summary>
            public int id;

            /// <summary>
            /// The name.
            /// </summary>
            public string name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="String"/> class.
            /// </summary>
            /// <remarks></remarks>
            public String()
            {
                type = ItemType.String;
            }

            #endregion
        }
    }
}