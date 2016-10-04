//-----------------------------------------------------------------------------
// <copyright file="SessionState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A base class that represents the current state of a session.  IE logging in, 
//   playing, etc.  All data for a connection flows through this class.
//   Created: September 2006 by Foxedup.
// </summary>
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
            this.Session = session;
        }

        /// <summary>Gets or sets the session this state applies to.</summary>
        protected Session Session { get; set; }

        /// <summary>Gets or sets the password.</summary>
        protected string Password { get; set; }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public abstract void ProcessInput(string command);

        /// <summary>Gets or sets the prompt builder for the session.</summary>
        /// <returns>The prompt.</returns>
        public abstract string BuildPrompt();
    }
}