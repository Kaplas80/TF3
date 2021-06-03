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

namespace TF3.Core.Converters
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// BinaryFormat replacer.
    /// </summary>
    public class BinaryReplace : IConverter<BinaryFormat, BinaryFormat>, IInitializer<BinaryFormat>
    {
        private BinaryFormat _newBinaryFormat = null;

        /// <summary>
        /// Set the new binary.
        /// </summary>
        /// <param name="parameters">The new binary.</param>
        public void Initialize(BinaryFormat parameters) => _newBinaryFormat = parameters;

        /// <summary>
        /// Fully replace a BinaryFormat.
        /// </summary>
        /// <param name="source">The original binary format.</param>
        /// <returns>The new binary format.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_newBinaryFormat == null)
            {
                throw new InvalidOperationException("Uninitialized.");
            }

            return _newBinaryFormat;
        }
    }
}
