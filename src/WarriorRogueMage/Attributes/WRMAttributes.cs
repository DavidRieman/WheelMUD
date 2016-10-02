//-----------------------------------------------------------------------------
// <copyright file="WRMAttributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Attributes
{
    using WheelMUD.Core;

    /// <summary>Warrior Attribute.</summary>
    [ExportGameAttribute]
    public class WarriorAttribute : WRMAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="WarriorAttribute"/> class.</summary>
        public WarriorAttribute() : base("Warrior", "WAR", "WARRIOR", 0, 0, 12)
        {
        }

        /// <summary>Called when a parent Thing has just been assigned this game element.</summary>
        public override void OnAdd()
        {
            base.OnAdd();
        }
    }

    /// <summary>Rogue Attribute.</summary>
    [ExportGameAttribute]
    public class RogueAttribute : WRMAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="RogueAttribute"/> class.</summary>
        public RogueAttribute() : base("Rogue", "ROG", "ROGUE", 0, 0, 12)
        {
        }
    }

    /// <summary>Mage Attribute.</summary>
    [ExportGameAttribute]
    public class MageAttribute : WRMAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="MageAttribute"/> class.</summary>
        public MageAttribute() : base("Mage", "MAG", "MAGE", 0, 0, 12)
        {
        }
    }
}