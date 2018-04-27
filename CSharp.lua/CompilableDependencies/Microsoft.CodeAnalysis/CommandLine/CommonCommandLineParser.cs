// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Roslyn.Utilities;
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

        internal IEnumerable<CommandLineSourceFile> ParseFileArgument(string arg, string baseDirectory, IList<Diagnostic> errors)
        {
            //Debug.Assert(IsScriptCommandLineParser || !arg.StartsWith("-", StringComparison.Ordinal) && !arg.StartsWith("@", StringComparison.Ordinal));

            // We remove all doubles quotes from a file name. So that, for example:
            //   "Path With Spaces"\goo.cs
            // becomes
            //   Path With Spaces\goo.cs

            string path = RemoveQuotesAndSlashes(arg);

            int wildcard = path.IndexOfAny(s_wildcards);
            if (wildcard != -1)
            {
                foreach (var file in ExpandFileNamePattern(path, baseDirectory, SearchOption.TopDirectoryOnly, errors))
                {
                    yield return file;
                }
            }
            else
            {
                string resolvedPath = FileUtilities.ResolveRelativePath(path, baseDirectory);
                if (resolvedPath == null)
                {
                    errors.Add(Diagnostic.Create(MessageProvider, (int)MessageProvider.FTL_InvalidInputFileName, path));
                }
                else
                {
                    yield return ToCommandLineSourceFile(resolvedPath);
                }
            }
        }

        private CommandLineSourceFile ToCommandLineSourceFile(string resolvedPath)
        {
            string extension = PathUtilities.GetExtension(resolvedPath);

            bool isScriptFile;
            if (IsScriptCommandLineParser)
            {
                isScriptFile = !string.Equals(extension, RegularFileExtension, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // TODO: uncomment when fixing https://github.com/dotnet/roslyn/issues/5325
                //isScriptFile = string.Equals(extension, ScriptFileExtension, StringComparison.OrdinalIgnoreCase);
                isScriptFile = false;
            }

            return new CommandLineSourceFile(resolvedPath, isScriptFile);
        }

        private IEnumerable<CommandLineSourceFile> ExpandFileNamePattern(
            string path,
            string baseDirectory,
            SearchOption searchOption,
            IList<Diagnostic> errors)
        {
            string directory = PathUtilities.GetDirectoryName(path);
            string pattern = PathUtilities.GetFileName(path);

            var resolvedDirectoryPath = (directory.Length == 0) ? baseDirectory : FileUtilities.ResolveRelativePath(directory, baseDirectory);

            IEnumerator<string> enumerator = null;
            try
            {
                bool yielded = false;

                // NOTE: Directory.EnumerateFiles(...) surprisingly treats pattern "." the 
                //       same way as "*"; as we don't expect anything to be found by this 
                //       pattern, let's just not search in this case
                pattern = pattern.Trim(s_searchPatternTrimChars);
                bool singleDotPattern = string.Equals(pattern, ".", StringComparison.Ordinal);

                if (!singleDotPattern)
                {
                    while (true)
                    {
                        string resolvedPath = null;
                        try
                        {
                            if (enumerator == null)
                            {
                                enumerator = EnumerateFiles(resolvedDirectoryPath, pattern, searchOption).GetEnumerator();
                            }

                            if (!enumerator.MoveNext())
                            {
                                break;
                            }

                            resolvedPath = enumerator.Current;
                        }
                        catch
                        {
                            resolvedPath = null;
                        }

                        if (resolvedPath != null)
                        {
                            // just in case EnumerateFiles returned a relative path
                            resolvedPath = FileUtilities.ResolveRelativePath(resolvedPath, baseDirectory);
                        }

                        if (resolvedPath == null)
                        {
                            errors.Add(Diagnostic.Create(MessageProvider, (int)MessageProvider.FTL_InvalidInputFileName, path));
                            break;
                        }

                        yielded = true;
                        yield return ToCommandLineSourceFile(resolvedPath);
                    }
                }

                // the pattern didn't match any files:
                if (!yielded)
                {
                    if (searchOption == SearchOption.AllDirectories)
                    {
                        // handling /recurse
                        GenerateErrorForNoFilesFoundInRecurse(path, errors);
                    }
                    else
                    {
                        // handling wildcard in file spec
                        errors.Add(Diagnostic.Create(MessageProvider, (int)MessageProvider.ERR_FileNotFound, path));
                    }
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
        }

        /// <summary>
        /// Enumerates files in the specified directory and subdirectories whose name matches the given pattern.
        /// </summary>
        /// <param name="directory">Full path of the directory to enumerate.</param>
        /// <param name="fileNamePattern">File name pattern. May contain wildcards '*' (matches zero or more characters) and '?' (matches any character).</param>
        /// <param name="searchOption">Specifies whether to search the specified <paramref name="directory"/> only, or all its subdirectories as well.</param>
        /// <returns>Sequence of file paths.</returns>
        internal virtual IEnumerable<string> EnumerateFiles(string directory, string fileNamePattern, SearchOption searchOption)
        {
            //Debug.Assert(PathUtilities.IsAbsolute(directory));
            return Directory.EnumerateFiles(directory, fileNamePattern, searchOption);
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

        internal static bool TryParseOption(string arg, out string name, out string value)
        {
            if (!IsOption(arg))
            {
                name = null;
                value = null;
                return false;
            }

            int colon = arg.IndexOf(':');

            // temporary heuristic to detect Unix-style rooted paths
            // pattern /goo/*  or  //* will not be treated as a compiler option
            //
            // TODO: consider introducing "/s:path" to disambiguate paths starting with /
            if (arg.Length > 1 && arg[0] != '-')
            {
                int separator = arg.IndexOf('/', 1);
                if (separator > 0 && (colon < 0 || separator < colon))
                {
                    //   "/goo/
                    //   "//
                    name = null;
                    value = null;
                    return false;
                }
            }

            if (colon >= 0)
            {
                name = arg.Substring(1, colon - 1);
                value = arg.Substring(colon + 1);
            }
            else
            {
                name = arg.Substring(1);
                value = null;
            }

            name = name.ToLowerInvariant();
            return true;
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
