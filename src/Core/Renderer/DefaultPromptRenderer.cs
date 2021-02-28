//-----------------------------------------------------------------------------
// <copyright file="DefaultPromptRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    [RendererExports.Prompt(0)]
    public class DefaultPromptRenderer : RendererDefinitions.Prompt
    {
        public override string Render(Thing player)
        {
            // WheelMUD Core does not have any knowledge of game-specific stats and so on (if even applicable), so
            // by default, the prompt always prints quite simply. Games should generally export their own prompt
            // renderer with a higher priority, and do something smarter here. The Core PlayerBehavior has a Prompt
            // property which can be used to store player-customized prompt templates, and your Prompt renderer can
            // (for example) transform their custom prompt template into their final prompt output.
            return "> ";
        }
    }
}
