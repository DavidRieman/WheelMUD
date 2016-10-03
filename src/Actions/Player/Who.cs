//-----------------------------------------------------------------------------
// <copyright file="Who.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to list the characters who are currently in-game.
//   Created: November 2006 by Foxedup.
//   Edited: January 2007 by Saquivor.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Utilities;

    /// <summary>A command to list the characters who are currently in-game.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("who", CommandCategory.Player)]
    [ActionDescription("See or query who is online.")]
    [ActionSecurity(SecurityRole.player)]
    public class Who : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            string div = string.Empty;
            string mudName = MudEngineAttributes.Instance.MudName;
            string mudNameLine = "                                ";
            string plural = string.Empty;
            string plural1 = string.Empty;
            StringBuilder sb = new StringBuilder();

            plural1 = "is";

            if (PlayerManager.Instance.Players.Count > 1)
            {
                plural = "s";
                plural1 = "are";
            }

            // TODO: Version, Sort and add by guild/class
            // div = "<%b%><%green%>" + string.Empty.PadLeft(sender.Entity.Terminal.Width, '~') + "<%n%>";
            div = "<%b%><%green%>~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<%n%>";
            mudNameLine += mudName;
            sb.AppendLine();
            sb.AppendLine(div);
            sb.AppendLine(mudNameLine);
            sb.AppendLine(div);
            sb.AppendLine();
            sb.AppendLine("The following player" + plural + " " + plural1 + " currently online:");
            foreach (PlayerBehavior player in PlayerManager.Instance.Players)
            {
                // TODO: I used string literal to handle "" issue is there a neater approach?
                sb.AppendFormat(@"<%mxpsecureline%><send ""finger {0}|tell {0}"" ""|finger|tell"">{0}</send>", player.Parent.Name);
                sb.AppendFormat(" - {0}", player.Parent.Name);

                // Add in AFK message
                if (player.IsAFK)
                {
                    sb.Append(" (afk");

                    if (!string.IsNullOrEmpty(player.AFKReason))
                    {
                        sb.AppendFormat(": {0}", player.AFKReason);
                    }

                    sb.Append(")");
                }

                // End with a newline char
                sb.Append("<%nl%>");
            }

            sb.AppendLine();
            sb.AppendLine(div);
            sb.AppendFormat("Counted {0} player{1} online.", PlayerManager.Instance.Players.Count, plural);
            sb.AppendLine();
            sb.AppendLine(div);
            actionInput.Controller.Write(sb.ToString().TrimEnd(null));
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return null;
        }
    }
}