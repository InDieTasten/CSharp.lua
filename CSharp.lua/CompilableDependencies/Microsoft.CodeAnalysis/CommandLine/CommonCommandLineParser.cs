// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.CodeAnalysis
{
    public abstract class CommandLineParser
    {
        private readonly CommonMessageProvider _messageProvider;
        internal readonly bool IsScriptCommandLineParser;
        private static readonly char[] s_searchPatternTrimChars = new char[] { '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', '\x00a0' };

        internal CommandLineParser(CommonMessageProvider messageProvider, bool isScriptCommandLineParser)
        {
            _messageProvider = messageProvider;
            IsScriptCommandLineParser = isScriptCommandLineParser;
        }

        internal CommonMessageProvider MessageProvider
        {
            get { return _messageProvider; }
        }

        protected abstract string RegularFileExtension { get; }
        protected abstract string ScriptFileExtension { get; }

        internal abstract CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory, string sdkDirectoryOpt, string additionalReferenceDirectories);

        /// <summary>
        /// Parses a command line.
        /// </summary>
        /// <param name="args">A collection of strings representing the command line arguments.</param>
        /// <param name="baseDirectory">The base directory used for qualifying file locations.</param>
        /// <param name="sdkDirectory">The directory to search for mscorlib, or null if not available.</param>
        /// <param name="additionalReferenceDirectories">A string representing additional reference paths.</param>
        /// <returns>a <see cref="CommandLineArguments"/> object representing the parsed command line.</returns>
        public CommandLineArguments Parse(IEnumerable<string> args, string baseDirectory, string sdkDirectory, string additionalReferenceDirectories)
        {
            return CommonParse(args, baseDirectory, sdkDirectory, additionalReferenceDirectories);
        }

        private static bool IsOption(string arg)
        {
            return !string.IsNullOrEmpty(arg) && (arg[0] == '/' || arg[0] == '-');
        }

        internal void FlattenArgs(
            IEnumerable<string> rawArguments,
            IList<Diagnostic> diagnostics,
            List<string> processedArgs,
            List<string> scriptArgsOpt,
            string baseDirectory,
            List<string> responsePaths = null)
        {
            bool parsingScriptArgs = false;
            bool sourceFileSeen = false;
            bool optionsEnded = false;

            var args = new Stack<string>(rawArguments.Reverse());
            while (args.Count > 0)
            {
                // EDMAURER trim off whitespace. Otherwise behavioral differences arise
                // when the strings which represent args are constructed by cmd or users.
                // cmd won't produce args with whitespace at the end.
                string arg = args.Pop().TrimEnd();

                if (parsingScriptArgs)
                {
                    scriptArgsOpt.Add(arg);
                    continue;
                }

                if (scriptArgsOpt != null)
                {
                    // The order of the following two checks matters.
                    //
                    // Command line:               Script:    Script args:
                    //   csi -- script.csx a b c   script.csx      ["a", "b", "c"]
                    //   csi script.csx -- a b c   script.csx      ["--", "a", "b", "c"]
                    //   csi -- @script.csx a b c  @script.csx     ["a", "b", "c"]
                    //
                    if (sourceFileSeen)
                    {
                        // csi/vbi: at most one script can be specified on command line, anything else is a script arg:
                        parsingScriptArgs = true;
                        scriptArgsOpt.Add(arg);
                        continue;
                    }

                    if (!optionsEnded && arg == "--")
                    {
                        // csi/vbi: no argument past "--" should be treated as an option/response file
                        optionsEnded = true;
                        processedArgs.Add(arg);
                        continue;
                    }
                }

                if (!optionsEnded && arg.StartsWith("@", StringComparison.Ordinal))
                {
                    // response file:
                    string path = RemoveQuotesAndSlashes(arg.Substring(1)).TrimEnd(null);
                    throw new NotImplementedException(); //  string resolvedPath = FileUtilities.ResolveRelativePath(path, baseDirectory);
                    //if (resolvedPath != null)
                    //{
                    //    foreach (string newArg in ParseResponseFile(resolvedPath, diagnostics).Reverse())
                    //    {
                    //        // Ignores /noconfig option specified in a response file
                    //        if (!string.Equals(newArg, "/noconfig", StringComparison.OrdinalIgnoreCase) && !string.Equals(newArg, "-noconfig", StringComparison.OrdinalIgnoreCase))
                    //        {
                    //            args.Push(newArg);
                    //        }
                    //        else
                    //        {
                    //            diagnostics.Add(Diagnostic.Create(_messageProvider, _messageProvider.WRN_NoConfigNotOnCommandLine));
                    //        }
                    //    }

                    //    if (responsePaths != null)
                    //    {
                    //        responsePaths.Add(FileUtilities.NormalizeAbsolutePath(PathUtilities.GetDirectoryName(resolvedPath)));
                    //    }
                    //}
                    //else
                    //{
                    //    diagnostics.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, path));
                    //}
                }
                else
                {
                    processedArgs.Add(arg);
                    sourceFileSeen |= optionsEnded || !IsOption(arg);
                }
            }
        }

        /// <summary>
        /// Remove the extraneous quotes and slashes from the argument.  This function is designed to have
        /// compat behavior with the native compiler.
        /// </summary>
        /// <remarks>
        /// Mimics the function RemoveQuotes from the native C# compiler.  The native VB equivalent of this 
        /// function is called RemoveQuotesAndSlashes.  It has virtually the same behavior except for a few 
        /// quirks in error cases.  
        /// </remarks>
        internal static string RemoveQuotesAndSlashes(string arg)
        {
            if (arg == null)
            {
                return arg;
            }

            var builder = new StringBuilder(); // [Lua] Originally comes from PooledStringBuilder.Instance.Builder
            var i = 0;
            while (i < arg.Length)
            {
                var cur = arg[i];
                switch (cur)
                {
                    case '\\':
                        ProcessSlashes(builder, arg, ref i);
                        break;
                    case '"':
                        // Intentionally dropping quotes that don't have explicit escaping.
                        i++;
                        break;
                    default:
                        builder.Append(cur);
                        i++;
                        break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Mimic behavior of the native function by the same name.
        /// </summary>
        internal static void ProcessSlashes(StringBuilder builder, string arg, ref int i)
        {
            //Debug.Assert(arg != null);
            //Debug.Assert(i < arg.Length);

            var slashCount = 0;
            while (i < arg.Length && arg[i] == '\\')
            {
                slashCount++;
                i++;
            }

            if (i < arg.Length && arg[i] == '"')
            {
                // Before a quote slashes are interpretted as escape sequences for other slashes so
                // output one for every two.
                while (slashCount >= 2)
                {
                    builder.Append('\\');
                    slashCount -= 2;
                }

                //Debug.Assert(slashCount >= 0);

                // If there is an odd number of slashes then the quote is escaped and hence a part
                // of the output.  Otherwise it is a normal quote and can be ignored. 
                if (slashCount == 1)
                {
                    // The quote is escaped so eat it.
                    builder.Append('"');
                }

                i++;
            }
            else
            {
                // Slashes that aren't followed by quotes are simply slashes.
                while (slashCount > 0)
                {
                    builder.Append('\\');
                    slashCount--;
                }
            }
        }

        /// <summary>
        /// Split a string, based on whether "splitHere" returned true on each character.
        /// </summary>
        private static IEnumerable<string> Split(string str, Func<char, bool> splitHere)
        {
            if (str == null)
            {
                yield break;
            }

            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (splitHere(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        private static readonly char[] s_pathSeparators = { ';', ',' };
        private static readonly char[] s_wildcards = new[] { '*', '?' };

        internal static IEnumerable<string> ParseSeparatedPaths(string str)
        {
            return ParseSeparatedStrings(str, s_pathSeparators, StringSplitOptions.RemoveEmptyEntries).Select(RemoveQuotesAndSlashes);
        }

        /// <summary>
        /// Split a string by a set of separators, taking quotes into account.
        /// </summary>
        internal static IEnumerable<string> ParseSeparatedStrings(string str, char[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            bool inQuotes = false;

            var result = Split(str,
                (c =>
                {
                    if (c == '\"')
                    {
                        inQuotes = !inQuotes;
                    }

                    return !inQuotes && separators.Contains(c);
                }));

            return (options == StringSplitOptions.RemoveEmptyEntries) ? result.Where(s => s.Length > 0) : result;
        }

        internal abstract void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> errors);
    }
}
