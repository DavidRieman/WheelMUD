//-----------------------------------------------------------------------------
// <copyright file="Say.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Communicate with others in the same location.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>Communicate with others in the same location.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("say", CommandCategory.Communicate)]
    [ActionAlias("'", CommandCategory.Communicate)]
    [ActionAlias("\"", CommandCategory.Communicate)]
    [ActionDescription("Say something to those nearby.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Say : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.RequiresAtLeastOneArgument
        };

        private string sayText;
        
        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            var contextMessage = new ContextualString(sender.Thing, sender.Thing.Parent)
            {
                ToOriginator = @"You say: " + this.sayText,
                ToReceiver = @"$ActiveThing.Name says: " + this.sayText,
                ToOthers = @"$ActiveThing.Name says: " + this.sayText,
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            var sayEvent = new VerbalCommunicationEvent(sender.Thing, sm, VerbalCommunicationType.Say);
            sender.Thing.Eventing.OnCommunicationRequest(sayEvent, EventScope.ParentsDown);
            if (!sayEvent.IsCancelled)
            {
                sender.Thing.Eventing.OnCommunicationEvent(sayEvent, EventScope.ParentsDown);
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

            this.sayText = actionInput.Tail.Trim();
            return null;
        }
    }
}