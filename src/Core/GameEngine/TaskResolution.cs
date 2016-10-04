//-----------------------------------------------------------------------------
// <copyright file="TaskResolution.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : June 5, 2011
//   Purpose   : This class will handle resolution of contests
//               using skills, talents, and other things.
// </summary>
//-----------------------------------------------------------------------------
/* Disabled for now: Seems unused, and raised errors:
     TaskResolution.cs(24,23): error CS0414: Warning as Error: The private field `WheelMUD.Core.TaskResolution.contestantA' is assigned but its value is never used
     TaskResolution.cs(26,23): error CS0414: Warning as Error: The private field `WheelMUD.Core.TaskResolution.contestantB' is assigned but its value is never used
namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>This class will handle resolution of contests using skills, talents, and other things.</summary>
    public class TaskResolution
    {
        private Thing contestantA;

        private Thing contestantB;

        public TaskResolution(Thing contestantA, Thing contestantB)
        {
            this.contestantA = contestantA;
            this.contestantB = contestantB;
        }

        /// <summary>Resolves the contest using the specified attributes.</summary>
        /// <param name="attributes">The <see cref="GameAttribute"/>s to use for this contest.</param>
        /// <returns>Returns the winning thing.</returns>
        public Thing Resolve(List<GameAttribute> attributes)
        {
            return null;
        }

        /// <summary>Resolves the contest using the specified stats.</summary>
        /// <param name="stats">The <see cref="GameStat"/>s to use for this contest.</param>
        /// <returns>Returns the winning thing.</returns>
        public Thing Resolve(List<GameStat> stats)
        {
            return null;
        }

        /// <summary>Resolves the contest using the specified skills.</summary>
        /// <param name="stats">The <see cref="GameSkill"/>s to use for this contest.</param>
        /// <returns>Returns the winning thing.</returns>
        public Thing Resolve(List<GameSkill> stats)
        {
            return null;
        }

        /// <summary>
        /// Resolves the contest using the specified formulas. This method will to make use of
        /// NCalc to evaluate the formulas against each other. It will look through the different
        /// lists of game attributes for each contestant.
        /// </summary>
        /// <param name="contestantAFormula">The contestant's A formula.</param>
        /// <param name="contestantBFormula">The contestant's B formula.</param>
        /// <returns>Returns the winning thing.</returns>
        public Thing Resolve(string contestantAFormula, string contestantBFormula)
        {
            return null;
        }
    }
}*/