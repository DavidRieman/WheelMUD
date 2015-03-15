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
    using System.Linq.Expressions;

    public class LessThanRule<R> : IRule<R> where R : IComparable<R>
    {
        private R _lessThan;
        private int _compareToResult;

        public LessThanRule(R lessThan, bool inclusive)
        {
            _lessThan = lessThan;
            if (inclusive)
            {
                _compareToResult = -1;
            }
            else
            {
                _compareToResult = 0;
            }
        }

        public ValidationResult Validate(R value)
        {
            if (_lessThan.CompareTo(value) > _compareToResult)
                return ValidationResult.Success;
            else
                return ValidationResult.Fail(_lessThan);
        }

        public string RuleKind
        {
            get
            {
                if (_compareToResult == 0)
                {
                    return "LessThanRule";
                }
                else
                {
                    return "LessThanOrEqualToRule";
                }
            }
        }

    }

    public class LessThanRule<T, R> : IRule<T> where R : IComparable<R>
    {
        private Func<T, R> _value1;
        private Func<T, R> _value2;
        object[] _arguments = new object[2];
        private int _compareToResult;

        public LessThanRule(Expression<Func<T, R>> value1, Expression<Func<T, R>> value2, bool inclusive)
        {
            _value1 = value1.Compile();
            _value2 = value2.Compile();
            if (inclusive)
            {
                _compareToResult = -1;
            }
            else
            {
                _compareToResult = 0;
            }
        }

        public ValidationResult Validate(T value)
        {
            IComparable<R> v1 = _value1(value);
            R v2 = _value2(value);

            if (v1.CompareTo(v2) > _compareToResult)
                return ValidationResult.Success;
            else
                return ValidationResult.Fail(v2);
        }

        public string RuleKind
        {
            get 
            {
                if (_compareToResult == 0)
                {
                    return "LessThanRule";
                }
                else
                {
                    return "LessThanOrEqualToRule";
                }
            }
        }
    }

}
