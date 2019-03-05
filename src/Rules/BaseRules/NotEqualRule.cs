﻿//-----------------------------------------------------------------------------
// <copyright file="NotEqualRule.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class NotEqualRule<R> : IRule<R>
    {
        private R value;
        private IEqualityComparer<R> comparer;

        public NotEqualRule(R value) : this(value, EqualityComparer<R>.Default)
        {
        }

        public NotEqualRule(R value, IEqualityComparer<R> comparer)
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
            get { return "NotEqualRule"; }
        }

        public ValidationResult Validate(R value)
        {
            if (!comparer.Equals(value, this.value))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(this.value);
        }
    }

    public class NotEqualRule<T, R> : IRule<T>
    {
        // CAUTION: rules of the same ruleKind must return the same number of arguments.
        private IEqualityComparer<R> comparer;
        private Func<T, R> value;
        private Func<T, R> value2;

        public NotEqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2)
            : this(value, value2, EqualityComparer<R>.Default)
        {
        }

        public NotEqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2, IEqualityComparer<R> comparer)
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
            get { return "NotEqualRule"; }
        }

        public ValidationResult Validate(T value)
        {
            R v1 = this.value(value);
            R v2 = this.value2(value);

            if (!comparer.Equals(v1, v2))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(v2);
        }
    }
}