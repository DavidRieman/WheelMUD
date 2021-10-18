//-----------------------------------------------------------------------------
// <copyright file="ActionInput.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>A base action command. This is created whenever input is detected on the sockets.</summary>
    /// <remarks>The text is parsed so that it can be presented to the command system easily.</remarks>
    public class ActionInput
    {
        /// <summary>Initializes a new instance of the ActionInput class.</summary>
        /// <param name="fullText">The full text input.</param>
        /// <param name="session">The Session which originated the input (if any) and which should receive feedback about malformed input.</param>
        /// <param name="actor">The Thing which will try to carry out this action.</param>
        public ActionInput(string fullText, Session session, Thing actor)
        {
            Session = session;
            Actor = actor;
            FullText = fullText;
            ParseText(FullText);
        }

        /// <summary>Gets the full text of this action.</summary>
        public string FullText { get; private set; }

        /// <summary>Gets the noun of this tail.</summary>
        public string Noun { get; private set; }

        /// <summary>Gets the tail of this action.</summary>
        public string Tail { get; private set; }

        /// <summary>Gets the session for the controller of this action (if any).</summary>
        /// <remarks>
        /// Actions originating from a non-UserControlled Thing may simply not have a Session; it may be null. Actions must be built assuming that
        /// the Session may be null. For example, some coded effect tried to make a non-player Thing do a player-like Action, or the player has
        /// suddenly become logged out while the action was about to be executed, etc.
        /// The Session can be used to directly give feedback about malformed requests and such directly to the action issuer.  In some cases, the
        /// action issuer might not be the same as the Actor. (For example, in the case of an admin trying to "force" a player to
        /// execute some action, it may make more sense to give the malformed command feedback to the admin instead of the targeted player.)
        /// TODO: Mobile AI may get coded to have ActionInput queues like players (or simply process ActionInputs immediately without a queue). If
        ///       so, they won't have a Session, but may just hook up to events so they can make decisions about things that are happening, etc.
        /// </remarks>
        public Session Session { get; private set; }

        /// <summary>The Actor is the Thing that is trying to perform this action.</summary>
        /// <remarks>
        /// When a UserControlled Player issues a command, the Actor is usually the Player Thing.
        /// Assume things like mobile AI will cause some ActionInputs to be processed where the Actor is not also a Player. (Use null safeties.)
        /// </remarks>
        public Thing Actor { get; private set; }

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