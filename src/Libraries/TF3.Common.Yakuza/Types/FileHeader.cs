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
namespace TF3.Common.Yakuza.Types
{
    using TF3.Common.Yakuza.Enums;
    using Yarhl.IO.Serialization.Attributes;

    /// <summary>
    /// Archive header.
    /// </summary>
    [Serializable]
    public class FileHeader
    {
        /// <summary>
        /// Gets or sets the file magic number.
        /// </summary>
        [BinaryString(FixedSize = 4, Terminator = null)]
        public string Magic { get; set; }

        /// <summary>
        /// Gets or sets the platform id value.
        /// </summary>
        [BinaryEnum(ReadAs = typeof(byte), WriteAs = typeof(byte))]
        public Platform PlatformId { get; set; }

        /// <summary>
        /// Gets or sets the file endianness.
        /// </summary>
        [BinaryEnum(ReadAs = typeof(byte), WriteAs = typeof(byte))]
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Gets or sets the size extended byte.
        /// </summary>
        public byte SizeExtended { get; set; }

        /// <summary>
        /// Gets or sets the relocated byte.
        /// </summary>
        public byte Relocated { get; set; }

        /// <summary>
        /// Gets or sets the version value.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets the data size.
        /// <remarks>In newer versions it is always 0.</remarks>
        /// </summary>
        public uint Size { get; set; }
    }
}
