//-----------------------------------------------------------------------------
// <copyright file="DayNameRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single DayName row in the DayName table.</summary>
    [Alias("DayNames")]
    public class DayNameRecord : BaseRelationalRecord
    {
        public virtual string DayName { get; set; }
    }
}