// <copyright file="Unfollow.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to stop following another player or mobile NPC, assuming one
//   was being followed.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions.Travel
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Behaviors;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>An action to start following another player or mobile whenever they move.</summary>
    [ExportGameAction]
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
            IController sender = actionInput.Controller;

            var myBehaviors = sender.Thing.Behaviors;
        
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
                            followedBehavior.Followers.Remove(sender.Thing);
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
                var message = new ContextualString(sender.Thing, null)
                {
                    ToOriginator = "You aren't following anybody."
                };

                var senseMessage = new SensoryMessage(SensoryType.All, 100, message);

                var followEvent = new FollowEvent(sender.Thing, senseMessage, sender.Thing, null);

                // Broadcast the event
                sender.Thing.Eventing.OnMiscellaneousEvent(followEvent, EventScope.ParentsDown);
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

            return null;
        }
    }
}
