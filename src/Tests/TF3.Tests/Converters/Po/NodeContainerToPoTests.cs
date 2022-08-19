namespace TF3.Tests.Converters
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters.Po;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    public class NodeContainerToPoTests
    {
        [Test]
        public void NullSourceThrowsException()
        {
            var converter = new NodeContainerToPo();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void InvalidPoThrowsException()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            using DataStream stream = DataStreamFactory.FromArray(data, 0, 3);
            using var format = new NodeContainerFormat();
            using var binaryFormat = new BinaryFormat(stream);
            using var node = new Node("Node", binaryFormat);
            format.Root.Add(node);

            var converter = new NodeContainerToPo();
            _ = Assert.Throws<FormatException>(() => converter.Convert(format));
        }

        [Test]
        public void OnePo()
        {
            var header = new PoHeader("testId", "reporter", "es");
            var po = new Po(header);
            var entry = new PoEntry("Test") { Translated = "Prueba" };
            po.Add(entry);

            BinaryFormat poBinary = ConvertFormat.With<Po2Binary>(po) as BinaryFormat;

            using var format = new NodeContainerFormat();
            using var node = new Node("Node", poBinary);
            format.Root.Add(node);

            var converter = new NodeContainerToPo();
            Po result = converter.Convert(format);

            Assert.IsNotNull(result);
            Assert.AreEqual(header.ProjectIdVersion, result.Header.ProjectIdVersion);
            Assert.AreEqual(1, result.Entries.Count);
            Assert.AreEqual(entry.Original, result.Entries[0].Original);
            Assert.AreEqual(entry.Translated, result.Entries[0].Translated);
        }

        [Test]
        public void MultiplePo()
        {
            var header = new PoHeader("testId", "reporter", "es");
            var po1 = new Po(header);
            var entry1 = new PoEntry("Test 1") { Translated = "Prueba 1" };
            po1.Add(entry1);

            BinaryFormat poBinary1 = ConvertFormat.With<Po2Binary>(po1) as BinaryFormat;

            var po2 = new Po(header);
            var entry2 = new PoEntry("Test 2") { Translated = "Prueba 2" };
            po2.Add(entry2);

            BinaryFormat poBinary2 = ConvertFormat.With<Po2Binary>(po2) as BinaryFormat;

            using var format = new NodeContainerFormat();
            using var node1 = new Node("Node 1", poBinary1);
            using var node2 = new Node("Node 2", poBinary2);
            format.Root.Add(node1);
            format.Root.Add(node2);

            var converter = new NodeContainerToPo();
            Po result = converter.Convert(format);

            Assert.IsNotNull(result);
            Assert.AreEqual(header.ProjectIdVersion, result.Header.ProjectIdVersion);
            Assert.AreEqual(2, result.Entries.Count);
            Assert.AreEqual(entry1.Original, result.Entries[0].Original);
            Assert.AreEqual(entry1.Translated, result.Entries[0].Translated);
            Assert.AreEqual(entry2.Original, result.Entries[1].Original);
            Assert.AreEqual(entry2.Translated, result.Entries[1].Translated);
        }
    }
}
