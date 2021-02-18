//-----------------------------------------------------------------------------
// <copyright file="RavenDocumentSessionBridge.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using Raven.Client.Documents.Session;
    using System.Linq;

    /// <summary>
    /// RavenDocumentSessionBridge implements the IBasicDocumentSession abstraction for RavenDB,
    /// allowing for technology-agnostic persistence. Since IBasicDocumentSession was modeled off
    /// of RavenDB's IDocumentSession, most methods will be simple pass-through methods.
    /// </summary>
    public class RavenDocumentSessionBridge : IBasicDocumentSession
    {
        private IDocumentSession documentSession;

        public RavenDocumentSessionBridge(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void Delete<T>(T entity)
        {
            documentSession.Delete<T>(entity);
        }

        public void Delete(string id)
        {
            documentSession.Delete(id);
        }

        public void Dispose()
        {
            documentSession.Dispose();
            documentSession = null;
        }

        public T Load<T>(string id)
        {
            return documentSession.Load<T>(id);
        }

        public IOrderedQueryable<T> Query<T>()
        {
            return documentSession.Query<T>();
        }

        public void SaveChanges()
        {
            documentSession.SaveChanges();
        }

        public void Store<T>(T entity)
        {
            documentSession.Store(entity);
        }
    }
}
