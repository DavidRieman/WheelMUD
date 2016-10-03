//-----------------------------------------------------------------------------
// <copyright file="Web.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   @@@ Temporary script to demonstrate a web effect.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>@@@ Temporary script to demonstrate a web effect.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("web", CommandCategory.Temporary)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.player)]
    public class Web : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile
        };

        /// <summary>The target entity to be effected by the web.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            ////target.Effects.AddEffect(new StatEffect("mobility", -2, "", "Your mobility increases"),
            ////                         new TimeSpan(0, 1, 0, 0));
            ////sender.Thing.Effects.AddEffect(new Unbalance(), new TimeSpan(0, 0, 3));

            // @@@ TODO: Should use Request and Event pattern and avoid direct Controller.Writes
            //var userControlledBehavior = sender.Thing.BehaviorManager.FindFirst<UserControlledBehavior>();
            //userControlledBehavior.Controller.Write("You cast a web over " + this.target.Name);
            //this.target.Controller.Write(sender.Thing.Name + " casts a web over you");
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
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

            string targetName = actionInput.Tail.Trim().ToLower();

            // Rule: Do we have a target?
            this.target = GameAction.GetPlayerOrMobile(targetName);
            if (this.target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target the initator?
            if (sender.Thing.Name.ToLower() == this.target.Name.ToLower())
            {
                return "You can't punch yourself.";
            }

            // Rule: Is the target in the same room?
            if (sender.Thing.Parent.ID != this.target.Parent.ID)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target alive?
            if (this.target.Stats["health"].Value <= 0)
            {
                return this.target.Name + " is dead.";
            }

            return null;
        }
    }
}