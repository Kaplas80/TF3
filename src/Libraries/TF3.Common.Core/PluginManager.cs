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

namespace TF3.Common.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Plugin manager.
    /// </summary>
    public static class PluginManager
    {
        private static List<IPlugin> _plugins = new List<IPlugin>();

        /// <summary>
        /// Gets a list of loaded plugins.
        /// </summary>
        public static IReadOnlyList<IPlugin> Plugins => _plugins.AsReadOnly();

        /// <summary>
        /// Loads all the plugins in a directory.
        /// </summary>
        /// <param name="path">The directory containing the plugins.</param>
        public static void LoadPlugins(string path)
        {
            _plugins.Clear();
            foreach (string plugin in Directory.EnumerateFiles(path, "TF3.Plugin.*.dll"))
            {
                PluginLoadContext loadContext = new PluginLoadContext(plugin);
                Assembly assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(plugin)));
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type) && Activator.CreateInstance(type) is IPlugin result)
                    {
                        _plugins.Add(result);
                    }
                }
            }
        }
    }
}
