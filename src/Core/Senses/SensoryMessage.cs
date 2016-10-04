//-----------------------------------------------------------------------------
// <copyright file="SensoryMessage.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A sense-based message.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections;

    /// <summary>A sense-based message.</summary>
    public class SensoryMessage
    {
        /// <summary>Initializes a new instance of the SensoryMessage class.</summary>
        /// <param name="targetedSense">The sense this message is for.</param>
        /// <param name="messageStrength">The strength of the message.</param>
        /// <param name="message">The message contents.</param>
        /// <param name="context">The context for the view processor to use to render the message.</param>
        public SensoryMessage(SensoryType targetedSense, int messageStrength, ContextualStringBuilder message, Hashtable context)
        {
            this.TargetedSense = targetedSense;
            this.MessageStrength = messageStrength;
            this.Message = message;
            this.Context = context;
        }

        /// <summary>Initializes a new instance of the SensoryMessage class.</summary>
        /// <param name="targetedSense">The sense this message is for.</param>
        /// <param name="messageStrength">The strength of the message.</param>
        /// <param name="message">The message contents.</param>
        public SensoryMessage(SensoryType targetedSense, int messageStrength, ContextualStringBuilder message)
            : this(targetedSense, messageStrength, message, new Hashtable())
        {
        }

        /// <summary>Initializes a new instance of the SensoryMessage class.</summary>
        /// <param name="targetedSense">The sense this message is for.</param>
        /// <param name="messageStrength">The strength of the message.</param>
        public SensoryMessage(SensoryType targetedSense, int messageStrength)
            : this(targetedSense, messageStrength, null, new Hashtable())
        {
        }

        /// <summary>Gets the sense this message is for.</summary>
        public SensoryType TargetedSense { get; private set; }

        /// <summary>Gets the strength of the message.</summary>
        public int MessageStrength { get; private set; }

        /// <summary>Gets the raw message to be processed by sense receptors.</summary>
        public ContextualStringBuilder Message { get; private set; }

        /// <summary>Gets the context for the view processor to use to render the message.</summary>
        public Hashtable Context { get; private set; }
    }
}