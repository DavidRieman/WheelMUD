/*********************************************************
 * Copyright © 2005, 2008 HoytSoft. All rights reserved. 
 * Please see included license for more details.
 *********************************************************/

namespace HoytSoft.Common.Services
{
    using System;

    public enum DirectTaskResult : byte
    {
        Success,
        Failure,
        NeedsElevation
    }

    public enum ServiceState : byte
    {
        Running,
        Stopped,
        Paused,
        ShuttingDown,
        Interrogating
    }

    [Flags]
    public enum ServiceControls
    {
        /// <summary>The service will respond to start and stop commands.</summary>
        StartAndStop = 1,

        /// <summary>The service will respond to pause and continue commands.</summary>
        PauseAndContinue = 2,

        /// <summary>The service will respond to shutdown commands.</summary>
        Shutdown = 4,

        /// <summary>The service will respond to start, stop, pause, and continue commands.</summary>
        Default = StartAndStop | PauseAndContinue
    }

    public enum ServiceType : uint
    {
        /// <summary>Driver service.</summary>
        KernelDriver = Native.ServiceType.SERVICE_KERNEL_DRIVER,

        /// <summary>File system driver service. </summary>
        FileSystemDriver = Native.ServiceType.SERVICE_FILE_SYSTEM_DRIVER,

        /// <summary>Service that runs in its own process.</summary>
        OwnProcess = Native.ServiceType.SERVICE_WIN32_OWN_PROCESS,

        /// <summary>Service that shares a process with one or more other services.</summary>
        ShareProcess = Native.ServiceType.SERVICE_WIN32_SHARE_PROCESS,

        /// <summary>The default value.</summary>
        Default = OwnProcess, /* A combo of Win32OwnProcess and Win32ShareProcess */

        /// <summary>The service can interact with the desktop. If the service is running in the context of the LocalSystem account, then you can use this.</summary>
        InteractiveProcessOwn = Native.ServiceType.SERVICE_INTERACTIVE_PROCESS | OwnProcess,

        /// <summary>The service can interact with the desktop. If the service is running in the context of the LocalSystem account, then you can use this.</summary>
        InteractiveProcessShare = Native.ServiceType.SERVICE_INTERACTIVE_PROCESS | ShareProcess,

        /// <summary>Do nothing.</summary>
        NoChange = Native.ServiceType.SERVICETYPE_NO_CHANGE
    }

    public enum ServiceAccessType : uint
    {
        /// <summary>Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.</summary>
        QueryConfig = Native.ACCESS_TYPE.SERVICE_QUERY_CONFIG,

        /// <summary>Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Because this grants the caller the right to change the executable file that the system runs, it should be granted only to administrators.</summary>
        ChangeConfig = Native.ACCESS_TYPE.SERVICE_CHANGE_CONFIG,

        /// <summary>Required to call the QueryServiceStatusEx function to ask the service control manager about the status of the service.</summary>
        QueryStatus = Native.ACCESS_TYPE.SERVICE_QUERY_STATUS,

        /// <summary>Required to call the EnumDependentServices function to enumerate all the services dependent on the service.</summary>
        EnumerateDependents = Native.ACCESS_TYPE.SERVICE_ENUMERATE_DEPENDENTS,

        /// <summary>Required to call the StartService function to start the service.</summary>
        Start = Native.ACCESS_TYPE.SERVICE_START,

        /// <summary>Required to call the ControlService function to stop the service.</summary>
        Stop = Native.ACCESS_TYPE.SERVICE_STOP,

        /// <summary>Required to call the ControlService function to pause or continue the service.</summary>
        PauseContinue = Native.ACCESS_TYPE.SERVICE_PAUSE_CONTINUE,

        /// <summary>Required to call the ControlService function to ask the service to report its status immediately.</summary>
        Interrogate = Native.ACCESS_TYPE.SERVICE_INTERROGATE,

        /// <summary>Required to call the ControlService function to specify a user-defined control code.</summary>
        UserDefinedControl = Native.ACCESS_TYPE.SERVICE_USER_DEFINED_CONTROL,

        /// <summary>The default value. Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.</summary>
        AllAccess = Native.ACCESS_TYPE.SERVICE_ALL_ACCESS
    }

    public enum ServiceStartType : uint
    {
        /// <summary>A device driver started by the system loader. This value is valid only for driver services.</summary>
        BootStart = Native.ServiceStartType.SERVICE_BOOT_START,

        /// <summary>A device driver started by the IoInitSystem function. This value is valid only for driver services.</summary>
        SystemStart = Native.ServiceStartType.SERVICE_SYSTEM_START,

        /// <summary>The default value. A service started automatically by the service control manager during system startup.</summary>
        AutoStart = Native.ServiceStartType.SERVICE_AUTO_START,

        /// <summary>A service started by the service control manager when a process calls the StartService function.</summary>
        DemandStart = Native.ServiceStartType.SERVICE_DEMAND_START,

        /// <summary>A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.</summary>
        Disabled = Native.ServiceStartType.SERVICE_DISABLED,

        /// <summary>Do nothing.</summary>
        NoChange = Native.ServiceStartType.SERVICESTARTTYPE_NO_CHANGE
    }

    public enum ServiceErrorControl : uint
    {
        /// <summary>The startup program logs the error but continues the startup operation.</summary>
        Ignore = Native.ServiceErrorControl.SERVICE_ERROR_IGNORE,

        /// <summary>The default value. The startup program logs the error and puts up a message box pop-up but continues the startup operation.</summary>
        Normal = Native.ServiceErrorControl.SERVICE_ERROR_NORMAL,

        /// <summary>The startup program logs the error. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.</summary>
        Severe = Native.ServiceErrorControl.SERVICE_ERROR_SEVERE,

        /// <summary>The startup program logs the error, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.</summary>
        Critical = Native.ServiceErrorControl.SERVICE_ERROR_CRITICAL,

        /// <summary>(No description available)</summary>
        MSIVital = Native.ServiceErrorControl.msidbServiceInstallErrorControlVital,

        /// <summary>Do nothing.</summary>
        NoChange = Native.ServiceErrorControl.SERVICEERRORCONTROL_NO_CHANGE
    }
}