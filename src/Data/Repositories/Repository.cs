//-----------------------------------------------------------------------------
// <copyright file="Repository.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using ServiceStack.OrmLite;

    /// <summary>Generic repository implementation for type T.</summary>
    public class Repository<T> where T : new()
    {
        public void Add(T obj)
        {
            using (IDbCommand session = Helpers.OpenSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Save(obj);
                transaction.Commit();
            }
        }

        public void Update(T obj)
        {
            using (IDbCommand session = Helpers.OpenSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Update(obj);
                transaction.Commit();
            }
        }

        public void Remove(T obj)
        {
            using (IDbCommand session = Helpers.OpenSession())
            using (IDbTransaction transaction = session.Connection.BeginTransaction())
            {
                session.Connection.Delete(obj);
                transaction.Commit();
            }
        }

        public T GetById(long id)
        {
            using (IDbCommand session = Helpers.OpenSession())
                return session.Connection.SingleWhere<T>("ID = {0}", id);
        }

        public T GetByName(string name)
        {
            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.SingleWhere<T>("Name = {0}", name);
            }
        }

        public ICollection<T> GetAll()
        {
            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<T>();
            }
        }
    }
}