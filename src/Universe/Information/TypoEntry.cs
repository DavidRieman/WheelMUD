//-----------------------------------------------------------------------------
// <copyright file="TypoEntry.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: December 2009 bengecko
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe.Information
{
    using System;
    using System.Globalization;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.Repositories;
    using WheelMUD.Interfaces;

    /// <summary>The Typo class is used to store information from players relating to problems with spelling in the world.</summary>
    public class TypoEntry : IPersistable
    {
        /// <summary>Initializes a new instance of the TypoEntry class.</summary>
        public TypoEntry()
        {
        }

        /// <summary>Initializes a new instance of the TypoEntry class.</summary>
        /// <param name="record"> The record that contains the information from the database.</param>
        public TypoEntry(TypoRecord record)
        {
            this.Id = record.ID;
            this.Note = record.Note;
            this.Resolved = record.Resolved;
            this.ResolvedByPlayerId = record.ResolvedByPlayerID;
            this.PlaceID = record.RoomID.ToString(CultureInfo.InvariantCulture);
            this.SubmittedByPlayerID = record.SubmittedByPlayerID.ToString(CultureInfo.InvariantCulture);
            this.SubmittedDateTime = DateTime.Parse(record.SubmittedDateTime);

            if (record.ResolvedDateTime != null)
            {
                this.ResolvedDateTime = DateTime.Parse(record.ResolvedDateTime);
            }
            else
            {
                this.ResolvedDateTime = null;
            }
        }

        /// <summary>Gets or sets the identifier of the typo.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the problem that was submitted by the player.</summary>
        public string Note { get; set; }

        /// <summary>Gets or sets the player that submitted the problem.</summary>
        public string SubmittedByPlayerID { get; set; }

        /// <summary>Gets or sets the room that the problem was found in.</summary>
        public string PlaceID { get; set; }

        /// <summary>Gets or sets the date and time that the problem was reported.</summary>
        public DateTime SubmittedDateTime { get; set; }

        /// <summary>Gets or sets a value indicating whether the issue has been resolved.</summary>
        public bool Resolved { get; set; }

        /// <summary>Gets or sets the player that submitted the problem.</summary>
        public long? ResolvedByPlayerId { get; set; }

        /// <summary>Gets or sets the date and time that the problem was resolved.</summary>
        public DateTime? ResolvedDateTime { get; set; }

        /// <summary>Save the typo to the database.</summary>
        public void Save()
        {
            var typoRepository = new TypoRepository();

            var typoRecord = new TypoRecord
            {
                ID = this.Id,
                Note = this.Note,
                Resolved = this.Resolved,
                ResolvedByPlayerID = (long)this.ResolvedByPlayerId,
                RoomID = Convert.ToInt64(this.PlaceID),
                SubmittedByPlayerID = Convert.ToInt64(this.SubmittedByPlayerID),
            };

            if (this.ResolvedDateTime != null)
            {
                typoRecord.ResolvedDateTime = this.ResolvedDateTime.ToString();
            }
            else
            {
                typoRecord.ResolvedDateTime = null;
            }

            typoRecord.SubmittedDateTime = this.SubmittedDateTime.ToString(CultureInfo.InvariantCulture);

            if (typoRecord.ID == 0)
            {
                typoRepository.Add(typoRecord);
                this.Id = typoRecord.ID;
            }
            else
            {
                typoRepository.Update(typoRecord);
            }
        }
    }
}