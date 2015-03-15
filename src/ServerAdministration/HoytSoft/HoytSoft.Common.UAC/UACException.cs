#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.UAC
{
    using System;

	public class UACException : ApplicationException 
    {
		// Default constructor
		public UACException() {}

		// Constructor accepting a single string message
		public UACException(string message) : base(message) {}

		// Constructor accepting a string message and an 
		// inner exception which will be wrapped by this 
		// custom exception class
		public UACException(string message, Exception inner) : base(message, inner) {}
	}
}
