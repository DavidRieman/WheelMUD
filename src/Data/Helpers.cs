//-----------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Data;
using System.Linq;

namespace WheelMUD.Data
{
    /// <summary>Helper methods for the WheelMUD.Data namespace.</summary>
    public class Helpers
    {
        private static IWheelMudDocumentStorageProvider configuredDocumentStorageProvider;

        static Helpers()
        {
            var providerCache = new ProviderCache();
            var configuredDocumentStorageProviderName = AppConfigInfo.Instance.DocumentDataProviderName;

            configuredDocumentStorageProvider = (from provider in providerCache.DocumentStorageProviders
                                                 where provider.Name.Equals(configuredDocumentStorageProviderName, StringComparison.OrdinalIgnoreCase)
                                                 select provider).FirstOrDefault();
            if (configuredDocumentStorageProvider == null)
            {
                throw new DataException("Could not find the configured document storage provider: " + configuredDocumentStorageProviderName);
            }
            configuredDocumentStorageProvider.Prepare();
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
