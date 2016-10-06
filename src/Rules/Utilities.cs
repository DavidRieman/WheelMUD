//-----------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: athoma13
//   Date      : Fri Sep 30 2011
//   Purpose   : Rule Engine
// </summary>
// <history>
//   Sat Jan 28 2012 by Fastalanasa - Added to WheelMUD.Rules
// </history>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;
    using System.Linq.Expressions;

    internal static class Utilities
    {
        public static Expression<Func<T, T>> ReturnSelf<T>()
        {
            var t = Expression.Parameter(typeof(T), "t");
            return Expression.Lambda<Func<T, T>>(t, t);
        }

        public static int CombineHash(int hashcode, params int[] otherHashes)
        {
            for (int i = 0; i < otherHashes.Length; i++)
            {
                hashcode = (hashcode << 5) - hashcode ^ otherHashes[i];
            }

            return hashcode;
        }
    }
}
