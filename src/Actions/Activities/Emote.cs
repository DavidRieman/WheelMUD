//-----------------------------------------------------------------------------
// <copyright file="Emote.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player to emote.</summary>
    [ExportGameAction(0)]
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
            var emoteString = $"<*{actionInput.Actor.Name} {actionInput.Tail}>";

            var contextualString = new ContextualString(actionInput.Actor, actionInput.Actor.Parent)
            {
                ToOriginator = emoteString,
                ToOthers = emoteString
            };

            var msg = new SensoryMessage(SensoryType.Sight, 100, contextualString);
            var emoteEvent = new VerbalCommunicationEvent(actionInput.Actor, msg, VerbalCommunicationType.Emote);
            actionInput.Actor.Eventing.OnCommunicationRequest(emoteEvent, EventScope.ParentsDown);
            if (!emoteEvent.IsCancelled)
            {
                actionInput.Actor.Eventing.OnCommunicationEvent(emoteEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}