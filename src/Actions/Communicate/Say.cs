//-----------------------------------------------------------------------------
// <copyright file="Say.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Communicate with others in the same location.</summary>
    [CoreExports.GameAction(0)]
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
            var actor = actionInput.Actor;
            var contextMessage = new ContextualString(actor, actor.Parent)
            {
                ToOriginator = $"You say: {sayText}",
                ToReceiver = $"{actor.Name} says: {sayText}",
                ToOthers = $"{actor.Name} says: {sayText}",
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            var sayEvent = new VerbalCommunicationEvent(actor, sm, VerbalCommunicationType.Say);
            actor.Eventing.OnCommunicationRequest(sayEvent, EventScope.ParentsDown);
            if (!sayEvent.IsCanceled)
            {
                actor.Eventing.OnCommunicationEvent(sayEvent, EventScope.ParentsDown);
            }
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

            sayText = actionInput.Tail.Trim();
            return null;
        }
    }
}