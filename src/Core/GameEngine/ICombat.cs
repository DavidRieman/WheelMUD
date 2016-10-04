//-----------------------------------------------------------------------------
// <copyright file="ICombat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : Jan 14, 2012
//   Purpose   : Generic interface for a combat system in a gaming system.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Generic interface for a combat system in a gaming system.</summary>
    public interface ICombat
    {
        GameCombatSession CombatSession { get; set; }

        /// <summary>To be used in combat systems that are turn based.</summary>
        void ProcessCombatRound();

        /// <summary>To be used in combat systems that are near-real time.</summary>
        void ProcessCombatActions();

        void CreateCombatOrder();

        void ProcessCombatantRoundActions(Thing combatant);
    }
}
