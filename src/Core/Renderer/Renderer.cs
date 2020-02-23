//-----------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using WheelMUD.Interfaces;

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
        public Lazy<RendererDefinitions.Inventory, ExportInventoryRendererAttribute>[] InventoryRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedRoom renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedRoom, ExportPerceivedRoomRendererAttribute>[] PerceivedRoomRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedThing renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedThing, ExportPerceivedThingRendererAttribute>[] PerceivedThingRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Score renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Score, ExportScoreRendererAttribute>[] ScoreRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available SplashScreen renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.SplashScreen, ExportSplashScreenRendererAttribute>[] SplashScreenRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Who renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Who, ExportWhoRendererAttribute>[] WhoRenderers { get; private set; }

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
            this.currentInventoryRenderer = this.FindActiveRenderer(this.InventoryRenderers);
            this.currentPerceivedRoomRenderer = this.FindActiveRenderer(this.PerceivedRoomRenderers);
            this.currentPerceivedThingRenderer = this.FindActiveRenderer(this.PerceivedThingRenderers);
            this.currentScoreRenderer = this.FindActiveRenderer(this.ScoreRenderers);
            this.currentSplashScreenRenderer = this.FindActiveRenderer(this.SplashScreenRenderers);
            this.currentWhoRenderer = this.FindActiveRenderer(this.WhoRenderers);
        }

        private T FindActiveRenderer<T, U>(IEnumerable<Lazy<T, U>> renderers)
            where U : IExportWithPriority
        {
            var priorityRenderer = (from renderer in renderers
                                    orderby renderer.Metadata.Priority descending
                                    select renderer.Value).First();
            return priorityRenderer;
        }
    }
}
