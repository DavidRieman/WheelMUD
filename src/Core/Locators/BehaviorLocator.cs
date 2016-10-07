//-----------------------------------------------------------------------------
// <copyright file="BehaviorLocator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//    A locator class that deals with behaviors.
//    Created By : Fastalanasa
//    Created On : May 12, 2011  
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Locators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using WheelMUD.Interfaces;

    /// <summary>A locator class that deals with behaviors.</summary>
    public class BehaviorLocator : IServiceLocator
    {
        private static readonly IServiceLocator SingletonInstance = new BehaviorLocator();
        private IDictionary<Type, object> instantiatedServices;
        private IDictionary<Type, Type> servicesType;

        private BehaviorLocator()
        {
            this.instantiatedServices = new Dictionary<Type, object>();
            this.servicesType = new Dictionary<Type, Type>();
        }

        /// <summary>Gets the singleton instance of the <see cref="BehaviorLocator"/> class.</summary>
        public static IServiceLocator Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Registers the service.</summary>
        /// <typeparam name="T">Type signature of the behavior being registered.</typeparam>
        /// <param name="behaviorToRegister">The behavior to register.</param>
        public void RegisterService<T>(T behaviorToRegister)
        {
            this.instantiatedServices.Add(typeof(T), behaviorToRegister);
        }

        /// <summary>Gets the service.</summary>
        /// <typeparam name="T">The signature (class) that we are looking for.</typeparam>
        /// <returns>Returns an instance of the class in question, if it exists.</returns>
        public T GetService<T>()
        {
            if (this.instantiatedServices.ContainsKey(typeof(T)))
            {
                return (T)this.instantiatedServices[typeof(T)];
            }

            // lazy initialization
            try
            {
                // use reflection to invoke the service
                ConstructorInfo constructor = this.servicesType[typeof(T)].GetConstructor(new Type[0]);
                Debug.Assert(constructor != null, "Cannot find a suitable constructor for " + typeof(T));

                T service = (T)constructor.Invoke(null);

                // add the service to the ones that we have already instantiated
                this.instantiatedServices.Add(typeof(T), service);

                return service;
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException("The requested service is not registered");
            }
        }
    }
}