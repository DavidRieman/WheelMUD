//-----------------------------------------------------------------------------
// <copyright file="Dice.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Provides a dice system (random number generator) to the world.
//   Created: October 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>DiceService provides a dice system (random number generator) to the world.</summary>
    public class DiceService
    {
        /// <summary>The synchronization locking object.</summary>
        private static readonly object lockObject = new object();
        
        /// <summary>The singleton instance of the DiceService.</summary>
        private static DiceService instance = null;

        /// <summary>The random number generator.</summary>
        private static Random rand;

        /// <summary>Prevents a default instance of the DiceService class from being created.</summary>
        private DiceService()
        {
        }

        /// <summary>Gets the singleton instance of the DiceService.</summary>
        public static DiceService Instance
        {
            get
            {
                lock (DiceService.lockObject)
                {
                    if (DiceService.instance == null)
                    {
                        DiceService.instance = new DiceService();
                        DiceService.rand = new Random();
                    }
                    
                    return DiceService.instance;
                }
            }
        }

        /// <summary>Creates a new die.</summary>
        /// <param name="numSides">The number of sides the die has.</param>
        /// <returns>A new die object.</returns>
        public Die GetDie(int numSides)
        {
            return new Die(numSides, ref rand);
        }
    }
}