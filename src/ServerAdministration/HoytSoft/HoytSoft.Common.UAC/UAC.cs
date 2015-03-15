#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.UAC
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Permissions;

	#region Enums

	public enum Result : byte 
    {
		Unknown = 3,
		Failed = 2,
		Success = 1,
		Cancelled = 0
	}

	#endregion

	#region IAdministrativeTask

	public interface IAdministrativeTask 
    {
		TaskResult RunTask();
	}

	[Serializable]
	public sealed class TaskResult 
    {
		private Result result;
		private string message;
		private object param;

		public TaskResult(Result Result) : this(Result, string.Empty, null) 
        {
		}

		public TaskResult(Result Result, string Message) : this(Result, Message, null) 
        {
		}

		public TaskResult(Result Result, string Message, object Param) 
        {
			this.result = Result;
			this.message = Message;
			this.param = Param;
		}

		public Result Result 
        {
			get { return this.result; }
		}

		public string Message 
        {
			get { return this.message; }
		}

		public object Param 
        {
			get { return this.param; }
		}
	}

	[Serializable]
	public abstract class AbstractAdministrativeTask : MarshalByRefObject, IAdministrativeTask 
    {
		public abstract TaskResult RunTask();
	}

	#endregion

	public partial class UAC 
    {
		public static TaskResult RunTask(IAdministrativeTask Task) 
        {
			//Only use UAC if we MUST! Otherwise, just execute normally.
			//The only caveat is that "Task" must not depend on memory/state 
			//in the same process the executable is running in.
			if ((Utilities.IsReallyVista() && !Utilities.IsElevated()) || (!Utilities.IsReallyVista() && !Utilities.IsAdministrator()))
				return runIt(Task);

			if (Task == null)
				throw new ArgumentNullException("Task is null");

			return Task.RunTask();
		}

		[SecurityPermission(SecurityAction.Demand)]
		private static TaskResult runIt(IAdministrativeTask Task) 
        {
			//Check if task is serializable
			if (!Task.GetType().IsDefined(typeof(SerializableAttribute), false))
				throw new ArgumentException("Task must be serializable");

			var s = new TcpListener(IPAddress.Loopback, 0);
			s.Start(1);

			string address = (s.LocalEndpoint as IPEndPoint).Address.ToString();
			int port = (s.LocalEndpoint as IPEndPoint).Port;
			
			//Have it connect
			if (!Utilities.RunElevatedAsync(Utilities.UAC_RUN_APPLICATION, address + " " + port))
				return new TaskResult(Result.Cancelled);

			TcpClient c = null;
			NetworkStream stream = null;
			var formatter = new BinaryFormatter();

			try 
            {
				//Wait for client to connect
				c = s.AcceptTcpClient();
				stream = c.GetStream();
				formatter.Serialize(stream, Task);
				stream.Flush();

				//Get the result of the task
				object o = formatter.Deserialize(stream);
				if (o != null && (o is TaskResult))
					return (o as TaskResult);
			} 
            catch 
            {} 
            finally 
            {
				if (stream != null)
					stream.Close();

				if (c != null)
					c.Close();

				if (s != null)
					s.Stop();
			}

			return new TaskResult(Result.Failed);
			//// Create and register an IPC channel
			//IpcServerChannel serverChannel = new IpcServerChannel(Utilities.UAC_REMOTING_CHANNEL_NAME);
			//serverChannel.IsSecured = true;

			//ChannelServices.RegisterChannel(serverChannel);

			////Expose an object
			//RemotingConfiguration.RegisterWellKnownServiceType(Task.GetType(), Utilities.UAC_REMOTING_CHANNEL_URI, WellKnownObjectMode.Singleton);

			//// Wait for calls
			//Console.WriteLine("Listening on {0}", serverChannel.GetChannelUri());
			//Console.ReadLine();
		}
	}
}
