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
        private readonly InvokerRegistry registry = new InvokerRegistry();

        static RulesEngine()
        {
            // Create a default ExpressionCache, ignoring lambda parameter names.
            DefaultExpressionCache = new ExpressionCache(new ExpressionComparerIgnoreLambdaParameterNames());
        }

        public RulesEngine()
        {
            this.ExpressionCache = DefaultExpressionCache;
        }

        /// <summary>Initializes a new instance of the <see cref="RulesEngine"/> class.</summary>
        /// <param name="expressionCache">The expression cache.</param>
        public RulesEngine(ExpressionCache expressionCache)
        {
            if (expressionCache == null)
            {
                throw new ArgumentNullException("expressionCache");
            }

            this.ExpressionCache = expressionCache;
        }

        /// <summary>Initializes a new instance of the <see cref="RulesEngine"/> class.</summary>
        /// <param name="basedOn">Copies rules from base Engine</param>
        public RulesEngine(RulesEngine basedOn)
        {
            if (basedOn == null)
            {
                throw new ArgumentNullException("basedOn");
            }

            registry = basedOn.registry.Clone();
            this.ExpressionCache = basedOn.ExpressionCache;
        }

        /// <summary>Initializes a new instance of the <see cref="RulesEngine"/> class.</summary>
        /// <param name="basedOn">Copies specific rules from base Engine</param>
        /// <param name="types">Copies rules for the specified types only.</param>
        public RulesEngine(RulesEngine basedOn, params Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }

            if (basedOn == null)
            {
                throw new ArgumentNullException("basedOn");
            }
            
            var registry = basedOn.registry.Clone();
            this.ExpressionCache = basedOn.ExpressionCache;
            foreach (var type in types)
            {
                var invokers = registry.GetInvokers(type);
                foreach (var invoker in invokers)
                {
                    this.registry.RegisterInvoker(invoker);
                }
            }
        }

        /// <summary>Gets DefaultExpressionCache.</summary>
        public static ExpressionCache DefaultExpressionCache { get; private set; }

        RulesEngine IRegisterInvoker.RulesRulesEngine
        {
            get { return this; }
        }

        /// <summary>Gets ExpressionCache.</summary>
        public ExpressionCache ExpressionCache { get; private set; }

        public ForClass<T> For<T>()
        {
            return new ForClass<T>(this);
        }

        public void Validate(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (value == null)
            {
                return;
            }

            foreach (var invoker in registry.GetInvokers(value.GetType()))
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
            registry.RegisterInvoker(ruleInvoker);
        }
    }
}