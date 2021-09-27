//-----------------------------------------------------------------------------
// <copyright file="RendererDefinitions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides definitions for the set of easily-overridden view renderers.
    /// There will be a default renderer for each of these housed here in WheelMUD.Core.
    /// However, each will also have an associated export attributes for a customized renderer to claim priority over the default.
    /// </summary>
    public static class RendererDefinitions
    {
        public abstract class CommandsCategories
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, IEnumerable<Command> commands);
        }

        public abstract class CommandsList
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, IEnumerable<Command> commands, string categoryName);
        }

        public abstract class HelpCommand
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Command command);
        }

        public abstract class HelpTopic
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Core.HelpTopic helpTopic);
        }

        public abstract class HelpTopics
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions);
        }

        public abstract class Inventory
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing player);
        }

        public abstract class PerceivedRoom
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing viewer, Thing viewedRoom);
        }

        public abstract class PerceivedThing
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing viewer, Thing viewedThing);
        }

        public abstract class Prompt
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing player);
        }

        public abstract class Score
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing player);
        }

        public abstract class SplashScreen
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions);
        }

        public abstract class Who
        {
            public abstract OutputBuilder Render(TerminalOptions terminalOptions, Thing player);
        }
    }
}
