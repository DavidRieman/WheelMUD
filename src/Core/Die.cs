//-----------------------------------------------------------------------------
// <copyright file="Die.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A rollable die.
//   Organized: April 2009 by Karak
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>A rollable die.</summary>
    public class Die
    {
        /// <summary>The total number of sides on this die.</summary>
        private int numberSides = 4;

        /// <summary>The random number generator.</summary>
        private Random rand;

        /// <summary>Initializes a new instance of the <see cref="Die"/> class.</summary>
        /// <param name="numberSides">The total sides of the die, IE 4 will yield die rolls of 1 through 4.</param>
        /// <param name="randomGenerator">The random number generator.</param>
        public Die(int numberSides, ref Random randomGenerator)
        {
            this.numberSides = numberSides;
            this.rand = randomGenerator;
        }

        /// <summary>Rolls the die and returns the result.</summary>
        /// <returns>The result of the die roll.</returns>
        public int Roll()
        {
            return this.rand.Next(1, this.numberSides + 1);
        }
    }
}