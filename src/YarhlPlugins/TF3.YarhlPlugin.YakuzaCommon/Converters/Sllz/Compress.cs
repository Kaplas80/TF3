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
namespace TF3.YarhlPlugin.YakuzaCommon.Converters.Sllz
{
    using System;
    using TF3.YarhlPlugin.YakuzaCommon.Enums;
    using TF3.YarhlPlugin.YakuzaCommon.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Creates SLLZ files.
    /// </summary>
    public class Compress : IConverter<BinaryFormat, ParFile>, IInitializer<CompressorParameters>
    {
        private CompressorParameters _compressorParameters = new ()
        {
            CompressionType = CompressionType.Standard,
            Endianness = Endianness.LittleEndian,
            OutputStream = null,
        };

        /// <summary>
        /// Initializes the compressor parameters.
        /// </summary>
        /// <param name="parameters">Compressor configuration.</param>
        public void Initialize(CompressorParameters parameters) => _compressorParameters = parameters;

        /// <summary>
        /// Create a SLLZ compressed BinaryFormat.
        /// </summary>
        /// <param name="source">original format.</param>
        /// <returns>The compressed binary.</returns>
        public ParFile Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return _compressorParameters.CompressionType switch
            {
                CompressionType.Standard => (ParFile)ConvertFormat.With<CompressStandard, CompressorParameters>(_compressorParameters, source),
                CompressionType.Zlib => (ParFile)ConvertFormat.With<CompressZlib, CompressorParameters>(_compressorParameters, source),
                _ => throw new FormatException($"SLLZ: Bad Compression Type ({_compressorParameters.CompressionType})")
            };
        }
    }
}
