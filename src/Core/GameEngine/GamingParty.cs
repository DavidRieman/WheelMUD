//-----------------------------------------------------------------------------
// <copyright file="GamingParty.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 5/15/2009 5:12:37 PM
//   Purpose   : Construct to represent adventuring parties or groups.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>This is the implementation of MUD parties, where a group of people are adventuring together.</summary>
    public class GamingParty
    {
        /// <summary>The members of this party.</summary>
        private readonly Dictionary<string, Thing> partyMembers = new Dictionary<string, Thing>();

        /// <summary>Adds a member to this party.</summary>
        /// <param name="partyMember">The Entity that needs to be added.</param>
        public void AddPartyMember(ref Thing partyMember)
        {
            this.partyMembers.Add(partyMember.Name, partyMember);
        }

        /// <summary>Remove a member from this party.</summary>
        /// <param name="partyMember">The Entity that needs to be removed.</param>
        public void RemovePartyMember(ref Thing partyMember)
        {
            this.partyMembers.Remove(partyMember.Name);
        }
    }
}