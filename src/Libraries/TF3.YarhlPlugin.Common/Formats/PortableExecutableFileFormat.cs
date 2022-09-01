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

namespace TF3.YarhlPlugin.Common.Formats
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using AsmResolver.PE.File;
    using TF3.YarhlPlugin.Common.Models;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// IFormat wrapper for AsmResolver.PE.File.PEFile.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PortableExecutableFileFormat : ICloneableFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortableExecutableFileFormat"/> class.
        /// </summary>
        public PortableExecutableFileFormat()
        {
            Internal = null;
            StringInfo = null;
        }

        /// <summary>
        /// Gets or sets the PEFile.
        /// </summary>
        public PEFile Internal { get; set; }

        /// <summary>
        /// Gets or sets the PEFile.
        /// </summary>
        public List<PortableExecutableStringInfo> StringInfo { get; set; }

        /// <inheritdoc />
        public object DeepClone()
        {
            DataStream newStream = DataStreamFactory.FromMemory();
            Internal.Write(newStream);

            newStream.Position = 0;
            var reader = new DataReader(newStream);
            byte[] data = reader.ReadBytes((int)newStream.Length);
            return new PortableExecutableFileFormat()
            {
                Internal = PEFile.FromBytes(data),
                StringInfo = StringInfo,
            };
        }
    }
}
