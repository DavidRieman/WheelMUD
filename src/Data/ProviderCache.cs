﻿//-----------------------------------------------------------------------------
// <copyright file="ProviderCache.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace WheelMUD.Data
{
    /// <summary>The ProviderCache manages discovery of the database providers (through MEF).</summary>
    public class ProviderCache
    {
        public ProviderCache()
        {
            PopulateProvierCache();
        }

        [ImportMany(typeof(IWheelMudDocumentStorageProvider))]
        public List<IWheelMudDocumentStorageProvider> DocumentStorageProviders { get; set; }

        private void PopulateProvierCache()
        {
            var catalog = new AggregateCatalog(new DirectoryCatalog("."), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}