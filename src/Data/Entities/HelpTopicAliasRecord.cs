//-----------------------------------------------------------------------------
// <copyright file="HelpTopicAliasRecord.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    using ServiceStack.DataAnnotations;

    /// <summary>Represents a single HelpTopicAlias row in the HelpTopicAlias table.</summary>
    [Alias("HelpTopicAliases")]
    public class HelpTopicAliasRecord : BaseRelationalRecord
    {
        public virtual string HelpTopicAlias { get; set; }
        public virtual long HelpTopicID { get; set; }
    }
}