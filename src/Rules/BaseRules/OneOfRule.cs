//-----------------------------------------------------------------------------
// <copyright file="OneOfRule.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections.Generic;
    using System.Linq;

    public class OneOfRule<R> : IRule<R>
    {
        IEnumerable<R> _values;
        IEqualityComparer<R> _comparer;

        public OneOfRule(IEnumerable<R> values)
            : this (values, EqualityComparer<R>.Default)
        {
        }

        public OneOfRule(IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            if (comparer == null) throw new System.ArgumentNullException("comparer");
            if (values == null) throw new System.ArgumentNullException("values");
            _values = values;
            _comparer = comparer;
        }

        public ValidationResult Validate(R value)
        {
            if (_values.Contains(value, _comparer))
                return ValidationResult.Success;
            else
                return ValidationResult.Fail(new object[] { _values });
        }

        public string RuleKind
        {
            get { return "OneOfRule"; }
        }
    }
}
