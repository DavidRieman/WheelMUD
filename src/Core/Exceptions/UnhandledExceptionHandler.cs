//-----------------------------------------------------------------------------
// <copyright file="UnhandledExceptionHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A fairly generic unhandled exception handler.
    /// </summary>
    public class UnhandledExceptionHandler
    {
        private Action<string> notifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionHandler"/> class.
        /// </summary>
        /// <param name="notifier">An action that handles exception message strings in some way, e.g. print to the console.</param>
        public UnhandledExceptionHandler(Action<string> notifier)
        {
            this.notifier = notifier;

            // Set up the global unhandled exception handler for this app domain.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // If the debugger is attached, rethrow the exception so the debugger can assess the issue
            // at the call site with the appropriate info right there.
            Exception ex = e.ExceptionObject as Exception;
            if (Debugger.IsAttached)
            {
                throw ex;
            }

            this.notifier(ex.ToDeepString());
        }
    }
}