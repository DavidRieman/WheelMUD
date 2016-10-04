//-----------------------------------------------------------------------------
// <copyright file="AreaRecord.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/2/2009 9:01:30 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using System.Collections.Generic;
    using ServiceStack.DataAnnotations;

    /// <summary>Custom code for the AreaRecord class.</summary>
    public partial class AreaRecord
    {
        /// <summary>Initializes a new instance of the AreaRecord class.</summary>
        public AreaRecord()
        {
            this.Rooms = new Dictionary<long, RoomRecord>();
        }

        /// <summary>Gets or sets the rooms within the area.</summary>
        [Ignore]
        public virtual Dictionary<long, RoomRecord> Rooms { get; set; }
    }
}