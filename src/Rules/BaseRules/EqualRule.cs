//-----------------------------------------------------------------------------
// <copyright file="CachedExpression.cs" company="http://rulesengine.codeplex.com">
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

    public class EqualRule<R>: IRule<R>
    {
        R _value;
        IEqualityComparer<R> _comparer;

        public EqualRule(R value) : this(value, EqualityComparer<R>.Default)
        {
        }

        public EqualRule(R value, IEqualityComparer<R> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
            _value = value;
        }

        public ValidationResult Validate(R value)
        {
            if (_comparer.Equals(value, _value))
                return ValidationResult.Success;

            return ValidationResult.Fail(_value);
        }

        public string RuleKind
        {
            get { return "EqualRule"; }
        }
    }

    public class EqualRule<T, R> : IRule<T>
    {
        IEqualityComparer<R> _comparer;
        Func<T, R> _value;
        Func<T, R> _value2;

        public EqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2) : this(value, value2, EqualityComparer<R>.Default)
        {
        }

        public EqualRule(Expression<Func<T, R>> value, Expression<Func<T, R>> value2, IEqualityComparer<R> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (value2 == null) throw new ArgumentNullException("value2");
            if (value == null) throw new ArgumentNullException("value");
            
            _value = value.Compile();
            _value2 = value2.Compile();
            _comparer = comparer;
        }

        public ValidationResult Validate(T value)
        {
            R v1 = _value(value);
            R v2 = _value2(value);
            
            if (_comparer.Equals(v1, v2))
                return ValidationResult.Success;

            return ValidationResult.Fail(v2);
        }

        public string RuleKind
        {
            get { return "EqualRule"; }
        }
    }
}
