//-----------------------------------------------------------------------------
// <copyright file="DoorRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single Door row in the Door table.</summary>
    [Alias("Doors")]
    public class DoorRecord : BaseRelationalRecord
    {
        public virtual long DoorSideAID { get; set; }
        public virtual long DoorSideBID { get; set; }
        public virtual long OpenState { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}