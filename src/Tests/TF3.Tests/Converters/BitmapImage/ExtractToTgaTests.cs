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

namespace TF3.Tests.Converters.BitmapImage
{
    using System;
    using NUnit.Framework;
    using TF3.YarhlPlugin.Common.Converters.BitmapImage;
    using TF3.YarhlPlugin.Common.Converters.Common;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.IO;

    public class ExtractToTgaTests
    {
        private readonly byte[] _validData = { 0x01 };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void NullImageThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });
            var format = new BitmapFileFormat();
            _ = Assert.Throws<NullReferenceException>(() => converter.Convert(format));
        }

        [Test]
        public void UnknownFormatThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData, 0, _validData.Length);
            using var format = new BinaryFormat(ds);

            var reader = new Reader();

            reader.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.A8, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat bmp = reader.Convert(format);

            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = (BitmapExtractionFormat)4 });
            _ = Assert.Throws<FormatException>(() => converter.Convert(bmp));
        }

        [Test]
        public void Export()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData, 0, _validData.Length);
            using var format = new BinaryFormat(ds);

            var reader = new Reader();

            reader.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.A8, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat bmp = reader.Convert(format);

            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });

            // TGA files do not have any magic number, so we can only check if no exception has been thrown
            BinaryFormat result = null;
            Assert.DoesNotThrow(() => result = converter.Convert(bmp));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Stream);
        }
    }
}
