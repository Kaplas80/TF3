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
namespace TF3.Common.Yakuza.Converters.Par
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Serializes PAR archives.
    /// </summary>
    public class Writer : IConverter<NodeContainerFormat, BinaryFormat>, IInitializer<WriterParameters>
    {
        private WriterParameters _writerParameters = new ()
        {
            PlatformId = Platform.PlayStation3,
            Endianness = Endianness.BigEndian,
            Version = 0x00020001,
            WriteDataSize = false,
            OutputStream = null,
        };

        /// <summary>
        /// Initializes the writer parameters.
        /// </summary>
        /// <param name="parameters">Writer configuration.</param>
        public void Initialize(WriterParameters parameters) => _writerParameters = parameters;

        /// <summary>
        /// Converts a BinaryFormat into a NodeContainerFormat.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The node container format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public virtual BinaryFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Reorder nodes
            source.Root.SortChildren((x, y) =>
                string.CompareOrdinal(x.Name.ToLowerInvariant(), y.Name.ToLowerInvariant()));

            // Fill node indexes
            FillNodeIndexes(source.Root);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DataStream stream = _writerParameters.OutputStream ?? DataStreamFactory.FromMemory();

            stream.Position = 0;

            var writer = new DataWriter(stream)
            {
                DefaultEncoding = Encoding.GetEncoding(1252),
                Endianness = _writerParameters.Endianness == Endianness.LittleEndian
                    ? EndiannessMode.LittleEndian
                    : EndiannessMode.BigEndian,
            };

            var directories = new List<Node>();
            var files = new List<Node>();
            uint fileOffset = 0;
            uint maxFileSize = 0;
            foreach (Node node in Navigator.IterateNodes(source.Root))
            {
                if (!node.IsContainer)
                {
                    uint compressedSize = (uint)node.Tags["CompressedSize"];
                    fileOffset = RoundSize(fileOffset, compressedSize);
                    if (compressedSize > maxFileSize)
                    {
                        maxFileSize = compressedSize;
                    }

                    files.Add(node);
                }
                else
                {
                    directories.Add(node);
                }
            }

            if (maxFileSize >= 0xFFFFFFFF)
            {
                throw new FormatException("Can not add files over 4GB");
            }

            var header = new FileHeader
            {
                Magic = "PARC",
                PlatformId = _writerParameters.PlatformId,
                Endianness = _writerParameters.Endianness,
                SizeExtended = 0,
                Relocated = 0,
                Version = _writerParameters.Version,
                Size = 0,
            };

            uint directoryStartOffset = (uint)(0x20 + (0x40 * (directories.Count + files.Count)));
            uint fileStartOffset = (uint)(directoryStartOffset + (0x20 * directories.Count));
            var index = new ParIndex
            {
                DirectoryCount = (uint)directories.Count,
                DirectoryStartOffset = directoryStartOffset,
                FileCount = (uint)files.Count,
                FileStartOffset = fileStartOffset,
            };

            uint headerSize = RoundSize((uint)((0x20 * files.Count) + fileStartOffset));
            writer.Stream.SetLength(RoundSize(fileOffset + headerSize));

            uint currentOffset = headerSize;

            if (_writerParameters.WriteDataSize)
            {
                header.Size = headerSize + fileOffset;
            }

            writer.WriteOfType(header);
            writer.WriteOfType(index);

            for (int i = 0; i < directories.Count; i++)
            {
                Node node = directories[i];
                writer.Write(node.Name, 0x40, false);

                long returnPosition = writer.Stream.Position;
                _ = writer.Stream.Seek(directoryStartOffset + (i * 0x20), System.IO.SeekOrigin.Begin);

                var directoryInfo = new ParDirectoryInfo
                {
                    SubdirectoryCount = (uint)node.Tags["SubdirectoryCount"],
                    SubdirectoryStartIndex = (uint)node.Tags["SubdirectoryStartIndex"],
                    FileCount = (uint)node.Tags["FileCount"],
                    FileStartIndex = (uint)node.Tags["FileStartIndex"],
                    RawAttributes = (uint)node.Tags["RawAttributes"],
                };

                writer.WriteOfType(directoryInfo);
                writer.WritePadding(0x00, 0x20);

                _ = writer.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);
            }

            for (int i = 0; i < files.Count; i++)
            {
                Node node = files[i];
                writer.Write(node.Name, 0x40, false);

                long returnPosition = writer.Stream.Position;
                _ = writer.Stream.Seek(fileStartOffset + (i * 0x20), System.IO.SeekOrigin.Begin);

                var fileInfo = new ParFileInfo
                {
                    Flags = (uint)node.Tags["Flags"],
                    OriginalSize = (uint)node.Tags["OriginalSize"],
                    CompressedSize = (uint)node.Tags["CompressedSize"],
                    DataOffset = (uint)node.Tags["DataOffset"],
                    RawAttributes = (uint)node.Tags["RawAttributes"],
                    ExtendedOffset = (uint)node.Tags["ExtendedOffset"],
                    Timestamp = (ulong)node.Tags["Timestamp"],
                };

                writer.WriteOfType(fileInfo);

                currentOffset = RoundOffset(currentOffset, fileInfo.CompressedSize);

                long returnPosition2 = writer.Stream.Position;
                _ = writer.Stream.Seek(currentOffset, System.IO.SeekOrigin.Begin);
                node.Stream.WriteTo(writer.Stream);
                _ = writer.Stream.Seek(returnPosition2, System.IO.SeekOrigin.Begin);

                currentOffset += fileInfo.CompressedSize;

                _ = writer.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);
            }

            return new BinaryFormat(stream);
        }

        private static void FillNodeIndexes(Node root)
        {
            if (!root.IsContainer)
            {
                throw new FormatException("Root node must be a directory.");
            }

            uint subdirectoryIndex = 1;
            uint fileIndex = 0;

            foreach (Node node in Navigator.IterateNodes(root))
            {
                if (!node.IsContainer)
                {
                    continue;
                }

                uint subdirectoryCount = (uint)node.Children.Count(x => x.IsContainer);
                uint fileCount = (uint)node.Children.Count(x => !x.IsContainer);

                node.Tags["SubdirectoryStartIndex"] = subdirectoryIndex;
                node.Tags["SubdirectoryCount"] = subdirectoryCount;
                node.Tags["FileStartIndex"] = fileIndex;
                node.Tags["FileCount"] = fileCount;

                subdirectoryIndex += subdirectoryCount;
                fileIndex += fileCount;
            }
        }

        private static uint RoundSize(uint offset, uint size) => size + RoundOffset(offset, size);

        private static uint RoundSize(uint size) => (size + 0x07FF) & 0xFFFFF800;

        private static uint RoundOffset(uint offset, uint size)
        {
            if (RoundNeeded(offset, size))
            {
                offset = RoundSize(offset);
            }

            return offset;
        }

        private static bool RoundNeeded(uint offset, uint size) => size + (offset & 0x07FF) >= 0x800;
    }
}
