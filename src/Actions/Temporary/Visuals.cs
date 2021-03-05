// -----------------------------------------------------------------------
// <copyright file="Visuals.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using WheelMUD.Interfaces;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions.Temporary
{
    /// <summary>
    /// Command to add, remove, or display the "visuals" associated with a 
    /// room. Visuals are like pseudo-items that can be looked at, e.g. to
    /// provide clues or just to enhance the room.
    /// </summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("visuals", CommandCategory.Temporary)]
    [ActionDescription("Add or remove a visual item/description to the current room.")]
    [ActionExample("Examples:\r\n  visuals add tree The tree is tall.\r\n  visuals remove tree\r\n  visuals show")]
    [ActionSecurity(SecurityRole.minorBuilder)]
    public class Visuals : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Number of arguments supplied to the action.</summary>
        private int argCount;

        /// <summary>The room where the sender is located.</summary>
        private RoomBehavior room;

        /// <summary>Name of the room where the sender is located. Cached for convenience.</summary>
        private string roomName;

        /// <summary>Id of the room where the sender is located. Cached for convenience.</summary>
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
            if (!(actionInput.Controller is Session session)) return;
            
            // Contextual message text to be supplied based on the action below
            var response = new ContextualString(actionInput.Controller.Thing, room.Parent);

            if (command == "add")
            {
                // Add or update the description
                room.Visuals[visualName] = visualDescription;
                response.ToOriginator = $"Visual '{visualName}' added/updated on room {roomName} [{roomId}].";

                //// TODO: Save change
                //room.Save();
            }
            else if (command == "remove")
            {
                if (room.Visuals.ContainsKey(visualName))
                {
                    room.Visuals.Remove(visualName);

                    response.ToOriginator = $"Visual '{visualName}' removed from room {roomName} [{roomId}]";
                }

                //// TODO: Save change
                //room.Save();
            }
            else if (command == "show")
            {
                var output = new OutputBuilder(session.TerminalOptions);

                if (room.Visuals.Count > 0)
                {
                    output.AppendLine($"Visuals for {roomName} [{roomId}]:");

                    foreach (var name in room.Visuals.Keys)
                    {
                        output.AppendLine($"  {name}: {room.Visuals[name]}");
                    }
                }
                else
                {
                    output.AppendLine($"No visuals found for {roomName} [{roomId}].");
                }

                actionInput.Controller.Write(output.ToString());

                // No need to raise event.
                return;
            }

            var message = new SensoryMessage(SensoryType.Sight, 100, response);
            var evt = new GameEvent(actionInput.Controller.Thing, message);
            actionInput.Controller.Thing.Eventing.OnMiscellaneousEvent(evt, EventScope.SelfDown);
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }
            
            if (!(actionInput.Controller is Session session)) return null;

            PreprocessInput(actionInput);

            // Ensure the sender of this command is currently located in a valid room.
            if (room == null)
            {
                return "You must be located in a valid room to change its visuals.";
            }

            var usageText = new OutputBuilder(session.TerminalOptions);
            usageText.AppendLine("Usage:");
            usageText.AppendLine("visuals add <name> <description>");
            usageText.AppendLine("visuals remove <name>");
            usageText.AppendLine("visuals show");

            switch (command)
            {
                case "add":
                    // Ensure "add" syntax includes both a name and description.
                    return string.IsNullOrEmpty(visualName) || string.IsNullOrEmpty(visualDescription)
                               ? usageText.ToString()
                               : null;

                case "remove":
                    // Ensure "remove" syntax includes a name but nothing else.
                    return argCount != 2 ? usageText.ToString() : null;

                case "show":
                    // Ensure "show" syntax has no additional arguments.
                    return argCount > 1 ? usageText.ToString() : null;

                default:
                    // Handle case for "visuals aalkdsfj lkajf" etc.
                    return usageText.ToString();
            }
        }

        /// <summary>Populate the private fields used by the Guards() and Execute() methods.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        private void PreprocessInput(ActionInput actionInput)
        {
            argCount = actionInput.Params.Length;

            // Location of the sender of the command.
            var location = actionInput.Controller.Thing.Parent;

            if (location.HasBehavior<RoomBehavior>())
            {
                room = location.Behaviors.FindFirst<RoomBehavior>();
                roomName = room.Parent.Name;
                roomId = room.Parent.Id;
            }

            // "visuals" with no arguments will default to "visuals show".
            if (argCount == 0)
            {
                command = "show";
            }

            // Name of the sub-command, i.e. "add", "remove", or "show".
            if (argCount > 0)
            {
                var firstParam = actionInput.Params[0].ToLower();
                switch (firstParam)
                {
                    case "add":
                        command = "add";
                        break;
                    case "remove":
                        command = "remove";
                        break;
                    case "show":
                        command = "show";
                        break;
                    default:
                        // command remains null, and there is no more processing to do.
                        return;
                }
            }

            // Name of the visual being added or removed.
            if (argCount > 1)
            {
                visualName = actionInput.Params[1].ToLower();
            }

            // The remainder of the command text is assumed to be the description.
            if (argCount > 2)
            {
                var tail = actionInput.Tail[command.Length..].TrimStart();
                tail = tail[visualName.Length..].TrimStart();
                visualDescription = tail;
            }
        }
    }
}
