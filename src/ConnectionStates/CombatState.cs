//-----------------------------------------------------------------------------
// <copyright file="CombatState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : April 23, 2012
//   Purpose   : Changes the state of the player, so that the MUD engine can
//               do combat.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;

    /// <summary>The player state for handling combat.</summary>
    public class CombatState : SessionState
    {
        /// <summary>Initializes a new instance of the <see cref="CombatState"/> class.</summary>
        /// <param name="session">The session entering this state.</param>
        public CombatState(Session session) : base(session)
        {
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            this.Session.AtPrompt = false;
            if (command != string.Empty)
            {
                var actionInput = new ActionInput(command, this.Session);
                this.Session.ExecuteAction(actionInput);
            }
            else
            {
                this.Session.SendPrompt();
            }
        }

        public override string BuildPrompt()
        {
            return "> ";
        }
    }
}