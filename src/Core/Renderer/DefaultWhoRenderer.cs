//-----------------------------------------------------------------------------
// <copyright file="DefaultWhoRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    /// <summary>The default "who" command output renderer.</summary>
    /// <remarks>
    /// The default "who" renderer may simply show all online players. One might wish to build a "who" renderer
    /// with higher priority to customize things like hiding invisible admins, showing AFK states, etc.
    /// </remarks>
    [RendererExports.Who(0)]
    public class DefaultWhoRenderer : RendererDefinitions.Who
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, Thing activePlayer)
        {
            string mudNameLine = "                                "; // TODO: Dynamic centering instead, if we want centering!
            string plural = string.Empty;
            var output = new OutputBuilder();

            string plural1 = "is";

            // TODO: Should query for players who can be known about by this player (e.g. omit wiz-inviz players, if any?)
            if (PlayerManager.Instance.Players.Count > 1)
            {
                plural = "s";
                plural1 = "are";
            }

            // TODO: A game-system specific renderer could be used to includ race/class info and so on, if desired.
            // TODO: Dividing lines could be influenced by activePlayer connection Terminal.Width.
            mudNameLine += GameConfiguration.Name;
            output.AppendLine();
            output.AppendSeparator();
            output.AppendLine(mudNameLine);
            output.AppendSeparator();
            output.AppendLine();
            output.AppendLine($"The following player{plural} {plural1} currently online:");
            foreach (PlayerBehavior player in PlayerManager.Instance.Players)
            {
                var playerName = player.Parent.Name;
                var playerState = GetPlayerState(player);
                if (terminalOptions.UseMXP)
                {
                    // TODO: "tell {0}" is not a good menu command; possibly friend add/remove, invite to group, hailing, and so on.
                    // TODO: #107: Fix handling of MXP Secure Lines...  (This wasn't being built in a safe way, and does not render correctly now.)
                    output.AppendLine($"<%mxpsecureline%><send \"finger {playerName}|tell {playerName}\" \"|finger|tell\">{playerName}</send> - {player.Parent.FullName} {playerState}");
                }
                else
                {
                    output.AppendLine($"{playerName} - {player.Parent.FullName} {playerState}");
                }
            }

            output.AppendLine();
            output.AppendSeparator();
            output.AppendLine($"Counted {PlayerManager.Instance.Players.Count} player{plural} online.");
            output.AppendLine();
            output.AppendSeparator();
            return output;
        }

        private string GetPlayerState(PlayerBehavior player)
        {
            // TODO: Maybe player state changes like AFK/Linkdead should just cause a recalculation of the player's FullName
            //       to include the details? Could be more efficient and enforce consistency with room perception, too.
            if (!player.IsAFK)
            {
                return string.Empty;
            }
            return string.IsNullOrEmpty(player.AFKReason) ? "(<%yellow%>AFK<%n%>)" : $"(<%yellow%>AFK<%n%>: {player.AFKReason})";
        }
    }
}
