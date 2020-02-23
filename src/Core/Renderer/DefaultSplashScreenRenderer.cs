//-----------------------------------------------------------------------------
// <copyright file="DefaultSplashScreenRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Renderer
{
    using System;

    [ExportSplashScreenRenderer(0)]
    public class DefaultSplashScreenRenderer : RendererDefinitions.SplashScreen
    {
        public override string Render()
        {
            return "TODO: MOVE SPLASH SCREEN RENDERING HERE!";
        }
    }
}
