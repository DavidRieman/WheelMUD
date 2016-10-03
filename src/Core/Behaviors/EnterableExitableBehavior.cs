//-----------------------------------------------------------------------------
// <copyright file="EnterableExitableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>EnterableExitableBehavior provides the ability for the parent Thing to be entered.</summary>
    /// <remarks>
    /// Entering/exiting simply reuses the MovableBehavior to move an actor. 
    /// @@@ TODO: Consider removing as the new flexible ExitBehavior probably completely covers this 
    ///     behavior simply by having "exit" or "enter" be the commands registered?
    /// </remarks>
    public class EnterableExitableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the EnterableExitableBehavior class.</summary>
        public EnterableExitableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EnterableExitableBehavior"/> class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public EnterableExitableBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets the exit command.</summary>
        /// <remarks>@@@ TODO: Should these be customizable and register context commands?</remarks>
        public string ExitCommand
        {
            get { return "exit"; }
        }

        /// <summary>Gets the enter command.</summary>
        /// <remarks>@@@ TODO: Should these be customizable and register context commands?</remarks>
        public string EnterCommand
        {
            get { return "enter"; }
        }

        /// <summary>Process an entry by the specified actor.</summary>
        /// <param name="actor">The actor.</param>
        public void Enter(Thing actor)
        {
            // Prepare the Close game event for sending as a request, and if not cancelled, again as an event.
            var movableBehavior = actor.Behaviors.FindFirst<MovableBehavior>();
            if (movableBehavior == null)
            {
                // An actor tried to 'enter' even though it is not mobile; abort. This should probably message the actor.
                return;
            }

            // Move the actor into this enterable Thing. We're using the same message for going and arriving ATM.
            var message = new SensoryMessage(
                SensoryType.Sight,
                100,
                new ContextualString(actor, this.Parent)
                {
                    ToOriginator = @"You enter $TargetThing.Name.",
                    ToReceiver = @"$ActiveThing.Name enters you.",
                    ToOthers = @"$ActiveThing.Name enters $TargetThing.Name.",
                });

            movableBehavior.Move(this.Parent, this.Parent, message, message);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}