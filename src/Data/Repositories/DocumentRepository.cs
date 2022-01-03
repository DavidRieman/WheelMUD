//-----------------------------------------------------------------------------
// <copyright file="DocumentRepository.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Data.Repositories
{
    /// <summary>Generic document repository implementation for type T.</summary>
    public static class DocumentRepository<T> where T : IIdentifiable, new()
    {
        public static void SaveTree(T mainDocument, Func<T, IEnumerable<T>> childDocumentFinder)
        {
            using var session = Helpers.OpenDocumentSession();
            AddChildTreeToSession(session, mainDocument, childDocumentFinder);
            session.SaveChanges();
        }

        public static void Save(T obj)
        {
            using var session = Helpers.OpenDocumentSession();
            // RavenDB will automatically either insert a new document or update the
            // existing document with this document ID, as appropriate.
            session.Store(obj);
            session.SaveChanges();
        }

        public static T Load(string id)
        {
            using var session = Helpers.OpenDocumentSession();
            return session.Load<T>(id);
        }

        private static void AddChildTreeToSession(IBasicDocumentSession session, T currentDocument, Func<T, IEnumerable<T>> childDocumentFinder)
        {
            session.Store(currentDocument);
            foreach (T childDocument in childDocumentFinder(currentDocument))
            {
                AddChildTreeToSession(session, childDocument, childDocumentFinder);
            }
        }
    }
}