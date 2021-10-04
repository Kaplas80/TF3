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
            NodeContainerToPo converter = new ();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void InvalidPoThrowsException()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            using DataStream stream = DataStreamFactory.FromArray(data, 0, 3);
            using NodeContainerFormat format = new ();
            using BinaryFormat binaryFormat = new (stream);
            using Node node = new ("Node", binaryFormat);
            format.Root.Add(node);

            NodeContainerToPo converter = new ();
            _ = Assert.Throws<FormatException>(() => converter.Convert(format));
        }

        [Test]
        public void OnePo()
        {
            PoHeader header = new ("testId", "reporter", "es");
            Po po = new (header);
            PoEntry entry = new ("Test") { Translated = "Prueba" };
            po.Add(entry);

            BinaryFormat poBinary = ConvertFormat.With<Po2Binary>(po) as BinaryFormat;

            using NodeContainerFormat format = new ();
            using Node node = new ("Node", poBinary);
            format.Root.Add(node);

            NodeContainerToPo converter = new ();
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
            PoHeader header = new ("testId", "reporter", "es");
            Po po1 = new (header);
            PoEntry entry1 = new ("Test 1") { Translated = "Prueba 1" };
            po1.Add(entry1);

            BinaryFormat poBinary1 = ConvertFormat.With<Po2Binary>(po1) as BinaryFormat;

            Po po2 = new (header);
            PoEntry entry2 = new ("Test 2") { Translated = "Prueba 2" };
            po2.Add(entry2);

            BinaryFormat poBinary2 = ConvertFormat.With<Po2Binary>(po2) as BinaryFormat;

            using NodeContainerFormat format = new ();
            using Node node1 = new ("Node 1", poBinary1);
            using Node node2 = new ("Node 2", poBinary2);
            format.Root.Add(node1);
            format.Root.Add(node2);

            NodeContainerToPo converter = new ();
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
