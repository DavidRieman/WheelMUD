//-----------------------------------------------------------------------------
// <copyright file="Spar.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WarriorRogueMage.Actions
{
    /// <summary>Initiates non-lethal combat.</summary>
    /// <remarks>TODO: Implement or retire? Doesn't seem to do anything right now...</remarks>
    [CoreExports.GameAction(100)]
    [ActionPrimaryAlias("spar", CommandCategory.Combat)]
    [ActionDescription("Initiates a bout of non-lethal combat.")]
    [ActionSecurity(SecurityRole.player)]
    public class Spar : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeMobile
        };

        /// <summary>Executes the command.</summary>
        /// <remarks>Verify that the Guards pass first.</remarks>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var actor = actionInput.Actor;
            var contextMessage = new ContextualString(actor, null)
            {
                ToOriginator = "You have entered the combat state.",
                ToReceiver = $"You see {actor.Name} shift into a combat stance.",
                ToOthers = $"You see {actor.Name} shift into a combat stance.",
            };

            var sm = new SensoryMessage(SensoryType.Debug, 100, contextMessage);
            var senseEvent = new SensoryEvent(actor, sm);
            actor.Eventing.OnMiscellaneousEvent(senseEvent, EventScope.ParentsDown);
        }

        /// <summary>Guards the command against incorrect usage.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>An error message describing the failure for the user, or null if all guards pass.</returns>
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
