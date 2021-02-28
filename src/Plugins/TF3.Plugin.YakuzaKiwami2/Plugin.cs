// Copyright (c) 2021 Kaplas
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

namespace TF3.Plugin.YakuzaKiwami2
{
    using TF3.Common.Core;
    using TF3.Common.Core.Enums;

    /// <summary>
    /// Yakuza Kiwami 2 plugin.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <inheritdoc/>
        public string Id => "ef6d1df4-dacc-4d3a-aa77-8e0f25bb21b4";

        /// <inheritdoc/>
        public string Game => "Yakuza Kiwami 2 (Steam version, build 4058515)";

        /// <inheritdoc/>
        public string Name => "kiwami2pc";

        /// <inheritdoc/>
        public Platform Platform => Platform.Any;

        /// <inheritdoc/>
        public void ExtractTexts(string installationPath, string outputPath)
        {
            // Method intentionally left empty.
        }
    }
}
