//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Utilities;
using System;
using System.Linq;

namespace WheelMUD.Core
{
    [RendererExports.HelpTopic(0)]
    public class DefaultHelpTopicRenderer : RendererDefinitions.HelpTopic
    {
        public override string Render(TerminalOptions terminalOptions, HelpTopic helpTopic)
        {
            var sb = new AnsiBuilder();
            
            // TODO: What was this !element doing? Does it still work? Test with zMUD or something and re-read MXP specs?
            if (terminalOptions.UseMXP)
                sb.AppendLine("<%mxpsecureline%><!element see '<send href=\"help &cref;\">' att='cref' open>");
            
            sb.AppendSeparator(color:"yellow", design:'=');
            sb.AppendLine($"HELP TOPIC: {helpTopic.Aliases.First()}");
            sb.AppendSeparator(color:"yellow");

            if (terminalOptions.UseMXP)
            {
                var lines = helpTopic.Contents.Split(new string[] { AnsiSequences.NewLine }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    sb.AppendLine($"<%mxpopenline%>{line}<%n%>");
                }
            }
            else
                sb.AppendLine($"{helpTopic.Contents}");
            
            sb.AppendSeparator(color:"yellow", design:'=');
            return sb.ToString();
        }
    }
}
