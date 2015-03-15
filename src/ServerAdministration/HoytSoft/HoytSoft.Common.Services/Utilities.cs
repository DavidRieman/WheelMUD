#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2005, 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

using System.Linq;

namespace HoytSoft.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using HoytSoft.Common.UAC;

	public class Utilities 
    {
		#region Constants

		public const uint
			MAXIMUM_SERVICE_NAME_LENGTH = 256,
			MAXIMUM_SERVICE_DISPLAY_NAME_LENGTH = 256
		;

		public static readonly Type 
			SERVICE_BASE_TYPE = typeof(ServiceBase), 
			SERVICE_ATTRIBUTE_TYPE = typeof(ServiceAttribute)
		;

		#endregion

		#region Overloaded Methods

		public static bool IsServiceInstalled(string ServiceName) 
        {
			return IsServiceInstalled(null, ServiceName);
		}

		public static bool IsServiceInstalled(Type t) 
        {
			return IsServiceInstalled(null, FindServiceAttribute(t));
		}

		public static bool IsServiceInstalled(string MachineName, Type t) 
        {
			return IsServiceInstalled(MachineName, FindServiceAttribute(t));
		}

		public static bool IsServiceInstalled(ServiceAttribute Definition) 
        {
			return IsServiceInstalled(null, Definition);
		}

		public static bool IsServiceInstalled(string MachineName, ServiceAttribute Definition) 
        {
			return IsServiceInstalled(MachineName, Definition.Name);
		}

		public static bool IsServiceRunning(string ServiceName) 
        {
			return IsServiceRunning(null, ServiceName);
		}

		public static bool IsServiceRunning(ServiceAttribute Definition) 
        {
			return IsServiceRunning(null, Definition);
		}

		public static bool IsServiceRunning(Type t) 
        {
			return IsServiceRunning(null, FindServiceAttribute(t));
		}

		public static bool IsServiceRunning(string MachineName, Type t) 
        {
			return IsServiceRunning(MachineName, FindServiceAttribute(t));
		}

		public static bool IsServiceRunning(string MachineName, ServiceAttribute Definition) 
        {
			return IsServiceRunning(MachineName, Definition.Name);
		}

		public static TaskResult InstallService(string Path, ServiceAttribute Definition) 
        {
			return InstallService(null, Path, Definition);
		}

		public static TaskResult InstallService(string MachineName, string Path, ServiceAttribute Definition) 
        {
			if (Definition == null)
				throw new ArgumentNullException("Definition is null");
			return InstallService(MachineName, Definition.Name, Path, Definition.DisplayName, Definition.Description, Definition.ServiceType, Definition.ServiceAccessType, Definition.ServiceStartType, Definition.ServiceErrorControl);
		}

		public static TaskResult UninstallService(string ServiceName) 
        {
			return UninstallService(null, ServiceName);
		}

		public static TaskResult UninstallService(ServiceAttribute Definition) 
        {
			return UninstallService(null, Definition);
		}

		public static TaskResult UninstallService(string MachineName, ServiceAttribute Definition) 
        {
			return UninstallService(MachineName, Definition.Name);
		}

		public static TaskResult StartService(string ServiceName) 
        {
			return StartService(null, ServiceName);
		}

		public static TaskResult StartService(Type t) 
        {
			return StartService(null, FindServiceAttribute(t));
		}

		public static TaskResult StartService(string MachineName, Type t) 
        {
			return StartService(MachineName, FindServiceAttribute(t));
		}

		public static TaskResult StartService(ServiceAttribute Definition) 
        {
			return StartService(null, Definition);
		}

		public static TaskResult StartService(string MachineName, ServiceAttribute Definition) 
        {
			return StartService(MachineName, Definition.Name);
		}

		public static TaskResult ContinueService(string ServiceName) 
        {
			return ContinueService(null, ServiceName);
		}

		public static TaskResult ContinueService(Type t) 
        {
			return ContinueService(null, FindServiceAttribute(t));
		}

		public static TaskResult ContinueService(string MachineName, Type t) 
        {
			return ContinueService(MachineName, FindServiceAttribute(t));
		}

		public static TaskResult ContinueService(ServiceAttribute Definition) 
        {
			return ContinueService(null, Definition);
		}

		public static TaskResult ContinueService(string MachineName, ServiceAttribute Definition) 
        {
			return ContinueService(MachineName, Definition.Name);
		}

		public static TaskResult PauseService(string ServiceName) 
        {
			return PauseService(null, ServiceName);
		}

		public static TaskResult PauseService(Type t) 
        {
			return PauseService(null, FindServiceAttribute(t));
		}

		public static TaskResult PauseService(string MachineName, Type t) 
        {
			return PauseService(MachineName, FindServiceAttribute(t));
		}

		public static TaskResult PauseService(ServiceAttribute Definition) 
        {
			return PauseService(null, Definition);
		}

		public static TaskResult PauseService(string MachineName, ServiceAttribute Definition) 
        {
			return PauseService(MachineName, Definition.Name);
		}

		public static TaskResult StopService(string ServiceName) 
        {
			return StopService(null, ServiceName);
		}

		public static TaskResult StopService(Type t) 
        {
			return StopService(null, FindServiceAttribute(t));
		}

		public static TaskResult StopService(string MachineName, Type t) 
        {
			return StopService(MachineName, FindServiceAttribute(t));
		}

		public static TaskResult StopService(ServiceAttribute Definition) 
        {
			return StopService(null, Definition);
		}

		public static TaskResult StopService(string MachineName, ServiceAttribute Definition) 
        {
			return StopService(MachineName, Definition.Name);
		}

		#endregion

		#region Public Methods

		///<summary>Has this object defined everything it needs to for us to run it?</summary>
		public static bool IsValidService(ServiceBase obj) 
        {
			if (obj == null)
				return false;

			return IsValidServiceType(obj.GetType());
		}

		///<summary>Has this type defined everything it needs to for us to run it?</summary>
		public static bool IsValidServiceType(Type t) 
        {
			//Is it a non-abstract class that derives from ServiceBase and has a ServiceAttribute defined?
			return !(t == null || t.IsAbstract || !t.IsClass || !SERVICE_BASE_TYPE.IsAssignableFrom(t) || !t.IsDefined(SERVICE_ATTRIBUTE_TYPE, true));
		}

		///<summary>Locates and returns all ServiceBase classes for an assembly.</summary>
		public static Type[] FindAllValidServices(Assembly Assembly) 
        {
			var lst = new List<Type>(1);
			Type[] types = Assembly.GetTypes();

		    lst.AddRange(types.Where(IsValidServiceType));

		    return lst.ToArray();
		}

		public static ServiceAttribute FindServiceAttribute(Type t) 
        {
			if (!IsValidServiceType(t))
				throw new ArgumentException("Invalid service type");

			var attribs = (ServiceAttribute[])t.GetCustomAttributes(SERVICE_ATTRIBUTE_TYPE, true);
			if (attribs == null || attribs.Length <= 0)
				return null;

			return attribs[0];
		}

		public static bool IsServiceRunning(string MachineName, string ServiceName) 
        {
			DirectTask<bool> dt = IsServiceRunningDirect(MachineName, ServiceName);

			switch(dt.OperationResult) {
				case DirectTaskResult.Success:
					return dt.Result;
				case DirectTaskResult.NeedsElevation:
					return (bool)UAC.RunTask(new IsServiceRunningTask(MachineName, ServiceName)).Param;
				default:
					throw new ServiceException("Unable to determine service state");
			}
		}

		public static bool IsServiceInstalled(string MachineName, string ServiceName) 
        {
			DirectTask<bool> dt = IsServiceInstalledDirect(MachineName, ServiceName);

			switch (dt.OperationResult)
            {
				case DirectTaskResult.Success:
					return dt.Result;
				case DirectTaskResult.NeedsElevation:
					return (bool)UAC.RunTask(new IsServiceInstalledTask(MachineName, ServiceName)).Param;
				default:
					throw new ServiceException("Unable to determine service state");
			}
		}

		public static TaskResult InstallService(string MachineName, string ServiceName, string Path, string DisplayName, string Description, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl) 
        {
			return UAC.RunTask(new InstallServiceTask(MachineName, ServiceName, Path, DisplayName, Description, ServiceType, ServiceAccessType, ServiceStartType, ServiceErrorControl));
		}

		public static TaskResult UninstallService(string MachineName, string ServiceName) 
        {
			return UAC.RunTask(new UninstallServiceTask(MachineName, ServiceName));
		}

		public static TaskResult StartService(string MachineName, string ServiceName) 
        {
			DirectTaskResult r = StartServiceDirect(MachineName, ServiceName);
			switch (r) {
				case DirectTaskResult.Success:
					return new TaskResult(Result.Success);
				case DirectTaskResult.Failure:
					return new TaskResult(Result.Failed);
				case DirectTaskResult.NeedsElevation:
					return UAC.RunTask(new StartServiceTask(MachineName, ServiceName));
				default:
					return new TaskResult(Result.Unknown);
			}
		}

		public static TaskResult ContinueService(string MachineName, string ServiceName) 
        {
			DirectTaskResult r = ContinueServiceDirect(MachineName, ServiceName);

			switch (r) {
				case DirectTaskResult.Success:
					return new TaskResult(Result.Success);
				case DirectTaskResult.Failure:
					return new TaskResult(Result.Failed);
				case DirectTaskResult.NeedsElevation:
					return UAC.RunTask(new ContinueServiceTask(MachineName, ServiceName));
				default:
					return new TaskResult(Result.Unknown);
			}
		}

		public static TaskResult PauseService(string MachineName, string ServiceName) 
        {
			DirectTaskResult r = PauseServiceDirect(MachineName, ServiceName);

			switch (r) {
				case DirectTaskResult.Success:
					return new TaskResult(Result.Success);
				case DirectTaskResult.Failure:
					return new TaskResult(Result.Failed);
				case DirectTaskResult.NeedsElevation:
					return UAC.RunTask(new PauseServiceTask(MachineName, ServiceName));
				default:
					return new TaskResult(Result.Unknown);
			}
		}

		public static TaskResult StopService(string MachineName, string ServiceName) 
        {
			DirectTaskResult r = StopServiceDirect(MachineName, ServiceName);
			switch (r) {
				case DirectTaskResult.Success:
					return new TaskResult(Result.Success);
				case DirectTaskResult.Failure:
					return new TaskResult(Result.Failed);
				case DirectTaskResult.NeedsElevation:
					return UAC.RunTask(new StopServiceTask(MachineName, ServiceName));
				default:
					return new TaskResult(Result.Unknown);
			}
		}

		#endregion

		#region Direct Methods

		public static DirectTask<bool> IsServiceRunningDirect(string MachineName, string ServiceName) 
        {
			IntPtr scm = Native.OpenSCManager(MachineName, null, Native.ServiceControlManagerType.SC_MANAGER_CONNECT);

			if (scm == IntPtr.Zero) 
            {
				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return new DirectTask<bool>(DirectTaskResult.NeedsElevation, false);
				return new DirectTask<bool>(DirectTaskResult.Failure, false);
			}

			IntPtr hSvc = Native.OpenService(scm, ServiceName, Native.ACCESS_TYPE.SERVICE_QUERY_STATUS);

			if (hSvc == IntPtr.Zero) 
            {
				//Close SCM handle
				Native.CloseServiceHandle(scm);

				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return new DirectTask<bool>(DirectTaskResult.NeedsElevation, false);

				return new DirectTask<bool>(DirectTaskResult.Failure, false);
			}

			bool ret = false;
			try 
            {
				var status = new Native.SERVICE_STATUS();
				Native.QueryServiceStatus(hSvc, ref status);
				ret = (status.dwCurrentState != Native.SERVICE_STOPPED && status.dwCurrentState != Native.SERVICE_STOP_PENDING);
			} 
            catch 
            {}

			//Close service handle
			Native.CloseServiceHandle(hSvc);

			//Close SCM handle
			Native.CloseServiceHandle(scm);

			return new DirectTask<bool>(DirectTaskResult.Success, ret);
		}

		public static DirectTask<bool> IsServiceInstalledDirect(string MachineName, string ServiceName) 
        {
			if (string.IsNullOrEmpty(ServiceName))
				throw new ArgumentOutOfRangeException("ServiceName cannot be empty or null");

			if (ServiceName.Length > MAXIMUM_SERVICE_NAME_LENGTH)
				throw new ArgumentOutOfRangeException(string.Format("The maximum length for a service name is {0} characters", MAXIMUM_SERVICE_NAME_LENGTH));

			bool ret;
			IntPtr sc_handle = IntPtr.Zero;
			IntPtr sv_handle = IntPtr.Zero;

			try 
            {
				sc_handle = Native.OpenSCManager(MachineName, null, Native.ServiceControlManagerType.SC_MANAGER_CONNECT);
				if (sc_handle == IntPtr.Zero) 
                {
					if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
						return new DirectTask<bool>(DirectTaskResult.NeedsElevation, false);
					return new DirectTask<bool>(DirectTaskResult.Failure, false);
				}

				sv_handle = Native.OpenService(sc_handle, ServiceName, Native.GENERIC_READ);
				ret = (sv_handle != IntPtr.Zero);
			} 
            catch 
            {
				ret = false;
			} 
            finally 
            {
				if (sv_handle != IntPtr.Zero)
					Native.CloseServiceHandle(sv_handle);
				if (sc_handle != IntPtr.Zero)
					Native.CloseServiceHandle(sc_handle);
			}

			return new DirectTask<bool>(DirectTaskResult.Success, ret);
		}

		public static DirectTaskResult StartServiceDirect(string MachineName, string ServiceName) 
        {
			IntPtr scm = Native.OpenSCManager(MachineName, null, Native.ServiceControlManagerType.SC_MANAGER_CONNECT);

			if (scm == IntPtr.Zero) 
            {
				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return DirectTaskResult.NeedsElevation;

				return DirectTaskResult.Failure;
			}

			IntPtr hSvc = Native.OpenService(scm, ServiceName, Native.ACCESS_TYPE.SERVICE_START);

			if (hSvc == IntPtr.Zero) 
            {
				//Close SCM handle
				Native.CloseServiceHandle(scm);
				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return DirectTaskResult.NeedsElevation;

				return DirectTaskResult.Failure;
			}

			bool ret = false;

			try 
            {
				ret = Native.StartService(hSvc, 0, null);
			} 
            catch 
            {}

			//Close service handle
			Native.CloseServiceHandle(hSvc);

			//Close SCM handle
			Native.CloseServiceHandle(scm);

			return DirectTaskResult.Success;
		}

		public static DirectTaskResult ContinueServiceDirect(string MachineName, string ServiceName) 
        {
			return ControlServiceDirect(MachineName, ServiceName, Native.ACCESS_TYPE.SERVICE_PAUSE_CONTINUE, Native.ServiceControlType.SERVICE_CONTROL_CONTINUE);
		}

		public static DirectTaskResult PauseServiceDirect(string MachineName, string ServiceName) 
        {
			return ControlServiceDirect(MachineName, ServiceName, Native.ACCESS_TYPE.SERVICE_PAUSE_CONTINUE, Native.ServiceControlType.SERVICE_CONTROL_PAUSE);
		}

		public static DirectTaskResult StopServiceDirect(string MachineName, string ServiceName) 
        {
			return ControlServiceDirect(MachineName, ServiceName, Native.ACCESS_TYPE.SERVICE_STOP, Native.ServiceControlType.SERVICE_CONTROL_STOP);
		}

		internal static DirectTaskResult ControlServiceDirect(string MachineName, string ServiceName, Native.ACCESS_TYPE AccessType, Native.ServiceControlType ControlType) 
        {
			IntPtr scm = Native.OpenSCManager(MachineName, null, Native.ServiceControlManagerType.SC_MANAGER_CONNECT);

			if (scm == IntPtr.Zero) 
            {
				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return DirectTaskResult.NeedsElevation;

				return DirectTaskResult.Failure;
			}

			IntPtr hSvc = Native.OpenService(scm, ServiceName, AccessType);

			if (hSvc == IntPtr.Zero) 
            {
				//Close SCM handle
				Native.CloseServiceHandle(scm);
				if (Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED)
					return DirectTaskResult.NeedsElevation;

				return DirectTaskResult.Failure;
			}

			bool ret = false;
			try {
				var status = new Native.SERVICE_STATUS();
				ret = Native.ControlService(hSvc, ControlType, ref status);
			} 
            catch 
            {}

			//Close service handle
			Native.CloseServiceHandle(hSvc);

			//Close SCM handle
			Native.CloseServiceHandle(scm);

			return DirectTaskResult.Success;
		}

		#endregion
	}
}
