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
                this.Priority = rendererPriority;
            }

            /// <summary>Gets or sets the priority of the exported renderer; the renderer with the highest priority will be used.</summary>
            public int Priority { get; set; }

            /// <summary>Initializes a new instance of the class.</summary>
            /// <param name="metadata">The metadata.</param>
            public BaseExportAttribute(IDictionary<string, object> metadata)
            {
                PropertyTools.SetProperties(this, metadata);
            }
        }

        /// <summary>Class for exporting an Inventory renderer for composition into the WheelMUD framework.</summary>
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
