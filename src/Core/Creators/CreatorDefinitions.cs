//-----------------------------------------------------------------------------
// <copyright file="CreatorDefinitions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Provides definitions for the set of easily-overridden world content creators.</summary>
    public static class CreatorDefinitions
    {
        public abstract class Area
        {
            public abstract string ID { get; }
            public abstract Thing Create();
        }

        /// <summary>Creates the base World, if there is none.</summary>
        /// <remarks>
        /// A World creator should only need to be called once, when there is no World yet in the DB.
        /// See DefaultWorldCreator for the default implementation, which should get overridden by game systems.
        /// </remarks>
        public abstract class World
        {
            public abstract Thing Create();
        }
    }
}
