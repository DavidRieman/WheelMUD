//-----------------------------------------------------------------------------
// <copyright file="RendererExports.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>A collection of export attributes used for marking a class to be discovered by WheelMUD MEF composition.</summary>
    /// <remarks>
    /// For example, if you want to import a customized HelpCommand renderer to be used instead of the core default version,
    /// implement "RendererDefinitions.HelpCommand" for your class and be mark it with "[RendererExports.HelpCommand(1)]"
    /// </remarks>
    public static class RendererExports
    {
        public class CommandsCategories : BasePrioritizedExportAttribute
        {
            public CommandsCategories(int priority) : base(priority, typeof(RendererDefinitions.CommandsCategories)) { }
            public CommandsCategories(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class CommandsList : BasePrioritizedExportAttribute
        {
            public CommandsList(int priority) : base(priority, typeof(RendererDefinitions.CommandsList)) { }
            public CommandsList(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class HelpCommand : BasePrioritizedExportAttribute
        {
            public HelpCommand(int priority) : base(priority, typeof(RendererDefinitions.HelpCommand)) { }
            public HelpCommand(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class HelpTopic : BasePrioritizedExportAttribute
        {
            public HelpTopic(int priority) : base(priority, typeof(RendererDefinitions.HelpTopic)) { }
            public HelpTopic(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class HelpTopics : BasePrioritizedExportAttribute
        {
            public HelpTopics(int priority) : base(priority, typeof(RendererDefinitions.HelpTopics)) { }
            public HelpTopics(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class Inventory : BasePrioritizedExportAttribute
        {
            public Inventory(int priority) : base(priority, typeof(RendererDefinitions.Inventory)) { }
            public Inventory(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class PerceivedRoom : BasePrioritizedExportAttribute
        {
            public PerceivedRoom(int priority) : base(priority, typeof(RendererDefinitions.PerceivedRoom)) { }
            public PerceivedRoom(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class PerceivedThing : BasePrioritizedExportAttribute
        {
            public PerceivedThing(int priority) : base(priority, typeof(RendererDefinitions.PerceivedThing)) { }
            public PerceivedThing(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class Prompt : BasePrioritizedExportAttribute
        {
            public Prompt(int priority) : base(priority, typeof(RendererDefinitions.Prompt)) { }
            public Prompt(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class Score : BasePrioritizedExportAttribute
        {
            public Score(int priority) : base(priority, typeof(RendererDefinitions.Score)) { }
            public Score(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class SplashScreen : BasePrioritizedExportAttribute
        {
            public SplashScreen(int priority) : base(priority, typeof(RendererDefinitions.SplashScreen)) { }
            public SplashScreen(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class Who : BasePrioritizedExportAttribute
        {
            public Who(int priority) : base(priority, typeof(RendererDefinitions.Who)) { }
            public Who(IDictionary<string, object> metadata) : base(metadata) { }
        }
    }
}
