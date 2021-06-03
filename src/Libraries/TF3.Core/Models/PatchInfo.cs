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

namespace TF3.Core.Models
{
    /// <summary>
    /// Binary patch info.
    /// </summary>
    public class PatchInfo
    {
        /// <summary>
        /// Gets or sets the patch id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the file to patch.
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// Gets or sets the patch to apply.
        /// </summary>
        public string Patch { get; set; }

        /// <summary>
        /// Gets or sets the data virtual address (in exe files).
        /// </summary>
        public long VirtualAddress { get; set; }

        /// <summary>
        /// Gets or sets the data raw address (in exe files).
        /// </summary>
        public long RawAddress { get; set; }
    }
}
