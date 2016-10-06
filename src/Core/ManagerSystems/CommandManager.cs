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
    using WheelMUD.Actions;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>High level manager that provides tracking and global collection of all commands.</summary>
    public class CommandManager : ManagerSystem, IRecomposable
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly CommandManager SingletonInstance = new CommandManager();

        /// <summary>The master command list contains all aliases of all commands.</summary>
        private Dictionary<string, Command> masterCommandList = new Dictionary<string, Command>();

        /// <summary>The master primary command list contains only the primary aliases of commands.</summary>
        private Dictionary<string, Command> primaryCommandList = new Dictionary<string, Command>();

        /// <summary>The queue of actions yet to be processed.</summary>
        private Queue<ActionInput> actionQueue = new Queue<ActionInput>();

        /// <summary>The list of CommandProcessor workers.</summary>
        private List<CommandProcessor> commandProcessors = new List<CommandProcessor>();
        
        /// <summary>Prevents a default instance of the <see cref="CommandManager"/> class from being created.</summary>
        private CommandManager()
        {
            this.Recompose();
        }

        /// <summary>Gets the singleton instance of the <see cref="CommandManager"/> class.</summary>
        public static CommandManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets the master command list.</summary>
        public Dictionary<string, Command> MasterCommandList
        {
            get { return this.masterCommandList; }
        }

        /// <summary>Gets or sets, through MEF composition, the available GameAction classes.</summary>
        [ImportMany]
        private List<GameAction> Commands { get; set; }

        /// <summary>Recompose the <see cref="CommandManager"/> system.</summary>
        public void Recompose()
        {
            DefaultComposer.Container.ComposeParts(this);

            // During non-reboot updates (recomposition): To minimize impact to the running application and players, the
            // existing command lists will remain intact until preparing the new commands list is complete. Then all the
            // commands will switch over by replacing the command list references. Thus, the old code will continue to
            // be used until all new commands are ready, at which point all processed commands start using the new code.
            // Old MasterCommandList references and old command objects themselves should garbage collect eventually, so
            // long as nothing is holding references to them incorrectly.
            var newPrimaryCommandList = new Dictionary<string, Command>();
            var newMasterCommandList = new Dictionary<string, Command>();

            var actionTypes = from c in this.Commands select c.GetType();
            foreach (Type type in actionTypes)
            {
                // Find the description of this command.
                object[] descripts = type.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                object[] examples = type.GetCustomAttributes(typeof(ActionExampleAttribute), false);
                string descript = (from d in descripts select (d as ActionDescriptionAttribute).Description).FirstOrDefault();
                string example = (from e in examples select (e as ActionExampleAttribute).Example).FirstOrDefault();

                // All alias variations are going to reference the same, single command.
                // The command defaults to let nobody run it, unless we find an attribute for it.
                var command = new Command(type, descript, example, SecurityRole.none);

                // Find out what security roles are associated with that action.
                // By default, the securityRole will be 'none' so if no security attribute 
                // was specified, then nobody will be able to execute it.
                object[] roles = type.GetCustomAttributes(typeof(ActionSecurityAttribute), false);
                command.SecurityRole = (from r in roles select (r as ActionSecurityAttribute).Role).FirstOrDefault();

                foreach (object obj in type.GetCustomAttributes(typeof(ActionPrimaryAliasAttribute), false))
                {
                    var attr = obj as ActionPrimaryAliasAttribute;
                    var alias = attr.Alias;
                    command.Category = attr.Category;
                    command.PrimaryAlias = true;
                    newPrimaryCommandList.Add(alias, command);
                    newMasterCommandList.Add(alias, command);
                }

                // For every alias associated with that action, store a reference to the Command 
                // that represents the action and allows the user to invoke an instance.
                // If using ActionAliasAttribute that means this is not considered a PrimaryAlias...find the primary
                // and add it to this.  While we're at it, add this to the primary.
                foreach (object obj in type.GetCustomAttributes(typeof(ActionAliasAttribute), false))
                {
                    var attr = obj as ActionAliasAttribute;
                    var alias = attr.Alias;
                    var secondaryCommand = new Command(type, command.SecurityRole)
                    {
                        Category = attr.Category,
                        PrimaryAlias = false
                    };
                    newMasterCommandList.Add(alias, secondaryCommand);
                }
            }

            this.primaryCommandList = newPrimaryCommandList;
            this.masterCommandList = newMasterCommandList;
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

        /// <summary>Places an action onto the queue for execution.</summary>
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

        /// <summary>Removes an action from the queue for execution. Should only be used by CommandProcessors.</summary>
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

        /// <summary>Registers the <see cref="CommandManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="CommandManager"/> system through MEF.</remarks>
        [ExportSystem]
        public class CommandManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance
            {
                get { return CommandManager.Instance; }
            }

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType
            {
                get { return typeof(CommandManager); }
            }
        }
    }
}