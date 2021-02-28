//-----------------------------------------------------------------------------
// <copyright file="PlayerRepositoryExtensions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System;
    using System.Linq;

    public static class PlayerRepositoryExtensions
    {
        public static User Authenticate(string userName, string password)
        {
            using var session = Helpers.OpenDocumentSession();
            var salt = (from u in session.Query<User>()
                        where u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                        select u.Salt).FirstOrDefault();
            if (salt == null)
            {
                return null;
            }
            var hashedPassword = User.Hash(salt, password);
            return (from u in session.Query<User>()
                    where u.UserName.Equals(userName) &&
                          u.HashedPassword.Equals(hashedPassword)
                    select u).FirstOrDefault();
        }

        public static bool UserNameExists(string userName)
        {
            using var session = Helpers.OpenDocumentSession();
            return (from u in session.Query<User>()
                    where u.UserName.Equals(userName)
                    select u).Any();
        }
    }
}