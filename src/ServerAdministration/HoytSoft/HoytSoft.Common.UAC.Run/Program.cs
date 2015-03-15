#region Copyright/Legal Notices
/*********************************************************
 * Copyright © 2008 HoytSoft. All rights reserved. 
 *	Please see included license for more details.
 *********************************************************/
#endregion

namespace HoytSoft.Common.UAC.Run
{
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Program class.
    /// </summary>
    public class Program 
    {
        /// <summary>
        /// This is the main entry into this class.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>Returns a result code in the form of an integer.</returns>
        public static int Main(string[] args) 
        {
            if (args == null || args.Length < 2) 
            {
                PrintUsage();
                return -1;
            }

            string host = args[0];
            int port = -1;

            if (!int.TryParse(args[1], out port)) 
            {
                PrintUsage();
                return -1;
            }

            var formatter = new BinaryFormatter();
            NetworkStream stream = null;
            TcpClient c = null;

            try 
            {
                c = new TcpClient();
                c.Connect(host, port);
                stream = c.GetStream();

                // Get the task from the UAC library
                // See HoytSoft.Common.UAC.UAC class, runIt() method
                object o = formatter.Deserialize(stream);
                if (!(o is IAdministrativeTask)) 
                {
                    PrintUsage();
                    return -1;
                }

                // Here's the meat!
                var task = o as IAdministrativeTask;
                TaskResult result = task.RunTask();

                formatter.Serialize(stream, result);
                stream.Flush();
            } 
            catch 
            {
                return -1;
            } 
            finally 
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (c != null)
                {
                    c.Close();
                }
            }

            return 0;
        }

        private static void PrintUsage()
        {
        }
    }
}
