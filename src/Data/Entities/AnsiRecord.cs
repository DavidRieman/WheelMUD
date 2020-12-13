//-----------------------------------------------------------------------------
// <copyright file="AnsiRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single ANSI row in the ANSI table.</summary>
    [Alias("ANSI")]
    public class AnsiRecord : BaseRelationalRecord
    {
        public virtual string EscapeCode { get; set; }
        public virtual string Tag { get; set; }
    }
}