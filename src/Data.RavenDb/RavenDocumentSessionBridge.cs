//-----------------------------------------------------------------------------
// <copyright file="RavenDocumentSessionBridge.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   RavenDocumentSessionBridge implements the IBasicDocumentSession abstraction for RavenDB,
//   allowing for technology-agnostic persistence. Since IBasicDocumentSession was modeled off
//   of RavenDB's IDocumentSession, most methods will be simple pass-through methods.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using Raven.Client.Documents.Session;

    public class RavenDocumentSessionBridge : IBasicDocumentSession
    {
        private IDocumentSession documentSession;

        public RavenDocumentSessionBridge(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void Delete<T>(T entity)
        {
            this.documentSession.Delete<T>(entity);
        }

        public void Delete(string id)
        {
            this.documentSession.Delete(id);
        }

        public void Dispose()
        {
            this.documentSession.Dispose();
            this.documentSession = null;
        }

        public T Load<T>(string id)
        {
            return this.documentSession.Load<T>(id);
        }

        public void SaveChanges()
        {
            this.documentSession.SaveChanges();
        }

        public void Store<T>(T entity)
        {
            this.documentSession.Store(entity);
        }
    }
}
