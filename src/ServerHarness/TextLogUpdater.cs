//-----------------------------------------------------------------------------
// <copyright file="TextLogUpdater.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities.Interfaces;

namespace ServerHarness
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>Text Log Updater. Logs notifications to a plain text file.</summary>
    public class TextLogUpdater : ISuperSystemSubscriber
    {
        /// <summary>The text stream writer for the log file to be appended.</summary>
        private StreamWriter writer;

        /// <summary>Initializes a new instance of the TextLogUpdater class.</summary>
        /// <param name="textLogFilePath">The log file path to append text messages to.</param>
        public TextLogUpdater(string textLogFilePath)
        {
            writer = new StreamWriter(textLogFilePath, true, Encoding.ASCII);
        }

        /// <summary>Finalizes an instance of the TextLogUpdater class.</summary>
        ~TextLogUpdater()
        {
            Dispose();
        }

        /// <summary>Dispose of any resources used by this TextLogUpdater.</summary>
        public void Dispose()
        {
            if (writer != null)
            {
                try
                {
                    writer.Dispose();
                    writer = null;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    Console.WriteLine(msg);
                }
            }
        }

        /// <summary>Notify user of the specified message via logging to a text file.</summary>
        /// <param name="message">The message to pass along.</param>
        public void Notify(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}
