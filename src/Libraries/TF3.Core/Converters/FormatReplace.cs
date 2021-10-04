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

    /// <summary>
    /// Format replacer.
    /// </summary>
    public class FormatReplace : IConverter<IFormat, IFormat>, IInitializer<IFormat>
    {
        private IFormat _newFormat;

        /// <summary>
        /// Set the new format.
        /// </summary>
        /// <param name="parameters">The new format.</param>
        public void Initialize(IFormat parameters) => _newFormat = parameters;

        /// <summary>
        /// Fully replace a IFormat.
        /// </summary>
        /// <param name="source">The original format.</param>
        /// <returns>The new format.</returns>
        public IFormat Convert(IFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_newFormat == null)
            {
                throw new InvalidOperationException("Uninitialized.");
            }

            return _newFormat;
        }
    }
}
