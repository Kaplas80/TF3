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
                Node asset = NodeFactory.CreateContainer(assetInfo.Id);

                foreach (Models.FileInfo fileInfo in assetInfo.Files)
                {
                    if (!containersDict.TryGetValue(fileInfo.ContainerId, out Node container))
                    {
                        throw new DirectoryNotFoundException($"Container not found: {fileInfo.ContainerId}");
                    }

                    Node file = Navigator.SearchNode(container, fileInfo.Path);

                    if (file == null)
                    {
                        throw new FileNotFoundException($"File not found: {fileInfo.Name}");
                    }

                    if (!Common.Core.Helpers.ChecksumHelper.Check(file.Stream, fileInfo.Checksum))
                    {
                        throw new ChecksumMismatchException($"Checksum mismatch in {fileInfo.Name}");
                    }

                    file.Transform(fileInfo.Readers, this.Parameters);

                    asset.Add(file);
                }

                asset.Transform(assetInfo.Readers, this.Parameters);

                foreach (Node node in asset.Children)
                {
                    node.Tags["OutputName"] = string.Concat(assetInfo.Id, ".po");
                    node.Stream.WriteTo(Path.Combine(outputPath, node.Tags["OutputName"]));
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

                if (!Common.Core.Helpers.ChecksumHelper.Check(node.Stream, containerInfo.Checksum))
                {
                    throw new ChecksumMismatchException($"Checksum mismatch in {containerInfo.Name}");
                }

                node.Transform(containerInfo.Readers, this.Parameters);

                dictionary.Add(containerInfo.Id, node);

                ReadContainers(containerInfo.Containers, node, dictionary);
            }
        }
    }
}
