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

    /// <summary>A fairly generic unhandled exception handler.</summary>
    public class UnhandledExceptionHandler
    {
        /// <summary>The active exception notifier.</summary>
        private static Action<string> notifier;

        /// <summary>Registers a notifier for receiving unhandled exception details.</summary>
        /// <remarks>
        /// Once called, the UnhandledExceptionHandler tries not to let unhandled exceptions crash the program.
        /// If the debugger is attached, the exception will be re-thrown to let the debugger handle it.  Otherwise,
        /// unhandled exceptions will get logged to the assigned notifier (such as a printer to the console output
        /// if running the server in console mode, or perhaps an event log during service mode, and so on).
        /// </remarks>
        /// <param name="newNotifier">An action that handles exception message strings in some way, e.g. print to the console.</param>
        public static void Register(Action<string> newNotifier)
        {
            notifier = newNotifier;

            // Set up the global unhandled exception handler for this app domain, or re-register it without 
            // doubling up, in the case of Register being called a second time (IE with a new notifier).
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>Unhandled exception handler for the current domain.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The unhandled exception event arguments.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // If the debugger is attached, rethrow the exception so the debugger can assess the issue
            // at the call site with the appropriate info right there.
            Exception ex = e.ExceptionObject as Exception;
            if (Debugger.IsAttached)
            {
                throw ex;
            }

            notifier(ex.ToDeepString());
        }
    }
}