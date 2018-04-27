using System;
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

            foreach (string arg in flattenedArgs)
            {
                //Debug.Assert(optionsEnded || !arg.StartsWith("@", StringComparison.Ordinal));

                string name, value;
                if (optionsEnded || !TryParseOption(arg, out name, out value))
                {
                    sourceFiles.AddRange(ParseFileArgument(arg, baseDirectory, diagnostics));
                    if (sourceFiles.Count > 0)
                    {
                        sourceFilesSpecified = true;
                    }

                    continue;
                }

                switch (name)
                {
                    case "?":
                    case "help":
                    case "version":
                    case "r":
                    case "reference":
                    case "features":
                    case "lib":
                    case "libpath":
                    case "libpaths":
#if DEBUG
                    case "attachdebugger":
#endif
                        throw new NotImplementedException(); // [Lua]
                }

                if (IsScriptCommandLineParser)
                {
                    throw new NotImplementedException(); // [Lua]
                }
                else
                {
                    switch (name)
                    {
                        case "a":
                        case "analyzer":
                        case "d":
                        case "define":
                        case "codepage":
                        case "checksumalgorithm":
                        case "checked":
                        case "checked+":
                        case "checked-":
                        case "instrument":
                        case "noconfig":
                        case "sqmsessionguid":
                        case "preferreduilang":
                        case "out":
                        case "refout":
                        case "refonly":
                        case "t":
                        case "target":
                        case "moduleassemblyname":
                        case "modulename":
                        case "platform":
                        case "recurse":
                        case "doc":
                        case "addmodule":
                        case "l":
                        case "link":
                        case "win32res":
                        case "win32icon":
                        case "win32manifest":
                        case "nowin32manifest":
                        case "res":
                        case "resource":
                        case "linkres":
                        case "linkresource":
                        case "sourcelink":
                        case "debug":
                        case "debug+":
                        case "debug-":
                        case "o":
                        case "optimize":
                        case "o+":
                        case "optimize+":
                        case "o-":
                        case "optimize-":
                        case "deterministic":
                        case "deterministic+":
                        case "deterministic-":
                        case "p":
                        case "parallel":
                        case "p+":
                        case "parallel+":
                        case "p-":
                        case "parallel-":
                        case "warnaserror":
                        case "warnaserror+":
                        case "warnaserror-":
                        case "w":
                        case "warn":
                        case "nowarn":
                        case "unsafe":
                        case "unsafe+":
                        case "unsafe-":
                        case "langversion":
                        case "delaysign":
                        case "delaysign+":
                        case "delaysign-":
                        case "publicsign":
                        case "publicsign+":
                        case "publicsign-":
                        case "keyfile":
                        case "keycontainer":
                        case "highentropyva":
                        case "highentropyva+":
                        case "highentropyva-":
                        case "nologo":
                        case "baseaddress":
                        case "subsystemversion":
                        case "touchedfiles":
                        case "bugreport":
                        case "m":
                        case "main":
                        case "fullpaths":
                        case "pathmap":
                        case "filealign":
                        case "pdb":
                        case "errorendlocation":
                        case "reportanalyzer":
                        case "nostdlib":
                        case "nostdlib+":
                        case "nostdlib-":
                        case "errorreport":
                        case "errorlog":
                        case "appconfig":
                        case "runtimemetadataversion":
                        case "ruleset":
                        case "additionalfile":
                        case "embed":
                            throw new NotImplementedException(); // [Lua]
                    }
                }

                // AddDiagnostic(diagnostics, ErrorCode.ERR_BadSwitch, arg);
            }

            throw new NotImplementedException(); // [Lua]
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

            //var parseOptions = new CSharpParseOptions
            //(
            //    languageVersion: languageVersion,
            //    preprocessorSymbols: defines.ToImmutableAndFree(),
            //    documentationMode: parseDocumentationComments ? DocumentationMode.Diagnose : DocumentationMode.None,
            //    kind: IsScriptCommandLineParser ? SourceCodeKind.Script : SourceCodeKind.Regular,
            //    features: parsedFeatures
            //);

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