#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2005, 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.Services
{
    using System;
    using System.Runtime.InteropServices;

    internal class Native 
    {
        #region DLL Name Constants

        public const string
            LIB_ADVAPI = "advapi32",
            LIB_KERNEL = "kernel32"
        ;

        #endregion

        #region Constants

        public const int ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061;
        public const uint NO_ERROR = 0;

        public const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        public const uint GENERIC_READ = 0x80000000; //-2147483648;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint SERVICE_NO_CHANGE = 0xffffffff; //-1
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint DELETE = 0x10000;
        public const int SERVICE_ACCEPT_STOP = 1;
        public const int SERVICE_ACCEPT_PAUSE_CONTINUE = 2;
        public const int SERVICE_ACCEPT_SHUTDOWN = 4;

        public const uint SERVICE_STOPPED = 1;
        public const uint SERVICE_START_PENDING = 2;
        public const uint SERVICE_STOP_PENDING = 3;
        public const uint SERVICE_RUNNING = 4;
        public const uint SERVICE_CONTINUE_PENDING = 5;
        public const uint SERVICE_PAUSE_PENDING = 6;
        public const uint SERVICE_PAUSED = 7;
        public const uint SERVICE_INTERROGATE = 0x80;

        public const int SERVICE_CONTROL_STOP = 1;
        public const int SERVICE_CONTROL_PAUSE = 2;
        public const int SERVICE_CONTROL_CONTINUE = 3;
        public const int SERVICE_CONTROL_INTERROGATE = 4;
        public const int SERVICE_CONTROL_SHUTDOWN = 5;

        public const uint SERVICE_WIN32 = (uint)(ServiceType.SERVICE_WIN32_OWN_PROCESS | ServiceType.SERVICE_WIN32_SHARE_PROCESS);

        public const int ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063;
        public const int ERROR_INVALID_DATA = 13;
        public const int ERROR_SERVICE_ALREADY_RUNNING = 1057;
        public const int ERROR_ACCESS_DENIED = 5;
        //public const int SERVICE_NO_CHANGE = 0xFFFF;

        #endregion

        #region Delegates

        public delegate void ServiceCtrlHandlerProc(uint Opcode);
        public delegate uint ServiceCtrlHandlerExProc(uint Opcode, uint EventType, IntPtr EventData, IntPtr Context);
        public delegate void ServiceMainProc(uint argc, [MarshalAs(UnmanagedType.LPArray)]string[] argv);

        #endregion

        #region Enums

        public enum ServiceType : uint 
        {
            SERVICE_KERNEL_DRIVER = 0x1,
            SERVICE_FILE_SYSTEM_DRIVER = 0x2,
            SERVICE_WIN32_OWN_PROCESS = 0x10,
            SERVICE_WIN32_SHARE_PROCESS = 0x20,
            SERVICE_INTERACTIVE_PROCESS = 0x100,
            SERVICETYPE_NO_CHANGE = SERVICE_NO_CHANGE
        }

        public enum ServiceStartType : uint 
        {
            SERVICE_BOOT_START = 0x0,
            SERVICE_SYSTEM_START = 0x1,
            SERVICE_AUTO_START = 0x2,
            SERVICE_DEMAND_START = 0x3,
            SERVICE_DISABLED = 0x4,
            SERVICESTARTTYPE_NO_CHANGE = SERVICE_NO_CHANGE
        }

        public enum ServiceErrorControl : uint 
        {
            SERVICE_ERROR_IGNORE = 0x0,
            SERVICE_ERROR_NORMAL = 0x1,
            SERVICE_ERROR_SEVERE = 0x2,
            SERVICE_ERROR_CRITICAL = 0x3,
            msidbServiceInstallErrorControlVital = 0x8000,
            SERVICEERRORCONTROL_NO_CHANGE = SERVICE_NO_CHANGE
        }

        public enum ServiceStateRequest
        {
            SERVICE_ACTIVE = 0x1,
            SERVICE_INACTIVE = 0x2,
            SERVICE_STATE_ALL = (SERVICE_ACTIVE | SERVICE_INACTIVE)
        }

        public enum ServiceControlType : uint 
        {
            SERVICE_CONTROL_STOP = 0x1,
            SERVICE_CONTROL_PAUSE = 0x2,
            SERVICE_CONTROL_CONTINUE = 0x3,
            SERVICE_CONTROL_INTERROGATE = 0x4,
            SERVICE_CONTROL_SHUTDOWN = 0x5,
            SERVICE_CONTROL_PARAMCHANGE = 0x6,
            SERVICE_CONTROL_NETBINDADD = 0x7,
            SERVICE_CONTROL_NETBINDREMOVE = 0x8,
            SERVICE_CONTROL_NETBINDENABLE = 0x9,
            SERVICE_CONTROL_NETBINDDISABLE = 0xA,
            SERVICE_CONTROL_DEVICEEVENT = 0xB,
            SERVICE_CONTROL_HARDWAREPROFILECHANGE = 0xC,
            SERVICE_CONTROL_POWEREVENT = 0xD,
            SERVICE_CONTROL_SESSIONCHANGE = 0xE,
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x1,
            SERVICE_START_PENDING = 0x2,
            SERVICE_STOP_PENDING = 0x3,
            SERVICE_RUNNING = 0x4,
            SERVICE_CONTINUE_PENDING = 0x5,
            SERVICE_PAUSE_PENDING = 0x6,
            SERVICE_PAUSED = 0x7,
        }

        public enum ServiceControlAccepted
        {
            SERVICE_ACCEPT_STOP = 0x1,
            SERVICE_ACCEPT_PAUSE_CONTINUE = 0x2,
            SERVICE_ACCEPT_SHUTDOWN = 0x4,
            SERVICE_ACCEPT_PARAMCHANGE = 0x8,
            SERVICE_ACCEPT_NETBINDCHANGE = 0x10,
            SERVICE_ACCEPT_HARDWAREPROFILECHANGE = 0x20,
            SERVICE_ACCEPT_POWEREVENT = 0x40,
            SERVICE_ACCEPT_SESSIONCHANGE = 0x80
        }

        public enum ServiceControlManagerType : uint 
        {
            SC_MANAGER_CONNECT = 0x1,
            SC_MANAGER_CREATE_SERVICE = 0x2,
            SC_MANAGER_ENUMERATE_SERVICE = 0x4,
            SC_MANAGER_LOCK = 0x8,
            SC_MANAGER_QUERY_LOCK_STATUS = 0x10,
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x20,
            SC_MANAGER_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS | SC_MANAGER_MODIFY_BOOT_CONFIG
        }

        public enum ACCESS_TYPE
        {
            SERVICE_QUERY_CONFIG = 0x1,
            SERVICE_CHANGE_CONFIG = 0x2,
            SERVICE_QUERY_STATUS = 0x4,
            SERVICE_ENUMERATE_DEPENDENTS = 0x8,
            SERVICE_START = 0x10,
            SERVICE_STOP = 0x20,
            SERVICE_PAUSE_CONTINUE = 0x40,
            SERVICE_INTERROGATE = 0x80,
            SERVICE_USER_DEFINED_CONTROL = 0x100,
            SERVICE_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL
        }

        public enum SC_ACTION_TYPE
        {
            SC_ACTION_NONE = 0,
            SC_ACTION_RESTART = 1,
            SC_ACTION_REBOOT = 2,
            SC_ACTION_RUN_COMMAND = 3,
        }

        public enum InfoLevel
        {
            SERVICE_CONFIG_DESCRIPTION = 1,
            SERVICE_CONFIG_FAILURE_ACTIONS = 2
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS 
        {
            public uint dwServiceType;
            public uint dwCurrentState;
            public uint dwControlsAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QUERY_SERVICE_CONFIG 
        {
            public int dwServiceType;
            public int dwStartType;
            public int dwErrorControl;
            public string lpBinaryPathName;
            public string lpLoadOrderGroup;
            public int dwTagId;
            public string lpDependencies;
            public string lpServiceStartName;
            public string lpDisplayName;
        }

        public struct SERVICE_TABLE_ENTRY 
        {
            public string lpServiceName;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ServiceMainProc lpServiceProc;

            /*public SERVICE_TABLE_ENTRY(string Name, ServiceMainProc ServiceMainMethod) {
                this.lpServiceName = Name;
                this.lpServiceProc = ServiceMainMethod;
                this.lpServiceNameNull = null;
                this.lpServiceProcNull = null;
            }/**/
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SC_ACTION 
        {
            public SC_ACTION_TYPE SCActionType;
            public int Delay;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_DESCRIPTION 
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_FAILURE_ACTIONS 
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwResetPeriod;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpCommand;
            [MarshalAs(UnmanagedType.U4)]
            public int cActions;
            public int lpsaActions;
        }

        #endregion

        #region PInvoke Declarations

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterServiceCtrlHandler(string lpServiceName, [MarshalAs(UnmanagedType.FunctionPtr)]ServiceCtrlHandlerProc lpHandlerProc);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterServiceCtrlHandlerEx(string lpServiceName, [MarshalAs(UnmanagedType.FunctionPtr)]ServiceCtrlHandlerExProc lpHandlerExProc);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetServiceStatus(IntPtr hServiceStatus, ref SERVICE_STATUS lpServiceStatus);

        [DllImport(LIB_ADVAPI, EntryPoint = "StartServiceCtrlDispatcher", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartServiceCtrlDispatcher(SERVICE_TABLE_ENTRY[] lpServiceStartTable);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName, uint dwDesiredAccess, uint dwServiceType, uint dwStartType, uint dwErrorControl, string lpPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName, Services.ServiceAccessType dwDesiredAccess, Services.ServiceType dwServiceType, Services.ServiceStartType dwStartType, Services.ServiceErrorControl dwErrorControl, string lpPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        public static extern IntPtr LockServiceDatabase(IntPtr hSCManager);

        [DllImport(LIB_ADVAPI)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnlockServiceDatabase(IntPtr hSCManager);

        [DllImport(LIB_KERNEL)]
        public static extern void CopyMemory(IntPtr pDst, SC_ACTION[] pSrc, int ByteLen);

        [DllImport(LIB_ADVAPI, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ChangeServiceConfig(IntPtr hService, ServiceType nServiceType, uint nStartType, uint nErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, IntPtr lpInfo);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, InfoLevel dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, InfoLevel dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);

        [DllImport(LIB_ADVAPI, EntryPoint = "OpenService", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string serviceName, ACCESS_TYPE desiredAccess);

        [DllImport(LIB_ADVAPI, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport(LIB_ADVAPI, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, ServiceControlManagerType dwAccess);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport(LIB_ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, uint cbBufSize, out uint pcbBytesNeeded);

        [DllImport(LIB_ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig(IntPtr hService, [MarshalAs(UnmanagedType.Struct)] ref QUERY_SERVICE_CONFIG lpServiceConfig, uint cbBufSize, out uint pcbBytesNeeded);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(IntPtr hService, ServiceControlType dwControl, ref SERVICE_STATUS lpServiceStatus);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

        [DllImport(LIB_ADVAPI, EntryPoint = "QueryServiceStatus", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS dwServiceStatus);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport(LIB_ADVAPI, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, Services.ServiceAccessType dwDesiredAccess);

        [DllImport(LIB_ADVAPI, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool DeleteService(IntPtr hService);

        #endregion
    }
}
