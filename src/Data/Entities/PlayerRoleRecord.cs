//-----------------------------------------------------------------------------
// <copyright file="PlayerRoleRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using ServiceStack.DataAnnotations;

namespace WheelMUD.Data.Entities
{
    /// <summary>Represents a single PlayerRole row in the PlayerRole table.</summary>
    [Alias("PlayerRoles")]
    public class PlayerRoleRecord : BaseRelationalRecord
    {
        public virtual long PlayerID { get; set; }
        public virtual long RoleID { get; set; }
    }
}