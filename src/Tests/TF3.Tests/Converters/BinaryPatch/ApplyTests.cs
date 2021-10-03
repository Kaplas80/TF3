namespace TF3.Tests.Converters.BinaryPatch
{
    using System;
    using NUnit.Framework;
    using TF3.Core.Converters.BinaryPatch;
    using TF3.Core.Exceptions;
    using TF3.Core.Formats;
    using Yarhl.IO;

    public class ApplyTests
    {
        private readonly byte[] _testData = { 0x54, 0x65, 0x73, 0x74, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67 };

        [Test]
        public void NullSourceThrowsException()
        {
            Apply converter = new ();
            _ = Assert.Throws<ArgumentNullException>(() => converter.Convert(null));
        }

        [Test]
        public void UninitializedConverterThrowsException()
        {
            using DataStream ds = DataStreamFactory.FromArray(_testData, 0, _testData.Length);
            using BinaryFormat format = new (ds);
            Apply converter = new ();

            _ = Assert.Throws<InvalidOperationException>(() => converter.Convert(format));
        }

        [Test]
        public void MismatchByteThrowsException()
        {
            BinaryPatch patch = new () { FileName = "test.exe", RawOffset = 0, };
            patch.Patches.Add((4, 0x21, 0x30));

            using DataStream ds = DataStreamFactory.FromArray(_testData, 0, _testData.Length);
            using BinaryFormat format = new (ds);
            Apply converter = new ();
            converter.Initialize(patch);
            _ = Assert.Throws<ByteMismatchException>(() => converter.Convert(format));
        }

        [Test]
        public void ApplyPatchWithoutOffset()
        {
            BinaryPatch patch = new () { FileName = "test.exe", RawOffset = 0, };
            patch.Patches.Add((4, 0x20, 0x30));

            using DataStream ds = DataStreamFactory.FromArray(_testData, 0, _testData.Length);
            using BinaryFormat format = new (ds);
            Apply converter = new ();
            converter.Initialize(patch);
            converter.Convert(format);

            DataReader reader = new (ds);
            ds.Position = 4;

            byte changedByte = reader.ReadByte();
            Assert.AreEqual(0x30, changedByte);
        }

        [Test]
        public void ApplyPatchWithOffset()
        {
            BinaryPatch patch = new () { FileName = "test.exe", RawOffset = 3, };
            patch.Patches.Add((4, 0x72, 0x62));

            using DataStream ds = DataStreamFactory.FromArray(_testData, 0, _testData.Length);
            using BinaryFormat format = new (ds);
            Apply converter = new ();
            converter.Initialize(patch);
            converter.Convert(format);

            DataReader reader = new (ds);
            ds.Position = 7; // 4 + 3

            byte changedByte = reader.ReadByte();
            Assert.AreEqual(0x62, changedByte);
        }
    }
}
