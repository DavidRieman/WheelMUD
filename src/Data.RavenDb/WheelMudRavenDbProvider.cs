//-----------------------------------------------------------------------------
// <copyright file="WheelMudRavenDbProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using Raven.Embedded;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>Raven DB provider for WheelMUD.</summary>
    /// <remarks>To use RavenDB as your document database provider, configure DocumentDataProviderName in your App.config to be RavenDB.</remarks>
    [Export(typeof(IWheelMudDocumentStorageProvider))]
    public class WheelMudRavenDbProvider : IWheelMudDocumentStorageProvider
    {
        public string Name { get; } = "RavenDB";

        public IBasicDocumentSession CreateDocumentSession()
        {
            var session = DocumentStoreHolder.Instance.OpenSession();
            return new RavenDocumentSessionBridge(session);
        }

        public void DebugExplore()
        {
            EmbeddedServer.Instance.OpenStudioInBrowser();
        }

        public void Prepare()
        {
            // Not strictly necessary, but creating instance here allows RavenDB to be warmed up before players connect.
            Task.Run(() => DocumentStoreHolder.Instance.Initialize());
        }
    }
}