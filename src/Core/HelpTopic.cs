//-----------------------------------------------------------------------------
// <copyright file="HelpTopic.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//      Created By: bengecko December 2009  
//      Modified by: Pure October 2010
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>A help topic that has been defined in the database.</summary>
    public class HelpTopic
    {
        public HelpTopic(string contents, List<string> aliases)
        {
            this.Contents = contents;
            this.Aliases = aliases;
        }

        /// <summary>Gets the aliases used for this help topic.</summary>
        public string Contents { get; private set; }

        /// <summary>Gets the contents of this help topic.</summary>
        public List<string> Aliases { get; private set; }
    }
}