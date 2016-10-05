//-----------------------------------------------------------------------------
// <copyright file="WordWrapHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This class wraps the output at the desired width.
//   Created: December 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>This class wraps the output at the desired width.</summary>
    internal static class WordWrapHandler
    {
        /// <summary>Place holder tag for stripped invisible tags.</summary>
        private const string TagPlaceHolder = "\x1A";

        /// <summary>Parse the specified text to insert line breaks based on word wrapping.</summary>
        /// <param name="originalText">The original text to be parsed.</param>
        /// <param name="maxWidth">The maximum width per line of output.</param>
        /// <returns>The modified version of the original text, with line breaks inserted.</returns>
        public static string Parse(string originalText, int maxWidth)
        {
            // @@@ Next line is a hack. Needs replacing.
            originalText = originalText.Replace("<%nl%>", Environment.NewLine);

            List<string> replacedTags = new List<string>();
            string taglessText = HideInvisibleTags(originalText, out replacedTags);

            string[] wrappedText = Wrapper(taglessText, maxWidth);

            string wrappedBlock = string.Empty;
            foreach (string textLine in wrappedText)
            {
                wrappedBlock = string.Format("{0}\r\n{1}", wrappedBlock, textLine);
            }

            wrappedBlock = wrappedBlock.Substring(1).TrimStart();

            if (replacedTags.Count > 0)
            {
                return ReplaceInvisibleTags(wrappedBlock, replacedTags);
            }

            return wrappedBlock;
        }

        /// <summary>Looks for ANSI tags and MXP tags that are "invisible" when it comes to word wrapping.</summary>
        /// <param name="originalText">The text to check for tags.</param>
        /// <param name="replacedText">A list of the replaced tags ready for reinsertion.</param>
        /// <returns>A string stripped of tags.</returns>
        private static string HideInvisibleTags(string originalText, out List<string> replacedText)
        {
            // Regex match
            RegexOptions options = RegexOptions.None;

            ////Regex regex = new Regex(@"</?\w+[^>]*>", options);
            Regex regex = new Regex(@"</?.[^>]*>", options);
            ////Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", options);

            StringBuilder modifiedText = new StringBuilder();
            replacedText = new List<string>();

            // Get matches
            Match match = regex.Match(originalText);
            int matchPosition = 0;
            while (match.Success == true)
            {
                replacedText.Add(match.Value);
                modifiedText.Append(originalText.Substring(matchPosition, match.Index - matchPosition));
                modifiedText.Append(TagPlaceHolder);
                matchPosition = match.Index + match.Length;
                match = match.NextMatch();
            }

            if (modifiedText.Length > 0)
            {
                modifiedText.Append(originalText.Substring(matchPosition));
                return modifiedText.ToString();
            }

            return originalText;
        }

        /// <summary>Replaces all place holder tags with their original invisible tags.</summary>
        /// <param name="originalText">The text to check for tags.</param>
        /// <param name="replacements">A list of the replaced tags ready for reinsertion.</param>
        /// <returns>The output, with all place holder tags replaced with their original tags.</returns>
        private static string ReplaceInvisibleTags(string originalText, IList<string> replacements)
        {
            StringBuilder modifiedText = new StringBuilder();

            string[] sections =
                originalText.Split(new string[] { TagPlaceHolder }, StringSplitOptions.None);

            int i = 0;

            foreach (string section in sections)
            {
                if (section.Length > 0)
                {
                    modifiedText.Append(section);
                }

                if (replacements.Count > i)
                {
                    modifiedText.Append(replacements[i]);
                }

                i++;
            }

            return modifiedText.ToString();
        }

        /// <summary>The core word wrapping functionality.</summary>
        /// <param name="originalText">The text we want to insert line breaks into.</param>
        /// <param name="maxWidth">The maximum width, in characters, of output per line.</param>
        /// <returns>The finished text, with line breaks inserted.</returns>
        private static string[] Wrapper(string originalText, int maxWidth)
        {
            originalText = originalText.Replace("\r\n", "\n");
            originalText = originalText.Replace("\r", "\n");
            originalText = originalText.Replace("\t", " ");
            string[] textParagraphs = originalText.Split('\n');
            ArrayList textLines = new ArrayList();

            for (int i = 0; i < textParagraphs.Length; i++)
            {
                if (textParagraphs[i].Length <= maxWidth)
                {
                    // Block of text is smaller then width, add it.
                    textLines.Add(textParagraphs[i]);
                }
                else
                {
                    // Block of text is longer, break it up in seperate lines.
                    string[] lines = BreakLines(textParagraphs[i], maxWidth);
                    for (int j = 0; j < lines.Length; j++)
                    {
                        textLines.Add(lines[j]);
                    }
                }
            }

            string[] wrappedText = new string[textLines.Count];
            textLines.CopyTo(wrappedText, 0);
            return wrappedText;
        }

        /// <summary>Inserts line breaks into lines that are longer than the max width, based on word bounds.</summary>
        /// <param name="originalText">The text we want to insert line breaks into.</param>
        /// <param name="maxWidth">The maximum width, in characters, of output per line.</param>
        /// <returns>The finished text, with line breaks inserted.</returns>
        private static string[] BreakLines(string originalText, int maxWidth)
        {
            string[] textWords = originalText.Trim().Split(' ');
            int wordIndex = 0;
            string tmpLine = string.Empty;
            ArrayList textLines = new ArrayList();

            while (wordIndex < textWords.Length)
            {
                if (textWords[wordIndex] == string.Empty)
                {
                    wordIndex++;
                }
                else
                {
                    string backupLine = tmpLine;
                    if (tmpLine == string.Empty)
                    {
                        tmpLine = textWords[wordIndex];
                    }
                    else
                    {
                        tmpLine = tmpLine + " " + textWords[wordIndex];
                    }

                    if (tmpLine.Length <= maxWidth)
                    {
                        wordIndex++;

                        // If our line is still small enough, but we don't have anymore words
                        // add the line to the collection.
                        if (wordIndex == textWords.Length)
                        {
                            textLines.Add(tmpLine);
                        }
                    }
                    else if (textWords[wordIndex].Length >= maxWidth)
                    {
                        wordIndex++;
                        textLines.Add(tmpLine);
                    }
                    else
                    {
                        // Our line is too long, add the previous line to the collection
                        // and reset the line, the word causing the 'overflow' will be
                        // the first word of the new line.
                        textLines.Add(backupLine);
                        tmpLine = string.Empty;
                    }
                }
            }

            string[] textLinesStr = new string[textLines.Count];
            textLines.CopyTo(textLinesStr, 0);
            return textLinesStr;
        }
    }
}