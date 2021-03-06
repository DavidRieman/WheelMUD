//-----------------------------------------------------------------------------
// <copyright file="HoldsLiquidBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Interfaces;
using WheelMUD.Server;

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>OpensClosesBehavior adds the ability to open and close a thing.</summary>
    public class HoldsLiquidBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the HoldsLiquidBehavior class.</summary>
        public HoldsLiquidBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the HoldsLiquidBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public HoldsLiquidBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        public void FillFrom(IController sender, HoldsLiquidBehavior source)
        {
            // TODO: If this behavior's thing has no room left, abort.
            // sender.Write(string.Format("The {0} is full and no more can be added to it.", destinationContainerName));

            // Iterate through the source thing's Children to find the first Liquid thing.
            LiquidBehavior liquidBehavior = null;
            foreach (Thing thing in source.Parent.Children)
            {
                liquidBehavior = thing.Behaviors.FindFirst<LiquidBehavior>();
                if (liquidBehavior != null)
                {
                    break;
                }
            }

            if (liquidBehavior == null)
            {
                sender.Write(new OutputBuilder().AppendLine("The source does not contain any liquids right now."));
                return;
            }

            // TODO: Maybe prevent the mixing of liquids or whatnot by default...
            // sender.Write("You cannot mix the two different types of liquids.");

            // TODO: Determine the maximum amount we can take from that liquid stack.

            // Move that liquid stack over into this liquid-holder.
            if (Parent.Add(liquidBehavior.Parent))
            {
                sender.Write(new OutputBuilder().AppendLine($"You filled {Parent.Name} with {liquidBehavior.Parent.Name} from {source.Parent.Name}."));

                // TODO: If the source is now empty, sender.Write that the source is empty.
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}