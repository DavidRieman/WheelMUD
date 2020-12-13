//-----------------------------------------------------------------------------
// <copyright file="PlayerChannelRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single PlayerChannel row in the PlayerChannel table.</summary>
    [Alias("PlayerChannels")]
    public class PlayerChannelRecord : BaseRelationalRecord
    {
        public virtual long PlayerID { get; set; }
        public virtual long ChannelID { get; set; }
    }
}