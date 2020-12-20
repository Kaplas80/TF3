// Copyright (c) 2020 Kaplas
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
namespace TF3.Common.Yakuza.Types
{
    using Yarhl.IO.Serialization.Attributes;

    /// <summary>
    /// Par directory info.
    /// </summary>
    [Serializable]
    public class ParDirectoryInfo
    {
        /// <summary>
        /// Gets or sets the number of subdirectories inside this directory.
        /// </summary>
        public uint SubdirectoryCount { get; set; }

        /// <summary>
        /// Gets or sets the index of the first subdirectory.
        /// </summary>
        public uint SubdirectoryStartIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of files inside this directory (not in subdirectories).
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// Gets or sets the index of the first file.
        /// </summary>
        public uint FileStartIndex { get; set; }

        /// <summary>
        /// Gets or sets the attributes of the directory.
        /// </summary>
        public uint RawAttributes { get; set; }
    }
}
