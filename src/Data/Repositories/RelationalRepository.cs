//-----------------------------------------------------------------------------
// <copyright file="Repository.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using ServiceStack.OrmLite;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>Generic relational repository implementation for type T.</summary>
    public class RelationalRepository<T> where T : new()
    {
        public long Add(T obj)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Insert<T>(obj, selectIdentity: true);
        }

        public void Update(T obj)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            session.Connection.Update(obj);
        }

        public void Remove(T obj)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            session.Connection.Delete(obj);
        }

        public T GetById(long id)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.SingleWhere<T>("ID", id);
        }

        public T GetByName(string name)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.SingleWhere<T>("Name", name);
        }

        public ICollection<T> GetAll()
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<T>();
        }
    }
}