//-----------------------------------------------------------------------------
// <copyright file="Look.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script to allow a player to look at things and at their environment.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows a player to look at their environment.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("look", CommandCategory.Inform)]
    [ActionAlias("l", CommandCategory.Inform)]
    [ActionDescription("Look at the room, item, person, or monster.")]
    [ActionSecurity(SecurityRole.player)]
    public class Look : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious
        };

        private SensesBehavior sensesBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string output;
            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                output = this.TryLookAtThing(actionInput.Tail, sender.Thing);
                if (string.IsNullOrEmpty(output))
                {
                    output = string.Format("You cannot see {0}.", actionInput.Tail);
                }
            }
            else
            {
                output = this.LookAtRoom(sender.Thing);
                if (string.IsNullOrEmpty(output))
                {
                    output = "You cannot see.";
                }
            }

            sender.Write(output);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = this.VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            this.sensesBehavior = actionInput.Controller.Thing.Behaviors.FindFirst<SensesBehavior>();
            if (this.sensesBehavior == null)
            {
                return "You do not have any senses to perceive with.";
            }

            return null;
        }

        /// <summary>Tries to look at a thing.</summary>
        /// <param name="thingToLookAt">The thing to look at.</param>
        /// <param name="sender">The sender.</param>
        /// <returns>Returns the rendered view.</returns>
        private string TryLookAtThing(string thingToLookAt, Thing sender)
        {
            // Look for the target in the current room.
            Thing thing = sender.Parent.FindChild(thingToLookAt);
            if (thing != null && this.sensesBehavior.CanPerceiveThing(thing))
            {
                return Renderer.Instance.RenderPerceivedThing(sender, thing);
            }

            // If no target was found, see if it matches any of the room's visuals.
            var room = sender.Parent.FindBehavior<RoomBehavior>();
            if (room != null)
            {
                string visual = room.FindVisual(thingToLookAt);
                if (!string.IsNullOrEmpty(visual))
                {
                    return visual;
                }
            }

            // Otherwise, see if it is a thing the player has.
            thing = sender.FindChild(thingToLookAt);
            if (thing != null && this.sensesBehavior.CanPerceiveThing(thing))
            {
                return Renderer.Instance.RenderPerceivedThing(sender, thing);
            }

            // At this point, target was not found.
            return string.Empty;
        }

        /// <summary>Looks at room. @@@ Move to SensesBehavior?</summary>
        /// <param name="sender">The sender.</param>
        /// <returns>Returns the text of the rendered room template.</returns>
        private string LookAtRoom(Thing sender)
        {
            var context = new Hashtable
            {
                { "Room", sender.Parent },
                { "Exits", this.sensesBehavior.PerceiveExits() },
                { "Entities", this.sensesBehavior.PerceiveEntities() },
                { "Items", this.sensesBehavior.PerceiveItems() }
            };

            return Renderer.Instance.RenderPerceivedRoom(sender, sender.Parent);
        }
    }
}