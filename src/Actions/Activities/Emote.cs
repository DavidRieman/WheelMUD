//-----------------------------------------------------------------------------
// <copyright file="Emote.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows a player to emote.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>Emote script.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("emote", CommandCategory.Activities)]
    [ActionAlias("em", CommandCategory.Activities)]
    [ActionDescription("Emote in freeform.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Emote : GameAction
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

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string emoteString = string.Format("<*$ActiveThing.Name {0}>", actionInput.Tail);

            var contextualString = new ContextualString(sender.Thing, sender.Thing.Parent)
            {
                ToOriginator = emoteString,
                ToOthers = emoteString
            };

            var msg = new SensoryMessage(SensoryType.Sight, 100, contextualString);
            var emoteEvent = new VerbalCommunicationEvent(sender.Thing, msg, VerbalCommunicationType.Emote);
            sender.Thing.Eventing.OnCommunicationRequest(emoteEvent, EventScope.ParentsDown);
            if (!emoteEvent.IsCancelled)
            {
                sender.Thing.Eventing.OnCommunicationEvent(emoteEvent, EventScope.ParentsDown);
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

            return null;
        }
    }
}