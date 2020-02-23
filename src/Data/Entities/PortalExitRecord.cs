//-----------------------------------------------------------------------------
// <copyright file="PortalExitRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single PortalExit row in the PortalExit table.</summary>
    [Alias("PortalExits")]
    public class PortalExitRecord : BaseRelationalRecord
    {
        public virtual long PortalID { get; set; }
        public virtual long RoomAID { get; set; }
        public virtual long RoomBID { get; set; }
    }
}