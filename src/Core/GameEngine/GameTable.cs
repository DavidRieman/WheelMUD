//-----------------------------------------------------------------------------
// <copyright file="GameTable.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: IRay
//   Date      : 1/17/2005 3:32 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>Determines the function of the tabular data.</summary>
    public enum GameTableFunction
    {
        /// <summary>Attributes table.</summary>
        Attributes = 0,

        /// <summary>Custom table.</summary>
        Custom = 1,

        /// <summary>Lookup table.</summary>
        LookupTable = 2,

        /// <summary>Modifier table.</summary>
        ModifierTable = 3,

        /// <summary>Stat table.</summary>
        StatTable = 4,

        /// <summary>Skills table.</summary>
        SkillTable = 5,

        /// <summary>Rule table.</summary>
        RuleTable = 6,

        /// <summary>Talents table.</summary>
        TalentTable = 7,

        /// <summary>Race table.</summary>
        RaceTable = 8,

        /// <summary>Clan table.</summary>
        ClanTable = 9,

        /// <summary>Profession table.</summary>
        ProfessionTable = 10,

        /// <summary>Gender table.</summary>
        GenderTable = 11
    }

    /// <summary>
    /// This represents a table from a gaming system. These rules can be anything 
    /// related to the current game system. These include Attributes 
    /// (stats and skills), Custom, LookupTables, and ModifierTables.
    /// </summary>
    /// <remarks>
    /// This is to represent a gaming table in the system.
    /// </remarks>
    /// <history>
    ///     [Iray]          1/17/2005   Class Created
    ///     [Fastalanasa]   9/9/2005    xml docs added
    ///     [Fastalanasa]   9/11/2005   Changed name from RuleSet to GameTable.
    ///     [Fastalanasa]   5/15/2009   Converted to C#.
    /// </history>
    /// -----------------------------------------------------------------------------
    public class GameTable
    {
        /// <summary>The delegates that will be used to deal with in-system game tables.</summary>
        ////private HybridDictionary gameTableDelegates;

        /// <summary>Initializes a new instance of the GameTable class.</summary>
        public GameTable()
        {
            this.GameTableEntries = new Dictionary<string, GenericTableEntry>();
            ////this.gameTableDelegates = new HybridDictionary();
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name of the game table.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the game table entries.</summary>
        /// <value>The game table entries.</value>
        public Dictionary<string, GenericTableEntry> GameTableEntries { get; set; }

        /// <summary>Gets or sets the type of the game table.</summary>
        /// <value>The type of the game table.</value>
        public GameTableFunction GameTableType { get; set; }
    }
}