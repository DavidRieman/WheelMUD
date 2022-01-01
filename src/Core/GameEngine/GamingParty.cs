//-----------------------------------------------------------------------------
// <copyright file="GamingParty.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>This is the implementation of MUD parties, where a group of people are adventuring together.</summary>
    public class GamingParty
    {
        /// <summary>The members of this party.</summary>
        private readonly Dictionary<string, Thing> partyMembers = new Dictionary<string, Thing>();

        /// <summary>Adds a member to this party.</summary>
        /// <param name="partyMember">The Entity that needs to be added.</param>
        public void AddPartyMember(ref Thing partyMember)
        {
            partyMembers.Add(partyMember.Name, partyMember);
        }

        /// <summary>Remove a member from this party.</summary>
        /// <param name="partyMember">The Entity that needs to be removed.</param>
        public void RemovePartyMember(ref Thing partyMember)
        {
            partyMembers.Remove(partyMember.Name);
        }
    }
}