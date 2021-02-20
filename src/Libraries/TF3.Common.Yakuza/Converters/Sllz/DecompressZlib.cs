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
namespace TF3.Common.Yakuza.Converters.Sllz
{
    using System;
    using System.IO;
    using System.Text;
    using Ionic.Zlib;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompress SLLZ zLib files.
    /// </summary>
    public class DecompressZlib : IConverter<BinaryFormat, BinaryFormat>
    {
        /// <summary>
        /// Decompress a SLLZ zlib compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The decompressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.ASCII,
            };

            _ = source.Stream.Seek(4, SeekOrigin.Begin);
            byte endianness = reader.ReadByte();
            reader.Endianness = endianness == 0 ? EndiannessMode.LittleEndian : EndiannessMode.BigEndian;

            source.Stream.Position = 0;

            // Read the file header
            var header = reader.Read<SllzHeader>() as SllzHeader;
            CheckHeader(header);

            _ = reader.Stream.Seek(header.HeaderSize, SeekOrigin.Begin);

            byte[] compressedData = reader.ReadBytes((int)(header.CompressedSize - header.HeaderSize));
            byte[] decompressedData = Decompress(compressedData, header.OriginalSize);

            DataStream newStream = DataStreamFactory.FromArray(
                decompressedData,
                0,
                (int)header.OriginalSize);

            return new BinaryFormat(newStream);
        }

        private static void CheckHeader(SllzHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "SLLZ")
            {
                throw new FormatException($"SLLZ zlib: Bad magic Id ({header.Magic} != SLLZ)");
            }

            if (header.CompressionType != CompressionType.Zlib)
            {
                throw new FormatException($"SLLZ zlib: Bad Compression Type ({header.CompressionType})");
            }
        }

        private static byte[] Decompress(byte[] inputData, uint decompressedSize)
        {
            byte[] outputData = new byte[decompressedSize];

            uint inputPosition = 0;
            uint outputPosition = 0;

            while (outputPosition < decompressedSize)
            {
                uint compressedChunkSize = (uint)((inputData[inputPosition] << 16) |
                                                 (inputData[inputPosition + 1] << 8) |
                                                 inputData[inputPosition + 2]);
                uint decompressedChunkSize = (uint)(((inputData[inputPosition + 3] << 8) |
                                                    inputData[inputPosition + 4]) + 1);

                bool isCompressed = (compressedChunkSize & 0x00800000) == 0;

                if (isCompressed)
                {
                    byte[] decompressedData = ZlibDecompress(
                        inputData,
                        (int)(inputPosition + 5),
                        (int)(compressedChunkSize - 5));

                    if (decompressedChunkSize != decompressedData.Length)
                    {
                        throw new FormatException("SLLZ zlib: Wrong decompressed data.");
                    }

                    Array.Copy(
                        decompressedData,
                        0,
                        outputData,
                        outputPosition,
                        decompressedData.Length);
                }
                else
                {
                    Array.Copy(
                        inputData,
                        inputPosition + 5,
                        outputData,
                        outputPosition,
                        decompressedChunkSize);

                    compressedChunkSize &= 0xFF7FFFFF;
                }

                inputPosition += compressedChunkSize;
                outputPosition += decompressedChunkSize;
            }

            return outputData;
        }

        private static byte[] ZlibDecompress(byte[] data, int index, int count)
        {
            using var inputMemoryStream = new MemoryStream(data, index, count);
            using var outputMemoryStream = new MemoryStream();
            using var zlibStream = new ZlibStream(outputMemoryStream, CompressionMode.Decompress);
            inputMemoryStream.CopyTo(zlibStream);
            zlibStream.Close();

            return outputMemoryStream.ToArray();
        }
    }
}
