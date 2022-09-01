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

namespace TF3.YarhlPlugin.Common.Converters.BitmapImage.Replace
{
    using System;
    using SixLabors.ImageSharp;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Replaces the original image with a new one.
    /// </summary>
    public abstract class AbstractReplace : IConverter<BitmapFileFormat, BitmapFileFormat>, IInitializer<BinaryFormat>
    {
        private Image _newImage;

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="parameters">New image binary.</param>
        public abstract void Initialize(BinaryFormat parameters);

        /// <summary>
        /// Replaces the original bitmap with a new one.
        /// </summary>
        /// <param name="source">Original bitmap.</param>
        /// <returns>New bitmap.</returns>
        public BitmapFileFormat Convert(BitmapFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_newImage == null)
            {
                throw new InvalidOperationException("Uninitialized");
            }

            return new BitmapFileFormat()
            {
                Internal = _newImage,
            };
        }

        /// <summary>
        /// Sets the new image to insert.
        /// </summary>
        /// <param name="value">New image.</param>
        protected void SetNewImage(Image value) => _newImage = value;
    }
}
