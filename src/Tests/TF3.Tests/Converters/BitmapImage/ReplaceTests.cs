namespace TF3.Tests.Converters.BitmapImage
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using TF3.YarhlPlugin.Common.Converters.BitmapImage;
    using TF3.YarhlPlugin.Common.Converters.BitmapImage.Replace;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.IO;

    public class ReplaceTests
    {
        private readonly Dictionary<BitmapPixelFormat, byte[]> _images = new Dictionary<BitmapPixelFormat, byte[]>
        {
            { BitmapPixelFormat.A8, new byte[] { 0x00 } },
            { BitmapPixelFormat.Abgr32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Argb32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Bgr24, new byte[] { 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Bgr565, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.Bgra32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Bgra4444, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.Bgra5551, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.Byte4, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.HalfSingle, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.HalfVector2, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.HalfVector4, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.L16, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.L8, new byte[] { 0x00 } },
            { BitmapPixelFormat.La16, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.La32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.NormalizedByte2, new byte[] { 0x00, 0x00 } },
            { BitmapPixelFormat.NormalizedByte4, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.NormalizedShort2, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.NormalizedShort4, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rg32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rgb24, new byte[] { 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rgb48, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rgba1010102, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rgba32, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Rgba64, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.RgbaVector, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Short2, new byte[] { 0x00, 0x00, 0x00, 0x00 } },
            { BitmapPixelFormat.Short4, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
        };

        private readonly Dictionary<BitmapPixelFormat, AbstractReplace> _replacers = new Dictionary<BitmapPixelFormat, AbstractReplace>
        {
            { BitmapPixelFormat.A8, new A8Replace() },
            { BitmapPixelFormat.Abgr32, new Abgr32Replace() },
            { BitmapPixelFormat.Argb32, new Argb32Replace() },
            { BitmapPixelFormat.Bgr24, new Bgr24Replace() },
            { BitmapPixelFormat.Bgr565, new Bgr565Replace() },
            { BitmapPixelFormat.Bgra32, new Bgra32Replace() },
            { BitmapPixelFormat.Bgra4444, new Bgra4444Replace() },
            { BitmapPixelFormat.Bgra5551, new Bgra5551Replace() },
            { BitmapPixelFormat.Byte4, new Byte4Replace() },
            { BitmapPixelFormat.HalfSingle, new HalfSingleReplace() },
            { BitmapPixelFormat.HalfVector2, new HalfVector2Replace() },
            { BitmapPixelFormat.HalfVector4, new HalfVector4Replace() },
            { BitmapPixelFormat.L16, new L16Replace() },
            { BitmapPixelFormat.L8, new L8Replace() },
            { BitmapPixelFormat.La16, new La16Replace() },
            { BitmapPixelFormat.La32, new La32Replace() },
            { BitmapPixelFormat.NormalizedByte2, new NormalizedByte2Replace() },
            { BitmapPixelFormat.NormalizedByte4, new NormalizedByte4Replace() },
            { BitmapPixelFormat.NormalizedShort2, new NormalizedShort2Replace() },
            { BitmapPixelFormat.NormalizedShort4, new NormalizedShort4Replace() },
            { BitmapPixelFormat.Rg32, new Rg32Replace() },
            { BitmapPixelFormat.Rgb24, new Rgb24Replace() },
            { BitmapPixelFormat.Rgb48, new Rgb48Replace() },
            { BitmapPixelFormat.Rgba1010102, new Rgba1010102Replace() },
            { BitmapPixelFormat.Rgba32, new Rgba32Replace() },
            { BitmapPixelFormat.Rgba64, new Rgba64Replace() },
            { BitmapPixelFormat.RgbaVector, new RgbaVectorReplace() },
            { BitmapPixelFormat.Short2, new Short2Replace() },
            { BitmapPixelFormat.Short4, new Short4Replace() },
        };

        private readonly byte[] _validPng =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C,
            0x08, 0x02, 0x00, 0x00, 0x00, 0xD9, 0x17, 0xCB,
            0xB0, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47,
            0x42, 0x00, 0xAE, 0xCE, 0x1C, 0xE9, 0x00, 0x00,
            0x00, 0x04, 0x67, 0x41, 0x4D, 0x41, 0x00, 0x00,
            0xB1, 0x8F, 0x0B, 0xFC, 0x61, 0x05, 0x00, 0x00,
            0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00,
            0x0E, 0xC3, 0x00, 0x00, 0x0E, 0xC3, 0x01, 0xC7,
            0x6F, 0xA8, 0x64, 0x00, 0x00, 0x00, 0x1A, 0x49,
            0x44, 0x41, 0x54, 0x28, 0x53, 0x63, 0xFC, 0xFF,
            0xFF, 0x3F, 0x03, 0x21, 0xC0, 0x04, 0xA5, 0xF1,
            0x82, 0x51, 0x45, 0xF4, 0x55, 0xC4, 0xC0, 0x00,
            0x00, 0x3F, 0xA9, 0x03, 0x15, 0x2D, 0xC6, 0xD3,
            0x9B, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E,
            0x44, 0xAE, 0x42, 0x60, 0x82,
        };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new A8Replace(); // does not matter which one select
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void UninitializedConverterThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_images[BitmapPixelFormat.Abgr32], 0, _images[BitmapPixelFormat.Abgr32].Length);
            using var format = new BinaryFormat(ds);

            var reader = new Reader();
            reader.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Abgr32, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat bmp = reader.Convert(format);

            var converter = new A8Replace();
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(bmp));
        }

        [Test]
        public void Replace([Values] BitmapPixelFormat pixelFormat)
        {
            if (pixelFormat == BitmapPixelFormat.Undefined)
            {
                Assert.Ignore("No image with this format");
            }

            byte[] srcImage = _images[pixelFormat];
            AbstractReplace replacer = _replacers[pixelFormat];

            using DataStream ds = DataStreamFactory.FromArray(srcImage, 0, srcImage.Length);
            using DataStream ds2 = DataStreamFactory.FromArray(_validPng, 0, _validPng.Length);
            using var format = new BinaryFormat(ds);
            using var bf = new BinaryFormat(ds2);

            var reader = new Reader();

            reader.Initialize(new ImageParameters { PixelFormat = pixelFormat, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat bmp = reader.Convert(format);

            Assert.AreEqual(1, bmp.Internal.Width);
            Assert.AreEqual(1, bmp.Internal.Height);

            replacer.Initialize(bf);

            BitmapFileFormat result = replacer.Convert(bmp);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Internal);

            Assert.AreEqual(12, result.Internal.Width);
            Assert.AreEqual(12, result.Internal.Height);
        }
    }
}
