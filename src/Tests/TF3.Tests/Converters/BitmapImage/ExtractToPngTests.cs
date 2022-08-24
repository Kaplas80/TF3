namespace TF3.Tests.Converters.BitmapImage
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters.BitmapImage;
    using TF3.Core.Converters.Common;
    using TF3.Core.Formats;
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

            reader.Initialize(new ImageParameters { PixelFormat = Core.Enums.BitmapPixelFormat.A8, ImageWidth = 1, ImageHeight = 1 });
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
