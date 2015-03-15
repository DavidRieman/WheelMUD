//-----------------------------------------------------------------------------
// <copyright file="StimulusProcessor.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.MobAI
{
    using WheelMUD.Core.Events;

    /// <summary>StimulusProcessor base class.</summary>
    public abstract class StimulusProcessor
    {
        /// <summary>Processes the specified stimulus.</summary>
        /// <param name="stimulus">The stimulus to be processed.</param>
        /// <param name="brain">The brain with which to process the stimulus.</param>
        protected internal virtual void Process(GameEvent stimulus, Brain brain)
        {
            /* @@@ Repair or replace?
            switch (stimulus.GetType().Name)
            {
                case "AttackEvent":
                    this.ProcessAttackEvent(stimulus as AttackEvent, brain);
                    break;
                case "ChangeOwnerEvent":
                    this.ProcessChangeOwnerEvent(stimulus as ChangeOwnerEvent, brain);
                    break;
                case "DeathEvent":
                    this.ProcessDeathEvent(stimulus as DeathEvent, brain);
                    break;
//                case "MoveEvent":
//                    this.ProcessMoveEvent(stimulus as MoveEvent, brain);
//                    break;
                case "PlayerLogInEvent":
                    this.ProcessPlayerLoggedInEvent(stimulus as PlayerLogInEvent, brain);
                    break;
            }
            */
        }

        /// <summary>Process the 'player logged in' event with the specified brain.</summary>
        /// <param name="loggedInEvent">The 'player logged in' event to be processed.</param>
        /// <param name="brain">The brain with which to process this event.</param>
        protected virtual void ProcessPlayerLoggedInEvent(PlayerLogInEvent loggedInEvent, Brain brain)
        {
            if (loggedInEvent.ActiveThing.Parent == brain.Thing.Parent)
            {
 //?               brain.Entity.ProcessSurroundings();
            }
        }

        /// <summary>Process the 'move' event with the specified brain.</summary>
        /// <param name="moveEvent">The 'move' event to be processed.</param>
        /// <param name="brain">The brain with which to process this event.</param>
        protected virtual void ProcessMoveEvent(ArriveEvent moveEvent, Brain brain)
        {
            // If it wasn't me that moved then look around.
            if (moveEvent.ActiveThing != brain.Thing)
            {
 //? also LeaveEvent?              brain.Entity.ProcessSurroundings();
            }
        }

        /// <summary>Process the 'death' event with the specified brain.</summary>
        /// <param name="deathEvent">The 'death' event to be processed.</param>
        /// <param name="brain">The brain with which to process this event.</param>
        protected virtual void ProcessDeathEvent(DeathEvent deathEvent, Brain brain)
        {
            // Was that me that died?
            if (deathEvent.ActiveThing == brain.Thing)
            {
                brain.Stop();    
            }
            else
            {
                if (brain.HateMemory.ContainsKey(deathEvent.ActiveThing))
                {
                    brain.HateMemory.Remove(deathEvent.ActiveThing);
                }
            }
        }

        /// <summary>Process the 'change owner' event with the specified brain.</summary>
        /// <param name="ownerEvent">The 'change owner' event to be processed.</param>
        /// <param name="basicGuardMobBrain">The brain with which to process this event.</param>
        protected virtual void ProcessChangeOwnerEvent(ChangeOwnerEvent ownerEvent, Brain basicGuardMobBrain)
        {
 //?           basicGuardMobBrain.Entity.ProcessSurroundings();
        }

        /// <summary>Process the 'attack' event with the specified brain.</summary>
        /// <param name="attackEvent">The 'attack' event to be processed.</param>
        /// <param name="brain">The brain with which to process this event.</param>
        protected virtual void ProcessAttackEvent(AttackEvent attackEvent, Brain brain)
        {
            /*if (attackEvent.ActiveThing == brain.Entity)
            {
                if (!brain.HateMemory.ContainsKey(attackEvent.Aggressor))
                {
                    brain.HateMemory.Add(attackEvent.Aggressor, 100);
                }
                else
                {
                    brain.HateMemory[attackEvent.Aggressor] = 100;
                }
            }*/
        }
    }
}
