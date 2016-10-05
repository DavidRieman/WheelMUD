//-----------------------------------------------------------------------------
// <copyright file="RulesHelper.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    public static class RulesHelper
    {
        private const BetweenRuleBoundsOption DefaultBoundOption = BetweenRuleBoundsOption.BothInclusive;

        public static M MustBeLessThan<M, T, R>(this IMustPassRule<M, T, R> mpr, R lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<R>(lessThan, false));
        }

        public static M MustBeLessThan<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<T, R>(mpr.Expression, lessThan, false));
        }

        public static M MustBeLessThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, R lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<R>(lessThan, true));
        }

        public static M MustBeLessThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<T, R>(mpr.Expression, lessThan, true));
        }

        public static M MustBeGreaterThan<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<R>(greaterThan, false));
        }

        public static M MustBeGreaterThan<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan)
        where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<T, R>(mpr.Expression, greaterThan, false));
        }

        public static M MustBeGreaterThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<R>(greaterThan, true));
        }

        public static M MustBeGreaterThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<T, R>(mpr.Expression, greaterThan, true));
        }

        public static M MustNotBeNull<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : class
        {
            return mpr.MustPassRule(new NotNullRule<R>());
        }

        public static M MustBeNull<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : class
        {
            return mpr.MustPassRule(new NullRule<R>());
        }

        public static M MustNotBeNullOrEmpty<M, T>(this IMustPassRule<M, T, string> mpr)
        {
            return mpr.MustPassRule(new NotNullOrEmpty());
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new BetweenRule<T, R>(mpr.Expression, greaterThan, lessThan, bounds));
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, R lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, R lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, R lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, R lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, params R[] values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values, comparer));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEqualityComparer<R> comparer, params R[] values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values, comparer));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, params R[] values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values, comparer));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEqualityComparer<R> comparer, params R[] values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values, comparer));
        }

        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value)
        {
            return mpr.MustPassRule(new EqualRule<R>(value));
        }

        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new EqualRule<R>(value, comparer));
        }

        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value)
        {
            return mpr.MustPassRule(new EqualRule<T, R>(mpr.Expression, value));
        }

        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new EqualRule<T, R>(mpr.Expression, value, comparer));
        }

        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value)
        {
            return mpr.MustPassRule(new NotEqualRule<R>(value));
        }

        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotEqualRule<R>(value, comparer));
        }

        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value)
        {
            return mpr.MustPassRule(new NotEqualRule<T, R>(mpr.Expression, value));
        }

        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotEqualRule<T, R>(mpr.Expression, value, comparer));
        }

        public static M MustMatchRegex<M, T>(this IMustPassRule<M, T, string> mpr, string pattern)
        {
            return mpr.MustPassRule(new RegexRule(new Regex(pattern, RegexOptions.Compiled)));
        }

        public static M MustMatchRegex<M, T>(this IMustPassRule<M, T, string> mpr, string pattern, RegexOptions options)
        {
            return mpr.MustPassRule(new RegexRule(new Regex(pattern, options)));
        }

        public static M MustBeOfType<M, T, R>(this IMustPassRule<M, T, R> mpr, Type type)
        {
            return mpr.MustPassRule(new OfTypeRule<R>(type));
        }

        public static M MustPassGenericRule<M, T, R>(this IMustPassRule<M, T, R> mpr, Func<R, bool> rule)
        {
            return mpr.MustPassRule(new GenericRule<R>(rule));
        }

        public static M CallValidate<M, T, R>(this IMustPassRule<M, T, R> mpr) where M : ISetupClass
        {
            return CallValidate(mpr, mpr.RulesRulesEngine);
        }

        public static M CallValidate<M, T, R>(this IMustPassRule<M, T, R> mpr, RulesEngine usingRulesEngine) where M : ISetupClass
        {
            usingRulesEngine.RegisterComposition(mpr.Expression, usingRulesEngine);
            return mpr.GetSelf();
        }

        public static M CallValidateForEachElement<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : IEnumerable
            where M : ISetupClass
        {
            return CallValidateForEachElement(mpr, mpr.RulesRulesEngine);
        }

        public static M CallValidateForEachElement<M, T, R>(this IMustPassRule<M, T, R> mpr, RulesEngine usingRulesEngine)
            where R : IEnumerable
            where M : ISetupClass
        {
            mpr.RulesRulesEngine.RegisterEnumerableComposition(mpr.Expression, usingRulesEngine);
            return mpr.GetSelf();
        }
    }
}