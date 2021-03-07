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
    using TF3.Common.Core.Models;

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
            PluginManager.LoadPlugins("plugins");

            Parser.Default.ParseArguments<Options.ListPluginsOptions, Options.NewProjectOptions, Options.ScanFilesOptions>(args)
                .WithParsed<Options.ListPluginsOptions>(_ => ListPlugins())
                .WithParsed<Options.NewProjectOptions>(options => NewProject(options))
                .WithParsed<Options.ScanFilesOptions>(options => ScanFiles(options));
        }

        private static void ListPlugins()
        {
            int maxNameLength = PluginManager.Plugins.Max<IPlugin>(x => x.Name.Length);
            int maxGameLength = PluginManager.Plugins.Max<IPlugin>(x => x.Game.Length);
            Console.WriteLine("Available plugins:");
            Console.WriteLine();
            Console.WriteLine(string.Format($"{{0, {-maxNameLength}}}\t{{1, {-maxGameLength}}}", "Name", "Game"));
            Console.WriteLine($"{new string('=', maxNameLength)}\t{new string('=', maxGameLength)}");
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                Console.WriteLine(string.Format($"{{0, {-maxNameLength}}}\t{{1, {-maxGameLength}}}", plugin.Name, plugin.Game));
            }
        }

        private static void NewProject(Options.NewProjectOptions options)
        {
            IPlugin plugin = PluginManager.Plugins.FirstOrDefault(x => x.Id == options.Game || x.Name == options.Game);

            if (plugin is null)
            {
                Console.WriteLine($"ERROR: There is no plugin available for {options.Game}");
                ListPlugins();
                return;
            }

            using var project = TranslationProject.New(options.Output, plugin.Id, !string.IsNullOrEmpty(options.Language) ? options.Language : "und");
        }

        private static void ScanFiles(Options.ScanFilesOptions options)
        {
            using var project = TranslationProject.Open(options.Project);

            ProjectInfo info = project.Info;

            if (info is null)
            {
                Console.WriteLine($"ERROR: No project info found.");
                return;
            }

            IPlugin plugin = PluginManager.Plugins.FirstOrDefault(x => x.Id == info.PluginId);

            if (plugin is null)
            {
                Console.WriteLine($"ERROR: Plugin not available.");
                return;
            }

            plugin.FileScanning += (sender, args) => Console.Write(args.FileName);
            plugin.FileScanned += (sender, args) => Console.WriteLine(args.Included ? " YES" : " NO");
            plugin.Scan(project, options.Game);
        }
    }
}
