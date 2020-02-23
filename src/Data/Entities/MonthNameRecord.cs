//-----------------------------------------------------------------------------
// <copyright file="MonthNameRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single MonthName row in the MonthName table.</summary>
    [Alias("MonthNames")]
    public class MonthNameRecord : BaseRelationalRecord
    {
        public virtual string MonthName { get; set; }
    }
}