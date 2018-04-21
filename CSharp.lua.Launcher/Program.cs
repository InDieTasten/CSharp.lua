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

namespace CSharpLua
{
    class Program
    {
        private const string HelpCmdString = @"Usage: CSharp.lua [-s srcfolder] [-d dstfolder]
Arguments 
-s              : source directory, all *.cs files will be compiled
-d              : destination directory, to which the .lua files will be output

Options
-h              : show the help message    
-l              : libraries referenced, use ';' to separate      
-m              : meta files, like System.xml, use ';' to separate     
-csc            : csc.exe command argumnets, use ';' to separate

-c              : support classic lua version(5.1), default support 5.3 
-i              : indent number, default is 2
-a              : attributes need to export, use ';' to separate, if ""-a"" only, all attributes whill be exported    
";
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    var commands = Utility.GetCommondLines(args);
                    if (commands.ContainsKey("-h"))
                    {
                        ShowHelpInfo();
                        return;
                    }

                    Console.WriteLine($"start {DateTime.Now}");

                    string folder = commands.GetArgument("-s");
                    string output = commands.GetArgument("-d");
                    string lib = commands.GetArgument("-l", true);
                    string meta = commands.GetArgument("-m", true);
                    string cscArguments = commands.GetArgument("-csc", true);
                    bool isClassic = commands.ContainsKey("-c");
                    string indentCount = commands.GetArgument("-i", true);
                    string attributes = commands.GetArgument("-a", true);
                    if (attributes == null && commands.ContainsKey("-a"))
                    {
                        attributes = string.Empty;
                    }
                    Worker w = new Worker(folder, output, lib, meta, cscArguments, isClassic, indentCount, attributes);
                    w.Do();
                    Console.WriteLine("all operator success");
                    Console.WriteLine($"end {DateTime.Now}");
                }
                catch (CmdArgumentException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    ShowHelpInfo();
                    Environment.ExitCode = -1;
                }
                catch (CompilationErrorException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Environment.ExitCode = -1;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                    Environment.ExitCode = -1;
                }
            }
            else
            {
                ShowHelpInfo();
                Environment.ExitCode = -1;
            }
        }

        private static void ShowHelpInfo()
        {
            Console.Error.WriteLine(HelpCmdString);
        }
    }
}
