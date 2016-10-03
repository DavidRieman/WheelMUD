// -----------------------------------------------------------------------
// <copyright file="Visuals.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created by: James
//   Date      : May 25, 2012
//   Command to add, remove, or display the "visuals" associated with a
//   room. Visuals are like pseudo-items that can be looked at, e.g. to
//   provide clues or just to enhance the room.
// </summary>
// -----------------------------------------------------------------------

namespace WheelMUD.Actions.Temporary
{
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>
    /// Command to add, remove, or display the "visuals" associated with a 
    /// room. Visuals are like pseudo-items that can be looked at, e.g. to
    /// provide clues or just to enhance the room.
    /// </summary>
    [ExportGameAction]
    [ActionPrimaryAlias("visuals", CommandCategory.Temporary)]
    [ActionDescription("Add or remove a visual item/description to the current room.")]
    [ActionExample("Examples:\r\n  visuals add tree The tree is tall.\r\n  visuals remove tree\r\n  visuals show")]
    [ActionSecurity(SecurityRole.minorBuilder)]
    public class Visuals : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Origin of the action.</summary>
        private IController sender;

        /// <summary>Number of arguments supplied to the action.</summary>
        private int argCount;

        /// <summary>The room where the sender is located.</summary>
        private RoomBehavior room;

        /// <summary>Name of the room where the sender is located. Cached for convenience.</summary>
        private string roomName;

        /// <summary>ID of the room where the sender is located. Cached for convenience.</summary>
        private string roomId;

        /// <summary>The visuals command, i.e. "add", "remove", or "show".</summary>
        private string command;

        /// <summary>Name of the visual being modified.</summary>
        private string visualName;

        /// <summary>Description of the visual, if one is being added.</summary>
        private string visualDescription;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Contextual message text to be supplied based on the action below
            var response = new ContextualString(this.sender.Thing, this.room.Parent);

            if (this.command == "add")
            {
                // Add or update the description
                this.room.Visuals[this.visualName] = this.visualDescription;
                response.ToOriginator = string.Format("Visual '{0}' added/updated on room {1} [{2}].", this.visualName, this.roomName, this.roomId);

                //// TODO: Save change
                this.room.Save();
            }
            else if (this.command == "remove")
            {
                if (this.room.Visuals.ContainsKey(this.visualName))
                {
                    this.room.Visuals.Remove(this.visualName);

                    response.ToOriginator = string.Format("Visual '{0}' removed from room {1} [{2}]", this.visualName, this.roomName, this.roomId);
                }

                //// TODO: Save change
                this.room.Save();
            }
            else if (this.command == "show")
            {
                var output = new StringBuilder();

                if (this.room.Visuals.Count > 0)
                {
                    output.AppendLine(string.Format("Visuals for {0} [{1}]:", this.roomName, this.roomId)).AppendLine();

                    foreach (var name in this.room.Visuals.Keys)
                    {
                        output.AppendLine(string.Format("  {0}: {1}", name, this.room.Visuals[name]));
                    }
                }
                else
                {
                    output.Append(string.Format("No visuals found for {0} [{1}].", this.roomName, this.roomId));
                }

                //// HACK: Using sender.Write() for now to avoid the ViewEngine stripping newlines.
                this.sender.Write(output.ToString());

                // No need to raise event.
                return;
            }

            var message = new SensoryMessage(SensoryType.Sight, 100, response);
            var evt = new GameEvent(this.sender.Thing, message);
            this.sender.Thing.Eventing.OnMiscellaneousEvent(evt, EventScope.SelfDown);
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            this.PreprocessInput(actionInput);

            // Ensure the sender of this command is currently located in a valid room.
            if (this.room == null)
            {
                return "You must be located in a valid room to change its visuals.";
            }

            string usageText = "Usage:\r\n  visuals add <name> <description>\r\n  visuals remove <name>\r\n  visuals show";

            switch (this.command)
            {
                case "add":
                    // Ensure "add" syntax includes both a name and description.
                    return (string.IsNullOrEmpty(this.visualName) || string.IsNullOrEmpty(this.visualDescription))
                               ? usageText
                               : null;

                case "remove":
                    // Ensure "remove" syntax includes a name but nothing else.
                    return (this.argCount != 2) ? usageText : null;

                case "show":
                    // Ensure "show" syntax has no additional arguments.
                    return (this.argCount > 1) ? usageText : null;
                
                default:
                    // Handle case for "visuals aalkdsfj lkajf" etc.
                    return usageText;
            }
        }

        /// <summary>Populate the private fields used by the Guards() and Execute() methods.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        private void PreprocessInput(ActionInput actionInput)
        {
            this.sender = actionInput.Controller;

            this.argCount = actionInput.Params.Length;

            // Location of the sender of the command.
            var location = this.sender.Thing.Parent;

            if (location.HasBehavior<RoomBehavior>())
            {
                this.room = location.Behaviors.FindFirst<RoomBehavior>();
                this.roomName = this.room.Parent.Name;
                this.roomId = this.room.Parent.ID;
            }

            // "visuals" with no arguments will default to "visuals show".
            if (this.argCount == 0)
            {
                this.command = "show";
            }

            // Name of the sub-command, i.e. "add", "remove", or "show".
            if (this.argCount > 0)
            {
                string firstParam = actionInput.Params[0].ToLower();
                switch (firstParam)
                {
                    case "add":
                        this.command = "add";
                        break;
                    case "remove":
                        this.command = "remove";
                        break;
                    case "show":
                        this.command = "show";
                        break;
                    default:
                        // this.command remains null, and there is no more processing to do.
                        return;
                }
            }

            // Name of the visual being added or removed.
            if (this.argCount > 1)
            {
                this.visualName = actionInput.Params[1].ToLower();
            }

            // The remainder of the command text is assumed to be the description.
            if (this.argCount > 2)
            {
                string tail = actionInput.Tail.Substring(this.command.Length).TrimStart();
                tail = tail.Substring(this.visualName.Length).TrimStart();
                this.visualDescription = tail;
            }
        }
    }
}
