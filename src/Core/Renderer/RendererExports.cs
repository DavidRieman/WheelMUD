//-----------------------------------------------------------------------------
// <copyright file="RendererExports.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    public static class RendererExports
    {
        public class BaseExportAttribute : ExportAttribute, IExportWithPriority
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            /// <param name="rendererDefinitionType">The system Type of the exported renderer definition.</param>
            /// <remarks>Unfortunately C# won't let us use generics to define ExportAttributes, or this file would be a lot less verbose.</remarks>
            public BaseExportAttribute(int rendererPriority, Type rendererDefinitionType) : base(rendererDefinitionType)
            {
                Priority = rendererPriority;
            }

            /// <summary>Gets or sets the priority of the exported renderer; the renderer with the highest priority will be used.</summary>
            /// <remarks>See DefaultComposer for detailed usage information.</remarks>
            public int Priority { get; set; }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public BaseExportAttribute(IDictionary<string, object> metadata)
            {
                PropertyTools.SetProperties(this, metadata);
            }
        }

        /// <summary>Class for exporting a CommandsCategories renderer for composition into the WheelMUD framework.</summary>
        public class CommandsCategories : BaseExportAttribute
        {
            public CommandsCategories(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.CommandsCategories)) { }

            public CommandsCategories(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a CommandsList renderer for composition into the WheelMUD framework.</summary>
        public class CommandsList : BaseExportAttribute
        {
            public CommandsList(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.CommandsList)) { }

            public CommandsList(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a HelpCommand renderer for composition into the WheelMUD framework.</summary>
        public class HelpCommand : BaseExportAttribute
        {
            public HelpCommand(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.HelpCommand)) { }

            public HelpCommand(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a HelpTopic renderer for composition into the WheelMUD framework.</summary>
        public class HelpTopic : BaseExportAttribute
        {
            public HelpTopic(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.HelpTopic)) { }

            public HelpTopic(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a HelpTopics renderer for composition into the WheelMUD framework.</summary>
        public class HelpTopics : BaseExportAttribute
        {
            public HelpTopics(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.HelpTopics)) { }

            public HelpTopics(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting an Inventory renderer for composition into the WheelMUD framework.</summary>
        public class Inventory : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public Inventory(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.Inventory)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public Inventory(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a PerceivedRoom renderer for composition into the WheelMUD framework.</summary>
        public class PerceivedRoom : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public PerceivedRoom(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.PerceivedRoom)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public PerceivedRoom(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a PerceivedThing renderer for composition into the WheelMUD framework.</summary>
        public class PerceivedThing : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public PerceivedThing(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.PerceivedThing)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public PerceivedThing(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a Score renderer for composition into the WheelMUD framework.</summary>
        public class Score : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public Score(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.Score)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public Score(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a SplashScreen renderer for composition into the WheelMUD framework.</summary>
        public class SplashScreen : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public SplashScreen(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.SplashScreen)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public SplashScreen(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a Who renderer for composition into the WheelMUD framework.</summary>
        public class Who : BaseExportAttribute
        {
            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="rendererPriority">The priority for this renderer. Highest number among composed renderers gets used.</param>
            public Who(int rendererPriority) : base(rendererPriority, typeof(RendererDefinitions.Who)) { }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public Who(IDictionary<string, object> metadata) : base(metadata) { }
        }
    }
}
