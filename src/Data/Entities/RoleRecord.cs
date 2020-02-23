//-----------------------------------------------------------------------------
// <copyright file="RoleRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single Role row in the Role table.</summary>
    [Alias("Roles")]
    public class RoleRecord : BaseRelationalRecord
    {
        public virtual string Name { get; set; }
        public virtual int SecurityRoleMask { get; set; }
    }
}