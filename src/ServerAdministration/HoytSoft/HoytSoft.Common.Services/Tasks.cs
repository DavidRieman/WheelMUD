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
    using HoytSoft.Common.UAC;

	public sealed class DirectTask<T> 
    {
		#region Variables

		private readonly DirectTaskResult operationResult;
		private readonly string message;
		private readonly T result;

		#endregion

		#region Constructors

		public DirectTask(DirectTaskResult OperationResult, T Result) : this(OperationResult, string.Empty, Result) 
        {}

		public DirectTask(DirectTaskResult OperationResult, string Message, T Result) 
        {
			this.operationResult = OperationResult;
			this.message = Message;
			this.result = Result;
		}

		#endregion

		#region Properties

		public DirectTaskResult OperationResult 
        {
			get { return this.operationResult; }
		}

		public string Message 
        {
			get { return this.message; }
		}

		public T Result 
        {
			get { return this.result; }
		}

		#endregion
	}

	[Serializable]
	public abstract class ServiceTask : AbstractAdministrativeTask 
    {
		#region Variables

		protected string serviceName;
		protected string machineName;

		#endregion

		#region Constructors

		protected ServiceTask(string MachineName, string ServiceName) 
        {
			this.serviceName = ServiceName;
			this.machineName = MachineName;
		}

		#endregion

		#region Properties

		public string ServiceName 
        {
			get { return this.serviceName; }
		}

		public string MachineName 
        {
			get { return this.machineName; }
		}

		#endregion

		#region AbstractAdministrativeTask Methods

		public override TaskResult RunTask() 
        {
			if (string.IsNullOrEmpty(serviceName))
				return result(Result.Failed, "Missing service name");

			if (serviceName.Length > Utilities.MAXIMUM_SERVICE_NAME_LENGTH)
				return result(Result.Failed, string.Format(@"The maximum length for a service name is {0} characters.", Utilities.MAXIMUM_SERVICE_NAME_LENGTH));

			return runServiceTask();
		}

		#endregion

		#region Protected Methods

		protected TaskResult result(Result Result) 
        {
			return new TaskResult(Result);
		}

		protected TaskResult result(Result Result, string Message) 
        {
			return new TaskResult(Result, Message);
		}

		protected TaskResult result(Result Result, object Param) 
        {
			return new TaskResult(Result, string.Empty, Param);
		}

		protected TaskResult result(Result Result, string Message, object Param) 
        {
			return new TaskResult(Result, Message, Param);
		}

		protected TaskResult processDirectTaskResult(DirectTaskResult DirectResult) 
        {
			switch (DirectResult) 
            {
				case DirectTaskResult.Success:
					return result(Result.Success);
				case DirectTaskResult.Failure:
					return result(Result.Failed, "Unable to process requested task");
				case DirectTaskResult.NeedsElevation:
					return result(Result.Failed, "No administrative rights to process requested task");
				default:
					return result(Result.Unknown);
			}
		}

		protected TaskResult processDirectTask(DirectTask<bool> dt)
        {
			switch (dt.OperationResult) 
            {
				case DirectTaskResult.Success:
					return result(Result.Success, dt.Result);
				case DirectTaskResult.Failure:
					return result(Result.Failed, "Unable to process requested task");
				case DirectTaskResult.NeedsElevation:
					return result(Result.Failed, "No administrative rights to process requested task");
				default:
					return result(Result.Unknown);
			}
		}

		protected abstract TaskResult runServiceTask();

		#endregion
	}

	[Serializable]
	public class InstallServiceTask : ServiceTask
    {
		#region Variables

		private string path;
		private readonly string displayName;
		private readonly string description;
		private readonly ServiceType serviceType;
		private readonly ServiceAccessType serviceAccessType;
		private readonly ServiceStartType serviceStartType;
		private readonly ServiceErrorControl serviceErrorControl;

		#endregion

		#region Constructors
		public InstallServiceTask(string MachineName, string ServiceName, string Path, string DisplayName, string Description, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl) : base(MachineName, ServiceName) {
			this.path = Path;
			this.displayName = DisplayName;
			this.description = Description;
			this.serviceType = ServiceType;
			this.serviceAccessType = ServiceAccessType;
			this.serviceStartType = ServiceStartType;
			this.serviceErrorControl = ServiceErrorControl;
		}
		#endregion

		protected override TaskResult runServiceTask() 
        {
			#region Validate parameters

			if (string.IsNullOrEmpty(serviceName))
				return new TaskResult(Result.Failed, "Missing service name");

			if (string.IsNullOrEmpty(machineName))
				machineName = null;

			if (serviceName.Length > Utilities.MAXIMUM_SERVICE_NAME_LENGTH) 
				return result(Result.Failed, string.Format(@"The maximum length for a service name is {0} characters.", Utilities.MAXIMUM_SERVICE_NAME_LENGTH));
			if (serviceName.IndexOf(@"\") >= 0 || serviceName.IndexOf(@"/") >= 0)
				return result(Result.Failed, @"Service names cannot contain \ or / characters.");
			if (displayName.Length > Utilities.MAXIMUM_SERVICE_DISPLAY_NAME_LENGTH)
				return result(Result.Failed, string.Format("The maximum length for a display name is {0} characters.", Utilities.MAXIMUM_SERVICE_DISPLAY_NAME_LENGTH));

			//The spec says that if a service's path has a space in it, then we must quote it...
			if (path.IndexOf(" ") >= 0)
				path = "\"" + path + "\"";
			path = path.Replace(@"\", @"\\");

			#endregion

			//Check if it's installed already. If so, then uninstall it first.
			TaskResult tr;

			if (Utilities.IsServiceInstalled(machineName, serviceName) && (tr = Utilities.UninstallService(machineName, serviceName)) != null && tr.Result != Result.Success)
				return result(Result.Failed, "Unable to remove previous service definition to install the new one");

			try 
            {
				IntPtr sc_handle = Native.OpenSCManager(machineName, null, Native.ServiceControlManagerType.SC_MANAGER_CREATE_SERVICE);
				if (sc_handle == IntPtr.Zero || Marshal.GetLastWin32Error() == Native.ERROR_ACCESS_DENIED) 
					return result(Result.Failed, "No administrative rights to install services");

				IntPtr sv_handle = Native.CreateService(sc_handle, serviceName, displayName, serviceAccessType, serviceType, serviceStartType, serviceErrorControl, path, null, IntPtr.Zero, null, null, null);
				if (sv_handle == IntPtr.Zero) 
                {
					Native.CloseServiceHandle(sc_handle);
					return result(Result.Failed, "No administrative rights to install services");
				}


				//Sets a service's description by adding a registry entry for it.
				if (!string.IsNullOrEmpty(description)) 
                {
					//Set the service's description
					try 
                    {
						var info = new Native.SERVICE_DESCRIPTION {lpDescription = description};
						//File.AppendAllText("C:\tasks.txt", description);
					    if (description == null)
							info.lpDescription = string.Empty;
						
						Native.ChangeServiceConfig2(sv_handle, Native.InfoLevel.SERVICE_CONFIG_DESCRIPTION, ref info);
					} 
                    catch 
                    {
						//If that didn't work, try the registry approach...
						try 
                        {
							using (Microsoft.Win32.RegistryKey serviceKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + serviceName, true)) 
                            {
								serviceKey.SetValue("Description", description);
							}
						} 
                        catch(Exception e) 
                        {
							return result(Result.Failed, e.Message);
						}
					}
				}

				Native.CloseServiceHandle(sv_handle);
				Native.CloseServiceHandle(sc_handle);

				return result(Result.Success);
			} 
            catch(Exception e) 
            {
				return result(Result.Failed, e.Message);
			}
		}
	}

	[Serializable]
	public class UninstallServiceTask : ServiceTask 
    {
		#region Constructors

		public UninstallServiceTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			if (string.IsNullOrEmpty(serviceName))
				return result(Result.Failed, "Missing service name");

			if (serviceName.Length > Utilities.MAXIMUM_SERVICE_NAME_LENGTH)
				return result(Result.Failed, string.Format(@"The maximum length for a service name is {0} characters.", Utilities.MAXIMUM_SERVICE_NAME_LENGTH));

			IntPtr sc_hndl = IntPtr.Zero;
			IntPtr svc_hndl = IntPtr.Zero;

			try 
            {
				sc_hndl = Native.OpenSCManager(machineName, null, Native.GENERIC_WRITE);

				if (sc_hndl == IntPtr.Zero)
					return result(Result.Failed, "No administrative rights to uninstall services");

				svc_hndl = Native.OpenService(sc_hndl, serviceName, Native.DELETE);
				if (svc_hndl == IntPtr.Zero)
					return result(Result.Failed, "No administrative rights to uninstall services");

				if (Native.DeleteService(svc_hndl))
					return result(Result.Success);
				else
					return result(Result.Failed, "Unable to uninstall services");
			} 
            catch 
            {
				return result(Result.Failed, "Unable to uninstall services");
			} 
            finally 
            {
				if (svc_hndl != IntPtr.Zero)
					Native.CloseServiceHandle(svc_hndl);
				if (sc_hndl != IntPtr.Zero)
					Native.CloseServiceHandle(sc_hndl);
			}
        }
	}

	[Serializable]
	public class StartServiceTask : ServiceTask 
    {
		#region Constructors

		public StartServiceTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTaskResult(Utilities.StartServiceDirect(machineName, serviceName));
		}
	}

	[Serializable]
	public class ContinueServiceTask : ServiceTask 
    {
		#region Constructors

		public ContinueServiceTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTaskResult(Utilities.ContinueServiceDirect(machineName, serviceName));
		}
	}

	[Serializable]
	public class PauseServiceTask : ServiceTask 
    {
		#region Constructors

		public PauseServiceTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTaskResult(Utilities.PauseServiceDirect(machineName, serviceName));
		}
	}

	[Serializable]
	public class StopServiceTask : ServiceTask 
    {
		#region Constructors

		public StopServiceTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTaskResult(Utilities.StopServiceDirect(machineName, serviceName));
		}
	}

	[Serializable]
	public class IsServiceRunningTask : ServiceTask
    {
		#region Constructors

		public IsServiceRunningTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTask(Utilities.IsServiceRunningDirect(machineName, serviceName));
		}
	}

	[Serializable]
	public class IsServiceInstalledTask : ServiceTask 
    {
		#region Constructors

		public IsServiceInstalledTask(string MachineName, string ServiceName) : base(MachineName, ServiceName) 
        {}

		#endregion

		protected override TaskResult runServiceTask() 
        {
			return processDirectTask(Utilities.IsServiceInstalledDirect(machineName, serviceName));
		}
	}
}
