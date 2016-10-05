//-----------------------------------------------------------------------------
// <copyright file="Yell.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by Shabubu on 4/28/2009
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>Allows a player to communicate across several rooms to all entities.</summary>
    /// <remarks>yell hey you!</remarks>
    [ExportGameAction]
    [ActionPrimaryAlias("yell", CommandCategory.Communicate)]
    [ActionAlias("shout", CommandCategory.Communicate)]
    [ActionDescription("Yell something to the surrounding area.")]
    [ActionSecurity(SecurityRole.player)]
    public class Yell : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Falloff value, yell will stop after so many rooms have been yelled at, set to -1 to hit everything</summary>
        /// <remarks>Should be mentioned, that this builds a list of previous rooms, so too large of a world with -1 will lead to memory issues.</remarks>
        private static readonly int RoomFallOff = 5;

        /// <summary>The sensory event that is fired into each room.</summary>
        private VerbalCommunicationEvent yellEvent;

        private string yellSentence;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing parent = sender.Thing.Parent;

            // This is to keep track of the previous rooms we've yelled at, to prevent echoes.
            List<Thing> previousRooms = new List<Thing>();

            this.CreateYellEvent(sender.Thing);
            this.TraverseRoom(parent, actionInput, sender, RoomFallOff, previousRooms);
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

            this.yellSentence = actionInput.Tail.Trim();

            return null;
        }

        /// <summary>This recursively traverses the rooms, stopping when TTL reaches 0, calling YellAtRoom at each stop.</summary>
        /// <remarks>
        /// @@@ TODO: This traversal logic should be centralized for reuse, taking an origin place, function, and options like whether
        ///     to go through closed exits. See 'Future Potential' at: https://wheelmud.codeplex.com/wikipage?title=Sensory%20Messages
        /// </remarks>
        /// <param name="place">The current place (generally a room) to work from.</param>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <param name="sender">The entity that yelled.</param>
        /// <param name="timeToLive">How many rooms to traverse, -1 has no stop.</param>
        /// <param name="visitedPlaces">List of previous rooms that have been yelled at. (No one likes to be screamed at twice.)</param>
        private void TraverseRoom(Thing place, ActionInput actionInput, IController sender, int timeToLive, List<Thing> visitedPlaces)
        {
            if (timeToLive == 0 || visitedPlaces.Contains(place))
            {
                return;
            }

            // Track that we've now traversed to this place.
            visitedPlaces.Add(place);

            // Broadcast the yell request at the specified place.
            place.Eventing.OnCommunicationRequest(this.yellEvent, EventScope.SelfDown);
            if (!this.yellEvent.IsCancelled)
            {
                List<ExitBehavior> exits = place.FindAllChildrenBehaviors<ExitBehavior>();
                foreach (ExitBehavior exit in exits)
                {
                    var opensClosesBehavior = exit.Parent.Behaviors.FindFirst<OpensClosesBehavior>();
                    if (opensClosesBehavior == null || opensClosesBehavior.IsOpen == true)
                    {
                        Thing destination = exit.GetDestination(place);
                        this.TraverseRoom(destination, actionInput, sender, (timeToLive == -1) ? timeToLive : (timeToLive - 1), visitedPlaces);
                    }
                }

                // @@@ TODO: Consider traversing into portal destinations and the like?
                // @@@ TODO: AmbientSenseBehavior or DrownSenseBehavior or whatnot, so one can make an ambient
                //     noise which drowns out the tail end of a quieting yell under the ambient noise, etc.
            }
            else
            {
                // Create a new non-cancelled yell event; just because something suppressed part of the yell 
                // doesn't necessarily mean all other branches of the yell should be suppressed too.  IE if
                // something to the west of our position prevents noise from going through there, the noise 
                // that was also going northwards shouldn't suddenly stop.
                this.CreateYellEvent(sender.Thing);
            }
        }

        private void CreateYellEvent(Thing entity)
        {
            var contextMessage = new ContextualString(entity, null)
            {
                ToOriginator = "You yell: " + this.yellSentence,
                ToReceiver = "You hear $ActiveThing.Name yell: " + this.yellSentence,
                ToOthers = "You hear $ActiveThing.Name yell: " + this.yellSentence,
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            this.yellEvent = new VerbalCommunicationEvent(entity, sm, VerbalCommunicationType.Yell);
        }
    }
}