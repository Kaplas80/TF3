namespace TF3.Tests.Converters
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class SingleNodeToFormatTests
    {
        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new SingleNodeToFormat();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void MoreThanOneChildThrowsException()
        {
            using var format = new NodeContainerFormat();
            using var node1 = new Node("Node1");
            using var node2 = new Node("Node2");
            format.Root.Add(node1);
            format.Root.Add(node2);

            var converter = new SingleNodeToFormat();
            _ = Assert.Throws<FormatException>(() => converter.Convert(format));
        }

        [Test]
        public void ConvertCloneableFormat()
        {
            using var format = new NodeContainerFormat();
            using var binaryFormat = new BinaryFormat();
            using var node = new Node("Node", binaryFormat);
            format.Root.Add(node);

            var converter = new SingleNodeToFormat();

            BinaryFormat result = converter.Convert(format) as BinaryFormat;
            Assert.IsNotNull(result);

            Assert.AreNotSame(binaryFormat, result);
        }

        [Test]
        public void ConvertNotCloneableFormat()
        {
            using var format = new NodeContainerFormat();
            var notCloneable = new NotCloneableFormat();
            using var node = new Node("Node", notCloneable);
            format.Root.Add(node);

            var converter = new SingleNodeToFormat();

            NotCloneableFormat result = converter.Convert(format) as NotCloneableFormat;
            Assert.IsNotNull(result);

            Assert.AreSame(notCloneable, result);
        }

        private class NotCloneableFormat : IFormat
        {
        }
    }
}
