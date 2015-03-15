//-----------------------------------------------------------------------------
// <copyright file="RulesEngine.cs" company="http://rulesengine.codeplex.com">
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

    public class RulesEngine : IRegisterInvoker
    {
        //Create a default ExpressionCache, ignoring lambda parameter names.
        private static readonly ExpressionCache _defaultExpressionCache = new ExpressionCache(new ExpressionComparerIgnoreLambdaParameterNames());
        private readonly InvokerRegistry _registry = new InvokerRegistry();
        private readonly ExpressionCache _expressionCache;

        public RulesEngine()
        {
            _expressionCache = _defaultExpressionCache;
        }

        public RulesEngine(ExpressionCache expressionCache)
        {
            if (expressionCache == null) throw new ArgumentNullException("expressionCache");
            _expressionCache = expressionCache;
        }

        /// <summary>
        /// Creates a Rules Engine.
        /// </summary>
        /// <param name="basedOn">Copies rules from base Engine</param>
        public RulesEngine(RulesEngine basedOn)
        {
            if (basedOn == null) throw new ArgumentNullException("basedOn");
            _registry = basedOn._registry.Clone();
            _expressionCache = basedOn._expressionCache;
        }

        /// <summary>
        /// Creates a Rules Engine.
        /// </summary>
        /// <param name="basedOn">Copies specific rules from base Engine</param>
        /// <param name="types">Copies rules for the specified types only.</param>
        public RulesEngine(RulesEngine basedOn, params Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            if (basedOn == null) throw new ArgumentNullException("basedOn");

            var registry = basedOn._registry.Clone();

            _expressionCache = basedOn._expressionCache;

            foreach (var type in types)
            {
                var invokers = registry.GetInvokers(type);
                foreach (var invoker in invokers)
                {
                    _registry.RegisterInvoker(invoker);
                }
            }
        }

        public ForClass<T> For<T>()
        {
            return new ForClass<T>(this);
        }

        public void Validate(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (value == null) return;
            foreach (var invoker in _registry.GetInvokers(value.GetType()))
            {
                invoker.Invoke(value, report, depth);
                if (report.HasErrors && depth == ValidationReportDepth.ShortCircuit)
                {
                    return;
                }
            }
        }

        public bool Validate(object value)
        {
            IValidationReport validationReport = new ValidationReport();
            Validate(value, validationReport, ValidationReportDepth.ShortCircuit);
            return !validationReport.HasErrors;
        }

        void IRegisterInvoker.RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            _registry.RegisterInvoker(ruleInvoker);
        }

        RulesEngine IRegisterInvoker.RulesRulesEngine
        {
            get { return this; }
        }

        /// <summary>
        /// Gets ExpressionCache
        /// </summary>
        public ExpressionCache ExpressionCache
        {
            get { return _expressionCache; }
        }

        /// <summary>
        /// Gets DefaultExpressionCache
        /// </summary>
        public static ExpressionCache DefaultExpressionCache
        {
            get { return _defaultExpressionCache; }
        }
    }
}
