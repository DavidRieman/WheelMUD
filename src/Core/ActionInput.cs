//-----------------------------------------------------------------------------
// <copyright file="ActionInput.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core.Interfaces;

namespace WheelMUD.Core
{
    using System;
    using System.Linq;

    /// <summary>A base action command. This is created whenever input is detected on the sockets.</summary>
    /// <remarks>The text is parsed so that it can be presented to the command system easily.</remarks>
    public class ActionInput
    {
        /// <summary>Initializes a new instance of the ActionInput class.</summary>
        /// <param name="fullText">The full text input.</param>
        /// <param name="controller">The controller which originated the input.</param>
        public ActionInput(string fullText, IController controller)
        {
            Controller = controller;
            FullText = fullText;
            ParseText(FullText);
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
                Noun = words[0].ToLower();
                Tail = fullText.Remove(0, Noun.Length).Trim();
                Params = words.Skip(1).ToArray();
            }
        }
    }
}