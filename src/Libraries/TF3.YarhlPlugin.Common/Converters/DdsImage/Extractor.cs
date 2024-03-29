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

namespace TF3.YarhlPlugin.Common.Converters.DdsImage
{
    using System;
    using BCnEncoder.ImageSharp;
    using BCnEncoder.Shared;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using TF3.YarhlPlugin.Common.Converters.Common;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// DDS to PNG converter.
    /// </summary>
    public class Extractor : IConverter<DdsFileFormat, BinaryFormat>, IInitializer<ImageExtractorParameters>
    {
        private ImageExtractorParameters _extractorParameters = new ImageExtractorParameters();

        /// <summary>
        /// Initializes the extractor parameters.
        /// </summary>
        /// <param name="parameters">Extractor configuration.</param>
        public void Initialize(ImageExtractorParameters parameters) => _extractorParameters = parameters;

        /// <summary>
        /// Converts a DDS file into PNG.
        /// </summary>
        /// <param name="source">The DDS file.</param>
        /// <returns>The PNG file.</returns>
        public BinaryFormat Convert(DdsFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var decoder = new BCnEncoder.Decoder.BcDecoder()
            {
                OutputOptions =
                {
                    Bc4Component = ColorComponent.Luminance,
                },
            };

            using Image<Rgba32> image = decoder.DecodeToImageRgba32(source.Internal);

            var result = new BinaryFormat();
            switch (_extractorParameters.ImageFormat)
            {
                case BitmapExtractionFormat.Png:
                    image.SaveAsPng(result.Stream);
                    break;

                case BitmapExtractionFormat.Tga:
                    image.SaveAsTga(result.Stream);
                    break;

                default:
                    throw new FormatException("Unknown image format");
            }

            return result;
        }
    }
}
