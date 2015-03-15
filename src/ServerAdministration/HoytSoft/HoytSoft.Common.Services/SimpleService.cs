#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.Services 
{
	[Service(
		"",
		"",
		"",
		AutoInstall = false,
		ServiceType = ServiceType.OwnProcess,
		ServiceStartType = ServiceStartType.AutoStart,
		ServiceControls = ServiceControls.StartAndStop | ServiceControls.Shutdown
	)]
	public class SimpleService : ServiceBase 
    {
		#region Variables

		private ParameterizedServiceDelegate funcStart, funcStop, funcShutdown;
		private ParameterizedServiceCustomMessageDelegate funcCustomMsg;
		private ServiceAttribute attrib;
		private object param;

		#endregion

		#region Constructors

		public SimpleService(string ServiceName, string DisplayName, string Description, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceDelegate StopMethod) : this(ServiceName, DisplayName, Description, ServiceType.OwnProcess, ServiceStartType.AutoStart, Param, StartMethod, null, StopMethod, null) 
        {}

		public SimpleService(string ServiceName, string DisplayName, string Description, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceDelegate StopMethod, ParameterizedServiceDelegate ShutdownMethod) : this(ServiceName, DisplayName, Description, ServiceType.OwnProcess, ServiceStartType.AutoStart, Param, StartMethod, null, StopMethod, ShutdownMethod) 
        {}

		public SimpleService(string ServiceName, string DisplayName, string Description, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceCustomMessageDelegate CustomMessageMethod, ParameterizedServiceDelegate StopMethod, ParameterizedServiceDelegate ShutdownMethod) : this(ServiceName, DisplayName, Description, ServiceType.OwnProcess, ServiceStartType.AutoStart, Param, StartMethod, CustomMessageMethod, StopMethod, ShutdownMethod) 
        {}

		public SimpleService(string ServiceName, string DisplayName, string Description, ServiceType ServiceType, ServiceStartType ServiceStartType, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceDelegate StopMethod) : this(ServiceName, DisplayName, Description, ServiceType, ServiceStartType, Param, StartMethod, null, StopMethod, null) 
        {}

		public SimpleService(string ServiceName, string DisplayName, string Description, ServiceType ServiceType, ServiceStartType ServiceStartType, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceDelegate StopMethod, ParameterizedServiceDelegate ShutdownMethod) : this(ServiceName, DisplayName, Description, ServiceType, ServiceStartType, Param, StartMethod, null, StopMethod, ShutdownMethod) 
        {}

		public SimpleService(string ServiceName, string DisplayName, string Description, ServiceType ServiceType, ServiceStartType ServiceStartType, object Param, ParameterizedServiceDelegate StartMethod, ParameterizedServiceCustomMessageDelegate CustomMessageMethod, ParameterizedServiceDelegate StopMethod, ParameterizedServiceDelegate ShutdownMethod) 
        {
			this.attrib = new ServiceAttribute(ServiceName, DisplayName, Description, ServiceType, ServiceStartType);
			this.attrib.ServiceControls = ServiceControls.StartAndStop | ServiceControls.Shutdown;
			this.attrib.AutoInstall = true;

			this.funcCustomMsg = CustomMessageMethod;
			this.funcShutdown = ShutdownMethod;
			this.funcStart = StartMethod;
			this.funcStop = StopMethod;
			this.param = Param;
		}

		#endregion

		#region Properties

		internal override ServiceAttribute ServiceAttribute 
        {
			get { return this.attrib; }
		}

		public object Param 
        {
			get { return this.param; }
		}

		#endregion

		#region Events

		public delegate void ParameterizedServiceDelegate(SimpleService Service, object Param);
		public delegate void ParameterizedServiceCustomMessageDelegate(SimpleService Service, uint Code, object Param);
		public delegate void ServiceCustomMessageDelegate(SimpleService Service, uint Code);
		public delegate void ServiceDelegate(SimpleService Service);

		public event ServiceDelegate ServiceStart;
		public event ServiceDelegate ServiceStop;
		public event ServiceDelegate ServiceShutdown;
		public event ServiceCustomMessageDelegate ServiceCustomMessage;

		protected void OnSimpleServiceStart() 
        {
			if (ServiceStart != null)
				ServiceStart(this);
		}

		protected void OnSimpleServiceStop() 
        {
			if (ServiceStop != null)
				ServiceStop(this);
		}

		protected void OnSimpleServiceShutdown() 
        {
			if (ServiceShutdown != null)
				ServiceShutdown(this);
		}

		protected void OnSimpleServiceCustomMessage(uint Code) 
        {
			if (ServiceCustomMessage != null)
				ServiceCustomMessage(this, Code);
		}

		#endregion

		#region Overrides

		protected override void Start() 
        {
			base.Start();
			OnSimpleServiceStart();
			if (this.funcStart != null)
				this.funcStart(this, this.param);
		}

		protected override void Stop() 
        {
			base.Stop();
			OnSimpleServiceStop();
			if (this.funcStop != null)
				this.funcStop(this, this.param);
		}

		protected override void Shutdown() 
        {
			base.Shutdown();
			OnSimpleServiceShutdown();
			if (this.funcShutdown != null)
				this.funcShutdown(this, this.param);
		}

		protected override void CustomMessage(uint Code) 
        {
			base.CustomMessage(Code);
			OnSimpleServiceCustomMessage(Code);
			if (this.funcCustomMsg != null)
				this.funcCustomMsg(this, Code, this.param);
		}

		#endregion

		#region Public Methods

		public void ServiceTestStop() 
        {
			this.TestStop();
		}

		public void ServiceTestShutdown() 
        {
			this.TestShutdown();
		}

		public void ServiceTestInterrogate() 
        {
			this.TestInterrogate();
		}

		public void ServiceTestCustomMessage(uint Code) 
        {
			this.TestCustomMessage(Code);
		}

		#endregion
	}
}
