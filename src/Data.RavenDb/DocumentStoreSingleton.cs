//-----------------------------------------------------------------------------
// <copyright file="DocumentStoreSingleton.cs" company="WheelMUD Development Team">
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
    public class DocumentStoreSingleton
    {
        private static EmbeddableDocumentStore instance;

        /// <summary>Gets the instance.</summary>
        public static EmbeddableDocumentStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmbeddableDocumentStore
                    {
                        DataDirectory = DalUtils.GetDbPath()
                    };

                    instance.Initialize();
                }

                return instance;
            }
        }
    }
}