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
namespace TF3.Common.Core
{
    using TF3.Common.Core.Enums;

    /// <summary>
    /// Interface for TF3 plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the plugin ID.
        /// <remarks>It must be UNIQUE, so using a generated Guid is recommended.</remarks>
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the plugin name alias (short name).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the plugin game name.
        /// </summary>
        string Game { get; }

        /// <summary>
        /// Gets the platform where the plugin is useable.
        /// </summary>
        Platform Platform { get; }

        /// <summary>
        /// Extract texts from game files.
        /// </summary>
        /// <param name="installationPath">Game installation path (to read from).</param>
        /// <param name="outputPath">Output directory.</param>
        void ExtractTexts(string installationPath, string outputPath);
    }
}
