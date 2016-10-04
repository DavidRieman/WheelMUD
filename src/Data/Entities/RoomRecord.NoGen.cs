//-----------------------------------------------------------------------------
// <copyright file="RoomRecord.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/3/2009 9:31:13 AM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using System.Collections.Generic;

    /// <summary>Custom code for the RoomRecord class.</summary>
    public partial class RoomRecord
    {
        /// <summary>Initializes a new instance of the <see cref="RoomRecord"/> class.</summary>
        public RoomRecord()
        {
            //this.Exits = new Dictionary<string, Exit>();
        }

        /// <summary>Gets the exits within the room.</summary>
        //public virtual Dictionary<string, Exit> Exits { get; private set; }
    }
}