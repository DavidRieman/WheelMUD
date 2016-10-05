//-----------------------------------------------------------------------------
// <copyright file="EqualRule.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class EqualRule<R> : IRule<R>
    {
        private R value;
        private IEqualityComparer<R> comparer;

        public EqualRule(R value) : this(value, EqualityComparer<R>.Default)
        {
        }

        public EqualRule(R value, IEqualityComparer<R> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.comparer = comparer;
            this.value = value;
        }

        public string RuleKind
        {
            get { return "EqualRule"; }
        }

        public ValidationResult Validate(R value)
        {
            return this.comparer.Equals(value, this.value) ? ValidationResult.Success : ValidationResult.Fail(this.value);
        }
    }

    public class EqualRule<T, R> : IRule<T>
    {
        private IEqualityComparer<R> comparer;
        private Func<T, R> value;
        private Func<T, R> value2;

        public EqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2) : this(value, value2, EqualityComparer<R>.Default)
        {
        }

        public EqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2, IEqualityComparer<R> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            if (value2 == null)
            {
                throw new ArgumentNullException("value2");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.value = value.Compile();
            this.value2 = value2.Compile();
            this.comparer = comparer;
        }

        public string RuleKind
        {
            get { return "EqualRule"; }
        }

        public ValidationResult Validate(T value)
        {
            R v1 = this.value(value);
            R v2 = this.value2(value);
            return this.comparer.Equals(v1, v2) ? ValidationResult.Success : ValidationResult.Fail(v2);
        }
    }
}