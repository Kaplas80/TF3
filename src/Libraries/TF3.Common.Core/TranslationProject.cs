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

    /// <summary>
    /// Represents a TF3 translation project.
    /// </summary>
    public class TranslationProject : IDisposable
    {
        private LiteDB.LiteDatabase _database;
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationProject"/> class.
        /// </summary>
        private TranslationProject()
        {
        }

        /// <summary>
        /// Gets the project info.
        /// </summary>
        public Models.ProjectInfo Info
        {
            get
            {
                LiteDB.ILiteCollection<Models.ProjectInfo> col = _database.GetCollection<Models.ProjectInfo>("info");

                // There is only one value in the collection, so the predicate isn't important.
                return col.FindOne(x => !string.IsNullOrEmpty(x.PluginId));
            }
        }

        /// <summary>
        /// Gets the text files.
        /// </summary>
        public IList<Models.File> TextFiles
        {
            get
            {
                LiteDB.ILiteCollection<Models.File> col = _database.GetCollection<Models.File>("files");
                col.EnsureIndex(x => x.Contents);

                return col.Query()
                    .Where(x => x.Contents == Enums.ContentType.Text)
                    .OrderBy(x => x.ContainerPath)
                    .ToList();
            }
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
            var project = new TranslationProject();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string dbPath = Path.Combine(path, "project.tf3");

            project._database = new LiteDB.LiteDatabase(dbPath);
            LiteDB.ILiteCollection<Models.ProjectInfo> col = project._database.GetCollection<Models.ProjectInfo>("info");
            col.Insert(new Models.ProjectInfo { PluginId = pluginId, Language = lang });

            return project;
        }

        /// <summary>
        /// Opens a existing <see cref="TranslationProject"/>.
        /// </summary>
        /// <param name="dbPath">Path of the project.</param>
        /// <returns>The opened project.</returns>
        public static TranslationProject Open(string dbPath)
        {
            var project = new TranslationProject();
            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException($"{dbPath} not found.");
            }

            project._database = new LiteDB.LiteDatabase(dbPath);

            return project;
        }

        /// <summary>
        /// Add a file to the project.
        /// </summary>
        /// <param name="file">The file info.</param>
        public void AddFile(Models.File file)
        {
            LiteDB.ILiteCollection<Models.File> col = _database.GetCollection<Models.File>("files");
            col.Insert(file);
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
