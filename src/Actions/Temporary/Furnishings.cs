// -----------------------------------------------------------------------
// <copyright file="Furnishings.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions.Temporary
{
    /// <summary>
    /// Command to add, remove, or display the "furnishings" associated with a room.
    /// Furnishings are like pseudo-items that can be looked at, e.g. to provide clues or just to enhance the room.
    /// </summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("furnishings", CommandCategory.Temporary)]
    [ActionDescription("Add or remove a furnishing item/description to the current room.")]
    [ActionExample("Examples:\r\n  furnishings add tree The tree is tall.\r\n  furnishings remove tree\r\n  furnishings show")]
    [ActionSecurity(SecurityRole.minorBuilder)]
    public class Furnishings : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>() { CommonGuards.InitiatorMustBeAPlayer };

        /// <summary>Number of arguments supplied to the action.</summary>
        private int argCount;

        /// <summary>The room where the sender is located.</summary>
        private RoomBehavior room;

        /// <summary>Name of the room where the sender is located. Cached for convenience.</summary>
        private string roomName;

        /// <summary>Id of the room where the sender is located. Cached for convenience.</summary>
        private string roomId;

        /// <summary>The furnishings command, i.e. "add", "remove", or "show".</summary>
        private string command;

        /// <summary>Name of the furnishing being modified.</summary>
        private string furnishingName;

        /// <summary>Description of the furnishing, if one is being added.</summary>
        private string furnishingDescription;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            // Contextual message text to be supplied based on the action below
            var response = new ContextualString(actionInput.Actor, room.Parent);

            if (command == "add")
            {
                // Add or update the description.
                room.Furnishings.Add(new Furnishing()
                {
                    Description = furnishingDescription,
                    Keywords = new string[] { furnishingName }
                });
                response.ToOriginator = $"Furnishing '{furnishingName}' added/updated on room {roomName} [{roomId}].";
                room.Parent.Save();
            }
            else if (command == "remove")
            {
                var furnishing = room.FindFurnishing(furnishingName);
                if (furnishing != null)
                {
                    room.Furnishings.Remove(furnishing);
                    response.ToOriginator = $"Furnishing '{furnishingName}' removed from room {roomName} [{roomId}]";
                }
                room.Parent.Save();
            }
            else if (command == "show")
            {
                var output = new OutputBuilder();

                if (room.Furnishings.Count > 0)
                {
                    output.AppendLine($"Furnishings for {roomName} [{roomId}]:");

                    foreach (var furnishing in room.Furnishings)
                    {
                        output.AppendLine($"  {furnishing.Keywords}: {furnishing.Description}");
                    }
                }
                else
                {
                    output.AppendLine($"No furnishings found for {roomName} [{roomId}].");
                }

                session.Write(output);

                // No need to raise event.
                return;
            }

            var message = new SensoryMessage(SensoryType.Sight, 100, response);
            var evt = new GameEvent(actionInput.Actor, message);
            actionInput.Actor.Eventing.OnMiscellaneousEvent(evt, EventScope.SelfDown);
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

            // TODO: Although common guards ensure the player must have at least minor builder permissions to use this
            //       command, we should further enforce permissions (such as being a builder who has write access to
            //       this particular area, especially for player-facing "published" rather than in-progress areas).

            PreprocessInput(actionInput);

            // Ensure the sender of this command is currently located in a valid room.
            if (room == null)
            {
                return "You must be located in a valid room to change its furnishings.";
            }

            var usageText = new StringBuilder();
            usageText.AppendLine("Usage:");
            usageText.AppendLine("furnishings add <name> <description>");
            usageText.AppendLine("furnishings remove <name>");
            usageText.AppendLine("furnishings show");

            switch (command)
            {
                case "add":
                    // Ensure "add" syntax includes both a name and description.
                    return string.IsNullOrEmpty(furnishingName) || string.IsNullOrEmpty(furnishingDescription)
                               ? usageText.ToString()
                               : null;

                case "remove":
                    // Ensure "remove" syntax includes a name but nothing else.
                    return argCount != 2 ? usageText.ToString() : null;

                case "show":
                    // Ensure "show" syntax has no additional arguments.
                    return argCount > 1 ? usageText.ToString() : null;

                default:
                    // Handle case for "furnishings aalkdsfj lkajf" etc.
                    return usageText.ToString();
            }
        }

        /// <summary>Populate the private fields used by the Guards() and Execute() methods.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        private void PreprocessInput(ActionInput actionInput)
        {
            argCount = actionInput.Params.Length;

            // Location of the sender of the command.
            var location = actionInput.Actor.Parent;

            var foundRoomBehavior = location.FindBehavior<RoomBehavior>();
            if (foundRoomBehavior != null)
            {
                room = foundRoomBehavior;
                roomName = room.Parent.Name;
                roomId = room.Parent.Id;
            }

            // "furnishings" with no arguments will default to "furnishings show".
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

            // Name of the furnishing being added or removed.
            if (argCount > 1)
            {
                furnishingName = actionInput.Params[1].ToLower();
            }

            // The remainder of the command text is assumed to be the description.
            if (argCount > 2)
            {
                var tail = actionInput.Tail[command.Length..].TrimStart();
                tail = tail[furnishingName.Length..].TrimStart();
                furnishingDescription = tail;
            }
        }
    }
}