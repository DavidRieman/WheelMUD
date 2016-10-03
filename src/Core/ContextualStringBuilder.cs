//-----------------------------------------------------------------------------
// <copyright file="ContextualStringBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Context-sensitive string builder, used to customize the output that any 
//   given entity can receive for a message.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections;
    using System.Collections.Generic;

    //// <ns></ns> - encloses elements to be parsed only if the string is NOT being passed to the sender
    //// <s></s>   - encloses elements to be parsed only if the string IS being passed to the sender
    //// <nr></nr> - encloses elements to be parsed only if the string is NOT being passed to the receiver
    //// <r></r>   - encloses elements to be parsed only if the string IS being passed to the receiver

    //// $sp$      - sender pronoun, will pass the sender's name (You if the string is passed to the sender)
    //// $rp$      - receiver pronoun, will pass receiver's name (You if the string is passed to the receiver)
    //// $grp$     - gendered reflexive pronoun, will pass himself, herself, itself (Yourself if the string is passed to the sender)
    //// $gpp$     - gendered posesive pronount, will pass his, her, its, (your if the string is passed to the sender)

    /// <summary>Contextual string usage flags.</summary>
    public enum ContextualStringUsage
    {
        /// <summary>Use this string anytime.</summary>
        Anytime,

        /// <summary>Use this string when not being passed to originator.</summary>
        WhenNotBeingPassedToOriginator,

        /// <summary>Use this string only when being passed to originator.</summary>
        OnlyWhenBeingPassedToOriginator,

        /// <summary>Use this string when not being passed to receiver.</summary>
        WhenNotBeingPassedToReceiver,

        /// <summary>Use this string only when being passed to receiver.</summary>
        OnlyWhenBeingPassedToReceiver,

        /// <summary>Use this string when not being passed to receiver or originator.</summary>
        WhenNotBeingPassedToReceiverOrOriginator
    }

    /// <summary>Contextual string builder.</summary>
    public class ContextualStringBuilder
    {
        ////private readonly StringBuilder sb = new StringBuilder();

        /// <summary>A list of contextually-constrained strings, used to build output.</summary>
        private readonly List<ContextualString> texts = new List<ContextualString>();

        /// <summary>Initializes a new instance of the ContextualStringBuilder class.</summary>
        /// <param name="originator">The originator of the message.</param>
        /// <param name="receiver">The main target of the message.</param>
        public ContextualStringBuilder(Thing originator, Thing receiver)
        {
            this.Originator = originator;
            this.Receiver = receiver;
            this.ViewEngineContext = new Hashtable();
        }

        /// <summary>Gets the originator.</summary>
        public Thing Originator { get; private set; }

        /// <summary>Gets the receiver.</summary>
        public Thing Receiver { get; private set; }

        /// <summary>Gets the view engine context.</summary>
        public Hashtable ViewEngineContext { get; private set; }

        /// <summary>Append the specified text, within the specified context only.</summary>
        /// <param name="text">The text to append.</param>
        /// <param name="usage">The context(s) which this string should be appended.</param>
        /// <returns>Returns a ContextualStringBuilder object.</returns>
        public ContextualStringBuilder Append(string text, ContextualStringUsage usage)
        {
            this.texts.Add(new ContextualString(text, usage));
            return this;
        }

        /// <summary>Appends the following text, regardless of context.</summary>
        /// <param name="text">The text to append.</param>
        /// <returns>Returns a ContextualStringBuilder.</returns>
        public ContextualStringBuilder Append(string text)
        {
            this.Append(text, ContextualStringUsage.Anytime);
            return this;
        }

        /// <summary>Parse the string with the context of the specified receiver.</summary>
        /// <param name="currentReceiver">The intended receiver.</param>
        /// <returns>The suitable string for the intended receiver.</returns>
        public string Parse(Thing currentReceiver)
        {
            string output = string.Empty;

            foreach (ContextualString text in this.texts)
            {
                switch (text.Usage)
                {
                    case ContextualStringUsage.WhenNotBeingPassedToOriginator:
                        if (this.Originator != currentReceiver)
                        {
                            output += text.Text;
                        }

                        break;
                    case ContextualStringUsage.OnlyWhenBeingPassedToOriginator:
                        if (this.Originator == currentReceiver)
                        {
                            output += text.Text;
                        }

                        break;
                    case ContextualStringUsage.WhenNotBeingPassedToReceiver:
                        if (this.Receiver != currentReceiver)
                        {
                            output += text.Text;
                        }

                        break;
                    case ContextualStringUsage.OnlyWhenBeingPassedToReceiver:
                        if (this.Receiver == currentReceiver)
                        {
                            output += text.Text;
                        }

                        break;
                    case ContextualStringUsage.WhenNotBeingPassedToReceiverOrOriginator:
                        if (this.Receiver != currentReceiver && this.Originator != currentReceiver)
                        {
                            output += text.Text;
                        }

                        break;
                    default:
                        output += text.Text;
                        break;
                }
            }

            return output;
        }

        /// <summary>A context-sensitive string.</summary>
        private class ContextualString
        {
            /// <summary>Initializes a new instance of the ContextualString class.</summary>
            /// <param name="text">The string text.</param>
            /// <param name="usage">The contextual usage of this string.</param>
            public ContextualString(string text, ContextualStringUsage usage)
            {
                this.Text = text;
                this.Usage = usage;
            }

            /// <summary>Gets the string text.</summary>
            public string Text { get; private set; }

            /// <summary>Gets the contextual usage of this string.</summary>
            public ContextualStringUsage Usage { get; private set; }
        }
    }
}