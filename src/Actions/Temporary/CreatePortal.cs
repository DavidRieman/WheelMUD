//-----------------------------------------------------------------------------
// <copyright file="CreatePortal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Universe;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows an admin to create a portal.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("create portal", CommandCategory.Admin)]
    [ActionAlias("createportal", CommandCategory.Admin)]
    [ActionDescription("Temporary test command. Creates a portal.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class CreatePortal : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>The current room that the player is within.</summary>
        private Thing targetPlace;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Create a portal targeting the specified room.
            var portalItem = new Thing();
            portalItem.Behaviors.Add(new PortalBehavior
            {
                DestinationThingID = targetPlace.Id,
            });

            // TODO: Should not be needed after OLC and instant-spawn commands which should work
            // for any type of Thing; not just portals.  Spawning (either way) should use the Request
            // and Event pattern (including for the resulting "it happened" sensory message).
            actionInput.Actor.Parent.Add(portalItem);
            actionInput.Session?.WriteLine("A magical portal opens up in front of you.");
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

            // Rule: We should have at least 1 item in our words array.
            var numberWords = actionInput.Params.Length;
            if (numberWords < 2)
            {
                return "You must specify a room to create a portal to.";
            }

            // Check to see if the first word is a number.
            var roomToGet = actionInput.Params[1];
            if (string.IsNullOrEmpty(roomToGet))
            {
                // Its not a number so it could be an entity... try it.
                targetPlace = GetPlayerOrMobile(actionInput.Params[0]);
                if (targetPlace == null)
                {
                    return $"Could not convert {actionInput.Params[0]} to a room number.";
                }
            }

            targetPlace = PlacesManager.Instance.WorldBehavior.FindRoom(roomToGet);
            if (targetPlace == null)
            {
                return $"Could not find the room {roomToGet}.";
            }

            return null;
        }
    }
}