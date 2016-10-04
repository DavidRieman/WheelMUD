//-----------------------------------------------------------------------------
// <copyright file="PlayerDocument.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : May 17, 2011
//   Purpose   : The representation of the player in the document database.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using System.Collections.Generic;
    using WheelMUD.Interfaces;
    /// <summary>The representation of the player in the document database.</summary>
    public class PlayerDocument
    {
        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the database id.</summary>
        public long DatabaseId { get; set; }

        /// <summary>Gets or sets the last updated date.</summary>
        public string LastUpdatedDate { get; set; }

        /// <summary>Gets or sets the player prompt.</summary>
        public string PlayerPrompt { get; set; }

        /// <summary>Gets or sets the behaviors.</summary>
        public List<IPersistsWithPlayer> Behaviors { get; set; }

        /// <summary>Gets or sets the stats.</summary>
        public Dictionary<string, IPersistsWithPlayer> Stats { get; set; }

        /// <summary>Gets or sets the secondary stats.</summary>
        public Dictionary<string, IPersistsWithPlayer> SecondaryStats { get; set; }

        /// <summary>Gets or sets the skills.</summary>
        public Dictionary<string, IPersistsWithPlayer> Skills { get; set; }

        /// <summary>Gets or sets the children.</summary>
        /// <remarks>TODO: This should be "Children" to match Thing's "Children" property, but needs a DB update to match...</remarks>
        public List<IThing> SubThings { get; set; }
    }
}