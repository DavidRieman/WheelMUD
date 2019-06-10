//-----------------------------------------------------------------------------
// <copyright file="DocumentRepository.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>Generic document repository implementation for type T.</summary>
    public class DocumentRepository<T> where T : new() // @@@ ### ENSURE T HAS "Id" PROPERTY VIA INTERFACE -> INTERFACE TO Thing.cs
    {
        public void Save(T obj)
        {

        }
        /*
        public void Add(T obj)
        {
            using (IDocumentSession session = Helpers.OpenDocumentSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Save(obj);
                transaction.Commit();
            }
        }

        public void Update(T obj)
        {
            using (IDbCommand session = Helpers.OpenDocumentSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Update(obj);
                transaction.Commit();
            }
        }

        public void Remove(T obj)
        {
            using (IDbCommand session = Helpers.OpenDocumentSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Delete(obj);
                transaction.Commit();
            }
        }

        public T GetById(long id)
        {
            using (IDbCommand session = Helpers.OpenDocumentSession())
                return session.Connection.SingleWhere<T>("ID = {0}", id);
        }

        public T GetByName(string name)
        {
            using (IDbCommand session = Helpers.OpenDocumentSession())
            {
                return session.Connection.SingleWhere<T>("Name = {0}", name);
            }
        }

        public ICollection<T> GetAll()
        {
            using (IDbCommand session = Helpers.OpenDocumentSession())
            {
                return session.Connection.Select<T>();
            }
        }*/
    }
}