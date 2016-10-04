//-----------------------------------------------------------------------------
// <copyright file="LoggingService.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 6/12/2009 8:20:41 AM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using log4net;
    using log4net.Config;

    /// <summary>This will handle logging for all of WheelMUD using log4net ( http://logging.apache.org/log4net/ ).</summary>
    public class LoggingService
    {
        /// <summary>These are all of the valid logging levels that can be used in code.</summary>
        public enum LogLevel
        {
            /// <summary>Log level is debug.</summary>
            DEBUG = 1,

            /// <summary>Log level is error.</summary>
            ERROR,

            /// <summary>Log level is fatal.</summary>
            FATAL,

            /// <summary>Log level is info.</summary>
            INFO,

            /// <summary>Log level is warning.</summary>
            WARN
        }

        /// <summary>Handles all the logging for the MUD engine.</summary>
        public static class MudLogger
        {
            /// <summary>The main logger.</summary>
            private static readonly ILog Logger = LogManager.GetLogger(typeof(MudLogger));

            /// <summary>Initializes static members of the <see cref="MudLogger"/> class.</summary>
            static MudLogger()
            {
                XmlConfigurator.Configure();
            }

            /// <summary>Writes to the log.</summary>
            /// <param name="logLevel">The log level.</param>
            /// <param name="log">The log message.</param>
            public static void WriteLog(LogLevel logLevel, string log)
            {
                if (logLevel.Equals(LogLevel.DEBUG))
                {
                    Logger.Debug(log);
                }
                else if (logLevel.Equals(LogLevel.ERROR))
                {
                    Logger.Error(log);
                }
                else if (logLevel.Equals(LogLevel.FATAL))
                {
                    Logger.Fatal(log);
                }
                else if (logLevel.Equals(LogLevel.INFO))
                {
                    Logger.Info(log);
                }
                else if (logLevel.Equals(LogLevel.WARN))
                {
                    Logger.Warn(log);
                }
            }
        }
    }
}