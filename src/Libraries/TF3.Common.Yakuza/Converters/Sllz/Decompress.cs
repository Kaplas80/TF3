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
    using System.Text;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompress SLLZ files.
    /// </summary>
    public class Decompress : IConverter<BinaryFormat, BinaryFormat>
    {
        /// <summary>
        /// Decompress a SLLZ compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The decompressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

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

            return header.CompressionType switch {
                CompressionType.Standard => (BinaryFormat)ConvertFormat.With<DecompressStandard>(source),
                CompressionType.Zlib => (BinaryFormat)ConvertFormat.With<DecompressZlib>(source),
                _ => throw new FormatException($"SLLZ: Bad Compression Type ({header.CompressionType})"),
            };
        }

        private static void CheckHeader(SllzHeader header)
        {
            if (header == null) {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "SLLZ") {
                throw new FormatException($"SLLZ: Bad magic Id ({header.Magic} != SLLZ)");
            }

            if (header.CompressionType is not CompressionType.Standard and not CompressionType.Zlib) {
                throw new FormatException($"SLLZ: Bad Compression Type ({header.CompressionType})");
            }
        }
    }
}
