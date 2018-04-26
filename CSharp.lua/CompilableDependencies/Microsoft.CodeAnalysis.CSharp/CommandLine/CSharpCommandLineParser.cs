﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.CodeAnalysis.CSharp
{
    public class CSharpCommandLineParser : CommandLineParser
    {
        public static CSharpCommandLineParser Default { get; } = new CSharpCommandLineParser();

        protected override string RegularFileExtension => throw new NotImplementedException();

        protected override string ScriptFileExtension => throw new NotImplementedException();

        internal CSharpCommandLineParser(bool isScriptCommandLineParser = false)
            : base(CSharp.MessageProvider.Instance, isScriptCommandLineParser)
        {
        }

        public new CSharpCommandLineArguments Parse(IEnumerable<string> args, string baseDirectory, string sdkDirectory, string additionalReferenceDirectories = null)
        {
            List<Diagnostic> diagnostics = new List<Diagnostic>();
            List<string> flattenedArgs = new List<string>();

            // Reduced imput parameters
            FlattenArgs(args, null, flattenedArgs, null, baseDirectory, null);

            string appConfigPath = null;
            bool displayLogo = true;
            bool displayHelp = false;
            bool displayVersion = false;
            bool displayLangVersions = false;
            bool optimize = false;
            bool checkOverflow = false;
            bool allowUnsafe = false;
            bool concurrentBuild = true;
            bool deterministic = false; // TODO(5431): Enable deterministic mode by default
            bool emitPdb = false;
            bool debugPlus = false;
            string pdbPath = null;
            bool noStdLib = IsScriptCommandLineParser; // don't add mscorlib from sdk dir when running scripts
            string outputDirectory = baseDirectory;
            string outputFileName = null;
            string outputRefFilePath = null;
            bool refOnly = false;
            string documentationPath = null;
            string errorLogPath = null;
            bool parseDocumentationComments = false; //Don't just null check documentationFileName because we want to do this even if the file name is invalid.
            bool utf8output = false;
            OutputKind outputKind = OutputKind.ConsoleApplication;
            SubsystemVersion subsystemVersion = SubsystemVersion.None;
            LanguageVersion languageVersion = LanguageVersion.Default;
            string mainTypeName = null;
            string win32ManifestFile = null;
            string win32ResourceFile = null;
            string win32IconFile = null;
            bool noWin32Manifest = false;
            Platform platform = Platform.AnyCpu;
            ulong baseAddress = 0;
            int fileAlignment = 0;
            bool? delaySignSetting = null;
            string keyFileSetting = null;
            string keyContainerSetting = null;
            List<ResourceDescription> managedResources = new List<ResourceDescription>();
            List<CommandLineSourceFile> sourceFiles = new List<CommandLineSourceFile>();
            List<CommandLineSourceFile> additionalFiles = new List<CommandLineSourceFile>();
            List<CommandLineSourceFile> embeddedFiles = new List<CommandLineSourceFile>();
            bool sourceFilesSpecified = false;
            bool embedAllSourceFiles = false;
            bool resourcesOrModulesSpecified = false;
            Encoding codepage = null;
            List<CommandLineReference> metadataReferences = new List<CommandLineReference>();
            List<CommandLineAnalyzerReference> analyzers = new List<CommandLineAnalyzerReference>();
            List<string> libPaths = new List<string>();
            List<string> sourcePaths = new List<string>();
            List<string> keyFileSearchPaths = new List<string>();
            List<string> usings = new List<string>();
            var generalDiagnosticOption = ReportDiagnostic.Default;
            var diagnosticOptions = new Dictionary<string, ReportDiagnostic>();
            var noWarns = new Dictionary<string, ReportDiagnostic>();
            var warnAsErrors = new Dictionary<string, ReportDiagnostic>();
            int warningLevel = 4;
            bool highEntropyVA = false;
            bool printFullPaths = false;
            string moduleAssemblyName = null;
            string moduleName = null;
            List<string> features = new List<string>();
            string runtimeMetadataVersion = null;
            bool errorEndLocation = false;
            bool reportAnalyzer = false;
            string touchedFilesPath = null;
            bool optionsEnded = false;
            bool interactiveMode = false;
            bool publicSign = false;
            string sourceLink = null;
            string ruleSetPath = null;

            // Process ruleset files first so that diagnostic severity settings specified on the command line via
            // /nowarn and /warnaserror can override diagnostic severity settings specified in the ruleset file.
            if (!IsScriptCommandLineParser)
            {
                foreach (string arg in flattenedArgs)
                {
                    string name, value;
                    if (TryParseOption(arg, out name, out value) && (name == "ruleset"))
                    {
                        var unquoted = RemoveQuotesAndSlashes(value);

                        if (string.IsNullOrEmpty(unquoted))
                        {
                            throw new NotImplementedException(); // [Lua] AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
                        }
                        else
                        {
                            throw new NotImplementedException(); // [Lua] ruleSetPath = ParseGenericPathToFile(unquoted, diagnostics, baseDirectory);
                            throw new NotImplementedException(); // [Lua] generalDiagnosticOption = GetDiagnosticOptionsFromRulesetFile(ruleSetPath, out diagnosticOptions, diagnostics);
                        }
                    }
                }
            }

            throw new NotImplementedException(); // [Lua]

//            foreach (string arg in flattenedArgs)
//            {
//                //Debug.Assert(optionsEnded || !arg.StartsWith("@", StringComparison.Ordinal));

//                string name, value;
//                if (optionsEnded || !TryParseOption(arg, out name, out value))
//                {
//                    sourceFiles.AddRange(ParseFileArgument(arg, baseDirectory, diagnostics));
//                    if (sourceFiles.Count > 0)
//                    {
//                        sourceFilesSpecified = true;
//                    }

//                    continue;
//                }

//                switch (name)
//                {
//                    case "?":
//                    case "help":
//                        displayHelp = true;
//                        continue;

//                    case "version":
//                        displayVersion = true;
//                        continue;

//                    case "r":
//                    case "reference":
//                        metadataReferences.AddRange(ParseAssemblyReferences(arg, value, diagnostics, embedInteropTypes: false));
//                        continue;

//                    case "features":
//                        if (value == null)
//                        {
//                            features.Clear();
//                        }
//                        else
//                        {
//                            features.Add(value);
//                        }
//                        continue;

//                    case "lib":
//                    case "libpath":
//                    case "libpaths":
//                        ParseAndResolveReferencePaths(name, value, baseDirectory, libPaths, MessageID.IDS_LIB_OPTION, diagnostics);
//                        continue;

//#if DEBUG
//                    case "attachdebugger":
//                        Debugger.Launch();
//                        continue;
//#endif
//                }

//                if (IsScriptCommandLineParser)
//                {
//                    switch (name)
//                    {
//                        case "-": // csi -- script.csx
//                            if (value != null) break;

//                            // Indicates that the remaining arguments should not be treated as options.
//                            optionsEnded = true;
//                            continue;

//                        case "i":
//                        case "i+":
//                            if (value != null) break;
//                            interactiveMode = true;
//                            continue;

//                        case "i-":
//                            if (value != null) break;
//                            interactiveMode = false;
//                            continue;

//                        case "loadpath":
//                        case "loadpaths":
//                            ParseAndResolveReferencePaths(name, value, baseDirectory, sourcePaths, MessageID.IDS_REFERENCEPATH_OPTION, diagnostics);
//                            continue;

//                        case "u":
//                        case "using":
//                        case "usings":
//                        case "import":
//                        case "imports":
//                            usings.AddRange(ParseUsings(arg, value, diagnostics));
//                            continue;
//                    }
//                }
//                else
//                {
//                    switch (name)
//                    {
//                        case "a":
//                        case "analyzer":
//                            analyzers.AddRange(ParseAnalyzers(arg, value, diagnostics));
//                            continue;

//                        case "d":
//                        case "define":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", arg);
//                                continue;
//                            }

//                            IEnumerable<Diagnostic> defineDiagnostics;
//                            defines.AddRange(ParseConditionalCompilationSymbols(RemoveQuotesAndSlashes(value), out defineDiagnostics));
//                            diagnostics.AddRange(defineDiagnostics);
//                            continue;

//                        case "codepage":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (value == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
//                                continue;
//                            }

//                            var encoding = TryParseEncodingName(value);
//                            if (encoding == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.FTL_BadCodepage, value);
//                                continue;
//                            }

//                            codepage = encoding;
//                            continue;

//                        case "checksumalgorithm":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
//                                continue;
//                            }

//                            var newChecksumAlgorithm = TryParseHashAlgorithmName(value);
//                            if (newChecksumAlgorithm == SourceHashAlgorithm.None)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.FTL_BadChecksumAlgorithm, value);
//                                continue;
//                            }

//                            checksumAlgorithm = newChecksumAlgorithm;
//                            continue;

//                        case "checked":
//                        case "checked+":
//                            if (value != null)
//                            {
//                                break;
//                            }

//                            checkOverflow = true;
//                            continue;

//                        case "checked-":
//                            if (value != null)
//                                break;

//                            checkOverflow = false;
//                            continue;

//                        case "instrument":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
//                            }
//                            else
//                            {
//                                foreach (InstrumentationKind instrumentationKind in ParseInstrumentationKinds(value, diagnostics))
//                                {
//                                    if (!instrumentationKinds.Contains(instrumentationKind))
//                                    {
//                                        instrumentationKinds.Add(instrumentationKind);
//                                    }
//                                }
//                            }

//                            continue;

//                        case "noconfig":
//                            // It is already handled (see CommonCommandLineCompiler.cs).
//                            continue;

//                        case "sqmsessionguid":
//                            // The use of SQM is deprecated in the compiler but we still support the parsing of the option for
//                            // back compat reasons.
//                            if (value == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_MissingGuidForOption, "<text>", name);
//                            }
//                            else
//                            {
//                                Guid sqmSessionGuid;
//                                if (!Guid.TryParse(value, out sqmSessionGuid))
//                                {
//                                    AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidFormatForGuidForOption, value, name);
//                                }
//                            }
//                            continue;

//                        case "preferreduilang":
//                            value = RemoveQuotesAndSlashes(value);

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", arg);
//                                continue;
//                            }

//                            try
//                            {
//                                preferredUILang = new CultureInfo(value);
//                                if (CorLightup.Desktop.IsUserCustomCulture(preferredUILang) ?? false)
//                                {
//                                    // Do not use user custom cultures.
//                                    preferredUILang = null;
//                                }
//                            }
//                            catch (CultureNotFoundException)
//                            {
//                            }

//                            if (preferredUILang == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.WRN_BadUILang, value);
//                            }

//                            continue;

//                        case "out":
//                            if (string.IsNullOrWhiteSpace(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                ParseOutputFile(value, diagnostics, baseDirectory, out outputFileName, out outputDirectory);
//                            }

//                            continue;

//                        case "refout":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                outputRefFilePath = ParseGenericPathToFile(value, diagnostics, baseDirectory);
//                            }

//                            continue;

//                        case "refonly":
//                            if (value != null)
//                                break;

//                            refOnly = true;
//                            continue;

//                        case "t":
//                        case "target":
//                            if (value == null)
//                            {
//                                break; // force 'unrecognized option'
//                            }

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.FTL_InvalidTarget);
//                            }
//                            else
//                            {
//                                outputKind = ParseTarget(value, diagnostics);
//                            }

//                            continue;

//                        case "moduleassemblyname":
//                            value = value != null ? value.Unquote() : null;

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", arg);
//                            }
//                            else if (!MetadataHelpers.IsValidAssemblyOrModuleName(value))
//                            {
//                                // Dev11 C# doesn't check the name (VB does)
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidAssemblyName, "<text>", arg);
//                            }
//                            else
//                            {
//                                moduleAssemblyName = value;
//                            }

//                            continue;

//                        case "modulename":
//                            var unquotedModuleName = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquotedModuleName))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "modulename");
//                                continue;
//                            }
//                            else
//                            {
//                                moduleName = unquotedModuleName;
//                            }

//                            continue;

//                        case "platform":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<string>", arg);
//                            }
//                            else
//                            {
//                                platform = ParsePlatform(value, diagnostics);
//                            }
//                            continue;

//                        case "recurse":
//                            value = RemoveQuotesAndSlashes(value);

//                            if (value == null)
//                            {
//                                break; // force 'unrecognized option'
//                            }
//                            else if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                int before = sourceFiles.Count;
//                                sourceFiles.AddRange(ParseRecurseArgument(value, baseDirectory, diagnostics));
//                                if (sourceFiles.Count > before)
//                                {
//                                    sourceFilesSpecified = true;
//                                }
//                            }
//                            continue;

//                        case "doc":
//                            parseDocumentationComments = true;
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), arg);
//                                continue;
//                            }
//                            string unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                // CONSIDER: This diagnostic exactly matches dev11, but it would be simpler (and more consistent with /out)
//                                // if we just let the next case handle /doc:"".
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/doc:"); // Different argument.
//                            }
//                            else
//                            {
//                                documentationPath = ParseGenericPathToFile(unquoted, diagnostics, baseDirectory);
//                            }
//                            continue;

//                        case "addmodule":
//                            if (value == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/addmodule:");
//                            }
//                            else if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                // NOTE(tomat): Dev10 used to report CS1541: ERR_CantIncludeDirectory if the path was a directory.
//                                // Since we now support /referencePaths option we would need to search them to see if the resolved path is a directory.
//                                // An error will be reported by the assembly manager anyways.
//                                metadataReferences.AddRange(ParseSeparatedPaths(value).Select(path => new CommandLineReference(path, MetadataReferenceProperties.Module)));
//                                resourcesOrModulesSpecified = true;
//                            }
//                            continue;

//                        case "l":
//                        case "link":
//                            metadataReferences.AddRange(ParseAssemblyReferences(arg, value, diagnostics, embedInteropTypes: true));
//                            continue;

//                        case "win32res":
//                            win32ResourceFile = GetWin32Setting(arg, value, diagnostics);
//                            continue;

//                        case "win32icon":
//                            win32IconFile = GetWin32Setting(arg, value, diagnostics);
//                            continue;

//                        case "win32manifest":
//                            win32ManifestFile = GetWin32Setting(arg, value, diagnostics);
//                            noWin32Manifest = false;
//                            continue;

//                        case "nowin32manifest":
//                            noWin32Manifest = true;
//                            win32ManifestFile = null;
//                            continue;

//                        case "res":
//                        case "resource":
//                            if (value == null)
//                            {
//                                break; // Dev11 reports unrecognized option
//                            }

//                            var embeddedResource = ParseResourceDescription(arg, value, baseDirectory, diagnostics, embedded: true);
//                            if (embeddedResource != null)
//                            {
//                                managedResources.Add(embeddedResource);
//                                resourcesOrModulesSpecified = true;
//                            }

//                            continue;

//                        case "linkres":
//                        case "linkresource":
//                            if (value == null)
//                            {
//                                break; // Dev11 reports unrecognized option
//                            }

//                            var linkedResource = ParseResourceDescription(arg, value, baseDirectory, diagnostics, embedded: false);
//                            if (linkedResource != null)
//                            {
//                                managedResources.Add(linkedResource);
//                                resourcesOrModulesSpecified = true;
//                            }

//                            continue;

//                        case "sourcelink":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                sourceLink = ParseGenericPathToFile(value, diagnostics, baseDirectory);
//                            }
//                            continue;

//                        case "debug":
//                            emitPdb = true;

//                            // unused, parsed for backward compat only
//                            value = RemoveQuotesAndSlashes(value);
//                            if (value != null)
//                            {
//                                if (value.IsEmpty())
//                                {
//                                    AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), name);
//                                    continue;
//                                }
//                                switch (value.ToLower())
//                                {
//                                    case "full":
//                                    case "pdbonly":
//                                        debugInformationFormat = PathUtilities.IsUnixLikePlatform ? DebugInformationFormat.PortablePdb : DebugInformationFormat.Pdb;
//                                        break;
//                                    case "portable":
//                                        debugInformationFormat = DebugInformationFormat.PortablePdb;
//                                        break;
//                                    case "embedded":
//                                        debugInformationFormat = DebugInformationFormat.Embedded;
//                                        break;
//                                    default:
//                                        AddDiagnostic(diagnostics, ErrorCode.ERR_BadDebugType, value);
//                                        break;
//                                }
//                            }
//                            continue;

//                        case "debug+":
//                            //guard against "debug+:xx"
//                            if (value != null)
//                                break;

//                            emitPdb = true;
//                            debugPlus = true;
//                            continue;

//                        case "debug-":
//                            if (value != null)
//                                break;

//                            emitPdb = false;
//                            debugPlus = false;
//                            continue;

//                        case "o":
//                        case "optimize":
//                        case "o+":
//                        case "optimize+":
//                            if (value != null)
//                                break;

//                            optimize = true;
//                            continue;

//                        case "o-":
//                        case "optimize-":
//                            if (value != null)
//                                break;

//                            optimize = false;
//                            continue;

//                        case "deterministic":
//                        case "deterministic+":
//                            if (value != null)
//                                break;

//                            deterministic = true;
//                            continue;

//                        case "deterministic-":
//                            if (value != null)
//                                break;
//                            deterministic = false;
//                            continue;

//                        case "p":
//                        case "parallel":
//                        case "p+":
//                        case "parallel+":
//                            if (value != null)
//                                break;

//                            concurrentBuild = true;
//                            continue;

//                        case "p-":
//                        case "parallel-":
//                            if (value != null)
//                                break;

//                            concurrentBuild = false;
//                            continue;

//                        case "warnaserror":
//                        case "warnaserror+":
//                            if (value == null)
//                            {
//                                generalDiagnosticOption = ReportDiagnostic.Error;

//                                // Reset specific warnaserror options (since last /warnaserror flag on the command line always wins),
//                                // and bump warnings to errors.
//                                warnAsErrors.Clear();
//                                foreach (var key in diagnosticOptions.Keys)
//                                {
//                                    if (diagnosticOptions[key] == ReportDiagnostic.Warn)
//                                    {
//                                        warnAsErrors[key] = ReportDiagnostic.Error;
//                                    }
//                                }

//                                continue;
//                            }

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                            }
//                            else
//                            {
//                                AddWarnings(warnAsErrors, ReportDiagnostic.Error, ParseWarnings(value));
//                            }
//                            continue;

//                        case "warnaserror-":
//                            if (value == null)
//                            {
//                                generalDiagnosticOption = ReportDiagnostic.Default;

//                                // Clear specific warnaserror options (since last /warnaserror flag on the command line always wins).
//                                warnAsErrors.Clear();

//                                continue;
//                            }

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                            }
//                            else
//                            {
//                                foreach (var id in ParseWarnings(value))
//                                {
//                                    ReportDiagnostic ruleSetValue;
//                                    if (diagnosticOptions.TryGetValue(id, out ruleSetValue))
//                                    {
//                                        warnAsErrors[id] = ruleSetValue;
//                                    }
//                                    else
//                                    {
//                                        warnAsErrors[id] = ReportDiagnostic.Default;
//                                    }
//                                }
//                            }
//                            continue;

//                        case "w":
//                        case "warn":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (value == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                                continue;
//                            }

//                            int newWarningLevel;
//                            if (string.IsNullOrEmpty(value) ||
//                                !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out newWarningLevel))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                            }
//                            else if (newWarningLevel < 0 || newWarningLevel > 4)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_BadWarningLevel, name);
//                            }
//                            else
//                            {
//                                warningLevel = newWarningLevel;
//                            }
//                            continue;

//                        case "nowarn":
//                            if (value == null)
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                                continue;
//                            }

//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                            }
//                            else
//                            {
//                                AddWarnings(noWarns, ReportDiagnostic.Suppress, ParseWarnings(value));
//                            }
//                            continue;

//                        case "unsafe":
//                        case "unsafe+":
//                            if (value != null)
//                                break;

//                            allowUnsafe = true;
//                            continue;

//                        case "unsafe-":
//                            if (value != null)
//                                break;

//                            allowUnsafe = false;
//                            continue;

//                        case "langversion":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/langversion:");
//                            }
//                            else if (value.StartsWith("0", StringComparison.Ordinal))
//                            {
//                                // This error was added in 7.1 to stop parsing versions as ints (behaviour in previous Roslyn compilers), and explicitly
//                                // treat them as identifiers (behaviour in native compiler). This error helps users identify that breaking change.
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_LanguageVersionCannotHaveLeadingZeroes, value);
//                            }
//                            else if (value == "?")
//                            {
//                                displayLangVersions = true;
//                            }
//                            else if (!value.TryParse(out languageVersion))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_BadCompatMode, value);
//                            }
//                            continue;

//                        case "delaysign":
//                        case "delaysign+":
//                            if (value != null)
//                            {
//                                break;
//                            }

//                            delaySignSetting = true;
//                            continue;

//                        case "delaysign-":
//                            if (value != null)
//                            {
//                                break;
//                            }

//                            delaySignSetting = false;
//                            continue;

//                        case "publicsign":
//                        case "publicsign+":
//                            if (value != null)
//                            {
//                                break;
//                            }

//                            publicSign = true;
//                            continue;

//                        case "publicsign-":
//                            if (value != null)
//                            {
//                                break;
//                            }

//                            publicSign = false;
//                            continue;

//                        case "keyfile":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, "keyfile");
//                            }
//                            else
//                            {
//                                keyFileSetting = value;
//                            }
//                            // NOTE: Dev11/VB also clears "keycontainer", see also:
//                            //
//                            // MSDN: In case both /keyfile and /keycontainer are specified (either by command line option or by 
//                            // MSDN: custom attribute) in the same compilation, the compiler will first try the key container. 
//                            // MSDN: If that succeeds, then the assembly is signed with the information in the key container. 
//                            // MSDN: If the compiler does not find the key container, it will try the file specified with /keyfile. 
//                            // MSDN: If that succeeds, the assembly is signed with the information in the key file and the key 
//                            // MSDN: information will be installed in the key container (similar to sn -i) so that on the next 
//                            // MSDN: compilation, the key container will be valid.
//                            continue;

//                        case "keycontainer":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "keycontainer");
//                            }
//                            else
//                            {
//                                keyContainerSetting = value;
//                            }
//                            // NOTE: Dev11/VB also clears "keyfile", see also:
//                            //
//                            // MSDN: In case both /keyfile and /keycontainer are specified (either by command line option or by 
//                            // MSDN: custom attribute) in the same compilation, the compiler will first try the key container. 
//                            // MSDN: If that succeeds, then the assembly is signed with the information in the key container. 
//                            // MSDN: If the compiler does not find the key container, it will try the file specified with /keyfile. 
//                            // MSDN: If that succeeds, the assembly is signed with the information in the key file and the key 
//                            // MSDN: information will be installed in the key container (similar to sn -i) so that on the next 
//                            // MSDN: compilation, the key container will be valid.
//                            continue;

//                        case "highentropyva":
//                        case "highentropyva+":
//                            if (value != null)
//                                break;

//                            highEntropyVA = true;
//                            continue;

//                        case "highentropyva-":
//                            if (value != null)
//                                break;

//                            highEntropyVA = false;
//                            continue;

//                        case "nologo":
//                            displayLogo = false;
//                            continue;

//                        case "baseaddress":
//                            value = RemoveQuotesAndSlashes(value);

//                            ulong newBaseAddress;
//                            if (string.IsNullOrEmpty(value) || !TryParseUInt64(value, out newBaseAddress))
//                            {
//                                if (string.IsNullOrEmpty(value))
//                                {
//                                    AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                                }
//                                else
//                                {
//                                    AddDiagnostic(diagnostics, ErrorCode.ERR_BadBaseNumber, value);
//                                }
//                            }
//                            else
//                            {
//                                baseAddress = newBaseAddress;
//                            }

//                            continue;

//                        case "subsystemversion":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "subsystemversion");
//                                continue;
//                            }

//                            // It seems VS 2012 just silently corrects invalid values and suppresses the error message
//                            SubsystemVersion version = SubsystemVersion.None;
//                            if (SubsystemVersion.TryParse(value, out version))
//                            {
//                                subsystemVersion = version;
//                            }
//                            else
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidSubsystemVersion, value);
//                            }

//                            continue;

//                        case "touchedfiles":
//                            unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "touchedfiles");
//                                continue;
//                            }
//                            else
//                            {
//                                touchedFilesPath = unquoted;
//                            }

//                            continue;

//                        case "bugreport":
//                            UnimplementedSwitch(diagnostics, name);
//                            continue;

//                        case "utf8output":
//                            if (value != null)
//                                break;

//                            utf8output = true;
//                            continue;

//                        case "m":
//                        case "main":
//                            // Remove any quotes for consistent behavior as MSBuild can return quoted or 
//                            // unquoted main.    
//                            unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
//                                continue;
//                            }

//                            mainTypeName = unquoted;
//                            continue;

//                        case "fullpaths":
//                            if (value != null)
//                                break;

//                            printFullPaths = true;
//                            continue;

//                        case "pathmap":
//                            // "/pathmap:K1=V1,K2=V2..."
//                            {
//                                unquoted = RemoveQuotesAndSlashes(value);

//                                if (unquoted == null)
//                                {
//                                    break;
//                                }

//                                pathMap = pathMap.Concat(ParsePathMap(unquoted, diagnostics));
//                            }
//                            continue;

//                        case "filealign":
//                            value = RemoveQuotesAndSlashes(value);

//                            ushort newAlignment;
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsNumber, name);
//                            }
//                            else if (!TryParseUInt16(value, out newAlignment))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidFileAlignment, value);
//                            }
//                            else if (!CompilationOptions.IsValidFileAlignment(newAlignment))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidFileAlignment, value);
//                            }
//                            else
//                            {
//                                fileAlignment = newAlignment;
//                            }
//                            continue;

//                        case "pdb":
//                            value = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
//                            }
//                            else
//                            {
//                                pdbPath = ParsePdbPath(value, diagnostics, baseDirectory);
//                            }
//                            continue;

//                        case "errorendlocation":
//                            errorEndLocation = true;
//                            continue;

//                        case "reportanalyzer":
//                            reportAnalyzer = true;
//                            continue;

//                        case "nostdlib":
//                        case "nostdlib+":
//                            if (value != null)
//                                break;

//                            noStdLib = true;
//                            continue;

//                        case "nostdlib-":
//                            if (value != null)
//                                break;

//                            noStdLib = false;
//                            continue;

//                        case "errorreport":
//                            continue;

//                        case "errorlog":
//                            unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, ":<file>", RemoveQuotesAndSlashes(arg));
//                            }
//                            else
//                            {
//                                errorLogPath = ParseGenericPathToFile(unquoted, diagnostics, baseDirectory);
//                            }
//                            continue;

//                        case "appconfig":
//                            unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, ":<text>", RemoveQuotesAndSlashes(arg));
//                            }
//                            else
//                            {
//                                appConfigPath = ParseGenericPathToFile(unquoted, diagnostics, baseDirectory);
//                            }
//                            continue;

//                        case "runtimemetadataversion":
//                            unquoted = RemoveQuotesAndSlashes(value);
//                            if (string.IsNullOrEmpty(unquoted))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
//                                continue;
//                            }

//                            runtimeMetadataVersion = unquoted;
//                            continue;

//                        case "ruleset":
//                            // The ruleset arg has already been processed in a separate pass above.
//                            continue;

//                        case "additionalfile":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, "<file list>", name);
//                                continue;
//                            }

//                            additionalFiles.AddRange(ParseSeparatedFileArgument(value, baseDirectory, diagnostics));
//                            continue;

//                        case "embed":
//                            if (string.IsNullOrEmpty(value))
//                            {
//                                embedAllSourceFiles = true;
//                                continue;
//                            }

//                            embeddedFiles.AddRange(ParseSeparatedFileArgument(value, baseDirectory, diagnostics));
//                            continue;
//                    }
//                }

//                AddDiagnostic(diagnostics, ErrorCode.ERR_BadSwitch, arg);
//            }

//            foreach (var o in warnAsErrors)
//            {
//                diagnosticOptions[o.Key] = o.Value;
//            }

//            // Specific nowarn options always override specific warnaserror options.
//            foreach (var o in noWarns)
//            {
//                diagnosticOptions[o.Key] = o.Value;
//            }

//            if (refOnly && outputRefFilePath != null)
//            {
//                AddDiagnostic(diagnostics, diagnosticOptions, ErrorCode.ERR_NoRefOutWhenRefOnly);
//            }

//            if (outputKind == OutputKind.NetModule && (refOnly || outputRefFilePath != null))
//            {
//                AddDiagnostic(diagnostics, diagnosticOptions, ErrorCode.ERR_NoNetModuleOutputWhenRefOutOrRefOnly);
//            }

//            if (!IsScriptCommandLineParser && !sourceFilesSpecified && (outputKind.IsNetModule() || !resourcesOrModulesSpecified))
//            {
//                AddDiagnostic(diagnostics, diagnosticOptions, ErrorCode.WRN_NoSources);
//            }

//            if (!noStdLib && sdkDirectory != null)
//            {
//                metadataReferences.Insert(0, new CommandLineReference(Path.Combine(sdkDirectory, "mscorlib.dll"), MetadataReferenceProperties.Assembly));
//            }

//            if (!platform.Requires64Bit())
//            {
//                if (baseAddress > uint.MaxValue - 0x8000)
//                {
//                    AddDiagnostic(diagnostics, ErrorCode.ERR_BadBaseNumber, string.Format("0x{0:X}", baseAddress));
//                    baseAddress = 0;
//                }
//            }

//            // add additional reference paths if specified
//            if (!string.IsNullOrWhiteSpace(additionalReferenceDirectories))
//            {
//                ParseAndResolveReferencePaths(null, additionalReferenceDirectories, baseDirectory, libPaths, MessageID.IDS_LIB_ENV, diagnostics);
//            }

//            ImmutableArray<string> referencePaths = BuildSearchPaths(sdkDirectory, libPaths, responsePaths);

//            ValidateWin32Settings(win32ResourceFile, win32IconFile, win32ManifestFile, outputKind, diagnostics);

//            // Dev11 searches for the key file in the current directory and assembly output directory.
//            // We always look to base directory and then examine the search paths.
//            keyFileSearchPaths.Add(baseDirectory);
//            if (baseDirectory != outputDirectory)
//            {
//                keyFileSearchPaths.Add(outputDirectory);
//            }

//            // Public sign doesn't use the legacy search path settings
//            if (publicSign && !string.IsNullOrWhiteSpace(keyFileSetting))
//            {
//                keyFileSetting = ParseGenericPathToFile(keyFileSetting, diagnostics, baseDirectory);
//            }

//            if (sourceLink != null && !emitPdb)
//            {
//                AddDiagnostic(diagnostics, ErrorCode.ERR_SourceLinkRequiresPdb);
//            }

//            if (embedAllSourceFiles)
//            {
//                embeddedFiles.AddRange(sourceFiles);
//            }

//            if (embeddedFiles.Count > 0 && !emitPdb)
//            {
//                AddDiagnostic(diagnostics, ErrorCode.ERR_CannotEmbedWithoutPdb);
//            }

//            var parsedFeatures = ParseFeatures(features);

//            string compilationName;
//            GetCompilationAndModuleNames(diagnostics, outputKind, sourceFiles, sourceFilesSpecified, moduleAssemblyName, ref outputFileName, ref moduleName, out compilationName);

//            var parseOptions = new CSharpParseOptions
//            (
//                languageVersion: languageVersion,
//                preprocessorSymbols: defines.ToImmutableAndFree(),
//                documentationMode: parseDocumentationComments ? DocumentationMode.Diagnose : DocumentationMode.None,
//                kind: IsScriptCommandLineParser ? SourceCodeKind.Script : SourceCodeKind.Regular,
//                features: parsedFeatures
//            );

//            // We want to report diagnostics with source suppression in the error log file.
//            // However, these diagnostics won't be reported on the command line.
//            var reportSuppressedDiagnostics = errorLogPath != null;

//            var options = new CSharpCompilationOptions
//            (
//                outputKind: outputKind,
//                moduleName: moduleName,
//                mainTypeName: mainTypeName,
//                scriptClassName: WellKnownMemberNames.DefaultScriptClassName,
//                usings: usings,
//                optimizationLevel: optimize ? OptimizationLevel.Release : OptimizationLevel.Debug,
//                checkOverflow: checkOverflow,
//                allowUnsafe: allowUnsafe,
//                deterministic: deterministic,
//                concurrentBuild: concurrentBuild,
//                cryptoKeyContainer: keyContainerSetting,
//                cryptoKeyFile: keyFileSetting,
//                delaySign: delaySignSetting,
//                platform: platform,
//                generalDiagnosticOption: generalDiagnosticOption,
//                warningLevel: warningLevel,
//                specificDiagnosticOptions: diagnosticOptions,
//                reportSuppressedDiagnostics: reportSuppressedDiagnostics,
//                publicSign: publicSign
//            );

//            if (debugPlus)
//            {
//                options = options.WithDebugPlusMode(debugPlus);
//            }

//            var emitOptions = new EmitOptions
//            (
//                metadataOnly: refOnly,
//                includePrivateMembers: !refOnly && outputRefFilePath == null,
//                debugInformationFormat: debugInformationFormat,
//                pdbFilePath: null, // to be determined later
//                outputNameOverride: null, // to be determined later
//                baseAddress: baseAddress,
//                highEntropyVirtualAddressSpace: highEntropyVA,
//                fileAlignment: fileAlignment,
//                subsystemVersion: subsystemVersion,
//                runtimeMetadataVersion: runtimeMetadataVersion,
//                instrumentationKinds: instrumentationKinds.ToImmutableAndFree(),
//                // TODO: set from /checksumalgorithm (see https://github.com/dotnet/roslyn/issues/24735)
//                pdbChecksumAlgorithm: HashAlgorithmName.SHA256
//            );

//            // add option incompatibility errors if any
//            diagnostics.AddRange(options.Errors);
//            diagnostics.AddRange(parseOptions.Errors);

            return new CSharpCommandLineArguments
            {
                IsScriptRunner = IsScriptCommandLineParser,
                InteractiveMode = interactiveMode || IsScriptCommandLineParser && sourceFiles.Count == 0,
                BaseDirectory = baseDirectory,
                // [Lua] PathMap = pathMap,
                // [Lua] Errors = diagnostics.AsImmutable(),
                Utf8Output = utf8output,
                // [Lua] CompilationName = compilationName,
                OutputFileName = outputFileName,
                OutputRefFilePath = outputRefFilePath,
                PdbPath = pdbPath,
                EmitPdb = emitPdb && !refOnly, // silently ignore emitPdb when refOnly is set
                SourceLink = sourceLink,
                RuleSetPath = ruleSetPath,
                OutputDirectory = outputDirectory,
                DocumentationPath = documentationPath,
                ErrorLogPath = errorLogPath,
                AppConfigPath = appConfigPath,
                // [Lua] SourceFiles = sourceFiles.AsImmutable(),
                Encoding = codepage,
                // [Lua] ChecksumAlgorithm = checksumAlgorithm,
                // [Lua] MetadataReferences = metadataReferences.AsImmutable(),
                // [Lua] AnalyzerReferences = analyzers.AsImmutable(),
                // [Lua] AdditionalFiles = additionalFiles.AsImmutable(),
                // [Lua] ReferencePaths = referencePaths,
                // [Lua] SourcePaths = sourcePaths.AsImmutable(),
                // [Lua] KeyFileSearchPaths = keyFileSearchPaths.AsImmutable(),
                Win32ResourceFile = win32ResourceFile,
                Win32Icon = win32IconFile,
                Win32Manifest = win32ManifestFile,
                NoWin32Manifest = noWin32Manifest,
                DisplayLogo = displayLogo,
                DisplayHelp = displayHelp,
                DisplayVersion = displayVersion,
                DisplayLangVersions = displayLangVersions,
                // [Lua] ManifestResources = managedResources.AsImmutable(),
                // [Lua] CompilationOptions = options,
                // [Lua] ParseOptions = parseOptions,
                // [Lua] EmitOptions = emitOptions,
                // [Lua] ScriptArguments = scriptArgs.AsImmutableOrEmpty(),
                TouchedFilesPath = touchedFilesPath,
                PrintFullPaths = printFullPaths,
                ShouldIncludeErrorEndLocation = errorEndLocation,
                // [Lua] PreferredUILang = preferredUILang,
                ReportAnalyzer = reportAnalyzer,
                // [Lua] EmbeddedFiles = embeddedFiles.AsImmutable()
            };
        }

        internal sealed override CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory, string sdkDirectoryOpt, string additionalReferenceDirectories)
        {
            return Parse(args, baseDirectory, sdkDirectoryOpt, additionalReferenceDirectories);
        }

        internal override void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> errors)
        {
            throw new NotImplementedException();
        }
    }
}