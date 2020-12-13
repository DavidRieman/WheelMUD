//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Text;
    using WheelMUD.Interfaces;

    [RendererExports.HelpTopic(0)]
    public class DefaultHelpTopicRenderer : RendererDefinitions.HelpTopic
    {
        public override string Render(ITerminal terminal, HelpTopic helpTopic)
        {
            if (terminal.UseMXP)
            {
                var sb = new StringBuilder();
                foreach (string line in helpTopic.Contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    sb.AppendLine("<%mxpopenline%>" + line);
                }

                return "<%mxpsecureline%><!element see '<send href=\"help &cref;\">' att='cref' open>" + sb.ToString().Trim(Environment.NewLine.ToCharArray());
            }
            else
            {
                // TODO: Output without MXP syntax!
                return "TODO: RENDER TOPIC: " + helpTopic.Contents;
            }
        }
    }
}
