//-----------------------------------------------------------------------------
// <copyright file="WrmChargenCommon.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Interfaces;

namespace WarriorRogueMage.CharacterCreation
{
    /// <summary>Common methods for Warrior, Rogue, and Mage character creation.</summary>
    public class WrmChargenCommon
    {
        public static T GetFirstPriorityMatch<T>(string userQuery, IEnumerable<T> collection) where T : INamed
        {
            return (from r in collection
                    where r.Name.StartsWith(userQuery, StringComparison.OrdinalIgnoreCase)
                    select r).FirstOrDefault() ??
                   (from r in collection
                    where r.Name.Contains(userQuery, StringComparison.OrdinalIgnoreCase)
                    select r).FirstOrDefault();
        }
    }
}