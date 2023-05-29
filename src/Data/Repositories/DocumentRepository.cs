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
    public static class DocumentRepository
    {
        public static void Save(IIdentifiable obj)
        {
            using var session = Helpers.OpenDocumentSession();
            // RavenDB will automatically either insert a new document or update the
            // existing document with this document ID, as appropriate.
            session.Store(obj);
            session.SaveChanges();
        }

        public static void SaveAll(params IIdentifiable[] objects)
        {
            // Similar to Save above, but optimized for multiple objects being stored in one batch.
            using var session = Helpers.OpenDocumentSession();
            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    session.Store(obj);
                }
            }
            session.SaveChanges();
        }
    }

    /// <summary>Generic document repository implementation for type T.</summary>
    public static class DocumentRepository<T> where T : IIdentifiable, new()
    {
        public static void SaveTree(T mainDocument, Func<T, IEnumerable<T>> childDocumentFinder)
        {
            using var session = Helpers.OpenDocumentSession();
            AddChildTreeToSession(session, mainDocument, childDocumentFinder);
            session.SaveChanges();
        }

        public static T Load(string id)
        {
            using var session = Helpers.OpenDocumentSession();
            return session.Load<T>(id);
        }

        private static void AddChildTreeToSession(IBasicDocumentSession session, T currentDocument, Func<T, IEnumerable<T>> childDocumentFinder)
        {
            // Depth First: Our persistence pattern is for parents to track their children only, so the inner-most
            // descendants have to be added first to ensure they have DB IDs first. Then cascading up, each layer will
            // be able to build their ChildrenIds properties and such correctly against the now-known IDs.
            foreach (T childDocument in childDocumentFinder(currentDocument))
            {
                AddChildTreeToSession(session, childDocument, childDocumentFinder);
            }
            session.Store(currentDocument);
        }
    }
}