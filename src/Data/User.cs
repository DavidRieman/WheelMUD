//-----------------------------------------------------------------------------
// <copyright file="User.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Data
{
    /// <summary>The base User class used for authentication.</summary>
    /// <remarks>
    /// Should not house any player-character-specific data.
    /// Might be reasonable to store user-specific settings like Telnet settings preferences and such, which
    /// the user would not wish to vary per character when disabling UserAccountIsPlayerCharacter.
    /// </remarks>
    public class User : IIdentifiable
    {
        private static readonly SHA256 SHA = SHA256.Create();
        private static readonly RandomNumberGenerator CryptoNumberProvider = RandomNumberGenerator.Create();

        /// <summary>Gets or sets the unique database ID of this User.</summary>
        /// <remarks>See Thing "Id" property for additional remarks about database Ids.</remarks>
        public string Id { get; set; }

        /// <summary>Gets or sets the salt used to help encrypt the passwords.</summary>
        /// <remarks>Set to string instead of byte[] to support serialization.</remarks>
        public string Salt { get; set; }

        /// <summary>Gets or sets the hashed password for this user.</summary>
        /// <remarks>Set to string instead of byte[] to support serialization.</remarks>
        public string HashedPassword { get; private set; }

        public List<string> PlayerCharacterIds { get; } = new List<string>();

        /// <summary>Gets pertinent user account history (such as creation date, last log in date).</summary>
        public UserAccountHistory AccountHistory { get; } = new UserAccountHistory();

        public UserTelnetSettings TelnetSettings { get; } = new UserTelnetSettings();

        public void SetPassword(string newPassword)
        {
            var salt = new byte[24];
            CryptoNumberProvider.GetBytes(salt);
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
            return Encoding.UTF8.GetString(SHA.ComputeHash(combinedBytes));
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

        public void Save()
        {
            DocumentRepository.Save(this);
        }
    }
}