//-----------------------------------------------------------------------------
// <copyright file="DefaultErrorResolver.cs" company="http://rulesengine.codeplex.com">
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

    public class DefaultErrorResolver : IErrorResolver
    {
        private Dictionary<string, string> formats = new Dictionary<string, string>();

        public DefaultErrorResolver()
        {
            this.formats["LessThanRule"] = "Must be less than {0}.";
            this.formats["LessThanOrEqualToRule"] = "Must be less than or equal to {0}.";
            this.formats["BetweenRule"] = "Must be between {0} and {1}.";
            this.formats["EqualRule"] = "Must equal {0}.";
            this.formats["GenericRule"] = "Generic rule failed.";
            this.formats["GreaterThanRule"] = "Must be greater than {0}.";
            this.formats["GreaterThanOrEqualToRule"] = "Must be greater than or equal to {0}.";
            this.formats["NoLeadingWhitespaceRule"] = "Must not start with whitespace";
            this.formats["NotEqualRule"] = "Must not equal {0}.";
            this.formats["NotNullRule"] = "Must not be null.";
            this.formats["NotOneOfRule"] = "Listed as invalid.";
            this.formats["NullRule"] = "Must be null.";
            this.formats["OfTypeRule"] = "Must be of type '{0}'.";
            this.formats["OneOfRule"] = "Must be listed as valid.";
            this.formats["RegexRule"] = "Failed regex validation '{0}'.";
            this.formats["NotNullOrEmpty"] = "Must not be null or empty.";
        }

        public string GetErrorMessage(ValidationError validationError)
        {
            string format;

            if (formats.TryGetValue(validationError.Rule.RuleKind, out format))
            {
                return string.Format(format, validationError.ValidationArguments);
            }

            return string.Format("Rule {0} failed.", validationError.Rule);
        }

        public void SetFormat(string ruleKind, string format)
        {
            formats[ruleKind] = format;
        }
    }
}
