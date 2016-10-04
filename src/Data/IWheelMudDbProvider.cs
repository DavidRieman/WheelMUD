//-----------------------------------------------------------------------------
// <copyright file="IWheelMudDbProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : April 30, 2012
//   Purpose   : Interface for supporting different database back ends under
//               ORMLite https://github.com/ServiceStack/ServiceStack.OrmLite
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Data;

    /// <summary>Interface for supporting different database back ends under ORMLite.</summary>
    public interface IWheelMudDbProvider
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; }

        string ProviderNamespace { get; }

        IDbConnection CreateDatabaseSession();
    }
}