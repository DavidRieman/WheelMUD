//-----------------------------------------------------------------------------
// <copyright file="CreatePortal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to create a portal.
//   Created: January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>A command that allows an admin to create a portal.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("create portal", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command.")]
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
        private Thing targetPlace = null;
        
        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Create a portal targeting the specified room.
            IController sender = actionInput.Controller;
            Thing portalItem = new Thing();
            portalItem.Behaviors.Add(new PortalBehavior()
            {
                DestinationThingID = this.targetPlace.ID,
            });

            // @@@ TODO: Should not be needed after OLC and instant-spawn commands which should work
            // for any type of Thing; not just portals.  Spawning (either way) should use the Request
            // and Event pattern.
            sender.Thing.Parent.Add(portalItem);
            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            userControlledBehavior.Controller.Write("A magical portal opens up in front of you.");
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

            // Rule: We should have at least 1 item in our words array.
            int numberWords = actionInput.Params.Length;
            if (numberWords < 2)
            {
                return "You must specify a room to create a portal to.";
            }

            // Check to see if the first word is a number.
            string roomToGet = actionInput.Params[1];
            if (string.IsNullOrEmpty(roomToGet))
            {
                // Its not a number so it could be an entity... try it.
                this.targetPlace = GameAction.GetPlayerOrMobile(actionInput.Params[0]);
                if (this.targetPlace == null)
                {
                    return "Could not convert " + actionInput.Params[0] + " to a room number.";
                }
            }

//            this.targetRoom = bridge.World.FindRoom(roomToGet);
            if (this.targetPlace == null)
            {
                return string.Format("Could not find the room {0}.", roomToGet);
            }

            return null;
        }
    }
}