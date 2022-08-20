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

namespace TF3.Core.Converters.BitmapImage
{
    using System;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Bmp;
    using SixLabors.ImageSharp.PixelFormats;
    using TF3.Core.Enums;
    using TF3.Core.Formats;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Bitmap file reader.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, BitmapFileFormat>, IInitializer<ReaderParameters>
    {
        private ReaderParameters _readerParameters = new ReaderParameters();

        /// <summary>
        /// Initializes the reader parameters.
        /// </summary>
        /// <param name="parameters">Reader configuration.</param>
        public void Initialize(ReaderParameters parameters) => _readerParameters = parameters;

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

            Image image;
            switch (_readerParameters.PixelFormat)
            {
                case BitmapPixelFormat.A8:
                    image = Image.LoadPixelData<A8>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Abgr32:
                    image = Image.LoadPixelData<Abgr32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Argb32:
                    image = Image.LoadPixelData<Argb32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Bgr24:
                    image = Image.LoadPixelData<Bgr24>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Bgr565:
                    image = Image.LoadPixelData<Bgr565>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Bgra32:
                    image = Image.LoadPixelData<Bgra32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Bgra4444:
                    image = Image.LoadPixelData<Bgra4444>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Bgra5551:
                    image = Image.LoadPixelData<Bgra5551>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Byte4:
                    image = Image.LoadPixelData<Byte4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.HalfSingle:
                    image = Image.LoadPixelData<HalfSingle>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.HalfVector2:
                    image = Image.LoadPixelData<HalfVector2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.HalfVector4:
                    image = Image.LoadPixelData<HalfVector4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.L16:
                    image = Image.LoadPixelData<L16>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.L8:
                    image = Image.LoadPixelData<L8>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.La16:
                    image = Image.LoadPixelData<La16>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.La32:
                    image = Image.LoadPixelData<La32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.NormalizedByte2:
                    image = Image.LoadPixelData<NormalizedByte2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.NormalizedByte4:
                    image = Image.LoadPixelData<NormalizedByte4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.NormalizedShort2:
                    image = Image.LoadPixelData<NormalizedShort2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.NormalizedShort4:
                    image = Image.LoadPixelData<NormalizedShort4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rg32:
                    image = Image.LoadPixelData<Rg32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rgb24:
                    image = Image.LoadPixelData<Rgb24>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rgb48:
                    image = Image.LoadPixelData<Rgb48>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rgba1010102:
                    image = Image.LoadPixelData<Rgba1010102>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rgba32:
                    image = Image.LoadPixelData<Rgba32>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Rgba64:
                    image = Image.LoadPixelData<Rgba64>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.RgbaVector:
                    image = Image.LoadPixelData<RgbaVector>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Short2:
                    image = Image.LoadPixelData<Short2>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                case BitmapPixelFormat.Short4:
                    image = Image.LoadPixelData<Short4>(imageData, _readerParameters.ImageWidth, _readerParameters.ImageHeight);
                    break;
                default:
                    throw new FormatException("Unknown image format");
            }

            var result = new BitmapFileFormat()
            {
                Internal = image,
            };

            return result;
        }
    }
}
