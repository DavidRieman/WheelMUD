//-----------------------------------------------------------------------------
// <copyright file="MudChannelRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single MudChannel row in the MudChannel table.</summary>
    [Alias("MudChannels")]
    public class MudChannelRecord : BaseRelationalRecord
    {
        public virtual string MudChannelName { get; set; }
    }
}