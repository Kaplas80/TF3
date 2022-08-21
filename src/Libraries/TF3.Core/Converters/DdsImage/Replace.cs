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

namespace TF3.Core.Converters.DdsImage
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using BCnEncoder.Encoder;
    using BCnEncoder.ImageSharp;
    using BCnEncoder.Shared;
    using BCnEncoder.Shared.ImageFiles;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using TF3.Core.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Replaces the original DDS with a new one.
    /// </summary>
    public class Replace : IConverter<DdsFileFormat, DdsFileFormat>, IInitializer<BinaryFormat>
    {
        private Image<Rgba32> _newImage;

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="parameters">New image binary.</param>
        public void Initialize(BinaryFormat parameters)
        {
            parameters.Stream.Seek(0);
            _newImage = Image.Load<Rgba32>(parameters.Stream);
        }

        /// <summary>
        /// Replaces the original DDS with a new one.
        /// </summary>
        /// <param name="source">Original DDS.</param>
        /// <returns>New DDS.</returns>
        public DdsFileFormat Convert(DdsFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_newImage == null)
            {
                throw new InvalidOperationException("Uninitialized");
            }

            var encoder = new BcEncoder()
            {
                OutputOptions =
                {
                    GenerateMipMaps = source.Internal.header.dwMipMapCount > 0,
                    Quality = CompressionQuality.Balanced,
                    Format = GetCompressionFormat(source.Internal),
                    FileFormat = OutputFileFormat.Dds,
                },
            };

            return new DdsFileFormat()
            {
                Internal = encoder.EncodeToDds(_newImage),
            };
        }

        [ExcludeFromCodeCoverage(Justification = "Too many cases")]
        private static CompressionFormat GetCompressionFormat(DdsFile dds)
        {
            DxgiFormat format = dds.header.ddsPixelFormat.IsDxt10Format ? dds.dx10Header.dxgiFormat : dds.header.ddsPixelFormat.DxgiFormat;

            return format switch
            {
                DxgiFormat.DxgiFormatR32G32B32A32Typeless or DxgiFormat.DxgiFormatR32G32B32A32Float or DxgiFormat.DxgiFormatR32G32B32A32Uint or DxgiFormat.DxgiFormatR32G32B32A32Sint => CompressionFormat.Rgba,
                DxgiFormat.DxgiFormatR32G32B32Typeless or DxgiFormat.DxgiFormatR32G32B32Float or DxgiFormat.DxgiFormatR32G32B32Uint or DxgiFormat.DxgiFormatR32G32B32Sint => CompressionFormat.Rgb,
                DxgiFormat.DxgiFormatR16G16B16A16Typeless or DxgiFormat.DxgiFormatR16G16B16A16Float or DxgiFormat.DxgiFormatR16G16B16A16Unorm or DxgiFormat.DxgiFormatR16G16B16A16Uint or DxgiFormat.DxgiFormatR16G16B16A16Snorm or DxgiFormat.DxgiFormatR16G16B16A16Sint => CompressionFormat.Rgba,
                DxgiFormat.DxgiFormatR32G32Typeless or DxgiFormat.DxgiFormatR32G32Float or DxgiFormat.DxgiFormatR32G32Uint or DxgiFormat.DxgiFormatR32G32Sint => CompressionFormat.Rg,
                DxgiFormat.DxgiFormatR10G10B10A2Typeless or DxgiFormat.DxgiFormatR10G10B10A2Unorm or DxgiFormat.DxgiFormatR10G10B10A2Uint or DxgiFormat.DxgiFormatR11G11B10Float or DxgiFormat.DxgiFormatR8G8B8A8Typeless or DxgiFormat.DxgiFormatR8G8B8A8Unorm or DxgiFormat.DxgiFormatR8G8B8A8UnormSrgb or DxgiFormat.DxgiFormatR8G8B8A8Uint or DxgiFormat.DxgiFormatR8G8B8A8Snorm or DxgiFormat.DxgiFormatR8G8B8A8Sint => CompressionFormat.Rgba,
                DxgiFormat.DxgiFormatR16G16Typeless or DxgiFormat.DxgiFormatR16G16Float or DxgiFormat.DxgiFormatR16G16Unorm or DxgiFormat.DxgiFormatR16G16Uint or DxgiFormat.DxgiFormatR16G16Snorm or DxgiFormat.DxgiFormatR16G16Sint => CompressionFormat.Rg,
                DxgiFormat.DxgiFormatR32Typeless or DxgiFormat.DxgiFormatR32Float or DxgiFormat.DxgiFormatR32Uint or DxgiFormat.DxgiFormatR32Sint => CompressionFormat.R,
                DxgiFormat.DxgiFormatR24G8Typeless or DxgiFormat.DxgiFormatR8G8Typeless or DxgiFormat.DxgiFormatR8G8Unorm or DxgiFormat.DxgiFormatR8G8Uint or DxgiFormat.DxgiFormatR8G8Snorm or DxgiFormat.DxgiFormatR8G8Sint => CompressionFormat.Rg,
                DxgiFormat.DxgiFormatR16Typeless or DxgiFormat.DxgiFormatR16Float or DxgiFormat.DxgiFormatR16Unorm or DxgiFormat.DxgiFormatR16Uint or DxgiFormat.DxgiFormatR16Snorm or DxgiFormat.DxgiFormatR16Sint or DxgiFormat.DxgiFormatR8Typeless or DxgiFormat.DxgiFormatR8Unorm or DxgiFormat.DxgiFormatR8Uint or DxgiFormat.DxgiFormatR8Snorm or DxgiFormat.DxgiFormatR8Sint => CompressionFormat.R,
                DxgiFormat.DxgiFormatBc1Typeless or DxgiFormat.DxgiFormatBc1Unorm or DxgiFormat.DxgiFormatBc1UnormSrgb => CompressionFormat.Bc1,
                DxgiFormat.DxgiFormatBc2Typeless or DxgiFormat.DxgiFormatBc2Unorm or DxgiFormat.DxgiFormatBc2UnormSrgb => CompressionFormat.Bc2,
                DxgiFormat.DxgiFormatBc3Typeless or DxgiFormat.DxgiFormatBc3Unorm or DxgiFormat.DxgiFormatBc3UnormSrgb => CompressionFormat.Bc3,
                DxgiFormat.DxgiFormatBc4Typeless or DxgiFormat.DxgiFormatBc4Unorm or DxgiFormat.DxgiFormatBc4Snorm => CompressionFormat.Bc4,
                DxgiFormat.DxgiFormatBc5Typeless or DxgiFormat.DxgiFormatBc5Unorm or DxgiFormat.DxgiFormatBc5Snorm => CompressionFormat.Bc5,
                DxgiFormat.DxgiFormatB5G5R5A1Unorm or DxgiFormat.DxgiFormatB8G8R8A8Unorm or DxgiFormat.DxgiFormatB8G8R8A8Typeless or DxgiFormat.DxgiFormatB8G8R8A8UnormSrgb => CompressionFormat.Bgra,
                DxgiFormat.DxgiFormatBc6HTypeless or DxgiFormat.DxgiFormatBc6HUf16 => CompressionFormat.Bc6U,
                DxgiFormat.DxgiFormatBc6HSf16 => CompressionFormat.Bc6S,
                DxgiFormat.DxgiFormatBc7Typeless or DxgiFormat.DxgiFormatBc7Unorm or DxgiFormat.DxgiFormatBc7UnormSrgb => CompressionFormat.Bc7,
                DxgiFormat.DxgiFormatAtcExt => CompressionFormat.Atc,
                DxgiFormat.DxgiFormatAtcExplicitAlphaExt => CompressionFormat.AtcExplicitAlpha,
                DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt => CompressionFormat.AtcInterpolatedAlpha,
                _ => throw new FormatException($"Unknown DxgiFormat: {format}"),
            };
        }
    }
}
