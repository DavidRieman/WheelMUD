#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.Services
{
    using System;

	public class ServiceRuntimeException : ServiceException 
    {
		public ServiceRuntimeException(string Message) : base(Message) { }
		public ServiceRuntimeException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceStartupException : ServiceException 
    {
		public ServiceStartupException(string Message) : base(Message) { }
		public ServiceStartupException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceUninstallException : ServiceException 
    {
		public ServiceUninstallException(string Message) : base(Message) { }
		public ServiceUninstallException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceInstallException : ServiceException 
    {
		public ServiceInstallException(string Message) : base(Message) { }
		public ServiceInstallException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}

	public class ServiceException : Exception 
    {
		public ServiceException(string Message) : base(Message) { }
		public ServiceException(string Message, Exception InnerException) : base(Message, InnerException) { }
	}
}
