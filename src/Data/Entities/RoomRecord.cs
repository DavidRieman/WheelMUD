//-----------------------------------------------------------------------------
// <copyright file="RoomRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single Room row in the Room table.</summary>
    [Alias("Rooms")]
    public class RoomRecord : BaseRelationalRecord
    {
        public virtual string UID { get; set; }
        public virtual long AreaID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual long RoomTypeID { get; set; }
    }
}