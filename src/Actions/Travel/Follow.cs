// <copyright file="Follow.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to start following another player or mobile whenever they move.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions.Travel
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Behaviors;
    using WheelMUD.Interfaces;

    /// <summary>An action to start following another player or mobile whenever they move.</summary>
    [ExportGameAction]
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
        private Thing target = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            var self = sender.Thing;

            if (actionInput.Params.Length == 0)
            {
                this.ShowStatus(actionInput);
                return;
            }

            // Check for existing FollowedBehavior on the target and create one if necessary.
            // Then add the sender to the list of followers.
            var targetBehaviors = this.target.Behaviors;
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

            followingBehavior.Target = this.target;
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);

            if (commonFailure != null)
            {
                return commonFailure;
            }

            if (actionInput.Params.Length == 0)
            {
                return null;
            }

            // TODO: CommonGuards.RequiresAtLeastOneArgument makes the length check redundant?
            string targetName = (actionInput.Params.Length > 0) ? actionInput.Params[0] : string.Empty;
            string targetFullName = actionInput.Tail.Trim().ToLower();

            // Try to find the target either by all the parameter text or by just the first parameter.
            this.target = GameAction.GetPlayerOrMobile(targetFullName) ?? GameAction.GetPlayerOrMobile(targetName);

            // Rule: Is the target an entity?
            if (this.target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target the initator?
            if (sender.Thing.Name.ToLower() == this.target.Name.ToLower())
            {
                return "You can't follow yourself.";
            }

            // Rule: Is the target in the same room?
            if (sender.Thing.Parent.ID != this.target.Parent.ID)
            {
                return targetName + " does not appear to be in the vicinity.";
            }

            SenseManager senses = new SenseManager();
            senses.AddSense(new Sense { SensoryType = SensoryType.Sight, Enabled = true });
            if (!this.target.IsDetectableBySense(senses))
            {
                return targetName + " does not appear to be in the vicinity.";
            }

            return null;
        }

        /// <summary>Shows who is currently following the player, and who the player is following.</summary>
        /// <remarks>TODO: Replace the sender.Write() calls with something more general, e.g. an event.</remarks>
        /// <param name="actionInput">The action input.</param>
        private void ShowStatus(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            sender.Write("You are following: ", false);

            var senderBehaviors = sender.Thing.Behaviors;
            var followingBehavior = senderBehaviors.FindFirst<FollowingBehavior>();
            var followedBehavior = senderBehaviors.FindFirst<FollowedBehavior>();

            if (followingBehavior == null)
            {
                sender.Write("(nobody)");
            }
            else
            {
                sender.Write(followingBehavior.Target.Name);
            }

            sender.Write("You are being followed by: ", false);

            if (followedBehavior == null)
            {
                sender.Write("(nobody)");
            }
            else
            {
                lock (followedBehavior.Followers)
                {
                    string followers = string.Join(", ", followedBehavior.Followers.Select(f => f.Name));
                    sender.Write(followers);
                }
            }
        }
    }
}