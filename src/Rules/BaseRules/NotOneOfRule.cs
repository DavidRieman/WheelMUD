//-----------------------------------------------------------------------------
// <copyright file="NotOneOfRule.cs" company="http://rulesengine.codeplex.com">
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
    using System.Linq;

    public class NotOneOfRule<R> : IRule<R>
    {
        private IEnumerable<R> values;
        private IEqualityComparer<R> comparer;

        public NotOneOfRule(IEnumerable<R> values) : this(values, EqualityComparer<R>.Default)
        {
        }

        public NotOneOfRule(IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.values = values;
            this.comparer = comparer;
        }

        public string RuleKind
        {
            get { return "NotOneOfRule"; }
        }

        public ValidationResult Validate(R value)
        {
            if (!values.Contains(value, comparer))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(new object[] { values });
        }
    }
}