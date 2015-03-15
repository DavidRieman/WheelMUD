#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

using System;
using System.EnterpriseServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationName(HoytSoft.Common.UAC.UACServer.COM_PROG_ID)]
[assembly: ApplicationID(HoytSoft.Common.UAC.UACServer.COM_GUID_APPLICATION)]
[assembly: ApplicationAccessControl(Authentication = AuthenticationOption.Default, ImpersonationLevel = ImpersonationLevelOption.Identify, AccessChecksLevel = AccessChecksLevelOption.ApplicationComponent)]

/*
 using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

using ManagedElevator.Guids;

namespace ManagedElevator.Components
{
    [Guid(SampleComponent.IHelloWorld)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IHelloWorld
    {
        [ComVisible(true)]
        void SayHello();
    }

    [Guid(SampleComponent.ClassToElevate)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ClassToElevate : IHelloWorld
    {
        public ClassToElevate()
        {
        }

        [ComVisible(true)]
        public void SayHello()
        {
            MessageBox.Show("Hello World");
        }
    }
}


 */
namespace HoytSoft.Common.UAC 
{
	#region Delegates

	[ComVisible(false)]
	public delegate bool RunAsAdministratorDelegate(object Param);

	#endregion

	#region COM Server

	[ComVisible(true)]
	[Guid(UACServer.COM_GUID_INTERFACE)]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[System.ComponentModel.Description("COM interface for UAC")]
	public interface IUACServer 
    {
		[DispId(1)]
		[ComVisible(true)]
		bool RunAsAdmin(object Param);
		//event RunAsAdministratorDelegate RunAsAdministrator;
	}

	//[ComVisible(true)]
	//[Guid(UACServer.COM_GUID_EVENTS)]
	//[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	//[System.ComponentModel.Description("COM event interface for UAC")]
	//public interface IUACServerEvents {
	//    [DispId(1)]
	//    [ComVisible(true)]
	//    bool OnRunAsAdministrator(object Param);
	//}

	[ComVisible(true)]
	[ProgId(UACServer.COM_PROG_ID)]
	[Guid(UACServer.COM_GUID_CLASS)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(IUACServer))]
	//[ComSourceInterfaces(typeof(IUACServerEvents))]
	[System.ComponentModel.Description("COM object for UAC")]
	public class UACServer : IUACServer 
    {
		#region Constants
		public const string
			COM_GUID_APPLICATION = "4712B32B-7E0D-4876-8C48-B2CE47C609FD",
			COM_GUID_INTERFACE = "65C4D8C6-A0CE-4983-9CD2-B8B2CDE5CD05",
			COM_GUID_EVENTS = "9457C462-207A-45a8-9BD1-96FAA8BDBC07",
			COM_GUID_CLASS = "25A747FD-F045-4d55-BA86-8E37A94AEE58"
		;

		public const string
			COM_PROG_ID = "HoytSoft.Common.UAC.UACServer"
		;
		#endregion

		#region Constructors
		public UACServer() {
		}
		#endregion

		#region IUACServer Members
		[ComVisible(true)]
		public bool RunAsAdmin(object Param) {
			//if (RunAsAdministrator != null)
			//    return RunAsAdministrator(Param);
			Console.WriteLine("TEST THIS!!");
			Console.ReadLine();
			return true;
		}
		#endregion

		#region DllRegisterServer/DllUnregisterServer
		[ComRegisterFunction]
		protected static void registerCOMObject(string RegistryLocation) {
			//Debug.WriteLine("REGISTERING UAC COM OBJ!: " + RegistryLocation);

			//Use RC resource ID #100 ("Application Administrative Task")
			Utilities.RegisterForElevation(typeof(UACServer).Assembly.Location, COM_GUID_CLASS, COM_GUID_APPLICATION, 100);
			//Add additional "for elevation" components here by duplicating the above
		}

		[ComUnregisterFunction]
		protected static void unregisterCOMObject(string RegistryLocation) {
			//Debug.WriteLine("UNREGISTERING UAC COM OBJ!: " + RegistryLocation);

			Utilities.UnRegisterFromElevation(typeof(UACServer).Assembly.Location, COM_GUID_APPLICATION);
		}
		#endregion

		#region Events
		//public event RunAsAdministratorDelegate RunAsAdministrator;
		#endregion
	}

	#endregion

	#region Interop

	namespace Interop 
    {
		///<summary>
		///	Create early-bound object so we don't have to do anything special to call it if no UAC exists 
		///	or we already have administrative access.
		///</summary>
		[ComImport]
		[Guid(UACServer.COM_GUID_INTERFACE)]
		[InterfaceType(ComInterfaceType.InterfaceIsDual)]
		public interface IUAC 
        {
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[PreserveSig]
			bool RunAsAdmin(object Param);
		}
	}

	#endregion

	#region UAC Partial

	public partial class UAC 
    {
		public static Result RunTaskUsingCOMServer(RunAsAdministratorDelegate Func) 
        {
			return RunTaskUsingCOMServer(null, Func);
		}

		public static Result RunTaskUsingCOMServer(object Param, RunAsAdministratorDelegate Func) 
        {
			if (Func == null)
				throw new ArgumentNullException("Func is null");

			bool ret;

			if (Utilities.IsReallyVista()) 
            {
				Interop.IUAC o = null;

				try 
                {
					o = (Interop.IUAC)Utilities.LaunchElevatedCOMObject(new Guid(UACServer.COM_GUID_CLASS), new Guid(UACServer.COM_GUID_INTERFACE));
				} 
                catch (COMException) 
                {
					if (o != null)
						Marshal.ReleaseComObject(o);

					//if (Utilities.IsCancelled(ce))
					return Result.Cancelled;
					//else
					//	return Result.Failed;
				}

				if (o == null)
					throw new UACException("Unable to create COM server to process task");

				try 
                {
					ret = o.RunAsAdmin(Param);
				} 
                catch (Exception e) 
                {
					throw e;
				} 
                finally 
                {
					Marshal.ReleaseComObject(o);
				}
			} 
            else 
            {
				var s = new UACServer();
				ret = s.RunAsAdmin(Param);
			}

			return (ret ? Result.Success : Result.Failed);
		}
	}

	#endregion
}
