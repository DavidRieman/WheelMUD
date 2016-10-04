//-----------------------------------------------------------------------------
// <copyright file="NotEqualRule.cs" company="http://rulesengine.codeplex.com">
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

    public class NotEqualRule<R> : IRule<R>
    {
        private R _value;
        private IEqualityComparer<R> _comparer;

        public NotEqualRule(R value) : this(value, EqualityComparer<R>.Default)
        {
        }

        public NotEqualRule(R value, IEqualityComparer<R> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            _comparer = comparer;
            _value = value;
        }

        public string RuleKind
        {
            get { return "NotEqualRule"; }
        }

        public ValidationResult Validate(R value)
        {
            if (!_comparer.Equals(value, _value))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(_value);
        }
    }

    public class NotEqualRule<T, R> : IRule<T>
    {
        // CAUTION: rules of the same ruleKind must return the same number of arguments.
        private IEqualityComparer<R> _comparer;
        private Func<T, R> _value;
        private Func<T, R> _value2;

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

            _value = value.Compile();
            _value2 = value2.Compile();
            _comparer = comparer;
        }

        public string RuleKind
        {
            get { return "NotEqualRule"; }
        }

        public ValidationResult Validate(T value)
        {
            R v1 = _value(value);
            R v2 = _value2(value);

            if (!_comparer.Equals(v1, v2))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(v2);
        }
    }
}