//-----------------------------------------------------------------------------
// <copyright file="DefaultSplashScreenRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.SplashScreen(0)]
    public class DefaultSplashScreenRenderer : RendererDefinitions.SplashScreen
    {
        public override string Render(TerminalOptions terminalOptions)
        {
            return new OutputBuilder(terminalOptions).SingleLine(SplashHandler.Get());
        }
    }
}
