// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RTH.cs" company="">
//   
// </copyright>
// <summary>
//   The rth data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RealTimeHalo
{
    using System.Runtime.InteropServices;
    using XDevkit;
    using System;
    using YeloDebug;

    /// <summary>
    /// The rth data.
    /// </summary>
    /// <remarks></remarks>
    public class RTHData
    {
        #region Constants and Fields

        /// <summary>
        /// The connected.
        /// </summary>
        public bool Connected;

        /// <summary>
        /// The x box ip addy.
        /// </summary>
        public string XBoxIPAddy = "0.0.0.0";

        #endregion
    }

    /// <summary>
    /// The rt h_ imports.
    /// </summary>
    /// <remarks></remarks>
    public class RTH_Imports
    {
        public enum debugType
        {
            None,
            YeloDebug,
            RthDLL
        }

        public enum BootFlag
        {
            cold,
            warm
        }

        const uint INDEX_BASE_ADDRESS = 0xD0061000;
        
        /// <summary>
        /// Used with Real Time Halo (Rth) DLL
        /// </summary>
        static XboxConsole console;
        
        /// <summary>
        /// Used with Yelo Debug DLL
        /// </summary>
        static Xbox xboxDebug;
        static uint BaseAddress = 0;
        static uint VirtMatgOffset = 0;
        private static debugType connectedDebugType = debugType.None;
        private static bool _IsConnected = false;
        private static string _debugName = string.Empty;
        private static string _debugIP = string.Empty;
        /// <summary>
        /// Returns a value stating whether the debug box has been initialized
        /// </summary>
        public static bool IsConnected { get { return _IsConnected;} }
        /// <summary>
        /// Contains the name of the xbox connected to with YeloDebug
        /// </summary>
        public static string DebugName { get { return _debugName; } }
        /// <summary>
        /// Contains the IP address of the current connection
        /// </summary>
        public static string DebugIP { get { return _debugIP; } }

        #region Public Methods

        /// <summary>
        /// The convert float.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The convert float.</returns>
        /// <remarks></remarks>
        [DllImport("RthDLL.dll", CharSet = CharSet.Ansi)]
        public static extern uint ConvertFloat(float f);

        [DllImport("RthDLL.dll", CharSet = CharSet.Ansi, EntryPoint = "DeInitRTH")]
        private static extern void DeInitRTHExtern();
        /// <summary>
        /// De-I/nitializes Real Time Halo.
        /// </summary>
        /// <remarks></remarks>
        public static void DeInitRTH()
        {
            switch (connectedDebugType)
            {
                case debugType.RthDLL:
                    DeInitRTHExtern();

                    _IsConnected = false;
                    break;
                case debugType.YeloDebug:
                    xboxDebug.Disconnect();
                    _IsConnected = false;
                    break;
            }
        }

        private static void InitYeloDebug(string IP)
        {
            xboxDebug = new YeloDebug.Xbox();
            // Do we really need a 5 second timeout period? How about half that?
            xboxDebug.Timeout = 2500;

            // If we are presented with an IP, use it
            if (IP.Split('.').Length == 4)
                xboxDebug.ConnectToIP(IP);
            // otherwise, use auto-detect
            else
            {
                System.Collections.Generic.List<DebugConnection> cons = xboxDebug.QueryXboxConnections();
                xboxDebug.Connect(cons[0].Name);
            }
            _debugName = xboxDebug.DebugName;
            _debugIP = xboxDebug.DebugIP.ToString();
        }
    

        /// <summary>
        /// The init rth.
        /// </summary>
        /// <param name="IP">The ip.</param>
        /// <returns>The init rth.</returns>
        /// <remarks></remarks>
        //[DllImport("RthDLL.dll", CharSet = CharSet.Ansi)]
        public static debugType InitRTH(string IP)
        {
            // Try to connect
            try
            {                
                InitYeloDebug(IP);
                connectedDebugType = debugType.YeloDebug;
                _IsConnected = true;
                return debugType.YeloDebug;
            }
            catch (Exception ex1)
            {
                if (IP == "<Auto>")
                {
                    Globals.Global.ShowErrorMsg("Could not connect to a Yelo debug Xbox and RthDLL does not support <Auto> Mode.", ex1);
                    return debugType.None;
                }
                else
                try
                {
                    console = new XboxManagerClass().OpenConsole(IP);
                    _debugIP = IP;
                    connectedDebugType = debugType.RthDLL;
                    _IsConnected = true;
                    return debugType.RthDLL;
                }
                catch (Exception ex2)
                {
                    Globals.Global.ShowErrorMsg("Could not connect to a debug Xbox", ex2);
                    return debugType.None; 
                }
            }
        }

        public static bool loadDebugMap(Map.Map map)
        {
            try
            {
                // Calculate our base address so we dont need to pass map to Poke()
                if (!map.isOpen)
                    map.OpenMap(HaloMap.Map.MapTypes.Internal);
                map.BR.BaseStream.Position = map.MapHeader.indexOffset;
                uint VirtIndexOffset = map.BR.ReadUInt32();
                map.BR.BaseStream.Position = map.MapHeader.indexOffset + 1456;
                VirtMatgOffset = map.BR.ReadUInt32();
                uint diff = (uint)(VirtMatgOffset - (VirtIndexOffset - 32));
                BaseAddress = (uint)(VirtMatgOffset - (VirtIndexOffset - 32) + INDEX_BASE_ADDRESS);
                map.CloseMap();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The poke.
        /// </summary>
        /// <param name="Addr">The addr.</param>
        /// <param name="Data">The data.</param>
        /// <param name="size">The size.</param>
        /// <remarks></remarks>
        //[DllImport("RthDLL.dll", CharSet = CharSet.Ansi)]
        public static void Poke(uint Addr, uint Data, int size)
        {
            if (!_IsConnected)
                return;
            // Calculate the offset

            switch (connectedDebugType)
            {
                case debugType.RthDLL:
                    uint Ptr = BaseAddress + (Addr - VirtMatgOffset);

                    // Init connection
                    uint con = console.OpenConnection(null);

                    // Poke
                    uint Out = 0;
                    byte[] Bytes = BitConverter.GetBytes(Data);
                    console.DebugTarget.SetMemory(Ptr, (uint)(size / 8), Bytes, out Out);

                    // Close connection
                    console.CloseConnection(con);
                    break;
                case debugType.YeloDebug:
                    switch (size)
                    {
                        case 1:
                            xboxDebug.SetMemory(Addr, (Byte)Data);
                            break;
                        case 2:
                            xboxDebug.SetMemory(Addr, (UInt16)Data);
                            break;
                        case 4:
                            xboxDebug.SetMemory(Addr, (UInt32)Data);
                            break;
                    }
                    break;
            }
        }

        public static void Poke(uint Addr, byte[] Data, int size)
        {
            if (!_IsConnected)
                return;
            // Calculate the offset

            switch (connectedDebugType)
            {
                case debugType.RthDLL:
                    uint Ptr = BaseAddress + (Addr - VirtMatgOffset);

                    // Init connection
                    uint con = console.OpenConnection(null);

                    // Poke
                    uint Out = 0;
                    console.DebugTarget.SetMemory(Ptr, (uint)(size / 8), Data, out Out);

                    // Close connection
                    console.CloseConnection(con);
                    break;
                case debugType.YeloDebug:
                    xboxDebug.SetMemory(Addr, Data);
                    break;
            }
        }

        /// <summary>
        /// Retrieves data from a connected debug xbox
        /// </summary>
        /// <param name="Addr">The address to retrieve data from</param>
        /// <param name="size">The size of the data to return</param>
        /// <returns></returns>
        public static Object Peek(uint Addr, uint size)
        {
            if (!_IsConnected)
                return null;

            // Calculate the offset

            switch (connectedDebugType)
            {
                case debugType.RthDLL:
                    uint Ptr = BaseAddress + (Addr - VirtMatgOffset);

                    // Init connection
                    uint con = console.OpenConnection(null);

                    // Poke
                    uint BytesRead = 0;
                    byte[] Bytes = new byte[size];
                    console.DebugTarget.GetMemory(Ptr, size, Bytes, out BytesRead);

                    // Close connection
                    console.CloseConnection(con);
                    return Bytes;
                case debugType.YeloDebug:
                    if (xboxDebug.ProcessID == 0)
                        throw new Exception("No xbox processes loaded.");
                    return xboxDebug.GetMemory(Addr, size);
            }
            return null;
        }

        #endregion

        public static bool Ping(int waitTime)
        {
            if (RTH_Imports.IsConnected)
                switch (connectedDebugType)
                {
                    case debugType.YeloDebug:
                        if (xboxDebug.Ping(waitTime) != true)
                            _IsConnected = false;
                        else
                            return true;
                        break;
                }
            return false;
        }

        /// <summary>
        /// Performs a reboot of the system
        /// </summary>
        /// <param name="bootFlag"></param>
        public static void Reboot(BootFlag bootFlag)
        {
            switch (bootFlag)
            {
                case BootFlag.cold:
                    xboxDebug.Reboot(YeloDebug.BootFlag.Cold);
                    break;
                case BootFlag.warm:
                    xboxDebug.Reboot(YeloDebug.BootFlag.Warm);
                    break;
            }
        }
    }
}