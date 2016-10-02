//-----------------------------------------------------------------------------
// <copyright file="WheelMudSqliteProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : April 30, 2012
//   Purpose   :ORMLite Sqlite provider for WheelMUD
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Sqlite
{
    using System;
    using System.ComponentModel.Composition;
    using System.Data;
    using System.IO;
    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.Sqlite;

    /// <summary>ORMLite Sqlite provider for WheelMUD.</summary>
    [Export(typeof(IWheelMudDbProvider))]
    public class WheelMudSqliteProvider : IWheelMudDbProvider
    {
        /// <summary>Initializes a new instance of the <see cref="WheelMudSqliteProvider"/> class.</summary>
        public WheelMudSqliteProvider()
        {
        }

        public WheelMudSqliteProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public string DatabaseName
        {
            get
            {
                return "SQLite";
            }
        }

        public string ProviderNamespace
        {
            get
            {
                return "system.data.sqlite";
            }
        }

        public IDbConnection CreateDatabaseSession()
        {
            VerifyValidSqLiteFile(this.ConnectionString);
            var connectionFactory = new OrmLiteConnectionFactory(this.ConnectionString, false, SqliteOrmLiteDialectProvider.Instance);
            return connectionFactory.OpenDbConnection();
        }

        /// <summary>Verifies that the SQLite DB file specified in a connection string is somewhat valid.</summary>
        /// <param name="connectionStringForSqlite">The SQLite connection string containing the file name.</param>
        private static void VerifyValidSqLiteFile(string connectionStringForSqlite)
        {
            // A connection is allowed to be made to an empty DB file, but the program
            // is not designed to work with an empty DB file; it is assumed data exists.
            // In order to prevent later issues from popping up, try to detect corrupted
            // DB files (or at least empty DB files since we've seen that occasionally).
            // Example connection string: "Data Source=Files\WheelMud.net.db;Version=3;"
            // Everything between the '=' and the first ';' should be the file to check.
            string fileName = connectionStringForSqlite.Substring(connectionStringForSqlite.IndexOf('=') + 1);
            fileName = fileName.Substring(0, fileName.IndexOf(';'));
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(string.Format("SQLite file is missing: {0}", fileName));
            }
            else if (fileInfo.Length <= 0)
            {
                throw new FileLoadException(string.Format("SQLite file is unexpectedly empty: {0}", fileName));
            }
        }
    }
}