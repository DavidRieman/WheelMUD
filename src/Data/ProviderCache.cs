//-----------------------------------------------------------------------------
// <copyright file="ProviderCache.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : April 30, 2012
//   Purpose   : Doing the MEF hosting here to load database providers.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;

    public class ProviderCache
    {
        public ProviderCache()
        {
            this.PopulateProvierCache();
        }

        public Dictionary<string, IWheelMudDbProvider> Providers { get; private set; }

        [ImportMany(typeof(IWheelMudDbProvider))]
        private List<IWheelMudDbProvider> DatabaseProviderCache { get; set; }

        private void PopulateProvierCache()
        {
            if (this.Providers == null)
            {
                this.Providers = new Dictionary<string, IWheelMudDbProvider>();
            }

            var catalog = new AggregateCatalog(new DirectoryCatalog("."), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (IWheelMudDbProvider mudDbProvider in this.DatabaseProviderCache)
            {
                this.Providers.Add(mudDbProvider.ProviderNamespace.ToLowerInvariant(), mudDbProvider);
            }
        }
    }
}