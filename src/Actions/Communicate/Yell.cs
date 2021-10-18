//-----------------------------------------------------------------------------
// <copyright file="Yell.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Allows a player to communicate across several rooms to all entities.</summary>
    /// <remarks>yell hey you!</remarks>
    [ExportGameAction(0)]
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
            var parent = actionInput.Actor.Parent;

            // This is to keep track of the previous rooms we've yelled at, to prevent echoes.
            var previousRooms = new List<Thing>();

            CreateYellEvent(actionInput.Actor);
            TraverseRoom(parent, actionInput, actionInput.Actor, RoomFallOff, previousRooms);
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

            yellSentence = actionInput.Tail.Trim();

            return null;
        }

        /// <summary>This recursively traverses the rooms, stopping when TTL reaches 0, calling YellAtRoom at each stop.</summary>
        /// <remarks>
        /// TODO: This traversal logic should be centralized for reuse, taking an origin place, function, and options like whether
        ///       to go through closed exits.
        /// </remarks>
        /// <param name="place">The current place (generally a room) to work from.</param>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <param name="sender">The entity that yelled.</param>
        /// <param name="timeToLive">How many rooms to traverse, -1 has no stop.</param>
        /// <param name="visitedPlaces">List of previous rooms that have been yelled at. (No one likes to be screamed at twice.)</param>
        private void TraverseRoom(Thing place, ActionInput actionInput, Thing yeller, int timeToLive, List<Thing> visitedPlaces)
        {
            if (timeToLive == 0 || visitedPlaces.Contains(place))
            {
                return;
            }

            // Track that we've now traversed to this place.
            visitedPlaces.Add(place);

            // Broadcast the yell request at the specified place.
            place.Eventing.OnCommunicationRequest(yellEvent, EventScope.SelfDown);
            if (!yellEvent.IsCancelled)
            {
                var exits = place.FindAllChildrenBehaviors<ExitBehavior>();
                var destinations = from exit in exits
                                   let opensClosesBehavior = exit.Parent.FindBehavior<OpensClosesBehavior>()
                                   where opensClosesBehavior == null || opensClosesBehavior.IsOpen
                                   select exit.GetDestination(place);
                foreach (var destination in destinations)
                {
                    TraverseRoom(destination, actionInput, yeller, (timeToLive == -1) ? timeToLive : (timeToLive - 1), visitedPlaces);
                }

                // TODO: Consider traversing into portal destinations and the like?
                // TODO: AmbientSenseBehavior or DrownSenseBehavior or whatnot, so one can make an ambient
                //     noise which drowns out the tail end of a quieting yell under the ambient noise, etc.
            }
            else
            {
                // Create a new non-cancelled yell event; just because something suppressed part of the yell 
                // doesn't necessarily mean all other branches of the yell should be suppressed too.  IE if
                // something to the west of our position prevents noise from going through there, the noise 
                // that was also going northwards shouldn't suddenly stop.
                CreateYellEvent(yeller);
            }
        }

        private void CreateYellEvent(Thing yeller)
        {
            var contextMessage = new ContextualString(yeller, null)
            {
                ToOriginator = $"You yell: {yellSentence}",
                ToReceiver = $"You hear {yeller.Name} yell: {yellSentence}",
                ToOthers = $"You hear {yeller.Name} yell: {yellSentence}",
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            yellEvent = new VerbalCommunicationEvent(yeller, sm, VerbalCommunicationType.Yell);
        }
    }
}