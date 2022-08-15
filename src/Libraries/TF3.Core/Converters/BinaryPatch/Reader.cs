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

namespace TF3.Core.Converters.BinaryPatch
{
    using System;
    using System.Text;
    using TF3.Core.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Patch file reader.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, BinaryPatch>, IInitializer<long>
    {
        private long _rawOffset;

        /// <summary>
        /// Initialize the reader.
        /// </summary>
        /// <param name="parameters">File Raw Offset.</param>
        public void Initialize(long parameters) => _rawOffset = parameters;

        /// <summary>
        /// Reads a patch file.
        /// </summary>
        /// <param name="source">The patch file.</param>
        /// <returns>The patch.</returns>
        public BinaryPatch Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new BinaryPatch
            {
                RawOffset = _rawOffset,
            };

            source.Stream.Seek(0);
            var reader = new TextDataReader(source.Stream, Encoding.UTF8);

            while (!source.Stream.EndOfStream)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                line = line.Trim();
                string[] split;
                switch (line[0])
                {
                    case ';':
                        continue;

                    case '>':
                        split = line.Split(';');
                        result.FileName = split[0].Substring(1);
                        break;

                    default:
                        split = line.Split(';');
                        string[] patchInfo = split[0].Split(':');
                        string[] bytes = patchInfo[1].Split("->");
                        long address = long.Parse(patchInfo[0], System.Globalization.NumberStyles.HexNumber);
                        byte original = byte.Parse(bytes[0], System.Globalization.NumberStyles.HexNumber);
                        byte patched = byte.Parse(bytes[1], System.Globalization.NumberStyles.HexNumber);
                        result.Patches.Add((address, original, patched));
                        break;
                }
            }

            if (string.IsNullOrEmpty(result.FileName) || result.Patches.Count == 0)
            {
                throw new FormatException("Invalid patch file.");
            }

            return result;
        }
    }
}
