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
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.IO;

    public class ExtractToPngTests
    {
        private readonly byte[] _validData = { 0x01 };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters());
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void NullImageThrowsException()
        {
            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters());
            var format = new BitmapFileFormat();
            _ = Assert.Throws<NullReferenceException>(() => converter.Convert(format));
        }

        [Test]
        public void Export()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData, 0, _validData.Length);
            using var format = new BinaryFormat(ds);

            var reader = new Reader();

            reader.Initialize(new ImageParameters { PixelFormat = YarhlPlugin.Common.Enums.BitmapPixelFormat.A8, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat bmp = reader.Convert(format);

            var converter = new Extractor();
            converter.Initialize(new ImageExtractorParameters());

            BinaryFormat result = converter.Convert(bmp);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Stream);

            result.Stream.Position = 0;
            var reader1 = new DataReader(result.Stream);
            ulong magic = reader1.ReadUInt64();

            Assert.AreEqual(727905341920923785, magic);
        }
    }
}
