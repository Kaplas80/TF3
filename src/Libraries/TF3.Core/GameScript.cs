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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using TF3.Core.Converters;
    using TF3.Core.EventArgs;
    using TF3.Core.Exceptions;
    using TF3.Core.Helpers;
    using TF3.Core.Models;
    using Yarhl.FileSystem;

    /// <summary>
    /// Game script. Includes all the needed info to extract and repack game files.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Public methods can not be tested without a real game script.")]
    public class GameScript
    {
        /// <summary>
        /// Event triggered BEFORE extraction begins.
        /// </summary>
        public event EventHandler<ScriptEventArgs> ScriptExtracting;

        /// <summary>
        /// Event triggered AFTER extraction ends.
        /// </summary>
        public event EventHandler<ScriptEventArgs> ScriptExtracted;

        /// <summary>
        /// Event triggered BEFORE rebuilding begins.
        /// </summary>
        public event EventHandler<ScriptEventArgs> ScriptRebuilding;

        /// <summary>
        /// Event triggered AFTER rebuilding ends.
        /// </summary>
        public event EventHandler<ScriptEventArgs> ScriptRebuilt;

        /// <summary>
        /// Event triggered BEFORE a container is read.
        /// </summary>
        public event EventHandler<ContainerEventArgs> ContainerReading;

        /// <summary>
        /// Event triggered AFTER a container is read.
        /// </summary>
        public event EventHandler<ContainerEventArgs> ContainerRead;

        /// <summary>
        /// Event triggered BEFORE a container is written.
        /// </summary>
        public event EventHandler<ContainerEventArgs> ContainerWriting;

        /// <summary>
        /// Event triggered AFTER a container is written.
        /// </summary>
        public event EventHandler<ContainerEventArgs> ContainerWrote;

        /// <summary>
        /// Event triggered BEFORE asset extraction begins.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetExtracting;

        /// <summary>
        /// Event triggered AFTER asset extraction ends.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetExtracted;

        /// <summary>
        /// Event triggered BEFORE an asset is read.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetReading;

        /// <summary>
        /// Event triggered AFTER an asset is read.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetRead;

        /// <summary>
        /// Event triggered BEFORE an asset is translated.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetTranslating;

        /// <summary>
        /// Event triggered AFTER an asset is translated.
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetTranslated;

        /// <summary>
        /// Event triggered if an asset failed to translate (missing files).
        /// </summary>
        public event EventHandler<AssetEventArgs> AssetTranslationFailed;

        /// <summary>
        /// Event triggered BEFORE a patch is applied.
        /// </summary>
        public event EventHandler<PatchEventArgs> PatchApplying;

        /// <summary>
        /// Event triggered AFTER a patch is appiled.
        /// </summary>
        public event EventHandler<PatchEventArgs> PatchApplied;

        /// <summary>
        /// Event triggered AFTER a patch is skipped.
        /// </summary>
        public event EventHandler<PatchEventArgs> PatchSkipped;

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
        /// Gets or sets the list of patches.
        /// </summary>
        public List<Models.PatchInfo> Patches { get; set; }

        /// <summary>
        /// Gets or sets the list of parameters.
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        /// Extracts all translatable assets from the game.
        /// </summary>
        /// <param name="gamePath">Game install directory.</param>
        /// <param name="outputPath">Output directory.</param>
        public void Extract(string gamePath, string outputPath)
        {
            ScriptExtracting?.Invoke(this, new ScriptEventArgs(this));
            Directory.CreateDirectory(outputPath);

            var containersDict = new Dictionary<string, Node>();

            Node gameRoot = NodeFactory.FromDirectory(gamePath, "*", "root", true, Yarhl.IO.FileOpenMode.Read);
            containersDict.Add("root", gameRoot);

            try
            {
                ReadContainers(Containers, gameRoot, containersDict);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error reading container: {e.Message}");
                return;
            }
            catch (ChecksumMismatchException e)
            {
                Console.WriteLine($"Bad container checksum: {e.Message}");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error reading containers: {e}");
                return;
            }

            foreach (AssetInfo assetInfo in Assets)
            {
                try
                {
                    ExtractAsset(assetInfo, containersDict, outputPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error extracting asset: {assetInfo.Id} - {e.Message}");
                }
            }

            ScriptExtracted?.Invoke(this, new ScriptEventArgs(this));
        }

        /// <summary>
        /// Rebuild all translatable assets from the game.
        /// </summary>
        /// <param name="gamePath">Game install directory.</param>
        /// <param name="translationPath">Translation and patches directory.</param>
        /// <param name="outputPath">Output directory.</param>
        public void Rebuild(string gamePath, string translationPath, string outputPath)
        {
            ScriptRebuilding?.Invoke(this, new ScriptEventArgs(this));
            Directory.CreateDirectory(outputPath);

            var containersDict = new Dictionary<string, Node>();
            Node gameRoot = NodeFactory.FromDirectory(gamePath, "*", "root", true, Yarhl.IO.FileOpenMode.Read);
            containersDict.Add("root", gameRoot);

            try
            {
                ReadContainers(Containers, gameRoot, containersDict);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error reading container: {e.Message}");
                return;
            }
            catch (ChecksumMismatchException e)
            {
                Console.WriteLine($"Bad container checksum: {e.Message}");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error reading containers: {e}");
                return;
            }

            foreach (AssetInfo assetInfo in Assets)
            {
                TranslateAsset(assetInfo, containersDict, translationPath);
            }

            foreach (PatchInfo patchInfo in Patches)
            {
                ApplyPatch(patchInfo, containersDict, translationPath);
            }

            try
            {
                WriteContainers(Containers, gameRoot);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error writing container: {e.Message}");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error writing containers: {e}");
                return;
            }

            foreach (Node node in Navigator.IterateNodes(gameRoot).Where(x => x.Tags.ContainsKey("Changed")))
            {
                List<string> outputPaths = node.Tags["OutputPath"];

                if (outputPaths.Count == 1)
                {
                    node.Stream.WriteTo(Path.Combine(outputPath, outputPaths[0]));
                }
                else
                {
                    for (int i = 0; i < outputPaths.Count; i++)
                    {
                        Node n = node.Children[i];

                        n.Stream.WriteTo(Path.Combine(outputPath, outputPaths[i]));
                    }
                }
            }

            ScriptRebuilt?.Invoke(this, new ScriptEventArgs(this));
        }

        private void ReadContainers(IList<ContainerInfo> containers, Node parent, Dictionary<string, Node> dictionary)
        {
            foreach (ContainerInfo containerInfo in containers)
            {
                ContainerReading?.Invoke(this, new ContainerEventArgs(containerInfo));

                Node node;
                if (containerInfo.Paths.Count == 1)
                {
                    node = Navigator.SearchNode(parent, containerInfo.Paths[0]);

                    if (node == null)
                    {
                        throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Paths[0]}");
                    }

                    if (!ChecksumHelper.Check(node.Stream, containerInfo.Checksums[0]))
                    {
                        throw new ChecksumMismatchException($"Checksum mismatch in {containerInfo.Name}");
                    }
                }
                else
                {
                    node = NodeFactory.CreateContainer(containerInfo.Id);
                    for (int i = 0; i < containerInfo.Paths.Count; i++)
                    {
                        Node n = Navigator.SearchNode(parent, containerInfo.Paths[i]);

                        if (n == null)
                        {
                            throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Paths[i]}");
                        }

                        if (!ChecksumHelper.Check(n.Stream, containerInfo.Checksums[i]))
                        {
                            throw new ChecksumMismatchException($"Checksum mismatch in {containerInfo.Name}");
                        }

                        node.Add(n); // Removes the node 'n' from parent.
                    }
                }

                node.Transform(containerInfo.Readers, Parameters);

                dictionary.Add(containerInfo.Id, node);

                ReadContainers(containerInfo.Containers, node, dictionary);

                if (containerInfo.Paths.Count != 1)
                {
                    parent.Add(node); // Add the converted node to parent
                }

                ContainerRead?.Invoke(this, new ContainerEventArgs(containerInfo));
            }
        }

        private void WriteContainers(List<ContainerInfo> containers, Node parent)
        {
            foreach (ContainerInfo containerInfo in containers)
            {
                ContainerWriting?.Invoke(this, new ContainerEventArgs(containerInfo));

                Node node;

                if (containerInfo.Paths.Count == 1)
                {
                    node = Navigator.SearchNode(parent, containerInfo.Paths[0]);

                    if (node == null)
                    {
                        throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Paths[0]}");
                    }
                }
                else
                {
                    node = Navigator.SearchNode(parent, containerInfo.Id);
                    if (node == null)
                    {
                        throw new DirectoryNotFoundException($"Parent: {parent.Path} - Node: {containerInfo.Id}");
                    }
                }

                WriteContainers(containerInfo.Containers, node);

                node.Transform(containerInfo.Writers, Parameters);
                node.Tags["Changed"] = true;
                node.Tags["OutputPath"] = containerInfo.Paths;

                ContainerWrote?.Invoke(this, new ContainerEventArgs(containerInfo));
            }
        }

        private void ExtractAsset(AssetInfo assetInfo, Dictionary<string, Node> containers, string outputPath)
        {
            AssetExtracting?.Invoke(this, new AssetEventArgs(assetInfo));

            Node asset = ReadAsset(assetInfo, containers);

            asset.Transform(assetInfo.Extractors, Parameters);

            if (asset.IsContainer)
            {
                if (assetInfo.OutputNames.Count == 1 && assetInfo.OutputNames[0].EndsWith("*"))
                {
                    string customPath = assetInfo.OutputNames[0].Substring(0, assetInfo.OutputNames[0].Length - 1);

                    // Use node names to extract
                    foreach (Node node in Navigator.IterateNodes(asset))
                    {
                        if (node.IsContainer)
                        {
                            continue;
                        }

                        string path = Path.Combine(outputPath, customPath, node.Path.Substring(1));
                        node.Stream.WriteTo(path);
                    }
                }
                else
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
            }
            else if (asset.Stream != null)
            {
                asset.Stream.WriteTo(Path.Combine(outputPath, assetInfo.OutputNames[0]));
            }
            else
            {
                throw new FormatException("Can't extract asset.");
            }

            AssetExtracted?.Invoke(this, new AssetEventArgs(assetInfo));
        }

        private Node ReadAsset(AssetInfo assetInfo, Dictionary<string, Node> containers)
        {
            AssetReading?.Invoke(this, new AssetEventArgs(assetInfo));
            Node asset = NodeFactory.CreateContainer(assetInfo.Id);

            foreach (AssetFileInfo fileInfo in assetInfo.Files)
            {
                Node file;
                try
                {
                    file = ReadFile(fileInfo, containers);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Error reading file: {e.Message}");
                    continue;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine($"Error reading file: {e.Message}");
                    continue;
                }
                catch (ChecksumMismatchException e)
                {
                    Console.WriteLine($"Bad file checksum: {e.Message}");
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error reading file: {e}");
                    continue;
                }

                // Add call will remove the node from its original parent, so we need to make a copy
                var newFile = new Node(file);
                newFile.Transform(fileInfo.Readers, Parameters);
                asset.Add(newFile);
            }

            if (asset.Children.Count == 1)
            {
                asset.TransformWith<SingleNodeToFormat>();
            }

            AssetRead?.Invoke(this, new AssetEventArgs(assetInfo));
            return asset;
        }

        private Node ReadFile(Models.FileInfo fileInfo, Dictionary<string, Node> containers)
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

            return file;
        }

        private void TranslateAsset(AssetInfo assetInfo, Dictionary<string, Node> containers, string translationPath)
        {
            AssetTranslating?.Invoke(this, new AssetEventArgs(assetInfo));
            Node translation = ReadTranslation(assetInfo, translationPath);
            if (translation == null)
            {
                // Skip the asset if there is no translation
                return;
            }

            Node asset = ReadAsset(assetInfo, containers);

            asset.Translate(translation, assetInfo.Translator);

            foreach (AssetFileInfo fileInfo in assetInfo.Files)
            {
                if (!containers.TryGetValue(fileInfo.ContainerId, out Node container))
                {
                    throw new DirectoryNotFoundException($"Container not found: {fileInfo.ContainerId}");
                }

                Node newFile;

                if (!asset.IsContainer || asset.Name == fileInfo.Name)
                {
                    newFile = asset;
                }
                else
                {
                    newFile = Navigator.SearchNode(asset, fileInfo.Name);
                }

                newFile.Transform(fileInfo.Writers, Parameters);

                Node originalFile = Navigator.SearchNode(container, fileInfo.Path);

                originalFile.ChangeFormat(newFile.Format);
                originalFile.Tags["Changed"] = true;
                originalFile.Tags["OutputPath"] = new List<string> { fileInfo.Path };
            }

            AssetTranslated?.Invoke(this, new AssetEventArgs(assetInfo));
        }

        private Node ReadTranslation(AssetInfo assetInfo, string translationPath)
        {
            Node translation;

            if (assetInfo.OutputNames.Count == 1 && assetInfo.OutputNames[0].EndsWith("*"))
            {
                // Can't check if all translation files exists, only the directory
                string customPath = assetInfo.OutputNames[0].Substring(0, assetInfo.OutputNames[0].Length - 1);
                string path = Path.Combine(translationPath, customPath, assetInfo.Id);
                if (!Directory.Exists(path))
                {
                    AssetTranslationFailed?.Invoke(this, new AssetEventArgs(assetInfo));
                    return null;
                }

                translation = NodeFactory.FromDirectory(path, "*", string.Concat(assetInfo.Id, "_Translation"), true);
            }
            else
            {
                translation = NodeFactory.CreateContainer(string.Concat(assetInfo.Id, "_Translation"));

                foreach (string outputFile in assetInfo.OutputNames.Select(file => Path.Combine(translationPath, file)))
                {
                    if (File.Exists(outputFile))
                    {
                        Node node = NodeFactory.FromFile(outputFile, Yarhl.IO.FileOpenMode.Read);
                        translation.Add(node);
                    }
                    else
                    {
                        // All files are needed for translating. If any of them is missing, skip the translation of this asset.
                        AssetTranslationFailed?.Invoke(this, new AssetEventArgs(assetInfo));
                        return null;
                    }
                }
            }

            translation.Transform(assetInfo.TranslationMergers, Parameters);

            if (translation.Children.Count == 1)
            {
                translation.TransformWith<SingleNodeToFormat>();
            }

            return translation;
        }

        private void ApplyPatch(PatchInfo patchInfo, Dictionary<string, Node> containers, string translationPath)
        {
            PatchApplying?.Invoke(this, new PatchEventArgs(patchInfo));

            string path = Path.Combine(translationPath, "patches", patchInfo.Patch);
            if (!File.Exists(path))
            {
                // Skip if there is no patch file
                PatchSkipped?.Invoke(this, new PatchEventArgs(patchInfo));
                return;
            }

            Node patch = NodeFactory.FromFile(path, Yarhl.IO.FileOpenMode.Read);
            if (patch == null)
            {
                // Skip if there is an error reading the patch
                PatchSkipped?.Invoke(this, new PatchEventArgs(patchInfo));
                return;
            }

            patch.TransformWith<Converters.BinaryPatch.Reader, long>(-patchInfo.VirtualAddress + patchInfo.RawAddress);

            Node file;
            try
            {
                file = ReadFile(patchInfo.File, containers);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error reading file: {e.Message}");
                return;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Error reading file: {e.Message}");
                return;
            }
            catch (ChecksumMismatchException e)
            {
                Console.WriteLine($"Bad file checksum: {e.Message}");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error reading file: {e}");
                return;
            }

            file.TransformWith<Converters.BinaryPatch.Apply, Formats.BinaryPatch>(patch.GetFormatAs<Formats.BinaryPatch>());

            file.Tags["Changed"] = true;
            file.Tags["OutputPath"] = new List<string> { patchInfo.File.Path };

            PatchApplied?.Invoke(this, new PatchEventArgs(patchInfo));
        }
    }
}
