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
    using System.Linq;

    /// <summary>Helper methods for the WheelMUD.Data namespace.</summary>
    public class Helpers
    {
        /// <summary>The session factory variable.</summary>
        private static IDbConnection sessionFactory;

        private static IWheelMudRelationalDbProvider configuredRelationalDatabaseProvider;
        private static IWheelMudDocumentStorageProvider configuredDocumentStorageProvider;

        static Helpers()
        {
            var providerCache = new ProviderCache();
            var configuredRelationalProviderName = AppConfigInfo.Instance.RelationalDataProviderName;
            var configuredDocumentStorageProviderName = AppConfigInfo.Instance.DocumentDataProviderName;
            configuredRelationalDatabaseProvider = (from provider in providerCache.RelationalDatabaseProviders
                                                    where provider.DatabaseName.Equals(configuredRelationalProviderName, StringComparison.OrdinalIgnoreCase)
                                                    select provider).FirstOrDefault();
            if (configuredRelationalDatabaseProvider == null)
            {
                throw new DataException("Could not find the configured relational database provider: " + configuredRelationalProviderName);
            }

            configuredDocumentStorageProvider = (from provider in providerCache.DocumentStorageProviders
                                                 where provider.Name.Equals(configuredDocumentStorageProviderName, StringComparison.OrdinalIgnoreCase)
                                                 select provider).FirstOrDefault();
            if (configuredDocumentStorageProvider == null)
            {
                throw new DataException("Could not find the configured document storage provider: " + configuredDocumentStorageProviderName);
            }
            configuredDocumentStorageProvider.Prepare();
        }

        /// <summary>Gets the session factory.</summary>
        /// <value>The session factory.</value>
        private static IDbConnection RelationalSessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    var connectionString = AppConfigInfo.Instance.RelationalConnectionString;
                    try
                    {
                        configuredRelationalDatabaseProvider.ConnectionString = connectionString;
                        sessionFactory = configuredRelationalDatabaseProvider.CreateDatabaseSession();
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
        public static IDbCommand OpenRelationalSession()
        {
            try
            {
                return RelationalSessionFactory.CreateCommand();
            }
            catch (NullReferenceException)
            {
                // This usually means that the connection string to the database is not correct.
                // Please fix the connection string in the *.config file and run the server again.
                throw;
            }
        }

        public static IBasicDocumentSession OpenDocumentSession()
        {
            return configuredDocumentStorageProvider.CreateDocumentSession();
        }

        public static void DebugExploreDocuments()
        {
            configuredDocumentStorageProvider.DebugExplore();
        }
    }
}
