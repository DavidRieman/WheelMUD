//-----------------------------------------------------------------------------
// <copyright file="InvokerRegistry.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections.Generic;
    using System.Linq;

    public class InvokerRegistry : ICloneable
    {
        // Use a list rather than a Dictionary here because the order in which the invokers are added is relevant.
        private List<KeyValuePair<Type, List<IRuleInvoker>>> invokers = new List<KeyValuePair<Type, List<IRuleInvoker>>>();

        // Store all invokers relevant for a type. E.g. For a 'Person' Type, you will get invokers that apply to parent class/interfaces of Person.
        private Dictionary<Type, IRuleInvoker[]> normalizedInvokers = new Dictionary<Type, IRuleInvoker[]>();

        public IRuleInvoker[] GetInvokers(Type type)
        {
            IRuleInvoker[] result;

            if (!this.normalizedInvokers.TryGetValue(type, out result))
            {
                result = invokers.Where(kp => IsTypeCompatible(type, kp.Key))
                                        .Select(kp => kp.Value)
                                        .SelectMany(m => m)
                                        .ToArray();

                normalizedInvokers[type] = result;
            }

            return result;
        }

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            // Recalculate normalized invokers every time a new invoker is added.
            normalizedInvokers.Clear();

            if (invokers.Any(ri => ri.Key == ruleInvoker.ParameterType))
            {
                var invokers = this.invokers.First((KeyValuePair<Type, List<IRuleInvoker>> ri) => ri.Key == ruleInvoker.ParameterType).Value;
                invokers.Add(ruleInvoker);
            }
            else
            {
                var invokers = new List<IRuleInvoker>();
                invokers.Add(ruleInvoker);
                this.invokers.Add(new KeyValuePair<Type, List<IRuleInvoker>>(ruleInvoker.ParameterType, invokers));
            }
        }

        public InvokerRegistry Clone()
        {
            var result = new InvokerRegistry();
            result.invokers = new List<KeyValuePair<Type, List<IRuleInvoker>>>(invokers);
            return result;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        private bool IsTypeCompatible(Type value, Type invokerType)
        {
            return invokerType.IsAssignableFrom(value);
        }
    }
}