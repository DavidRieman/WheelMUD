//-----------------------------------------------------------------------------
// <copyright file="MxpRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single MXP row in the MXP table.</summary>
    [Alias("MXP")]
    public class MxpRecord : BaseRelationalRecord
    {
        public virtual string ElementName { get; set; }
        public virtual string ElementDefinition { get; set; }
    }
}