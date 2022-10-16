//-----------------------------------------------------------------------------
// <copyright file="ExitDestinationInfo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Runtime.Serialization;
using WheelMUD.Utilities;

namespace WheelMUD.Core.Behaviors
{
    /// <summary>Information about an exit's destination.</summary>
    internal class ExitDestinationInfo
    {
        /// <summary>Initializes a new instance of the DestinationInfo class.</summary>
        /// <param name="command">The command which is used to reach the target destination.</param>
        /// <param name="targetID">The ID of the target destination.</param>
        public ExitDestinationInfo(string command, Thing target)
        {
            ExitCommand = command;
            TargetID = target?.PersistedId;
            CachedTarget = new SimpleWeakReference<Thing>(target);
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            // The target Thing may have persisted or otherwise established/changed their ID since we first rigged up.
            // Since we are about to persist, fixing it now ensures we'll come back with the current link too.
            TargetID = CachedTarget.Target?.PersistedId;
        }

        /// <summary>Gets or sets the command which is used to reach the target destination.</summary>
        public string ExitCommand { get; set; }

        /// <summary>Gets or sets the ID of the target destination.</summary>
        public string TargetID { get; set; }

        /// <summary>Gets or sets the cached destination thing.</summary>
        [JsonIgnore]
        public SimpleWeakReference<Thing> CachedTarget { get; set; }
    }
}
