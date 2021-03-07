//-----------------------------------------------------------------------------
// <copyright file="Unfollow.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to stop following another player or mobile around.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("unfollow", CommandCategory.Travel)]
    [ActionDescription("Stop following a friend or foe whenever they move.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Unfollow : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var myBehaviors = actionInput.Controller.Thing.Behaviors;

            var followingBehavior = myBehaviors.FindFirst<FollowingBehavior>();
            if (followingBehavior != null)
            {
                var target = followingBehavior.Target;
                if (target != null)
                {
                    var targetBehaviors = target.Behaviors;
                    var followedBehavior = targetBehaviors.FindFirst<FollowedBehavior>();
                    if (followedBehavior != null)
                    {
                        lock (followedBehavior.Followers)
                        {
                            followedBehavior.Followers.Remove(actionInput.Controller.Thing);
                            if (followedBehavior.Followers.Count == 0)
                            {
                                targetBehaviors.Remove(followedBehavior);
                            }
                        }
                    }
                }

                myBehaviors.Remove(followingBehavior);
            }
            else
            {
                var message = new ContextualString(actionInput.Controller.Thing, null)
                {
                    ToOriginator = "You aren't following anybody."
                };

                var senseMessage = new SensoryMessage(SensoryType.All, 100, message);

                var followEvent = new FollowEvent(actionInput.Controller.Thing, senseMessage, actionInput.Controller.Thing, null);

                // Broadcast the event
                actionInput.Controller.Thing.Eventing.OnMiscellaneousEvent(followEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}
