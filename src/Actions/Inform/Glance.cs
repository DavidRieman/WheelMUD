//-----------------------------------------------------------------------------
// <copyright file="Glance.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to have a quick look at your surroundings.
//   @@@ TODO: Implement
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>An action to have a quick look at your surroundings.</summary>
    [ExportGameAction]
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
            // Simply send a sensory event to the glancer; if they can see it, they'll get the output.
            // @@@ TODO: For advanced scenarios, this might want to somehow restrict the event from cascading; 
            //     IE if a tiny pixie character is inside a human player's backpack, they shouldn't receive 
            //     the human player's glance output.  (As is the event cascades and all Children get it too).
            //     Alternatively, the sense processor can look at the ActiveThing and the sense type to decide
            //     whether to echo the output; IE said pixie might still get Sound-based simple sensory event
            //     output even though they aren't in the same location; they may hear it through the backpack.
            // @@@ Actually, BuildGlance might cause the print to only occur to the sender anyway due to the
            //     usage of ContextualStringUsage.OnlyWhenBeingPassedToReceiver; needs testing!
            IController sender = actionInput.Controller;
            var message = new SensoryMessage(SensoryType.Sight, 100, this.BuildGlance(sender.Thing));
            var sensoryEvent = new SensoryEvent(sender.Thing, message);
            sender.Thing.Eventing.OnMiscellaneousEvent(sensoryEvent, EventScope.SelfDown);
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

            // Rule: the sender must be capable of sensing/perceiving things.
            this.sensesBehavior = actionInput.Controller.Thing.Behaviors.FindFirst<SensesBehavior>();
            if (this.sensesBehavior == null)
            {
                return "You are incapable of perceiving anything.";
            }
            
            return null;
        }

        /// <summary>Build the glance command response.</summary>
        /// <param name="sender">The sender of the glance command.</param>
        /// <returns>A fully generated glance of the room.</returns>
        private ContextualStringBuilder BuildGlance(Thing sender)
        {
            // Just basic generation of a general room description until furniture is implemented.
            var glanceString = new ContextualStringBuilder(sender, sender);
            var perceivedExits = this.sensesBehavior.PerceiveExits();
            var perceivedEntities = this.sensesBehavior.PerceiveEntities();
            var perceivedItems = this.sensesBehavior.PerceiveItems();

            glanceString.Append("You seem to be in a " + sender.Parent.Name + ".  ", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);

            if (perceivedItems.Count != 0)
            {
                if (perceivedEntities.Count != 0)
                {
                    if (perceivedExits.Count != 0)
                    {
                        glanceString.Append("It appears there are various items, figures, and exits in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                    else
                    {
                        glanceString.Append("It appears there are various items, and figures in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                }
                else
                {
                    if (perceivedExits.Count != 0)
                    {
                        glanceString.Append("It appears there are various items, and exits in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                    else
                    {
                        glanceString.Append("It appears there are various items in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                }
            }
            else
            {
                if (perceivedEntities.Count != 0)
                {
                    if (perceivedExits.Count != 0)
                    {
                        glanceString.Append("It appears there are various figures, and exits in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                    else
                    {
                        glanceString.Append("It appears there are various figures in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                }
                else
                {
                    if (perceivedExits.Count != 0)
                    {
                        glanceString.Append("It appears there are various exits in this room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                    else
                    {
                        glanceString.Append("You don't see anything in the room.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
                    }
                }
            }

            return glanceString;
        }
    }
}