//-----------------------------------------------------------------------------
// <copyright file="Spar.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 6/12/2011
//   Purpose   : Initiates non-lethal combat.
// </summary>
// <history>
//   June 13, 2010 - By Fastalanasa
//   Moved to WarriorRogueMage.Actions from WheelMUD.Actions.
// </history>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Actions
{
    using System.Collections.Generic;

    using WheelMUD.Actions;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>Initiates non-lethal combat.</summary>
    [ExportGameAction]
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
            IController sender = actionInput.Controller;
            var contextMessage = new ContextualString(sender.Thing, sender.Thing)
            {
                ToOriginator = "You have entered the combat state.",
                ToReceiver = "You see $Aggressor.Name shift into a combat stance.",
                ToOthers = "You see $Aggressor.Name shift into a combat stance.",
            };

            var sm = new SensoryMessage(SensoryType.Debug, 100, contextMessage);
            var senseEvent = new SensoryEvent(sender.Thing, sm);
            sender.Thing.Eventing.OnMiscellaneousEvent(senseEvent, EventScope.ParentsDown);
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
