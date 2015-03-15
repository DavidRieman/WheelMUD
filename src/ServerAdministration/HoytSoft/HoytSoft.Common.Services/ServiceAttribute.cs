#region Copyright/Legal Notices
/***************************************************
 * Copyright © 2005 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 ***************************************************/
#endregion

namespace HoytSoft.Common.Services
{
    using System;

	///<summary>Describes a new Win32 service.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public class ServiceAttribute : System.Attribute
    {
		#region Private Variables

		private string name;
	    private bool autoInstall = true;

	    #endregion

		#region Constructors
		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		/// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
		/// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
		/// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
		/// <param name="ServiceControls">The controls or actions the service responds to.</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl, ServiceControls ServiceControls) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = Run;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		/// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
		/// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
		/// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = Run;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		/// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
		/// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
		/// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = true;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		/// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType, ServiceStartType ServiceStartType) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = true;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, ServiceStartType ServiceStartType) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = true;
			this.ServiceType = ServiceType.Default;;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = true;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
		/// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = Run;
			this.ServiceType = ServiceType;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		/// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
		public ServiceAttribute(string Name, string DisplayName, string Description, bool Run) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = Run;
			this.ServiceType = ServiceType.Default;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		/// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
		public ServiceAttribute(string Name, string DisplayName, string Description) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = Description;
			this.Run = true;
			this.ServiceType = ServiceType.Default;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		/// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
		public ServiceAttribute(string Name, string DisplayName) 
        {
			this.name = Name;
			this.DisplayName = DisplayName;
			this.Description = "";
			this.Run = true;
			this.ServiceType = ServiceType.Default;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		///<summary>Describes a new Win32 service.</summary>
		/// <param name="Name">The name of the service used in the service database.</param>
		public ServiceAttribute(string Name) 
        {
			this.name = Name;
			this.DisplayName = Name; //If no display name is specified, then make it the same as the name...
			this.Description = "";
			this.Run = true;
			this.ServiceType = ServiceType.Default;
			this.ServiceAccessType = ServiceAccessType.AllAccess;
			this.ServiceStartType = ServiceStartType.AutoStart;
			this.ServiceErrorControl = ServiceErrorControl.Normal;
			this.ServiceControls = ServiceControls.Default;
			this.LogName = "Services";
		}

		#endregion

		#region Properties

		///<summary>The name of the service used in the service database.</summary>
		public string Name { get { return this.name; } set { this.Name = value; } }

	    ///<summary>The name of the service that will be displayed in the services snap-in.</summary>
	    public string DisplayName { get; set; }

	    ///<summary>The description of the service that will be displayed in the service snap-in.</summary>
	    public string Description { get; set; }

	    ///<summary>Indicates if you want the service to run or not on program startup.</summary>
	    public bool Run { get; set; }

	    ///<summary>Indicates if you want to attempt to automatically install the service if it doesn't already exist.</summary>
		public bool AutoInstall { get { return this.autoInstall; } set { this.autoInstall = value; } }

	    ///<summary>Indicates the type of service you want to run.</summary>
	    public ServiceType ServiceType { get; set; }

	    ///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
	    public ServiceAccessType ServiceAccessType { get; set; }

	    ///<summary>Service start options.</summary>
	    public ServiceStartType ServiceStartType { get; set; }

	    ///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
	    public ServiceErrorControl ServiceErrorControl { get; set; }

	    ///<summary>The controls or actions the service responds to.</summary>
	    public ServiceControls ServiceControls { get; set; }

	    ///<summary>The name of the log you want to write to.</summary>
	    public string LogName { get; set; }

	    #endregion
	}
}
