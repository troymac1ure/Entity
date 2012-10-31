// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaItemComparer.cs" company="">
//   
// </copyright>
// <summary>
//   The meta item comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.MetaFuncs
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using entity.MapForms;
    using System;
    using Globals;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;

    /// <summary>
    /// The meta item comparer.
    /// </summary>
    /// <remarks></remarks>
    internal class MetaItemComparer
    {
        #region Constants and Fields

        /// <summary>
        /// The ifp meta.
        /// </summary>
        private readonly Meta ifpMeta;

        /// <summary>
        /// The manual meta.
        /// </summary>
        private readonly Meta manualMeta;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaItemComparer"/> class.
        /// </summary>
        /// <param name="currentForm">The current form.</param>
        /// <remarks></remarks>
        public MetaItemComparer(MapForm currentForm)
        {
            Map map = currentForm.map;

            int counter = 0;
            for (counter = 0; counter < map.MapHeader.fileCount; counter++)
            {
                currentForm.SetProgressBar(counter * 100 / map.MapHeader.fileCount);
                
                ifpMeta = new Meta(map);
                manualMeta = new Meta(map);
                manualMeta.ReadMetaFromMap(counter, false);
                ifpMeta.ReadMetaFromMap(counter, false);

                // parse ifp and scan meta with it
                try
                {
                    IFPIO io = IFPHashMap.GetIfp(ifpMeta.type, map.HaloVersion);

                    ifpMeta.headersize = io.headerSize;
                    manualMeta.headersize = io.headerSize;
                    try
                    {
                        ifpMeta.scanner.ScanWithIFP(ref io);
                    }
                    catch (Exception ex)
                    {
                        Global.ShowErrorMsg("Broken IFP - " + ifpMeta.type, ex);
                    }

                    manualMeta.scanner.ScanManually();
                    check(map);
                }
                catch (Exception ex)
                {
                    Globals.Global.ShowErrorMsg(string.Empty, ex);
                }

            }

            currentForm.SetProgressBar(0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The fix tag type file name.
        /// </summary>
        /// <param name="temp">The temp.</param>
        /// <returns>The fix tag type file name.</returns>
        /// <remarks></remarks>
        private string FixTagTypeFileName(string temp)
        {
            temp = temp.Replace("<", "_").Replace(">", "_");
            return temp;
        }

        /// <summary>
        /// The write plugin error.
        /// </summary>
        /// <param name="iList">The i list.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        private void WritePluginError(List<Meta.Item> iList, Map map)
        {
            string difolder = Global.StartupPath+ "\\PluginTestResults\\" + FixTagTypeFileName(ifpMeta.type.Trim()) + "\\";
            DirectoryInfo di = new DirectoryInfo(difolder);
            if (di.Exists == false)
            {
                di.Create();
            }

            string fileName = difolder + map.FileNames.Name[manualMeta.TagIndex].Replace('\\', '-') + ".check";
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter swFromFileStream = new StreamWriter(fs);
            for (int counter = 0; counter < iList.Count; counter++)
            {
                switch (iList[counter].type)
                {
                    case Meta.ItemType.Reflexive:
                        {
                            swFromFileStream.WriteLine(
                                "Reflexive | " + " Map Offset : " +
                                iList[counter].mapOffset.ToString().PadLeft(10, '0') + " | Offset in Tag : " +
                                iList[counter].offset.ToString().PadLeft(10, '0'));
                            break;
                        }

                    case Meta.ItemType.Ident:
                        {
                            swFromFileStream.WriteLine(
                                "Ident | " + " Map Offset : " + iList[counter].mapOffset.ToString().PadLeft(10, '0') +
                                " | Offset in Tag : " + iList[counter].offset.ToString().PadLeft(10, '0') +
                                " | Ident Type" + ((Meta.Ident)iList[counter]).pointstotagtype + " | Ident Name " +
                                ((Meta.Ident)iList[counter]).pointstotagname);
                            break;
                        }

                    case Meta.ItemType.String:
                        {
                            swFromFileStream.WriteLine(
                                "Sid |" + " Map Offset : " + iList[counter].mapOffset.ToString().PadLeft(10, '0') +
                                " Offset In Tag : " + iList[counter].offset.ToString().PadLeft(10, '0') +
                                " | SID Name : " + ((Meta.String)iList[counter]).name);
                            break;
                        }
                }
            }

            swFromFileStream.Close();
            fs.Close();
        }

        /// <summary>
        /// The check.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        private void check(Map map)
        {
            List<Meta.Item> iList = new List<Meta.Item>(0);
            bool allOK = true;
            for (int counter = 0; counter < manualMeta.items.Count; counter++)
            {
                bool Ok = false;
                for (int counter2 = 0; counter2 < ifpMeta.items.Count; counter2++)
                {
                    if (manualMeta.items[counter].mapOffset == ifpMeta.items[counter2].mapOffset)
                    {
                        Ok = true;
                    }
                }

                if (Ok == false)
                {
                    iList.Add(manualMeta.items[counter]);
                    allOK = false;
                }
            }

            if (allOK == false)
            {
                WritePluginError(iList, map);
            }
        }

        #endregion
    }
}