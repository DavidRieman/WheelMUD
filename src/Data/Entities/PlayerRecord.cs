//-----------------------------------------------------------------------------
// <copyright file="PlayerRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Data.Entities
{
    /// <summary>Represents a single Player row in the Player table.</summary>
    public class PlayerRecord
    {
        public virtual string DisplayName { get; set; }
        public virtual string Suffix { get; set; }
        public virtual string Prefix { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int Age { get; set; }
        public virtual string CreateDate { get; set; }
        public virtual long CurrentRoomID { get; set; }
        public virtual string Email { get; set; }
        public virtual int BufferLength { get; set; }
    }
}