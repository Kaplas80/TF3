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
    using TF3.YarhlPlugin.Common.Converters.DdsImage;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.IO;

    public class ReplaceTests
    {
        private readonly byte[] _validDds = { 0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x0A, 0x00, 0x08, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x31, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x10, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x62, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0xC0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x0F, 0xFC, 0xFF, 0xFF, 0xFF, 0x10, 0x00, 0x00, 0x00, 0xC0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };
        private readonly byte[] _validPng = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x08, 0x02, 0x00, 0x00, 0x00, 0xD9, 0x17, 0xCB, 0xB0, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xAE, 0xCE, 0x1C, 0xE9, 0x00, 0x00, 0x00, 0x04, 0x67, 0x41, 0x4D, 0x41, 0x00, 0x00, 0xB1, 0x8F, 0x0B, 0xFC, 0x61, 0x05, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0E, 0xC3, 0x00, 0x00, 0x0E, 0xC3, 0x01, 0xC7, 0x6F, 0xA8, 0x64, 0x00, 0x00, 0x00, 0x1A, 0x49, 0x44, 0x41, 0x54, 0x28, 0x53, 0x63, 0xFC, 0xFF, 0xFF, 0x3F, 0x03, 0x21, 0xC0, 0x04, 0xA5, 0xF1, 0x82, 0x51, 0x45, 0xF4, 0x55, 0xC4, 0xC0, 0x00, 0x00, 0x3F, 0xA9, 0x03, 0x15, 0x2D, 0xC6, 0xD3, 0x9B, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        private readonly byte[] _invalidData = { 0x54, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67 };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new Replace();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void UninitializedConverterThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validDds, 0, _validDds.Length);
            var format = new DdsFileFormat()
            {
                Internal = DdsFile.Load(ds),
            };

            var converter = new Replace();

            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void InvalidReplacementImageThrowsException()
        {
            using DataStream ds2 = DataStreamFactory.FromArray(_invalidData, 0, _invalidData.Length);
            var bf = new BinaryFormat(ds2);

            var converter = new Replace();
            _ = Assert.Throws<SixLabors.ImageSharp.UnknownImageFormatException>(() => converter.Initialize(bf));
        }

        [Test]
        public void WriteDds()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validDds, 0, _validDds.Length);
            using DataStream ds2 = DataStreamFactory.FromArray(_validPng, 0, _validPng.Length);
            var format = new DdsFileFormat()
            {
                Internal = DdsFile.Load(ds),
            };
            var bf = new BinaryFormat(ds2);

            var converter = new Replace();
            converter.Initialize(bf);

            Assert.AreEqual(8, format.Internal.header.dwWidth);
            Assert.AreEqual(8, format.Internal.header.dwHeight);

            DdsFileFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Internal);

            Assert.AreEqual(12, result.Internal.header.dwWidth);
            Assert.AreEqual(12, result.Internal.header.dwHeight);
        }
    }
}
