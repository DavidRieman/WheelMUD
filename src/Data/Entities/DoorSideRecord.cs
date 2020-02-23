//-----------------------------------------------------------------------------
// <copyright file="DoorSideRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single DoorSide row in the DoorSide table.</summary>
    [Alias("DoorSides")]
    public class DoorSideRecord : BaseRelationalRecord
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}