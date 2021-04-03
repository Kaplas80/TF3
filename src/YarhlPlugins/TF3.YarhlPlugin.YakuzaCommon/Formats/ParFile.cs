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

namespace TF3.YarhlPlugin.YakuzaCommon.Formats
{
    using TF3.YarhlPlugin.YakuzaCommon.Types;
    using Yarhl.IO;

    public class ParFile : BinaryFormat
    {
        public ParFileInfo FileInfo { get; set; }

        public ParFile(System.IO.Stream stream)
            : base(stream, 0, stream.Length)
        {
            FileInfo = new ParFileInfo
            {
                Flags = 0x00000000,
                OriginalSize = (uint)stream.Length,
                CompressedSize = (uint)stream.Length,
                DataOffset = 0,
                RawAttributes = 0,
                ExtendedOffset = 0,
                Timestamp = 0,
            };
        }

        public ParFile(ParFileInfo fileInfo, System.IO.Stream stream)
            : base(stream, 0, stream.Length)
        {
            FileInfo = fileInfo;
        }
    }
}
