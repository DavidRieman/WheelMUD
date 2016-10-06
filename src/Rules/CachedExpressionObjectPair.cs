//-----------------------------------------------------------------------------
// <copyright file="CachedExpressionObjectPair.cs" company="http://rulesengine.codeplex.com">
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

    /// <summary>A cached rule expression that consists of a pair of objects.</summary>
    public sealed class CachedExpressionObjectPair : IEquatable<CachedExpressionObjectPair>
    {
        private readonly CachedExpression privateCachedExpression;
        private readonly object privateValue;
        private readonly int hashCode;

        /// <summary>Initializes a new instance of the <see cref="CachedExpressionObjectPair"/> class.</summary>
        /// <param name="cachedExpression">The cached expression.</param>
        /// <param name="value">The value.</param>
        public CachedExpressionObjectPair(CachedExpression cachedExpression, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (cachedExpression == null)
            {
                throw new ArgumentNullException("cachedExpression");
            }

            this.privateCachedExpression = cachedExpression;
            this.privateValue = value;
            hashCode = Utilities.CombineHash(cachedExpression.GetHashCode(), value.GetHashCode());
        }

        /// <summary>Gets the cached expression.</summary>
        public CachedExpression CachedExpression
        {
            get { return this.privateCachedExpression; }
        }

        /// <summary>Gets the value.</summary>
        public object Value
        {
            get { return this.privateValue; }
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(CachedExpressionObjectPair t1, CachedExpressionObjectPair t2)
        {
            return t1.Equals(t2);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(CachedExpressionObjectPair t1, CachedExpressionObjectPair t2)
        {
            return !t1.Equals(t2);
        }

        /// <summary>Determines whether the specified <see cref="System.Object"/> is equal to this instance.</summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as CachedExpressionObjectPair);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public bool Equals(CachedExpressionObjectPair other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(other.Value, this.Value) && ReferenceEquals(other.CachedExpression, this.CachedExpression))
            {
                return true;
            }

            return false;
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
