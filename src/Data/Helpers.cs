//-----------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System;
    using System.Data;

    /// <summary>Helper methods for the WheelMUD.Data namespace.</summary>
    public class Helpers
    {
        /// <summary>The session factory variable.</summary>
        private static IDbConnection sessionFactory;

        /// <summary>The database provider string value.</summary>
        private static string provider;

        /// <summary>The database connection string variable.</summary>
        private static string connectionString;

        /// <summary>Gets the session factory.</summary>
        /// <value>The session factory.</value>
        private static IDbConnection SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    connectionString = HelperConfigInfo.Instance.ConnectionString;
                    provider = HelperConfigInfo.Instance.Provider;

                    var cache = new ProviderCache();

                    try
                    {
                        IWheelMudDbProvider factory;
                        cache.Providers.TryGetValue(provider.ToLower(), out factory);
                        factory.ConnectionString = connectionString;

                        sessionFactory = factory.CreateDatabaseSession();
                    }
                    catch (Exception)
                    {
                        // NOTE: If you get a FluentConfigurationException here, if the inner exception is 
                        // something like "Could not create the driver from..." then you should first check to 
                        // ensure that the appropriate DLL version referred to in the exception is set up to 
                        // automatically copy to the run directory.  However, if you used an installer for WheelMUD, 
                        // you might need to check if the DLL has been automatically registered with the GAC.
                        throw;
                    }
                }

                return sessionFactory;
            }
        }

        /// <summary>Opens a session for the current database provider.</summary>
        /// <returns>Returns a Session object.</returns>
        public static IDbCommand OpenSession()
        {
            try
            {
                return SessionFactory.CreateCommand();
            }
            catch (NullReferenceException)
            {
                // This usually means that the connection string to the database is not correct.
                // Please fix the connection string in the *.config file and run the server again.
                throw;
            }
        }

        /// <summary>Gets the name of the current database provider.</summary>
        /// <returns>Returns the current database provider name.</returns>
        public static string GetCurrentProviderName()
        {
            HelperConfigInfo config = HelperConfigInfo.Instance;

            return config.Provider;
        }

        /// <summary>Gets the name of the current connection string.</summary>
        /// <returns>Returns the name used as the key for the connection string.</returns>
        public static string GetCurrentConnectionStringName()
        {
            HelperConfigInfo config = HelperConfigInfo.Instance;

            return config.ConnectionStringName;
        }

        /// <summary>Gets the current connection string.</summary>
        /// <returns>Returns the current connection string used to talk to the relational database.</returns>
        public static string GetCurrentConnectionString()
        {
            HelperConfigInfo config = HelperConfigInfo.Instance;

            return config.ConnectionString;
        }
    }
}
