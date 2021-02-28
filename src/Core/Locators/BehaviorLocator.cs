//-----------------------------------------------------------------------------
// <copyright file="BehaviorLocator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core.Locators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>A locator class that deals with behaviors.</summary>
    public class BehaviorLocator : IServiceLocator
    {
        private IDictionary<Type, object> instantiatedServices;
        private IDictionary<Type, Type> servicesType;

        private BehaviorLocator()
        {
            instantiatedServices = new Dictionary<Type, object>();
            servicesType = new Dictionary<Type, Type>();
        }

        /// <summary>Gets the singleton instance of the <see cref="BehaviorLocator"/> class.</summary>
        public static IServiceLocator Instance { get; } = new BehaviorLocator();

        /// <summary>Registers the service.</summary>
        /// <typeparam name="T">Type signature of the behavior being registered.</typeparam>
        /// <param name="behaviorToRegister">The behavior to register.</param>
        public void RegisterService<T>(T behaviorToRegister)
        {
            instantiatedServices.Add(typeof(T), behaviorToRegister);
        }

        /// <summary>Gets the service.</summary>
        /// <typeparam name="T">The signature (class) that we are looking for.</typeparam>
        /// <returns>Returns an instance of the class in question, if it exists.</returns>
        public T GetService<T>()
        {
            if (instantiatedServices.ContainsKey(typeof(T)))
            {
                return (T)instantiatedServices[typeof(T)];
            }

            // lazy initialization
            try
            {
                // use reflection to invoke the service
                ConstructorInfo constructor = servicesType[typeof(T)].GetConstructor(new Type[0]);
                Debug.Assert(constructor != null, "Cannot find a suitable constructor for " + typeof(T));

                T service = (T)constructor.Invoke(null);

                // add the service to the ones that we have already instantiated
                instantiatedServices.Add(typeof(T), service);

                return service;
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException("The requested service is not registered");
            }
        }
    }
}