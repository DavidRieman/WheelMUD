//-----------------------------------------------------------------------------
// <copyright file="RoleRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/18/2009 1:17:01 AM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Data;
    using System.Linq;
    using System.Text;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Custom code for the RoleRepository class.</summary>
    public partial class RoleRepository
    {
        /// <summary>Gets a role record that is associated with the role name.</summary>
        /// <param name="roleName">The user name to look up a role.</param>
        /// <returns>Returns a role record loaded with the role's data.</returns>
        public RoleRecord GetRoleByName(string roleName)
        {
            RoleRecord roleRecord;

            using (IDbCommand session = Helpers.OpenSession())
            {
                if (Helpers.GetCurrentProviderName().ToLower() == "system.data.sqlite")
                {
                    var sql = new StringBuilder();

                    sql.Append("SELECT * FROM Roles ");
                    sql.Append("WHERE Name = {0} ");
                    sql.Append(" COLLATE NOCASE ");

                    roleRecord = session.Connection.Select<RoleRecord>(sql.ToString(), roleName).First();
                }
                else
                {
                    roleRecord = session.Connection.Select<RoleRecord>("Name = {0}", roleName).First();
                }
            }

            return roleRecord;
        }
    }
}