//-----------------------------------------------------------------------------
// <copyright file="ContextualString.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>
    /// ContextualString is a simplified form of ContextualStringBuilder which supplies the most common 
    /// set of message forms to the ContextualStringBuilder.  This simplifies construction of most events.
    /// </summary>
    public class ContextualString : ContextualStringBuilder
    {
        public ContextualString(Thing originator, Thing receiver)
            : base(originator, receiver)
        {
        }

        public string ToOriginator
        {
            set
            {
                // @@@ TODO: remove existing ToOriginator appends, if any.
                base.Append(value, ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            }
        }

        public string ToReceiver
        {
            set
            {
                // @@@ TODO: remove existing ToReceiver appends, if any.
                base.Append(value, ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
            }
        }

        public string ToOthers
        {
            set
            {
                // @@@ TODO: remove existing ToOthers appends, if any.
                base.Append(value, ContextualStringUsage.WhenNotBeingPassedToReceiverOrOriginator);
            }
        }
    }
}