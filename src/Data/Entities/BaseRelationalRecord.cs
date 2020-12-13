//-----------------------------------------------------------------------------
// <copyright file="BaseRelationalRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>A base class for entity records.</summary>
    public class BaseRelationalRecord
    {
        /// <summary>The unique ID for this record. Receives an automatically incremented value from the DB.</summary>
        [AutoIncrement]
        public virtual long ID { get; set; }
    }
}
