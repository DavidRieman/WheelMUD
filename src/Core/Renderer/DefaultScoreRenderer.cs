//-----------------------------------------------------------------------------
// <copyright file="DefaultScoreRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>The default Score renderer.</summary>
    /// <remarks>
    /// The default score renderer is pretty basic. Game systems will generally be expected to replace this
    /// renderer with one that understands their game-specific stats, races, rules, and so on.
    /// </remarks>
    [RendererExports.Score(0)]
    public class DefaultScoreRenderer : RendererDefinitions.Score
    {
        public override string Render(Thing player)
        {
            return "TODO: MOVE SCORE RENDER HERE: " + player.Name;
        }
    }
}
