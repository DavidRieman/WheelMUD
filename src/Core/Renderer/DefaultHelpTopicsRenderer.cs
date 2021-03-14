//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicsRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;
using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.HelpTopics(0)]
    public class DefaultHelpTopicsRenderer : RendererDefinitions.HelpTopics
    {
        public override OutputBuilder Render()
        {
            var topics = HelpManager.Instance.GetHelpTopics().Where(topic => topic.Aliases.Count > 0);

            var output = new OutputBuilder();
            if (!topics.Any())
            {
                output.AppendLine("There are no help topics written yet.");
                return output;
            }

            // Format the first column to the number of characters to fit the longest topic, then have the description to the right of that.
            var lineFormat = "{0," + -1 * HelpManager.Instance.MaxPrimaryAliasLength + "} {1}";

            output.AppendLine("Available help topics:");
            output.AppendSeparator(color: "yellow");
            foreach (var topic in topics)
            {
                var primaryAlias = topic.Aliases.FirstOrDefault();
                output.AppendLine(string.Format(lineFormat, primaryAlias, "TODO: Topic Short Description Support Here"));
            }
            output.AppendSeparator(color: "yellow");

            output.AppendLine();
            output.AppendLine("<%yellow%>NOTE:<%n%> You can also use the \"<%green%>commands<%n%>\" command to list out commands, and you can get help for a specific command with \"<%green%>help <command name><%n%>\".");
            return output;
        }
    }
}
