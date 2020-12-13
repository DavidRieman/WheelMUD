//-----------------------------------------------------------------------------
// <copyright file="MobRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;
    using System;

    /// <summary>Represents a single Mob row in the Mob table.</summary>
    [Alias("Mobs")]
    public class MobRecord : BaseRelationalRecord
    {
        public virtual int MobTypeID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int Age { get; set; }
        public virtual int CurrentRoomID { get; set; }
        public virtual string Prompt { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}