// Copyright (c) 2020 Kaplas
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
    using System.Collections.Generic;
    using System.Text;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompress SLLZ standard files.
    /// </summary>
    public class DecompressStandard : IConverter<BinaryFormat, BinaryFormat>
    {
        /// <summary>
        /// Decompress a SLLZ standard compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The decompressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.ASCII,
            };

            source.Stream.Seek(4);
            byte endianness = reader.ReadByte();
            reader.Endianness = endianness == 0 ? EndiannessMode.LittleEndian : EndiannessMode.BigEndian;

            source.Stream.Position = 0;

            // Read the file header
            var header = reader.Read<SllzHeader>() as SllzHeader;
            CheckHeader(header);

            reader.Stream.Seek(header.HeaderSize);

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
            if (header == null) {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "SLLZ") {
                throw new FormatException($"SLLZ Standard: Bad magic Id ({header.Magic} != SLLZ)");
            }

            if (header.CompressionType != CompressionType.Standard) {
                throw new FormatException($"SLLZ Standard: Bad Compression Type ({header.CompressionType})");
            }
        }

        private static byte[] Decompress(IReadOnlyList<byte> inputData, uint decompressedSize)
        {
            byte[] outputData = new byte[decompressedSize];

            int inputPosition = 0;
            int outputPosition = 0;

            byte flag = inputData[inputPosition];
            inputPosition++;
            int bitsRemaining = 8;

            do {
                if ((flag & 0x80) == 0x80) {
                    flag = (byte)(flag << 1);
                    bitsRemaining--;
                    if (bitsRemaining == 0) {
                        flag = inputData[inputPosition];
                        inputPosition++;
                        bitsRemaining = 8;
                    }

                    int copyCount = (inputData[inputPosition] & 0xF) + 3;
                    int copyDistance = ((inputData[inputPosition] >> 4) | (inputData[inputPosition + 1] << 4)) + 1;
                    inputPosition += 2;

                    for (int i = 0; i < copyCount; i++) {
                        outputData[outputPosition] = outputData[outputPosition - copyDistance];
                        outputPosition++;
                    }
                }
                else {
                    flag = (byte)(flag << 1);
                    bitsRemaining--;
                    if (bitsRemaining == 0) {
                        flag = inputData[inputPosition];
                        inputPosition++;
                        bitsRemaining = 8;
                    }

                    outputData[outputPosition] = inputData[inputPosition];
                    inputPosition++;
                    outputPosition++;
                }
            }
            while (outputPosition < decompressedSize);

            return outputData;
        }
    }
}
