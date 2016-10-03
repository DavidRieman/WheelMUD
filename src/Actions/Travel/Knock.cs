//-----------------------------------------------------------------------------
// <copyright file="Knock.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to knock on a door.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>An action to knock on a door.</summary>
    [ExportGameAction]
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
            IController sender = actionInput.Controller;
            var thisRoomMessage = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = @"You knock on $ActiveThing.Name.",
                ToOthers = @"$Knocker.Name knocks on $ActiveThing.Name.",
                ToReceiver = @"$Knocker.Name knocks on you.",
            };
            var nextRoomMessage = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = null,
                ToOthers = @"Someone knocks on $ActiveThing.Name.",
                ToReceiver = null,
            };

            // Create sensory messages.
            var thisRoomSM = new SensoryMessage(SensoryType.Sight | SensoryType.Hearing, 100, thisRoomMessage, new Hashtable { { "Knocker", sender.Thing } });
            var nextRoomSM = new SensoryMessage(SensoryType.Hearing, 100, nextRoomMessage);

            // Generate our knock events.
            var thisRoomKnockEvent = new KnockEvent(this.target, thisRoomSM);
            var nextRoomKnockEvent = new KnockEvent(this.target, nextRoomSM);

            // Broadcast the requests/events; the events handle sending the sensory messages.
            sender.Thing.Eventing.OnCommunicationRequest(thisRoomKnockEvent, EventScope.ParentsDown);
            if (!thisRoomKnockEvent.IsCancelled)
            {
                // The knocking here happens regardless of whether it's cancelled on the inside.
                sender.Thing.Eventing.OnCommunicationEvent(thisRoomKnockEvent, EventScope.ParentsDown);

                // Next try to send a knock event into the adjacent place too.
                this.nextRoom.Eventing.OnCommunicationRequest(nextRoomKnockEvent, EventScope.SelfDown);
                if (!nextRoomKnockEvent.IsCancelled)
                {
                    this.nextRoom.Eventing.OnCommunicationEvent(nextRoomKnockEvent, EventScope.SelfDown);
                }
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            string targetName = actionInput.Tail.Trim().ToLower();

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
                    return this.CheckForDoor(actionInput);
                }
            }

            return "Try knocking on a door instead.";
        }

        /* @@@ TODO: Allow Knocking on any specified Thing; based on that thing, it may relay the
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
            /* @@@ TODO: Fix
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

            this.target = exit.Door;

            // Which room does the door lead to?
            if (!Equals(exit.ExitEndA.Room, sender.Thing.Parent))
            {
                this.adjacentRoom = exit.ExitEndA.Room;
            }
            else
            {
                this.adjacentRoom = exit.ExitEndB.Room;
            }
            */
            return null;
        }
    }
}