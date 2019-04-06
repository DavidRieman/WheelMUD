//-----------------------------------------------------------------------------
// <copyright file="GenericTableEntry.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   TODO: Add summary
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Rules;

    /// <summary>Defines custom game tables that don't fit under the set game tables or other game engine classes.</summary>
    public class GenericTableEntry
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<GameRule> Rules { get; set; }
    }
}