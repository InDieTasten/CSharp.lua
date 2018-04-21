/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpLua
{
    public sealed class Compiler
    {
        private static readonly string[] SystemDlls = new string[] {
            "System.dll",
            "System.Core.dll",
            "System.Runtime.dll",
            "System.Linq.dll",
            "Microsoft.CSharp.dll",
        };
        private const string _dllFileExtension = ".dll";
        private const string _systemMetadata = "~/System.xml";

        private string _inputDirectory;
        private string _outputDirectory;
        private string[] _libraries;
        private string[] _metas;
        private string[] _cscArguments;
        private bool _isNewest;
        private int _indentCount;
        private bool _useSemicolons;
        private string[] _attributes;

        public Compiler(string inputDirectory, string outputDirectory, string libraries, string metas, string cscArguments, bool isClassic, string indentCount, string attributes)
        {
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
            _libraries = Utility.Split(libraries);
            _metas = Utility.Split(metas);
            _cscArguments = string.IsNullOrEmpty(cscArguments) ? Array.Empty<string>() : cscArguments.Trim().Split(';', ',', ' ');
            _isNewest = !isClassic;
            _useSemicolons = false;
            int.TryParse(indentCount, out _indentCount);
            if (attributes != null)
            {
                _attributes = Utility.Split(attributes, false);
            }
        }

        private IEnumerable<string> GetMetas()
        {
            List<string> metas = new List<string>() { Utility.GetCurrentDirectory(_systemMetadata) };
            metas.AddRange(_metas);
            return metas;
        }

        private bool IsCorrectSystemDll(string path)
        {
            try
            {
                Assembly.LoadFile(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private IEnumerable<string> GetLibraries()
        {
            string privateCorePath = typeof(object).Assembly.Location;
            List<string> libraries = new List<string>() { privateCorePath };

            string systemDirectory = Path.GetDirectoryName(privateCorePath);
            foreach (string path in Directory.EnumerateFiles(systemDirectory, "*.dll"))
            {
                if (IsCorrectSystemDll(path))
                {
                    libraries.Add(path);
                }
            }

            foreach (string library in _libraries)
            {
                string path = library.EndsWith(_dllFileExtension) ? library : library + _dllFileExtension;
                if (File.Exists(path))
                {
                    libraries.Add(path);
                }
                else
                {
                    string file = Path.Combine(systemDirectory, Path.GetFileName(path));
                    if (!File.Exists(file))
                    {
                        throw new CmdArgumentException($"-l {path} couldn't be found.");
                    }
                }
            }
            return libraries;
        }

        public void Do()
        {
            Compiler();
        }

        /// <summary>
        /// Compiles .cs files to CS syntax tree, generates Lua syntax tree, and generates .lua files
        /// </summary>
        private void Compiler()
        {
            // Apply configurations
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(_cscArguments, null, null);
            var parseOptions = commandLineArguments.ParseOptions.WithDocumentationMode(DocumentationMode.Parse);
            var files = Directory.EnumerateFiles(_inputDirectory, "*.cs", SearchOption.AllDirectories);
            var generatorSettings = new LuaSyntaxGenerator.SettingInfo()
            {
                IsNewest = _isNewest,
                HasSemicolon = _useSemicolons,
                Indent = _indentCount,
            };

            // Build CS syntax trees
            var syntaxTrees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), parseOptions, file));
            var references = GetLibraries().Select(i => MetadataReference.CreateFromFile(i));

            // Generate lua output
            LuaSyntaxGenerator generator = new LuaSyntaxGenerator(
                syntaxTrees,
                references,
                commandLineArguments.CompilationOptions,
                GetMetas(),
                generatorSettings,
                _attributes,
                _inputDirectory);
            generator.Generate(_outputDirectory);
        }
    }
}
