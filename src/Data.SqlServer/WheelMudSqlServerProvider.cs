//-----------------------------------------------------------------------------
// <copyright file="WheelMudSqlServerProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : April 30, 2012
//   Purpose   : ORMLite Microsoft SQL Server provider for WheelMUD
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.SqlServer
{
    using System.ComponentModel.Composition;
    using System.Data;
    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.SqlServer;

    /// <summary>ORMLite Microsoft SQL Server provider for WheelMUD.</summary>
    [Export(typeof(IWheelMudDbProvider))]
    public class WheelMudSqlServerProvider : IWheelMudDbProvider
    {
        public WheelMudSqlServerProvider()
        {
        }

        public WheelMudSqlServerProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public string DatabaseName
        {
            get { return "Microsoft SQL Server"; }
        }

        public string ProviderNamespace
        {
            get { return "system.data.sqlclient"; }
        }

        public IDbConnection CreateDatabaseSession()
        {
            var connectionFactory = new OrmLiteConnectionFactory(this.ConnectionString, false, SqlServerOrmLiteDialectProvider.Instance);
            IDbConnection connection = connectionFactory.OpenDbConnection();
            return connection;
        }
    }
}