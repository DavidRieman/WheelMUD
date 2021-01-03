//-----------------------------------------------------------------------------
// <copyright file="ExportSessionStateAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// An attribute used to mark 
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSessionStateAttribute : ExportAttribute
    {
        public ExportSessionStateAttribute(int statePriority)
            : base(typeof(SessionState))
        {
            StatePriority = statePriority;
        }

        public ExportSessionStateAttribute(IDictionary<string, object> metadata)
        {
            // TODO: Replace this loop with a generic reusable method using reflection to automatically
            //       find and fill in the properties by key, with the values.  IE reuse the methods that
            //       the properties persistence code will be using...
            foreach (var key in metadata.Keys)
            {
                switch (key.ToLower())
                {
                    case "statepriority":
                        StatePriority = (int)metadata[key];
                        break;
                    case "exporttypeidentity":
                        break;
                    default:
                        throw new ArgumentException("Unrecognized parameterization in ExportFooAttribute");
                }
            }
        }

        /// <summary>Gets or sets the priority of the exported state; the state with the highest priority will be the default initial state.</summary>
        /// <remarks>Do not exceed ConnectedState's priority (100 ATM) unless the state you're exporting is intended to replace ConnectedState.</remarks>
        public int StatePriority { get; set; }
    }
}