//-----------------------------------------------------------------------------
// <copyright file="DefaultInventoryRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    [RendererExports.Inventory(0)]
    public class DefaultInventoryRenderer : RendererDefinitions.Inventory
    {
        public override string Render(Thing player)
        {
            return "TODO: RENDER INVENTORY FOR: " + player.Name;
        }
    }
}
