//-----------------------------------------------------------------------------
// <copyright file="Brain.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.MobAI
{
    using System.Collections.Generic;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>Brain base class.</summary>
    public abstract class Brain : IController
    {
        /// <summary>The pulse timing mechanism.</summary>
        private Timer pulse = null;

        /// <summary>
        /// Initializes a new instance of the Brain class.
        /// </summary>
        public Brain()
        {
            this.HateMemory = new Dictionary<Thing, int>();
            this.AttackThreshold = 70;
        }

        /// <summary>Action received.</summary>
        public event ActionReceivedEventHandler ActionReceived;

        /// <summary>Mob state.</summary>
        protected enum MobState
        {
            /// <summary>The mob is idle.</summary>
            Idle = 0,

            /// <summary>The mob is attacking.</summary>
            Attacking,

            /// <summary>The mob is retreating.</summary>
            Retreating,
        }

        /// <summary>Gets the hate memory of this entity.</summary>
        public Dictionary<Thing, int> HateMemory { get; private set; }

        /// <summary>Gets or sets the last action input of this entity.</summary>
        public ActionInput LastActionInput { get; protected set; }

        /// <summary>Gets or sets the host entity of this brain.</summary>
        public Thing Thing { get; set; }

        /// <summary>Gets the living behavior of the thing attached to this brain.</summary>
        public LivingBehavior LivingBehavior { get; set; }

        /// <summary>Gets or sets the current state of this brain's entity.</summary>
        protected MobState CurrentState { get; set; }

        /// <summary>Gets or sets the currently targeted Thing (if any) of this brain.</summary>
        protected Thing CurrentTarget { get; set; }

        /// <summary>Gets or sets the attack threshold.</summary>
        protected int AttackThreshold { get; set; }
        
        /// <summary>Instructs the brain to process the specified stimulus.</summary>
        /// <param name="stimulus">The stimulus to be processed.</param>
        public abstract void ProcessStimulus(GameEvent stimulus);

        /// <summary>Instructs the brain to decide on an action to take.</summary>
        public abstract void DecideAction();

        /// <summary>Writes the specified data.</summary>
        /// <param name="data">The data to write.</param>
        public void Write(string data)
        {
            var contextualData = new ContextualStringBuilder(this.Thing, this.Thing);
            contextualData.Append(data, ContextualStringUsage.Anytime);
            SensoryMessage message = new SensoryMessage(SensoryType.Debug, 0, contextualData);
//?            var debugEvent = new DebugEvent();
//?            debugEvent.ActiveThing = this.Entity;
//?            debugEvent.Message = message;
//?            this.Entity.Parent.OnMiscellaneousEvent(debugEvent);
        }

        /// <summary>Writes the specified data.</summary>
        /// <param name="data">The data to write.</param>
        /// <param name="sendPrompt">If true, sends a prompt.</param>
        public void Write(string data, bool sendPrompt)
        {
            this.Write(data);
        }

        /// <summary>
        /// Place an action on the command queue for immediate execution.
        /// </summary>
        /// <param name="actionInput">The action input to attempt to execute.</param>
        public void ExecuteAction(ActionInput actionInput)
        {
            if (this.ActionReceived != null)
            {
                this.ActionReceived(this, actionInput);
            }
        }

        /// <summary>
        /// Stop the brain ticking.
        /// </summary>
        public void Stop()
        {
            if (this.pulse != null)
            {
                this.pulse.Change(Timeout.Infinite, Timeout.Infinite);
            }

            this.HateMemory.Clear();
        }

        /// <summary>
        /// Start the brain.
        /// </summary>
        public void Start()
        {
            if (this.pulse == null)
            {
                this.pulse = new Timer(this.Pulse, null, 2000, 2000);
            }
        }

        /// <summary>
        /// Pulse the brain.
        /// </summary>
        /// <param name="state">@@@ What is this?</param>
        private void Pulse(object state)
        {
            this.DecideAction();
        }
    }
}
