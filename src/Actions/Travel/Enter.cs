//-----------------------------------------------------------------------------
// <copyright file="Enter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player to enter a Thing.</summary>
    /// <remarks>
    /// TODO: An "enter" action should only be present through a ContextCommand added by an EnterableBehavior,
    ///           like how OpensClosesBehavior handles it; move action to be EnterableBehavior.cs private class?
    /// </remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("enter", CommandCategory.Travel)]
    [ActionDescription("Enter a thing.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class EnterPortal : GameAction
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

        private EnterableExitableBehavior enterableBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            enterableBehavior.Enter(actionInput.Actor);
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

            // Rule: A thing must be singly targeted.
            // TODO: Try to find a single target according to the specified identifiers.  If more than one thing
            //       meets the identifiers then use a disambiguation targeting system to try to narrow to one thing.
            // TODO: This sort of find pattern may become common; maybe we need to simplify 
            //       to having a Thing method which does this?  IE "List<Thing> FindChildren<T>(string id)"?
            var enterableThings = actionInput.Actor.Parent.Children.Where(t => t.FindBehavior<EnterableExitableBehavior>() != null).ToList();

            if (enterableThings.Count > 1)
            {
                return "There is more than one thing by that identity.";
            }

            if (enterableThings.Count == 1)
            {
                var thing = enterableThings.First();
                enterableBehavior = thing.FindBehavior<EnterableExitableBehavior>();
                if (enterableBehavior == null)
                {
                    return $"You can not enter {thing.Name}.";
                }
            }

            // If we got this far, we couldn't find an appropriate enterable thing in the room.
            return "You can't see anything like that to enter.";
        }
    }
}