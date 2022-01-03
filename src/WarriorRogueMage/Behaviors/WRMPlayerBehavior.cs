//-----------------------------------------------------------------------------
// <copyright file="WRMPlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;
using WheelMUD.Server;

namespace WarriorRogueMage.Behaviors
{
    /// <summary>Behavior which defines a Warrior Rogue and Mage player.</summary>
    public class WRMPlayerBehavior : PlayerBehavior
    {
        /// <summary>Gets the Fate statistic of this player, in a format suitable for display in a Prompt.</summary>
        /// <returns>The Fate statistic of this player, in a format suitable for display in a Prompt.</returns>
        [PlayerPromptable("%FATE%", "Displays your current Fate.")]
        public string GetCurrentFate()
        {
            return Parent.Stats["FATE"].Value.ToString();
        }

        /// <summary>Builds the prompt string for this player.</summary>
        /// <returns>The formatted prompt for this player.</returns>
        public override OutputBuilder BuildPrompt(TerminalOptions terminalOptions)
        {
            return new OutputBuilder(5).Append("WRM> ");
            /* TODO: Allow for player-customized prompts...
            // Returns a string with the %tokens% replaced with appropriate values
            string prompt = Prompt;

            if (prompt != null && prompt.Contains("%"))  // Quick check, we can skip parsing if we have no chance of tokens
            {
                // TODO: This routine needs optimization
                // Find all PlayerPromptables and replace their tokens with values if they exist in the supplied prompt string
                foreach (MethodInfo m in GetType().GetMethods())
                {
                    object[] promptAttrs = m.GetCustomAttributes(typeof(PlayerPromptableAttribute), false);
                    if (promptAttrs.Length > 0 &&
                        prompt.IndexOf(((PlayerPromptableAttribute)promptAttrs[0]).Token) >= 0)
                    {
                        var promptAttr = (PlayerPromptableAttribute)promptAttrs[0];
                        prompt = prompt.Replace(promptAttr.Token, (string)m.Invoke(this, new object[] { }));
                    }
                }
            }

            return prompt;
            */
        }
    }
}