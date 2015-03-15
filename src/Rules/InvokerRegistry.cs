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
        //Use a list rather than a Dictionary here because the order in which the invokers are added is relevant.
        List<KeyValuePair<Type, List<IRuleInvoker>>> _invokers = new List<KeyValuePair<Type, List<IRuleInvoker>>>();

        //Store all invokers relevant for a type. E.g. For a 'Person' Type, you will get invokers that apply to parent class/interfaces of Person.
        Dictionary<Type, IRuleInvoker[]> _normalizedInvokers = new Dictionary<Type, IRuleInvoker[]>();

        public IRuleInvoker[] GetInvokers(Type type)
        {
            IRuleInvoker[] result;

            if (!_normalizedInvokers.TryGetValue(type, out result))
            {
                result = _invokers.Where(kp => IsTypeCompatible(type, kp.Key))
                                        .Select(kp => kp.Value)
                                        .SelectMany(m => m)
                                        .ToArray();

                _normalizedInvokers[type] = result;

            }

            return result;
        }

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            //Re-Calculate normalized invokers every time a new invoker is added.
            _normalizedInvokers.Clear();

            if (_invokers.Any(ri => ri.Key == ruleInvoker.ParameterType))
            {
                var invokers = _invokers.First(ri => ri.Key == ruleInvoker.ParameterType).Value;
                invokers.Add(ruleInvoker);
            }
            else
            {
                var invokers = new List<IRuleInvoker>();
                invokers.Add(ruleInvoker);
                _invokers.Add(new KeyValuePair<Type, List<IRuleInvoker>>(ruleInvoker.ParameterType, invokers));
            }
        }

        private bool IsTypeCompatible(Type value, Type invokerType)
        {
            return invokerType.IsAssignableFrom(value);
        }

        public InvokerRegistry Clone()
        {
            var result = new InvokerRegistry();
            result._invokers = new List<KeyValuePair<Type, List<IRuleInvoker>>>(_invokers);
            return result;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
