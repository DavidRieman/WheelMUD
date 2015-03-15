//-----------------------------------------------------------------------------
// <copyright file="DefaultState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Minimal implementation of <see cref="SessionState"/>.</summary>
    public class DefaultState : SessionState
    {
        /// <summary>Initializes a new instance of the <see cref="DefaultState"/> class.</summary>
        /// <param name="session">The session entering this state.</param>
        public DefaultState(Session session)
            : base(session)
        {
            this.Prompt = string.Empty;
        }

        /// <summary>Gets or sets the prompt string to be shown by BuildPrompt.</summary>
        /// <value>The prompt.</value>
        public string Prompt { get; set; }

        /// <summary>Gets or sets the prompt builder for the session.</summary>
        /// <returns>The prompt.</returns>
        public override string BuildPrompt()
        {
            return this.Prompt;
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
        }
    }
}