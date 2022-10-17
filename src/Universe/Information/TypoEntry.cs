//-----------------------------------------------------------------------------
// <copyright file="TypoEntry.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using WheelMUD.Data.Entities;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Universe.Information
{
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
            Id = record.ID;
            Note = record.Note;
            Resolved = record.Resolved;
            ResolvedByPlayerId = record.ResolvedByPlayerID;
            PlaceID = record.RoomID.ToString(CultureInfo.InvariantCulture);
            SubmittedByPlayerID = record.SubmittedByPlayerID.ToString(CultureInfo.InvariantCulture);
            SubmittedDateTime = DateTime.Parse(record.SubmittedDateTime);

            if (record.ResolvedDateTime != null)
            {
                ResolvedDateTime = DateTime.Parse(record.ResolvedDateTime);
            }
            else
            {
                ResolvedDateTime = null;
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
            // TODO: https://github.com/DavidRieman/WheelMUD/issues/155: Change Typo to simpler Document DB storage.
            /*var typoRepository = new RelationalRepository<TypoRecord>();

            var typoRecord = new TypoRecord
            {
                ID = Id,
                Note = Note,
                Resolved = Resolved,
                ResolvedByPlayerID = (long)ResolvedByPlayerId,
                RoomID = Convert.ToInt64(PlaceID),
                SubmittedByPlayerID = Convert.ToInt64(SubmittedByPlayerID),
            };

            if (ResolvedDateTime != null)
            {
                typoRecord.ResolvedDateTime = ResolvedDateTime.ToString();
            }
            else
            {
                typoRecord.ResolvedDateTime = null;
            }

            typoRecord.SubmittedDateTime = SubmittedDateTime.ToString(CultureInfo.InvariantCulture);

            if (typoRecord.ID == 0)
            {
                typoRepository.Add(typoRecord);
                Id = typoRecord.ID;
            }
            else
            {
                typoRepository.Update(typoRecord);
            }*/
        }
    }
}