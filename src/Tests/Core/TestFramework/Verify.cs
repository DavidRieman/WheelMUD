//-----------------------------------------------------------------------------
// <copyright file="Verify.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: August 2011 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Wrapper for the Assert methods of whatever test case framework is running the current unit tests.
    /// </summary>
    public static class Verify
    {
        /// <summary>Framework-specific actions for verifying comparisons of two objects.</summary>
        private static Action<object, object, string, object[]> areSame, areNotSame;

        /// <summary>Framework-specific actions for verifying a boolean value.</summary>
        private static Action<bool, string, object[]> isTrue;

        private static Action<object, string> isNull;

        private static Action<object, string> notNull;

        private static Action<object, object> areEqual;

        private static Action<object, object> areNotEqual;

        /// <summary>Initializes static members of the Verify class.</summary>
        static Verify()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            if (processName.StartsWith("nunit", StringComparison.CurrentCultureIgnoreCase) ||
                processName.StartsWith("JetBrains.ReSharper.TaskRunner", StringComparison.CurrentCultureIgnoreCase) ||
                processName.StartsWith("vstest.executionengine", StringComparison.CurrentCultureIgnoreCase))
            {
                // The test is running in NUnit or ReSharper; rig up to the NUnit assert methods.
                WireNUnitAsserts();
            }
            else if (processName.StartsWith("QTAgent", StringComparison.CurrentCultureIgnoreCase))
            {
                // The test is running in the MS unit testing framework; rig up those assert methods.
                WireMSTestAsserts();
            }
            else
            {
                string message = string.Format("{0}: Could not recognize the hosting test framework assembly!", processName);
                throw new InvalidProgramException(message);
            }
        }

        /// <summary>
        /// Verify that two objects are the same.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="message">The message to provide upon failure.</param>
        /// <param name="parameters">Additional message parameters.</param>
        public static void AreSame(object expected, object actual, string message = null, params object[] parameters)
        {
            areSame(expected, actual, message, parameters);
        }

        /// <summary>
        /// Verify that two objects are not the same.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="message">The message to provide upon failure.</param>
        /// <param name="parameters">Additional message parameters.</param>
        public static void AreNotSame(object expected, object actual, string message = null, params object[] parameters)
        {
            areNotSame(expected, actual, message, parameters);
        }

        /// <summary>
        /// Verify that a condition is true.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The message to provide upon failure.</param>
        /// <param name="parameters">Additional message parameters.</param>
        public static void IsTrue(bool condition, string message = null, params object[] parameters)
        {
            isTrue(condition, message, parameters);
        }

        /// <summary>
        /// Determines whether the specified test object is null.
        /// </summary>
        /// <param name="testObject">The test object.</param>
        public static void IsNull(object testObject, string errorMessage = null)
        {
            isNull(testObject, errorMessage);
        }

        /// <summary>
        /// Determines whether [is not null] [the specified test object].
        /// </summary>
        /// <param name="testObject">The test object.</param>
        public static void IsNotNull(object testObject, string errorMessage = null)
        {
            notNull(testObject, errorMessage);
        }

        /// <summary>
        /// Determines whether the two value type objects are equal.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        public static void AreEqual(object first, object second)
        {
            areEqual(first, second);
        }

        /// <summary>
        /// Determines whether the two value type objects are not equal.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        public static void AreNotEqual(object first, object second)
        {
            areNotEqual(first, second);
        }

        /// <summary>
        /// Wire up our verification methods against the NUnit assert methods.
        /// </summary>
        private static void WireNUnitAsserts()
        {
            areSame = NUnit.Framework.Assert.AreSame;
            areNotSame = NUnit.Framework.Assert.AreNotSame;
            isTrue = NUnit.Framework.Assert.IsTrue;
            notNull = NUnit.Framework.Assert.NotNull;
            isNull = NUnit.Framework.Assert.IsNull;
            areEqual = NUnit.Framework.Assert.AreEqual;
            areNotEqual = NUnit.Framework.Assert.AreNotEqual;
        }

        /// <summary>
        /// Wire up our verification methods against the MSTest assert methods.
        /// </summary>
        private static void WireMSTestAsserts()
        {
            areSame = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame;
            areNotSame = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotSame;
            isTrue = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue;
            notNull = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull;
            isNull = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull;
            areEqual = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual;
            areNotEqual = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual;
        }
    }
}