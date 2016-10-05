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
        private Func<T, R> greaterThan;
        private Func<T, R> lessThan;
        private Func<T, R> value;
        private int compareToResultLower;
        private int compareToResultUpper;
        private BetweenRuleBoundsOption options;

        public BetweenRule(Expression<Func<T, R>> value, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption options)
        {
            this.value = value.Compile();
            this.greaterThan = greaterThan.Compile();
            this.lessThan = lessThan.Compile();
            this.options = options;
            Initialize();
        }

        public string RuleKind
        {
            get { return "BetweenRule"; }
        }

        public ValidationResult Validate(T value)
        {
            R v = this.value(value);
            IComparable<R> lowerBound = greaterThan(value);
            IComparable<R> upperBound = lessThan(value);

            if (lowerBound.CompareTo(v) < compareToResultLower && upperBound.CompareTo(v) > compareToResultUpper)
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(lowerBound, upperBound, v, options);
        }

        private void Initialize()
        {
            if (options == BetweenRuleBoundsOption.BothInclusive)
            {
                compareToResultLower = 1;
                compareToResultUpper = -1;
            }
            else if (options == BetweenRuleBoundsOption.LowerInclusiveUpperExclusive)
            {
                compareToResultLower = 1;
                compareToResultUpper = 0;
            }
            else if (options == BetweenRuleBoundsOption.LowerExclusiveUpperInclusive)
            {
                compareToResultLower = 0;
                compareToResultUpper = -1;
            }
            else
            {
                compareToResultLower = 0;
                compareToResultUpper = 0;
            }
        }
    }
}