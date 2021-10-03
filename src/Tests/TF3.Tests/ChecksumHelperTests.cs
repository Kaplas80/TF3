namespace TF3.Tests
{
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using TF3.Core.Helpers;

    public class ChecksumHelperTests
    {
        private const string TestData = "Test Data";

        private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "TF3.Tests");

        [SetUp]
        public void Init()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }

            Directory.CreateDirectory(_tempPath);
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }
        }

        [Test]
        public void CheckFile()
        {
            string filePath = Path.Combine(_tempPath, "ChecksumTest.txt");
            File.WriteAllText(filePath, TestData);
            bool result = ChecksumHelper.Check(filePath, 0x5AB1599F68E64610);
            Assert.IsTrue(result);
            result = ChecksumHelper.Check(filePath, 0x01);
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckStream()
        {
            using MemoryStream stream = new (Encoding.ASCII.GetBytes(TestData));
            bool result = ChecksumHelper.Check(stream, 0x5AB1599F68E64610);
            Assert.IsTrue(result);
            result = ChecksumHelper.Check(stream, 0x01);
            Assert.IsFalse(result);
        }

        [Test]
        public void InvalidPathThrowsException()
        {
            string filePath = Path.Combine(_tempPath, "dummy.txt");
            Assert.Throws<FileNotFoundException>(() => ChecksumHelper.Check(filePath, 0x01));
        }

        [Test]
        public void ExpectZeroReturnsTrue()
        {
            string filePath = Path.Combine(_tempPath, "ChecksumTest.txt");
            File.WriteAllText(filePath, TestData);
            Assert.IsTrue(ChecksumHelper.Check(filePath, 0x00));

            using MemoryStream stream = new (Encoding.ASCII.GetBytes(TestData));
            Assert.IsTrue(ChecksumHelper.Check(stream, 0x00));
        }
    }
}
