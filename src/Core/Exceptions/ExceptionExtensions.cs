//-----------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: November 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Text;

    /// <summary>Extensions to the Exception class.</summary>
    public static class ExceptionExtensions
    {
        /// <summary>Build the exception message, stack trace, and so on for inner exceptions, into a string.</summary>
        /// <param name="ex">The exception.</param>
        /// <returns>A potentially-long multi-line report string.</returns>
        public static string ToDeepString(this Exception ex)
        {
            // Start building an exception report with the full exception details.
            StringBuilder sb = new StringBuilder();

            // Display the top level exception message and the call stack, IE:
            // Unhandled NullReferenceException: exception text. At Stack Trace: ...
            sb.Append("Unhandled ");
            AppendExceptionInfo(ex, sb);

            // Traverse all inner exceptions to append their data too.
            ex = ex.InnerException;
            while (ex != null)
            {
                sb.AppendLine();
                sb.Append("Inner ");
                AppendExceptionInfo(ex, sb);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        /// <summary>Append full exception information into a StringBuilder.</summary>
        /// <param name="ex">The exception whose information is being mined.</param>
        /// <param name="sb">The StringBuilder to append.</param>
        private static void AppendExceptionInfo(Exception ex, StringBuilder sb)
        {
            sb.AppendLine(string.Format("{0}: {1}", ex.GetType(), ex.Message));
            sb.AppendLine();
            sb.AppendLine("At StackTrace:");
            sb.AppendLine(ex.StackTrace);
        }
    }
}