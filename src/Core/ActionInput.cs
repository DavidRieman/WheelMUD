//-----------------------------------------------------------------------------
// <copyright file="ActionInput.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A base action input command. This is created whenever input is detected on 
//   the sockets, or action input is created by mobile AI.
//   The text is parsed so that it can be presented to the command system easily.
//   Created: January 2007 by Foxedup.
//   Renamed: April 2009 by Karak. "Action" to "ActionInput" to fit its purpose.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Linq;
    using WheelMUD.Interfaces;

    /// <summary>A base action command. This is created whenever input is detected on the sockets.</summary>
    /// <remarks>The text is parsed so that it can be presented to the command system easily.</remarks>
    public class ActionInput
    {
        /// <summary>Initializes a new instance of the ActionInput class.</summary>
        /// <param name="fullText">The full text input.</param>
        /// <param name="controller">The controller which originated the input.</param>
        public ActionInput(string fullText, IController controller)
        {
            this.Controller = controller;
            this.FullText = fullText;
            this.ParseText(this.FullText);
        }

        /// <summary>Gets the full text of this action.</summary>
        public string FullText { get; private set; }

        /// <summary>Gets the noun of this tail.</summary>
        public string Noun { get; private set; }

        /// <summary>Gets the tail of this action.</summary>
        public string Tail { get; private set; }

        /// <summary>Gets the controller of this action.</summary>
        public IController Controller { get; private set; }

        /// <summary>Gets the parameters of this action.</summary>
        public string[] Params { get; private set; }

        /// <summary>Gets or sets the context for this action.</summary>
        public object Context { get; set; }

        /// <summary>Parses the supplied text.</summary>
        /// <param name="fullText">The full text to be parsed.</param>
        private void ParseText(string fullText)
        {
            fullText = string.IsNullOrEmpty(fullText) ? string.Empty : fullText.Trim();

            string[] words = fullText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                this.Noun = words[0].ToLower();
                this.Tail = fullText.Remove(0, this.Noun.Length).Trim();
                this.Params = words.Skip(1).ToArray();
            }
        }
    }
}