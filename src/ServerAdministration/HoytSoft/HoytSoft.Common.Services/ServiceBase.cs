#region Copyright/Legal Notices

/*********************************************************
 * Copyright © 2005, 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/

#endregion

namespace HoytSoft.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using HoytSoft.Common.UAC;

    public abstract class ServiceBase : IDisposable
    {
        #region Enums

        private enum ArgumentAction : byte
        {
            Nothing = 0,
            Install = 1,
            Uninstall = 2
        }

        #endregion

        #region Variables

        private readonly Native.ServiceCtrlHandlerExProc servCtrlHandlerExProc;
        private string[] args;
        private bool initialized;
        private EventLog log;
        private string logName;

        private ServiceState servState = ServiceState.Stopped;
        private Native.SERVICE_STATUS servStatus;
        private IntPtr servStatusHandle;

        private ManualResetEvent stopEvent;

        #endregion

        #region Constructors

        protected ServiceBase()
        {
            Debug = false;
            servCtrlHandlerExProc = new Native.ServiceCtrlHandlerExProc(serviceControlHandlerEx);
            servStatus = new Native.SERVICE_STATUS();
            servState = ServiceState.Stopped;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void init()
        {
            if (initialized)
                return;
            initialized = true;

            ServiceAttribute serviceDef = ServiceAttribute;
            Name = serviceDef.Name;
            DisplayName = serviceDef.DisplayName;
            Description = serviceDef.Description;
            Run = serviceDef.Run;
            ServiceType = serviceDef.ServiceType;
            ServiceAccessType = serviceDef.ServiceAccessType;
            ServiceStartType = serviceDef.ServiceStartType;
            ServiceErrorControl = serviceDef.ServiceErrorControl;
            ServiceControls = serviceDef.ServiceControls;
            logName = serviceDef.LogName;
        }

        #endregion

        #region Properties

        internal virtual ServiceAttribute ServiceAttribute
        {
            get
            {
                var attribs = (ServiceAttribute[]) GetType().GetCustomAttributes(Utilities.SERVICE_ATTRIBUTE_TYPE, true);
                if (attribs == null || attribs.Length <= 0)
                    return null;
                return attribs[0];
            }
        }

        ///<summary>The name of the service used in the service database.</summary>
        public string Name { get; private set; }

        ///<summary>The name of the service that will be displayed in the services snap-in.</summary>
        public string DisplayName { get; private set; }

        ///<summary>The description of the service that will be displayed in the service snap-in.</summary>
        public string Description { get; private set; }

        ///<summary>Indicates if you want the service to run or not on program startup.</summary>
        public bool Run { get; private set; }

        ///<summary>Indicates the type of service you want to run.</summary>
        public ServiceType ServiceType { get; private set; }

        ///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
        public ServiceAccessType ServiceAccessType { get; private set; }

        ///<summary>Service start options.</summary>
        public ServiceStartType ServiceStartType { get; private set; }

        ///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
        public ServiceErrorControl ServiceErrorControl { get; private set; }

        ///<summary>The controls or actions the service responds to.</summary>
        public ServiceControls ServiceControls { get; private set; }

        ///<summary>The current state of the service.</summary>
        public ServiceState ServiceState
        {
            get { return servState; }
        }

        public bool IsDisposed { get; private set; }

        ///<summary>Treats the service as a console application instead of a normal service.</summary>
        public bool Debug { get; private set; }

        #endregion

        #region Override Methods

        protected virtual bool Initialize(string[] Arguments)
        {
            return true;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Pause()
        {
        }

        protected virtual void Stop()
        {
        }

        protected virtual void Continue()
        {
        }

        protected virtual void Shutdown()
        {
        }

        protected virtual void Interrogate()
        {
        }

        protected virtual void CustomMessage(uint Code)
        {
        }

        #endregion

        #region Helper Methods

        /*
		//
		// Purpose: 
		//   Sets the current service status and reports it to the SCM
		//
		// Parameters:
		//   dwCurrentState - The current state (see SERVICE_STATUS)
		//   dwWin32ExitCode - The system error code
		//   dwWaitHint - Estimated time for pending operation, 
		//     in milliseconds
		// 
		// Return value:
		//   None
		//
		VOID ReportSvcStatus( DWORD dwCurrentState,
					  DWORD dwWin32ExitCode,
					  DWORD dwWaitHint)
		{
		static DWORD dwCheckPoint = 1;

		// Fill in the SERVICE_STATUS structure.

		gSvcStatus.dwCurrentState = dwCurrentState;
		gSvcStatus.dwWin32ExitCode = dwWin32ExitCode;
		gSvcStatus.dwWaitHint = dwWaitHint;

		if (dwCurrentState == SERVICE_START_PENDING)
		gSvcStatus.dwControlsAccepted = 0;
		else gSvcStatus.dwControlsAccepted = SERVICE_ACCEPT_STOP;

		if ( (dwCurrentState == SERVICE_RUNNING) ||
		   (dwCurrentState == SERVICE_STOPPED) )
		gSvcStatus.dwCheckPoint = 0;
		else gSvcStatus.dwCheckPoint = dwCheckPoint++;

		// Report the status of the service to the SCM.
		SetServiceStatus( gSvcStatusHandle, &gSvcStatus );
		}
		 */

        private uint checkPoint = 1;

        private static ArgumentAction interpretAction(string[] args)
        {
            if (args == null || args.Length <= 0)
                return ArgumentAction.Nothing;

            string arg = args[0];

            if (string.IsNullOrEmpty(arg))
                return ArgumentAction.Nothing;

            switch (arg.ToLower())
            {
                case "i":
                case "install":
                    return ArgumentAction.Install;
                case "u":
                case "uninstall":
                    return ArgumentAction.Uninstall;
                default:
                    return ArgumentAction.Nothing;
            }
        }

        internal void ReportSvcStatus(uint CurrentState, uint Win32ExitCode, uint WaitHint)
        {
            servStatus.dwCurrentState = CurrentState;
            servStatus.dwWin32ExitCode = Win32ExitCode;
            servStatus.dwWaitHint = WaitHint;

            if (CurrentState == Native.SERVICE_START_PENDING)
                servStatus.dwControlsAccepted = 0;
            else
                servStatus.dwControlsAccepted = (uint) ServiceControls;

            if (CurrentState == Native.SERVICE_RUNNING || CurrentState == Native.SERVICE_STOPPED)
                servStatus.dwCheckPoint = 0;
            else
                servStatus.dwCheckPoint = checkPoint++;

            Native.SetServiceStatus(servStatusHandle, ref servStatus);
        }

        #endregion

        #region Logging Methods

        private void checkLog()
        {
            if (log == null)
            {
                log = new EventLog(logName) {Source = DisplayName};
            }
        }

        public void Log(string Message)
        {
            try
            {
                checkLog();
                log.WriteEntry(Message);
                log.Close();
            }
            catch (Win32Exception)
            {
                //In case the event log is full....
            }
        }

        public void Log(string Message, EventLogEntryType EntryType)
        {
            try
            {
                checkLog();
                log.WriteEntry(Message, EntryType);
            }
            catch (Win32Exception)
            {
            }
        }

        public void Log(string Message, EventLogEntryType EntryType, int EventID)
        {
            try
            {
                checkLog();
                log.WriteEntry(Message, EntryType, EventID);
            }
            catch (Win32Exception)
            {
            }
        }

        public void Log(string Message, EventLogEntryType EntryType, short Category, int EventID)
        {
            try
            {
                checkLog();
                log.WriteEntry(Message, EntryType, EventID, Category);
            }
            catch (Win32Exception)
            {
            }
        }

        #endregion

        #region Testing Methods

        private bool inTestAction;

        protected void TestStop()
        {
            var t = new Thread(delegate()
                                   {
                                       while (inTestAction || servState == ServiceState.Interrogating)
                                           ;
                                       inTestAction = true;
                                       servState = ServiceState.Stopped;
                                       Stop();
                                       inTestAction = false;
                                   }) {Name = "Test Stop: " + DisplayName, IsBackground = true};

            t.Start();
        }

        protected void TestPause()
        {
            var t = new Thread(delegate()
                                   {
                                       if (servState != ServiceState.ShuttingDown)
                                       {
                                           while (inTestAction || servState == ServiceState.Interrogating)
                                               ;
                                           inTestAction = true;
                                           servState = ServiceState.Paused;
                                           Pause();
                                           inTestAction = false;
                                       }
                                   }) {Name = "Test Pause: " + DisplayName, IsBackground = true};

            t.Start();
        }

        protected void TestContinue()
        {
            var t = new Thread(delegate()
                                   {
                                       while (inTestAction || servState == ServiceState.Interrogating)
                                           ;
                                       inTestAction = true;
                                       servState = ServiceState.Running;
                                       Continue();
                                       inTestAction = false;
                                   }) {Name = "Test Continue: " + DisplayName, IsBackground = true};

            t.Start();
        }

        protected void TestShutdown()
        {
            var t = new Thread(delegate()
                                   {
                                       while (inTestAction || servState == ServiceState.Interrogating)
                                           ;
                                       inTestAction = true;
                                       servState = ServiceState.ShuttingDown;
                                       Shutdown();
                                       inTestAction = false;
                                   }) {Name = "Test Shutdown: " + DisplayName, IsBackground = true};

            t.Start();
        }

        protected void TestInterrogate()
        {
            var t = new Thread(delegate()
                                   {
                                       while (inTestAction)
                                           ;
                                       inTestAction = true;
                                       servState = ServiceState.Interrogating;
                                       Interrogate();
                                       servState = ServiceState.Running;
                                       inTestAction = false;
                                   }) {Name = "Test Interrogate: " + DisplayName, IsBackground = true};

            t.Start();
        }

        protected void TestCustomMessage(uint Code)
        {
            var t = new Thread(delegate(object param)
                                   {
                                       while (inTestAction)
                                           ;
                                       inTestAction = true;
                                       servState = ServiceState.Running;
                                       CustomMessage((uint) param);
                                       servState = ServiceState.Running;
                                       inTestAction = false;
                                   })
                        {
                            Name = "Test Custom Message (" + Code + "): " + DisplayName,
                            IsBackground = true
                        };

            t.Start(Code);
        }

        #endregion

        #region IDisposable Members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            if (!string.IsNullOrEmpty(Name) && Utilities.IsServiceInstalled(Name) && Utilities.IsServiceRunning(Name))
                Utilities.StopService(Name);

            if (stopEvent != null)
                stopEvent.Close();

            DisposeService();

            if (log != null)
            {
                log.Close();
                log.Dispose();
                log = null;
            }
        }

        #endregion

        protected virtual void DisposeService()
        {
        }

        ~ServiceBase()
        {
            Dispose();
        }

        #region Overloaded Methods

        ///<summary>Run all services in the executing assembly.</summary>
        public static void RunServices(string[] Args)
        {
            RunServices(Args, Assembly.GetEntryAssembly());
        }

        ///<summary>Run the service defined by ServiceType.</summary>
        public static void RunService(string[] Args, Type ServiceType)
        {
            RunServices(Args, new[] {ServiceType});
        }

        ///<summary>Run the service.</summary>
        public static void RunService(string[] Args, ServiceBase Service)
        {
            RunServices(Args, new[] {Service});
        }

        ///<summary>Run all services in the assembly.</summary>
        public static void RunServices(string[] Args, Assembly Assembly)
        {
            if (Assembly == null) throw new ServiceException("No currently executing assembly.");
            RunServices(Args, Utilities.FindAllValidServices(Assembly));
        }

        #endregion

        #region Entry Point

        ///<summary>If you do not want to specify your own main entry point, you can use this one.</summary>
        public static void Main(string[] Args)
        {
            RunServices(Args);
        }

        #endregion

        #region Public Static Methods

        ///<summary>Executes your service. If multiple services are defined in the assembly, it will run them all in separate threads.</summary>
        /// <param name="Args">The arguments passed in from the command line.</param>
        /// <param name="Types">An array of types we want to inspect for services.</param>
        public static void RunServices(string[] Args, Type[] Types)
        {
            var services = new List<ServiceBase>(1);

            services.AddRange(from t in Types
                              where Utilities.IsValidServiceType(t)
                              select (ServiceBase) Activator.CreateInstance(t));

            if (services.Count > 0)
                RunServices(Args, services.ToArray());
        }

        public static void RunServices(string[] Args, ServiceBase[] Services)
        {
            #region Setup variables

            if (Args == null)
                Args = new string[0];

            string path = Assembly.GetEntryAssembly().Location;
            ArgumentAction action = interpretAction(Args);
            TaskResult taskResult;
            var dispatchTables = new List<Native.SERVICE_TABLE_ENTRY>(1);

            #endregion

            foreach (ServiceBase service in Services)
            {
                #region Validate type

                if (service == null)
                    continue;

                ServiceAttribute serviceDef = service.ServiceAttribute;
                if (serviceDef == null)
                    continue;

                #endregion

                #region Attempt to install, if necessary

                //Did the developer specify to automatically install this? If not, do nothing!
                if (action == ArgumentAction.Install ||
                    (action != ArgumentAction.Uninstall && serviceDef.AutoInstall &&
                     !Utilities.IsServiceInstalled(serviceDef)))

                    if ((taskResult = Utilities.InstallService(path, serviceDef)) == null ||
                        taskResult.Result != Result.Success)
                        throw new ServiceInstallException("Unable to install service: " + serviceDef.DisplayName);

                #endregion

                #region Check for uninstall action, if necessary

                if (action == ArgumentAction.Uninstall && Utilities.IsServiceInstalled(serviceDef))
                    if ((taskResult = Utilities.UninstallService(serviceDef)) == null ||
                        taskResult.Result != Result.Success)
                        throw new ServiceUninstallException("Unable to uninstall service: " + serviceDef.DisplayName);

                #endregion

                //Skip to next if we just want to install/uninstall
                if (action != ArgumentAction.Nothing)
                    continue;

                //Make sure we want to run this service
                if (!serviceDef.Run)
                    continue;

                var entry = new Native.SERVICE_TABLE_ENTRY();
                entry.lpServiceName = serviceDef.Name;
                entry.lpServiceProc = new Native.ServiceMainProc(service.serviceMain);
                dispatchTables.Add(entry);
                service.Debug = false;
            }

            if (dispatchTables.Count > 0)
            {
                //Add a null entry to tell the API it's the last entry in the table...
                var entry = new Native.SERVICE_TABLE_ENTRY();
                entry.lpServiceName = null;
                entry.lpServiceProc = null;
                dispatchTables.Add(entry);

                //Send dispatch table to service control manager so it knows where to call the main func.
                Native.SERVICE_TABLE_ENTRY[] table = dispatchTables.ToArray();

                if (!Native.StartServiceCtrlDispatcher(table))
                {
                    //There was an error. What was it?
                    switch (Marshal.GetLastWin32Error())
                    {
                        case Native.ERROR_INVALID_DATA:
                            throw new ServiceStartupException(
                                "The specified dispatch table contains entries that are not in the proper format.");
                        case Native.ERROR_SERVICE_ALREADY_RUNNING:
                            throw new ServiceStartupException("A service is already running.");
                        case Native.ERROR_FAILED_SERVICE_CONTROLLER_CONNECT:
                            //"A service is being run as a console application. Try setting the Service attribute's \"Debug\" property to true if you're testing an application."
                            //If we've started up as a console/windows app, then we'll get this error in which case we treat the program
                            //like a normal app instead of a service and start it up in "debug" mode...
                            foreach (ServiceBase service in Services)
                            {
                                service.Debug = true;
                                service.args = Args;
                                var t = new Thread(service.serviceDebugMain);
                                t.Name = "Service: " + service.ServiceAttribute.DisplayName;
                                t.IsBackground = false;
                                t.Start();
                            } /**/
                            break;
                        default:
                            throw new ServiceStartupException(
                                "An unknown error occurred while starting up the service(s).");
                    }
                }
            }
        }

        #endregion

        #region Service Methods

        private static void msg(string msg)
        {
            File.AppendAllText(Path.Combine(@"C:\Users\David\Desktop", "log.txt"), msg + "\r\n");
        }

        private void serviceMain(uint argc, string[] argv)
        {
            init();

            servStatusHandle = Native.RegisterServiceCtrlHandlerEx(Name, servCtrlHandlerExProc);

            //These SERVICE_STATUS members remain as set here.
            servStatus.dwServiceType = (uint) ServiceType;
            servStatus.dwServiceSpecificExitCode = 0;

            //Report initial status to the SCM.
            ReportSvcStatus(Native.SERVICE_START_PENDING, Native.NO_ERROR, 3000);

            stopEvent = new ManualResetEvent(false);

            if (!Initialize(argv))
            {
                ReportSvcStatus(Native.SERVICE_STOPPED, Native.NO_ERROR, 0);
                return;
            }

            ReportSvcStatus(Native.SERVICE_RUNNING, Native.NO_ERROR, 0);
            Start();

            while (true)
            {
                stopEvent.WaitOne();
                ReportSvcStatus(Native.SERVICE_STOPPED, Native.NO_ERROR, 0);
            }
        }

        private void serviceDebugMain()
        {
            if (Initialize(args))
            {
                servState = ServiceState.Running;
                Start();
                //Wait for all tests to finish...
                //while(this.inTestAction)
                //	;
                //this.TestStop();
            }
        }

        private uint serviceControlHandlerEx(uint OpCode, uint EventType, IntPtr EventData, IntPtr Context)
        {
            switch (OpCode)
            {
                case Native.SERVICE_CONTROL_STOP:
                    servState = ServiceState.Stopped;
                    ReportSvcStatus(Native.SERVICE_STOP_PENDING, Native.NO_ERROR, 3000);
                    try
                    {
                        Stop();
                    }
                    catch (Exception e)
                    {
                        Log("An exception occurred while trying to stop the service:" + e);
                    }

                    stopEvent.Set();

                    break;

                case Native.SERVICE_CONTROL_PAUSE:
                    servState = ServiceState.Paused;
                    ReportSvcStatus(Native.SERVICE_PAUSE_PENDING, Native.NO_ERROR, 3000);
                    try
                    {
                        Pause();
                    }
                    catch (Exception e)
                    {
                        Log("An exception occurred while trying to pause the service:" + e);
                    }

                    servStatus.dwCurrentState = Native.SERVICE_PAUSED;
                    break;

                case Native.SERVICE_CONTROL_CONTINUE:
                    servState = ServiceState.Running;
                    ReportSvcStatus(Native.SERVICE_CONTINUE_PENDING, Native.NO_ERROR, 3000);
                    try
                    {
                        Continue();
                    }
                    catch (Exception e)
                    {
                        Log("An exception occurred while trying to continue the service:" + e);
                    }

                    servStatus.dwCurrentState = Native.SERVICE_RUNNING;
                    break;

                case Native.SERVICE_CONTROL_SHUTDOWN:
                    servState = ServiceState.Running;
                    ReportSvcStatus(Native.SERVICE_STOP_PENDING, Native.NO_ERROR, 3000);
                    try
                    {
                        Shutdown();
                    }
                    catch (Exception e)
                    {
                        Log("An exception occurred while trying to shutdown the service:" + e);
                    }

                    stopEvent.Set();
                    break;

                case Native.SERVICE_INTERROGATE:
                    servState = ServiceState.Interrogating;
                    try
                    {
                        Interrogate();
                    }
                    catch (Exception e)
                    {
                        Log("An exception occurred while trying to interrogate the service:" + e);
                    }
                    break;

                default:
                    if (OpCode >= 128 && OpCode <= 255)
                        try
                        {
                            CustomMessage(OpCode);
                        }
                        catch (Exception e)
                        {
                            Log("An exception occurred while trying to interrogate the service:" + e);
                        }

                    break;
            }

            ReportSvcStatus(servStatus.dwCurrentState, Native.NO_ERROR, 0);

            return Native.NO_ERROR;
        }

        #endregion
    }
}