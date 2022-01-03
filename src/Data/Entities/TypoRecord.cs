//-----------------------------------------------------------------------------
// <copyright file="TypoRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using ServiceStack.DataAnnotations;

namespace WheelMUD.Data.Entities
{
    /// <summary>Represents a single Typo row in the Typo table.summary>
    [Alias("Typos")]
    public class TypoRecord : BaseRelationalRecord
    {
        public virtual string Note { get; set; }
        public virtual long SubmittedByPlayerID { get; set; }
        public virtual long RoomID { get; set; }
        public virtual string SubmittedDateTime { get; set; }
        public virtual bool Resolved { get; set; }
        public virtual long ResolvedByPlayerID { get; set; }
        public virtual string ResolvedDateTime { get; set; }
    }
}