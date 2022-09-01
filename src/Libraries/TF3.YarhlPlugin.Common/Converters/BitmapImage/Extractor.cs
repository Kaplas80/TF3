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

namespace TF3.YarhlPlugin.Common.Converters.BitmapImage
{
    using System;
    using SixLabors.ImageSharp;
    using TF3.YarhlPlugin.Common.Converters.Common;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Bitmap to PNG converter.
    /// </summary>
    public class Extractor : IConverter<BitmapFileFormat, BinaryFormat>, IInitializer<ImageExtractorParameters>
    {
        private ImageExtractorParameters _extractorParameters = new ImageExtractorParameters();

        /// <summary>
        /// Initializes the extractor parameters.
        /// </summary>
        /// <param name="parameters">Extractor configuration.</param>
        public void Initialize(ImageExtractorParameters parameters) => _extractorParameters = parameters;

        /// <summary>
        /// Converts a Bitmap file to a known format.
        /// </summary>
        /// <param name="source">The Bitmap file.</param>
        /// <returns>The output file.</returns>
        public BinaryFormat Convert(BitmapFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new BinaryFormat();

            switch (_extractorParameters.ImageFormat)
            {
                case BitmapExtractionFormat.Png:
                    source.Internal.SaveAsPng(result.Stream);
                    break;

                case BitmapExtractionFormat.Tga:
                    source.Internal.SaveAsTga(result.Stream);
                    break;

                case BitmapExtractionFormat.Bmp:
                    source.Internal.SaveAsBmp(result.Stream);
                    break;

                default:
                    throw new FormatException("Unknown image format");
            }

            source.Internal.Dispose();

            return result;
        }
    }
}
