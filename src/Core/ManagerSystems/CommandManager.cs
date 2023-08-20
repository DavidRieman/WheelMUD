//-----------------------------------------------------------------------------
// <copyright file="CommandManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of all commands.</summary>
    public class CommandManager : ManagerSystem, IRecomposable
    {
        /// <summary>The master primary command list contains only the primary aliases of commands.</summary>
        private Dictionary<string, Command> primaryCommandList = new();

        /// <summary>The queue of actions yet to be processed.</summary>
        private readonly Queue<ActionInput> actionQueue = new();

        /// <summary>The list of CommandProcessor workers.</summary>
        private readonly List<CommandProcessor> commandProcessors = new();

        /// <summary>Prevents a default instance of the <see cref="CommandManager"/> class from being created.</summary>
        private CommandManager()
        {
            Recompose();
        }

        /// <summary>Gets the singleton instance of the <see cref="CommandManager"/> class.</summary>
        public static CommandManager Instance { get; } = new CommandManager();

        /// <summary>Gets the master command list, containing all aliases of all commands.</summary>
        public Dictionary<string, Command> MasterCommandList { get; private set; } = new Dictionary<string, Command>();

        /// <summary>Gets or sets, through MEF composition, the available GameAction classes.</summary>
        [ImportMany]
        private Lazy<GameAction, CoreExports.GameAction>[] GameActions { get; set; }

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

            var actionTypes = DefaultComposer.GetTypes(GameActions);
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

            // Avoid replacing our lists while another thread is iterating them.
            // TODO: Exposing MasterCommandList this way is very likely NOT thread-safe. Change to a lock-protected getter?
            lock (this)
            {
                primaryCommandList = newPrimaryCommandList;
                MasterCommandList = newMasterCommandList;
            }
        }

        /// <summary>Starts this system.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            // TODO: Test > 1, then allow total command processors to be configurable.
            //  This will take a significant effort to work out any race conditions which may leave
            //  things with multiple parents (essentially duplication bugs), deadlocks should we try
            //  to use multiple locks for transaction-like parenting changes, and so on. (We may need
            //  to end up using a fairly global lock for any Parent changes, etc. Note that the Thing
            //  locks in place now on getters and setters are definitely not effective / sufficient.)
            int totalCommandProcessors = 1;
            for (int i = 0; i < totalCommandProcessors; i++)
            {
                var commandProcessor = new CommandProcessor(this);
                commandProcessor.Start();
                commandProcessors.Add(commandProcessor);
            }

            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            foreach (var commandProcessor in commandProcessors)
            {
                commandProcessor.Stop();
            }

            commandProcessors.Clear();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Gets a list of commands that the controller has permissions to execute.</summary>
        /// <param name="controller">The controller to query for permissions.</param>
        /// <returns>A list of commands that the controller has permissions to execute.</returns>
        public List<Command> GetCommandsFor(Thing actor)
        {
            lock (this)
            {
                // TODO: IFF this controller/entity has privileges to use the command, add it. (LINQ query, ToList?)
                // TODO: We could cache any built map of privelege-set to commands-list (so long as we invalidate
                //       it whenever we recompose commands from MEF.)
                List<Command> commands = new();
                foreach (Command command in primaryCommandList.Values)
                {
                    commands.Add(command);
                }

                return commands;
            }
        }

        public IEnumerable<ContextCommand> GetContextCommands(Thing sender, string alias)
        {
            // TODO: Ascertain the ACTUAL sender's specific permissions, so we can check for fullAdmin, fullBuilder, and
            //       so on, instead of assuming just 'SecurityRole.player' (SEE ALSO CommandGuard.cs for another case...)
            SecurityRole playerRoles = SecurityRole.player | SecurityRole.minorBuilder | SecurityRole.fullBuilder | SecurityRole.minorAdmin | SecurityRole.fullAdmin;
            return GetPossibleContextCommands(sender, alias, playerRoles).Where(cmd => cmd != null);
        }

        private IEnumerable<ContextCommand> GetPossibleContextCommands(Thing sender, string alias, SecurityRole senderRoles)
        {
            // The order we return commands establishes the priority order, with the highest priority returned first.
            // Check the sender's current parent (like the "room") for such a context command applicable to its children.
            if (sender.Parent.Commands.TryGetValue(alias, out ContextCommand cmd) &&
                (cmd.Availability & ContextAvailability.ToChildren) != ContextAvailability.ToNone &&
                (cmd.SecurityRole & senderRoles) != SecurityRole.none)
            {
                yield return cmd;
            }

            // Else check the sender itself for such a context command applicable to use itself (such as a context command
            // granted by a Behavior or Effect currently on the parent).
            if (sender.Commands.TryGetValue(alias, out cmd) &&
                (cmd.Availability & ContextAvailability.ToSelf) != ContextAvailability.ToNone &&
                (cmd.SecurityRole & senderRoles) != SecurityRole.none)
            {
                yield return cmd;
            }

            // Else check siblings of the sender (like other Things in the Room) for a context command applicable to its siblings.
            foreach (Thing sibling in sender.Parent.Children)
            {
                if (sibling != sender &&
                    sibling.Commands.TryGetValue(alias, out cmd) &&
                    (cmd.Availability & ContextAvailability.ToSiblings) != ContextAvailability.ToNone &&
                    (cmd.SecurityRole & senderRoles) != SecurityRole.none)
                {
                    yield return cmd;
                }
            }

            // Else check children of the sender (like inventory items that grant commands/powers) for such a context command
            // applicable to their parent. (Note that this needs to work for children with MultipleParentsBehavior too.)
            foreach (Thing child in sender.Children)
            {
                if (child.Commands.TryGetValue(alias, out cmd) &&
                    (cmd.Availability & ContextAvailability.ToParent) != ContextAvailability.ToNone &&
                    (cmd.SecurityRole & senderRoles) != SecurityRole.none)
                {
                    yield return cmd;
                }
            }
        }

        /// <summary>Places an action onto the queue for execution.</summary>
        /// <param name="actionInput">The action input to enqueue.</param>
        public void EnqueueAction(ActionInput actionInput)
        {
            Debug.Assert(actionInput != null);
            lock (actionQueue)
            {
                actionQueue.Enqueue(actionInput);
            }
        }

        /// <summary>Removes an action from the queue for execution. Should only be used by CommandProcessors.</summary>
        /// <returns>The next action from the queue.</returns>
        internal ActionInput DequeueAction()
        {
            lock (actionQueue)
            {
                if (actionQueue.Count <= 0)
                {
                    return null;
                }

                return actionQueue.Dequeue();
            }
        }

        /// <summary>Registers the <see cref="CommandManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="CommandManager"/> system through MEF.</remarks>
        [CoreExports.System(0)]
        public class CommandManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance => CommandManager.Instance;

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType => typeof(CommandManager);
        }
    }
}