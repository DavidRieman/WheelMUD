//-----------------------------------------------------------------------------
// <copyright file="WordWrapHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WheelMUD.Utilities;

namespace WheelMUD.Server
{
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
            // TODO: Next line is a hack. Needs replacing.
            originalText = originalText.Replace("<%nl%>", Environment.NewLine);
            
            var taglessText = HideInvisibleTags(originalText, out var replacedTags);

            var wrappedText = Wrapper(taglessText, maxWidth);

            var wrappedBlock = wrappedText.Aggregate(string.Empty, (current, textLine) => $"{current}{AnsiSequences.NewLine}{textLine}");

            wrappedBlock = wrappedBlock[1..].TrimStart();

            return replacedTags.Count > 0 ? ReplaceInvisibleTags(wrappedBlock, replacedTags) : wrappedBlock;
        }

        /// <summary>Looks for ANSI tags and MXP tags that are "invisible" when it comes to word wrapping.</summary>
        /// <param name="originalText">The text to check for tags.</param>
        /// <param name="replacedText">A list of the replaced tags ready for reinsertion.</param>
        /// <returns>A string stripped of tags.</returns>
        private static string HideInvisibleTags(string originalText, out List<string> replacedText)
        {
            // Regex match
            var options = RegexOptions.None;

            ////Regex regex = new Regex(@"</?\w+[^>]*>", options);
            var regex = new Regex(@"</?.[^>]*>", options);
            ////Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", options);

            var modifiedText = new AnsiBuilder();
            replacedText = new List<string>();

            // Get matches
            var match = regex.Match(originalText);
            var matchPosition = 0;
            while (match.Success)
            {
                replacedText.Add(match.Value);
                modifiedText.Append(originalText.Substring(matchPosition, match.Index - matchPosition));
                modifiedText.Append(TagPlaceHolder);
                matchPosition = match.Index + match.Length;
                match = match.NextMatch();
            }

            if (modifiedText.Length <= 0) return originalText;
            
            modifiedText.Append(originalText[matchPosition..]);
            return modifiedText.ToString();

        }

        /// <summary>Replaces all place holder tags with their original invisible tags.</summary>
        /// <param name="originalText">The text to check for tags.</param>
        /// <param name="replacements">A list of the replaced tags ready for reinsertion.</param>
        /// <returns>The output, with all place holder tags replaced with their original tags.</returns>
        private static string ReplaceInvisibleTags(string originalText, IList<string> replacements)
        {
            var modifiedText = new AnsiBuilder();

            var sections =
                originalText.Split(new[] { TagPlaceHolder }, StringSplitOptions.None);

            var i = 0;

            foreach (var section in sections)
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
            var textParagraphs = originalText.Split('\n');
            var textLines = new ArrayList();

            foreach (var t in textParagraphs)
            {
                if (t.Length <= maxWidth)
                {
                    // Block of text is smaller then width, add it.
                    textLines.Add(t);
                }
                else
                {
                    // Block of text is longer, break it up in seperate lines.
                    var lines = BreakLines(t, maxWidth);
                    foreach (var t1 in lines)
                    {
                        textLines.Add(t1);
                    }
                }
            }

            var wrappedText = new string[textLines.Count];
            textLines.CopyTo(wrappedText, 0);
            return wrappedText;
        }

        /// <summary>Inserts line breaks into lines that are longer than the max width, based on word bounds.</summary>
        /// <param name="originalText">The text we want to insert line breaks into.</param>
        /// <param name="maxWidth">The maximum width, in characters, of output per line.</param>
        /// <returns>The finished text, with line breaks inserted.</returns>
        private static string[] BreakLines(string originalText, int maxWidth)
        {
            var textWords = originalText.Trim().Split(' ');
            var wordIndex = 0;
            var tmpLine = string.Empty;
            var textLines = new ArrayList();

            while (wordIndex < textWords.Length)
            {
                if (textWords[wordIndex] == string.Empty)
                {
                    wordIndex++;
                }
                else
                {
                    var backupLine = tmpLine;
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

            var textLinesStr = new string[textLines.Count];
            textLines.CopyTo(textLinesStr, 0);
            return textLinesStr;
        }
    }
}