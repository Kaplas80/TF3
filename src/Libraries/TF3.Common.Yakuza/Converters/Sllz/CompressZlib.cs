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
    /// Compress SLLZ zlib files.
    /// </summary>
    public class CompressZlib : IConverter<BinaryFormat, BinaryFormat>, IInitializer<CompressorParameters>
    {
        private CompressorParameters _compressorParameters = new ()
        {
            CompressionType = CompressionType.Zlib,
            Endianness = Endianness.LittleEndian,
            OutputStream = null,
        };

        /// <summary>
        /// Initializes the compressor parameters.
        /// </summary>
        /// <param name="parameters">Compressor configuration.</param>
        public void Initialize(CompressorParameters parameters) => _compressorParameters = parameters;

        /// <summary>
        /// Creates a SLLZ zlib compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Original format.</param>
        /// <returns>The compressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Seek(0);

            byte[] data = new byte[source.Stream.Length];
            _ = source.Stream.Read(data, 0, data.Length);

            byte[] compressedData;

            try
            {
                compressedData = Compress(data);
            }
            catch (SllzException)
            {
                // Data can't be compressed
                return source;
            }

            DataStream outputDataStream = _compressorParameters.OutputStream ?? DataStreamFactory.FromMemory();
            outputDataStream.Position = 0;

            var writer = new DataWriter(outputDataStream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = _compressorParameters.Endianness == Endianness.LittleEndian
                    ? EndiannessMode.LittleEndian
                    : EndiannessMode.BigEndian,
            };

            var header = new SllzHeader
            {
                Magic = "SLLZ",
                Endianness = _compressorParameters.Endianness,
                CompressionType = _compressorParameters.CompressionType,
                HeaderSize = 0x10,
                OriginalSize = (uint)source.Stream.Length,
                CompressedSize = (uint)compressedData.Length + 0x10, // includes header length
            };

            writer.WriteOfType(header);
            writer.Write(compressedData);

            return new BinaryFormat(outputDataStream);
        }

        private static byte[] Compress(byte[] inputData)
        {
            if (inputData.Length < 0x1B)
            {
                throw new SllzException("Input data length < 0x1B.");
            }

            uint outputSize = (uint)inputData.Length - 16;
            byte[] outputData = new byte[outputSize];

            int inputPosition = 0;
            int outputPosition = 0;
            int remaining = inputData.Length;

            while (remaining > 0)
            {
                if (outputPosition >= outputSize)
                {
                    throw new SllzException("Compressed size is bigger than original size.");
                }

                int decompressedChunkSize = Math.Min(remaining, 0x10000);

                byte[] compressedData = ZlibCompress(inputData, inputPosition, decompressedChunkSize);

                if (compressedData.Length > decompressedChunkSize)
                {
                    int compressedDataLength = decompressedChunkSize + 5;
                    int decompressedDataLength = decompressedChunkSize - 1;
                    outputData[outputPosition] = (byte)((compressedDataLength >> 16) | 0x80);
                    outputData[outputPosition + 1] = (byte)(compressedDataLength >> 8);
                    outputData[outputPosition + 2] = (byte)compressedDataLength;
                    outputData[outputPosition + 3] = (byte)(decompressedDataLength >> 8);
                    outputData[outputPosition + 4] = (byte)decompressedDataLength;

                    outputPosition += 5;
                    Array.Copy(
                        inputData,
                        inputPosition,
                        outputData,
                        outputPosition,
                        decompressedChunkSize);

                    outputPosition += decompressedChunkSize;
                }
                else
                {
                    int compressedDataLength = compressedData.Length + 5;
                    int decompressedDataLength = decompressedChunkSize - 1;
                    outputData[outputPosition] = (byte)(compressedDataLength >> 16);
                    outputData[outputPosition + 1] = (byte)(compressedDataLength >> 8);
                    outputData[outputPosition + 2] = (byte)compressedDataLength;
                    outputData[outputPosition + 3] = (byte)(decompressedDataLength >> 8);
                    outputData[outputPosition + 4] = (byte)decompressedDataLength;

                    outputPosition += 5;
                    Array.Copy(
                        compressedData,
                        0,
                        outputData,
                        outputPosition,
                        compressedData.Length);

                    outputPosition += compressedData.Length;
                }

                inputPosition += decompressedChunkSize;
                remaining -= decompressedChunkSize;
            }

            if (outputPosition > inputData.Length)
            {
                throw new SllzException("Compressed size is bigger than original size.");
            }

            byte[] result = new byte[outputPosition];
            Array.Copy(outputData, result, outputPosition);
            return result;
        }

        private static byte[] ZlibCompress(byte[] data, int index, int count)
        {
            using var inputMemoryStream = new MemoryStream(data, index, count);
            using var outputMemoryStream = new MemoryStream();
            using var zlibStream = new ZlibStream(outputMemoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
            inputMemoryStream.CopyTo(zlibStream);
            zlibStream.Close();

            return outputMemoryStream.ToArray();
        }
    }
}
