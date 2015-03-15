//-----------------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="http://rulesengine.codeplex.com">
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
    public struct ValidationResult
    {
        private bool _isValid;
        private object[] _arguments;

        /// <summary>
        /// Gets IsValid
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
        }

        /// <summary>
        /// Gets Arguments
        /// </summary>
        public object[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Successful result for a validation.
        /// </summary>
        public static ValidationResult Success
        {
            get { return _valid; }
        }

        public ValidationResult(bool isValid, object[] arguments)
        {
            _isValid = isValid;
            _arguments = arguments;
        }

        private static readonly ValidationResult _valid = new ValidationResult(true, null);

        public static ValidationResult Fail(params object[] arguments)
        {
            return new ValidationResult(false, arguments);
        }

        public static ValidationResult Fail()
        {
            return new ValidationResult(false, new object[0]);
        }
    }
}
