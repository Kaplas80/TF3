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

namespace TF3.YarhlPlugin.Common.Converters.BitmapImage
{
    using System;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Bitmap file reader.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, BitmapFileFormat>, IInitializer<ImageParameters>
    {
        private ImageParameters _readerParameters = new ImageParameters();

        /// <summary>
        /// Initializes the reader parameters.
        /// </summary>
        /// <param name="parameters">Reader configuration.</param>
        public void Initialize(ImageParameters parameters) => _readerParameters = parameters;

        /// <summary>
        /// Reads a bitmap file.
        /// </summary>
        /// <param name="source">The image file.</param>
        /// <returns>The image format.</returns>
        public BitmapFileFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Seek(0);

            if ((_readerParameters.PixelFormat == BitmapPixelFormat.Undefined) ||
                (_readerParameters.ImageWidth == 0) ||
                (_readerParameters.ImageHeight == 0))
            {
                throw new InvalidOperationException("Uninitialized image parameters.");
            }

            var reader = new DataReader(source.Stream);
            byte[] imageData = reader.ReadBytes((int)source.Stream.Length);

            Image image = _readerParameters.PixelFormat switch
            {
                BitmapPixelFormat.A8 => Image.LoadPixelData<A8>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Abgr32 => Image.LoadPixelData<Abgr32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Argb32 => Image.LoadPixelData<Argb32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Bgr24 => Image.LoadPixelData<Bgr24>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Bgr565 => Image.LoadPixelData<Bgr565>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Bgra32 => Image.LoadPixelData<Bgra32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Bgra4444 => Image.LoadPixelData<Bgra4444>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Bgra5551 => Image.LoadPixelData<Bgra5551>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Byte4 => Image.LoadPixelData<Byte4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.HalfSingle => Image.LoadPixelData<HalfSingle>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.HalfVector2 => Image.LoadPixelData<HalfVector2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.HalfVector4 => Image.LoadPixelData<HalfVector4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.L16 => Image.LoadPixelData<L16>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.L8 => Image.LoadPixelData<L8>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.La16 => Image.LoadPixelData<La16>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.La32 => Image.LoadPixelData<La32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.NormalizedByte2 => Image.LoadPixelData<NormalizedByte2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.NormalizedByte4 => Image.LoadPixelData<NormalizedByte4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.NormalizedShort2 => Image.LoadPixelData<NormalizedShort2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.NormalizedShort4 => Image.LoadPixelData<NormalizedShort4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rg32 => Image.LoadPixelData<Rg32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rgb24 => Image.LoadPixelData<Rgb24>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rgb48 => Image.LoadPixelData<Rgb48>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rgba1010102 => Image.LoadPixelData<Rgba1010102>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rgba32 => Image.LoadPixelData<Rgba32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Rgba64 => Image.LoadPixelData<Rgba64>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.RgbaVector => Image.LoadPixelData<RgbaVector>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Short2 => Image.LoadPixelData<Short2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                BitmapPixelFormat.Short4 => Image.LoadPixelData<Short4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight),
                _ => throw new InvalidOperationException("Unknown image format"),
            };

            var result = new BitmapFileFormat()
            {
                Internal = image,
            };

            return result;
        }
    }
}
