//-----------------------------------------------------------------------------
// <copyright file="DocumentStore.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Encapsulates the Raven.Client.Embedded.EmbeddableDocumentStore into a singleton.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using Raven.Client.Documents;
    using Raven.Embedded;
    using System;

    /// <summary>Encapsulates the RavenDB DocumentStore into a singleton.</summary>
    public class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateDocumentStore);

        private static IDocumentStore CreateDocumentStore()
        {
            // TODO: To allow for using a remote RavenDB server, we could read app.config details and choose
            // to initialize the DocumentStore remotely instead of via an EmbeddedServer.  For implementation
            // details, see: https://demo.ravendb.net/demos/basics/the-document-store
            EmbeddedServer.Instance.StartServer();
            return EmbeddedServer.Instance.GetDocumentStore("Embedded");
        }

        public static IDocumentStore Instance
        {
            get { return store.Value; }
        }
    }
}