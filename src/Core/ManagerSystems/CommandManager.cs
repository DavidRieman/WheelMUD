//-----------------------------------------------------------------------------
// <copyright file="CommandManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of all commands.
//   Created: August 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Timers;
    using WheelMUD.Actions;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>
    /// High level manager that provides tracking and global collection of all commands.
    /// </summary>
    public class CommandManager : ManagerSystem, IRecomposable
    {
        /// <summary>The master command list contains all aliases of all commands.</summary>
        private readonly Dictionary<string, Command> masterCommandList = new Dictionary<string, Command>();

        /// <summary>The master primary command list contains only the primary aliases of commands.</summary>
        private readonly Dictionary<string, Command> primaryCommandList = new Dictionary<string, Command>();

        /// <summary>The dictionary of delayed commands.</summary>
        private readonly Dictionary<Timer, ScriptingCommand> waitingCommands = new Dictionary<Timer, ScriptingCommand>();

        /// <summary>The singleton instance synchronization locking object.</summary>
        private static readonly object instanceLockObject = new object();

        /// <summary>The synchronization locking object for waiting commands.</summary>
        private static readonly object waitingCommandsLockObject = new object();

        /// <summary>The singleton instance of this class.</summary>
        private static CommandManager instance;

        /// <summary>The queue of actions yet to be processed.</summary>
        private Queue<ActionInput> actionQueue = new Queue<ActionInput>();

        private List<CommandProcessor> commandProcessors = new List<CommandProcessor>();

        private List<Type> commandTypes = new List<Type>();

        /// <summary>
        /// Prevents a default instance of the <see cref="CommandManager"/> class from being created.
        /// </summary>
        private CommandManager()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CommandManager Instance
        {
            get
            {
                // Using if-lock-if pattern to avoid locks for most cases yet create only once instance in early init.
                if (instance == null)
                {
                    lock (instanceLockObject)
                    {
                        if (instance == null)
                        {
                            instance = new CommandManager();
                            instance.Recompose();
                        }
                    }
                }

                return instance;
            }
        }

        public Dictionary<string, Command> MasterCommandList
        {
            get { return this.masterCommandList; }
        }

        [ImportMany]
        private List<GameAction> Commands { get; set; }

        /// <summary>
        /// Recompose the subcomponents of this CommandManager
        /// </summary>
        public void Recompose()
        {
            DefaultComposer.Container.ComposeParts(this);

            lock (this.commandTypes)
            {
                this.commandTypes.Clear();

                var actionTypes = from c in this.Commands select c.GetType();
                foreach (Type type in actionTypes)
                {
                    // Find the description of this command.
                    object[] descripts = type.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                    object[] examples = type.GetCustomAttributes(typeof(ActionExampleAttribute), false);
                    string descript = (from d in descripts select ((ActionDescriptionAttribute)d).Description).FirstOrDefault();
                    string example = (from e in examples select ((ActionExampleAttribute)e).Example).FirstOrDefault();

                    // All alias variations are going to reference the same, single command.
                    // The command defaults to let nobody run it, unless we find an attribute for it.
                    Command command = new Command(type, descript, example, SecurityRole.none);

                    // Find out what security roles are associated with that action.
                    // By default, the securityRole will be 'none' so if no security attribute 
                    // was specified, then nobody will be able to execute it.
                    object[] roles = type.GetCustomAttributes(typeof(ActionSecurityAttribute), false);
                    command.SecurityRole = (from r in roles select ((ActionSecurityAttribute)r).Role).FirstOrDefault();

                    foreach (object obj in type.GetCustomAttributes(typeof(ActionPrimaryAliasAttribute), false))
                    {
                        string alias = ((ActionPrimaryAliasAttribute)obj).Alias;
                        command.PrimaryAlias = true;
                        command.Category = ((ActionPrimaryAliasAttribute)obj).Category;

                        this.primaryCommandList.Add(alias, command);
                        this.masterCommandList.Add(alias, command);
                    }

                    // For every alias associated with that action, store a reference to the Command 
                    // that represents the action and allows the user to invoke an instance.
                    // If using ActionAliasAttribute that means this is not considered a PrimaryAlias...find the primary
                    // and add it to this.  While we're at it, add this to the primary.
                    foreach (object obj in type.GetCustomAttributes(typeof(ActionAliasAttribute), false))
                    {
                        string alias = ((ActionAliasAttribute)obj).Alias;
                        Command secondaryCommand = new Command(type, command.SecurityRole);
                        secondaryCommand.PrimaryAlias = false;
                        secondaryCommand.Category = ((ActionAliasAttribute)obj).Category;
                        this.masterCommandList.Add(alias, secondaryCommand);
                    }
                }
            }
        }

        /// <summary>Starts this system.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            // @@@ TODO: Test > 1, then allow total command processors to be configurable.
            int totalCommandProcessors = 1;
            for (int i = 0; i < totalCommandProcessors; i++)
            {
                var commandProcessor = new CommandProcessor(this);
                commandProcessor.Start();
                this.commandProcessors.Add(commandProcessor);
            }

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            foreach (var commandProcessor in this.commandProcessors)
            {
                commandProcessor.Stop();
            }

            this.commandProcessors.Clear();

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Gets a list of commands that the controller has permissions to execute.</summary>
        /// <param name="controller">The controller to query for permissions.</param>
        /// <returns>A list of commands that the controller has permissions to execute.</returns>
        public List<Command> GetCommandsForController(IController controller)
        {
            List<Command> commands = new List<Command>();
            foreach (Command command in this.primaryCommandList.Values)
            {
                // @@@ IFF this controller/entity has sufficient privileges to use the command, add it
                commands.Add(command);
            }

            return commands;
        }

        /// <summary>
        /// Places an action onto the queue for execution.
        /// </summary>
        /// <param name="actionInput">The action input to enqueue.</param>
        public void EnqueueAction(ActionInput actionInput)
        {
            if (actionInput != null)
            {
                lock (this.actionQueue)
                {
                    this.actionQueue.Enqueue(actionInput);
                }
            }
        }

        /// <summary>
        /// Removes an action from the queue for execution. Should only be used by CommandProcessors.
        /// </summary>
        /// <returns>The next action from the queue.</returns>
        internal ActionInput DequeueAction()
        {
            lock (this.actionQueue)
            {
                if (this.actionQueue.Count <= 0)
                {
                    return null;
                }

                return this.actionQueue.Dequeue();
            }
        }

        [ExportSystem]
        public class CommandManagerExporter : SystemExporter
        {
            public override ISystem Instance
            {
                get { return CommandManager.Instance; }
            }

            public override Type SystemType
            {
                get { return typeof(CommandManager); }
            }
        }
    }
}