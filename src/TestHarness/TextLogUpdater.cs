//-----------------------------------------------------------------------------
// <copyright file="TextLogUpdater.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Logs notifications to a plain text file.
// </summary>
//-----------------------------------------------------------------------------

namespace TestHarness
{
    using System;
    using System.IO;
    using System.Text;
    using WheelMUD.Interfaces;

    /// <summary>Text Log Updater.</summary>
    public class TextLogUpdater : ISuperSystemSubscriber
    {
        /// <summary>The text stream writer for the log file to be appended.</summary>
        private StreamWriter writer;
        
        /// <summary>Initializes a new instance of the TextLogUpdater class.</summary>
        /// <param name="textLogFilePath">The log file path to append text messages to.</param>
        public TextLogUpdater(string textLogFilePath)
        {
            this.writer = new StreamWriter(textLogFilePath, true, Encoding.ASCII);
        }

        /// <summary>Finalizes an instance of the TextLogUpdater class.</summary>
        ~TextLogUpdater()
        {
            this.Dispose();
        }

        /// <summary>Dispose of any resources used by this TextLogUpdater.</summary>
        public void Dispose()
        {
            if (this.writer != null)
            {
                try
                {
                    this.writer.Dispose();
                    this.writer = null;
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
            this.writer.WriteLine(message);
            this.writer.Flush();
        }
    }
}
