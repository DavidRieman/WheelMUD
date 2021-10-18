//-----------------------------------------------------------------------------
// <copyright file="Glance.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to have a quick look at your surroundings.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("glance", CommandCategory.Inform)]
    [ActionAlias("quick look", CommandCategory.Inform)]
    [ActionAlias("quicklook", CommandCategory.Inform)]
    [ActionDescription("Have a quick look at your surroundings.")]
    [ActionSecurity(SecurityRole.player)]
    public class Glance : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious
        };

        private SensesBehavior sensesBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Simply send a sensory event to the glancer; If they can see it, they'll get the output.
            var actor = actionInput.Actor;
            var message = new SensoryMessage(SensoryType.Sight, 100, BuildGlance(actor));
            var sensoryEvent = new SensoryEvent(actor, message);
            actor.Eventing.OnMiscellaneousEvent(sensoryEvent, EventScope.SelfOnly);
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

            // Rule: the sender must be capable of sensing/perceiving things.
            sensesBehavior = actionInput.Actor.FindBehavior<SensesBehavior>();

            return sensesBehavior == null ? "You are incapable of perceiving anything." : null;
        }

        /// <summary>Build the glance command response.</summary>
        /// <param name="sender">The sender of the glance command.</param>
        /// <returns>A fully generated glance of the room.</returns>
        private ContextualStringBuilder BuildGlance(Thing sender)
        {
            // Just basic generation of a general room description until furniture is implemented.
            var glanceString = new ContextualStringBuilder(sender, sender);
            var perceivedExits = sensesBehavior.PerceiveExits();
            var perceivedEntities = sensesBehavior.PerceiveEntities();
            var perceivedItems = sensesBehavior.PerceiveItems();

            glanceString.Append($"You are in <%red%>{sender.Parent.Name}<%n%>.  ", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);

            if (perceivedItems.Count != 0)
            {
                if (perceivedEntities.Count != 0)
                {
                    glanceString.Append(
                        perceivedExits.Count != 0
                            ? "There are various items, figures, and exits here."
                            : "There are various items, and figures here.",
                        ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                }
                else
                {
                    glanceString.Append(
                        perceivedExits.Count != 0
                            ? "There are various items, and exits here."
                            : "There are various items here.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                }
            }
            else
            {
                if (perceivedEntities.Count != 0)
                {
                    glanceString.Append(
                        perceivedExits.Count != 0
                            ? "There are various figures, and exits here."
                            : "There are various figures here.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                }
                else
                {
                    glanceString.Append(
                        perceivedExits.Count != 0 ? "There are various exits here." : "You don't see anything here.",
                        ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                }
            }

            return glanceString;
        }
    }
}