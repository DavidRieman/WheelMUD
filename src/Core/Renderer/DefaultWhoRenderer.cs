//-----------------------------------------------------------------------------
// <copyright file="DefaultWhoRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Text;
    using WheelMUD.Utilities;

    /// <summary>The default "who" command output renderer.</summary>
    /// <remarks>
    /// The default "who" renderer may simply show all online players. One might wish to build a "who" renderer
    /// with higher priority to customize things like hiding invisible admins, showing AFK states, etc.
    /// </remarks>
    [RendererExports.Who(0)]
    public class DefaultWhoRenderer : RendererDefinitions.Who
    {
        public override string Render(Thing activePlayer)
        {
            string mudName = GameConfiguration.Name;
            string mudNameLine = "                                "; // TODO: Dynamic centering instead, if we want centering!
            string plural = string.Empty;
            var sb = new StringBuilder();

            string plural1 = "is";

            // TODO: Should query for players who can be known about by this player (e.g. omit wiz-inviz players, if any?)
            if (PlayerManager.Instance.Players.Count > 1)
            {
                plural = "s";
                plural1 = "are";
            }

            // TODO: A game-system specific renderer could be used to includ race/class info and so on, if desired.
            // TODO: Dividing lines could be influenced by activePlayer connection Terminal.Width.
            string div = "<%b%><%green%>~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<%n%>";
            mudNameLine += mudName;
            sb.AppendLine();
            sb.AppendLine(div);
            sb.AppendLine(mudNameLine);
            sb.AppendLine(div);
            sb.AppendLine();
            sb.AppendLine("The following player" + plural + " " + plural1 + " currently online:");
            foreach (PlayerBehavior player in PlayerManager.Instance.Players)
            {
                // TODO: "tell {0}" is not a good menu command; possibly friend add/remove, invite to group, hailing, and so on.
                var playerName = player.Parent.Name;
                var playerState = GetPlayerState(player);
                sb.AppendLine($"<%mxpsecureline%><send \"finger {playerName}|tell {playerName}\" \"|finger|tell\">{playerName}</send> - {player.Parent.FullName} {playerState}");
            }

            sb.AppendLine();
            sb.AppendLine(div);
            sb.AppendFormat("Counted {0} player{1} online.", PlayerManager.Instance.Players.Count, plural);
            sb.AppendLine();
            sb.AppendLine(div);
            return sb.ToString();
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
