namespace TF3.Tests.Converters
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class FormatToSingleNodeTests
    {
        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new FormatToSingleNode();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void UninitializedConverter()
        {
            using var format = new BinaryFormat();
            var converter = new FormatToSingleNode();
            NodeContainerFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Root.Children.Count);
            Assert.AreEqual("single_node", result.Root.Children[0].Name);
        }

        [Test]
        public void EmptyStringInitialization()
        {
            using var format = new BinaryFormat();
            var converter = new FormatToSingleNode();
            converter.Initialize(string.Empty);

            NodeContainerFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Root.Children.Count);
            Assert.AreEqual("single_node", result.Root.Children[0].Name);
        }

        [Test]
        public void InitializeConverter()
        {
            using var format = new BinaryFormat();
            var converter = new FormatToSingleNode();
            converter.Initialize("test_name");

            NodeContainerFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Root.Children.Count);
            Assert.AreEqual("test_name", result.Root.Children[0].Name);
        }

        [Test]
        public void ConvertCloneableFormat()
        {
            using var format = new BinaryFormat();
            var converter = new FormatToSingleNode();

            NodeContainerFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Root.Children.Count);

            Assert.AreNotSame(format, result.Root.Children[0].GetFormatAs<BinaryFormat>());
        }

        [Test]
        public void ConvertNotCloneableFormat()
        {
            var format = new NotCloneableFormat();
            var converter = new FormatToSingleNode();

            NodeContainerFormat result = converter.Convert(format);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Root.Children.Count);

            Assert.AreSame(format, result.Root.Children[0].GetFormatAs<NotCloneableFormat>());
        }

        private class NotCloneableFormat : IFormat
        {
        }
    }
}
