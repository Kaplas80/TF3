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
    using TF3.Common.Yakuza.Enums;
    using Yarhl.IO.Serialization.Attributes;

    /// <summary>
    /// SLLZ compressed file header.
    /// </summary>
    [Serializable]
    public class SllzHeader
    {
        /// <summary>
        /// Gets or sets the file magic number: "SLLZ".
        /// </summary>
        [BinaryString(FixedSize = 4, Terminator = null)]
        public string Magic { get; set; }

        /// <summary>
        /// Gets or sets the file endianness.
        /// </summary>
        [BinaryEnum(ReadAs = typeof(byte), WriteAs = typeof(byte))]
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Gets or sets the compressor type.
        /// </summary>
        [BinaryEnum(ReadAs = typeof(byte), WriteAs = typeof(byte))]
        public CompressionType CompressionType { get; set; }

        /// <summary>
        /// Gets or sets the header size.
        /// </summary>
        public ushort HeaderSize { get; set; }

        /// <summary>
        /// Gets or sets the original file size.
        /// </summary>
        public uint OriginalSize { get; set; }

        /// <summary>
        /// Gets or sets the compressed file size.
        /// <remarks>Includes the header size.</remarks>
        /// </summary>
        public uint CompressedSize { get; set; }
    }
}
