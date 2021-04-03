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
namespace TF3.YarhlPlugin.YakuzaCommon.Converters.Sllz
{
    using System;
    using System.Text;
    using TF3.YarhlPlugin.YakuzaCommon.Enums;
    using TF3.YarhlPlugin.YakuzaCommon.Formats;
    using TF3.YarhlPlugin.YakuzaCommon.Types;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Compress SLLZ standard files.
    /// </summary>
    public class CompressStandard : IConverter<BinaryFormat, ParFile>, IInitializer<CompressorParameters>
    {
        private const uint MaxWindowSize = 4096;
        private const uint MaxEncodedLength = 18;

        private CompressorParameters compressorParameters = new ()
        {
            CompressionType = CompressionType.Standard,
            Endianness = Endianness.LittleEndian,
            OutputStream = null,
        };

        /// <summary>
        /// Initializes the compressor parameters.
        /// </summary>
        /// <param name="parameters">Compressor configuration.</param>
        public void Initialize(CompressorParameters parameters) => compressorParameters = parameters;

        /// <summary>
        /// Creates a SLLZ standard compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Original format.</param>
        /// <returns>The compressed binary.</returns>
        public ParFile Convert(BinaryFormat source)
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
                return new ParFile(source.Stream);
            }

            DataStream outputDataStream = compressorParameters.OutputStream ?? DataStreamFactory.FromMemory();
            outputDataStream.Position = 0;

            var writer = new DataWriter(outputDataStream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = compressorParameters.Endianness == Endianness.LittleEndian
                    ? EndiannessMode.LittleEndian
                    : EndiannessMode.BigEndian,
            };

            var header = new SllzHeader
            {
                Magic = "SLLZ",
                Endianness = compressorParameters.Endianness,
                CompressionType = compressorParameters.CompressionType,
                HeaderSize = 0x10,
                OriginalSize = (uint)source.Stream.Length,
                CompressedSize = (uint)compressedData.Length + 0x10, // includes header length
            };

            writer.WriteOfType(header);
            writer.Write(compressedData);

            var fileInfo = new ParFileInfo
            {
                Flags = 0x80000000,
                OriginalSize = (uint)source.Stream.Length,
                CompressedSize = (uint)outputDataStream.Length,
                DataOffset = 0,
                RawAttributes = 0,
                ExtendedOffset = 0,
                Timestamp = 0,
            };

            return new ParFile(fileInfo, outputDataStream);
        }

        private static byte[] Compress(byte[] inputData)
        {
            uint outputSize = (uint)inputData.Length - 16;
            byte[] outputData = new byte[outputSize];

            byte flag = 0x00;
            int bitsRemaining = 8;

            uint inputPosition = 0;
            uint outputPosition = 1; // First flag is always 0x00
            uint flagPosition = 16;

            while (inputPosition < inputData.Length)
            {
                uint windowSize = Math.Min(inputPosition, MaxWindowSize);
                uint maxOffsetLength = Math.Min((uint)(inputData.Length - inputPosition), MaxEncodedLength);

                Match match = FindMatch(inputData, inputPosition, windowSize, maxOffsetLength);
                if (match != null)
                {
                    flag = (byte)(flag << 1);
                    flag = (byte)(flag | 0x01);
                    bitsRemaining--;
                    if (bitsRemaining == 0)
                    {
                        outputData[flagPosition] = flag;
                        flag = 0x00;
                        bitsRemaining = 8;
                        flagPosition = outputPosition;
                        outputPosition++;
                        if (outputPosition >= outputSize)
                        {
                            throw new SllzException("Compressed size is bigger than original size.");
                        }
                    }

                    ushort offset = (ushort)((match.Offset - 1) << 4);
                    ushort length = (ushort)((match.Length - 3) & 0x0F);

                    ushort tuple = (ushort)(offset | length);

                    outputData[outputPosition] = (byte)tuple;
                    outputPosition++;
                    if (outputPosition >= outputSize)
                    {
                        throw new SllzException("Compressed size is bigger than original size.");
                    }

                    outputData[outputPosition] = (byte)(tuple >> 8);
                    outputPosition++;

                    if (outputPosition >= outputSize)
                    {
                        throw new SllzException("Compressed size is bigger than original size.");
                    }

                    inputPosition += match.Length;
                }
                else
                {
                    flag = (byte)(flag << 1);
                    bitsRemaining--;
                    if (bitsRemaining == 0)
                    {
                        outputData[flagPosition] = flag;
                        flag = 0x00;
                        bitsRemaining = 8;
                        flagPosition = outputPosition;
                        outputPosition++;
                        if (outputPosition >= outputSize)
                        {
                            throw new SllzException("Compressed size is bigger than original size.");
                        }
                    }

                    outputData[outputPosition++] = inputData[inputPosition++];
                    if (outputPosition >= outputSize)
                    {
                        throw new SllzException("Compressed size is bigger than original size.");
                    }
                }
            }

            if (bitsRemaining != 8)
            {
                while (bitsRemaining > 0)
                {
                    flag = (byte)(flag << 1);
                    bitsRemaining--;
                }
            }

            outputData[flagPosition] = flag;

            byte[] result = new byte[outputPosition];
            Array.Copy(outputData, result, outputPosition);
            return result;
        }

        private static Match FindMatch(byte[] inputData, uint inputPosition, uint windowSize, uint maxOffsetLength)
        {
            ReadOnlySpan<byte> bytes = inputData;
            ReadOnlySpan<byte> data = bytes.Slice((int)(inputPosition - windowSize), (int)windowSize);

            uint currentLength = maxOffsetLength;

            while (currentLength >= 3)
            {
                ReadOnlySpan<byte> pattern = bytes.Slice((int)inputPosition, (int)currentLength);

                int pos = data.LastIndexOf(pattern);

                if (pos >= 0)
                {
                    return new Match
                    {
                        Length = currentLength,
                        Offset = (uint)(windowSize - pos),
                    };
                }

                currentLength--;
            }

            return null;
        }

        private class Match
        {
            public uint Length { get; set; }

            public uint Offset { get; set; }
        }
    }
}
