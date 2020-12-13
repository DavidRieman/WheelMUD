//-----------------------------------------------------------------------------
// <copyright file="ExitRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single Exit row in the Exit table.</summary>
    [Alias("Exits")]
    public class ExitRecord : BaseRelationalRecord
    {
        public virtual long ExitRoomAID { get; set; }
        public virtual string DirectionA { get; set; }
        public virtual long ExitRoomBID { get; set; }
        public virtual string DirectionB { get; set; }
        public virtual long DoorID { get; set; }
    }
}