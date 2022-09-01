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

namespace TF3.Tests.Converters.DdsImage
{
    using System;
    using BCnEncoder.Shared.ImageFiles;
    using NUnit.Framework;
    using TF3.YarhlPlugin.Common.Converters.Common;
    using TF3.YarhlPlugin.Common.Converters.DdsImage;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.IO;

    public class ExtractToTgaTests
    {
        private readonly byte[] _validData =
        {
            0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x0A,
            0x00, 0x08, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x40, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20,
            0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x31, 0x30,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x10,
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF,
            0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
            0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
            0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
            0x00, 0xC0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x0F, 0xFC, 0xFF,
            0xFF, 0xFF, 0x10, 0x00, 0x00, 0x00, 0xC0, 0x0F, 0x00, 0x00, 0x00,
            0x00, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void NullDdsThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });
            var format = new DdsFileFormat();
            _ = Assert.Throws<NullReferenceException>(() => converter.Convert(format));
        }

        [Test]
        public void UnknownFormatThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData, 0, _validData.Length);
            var format = new DdsFileFormat() { Internal = DdsFile.Load(ds) };

            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = (BitmapExtractionFormat)4 });
            _ = Assert.Throws<FormatException>(() => converter.Convert(format));
        }

        [Test]
        public void Export()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData, 0, _validData.Length);
            var format = new DdsFileFormat() { Internal = DdsFile.Load(ds) };

            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters { ImageFormat = BitmapExtractionFormat.Tga });

            // TGA files do not have any magic number, so we can only check if no exception has been thrown
            BinaryFormat result = null;
            Assert.DoesNotThrow(() => result = converter.Convert(format));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Stream);
        }
    }
}
