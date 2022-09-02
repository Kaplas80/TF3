// Copyright (c) 2022 Kaplas
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using Dahomey.Json;
    using Yarhl;

    /// <summary>
    /// Script manager.
    /// </summary>
    public static class ScriptManager
    {
        private static readonly List<GameScript> _scripts = new List<GameScript>();

        /// <summary>
        /// Event triggered on errors.
        /// </summary>
        public static event EventHandler<(string file, string message)> ErrorLoading;

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

            Clear();

            JsonSerializerOptions options = new JsonSerializerOptions().SetupExtensions();
            options.SetMissingMemberHandling(MissingMemberHandling.Error);

            foreach (string file in Directory.EnumerateFiles(path, "TF3.Script.*.json"))
            {
                try
                {
                    string scriptContents = File.ReadAllText(file);
                    GameScript script = JsonSerializer.Deserialize<GameScript>(scriptContents, options);
                    _scripts.Add(script);
                }
                catch (Exception e)
                {
                    ErrorLoading?.Invoke(null, (file, e.Message));
                }
            }
        }

        /// <summary>
        /// Empties the loaded scripts.
        /// </summary>
        public static void Clear()
        {
            _scripts.Clear();
        }
    }
}
