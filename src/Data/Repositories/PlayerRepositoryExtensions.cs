//-----------------------------------------------------------------------------
// <copyright file="PlayerRepositoryExtensions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;

namespace WheelMUD.Data.Repositories
{
    public static class PlayerRepositoryExtensions
    {
        /// <summary>Determines if the password is sufficent to authenticate the login.</summary>
        /// <param name="loadedUser">A single loaded User instance to check against.</param>
        /// <param name="password">The password to check.</param>
        /// <returns>The User instance if authenticated, else null.</returns>
        public static User Authenticate(User loadedUser, string password)
        {
            return loadedUser.PasswordMatches(password) ? loadedUser : null;
        }

        public static User Authenticate(string userName, string password)
        {
            var targetId = $"user/{userName.ToLower()}";
            using var session = Helpers.OpenDocumentSession();
            var salt = (from u in session.Query<User>()
                        where u.Id.Equals(targetId)
                        select u.Salt).FirstOrDefault();
            if (salt == null)
            {
                return null;
            }
            var hashedPassword = User.Hash(salt, password);
            return (from u in session.Query<User>()
                    where u.Id.Equals(targetId) &&
                          u.HashedPassword.Equals(hashedPassword)
                    select u).FirstOrDefault();
        }

        public static bool UserExists(string userName)
        {
            using var session = Helpers.OpenDocumentSession();
            var targetId = $"user/{userName.ToLower()}";
            return (from u in session.Query<User>()
                    where u.Id == targetId
                    select u).Any();
        }
    }
}