// Copyright (c) 2022 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace TF3.Tests
{
    using System.IO;
    using NUnit.Framework;

    public class ScriptManagerTests
    {
        private const string TestScript = /*lang=json,strict*/ "{\"Name\":\"test-script\",\"Game\":\"Test Script\",\"Parameters\":[],\"Containers\":[],\"Assets\":[],\"Patches\":[]}";
        private const string InvalidTestScript = /*lang=json,strict*/ "{\"Name\":\"test-script\",\"Game_#$\":\"Test Script\",\"Parameters\":[],\"Containers\":[],\"Assets\":[],\"Patches\":[]}";

        private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "TF3.Tests");

        [SetUp]
        public void Init()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }

            _ = Directory.CreateDirectory(_tempPath);
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
        public void PathWithInvalidScriptsCallsEvent()
        {
            string scriptPath = Path.Combine(_tempPath, "TF3.Script.Test.json");
            File.WriteAllText(scriptPath, InvalidTestScript);

            int calls = 0;
            Core.ScriptManager.ErrorLoading += (object _, (string file, string message) _) => calls++;

            Assert.AreEqual(0, Core.ScriptManager.Scripts.Count);
            Core.ScriptManager.LoadScripts(_tempPath);
            Assert.AreEqual(1, calls);
        }
    }
}
