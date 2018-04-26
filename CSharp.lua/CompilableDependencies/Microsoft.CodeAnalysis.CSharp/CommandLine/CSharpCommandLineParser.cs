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
            List<string> flattenedArgs = new List<string>();

            // Reduced imput parameters
            FlattenArgs(args, null, flattenedArgs, null, baseDirectory, null);

            throw new NotImplementedException();
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