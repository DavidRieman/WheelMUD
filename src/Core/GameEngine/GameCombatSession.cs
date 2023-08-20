//-----------------------------------------------------------------------------
// <copyright file="GameCombatSession.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections;

namespace WheelMUD.Core
{
    /// <summary>Implements the mechanism to deal with combat rounds/turns.</summary>
    public class GameCombatSession
    {
        /// <summary>Keeps a list of all the members of this combat session.</summary>
        private readonly Hashtable combatantHashtable = new();

        /// <summary>Gets the entities that are part of this combat session.</summary>
        public Hashtable Combatants
        {
            get { return combatantHashtable; }
        }

        /// <summary>Adds a combatant to this session.</summary>
        /// <param name="combatant">The Entity that needs to be added.</param>
        public void AddCombatant(ref Thing combatant)
        {
            combatantHashtable.Add(combatant.Name, combatant);
        }

        /// <summary>Remove a combatant from this session.</summary>
        /// <param name="combatant">The Entity that needs to be removed.</param>
        public void RemoveCombatant(ref Thing combatant)
        {
            combatantHashtable.Remove(combatant.Name);
        }
    }
}