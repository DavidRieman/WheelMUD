//-----------------------------------------------------------------------------
// <copyright file="ViewEngine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using NVelocity;
    using NVelocity.App;
    using WheelMUD.Utilities;

    /// <summary>The view engine.</summary>
    public class ViewEngine
    {
        /// <summary>A cache of the loaded templates.</summary>
        private static readonly Dictionary<string, string> CachedTemplates;

        /// <summary>Object to protect the access to the Cached Templates.</summary>
        private static readonly object LockObject = new object();

        /// <summary>The velocity context.</summary>
        private readonly VelocityContext velocityContext = new VelocityContext();

        /// <summary>Initializes static members of the ViewEngine class.</summary>
        static ViewEngine()
        {
            Velocity.Init();
            CachedTemplates = new Dictionary<string, string>();
        }

        /// <summary>Initializes a new instance of the ViewEngine class.</summary>
        public ViewEngine()
        {
            this.ReplaceNewLine = true;
        }

        /// <summary>Gets or sets a value indicating whether the engine replaces any new lines found in the text it is rendering.</summary>
        /// <remarks>This is used in the word wrapping bits. Default is true.</remarks>
        public bool ReplaceNewLine { get; set; }

        /// <summary>Adds data to the context to allow it to be evaluated at runtime.</summary>
        /// <remarks>This data is available for the lifetime of the object
        /// so be careful what you put in here. ie dont put stuff that is
        /// going to change a lot such as room or inventory info.
        /// For transient context use the RenderView overload with a context list</remarks>
        /// <param name="key">The key to refer to the object as in the view.</param>
        /// <param name="context">The object you want evaluating.</param>
        public void AddContext(string key, object context)
        {
            this.velocityContext.Put(key, context);
        }

        /// <summary>Renders the description of a thing.</summary>
        /// <param name="thing">The thing to render a view of.</param>
        /// <returns>A string representing the rendered view of the things description.</returns>
        public string RenderView(Thing thing)
        {
            return this.RenderView(thing.Description);
        }

        /// <summary>Renders a string.</summary>
        /// <param name="textToRender">The string to render.</param>
        /// <returns>A rendered string.</returns>
        public string RenderView(string textToRender)
        {
            return this.RenderView(textToRender, null);
        }

        /// <summary>
        /// Renders a string using the context specified along with any context already in "in memory"
        /// The context supplied here will not be stored. This context will also override any "in memory"
        /// context for this render.
        /// </summary>
        /// <param name="textToRender">The text to render.</param>
        /// <param name="context">Hashtable of items that will be evaluated at runtime.</param>
        /// <returns>A rendered string.</returns>
        public string RenderView(string textToRender, Hashtable context)
        {
            var writer = new StringWriter();
            if (context != null)
            {
                var velContext = new VelocityContext(context);

                foreach (object key in this.velocityContext.Keys)
                {
                    if (!context.ContainsKey(key.ToString()))
                    {
                        context.Add(key.ToString(), this.velocityContext.InternalGet(key.ToString()));
                    }
                }

                Velocity.Evaluate(velContext, writer, "View", textToRender);
            }
            else
            {
                Velocity.Evaluate(this.velocityContext, writer, "View", textToRender);
            }

            return this.ReplaceNewLine
                ? writer.GetStringBuilder().ToString().Replace(Environment.NewLine, string.Empty).Replace("\t", string.Empty)
                : writer.GetStringBuilder().ToString();
        }

        /// <summary>
        /// Renders a string using the context specified along with any context already in "in memory"
        /// The context supplied here will not be stored. This context will also override any "in memory"
        /// context for this render.
        /// </summary>
        /// <param name="fileName">The name of the view template to render.</param>
        /// <param name="context">Hashtable of items that will be evaluated at runtime.</param>
        /// <returns>A rendered string.</returns>
        public string RenderCachedView(string fileName, Hashtable context)
        {
            string textToRender = GetTemplate(fileName);

            return this.RenderView(textToRender, context);
        }

        /// <summary>Gets a template from the cache or loads it directly and adds it to the cache.</summary>
        /// <param name="templateName">Name of the template to get.</param>
        /// <returns>The contents of the template file.</returns>
        private static string GetTemplate(string templateName)
        {
            lock (LockObject)
            {
                if (CachedTemplates.ContainsKey(templateName))
                {
                    return CachedTemplates[templateName];
                }
                else
                {
                    string template = LoadTemplate(templateName);

                    CachedTemplates.Add(templateName, template);

                    return template;
                }
            }
        }

        /// <summary>Loads a template from disk.</summary>
        /// <param name="templateName">The name of the template to load.</param>
        /// <returns>the contents of the template file.</returns>
        private static string LoadTemplate(string templateName)
        {
            string fileName = Path.Combine(Configuration.GetDataStoragePath(), templateName);
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            try
            {
                using (var fs = new StreamReader(fileName))
                {
                    return fs.ReadToEnd();
                }
            }
            catch (DirectoryNotFoundException)
            {
                return string.Empty;
            }
        }
    }
}