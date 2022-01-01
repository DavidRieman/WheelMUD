//-----------------------------------------------------------------------------
// <copyright file="User.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WheelMUD.Data
{
    /// <summary>The base User class used for authentication.</summary>
    /// <remarks>
    /// Should not house any player-character-specific data.
    /// Might be reasonable to store user-specific settings like Telnet settings preferences and such, which
    /// the user would not wish to vary per character when disabling UserAccountIsPlayerCharacter.
    /// </remarks>
    public class User
    {
        private static SHA256Managed SHA256 = new SHA256Managed();
        private static readonly RNGCryptoServiceProvider CryptoProvider = new RNGCryptoServiceProvider();

        public string UserName { get; set; }

        /// <summary>Gets or sets the salt used to help encrypt the passwords.</summary>
        /// <remarks>Set to string instead of byte[] to support serialization.</remarks>
        public string Salt { get; set; }

        /// <summary>Gets or sets the hashed password for this user.</summary>
        /// <remarks>Set to string instead of byte[] to support serialization.</remarks>
        public string HashedPassword { get; private set; }

        public List<string> PlayerCharacterIds { get; } = new List<string>();

        public string LastUsedIPAddress { get; set; }

        public DateTime LastLogInTime { get; set; }

        public DateTime AccountCreationDate { get; set; }

        public UserTelnetSettings TelnetSettings { get; } = new UserTelnetSettings();

        public void SetPassword(string newPassword)
        {
            var salt = new Byte[24];
            CryptoProvider.GetBytes(salt);
            Salt = Encoding.UTF8.GetString(salt);
            HashedPassword = Hash(Salt, newPassword);
        }

        public bool PasswordMatches(string password)
        {
            var hash = Hash(Salt, password);
            return StructuralComparisons.StructuralEqualityComparer.Equals(hash, HashedPassword);
        }

        public static string Hash(string salt, string password)
        {
            var combinedBytes = Encoding.UTF8.GetBytes(salt + password);
            return Encoding.UTF8.GetString(SHA256.ComputeHash(combinedBytes));
        }

        public void AddPlayerCharacter(string playerCharacterId)
        {
            lock (PlayerCharacterIds)
            {
                if (!PlayerCharacterIds.Contains(playerCharacterId))
                {
                    PlayerCharacterIds.Add(playerCharacterId);
                }
            }
        }
    }
}