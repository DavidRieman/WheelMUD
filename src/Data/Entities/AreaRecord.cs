//-----------------------------------------------------------------------------
// <copyright file="AreaRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;
    using System.Collections.Generic;

    /// <summary>Represents a single Area row in the Area table.</summary>
    [Alias("Areas")]
    public class AreaRecord : BaseRelationalRecord
    {
        /// <summary>Initializes a new instance of the AreaRecord class.</summary>
        public AreaRecord()
        {
            Rooms = new Dictionary<long, RoomRecord>();
        }

        public virtual string Name { get; set; }

        /// <summary>Gets or sets the rooms within the area.</summary>
        [Ignore]
        public virtual Dictionary<long, RoomRecord> Rooms { get; set; }
    }
}