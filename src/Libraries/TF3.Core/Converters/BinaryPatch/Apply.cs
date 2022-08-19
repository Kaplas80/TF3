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
    using TF3.Core.Exceptions;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Binary patch applier.
    /// </summary>
    public class Apply : IConverter<BinaryFormat, BinaryFormat>, IInitializer<Formats.BinaryPatch>
    {
        private Formats.BinaryPatch _patch;

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="parameters">Patch info.</param>
        public void Initialize(Formats.BinaryPatch parameters) => _patch = parameters;

        /// <summary>
        /// Applies a binary patch to a file.
        /// </summary>
        /// <param name="source">The original BinaryFormat.</param>
        /// <returns>The BinaryFormat with the applied patch.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_patch == null)
            {
                throw new InvalidOperationException("Uninitialized");
            }

            var reader = new DataReader(source.Stream);
            var writer = new DataWriter(source.Stream);
            foreach ((long rva, byte expectedByte, byte newByte) in _patch.Patches)
            {
                source.Stream.Seek(rva + _patch.RawOffset);
                byte original = reader.ReadByte();

                if (original != expectedByte)
                {
                    throw new ByteMismatchException($"Address: 0x{rva + _patch.RawOffset:X16} - Byte: {original:X2} - Expected: {expectedByte:X2}");
                }

                source.Stream.Seek(rva + _patch.RawOffset);
                writer.Write(newByte);
            }

            return source;
        }
    }
}
