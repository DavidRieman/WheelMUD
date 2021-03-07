//-----------------------------------------------------------------------------
// <copyright file="Knock.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to knock on a door.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("knock", CommandCategory.Travel)]
    [ActionDescription("Knock on a door.")]
    [ActionSecurity(SecurityRole.player)]
    public class Knock : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The found target object.</summary>
        private Thing target = null;

        /// <summary>The adjacent room object.</summary>
        private Thing nextRoom = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Build our knock messages for this room and the next.  Only send message to the door-type thing once.
            var thisRoomMessage = new ContextualString(actionInput.Controller.Thing, target)
            {
                ToOriginator = $"You knock on {target.Name}.",
                ToOthers = $"{actionInput.Controller.Thing.Name} knocks on {target.Name}.",
                ToReceiver = $"{actionInput.Controller.Thing.Name} knocks on you.",
            };
            var nextRoomMessage = new ContextualString(actionInput.Controller.Thing, target)
            {
                ToOriginator = null,
                ToOthers = $"Someone knocks on {target.Name}.",
                ToReceiver = null,
            };

            // Create sensory messages.
            var thisRoomSM = new SensoryMessage(SensoryType.Sight | SensoryType.Hearing, 100, thisRoomMessage);
            var nextRoomSM = new SensoryMessage(SensoryType.Hearing, 100, nextRoomMessage);

            // Generate our knock events.
            var thisRoomKnockEvent = new KnockEvent(target, thisRoomSM);
            var nextRoomKnockEvent = new KnockEvent(target, nextRoomSM);

            // Broadcast the requests/events; the events handle sending the sensory messages.
            actionInput.Controller.Thing.Eventing.OnCommunicationRequest(thisRoomKnockEvent, EventScope.ParentsDown);
            if (!thisRoomKnockEvent.IsCancelled)
            {
                // The knocking here happens regardless of whether it's cancelled on the inside.
                actionInput.Controller.Thing.Eventing.OnCommunicationEvent(thisRoomKnockEvent, EventScope.ParentsDown);

                // Next try to send a knock event into the adjacent place too.
                nextRoom.Eventing.OnCommunicationRequest(nextRoomKnockEvent, EventScope.SelfDown);
                if (!nextRoomKnockEvent.IsCancelled)
                {
                    nextRoom.Eventing.OnCommunicationEvent(nextRoomKnockEvent, EventScope.SelfDown);
                }
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            var targetName = actionInput.Tail.Trim().ToLower();

            // Rule: Do we have a target?
            if (targetName.Length == 0)
            {
                return "Knock on what?";
            }

            // Find out how many parameters were supplied.
            if (actionInput.Params.Length > 1)
            {
                // Are they trying to knock on a door?
                if (actionInput.Params[0].ToLower() == "door")
                {
                    return CheckForDoor(actionInput);
                }
            }

            return "Try knocking on a door instead.";
        }

        /* TODO: Allow Knocking on any specified Thing; based on that thing, it may relay the
         * sensory event to just the current room, if it has an ExitBehavior and an OpensClosesBehavior
         * (IE it is a physical door rather than just a connection or portal) relays the event to both 
         * sides.  The event of course relays through all Children so a container's contents (or a 
         * vehicle's contents) all can receive the event, and behavior responses to the event could
         * potentially occur.
        /// <summary>Converts a short direction alias to a full-length direction string.</summary>
        /// <param name="shortDirection">Short direction alias, like "n" or "ne".</param>
        /// <returns>The expanded version of the direction, if found, like "north" or "northeast".</returns>
        private static string ConvertShortDirectionToLong(string shortDirection)
        {
            string longDir = shortDirection;

            switch (shortDirection.ToLower())
            {
                case "n":
                    longDir = "north";
                    break;
                case "e":
                    longDir = "east";
                    break;
                case "s":
                    longDir = "south";
                    break;
                case "w":
                    longDir = "west";
                    break;
                case "ne":
                    longDir = "northeast";
                    break;
                case "se":
                    longDir = "southeast";
                    break;
                case "sw":
                    longDir = "southwest";
                    break;
                case "nw":
                    longDir = "northwest";
                    break;
                case "u":
                    longDir = "up";
                    break;
                case "d":
                    longDir = "down";
                    break;
            }

            return longDir;
        }*/

        /// <summary>Checks to see if an openable door is in the specified direction.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>True if a door was found, else false.</returns>
        private string CheckForDoor(ActionInput actionInput)
        {
            /* TODO: Fix
            // Rule: Is the next parameter a valid direction?
            IController sender = actionInput.Controller;
            string dir = actionInput.Params[1].ToLower();
            Exit exit;
            sender.Thing.Parent.Exits.TryGetValue(ConvertShortDirectionToLong(dir), out exit);
            if (exit == null)
            {
                return "There is no door there to knock on.";
            }

            // Rule: does the exit have a door?
            if (exit.Door == null)
            {
                return "That exit has no door to knock on.";
            }

            // Rule: the door is not open.
            if (exit.Door.OpenState == OpenState.Open)
            {
                return "That door is open. Why knock on an open door?";
            }

            target = exit.Door;

            // Which room does the door lead to?
            if (!Equals(exit.ExitEndA.Room, sender.Thing.Parent))
            {
                adjacentRoom = exit.ExitEndA.Room;
            }
            else
            {
                adjacentRoom = exit.ExitEndB.Room;
            }
            */
            return null;
        }
    }
}