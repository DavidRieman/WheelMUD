//-----------------------------------------------------------------------------
// <copyright file="PlayerIpAddressRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single PlayerIPAddress row in the PlayerIPAddress table.</summary>
    [Alias("PlayerIPAddress")]
    public class PlayerIpAddressRecord : BaseRelationalRecord
    {
        public virtual long PlayerID { get; set; }
        public virtual string IPAddress { get; set; }
    }
}