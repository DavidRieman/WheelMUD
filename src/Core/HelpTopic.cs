//-----------------------------------------------------------------------------
// <copyright file="HelpTopic.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>A help topic that has been defined in the database.</summary>
    public class HelpTopic
    {
        public HelpTopic(string contents, List<string> aliases)
        {
            Contents = contents;
            Aliases = aliases;
        }

        /// <summary>Gets the contents of this help topic.</summary>
        public string Contents { get; private set; }

        /// <summary>Gets the aliases used for this help topic.</summary>
        /// <remarks>The first alias is the primary alias. E.G. when displaying a topics list, we may choose to display only the first alias for a topic.</remarks>
        public List<string> Aliases { get; private set; }
    }
}