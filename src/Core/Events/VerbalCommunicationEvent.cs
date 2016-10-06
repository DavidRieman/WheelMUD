//-----------------------------------------------------------------------------
// <copyright file="VerbalCommunicationEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>Enum to represent a type of communication.</summary>
    public enum VerbalCommunicationType
    {
        /// <summary>Normal speech that can be perceived within the bounds of a room.</summary>
        Say,

        /// <summary>Private communication with a single sender and receiver.</summary>
        Tell,

        /// <summary>Loud communication that can be heard at long distances.</summary>
        Yell,

        /// <summary>A user-defined action perceived to all in the room</summary>
        Emote,
    }

    /// <summary>Event associated with verbal communication.</summary>
    public class VerbalCommunicationEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="VerbalCommunicationEvent"/> class.</summary>
        /// <param name="activeThing">The thing that is communicating.</param>
        /// <param name="sensoryMessage">The sensory message describing the communication to those who can perceive it.</param>
        /// <param name="communicationType">Type of the communication.</param>
        public VerbalCommunicationEvent(
            Thing activeThing,
            SensoryMessage sensoryMessage,
            VerbalCommunicationType communicationType)
            : base(activeThing, sensoryMessage)
        {
            this.CommunicationType = communicationType;
        }

        /// <summary>Gets the type of the communication.</summary>
        /// <value>The type of the communication.</value>
        public VerbalCommunicationType CommunicationType { get; private set; }
    }
}