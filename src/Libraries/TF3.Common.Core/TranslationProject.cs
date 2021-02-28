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
    using System.IO;

    /// <summary>
    /// Represents a TF3 translation project.
    /// </summary>
    public class TranslationProject : IDisposable
    {
        private static TranslationProject _instance;
        private LiteDB.LiteDatabase _database;
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationProject"/> class.
        /// </summary>
        private TranslationProject()
        {
        }

        /// <summary>
        /// Creates a new <see cref="TranslationProject"/>.
        /// </summary>
        /// <param name="path">Path to save the project.</param>
        /// <param name="pluginId">Id of the used plugin.</param>
        /// <param name="lang">Translation language.</param>
        /// <returns>The new project.</returns>
        public static TranslationProject New(string path, string pluginId, string lang = "und")
        {
            _instance = new TranslationProject();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string dbPath = Path.Combine(path, "project.tf3");

            _instance._database = new LiteDB.LiteDatabase(dbPath);
            LiteDB.ILiteCollection<POCO.ProjectInfo> col = _instance._database.GetCollection<POCO.ProjectInfo>("info");
            col.Insert(new POCO.ProjectInfo { PluginId = pluginId, Language = lang });

            return _instance;
        }

        /// <summary>
        /// Opens a existing <see cref="TranslationProject"/>.
        /// </summary>
        /// <param name="dbPath">Path of the project.</param>
        /// <returns>The opened project.</returns>
        public static TranslationProject Open(string dbPath)
        {
            _instance = new TranslationProject();
            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException($"{dbPath} not found.");
            }

            _instance._database = new LiteDB.LiteDatabase(dbPath);

            return _instance;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose allocated resources.
        /// </summary>
        /// <param name="disposing">Free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _database.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
