//-----------------------------------------------------------------------------
// <copyright file="BetweenRule.cs" company="http://rulesengine.codeplex.com">
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

    public class BetweenRule<T, R> : IRule<T> where R : IComparable<R>
    {
        private Func<T, R> _greaterThan;
        private Func<T, R> _lessThan;
        private Func<T, R> _value;
        private int _compareToResultLower;
        private int _compareToResultUpper;
        private BetweenRuleBoundsOption _options;

        public BetweenRule(Expression<Func<T, R>> value, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption options)
        {
            _value = value.Compile();
            _greaterThan = greaterThan.Compile();
            _lessThan = lessThan.Compile();
            _options = options;
            Initialize();
        }

        private void Initialize()
        {
            if (_options == BetweenRuleBoundsOption.BothInclusive)
            {
                _compareToResultLower = 1;
                _compareToResultUpper = -1;
            }
            else if (_options == BetweenRuleBoundsOption.LowerInclusiveUpperExclusive)
            {
                _compareToResultLower = 1;
                _compareToResultUpper = 0;
            }
            else if (_options == BetweenRuleBoundsOption.LowerExclusiveUpperInclusive)
            {
                _compareToResultLower = 0;
                _compareToResultUpper = -1;
            }
            else
            {
                _compareToResultLower = 0;
                _compareToResultUpper = 0;
            }
        }

        public ValidationResult Validate(T value)
        {
            R v = _value(value);
            IComparable<R> lowerBound = _greaterThan(value);
            IComparable<R> upperBound = _lessThan(value);

            if (lowerBound.CompareTo(v) < _compareToResultLower && upperBound.CompareTo(v) > _compareToResultUpper)
                return ValidationResult.Success;

            return ValidationResult.Fail(lowerBound, upperBound, v, _options);
        }

        public string RuleKind
        {
            get
            {
                return "BetweenRule";
            }
        }

    }
}
