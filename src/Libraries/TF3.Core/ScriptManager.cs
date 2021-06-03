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

namespace TF3.Core
{
    using System.Collections.Generic;
    using System.IO;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl;

    /// <summary>
    /// Script manager.
    /// </summary>
    public static class ScriptManager
    {
        private static List<GameScript> _scripts = new ();

        /// <summary>
        /// Gets a list of loaded scripts.
        /// </summary>
        public static IReadOnlyList<GameScript> Scripts => _scripts.AsReadOnly();

        /// <summary>
        /// Loads all the scripts in a directory.
        /// </summary>
        /// <param name="path">The directory containing the scripts.</param>
        public static void LoadScripts(string path)
        {
            // This is needed to load all the Yarhl plugins and make their types available in scripts.
            _ = PluginManager.Instance;

            _scripts.Clear();
            IDeserializer deserializer = new DeserializerBuilder()
                                             .WithTypeConverter(new Yaml.ParameterInfoTypeConverter())
                                             .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                             .Build();
            foreach (string file in Directory.EnumerateFiles(path, "TF3.Script.*.yaml"))
            {
                string scriptContents = File.ReadAllText(file);
                GameScript script = deserializer.Deserialize<GameScript>(scriptContents);
                _scripts.Add(script);
            }
        }
    }
}
