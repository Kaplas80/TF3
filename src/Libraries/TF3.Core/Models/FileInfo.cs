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

namespace TF3.Core.Models
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using TF3.Core.Helpers;

    /// <summary>
    /// Game file info.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FileInfo
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the container id.
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the file path (inside the container).
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the file checksum.
        /// If it is 0x0, it won't be checked.
        /// </summary>
        [JsonConverter(typeof(HexStringJsonConverter))]
        public ulong Checksum { get; set; }
    }
}
