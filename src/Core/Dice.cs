//-----------------------------------------------------------------------------
// <copyright file="Dice.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>DiceService provides a dice system (random number generator) to the world.</summary>
    public class DiceService
    {
        /// <summary>The random number generator.</summary>
        private Random Random { get; } = new Random();

        /// <summary>Prevents a default instance of the DiceService class from being created.</summary>
        private DiceService()
        {
        }

        /// <summary>Gets the singleton instance of the DiceService.</summary>
        public static DiceService Instance { get; } = new DiceService();

        /// <summary>Creates a new die.</summary>
        /// <param name="numSides">The number of sides the die has.</param>
        /// <returns>A new die object.</returns>
        public Die GetDie(int numSides)
        {
            return new Die(numSides, Random);
        }
    }
}