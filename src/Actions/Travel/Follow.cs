//-----------------------------------------------------------------------------
// <copyright file="Follow.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Actions
{
    /// <summary>An action to start following another player or mobile around.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("follow", CommandCategory.Travel)]
    [ActionDescription("Begin following a friend or foe whenever they move.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Follow : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile
        };

        /// <summary>The found target object.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var self = actionInput.Controller.Thing;

            if (actionInput.Params.Length == 0)
            {
                ShowStatus(actionInput);
                return;
            }

            // Check for existing FollowedBehavior on the target and create one if necessary.
            // Then add the sender to the list of followers.
            var targetBehaviors = target.Behaviors;
            var followedBehavior = targetBehaviors.FindFirst<FollowedBehavior>();
            if (followedBehavior == null)
            {
                followedBehavior = new FollowedBehavior();
                targetBehaviors.Add(followedBehavior);
            }

            followedBehavior.Followers.Add(self);

            // Check for existing FollowingBehavior on the sender and create one if necessary.
            // Then mark the target as the Thing being followed.
            var senderBehaviors = self.Behaviors;
            var followingBehavior = senderBehaviors.FindFirst<FollowingBehavior>();
            if (followingBehavior == null)
            {
                followingBehavior = new FollowingBehavior();
                senderBehaviors.Add(followingBehavior);
            }
            else if (followingBehavior.Target != null)
            {
                var oldTarget = followingBehavior.Target;
                var oldTargetFollowedBehavior = oldTarget.Behaviors.FindFirst<FollowedBehavior>();
                if (oldTargetFollowedBehavior != null)
                {
                    oldTargetFollowedBehavior.Followers.Remove(self);
                    if (oldTargetFollowedBehavior.Followers.Count == 0)
                    {
                        oldTarget.Behaviors.Remove(oldTargetFollowedBehavior);
                    }
                }
            }

            followingBehavior.Target = target;
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

            if (actionInput.Params.Length == 0)
            {
                return null;
            }

            // TODO: CommonGuards.RequiresAtLeastOneArgument makes the length check redundant?
            var targetName = (actionInput.Params.Length > 0) ? actionInput.Params[0] : string.Empty;
            var targetFullName = actionInput.Tail.Trim().ToLower();

            // Try to find the target either by all the parameter text or by just the first parameter.
            target = GetPlayerOrMobile(targetFullName) ?? GetPlayerOrMobile(targetName);

            // Rule: Is the target an entity?
            if (target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target the initiator?
            if (string.Equals(actionInput.Controller.Thing.Name, target.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return "You can't follow yourself.";
            }

            // Rule: Is the target in the same room?
            if (actionInput.Controller.Thing.Parent.Id != target.Parent.Id)
            {
                return $"{targetName} does not appear to be in the vicinity.";
            }

            var senses = new SenseManager();
            senses.AddSense(new Sense { SensoryType = SensoryType.Sight, Enabled = true });
            if (!target.IsDetectableBySense(senses))
            {
                return $"{targetName} does not appear to be in the vicinity.";
            }

            return null;
        }

        /// <summary>Shows who is currently following the player, and who the player is following.</summary>
        /// <remarks>TODO: Replace the sender.Write() calls with something more general, e.g. an event.</remarks>
        /// <param name="actionInput">The action input.</param>
        private void ShowStatus(ActionInput actionInput)
        {
            var senderBehaviors = actionInput.Controller.Thing.Behaviors;
            var followingBehavior = senderBehaviors.FindFirst<FollowingBehavior>();
            var followedBehavior = senderBehaviors.FindFirst<FollowedBehavior>();

            var output = new OutputBuilder();
            
            output.Append("You are following: ");

            output.AppendLine(followingBehavior == null ? "(nobody)" : followingBehavior.Target.Name);

            output.Append("You are being followed by: ");

            if (followedBehavior == null)
            {
                output.AppendLine("(nobody)");
            }
            else
            {
                lock (followedBehavior.Followers)
                {
                    var followers = followedBehavior.Followers.Select(f => f.Name).BuildPrettyList();
                    output.AppendLine(followers);
                }
            }
            
            actionInput.Controller.Write(output);
        }
    }
}