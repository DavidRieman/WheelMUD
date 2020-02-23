//-----------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.ComponentModel.Composition;

    public class Renderer
    {
        /// <summary>The singleton instance of the Renderer class.</summary>
        public static Renderer Instance { get; } = new Renderer();

        /// <summary>Prevents a default instance of the Renderer class from being created.</summary>
        private Renderer()
        {
            this.Recompose();
        }

        /// <summary>The current Inventory renderer.</summary>
        private RendererDefinitions.Inventory currentInventoryRenderer;

        /// <summary>The current PerceivedRoom renderer.</summary>
        private RendererDefinitions.PerceivedRoom currentPerceivedRoomRenderer;

        /// <summary>The current PerceivedThing renderer.</summary>
        private RendererDefinitions.PerceivedThing currentPerceivedThingRenderer;

        /// <summary>The current Score renderer.</summary>
        private RendererDefinitions.Score currentScoreRenderer;

        /// <summary>The current SplashScreen renderer.</summary>
        private RendererDefinitions.SplashScreen currentSplashScreenRenderer;

        /// <summary>The current Who renderer.</summary>
        private RendererDefinitions.Who currentWhoRenderer;

        /// <summary>Gets, via MEF composition, an enumerable collection of available Inventory renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Inventory, RendererExports.Inventory>[] InventoryRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedRoom renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedRoom, RendererExports.PerceivedRoom>[] PerceivedRoomRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedThing renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedThing, RendererExports.PerceivedThing>[] PerceivedThingRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Score renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Score, RendererExports.Score>[] ScoreRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available SplashScreen renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.SplashScreen, RendererExports.SplashScreen>[] SplashScreenRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Who renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Who, RendererExports.Who>[] WhoRenderers { get; private set; }

        public string RenderInventory(Thing player)
        {
            return this.currentInventoryRenderer.Render(player);
        }

        public string RenderPerceivedRoom(Thing viewer, Thing viewedRoom)
        {
            return this.currentPerceivedRoomRenderer.Render(viewer, viewedRoom);
        }

        public string RenderPerceivedThing(Thing viewer, Thing viewedThing)
        {
            return this.currentPerceivedThingRenderer.Render(viewer, viewedThing);
        }

        public string RenderScore(Thing player)
        {
            return this.currentInventoryRenderer.Render(player);
        }

        public string RenderSplashScreen()
        {
            return this.currentSplashScreenRenderer.Render();
        }

        public string RenderWho(Thing player)
        {
            return this.currentWhoRenderer.Render(player);
        }

        /// <summary>Recompose the subcomponents of this Renderer.</summary>
        public void Recompose()
        {
            DefaultComposer.Container.ComposeParts(this);

            // Search each of the renderers for the one which has the highest priority.
            this.currentInventoryRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.InventoryRenderers);
            this.currentPerceivedRoomRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.PerceivedRoomRenderers);
            this.currentPerceivedThingRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.PerceivedThingRenderers);
            this.currentScoreRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.ScoreRenderers);
            this.currentSplashScreenRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.SplashScreenRenderers);
            this.currentWhoRenderer = DefaultComposer.GetLatestPriorityTypeInstance(this.WhoRenderers);
        }
    }
}
