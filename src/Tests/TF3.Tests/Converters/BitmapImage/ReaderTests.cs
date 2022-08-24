namespace TF3.Tests.Converters.BitmapImage
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using TF3.Core.Converters.BitmapImage;
    using TF3.Core.Enums;
    using TF3.Core.Formats;
    using Yarhl.IO;

    public class ReaderTests
    {
        private readonly Dictionary<BitmapPixelFormat, byte[]> _validData = new Dictionary<BitmapPixelFormat, byte[]>
        {
            { BitmapPixelFormat.A8, new byte[] { 0x01 } },
            { BitmapPixelFormat.Abgr32, new byte[] { 0x01, 0x10, 0x20, 0x30 } },
            { BitmapPixelFormat.Argb32, new byte[] { 0x01, 0x10, 0x20, 0x30 } },
            { BitmapPixelFormat.Bgr24, new byte[] { 0x10, 0x20, 0x30 } },
            { BitmapPixelFormat.Bgr565, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.Bgra32, new byte[] { 0x10, 0x20, 0x30, 0x01 } },
            { BitmapPixelFormat.Bgra4444, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.Bgra5551, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.Byte4, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.HalfSingle, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.HalfVector2, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.HalfVector4, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.L16, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.L8, new byte[] { 0x10 } },
            { BitmapPixelFormat.La16, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.La32, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.NormalizedByte2, new byte[] { 0x10, 0x20 } },
            { BitmapPixelFormat.NormalizedByte4, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.NormalizedShort2, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.NormalizedShort4, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Rg32, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Rgb24, new byte[] { 0x10, 0x20, 0x30 } },
            { BitmapPixelFormat.Rgb48, new byte[] { 0x10, 0x20, 0x30, 0x10, 0x20, 0x30 } },
            { BitmapPixelFormat.Rgba1010102, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Rgba32, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Rgba64, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.RgbaVector, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Short2, new byte[] { 0x10, 0x20, 0x30, 0x40 } },
            { BitmapPixelFormat.Short4, new byte[] { 0x10, 0x20, 0x30, 0x40, 0x10, 0x20, 0x30, 0x40 } },
        };

        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new Reader();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void InvalidParametersThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData[BitmapPixelFormat.Abgr32], 0, _validData[BitmapPixelFormat.Abgr32].Length);
            using var format = new BinaryFormat(ds);

            var converter = new Reader();

            converter.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Undefined });
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));

            converter.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Rgba32 });
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));

            converter.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Rgba32, ImageWidth = 0 });
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));

            converter.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Rgba32, ImageWidth = 128, ImageHeight = 0 });
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void InvalidFormatThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData[BitmapPixelFormat.A8], 0, _validData[BitmapPixelFormat.A8].Length);
            using var format = new BinaryFormat(ds);

            var converter = new Reader();

            converter.Initialize(new ImageParameters { PixelFormat = (BitmapPixelFormat)1000, ImageWidth = 1, ImageHeight = 1 });
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void InvalidDataThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_validData[BitmapPixelFormat.A8], 0, _validData[BitmapPixelFormat.A8].Length);
            using var format = new BinaryFormat(ds);

            var converter = new Reader();

            converter.Initialize(new ImageParameters { PixelFormat = BitmapPixelFormat.Abgr32, ImageWidth = 1, ImageHeight = 1 });
            Assert.That(() => converter.Convert(format), Throws.Exception);
        }

        [Test]
        public void ReadImage([Values] BitmapPixelFormat pixelFormat)
        {
            if (pixelFormat == BitmapPixelFormat.Undefined)
            {
                Assert.Ignore("Case already tested");
            }

            using DataStream ds = DataStreamFactory.FromArray(_validData[pixelFormat], 0, _validData[pixelFormat].Length);
            using var format = new BinaryFormat(ds);

            var converter = new Reader();

            converter.Initialize(new ImageParameters { PixelFormat = pixelFormat, ImageWidth = 1, ImageHeight = 1 });
            BitmapFileFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Internal);
            Assert.AreEqual(1, result.Internal.Height);
            Assert.AreEqual(1, result.Internal.Width);
        }
    }
}
