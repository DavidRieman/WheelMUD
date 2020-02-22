//-----------------------------------------------------------------------------
// <copyright file="DocumentStore.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Instantiates and houses one RavenDB document store through a singleton.
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
        private static IDocumentStore CreateDocumentStore()
        {
            // TODO: To allow for using a remote RavenDB server, we could use ConnectionString details to choose
            // to initialize the DocumentStore remotely instead of via an EmbeddedServer.  For implementation
            // details, see: https://demo.ravendb.net/demos/basics/the-document-store
            // and: https://ravendb.net/docs/article-page/3.0/Csharp/client-api/setting-up-connection-string
            if (!"embedded".Equals(AppConfigInfo.Instance.DocumentConnectionString, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NotImplementedException("WheelMUD.Data.RavenDb only supports embedded mode.");
            }

            EmbeddedServer.Instance.StartServer();

            // If you get here with an exception like "The specified framework 'Microsoft.NETCore.App',
            // version '2.2.8' was not found...", then that is probably the only basic pre-requisite you
            // are missing. Check for current dependencies and find installation links from:
            //   https://github.com/WheelMud/WheelMUD/blob/master/Documentation/BasicPrerequisites.md
            return EmbeddedServer.Instance.GetDocumentStore("Embedded");
        }

        public static IDocumentStore Instance { get; } = CreateDocumentStore();
    }
}