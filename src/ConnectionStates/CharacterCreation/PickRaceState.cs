//-----------------------------------------------------------------------------
// <copyright file="PickRaceState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the player's race.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;

    /// <summary>
    /// The character creation step where the player will pick their race.
    /// </summary>
    public class PickRaceState : CharacterCreationSubState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickRaceState"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public PickRaceState(Session session) : base(session)
        {
        }

        /// <summary>
        /// ProcessInput is used to receive the user input during this state.
        /// </summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
        }
    }
}
