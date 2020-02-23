//-----------------------------------------------------------------------------
// <copyright file="RoomVisualRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single RoomVisual row in the RoomVisual table.</summary>
    [Alias("RoomVisuals")]
    public class RoomVisualRecord : BaseRelationalRecord
    {
        public virtual long RoomID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}