//-----------------------------------------------------------------------------
// <copyright file="IBasicDocumentSession.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Linq;

namespace WheelMUD.Data
{
    /// <summary>
    /// Provides a layer of abstraction over document storage sessions which allows for technology-agnostic
    /// persistence approach. This means the admin can configure WheelMUD to use a tech like RavenDB, or can
    /// build and configure another document DB (or even a direct-to-disk implementation) if so desired.
    /// </summary>
    public interface IBasicDocumentSession : IDisposable
    {
        void Delete<T>(T entity);
        void Delete(string id);
        T Load<T>(string id);
        IOrderedQueryable<T> Query<T>();
        void Store<T>(T entity);
        void SaveChanges();
    }
}
