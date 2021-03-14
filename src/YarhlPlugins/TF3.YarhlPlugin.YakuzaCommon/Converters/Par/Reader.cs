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
namespace TF3.YarhlPlugin.YakuzaCommon.Converters.Par
{
    using System;
    using System.Text;
    using TF3.YarhlPlugin.YakuzaCommon.Enums;
    using TF3.YarhlPlugin.YakuzaCommon.Types;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Deserializes PAR archives.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, NodeContainerFormat>
    {
        /// <summary>
        /// Converts a BinaryFormat into a NodeContainerFormat.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The node container format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public virtual NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var result = new NodeContainerFormat();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding(1252),
                Endianness = EndiannessMode.BigEndian,
            };

            // Read the file header
            var header = reader.Read<FileHeader>() as FileHeader;
            CheckHeader(header);

            if (header.Endianness == Endianness.LittleEndian)
            {
                reader.Endianness = EndiannessMode.LittleEndian;
            }

            var index = reader.Read<ParIndex>() as ParIndex;

            bool[] processedDirectories = new bool[index.DirectoryCount];

            for (uint i = 0; i < index.DirectoryCount; i++)
            {
                if (processedDirectories[i])
                {
                    continue;
                }

                Node directory = ProcessDirectory(i, reader, index, ref processedDirectories);
                if (directory != null)
                {
                    result.Root.Add(directory);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks the validity of the PAR header.
        /// <remarks>Throws an exception if any of the values is invalid.</remarks>
        /// </summary>
        /// <param name="header">The header to check.</param>
        /// <exception cref="ArgumentNullException">Thrown if header is null.</exception>
        /// <exception cref="FormatException">Thrown when some value is invalid.</exception>
        private static void CheckHeader(FileHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "PARC")
            {
                throw new FormatException($"PARC: Bad magic Id ({header.Magic} != PARC)");
            }

            if (header.PlatformId != Platform.PlayStation3)
            {
                throw new FormatException($"PARC: Bad platform Id ({header.PlatformId} != PlayStation3)");
            }
        }

        private Node ProcessDirectory(
            uint directoryIndex,
            DataReader reader,
            ParIndex index,
            ref bool[] processedDirectories)
        {
            if (processedDirectories[directoryIndex])
            {
                return null;
            }

            _ = reader.Stream.Seek(0x20 + (directoryIndex * 0x40), System.IO.SeekOrigin.Begin);
            string name = reader.ReadString(0x40).TrimEnd('\0');
            if (string.IsNullOrEmpty(name))
            {
                name = ".";
            }

            _ = reader.Stream.Seek(index.DirectoryStartOffset + (directoryIndex * 0x20), System.IO.SeekOrigin.Begin);
            var directoryInfo = reader.Read<ParDirectoryInfo>() as ParDirectoryInfo;

            var directory = new Node(name, new NodeContainerFormat())
            {
                Tags =
                {
                    ["SubdirectoryCount"] = directoryInfo.SubdirectoryCount,
                    ["SubdirectoryStartIndex"] = directoryInfo.SubdirectoryStartIndex,
                    ["FileCount"] = directoryInfo.FileCount,
                    ["FileStartIndex"] = directoryInfo.FileStartIndex,
                    ["RawAttributes"] = directoryInfo.RawAttributes,
                },
            };

            for (uint i = directoryInfo.SubdirectoryStartIndex;
                 i < directoryInfo.SubdirectoryStartIndex + directoryInfo.SubdirectoryCount;
                 i++)
            {
                Node child = ProcessDirectory(i, reader, index, ref processedDirectories);
                if (child != null)
                {
                    directory.Add(child);
                }
            }

            for (uint i = directoryInfo.FileStartIndex;
                 i < directoryInfo.FileStartIndex + directoryInfo.FileCount;
                 i++)
            {
                _ = reader.Stream.Seek(0x20 + (0x40 * index.DirectoryCount) + (i * 0x40), System.IO.SeekOrigin.Begin);
                string fileName = reader.ReadString(0x40).TrimEnd('\0');

                _ = reader.Stream.Seek(index.FileStartOffset + (i * 0x20), System.IO.SeekOrigin.Begin);

                var fileInfo = reader.Read<ParFileInfo>() as ParFileInfo;

                long offset = ((long)fileInfo.ExtendedOffset << 32) | fileInfo.DataOffset;
                DataStream stream = DataStreamFactory.FromStream(reader.Stream, offset, fileInfo.CompressedSize);
                var binaryFormat = new BinaryFormat(stream);
                var file = new Node(fileName, binaryFormat)
                {
                    Tags =
                    {
                        ["Flags"] = fileInfo.Flags,
                        ["OriginalSize"] = fileInfo.OriginalSize,
                        ["CompressedSize"] = fileInfo.CompressedSize,
                        ["DataOffset"] = fileInfo.DataOffset,
                        ["RawAttributes"] = fileInfo.RawAttributes,
                        ["ExtendedOffset"] = fileInfo.ExtendedOffset,
                        ["Timestamp"] = fileInfo.Timestamp,
                    },
                };

                directory.Add(file);
            }

            processedDirectories[directoryIndex] = true;
            return directory;
        }
    }
}
