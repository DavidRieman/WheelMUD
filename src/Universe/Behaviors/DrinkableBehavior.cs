//-----------------------------------------------------------------------------
// <copyright file="DrinkableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>This behavior allows the attached thing to be drinkable.</summary>
    public class DrinkableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the DrinkableBehavior class.</summary>
        public DrinkableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the DrinkableBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public DrinkableBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Have a thing try to drink from this drinkable thing.</summary>
        /// <param name="drinker">The thing trying to take a drink.</param>
        public void Drink(Thing drinker)
        {
            // TODO: Implement request+event pattern, with a SensoryMessage to inform all witnesses.
            // TODO: Implement destroying parent thing (reduce count) by an appropriate amount (1 for now?).
            // TODO: Implement a sample behavior which responds to being drunk (so as to implement a potion that imbues the
            //       configured Effect on the actor when drunk).
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}