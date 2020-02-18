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

            // Exceptions at the following line may be due to the RavenDBServer folder failing to get set up
            // correctly and automatically in the applicable binDebug or binRelease folder. Check that the
            // folder exists and contains a bunch of files, including Raven.Server.dll and so on.
            // TODO: Soon after we port to .NET Core, we hope to upgrade RavenDB version as well; This seems
            // like the new version will handle setting up embedded mode in a more consistent manner.
            // Until then, you may want to keep a copy of the RavenDBServer folder handy for manual repairs.
            return EmbeddedServer.Instance.GetDocumentStore("Embedded");
        }

        public static IDocumentStore Instance { get; } = CreateDocumentStore();
    }
}