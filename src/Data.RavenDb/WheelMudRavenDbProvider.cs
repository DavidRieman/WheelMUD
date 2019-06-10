//-----------------------------------------------------------------------------
// <copyright file="WheelMudRavenDbProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Raven DB provider for WheelMUD
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using System.ComponentModel.Composition;

    [Export(typeof(IWheelMudDocumentStorageProvider))]
    public class WheelMudRavenDbProvider : IWheelMudDocumentStorageProvider
    {
        public string Name { get; } = "RavenDB";

        public IBasicDocumentSession CreateDatabaseSession()
        {
            var session = DocumentStoreHolder.Instance.OpenSession();
            return new RavenDocumentSessionBridge(session);
        }
    }
}