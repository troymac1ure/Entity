using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Diagnostics;   // debugger display

using System.Windows.Forms;

using YeloDebug.Exceptions;

using System.Xml;

// 0xD0000000
// 0xF0000000   Physical address space
// 0xFD000000   Nvidia GPU address space
// 0xFEF00000   NIC address space
// 0xFF000000   Flash address space


namespace YeloDebug
{
	#region Enums

    public enum ATACommandType
    {
        Read = 0,
        Write = 1,
        NonData = 2
    };

    public enum ATADevice
    {
        HardDrive = 0,
        DVDROM = 1
    }

    /// <summary>
    /// Ethernet link status.
    /// </summary>
    [Flags]
    public enum LinkStatus
    {
        /// <summary>
        /// Ethernet cable is connected and active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Ethernet link is set to 100 Mbps.
        /// </summary>
        Speed100MBPS = 2,

        /// <summary>
        /// Ethernet link is set to 10Mbps.
        /// </summary>
        Speed10MBPS = 4,

        /// <summary>
        /// Ethernet link is in full duplex mode.
        /// </summary>
        FullDuplex = 8,

        /// <summary>
        /// Ethernet link is in half duplex mode.
        /// </summary>
        HalfDuplex = 16
    };
    
    public enum AVPack
    {
        SCART,
        HDTV,
        VGA,
        RFU,
        SVideo,
        Undefined,
        StandardRGB,
        Missing,
        Unknown
    };


    public enum StopOnFlags
    {
        /// <summary>
        /// Suspend title execution when a thread is created.
        /// </summary>
        CreateThread,

        /// <summary>
        /// Suspend title execution when a first-chance exception occurs.
        /// </summary>
        FCE,

        /// <summary>
        /// Suspend title execution when a string is sent to debug out.
        /// </summary>
        DebugStr
    };

    public enum BreakpointType
    {
        /// <summary>
        /// Clear this data breakpoint.
        /// </summary>
        None = 1,

        /// <summary>
        /// Break if the address is written to.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Break if the address is read from or written to.
        /// </summary>
        ReadWrite = 4,

        /// <summary>
        /// Break if the address is executed.
        /// </summary>
        Execute = 8
    };

    public enum NotificationType
    {
        None,
        Breakpoint,
        DebugString,
        Execution,
        SingleStep,
        ModuleLoad,
        ModuleUnload,
        CreateThread,
        DestroyThread,
        Exception,
        Assert,
        DataBreak,
        RIP,
        ThreadSwitch
    };

    public enum XboxVersion
    {
        Devkit,
        DebugKit,   // Green
        V1_0,
        V1_1,
        V1_2,
        V1_3,
        V1_4,
        V1_6,
        Unknown
    };


    /// <summary>
    /// Xbox boot type.
    /// </summary>
	public enum BootFlag : uint
	{
		/// <summary>
		/// When the reboot is complete, the system software will wait 15 
		/// seconds before launching the default title. If you call DmGo 
		/// during this time, the system software will launch the title immediately.
		/// </summary>
		Wait,

		/// <summary>
		/// Perform a "warm" reboot of the console.
		/// </summary>
		Warm,

		/// <summary>
		/// Prevent debugging after reboot. The Xbox debug manager will not be 
		/// loaded on any subsequent warm reboot; a cold boot is required.
		/// </summary>
		NoDebug,

		/// <summary>
		/// When the reboot is complete, the system software will wait 
		/// (with no timeout) before launching the default title. Calling 
		/// DmGo while the system is waiting will launch the title.
		/// </summary>
		Stop,

		/// <summary>
		/// Complete shut-down of the system before rebooting.
		/// </summary>
		Cold,

		/// <summary>
		/// Warm reboot to active title.
		/// </summary>
		Current
	};

    /// <summary>
    /// Xbox video standard.
    /// </summary>
	public enum VideoStandard : byte
	{
		NTSCAmerica = 1,
		NTSCJapan = 2,
		PAL = 3
	};

    /// <summary>
    /// Xbox video flags.
    /// </summary>
	[Flags]
	public enum VideoFlags
	{
		Normal		= 0,
		Widescreen	= 0x1,
		HDTV_720p	= 0x2,
		HDTV_1080i	= 0x4,
		HDTV_480p	= 0x8,
		Letterbox	= 0x10,
		PAL_60Hz	= 0x40
	};

    /// <summary>
    /// Xbox memory allocation type.
    /// </summary>
	public enum AllocationType : byte
	{
		Debug,
		Virtual,
		Physical,
		System
	};

    /// <summary>
    /// Xbox memory flags.
    /// </summary>
	[Flags]
	public enum MEMORY_FLAGS
	{
		PAGE_VIDEO				= 0x0,
		PAGE_NOACCESS			= 0x1,
		PAGE_READONLY			= 0x2,
		PAGE_READWRITE			= 0x4,
		PAGE_WRITECOPY			= 0x8,
		PAGE_EXECUTE			= 0x10,
		PAGE_EXECUTE_READ		= 0x20,
		PAGE_EXECUTE_READWRITE	= 0x40,
		PAGE_EXECUTE_WRITECOPY	= 0x80,
		PAGE_GUARD				= 0x100,
		PAGE_NOCACHE			= 0x200,
		PAGE_WRITECOMBINE		= 0x400,
		MEM_COMMIT				= 0x1000,
		MEM_RESERVE				= 0x2000,
		MEM_DECOMMIT			= 0x4000,
		MEM_RELEASE				= 0x8000,
		MEM_FREE				= 0x10000,
		MEM_PRIVATE				= 0x20000,
		MEM_RESET				= 0x80000,
		MEM_TOP_DOWN			= 0x100000,
		MEM_NOZERO				= 0x800000
	};

	/// <summary>
	/// Represents one of the 4 possible Xbox LED states.
	/// </summary>
	public enum LEDState : byte
	{
		Off		= 0,
		Red		= 0x80,
		Green	= 8,
		Orange	= 0x88
	};

	/// <summary>
	/// Xbox gamepad buttons.
	/// </summary>
	[Flags]
	public enum Buttons : ushort
	{
		Up					    = 1 << 0,
		Down				    = 1 << 1,
		Left				    = 1 << 2,
		Right				    = 1 << 3,
		Start					= 1 << 4,
		Back					= 1 << 5,
		LeftThumb				= 1 << 6,
		RightThumb				= 1 << 7,
		LightGunOnScreen		= 1 << 13,
		LightGunFrameDoubler	= 1 << 14,
		LightGunLineDoubler	    = 1 << 15
	};

	/// <summary>
	/// Xbox analog gamepad buttons.
	/// </summary>
	[Flags]
	public enum AnalogButtons
	{
		A,
		B,
		X,
		Y,
		Black,
		White,
		LeftTrigger,
		RightTrigger
	};

    /// <summary>
    /// Xbox response type.
    /// </summary>
	public enum ResponseType
	{
		// Success
		SingleResponse		            = 200,  //OK
		Connected			            = 201,
		MultiResponse		            = 202,  //terminates with period
		BinaryResponse		            = 203,
		ReadyForBinary		            = 204,
		NowNotifySession	            = 205,  // notificaiton channel/ dedicated connection

		// Errors
		UndefinedError		            = 400,
        MaxConnectionsExceeded          = 401,
		FileNotFound		            = 402,
		NoSuchModule		            = 403,
		MemoryNotMapped		            = 404,  //setzerobytes or setmem failed
        NoSuchThread                    = 405,
		ClockNotSet                     = 406,  //linetoolong or clocknotset
		UnknownCommand		            = 407,
        NotStopped                      = 408,
        FileMustBeCopied                = 409,
		FileAlreadyExists	            = 410,
        DirectoryNotEmpty               = 411,
        BadFileName                     = 412,
        FileCannotBeCreated             = 413,
		AccessDenied		            = 414,
        NoRoomOnDevice                  = 415,
        NotDebuggable                   = 416,
        TypeInvalid                     = 417,
        DataNotAvailable                = 418,
		BoxIsNotLocked		            = 420,
		KeyExchangeRequired	            = 421,
        DedicatedConnectionRequired     = 422,
        InvalidArgument                 = 423,
        ProfileNotStarted               = 424,
        ProfileAlreadyStarted           = 425,
        D3DDebugCommandNotImplemented   = 480,
        D3DInvalidSurface               = 481,
        VxTaskPending                   = 496,
        VxTooManySessions               = 497,
	};

    /// <summary>
    /// Receive wait type.
    /// </summary>
	public enum WaitType
	{
		/// <summary>
		/// Does not wait.
		/// </summary>
		None,

		/// <summary>
		/// Waits until some data starts being received.
		/// </summary>
		Partial,

		/// <summary>
		/// Waits until all data has been received. Use only with continuous flows of data, never file IO.
		/// </summary>
		Full
	};

	/// <summary>
	/// Items to include in a memdump.
	/// </summary>
	[Flags]
	public enum DumpFlags
	{
		/// <summary>
		/// Xbe code segment.
		/// </summary>
		Code,

        /// <summary>
        /// Xbe data segment.
        /// </summary>
        Data,

		/// <summary>
		/// System pages. (kernel, stacks, pools, etc...)
		/// </summary>
		System,

		/// <summary>
		/// Debugger pages.
		/// </summary>
		Debug
	};


    /// <summary>
    /// Devices on the SMBus
    /// </summary>
    public enum SMBusDevices
    {
        PIC = 0x20,
        VideoEncoderConnexant = 0x8a,
        VideoEncoderFocus = 0xd4,
        VideoEncoderXcalibur = 0xe0,
        TempMonitor = 0x98,
        EEPROM = 0xA8
    };

    /// <summary>
    /// Commands that can be sent to the PIC.
    /// </summary>
    public enum PICCommand
    {
        Version = 0x1,
        Power = 0x2,
        DvdTrapState = 0x3,
        AVPack = 0x4,
        FanMode = 0x5,
        FanRegister = 0x6,
        LedMode = 0x7,
        LedRegister = 0x8,
        CPUTemp = 0x9,
        GPUTemp = 0xA,
        FanReadBack = 0x10,
        Eject = 0xC,
        InterruptReason = 0x11,
        ResetOnEject = 0x19,
        ScratchRegister = 0x1B
    };

    /// <summary>
    /// Commands that can be sent to the video encoder.
    /// </summary>
    public enum VideoEncoderCommand // TVEncoderSMBusID
    {
        Detect = 0x00//,
        //Unknown = 0x5 // subcommand > 0 spits out random numbers, its fucking weird...
    };

    /// <summary>
    /// Values for the video encoder.
    /// </summary>
    public enum VideoEncoder
    {
        Unknown = 0,
        Connexant = 1,
        Focus = 2,
        Xcalibur = 3
    };

    /// <summary>
    /// Sub-commands for the PIC power command.
    /// </summary>
    public enum PowerSubCommand
    {
        Reset = 0x01,
        Cycle = 0x40,
        PowerOff = 0x80
    };

    /// <summary>
    /// Sub-commands for the PIC led command.
    /// </summary>
    public enum LEDSubCommand
    {
        Default = 0x00,
        Custom = 0x01
    };

    /// <summary>
    /// Sub-commands for the PIC eject command.
    /// </summary>
    public enum EjectSubCommand
    {
        Eject = 0x00,
        Load = 0x01
    };

    public enum FanModeSubCommand
    {
        Default = 0x0,
        Custom = 0x1
    };

    /// <summary>
    /// Xbox tray state.
    /// </summary>
    [Flags]
    public enum TrayState : byte
    {
        /// <summary>
        /// Drive is not ready.
        /// </summary>
        Busy = 0x1,

        /// <summary>
        /// Tray is open.
        /// </summary>
        Open = 0x10,

        /// <summary>
        /// Media disk present in tray.
        /// </summary>
        Disc = 0x20,

        /// <summary>
        /// Tray is closed.
        /// </summary>
        Closed = 0x40,

        /// <summary>
        /// No optical drive detected.
        /// </summary>
        None = 0x5

        //Loading = 0x1,           // loading rom
        //EjectingFull = 0x21,         // ejecting with rom in tray
        //EjectingEmpty = 0x31,       // ejecting empty tray
        //Open = 0x10,
        //Closing = 0x51,             // closing tray
        //ClosedAndEmpty = 0x40,      // closed with no rom
        //ClosedAndFull = 0x60      // closed with rom
    };

    //Reason for interrupt
    public enum InterruptReason
    {
        PowerButton = 0x01,
        AvCableRemoved = 0x10,
        EjectButton = 0x20
    };

    /// <summary>
    /// Sub-commands for reset on eject PIC command.
    /// </summary>
    public enum ResetOnEjectSubCommand
    {
        Enable = 0x00,
        Disable = 0x01
    };

    /// <summary>
    /// Scratch register values.
    /// </summary>
    public enum ScratchRegisterValue
    {
        EjectAfterBoot = 0x01,
        DisplayError = 0x02,
        NoAnimation = 0x04,
        RunDashboard = 0x08
    };

    /// <summary>
    /// Xbox device.
    /// </summary>
    public enum Device
    {
        CDRom,
        DriveC,
        DriveE,
        DriveF,
        //DriveG, // seems to be disabled in debug bios
        //DriveH, // seems to be disabled in debug bios
        DriveX,
        DriveY,
        DriveZ
    }

    /// <summary>
    /// Xbox drive name.
    /// </summary>
    public enum Drive
    {
        A,  // DVD-ROM drive
        B,  // Volume
        C,  // Main volume
        D,  // Active title media
        E,  // Game development volume
        F,  // Memory unit 1A
        G,  // Memory unit 1B
        H,  // Memory unit 2A
        I,  // Memory unit 2B
        J,  // Memory unit 3A
        K,  // Memory unit 3B
        L,  // Memory unit 4A
        M,  // Memory unit 4B
        N,  // Secondary active utility drive
        O,  // Volume
        P,  // Utility drive for unknown title
        Q,  // Utility drive for unknown title  
        R,  // Utility drive for unknown title
        S,  // Persistent data for all titles
        T,  // Persistent data for active title
        U,  // Saved games for active title
        V,  // Saved games for all titles
        W,  // Persistant data for alternate title
        X,  // Saved games for alternate title
        Y,  // Reserved/unmappable while in debug???
        Z   // Active utility drive
    };

	#endregion

	#region Structs

    public class ATAInputRegisters
    {
        public byte Features;      // Used for specifying SMART "commands".
        public byte SectorCount;   // IDE sector count register
        public byte SectorNumber;  // IDE sector number register
        public byte CylinderLow;   // IDE low order cylinder value
        public byte CylinderHigh;  // IDE high order cylinder value
        public byte DriveHead;     // IDE drive/head register
        public byte Command;       // Actual IDE command.
        public byte Reserved = 0;  // reserved for future use.  Must be zero.
    };
    public struct ATAOutputRegisters
    {
        public byte Error;
        public byte SectorCount;
        public byte SectorNumber;
        public byte CylinderLow;
        public byte CylinderHigh;
        public byte DriveHead;
        public byte Status;
    };
    public class ATACommandObject
    {
        public ATAInputRegisters InputRegisters;
        public ATAOutputRegisters OutputRegisters;
        public byte[] Data = new byte[512];
        public uint DataSize = 512;
    };

    public class IDERegisters
    {
        public byte Features;
        public byte SectorCount;
        public byte SectorNumber;
        public byte CylinderLow;
        public byte CylinderHigh;
        public byte DriveHead;
        public byte CommandRegister;
        public byte HostSendsData;
    };
    public unsafe class ATAPassThrough
    {
        public IDERegisters Registers;
        public uint DataBufferSize;
        public byte* DataBuffer;
    };

    public struct ProcessorInformation
    {
        public uint Stepping;
        public uint Model;
        public uint Family;
    };

    public struct ProductionInfo
    {
        public string Country;
        public uint LineNumber;
        public uint Week;
        public uint Year;
    };
    //public struct ThreadStopInfo
    //{
    //    public bool IsStopped;
    //    public uint Address;
    //    public uint Thread;

    //}

    //// maybe do separate notification structs for each type: BreakNotification, ExecutionNotification etc...
    //public struct Notification
    //{
    //    NotificationType Type;
    //    string Message;
    //}

    public struct ThreadInfo
    {
        public uint ID;
        public uint Suspend;
        public uint Priority;
        public uint TlsBase;
        public uint Start;
        public uint Base;
        public uint Limit;
        public DateTime CreationTime;
    };

	/// <summary>
	/// Module information.
	/// </summary>
	public class ModuleInfo
	{
		/// <summary>
		/// Name of the module that was loaded.
		/// </summary>
		public string Name;

		/// <summary>
		/// Address that the module was loaded to.
		/// </summary>
		public uint BaseAddress;

		/// <summary>
		/// Size of the module.
		/// </summary>
		public uint Size;

		/// <summary>
		/// Time stamp of the module.
		/// </summary>
        public DateTime TimeStamp;

		/// <summary>
		/// Checksum of the module.
		/// </summary>
		public uint Checksum;

        /// <summary>
        /// Sections contained within the module.
        /// </summary>
        public List<ModuleSection> Sections;
	};

    /// <summary>
    /// Module section information.
    /// </summary>
    public class ModuleSection
    {
        public string Name;
        public uint Base;
        public uint Size;
        public uint Index;
        public uint Flags;
    };

    /// <summary>
    /// Structure that contains information about the XBE.
    /// </summary>
	public class XbeInfo
	{
		public string LaunchPath;
        public DateTime TimeStamp;
		public uint Checksum;
		public uint StackSize;
	};

    /// <summary>
    /// Xbox file information.
    /// </summary>
	public class FileInformation
	{
		public string Name;
		public ulong Size;
		public FileAttributes Attributes;
		public DateTime CreationTime;
		public DateTime ChangeTime;
	};

    /// <summary>
    /// Xbox memory statistics.
    /// </summary>
	public class MemoryStatistics
	{
		public uint TotalPages;
		public uint AvailablePages;
		public uint StackPages;
		public uint VirtualPageTablePages;
		public uint SystemPageTablePages;
		public uint PoolPages;
		public uint VirtualMappedPages;
		public uint ImagePages;
		public uint FileCachePages;
		public uint ContiguousPages;
		public uint DebuggerPages;
	};

    /// <summary>
    /// Xbox memory region.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class MemoryRegion
	{
		public UIntPtr BaseAddress;
		public uint Size;
		public uint Protect;
	};

    /// <summary>
    /// Xbox memory address range.
    /// </summary>
	public class AddressRange // really should be a struct, but then you give up the parameterless ctor. sacrifices mike?
	{
		public uint Low;
		public uint High;

		/// <summary>
		/// Specifies a default range of all addresses.
		/// </summary>
		public AddressRange()
		{
			Low = 0;
			High = 0xFFFFFFFF;
		}

		/// <summary>
		/// Specifies a custom address range.
		/// </summary>
		/// <param name="low"></param>
		/// <param name="high"></param>
		public AddressRange(uint low, uint high)
		{
			Low = low;
			High = high;
		}
	};

    /// <summary>
    /// Xbox memory allocation entry.
    /// </summary>
	public struct AllocationEntry
	{
		public uint Address;
		public uint Size;
		public AllocationType Type;

		public AllocationEntry(uint address, uint size, AllocationType type)
		{
			Address = address;
			Size = size;
			Type = type;
		}
	};

    /// <summary>
    /// Basic xbox memory information.
    /// </summary>
	public struct MEMORY_BASIC_INFORMATION
	{
		public uint BaseAddress;
		public uint AllocationBase;
		public MEMORY_FLAGS AllocationProtect;
		public uint RegionSize;
		public MEMORY_FLAGS State;
		public MEMORY_FLAGS Protect;
		public MEMORY_FLAGS Type;
	};

	/// <summary>
	/// Cpu general purpose register context.  Only the registers you change will be set before the call.
	/// </summary>
	public class CPUContext
	{
        // general purpose - assumes integer assignment
        public object Eax;
        public object Ebx;
        public object Ecx;
        public object Edx;
        public object Esi;
        public object Edi;
        public object Esp;
        public object Ebp;

        // sse - assumes floating point assignment
        public object Xmm0;
        public object Xmm1;
        public object Xmm2;
        public object Xmm3;
        public object Xmm4;
        public object Xmm5;
        public object Xmm6;
        public object Xmm7;
	};

	/// <summary>
	/// Xbox gamepad input state.
	/// </summary>
	public class InputState
	{
		public Buttons Buttons = 0;
		public byte[] AnalogButtons = new byte[8];
		public short ThumbLX = 0;
		public short ThumbLY = 0;
		public short ThumbRX = 0;
		public short ThumbRY = 0;

		public void AssignState(InputState state)
		{
			this.Buttons = state.Buttons;
			this.AnalogButtons = state.AnalogButtons;
			this.ThumbLX = state.ThumbLX;
			this.ThumbLY = state.ThumbLY;
			this.ThumbRX = state.ThumbRX;
			this.ThumbRY = state.ThumbRY;
		}
	};

    /// <summary>
    /// Xbox command status response.
    /// </summary>
	public class StatusResponse
	{
        public string Full;
		public ResponseType Type;
		public string Message;
		public bool Success;

		public StatusResponse(string original, ResponseType type, string message)
		{
            Full = original;
			Type = type;
			Message = message;
			Success = ((int)type & 200) == 200;
		}
	};

    /// <summary>
    /// Xbox debug connection information.
    /// </summary>
    public struct DebugConnection
    {
        public IPAddress LocalIP;
        public IPAddress IP;
        public string Name;
        public DebugConnection(IPAddress localip, IPAddress ip, string name)
        {
            LocalIP = localip;
            IP = ip;
            Name = name;
        }
    };
	#endregion

	/// <summary>
	/// Xbox debug connection.
	/// </summary> 
	public partial class Xbox
	{
		#region Fields

        /// <summary>
        /// Registry entry to store the last connection used.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string xdkRegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\XboxSDK";   // default

        /// <summary>
        /// Keeps a list of available connections
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<DebugConnection> connections = new List<DebugConnection>();

		#endregion

		#region Properties

        /// <summary>
        /// Gets the main connection used for pc to xbox communication.
        /// </summary>
        public TcpClient Connection { get { return connection; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TcpClient connection = new TcpClient();

        /// <summary>
        /// Gets the xbox kernel information.
        /// </summary>
        public XboxKernel Kernel { get { return kernel; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XboxKernel kernel;

        /// <summary>
        /// Gets or sets the maximum waiting time given (in milliseconds) for a response.
        /// </summary>
        public int Timeout { get { return timeout; } set { timeout = value; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int timeout = 5000;

		/// <summary>
		/// Gets the current connection status known to YeloDebug.  For an actual status update you need to Ping() the xbox.
		/// </summary>
		public bool Connected	{ get { return connected; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool connected = false;

        /// <summary>
		/// Gets the xbox debug ip address.
		/// </summary>
		public IPAddress DebugIP   { get { return debugIP; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IPAddress debugIP;

        /// <summary>
        /// Gets the xbox debug name.
        /// </summary>
        public string DebugName { get { return debugName; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string debugName;

        /// <summary>
        /// Gets or sets whether or not the notification session will be enabled.
        /// </summary>
        public bool EnableNotificationSession { get { return notificationSessionEnabled; } set { notificationSessionEnabled = value; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool notificationSessionEnabled = true;

        /// <summary>
        /// Gets the notification listener registered with the xbox that listens for incoming notification session requests.
        /// </summary>
        public TcpListener NotificationListener { get { return notificationListener; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TcpListener notificationListener;

        /// <summary>
        /// Gets the current notification session registered with the xbox.
        /// </summary>
        public TcpClient NotificationSession { get { return notificationSession; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TcpClient notificationSession;

        /// <summary>
        /// Gets or sets the xbox notification port.
        /// </summary>
        public int NotificationPort { get { return notificationPort; } set { notificationPort = value; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int notificationPort = 731;

        /// <summary>
        /// Gets or sets the list of notifications.
        /// </summary>
        public List<string> Notifications { get { return notifications; } set { notifications = value; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> notifications = new List<string>();

		/// <summary>
		/// Gets the xbox game ip address.
		/// </summary>
        public IPAddress TitleIP { get { return titleIP; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPAddress titleIP;

		/// <summary>
		/// Gets the xbox process id.
		/// </summary>
		public uint ProcessID { get	{ return processID;	} }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint processID = 0;

        /// <summary>
        /// Gets a list of modules loaded by the xbox.
        /// </summary>
        public List<ModuleInfo> Modules { get { return modules; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<ModuleInfo> modules;

        /// <summary>
        /// Gets xbox executable information.
        /// </summary>
        public XbeInfo XbeInfo { get { return xbeInfo; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XbeInfo xbeInfo;

        /// <summary>
        /// Gets the xbox debug monitor version.
        /// </summary>
        public Version DebugMonitorVersion { get { return debugMonitorVersion; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Version debugMonitorVersion;

        /// <summary>
        /// Gets the xbox kernel version.  Note that non-debug kernel build versions are substituted with the build year instead.
        /// </summary>
        public Version KernelVersion { get { return kernelVersion; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Version kernelVersion;

        /// <summary>
        /// Gets the xbox hard drive key.
        /// </summary>
        public byte[] HardDriveKey { get { return hardDriveKey; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] hardDriveKey;

        /// <summary>
        /// Gets the xbox EEPROM key.
        /// </summary>
        public byte[] EEPROMKey { get { return eepromKey; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] eepromKey;

        /// <summary>
        /// Gets the xbox signature key.
        /// </summary>
        public byte[] SignatureKey { get { return signatureKey; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] signatureKey;

        /// <summary>
        /// Gets the xbox lan key.
        /// </summary>
        public byte[] LanKey { get { return lanKey; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] lanKey;

        /// <summary>
        /// Gets the alternate xbox signature keys.
        /// </summary>
        public byte[][] AlternateSignatureKeys { get { return alternateSignatureKeys; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[][] alternateSignatureKeys;

        /// <summary>
        /// Gets the xbox hard drive serial number.
        /// </summary>
        public string HardDriveSerial { get { return hardDriveSerial; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string hardDriveSerial;

        /// <summary>
        /// Gets the xbox hard drive model.
        /// </summary>
        public string HardDriveModel { get { return hardDriveModel; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string hardDriveModel;

        /// <summary>
        /// Gets the xbox serial number.
        /// </summary>
        public ulong SerialNumber { get { return serialNumber; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ulong serialNumber;

        /// <summary>
        /// Gets the xbox mac address.
        /// </summary>
        public string MacAddress { get { return macAddress; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string macAddress;

        /// <summary>
        /// Gets the xbox version.
        /// </summary>
        public string Version { get { return version; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string version;

        /// <summary>
        /// Gets the xbox hardware info.
        /// </summary>
        public string HardwareInfo { get { return hardwareInfo; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string hardwareInfo;

        /// <summary>
        /// Gets the video encoder type installed on the xbox.
        /// </summary>
        public VideoEncoder VideoEncoderType { get { return videoEncoderType; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private VideoEncoder videoEncoderType;

        /// <summary>
        /// Gets the xbox processor frequency.
        /// </summary>
        public string CPUFrequency { get { return cpuFrequency; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string cpuFrequency;

        /// <summary>
        /// Gets the xbox processor information.
        /// </summary>
        public ProcessorInformation ProcessorInformation { get { return processorInformation; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ProcessorInformation processorInformation;

        /// <summary>
        /// Gets the current xbox ethernet link status.
        /// </summary>
        public LinkStatus LinkStatus { get { return linkStatus; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private LinkStatus linkStatus;

        /// <summary>
        /// Gets the xbox EEPROM.
        /// </summary>
        public byte[] EEPROM { get { return eeprom; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte[] eeprom;

        /// <summary>
        /// Gets or sets the last xbox connection used.
        /// </summary>
        public string LastConnectionUsed
        {
            get { return (string)Microsoft.Win32.Registry.GetValue(xdkRegistryPath, "XboxName", string.Empty); }
            set { Microsoft.Win32.Registry.SetValue(xdkRegistryPath, "XboxName", value); }
        }

        /// <summary>
        /// Gets the xbox manufacturer production information.
        /// </summary>
        public ProductionInfo ProductionInfo
        {
            get
            {
                ProductionInfo info = new ProductionInfo();
                string serial = ASCIIEncoding.ASCII.GetString(eeprom, 0x34, 12);
                switch (serial[11])
                {
                    case '2': info.Country = "Mexico"; break;
                    case '3': info.Country = "Hungary"; break;
                    case '5': info.Country = "China"; break;
                    case '6': info.Country = "Taiwan"; break;
                    default: info.Country = "Unknown"; break;
                }
                info.LineNumber = Convert.ToUInt32(serial.Substring(0, 1));
                info.Week = Convert.ToUInt32(serial.Substring(8, 2));
                info.Year = Convert.ToUInt32("200" + serial[7]);

                return info;
            }
        }

        /// <summary>
        /// Gets the current AV-Pack status.
        /// </summary>
        public AVPack AVPack
        {
            get
            {
                CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.AVPack, 0, scratchBuffer);
                return (AVPack)GetByte(scratchBuffer);
            }
        }

		/// <summary>
		/// Gets or sets the xbox system time.
		/// </summary>
		public unsafe DateTime SystemTime
		{
			get
			{
				StatusResponse response = SendCommand("systime");
				if (response.Type == ResponseType.SingleResponse)
				{
					string ticks = string.Format("0x{0}{1}",
						response.Message.Substring(7, 7),
						response.Message.Substring(21).PadLeft(8, '0')
						);
					return DateTime.FromFileTime(Convert.ToInt64(ticks, 16));
				}
				else throw new ApiException("Failed to get xbox system time.");
			}
            set
            {
                long fileTime = value.ToFileTimeUtc();
                int lo = *(int*)&fileTime;
                int hi = *((int*)&fileTime + 1);

                StatusResponse response = SendCommand(string.Format("setsystime clockhi=0x{0} clocklo=0x{1} tz=1", Convert.ToString(hi, 16), Convert.ToString(lo, 16)));
                if (response.Type != ResponseType.SingleResponse)
                    throw new ApiException("Failed to set xbox system time.");
            }
		}

        /// <summary>
        /// Gets the amount of time the xbox has been powered on for or the time since its last cold boot.
        /// </summary>
        public TimeSpan TimePoweredOn
        {
            get
            {
                //rdtsc
                //mov	dword ptr ds:[010004h], eax
                //mov	dword ptr ds:[010008h], edx
                //mov	eax, 02DB0000h	;fake success
                //retn	010h
                SetMemory(ScriptBufferAddress, Util.StringToHexBytes("0F31A304000100891508000100B80000DB02C21000"));
                SendCommand("crashdump");

                uint performaceFrequency;
                if (processorInformation.Model == 11) performaceFrequency = 1481200000; // DreamX console
                else if (processorInformation.Model == 8 && processorInformation.Stepping == 6) performaceFrequency = 999985000;   // Intel Pentium III Coppermine
                else performaceFrequency = 733333333;

                return TimeSpan.FromSeconds(GetUInt64(0x10004) / performaceFrequency);    // performanceFrequency in Hz (counts per second)
            }
        }

		/// <summary>
		/// Gets the xbox memory statistics.
		/// </summary>
		public MemoryStatistics MemoryStatistics
		{
			get
			{
				SendCommand("mmglobal");
				MemoryStatistics Statistics = new MemoryStatistics();

				string[] stats1 = ReceiveSocketLine().Split(' ');
				uint addr = Convert.ToUInt32(stats1[4].Substring(24), 16);
				string[] stats2 = ReceiveSocketLine().Split(' ');
				uint totalPages = Convert.ToUInt32(stats2[4].Substring(26), 16);
				uint availablePages = Convert.ToUInt32(stats2[5].Substring(19).Replace("\r\n", ""), 16);
				ReceiveSocketLine();    // '.'

				byte[] mem = GetMemory(addr, 0x2C);
				Statistics.AvailablePages = availablePages;
				Statistics.TotalPages = totalPages;
				Statistics.StackPages = BitConverter.ToUInt32(mem, 4);
				Statistics.VirtualPageTablePages = BitConverter.ToUInt32(mem, 8);
				Statistics.SystemPageTablePages = BitConverter.ToUInt32(mem, 12);
				Statistics.PoolPages = BitConverter.ToUInt32(mem, 16);
				Statistics.VirtualMappedPages = BitConverter.ToUInt32(mem, 20);
				Statistics.ImagePages = BitConverter.ToUInt32(mem, 28);
				Statistics.FileCachePages = BitConverter.ToUInt32(mem, 32);
				Statistics.ContiguousPages = BitConverter.ToUInt32(mem, 36);
				Statistics.DebuggerPages = BitConverter.ToUInt32(mem, 40);
				return Statistics;
			}
		}

		/// <summary>
		/// Gets a list of valid xbox memory segments.
		/// </summary>
		public List<MemoryRegion> CommittedMemory
		{
			get
			{
				SendCommand("walkmem");
				List<MemoryRegion> mem = new List<MemoryRegion>();

				string page = ReceiveSocketLine();
				while (page[0] != '.')
				{
					MemoryRegion region = new MemoryRegion();

                    region.BaseAddress = (UIntPtr)(uint)Util.GetResponseInfo(page, 0);
                    region.Size = (uint)Util.GetResponseInfo(page, 1);
                    region.Protect = (uint)Util.GetResponseInfo(page, 2);
					mem.Add(region);
					page = ReceiveSocketLine();
				}
				return mem;
			}
		}

        /// <summary>
        /// Gets a list of xbox threads.
        /// </summary>
		public List<ThreadInfo> Threads
		{
			get
			{
				SendCommand("threads");
				List<uint> threadIDs = new List<uint>();
				string line = ReceiveSocketLine();
				while (line[0] != '.')
				{
                    threadIDs.Add((uint)Util.GetResponseInfo(line, 0));
					line = ReceiveSocketLine();
				}

                List<ThreadInfo> threads = new List<ThreadInfo>();
                foreach (uint id in threadIDs)
                {
                    ThreadInfo threadInfo = new ThreadInfo();
                    SendCommand("threadinfo thread={0}", id);
                    threadInfo.ID = id;
                    List<object> info = Util.ExtractResponseInformation(ReceiveSocketLine());
                    threadInfo.Suspend = (uint)info[0];
                    threadInfo.Priority = (uint)info[1];
                    threadInfo.TlsBase = (uint)info[2];
                    threadInfo.Start = (uint)info[3];
                    threadInfo.Base = (uint)info[4];
                    threadInfo.Limit = (uint)info[5];
                    long ticks = (uint)info[7];
                    ticks |= (((long)(uint)info[6] << 32));
                    threadInfo.CreationTime = DateTime.FromFileTime(ticks);
                    threads.Add(threadInfo);
                    ReceiveSocketLine();    //'.'
                }
                return threads;
			}
		}

        /// <summary>
        /// Gets the xbox CPU temperature in degrees celsius.
        /// </summary>
        public uint CPUTemperature
        {
            get
            {
                CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.CPUTemp, 0, scratchBuffer);
                return GetUInt32(scratchBuffer);
            }
        }

        /// <summary>
        /// Gets the xbox GPU temperature in degrees celsius.
        /// </summary>
        public uint GPUTemperature
        {
            get
            {
                CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.GPUTemp, 0, scratchBuffer);
                uint temp = GetUInt32(scratchBuffer);
                if (version == "Xbox v1.6") temp = (uint)(temp * 0.8f); // v1.6 box shows temp too high
                return temp;
            }
        }

        /// <summary>
        /// Gets or sets the xbox fan speed percentage.
        /// </summary>
        public int FanSpeed
        {
            get
            {
                CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.FanReadBack, 0, scratchBuffer);
                int result = GetInt32(scratchBuffer);
                return (int)(((float)result / 50) * 100);
            }
            set
            {
                int speed = (int)(value * 0.5f);
                CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.FanMode, 0, FanModeSubCommand.Custom);
                CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.FanRegister, 0, speed);
                CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.FanMode, 0, FanModeSubCommand.Custom);
                CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.FanRegister, 0, speed);
            }
        }

		#endregion

		#region Constructor / Destructor
		/// <summary>
		/// Xbox connection.
		/// </summary>
		public Xbox()
        {
            xdkRegistryPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\XboxSDK";
            try
            {
                // load settings file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Application.StartupPath + "\\YeloDebugSettings.xml");

                // check that setting and assembly versions match
                string assemblyVersion = Assembly.GetExecutingAssembly().FullName.Substring(19, 10);
                string settingsVersion = xmlDoc.GetElementsByTagName("Version")[0].InnerText;
                if (assemblyVersion != settingsVersion) throw new ApiException("YeloDebug version does not match the version of the settings file.");

                // get settings information
                xdkRegistryPath = xmlDoc.GetElementsByTagName("XdkRegistryPath")[0].InnerText;
            }
            catch
            {
                if (MessageBox.Show("YeloDebugSettings.xml not found or unreadable.\nAttempt to continue?", "Yelo Error", MessageBoxButtons.YesNo) == DialogResult.No)
                    throw new Exception("YeloDebugSettings.xml not found or unreadable.");
            }
        }

		~Xbox() { Disconnect(); }
		#endregion

		#region Connection
        /// <summary>
        /// Returns a list containing all consoles detected on the network.
        /// </summary>
        /// <returns></returns>
        public List<DebugConnection> QueryXboxConnections()
        {
            // Remove any previous listings of sockets
            connections.Clear();

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                        
            try
            {
                // Cycle through all available network interfaces
                foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    foreach (var ua in i.GetIPProperties().UnicastAddresses)
                    {
                        try
                        {
                            // Bind to our chosen network interfaces Local IP Address
                            s.Bind(new IPEndPoint(ua.Address, 0));

                            // broadcast our request
                            byte[] sendBuf = { 3, 0 };
                            s.SendTo(sendBuf, new IPEndPoint(IPAddress.Broadcast, 731));

                            // wait for response
                            DateTime before = DateTime.Now;
                            TimeSpan elapse = new TimeSpan();
                            while (s.Available == 0)
                            {
                                Thread.Sleep(0);
                                elapse = DateTime.Now - before;
                                if (elapse.TotalMilliseconds > timeout)
                                    break;
                            }

                            // If we find a connection, break out
                            if (s.Available != 0)
                            {
                                // parse any information returned
                                byte[] data = new byte[s.Available];
                                EndPoint end = new IPEndPoint(IPAddress.Any, 0);
                                while (s.Available > 0)
                                {
                                    s.ReceiveFrom(data, ref end);
                                    IPEndPoint endpoint = (IPEndPoint)end;
                                    connections.Add(new DebugConnection(ua.Address, ((IPEndPoint)end).Address, ASCIIEncoding.ASCII.GetString(data, 2, data.Length - 2).Replace("\0", "")));
                                }
                            }
                            
                            // Destroy our binded socket
                            s.Close();
                            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                        }
                    }

                // If no connections were found, throw an exception
                if (connections.Count == 0)
                    throw new NoConnectionException("No xbox connection detected.");
            }
            finally
            {
                // close the connection
                s.Close();
            }

            // check to make sure that each box has unique connection information...(case sensitive)
            for (int i = 0; i < connections.Count; i++)
                for (int j = i; j < connections.Count; j++)
                    if (i != j && (connections[i].Name == connections[j].Name || connections[i].IP == connections[j].IP))
                        throw new NoConnectionException("Multiple consoles found that have the same connection information.  Please ensure that each box connected to the network has different debug names and ips.");

            return connections;
        }

        /// <summary>
        /// Shared connection function
        /// </summary>
        private void _connect()
        {
            // establish debug session
            connection = new TcpClient();
            connection.ReceiveBufferSize = 20 * 0x100000;
            connection.SendBufferSize = 20 * 0x100000;
            connection.NoDelay = true;           
            foreach (DebugConnection dc in connections)
                if (dc.IP == debugIP)
                {
                    connection.Client.Bind(new IPEndPoint(dc.LocalIP, 0));
                    break;
                }
            connection.Connect(debugIP, notificationPort);
            connected = Ping(100);  // make sure it is successful

            // make sure they are using the current xbdm.dll v7887
            debugMonitorVersion = new Version(SendCommand("dmversion").Message);
            if (DebugMonitorVersion != new Version("1.00.7887.1"))
            {
                Disconnect();   // unsafe to proceed, so disconnect...
                throw new ApiException("Must use xbdm.dll v1.00.7887.1 before connecting");
            }

            // register our notification session
            RegisterNotificationSession(notificationPort);

            // must have for our shitty setmem hack to work ;P
            CreateFile("E:\\fUkM$DeVs", FileMode.Create);

            //initialize components
            memoryStream = new MemoryStream(this);
            memoryReader = new BinaryReader(memoryStream);
            memoryWriter = new BinaryWriter(memoryStream);
            kernel = new XboxKernel(this);
            SetMemory(0xB00292D0, ScriptBufferAddress); // set up the script buffer
            InitializeHistory();
            eeprom = ReadEEPROM();
            GetXboxInformation();
        }

        /// <summary>
        /// Connects to an xbox on the network. If multiple consoles are detected this method 
        /// will attempt to connect to the last connection used. If that connection or information
        /// is unavailable this method will fail.
        /// </summary>
        public void Connect()
        {
            try
            {
                connected = Ping(100); // update connection status
                if (!connected)
                {
                    Disconnect();   // destroy any old connection we might have had

                    // determines the debug names and ips of the connected xbox systems
                    List<DebugConnection> connections = QueryXboxConnections();

                    // attempt to narrow the list down to one connection
                    if (connections.Count > 1)
                    {

                        #region Create a form to allow an Xbox choice selection
                        Form tempForm = new Form() { Size = new Size(200, 400) };
                        ListBox lb = new ListBox() { Dock = DockStyle.Fill  };
                        tempForm.Controls.Add(lb);
                        foreach ( DebugConnection dc in connections) 
                            lb.Items.Add(dc.IP + " [" + dc.Name + "]");
                        tempForm.ShowDialog();
                        #endregion

                        bool found = false;
                        foreach (DebugConnection dbgConnection in connections)
                        {
                            if (LastConnectionUsed == null) break;
                            if (dbgConnection.IP.ToString() == LastConnectionUsed || dbgConnection.Name == LastConnectionUsed)
                            {

                                //store debug info
                                debugName = LastConnectionUsed = dbgConnection.Name;
                                debugIP = dbgConnection.IP;
                                found = true;
                                break;
                            }
                        }
                        if (!found) throw new NoConnectionException("Unable to distinguish between multiple connections. Please turn off all other consoles or try to connect again using a specific ip.");

                    }
                    else if (connections.Count == 1)
                    {
                        //store debug info
                        debugName = LastConnectionUsed = connections[0].Name;
                        debugIP = connections[0].IP;
                    }
                    else throw new NoConnectionException("Unable to detect a connection.");

                    // Call shared connect function
                    _connect();
                }
                else throw new NoConnectionException("You are already connected.");
            }
            catch
            {
                connected = false;
                throw new NoConnectionException("Unable to connect.");
            }
        }

		/// <summary>
		/// Connects to a specified xbox using the box name.
		/// </summary>
		/// <param name="debugInfo">Case insensitive information may either be a debug ip or the name of a specific xbox.</param>
        public void Connect(string xbox)
        {
            try
            {
                connected = Ping(100); // update connection status
                if (!connected)
                {
                    Disconnect();   // destroy any old connection we might have had

                    // determines the debug name and ip of the specified xbox system
                    int index = -1;
                    if (connections.Count == 0)
                        connections = QueryXboxConnections();
                    for (int i = 0; i < connections.Count; i++)
                        if (connections[i].Name.ToLower() == xbox.ToLower() || connections[i].IP.ToString().ToLower() == xbox.ToLower())
                        {
                            index = i;
                            break;
                        }
                    if (index == -1) throw new NoConnectionException("Unable to connect to the specified xbox.");

                    //store debug info
                    debugName = LastConnectionUsed = connections[index].Name;
                    debugIP = connections[index].IP;

                    // Call shared connect function
                    _connect();
                }
            }
            catch
            {
                connected = false;
                throw new NoConnectionException("No xbox connection detected.");
            }
        }

        /// <summary>
        /// Connects to an xbox using the IP address
        /// </summary>
        /// <param name="ip"></param>
        public void ConnectToIP(string ip)
        {
            // Change our string into a byte array
            string[] ipStrs = ip.Split('.');
            byte[] bytes = new byte[4];
            try
            {
                for (int i = 0; i < 4; i++)
                    bytes[i] = byte.Parse(ipStrs[i]);
            }
            catch
            {
                throw new Exception("Invalid IP Address");
            }

            // create our connection
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            connected = false;

            try
            {
                foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    foreach (var ua in i.GetIPProperties().UnicastAddresses)
                    {
                        s.Bind(new IPEndPoint(ua.Address, 0));
                        byte[] sendBuf = { 3, 0 };
                        IPEndPoint IPEnd = new IPEndPoint(new IPAddress(bytes), notificationPort);
                        s.SendTo(sendBuf, IPEnd);

                        // wait for response
                        DateTime before = DateTime.Now;
                        TimeSpan elapse = new TimeSpan();
                        while (s.Available == 0)
                        {
                            Thread.Sleep(0);
                            elapse = DateTime.Now - before;
                            if (elapse.TotalMilliseconds > timeout)
                                break;
                        }

                        if (s.Available != 0)
                        {
                            // parse any information returned
                            byte[] data = new byte[s.Available];
                            EndPoint end = new IPEndPoint(IPEnd.Address, 0);
                            DebugConnection dc = new DebugConnection();
                            while (s.Available > 0)
                            {
                                s.ReceiveFrom(data, ref end);
                                IPEndPoint endpoint = (IPEndPoint)end;
                                dc = new DebugConnection(ua.Address, ((IPEndPoint)end).Address, ASCIIEncoding.ASCII.GetString(data, 2, data.Length - 2).Replace("\0", ""));
                            }

                            // If we don't receive data back, then don't try to connect
                            if (dc.IP == null)
                            {
                                s.Close();
                                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                continue;
                            }

                            //store debug info
                            connections.Clear();
                            connections.Add(dc);
                            debugName = LastConnectionUsed = dc.Name;
                            debugIP = dc.IP;
                            connected = true;

                            // close the connection
                            s.Close();
                            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                            // Call shared connect function
                            _connect();
                            return;
                        }
                        // close the connection
                        s.Close();
                        s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    }
            }
            finally
            {
                s.Close();
            }
            if (!connected)
                throw new NoConnectionException("No xbox connection detected.");

        }

        /// <summary>
        /// Registers a notification session on the specified port.
        /// </summary>
        /// <param name="port"></param>
		public void RegisterNotificationSession(int port)
		{
			try
			{
                if (notificationSessionEnabled)
                {
                    IPAddress addr = Dns.GetHostAddresses(Dns.GetHostName())[0];
                    IPEndPoint NotificationEndpoint = new IPEndPoint(addr, port);
                    notificationListener = new TcpListener(NotificationEndpoint);
                    notificationListener.Start();
                    StatusResponse res = SendCommand("notifyat port=" + port);
                    if (res.Type == ResponseType.SingleResponse && notificationListener.Pending())
                    {
                        notificationSession = new TcpClient();
                        notificationSession.Client = notificationListener.AcceptSocket();
                        ReceiveNotifications();
                    }
                }
			}
            catch { notificationSessionEnabled = false; }
		}

		/// <summary>
		/// Disconnects from the xbox.
		/// </summary>
		public void Disconnect()
		{
            try
            {
                // attempt to clean up if still connected
                if (Ping())
                {
                    if (AllocationTable.Count > 0) FreeAllMemory();    // release any memory we have allocated here
                    SendCommand("notifyat port=" + notificationPort + " drop"); // drop the notification session
                    SendCommand("bye"); // we cant leave without saying goodbye ;)
                }
            }
            catch { }
            finally
            {
                connected = false;

                // cleanup 





            }
		}

        /// <summary>
        /// Attempts to re-establish a connection with the currently selected xbox console.
        /// </summary>
        /// <param name="timeout"></param>
		public void Reconnect(int timeout)
		{
			Disconnect();   // close our old connection
            DateTime Before = DateTime.Now;

            while (!connected)
            {
                try
                {
                    Connect(DebugIP.ToString());    // create a new one using the current connection information
                }
                catch
                {
                    DateTime After = DateTime.Now;
                    TimeSpan Elapse = After - Before;
                    if (Elapse.Milliseconds > timeout)
                    {
                        Disconnect();
                        throw new TimeoutException("Connection lost - unable to reconnect.");
                    }
                }
            }
		}

        /// <summary>
        /// Re-establishes a connection with the currently selected xbox console.
        /// </summary>
        public void Reconnect()
        {
            Reconnect(1000);
        }

        /// <summary>
        /// Checks the connection status between xbox and pc.
        /// This function only checks what YeloDebug believes to be the current connection status.
        /// For a true status check you will need to ping the xbox regularly.
        /// </summary>
        public void ConnectionCheck()
        {
            //takes too much time to ping when used by continuously called functions 
            //if connection drops attempt to reconnect, otherwise fuck them, let it crash and burn...
            if (!connected) Reconnect(1000);    // try to re-establish a connection
        }

        /// <summary>
        /// Retrieves xbox connection status. Average execution time of 2200 executions per second, or one every half millisecond.
        /// </summary>
        /// <param name="waitTime">Time to wait for a response</param>
        /// <returns>Connection status</returns>
        public bool Ping(int waitTime)
        {
            int oldTimeOut = timeout;
            try
            {
                if (connection.Available > 0)
                    connection.Client.Receive(new byte[connection.Available]);    // keeps the connection in sync :D

                connection.Client.Send(ASCIIEncoding.ASCII.GetBytes(Environment.NewLine));
                timeout = waitTime;

                // wait for all of the response "400- Unknown Command\r\n"
                if (connection.Available < 0x16)   // avoid waiting if we already have data in our buffer...
                {
                    DateTime before = DateTime.Now;
                    TimeSpan elapse = new TimeSpan();

                    while (connection.Available < 0x16)
                    {
                        Thread.Sleep(0);
                        elapse = DateTime.Now - before;
                        if (elapse.TotalMilliseconds > timeout)
                            throw new TimeoutException("Operation timed out.");
                    }
                }

                if (connection.Available > 0)
                    connection.Client.Receive(new byte[connection.Available]);   // get rid of any garbage
                connected = true;
                return true;
            }
            catch
            {
                connected = false;
                Connection.Close();
                return false;
            }
            finally
            {
                timeout = oldTimeOut;   // make sure to restore old timeout
            }
        }

        /// <summary>
        /// Retrieves xbox connection status. Average execution time of 2200 executions per second, or one every half millisecond.
        /// </summary>
        /// <returns>Connection status</returns>
        public bool Ping()
        {
            return Ping(Timeout);
        }

        /// <summary>
        /// Reboots the xbox with the specified BootFlag.
        /// </summary>
        /// <param name="flag"></param>
		public void Reboot(BootFlag flag)
		{
			switch (flag)
			{
				case BootFlag.Cold:		SendCommand("reboot");			break;
				case BootFlag.Warm:		SendCommand("reboot warm");		break;
				case BootFlag.NoDebug:	SendCommand("reboot nodebug");	break;
				case BootFlag.Wait:		SendCommand("reboot wait");		break;
				case BootFlag.Stop:		SendCommand("reboot stop");		break;
				case BootFlag.Current:
					FlushSocketBuffer();
                    connection.Client.Send(ASCIIEncoding.ASCII.GetBytes("magicboot title=\"" + XbeInfo.LaunchPath + "\" debug" + Environment.NewLine));
                    if (connection.Available > 0)
                        connection.Client.Receive(new byte[connection.Available]);
					break;
			}
            Reconnect(15000);
		}

		/// <summary>
		/// Performs a warm reboot on the xbox.
		/// </summary>
        public void Reboot()
        {
            Reboot(BootFlag.Warm);
        }

        /// <summary>
        /// Launches the specified xbox title and attempts to establish a new connection with that title.
        /// </summary>
        /// <param name="path"></param>
        public void LaunchTitle(string path)
        {
            FlushSocketBuffer();
            connection.Client.Send(ASCIIEncoding.ASCII.GetBytes("magicboot title=\"" + path + "\" debug" + Environment.NewLine));
            if (connection.Available > 0)
                connection.Client.Receive(new byte[connection.Available]);
            Reconnect(15000);
        }

        /// <summary>
        /// Reads all static xbox information that will remain constant throughout a session.
        /// </summary>
        private void GetXboxInformation()
        {
            // xbox video encoder type
            if (CallAddressEx(Kernel.HalReadSMBusValue, null, true, SMBusDevices.VideoEncoderXcalibur, VideoEncoderCommand.Detect, 0, scratchBuffer) == 0) videoEncoderType = VideoEncoder.Xcalibur;
            else if (CallAddressEx(Kernel.HalReadSMBusValue, null, true, SMBusDevices.VideoEncoderConnexant, VideoEncoderCommand.Detect, 0, scratchBuffer) == 0) videoEncoderType = VideoEncoder.Connexant;
            else if (CallAddressEx(Kernel.HalReadSMBusValue, null, true, SMBusDevices.VideoEncoderFocus, VideoEncoderCommand.Detect, 0, scratchBuffer) == 0) videoEncoderType = VideoEncoder.Focus;
            else videoEncoderType = VideoEncoder.Unknown;

            // xbox version info
            CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Version, 0, scratchBuffer);
            CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Version, 0, scratchBuffer + 1);
            CallAddressEx(Kernel.HalReadSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Version, 0, scratchBuffer + 2);
            string code = ASCIIEncoding.ASCII.GetString(GetMemory(scratchBuffer, 3));
            switch (code)
            {
                case "01D":
                case "D01":
                case "1D0":
                case "0D1": version = "Xbox Development Kit"; break;
                case "P01": version = "Xbox v1.0"; break;
                case "P05": version = "Xbox v1.1"; break;
                case "P11":
                case "1P1":
                case "11P":
                    if (videoEncoderType == VideoEncoder.Focus) version = "1.4";
                    else version = "Xbox v1.2/1.3"; break;
                case "P2L": version = "Xbox v1.6"; break;
                case "B11":
                case "DBG": version = "Xbox Debug Kit"; break;   // green

                default: version = code + ": Unknown Xbox"; break;
            }

            // processor information
            SetMemory(ScriptBufferAddress, Util.StringToHexBytes("B8010000000FA2A300000100B80000DB02C21000"));
            SendCommand("crashdump");
            uint eax = GetUInt32(0x10000);
            processorInformation.Stepping = eax & 0xf;
            processorInformation.Model = (eax >> 4) & 0xf;
            processorInformation.Family = (eax >> 8) & 0xf;
            if (processorInformation.Model == 11) cpuFrequency = "1.48 GHz"; // DreamX console
            else if (processorInformation.Model == 8 && processorInformation.Stepping == 6) cpuFrequency = "1.00 GHz";   // Intel Pentium III Coppermine
            else cpuFrequency = "733.33 MHz";

            // hardware info
            uint ver = GetUInt32(Kernel.HardwareInfo);
            string vstr = Convert.ToString(ver, 16).PadLeft(8, '0');
            string vstr2 = Util.HexBytesToString(GetMemory(Kernel.HardwareInfo + 4, 2)).Insert(2, " ");
            hardwareInfo = vstr + " " + vstr2;

            macAddress = BitConverter.ToString(eeprom, 0x40, 6).Replace('-', ':');

            serialNumber = Convert.ToUInt64(ASCIIEncoding.ASCII.GetString(eeprom, 0x34, 12));
            lanKey = GetMemory(Kernel.XboxLANKey, 16);
            signatureKey = GetMemory(Kernel.XboxSignatureKey, 16);
            eepromKey = GetMemory(Kernel.XboxEEPROMKey, 16);
            hardDriveKey = GetMemory(Kernel.XboxHDKey, 16);
            
            byte[] hdModelInfo = GetMemory(Kernel.HalDiskModelNumber, 40);
            uint unk1 = BitConverter.ToUInt32(hdModelInfo, 0);
            uint index = BitConverter.ToUInt32(hdModelInfo, 4);
            hardDriveModel = ASCIIEncoding.ASCII.GetString(hdModelInfo, 8, 32).Trim().Replace("\0", "");

            byte[] hdSerialInfo = GetMemory(Kernel.HalDiskSerialNumber, 32);
            unk1 = BitConverter.ToUInt32(hdSerialInfo, 0);
            index = BitConverter.ToUInt32(hdSerialInfo, 4);
            hardDriveSerial = ASCIIEncoding.ASCII.GetString(hdSerialInfo, 8, 16).Trim().Replace("\0", "");

            alternateSignatureKeys = new byte[16][];
            byte[] keyData = GetMemory(Kernel.XboxAlternateSignatureKeys, 256);
            for (int i = 0; i < 16; i++)
            {
                alternateSignatureKeys[i] = new byte[16];
                Buffer.BlockCopy(keyData, i * 16, alternateSignatureKeys[i], 0, 16);
            }

            StringBuilder krnlStr = new StringBuilder();
            byte[] krnlVersion = GetMemory(Kernel.XboxKrnlVersion, 8);
            krnlStr.AppendFormat("{0}.{1}.{2}.{3}",
                BitConverter.ToUInt16(krnlVersion, 0),
                BitConverter.ToUInt16(krnlVersion, 2),
                BitConverter.ToUInt16(krnlVersion, 4),
                BitConverter.ToUInt16(krnlVersion, 6)
                );
            kernelVersion = new Version(krnlStr.ToString());
            
            SendCommand("modules");
            modules = new List<ModuleInfo>();
            string line = ReceiveSocketLine();
            while (line[0] != '.')
            {
                ModuleInfo module = new ModuleInfo();
                module.Sections = new List<ModuleSection>();
                List<object> info = Util.ExtractResponseInformation(line);
                module.Name = (string)info[0];
                module.BaseAddress = (uint)info[1];
                module.Size = (uint)info[2];
                module.Checksum = (uint)info[3];

                module.TimeStamp = Util.TimeStampToUniversalDateTime((uint)info[4]);
                modules.Add(module);
                line = ReceiveSocketLine();
            }
            foreach (ModuleInfo module in modules)
            {
                SendCommand("modsections name={0}", module.Name);
                line = ReceiveSocketLine();
                while (line[0] != '.')
                {
                    ModuleSection modSection = new ModuleSection();
                    List<object> info = Util.ExtractResponseInformation(line);
                    modSection.Name = (string)info[0];
                    modSection.Base = (uint)info[1];
                    modSection.Size = (uint)info[2];
                    modSection.Index = (uint)info[3];
                    modSection.Flags = (uint)info[4];
                    module.Sections.Add(modSection);
                    line = ReceiveSocketLine();
                }
            }
            string hex = SendCommand("altaddr").Message.Substring(7);
            titleIP = new IPAddress(Util.StringToHexBytes(hex));

            linkStatus = (LinkStatus)CallAddressEx(Kernel.PhyGetLinkState, null, true, 0);

            // Attempt to load title/game info. Will throw exception if we are in Debug Dash
            try
            {
                getTitleInformation();
            }
            catch { }

        }

        public void getTitleInformation()
        {
            processID = Convert.ToUInt32(SendCommand("getpid").Message.Substring(6), 16);

            SendCommand("xbeinfo running");
            xbeInfo = new XbeInfo();
            string line = ReceiveSocketLine();
            XbeInfo.TimeStamp = Util.TimeStampToUniversalDateTime((uint)Util.GetResponseInfo(line, 0));
            XbeInfo.Checksum = (uint)Util.GetResponseInfo(line, 1);
            line = ReceiveSocketLine();
            XbeInfo.LaunchPath = (string)Util.GetResponseInfo(line, 0);
            ReceiveSocketLine();    // '.'
        }
		#endregion

		#region Command Processing

		/// <summary>
        /// Clear everything from the receive buffer.
        /// Call this before you send anything to the xbox to help keep the channel in sync.
		/// </summary>
		public void FlushSocketBuffer()
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");
            try
            {
                if (connection.Available > 0)
                    connection.Client.Receive(new byte[connection.Available]);
            }
            catch
            {
                Disconnect();
                throw new NoConnectionException();
            }
		}

		/// <summary>
		/// Clear a specified amount from the receive buffer.
		/// If amount in buffer is less than requested size, everything will be cleared.
		/// </summary>
		/// <param name="size">Size to flush</param>
		public void FlushSocketBuffer(int size)
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");
            try
            {
                if (connection.Available > 0 && connection.Available >= size)
                    connection.Client.Receive(new byte[size]);
                else if (connection.Available == 0) return;
                else if (connection.Available > 0) connection.Client.Receive(new byte[connection.Available]);
            }
            catch
            {
                Disconnect();
                throw new NoConnectionException();
            }
		}

		/// <summary>
		/// Waits for data to be received.  During execution this method will enter a spin-wait loop and appear to use 100% cpu when in fact it is just a suspended thread.  
        /// This is much more efficient than waiting a millisecond since most commands take fractions of a millisecond.
        /// It will either resume after the condition is met or throw a timeout exception.
		/// </summary>
		/// <param name="type">Wait type</param>
		public void Wait(WaitType type)
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");

			DateTime before = DateTime.Now;
			TimeSpan elapse = new TimeSpan();

			switch (type)
			{
				case WaitType.Partial:
					while (connection.Available == 0)
					{
						Thread.Sleep(0);
						elapse = DateTime.Now - before;
                        if (elapse.TotalMilliseconds > timeout)
                        {
                            Disconnect();
                            throw new TimeoutException("Operation timed out.");
                        }
					}
					break;

				case WaitType.Full:

					// do a partial wait first
					while (connection.Available == 0)
					{
						Thread.Sleep(0);
						elapse = DateTime.Now - before;
                        if (elapse.TotalMilliseconds > timeout)
                        {
                            Disconnect();
                            throw new TimeoutException("Operation timed out.");
                        }
					}

					// wait for rest of data to be received
					int avail = connection.Available;
					while (connection.Available != avail)
					{
						avail = connection.Available;
						Thread.Sleep(0);
					}
					break;
			}
		}

		/// <summary>
		/// Waits for a specified amount of data to be received.  Use with file IO.
		/// </summary>
		/// <param name="targetLength">Amount of data to wait for</param>
		public void Wait(int targetLength)
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");
			if (connection.Available >= targetLength) return;   // avoid waiting if we already have data in our buffer...

			DateTime before = DateTime.Now;
			TimeSpan elapse = new TimeSpan();

			while (connection.Available < targetLength)
			{
				Thread.Sleep(0);
				elapse = DateTime.Now - before;
                if (elapse.TotalMilliseconds > timeout)
                {
                    Disconnect();
                    throw new TimeoutException("Operation timed out.");
                }
			}
		}

		// TODO: make this a blocking call that times out.  have it wait for "\r\n"
		/// <summary>
		/// Receives a line of text from the xbox.
		/// </summary>
		/// <returns></returns>
		public string ReceiveSocketLine()
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");
			const int bData_sizeof = 1024;

			// wait for data to appear in the receive buffer
			Wait(WaitType.Partial);

            try
            {
                string sData;
                byte[] bData = new byte[bData_sizeof];

                try
                {
                    // peek at the received data
                    int avail = connection.Available;   // only get once
                    if (avail < bData_sizeof)
                        connection.Client.Receive(bData, avail, SocketFlags.Peek);
                    else
                        connection.Client.Receive(bData, bData_sizeof, SocketFlags.Peek);

                    // determine the length of the string
                    sData = ASCIIEncoding.ASCII.GetString(bData);
                    sData = sData.Substring(0, sData.IndexOf("\r\n") + 2);
                }
                catch   // newline not detected...try waiting a little more
                {
                    Thread.Sleep(1);    // this could really hurt performance...make a blocking call with a spin-wait instead!

                    // peek at the received data
                    int avail = connection.Available;   // only get once
                    if (avail < bData_sizeof)
                        connection.Client.Receive(bData, avail, SocketFlags.Peek);
                    else
                        connection.Client.Receive(bData, bData_sizeof, SocketFlags.Peek);

                    // determine the length of the string
                    sData = ASCIIEncoding.ASCII.GetString(bData);
                    sData = sData.Substring(0, sData.IndexOf("\r\n") + 2);
                }
                connection.Client.Receive(bData, sData.Length, SocketFlags.None);
                return sData;
            }
            catch
            {
                Disconnect();
                throw new NoConnectionException();
            }
		}

        /// <summary>
        /// Receives multiple lines of text from the xbox.
        /// </summary>
        /// <returns></returns>
		public string ReceiveMultilineResponse()
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");
			StringBuilder response = new StringBuilder();
			string line = string.Empty;
			while (true)
			{
				line = ReceiveSocketLine();
				if (line[0] == '.')
					break;
				else response.Append(line);
			}
			return response.ToString();
		}

		/// <summary>
		/// Receives a notification if one is present.
		/// </summary>
        /// <returns></returns>
		public bool ReceiveNotification()
		{
            if (connection == null) throw new NoConnectionException("Must connect first.");

            // tell user if notification session has dropped...
            if (!NotificationSession.Connected) throw new ApiException("Notification session has been dropped.");

            try
            {
                const int bData_sizeof = 256;

                // peek at the received data
                int avail = NotificationSession.Available;
                byte[] bData = new byte[bData_sizeof];
                if (avail == 0) return false;
                else if (avail < bData_sizeof)
                    NotificationSession.Client.Receive(bData, avail, SocketFlags.Peek);
                else
                    NotificationSession.Client.Receive(bData, bData_sizeof, SocketFlags.Peek);

                // extract the notification
                string notification = string.Empty;
                string sData = ASCIIEncoding.ASCII.GetString(bData);
                notification = sData.Substring(0, sData.IndexOf("\r\n"));
                NotificationSession.Client.Receive(bData, notification.Length + 2, SocketFlags.None);

                notifications.Add(notification);
                return true;
            }
            catch
            {
                Disconnect();
                throw new NoConnectionException();
            }
		}

        /// <summary>
        /// Receives any notifications that may be present.
        /// </summary>
        public void ReceiveNotifications()
        {
            if (connection == null) throw new NoConnectionException("Must connect first.");

            while (ReceiveNotification());
        }

        // IMPORTANT: make a blocking call (fucks up when relying on the hard disk)
        // if single line, wait until /r/n
        // if multiple line, wait for "./r/n"
        // if binary response, let user Wait(length)
        // todo: add timeouts

        /// <summary>
        /// Waits for a status response to be received from the xbox.
        /// </summary>
        /// <returns>Status response</returns>
		public StatusResponse ReceiveStatusResponse()
		{
            // get response type
            // then determine how much longer to wait
            // no wait for binary responses (since they rely on sizes instead anyways)
            // full for single or multiline (will be slow for large getmem values...but we use getmem2 ;))

            //DateTime before = DateTime.Now;
            //TimeSpan elapse = new TimeSpan();
            //while (Connection.Available < 3)
            //{
            //    Thread.Sleep(0);
            //    elapse = DateTime.Now - before;
            //    if (elapse.TotalMilliseconds > Timeout)
            //        throw new TimeoutException("Operation timed out.");
            //}
            //byte[] statusCode = new byte[3];
            //Connection.Client.Receive(statusCode, 3, SocketFlags.Peek);
            //ResponseType type = (ResponseType)Convert.ToUInt32(ASCIIEncoding.ASCII.GetString(statusCode));

            //if (type == ResponseType.SingleResponse)
            //{
            //    byte[] recvbuf;
            //    bool searchForReturn = true;
            //    while (searchForReturn)
            //    {
            //        // get max line return 66
            //        if (Connection.Available >= 66) recvbuf = new byte[66];
            //        else recvbuf = new byte[Connection.Available];
            //        Connection.Client.Receive(recvbuf, recvbuf.Length, SocketFlags.Peek);
            //        for (int i = 0; i < recvbuf.Length; i++)
            //            if (recvbuf[i] == 0xD)
            //            {
            //                searchForReturn = false;
            //                break;
            //            }
            //    }
            //}
            //else if (type == ResponseType.MultiResponse)
            //{

            //}



            if (connection == null) throw new NoConnectionException("Must connect first.");
			string response = ReceiveSocketLine();
			response = response.Remove(response.Length - 2);    // remove line carriages from end...

			if (response.Length > 0)
				return new StatusResponse(response, (ResponseType)Convert.ToInt32(response.ToString().Remove(3)), response.Remove(0, 5).ToString());
			else
			{
				connected = false;
                return new StatusResponse(response, ResponseType.UndefinedError, "No connection detected.");
			}
		}

        /// <summary>
        /// Sends a command to the xbox.
        /// </summary>
        /// <param name="command">Command to be sent</param>
        /// <param name="args">Arguments</param>
        /// <returns>Status response</returns>
		public StatusResponse SendCommand(string command, params object[] args)
        {
            ConnectionCheck();
            FlushSocketBuffer();

            try
            {
                connection.Client.Send(ASCIIEncoding.ASCII.GetBytes(string.Format(command, args) + Environment.NewLine));
            }
            catch
            {
                Disconnect();
                throw new NoConnectionException();
            }

            StatusResponse response = ReceiveStatusResponse();
            // sendcommand(stop) - already stopped...counts as an error...although it really doesnt affect anything negatively
            // sendcommand(go) - not stopped
            // consider catching these and doing nothing instead...

            // notify user if something isnt right
            if (!response.Success)    //&& response.Message != "not stopped" or maybe command != "stop"
            {
                throw new ApiException(response.Message);
            }
            return response;
        }

        
        /// <summary>
        /// Sends binary data to the xbox.
        /// </summary>
        /// <param name="data"></param>
        public void SendBinaryData(byte[] data)
        {
            ConnectionCheck();
            FlushSocketBuffer();
            connection.Client.Send(data);
        }

        /// <summary>
        /// Sends binary data of specified length to the xbox.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void SendBinaryData(byte[] data, int length)
        {
            ConnectionCheck();
            FlushSocketBuffer();
            connection.Client.Send(data, length, SocketFlags.None);
        }

        /// <summary>
        /// Receives all available binary data sent from the xbox.
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBinaryData()
        {
            if (connection.Available > 0)
            {
                byte[] binData = new byte[connection.Available];
                connection.Client.Receive(binData, binData.Length, SocketFlags.None);
                return binData;
            }
            else return null;
        }

        /// <summary>
        /// Receives binary data of specified size sent from the xbox.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] ReceiveBinaryData(int size)
        {
            Wait(size);
            byte[] binData = new byte[size];
            connection.Client.Receive(binData, binData.Length, SocketFlags.None);
            return binData;
        }

        /// <summary>
        /// Receives binary data of specified size sent from the xbox.
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveBinaryData(ref byte[] data)
        {
            Wait(data.Length);
            connection.Client.Receive(data, data.Length, SocketFlags.None);
        }
        
		#endregion

		#region Filesystem

        /// <summary>
        /// Creates a standard xbox file stream.
        /// </summary>
        public class FileStream : Stream
        {
            #region Fields
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Xbox Client;

            protected uint position;
            public override long Position
            {
                get { return position; }
                set { position = (uint)value; }
            }
            protected string FileName;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new file stream using a client connection to a debug xbox.
            /// </summary>
            /// <param name="client">Connection to use.</param>
            /// <param name="fileName">Name of the file to expose stream to.</param>
            /// <param name="mode">File create disposition.</param>
            public FileStream(Xbox client, string fileName, FileMode mode)
            {
                this.Client = client;
                if (client == null || !client.Connected)
                    throw new NoConnectionException("Must connect first.");

                FileName = fileName;
                position = 0;
                client.CreateFile(fileName, mode);
            }
            public FileStream(Xbox client, string fileName)
            {
                this.Client = client;
                if (client == null)
                    throw new NoConnectionException("Must connect first.");

                this.FileName = fileName;
                position = 0;
                client.CreateFile(fileName, FileMode.Create);   // creates the file by default
            }
            #endregion

            #region Methods
            public override bool CanRead { get { return Client.Connected; } }
            public override bool CanSeek { get { return Client.Connected; } }
            public override bool CanWrite { get { return Client.Connected; } }
            public override void Flush() { throw new UnsupportedException(); }
            public override long Length
            {
                get { return Client.GetFileSize(FileName); }    // possibly get once, then keep track internally...
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin: return position = (uint)offset;
                    case SeekOrigin.Current: return position += (uint)offset;
                    case SeekOrigin.End: return position = (uint)Length - (uint)offset;  // not recommended to be used since it has to talk to xbox to get length
                    default: throw new Exception("Invalid SeekOrigin.");
                }
            }

            public override void SetLength(long value)
            {
                Client.SetFileSize(FileName, (int)value);
            }

            public unsafe override int Read(byte[] buffer, int offset, int count)
            {
                int bytesRead = 0;
                Client.ReadFilePartial(FileName, (int)position + offset, ref buffer, count, ref bytesRead);
                position += (uint)bytesRead;
                return bytesRead;
            }

            public unsafe override void Write(byte[] buffer, int offset, int count)
            {
                Client.WriteFilePartial(FileName, (int)position + offset, ref buffer, count);
                position += (uint)count;
            }
            #endregion
        };

        /// <summary>
        /// Dont use this, higher-level methods are available.  Use FileStream instead.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="read"></param>
		private void ReadFilePartial(string name, int offset, ref byte[] buffer, int length, ref int read)
		{
            int bufferSize = 0x1000 * 64; //0xA0000;                // TODO: experiment with different buffer sizes
			int iterations = length / bufferSize;
			int remainder = length % bufferSize;
			int index = 0;

			StatusResponse Response;

			for (int i = 0; i < iterations; i++)
			{
				Response = SendCommand("getfile name={0} offset={1} size={2}", name, offset + index, bufferSize);
				if (Response.Type == ResponseType.BinaryResponse)
				{
					Wait(4);
					int bytesRead;
					byte[] temp = new byte[4];
					connection.Client.Receive(temp, 4, SocketFlags.None);
					bytesRead = BitConverter.ToInt32(temp, 0);
					read += bytesRead;

					Wait((int)bytesRead);
					connection.Client.Receive(buffer, index, bufferSize, SocketFlags.None);
					index += bufferSize;
				}
				else throw new Exceptions.ApiException("SendCommand");
			}

            if (remainder > 0)
            {
                Response = SendCommand("getfile name={0} offset={1} size={2}", name, offset + index, remainder);
                if (Response.Type == ResponseType.BinaryResponse)
                {
                    Wait(4);
                    int bytesRead;
                    byte[] temp = new byte[4];
                    connection.Client.Receive(temp, 4, SocketFlags.None);
                    bytesRead = BitConverter.ToInt32(temp, 0);


                    read += bytesRead;

                    Wait((int)bytesRead);
                    connection.Client.Receive(buffer, index, remainder, SocketFlags.None);
                }
                else throw new Exceptions.ApiException("SendCommand");
            }
		}

        /// <summary>
        /// Dont use this, higher-level methods are available.  Use FileStream instead.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
		private void WriteFilePartial(string name, int offset, ref byte[] buffer, int length)
        {
            int bufferSize = 0xA0000;                // TODO: experiment with different buffer sizes
            int iterations = length / bufferSize;
            int remainder = length % bufferSize;
            int index = 0;

            StatusResponse Response;

            for (int i = 0; i < iterations; i++)
            {
                Response = SendCommand("writefile name={0} offset={1} length={2}", name, offset + index, bufferSize);
                if (Response.Type == ResponseType.ReadyForBinary)
                {
                    connection.Client.Send(buffer, offset, bufferSize, SocketFlags.None);
                    Response = ReceiveStatusResponse();
                    // check for failure
                    index += bufferSize;
                }
                else throw new Exceptions.ApiException("SendCommand");
            }

            if (remainder > 0)
            {
                Response = SendCommand("writefile name={0} offset={1} length={2}", name, offset + index, remainder);
                if (Response.Type == ResponseType.ReadyForBinary)
                {
                    connection.Client.Send(buffer, offset, remainder, SocketFlags.None);
                    Response = ReceiveStatusResponse();
                    // check for failure - parse message and determine bytes written, then return 
                    index += bufferSize;
                }
                else throw new Exceptions.ApiException("SendCommand");
            }
        }

        /// <summary>
        /// Dont use this, higher-level methods are available.  Use GetDriveFreeSpace or GetDriveSize instead.
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="freeBytes"></param>
        /// <param name="driveSize"></param>
        /// <param name="totalFreeBytes"></param>
        private void GetDriveInformation(Drive drive, out ulong freeBytes, out ulong driveSize, out ulong totalFreeBytes)
		{
            freeBytes = 0; driveSize = 0; totalFreeBytes = 0;
            SendCommand("drivefreespace name=\"{0}\"", drive.ToString() + ":\\");

			string msg = ReceiveMultilineResponse();
			freeBytes = Convert.ToUInt64(msg.Substring(msg.IndexOf("freetocallerlo") + 17, 8), 16);
			freeBytes |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("freetocallerhi") + 17, 8), 16) << 32);

			driveSize = Convert.ToUInt64(msg.Substring(msg.IndexOf("totalbyteslo") + 15, 8), 16);
			driveSize |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("totalbyteshi") + 15, 8), 16) << 32);

			totalFreeBytes = Convert.ToUInt64(msg.Substring(msg.IndexOf("totalfreebyteslo") + 19, 8), 16);
			totalFreeBytes |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("totalfreebyteshi") + 19, 8), 16) << 32);
		}

		/// <summary>
		/// Retrieves xbox drive freespace.
		/// </summary>
		/// <param name="drive">Drive name.</param>
		/// <returns>Free space available.</returns>
		public ulong GetDriveFreeSpace(Drive drive)
		{
			ulong FreeBytes = 0, DriveSize = 0, TotalFreeBytes = 0;
            GetDriveInformation(drive, out FreeBytes, out DriveSize, out TotalFreeBytes);
			return FreeBytes;
		}

		/// <summary>
		/// Retrieves xbox drive size.
		/// </summary>
		/// <param name="drive">Drive name.</param>
		/// <returns>Total space available.</returns>
        public ulong GetDriveSize(Drive drive)
		{
			ulong FreeBytes = 0, DriveSize = 0, TotalFreeBytes = 0;
            GetDriveInformation(drive, out FreeBytes, out DriveSize, out TotalFreeBytes);
			return DriveSize;
		}

        /// <summary>
        /// Gets the current file access count.
        /// </summary>
        /// <returns></returns>
        public int GetFileAccessCount() { return GetInt32(0xB002BAA8); }

        /// <summary>
        /// Gets a list of partitions on the xbox hard drive.
        /// </summary>
        public List<string> GetPartitions()
        {
            int oldTimeout = timeout;   // sometimes hdd can be slow so we increase our timeout
            timeout = 10000;
            List<string> List = new List<string>();
            StatusResponse response = SendCommand("drivelist");

            for (int i = 0; i < response.Message.Length; i++)
                List.Add(response.Message[i] + ":\\");

            List.Sort();

            timeout = oldTimeout;
            return List;
        }

		/// <summary>
		/// Retrieves files that belong to a given directory.
		/// </summary>
		/// <param name="name">Directory name.</param>
		/// <returns>List of files.</returns>
		public List<FileInformation> GetDirectoryList(string name)
		{
			List<FileInformation> files = new List<FileInformation>();

			StatusResponse response = SendCommand("dirlist name=\"{0}\"", name);
			if (response.Type == ResponseType.MultiResponse)
			{
				string msg = ReceiveSocketLine();
				while (msg[0] != '.')
				{
					FileInformation info = new FileInformation();

					info.Name = msg.Substring(msg.IndexOf("\"") + 1, msg.LastIndexOf("\"") - msg.IndexOf("\"") - 1);

					// devs fucked up size output so we need to parse carefully... ;X
					int sizehistart = msg.IndexOf("sizehi") + 9;
					int sizehiend = msg.IndexOf("sizelo") - 1;
					int sizelostart = sizehiend + 10;
					int sizeloend = msg.IndexOf("createhi") - 1;

					info.Size = Convert.ToUInt64(msg.Substring(sizelostart, sizeloend - sizelostart), 16);
					info.Size |= (Convert.ToUInt64(msg.Substring(sizehistart, sizehiend - sizehistart), 16) << 32);

					ulong createtime;
					createtime = Convert.ToUInt64(msg.Substring(msg.IndexOf("createlo") + 11, 8), 16);
					createtime |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("createhi") + 11, 8), 16) << 32);
					info.CreationTime = DateTime.FromFileTime((long)createtime);

					ulong changetime;
					changetime = Convert.ToUInt64(msg.Substring(msg.IndexOf("changelo") + 11, 8), 16);
					changetime |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("changehi") + 11, 8), 16) << 32);
					info.ChangeTime = DateTime.FromFileTime((long)changetime);

					if (msg.Contains("directory"))	info.Attributes |= FileAttributes.Directory;
					else							info.Attributes |= FileAttributes.Normal;

					if (msg.Contains("readonly"))	info.Attributes |= FileAttributes.ReadOnly;
					if (msg.Contains("hidden"))		info.Attributes |= FileAttributes.Hidden;

					files.Add(info);
					msg = ReceiveSocketLine();
				}
			}
			return files;
		}

		/// <summary>
		/// Creates a directory on the xbox.
		/// </summary>
		/// <param name="name">Directory name.</param>
		public void CreateDirectory(string name)
		{
			SendCommand("mkdir name=\"{0}\"", name);
		}

		/// <summary>
		/// Deletes a directory on the xbox.
		/// </summary>
		/// <param name="name">Directory name.</param>
		public void DeleteDirectory(string name)
		{
			SendCommand("delete name=\"{0}\" dir", name);
		}

        /// <summary>
        /// Determines if the given file exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
		public bool FileExists(string fileName)
		{
            try
            {
                SendCommand("getfileattributes name=\"{0}\"", fileName);
                ReceiveMultilineResponse();
                return true;
            }
            catch { return false; }
		}

		/// <summary>
		/// Sends a file to the xbox.
		/// </summary>
		/// <param name="localName">PC file name.</param>
		/// <param name="remoteName">Xbox file name.</param>
		public void SendFile(string localName, string remoteName)
		{
            System.IO.FileStream lfs = new System.IO.FileStream(localName, FileMode.Open);
            byte[] fileData = new byte[connection.Client.SendBufferSize];
            SendCommand("sendfile name=\"{0}\" length={1}", remoteName, lfs.Length);

            int mainIterations = (int)lfs.Length / connection.Client.SendBufferSize;
            int remainder = (int)lfs.Length % connection.Client.SendBufferSize;

            for (int i = 0; i < mainIterations; i++)
            {
                lfs.Read(fileData, 0, fileData.Length);
                SendBinaryData(fileData);
            }
            lfs.Read(fileData, 0, remainder);
            SendBinaryData(fileData, remainder);
        
            lfs.Close();
		}

        /// <summary>
        /// Receives a file from the xbox.
        /// </summary>
        /// <param name="localName">PC file name.</param>
        /// <param name="remoteName">Xbox file name.</param>
        public void ReceiveFile(string localName, string remoteName)
		{
            SendCommand("getfile name=\"{0}\"", remoteName);
            int fileSize = BitConverter.ToInt32(ReceiveBinaryData(4), 0);
            System.IO.FileStream lfs = new System.IO.FileStream(localName, FileMode.CreateNew);
            byte[] fileData = new byte[connection.Client.ReceiveBufferSize];

            int mainIterations = fileSize / connection.Client.ReceiveBufferSize;
            int remainder = fileSize % connection.Client.ReceiveBufferSize;

            for (int i = 0; i < mainIterations; i++)
            {
                fileData = ReceiveBinaryData(fileData.Length);
                lfs.Write(fileData, 0, fileData.Length);
            }
            fileData = ReceiveBinaryData(remainder);
            lfs.Write(fileData, 0, remainder);

			lfs.Close();
		}

		/// <summary>
		/// Renames or moves a file on the xbox.
		/// </summary>
		/// <param name="oldFileName">Old file name.</param>
		/// <param name="newFileName">New file name.</param>
		public void RenameFile(string oldFileName, string newFileName)
		{
			SendCommand("rename name=\"{0}\" newname=\"{1}\"", oldFileName, newFileName);
		}

		/// <summary>
		/// Creates a file on the xbox.
		/// </summary>
		/// <param name="fileName">File to create.</param>
		/// <param name="createDisposition">Creation options.</param>
		public void CreateFile(string fileName, FileMode createDisposition)
		{
            if (createDisposition == FileMode.Open) { if (!FileExists(fileName)) throw new Exception("File does not exist."); }
            else if (createDisposition == FileMode.Create)      SendCommand("fileeof name=\"" + fileName + "\" size=0 cancreate");
            else if (createDisposition == FileMode.CreateNew)   SendCommand("fileeof name=\"" + fileName + "\" size=0 mustcreate");
            else throw new UnsupportedException("Unsupported FileMode.");
		}

		/// <summary>
		/// Deletes a file on the xbox.
		/// </summary>
		/// <param name="fileName">File to delete.</param>
		public void DeleteFile(string fileName)
		{
			SendCommand("delete name=\"{0}\"", fileName);
		}

        /// <summary>
        /// Sets the size of a specified file on the xbox.  This method will not zero out any extra bytes that may have been created.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        public void SetFileSize(string fileName, int size)
        {
            SendCommand("fileeof name=\"{0}\" size={1}", fileName, size);
        }

		/// <summary>
		/// Modifies file creation information.  If you wish to specify a new file size, use SetFileSize instead.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="info">File information.</param>
		public void SetFileInformation(string fileName, FileInformation info)
		{
			uint createhi = (uint)(info.CreationTime.ToFileTime() >> 32);
			uint createlo = (uint)(info.CreationTime.ToFileTime() & 0xFFFFFFFF);
			uint changehi = (uint)(info.ChangeTime.ToFileTime() >> 32);
			uint changelo = (uint)(info.ChangeTime.ToFileTime() & 0xFFFFFFFF);

			string attr = string.Empty;
			if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				attr += "readonly=1";
			else attr += "readonly=0";
			if ((info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
				attr += "hidden=1";
			else attr += "hidden=0";

			SendCommand("setfileattributes name=\"{0}\" createhi=0x{1} createlo=0x{2} changehi=0x{3} changelo=0x{4} {5}",
				fileName,
				Convert.ToString(createhi, 16), Convert.ToString(createlo, 16),
				Convert.ToString(changehi, 16), Convert.ToString(changelo, 16),
				attr);
		}

		/// <summary>
		/// Retrieves file information.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <returns>File information.</returns>
		public FileInformation GetFileInformation(string fileName)
		{
			FileInformation info = new FileInformation();
			info.Name = fileName;
			SendCommand("getfileattributes name=\"{0}\"", fileName);
			string msg = ReceiveMultilineResponse();

			// devs fucked up size output so we need to parse carefully... ;X
			int sizehiend = msg.IndexOf("sizelo") - 1;
			int sizelostart = sizehiend + 10;
			int sizeloend = msg.IndexOf("createhi") - 1;

			info.Size = Convert.ToUInt64(msg.Substring(sizelostart, sizeloend - sizelostart), 16);
			info.Size |= (Convert.ToUInt64(msg.Substring(9, sizehiend - 9), 16) << 32); // should be 0

			ulong createtime;
			createtime = Convert.ToUInt64(msg.Substring(msg.IndexOf("createlo") + 11, 8), 16);
			createtime |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("createhi") + 11, 8), 16) << 32);
			info.CreationTime = DateTime.FromFileTime((long)createtime);

			ulong changetime;
			changetime = Convert.ToUInt64(msg.Substring(msg.IndexOf("changelo") + 11, 8), 16);
			changetime |= (Convert.ToUInt64(msg.Substring(msg.IndexOf("changehi") + 11, 8), 16) << 32);
			info.ChangeTime = DateTime.FromFileTime((long)changetime);

			if (msg.Contains("directory"))	info.Attributes |= FileAttributes.Directory;
			else							info.Attributes |= FileAttributes.Normal;

			if (msg.Contains("readonly"))	info.Attributes |= FileAttributes.ReadOnly;
			if (msg.Contains("hidden"))		info.Attributes |= FileAttributes.Hidden;
			
			return info;
		}

		/// <summary>
		/// Retrieves file attributes.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <returns>File attributes.</returns>
		public FileAttributes GetFileAttributes(string fileName)
		{
			return GetFileInformation(fileName).Attributes;
		}

		/// <summary>
		/// Sets file attributes.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <returns>File attributes.</returns>
		public void SetFileAttributes(string fileName, FileAttributes attributes)
		{
			if ((attributes & FileAttributes.Normal) != FileAttributes.Normal && (attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly && (attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				throw new UnsupportedException("Unsupported file attribute.");

            // LOOK INTO HIDDEN ATTRIBUTES
			FileInformation fi = GetFileInformation(fileName);
			fi.Attributes = attributes;
			SetFileInformation(fileName, fi);
		}

		/// <summary>
		/// Retrieves file size.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <returns>File size.</returns>
		public uint GetFileSize(string fileName)
		{
			return (uint)GetFileInformation(fileName).Size;
		}

        /// <summary>
        /// Mounts the specified device to the specified drive letter.
        /// </summary>
        /// <param name="device">Device name</param>
        /// <param name="drive">Drive letter</param>
        public void MountDevice(Device device, Drive drive)
        {
            string driveName = "\\??\\" + drive.ToString() + ":";
            string deviceName = string.Empty;
            switch (device)
            {
                case Device.CDRom: deviceName = "\\Device\\CdRom0"; break;
                case Device.DriveC: deviceName = "\\Device\\Harddisk0\\Partition2"; break;
                case Device.DriveE: deviceName = "\\Device\\Harddisk0\\Partition1"; break;
                case Device.DriveF: deviceName = "\\Device\\Harddisk0\\Partition6"; break;
                //case Device.DriveG: deviceName = "\\Device\\Harddisk0\\Partition7"; break;    // seems to be disabled in debug bios
                //case Device.DriveH: deviceName = "\\Device\\Harddisk0\\Partition8"; break;    // seems to be disabled in debug bios
                case Device.DriveX: deviceName = "\\Device\\Harddisk0\\Partition3"; break;
                case Device.DriveY: deviceName = "\\Device\\Harddisk0\\Partition4"; break;
                case Device.DriveZ: deviceName = "\\Device\\Harddisk0\\Partition5"; break;
            }

            // send mounting info to xbox
            SetMemory(scratchBuffer, (ushort)driveName.Length, (ushort)(driveName.Length + 1),
                (uint)(scratchBuffer + 16), (ushort)deviceName.Length, (ushort)(deviceName.Length + 1),
                (uint)(scratchBuffer + 16 + driveName.Length + 1), driveName, deviceName);

            // attempt to mount device
            uint error = CallAddressEx(Kernel.IoCreateSymbolicLink, null, true, scratchBuffer, scratchBuffer + 8);
            if (error != 0) throw new ApiException("Failed to mount the device");
        }

        /// <summary>
        /// Unmounts the specified drive.
        /// </summary>
        /// <param name="drive">Drive letter.</param>
        public void UnMountDevice(Drive drive)
        {
            string driveName = "\\??\\" + drive.ToString() + ":";

            // send unmounting info to xbox
            SetMemory(scratchBuffer, (ushort)driveName.Length, (ushort)(driveName.Length + 1),
                (uint)(scratchBuffer + 8), driveName);

            // attempt to unmount device
            uint error = CallAddressEx(Kernel.IoDeleteSymbolicLink, null, true, scratchBuffer);
            if (error != 0) throw new ApiException("Failed to unmount the device");
        }
		#endregion

		#region Memory

        /// <summary>
        /// Creates a standard xbox memory stream.
        /// </summary>
        public class MemoryStream : Stream
        {
            #region Fields
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Xbox Client;

            protected uint position;
            public override long Position
            {
                get { return position; }
                set { position = (uint)value; }
            }
            public override bool CanRead { get { return Client.Connected; } }
            public override bool CanSeek { get { return Client.Connected; } }
            public override bool CanWrite { get { return Client.Connected; } }
            #endregion

            #region Constructor
            /// <summary>
            /// Creates a new memory stream using a client connection to a debug xbox.
            /// </summary>
            /// <param name="client">Connection to use.</param>
            public MemoryStream(Xbox client)
            {
                Client = client;
                if (client == null || !client.Connected)
                    throw new NoConnectionException();
                position = 0x10000; // start at a valid memory address
            }
            #endregion

            #region Methods
            public override void Flush() { throw new UnsupportedException(); }
            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin: return position = (uint)offset; // zero-based offset
                    case SeekOrigin.Current: return position += (uint)offset;
                    default: throw new Exception("Invalid SeekOrigin.");
                }
            }
            public long SeekTo(long offset)
            {
                return position = (uint)offset;
            }
            public override long Length { get { throw new UnsupportedException(); } }
            public override void SetLength(long value) { throw new UnsupportedException(); }
            public unsafe override int Read(byte[] buffer, int offset, int count)
            {
                int read = 0;
                Client.Getmem(position, count, ref buffer, offset, ref read);
                position += (uint)read;
                return read;
            }

            public unsafe override void Write(byte[] buffer, int offset, int count)
            {
                Client.Setmem((int)position, count, ref buffer, offset);
                position += (uint)count;
            }
            #endregion
        };

		/// <summary>
		/// Xbox memory stream.
		/// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
		private MemoryStream memoryStream;

		/// <summary>
		/// Xbox memory stream reader.
		/// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]  
		private BinaryReader memoryReader;

		/// <summary>
		/// Xbox memory stream writer.
		/// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]  
        private BinaryWriter memoryWriter;

        /// <summary>
        /// Dont use this, higher-level methods are available.  Use MemoryReader instead.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="read"></param>
        private void Getmem(uint address, int length, ref byte[] buffer, int offset, ref int read)
		{
			int maxBufferSize = 0x200000;
			int iterations = (int)length / maxBufferSize;
			int remainder = (int)length % maxBufferSize;
			read = 0;

			StatusResponse response;

			for (int i = 0; i < iterations; i++)
			{
				response = SendCommand("getmem2 addr=0x{0} length={1}", Convert.ToString(address + read, 16).PadLeft(8, '0'), maxBufferSize);
				Wait(maxBufferSize);
				connection.Client.Receive(buffer, (int)(offset + read), maxBufferSize, SocketFlags.None);
				read += maxBufferSize;
			}

            if (remainder > 0)
            {
                response = SendCommand("getmem2 addr=0x{0} length={1}", Convert.ToString(address + read, 16).PadLeft(8, '0'), remainder);
                Wait(remainder);
                connection.Client.Receive(buffer, (int)(offset + read), remainder, SocketFlags.None);
                read += remainder;
            }
		}

        /// <summary>
        /// Dont use this, higher-level methods are available.  Use MemoryWriter instead.
        /// Writes to xbox memory. Performance of ~10MB/s due to a simple xbdm.dll modification.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        private void Setmem(int address, int length, ref byte[] buffer, int offset)
        {
            int bufferSize = 0x100000;                // TODO: experiment with different buffer sizes
            int iterations = length / bufferSize;
            int remainder = length % bufferSize;
            int index = 0;

            StatusResponse Response;

            for (int i = 0; i < iterations; i++)
            {
                Response = SendCommand("writefile name=| offset=0x" + Convert.ToString(address, 16) + " length=" + bufferSize);
                if (Response.Type == ResponseType.ReadyForBinary)
                {
                    connection.Client.Send(buffer, offset, bufferSize, SocketFlags.None);
                    Response = ReceiveStatusResponse(); // garbage number of bytes set...it keeps track of total, dont really care to find how to reset it
                    // check for failure
                    index += bufferSize;
                }
                else throw new Exceptions.ApiException("SendCommand");
            }

            if (remainder > 0)
            {
                Response = SendCommand("writefile name=| offset=0x" + Convert.ToString(address, 16) + " length=" + remainder);
                if (Response.Type == ResponseType.ReadyForBinary)
                {
                    connection.Client.Send(buffer, offset, remainder, SocketFlags.None);
                    Response = ReceiveStatusResponse();
                    // check for failure - parse message and determine bytes written, then return 
                    index += bufferSize;
                }
                else throw new Exceptions.ApiException("SendCommand");
            }
        }

        /// <summary>
        /// Calculates the checksum of a block of memory on the xbox.
        /// </summary>
        /// <param name="address">Memory address on the Xbox console of the first byte of memory in the block. This address must be aligned on an 8-byte boundary, and it cannot point to code.</param>
        /// <param name="length">Number of bytes on which to perform the checksum. This value must be a multiple of 8.</param>
        /// <returns></returns>
        public uint GetMemoryChecksum(int address, int length)
        {
            if ((address % 8) != 0)     throw new ApiException("Address must be aligned on an 8-byte boundary.");
            if ((length % 8) != 0)      throw new ApiException("Length must be a multiple of 8.");
            SendCommand("getsum addr={0} length={1} blocksize={1}", address, length);
            int avail = connection.Available;
            return BitConverter.ToUInt32(ReceiveBinaryData(4), 0);
        }

        //TODO: add dump options with things to skip like gamecode/system/debug memory
        /// <summary>
        /// A complete dump of xbox memory.
        /// </summary>
        /// <returns></returns>
        public byte[] DumpMemory()
        {
            // combine any contiguous memory regions
            List<MemoryRegion> regions = CommittedMemory;
            for (int i = 0; i < regions.Count; i++)
                if (i > 0 && (uint)regions[i].BaseAddress == (uint)regions[i - 1].BaseAddress + (uint)regions[i - 1].Size)
                {
                    regions[i - 1].Size += regions[i].Size;
                    regions.RemoveAt(i);
                    i--;
                }

            // get total size of dump
            uint dumpSize = 0;
            foreach (MemoryRegion r in regions)
                dumpSize += (uint)r.Size;

            Pause();
            byte[] XboxMemory = new byte[dumpSize];

            int oldTimeout = timeout;
            int index = 0;
            int read = 0;
            timeout = 7000; // make sure we don't timeout waiting for large memory reads
            //uint skipped = 0;
            foreach (MemoryRegion r in regions)
            {
                // skip code and system memory
                //if ((int)r.BaseAddress == 0x10000 || (uint)r.BaseAddress > 0xb0000000)
                //{
                //    skipped += (uint)r.Size;
                //    continue;
                //}
                Getmem(r.BaseAddress.ToUInt32(), (int)r.Size, ref XboxMemory, index, ref read);
                index += read;
            }
            timeout = oldTimeout;
            Continue();

            return XboxMemory;
        }

		/// <summary>
		/// Retrieves an object from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <param name="dataType">Object type.</param>
		/// <returns>Received object.</returns>
		public object GetMemory(uint address, TypeCode dataType)
		{
			switch (dataType)
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:		return GetByte(address);
				case TypeCode.Char:		return GetUInt16(address);
				case TypeCode.Int16:	return GetInt16(address);
				case TypeCode.UInt16:	return GetUInt16(address);
				case TypeCode.Int32:	return GetInt32(address);
				case TypeCode.UInt32:	return GetUInt32(address);
				case TypeCode.Int64:	return GetInt64(address);
				case TypeCode.UInt64:	return GetUInt64(address);
				case TypeCode.Single:	return GetSingle(address);
				case TypeCode.Double:	return GetDouble(address);
                case TypeCode.String:   return GetString(address);
				default:				throw new UnsupportedException("Invalid datatype.");
			}
		}

		/// <summary>
		/// Retrieves data from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <param name="length">Length of data to receive.</param>
		/// <returns>Received data.</returns>
		public byte[] GetMemory(uint address, uint length)
		{
			byte[] Buffer = new byte[length];

            memoryStream.Position = address;
			memoryReader.Read(Buffer, 0, (int)length);

			return Buffer;
		}

		/// <summary>
		/// Retrieves byte from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received byte.</returns>
        public byte GetByte(uint address) { memoryStream.Position = address; return memoryReader.ReadByte(); }

		/// <summary>
		/// Retrieves Int16 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received Int16.</returns>
        public Int16 GetInt16(uint address) { memoryStream.Position = address; return memoryReader.ReadInt16(); }

		/// <summary>
		/// Retrieves UInt16 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received UInt16.</returns>
        public UInt16 GetUInt16(uint address) { memoryStream.Position = address; return memoryReader.ReadUInt16(); }

		/// <summary>
		/// Retrieves Int32 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received Int32.</returns>
        public Int32 GetInt32(uint address) { memoryStream.Position = address; return memoryReader.ReadInt32(); }

		/// <summary>
		/// Retrieves UInt32 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received UInt32.</returns>
        public UInt32 GetUInt32(uint address) { memoryStream.Position = address; return memoryReader.ReadUInt32(); }

		/// <summary>
		/// Retrieves Int64 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received Int64.</returns>
        public Int64 GetInt64(uint address) { memoryStream.Position = address; return memoryReader.ReadInt64(); }

		/// <summary>
		/// Retrieves UInt64 from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received UInt64.</returns>
        public UInt64 GetUInt64(uint address) { memoryStream.Position = address; return memoryReader.ReadUInt64(); }

		/// <summary>
		/// Retrieves Single from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received Single.</returns>
        public Single GetSingle(uint address) { memoryStream.Position = address; return memoryReader.ReadSingle(); }

		/// <summary>
		/// Retrieves Double from xbox memory.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received Double.</returns>
        public Double GetDouble(uint address) { memoryStream.Position = address; return memoryReader.ReadDouble(); }

        /// <summary>
        /// Retrieves a string from xbox memory.  Will automatically detect ascii or unicode strings of size greater than 1 char and retrieve as ascii.
        /// Maximum string size of 512 characters.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string GetString(uint address)
        {
            byte[] StringBuffer = new byte[1026];   // max string size of unicode x 512 + 2 byte terminator
            memoryStream.Position = address;
            memoryReader.Read(StringBuffer, 0, 1026);

            // ascii string
            if (StringBuffer[1] != 0)
            {
                string ascii = ASCIIEncoding.ASCII.GetString(StringBuffer);
                return ascii.Remove(ascii.IndexOf('\0'));
            }
            else // unicode
            {
                string unicode = UnicodeEncoding.Unicode.GetString(StringBuffer);
                return unicode.Remove(unicode.IndexOf("\0\0"));
            }
        }

        /// <summary>
        /// Retrieves a string of specified length from xbox memory.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetString(uint address, uint length)
        {
            return ASCIIEncoding.ASCII.GetString(GetMemory(address, length));
        }

		/// <summary>
		/// Retrieves a null-terminated ascii string from xbox memory.
		/// Maximum length of 512 characters.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received ascii string.</returns>
		public string GetASCIIString(uint address)
		{
			byte[] StringBuffer = new byte[512];
            memoryStream.Position = address;
			memoryReader.Read(StringBuffer, 0, 512);

			string Str = ASCIIEncoding.ASCII.GetString(StringBuffer);
			return Str.Remove(Str.IndexOf('\0'));
		}

		/// <summary>
		/// Retrieves a null-terminated unicode string from xbox memory.
		/// Maximum length of 512 characters.
		/// </summary>
		/// <param name="address">Memory location.</param>
		/// <returns>Received unicode string.</returns>
		public string GetUnicodeString(uint address)
		{
			byte[] StringBuffer = new byte[1024];
            memoryStream.Position = address;
			memoryReader.Read(StringBuffer, 0, 1024);
			string Str = UnicodeEncoding.Unicode.GetString(StringBuffer);
			return Str.Remove(Str.IndexOf("\0\0"));
		}

        /// <summary>
        /// Determines whether or not the specified address exists in xbox memory.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
		public bool IsValidAddress(uint address)
		{
			StatusResponse response = SendCommand("getmem addr=0x{0} length=1", Convert.ToString(address, 16));
			string mem = ReceiveSocketLine().Remove(2);
			ReceiveSocketLine();
			return (mem != "??");
		}


        /*
        /// <summary>
        /// Checks for a valid address range.
        /// </summary>
        public bool IsValidAddressRange(uint address, int size)
        {
            string script = "BB78563412B97856341281E300F0FFFF81C1FF0F000081E100F0FFFFC1E90C68AE000000E81F00000053FFD085C0741081C300100000E2E7B80000DB02C21000B800400080C21000558BEC5351BB000001808B4D08498B433C8B4418788B44181C03C38B048803C3595BC9C20400";
            int argIndex = script.IndexOf("78563412");
            int arg2Index = script.LastIndexOf("78563412");
            script = script.Replace("78563412", "");
            script = script.Insert(argIndex, Convert.ToString(address, 16).PadLeft(8, '0'));
            script = script.Insert(arg2Index, Convert.ToString(size, 16).PadLeft(8, '0'));

            //byte[] callScript = Util.StringToHexBytes("BB78563412B97856341281E300F0FFFF81C1FF0F000081E100F0FFFFC1E90C68AE000000E81F00000053FFD085C0741081C300100000E2E7B80000DB02C21000B800400080C21000558BEC5351BB000001808B4D08498B433C8B4418788B44181C03C38B048803C3595BC9C20400");
            byte[] callScript = Util.StringToHexBytes(script);

            SetMemory(ScriptBufferAddress, callScript);
           
            return SendCommand("crashdump").Success;
        }
        */

        /// <summary>
        /// Checks for a valid address range.
        /// </summary>
        public bool IsValidAddressRange(uint address, int size)
        {
            // combine any contiguous memory regions
            List<MemoryRegion> regions = CommittedMemory;
            for (int i = 0; i < regions.Count; i++)
                if (i > 0 && (uint)regions[i].BaseAddress == (uint)regions[i - 1].BaseAddress + (uint)regions[i - 1].Size)
                {
                    regions[i - 1].Size += regions[i].Size;
                    regions.RemoveAt(i);
                    i--;
                }

            // check if memory range is within a region
            foreach (MemoryRegion r in regions)
            {
                if ((address >= (uint)r.BaseAddress) && (address + size <= (uint)r.BaseAddress + r.Size))
                    return true;  //valid address
            }
            return false;   // no valid address range found*/
        }

		/// <summary>
		/// Writes object(s) to a specified xbox memory location.
		/// </summary>
		/// <param name="address">Xbox memory location.</param>
		/// <param name="data">Data to write. Specify multiple data 
        /// instances whenever you have multiple contiguous writes
		/// since this function will combine multiple data internally
        /// and then send all at once.</param>
		public void SetMemory(uint address, params object[] data)
		{
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			foreach (object obj in data)
			{
				switch (Convert.GetTypeCode(obj))
				{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Char: bw.Write(Convert.ToByte(obj)); break;
					case TypeCode.Int16:
					case TypeCode.UInt16: bw.Write(Convert.ToUInt16(obj)); break;
					case TypeCode.Int32:
					case TypeCode.UInt32: bw.Write(Convert.ToUInt32(obj)); break;
					case TypeCode.Int64:
					case TypeCode.UInt64: bw.Write(Convert.ToUInt64(obj)); break;
					case TypeCode.Single: bw.Write(Convert.ToSingle(obj)); break;
					case TypeCode.Double: bw.Write(Convert.ToDouble(obj)); break;
					case TypeCode.String: bw.Write(ASCIIEncoding.ASCII.GetBytes((string)obj + "\0")); break;    // assumes youre writing an ascii string
					case TypeCode.Object:
						byte[] bytes = obj as byte[]; // tries converting unknown object to byte array
						if (bytes != null) bw.Write(bytes);
						else throw new UnsupportedException("Invalid datatype.");
						break;
					default:	throw new UnsupportedException("Invalid datatype.");
				}
			}
            memoryStream.Position = address;
			memoryWriter.Write(ms.ToArray());
			bw.Close();
		}

		#endregion

		#region Threading

        /// <summary>
        /// Some debugging events require title execution to be suspended.
        /// But there are some debugging events where thread suspension is an option.
        /// For these events, you can elect to have the debugging subsystem suspend title execution by calling this function.
        /// </summary>
        /// <param name="flags"></param>
        public void StopOn(StopOnFlags flags)
        {
            SendCommand("stopon {0}", flags.ToString().Replace(",", ""));
        }

        /// <summary>
        /// Removes all breakpoints in an xbox title.
        /// </summary>
        public void ClearAllBreakpoints()
        {
            SendCommand("break clearall");
        }

        /// <summary>
        /// Sets a hardware breakpoint that suspends title execution when a particular section of memory is referenced.
        /// </summary>
        /// <param name="address">Address of the memory to watch.</param>
        /// <param name="size">The size, in bytes, of the memory to be watched.
        /// The only allowable values for this parameter are 1, 2, and 4.</param>
        /// <param name="type">Type of access to watch for.</param>
        public void SetBreakPoint(uint address, uint size, BreakpointType type)
        {
            SendCommand("break addr={0} size={1} {2}", address, type.ToString().Replace(",", ""));
        }

        /// <summary>
        /// Sets a breakpoint in an xbox title.
        /// </summary>
        /// <param name="address">Address where you would like to set a breakpoint.</param>
        public void SetBreakPoint(uint address)
        {
            SetBreakPoint(address, 1, BreakpointType.ReadWrite | BreakpointType.Execute);
        }

        /// <summary>
        /// Removes a breakpoint in an xbox title.
        /// </summary>
        /// <param name="address">Address where you would like to remove a breakpoint.</param>
        public void RemoveBreakPoint(uint address)
        {
            SendCommand("break clear addr={0}", address);
        }

        /// <summary>
        /// Sends a request to the xbox that the specified thread break as soon as possible.
        /// </summary>
        /// <param name="thread">ID of the thread to be halted. Send 0 as the thread ID to have the debugging subsystem select a thread to break into.</param>
        public void HaltThread(uint thread)
        {
            SendCommand("halt thread={0}", thread);
        }

        /// <summary>
        /// Resumes the execution of an xbox thread that has been stopped.
        /// </summary>
        /// <param name="thread">Thread ID.</param>
        public void ContinueThread(uint thread)
        {
            try { SendCommand("continue thread={0}", thread); } catch { }
        }

        /// <summary>
        /// Resumes the execution of all xbox threads that have been stopped.
        /// </summary>
        public void ContinueAllThreads()
        {
            foreach (ThreadInfo thread in Threads)
                try { SendCommand("continue thread={0}", thread.ID); } catch { }
        }

        /// <summary>
        /// Suspends a given xbox thread.
        /// </summary>
        /// <param name="thread">ID of the thread to suspend.</param>
        public void SuspendThread(uint thread)
        {
            SendCommand("suspend thread={0}", thread);
        }

        /// <summary>
        /// Resumes a given xbox thread.
        /// </summary>
        /// <param name="thread">ID of the thread to resume.</param>
        public void ResumeThread(uint thread)
        {
            SendCommand("resume thread={0}", thread);
        }

        /// <summary>
        /// Determines whether the specified thread is stopped.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool IsThreadStopped(uint thread)
        {
            try
            {
                StatusResponse res = SendCommand("isstopped thread={0}", thread); return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// If the specified thread is stopped this will return information about the circumstances that forced the thread to stop.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public string GetThreadStopInfo(uint thread)
        {
            try
            {
                return SendCommand("isstopped thread={0}", thread).Message;
            }
            catch { return string.Empty; }
        }

		/// <summary>
		/// Suspends all xbox title threads.
		/// </summary>
		public void Pause()
		{
            try { SendCommand("stop"); } catch { }
		}

		/// <summary>
		/// Resumes all xbox title threads.
		/// </summary>
        public void Continue()
		{
            try { SendCommand("go"); } catch { }
		}
		#endregion

		#region Misc.

        /// <summary>
        /// Synchronizes the xbox system time with the computer's current time.
        /// </summary>
        public void SynchronizeTime()
        {
            SystemTime = DateTime.Now;
        }

        //public void CacheScreenshotToHdd()
        //{




        //}

        /// <summary>
        /// Use this if the Screenshot function doesnt dump properly and you wish to add a new format.
        /// Framebuffer will be in a swizzled format.
        /// </summary>
        /// <returns></returns>
        public byte[] RawFramebufferDump(int width, int height, int pixelSizeInBytes)
        {
            ConnectionCheck();
            Pause();
            Thread.Sleep(25);   // give it enough time to break into thread and stop writing to framebuffer
            uint framebufferPtr = 0x80000000 | GetUInt32(0xFD600800);
            byte[] data = new byte[width * height * pixelSizeInBytes];  // assumes 640x480 resolution with rgba channels of 8 bits each
            data = GetMemory(framebufferPtr, (uint)data.Length);
            Continue();
            return data;
        }

        /// <summary>
        /// Takes a screenshot of the xbox display.
        /// </summary>
        public Image Screenshot()
        {
            ConnectionCheck();
            Pause();
            Thread.Sleep(25);   // give it enough time to break into thread and stop writing to framebuffer
            uint framebufferPtr = 0x80000000 | GetUInt32(0xFD600800);
            byte[] data = new byte[640 * 480 * 4];  // assumes 640x480 resolution with rgba channels of 8 bits each
            data = GetMemory(framebufferPtr, (uint)data.Length);
            Continue();
            return Bitmap.FromStream(RawToBMP(data));
        }

		// used for grabbing screenshots directly from the backbuffer
		private Byte[] Deswizzle(Byte[] swizzled)
		{
			int[] block = {
							  0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
							  18, 19, 16, 17, 22, 23, 20, 21, 26, 27, 24, 25, 30, 31, 28, 29,
							  33, 34, 35, 32, 37, 38, 39, 36, 41, 42, 43, 40, 45, 46, 47, 44,
							  51, 48, 49, 50, 55, 52, 53, 54, 59, 56, 57, 58, 63, 60, 61, 62
						  };
			Byte[] deswizzled = new Byte[1228800];
			int swiz = 0, index = 0, offset = 0;
			int deswiz = 1226240;
			int i, j, k, l;
			for (i = 0; i < 30; i++)
			{
				for (j = 0; j < 10; j++)
				{
					if ((i & 1) == 1)
						if ((j & 1) == 1) deswiz -= 256;
						else deswiz += 256;

					for (l = 0; l < 4; l++)
					{
						for (k = 0; k < 16; k++)
						{
							offset = (((int)(block[index] & 0xFFFFFFFE) >> 2) * 256) + ((block[index] & 3) * 16);
							index = (index + 1) & 63;
							for (int v = 0; v < 15; v++)
								if ((v & 3) != 3)
								{
									deswizzled[deswiz + v] = swizzled[swiz + offset + v];
									deswizzled[deswiz - 2560 + v] = swizzled[swiz + offset + 64 + v];
									deswizzled[deswiz - 5120 + v] = swizzled[swiz + offset + 128 + v];
									deswizzled[deswiz - 7680 + v] = swizzled[swiz + offset + 192 + v];
								}
							deswiz += 16;
						}
						deswiz -= 10496;
					}
					deswiz += 41216;
					swiz += 4096;

					if ((i & 1) == 1)
						if ((j & 1) == 1) deswiz += 256;
						else deswiz -= 256;
				}
				deswiz -= 43520;
			}
			return deswizzled;
		}
        private System.IO.MemoryStream RawToBMP(Byte[] raw)
		{
			if (raw.Length != 1228800)
			{
				return null;
			}
			raw = Deswizzle(raw);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			// BITMAPFILEHEADER
			bw.Write(new char[] { 'B', 'M' }); // ushort bfType - BM (19778)
			bw.Write((uint)824); // uint bfSize - Size of the file (bytes) - Header + ImageData + (ushort)0
			bw.Write((ushort)0); // ushort bfReserved1 - Zero
			bw.Write((ushort)0); // ushort bfReserved2 - Zero
			bw.Write((uint)54); // uint bfOffBits - Offset to image data
			// BITMAPINFOHEADER
			bw.Write((uint)40); // uint biSize - Size of BITMAPINFOHEADER (bytes)
			bw.Write((uint)640); // uint biWidth - Width of image (pixels)
			bw.Write((uint)480); // uint biHeight - Height of image (pixels)
			bw.Write((ushort)1); // ushort biPlanes - Number of planes of the target device (usually one)
			bw.Write((ushort)24); // ushort biBitCount - Bits per pixel (1=black/white, 4=16 colors, 8=256 colors, 24=16.7 million colors)
			bw.Write((uint)0); // uint biCompression - Type of compression (0=None)
			bw.Write((uint)770); // uint biSizeImage - Size of image data (bytes) - Zero if no compression
			bw.Write((uint)2834); // uint biXPelsPerMeter - Hoizontal pixels per meter (usually zero) (2834=72 Pixels Per Inch)
			bw.Write((uint)2834); // uint biYPelsPerMeter - Vertical pixels per meter (usually zero) (2834=72 Pixels Per Inch)
			bw.Write((uint)0); // uint biClrUsed - Number of colors used - If zero, calculated by biBitCount
			bw.Write((uint)0); // uint biClrImportant - Number of "important" colors (0=All)
			// Deswizzle code flips the image, so we don't need to do it here
			// for(int offset=1226240; offset>=0; offset-=2560)
			for (int offset = 0; offset < 1228800; offset += 2560)
			{
				for (int i = 0; i < 2560; i += 4)
				{
					bw.Write(raw[offset + i]);
					bw.Write(raw[offset + i + 1]);
					bw.Write(raw[offset + i + 2]);
				}
			}
			bw.Write((ushort)0);
			bw.Flush();
			return ms;
		}

        /// <summary>
        /// Benchmark utility function best when ran with xdk dash.  Lower memory situations will yeild lower speeds.
        /// </summary>
        /// <returns></returns>
		public string StreamTest()
		{
			float toMegs = 1.0f / (1024.0f * 1024.0f);

			int fileBufferSize = 0xA0000;
			uint memBufferSize = 0x20000;   //128kb

			if (MemoryStatistics.AvailablePages * 0x1000 < memBufferSize)
			{
				SetFileCacheSize(1);
				if (MemoryStatistics.AvailablePages * 0x1000 < memBufferSize)
					return "Need at least 128kb of available memory";
			}

            // memory tests
			uint TestBuffer = AllocateMemory(memBufferSize); //64kb
			byte[] membuf = new byte[memBufferSize];

			DateTime memReadStart = DateTime.Now;
			for (int i = 0; i < 400; i++)
				membuf = GetMemory(TestBuffer, memBufferSize);
			TimeSpan memReadElapse = DateTime.Now - memReadStart;
			string memReadSpeed = (((float)400 * (float)memBufferSize * toMegs) / (float)memReadElapse.TotalSeconds).ToString();

			DateTime memWriteStart = DateTime.Now;
            for (int i = 0; i < 400; i++)
            {
                Setmem((int)TestBuffer, (int)membuf.Length, ref membuf, 0);
            }
			TimeSpan memWriteElapse = DateTime.Now - memWriteStart;
			string memWriteSpeed = (((float)400 * (float)memBufferSize * toMegs) / (float)memWriteElapse.TotalSeconds).ToString();

			FreeMemory(TestBuffer);

			// filestream tests
			FileStream xbfs = new FileStream(this, "E:\\test.bin");
			BinaryReader br = new BinaryReader(xbfs);
			BinaryWriter bw = new BinaryWriter(xbfs);
			byte[] filebuf = new byte[fileBufferSize];

			DateTime fileWriteStart = DateTime.Now;
			for (int i = 0; i < 16; i++)
			{
				xbfs.Position = 0;
				bw.Write(filebuf, 0, fileBufferSize);
			}
			TimeSpan fileWriteElapse = DateTime.Now - fileWriteStart;
			string fileWriteSpeed = (((float)16 * (float)fileBufferSize * toMegs) / (float)fileWriteElapse.TotalSeconds).ToString();

			DateTime fileReadStart = DateTime.Now;
			for (int i = 0; i < 16; i++)
			{
				xbfs.Position = 0;
				filebuf = br.ReadBytes(fileBufferSize);
			}
			TimeSpan fileReadElapse = DateTime.Now - fileReadStart;
			string fileReadSpeed = (((float)16 * (float)fileBufferSize * toMegs) / (float)fileReadElapse.TotalSeconds).ToString();

			xbfs.Close();
			DeleteFile("E:\\test.bin");

            // determine link speed
            //byte[] bigbuffer = new byte[fileBufferSize];
            //DateTime linkSpeedStart = DateTime.Now;
            //for (int i = 0; i < 100; i++)
            //{
            //    this.Connection.Client.Send(bigbuffer);
            //}
            //TimeSpan linkSpeedElapse = DateTime.Now - linkSpeedStart;
            //string linkSpeed = (((float)100 * (float)fileBufferSize * toMegs) / (float)linkSpeedElapse.TotalSeconds).ToString();

			StringBuilder results = new StringBuilder();
            //results.AppendFormat("Link Speed: {0}Mb/s\n", linkSpeed);
			results.AppendFormat("Memory Read: {0}Mb/s\n", memReadSpeed);
			results.AppendFormat("Memory Write: {0}Mb/s\n", memWriteSpeed);
			results.AppendFormat("File Read: {0}Mb/s\n", fileReadSpeed);
			results.AppendFormat("File Write: {0}Mb/s\n", fileWriteSpeed);

			return results.ToString();
		}


		#endregion

        #region Console

        private void SendATACommand()
        {


        }

        public TrayState GetTrayState()
        {
            //// likely flags
            //0x1 = busy
            //0x10 = open
            //0x20 = dvd
            //0x40 = closed

            //// opening a closed empty tray
            //ClosedAndEmpty =  0x40	1000000
            //                  0x41	1000001
            //EjectingEmpty =   0x31	0110001
            //                  0x11	0010001
            //Open =            0x10	0010000

            //// closing an empty tray
            //Closing =         0x51    1010001
            //Loading =         0x01	0000001
            //                  0x41	1000001
            //ClosedAndEmpty =  0x40	1000000

            //// opening a closed full tray
            //ClosedAndFull =   0x60	1100000
            //EjectingFull =    0x21	0100001
            //EjectingEmpty =   0x31	0110001
            //                  0x11	0010001
            //Open =            0x10	0010000

            //// closing a full tray
            //Open =            0x10	0010000
            //Closing =         0x51	1010001     Busy | Open | Closed - weird, but interpret as it being open but in the process of closing
            //Loading =         0x01    0000001
            //                  0x61	1100001
            //ClosedAndFull =   0x60	1100000

            CallAddressEx(Kernel.HalReadSMBusValue, null, true, SMBusDevices.PIC, PICCommand.DvdTrapState, 0, scratchBuffer);
            return (TrayState)GetUInt16(scratchBuffer);
        }

        /// <summary>
        /// Shuts down the xbox console and then turns it on again.
        /// </summary>
        public void CyclePower()
        {
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Power, 0, PowerSubCommand.Cycle);
            Thread.Sleep(25);   // let it shut down first
            Disconnect();
            throw new ApiException("Xbox has been shut down.  Make sure you are running in debug mode again before reconnecting.");
        }

        /// <summary>
        /// Resets the xbox console.
        /// </summary>
        public void Reset()
        {
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Power, 0, PowerSubCommand.Reset);
            Reconnect(15000);
        }

        /// <summary>
		/// Shuts down the xbox console.
		/// </summary>
		public void Shutdown()
		{
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Power, 0, PowerSubCommand.PowerOff);
            Thread.Sleep(25);  // let it shut down first
            Disconnect();
            throw new ApiException("Xbox has been shut down.  You cannot continue until you restart your xbox.");
		}

        /// <summary>
        /// Disables reset on DVD tray eject.
        /// </summary>
        public void DisableDVDEjectReset()
        {
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.ResetOnEject, 0, ResetOnEjectSubCommand.Disable);
        }

        /// <summary>
        /// Enables reset on DVD tray eject.
        /// </summary>
        public void EnableDVDEjectReset()
        {
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.ResetOnEject, 0, ResetOnEjectSubCommand.Enable);
        }

		/// <summary>
		/// Ejects xbox tray.
		/// </summary>
		public void EjectTray()
		{
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Eject, 0, EjectSubCommand.Eject); // eject tray
			Thread.Sleep(250);
		}

		/// <summary>
		/// Loads xbox tray.
		/// </summary>
		public void LoadTray()
		{
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.Eject, 0, EjectSubCommand.Load); // load tray
			Thread.Sleep(250);
		}

		/// <summary>
		/// Sets the xbox LED state.
		/// </summary>
		/// <param name="state1">First LED state.</param>
		/// <param name="state2">Second LED state.</param>
		/// <param name="state3">Third LED state.</param>
		/// <param name="state4">Fourth LED state.</param>
		public void SetLEDState(LEDState state1, LEDState state2, LEDState state3, LEDState state4)
		{
			byte State = 0;
			State |= (byte)state1;
			State |= (byte)((byte)state2 >> 1);
			State |= (byte)((byte)state3 >> 2);
			State |= (byte)((byte)state4 >> 3);
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.LedRegister, 0, State);
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.LedMode, 0, LEDSubCommand.Custom);
			Thread.Sleep(10);
		}

        /// <summary>
        /// Restores the xbox LED to its default state.
        /// </summary>
        public void RestoreDefaultLEDState()
        {
            CallAddressEx(Kernel.HalWriteSMBusValue, null, false, SMBusDevices.PIC, PICCommand.LedMode, 0, LEDSubCommand.Default);
            Thread.Sleep(10);
        }

        /// <summary>
        /// Gets the xbox video flags.
        /// </summary>
        /// <returns></returns>
		public VideoFlags GetVideoFlags()
		{
            CallAddressEx(Kernel.ExQueryNonVolatileSetting, null, false, 0x8, 0x10008, 0x10004, 4, 0);
			return (VideoFlags)((GetUInt32(0x10004) >> 16) & 0x5F);
		}

        /// <summary>
        /// Gets the xbox video standard.
        /// </summary>
        /// <returns></returns>
		public VideoStandard GetVideoStandard()
		{
            CallAddressEx(Kernel.ExQueryNonVolatileSetting, null, false, 0x103, 0x10008, 0x10004, 4, 0);
			return (VideoStandard)GetByte(0x10005);
		}

        /// <summary>
        /// Reads the xbox EEPROM.
        /// </summary>
        /// <returns></returns>
		public byte[] ReadEEPROM()
		{
			// build call script
            //xor	ecx, ecx
            //mov	eax, 012345678h			;temp buffer
            //readLoop:

            //pushad
            //mov	word ptr ds:[eax + ecx], 0	;ZeroMemory
            //lea	ebx, [eax + ecx]		;EEPROMDATA+i
            //push	ebx
            //push	0
            //push	ecx				;i
            //push	0A8h
            //mov	edx, 012345678h			;HalReadSMBusValue
            //call	edx
            //popad

            //push	ecx
            //mov	ecx, 010000h
            //spinwaitloop:
            //dw	090F3h				;pause	
            //loop	spinwaitloop
            //pop	ecx

            //inc	ecx
            //cmp	ecx, 0FFh
            //jl	readLoop
            //mov	eax, 02DB0000h		;fake success
            //retn	010h

            byte[] callScript = new byte[62];
            BinaryWriter call = new BinaryWriter(new System.IO.MemoryStream(callScript));
            call.BaseStream.Position = 0;
            call.Write((ushort)0xC933);
            call.Write((byte)0xB8);
            call.Write(scratchBuffer);
            byte[] one = { 0x60, 0x66, 0xC7, 0x04, 0x01, 0x00, 0x00, 0x8D, 0x1C, 0x01, 0x53, 0x6A, 0x00, 0x51, 0x68, 0xA8, 0x00, 0x00, 0x00, 0xBA };
            call.Write(one);
            call.Write(Kernel.HalReadSMBusValue);
            byte[] two = { 0xFF, 0xD2, 0x61, 0x51, 0xB9 };
            call.Write(two);
            call.Write((int)0x100);   // modify spin count if we find out that its not reading all of the eeprom
            byte[] three = { 0xF3, 0x90, 0xE2, 0xFC, 0x59, 0x41, 0x81, 0xF9, 0xFF, 0x00, 0x00, 0x00, 0x7C, 0xD1, 0xB8, 0x00, 0x00, 0xDB, 0x02, 0xC2, 0x10, 0x00 };
            call.Write(three);  // change 0xF3 to 0x90 to get rid of the pause instruction
            call.Close();

			// inject script
			SetMemory(ScriptBufferAddress, callScript);

            // execute script via hijacked crashdump function
            SendCommand("crashdump");

			return GetMemory(scratchBuffer, 256);
		}
		#endregion

		#region Remote Execution

        // current script buffer information
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint ScriptBufferAddress = 0xB0037800;  // location in xbdm.dll memory
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint ScriptBufferSize = 2048;

        /// <summary>
        /// Changes the script buffer size.
        /// </summary>
        /// <param name="size"></param>
        public void ChangeScriptBufferSize(uint size)
        {
            if (ScriptBufferAddress != 0xB0037800)
                FreeMemory(ScriptBufferAddress);

            ScriptBufferAddress = AllocateDebugMemory(size);
            ScriptBufferSize = size;
            SetMemory(0xB00292D0, ScriptBufferAddress);   // reroutes the ScriptBufferAddress ptr...(assumes were already running v7887 xbdm)
        }

		/// <summary>
		/// Simple function call with optional return value.  Average execution time of 1.3 ms without return or 1.75ms with return.
		/// </summary>
		/// <param name="address">Xbox procedure address.</param>
		/// <returns></returns>
		/// <remarks>0x10000 is reserved for function use.</remarks>
		public uint CallAddress(uint address, bool returnValue)
		{
			// call address and store result
            //mov   eax, address
			//call	eax
			//mov	dword ptr ds:[010000h], eax
            //mov   eax, 02DB0000h  ;fake success
			//retn  10h
            System.IO.MemoryStream callBuffer = new System.IO.MemoryStream();
            BinaryWriter callScript = new BinaryWriter(callBuffer);
            callBuffer.Position = 0;
			callScript.Write((byte)0xB8);
			callScript.Write(address);
            byte[] script = { 0xFF, 0xD0, 0xA3, 0x00, 0x00, 0x01, 0x00, 0xB8, 0x00, 0x00, 0xDB, 0x02, 0xC2, 0x10, 0x00 };
			callScript.Write(script);

            // inject call script
            if (callBuffer.Length > ScriptBufferSize) throw new Exception("Script too big. Try allocating more memory and specifying new script buffer information.");
            SetMemory(ScriptBufferAddress, callBuffer.ToArray());
            callBuffer.Close();

            // execute script via hijacked crashdump function
            SendCommand("crashdump");

			// return the value of eax after the call
            if (returnValue) return GetUInt32(0x10000);
            else return 0;

		}


        //CallAddressEx usage
        //public void Function(Arg1, Arg2, Arg3);
        //CallAddressEx(FunctionAddress, null, false, Arg1, Arg2, Arg3);
        //assembly:
        //push  Arg3
        //push  Arg2
        //push  Arg1
        //call  Function
		/// <summary>
		/// Extended function call with optional context, arguments, and return value.  Average execution time of 1.3ms without return or 1.75ms with return.
        /// </summary>
		/// <param name="address">Xbox procedure address.</param>
		/// <param name="context">Cpu context.  This parameter may be null.</param>
		/// <param name="pushArgs">Arguments to be pushed before the call is made.  These are optional of course.</param>
		/// <returns></returns>
		public uint CallAddressEx(uint address, CPUContext context, bool returnValue, params object[] pushArgs)
		{
			#region Build Call Script

			// buffer to hold our call data
            System.IO.MemoryStream callScript = new System.IO.MemoryStream();
			BinaryWriter call = new BinaryWriter(callScript);
			call.BaseStream.Position = 0;

            // push arguments in reverse order
            for (int i = pushArgs.Length - 1; i >= 0; i--)
            {
                call.Write((byte)0x68); //push
                call.Write(Convert.ToUInt32(pushArgs[i]));
            }

			if (context != null)
			{
				// assign registers
				if (context.Eax != null)
				{
					call.Write((byte)0xB8); //mov eax
					call.Write(Convert.ToUInt32(context.Eax));
				}
				if (context.Ebx != null)
				{
					call.Write((byte)0xBB); //mov ebx
					call.Write(Convert.ToUInt32(context.Ebx));
				}
				if (context.Ecx != null)
				{
					call.Write((byte)0xB9); //mov ecx
					call.Write(Convert.ToUInt32(context.Ecx));
				}
				if (context.Edx != null)
				{
					call.Write((byte)0xBA); //mov edx
					call.Write(Convert.ToUInt32(context.Edx));
				}
				if (context.Esi != null)
				{
					call.Write((byte)0xBE); //mov esi
					call.Write(Convert.ToUInt32(context.Esi));
				}
				if (context.Edi != null)
				{
					call.Write((byte)0xBF); //mov edi
					call.Write(Convert.ToUInt32(context.Edi));
				}
				if (context.Esp != null)
				{
					call.Write((byte)0xBC); //mov esp
					call.Write(Convert.ToUInt32(context.Esp));
				}
				if (context.Ebp != null)
				{
					call.Write((byte)0xBD); //mov ebp
					call.Write(Convert.ToUInt32(context.Ebp));
				}

				// assign xmm registers
				// remember that its a pointer, not a value you are db'ing
				// so we need to dump the values somewhere, then store the pointers to those...

                uint XmmContextBuffer = 0x10004;
                if (context.Xmm0 != null)
				{
                    SetMemory(XmmContextBuffer, Convert.ToSingle(context.Xmm0));
					call.Write(0x05100FF3); //movss xmm0
					call.Write(XmmContextBuffer);   //dword ptr ds:[addr]
				}
                if (context.Xmm1 != null)
				{
					SetMemory(XmmContextBuffer + 4, Convert.ToSingle(context.Xmm1));
					call.Write(0x0D100FF3); //movss xmm1
					call.Write(XmmContextBuffer + 4);
				}
                if (context.Xmm2 != null)
				{
					SetMemory(XmmContextBuffer + 8, Convert.ToSingle(context.Xmm2));
					call.Write(0x15100FF3); //movss xmm2
					call.Write(XmmContextBuffer + 8);
				}
                if (context.Xmm3 != null)
				{
					SetMemory(XmmContextBuffer + 12, Convert.ToSingle(context.Xmm3));
					call.Write(0x1D100FF3); //movss xmm3
					call.Write(XmmContextBuffer + 12);
				}
                if (context.Xmm4 != null)
				{
					SetMemory(XmmContextBuffer + 16, Convert.ToSingle(context.Xmm4));
					call.Write(0x25100FF3); //movss xmm4
					call.Write(XmmContextBuffer + 16);
				}
                if (context.Xmm5 != null)
				{
					SetMemory(XmmContextBuffer + 20, Convert.ToSingle(context.Xmm5));
					call.Write(0x2D100FF3); //movss xmm5
					call.Write(XmmContextBuffer + 20);
				}
                if (context.Xmm6 != null)
				{
					SetMemory(XmmContextBuffer + 24, Convert.ToSingle(context.Xmm6));
					call.Write(0x35100FF3); //movss xmm6
					call.Write(XmmContextBuffer + 24);
				}
				if (context.Xmm7 != null)
				{
					SetMemory(XmmContextBuffer + 28, Convert.ToSingle(context.Xmm7));
					call.Write(0x3D100FF3); //movss xmm7
					call.Write(XmmContextBuffer + 28);
				}
			}


			// call address and store result
			//call	dword ptr ds:[CallAddress]
			//mov	dword ptr ds:[ReturnAddress], eax
            //mov   eax, 02DB0000h  ;fake success
			//retn  10h
			call.Write((ushort)0x15FF);
			call.Write((uint)(ScriptBufferAddress + call.BaseStream.Position + 17));
			call.Write((byte)0xA3);
			call.Write((uint)0x10000);
            call.Write(0x00DB02B8);
            call.Write(0x0010C200);
            call.Write(address);
			#endregion

            // inject call script
            if (callScript.Length > ScriptBufferSize) throw new Exception("Script too big. Try allocating more memory and specifying new script buffer information.");
            SetMemory(ScriptBufferAddress, callScript.ToArray());
            call.Close();

            // execute script via hijacked crashdump function
            FlushSocketBuffer();
            SendCommand("crashdump");

			// return the value of eax after the call
            if (returnValue) return GetUInt32(0x10000);
            else return 0;
		}
		#endregion

		#region Memory Management
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int MaxAllocTableSize = 400;  // limit on how big our allocation table can grow...

		/// <summary>
		/// Keeps track of all the memory YeloDebug uses.
		/// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<AllocationEntry> AllocationTable = new List<AllocationEntry>();

		/// <summary>
		/// Prevents YeloDebug from syncing its allocation table with the xbox.
		/// This is usefull if you are confident that you wont lose your connection,
		/// then you can unblock when you wish to manually sync again.  Otherwise, YeloDebug 
		/// will sync after each memory allocation or release.
		/// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool BlockAllocationTableSync = false;      // only update on pc

		/// <summary>
		/// Gets or sets whether or not allocation table will be kept in sync with the xbox.
		/// </summary>
        public bool AllocationTableSyncing { get { return allocationTableSyncing; } set { allocationTableSyncing = value; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool allocationTableSyncing = true;

		/// <summary>
		/// Reads the allocation table from the xbox.
		/// </summary>
		/// <returns></returns>
		private List<AllocationEntry> LoadAllocationTable()
		{
			List<AllocationEntry> allocationTable = new List<AllocationEntry>();

			// gets the number of entries in our table
			uint tableCount = GetUInt32(History.AllocationTable.CountAddress);
			if (tableCount == 0)
				return new List<AllocationEntry>();
			else if (tableCount > MaxAllocTableSize)
				throw new Exception("Allocation table corruption."); // maximum allocation count has either been exceeded, or theres a corruption

			// read the allocation table from xbox memory
			byte[] allocBuffer = GetMemory(History.AllocationTable.BufferAddress, tableCount * 9);
            BinaryReader alloc = new BinaryReader(new System.IO.MemoryStream(allocBuffer));
			alloc.BaseStream.Position = 0;

			// build our alloc table
			for (int i = 0; i < tableCount; i++)
			{
				// check for unaligned addresses as signs of a corrupted table
				uint address = alloc.ReadUInt32();
				if ((address & 0xFFF) > 0)
					throw new Exception("Allocation table corruption.");

				// check for sizes greater than 128mb as signs of corrupted data
				uint size = alloc.ReadUInt32();
				if (size > 0x8000000)
					throw new Exception("Allocation table corruption.");

				// check for invalid type as signs of corrupted data
				AllocationType type = (AllocationType)alloc.ReadByte();
				if ((byte)type > 3)
					throw new Exception("Allocation table corruption.");

				// otherwise add to table
				if (allocationTable.Count < MaxAllocTableSize)
					allocationTable.Add(new AllocationEntry(address, size, type));
			}
			// possibly check if those addresses are still allocated (kernel.isAddressValid), if failed, 
            // remove from table and resave to xbox at the expense of higher network traffic :X

			return allocationTable;
		}

		/// <summary>
		/// Syncs the allocation table with the xbox.
		/// </summary>
		private void SyncAllocationTable()
		{
			// dont update if blocked
			if (BlockAllocationTableSync)
				return;

			// buffer to hold our allocation table
			byte[] allocBuffer = new byte[AllocationTable.Count * 9 + 4];
            BinaryWriter alloc = new BinaryWriter(new System.IO.MemoryStream(allocBuffer));
			alloc.BaseStream.Position = 0;
			alloc.Write(AllocationTable.Count); // prefixed with total count

			// build our alloc table
			foreach (AllocationEntry entry in AllocationTable)
			{
				alloc.Write(entry.Address);
				alloc.Write(entry.Size);
				alloc.Write((byte)entry.Type);
			}

			// store our table in xbox memory
			SetMemory(History.AllocationTable.CountAddress, allocBuffer);
		}

		/// <summary>
		/// Removes an entry from the allocation table.
		/// </summary>
		/// <param name="address"></param>
		/// <returns>Allocation size.</returns>
		private uint RemoveAllocationEntry(uint address)
		{
			uint size = 0;
			for (int i = 0; i < AllocationTable.Count; i++)
			{
				if (AllocationTable[i].Address == address)  // destroys any duplicates...although there shouldnt be any! ;P
				{
					size = AllocationTable[i].Size;
					AllocationTable.RemoveAt(i);
					i--;
				}
			}

			// update our information page
			SyncAllocationTable();

			return size;
		}

		/// <summary>
		/// Makes sure the given allocation exists.
		/// </summary>
		/// <param name="address"></param>
		/// <returns>Allocation table index.</returns>
		private int AssertAllocationExists(uint address)
		{
			for (int i = 0; i < AllocationTable.Count; i++)
				if (address == AllocationTable[i].Address)
					return i;
			throw new Exception("Allocation does not exist.");
		}

		/// <summary>
		/// Makes sure the given allocation does not exist.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private void AssertAllocationNonexistant(uint address)
		{
			foreach (AllocationEntry entry in AllocationTable)
				if (address == entry.Address)
					throw new Exception("Allocation already exists.");
			return;
		}

		/// <summary>
		/// Determines a given allocations type.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private AllocationType GetAllocationType(uint address)
		{
			int index = AssertAllocationExists(address);
			return AllocationTable[index].Type;
		}

		/// <summary>
		/// Allocates memory on the xbox.
		/// </summary>
		/// <param name="size"></param>
		/// <returns>Allocated address.</returns>
		public uint AllocateMemory(uint size)	{ return AllocateVirtualMemory(size); }

		/// <summary>
		/// Allocates memory of specified type on the xbox.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="size"></param>
		/// <returns>Allocated address.</returns>
		public uint AllocateMemory(AllocationType type, uint size)
		{
			switch (type)
			{
				case AllocationType.Debug:		return AllocateDebugMemory(size);
				case AllocationType.Physical:	return AllocatePhysicalMemory(size);
				case AllocationType.System:		return AllocateSystemMemory(size);
				case AllocationType.Virtual:	return AllocateVirtualMemory(size);
				default:						throw new Exception("Invalid allocation type.");
			}
		}

		/// <summary>
		/// Frees xbox memory.
		/// </summary>
		/// <param name="address"></param>
		/// <returns>Size of freed memory.</returns>
		public uint FreeMemory(uint address)
		{
			// frees memory based on allocation type
			switch (GetAllocationType(address))
			{
				case AllocationType.Debug:		return FreeDebugMemory(address);
				case AllocationType.Physical:	return FreePhysicalMemory(address);
				case AllocationType.System:		return FreeSystemMemory(address);
				case AllocationType.Virtual:	return FreeVirtualMemory(address);
				default:						return 0;   // shouldnt reach here
			}
		}

		/// <summary>
		/// Frees all memory associated with the current allocation table.
		/// </summary>
		/// <returns>Total memory freed.</returns>
		public uint FreeAllMemory()
		{
			// we block so it wont have to save after each free
			// hopefully the connection wont drop inbetween ;P
			BlockAllocationTableSync = true;
			uint totalFreed = 0;

			try
			{
				for (int i = 0; i < AllocationTable.Count; i++)
				{
					totalFreed += FreeMemory(AllocationTable[i].Address);
					i--;
				}
			}
            finally // dont catch the exception, let it propogate
			{
				BlockAllocationTableSync = false; // but be sure to unblock no matter what ;)

				// update our information page
				SyncAllocationTable();
			}

			// return size of memory freed
			return totalFreed;
		}

		/// <summary>
		/// Determines if the requested size of memory can be allocated.  This function will also borrow from the file cache to meet the requested size if needed.
		/// </summary>
		/// <param name="requestedSize"></param>
		/// <returns></returns>
		private bool IsEnoughMemory(uint requestedSize)
		{
			// determine number of pages needed
			uint pagesNeeded = requestedSize / 0x1000;

			uint pagesAvailable = MemoryStatistics.AvailablePages;

			// tests for low memory (todo: try not to take all available pages...leave at least 1)
			if (pagesAvailable < pagesNeeded)
			{
				uint availableCache = GetFileCacheSize() - 1;

				// try to borrow from filecache ;)
				if ((pagesAvailable + availableCache) > pagesNeeded)
					SetFileCacheSize((availableCache + 1) - (pagesNeeded - pagesAvailable));
				else throw new OutOfMemoryException("Failed to allocate additional xbox memory.");  // cant steal any more from filecache ;(
			}
			return true;
		}

		/// <summary>
		/// Makes sure size is aligned to specs.  Unaligned sizes will be rounded up to the next 4kb page boundary.
		/// </summary>
		/// <param name="size"></param>
		/// <returns>Aligned size.</returns>
		private uint AlignmentCheck(uint size)
		{
			// calculate actual size of allocation since we are working with 4kb pages
			size += 0xFFF;
			size &= 0xFFFFF000; //rounds up to next 4kb if needed
			return size;
		}

		/// <summary>
		/// Allocates debug memory on the xbox. Address space of 0xB0000000 to 0xC0000000.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocateDebugMemory(uint size)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// allocate our debug memory
            uint ptr = (uint)CallAddressEx(Kernel.MmDbgAllocateMemory, null, true, size, 4);

			// add to our allocation table if allocation succeeded
			if (ptr == 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.Debug));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Frees debug memory on the xbox.
		/// </summary>
		/// <param name="address">Memory address.</param>
		/// <returns>Size of freed memory.</returns>
		public uint FreeDebugMemory(uint address)
		{
			// make sure allocation exists
			AssertAllocationExists(address);

			// make sure we are only trying to free debug memory
			if (GetAllocationType(address) != AllocationType.Debug)
				throw new Exception("Attempting to free memory of a different type.");

			// returns pages freed
			uint result = (uint)CallAddressEx(Kernel.MmDbgFreeMemory, null, true, address, 0);
			if (result == 0)
				throw new Exception("Failure to free xbox memory.");

			// return size of memory freed
			return RemoveAllocationEntry(address);
		}

		/// <summary>
		/// Allocates physical memory on the xbox.  Address space of 0x80000000 to 0x84000000.  Extends to 0x88000000 on a developer kit.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocatePhysicalMemory(uint size)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// allocate the physical memory
			uint ptr = (uint)CallAddressEx(Kernel.MmAllocateContiguousMemory, null, true, size);

			// add to our allocation table if allocation succeeded
			if (ptr == 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.Physical));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Allocates physical memory on the xbox.  Input range of 0x0 - 0x4000000.  Xbox address space of 0x80000000 to 0x84000000.  Extends to 0x88000000 on a developer kit.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <param name="range">Range of memory to place allocation.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocatePhysicalMemoryEx(uint size, AddressRange range)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// allocate the physical memory
			uint ptr = (uint)CallAddressEx(Kernel.MmAllocateContiguousMemoryEx, null, true, size, range.Low, range.High, 0, 4);

			// add to our allocation table if allocation succeeded
			if (ptr == 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.Physical));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Frees physical memory on the xbox.
		/// </summary>
		/// <param name="address">Memory address.</param>
		/// <returns>Size of freed memory.</returns>
		public uint FreePhysicalMemory(uint address)
		{
			// make sure allocation exists
			AssertAllocationExists(address);

			// make sure we are only trying to free physical memory
			if (GetAllocationType(address) != AllocationType.Physical)
				throw new Exception("Attempting to free memory of a different type.");

			// free the physical memory
			uint result = (uint)CallAddressEx(Kernel.MmFreeContiguousMemory, null, true, address);
			if (result != 0)
				throw new Exception("Failure to free xbox memory.");

			// return size of memory freed
			return RemoveAllocationEntry(address);
		}

		/// <summary>
		/// Allocates virtual memory on the xbox.  Address space of 0x00000000 to 0x80000000.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocateVirtualMemory(uint size)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// prepares arguments
			uint pSize = History.RemoteExecution.ArgumentBuffer;
			uint pAddress = History.RemoteExecution.ArgumentBuffer + 4;
			SetMemory(pSize, size); //pSize
			SetMemory(pAddress, 0);  //pAddress

			// allocates virtual memory
			uint result = (uint)CallAddressEx(Kernel.NtAllocateVirtualMemory, null, true, pAddress, 0, pSize, 0x1000, 4);
			uint ptr = GetUInt32(pAddress);

			// add to our allocation table if allocation succeeded
			if (result != 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.Virtual));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Allocates virtual memory on the xbox.  Address space of 0x00000000 to 0x80000000.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <param name="address">Preferred address.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocateVirtualMemoryEx(uint size, uint address)
		{
			// if they want to allocate at address 0x0, and not want the function to determine where for them ;P
			address |= 1;

			// make sure allocation doesnt already exist
			AssertAllocationNonexistant(address & 0xFFFFF000);  // check for the address it will try to allocate...

			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// prepares arguments
			uint pSize = History.RemoteExecution.ArgumentBuffer;
			uint pAddress = History.RemoteExecution.ArgumentBuffer + 4;
			SetMemory(pSize, size); //pSize
			SetMemory(pAddress, address);  //pAddress

			// reserves virtual memory - for some reason you need to do this first when specifying your own address...
			// took forever to figure this one out ;x
			uint res = (uint)CallAddressEx(Kernel.NtAllocateVirtualMemory, null, true, pAddress, 0, pSize, MEMORY_FLAGS.MEM_RESERVE, 4);
			if (res != 0) throw new Exception("Xbox memory allocation failed.");

			// prepares arguments
			SetMemory(pSize, size); //pSize
			SetMemory(pAddress, address);  //pAddress

			// allocates virtual memory
            uint result = (uint)CallAddressEx(Kernel.NtAllocateVirtualMemory, null, true, pAddress, 0, pSize, MEMORY_FLAGS.MEM_COMMIT, 4);
			uint ptr = GetUInt32(pAddress);

			// add to our allocation table if allocation succeeded
			if (result != 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.Virtual));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Frees virtual memory on the xbox.
		/// </summary>
		/// <param name="address">Memory address.</param>
		/// <returns>Size of freed memory.</returns>
		public uint FreeVirtualMemory(uint address)
		{
			// make sure allocation exists
			AssertAllocationExists(address);

			// make sure we are only trying to free virtual memory
			if (GetAllocationType(address) != AllocationType.Virtual)
				throw new Exception("Attempting to free memory of a different type.");

			// prepares arguments
			uint pSize = History.RemoteExecution.ArgumentBuffer;
			uint pAddress = History.RemoteExecution.ArgumentBuffer + 4;
			SetMemory(pSize, 0); //pSize
			SetMemory(pAddress, address);  //pAddress

			// free the virtual memory
			uint result = (uint)CallAddressEx(Kernel.NtFreeVirtualMemory, null, true, pAddress, pSize, 0x8000);
			if (result != 0)
				throw new Exception("Failure to free xbox memory.");

			// return size of memory freed
			return RemoveAllocationEntry(address);
		}

		/// <summary>
		/// Allocates system memory on the xbox.  Address space of 0xD0000000 to 0xFFFFFFFC.
		/// </summary>
		/// <param name="size">Size of memory to be allocated.  Note that an unaligned size will be rounded up to the next 4kb page boundary.</param>
		/// <returns>Address of allocated memory.</returns>
		public uint AllocateSystemMemory(uint size)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			// allocates system memory
			uint ptr = (uint)CallAddressEx(Kernel.MmAllocateSystemMemory, null, true, size, 4);

			// add to our allocation table if allocation succeeded
			if (ptr == 0)
				throw new Exception("Xbox memory allocation failed.");
			if (AllocationTable.Count > MaxAllocTableSize)
				throw new Exception("Maximum allocation count has been reached.");
			if (allocationTableSyncing)
				AllocationTable.Add(new AllocationEntry(ptr, size, AllocationType.System));

			// update our information page
			SyncAllocationTable();

			return ptr;
		}

		/// <summary>
		/// Frees system memory on the xbox.
		/// </summary>
		/// <param name="address">Memory address.</param>
		/// <returns>Size of freed memory.</returns>
		public uint FreeSystemMemory(uint address)
		{
			// make sure allocation exists
			AssertAllocationExists(address);

			// make sure we are only trying to free system memory
			if (GetAllocationType(address) != AllocationType.System)
				throw new Exception("Attempting to free memory of a different type.");

			// returns pages freed
			uint result = (uint)CallAddressEx(Kernel.MmFreeSystemMemory, null, true, address, 0);
			if (result == 0)
				throw new Exception("Failure to free xbox memory.");

			// return size of memory freed
			return RemoveAllocationEntry(address);
		}

		private MEMORY_BASIC_INFORMATION QueryVirtualMemory(uint ptr)
		{
			MEMORY_BASIC_INFORMATION info;
            uint result = (uint)CallAddressEx(Kernel.NtQueryVirtualMemory, null, true, ptr, 0x10008);

			info.BaseAddress = GetUInt32(0x10008);
			info.AllocationBase = GetUInt32(0x1000C);
			info.AllocationProtect = (MEMORY_FLAGS)GetUInt32(0x10010);
			info.RegionSize = GetUInt32(0x10014);
			info.State = (MEMORY_FLAGS)GetUInt32(0x10018);
			info.Protect = (MEMORY_FLAGS)GetUInt32(0x1001C);
			info.Type = (MEMORY_FLAGS)GetUInt32(0x10020);

			return info;
		}

		// could use this to set our pages to noaccess after we are done reading/writing...
		// might be a bit time consuming to give ourselves permission and then deny everyone else, every time we need to access our info...
		// worst case scenario of two calls for one byte read/written...is it worth it?
		private void VirtualProtect(uint address, uint size, uint newProtect, out uint oldProtect)
		{
			uint pOldProtect = History.RemoteExecution.ArgumentBuffer;
			uint pSize = History.RemoteExecution.ArgumentBuffer + 4;
			uint pAddress = History.RemoteExecution.ArgumentBuffer + 8;
			SetMemory(pSize, size);
			SetMemory(pAddress, address);

			uint result = (uint)CallAddressEx(Kernel.NtProtectVirtualMemory, null, true, pAddress, pSize, newProtect, pOldProtect);
			oldProtect = GetUInt32(pOldProtect);
		}

		/// <summary>
		/// Retrieves the file cache size associated with the current title.
		/// </summary>
		/// <returns></returns>
		public uint GetFileCacheSize()
		{
			return (uint)CallAddress(Kernel.FscGetCacheSize, true);
		}

		/// <summary>
		/// Allows us to borrow extra memory from the file cache :)
		/// </summary>
		/// <param name="size">Page count.</param>
		/// <returns></returns>
		public bool SetFileCacheSize(uint size)
		{
			//make sure you dont take all of the pages, although perfectly acceptable, some games might freak...
			if (size == 0) size = 1;
			return (uint)CallAddressEx(Kernel.FscSetCacheSize, null, true, size) == 0;
		}
       
        ///// <summary>
        ///// Restores the file cache by taking some of the available memory and giving it back to the cache.
        ///// </summary>
        ///// <returns></returns>
        //private void RestoreFileCache()
        //{
        //    // dont take all available, leave at least a page or two
        //    // restore to the default of 16 pages (64kb)

        //}
		#endregion

		#region History
		/// <summary>
		/// Gets the address of the buffer automatically allocated for temporary storage in xbox memory.
		/// </summary>
        public uint ScratchBuffer { get { return scratchBuffer; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint scratchBuffer = History.TempBuffer.BaseAddress;

		/// <summary>
		/// Gets the size of the temporary buffer allocated on the xbox.
		/// </summary>
        public uint ScratchBufferSize { get { return scratchBufferSize; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint scratchBufferSize = 0x1000;

        // do this when scratch buffer is out of history...
        ///// <summary>
        ///// Allocates a new scratch buffer and frees the old.
        ///// </summary>
        ///// <param name="size"></param>
        //public void ChangeScratchBufferSize(uint size)
        //{
        //    uint memptr = AllocateDebugMemory(size);
        //    FreeMemory(scratchBuffer);
        //    scratchBuffer = memptr;
        //    scratchBufferSize = size;
        //}

		// we leave some form of history on the xbox so if an app that uses yelodebug crashes or quits, 
		// the next time another one reconnects they can read the history and clean up or reuse whatever old memory 
		// the app had allocated, then be back in business ;)


		/// <summary>
		/// Keeps track of history addresses
		/// </summary>
		private static class History
		{
			public const int Size = 0x4000;
			public const uint BaseAddress = 0x40000000;

			public static class Header
			{
				public const uint BaseAddress = 0x40000000; // "Yelo"
				public const int Size = 32;
			}

			public static class AllocationTable
			{
				public const uint CountAddress = 0x40000020;
				public const uint BufferAddress = 0x40000024;
				public const int BufferSize = 4064;
				public const int MaxCount = 450;
			}

            public static class RemoteExecution
            {
                public const uint ArgumentBuffer = 0x40001008;  // use with pointers
                public const uint XmmContextBuffer = 0x40001040;
                public const uint BufferAddress = 0x40001060;   // call/return address, xmm registers (TODO), script buffer
                public const int MaxParameterCount = 10;
                public const int BufferSize = 992;
            }

			public static class Gamepad
			{
				public const uint XInputGetState = 0x40001440;
				public const uint EnabledAddress = 0x40001444;      //bool IsEnabled
				public const uint PortStatusAddress = 0x40001448;       //uint[4] isPluggedIn <--counts get incremented by one if its plugged in
				public const uint OriginalCodeBuffer = 0x40001458;  //byte[10]
				public const uint StateBufferAddress = 0x40001500;  //4 32-byte buffers
				public const uint ScriptAddress = 0x40001580;
			}

			public static class TempBuffer
			{
				public const uint BaseAddress = 0x40003000;
				public const int Size = 4096;
			}
		}

		/// <summary>
		/// Initializes the history page.
		/// </summary>
		private void InitializeHistory()
		{
			if (IsHistoryPresent())
			{
				// restore our current allocations
				AllocationTable = LoadAllocationTable();

				// check other settings like controller hook etc...
				XInputGetState = GetUInt32(History.Gamepad.XInputGetState);
				OriginalGamepadCode = GetMemory(History.Gamepad.OriginalCodeBuffer, 10);
			}
			else
			{
				// allocate memory for our history pages
				AllocateHistoryPages(History.Size);
				SetMemory(History.BaseAddress, 0x6F6C6559);   // "Yelo"
			}
		}

		private bool IsHistoryPresent()
		{
			SendCommand("getmem addr=0x{0} length=4", Convert.ToString(History.BaseAddress, 16));
			string yelo = ReceiveSocketLine().Replace("\r\n", "");
			ReceiveSocketLine();
			return (yelo == "59656C6F");
		}

		/// <summary>
		/// Use in beginning when setting up our history page, since calladdressex will depend on that memory
		/// </summary>
		/// <param name="size"></param>
		/// <returns>Allocated address.</returns>
		private uint AllocateHistoryPages(uint size)
		{
			// calculate actual size of allocation
			size = AlignmentCheck(size);

			// checks if theres enough memory for allocation to take place
			IsEnoughMemory(size);

			#region Reserve the memory
			// store address to call
			SetMemory(0x10000, size);
			SetMemory(0x10004, 0x40000000);

			// inject script
			//push	4	;protect
			//push	2000h	;type
			//push	10000h	;pSize
			//push	0
			//push	10004h	;pAddress
			//mov	eax, 012345678h	;export address
			//call	eax
            //mov	eax, 02DB0000h	;fake success
			//retn	010h
			memoryStream.Position = ScriptBufferAddress;
			byte[] pt1 = { 0x6A, 0x04, 0x68, 0x00, 0x20, 0x00, 0x00, 0x68, 0x00, 0x00, 0x01, 0x00, 0x6A, 0x00, 0x68, 0x04, 0x00, 0x01, 0x00, 0xB8 };
			memoryWriter.Write(pt1);
			memoryWriter.Write(Kernel.NtAllocateVirtualMemory);
			byte[] pt2 = { 0xFF, 0xD0, 0xB8, 0x00, 0x00, 0xDB, 0x02, 0xC2, 0x10, 0x00 };
			memoryWriter.Write(pt2);

            // execute script via hijacked crashdump function
            SendCommand("crashdump");

			// return the value of eax after the call
			uint ptr = GetUInt32(0x10004);

			#endregion

			#region Commit the memory
			// store address to call
			SetMemory(0x10000, size);
			SetMemory(0x10004, 0x40000000);

			// inject script
			//push	4	;protect
			//push	1000h	;type
			//push	10000h	;pSize
			//push	0
			//push	10004h	;pAddress
			//mov	eax, 012345678h	;export address
			//call	eax
            //mov	eax, 02DB0000h	;fake success
			//retn	010h
			memoryStream.Position = ScriptBufferAddress;
			byte[] pt3 = { 0x6A, 0x04, 0x68, 0x00, 0x10, 0x00, 0x00, 0x68, 0x00, 0x00, 0x01, 0x00, 0x6A, 0x00, 0x68, 0x04, 0x00, 0x01, 0x00, 0xB8 };
			memoryWriter.Write(pt3);
			memoryWriter.Write(Kernel.NtAllocateVirtualMemory);
			byte[] pt4 = { 0xFF, 0xD0, 0xB8, 0x00, 0x00, 0xDB, 0x02, 0xC2, 0x10, 0x00 };
			memoryWriter.Write(pt4);

            // execute script via hijacked crashdump function
            SendCommand("crashdump");

			// return the value of eax after the call
			ptr = GetUInt32(0x10004);
			#endregion

			// check for success, but DONT add to our allocation table...
			if (ptr == 0)
				throw new Exception("Failed to initialize YeloDebug in xbox memory.");

			return ptr;
		}
		#endregion

		#region Gamepad
		// keep this stuff locally to avoid repeatedly grabbing it off of the xbox...
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint XInputGetState;    // base address of our xinputgetstate function
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private byte[] OriginalGamepadCode; // store our original code so we can unhook if necessary
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InputState PreviousState = new InputState();  // keep track of our previous state so we can detect changes
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private uint GamepadPacketNumber = 0;   // if we detect any changes, update the packet number

		/// <summary>
		/// Searches xbox memory for XInputGetState(). Average execution time of 15ms.
		/// </summary>
		/// <returns></returns>
		private uint XInputGetStateAddress()
		{
			// assumes everything we need is in header...why would they put it elsewhere???

			// buffer to store our xbe header
			byte[] xbeh = GetMemory(0x10000, (uint)0x1000);
            BinaryReader header = new BinaryReader(new System.IO.MemoryStream(xbeh));

			header.BaseStream.Position = 0x11C;
			uint SegmentCount = header.ReadUInt32();                //gets segment count
			uint SegmentBase = header.ReadUInt32() - 0x10000;       //gets base address of segment info table

			//loops through each segment
			for (int i = 0; i < SegmentCount; i++)
			{
				header.BaseStream.Position = (uint)(SegmentBase + i * 56) + 4;
				uint SegAddress = header.ReadUInt32();
				uint SegSize = header.ReadUInt32();
				header.BaseStream.Position += 8;
				header.BaseStream.Position = header.ReadUInt32() - 0x10000;
				string SegName = ASCIIEncoding.ASCII.GetString(header.ReadBytes(3));

				if (SegName.Equals("XPP"))
				{
					//dumps xpp segment
					byte[] SegDump = GetMemory(SegAddress, SegSize);

					//searches for xinputgetstate function
					for (int j = 0; j < SegSize; j++)
						if ((BitConverter.ToUInt64(SegDump, j) & 0xFFFFFFFFFFFF) == 0x000015FFDB335653)
							return (uint)(SegAddress + j);
				}
			}
			throw new Exception("Unable to find XInputGetState() in memory, you must manually specify this address instead if you wish to initialize a controller hook.");
		}

        /// <summary>
        /// Compares the differences between two states and determines if they have changed.
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        private bool StateChanged(InputState oldState, InputState newState)
        {
            for (int i = 0; i < 8; i++)
                if (oldState.AnalogButtons[i] != newState.AnalogButtons[i])
                    return true;

            if (oldState.Buttons != newState.Buttons ||
                oldState.ThumbRX != newState.ThumbRX ||
                oldState.ThumbRY != newState.ThumbRY ||
                oldState.ThumbLX != newState.ThumbLX ||
                oldState.ThumbLY != newState.ThumbLY)
                return true;

            return false;
        }

		/// <summary>
		/// Detects whether or not we have already hooked into XInputGetState().
		/// </summary>
		/// <returns></returns>
		public bool IsGamepadHooked()
		{
			if (XInputGetState == 0) return false;
			return GetByte(XInputGetState) == 0x68; // test for "push   dword ptr ds:[address]"
		}

		/// <summary>
		/// Hooks into the XInputGetState procedure on your xbox, and hijacks controller input.
		/// </summary>
		public void InitializeControllerHook()
		{
			// dont hook again if we are already hooked
			if (IsGamepadHooked()) return;

			// get hook address
			XInputGetState = XInputGetStateAddress();
			SetMemory(History.Gamepad.XInputGetState, XInputGetState);

			// get first part of original code
			byte[] origCode = GetMemory(XInputGetState, 10);
			SetMemory(History.Gamepad.OriginalCodeBuffer, origCode);

			#region Build Script
			// store our script in memory that reads from our buffers instead...
			byte[] scriptData = new byte[71];
            BinaryWriter script = new BinaryWriter(new System.IO.MemoryStream(scriptData));
			script.BaseStream.Position = 0;

			//cmp	dword ptr ds:[EnabledAddress], 1	;IsEnabled
			//je	Intercept
			//db	10	dup (0)	;replace with orig code
			//push	XInputGetState + 10
			//ret
			//Intercept:
			script.Write((ushort)0x3D83);
			script.Write(History.Gamepad.EnabledAddress);
			script.Write((ushort)0x7401);
			script.Write((byte)0x10);
			script.Write(origCode);
			script.Write((byte)0x68);
			script.Write(XInputGetState + 10);
			script.Write((byte)0xC3);

			//pushad					; esp -= 32
			//mov	eax, [esp + 36]			; get port from handle
			//mov	eax, [eax]
			//mov	eax, [eax]
			//mov	eax, [eax + 14h]
			//add	dword ptr ds:[012345678h + eax * 4], 1	; controller in that port is plugged in
			//shl	eax, 5				; get buffer index
			//lea	esi, [012345678h + eax]		; buffer address
			//mov	edi, [esp + 40]	;pInputState	; their buffer address
			//mov	ecx, 22	;XINPUT_STATE size	; replace their data with ours
			//rep	movsb 
			//popad	
			//xor	eax, eax	;ERROR_SUCCESS
			//retn    8
			byte[] pt1 = { 0x60, 0x8B, 0x44, 0x24, 0x24, 0x8B, 0x00, 0x8B, 0x00, 0x8B, 0x40, 0x14, 0x83, 0x04, 0x85 };
			script.Write(pt1);
			script.Write(History.Gamepad.PortStatusAddress);
			byte[] pt2 = { 0x01, 0xC1, 0xE0, 0x05, 0x8D, 0xB0 };
			script.Write(pt2);
			script.Write(History.Gamepad.StateBufferAddress);
			byte[] pt3 = { 0x8B, 0x7C, 0x24, 0x28, 0xB9, 0x16, 0x00, 0x00, 0x00, 0xF3, 0xA4, 0x61, 0x33, 0xC0, 0xC2, 0x08, 0x00 };
			script.Write(pt3);
			script.Close();
			SetMemory(History.Gamepad.ScriptAddress, scriptData);
			#endregion

			// now inject our hook which jumps to our script we have just created...
			byte[] hookData = new byte[10];
            BinaryWriter hook = new BinaryWriter(new System.IO.MemoryStream(hookData));
			hook.BaseStream.Position = 0;
			hook.Write((byte)0x68);
			hook.Write(History.Gamepad.ScriptAddress);
			hook.Write((byte)0xC3);
			hook.Close();
			SetMemory(XInputGetState, hookData);
		}

        /// <summary>
		/// This is to be used if YeloDebug cannot automatically detect the XInputGetState function address.
		/// Then you will need to specify your own if you wish to hook into the controllers.
        /// This function has not been tested since I have not found a game which doesn't use the standard XInputGetState function.
		/// </summary>
		/// <param name="xinputAddress"></param>
        public void ManuallyInitializeControllerHook(uint xinputAddress)
        {
            // dont hook again if we are already hooked
            if (IsGamepadHooked()) return;

            // get hook address
            XInputGetState = xinputAddress;
            SetMemory(History.Gamepad.XInputGetState, XInputGetState);

            // get first part of original code
            byte[] origCode = GetMemory(XInputGetState, 10);
            SetMemory(History.Gamepad.OriginalCodeBuffer, origCode);

            #region Build Script
            // store our script in memory that reads from our buffers instead...
            byte[] scriptData = new byte[71];
            BinaryWriter script = new BinaryWriter(new System.IO.MemoryStream(scriptData));
            script.BaseStream.Position = 0;

            //cmp	dword ptr ds:[EnabledAddress], 1	;IsEnabled
            //je	Intercept
            //db	10	dup (0)	;replace with orig code
            //push	XInputGetState + 10
            //ret
            //Intercept:
            script.Write((ushort)0x3D83);
            script.Write(History.Gamepad.EnabledAddress);
            script.Write((ushort)0x7401);
            script.Write((byte)0x10);
            script.Write(origCode);
            script.Write((byte)0x68);
            script.Write(XInputGetState + 10);
            script.Write((byte)0xC3);

            //pushad					; esp -= 32
            //mov	eax, [esp + 36]			; get port from handle
            //mov	eax, [eax]
            //mov	eax, [eax]
            //mov	eax, [eax + 14h]
            //add	dword ptr ds:[012345678h + eax * 4], 1	; controller in that port is plugged in
            //shl	eax, 5				; get buffer index
            //lea	esi, [012345678h + eax]		; buffer address
            //mov	edi, [esp + 40]	;pInputState	; their buffer address
            //mov	ecx, 22	;XINPUT_STATE size	; replace their data with ours
            //rep	movsb 
            //popad	
            //xor	eax, eax	;ERROR_SUCCESS
            //retn    8
            byte[] pt1 = { 0x60, 0x8B, 0x44, 0x24, 0x24, 0x8B, 0x00, 0x8B, 0x00, 0x8B, 0x40, 0x14, 0x83, 0x04, 0x85 };
            script.Write(pt1);
            script.Write(History.Gamepad.PortStatusAddress);
            byte[] pt2 = { 0x01, 0xC1, 0xE0, 0x05, 0x8D, 0xB0 };
            script.Write(pt2);
            script.Write(History.Gamepad.StateBufferAddress);
            byte[] pt3 = { 0x8B, 0x7C, 0x24, 0x28, 0xB9, 0x16, 0x00, 0x00, 0x00, 0xF3, 0xA4, 0x61, 0x33, 0xC0, 0xC2, 0x08, 0x00 };
            script.Write(pt3);
            script.Close();
            SetMemory(History.Gamepad.ScriptAddress, scriptData);
            #endregion

            // now inject our hook which jumps to our script we have just created...
            byte[] hookData = new byte[10];
            BinaryWriter hook = new BinaryWriter(new System.IO.MemoryStream(hookData));
            hook.BaseStream.Position = 0;
            hook.Write((byte)0x68);
            hook.Write(History.Gamepad.ScriptAddress);
            hook.Write((byte)0xC3);
            hook.Close();
            SetMemory(XInputGetState, hookData);
        }

		/// <summary>
		/// Specifies if the xbox should override controller input and start listening for pc input.
		/// </summary>
		/// <param name="enabled"></param>
		public void OverrideControllers(bool enabled)
		{
			if (enabled)
				SetMemory(History.Gamepad.EnabledAddress, (int)1);
			else
			{
				// clear states so if we re-enable, it doesnt perform old actions...
				SetMemory(History.Gamepad.StateBufferAddress, new byte[32 * 4]);
				SetMemory(History.Gamepad.EnabledAddress, (int)0);
			}
		}

		/// <summary>
		/// Sends gamepad input to a specified controller port.  
		/// A controller must be plugged into the port for this to work.
		/// You are also responsible for the polling frequency.
		/// </summary>
		/// <param name="port">Controller port. [0-3]</param>
		/// <param name="input">Gamepad input.</param>
		public void SetGamepadState(uint port, InputState input)
		{
			if (port > 3)
				throw new Exception("Invalid controller port specified.  Valid ports are within the range of [0-3].");

			// only update if state has changed...otherwise its just a waste of bandwidth ;p
			if (StateChanged(PreviousState, input))
			{
				// get our XINPUT_STATE address
				uint inputState = History.Gamepad.StateBufferAddress + port * 32;

				// indicate a changed gamepad state
				GamepadPacketNumber++;

				//convert pad to byte array
				byte[] gamepadData = new byte[22];
                BinaryWriter pad = new BinaryWriter(new System.IO.MemoryStream(gamepadData));
				pad.BaseStream.Position = 0;
				pad.Write(GamepadPacketNumber);
				pad.Write((ushort)input.Buttons);
				pad.Write(BitConverter.ToUInt64(input.AnalogButtons, 0));
				pad.Write((short)input.ThumbLX);
				pad.Write((short)input.ThumbLY);
				pad.Write((short)input.ThumbRX);
				pad.Write((short)input.ThumbRY);
				pad.Close();

				// store new state
				SetMemory(inputState, gamepadData);

				// replace old with new
				PreviousState = input;
			}
		}
		#endregion
    };
}