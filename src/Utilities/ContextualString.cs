//-----------------------------------------------------------------------------
// <copyright file="ContextualString.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    /// <summary>
    /// ContextualString is a simplified form of ContextualStringBuilder which supplies the most common 
    /// set of message forms to the ContextualStringBuilder.  This simplifies construction of most events.
    /// </summary>
    public class ContextualString : ContextualStringBuilder
    {
        public ContextualString(object originator, object receiver)
            : base(originator, receiver)
        {
        }

        public string ToOriginator
        {
            // TODO: remove existing ToOriginator appends, if any.
            set => Append(value, ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
        }

        public string ToReceiver
        {
            // TODO: remove existing ToReceiver appends, if any.
            set => Append(value, ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
        }

        public string ToOthers
        {
            // TODO: remove existing ToOthers appends, if any.
            set => Append(value, ContextualStringUsage.WhenNotBeingPassedToReceiverOrOriginator);
        }
    }
}