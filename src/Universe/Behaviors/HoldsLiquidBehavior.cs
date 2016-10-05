//-----------------------------------------------------------------------------
// <copyright file="HoldsLiquidBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: November 2009 by bengecko.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;

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
            this.ID = instanceID;
        }

        public void FillFrom(IController sender, HoldsLiquidBehavior source)
        {
            // @@@ If this behavior's thing has no room left, abort.
            // sender.Write(string.Format("The {0} is full and no more can be added to it.", this.destinationContainerName));

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
                sender.Write("The source does not contain any liquids right now.");
                return;
            }

            // @@@ Maybe prevent the mixing of liquids or whatnot by default...
            // sender.Write("You cannot mix the two different types of liquids.");

            // @@@ Determine the maximum amount we can take from that liquid stack.

            // Move that liquid stack over into this liquid-holder.
            if (this.Parent.Add(liquidBehavior.Parent))
            {
                string message = string.Format(
                    "You filled {0} with {1} from {2}.",
                    this.Parent.Name,
                    liquidBehavior.Parent.Name,
                    source.Parent.Name);
                sender.Write(message);

                // @@@ If the source is now empty, sender.Write that the source is empty.
            }

            return;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}