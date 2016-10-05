// -----------------------------------------------------------------------
// <copyright file="WieldableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Enables an object to be wielded by a player or mobile NPC.
// </summary>
// -----------------------------------------------------------------------

namespace WarriorRogueMage.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;

    /// <summary>A <see cref="Thing"/> having WieldableBehavior can be wielded by a player or NPC.</summary>
    /// <remarks>
    /// Typically wielding would be for weapons, but it could also be applied to non-weapons
    /// for roleplay or quests; thus WieldableBehavior is independent from WeaponBehavior.
    /// </remarks>
    public class WieldableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="WieldableBehavior"/> class.</summary>
        public WieldableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WieldableBehavior"/> class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public WieldableBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a weapon must be held in the wielder's
        /// inventory before wielding. Generally this is true, but it could be false.
        /// For example, a player might be able to wield a gun affixed to a turret
        /// without holding the gun in inventory. In that case, MustBeHeld would be false.
        /// </summary>
        public bool MustBeHeld { get; set; }

        /// <summary>Gets or sets the <see cref="Thing"/> that is currently wielding this weapon.</summary>
        public Thing Wielder { get; set; }

        /// <summary>
        /// Gets or sets the an event handler that is added to the item's MovementRequest
        /// and stored here when an item is wielded. When unwielded, this reference is
        /// removed from MovementRequest, and this property is set to null.
        /// The handler could either automatically unwield the item, or prevent dropping
        /// or otherwise transferring the item to a new owner while it is wielded.
        /// </summary>
        public CancellableGameEventHandler MovementInterceptor { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.MustBeHeld = true;
        }
    }
}
