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
    using TF3.Common.Core.Exceptions;
    using TF3.Common.Core.Helpers;
    using TF3.Common.Core.Models;
    using Yarhl.FileSystem;

    /// <summary>
    /// Game script. Includes all the needed info to extract and repack game files.
    /// </summary>
    public class GameScript
    {
        /// <summary>
        /// Gets or sets the script name (short name).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the script game name.
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Gets or sets the list of containers in the game.
        /// </summary>
        public List<ContainerInfo> Containers { get; set; }

        /// <summary>
        /// Gets or sets the list of translatable assets.
        /// </summary>
        public List<AssetInfo> Assets { get; set; }

        /// <summary>
        /// Gets or sets the list of parameters.
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        /// Extracts all translatable assets from the game.
        /// </summary>
        /// <param name="gamePath">Game install directory.</param>
        /// <param name="outputPath">Output directory.</param>
        public void ExtractAssets(string gamePath, string outputPath)
        {
            Directory.CreateDirectory(outputPath);

            var containersDict = new Dictionary<string, Node>();

            Node gameRoot = NodeFactory.FromDirectory(gamePath, "*", "root", true, Yarhl.IO.FileOpenMode.Read);
            containersDict.Add("root", gameRoot);

            ReadContainers(Containers, gameRoot, containersDict);

            foreach (AssetInfo assetInfo in Assets)
            {
                ExtractAsset(assetInfo, containersDict, outputPath);
            }
        }

        /// <summary>
        /// Rebuild all translatable assets from the game.
        /// </summary>
        /// <param name="gamePath">Game install directory.</param>
        /// <param name="translationPath">Translation directory.</param>
        /// <param name="outputPath">Output directory.</param>
        public void RebuildAssets(string gamePath, string translationPath, string outputPath)
        {
            Directory.CreateDirectory(outputPath);

            var containersDict = new Dictionary<string, Node>();
            var containersDict2 = new Dictionary<string, Node>();

            Node gameRoot = NodeFactory.FromDirectory(gamePath, "*", "root", true, Yarhl.IO.FileOpenMode.Read);
            Node gameRoot2 = NodeFactory.FromDirectory(gamePath, "*", "root", true, Yarhl.IO.FileOpenMode.Read);
            containersDict.Add("root", gameRoot);
            containersDict2.Add("root", gameRoot2);

            ReadContainers(Containers, gameRoot, containersDict);
            ReadContainers(Containers, gameRoot2, containersDict2);

            foreach (AssetInfo assetInfo in Assets)
            {
                Node asset = ReadAsset(assetInfo, containersDict); // This removes the nodes from their original parents. That's why there is a containersDict2.
                Node translation = ReadTranslation(assetInfo, translationPath);

                asset.Translate(translation, assetInfo.Translator);

                asset.Transform(assetInfo.Splitters, Parameters);

                foreach (Models.FileInfo fileInfo in assetInfo.Files)
                {
                    if (!containersDict2.TryGetValue(fileInfo.ContainerId, out Node container))
                    {
                        throw new DirectoryNotFoundException($"Container not found: {fileInfo.ContainerId}");
                    }

                    Node newFile;
                    if (asset.Children[0].Name == "single_node")
                    {
                        newFile = asset.Children[0];
                    }
                    else
                    {
                        newFile = Navigator.SearchNode(asset, fileInfo.Name);
                    }

                    newFile.Transform(fileInfo.Writers, Parameters);

                    Node originalFile = Navigator.SearchNode(container, fileInfo.Path);

                    originalFile.ChangeFormat(newFile.Format);
                    originalFile.Tags["Changed"] = true;
                    originalFile.Tags["OutputPath"] = fileInfo.Path;
                }
            }

            WriteContainers(Containers, gameRoot2);

            foreach (Node node in Navigator.IterateNodes(gameRoot2))
            {
                if (node.Tags.ContainsKey("Changed"))
                {
                    node.Stream.WriteTo(Path.Combine(outputPath, node.Tags["OutputPath"]));
                }
            }
        }

        private void ReadContainers(IList<ContainerInfo> containers, Node parent, Dictionary<string, Node> dictionary)
        {
            foreach (ContainerInfo containerInfo in containers)
            {
                Node node = Navigator.SearchNode(parent, containerInfo.Path);

                if (node == null)
                {
                    throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Path}");
                }

                if (!ChecksumHelper.Check(node.Stream, containerInfo.Checksum))
                {
                    throw new ChecksumMismatchException($"Checksum mismatch in {containerInfo.Name}");
                }

                node.Transform(containerInfo.Readers, Parameters);

                dictionary.Add(containerInfo.Id, node);

                ReadContainers(containerInfo.Containers, node, dictionary);
            }
        }

        private void WriteContainers(List<ContainerInfo> containers, Node parent)
        {
            foreach (ContainerInfo containerInfo in containers)
            {
                Node node = Navigator.SearchNode(parent, containerInfo.Path);

                if (node == null)
                {
                    throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Path}");
                }

                WriteContainers(containerInfo.Containers, node);

                node.Transform(containerInfo.Writers, Parameters);
                node.Tags["Changed"] = true;
                node.Tags["OutputPath"] = containerInfo.Path;
            }
        }

        private void ExtractAsset(AssetInfo assetInfo, Dictionary<string, Node> containers, string outputPath)
        {
            Node asset = ReadAsset(assetInfo, containers);

            asset.Transform(assetInfo.Extractors, Parameters);

            if (asset.IsContainer)
            {
                int outputIndex = 0;
                foreach (Node node in Navigator.IterateNodes(asset))
                {
                    if (node.IsContainer)
                    {
                        continue;
                    }

                    node.Stream.WriteTo(Path.Combine(outputPath, assetInfo.OutputNames[outputIndex]));
                    outputIndex++;
                }
            }
            else if (asset.Stream != null)
            {
                asset.Stream.WriteTo(Path.Combine(outputPath, assetInfo.OutputNames[0]));
            }
            else
            {
                throw new FormatException("Can't extract asset.");
            }
        }

        private Node ReadAsset(AssetInfo assetInfo, Dictionary<string, Node> containers)
        {
            Node asset = NodeFactory.CreateContainer(assetInfo.Id);

            foreach (Models.FileInfo fileInfo in assetInfo.Files)
            {
                if (!containers.TryGetValue(fileInfo.ContainerId, out Node container))
                {
                    throw new DirectoryNotFoundException($"Container not found: {fileInfo.ContainerId}");
                }

                Node file = Navigator.SearchNode(container, fileInfo.Path);

                if (file == null)
                {
                    throw new FileNotFoundException($"File not found: {fileInfo.Name}");
                }

                if (!ChecksumHelper.Check(file.Stream, fileInfo.Checksum))
                {
                    throw new ChecksumMismatchException($"Checksum mismatch in {fileInfo.Name}");
                }

                file.Transform(fileInfo.Readers, Parameters);
                asset.Add(file);
            }

            asset.Transform(assetInfo.Mergers, Parameters);
            return asset;
        }

        private Node ReadTranslation(AssetInfo assetInfo, string translationPath)
        {
            Node translation = NodeFactory.CreateContainer(string.Concat(assetInfo.Id, "_Translation"));

            foreach (string outputFile in assetInfo.OutputNames)
            {
                string path = Path.Combine(translationPath, outputFile);

                Node file = NodeFactory.FromFile(path, Yarhl.IO.FileOpenMode.Read);

                translation.Add(file);
            }

            translation.Transform(assetInfo.TranslationMergers, Parameters);
            return translation;
        }
    }
}
