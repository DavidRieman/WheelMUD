//-----------------------------------------------------------------------------
// <copyright file="TypoEntry.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Universe.Information
{
    /// <summary>The Typo class is used to store information from players relating to problems with spelling in the world.</summary>
    public class TypoEntry : IIdentifiable
    {
        /// <summary>Initializes a new instance of the TypoEntry class.</summary>
        public TypoEntry()
        {
        }

        /// <summary>Saves this TypoEntry to the Document DB.</summary>
        public void Save()
        {
            DocumentRepository<TypoEntry>.Save(this);
        }

        /// <summary>Gets or sets the identifier of the typo. Becomes an auto-incremented ID from the Document DB.</summary>
        public string Id { get; set; } = "typo|";

        /// <summary>Gets or sets the problem that was submitted by the player.</summary>
        public string Note { get; set; }

        /// <summary>Gets or sets the player that submitted the problem.</summary>
        public string SubmittedByPlayerID { get; set; }

        /// <summary>Gets or sets the room ID where the player reported the problem from.</summary>
        public string PlaceID { get; set; }

        /// <summary>Gets or sets the date and time that the problem was reported.</summary>
        public DateTime SubmittedDateTime { get; set; }

        /// <summary>Gets or sets a value indicating whether the issue has been resolved.</summary>
        public bool Resolved { get; set; } = false;

        /// <summary>Gets or sets the player that submitted the problem.</summary>
        public string ResolvedByPlayerId { get; set; }

        /// <summary>Gets or sets the date and time that the problem was resolved.</summary>
        public DateTime? ResolvedDateTime { get; set; }
    }
}