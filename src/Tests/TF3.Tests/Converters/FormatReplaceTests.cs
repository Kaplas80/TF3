namespace TF3.Tests.Converters
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters;
    using Yarhl.IO;

    public class FormatReplaceTests
    {
        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new FormatReplace();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void UninitializedConverterThrowsException()
        {
            using var format = new BinaryFormat();
            var converter = new FormatReplace();
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void NullInitializationThrowsException()
        {
            using var format = new BinaryFormat();
            var converter = new FormatReplace();
            converter.Initialize(null);
            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void ConvertFormat()
        {
            using var format1 = new BinaryFormat();
            using var format2 = new BinaryFormat();
            var converter = new FormatReplace();
            converter.Initialize(format2);

            BinaryFormat result = converter.Convert(format1) as BinaryFormat;
            Assert.AreSame(format2, result);
            Assert.AreNotSame(format1, result);
        }
    }
}
