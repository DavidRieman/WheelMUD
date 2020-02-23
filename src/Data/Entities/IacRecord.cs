//-----------------------------------------------------------------------------
// <copyright file="IacRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single IAC row in the IAC table.</summary>
    [Alias("IAC")]
    public class IacRecord : BaseRelationalRecord
    {
        public virtual string Name { get; set; }
        public virtual int OptionCode { get; set; }
        public virtual bool NegotiateAtConnect { get; set; }
        public virtual bool RequiresSubNegotiation { get; set; }
        public virtual string SubNegAssembly { get; set; }
        public virtual string NegotiationStartValue { get; set; }
    }
}