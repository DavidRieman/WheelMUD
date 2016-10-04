//-----------------------------------------------------------------------------
// <copyright file="HelpTopicRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: bengecko December 2009
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Specific calls that are not auto generated.</summary>
    public partial class HelpTopicRepository
    {
    }

    /// <summary>Specific calls that are not auto generated.</summary>
    public partial class HelpTopicAliasRepository
    {

        /// <summary>Loads the alias entries for a given help topic.</summary>
        /// <param name="helpTopicId">The ID of the parent help topic</param>
        /// <returns>List of Help Topic Alias records</returns>
        public List<HelpTopicAliasRecord> LoadAliasForTopic(long helpTopicId)
        {
            string sql = @"SELECT * 
                           FROM HelpTopicAliases 
                           WHERE HelpTopicID = {0}";

            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<HelpTopicAliasRecord>(sql, helpTopicId);
            }
        }
    }
}
