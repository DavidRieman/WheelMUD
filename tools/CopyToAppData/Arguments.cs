//-----------------------------------------------------------------------------
// <copyright file="Arguments.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   A program for copying a folder into the ProgramData area of the OS.
// </summary>
//-----------------------------------------------------------------------------

namespace CopyToAppData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility class to parse any time of command line argument.
    /// </summary>
    public class Arguments
    {
        // Variables
        private readonly StringDictionary parameters;
        private readonly Hashtable intParameters = new Hashtable();
        private readonly int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> class.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public Arguments(IEnumerable<string> args)
        {
            this.parameters = new StringDictionary();
            var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string text in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                string[] parts = spliter.Split(text, 3);

                switch (parts.Length)
                {
                        // Found a value (for the last parameter 
                        // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                parts[0] = remover.Replace(parts[0], "$1");

                                this.parameters.Add(parameter, parts[0]);
                                this.intParameters.Add(this.index, parts[0]);

                                this.index += 1;
                            }

                            parameter = null;
                        }

                        // else Error: no parameter waiting for a value (skipped)
                        break;

                        // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                                this.intParameters.Add(this.index, parameter);

                                this.index += 1;
                            }
                        }

                        parameter = parts[1];
                        break;

                        // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                                this.intParameters.Add(this.index, parameter);

                                this.index += 1;
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!this.parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            this.parameters.Add(parameter, parts[2]);
                            this.intParameters.Add(this.index, parts[2]);

                            this.index += 1;
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!this.parameters.ContainsKey(parameter))
                {
                    this.parameters.Add(parameter, "true");
                    this.intParameters.Add(this.index, parameter);

                    this.index += 1;
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.parameters.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified parameter.
        /// </summary>
        /// <param name="param">The parameter name to retrieve.</param>
        /// <returns>The parameter value.</returns>
        /// <remarks>
        /// Retrieve a parameter value if it exists (overriding C# indexer property)
        /// </remarks>
        public string this[string param]
        {
            get
            {
                return this.parameters[param];
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <param name="parameterIndex">The index for the parameter.</param>
        /// <returns>The parameter value.</returns>
        public string this[int parameterIndex]
        {
            get
            {
                return this.intParameters[parameterIndex].ToString();
            }
        }
    }
}