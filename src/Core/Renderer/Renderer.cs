//-----------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    public class Renderer
    {
        /// <summary>The singleton instance of the Renderer class.</summary>
        public static Renderer Instance { get; } = new Renderer();

        /// <summary>Prevents a default instance of the Renderer class from being created.</summary>
        private Renderer()
        {
            Recompose();
        }

        /// <summary>The current CommandsCategories renderer.</summary>
        private RendererDefinitions.CommandsCategories currentCommandsCategoriesRenderer;

        /// <summary>The current CommandsList renderer.</summary>
        private RendererDefinitions.CommandsList currentCommandsListRenderer;

        /// <summary>The current HelpCommand renderer.</summary>
        private RendererDefinitions.HelpCommand currentHelpCommandRenderer;

        /// <summary>The current HelpTopic renderer.</summary>
        private RendererDefinitions.HelpTopic currentHelpTopicRenderer;

        /// <summary>The current HelpTopics renderer.</summary>
        private RendererDefinitions.HelpTopics currentHelpTopicsRenderer;

        /// <summary>The current Inventory renderer.</summary>
        private RendererDefinitions.Inventory currentInventoryRenderer;

        /// <summary>The current PerceivedRoom renderer.</summary>
        private RendererDefinitions.PerceivedRoom currentPerceivedRoomRenderer;

        /// <summary>The current PerceivedThing renderer.</summary>
        private RendererDefinitions.PerceivedThing currentPerceivedThingRenderer;

        /// <summary>The current Prompt renderer.</summary>
        private RendererDefinitions.Prompt currentPromptRenderer;

        /// <summary>The current Score renderer.</summary>
        private RendererDefinitions.Score currentScoreRenderer;

        /// <summary>The current SplashScreen renderer.</summary>
        private RendererDefinitions.SplashScreen currentSplashScreenRenderer;

        /// <summary>The current Who renderer.</summary>
        private RendererDefinitions.Who currentWhoRenderer;

        /// <summary>Gets, via MEF composition, an enumerable collection of available CommandsCategories renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.CommandsCategories, RendererExports.CommandsCategories>[] CommandsCategoriesRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available CommandsList renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.CommandsList, RendererExports.CommandsList>[] CommandsListRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available HelpCommand renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.HelpCommand, RendererExports.HelpCommand>[] HelpCommandRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available HelpTopic renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.HelpTopic, RendererExports.HelpTopic>[] HelpTopicRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available HelpTopics renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.HelpTopics, RendererExports.HelpTopics>[] HelpTopicsRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Inventory renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Inventory, RendererExports.Inventory>[] InventoryRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedRoom renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedRoom, RendererExports.PerceivedRoom>[] PerceivedRoomRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedThing renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.PerceivedThing, RendererExports.PerceivedThing>[] PerceivedThingRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available PerceivedThing renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Prompt, RendererExports.Prompt>[] PromptRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Score renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Score, RendererExports.Score>[] ScoreRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available SplashScreen renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.SplashScreen, RendererExports.SplashScreen>[] SplashScreenRenderers { get; private set; }

        /// <summary>Gets, via MEF composition, an enumerable collection of available Who renderers.</summary>
        [ImportMany]
        public Lazy<RendererDefinitions.Who, RendererExports.Who>[] WhoRenderers { get; private set; }

        public OutputBuilder RenderCommandsCategories(TerminalOptions terminalOptions, IEnumerable<Command> commands)
        {
            return currentCommandsCategoriesRenderer.Render(terminalOptions, commands);
        }

        public OutputBuilder RenderCommandsList(TerminalOptions terminalOptions, IEnumerable<Command> commands, string categoryName)
        {
            return currentCommandsListRenderer.Render(terminalOptions, commands, categoryName);
        }

        public OutputBuilder RenderHelpCommand(TerminalOptions terminalOptions, Command command)
        {
            return currentHelpCommandRenderer.Render(terminalOptions, command);
        }

        public OutputBuilder RenderHelpTopic(TerminalOptions terminalOptions, HelpTopic helpTopic)
        {
            return currentHelpTopicRenderer.Render(terminalOptions, helpTopic);
        }

        public OutputBuilder RenderHelpTopics(TerminalOptions terminalOptions)
        {
            return currentHelpTopicsRenderer.Render(terminalOptions);
        }

        public OutputBuilder RenderInventory(TerminalOptions terminalOptions, Thing player)
        {
            return currentInventoryRenderer.Render(terminalOptions, player);
        }

        public OutputBuilder RenderPerceivedRoom(TerminalOptions terminalOptions, Thing viewer, Thing viewedRoom)
        {
            return currentPerceivedRoomRenderer.Render(terminalOptions, viewer, viewedRoom);
        }

        public OutputBuilder RenderPerceivedThing(TerminalOptions terminalOptions, Thing viewer, Thing viewedThing)
        {
            return currentPerceivedThingRenderer.Render(terminalOptions, viewer, viewedThing);
        }

        public OutputBuilder RenderPrompt(TerminalOptions terminalOptions, Thing player)
        {
            return currentPromptRenderer.Render(terminalOptions, player);
        }

        public OutputBuilder RenderScore(TerminalOptions terminalOptions, Thing player)
        {
            return currentScoreRenderer.Render(terminalOptions, player);
        }

        public OutputBuilder RenderSplashScreen(TerminalOptions terminalOptions)
        {
            return currentSplashScreenRenderer.Render(terminalOptions);
        }

        public OutputBuilder RenderWho(TerminalOptions terminalOptions, Thing player)
        {
            return currentWhoRenderer.Render(terminalOptions, player);
        }

        /// <summary>Recompose the subcomponents of this Renderer.</summary>
        public void Recompose()
        {
            DefaultComposer.Container.ComposeParts(this);

            // Search each of the renderers for the one which has the highest priority.
            currentCommandsCategoriesRenderer = DefaultComposer.GetInstance(CommandsCategoriesRenderers);
            currentCommandsListRenderer = DefaultComposer.GetInstance(CommandsListRenderers);
            currentHelpCommandRenderer = DefaultComposer.GetInstance(HelpCommandRenderers);
            currentHelpTopicRenderer = DefaultComposer.GetInstance(HelpTopicRenderers);
            currentHelpTopicsRenderer = DefaultComposer.GetInstance(HelpTopicsRenderers);
            currentInventoryRenderer = DefaultComposer.GetInstance(InventoryRenderers);
            currentPerceivedRoomRenderer = DefaultComposer.GetInstance(PerceivedRoomRenderers);
            currentPerceivedThingRenderer = DefaultComposer.GetInstance(PerceivedThingRenderers);
            currentPromptRenderer = DefaultComposer.GetInstance(PromptRenderers);
            currentScoreRenderer = DefaultComposer.GetInstance(ScoreRenderers);
            currentSplashScreenRenderer = DefaultComposer.GetInstance(SplashScreenRenderers);
            currentWhoRenderer = DefaultComposer.GetInstance(WhoRenderers);
        }
    }
}
