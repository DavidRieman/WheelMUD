﻿//-----------------------------------------------------------------------------
// <copyright file="PlayerRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using ServiceStack.DataAnnotations;

namespace WheelMUD.Data.Entities
{
    /// <summary>Represents a single Player row in the Player table.</summary>
    [Alias("Players")]
    public class PlayerRecord : BaseRelationalRecord
    {
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Suffix { get; set; }
        public virtual string Prefix { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int Age { get; set; }
        public virtual string CreateDate { get; set; }
        public virtual long CurrentRoomID { get; set; }
        public virtual bool WantAnsi { get; set; }
        public virtual bool WantMXP { get; set; }
        public virtual bool WantMCCP { get; set; }
        public virtual string LastLogin { get; set; }
        public virtual string LastLogout { get; set; }
        public virtual string LastIPAddress { get; set; }
        public virtual string Email { get; set; }
        public virtual string HomePage { get; set; }
        public virtual string PlanText { get; set; }
        public virtual int BufferLength { get; set; }
    }
}