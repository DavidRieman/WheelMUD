//-----------------------------------------------------------------------------
// <copyright file="Tell.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to send a message directly to one entity.
//   Created: January 2007 by Saquivor.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>A command to send a message directly to one entity.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("tell", CommandCategory.Communicate)]
    [ActionAlias("t", CommandCategory.Communicate)]
    [ActionDescription("Tell something to a character or monster.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Tell : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>The found target object.</summary>
        private Thing target = null;

        /// <summary>Sentence to tell to player, stored globally so as to not build twice.</summary>
        private string sentence;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <remarks>TODO: ability to block via ear muffs, send tell to NPC? Also remote rtell via @</remarks>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string fixedSentence = "\"<%yellow%>" + this.sentence + "<%n%>\"";
            var contextMessage = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = this.BuildOriginatorMessage(fixedSentence),
                ToReceiver = string.Format("{0} tells you: {1}", sender.Thing.Name, fixedSentence),
                ToOthers = null,
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            var tellEvent = new VerbalCommunicationEvent(sender.Thing, sm, VerbalCommunicationType.Tell);
            
            // Make sure both the user is allowed to do the tell and the target is allowed to receive it.
            this.target.Eventing.OnCommunicationRequest(tellEvent, EventScope.SelfDown);
            sender.Thing.Eventing.OnCommunicationRequest(tellEvent, EventScope.SelfDown);
            if (!tellEvent.IsCancelled)
            {
                // Printing the sensory message is all that's left to do, which the event itself will take care of.
                this.target.Eventing.OnCommunicationEvent(tellEvent, EventScope.SelfDown);
                sender.Thing.Eventing.OnCommunicationEvent(tellEvent, EventScope.SelfDown);
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Changing strings here seem to cause casting error?
            // string targetName = command.Action.Tail.Trim().ToLower();
            string targetName = actionInput.Params[0].Trim().ToLower();

            // TODO: InterMUD support? string mudName = MudEngineAttributes.Instance.MudName;

            // Rule: Target must be an entity. @@@ REMOVE CHECK?
            this.target = GameAction.GetPlayerOrMobile(targetName);
            if (this.target == null)
            {
                // Make first char Upper?  IE textInfo.ToTitleCase.
                return targetName + " is not part of reality.";
            }

            //// TODO what if player offline ?

            // Rule: Prevent talking to yourself.
            if (actionInput.Controller.Thing.Name.ToLower() == this.target.Name.ToLower())
            {
                return "Talking to yourself is the first sign of madness!";
            }

            // Rule: Give help if too few Params, having problems with the causing casting errors ?
            if (actionInput.Params.Length <= 1)
            {
                return "Syntax:\n<tell [player] [message]>\n\nSee also say, yell, shout, emote.";
            }

            // The sentence to be relayed consists of all input beyond the first parameter, 
            // which was used to identify this.entity already.
            int firstCommandLength = actionInput.Params[0].Length;
            this.sentence = actionInput.Tail.Substring(firstCommandLength).Trim();
            
            return null;
        }

        /// <summary>Builds the message the sender sees on the screen, and deals with AFK stuff.</summary>
        /// <param name="fixedSentence">The base fixed sentence.</param>
        /// <returns>The final string the sender of the tell will see.</returns>
        private string BuildOriginatorMessage(string fixedSentence)
        {
            PlayerBehavior playerBehavior = this.target.Behaviors.FindFirst<PlayerBehavior>();

            if (playerBehavior != null && playerBehavior.IsAFK)
            {
                string afkMessage;

                if (!string.IsNullOrEmpty(playerBehavior.AFKReason))
                {
                    afkMessage = string.Concat("AFK: ", playerBehavior.AFKReason);
                }
                else
                {
                    afkMessage = "AFK";
                }

                return string.Format("You tell {0}: {1}<%nl%>{0} is {2}.", this.target.Name, fixedSentence, afkMessage);
            }
            else
            {
                return string.Format("You tell {0}: {1}", this.target.Name, fixedSentence);
            }
        }
    }
}