//-----------------------------------------------------------------------------
// <copyright file="LessThanRule.cs" company="http://rulesengine.codeplex.com">
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

    public class LessThanRule<R> : IRule<R> where R : IComparable<R>
    {
        private R lessThan;
        private int compareToResult;

        public LessThanRule(R lessThan, bool inclusive)
        {
            this.lessThan = lessThan;
            this.compareToResult = inclusive ? -1 : 0;
        }

        public string RuleKind
        {
            get
            {
                return compareToResult == 0 ? "LessThanRule" : "LessThanOrEqualToRule";
            }
        }

        public ValidationResult Validate(R value)
        {
            if (lessThan.CompareTo(value) > compareToResult)
            {
                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Fail(lessThan);
            }
        }
    }

    public class LessThanRule<T, R> : IRule<T> where R : IComparable<R>
    {
        private Func<T, R> value1;
        private Func<T, R> value2;
        private int compareToResult;

        public LessThanRule(Expression<Func<T, R>> value1, Expression<Func<T, R>> value2, bool inclusive)
        {
            this.value1 = value1.Compile();
            this.value2 = value2.Compile();
            compareToResult = inclusive ? -1 : 0;
        }

        public string RuleKind
        {
            get
            {
                return compareToResult == 0 ? "LessThanRule" : "LessThanOrEqualToRule";
            }
        }

        public ValidationResult Validate(T value)
        {
            IComparable<R> v1 = this.value1(value);
            R v2 = this.value2(value);

            if (v1.CompareTo(v2) > compareToResult)
            {
                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Fail(v2);
            }
        }
    }
}