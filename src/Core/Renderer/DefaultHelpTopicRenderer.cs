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
        public override OutputBuilder Render(TerminalOptions terminalOptions, HelpTopic helpTopic)
        {
            var output = new OutputBuilder();
            
            // TODO: What was this !element doing? Does it still work? Test with zMUD or something and re-read MXP specs?
            if (terminalOptions.UseMXP)
                output.AppendLine($"{AnsiSequences.MxpSecureLine}<!element see '<send href=\"help &cref;\">' att='cref' open>");
            
            output.AppendSeparator(color:"yellow", design:'=');
            output.AppendLine($"HELP TOPIC: {helpTopic.Aliases.First()}");
            output.AppendSeparator(color:"yellow");

            if (terminalOptions.UseMXP)
            {
                var lines = helpTopic.Contents.Split(new string[] { AnsiSequences.NewLine }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    output.AppendLine($"{AnsiSequences.MxpOpenLine}{line}<%n%>");
                }
            }
            else
                output.AppendLine($"{helpTopic.Contents}");
            
            output.AppendSeparator(color:"yellow", design:'=');
            return output;
        }
    }
}
