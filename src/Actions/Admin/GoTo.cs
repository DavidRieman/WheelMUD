//-----------------------------------------------------------------------------
// <copyright file="GoTo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player move to the room Id or entity specified.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("goto", CommandCategory.Admin)]
    [ActionAlias("go to", CommandCategory.Admin)]
    [ActionDescription("Go to the specified entity or room number.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.mobile)]
    public class GoTo : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
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
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            // If input is a simple number, assume we mean a room
            // TODO: IMPROVE TARGETING!
            var targetPlace = int.TryParse(actionInput.Tail, out var roomNum) ? ThingManager.Instance.FindThing("room/" + roomNum) : ThingManager.Instance.FindThingByName(actionInput.Tail, false, true);

            if (targetPlace == null)
            {
                session.WriteLine("Room or Entity not found.");
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
                    session.WriteLine("Target is not a room and is not in a room!");
                    return;
                }
            }

            var adminName = actionInput.Actor.Name;
            var leaveContextMessage = new ContextualString(actionInput.Actor, actionInput.Actor.Parent)
            {
                ToOriginator = null,
                ToReceiver = $"{adminName} disappears into nothingness.",
                ToOthers = $"{adminName} disappears into nothingness.",
            };
            var arriveContextMessage = new ContextualString(actionInput.Actor, targetPlace)
            {
                ToOriginator = $"You teleported to {targetPlace.Name}.",
                ToReceiver = $"{adminName} appears from nothingness.",
                ToOthers = $"{adminName} appears from nothingness.",
            };
            var leaveMessage = new SensoryMessage(SensoryType.Sight, 100, leaveContextMessage);
            var arriveMessage = new SensoryMessage(SensoryType.Sight, 100, arriveContextMessage);

            // If we successfully move (IE the move may get canceled if the user doesn't have permission
            // to enter a particular location, some other behavior cancels it, etc), then perform a 'look'
            // command to get immediate feedback about the new location.
            // TODO: This should not 'enqueue' a command since, should the player have a bunch of 
            //     other commands entered, the 'look' feedback will not immediately accompany the 'goto' 
            //     command results like it should.
            var movableBehavior = actionInput.Actor.FindBehavior<MovableBehavior>();

            if (movableBehavior != null && movableBehavior.Move(targetPlace, actionInput.Actor, leaveMessage, arriveMessage))
            {
                CommandManager.Instance.EnqueueAction(new ActionInput("look", actionInput.Session, actionInput.Actor));
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}