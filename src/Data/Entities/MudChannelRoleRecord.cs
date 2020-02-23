//-----------------------------------------------------------------------------
// <copyright file="MudChannelRoleRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single MudChannelRole row in the MudChannelRole table.</summary>
    [Alias("MudChannelRoles")]
    public class MudChannelRoleRecord : BaseRelationalRecord
    {
        public virtual long MudChannelID { get; set; }
        public virtual long RoleID { get; set; }
    }
}