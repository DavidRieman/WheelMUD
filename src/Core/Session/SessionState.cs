//-----------------------------------------------------------------------------
// <copyright file="SessionState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>A base class that represents the current state of a session.</summary>
    /// <remarks>IE logging in, playing, etc. All data for a connection flows through this class.</remarks>
    public abstract class SessionState
    {
        /// <summary>Initializes a new instance of the SessionState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public SessionState(Session session)
        {
            Session = session;
        }

        /// <summary>Gets or sets the session this state applies to.</summary>
        protected Session Session { get; set; }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public abstract void ProcessInput(string command);

        /// <summary>Gets or sets the prompt builder for the session.</summary>
        /// <returns>The prompt.</returns>
        public abstract string BuildPrompt();

        /// <summary>Called when this SessionState has become the active state for a session.</summary>
        /// <remarks>This is the first opportunity to write introductory information for the state (with a prompt, instead of defaulting to only printing a prompt) to the client.</remarks>
        public virtual void Begin()
        {
            Session.WritePrompt();
        }

        /// <summary>Gets a value indicating whether this SessionState supports the Paging system (by responding to commands like "m" or "more").</summary>
        /// <remarks>SessionStates which do not support paging will always send all data regardless of the client's negotiated terminal size.</remarks>
        public virtual bool SupportsPaging { get; } = false;
    }
}