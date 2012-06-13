using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;   // debugger display
using YeloDebug.Exceptions;

namespace YeloDebug
{
	/// <summary>
	/// Xbox kernel information.
	/// </summary>
	public class XboxKernel
	{
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Xbox Xbox;

		#region Exports
		public uint AvGetSavedDataAddress;
		public uint AvSendTVEncoderOption;
		public uint AvSetDisplayMode;
		public uint AvSetSavedDataAddress;
		public uint DbgBreakPoint;
		public uint DbgBreakPointWithStatus;
		public uint DbgLoadImageSymbols;
		public uint DbgPrint;
		public uint HalReadSMCTrayState;
		public uint DbgPrompt;
		public uint DbgUnLoadImageSymbols;
		public uint ExAcquireReadWriteLockExclusive;
		public uint ExAcquireReadWriteLockShared;
		public uint ExAllocatePool;
		public uint ExAllocatePoolWithTag;
		public uint ExEventObjectType;
		public uint ExFreePool;
		public uint ExInitializeReadWriteLock;
		public uint ExInterlockedAddLargeInteger;
		public uint ExInterlockedAddLargeStatistic;
		public uint ExInterlockedCompareExchange64;
		public uint ExMutantObjectType;
		public uint ExQueryPoolBlockSize;
		public uint ExQueryNonVolatileSetting;
		public uint ExReadWriteRefurbInfo;
		public uint ExRaiseException;
		public uint ExRaiseStatus;
		public uint ExReleaseReadWriteLock;
		public uint ExSaveNonVolatileSetting;
		public uint ExSemaphoreObjectType;
		public uint ExTimerObjectType;
		public uint ExfInterlockedInsertHeadList;
		public uint ExfInterlockedInsertTailList;
		public uint ExfInterlockedRemoveHeadList;
		public uint FscGetCacheSize;
		public uint FscInvalidateIdleBlocks;
		public uint FscSetCacheSize;
		public uint HalClearSoftwareInterrupt;
		public uint HalDisableSystemInterrupt;
		public uint IdexDiskPartitionPrefixBuffer;
		public uint HalDiskModelNumber;
		public uint HalDiskSerialNumber;
		public uint HalEnableSystemInterrupt;
		public uint HalGetInterruptVector;
		public uint HalReadSMBusValue;
		public uint HalReadWritePCISpace;
		public uint HalRegisterShutdownNotification;
		public uint HalRequestSoftwareInterrupt;
		public uint HalReturnToFirmware;
		public uint HalWriteSMBusValue;
		public uint InterlockedCompareExchange;
		public uint InterlockedDecrement;
		public uint InterlockedIncrement;
		public uint InterlockedExchange;
		public uint InterlockedExchangeAdd;
		public uint InterlockedFlushSList;
		public uint InterlockedPopEntrySList;
		public uint InterlockedPushEntrySList;
		public uint IoAllocateIrp;
		public uint IoBuildAsynchronousFsdRequest;
		public uint IoBuildDeviceIoControlRequest;
		public uint IoBuildSynchronousFsdRequest;
		public uint IoCheckShareAccess;
		public uint IoCompletionObjectType;
		public uint IoCreateDevice;
		public uint IoCreateFile;
		public uint IoCreateSymbolicLink;
		public uint IoDeleteDevice;
		public uint IoDeleteSymbolicLink;
		public uint IoFileObjectType;
		public uint IoFreeIrp;
		public uint IoInitializeIrp;
		public uint IoInvalidDeviceRequest;
		public uint IoQueryFileInformation;
		public uint IoQueryVolumeInformation;
		public uint IoQueueThreadIrp;
		public uint IoRemoveShareAccess;
		public uint IoSetIoCompletion;
		public uint IoSetShareAccess;
		public uint IoStartNextPacket;
		public uint IoStartNextPacketByKey;
		public uint IoStartPacket;
		public uint IoSynchronousDeviceIoControlRequest;
		public uint IoSynchronousFsdRequest;
		public uint IofCallDriver;
		public uint IofCompleteRequest;
		public uint KdDebuggerEnabled;
		public uint KdDebuggerNotPresent;
		public uint IoDismountVolume;
		public uint IoDismountVolumeByName;
		public uint KeAlertResumeThread;
		public uint KeAlertThread;
		public uint KeBoostPriorityThread;
		public uint KeBugCheck;
		public uint KeBugCheckEx;
		public uint KeCancelTimer;
		public uint KeConnectInterrupt;
		public uint KeDelayExecutionThread;
		public uint KeDisconnectInterrupt;
		public uint KeEnterCriticalRegion;
		public uint MmGlobalData;
		public uint KeGetCurrentIrql;
		public uint KeGetCurrentThread;
		public uint KeInitializeApc;
		public uint KeInitializeDeviceQueue;
		public uint KeInitializeDpc;
		public uint KeInitializeEvent;
		public uint KeInitializeInterrupt;
		public uint KeInitializeMutant;
		public uint KeInitializeQueue;
		public uint KeInitializeSemaphore;
		public uint KeInitializeTimerEx;
		public uint KeInsertByKeyDeviceQueue;
		public uint KeInsertDeviceQueue;
		public uint KeInsertHeadQueue;
		public uint KeInsertQueue;
		public uint KeInsertQueueApc;
		public uint KeInsertQueueDpc;
		public uint KeInterruptTime;
		public uint KeIsExecutingDpc;
		public uint KeLeaveCriticalRegion;
		public uint KePulseEvent;
		public uint KeQueryBasePriorityThread;
		public uint KeQueryInterruptTime;
		public uint KeQueryPerformanceCounter;
		public uint KeQueryPerformanceFrequency;
		public uint KeQuerySystemTime;
		public uint KeRaiseIrqlToDpcLevel;
		public uint KeRaiseIrqlToSynchLevel;
		public uint KeReleaseMutant;
		public uint KeReleaseSemaphore;
		public uint KeRemoveByKeyDeviceQueue;
		public uint KeRemoveDeviceQueue;
		public uint KeRemoveEntryDeviceQueue;
		public uint KeRemoveQueue;
		public uint KeRemoveQueueDpc;
		public uint KeResetEvent;
		public uint KeRestoreFloatingPointState;
		public uint KeResumeThread;
		public uint KeRundownQueue;
		public uint KeSaveFloatingPointState;
		public uint KeSetBasePriorityThread;
		public uint KeSetDisableBoostThread;
		public uint KeSetEvent;
		public uint KeSetEventBoostPriority;
		public uint KeSetPriorityProcess;
		public uint KeSetPriorityThread;
		public uint KeSetTimer;
		public uint KeSetTimerEx;
		public uint KeStallExecutionProcessor;
		public uint KeSuspendThread;
		public uint KeSynchronizeExecution;
		public uint KeSystemTime;
		public uint KeTestAlertThread;
		public uint KeTickCount;
		public uint KeTimeIncrement;
		public uint KeWaitForMultipleObjects;
		public uint KeWaitForSingleObject;
		public uint KfRaiseIrql;
		public uint KfLowerIrql;
		public uint KiBugCheckData;
		public uint KiUnlockDispatcherDatabase;
		public uint LaunchDataPage;
		public uint MmAllocateContiguousMemory;
		public uint MmAllocateContiguousMemoryEx;
		public uint MmAllocateSystemMemory;
		public uint MmClaimGpuInstanceMemory;
		public uint MmCreateKernelStack;
		public uint MmDeleteKernelStack;
		public uint MmFreeContiguousMemory;
		public uint MmFreeSystemMemory;
		public uint MmGetPhysicalAddress;
		public uint MmIsAddressValid;
		public uint MmLockUnlockBufferPages;
		public uint MmLockUnlockPhysicalPage;
		public uint MmMapIoSpace;
		public uint MmPersistContiguousMemory;
		public uint MmQueryAddressProtect;
		public uint MmQueryAllocationSize;
		public uint MmQueryStatistics;
		public uint MmSetAddressProtect;
		public uint MmUnmapIoSpace;
		public uint NtAllocateVirtualMemory;
		public uint NtCancelTimer;
		public uint NtClearEvent;
		public uint NtClose;
		public uint NtCreateDirectoryObject;
		public uint NtCreateEvent;
		public uint NtCreateFile;
		public uint NtCreateIoCompletion;
		public uint NtCreateMutant;
		public uint NtCreateSemaphore;
		public uint NtCreateTimer;
		public uint NtDeleteFile;
		public uint NtDeviceIoControlFile;
		public uint NtDuplicateObject;
		public uint NtFlushBuffersFile;
		public uint NtFreeVirtualMemory;
		public uint NtFsControlFile;
		public uint NtOpenDirectoryObject;
		public uint NtOpenFile;
		public uint NtOpenSymbolicLinkObject;
		public uint NtProtectVirtualMemory;
		public uint NtPulseEvent;
		public uint NtQueueApcThread;
		public uint NtQueryDirectoryFile;
		public uint NtQueryDirectoryObject;
		public uint NtQueryEvent;
		public uint NtQueryFullAttributesFile;
		public uint NtQueryInformationFile;
		public uint NtQueryIoCompletion;
		public uint NtQueryMutant;
		public uint NtQuerySemaphore;
		public uint NtQuerySymbolicLinkObject;
		public uint NtQueryTimer;
		public uint NtQueryVirtualMemory;
		public uint NtQueryVolumeInformationFile;
		public uint NtReadFile;
		public uint NtReadFileScatter;
		public uint NtReleaseMutant;
		public uint NtReleaseSemaphore;
		public uint NtRemoveIoCompletion;
		public uint NtResumeThread;
		public uint NtSetEvent;
		public uint NtSetInformationFile;
		public uint NtSetIoCompletion;
		public uint NtSetSystemTime;
		public uint NtSetTimerEx;
		public uint NtSignalAndWaitForSingleObjectEx;
		public uint NtSuspendThread;
		public uint NtUserIoApcDispatcher;
		public uint NtWaitForSingleObject;
		public uint NtWaitForSingleObjectEx;
		public uint NtWaitForMultipleObjectsEx;
		public uint NtWriteFile;
		public uint NtWriteFileGather;
		public uint NtYieldExecution;
		public uint ObCreateObject;
		public uint ObDirectoryObjectType;
		public uint ObInsertObject;
		public uint ObMakeTemporaryObject;
		public uint ObOpenObjectByName;
		public uint ObOpenObjectByPointer;
		public uint ObpObjectHandleTable;
		public uint ObReferenceObjectByHandle;
		public uint ObReferenceObjectByName;
		public uint ObReferenceObjectByPointer;
		public uint ObSymbolicLinkObjectType;
		public uint ObfDereferenceObject;
		public uint ObfReferenceObject;
		public uint PhyGetLinkState;
		public uint PhyInitialize;
		public uint PsCreateSystemThread;
		public uint PsCreateSystemThreadEx;
		public uint PsQueryStatistics;
		public uint PsSetCreateThreadNotifyRoutine;
		public uint PsTerminateSystemThread;
		public uint PsThreadObjectType;
		public uint RtlAnsiStringToUnicodeString;
		public uint RtlAppendStringToString;
		public uint RtlAppendUnicodeStringToString;
		public uint RtlAppendUnicodeToString;
		public uint RtlAssert;
		public uint RtlCaptureContext;
		public uint RtlCaptureStackBackTrace;
		public uint RtlCharToInteger;
		public uint RtlCompareMemory;
		public uint RtlCompareMemoryUlong;
		public uint RtlCompareString;
		public uint RtlCompareUnicodeString;
		public uint RtlCopyString;
		public uint RtlCopyUnicodeString;
		public uint RtlCreateUnicodeString;
		public uint RtlDowncaseUnicodeChar;
		public uint RtlDowncaseUnicodeString;
		public uint RtlEnterCriticalSection;
		public uint RtlEnterCriticalSectionAndRegion;
		public uint RtlEqualString;
		public uint RtlEqualUnicodeString;
		public uint RtlExtendedIntegerMultiply;
		public uint RtlExtendedLargeIntegerDivide;
		public uint RtlExtendedMagicDivide;
		public uint RtlFillMemory;
		public uint RtlFillMemoryUlong;
		public uint RtlFreeAnsiString;
		public uint RtlGetCallersAddress;
		public uint RtlInitAnsiString;
		public uint RtlInitUnicodeString;
		public uint RtlInitializeCriticalSection;
		public uint RtlIntegerToChar;
		public uint RtlIntegerToUnicodeString;
		public uint RtlLeaveCriticalSection;
		public uint RtlLeaveCriticalSectionAndRegion;
		public uint RtlLowerChar;
		public uint RtlMapGenericMask;
		public uint RtlMoveMemory;
		public uint RtlMultiByteToUnicodeN;
		public uint RtlMultiByteToUnicodeSize;
		public uint RtlNtStatusToDosError;
		public uint RtlRaiseException;
		public uint RtlRaiseStatus;
		public uint RtlTimeFieldsToTime;
		public uint RtlTimeToTimeFields;
		public uint RtlTryEnterCriticalSection;
		public uint RtlUlongByteSwap;
		public uint RtlUnicodeStringToAnsiString;
		public uint RtlUnicodeStringToInteger;
		public uint RtlUnicodeToMultiByteN;
		public uint RtlUnicodeToMultiByteSize;
		public uint RtlUnwind;
		public uint RtlUpcaseUnicodeChar;
		public uint RtlUpcaseUnicodeString;
		public uint RtlUpcaseUnicodeToMultiByteN;
		public uint RtlUpperChar;
		public uint RtlUpperString;
		public uint RtlUshortByteSwap;
		public uint RtlWalkFrameChain;
		public uint RtlZeroMemory;
		public uint XboxEEPROMKey;
		public uint HardwareInfo;
		public uint XboxHDKey;
		public uint XboxKrnlVersion;
		public uint XboxSignatureKey;
		public uint XeImageFileName;
		public uint XeLoadSection;
		public uint XeUnloadSection;
		public uint READ_PORT_BUFFER_UCHAR;
		public uint READ_PORT_BUFFER_USHORT;
		public uint READ_PORT_BUFFER_ULONG;
		public uint WRITE_PORT_BUFFER_UCHAR;
		public uint WRITE_PORT_BUFFER_USHORT;
		public uint WRITE_PORT_BUFFER_ULONG;
		public uint XcSHAInit;
		public uint XcSHAUpdate;
		public uint XcSHAFinal;
		public uint XcRC4Key;
		public uint XcRC4Crypt;
		public uint XcHMAC;
		public uint XcPKEncPublic;
		public uint XcPKDecPrivate;
		public uint XcPKGetKeyLen;
		public uint XcVerifyPKCS1Signature;
		public uint XcModExp;
		public uint XcDESKeyParity;
		public uint XcKeyTable;
		public uint XcBlockCrypt;
		public uint XcBlockCryptCBC;
		public uint XcCryptService;
		public uint XcUpdateCrypto;
		public uint RtlRip;
		public uint XboxLANKey;
		public uint XboxAlternateSignatureKeys;
		public uint XePublicKeyData;
		public uint HalBootSMCVideoMode;
		public uint IdexChannelObject;
		public uint HalIsResetOrShutdownPending;
		public uint IoMarkIrpMustComplete;
		public uint HalInitiateShutdown;
		public uint snprintf;
		public uint sprintf;
		public uint vsnprintf;
		public uint vsprintf;
		public uint HalEnableSecureTrayEject;
		public uint HalWriteSMCScratchRegister;
		public uint MmDbgAllocateMemory;

		/// <summary>
		/// Returns number of pages released.
		/// </summary>
		public uint MmDbgFreeMemory;
		public uint MmDbgQueryAvailablePages;
		public uint MmDbgReleaseAddress;
		public uint MmDbgWriteCheck;
		#endregion

		#region Properties
		public static uint Base	{ get { return 0x80010000; } }
		public uint Size
        {
            get
            {
                uint peBase = Xbox.GetUInt32(0x8001003C);
                return Xbox.GetUInt32(Base + peBase + 0x30);
            }
        }
		#endregion

		#region Constructor
		/// <summary>
		/// Contains everything needed to communicate with the Xbox kernel.
		/// </summary>
		/// <param name="xbox">Connection to use.</param>
        public XboxKernel(Xbox xbox)
		{
			Xbox = xbox;
			if (xbox == null)
				throw new NoConnectionException("Requires debug connection.");
			xbox.ConnectionCheck();
			try
			{
				InitializeKernelExports();
			}
			catch
			{
				SaveAsFile();
				throw new ApiException("Failed to obtain kernel exports.  The kernel has been saved to your computer for further examination.");
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Saves xbox kernel as local file on pc.
		/// </summary>
		public void SaveAsFile()
		{
			Xbox.ConnectionCheck();

			//dumps kernel
			byte[] KrnlDump = Xbox.GetMemory(Base, Size);

			//saves kernel
            FileStream fs = new FileStream(Xbox.Version + " Kernel v" + Xbox.KernelVersion + " " + Xbox.Modules[0].TimeStamp.ToString().Replace(':', '.').Replace('/','.') + " (UTC) .exe", FileMode.Create);
			fs.Write(KrnlDump, 0, KrnlDump.Length);
			fs.Close();
		}

		/// <summary>
		/// Retrieves kernel export table.
		/// </summary>
		/// <returns>Kernel export table.</returns>
		private uint[] GetExportTable()
		{
			Xbox.ConnectionCheck();

			uint TempPtr;
			uint ExportCount;

			//gets export table info
			TempPtr = Xbox.GetUInt32(Base + 0x3C);
			TempPtr = Xbox.GetUInt32(Base + TempPtr + 0x78);
			ExportCount = Xbox.GetUInt32(Base + TempPtr + 0x14);
			TempPtr = Base + Xbox.GetUInt32(Base + TempPtr + 0x1C);        //export table base address

			//dumps raw export table
			byte[] RawExportTable = Xbox.GetMemory(TempPtr, ExportCount * 4);

			//adjusts for actual addresses
			uint[] ExportTable = new uint[450];
			for (int i = 0; i < ExportCount; i++)
				ExportTable[i + 1] = Base + BitConverter.ToUInt32(RawExportTable, i * 4);

			return ExportTable;
		}

		/// <summary>
		/// Initializes kernel exports with actual addresses.
		/// </summary>
		private void InitializeKernelExports()
		{
			uint[] ExportTable = GetExportTable();

			// Index signifies ordinal number
			AvGetSavedDataAddress = ExportTable[1];
			AvSendTVEncoderOption = ExportTable[2];
			AvSetDisplayMode = ExportTable[3];
			AvSetSavedDataAddress = ExportTable[4];
			DbgBreakPoint = ExportTable[5];
			DbgBreakPointWithStatus = ExportTable[6];
			DbgLoadImageSymbols = ExportTable[7];
			DbgPrint = ExportTable[8];
			HalReadSMCTrayState = ExportTable[9];
			DbgPrompt = ExportTable[10];
			DbgUnLoadImageSymbols = ExportTable[11];
			ExAcquireReadWriteLockExclusive = ExportTable[12];
			ExAcquireReadWriteLockShared = ExportTable[13];
			ExAllocatePool = ExportTable[14];
			ExAllocatePoolWithTag = ExportTable[15];
			ExEventObjectType = ExportTable[16];
			ExFreePool = ExportTable[17];
			ExInitializeReadWriteLock = ExportTable[18];
			ExInterlockedAddLargeInteger = ExportTable[19];
			ExInterlockedAddLargeStatistic = ExportTable[20];
			ExInterlockedCompareExchange64 = ExportTable[21];
			ExMutantObjectType = ExportTable[22];
			ExQueryPoolBlockSize = ExportTable[23];
			ExQueryNonVolatileSetting = ExportTable[24];
			ExReadWriteRefurbInfo = ExportTable[25];
			ExRaiseException = ExportTable[26];
			ExRaiseStatus = ExportTable[27];
			ExReleaseReadWriteLock = ExportTable[28];
			ExSaveNonVolatileSetting = ExportTable[29];
			ExSemaphoreObjectType = ExportTable[30];
			ExTimerObjectType = ExportTable[31];
			ExfInterlockedInsertHeadList = ExportTable[32];
			ExfInterlockedInsertTailList = ExportTable[33];
			ExfInterlockedRemoveHeadList = ExportTable[34];
			FscGetCacheSize = ExportTable[35];
			FscInvalidateIdleBlocks = ExportTable[36];
			FscSetCacheSize = ExportTable[37];
			HalClearSoftwareInterrupt = ExportTable[38];
			HalDisableSystemInterrupt = ExportTable[39];
			IdexDiskPartitionPrefixBuffer = ExportTable[40];
			HalDiskModelNumber = ExportTable[41];
			HalDiskSerialNumber = ExportTable[42];
			HalEnableSystemInterrupt = ExportTable[43];
			HalGetInterruptVector = ExportTable[44];
			HalReadSMBusValue = ExportTable[45];
			HalReadWritePCISpace = ExportTable[46];
			HalRegisterShutdownNotification = ExportTable[47];
			HalRequestSoftwareInterrupt = ExportTable[48];
			HalReturnToFirmware = ExportTable[49];
			HalWriteSMBusValue = ExportTable[50];
			InterlockedCompareExchange = ExportTable[51];
			InterlockedDecrement = ExportTable[52];
			InterlockedIncrement = ExportTable[53];
			InterlockedExchange = ExportTable[54];
			InterlockedExchangeAdd = ExportTable[55];
			InterlockedFlushSList = ExportTable[56];
			InterlockedPopEntrySList = ExportTable[57];
			InterlockedPushEntrySList = ExportTable[58];
			IoAllocateIrp = ExportTable[59];
			IoBuildAsynchronousFsdRequest = ExportTable[60];
			IoBuildDeviceIoControlRequest = ExportTable[61];
			IoBuildSynchronousFsdRequest = ExportTable[62];
			IoCheckShareAccess = ExportTable[63];
			IoCompletionObjectType = ExportTable[64];
			IoCreateDevice = ExportTable[65];
			IoCreateFile = ExportTable[66];
			IoCreateSymbolicLink = ExportTable[67];
			IoDeleteDevice = ExportTable[68];
			IoDeleteSymbolicLink = ExportTable[69];
			IoFileObjectType = ExportTable[71];
			IoFreeIrp = ExportTable[72];
			IoInitializeIrp = ExportTable[73];
			IoInvalidDeviceRequest = ExportTable[74];
			IoQueryFileInformation = ExportTable[75];
			IoQueryVolumeInformation = ExportTable[76];
			IoQueueThreadIrp = ExportTable[77];
			IoRemoveShareAccess = ExportTable[78];
			IoSetIoCompletion = ExportTable[79];
			IoSetShareAccess = ExportTable[80];
			IoStartNextPacket = ExportTable[81];
			IoStartNextPacketByKey = ExportTable[82];
			IoStartPacket = ExportTable[83];
			IoSynchronousDeviceIoControlRequest = ExportTable[84];
			IoSynchronousFsdRequest = ExportTable[85];
			IofCallDriver = ExportTable[86];
			IofCompleteRequest = ExportTable[87];
			KdDebuggerEnabled = ExportTable[88];
			KdDebuggerNotPresent = ExportTable[89];
			IoDismountVolume = ExportTable[90];
			IoDismountVolumeByName = ExportTable[91];
			KeAlertResumeThread = ExportTable[92];
			KeAlertThread = ExportTable[93];
			KeBoostPriorityThread = ExportTable[94];
			KeBugCheck = ExportTable[95];
			KeBugCheckEx = ExportTable[96];
			KeCancelTimer = ExportTable[97];
			KeConnectInterrupt = ExportTable[98];
			KeDelayExecutionThread = ExportTable[99];
			KeDisconnectInterrupt = ExportTable[100];
			KeEnterCriticalRegion = ExportTable[101];
			MmGlobalData = ExportTable[102];
			KeGetCurrentIrql = ExportTable[103];
			KeGetCurrentThread = ExportTable[104];
			KeInitializeApc = ExportTable[105];
			KeInitializeDeviceQueue = ExportTable[106];
			KeInitializeDpc = ExportTable[107];
			KeInitializeEvent = ExportTable[108];
			KeInitializeInterrupt = ExportTable[109];
			KeInitializeMutant = ExportTable[110];
			KeInitializeQueue = ExportTable[111];
			KeInitializeSemaphore = ExportTable[112];
			KeInitializeTimerEx = ExportTable[113];
			KeInsertByKeyDeviceQueue = ExportTable[114];
			KeInsertDeviceQueue = ExportTable[115];
			KeInsertHeadQueue = ExportTable[116];
			KeInsertQueue = ExportTable[117];
			KeInsertQueueApc = ExportTable[118];
			KeInsertQueueDpc = ExportTable[119];
			KeInterruptTime = ExportTable[120];
			KeIsExecutingDpc = ExportTable[121];
			KeLeaveCriticalRegion = ExportTable[122];
			KePulseEvent = ExportTable[123];
			KeQueryBasePriorityThread = ExportTable[124];
			KeQueryInterruptTime = ExportTable[125];
			KeQueryPerformanceCounter = ExportTable[126];
			KeQueryPerformanceFrequency = ExportTable[127];
			KeQuerySystemTime = ExportTable[128];
			KeRaiseIrqlToDpcLevel = ExportTable[129];
			KeRaiseIrqlToSynchLevel = ExportTable[130];
			KeReleaseMutant = ExportTable[131];
			KeReleaseSemaphore = ExportTable[132];
			KeRemoveByKeyDeviceQueue = ExportTable[133];
			KeRemoveDeviceQueue = ExportTable[134];
			KeRemoveEntryDeviceQueue = ExportTable[135];
			KeRemoveQueue = ExportTable[136];
			KeRemoveQueueDpc = ExportTable[137];
			KeResetEvent = ExportTable[138];
			KeRestoreFloatingPointState = ExportTable[139];
			KeResumeThread = ExportTable[140];
			KeRundownQueue = ExportTable[141];
			KeSaveFloatingPointState = ExportTable[142];
			KeSetBasePriorityThread = ExportTable[143];
			KeSetDisableBoostThread = ExportTable[144];
			KeSetEvent = ExportTable[145];
			KeSetEventBoostPriority = ExportTable[146];
			KeSetPriorityProcess = ExportTable[147];
			KeSetPriorityThread = ExportTable[148];
			KeSetTimer = ExportTable[149];
			KeSetTimerEx = ExportTable[150];
			KeStallExecutionProcessor = ExportTable[151];
			KeSuspendThread = ExportTable[152];
			KeSynchronizeExecution = ExportTable[153];
			KeSystemTime = ExportTable[154];
			KeTestAlertThread = ExportTable[155];
			KeTickCount = ExportTable[156];
			KeTimeIncrement = ExportTable[157];
			KeWaitForMultipleObjects = ExportTable[158];
			KeWaitForSingleObject = ExportTable[159];
			KfRaiseIrql = ExportTable[160];
			KfLowerIrql = ExportTable[161];
			KiBugCheckData = ExportTable[162];
			KiUnlockDispatcherDatabase = ExportTable[163];
			LaunchDataPage = ExportTable[164];
			MmAllocateContiguousMemory = ExportTable[165];
			MmAllocateContiguousMemoryEx = ExportTable[166];
			MmAllocateSystemMemory = ExportTable[167];
			MmClaimGpuInstanceMemory = ExportTable[168];
			MmCreateKernelStack = ExportTable[169];
			MmDeleteKernelStack = ExportTable[170];
			MmFreeContiguousMemory = ExportTable[171];
			MmFreeSystemMemory = ExportTable[172];
			MmGetPhysicalAddress = ExportTable[172];
			MmIsAddressValid = ExportTable[174];
			MmLockUnlockBufferPages = ExportTable[175];
			MmLockUnlockPhysicalPage = ExportTable[176];
			MmMapIoSpace = ExportTable[177];
			MmPersistContiguousMemory = ExportTable[178];
			MmQueryAddressProtect = ExportTable[179];
			MmQueryAllocationSize = ExportTable[180];
			MmQueryStatistics = ExportTable[181];
			MmSetAddressProtect = ExportTable[182];
			MmUnmapIoSpace = ExportTable[183];
			NtAllocateVirtualMemory = ExportTable[184];
			NtCancelTimer = ExportTable[185];
			NtClearEvent = ExportTable[186];
			NtClose = ExportTable[187];
			NtCreateDirectoryObject = ExportTable[188];
			NtCreateEvent = ExportTable[189];
			NtCreateFile = ExportTable[190];
			NtCreateIoCompletion = ExportTable[191];
			NtCreateMutant = ExportTable[192];
			NtCreateSemaphore = ExportTable[193];
			NtCreateTimer = ExportTable[194];
			NtDeleteFile = ExportTable[195];
			NtDeviceIoControlFile = ExportTable[196];
			NtDuplicateObject = ExportTable[197];
			NtFlushBuffersFile = ExportTable[198];
			NtFreeVirtualMemory = ExportTable[199];
			NtFsControlFile = ExportTable[200];
			NtOpenDirectoryObject = ExportTable[201];
			NtOpenFile = ExportTable[202];
			NtOpenSymbolicLinkObject = ExportTable[203];
			NtProtectVirtualMemory = ExportTable[204];
			NtPulseEvent = ExportTable[205];
			NtQueueApcThread = ExportTable[206];
			NtQueryDirectoryFile = ExportTable[207];
			NtQueryDirectoryObject = ExportTable[208];
			NtQueryEvent = ExportTable[209];
			NtQueryFullAttributesFile = ExportTable[210];
			NtQueryInformationFile = ExportTable[211];
			NtQueryIoCompletion = ExportTable[212];
			NtQueryMutant = ExportTable[213];
			NtQuerySemaphore = ExportTable[214];
			NtQuerySymbolicLinkObject = ExportTable[215];
			NtQueryTimer = ExportTable[216];
			NtQueryVirtualMemory = ExportTable[217];
			NtQueryVolumeInformationFile = ExportTable[218];
			NtReadFile = ExportTable[219];
			NtReadFileScatter = ExportTable[220];
			NtReleaseMutant = ExportTable[221];
			NtReleaseSemaphore = ExportTable[222];
			NtRemoveIoCompletion = ExportTable[223];
			NtResumeThread = ExportTable[224];
			NtSetEvent = ExportTable[225];
			NtSetInformationFile = ExportTable[226];
			NtSetIoCompletion = ExportTable[227];
			NtSetSystemTime = ExportTable[228];
			NtSetTimerEx = ExportTable[229];
			NtSignalAndWaitForSingleObjectEx = ExportTable[230];
			NtSuspendThread = ExportTable[231];
			NtUserIoApcDispatcher = ExportTable[232];
			NtWaitForSingleObject = ExportTable[233];
			NtWaitForSingleObjectEx = ExportTable[234];
			NtWaitForMultipleObjectsEx = ExportTable[235];
			NtWriteFile = ExportTable[236];
			NtWriteFileGather = ExportTable[237];
			NtYieldExecution = ExportTable[238];
			ObCreateObject = ExportTable[239];
			ObDirectoryObjectType = ExportTable[240];
			ObInsertObject = ExportTable[241];
			ObMakeTemporaryObject = ExportTable[242];
			ObOpenObjectByName = ExportTable[243];
			ObOpenObjectByPointer = ExportTable[244];
			ObpObjectHandleTable = ExportTable[245];
			ObReferenceObjectByHandle = ExportTable[246];
			ObReferenceObjectByName = ExportTable[247];
			ObReferenceObjectByPointer = ExportTable[248];
			ObSymbolicLinkObjectType = ExportTable[249];
			ObfDereferenceObject = ExportTable[250];
			ObfReferenceObject = ExportTable[251];
			PhyGetLinkState = ExportTable[252];
			PhyInitialize = ExportTable[253];
			PsCreateSystemThread = ExportTable[254];
			PsCreateSystemThreadEx = ExportTable[255];
			PsQueryStatistics = ExportTable[256];
			PsSetCreateThreadNotifyRoutine = ExportTable[257];
			PsTerminateSystemThread = ExportTable[258];
			PsThreadObjectType = ExportTable[259];
			RtlAnsiStringToUnicodeString = ExportTable[260];
			RtlAppendStringToString = ExportTable[261];
			RtlAppendUnicodeStringToString = ExportTable[262];
			RtlAppendUnicodeToString = ExportTable[263];
			RtlAssert = ExportTable[264];
			RtlCaptureContext = ExportTable[265];
			RtlCaptureStackBackTrace = ExportTable[266];
			RtlCharToInteger = ExportTable[267];
			RtlCompareMemory = ExportTable[268];
			RtlCompareMemoryUlong = ExportTable[269];
			RtlCompareString = ExportTable[270];
			RtlCompareUnicodeString = ExportTable[271];
			RtlCopyString = ExportTable[272];
			RtlCopyUnicodeString = ExportTable[273];
			RtlCreateUnicodeString = ExportTable[274];
			RtlDowncaseUnicodeChar = ExportTable[275];
			RtlDowncaseUnicodeString = ExportTable[276];
			RtlEnterCriticalSection = ExportTable[277];
			RtlEnterCriticalSectionAndRegion = ExportTable[278];
			RtlEqualString = ExportTable[279];
			RtlEqualUnicodeString = ExportTable[280];
			RtlExtendedIntegerMultiply = ExportTable[281];
			RtlExtendedLargeIntegerDivide = ExportTable[282];
			RtlExtendedMagicDivide = ExportTable[283];
			RtlFillMemory = ExportTable[284];
			RtlFillMemoryUlong = ExportTable[285];
			RtlFreeAnsiString = ExportTable[286];
			RtlGetCallersAddress = ExportTable[288];
			RtlInitAnsiString = ExportTable[289];
			RtlInitUnicodeString = ExportTable[290];
			RtlInitializeCriticalSection = ExportTable[291];
			RtlIntegerToChar = ExportTable[292];
			RtlIntegerToUnicodeString = ExportTable[293];
			RtlLeaveCriticalSection = ExportTable[294];
			RtlLeaveCriticalSectionAndRegion = ExportTable[295];
			RtlLowerChar = ExportTable[296];
			RtlMapGenericMask = ExportTable[297];
			RtlMoveMemory = ExportTable[298];
			RtlMultiByteToUnicodeN = ExportTable[299];
			RtlMultiByteToUnicodeSize = ExportTable[300];
			RtlNtStatusToDosError = ExportTable[301];
			RtlRaiseException = ExportTable[302];
			RtlRaiseStatus = ExportTable[303];
			RtlTimeFieldsToTime = ExportTable[304];
			RtlTimeToTimeFields = ExportTable[305];
			RtlTryEnterCriticalSection = ExportTable[306];
			RtlUlongByteSwap = ExportTable[307];
			RtlUnicodeStringToAnsiString = ExportTable[308];
			RtlUnicodeStringToInteger = ExportTable[309];
			RtlUnicodeToMultiByteN = ExportTable[310];
			RtlUnicodeToMultiByteSize = ExportTable[311];
			RtlUnwind = ExportTable[312];
			RtlUpcaseUnicodeChar = ExportTable[313];
			RtlUpcaseUnicodeString = ExportTable[314];
			RtlUpcaseUnicodeToMultiByteN = ExportTable[315];
			RtlUpperChar = ExportTable[316];
			RtlUpperString = ExportTable[317];
			RtlUshortByteSwap = ExportTable[318];
			RtlWalkFrameChain = ExportTable[319];
			RtlZeroMemory = ExportTable[320];
			XboxEEPROMKey = ExportTable[321];
			HardwareInfo = ExportTable[322];
			XboxHDKey = ExportTable[323];
			XboxKrnlVersion = ExportTable[324];
			XboxSignatureKey = ExportTable[325];
			XeImageFileName = ExportTable[326];
			XeLoadSection = ExportTable[327];
			XeUnloadSection = ExportTable[328];
			READ_PORT_BUFFER_UCHAR = ExportTable[329];
			READ_PORT_BUFFER_USHORT = ExportTable[330];
			READ_PORT_BUFFER_ULONG = ExportTable[331];
			WRITE_PORT_BUFFER_UCHAR = ExportTable[332];
			WRITE_PORT_BUFFER_USHORT = ExportTable[333];
			WRITE_PORT_BUFFER_ULONG = ExportTable[334];
			XcSHAInit = ExportTable[335];
			XcSHAUpdate = ExportTable[336];
			XcSHAFinal = ExportTable[337];
			XcRC4Key = ExportTable[338];
			XcRC4Crypt = ExportTable[339];
			XcHMAC = ExportTable[340];
			XcPKEncPublic = ExportTable[341];
			XcPKDecPrivate = ExportTable[342];
			XcPKGetKeyLen = ExportTable[343];
			XcVerifyPKCS1Signature = ExportTable[344];
			XcModExp = ExportTable[345];
			XcDESKeyParity = ExportTable[346];
			XcKeyTable = ExportTable[347];
			XcBlockCrypt = ExportTable[348];
			XcBlockCryptCBC = ExportTable[349];
			XcCryptService = ExportTable[350];
			XcUpdateCrypto = ExportTable[351];
			RtlRip = ExportTable[352];
			XboxLANKey = ExportTable[353];
			XboxAlternateSignatureKeys = ExportTable[354];
			XePublicKeyData = ExportTable[355];
			HalBootSMCVideoMode = ExportTable[356];
			IdexChannelObject = ExportTable[357];
			HalIsResetOrShutdownPending = ExportTable[358];
			IoMarkIrpMustComplete = ExportTable[359];
			HalInitiateShutdown = ExportTable[360];
			snprintf = ExportTable[361];
			sprintf = ExportTable[362];
			vsnprintf = ExportTable[363];
			vsprintf = ExportTable[364];
			HalEnableSecureTrayEject = ExportTable[365];
			HalWriteSMCScratchRegister = ExportTable[366];
			MmDbgAllocateMemory = ExportTable[374];
			MmDbgFreeMemory = ExportTable[375];
			MmDbgQueryAvailablePages = ExportTable[376];
			MmDbgReleaseAddress = ExportTable[377];
			MmDbgWriteCheck = ExportTable[378];
		}

		private uint GetExportAddress(uint ordinal)
		{
			// traverse through export table info
			uint addr = Xbox.GetUInt32(Base + 0x3C);
			addr = Xbox.GetUInt32(Base + addr + 0x78);
			addr = Xbox.GetUInt32(Base + addr + 0x1C);
			addr += Base;

			// grab export address
			addr = Xbox.GetUInt32(addr + (ordinal - 1) * 4);

			return Base + addr;
		}

		#endregion
	};
}