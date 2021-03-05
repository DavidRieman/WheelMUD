//-----------------------------------------------------------------------------
// <copyright file="DefaultScoreRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    /// <summary>The default Score renderer.</summary>
    /// <remarks>
    /// The default score renderer is pretty basic. Game systems will generally be expected to replace this
    /// renderer with one that understands their game-specific stats, races, rules, and so on.
    /// </remarks>
    [RendererExports.Score(0)]
    public class DefaultScoreRenderer : RendererDefinitions.Score
    {
        public override string Render(TerminalOptions terminalOptions, Thing player)
        {
            // Pretty basic placeholder. Most game systems will probably want to define their own stats systems
            // and races and attributes and so on, and provide a more traditional "score" breakdown of the
            // current character state as pertains to the actual game system.
            var livingBehavior = player.FindBehavior<LivingBehavior>();
            var ab = new OutputBuilder(terminalOptions);
            ab.AppendLine($"{player.Name}. You are {livingBehavior.Consciousness}");
            return ab.ToString();
        }
    }
}
