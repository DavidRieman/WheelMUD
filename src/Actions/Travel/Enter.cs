//-----------------------------------------------------------------------------
// <copyright file="Enter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows a player to enter a Thing.</summary>
    /// <remarks>
    /// @@@ TODO: An "enter" action should only be present through a ContextCommand added by an EnterableBehavior,
    ///           like how OpensClosesBehavior handles it; move action to be EnterableBehavior.cs private class?
    /// </remarks>
    [ExportGameAction]
    [ActionPrimaryAlias("enter", CommandCategory.Travel)]
    [ActionDescription("Enter a thing.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class EnterPortal : GameAction
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

        private EnterableExitableBehavior enterableBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            this.enterableBehavior.Enter(sender.Thing);
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

            // Rule: A thing must be singly targeted.
            // @@@ TODO: Try to find a single target according to the specified identifiers.  If more than one thing
            //           meets the identifiers then use a disambiguation targeting system to try to narrow to one thing.
            // @@@ TODO: This sort of find pattern may become common; maybe we need to simplify 
            //           to having a Thing method which does this?  IE "List<Thing> FindChildren<T>(string id)"?
            Predicate<Thing> findPredicate = (Thing t) => t.Behaviors.FindFirst<EnterableExitableBehavior>() != null;
            List<Thing> enterableThings = sender.Thing.Parent.FindAllChildren(findPredicate);

            if (enterableThings.Count > 1)
            {
                return "There is more than one thing by that identity.";
            }
            else if (enterableThings.Count == 1)
            {
                Thing thing = enterableThings.First();
                this.enterableBehavior = thing.Behaviors.FindFirst<EnterableExitableBehavior>();
                if (this.enterableBehavior == null)
                {
                    return "You can not enter " + thing.Name + ".";
                }
            }

            // If we got this far, we couldn't find an appropriate enterable thing in the room.
            return "You can't see anything like that to enter.";
        }
    }
}