//-----------------------------------------------------------------------------
// <copyright file="RoomTypeRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single RoomType row in the RoomType table.</summary>
    [Alias("RoomTypes")]
    public class RoomTypeRecord : BaseRelationalRecord
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}