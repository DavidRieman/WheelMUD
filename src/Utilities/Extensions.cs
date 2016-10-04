//-----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Provides extension methods for the whole MUD framework.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>Provides extension methods for the whole MUD framework.</summary>
    public static class Extensions
    {
        /// <summary>Extension method that converts anything enumerable to a dictionary.</summary>
        /// <typeparam name="TKey">The type of dictionary key.</typeparam>
        /// <typeparam name="TValue">The type of dictionary value, and the type of the enumerable.</typeparam>
        /// <param name="values">The enumerable values.</param>
        /// <param name="indexer">The lambda for creating a key from the value.</param>
        /// <returns>The created dictionary.</returns>
        public static Dictionary<TKey, TValue> CreateMapping<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> indexer)
        {
            var mappings = new Dictionary<TKey, TValue>();

            foreach (TValue value in values)
            {
                mappings[indexer(value)] = value;
            }

            return mappings;
        }

        /// <summary>Gets the value from the dictionary or uses the supplied lambda to create an exception message.</summary>
        /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the value in the dictionary.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The actual key.</param>
        /// <param name="messageBuilder">The lambda to create the exception message.</param>
        /// <returns>The value, if it can correctly be accessed the key.</returns>
        public static TValue GetValueOrFailWith<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, string> messageBuilder)
        {
            TValue ret;

            if (!dict.TryGetValue(key, out ret))
            {
                throw new KeyNotFoundException(messageBuilder(key));
            }

            return ret;
        }
    }
}
