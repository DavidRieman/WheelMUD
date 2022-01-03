//-----------------------------------------------------------------------------
// <copyright file="Die.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>A rollable die.</summary>
    public class Die
    {
        /// <summary>The total number of sides on this die.</summary>
        private int numberSides = 4;

        /// <summary>The random number generator.</summary>
        private readonly Random rand;

        /// <summary>Initializes a new instance of the <see cref="Die"/> class.</summary>
        /// <param name="numberSides">The total sides of the die, IE 4 will yield die rolls of 1 through 4.</param>
        /// <param name="randomGenerator">The random number generator.</param>
        public Die(int numberSides, Random randomGenerator)
        {
            this.numberSides = numberSides;
            rand = randomGenerator;
        }

        /// <summary>Rolls the die and returns the result.</summary>
        /// <returns>The result of the die roll.</returns>
        public int Roll()
        {
            return rand.Next(1, numberSides + 1);
        }
    }
}