using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HaloMap.RealTimeHalo;
using System.IO;

namespace entity.Main
{
    partial class Form1
    {
        #region Fields
        public enum MapType
        {
            SinglePlayerShared,
            Multiplayer,
            MainMenu,
            Shared
        }

        class MapHeader
        {
            bool mapLoaded;
            string head;
            uint version;
            uint fileSize;
            MapType mapType;
            string mapName;
            string scenarioName;
            uint signature;

            #region constructor
            public MapHeader(byte[] headerData)
            {
                if (headerData.Length != 2048)
                {
                    mapLoaded = false;
                    return;
                }
                try
                {
                    // The follwing contains any useful information about the loaded map
                    Array.Reverse(headerData, 0, 4);
                    head = ASCIIEncoding.ASCII.GetString(headerData, 0, 4);
                    if (head != "head")
                    {
                        mapLoaded = false;
                        return;
                    }

                    version = BitConverter.ToUInt32(headerData, 4);
                    fileSize = BitConverter.ToUInt32(headerData, 8);
                    mapType = (MapType)BitConverter.ToUInt32(headerData, 0x140);
                    mapName = ASCIIEncoding.ASCII.GetString(headerData, 0x198, 32).Replace("\0", "");
                    scenarioName = ASCIIEncoding.ASCII.GetString(headerData, 0x1BC, 256).Replace("\0", "");
                    signature = BitConverter.ToUInt32(headerData, 0x2D0);
                    mapLoaded = true;
                }
                catch
                {
                    mapLoaded = false;
                }
            }
            #endregion

            #region Methods

            /// <summary>
            /// Returns the name of the loaded map
            /// </summary>
            public string MapName { get { return mapName; } }

            /// <summary>
            /// Returns the type of the loaded map
            /// </summary>
            public MapType MapType { get { return mapType; } }

            /// <summary>
            /// Returns the scenario name of the loaded map
            /// </summary>
            public string ScenarioName { get { return scenarioName; } }

            /// <summary>
            /// Compares certain features of two maps to see how similar they are
            /// </summary>
            /// <param name="map"></param>
            /// <returns>0 = identical, 10 = very different</returns>
            public int Compare(MapHeader map)
            {
                int temp = 0;
                temp += (fileSize != map.fileSize) ? 3 : 0;
                temp += (mapType != map.mapType) ? 3 : 0;
                temp += (mapName != map.mapName) ? 1 : 0;
                temp += (scenarioName != map.scenarioName) ? 2 : 0;
                temp += (signature != map.signature) ? 1 : 0;

                return temp;
            }
            #endregion
        }
        #endregion

        #region Debug Toolbar
        private void tstbDebugIP_TextChanged(object sender, EventArgs e)
        {
            if (tstbDebugIP.Text.Length == 0)
            {
                tstbDebugIP.Text = "<Auto>";
                tstbDebugIP.SelectionStart = 0;
                tstbDebugIP.SelectionLength = 6;
            }
        }

        private void tsbtnDebugConnect_Click(object sender, EventArgs e)
        {
            tslblDebugStatus.Text = "[Attempting to connect...]";
            System.Windows.Forms.Application.DoEvents();

            switch (RTH_Imports.InitRTH(tstbDebugIP.Text))
            {
                case RTH_Imports.debugType.RthDLL:
                case RTH_Imports.debugType.YeloDebug:
                    tstbDebugIP.Text = RTH_Imports.DebugIP;
                    tslblDebugStatus.Text = "[Connected: " + RTH_Imports.DebugName + "]";
                    tsbtnDebugConnect.Enabled = false;
                    tsbtnDebugDisconnect.Enabled = true;
                    tsbtnDebugLoadMap.Enabled = true;
                    timerDebug.Enabled = true;
                    tsbtnDebugReset.Enabled = true;
                    break;
                case RTH_Imports.debugType.None:
                    tslblDebugStatus.Text = "[Not Connected]";
                    tsbtnDebugConnect.Enabled = true;
                    tsbtnDebugDisconnect.Enabled = false;
                    tsbtnDebugLoadMap.Enabled = false;
                    tsbtnDebugReset.Enabled = false;
                    break;
            }
        }

        private void tsbtnDebugDisconnect_Click(object sender, EventArgs e)
        {
            RTH_Imports.DeInitRTH();
            tslblDebugStatus.Text = "[Not Connected]";
            tsbtnDebugConnect.Enabled = true;
            tsbtnDebugDisconnect.Enabled = false;
            tsbtnDebugLoadMap.Enabled = false;
            timerDebug.Enabled = false;
        }

        private void tsbtnDebugLoadMap_Click(object sender, EventArgs e)
        {
            if (!RTH_Imports.IsConnected)
                return;

            uint HeaderAddress = 0x547700;
            string connectInfo = tslblDebugStatus.Text;

            try
            {
                tslblDebugStatus.Text = "[Loading Debug Map...]";
                byte[] data = new byte[2048];
                data = (byte[])RTH_Imports.Peek(HeaderAddress, (uint)data.Length);

                MapHeader LoadedMap = new MapHeader(data);

                string[] filePaths = Directory.GetFiles(Globals.Prefs.pathMapsFolder, "*.map");

                tslblDebugStatus.Text = "[Comparing Maps...]";
                foreach (string filename in filePaths)
                {
                    // Load file and read header
                    FileStream fs = new FileStream(filename, FileMode.Open);
                    byte[] headerData = new byte[2048];
                    fs.Read(headerData, 0, 2048);
                    fs.Close();
                    // Compare file to debug header
                    if (LoadedMap.Compare(new MapHeader(headerData)) == 0)
                    {
                        // Found our MAP!! YES!! Load It!!
                        TryLoadMapForm(filename);
                        return;
                    }
                }
                System.Windows.Forms.MessageBox.Show("No matching map found!" +
                                                      "\nType: " + LoadedMap.MapType +
                                                      "\nName: " + LoadedMap.MapName +
                                                      "\nScenario Name: " + LoadedMap.ScenarioName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No xbox processes loaded.")
                    Globals.Global.ShowErrorMsg("Halo 2 not loaded", ex);
                else
                    Globals.Global.ShowErrorMsg("Load map failed!", ex);
            }
            finally
            {
                tslblDebugStatus.Text = connectInfo;
            }
        }

        /// <summary>
        /// Performs a warm reboot of the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsbtnDebugReset_Click(object sender, System.EventArgs e)
        {            
            if (RTH_Imports.IsConnected)
            {
                RTH_Imports.Reboot(RTH_Imports.BootFlag.warm);
            }
        }

        /// <summary>
        /// Performs a cold reboot of the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsbtnDebugReset_DoubleClick(object sender, System.EventArgs e)
        {
            if (RTH_Imports.IsConnected)
            {
                RTH_Imports.Reboot(RTH_Imports.BootFlag.cold);
            }
        }

        #endregion

        private void timerDebug_Tick(object sender, EventArgs e)
        {
            if (!RTH_Imports.Ping(1600))
            {
                tslblDebugStatus.Text = "[Not Connected]";
                tsbtnDebugConnect.Enabled = true;
                tsbtnDebugDisconnect.Enabled = false;
                tsbtnDebugLoadMap.Enabled = false;
            }
        }
    }
}
