//-----------------------------------------------------------------------------
// <copyright file="GoTo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows a player move to the room ID or entity specified.
//   Created: January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows a player move to the room ID or entity specified.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("goto", CommandCategory.Admin)]
    [ActionAlias("go to", CommandCategory.Admin)]
    [ActionDescription("Go to the specified entity or room number.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.mobile)]
    public class GoTo : GameAction
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

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Send just one message to the command sender since they know what's going on.
            IController sender = actionInput.Controller;

            // Attempt exact ID match
            var targetPlace = ThingManager.Instance.FindThing(actionInput.Tail);

            // If input is a simple number, assume we mean a room
            int roomNum;
            targetPlace = int.TryParse(actionInput.Tail, out roomNum) ? ThingManager.Instance.FindThing("room/" + roomNum) : ThingManager.Instance.FindThingByName(actionInput.Tail, false, true);

            if (targetPlace == null)
            {
                sender.Write("Room or Entity not found.\n");
                return;
            }

            if (targetPlace.FindBehavior<RoomBehavior>() == null)
            {
                // If the target's parent is a room, go there instead
                if (targetPlace.Parent != null && targetPlace.Parent.FindBehavior<RoomBehavior>() != null)
                {
                    targetPlace = targetPlace.Parent;
                }
                else
                {
                    sender.Write("Target is not a room and is not in a room!\n");
                    return;
                }
            }

            var leaveContextMessage = new ContextualString(sender.Thing, sender.Thing.Parent)
            {
                ToOriginator = null,
                ToReceiver = @"$ActiveThing.Name disappears into nothingness.",
                ToOthers = @"$ActiveThing.Name disappears into nothingness.",
            };
            var arriveContextMessage = new ContextualString(sender.Thing, targetPlace)
            {
                ToOriginator = "You teleport to " + targetPlace.Name + ".",
                ToReceiver = @"$ActiveThing.Name appears from nothingness.",
                ToOthers = @"$ActiveThing.Name appears from nothingness.",
            };
            var leaveMessage = new SensoryMessage(SensoryType.Sight, 100, leaveContextMessage);
            var arriveMessage = new SensoryMessage(SensoryType.Sight, 100, arriveContextMessage);

            // If we successfully move (IE the move may get cancelled if the user doesn't have permission
            // to enter a particular location, some other behavior cancels it, etc), then perform a 'look'
            // command to get immediate feedback about the new location.
            // @@@ TODO: This should not 'enqueue' a command since, should the player have a bunch of 
            //     other commands entered, the 'look' feedback will not immediately accompany the 'goto' 
            //     command results like it should.
            var movableBehavior = sender.Thing.FindBehavior<MovableBehavior>();

            if (movableBehavior != null && movableBehavior.Move(targetPlace, sender.Thing, leaveMessage, arriveMessage))
            {
                CommandManager.Instance.EnqueueAction(new ActionInput("look", sender));
            }
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

            return null;
        }
    }
}