//-----------------------------------------------------------------------------
// <copyright file="SenseManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A manager of senses, as used for perception by Things.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>A manager of senses, as used for perception by Things.</summary>
    public class SenseManager
    {
        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>Gets or sets the internal dictionary of senses.</summary>
        /// <remarks>This is needed so that the senses can be saved to the document database.</remarks>
        public Dictionary<SensoryType, Sense> SenseDictionary { get; set; } = new Dictionary<SensoryType, Sense>();

        /// <summary>Provide array-style indexing for the managed senses.</summary>
        /// <param name="index">The senses index.</param>
        /// <returns>The sense found at that index.</returns>
        public Sense this[SensoryType index]
        {
            get
            {
                return this.SenseDictionary[index];
            }

            set
            {
                this.SenseDictionary[index] = value;
            }
        }

        /// <summary>Adds the specified sense to this SenseManager.</summary>
        /// <param name="sense">The sense to add.</param>
        public void AddSense(Sense sense)
        {
            lock (this.lockObject)
            {
                this.SenseDictionary.Add(sense.SensoryType, sense);
            }
        }

        /// <summary>Removes the specified sense from this SenseManager.</summary>
        /// <param name="sense">The sense to remove.</param>
        public void RemoveSense(Sense sense)
        {
            lock (this.lockObject)
            {
                this.SenseDictionary.Remove(sense.SensoryType);
            }
        }

        /// <summary>Determines if the collection of senses contain the specified sense.</summary>
        /// <param name="sense">The sense to search for.</param>
        /// <returns>True if the collection of senses contains this sense, else false.</returns>
        public bool Contains(Sense sense)
        {
            return this.SenseDictionary.ContainsKey(sense.SensoryType);
        }

        /// <summary>Determines if the collection of senses contain the specified sense.</summary>
        /// <param name="senseType">The sense to search for.</param>
        /// <returns>True if the collection of senses contains this sense, else false.</returns>
        public bool Contains(SensoryType senseType)
        {
            lock (this.lockObject)
            {
                return this.SenseDictionary.ContainsKey(senseType);
            }
        }

        /// <summary>Gets the total count of all senses.</summary>
        /// <returns>The total count of all senses.</returns>
        public int Count()
        {
            return this.SenseDictionary.Count;
        }

        /// <summary>Gets an enumerator for the senses.</summary>
        /// <returns>An enumerator for the senses.</returns>
        public IEnumerator<Sense> GetEnumerator()
        {
            return this.SenseDictionary.Values.GetEnumerator();
        }

        /// <summary>Gets the dictionary of senses.</summary>
        /// <returns>A dictionary of senses.</returns>
        public Dictionary<SensoryType, Sense> GetSenses()
        {
            return this.SenseDictionary;
        }

        /// <summary>Determine if the specified sensory message can be processed.</summary>
        /// <param name="message">The sensory message.</param>
        /// <returns>True if it can be processed, else false.</returns>
        public bool CanProcessSensoryMessage(SensoryMessage message)
        {
            if (this.SenseDictionary.ContainsKey(message.TargetedSense) &&
                this.SenseDictionary[message.TargetedSense].LowThreshold <= message.MessageStrength &&
                this.SenseDictionary[message.TargetedSense].HighThreshold >= message.MessageStrength &&
                this.SenseDictionary[message.TargetedSense].Enabled)
            {
                return true;
            }

            return false;
        }
    }
}