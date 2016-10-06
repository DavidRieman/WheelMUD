//-----------------------------------------------------------------------------
// <copyright file="RegisterHelper.cs" company="http://rulesengine.codeplex.com">
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
    using System.Linq.Expressions;

    internal static class RegisterHelper
    {
        public static void RegisterRule<T, R>(this IRegisterInvoker register, IRule<R> rule, Expression<Func<T, R>> expressionToInvoke, Expression expressionToBlame)
        {
            var ruleInvoker = new RuleInvoker<T, R>(rule, expressionToInvoke, register.RulesRulesEngine.ExpressionCache.Get(expressionToBlame));
            register.RegisterInvoker(ruleInvoker);
        }

        public static void RegisterComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> compositionExpression)
        {
            RegisterComposition(register, compositionExpression, register.RulesRulesEngine);
        }

        public static void RegisterComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> compositionExpression, RulesEngine usingRulesEngine)
        {
            var compositionInvoker = new CompositionInvoker<T, R>(usingRulesEngine, compositionExpression);
            register.RegisterInvoker(compositionInvoker);
        }

        public static void RegisterEnumerableComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> enumerableCompositionExpression)
                where R : IEnumerable
        {
            RegisterEnumerableComposition(register, enumerableCompositionExpression, register.RulesRulesEngine);
        }

        public static void RegisterEnumerableComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> enumerableCompositionExpression, RulesEngine usingRulesEngine)
            where R : IEnumerable
        {
            var enumerableCompositionInvoker = new EnumerableCompositionInvoker<T, R>(usingRulesEngine, enumerableCompositionExpression);
            register.RegisterInvoker(enumerableCompositionInvoker);
        }

        public static ConditionalInvoker<T> RegisterCondition<T>(this IRegisterInvoker register, Expression<Func<T, bool>> conditionalExpression)
        {
            var conditionalInvoker = new ConditionalInvoker<T>(conditionalExpression, register.RulesRulesEngine);
            register.RegisterInvoker(conditionalInvoker);
            return conditionalInvoker;
        }

        public static void RegisterInvoker(this IRegisterInvoker register, IRuleInvoker ruleInvoker)
        {
            register.RegisterInvoker(ruleInvoker);
        }
    }
}