//-----------------------------------------------------------------------------
// <copyright file="BasicGuardMobBrain.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.MobAI.BasicGuardMob
{
    using WheelMUD.Core;
    using WheelMUD.Core.Events;

    /// <summary>Basic guard mob brain.</summary>
    public class BasicGuardMobBrain : Brain
    {
        /// <summary>The stimulus processer of this brain.</summary>
        private readonly BasicGuardMobStimulusProcessor stimulusProcessor = new BasicGuardMobStimulusProcessor();

        /// <summary>Processes the specified stimulus.</summary>
        /// <param name="stimulus">The stimulus to be processed.</param>
        public override void ProcessStimulus(GameEvent stimulus)
        {
            this.stimulusProcessor.Process(stimulus, this);
 //?           this.Entity.ProcessSurroundings();
        }

        /// <summary>Decides on and begins execution for an action.</summary>
        public override void DecideAction()
        {
            ActionInput actionInput = null;
            switch (this.CurrentState)
            {
                case MobState.Idle:
                    actionInput = this.DecideWhenIdle();
                    break;
                case MobState.Attacking:
                    actionInput = this.DecideWhenAttacking();
                    break;
                case MobState.Retreating:

                    break;
            }

            if (actionInput != null)
            {
                this.ExecuteAction(actionInput);
                this.LastActionInput = actionInput;
            }
        }

        /// <summary>Decides on and begins executing an action, while in combat.</summary>
        /// <returns>The ActionInput describing a (hopefully valid) action that was decided upon.</returns>
        private ActionInput DecideWhenAttacking()
        {
            /* @@@ Refactor into something like MobileAttacksBehavior
            // Can I still see my current target?
            if (this.Entity.CanPerceiveEntity(CurrentTarget))
            {
                if (CurrentTarget.Consciousness == Consciousness.Dead)
                {
                    this.CurrentState = MobState.Idle;
                    return this.DecideWhenIdle();
                }
                
                return new ActionInput("punch " + CurrentTarget.Name, this);
            }

            this.CurrentState = MobState.Idle;
            return this.DecideWhenIdle();
            */ return null;
        }

        /// <summary>Decides on and begins executing an action, while idling.</summary>
        /// <returns>The ActionInput describing a (hopefully valid) action that was decided upon.</returns>
        private ActionInput DecideWhenIdle()
        { 
            /* @@@ Refactor into something like MobileAttacksBehavior
            // Is there anything here for me to attack?
            foreach (Thing thing in this.Entity.VisibleThings)
            {
                if (!(thing is Entity))
                {
                    continue;
                }
                
                var entity = (Entity)thing;

                if (!this.HateMemory.ContainsKey(entity) || this.HateMemory[entity] < this.AttackThreshold)
                {
                    continue;
                }
                
                this.CurrentTarget = entity;
                this.CurrentState = MobState.Attacking;

                return new ActionInput("punch " + entity.Name, this);
            }

            // Roll a die to pick something random to do.
            int roll = DiceService.Instance.GetDie(100).Roll();

            // If it doesnt attack, give it a chance to wander around.
            if (roll > 80)
            {
                if (!Equals(Entity.Parent, null))
                {
                    // Pick a random exit.
                    int exitNum = DiceService.Instance.GetDie(Entity.Parent.Exits.Count).Roll();
                    IEnumerator<string> enumerator = Entity.Parent.Exits.Keys.GetEnumerator();
                    enumerator.MoveNext();

                    for (int i = 1; i < exitNum; i++)
                    {
                        enumerator.MoveNext();
                    }

                    // Return the direction command for this mob to execute.
                    return new ActionInput(enumerator.Current, this);
                }

                return null;
            }
            */
            return null;
        }
    }
}