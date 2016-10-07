//-----------------------------------------------------------------------------
// <copyright file="DocumentStore.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Encapsulates the Raven.Client.Embedded.EmbeddableDocumentStore into a singleton.
//   Date: May 15, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using Raven.Client.Embedded;

    /// <summary>Encapsulates the Raven.Client.Embedded.EmbeddableDocumentStore into a singleton.</summary>
    public class DocumentStore
    {
        private static readonly EmbeddableDocumentStore SingletonInstance;

        /// <summary>Initializes static members of the <see cref="DocumentStore"/> class.</summary>
        static DocumentStore()
        {
            SingletonInstance = new EmbeddableDocumentStore
            {
                DataDirectory = DalUtils.GetDbPath()
            };
            SingletonInstance.Initialize();
        }

        /// <summary>Gets the singleton instance.</summary>
        public static EmbeddableDocumentStore Instance
        {
            get { return SingletonInstance; }
        }
    }
}