#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.UAC
{
    using System;
    using System.Runtime.InteropServices;

	#region Enums

	public enum ElevationType
	{
		TokenElevationTypeDefault = 1,
		TokenElevationTypeFull = 2,
		TokenElevationTypeLimited = 3
	}

	public enum Platform : byte 
    {
		X86,
		X64,
		Unknown
	}

	#endregion

	//sample by Tim Anderson http://www.itwriting.com/blog
	//Email: tim@itwriting.com

	//THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
	//ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED
	//TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
	//PARTICULAR PURPOSE.

	//This sample is a C# implementation of some of the functions in 
	//Andrei Belogortseff [ http://www.tweak-uac.com ]
	//though IsReallyVista is nothing to do with Andrei

	//The intention is to make it easy for .NET developers to discover whether or not 
	//UAC is enabled and/or the current process is elevated

	//Portions changed by David Hoyt
	internal class Native 
    {
		#region DLL Name Constants

		public const string 
			LIB_ADVAPI	= "advapi32", 
			LIB_KERNEL	= "kernel32",
			LIB_OLE		= "ole32"
		;

		#endregion

		#region Method Name Constants

		public const string
			IS_WOW_64_PROCESS = "IsWow64Process"
		;

		public const string
			VISTA_ONLY_API_METHOD = "CreateThreadpoolWait"
		;

		#endregion

		#region Structs/Enums

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEM_INFO 
        {
			public ushort wProcessorArchitecture;
			public ushort wReserved;
			public uint dwPageSize;
			public IntPtr lpMinimumApplicationAddress;
			public IntPtr lpMaximumApplicationAddress;
			public UIntPtr dwActiveProcessorMask;
			public uint dwNumberOfProcessors;
			public uint dwProcessorType;
			public uint dwAllocationGranularity;
			public ushort wProcessorLevel;
			public ushort wProcessorRevision;
		}

		public struct TOKEN_ELEVATION 
        {
			public UInt32 TokenIsElevated;
		}

		public enum TOKEN_INFORMATION_CLASS 
        {
			TokenUser = 1,
			TokenGroups = 2,
			TokenPrivileges = 3,
			TokenOwner = 4,
			TokenPrimaryGroup = 5,
			TokenDefaultDacl = 6,
			TokenSource = 7,
			TokenType = 8,
			TokenImpersonationLevel = 9,
			TokenStatistics = 10,
			TokenRestrictedSids = 11,
			TokenSessionId = 12,
			TokenGroupsAndPrivileges = 13,
			TokenSessionReference = 14,
			TokenSandBoxInert = 15,
			TokenAuditPolicy = 16,
			TokenOrigin = 17,
			TokenElevationType = 18,
			TokenLinkedToken = 19,
			TokenElevation = 20,
			TokenHasRestrictions = 21,
			TokenAccessInformation = 22,
			TokenVirtualizationAllowed = 23,
			TokenVirtualizationEnabled = 24,
			TokenIntegrityLevel = 25,
			TokenUIAccess = 26,
			TokenMandatoryPolicy = 27,
			TokenLogonSid = 28,
			MaxTokenInfoClass = 29  // MaxTokenInfoClass should always be the last enum
		}

		[Flags]
		internal enum CLSCTX 
        {
			CLSCTX_INPROC_SERVER = 0x1,
			CLSCTX_INPROC_HANDLER = 0x2,
			CLSCTX_LOCAL_SERVER = 0x4,
			CLSCTX_REMOTE_SERVER = 0x10,
			CLSCTX_NO_CODE_DOWNLOAD = 0x400,
			CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
			CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
			CLSCTX_NO_FAILURE_LOG = 0x4000,
			CLSCTX_DISABLE_AAA = 0x8000,
			CLSCTX_ENABLE_AAA = 0x10000,
			CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
			CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
			CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
			CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct BIND_OPTS3 
        {
			internal uint cbStruct;
			internal uint grfFlags;
			internal uint grfMode;
			internal uint dwTickCountDeadline;
			internal uint dwTrackFlags;
			internal uint dwClassContext;
			internal uint locale;
			object pServerInfo; // will be passing null, so type doesn't matter
			internal IntPtr hwnd;
		}

		#endregion

		#region Constants

		public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
		public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
		public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
		public const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

		public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
		public const uint TOKEN_DUPLICATE = 0x0002;
		public const uint TOKEN_IMPERSONATE = 0x0004;
		public const uint TOKEN_QUERY = 0x0008;
		public const uint TOKEN_QUERY_SOURCE = 0x0010;
		public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
		public const uint TOKEN_ADJUST_GROUPS = 0x0040;
		public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
		public const uint TOKEN_ADJUST_SESSIONID = 0x0100;

		public const uint TOKEN_ALL_ACCESS_P = (
			STANDARD_RIGHTS_REQUIRED |
			TOKEN_ASSIGN_PRIMARY |
			TOKEN_DUPLICATE |
			TOKEN_IMPERSONATE |
			TOKEN_QUERY |
			TOKEN_QUERY_SOURCE |
			TOKEN_ADJUST_PRIVILEGES |
			TOKEN_ADJUST_GROUPS |
			TOKEN_ADJUST_DEFAULT
		);

		#endregion

		#region P/Invoke Declarations

		[DllImport(LIB_ADVAPI, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

		[DllImport(LIB_KERNEL, SetLastError = true)]
		public static extern IntPtr GetCurrentProcess();

		[DllImport(LIB_ADVAPI, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

		[DllImport(LIB_KERNEL, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport(LIB_KERNEL, CharSet = CharSet.Ansi, ExactSpelling = false)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport(LIB_KERNEL, CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern IntPtr GetProcAddress(IntPtr hmodule, string procName);

		[DllImport(LIB_KERNEL, CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport(LIB_KERNEL, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);
		public delegate bool IsWow64ProcessDelegate(IntPtr hProcess, out bool bSystemInfo);

		[DllImport(LIB_OLE, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public static extern object CoGetObject(string pszName, [In] ref BIND_OPTS3 pBindOptions, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		[DllImport(LIB_KERNEL)]
		public static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo); 

		#endregion
	}
}
