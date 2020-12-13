//-----------------------------------------------------------------------------
// <copyright file="BannedIpAddressRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;
    using System;

    /// <summary>Represents a single BannedIPAddress row in the BannedIPAddress table.</summary>
    [Alias("BannedIPAddresses")]
    public class BannedIpAddressRecord : BaseRelationalRecord
    {
        public virtual string StartIPAddress { get; set; }
        public virtual string EndIPAddress { get; set; }
        public virtual string Note { get; set; }
        public virtual long BannedByPlayerID { get; set; }
        public virtual DateTime BannedDateTime { get; set; }
    }
}