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

namespace TF3.YarhlPlugin.Common.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using TF3.YarhlPlugin.Common.Helpers;

    /// <summary>
    /// Parameters for Portable Executable reading.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PortableExecutableStringInfo
    {
        /// <summary>
        /// Gets or sets the string found inside the executable file.
        /// </summary>
        public string FoundString { get; set; }

        /// <summary>
        /// Gets or sets the string address inside the executable file.
        /// </summary>
        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes of the string.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the string encoding.
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the list of pointers referencing the string.
        /// </summary>
        [JsonConverter(typeof(HexStringListJsonConverter))]
        public List<int> Pointers { get; set; }
    }
}
