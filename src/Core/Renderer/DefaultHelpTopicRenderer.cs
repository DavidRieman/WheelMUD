//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Core.Renderer
{
    [RendererExports.HelpTopic(0)]
    public class DefaultHelpTopicRenderer : RendererDefinitions.HelpTopic
    {
        private const string HeaderLine = "<%b%><%yellow%>~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<%n%>" + AnsiSequences.NewLine;

        public override string Render(TerminalOptions terminalOptions, HelpTopic helpTopic)
        {
            if (terminalOptions.UseMXP)
            {
                // TODO: What was this !element doing? Does it still work? Test with zMUD or something and re-read MXP specs?
                var sb = new StringBuilder("<%mxpsecureline%><!element see '<send href=\"help &cref;\">' att='cref' open>");
                sb.Append($"{HeaderLine}HELP TOPIC: {helpTopic.Aliases.First()}{AnsiSequences.NewLine}{HeaderLine}{AnsiSequences.NewLine}");

                var lines = helpTopic.Contents.Split(new string[] { AnsiSequences.NewLine }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    sb.Append($"<%mxpopenline%>{line}{AnsiSequences.NewLine}");
                }
                sb.Append("<%n%>");

                return sb.ToString().Trim();
            }
            else
            {
                return $"{HeaderLine}HELP TOPIC: {helpTopic.Aliases.First()}{AnsiSequences.NewLine}{HeaderLine}{AnsiSequences.NewLine}{helpTopic.Contents}<%n%>";
            }
        }
    }
}
