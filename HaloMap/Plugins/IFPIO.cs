// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFPIO.cs" company="">
//   
// </copyright>
// <summary>
//   The ifpio.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    // modified 6-28-06 by TheTyckoMan
    // modified 7-23-06 by TheTyckoMan
    // modified 8-10-06 by TheTyckoMan
    /// <summary>
    /// The ifpio.
    /// </summary>
    /// <remarks></remarks>
    public class IFPIO
    {
        #region Constants and Fields

        /// <summary>
        /// The ent elements.
        /// </summary>
        public List<BaseObject> ENTElements; // Contains all items including those in reflexives/structs

        /// <summary>
        /// The author.
        /// </summary>
        public string author;

        /// <summary>
        /// The class type.
        /// </summary>
        public string classType;

        /// <summary>
        /// The header size.
        /// </summary>
        public int headerSize;

        /// <summary>
        /// The items.
        /// </summary>
        public object[] items;

        /// <summary>
        /// The revisions.
        /// </summary>
        public string[] revisions;

        /// <summary>
        /// The version.
        /// </summary>
        public string version;

        /// <summary>
        /// The al revisions.
        /// </summary>
        private readonly ArrayList alRevisions = new ArrayList(0);

        /// <summary>
        /// The file path.
        /// </summary>
        private string FilePath;

        /// <summary>
        /// The size.
        /// </summary>
        private int size;

        #endregion

        #region Enums

        /// <summary>
        /// The object enum.
        /// </summary>
        /// <remarks></remarks>
        public enum ObjectEnum
        {
            // Sorted by size listings. Sizes are found by dividing the value by 100.
            // Size Unknown
            /// <summary>
            /// The unused.
            /// </summary>
            Unused = 000, 

            // Size 1
            /// <summary>
            /// The byte_ flags.
            /// </summary>
            Byte_Flags = 100, // Bitmask8
            /// <summary>
            /// The byte.
            /// </summary>
            Byte = 110, 

            /// <summary>
            /// The char_ enum.
            /// </summary>
            Char_Enum = 120, // Enum8

            // Size 2
            /// <summary>
            /// The enum.
            /// </summary>
            Enum = 200, // Enum16
            /// <summary>
            /// The short.
            /// </summary>
            Short = 210, 

            /// <summary>
            /// The u short.
            /// </summary>
            UShort = 220, 

            /// <summary>
            /// The word_ flags.
            /// </summary>
            Word_Flags = 230, // Bitmask16

            // Size 4
            /// <summary>
            /// The float.
            /// </summary>
            Float = 400, 

            /// <summary>
            /// The ident.
            /// </summary>
            Ident = 410, 

            /// <summary>
            /// The int.
            /// </summary>
            Int = 420, 

            /// <summary>
            /// The long_ enum.
            /// </summary>
            Long_Enum = 430, // Enum32
            /// <summary>
            /// The long_ flags.
            /// </summary>
            Long_Flags = 440, // Bitmask32
            /// <summary>
            /// The tag type.
            /// </summary>
            TagType = 450, 

            /// <summary>
            /// The u int.
            /// </summary>
            UInt = 460, 

            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown = 470, 

            // Size 8
            /// <summary>
            /// The block.
            /// </summary>
            Block = 800, // TagType/Ident combo
            /// <summary>
            /// The struct.
            /// </summary>
            Struct = 810, 

            /// <summary>
            /// The string id.
            /// </summary>
            StringID, 

            /// <summary>
            /// The string.
            /// </summary>
            String, 

            /// <summary>
            /// The string 32.
            /// </summary>
            String32, 

            /// <summary>
            /// The unicode string 64.
            /// </summary>
            UnicodeString64, 

            /// <summary>
            /// The string 256.
            /// </summary>
            String256, 

            /// <summary>
            /// The unicode string 256.
            /// </summary>
            UnicodeString256, 

            /// <summary>
            /// The option.
            /// </summary>
            Option, 

            /// <summary>
            /// The index.
            /// </summary>
            Index, 

            /// <summary>
            /// The text box.
            /// </summary>
            TextBox
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The ent output.
        /// </summary>
        /// <param name="pluginNameAndPath">The plugin name and path.</param>
        /// <param name="reWritePlugin">The re write plugin.</param>
        /// <remarks></remarks>
        public void EntOutput(string pluginNameAndPath, bool reWritePlugin)
        {
            if (reWritePlugin)
            {
                pluginNameAndPath = FilePath;
            }

            XmlTextWriter xtw = new XmlTextWriter(pluginNameAndPath, Encoding.Default);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("plugin");
            xtw.WriteAttributeString("class", this.classType);
            xtw.WriteAttributeString("author", this.author);
            xtw.WriteAttributeString("version", this.version);
            xtw.WriteAttributeString("headersize", this.headerSize.ToString());
            for (int counter = 0; counter < this.revisions.Length; counter += 3)
            {
                xtw.WriteStartElement("revision");
                xtw.WriteAttributeString("author", this.revisions[counter]);
                xtw.WriteAttributeString("version", this.revisions[counter + 1]);
                xtw.WriteString(this.revisions[counter + 2]);
                xtw.WriteEndElement();
            }

            EntWriter(xtw, 0);
            xtw.WriteEndElement();
            xtw.Close();
        }

        /// <summary>
        /// The ifp output.
        /// </summary>
        /// <param name="pluginNameAndPath">The plugin name and path.</param>
        /// <remarks></remarks>
        public void IFPOutput(string pluginNameAndPath)
        {
            XmlTextWriter xtw = new XmlTextWriter(pluginNameAndPath, Encoding.Default);

            // try
            // {
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartElement("plugin");
            xtw.WriteAttributeString("class", this.classType);
            xtw.WriteAttributeString("author", this.author);
            xtw.WriteAttributeString("version", this.version);
            xtw.WriteAttributeString("headersize", this.headerSize.ToString());
            for (int counter = 0; counter < this.revisions.Length; counter += 3)
            {
                xtw.WriteStartElement("revision");
                xtw.WriteAttributeString("author", this.revisions[counter]);
                xtw.WriteAttributeString("version", this.revisions[counter + 1]);
                xtw.WriteString(this.revisions[counter + 2]);
                xtw.WriteEndElement();
            }

            IFPWriter(xtw, this.items);
            xtw.WriteEndElement();

            // }
            // catch
            // {
            // MessageBox.Show("Error in writing IFP");
            // }
            // finally
            // {
            xtw.Close();

            // }
        }

        // What is this listing??

        /// <summary>
        /// The read ifp.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <remarks></remarks>
        public void ReadIFP(string inputFilePath)
        {
            FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
            XmlTextReader xtr = new XmlTextReader(fs);
            try
            {
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                items = IFPReader(ref xtr);
                alRevisions.TrimToSize();
                revisions = (string[])alRevisions.ToArray(typeof(string));
                alRevisions.Clear();
                headerSize = size;
            }
            finally
            {
                xtr.Close();
                fs.Close();
            }

            this.newReadIFP(inputFilePath);
        }

        /// <summary>
        /// The new read ifp.
        /// </summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <remarks></remarks>
        public void newReadIFP(string inputFilePath)
        {
            FilePath = inputFilePath;
            ENTElements = new List<BaseObject>(0);
            FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
            XmlTextReader xtr = new XmlTextReader(fs);
            try
            {
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                NewIFPReader(ref xtr, -1, false);
                alRevisions.TrimToSize();
                revisions = (string[])alRevisions.ToArray(typeof(string));
                alRevisions.Clear();
                headerSize = size;
            }
            finally
            {
                xtr.Close();
                fs.Close();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ent writer.
        /// </summary>
        /// <param name="xtw">The xtw.</param>
        /// <param name="counter">The counter.</param>
        /// <remarks></remarks>
        private void EntWriter(XmlTextWriter xtw, int counter)
        {
            while (counter != -1)
            {
                switch (this.ENTElements[counter].GetType().ToString())
                {
                    case "Entity.IFPIO+Reflexive":
                        {
                            // <struct name="Skies11" offset="8" visible="true" size="8">
                            xtw.WriteStartElement("struct");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString().ToLower());
                            xtw.WriteAttributeString(
                                "size", ((Reflexive)this.ENTElements[counter]).chunkSize.ToString());
                            xtw.WriteAttributeString("label", ((Reflexive)this.ENTElements[counter]).label);
                            if (((Reflexive)this.ENTElements[counter]).HasCount == false)
                            {
                                xtw.WriteAttributeString(
                                    "HasCount", ((Reflexive)this.ENTElements[counter]).HasCount.ToString());
                            }

                            EntWriter(xtw, this.ENTElements[counter].child);
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+SID":
                        {
                            // <stringid name="Unknown" offset="52" visible="false" />
                            xtw.WriteStartElement("stringid");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+TagType":
                        {
                            xtw.WriteStartElement("tag");

                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", (this.ENTElements[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+Ident":
                        {
                            xtw.WriteStartElement("id");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", (this.ENTElements[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPString":
                        {
                            switch (((IFPString)this.ENTElements[counter]).type)
                            {
                                case true:
                                    {
                                        // <unicode64 name="English Name" />
                                        if (((IFPString)this.ENTElements[counter]).size == 64)
                                        {
                                            xtw.WriteStartElement("unicode64");
                                        }
                                        else
                                        {
                                            xtw.WriteStartElement("unicode256");
                                        }

                                        xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                                        xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                                        xtw.WriteAttributeString(
                                            "visible", this.ENTElements[counter].visible.ToString());
                                        xtw.WriteEndElement();
                                        break;
                                    }

                                case false:
                                    {
                                        if (((IFPString)this.ENTElements[counter]).size == 32)
                                        {
                                            xtw.WriteStartElement("string32");
                                        }
                                        else
                                        {
                                            xtw.WriteStartElement("string256");
                                        }

                                        xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                                        xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                                        xtw.WriteAttributeString(
                                            "visible", this.ENTElements[counter].visible.ToString());
                                        xtw.WriteEndElement();
                                        break;
                                    }
                            }

                            break;
                        }

                    case "Entity.IFPIO+Unused":
                        {
                            xtw.WriteStartElement("unused");
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("size", ((Unused)this.ENTElements[counter]).size.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+Unknown":
                        {
                            xtw.WriteStartElement("undefined");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPFloat":
                        {
                            xtw.WriteStartElement("float");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPInt":
                        {
                            switch (ENTElements[counter].ObjectType)
                            {
                                case ObjectEnum.Int:
                                    {
                                        xtw.WriteStartElement("int");
                                        break;
                                    }

                                case ObjectEnum.UInt:
                                    {
                                        xtw.WriteStartElement("uint");
                                        break;
                                    }

                                case ObjectEnum.Short:
                                    {
                                        xtw.WriteStartElement("short");
                                        break;
                                    }

                                case ObjectEnum.UShort:
                                    {
                                        xtw.WriteStartElement("ushort");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            if (((IFPInt)this.ENTElements[counter]).entIndex.nulled == false)
                            {
                                xtw.WriteAttributeString(
                                    "reflexiveoffset", 
                                    ((IFPInt)this.ENTElements[counter]).entIndex.ReflexiveOffset.ToString());
                                xtw.WriteAttributeString(
                                    "reflexivesize", 
                                    ((IFPInt)this.ENTElements[counter]).entIndex.ReflexiveSize.ToString());
                                xtw.WriteAttributeString(
                                    "itemoffset", ((IFPInt)this.ENTElements[counter]).entIndex.ItemOffset.ToString());
                                xtw.WriteAttributeString(
                                    "itemtype", ((IFPInt)this.ENTElements[counter]).entIndex.ItemType);
                                xtw.WriteAttributeString(
                                    "layer", ((IFPInt)this.ENTElements[counter]).entIndex.reflexiveLayer);
                            }

                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPByte":
                        {
                            // "reflexiveoffset"), xtr.GetAttribute("reflexivesize"), xtr.GetAttribute("itemoffset"), xtr.GetAttribute("itemtype"), xtr.GetAttribute("layer")
                            xtw.WriteStartElement("byte");
                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            if (((IFPByte)this.ENTElements[counter]).entIndex.nulled == false)
                            {
                                xtw.WriteAttributeString(
                                    "reflexiveoffset", 
                                    ((IFPByte)this.ENTElements[counter]).entIndex.ReflexiveOffset.ToString());
                                xtw.WriteAttributeString(
                                    "reflexivesize", 
                                    ((IFPByte)this.ENTElements[counter]).entIndex.ReflexiveSize.ToString());
                                xtw.WriteAttributeString(
                                    "itemoffset", ((IFPByte)this.ENTElements[counter]).entIndex.ItemOffset.ToString());
                                xtw.WriteAttributeString(
                                    "itemtype", ((IFPByte)this.ENTElements[counter]).entIndex.ItemType);
                                xtw.WriteAttributeString(
                                    "layer", ((IFPByte)this.ENTElements[counter]).entIndex.reflexiveLayer);
                            }

                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                        // Added 6-9-06 Start
                    case "Entity.IFPIO+Bitmask":
                        {
                            switch (((Bitmask)this.ENTElements[counter]).bitmaskSize)
                            {
                                case 8:
                                    {
                                        xtw.WriteStartElement("bitmask8");
                                        break;
                                    }

                                case 16:
                                    {
                                        xtw.WriteStartElement("bitmask16");
                                        break;
                                    }

                                case 32:
                                    {
                                        xtw.WriteStartElement("bitmask32");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            if (((Bitmask)this.ENTElements[counter]).options != null)
                            {
                                for (int counter2 = 0;
                                     counter2 < ((Bitmask)this.ENTElements[counter]).options.Length;
                                     counter2++)
                                {
                                    xtw.WriteStartElement("option");
                                    xtw.WriteAttributeString(
                                        "name", ((Option)((Bitmask)this.ENTElements[counter]).options[counter2]).name);
                                    xtw.WriteAttributeString(
                                        "value", 
                                        ((Option)((Bitmask)this.ENTElements[counter]).options[counter2]).value.ToString(
                                            ));
                                    xtw.WriteEndElement();
                                }
                            }

                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPEnum":
                        {
                            switch (((IFPEnum)this.ENTElements[counter]).enumSize)
                            {
                                case 8:
                                    {
                                        xtw.WriteStartElement("enum8");
                                        break;
                                    }

                                case 16:
                                    {
                                        xtw.WriteStartElement("enum16");
                                        break;
                                    }

                                case 32:
                                    {
                                        xtw.WriteStartElement("enum32");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", this.ENTElements[counter].name);
                            xtw.WriteAttributeString("offset", this.ENTElements[counter].offset.ToString());
                            xtw.WriteAttributeString("visible", this.ENTElements[counter].visible.ToString());
                            if (((IFPEnum)this.ENTElements[counter]).options != null)
                            {
                                for (int counter2 = 0;
                                     counter2 < ((IFPEnum)this.ENTElements[counter]).options.Length;
                                     counter2++)
                                {
                                    xtw.WriteStartElement("option");
                                    xtw.WriteAttributeString(
                                        "name", ((Option)((IFPEnum)this.ENTElements[counter]).options[counter2]).name);
                                    xtw.WriteAttributeString(
                                        "value", 
                                        ((Option)((IFPEnum)this.ENTElements[counter]).options[counter2]).value.ToString(
                                            ));
                                    xtw.WriteEndElement();
                                }
                            }

                            xtw.WriteEndElement();
                            break;
                        }

                        // End
                }

                counter = this.ENTElements[counter].siblingNext;
            }
        }

        // function calls its self recursively
        /// <summary>
        /// The ifp reader.
        /// </summary>
        /// <param name="xtr">The xtr.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private object[] IFPReader(ref XmlTextReader xtr)
        {
            ArrayList IFPElements = new ArrayList(0);
            int offset = 0;
            bool visible;
            int dependencyOffset = -1;
            bool endElement = false;
            while (xtr.Read())
            {
                visible = xtr.GetAttribute("visible") != null
                              ? xtr.GetAttribute("visible").ToLower() == "false" ? false : true
                              : true;
                switch (xtr.NodeType.ToString())
                {
                    case "Element":
                        {
                            switch (xtr.Name.ToLower())
                            {
                                    // <plugin class="bipd" author="Iron_Forge" version="0.3">
                                case "plugin":
                                    {
                                        author = xtr.GetAttribute("author");
                                        version = xtr.GetAttribute("version");
                                        classType = xtr.GetAttribute("class");
                                        break;
                                    }

                                    // <revision author="Iron_Forge" version="0.3">Added some known values...</revision>
                                case "revision":
                                    {
                                        alRevisions.Add(xtr.GetAttribute("author"));
                                        alRevisions.Add(xtr.GetAttribute("version"));
                                        alRevisions.Add(xtr.ReadString());
                                        break;
                                    }

                                    // <struct name="Predicted Resources24" offset="32" visible="true" size="8">
                                case "struct":
                                    {
                                        bool temp = true;
                                        string i = xtr.GetAttribute("HasCount");
                                        if (i != null)
                                        {
                                            temp = bool.Parse(i);
                                        }

                                        IFPElements.Add(
                                            new Reflexive(
                                                xtr.LineNumber, 
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.GetAttribute("label"), 
                                                IFPReader(ref xtr), 
                                                size, 
                                                temp, 
                                                -1, 
                                                -1));
                                        if (temp == false)
                                        {
                                            offset += 4;
                                        }
                                        else
                                        {
                                            offset += 8;
                                        }

                                        break;
                                    }

                                    // <tag name="" visible="false"/>
                                case "block":
                                    {
                                        IFPElements.Add(
                                            new TagBlock(
                                                xtr.GetAttribute("name"), visible, offset, xtr.LineNumber, -1, -1));
                                        offset += 8;
                                        break;
                                    }

                                case "tag":
                                    {
                                        IFPElements.Add(
                                            new TagType(
                                                offset, visible, xtr.GetAttribute("name"), xtr.LineNumber, -1, -1));
                                        offset += 4;
                                        dependencyOffset = offset;
                                        break;
                                    }

                                    // <id name="" visible="false"/>
                                case "id":
                                    {
                                        bool isDependency = dependencyOffset == offset ? true : false;
                                        IFPElements.Add(
                                            new Ident(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                isDependency, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <stringid name="" visible="false" />
                                case "stringid":
                                    {
                                        IFPElements.Add(
                                            new SID(xtr.GetAttribute("name"), visible, offset, xtr.LineNumber, -1, -1));
                                        offset += 4;
                                        break;
                                    }

                                case "script":
                                    {
                                        goto case "stringid";
                                    }

                                    // <unused size="32" default="0" />
                                case "unused":
                                    {
                                        int sizeOfUnusedSpace = Convert.ToInt32(xtr.GetAttribute("size"));
                                        IFPElements.Add(new Unused(offset, sizeOfUnusedSpace, xtr.LineNumber, -1, -1));
                                        offset += sizeOfUnusedSpace;
                                        break;
                                    }

                                    // <undefined name="Unknown" visible="false" />
                                case "undefined":
                                    {
                                        IFPElements.Add(
                                            new Unknown(
                                                offset, visible, xtr.GetAttribute("name"), xtr.LineNumber, -1, -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <float name="" visible="false" />
                                case "float":
                                    {
                                        IFPElements.Add(
                                            new IFPFloat(
                                                offset, visible, xtr.GetAttribute("name"), xtr.LineNumber, -1, -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <int name="Unknown" visible="false" />
                                case "int":
                                    {
                                        IFPElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.Int, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "int"), 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <uint name="Structure BSP Offset" />
                                case "uint":
                                    {
                                        IFPElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.UInt, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "uint"), 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <short name="Palette Chunk #" visible="true" />
                                case "short":
                                    {
                                        IFPElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.Short, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "short"), 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 2;
                                        break;
                                    }

                                    // <ushort name="Palette Chunk #" visible="true" />
                                case "ushort":
                                    {
                                        IFPElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.UShort, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "ushort"), 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 2;
                                        break;
                                    }

                                    // <byte name="Unknown" visible="false" />
                                case "byte":
                                    {
                                        IFPElements.Add(
                                            new IFPByte(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "byte"), 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 1;
                                        break;
                                    }

                                    // <bitmask name="Spawns in (none means all)">
                                case "bitmask":
                                    {
                                        goto case "bitmask32";
                                    }

                                    // <bitmask32 name="Spawns in (none means all)">
                                    // added 6-9-06 start
                                case "bitmask32":
                                    {
                                        IFPElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                32, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <bitmask16 name="Spawns in (none means all)">
                                case "bitmask16":
                                    {
                                        IFPElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                16, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 2;
                                        break;
                                    }

                                case "bitmask8":
                                    {
                                        IFPElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                8, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 1;
                                        break;
                                    }

                                    // <enum name="Only Use Once">
                                case "enum":
                                    {
                                        goto case "enum32";
                                    }

                                    // <enum32 name="Only Use Once">
                                case "enum32":
                                    {
                                        IFPElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                32, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 4;
                                        break;
                                    }

                                    // <enum16 name="Only Use Once">
                                case "enum16":
                                    {
                                        IFPElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                16, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 2;
                                        break;
                                    }

                                case "enum8":
                                    {
                                        IFPElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                8, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 1;
                                        break;
                                    }

                                    // end
                                    // <string name="Tag Name" or whatever size you wish />
                                case "string":
                                    {
                                        int sizeOfString = Convert.ToInt32(xtr.GetAttribute("size"));
                                        if (sizeOfString == 0)
                                        {
                                            // Default value of 256
                                            sizeOfString = 256;
                                        }

                                        IFPElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                sizeOfString, 
                                                false, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += sizeOfString;
                                        break;
                                    }

                                    // <string32 name="Profile Name" />
                                case "string32":
                                    {
                                        IFPElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                32, 
                                                false, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 32;
                                        break;
                                    }

                                    // <string256 name="Scenario Path" />
                                case "string256":
                                    {
                                        IFPElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                256, 
                                                false, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 256;
                                        break;
                                    }

                                    // <unicode64 name="English Name" />
                                case "unicode64":
                                    {
                                        IFPElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                64, 
                                                true, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 64;
                                        break;
                                    }

                                    // <unicode256 name="English Name" />
                                case "unicode256":
                                    {
                                        IFPElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                256, 
                                                true, 
                                                xtr.LineNumber, 
                                                -1, 
                                                -1));
                                        offset += 256;
                                        break;
                                    }

                                default:
                                    MessageBox.Show(
                                        "Unknown class: \"" + xtr.Name + "\"\n Name: " + xtr.GetAttribute("name") +
                                        "\n Offset: " + offset, 
                                        classType + ".ent");
                                    break;
                            }

                            break;
                        }

                    case "EndElement":
                        {
                            switch (xtr.Name)
                            {
                                case "struct":
                                    {
                                        endElement = true;
                                        break;
                                    }

                                case "plugin":
                                    {
                                        goto case "struct";
                                    }
                            }

                            break;
                        }
                }

                if (endElement)
                {
                    break;
                }
            }

            size = offset;
            IFPElements.TrimToSize();
            object[] tempobjectarray = IFPElements.ToArray();
            return tempobjectarray;
        }

        /// <summary>
        /// The ifp writer.
        /// </summary>
        /// <param name="xtw">The xtw.</param>
        /// <param name="itemsToWrite">The items to write.</param>
        /// <remarks></remarks>
        private void IFPWriter(XmlTextWriter xtw, object[] itemsToWrite)
        {
            for (int counter = 0; counter < itemsToWrite.Length; counter++)
            {
                string x = itemsToWrite[counter].GetType().ToString();
                switch (itemsToWrite[counter].GetType().ToString())
                {
                    case "Entity.IFPIO+Reflexive":
                        {
                            // <struct name="Skies11" offset="8" visible="true" size="8">
                            xtw.WriteStartElement("struct");
                            xtw.WriteAttributeString("name", ((Reflexive)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((Reflexive)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString(
                                "visible", ((Reflexive)itemsToWrite[counter]).visible.ToString().ToLower());
                            xtw.WriteAttributeString("size", ((Reflexive)itemsToWrite[counter]).chunkSize.ToString());
                            xtw.WriteAttributeString("label", ((Reflexive)itemsToWrite[counter]).label);
                            if (((Reflexive)itemsToWrite[counter]).HasCount == false)
                            {
                                xtw.WriteAttributeString(
                                    "HasCount", ((Reflexive)itemsToWrite[counter]).HasCount.ToString());
                            }

                            IFPWriter(xtw, ((Reflexive)itemsToWrite[counter]).items);
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+SID":
                        {
                            // <stringid name="Unknown" offset="52" visible="false" />
                            xtw.WriteStartElement("stringid");
                            xtw.WriteAttributeString("name", ((SID)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((SID)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((SID)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+TagType":
                        {
                            xtw.WriteStartElement("tag");

                            xtw.WriteAttributeString("name", ((TagType)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((TagType)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((TagType)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+Ident":
                        {
                            xtw.WriteStartElement("id");
                            xtw.WriteAttributeString("name", ((Ident)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((Ident)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((Ident)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPString":
                        {
                            switch (((IFPString)itemsToWrite[counter]).type)
                            {
                                case true:
                                    {
                                        // <unicode64 name="English Name" />
                                        if (((IFPString)itemsToWrite[counter]).size == 64)
                                        {
                                            xtw.WriteStartElement("unicode64");
                                        }
                                        else
                                        {
                                            xtw.WriteStartElement("unicode256");
                                        }

                                        xtw.WriteAttributeString("name", ((IFPString)itemsToWrite[counter]).name);
                                        xtw.WriteAttributeString(
                                            "offset", ((IFPString)itemsToWrite[counter]).offset.ToString());
                                        xtw.WriteAttributeString(
                                            "visible", ((IFPString)itemsToWrite[counter]).visible.ToString());
                                        xtw.WriteEndElement();
                                        break;
                                    }

                                case false:
                                    {
                                        if (((IFPString)itemsToWrite[counter]).size == 32)
                                        {
                                            xtw.WriteStartElement("string32");
                                        }
                                        else
                                        {
                                            xtw.WriteStartElement("string256");
                                        }

                                        xtw.WriteAttributeString("name", ((IFPString)itemsToWrite[counter]).name);
                                        xtw.WriteAttributeString(
                                            "offset", ((IFPString)itemsToWrite[counter]).offset.ToString());
                                        xtw.WriteAttributeString(
                                            "visible", ((IFPString)itemsToWrite[counter]).visible.ToString());
                                        xtw.WriteEndElement();
                                        break;
                                    }
                            }

                            break;
                        }

                    case "Entity.IFPIO+Unused":
                        {
                            xtw.WriteStartElement("unused");
                            xtw.WriteAttributeString("offset", ((Unused)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("size", ((Unused)itemsToWrite[counter]).size.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+Unknown":
                        {
                            xtw.WriteStartElement("undefined");
                            xtw.WriteAttributeString("name", ((Unknown)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((Unknown)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((Unknown)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPFloat":
                        {
                            xtw.WriteStartElement("float");
                            xtw.WriteAttributeString("name", ((IFPFloat)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((IFPFloat)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((IFPFloat)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPInt":
                        {
                            switch (((IFPInt)itemsToWrite[counter]).ObjectType)
                            {
                                case ObjectEnum.Int:
                                    {
                                        xtw.WriteStartElement("int");
                                        break;
                                    }

                                case ObjectEnum.UInt:
                                    {
                                        xtw.WriteStartElement("uint");
                                        break;
                                    }

                                case ObjectEnum.Short:
                                    {
                                        xtw.WriteStartElement("short");
                                        break;
                                    }

                                case ObjectEnum.UShort:
                                    {
                                        xtw.WriteStartElement("ushort");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", ((IFPInt)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((IFPInt)itemsToWrite[counter]).offset.ToString());
                            if (((IFPInt)itemsToWrite[counter]).entIndex.nulled == false)
                            {
                                xtw.WriteAttributeString(
                                    "reflexiveoffset", 
                                    ((IFPInt)itemsToWrite[counter]).entIndex.ReflexiveOffset.ToString());
                                xtw.WriteAttributeString(
                                    "reflexivesize", ((IFPInt)itemsToWrite[counter]).entIndex.ReflexiveSize.ToString());
                                xtw.WriteAttributeString(
                                    "itemoffset", ((IFPInt)itemsToWrite[counter]).entIndex.ItemOffset.ToString());
                                xtw.WriteAttributeString("itemtype", ((IFPInt)itemsToWrite[counter]).entIndex.ItemType);
                                xtw.WriteAttributeString(
                                    "layer", ((IFPInt)itemsToWrite[counter]).entIndex.reflexiveLayer);
                            }

                            xtw.WriteAttributeString("visible", ((IFPInt)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPByte":
                        {
                            // "reflexiveoffset"), xtr.GetAttribute("reflexivesize"), xtr.GetAttribute("itemoffset"), xtr.GetAttribute("itemtype"), xtr.GetAttribute("layer")
                            xtw.WriteStartElement("byte");
                            xtw.WriteAttributeString("name", ((IFPByte)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((IFPByte)itemsToWrite[counter]).offset.ToString());
                            if (((IFPByte)itemsToWrite[counter]).entIndex.nulled == false)
                            {
                                xtw.WriteAttributeString(
                                    "reflexiveoffset", 
                                    ((IFPByte)itemsToWrite[counter]).entIndex.ReflexiveOffset.ToString());
                                xtw.WriteAttributeString(
                                    "reflexivesize", ((IFPByte)itemsToWrite[counter]).entIndex.ReflexiveSize.ToString());
                                xtw.WriteAttributeString(
                                    "itemoffset", ((IFPByte)itemsToWrite[counter]).entIndex.ItemOffset.ToString());
                                xtw.WriteAttributeString("itemtype", ((IFPByte)itemsToWrite[counter]).entIndex.ItemType);
                                xtw.WriteAttributeString(
                                    "layer", ((IFPByte)itemsToWrite[counter]).entIndex.reflexiveLayer);
                            }

                            xtw.WriteAttributeString("visible", ((IFPByte)itemsToWrite[counter]).visible.ToString());
                            xtw.WriteEndElement();
                            break;
                        }

                        // Added 6-9-06 Start
                    case "Entity.IFPIO+Bitmask":
                        {
                            switch (((Bitmask)itemsToWrite[counter]).bitmaskSize)
                            {
                                case 8:
                                    {
                                        xtw.WriteStartElement("bitmask8");
                                        break;
                                    }

                                case 16:
                                    {
                                        xtw.WriteStartElement("bitmask16");
                                        break;
                                    }

                                case 32:
                                    {
                                        xtw.WriteStartElement("bitmask32");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", ((Bitmask)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((Bitmask)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((Bitmask)itemsToWrite[counter]).visible.ToString());
                            if (((Bitmask)itemsToWrite[counter]).options != null)
                            {
                                for (int counter2 = 0;
                                     counter2 < ((Bitmask)itemsToWrite[counter]).options.Length;
                                     counter2++)
                                {
                                    xtw.WriteStartElement("option");
                                    xtw.WriteAttributeString(
                                        "name", ((Option)((Bitmask)itemsToWrite[counter]).options[counter2]).name);
                                    xtw.WriteAttributeString(
                                        "value", 
                                        ((Option)((Bitmask)itemsToWrite[counter]).options[counter2]).value.ToString());
                                    xtw.WriteEndElement();
                                }
                            }

                            xtw.WriteEndElement();
                            break;
                        }

                    case "Entity.IFPIO+IFPEnum":
                        {
                            switch (((IFPEnum)itemsToWrite[counter]).enumSize)
                            {
                                case 8:
                                    {
                                        xtw.WriteStartElement("enum8");
                                        break;
                                    }

                                case 16:
                                    {
                                        xtw.WriteStartElement("enum16");
                                        break;
                                    }

                                case 32:
                                    {
                                        xtw.WriteStartElement("enum32");
                                        break;
                                    }
                            }

                            xtw.WriteAttributeString("name", ((IFPEnum)itemsToWrite[counter]).name);
                            xtw.WriteAttributeString("offset", ((IFPEnum)itemsToWrite[counter]).offset.ToString());
                            xtw.WriteAttributeString("visible", ((IFPEnum)itemsToWrite[counter]).visible.ToString());
                            if (((IFPEnum)itemsToWrite[counter]).options != null)
                            {
                                for (int counter2 = 0;
                                     counter2 < ((IFPEnum)itemsToWrite[counter]).options.Length;
                                     counter2++)
                                {
                                    xtw.WriteStartElement("option");
                                    xtw.WriteAttributeString(
                                        "name", ((Option)((IFPEnum)itemsToWrite[counter]).options[counter2]).name);
                                    xtw.WriteAttributeString(
                                        "value", 
                                        ((Option)((IFPEnum)itemsToWrite[counter]).options[counter2]).value.ToString());
                                    xtw.WriteEndElement();
                                }
                            }

                            xtw.WriteEndElement();
                            break;
                        }

                        // End
                }
            }
        }

        // function calls its self recursively
        /// <summary>
        /// The new ifp reader.
        /// </summary>
        /// <param name="xtr">The xtr.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="nested">The nested.</param>
        /// <remarks></remarks>
        private void NewIFPReader(ref XmlTextReader xtr, int parent, bool nested)
        {
            int offset = 0;
            bool visible;
            int dependencyOffset = -1;
            bool endElement = false;
            int sibling = -1;
            int tempsibint = 0;
            while (xtr.Read())
            {
                visible = xtr.GetAttribute("visible") != null
                              ? xtr.GetAttribute("visible").ToLower() == "false" ? false : true
                              : true;
                tempsibint = ENTElements.Count;
                switch (xtr.NodeType.ToString())
                {
                    case "Element":
                        {
                            if (sibling > -1 && ENTElements.Count > 0)
                            {
                                ENTElements[sibling].siblingNext = tempsibint;
                            }

                            switch (xtr.Name.ToLower())
                            {
                                    // <plugin class="bipd" author="Iron_Forge" version="0.3">
                                case "plugin":
                                    {
                                        author = xtr.GetAttribute("author");
                                        version = xtr.GetAttribute("version");
                                        classType = xtr.GetAttribute("class");
                                        break;
                                    }

                                    // <revision author="Iron_Forge" version="0.3">Added some known values...</revision>
                                case "revision":
                                    {
                                        alRevisions.Add(xtr.GetAttribute("author"));
                                        alRevisions.Add(xtr.GetAttribute("version"));
                                        alRevisions.Add(xtr.ReadString());
                                        break;
                                    }

                                    // <struct name="Predicted Resources24" offset="32" visible="true" size="8">
                                case "struct":
                                    {
                                        bool temp = true;
                                        string i = xtr.GetAttribute("HasCount");
                                        if (i != null)
                                        {
                                            temp = bool.Parse(i);
                                        }

                                        ENTElements.Add(
                                            new Reflexive(
                                                xtr.LineNumber, 
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.GetAttribute("label"), 
                                                null, 
                                                size, 
                                                temp, 
                                                parent, 
                                                sibling));
                                        NewIFPReader(ref xtr, tempsibint, true);
                                        if (tempsibint != this.ENTElements.Count - 1)
                                        {
                                            this.ENTElements[this.ENTElements.Count - 1].siblingNext = -1;
                                            ENTElements[tempsibint].child = tempsibint + 1;
                                            ((Reflexive)ENTElements[tempsibint]).chunkSize = size;
                                        }

                                        if (temp == false)
                                        {
                                            offset += 4;
                                        }
                                        else
                                        {
                                            offset += 8;
                                        }

                                        break;
                                    }

                                    // <tag name="" visible="false"/>
                                case "block":
                                    {
                                        ENTElements.Add(
                                            new TagBlock(
                                                xtr.GetAttribute("name"), visible, offset, xtr.LineNumber, -1, -1));
                                        offset += 8;
                                        break;
                                    }

                                case "tag":
                                    {
                                        ENTElements.Add(
                                            new TagType(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        dependencyOffset = offset;
                                        break;
                                    }

                                    // <id name="" visible="false"/>
                                case "id":
                                    {
                                        bool isDependency = dependencyOffset == offset ? true : false;
                                        ENTElements.Add(
                                            new Ident(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                isDependency, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <stringid name="" visible="false" />
                                case "stringid":
                                    {
                                        ENTElements.Add(
                                            new SID(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                case "script":
                                    {
                                        goto case "stringid";
                                    }

                                    // <unused size="32" default="0" />
                                case "unused":
                                    {
                                        int sizeOfUnusedSpace = Convert.ToInt32(xtr.GetAttribute("size"));
                                        ENTElements.Add(
                                            new Unused(offset, sizeOfUnusedSpace, xtr.LineNumber, parent, sibling));
                                        offset += sizeOfUnusedSpace;
                                        break;
                                    }

                                    // <undefined name="Unknown" visible="false" />
                                case "undefined":
                                    {
                                        ENTElements.Add(
                                            new Unknown(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <float name="" visible="false" />
                                case "float":
                                    {
                                        ENTElements.Add(
                                            new IFPFloat(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <int name="Unknown" visible="false" />
                                case "int":
                                    {
                                        ENTElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.Int, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "int"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <uint name="Structure BSP Offset" />
                                case "uint":
                                    {
                                        ENTElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.UInt, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "uint"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <short name="Palette Chunk #" visible="true" />
                                case "short":
                                    {
                                        ENTElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.Short, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "short"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 2;
                                        break;
                                    }

                                    // <ushort name="Palette Chunk #" visible="true" />
                                case "ushort":
                                    {
                                        ENTElements.Add(
                                            new IFPInt(
                                                offset, 
                                                ObjectEnum.UShort, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "ushort"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 2;
                                        break;
                                    }

                                    // <byte name="Unknown" visible="false" />
                                case "byte":
                                    {
                                        ENTElements.Add(
                                            new IFPByte(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                makeIndex(ref xtr, "byte"), 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 1;
                                        break;
                                    }

                                    // <bitmask name="Spawns in (none means all)">
                                case "bitmask":
                                    {
                                        goto case "bitmask32";
                                    }

                                    // <bitmask32 name="Spawns in (none means all)">
                                    // added 6-9-06 start
                                case "bitmask32":
                                    {
                                        ENTElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                32, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <bitmask16 name="Spawns in (none means all)">
                                case "bitmask16":
                                    {
                                        ENTElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                16, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 2;
                                        break;
                                    }

                                case "bitmask8":
                                    {
                                        ENTElements.Add(
                                            new Bitmask(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                8, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 1;
                                        break;
                                    }

                                    // <enum name="Only Use Once">
                                case "enum":
                                    {
                                        goto case "enum32";
                                    }

                                    // <enum32 name="Only Use Once">
                                case "enum32":
                                    {
                                        ENTElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                32, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                    // <enum16 name="Only Use Once">
                                case "enum16":
                                    {
                                        ENTElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                16, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 2;
                                        break;
                                    }

                                case "enum8":
                                    {
                                        ENTElements.Add(
                                            new IFPEnum(
                                                offset, 
                                                visible, 
                                                xtr.GetAttribute("name"), 
                                                xtr.IsEmptyElement == false ? Options(ref xtr) : null, 
                                                8, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 1;
                                        break;
                                    }

                                    // end
                                    // <string32 name="Tag Name" />
                                case "string":
                                    {
                                        int sizeOfString = Convert.ToInt32(xtr.GetAttribute("size"));
                                        if (sizeOfString == 0)
                                        {
                                            // Default size of 256
                                            sizeOfString = 256;
                                        }

                                        ENTElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                sizeOfString, 
                                                false, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += sizeOfString;
                                        break;
                                    }

                                    // <string32 name="Profile Name" />
                                case "string32":
                                    {
                                        ENTElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                32, 
                                                false, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 32;
                                        break;
                                    }

                                    // <string256 name="Scenario Path" />
                                case "string256":
                                    {
                                        ENTElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                256, 
                                                false, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 256;
                                        break;
                                    }

                                    // <unicode64 name="English Name" />
                                case "unicode64":
                                    {
                                        ENTElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                64, 
                                                true, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 64;
                                        break;
                                    }

                                    // <unicode256 name="English Name" />
                                case "unicode256":
                                    {
                                        ENTElements.Add(
                                            new IFPString(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                256, 
                                                true, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 256;
                                        break;
                                    }

                                case "textbox":
                                    {
                                        ENTElements.Add(
                                            new IFPTextBox(
                                                xtr.GetAttribute("name"), 
                                                visible, 
                                                offset, 
                                                xtr.LineNumber, 
                                                parent, 
                                                sibling));
                                        offset += 4;
                                        break;
                                    }

                                default:
                                    MessageBox.Show(
                                        "There is an unknown tag type in the plugin: \n" // + "\"" + filePath + "\"\n"
                                        + "\"" + xtr.Name.ToUpper() + "\" @ " + "line " + xtr.LineNumber + ", " + "pos " +
                                        xtr.LinePosition + "\n");
                                    break;
                            }

                            break;
                        }

                    case "EndElement":
                        {
                            switch (xtr.Name)
                            {
                                case "plugin":
                                case "struct":
                                    {
                                        endElement = true;
                                        break;
                                    }
                            }

                            break;
                        }
                }

                // If (Count < 1) sibling = -1 ELSE sibling = tempsibint;
                sibling = ENTElements.Count < 1 ? -1 : tempsibint;
                if (endElement)
                {
                    break;
                }
            }

            size = offset;
            ENTElements.TrimExcess();
        }

        /// <summary>
        /// The options.
        /// </summary>
        /// <param name="xtr">The xtr.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private object[] Options(ref XmlTextReader xtr)
        {
            ArrayList options = new ArrayList(0);
            bool endElement = false;
            while (xtr.Read())
            {
                switch (xtr.NodeType.ToString())
                {
                    case "Element":
                        {
                            switch (xtr.Name.ToLower())
                            {
                                    // <option value="1" name="CTF" />
                                case "option":
                                    {
                                        options.Add(
                                            new Option(
                                                xtr.GetAttribute("name"), xtr.GetAttribute("value"), xtr.LineNumber));
                                        break;
                                    }
                            }

                            break;
                        }

                    case "EndElement":
                        {
                            switch (xtr.Name)
                            {
                                case "struct":
                                    {
                                        endElement = true;
                                        break;
                                    }

                                case "bitmask":
                                    {
                                        goto case "struct";
                                    }

                                case "bitmask32":
                                    {
                                        goto case "struct";
                                    }

                                case "bitmask16":
                                    {
                                        goto case "struct";
                                    }

                                case "bitmask8":
                                    {
                                        goto case "struct";
                                    }

                                case "enum8":
                                    {
                                        goto case "struct";
                                    }

                                case "enum32":
                                    {
                                        goto case "struct";
                                    }

                                case "enum16":
                                    {
                                        goto case "struct";
                                    }

                                case "enum":
                                    {
                                        goto case "struct";
                                    }

                                case "plugin":
                                    {
                                        goto case "struct";
                                    }
                            }

                            break;
                        }
                }

                if (endElement)
                {
                    break;
                }
            }

            object[] temp = options.ToArray();
            return temp;
        }

        /// <summary>
        /// The make index.
        /// </summary>
        /// <param name="xtr">The xtr.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private Index makeIndex(ref XmlTextReader xtr, string type)
        {
            return new Index(
                xtr.GetAttribute("reflexiveoffset"), 
                xtr.GetAttribute("reflexivesize"), 
                xtr.GetAttribute("itemoffset"), 
                xtr.GetAttribute("itemtype"), 
                xtr.GetAttribute("layer"), 
                xtr.LineNumber);
        }

        #endregion

        /// <summary>
        /// The base object.
        /// </summary>
        /// <remarks></remarks>
        public class BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The object type.
            /// </summary>
            public ObjectEnum ObjectType;

            /// <summary>
            /// The child.
            /// </summary>
            public int child = -1;

            /// <summary>
            /// The line number.
            /// </summary>
            public int lineNumber;

            /// <summary>
            /// The name.
            /// </summary>
            public string name;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            /// <summary>
            /// The parent.
            /// </summary>
            public int parent = -1;

            /// <summary>
            /// The sibling next.
            /// </summary>
            public int siblingNext = -1;

            /// <summary>
            /// The sibling previous.
            /// </summary>
            public int siblingPrevious = -1;

            /// <summary>
            /// The visible.
            /// </summary>
            public bool visible;

            #endregion
        }

        // added 6-8-06
        // Start

        /// <summary>
        /// The bitmask.
        /// </summary>
        /// <remarks></remarks>
        public class Bitmask : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The bitmask size.
            /// </summary>
            public int bitmaskSize;

            /// <summary>
            /// The options.
            /// </summary>
            public object[] options;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Bitmask"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="ifpoptions">The ifpoptions.</param>
            /// <param name="ifpsize">The ifpsize.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public Bitmask(
                int ifpoffset, 
                bool ifpvisible, 
                string ifpname, 
                object[] ifpoptions, 
                int ifpsize, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                switch (ifpsize)
                {
                    case 8:
                        this.ObjectType = ObjectEnum.Byte_Flags;
                        break;
                    case 16:
                        this.ObjectType = ObjectEnum.Word_Flags;
                        break;
                    case 32:
                        this.ObjectType = ObjectEnum.Long_Flags;
                        break;
                }

                this.offset = ifpoffset;
                this.name = ifpname;
                this.options = ifpoptions;
                this.visible = ifpvisible;
                this.bitmaskSize = ifpsize;
            }

            #endregion
        }

        /// <summary>
        /// The ifp byte.
        /// </summary>
        /// <remarks></remarks>
        public class IFPByte : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The ent index.
            /// </summary>
            public Index entIndex;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPByte"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="iIndex">Index of the i.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPByte(
                int ifpoffset, 
                bool ifpvisible, 
                string ifpname, 
                Index iIndex, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.entIndex = iIndex;
                this.ObjectType = ObjectEnum.Byte;
                this.offset = ifpoffset;
                this.visible = ifpvisible;
                this.name = ifpname;
            }

            #endregion
        }

        /// <summary>
        /// The ifp enum.
        /// </summary>
        /// <remarks></remarks>
        public class IFPEnum : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The enum size.
            /// </summary>
            public int enumSize;

            /// <summary>
            /// The options.
            /// </summary>
            public object[] options;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPEnum"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="ifpoptions">The ifpoptions.</param>
            /// <param name="ifpsize">The ifpsize.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPEnum(
                int ifpoffset, 
                bool ifpvisible, 
                string ifpname, 
                object[] ifpoptions, 
                int ifpsize, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                switch (ifpsize)
                {
                    case 8:
                        this.ObjectType = ObjectEnum.Char_Enum;
                        break;
                    case 16:
                        this.ObjectType = ObjectEnum.Enum;
                        break;
                    case 32:
                        this.ObjectType = ObjectEnum.Long_Enum;
                        break;
                }

                this.offset = ifpoffset;
                this.name = ifpname;
                this.options = ifpoptions;
                this.visible = ifpvisible;
                this.enumSize = ifpsize;
            }

            #endregion
        }

        /// <summary>
        /// The ifp float.
        /// </summary>
        /// <remarks></remarks>
        public class IFPFloat : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPFloat"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPFloat(
                int ifpoffset, bool ifpvisible, string ifpname, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Float;
                this.offset = ifpoffset;
                this.visible = ifpvisible;
                this.name = ifpname;
            }

            #endregion
        }

        /// <summary>
        /// The ifp int.
        /// </summary>
        /// <remarks></remarks>
        public class IFPInt : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The ent index.
            /// </summary>
            public Index entIndex;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPInt"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="iType">Type of the i.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="iIndex">Index of the i.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPInt(
                int ifpoffset, 
                ObjectEnum iType, 
                bool ifpvisible, 
                string ifpname, 
                Index iIndex, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.entIndex = iIndex;
                this.ObjectType = iType;
                this.offset = ifpoffset;
                this.visible = ifpvisible;
                this.name = ifpname;
            }

            #endregion
        }

        /// <summary>
        /// The ifp string.
        /// </summary>
        /// <remarks></remarks>
        public class IFPString : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The size.
            /// </summary>
            public int size;

            /// <summary>
            /// The type.
            /// </summary>
            public bool type; // true means unicode, false means normal string

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPString"/> class.
            /// </summary>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="ifpSize">Size of the ifp.</param>
            /// <param name="ifpType">if set to <c>true</c> [ifp type].</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPString(
                string ifpName, 
                bool ifpVisible, 
                int ifpOffset, 
                int ifpSize, 
                bool ifpType, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                switch (ifpType)
                {
                    case true:
                        {
                            // <unicode64 name="English Name" />
                            if (ifpSize == 64)
                            {
                                this.ObjectType = ObjectEnum.UnicodeString64;
                            }
                            else if (ifpSize == 256)
                            {
                                this.ObjectType = ObjectEnum.UnicodeString256;
                            }
                            else
                            {
                                this.ObjectType = ObjectEnum.String;
                            }

                            break;
                        }

                    case false:
                        {
                            if (ifpSize == 32)
                            {
                                this.ObjectType = ObjectEnum.String32;
                            }
                            else if (ifpSize == 256)
                            {
                                this.ObjectType = ObjectEnum.String256;
                            }
                            else
                            {
                                this.ObjectType = ObjectEnum.String;
                            }

                            break;
                        }
                }

                this.offset = ifpOffset;
                this.name = ifpName;
                this.visible = ifpVisible;
                this.size = ifpSize;
                this.type = ifpType;
            }

            #endregion
        }

        /// <summary>
        /// The ifp text box.
        /// </summary>
        /// <remarks></remarks>
        public class IFPTextBox : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="IFPTextBox"/> class.
            /// </summary>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public IFPTextBox(
                string ifpName, bool ifpVisible, int ifpOffset, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.TextBox;
                this.offset = ifpOffset;
                this.name = ifpName;
                this.visible = ifpVisible;
            }

            #endregion
        }

        /// <summary>
        /// The ident.
        /// </summary>
        /// <remarks></remarks>
        public class Ident : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The has tag type.
            /// </summary>
            public bool hasTagType;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Ident"/> class.
            /// </summary>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="ifpHasTagType">if set to <c>true</c> [ifp has tag type].</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public Ident(
                string ifpName, 
                bool ifpVisible, 
                int ifpOffset, 
                bool ifpHasTagType, 
                int entlineNumber, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Ident;
                this.offset = ifpOffset;
                this.name = ifpName;
                this.visible = ifpVisible;
                this.hasTagType = ifpHasTagType;
            }

            #endregion
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <remarks></remarks>
        public class Index
        {
            #region Constants and Fields

            /// <summary>
            /// The item offset.
            /// </summary>
            public int ItemOffset;

            /// <summary>
            /// The item type.
            /// </summary>
            public string ItemType;

            /// <summary>
            /// The reflexive offset.
            /// </summary>
            public int ReflexiveOffset;

            /// <summary>
            /// The reflexive size.
            /// </summary>
            public int ReflexiveSize;

            /// <summary>
            /// The line number.
            /// </summary>
            public int lineNumber;

            /// <summary>
            /// The nulled.
            /// </summary>
            public bool nulled = true;

            /// <summary>
            /// The reflexive layer.
            /// </summary>
            public string reflexiveLayer;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Index"/> class.
            /// </summary>
            /// <param name="ifpReflexiveOffset">The ifp reflexive offset.</param>
            /// <param name="ifpReflexiveSize">Size of the ifp reflexive.</param>
            /// <param name="ifpItemOffset">The ifp item offset.</param>
            /// <param name="ENTItemType">Type of the ENT item.</param>
            /// <param name="Layer">The layer.</param>
            /// <param name="linenumber">The linenumber.</param>
            /// <remarks></remarks>
            public Index(
                string ifpReflexiveOffset, 
                string ifpReflexiveSize, 
                string ifpItemOffset, 
                string ENTItemType, 
                string Layer, 
                int linenumber)
            {
                this.reflexiveLayer = Layer;
                lineNumber = linenumber;
                if (ifpReflexiveOffset == null | ifpItemOffset == null | ifpReflexiveSize == null)
                {
                    return;
                }

                if (ifpReflexiveOffset.Contains("0x") | ifpItemOffset.Contains("0x") | ifpReflexiveSize.Contains("0x"))
                {
                    MessageBox.Show(
                        "Please use Decimal numbers and not hex numbers in your indicies. You used hex on line " +
                        linenumber);
                    return;
                }

                this.nulled = false;
                try
                {
                    this.ReflexiveOffset = Convert.ToInt32(ifpReflexiveOffset);
                    this.ReflexiveSize = Convert.ToInt32(ifpReflexiveSize);
                    this.ItemOffset = Convert.ToInt32(ifpItemOffset);
                    this.ItemType = ENTItemType;
                }
                catch
                {
                    this.nulled = true;
                    return;
                }
            }

            #endregion
        }

        // End

        /// <summary>
        /// The option.
        /// </summary>
        /// <remarks></remarks>
        public class Option : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The value.
            /// </summary>
            public int value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Option"/> class.
            /// </summary>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="ifpvalue">The ifpvalue.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <remarks></remarks>
            public Option(string ifpname, string ifpvalue, int entlineNumber)
            {
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Option;
                this.name = ifpname;
                if (ifpvalue.Length >= 2)
                {
                    string tempx = ifpvalue.Substring(0, 2);
                    string tempx2 = ifpvalue.Substring(2, ifpvalue.Length - 2);
                    this.value = tempx != "0x" ? Convert.ToInt32(ifpvalue) : int.Parse(tempx2, NumberStyles.HexNumber);
                }
                else
                {
                    this.value = Convert.ToInt32(ifpvalue);
                }
            }

            #endregion
        }

        /// <summary>
        /// The reflexive.
        /// </summary>
        /// <remarks></remarks>
        public class Reflexive : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The has count.
            /// </summary>
            public bool HasCount = true;

            /// <summary>
            /// The chunk size.
            /// </summary>
            public int chunkSize;

            /// <summary>
            /// The items.
            /// </summary>
            public object[] items;

            /// <summary>
            /// The label.
            /// </summary>
            public string label;

            /// <summary>
            /// The total size.
            /// </summary>
            public int totalSize;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Reflexive"/> class.
            /// </summary>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpLabel">The ifp label.</param>
            /// <param name="ifpitems">The ifpitems.</param>
            /// <param name="ifpChunkSize">Size of the ifp chunk.</param>
            /// <param name="hascount">if set to <c>true</c> [hascount].</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public Reflexive(
                int entlineNumber, 
                int ifpOffset, 
                bool ifpVisible, 
                string ifpName, 
                string ifpLabel, 
                object[] ifpitems, 
                int ifpChunkSize, 
                bool hascount, 
                int entparent, 
                int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Struct;
                this.offset = ifpOffset;
                this.visible = ifpVisible;
                this.chunkSize = ifpChunkSize;
                this.name = ifpName;
                this.label = ifpLabel;
                this.items = ifpitems;
                this.HasCount = hascount;
            }

            #endregion
        }

        /// <summary>
        /// The sid.
        /// </summary>
        /// <remarks></remarks>
        public class SID : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SID"/> class.
            /// </summary>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public SID(
                string ifpName, bool ifpVisible, int ifpOffset, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.StringID;
                this.offset = ifpOffset;
                this.name = ifpName;
                this.visible = ifpVisible;
            }

            #endregion
        }

        /// <summary>
        /// The tag block.
        /// </summary>
        /// <remarks></remarks>
        public class TagBlock : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TagBlock"/> class.
            /// </summary>
            /// <param name="ifpName">Name of the ifp.</param>
            /// <param name="ifpVisible">if set to <c>true</c> [ifp visible].</param>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public TagBlock(
                string ifpName, bool ifpVisible, int ifpOffset, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Block;
                this.offset = ifpOffset;
                this.name = ifpName;
                this.visible = ifpVisible;
            }

            #endregion
        }

        /// <summary>
        /// The tag type.
        /// </summary>
        /// <remarks></remarks>
        public class TagType : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TagType"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public TagType(
                int ifpoffset, bool ifpvisible, string ifpname, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.TagType;
                this.offset = ifpoffset;
                this.visible = ifpvisible;
                this.name = ifpname;
            }

            #endregion
        }

        /// <summary>
        /// The unknown.
        /// </summary>
        /// <remarks></remarks>
        public class Unknown : BaseObject
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Unknown"/> class.
            /// </summary>
            /// <param name="ifpoffset">The ifpoffset.</param>
            /// <param name="ifpvisible">if set to <c>true</c> [ifpvisible].</param>
            /// <param name="ifpname">The ifpname.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public Unknown(
                int ifpoffset, bool ifpvisible, string ifpname, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Unknown;
                this.offset = ifpoffset;
                this.visible = ifpvisible;
                this.name = ifpname;
            }

            #endregion
        }

        /// <summary>
        /// The unused.
        /// </summary>
        /// <remarks></remarks>
        public class Unused : BaseObject
        {
            #region Constants and Fields

            /// <summary>
            /// The size.
            /// </summary>
            public int size;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Unused"/> class.
            /// </summary>
            /// <param name="ifpOffset">The ifp offset.</param>
            /// <param name="ifpSize">Size of the ifp.</param>
            /// <param name="entlineNumber">The entline number.</param>
            /// <param name="entparent">The entparent.</param>
            /// <param name="entPrevSibling">The ent prev sibling.</param>
            /// <remarks></remarks>
            public Unused(int ifpOffset, int ifpSize, int entlineNumber, int entparent, int entPrevSibling)
            {
                this.siblingPrevious = entPrevSibling;
                this.parent = entparent;
                this.lineNumber = entlineNumber;
                this.ObjectType = ObjectEnum.Unused;
                this.offset = ifpOffset;
                this.size = ifpSize;
            }

            #endregion
        }
    }
}