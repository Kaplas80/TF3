namespace TF3.Tests
{
    using System.IO;
    using System.Text.Json;
    using NUnit.Framework;

    public class GameScriptTests
    {
        private const string TestScript = "{\"Name\":\"test-script\",\"Game\":\"Test Script\",\"Parameters\":[],\"Containers\":[],\"Assets\":[],\"Patches\":[]}";
        private const string InvalidTestScript = "{\"Name\":\"test-script\",\"Game_#$\":\"Test Script\",\"Parameters\":[],\"Containers\":[],\"Assets\":[],\"Patches\":[]}";

        private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "TF3.Tests");

        [SetUp]
        public void Init()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }

            Directory.CreateDirectory(_tempPath);
            Core.ScriptManager.Clear();
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
        public void PathWithScriptsWorks()
        {
            string scriptPath = Path.Combine(_tempPath, "TF3.Script.Test.json");
            File.WriteAllText(scriptPath, TestScript);

            Assert.AreEqual(0, Core.ScriptManager.Scripts.Count);
            Core.ScriptManager.LoadScripts(_tempPath);
            Assert.AreEqual(1, Core.ScriptManager.Scripts.Count);
        }

        [Test]
        public void PathWithoutScriptsDoesNotThrow()
        {
            Assert.AreEqual(0, Core.ScriptManager.Scripts.Count);
            Core.ScriptManager.LoadScripts(_tempPath);
            Assert.AreEqual(0, Core.ScriptManager.Scripts.Count);
        }

        [Test]
        public void PathWithInvalidScriptsThrowsException()
        {
            string scriptPath = Path.Combine(_tempPath, "TF3.Script.Test.json");
            File.WriteAllText(scriptPath, InvalidTestScript);

            Assert.AreEqual(0, Core.ScriptManager.Scripts.Count);
            Assert.Throws<JsonException>(() => Core.ScriptManager.LoadScripts(_tempPath));
        }
    }
}
