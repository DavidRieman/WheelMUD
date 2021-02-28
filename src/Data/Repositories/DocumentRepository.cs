//-----------------------------------------------------------------------------
// <copyright file="DocumentRepository.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Data.Repositories
{
    /// <summary>Generic document repository implementation for type T.</summary>
    public static class DocumentRepository<T> where T : IIdentifiable, new()
    {
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
    }
}