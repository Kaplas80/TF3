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

namespace TF3.Plugin.YakuzaKiwami2
{
    using System;
    using System.IO;
    using TF3.Common.Core;
    using TF3.Common.Core.Enums;
    using TF3.Common.Core.EventArgs;
    using Yarhl.FileSystem;

    /// <summary>
    /// Yakuza Kiwami 2 plugin.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <inheritdoc/>
        public event EventHandler<FileScanningEventArgs> FileScanning;

        /// <inheritdoc/>
        public event EventHandler<FileScannedEventArgs> FileScanned;

        /// <inheritdoc/>
        public string Id => "ef6d1df4-dacc-4d3a-aa77-8e0f25bb21b4";

        /// <inheritdoc/>
        public string Game => "Yakuza Kiwami 2 (Steam version, build 4058515)";

        /// <inheritdoc/>
        public string Name => "kiwami2pc";

        /// <inheritdoc/>
        public Platform Platform => Platform.Any;

        /// <inheritdoc/>
        public void Scan(TranslationProject project, string gameDir)
        {
            // Scan db.par
            string dbParPath = Path.Combine(gameDir, "data", "db.par");
            if (!File.Exists(dbParPath))
            {
                throw new FileNotFoundException("\"db.par\" not found.");
            }

            using Node dbPar = NodeFactory.FromFile(dbParPath, Yarhl.IO.FileOpenMode.Read);
            _ = dbPar.TransformWith<Common.Yakuza.Converters.Par.Reader>();

            foreach (Node n in Navigator.IterateNodes(dbPar))
            {
                if (n.IsContainer)
                {
                    continue;
                }

                var scanningArgs = new FileScanningEventArgs { FileName = Path.Combine(dbParPath, n.Path) };
                var scannedArgs = new FileScannedEventArgs { FileName = Path.Combine(dbParPath, n.Path) };
                OnFileScanning(scanningArgs);

                if (scanningArgs.Cancel)
                {
                    scannedArgs.Included = false;
                }
                else
                {
                    try
                    {
                        ulong checksum = Common.Core.Helpers.ChecksumHelper.Calculate(n.Stream);

                        if (n.Tags["Flags"] == 0x80000000)
                        {
                            _ = n.TransformWith<Common.Yakuza.Converters.Sllz.Decompress>();
                        }

                        _ = n.TransformWith<Converters.Armp.Reader>();

                        if (n.GetFormatAs<Formats.ArmpTable>().ValueStringCount > 0)
                        {
                            var file = new TF3.Common.Core.Models.File
                            {
                                Name = n.Name,
                                ContainerPath = dbParPath,
                                ContainerType = ContainerType.Archive,
                                RelativePath = n.Path,
                                Contents = ContentType.Text,
                                CheckSum = checksum,
                            };

                            project.AddFile(file);

                            scannedArgs.Included = true;
                        }
                        else
                        {
                            scannedArgs.Included = false;
                        }
                    }
                    catch (Exception)
                    {
                        scannedArgs.Included = false;
                    }
                }

                OnFileScanned(scannedArgs);
            }
        }

        /// <inheritdoc/>
        public void ExtractTexts(string installationPath, string outputPath)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Invokes FileScanning event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnFileScanning(FileScanningEventArgs e)
        {
            FileScanning?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes FileScanned event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnFileScanned(FileScannedEventArgs e)
        {
            FileScanned?.Invoke(this, e);
        }
    }
}
