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

namespace TF3.Core.Converters.DdsImage
{
    using System;
    using BCnEncoder.Shared.ImageFiles;
    using TF3.Core.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// DDS file reader.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, DdsFileFormat>
    {
        /// <summary>
        /// Reads a DDS file.
        /// </summary>
        /// <param name="source">The DDS file.</param>
        /// <returns>The DDS format.</returns>
        public DdsFileFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Seek(0);
            DdsFileFormat result = new ()
            {
                Internal = DdsFile.Load(source.Stream),
            };

            return result;
        }
    }
}
