// Copyright (c) 2021 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace TF3.CommandLine
{
    using System;
    using System.Linq;
    using global::CommandLine;
    using TF3.Common.Core;

    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry-point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            ScriptManager.LoadScripts("scripts");

            Parser.Default.ParseArguments<Options.ListScriptsOptions, Options.NewProjectOptions, Options.ScanFilesOptions, Options.ExtractOptions>(args)
                .WithParsed<Options.ListScriptsOptions>(ListScripts)
                .WithParsed<Options.NewProjectOptions>(NewProject)
                .WithParsed<Options.ScanFilesOptions>(ScanFiles)
                .WithParsed<Options.ExtractOptions>(Extract);
        }

        private static void ListScripts(Options.ListScriptsOptions options)
        {
            int maxNameLength = ScriptManager.Scripts.Max(x => x.Name.Length);
            int maxGameLength = ScriptManager.Scripts.Max(x => x.Game.Length);
            Console.WriteLine("Available scripts:");
            Console.WriteLine();
            Console.WriteLine(string.Format($"{{0, {-maxNameLength}}}\t{{1, {-maxGameLength}}}", "Name", "Game"));
            Console.WriteLine($"{new string('=', maxNameLength)}\t{new string('=', maxGameLength)}");
            foreach (GameScript script in ScriptManager.Scripts)
            {
                Console.WriteLine(string.Format($"{{0, {-maxNameLength}}}\t{{1, {-maxGameLength}}}", script.Name, script.Game));
            }
        }

        private static void NewProject(Options.NewProjectOptions options)
        {
        }

        private static void ScanFiles(Options.ScanFilesOptions options)
        {
        }

        private static void Extract(Options.ExtractOptions options)
        {
        }
    }
}
