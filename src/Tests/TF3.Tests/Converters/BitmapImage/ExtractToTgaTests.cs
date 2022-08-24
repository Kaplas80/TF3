namespace TF3.Tests.Converters.BitmapImage
{
    using System;
    using BCnEncoder.Shared.ImageFiles;
    using NUnit.Framework;
    using TF3.Core.Converters.BitmapImage;
    using TF3.Core.Converters.Common;
    using TF3.Core.Enums;
    using TF3.Core.Formats;
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
