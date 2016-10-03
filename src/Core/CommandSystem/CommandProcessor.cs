//-----------------------------------------------------------------------------
// <copyright file="CommandProcessor.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: December 2010 by Karak, based largely on the old CommandQueue.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using WheelMUD.CommandSystem;
    using WheelMUD.Interfaces;

    /// <summary>A command processor, which houses a thread to process commands.</summary>
    public class CommandProcessor : ISubSystem
    {
        /// <summary>The worker thread.</summary>
        private Thread workerThread;

        /// <summary>The host of this SubSystem.</summary>
        private ISubSystemHost host;

        /// <summary>Initializes a new instance of the CommandProcessor class.</summary>
        /// <param name="host">The host of this CommandProcessor SubSystem.</param>
        public CommandProcessor(ISubSystemHost host)
        {
            this.host = host;
        }

        /// <summary>Start this CommandProcessor SubSystem.</summary>
        public void Start()
        {
            this.host.UpdateSubSystemHost(this, "Starting...");
            this.workerThread = new Thread(this.ProcessCommandsThread);
            this.workerThread.Start();
        }

        /// <summary>Stop this CommandProcessor SubSystem.</summary>
        public void Stop()
        {
            this.host.UpdateSubSystemHost(this, "Stopping...");
            this.workerThread.Abort();
            this.workerThread.Join(5000);
            this.workerThread = null;
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            this.host = sender;
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string msg)
        {
            this.host.UpdateSubSystemHost(this, msg);
        }

        /// <summary>Method executed as the worker thread to perform the work indefinitely.</summary>
        private void ProcessCommandsThread()
        {
            while (true)
            {
                ActionInput action = CommandManager.Instance.DequeueAction();
                if (action != null)
                {
                    this.TryExecuteAction(action);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>Try to execute the specified action input as a command.</summary>
        /// <param name="actionInput">The action input to try to execute.</param>
        private void TryExecuteAction(ActionInput actionInput)
        {
            try
            {
                ScriptingCommand command = CommandCreator.Instance.Create(actionInput);
                if (command == null)
                {
                    return;
                }

                // Verify the user has permissions to use this command.
                string guardsErrorMessage = CommandGuardHelpers.VerifyCommandPermission(command);
                if (guardsErrorMessage == null)
                {
                    guardsErrorMessage = (string)command.GuardsDelegate.DynamicInvoke(actionInput);
                }

                // Verify that the other command-specific guards are passed.
                if (guardsErrorMessage == null)
                {
                    // Execute the command if we passed all the guards.
                    command.ExecuteDelegate.DynamicInvoke(actionInput);
                }
                else
                {
                    // Otherwise display what went wrong to the user of the action.
                    IController controller = actionInput.Controller;
                    controller.Write(guardsErrorMessage);
                }
            }
            catch (Exception ex)
            {
                // Most of our exceptions should be TargetInvocationExceptions but we're going to
                // handle them basically the same way as others, except that we only care about the
                // inner exception (what actually went wrong, since we know we're doing invokes here).
                if (ex is TargetInvocationException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                // In order to isolate command-specific issues, we're going to trap the exception, log
                // the details, and kill that command.  (Other commands and the game itself should be 
                // able to continue through such situations.)
                // Start gathering info, but carefully to avoid causing potential further exceptions here.
                IController controller = actionInput.Controller;
#if DEBUG
                Thing thing = controller != null ? controller.Thing : null;
                string thingName = thing != null ? thing.Name : "[null]";
                string thingID = thing != null ? thing.ID.ToString() : "[null]";
                string fullCommand = actionInput != null ? actionInput.FullText : "[null]";
                string format = "Exception encountered for command: {1}{0}From thing: {2} (ID: {3}){0}{4}";
                string message = string.Format(format, Environment.NewLine, fullCommand, thingName, thingID, ex.ToDeepString());
#endif

                // If the debugger is attached, we probably want to break now in order to better debug 
                // the issue closer to where it occurred; if your debugger broke here you may want to 
                // look at the stack trace to see where the exception originated.
                if (Debugger.IsAttached)
                {
                    string stackTrace = ex.StackTrace;
                    Debugger.Break();
                }

                // @@@ FIX: this.host..UpdateSubSystemHost(this, message);
                if (controller != null)
                {
#if DEBUG
                    controller.Write(message);
#else
                    controller.Write("An error occured processing your command.");
#endif
                }
            }
        }
    }
}