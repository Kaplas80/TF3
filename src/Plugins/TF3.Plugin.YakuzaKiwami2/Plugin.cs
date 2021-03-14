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
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

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
        public string Game => "Yakuza Kiwami 2 English (Steam version, build 4058515)";

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

            using Node dbPar = NodeFactory.FromFile(dbParPath, FileOpenMode.Read);
            _ = dbPar.TransformWith<YarhlPlugin.YakuzaCommon.Converters.Par.Reader>();

            foreach (Node node in Navigator.IterateNodes(dbPar))
            {
                if (node.IsContainer)
                {
                    continue;
                }

                var scanningArgs = new FileScanningEventArgs { FileName = Path.Combine(dbParPath, node.Path) };
                var scannedArgs = new FileScannedEventArgs { FileName = Path.Combine(dbParPath, node.Path), Included = false };
                OnFileScanning(scanningArgs);

                if (!scanningArgs.Cancel)
                {
                    try
                    {
                        if (node.Tags["Flags"] == 0x80000000)
                        {
                            // File is compressed
                            _ = node.TransformWith<YarhlPlugin.YakuzaCommon.Converters.Sllz.Decompress>();
                        }

                        ulong checksum = Common.Core.Helpers.ChecksumHelper.Calculate(node.Stream);

                        _ = node.TransformWith<YarhlPlugin.YakuzaKiwami2.Converters.Armp.Reader>();

                        _ = node.TransformWith<YarhlPlugin.YakuzaKiwami2.Converters.Armp.PoWriter>();
                        if (node.Children.Count > 0)
                        {
                            var file = new Common.Core.Models.File
                            {
                                Name = node.Name,
                                ContainerPath = dbParPath,
                                ContainerType = ContainerType.Archive,
                                RelativePath = node.Path,
                                Contents = ContentType.Text,
                                CheckSum = checksum,
                            };

                            project.AddFile(file);

                            scannedArgs.Included = true;
                        }
                    }
                    catch (Exception)
                    {
                        // File is not an ARMP
                    }
                }

                OnFileScanned(scannedArgs);
            }
        }

        /// <inheritdoc/>
        public void ExtractTexts(TranslationProject project, string outputPath)
        {
            Directory.CreateDirectory(outputPath);

            string lastContainer = string.Empty;
            Node containerNode = null;
            foreach (Common.Core.Models.File file in project.TextFiles)
            {
                if (file.ContainerPath != lastContainer)
                {
                    containerNode?.Dispose();
                    switch (file.ContainerType)
                    {
                        case ContainerType.Directory:
                            containerNode = NodeFactory.FromDirectory(file.ContainerPath, "*", FileOpenMode.Read);
                            break;
                        case ContainerType.Archive:
                            containerNode = NodeFactory.FromFile(file.ContainerPath, FileOpenMode.Read);
                            _ = containerNode.TransformWith<YarhlPlugin.YakuzaCommon.Converters.Par.Reader>();
                            break;
                    }
                }

                Node node = Navigator.SearchNode(containerNode, file.RelativePath);

                if (node == null)
                {
                    continue;
                }

                if (node.Tags.ContainsKey("Flags") && node.Tags["Flags"] == 0x80000000)
                {
                    _ = node.TransformWith<YarhlPlugin.YakuzaCommon.Converters.Sllz.Decompress>();
                }

                _ = node.TransformWith<YarhlPlugin.YakuzaKiwami2.Converters.Armp.Reader>();
                _ = node.TransformWith<YarhlPlugin.YakuzaKiwami2.Converters.Armp.PoWriter>();

                foreach (Node poNode in Navigator.IterateNodes(node))
                {
                    _ = poNode.TransformWith<Po2Binary>();
                    BinaryFormat format = poNode.GetFormatAs<BinaryFormat>();
                    if (format != null)
                    {
                        format.Stream.WriteTo(Path.Combine(outputPath, $"{node.Parent.Name}-{node.Name}-{poNode.Name}.po"));
                    }
                }

                lastContainer = file.ContainerPath;
            }

            containerNode?.Dispose();
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
