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

namespace TF3.CommandLine.Options
{
    using global::CommandLine;

    /// <summary>
    /// Rebuild editable contents.
    /// </summary>
    [Verb("rebuild", HelpText = "Rebuild editable contents.")]
    public class RebuildOptions
    {
        /// <summary>
        /// Gets or sets the script to use.
        /// </summary>
        [Option("script", Required = true, HelpText = "Script to use")]
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the game directory.
        /// </summary>
        [Option("install-dir", Required = true, HelpText = "Game directory.")]
        public string GameDir { get; set; }

        /// <summary>
        /// Gets or sets the translation directory.
        /// </summary>
        [Option("translation-dir", Required = true, HelpText = "Translation directory.")]
        public string TranslationDir { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Option("output-dir", Required = true, HelpText = "Output directory.")]
        public string Output { get; set; }
    }
}
