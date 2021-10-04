namespace TF3.Tests.Converters.BinaryPatch
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters.BinaryPatch;
    using TF3.Core.Formats;
    using Yarhl.IO;

    public class ReaderTests
    {
        [Test]
        public void NullSourceThrowsException()
        {
            Reader converter = new ();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void NoFileNameThrowsException()
        {
            const string patch = "0000000000123456:01->02";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            _ = Assert.Throws<FormatException>(() => converter.Convert(textFormat));
        }

        [Test]
        public void NoPatchesThrowsException()
        {
            const string patch = ";Test\n>test.exe";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            _ = Assert.Throws<FormatException>(() => converter.Convert(textFormat));
        }

        [Test]
        public void UninitializedConverter()
        {
            const string patch = ">test.exe\n0000000000123456:01->02";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            BinaryPatch result = converter.Convert(textFormat);

            Assert.IsNotNull(result);
            Assert.AreEqual("test.exe", result.FileName);
            Assert.AreEqual(1, result.Patches.Count);
            Assert.AreEqual(0x123456, result.Patches[0].Rva);
            Assert.AreEqual(0x01, result.Patches[0].OriginalByte);
            Assert.AreEqual(0x02, result.Patches[0].NewByte);
            Assert.AreEqual(0, result.RawOffset);
        }

        [Test]
        public void InitializedConverter()
        {
            const string patch = ">test.exe\n0000000000123456:01->02";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            converter.Initialize(1000);
            BinaryPatch result = converter.Convert(textFormat);

            Assert.IsNotNull(result);
            Assert.AreEqual("test.exe", result.FileName);
            Assert.AreEqual(1, result.Patches.Count);
            Assert.AreEqual(0x123456, result.Patches[0].Rva);
            Assert.AreEqual(0x01, result.Patches[0].OriginalByte);
            Assert.AreEqual(0x02, result.Patches[0].NewByte);
            Assert.AreEqual(1000, result.RawOffset);
        }

        [Test]
        public void ReadEmptyStrings()
        {
            const string patch = ">test.exe\n    \n0000000000123456:01->02";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            BinaryPatch result = converter.Convert(textFormat);

            Assert.IsNotNull(result);
            Assert.AreEqual("test.exe", result.FileName);
            Assert.AreEqual(1, result.Patches.Count);
            Assert.AreEqual(0x123456, result.Patches[0].Rva);
            Assert.AreEqual(0x01, result.Patches[0].OriginalByte);
            Assert.AreEqual(0x02, result.Patches[0].NewByte);
        }

        [Test]
        public void ReadComments()
        {
            const string patch = ";This is a comment\n>test.exe\n0000000000123456:01->02;This is an inlined comment\n;This is another comment";
            using BinaryFormat textFormat = new ();
            new TextDataWriter(textFormat.Stream).Write(patch);

            Reader converter = new ();
            BinaryPatch result = converter.Convert(textFormat);

            Assert.IsNotNull(result);
            Assert.AreEqual("test.exe", result.FileName);
            Assert.AreEqual(1, result.Patches.Count);
            Assert.AreEqual(0x123456, result.Patches[0].Rva);
            Assert.AreEqual(0x01, result.Patches[0].OriginalByte);
            Assert.AreEqual(0x02, result.Patches[0].NewByte);
        }
    }
}
