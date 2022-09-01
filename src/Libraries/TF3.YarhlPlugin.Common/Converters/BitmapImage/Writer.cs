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
    using System.Runtime.CompilerServices;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using TF3.YarhlPlugin.Common.Enums;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Bitmap file writer.
    /// </summary>
    public class Writer : IConverter<BitmapFileFormat, BinaryFormat>, IInitializer<ImageParameters>
    {
        private ImageParameters _writerParameters = new ImageParameters();

        /// <summary>
        /// Initializes the writer parameters.
        /// </summary>
        /// <param name="parameters">Writer configuration.</param>
        public void Initialize(ImageParameters parameters) => _writerParameters = parameters;

        /// <summary>
        /// Writes a Bitmap file.
        /// </summary>
        /// <param name="source">The Bitmap format.</param>
        /// <returns>The Bitmap file.</returns>
        public BinaryFormat Convert(BitmapFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            byte[] pixelBytes;
            switch (_writerParameters.PixelFormat)
            {
                case BitmapPixelFormat.A8:
                {
                    var image = source.Internal as Image<A8>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<A8>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Abgr32:
                {
                    var image = source.Internal as Image<Abgr32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Abgr32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Argb32:
                {
                    var image = source.Internal as Image<Argb32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Argb32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Bgr24:
                {
                    var image = source.Internal as Image<Bgr24>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgr24>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Bgr565:
                {
                    var image = source.Internal as Image<Bgr565>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgr565>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Bgra32:
                {
                    var image = source.Internal as Image<Bgra32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgra32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Bgra4444:
                {
                    var image = source.Internal as Image<Bgra4444>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgra4444>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Bgra5551:
                {
                    var image = source.Internal as Image<Bgra5551>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Bgra5551>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Byte4:
                {
                    var image = source.Internal as Image<Byte4>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Byte4>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.HalfSingle:
                {
                    var image = source.Internal as Image<HalfSingle>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<HalfSingle>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.HalfVector2:
                {
                    var image = source.Internal as Image<HalfVector2>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<HalfVector2>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.HalfVector4:
                {
                    var image = source.Internal as Image<HalfVector4>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<HalfVector4>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.L16:
                {
                    var image = source.Internal as Image<L16>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<L16>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.L8:
                {
                    var image = source.Internal as Image<L8>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<L8>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.La16:
                {
                    var image = source.Internal as Image<La16>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<La16>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.La32:
                {
                    var image = source.Internal as Image<La32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<La32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.NormalizedByte2:
                {
                    var image = source.Internal as Image<NormalizedByte2>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<NormalizedByte2>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.NormalizedByte4:
                {
                    var image = source.Internal as Image<NormalizedByte4>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<NormalizedByte4>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.NormalizedShort2:
                {
                    var image = source.Internal as Image<NormalizedShort2>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<NormalizedShort2>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.NormalizedShort4:
                {
                    var image = source.Internal as Image<NormalizedShort4>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<NormalizedShort4>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rg32:
                {
                    var image = source.Internal as Image<Rg32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rg32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rgb24:
                {
                    var image = source.Internal as Image<Rgb24>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgb24>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rgb48:
                {
                    var image = source.Internal as Image<Rgb48>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgb48>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rgba1010102:
                {
                    var image = source.Internal as Image<Rgba1010102>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba1010102>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rgba32:
                {
                    var image = source.Internal as Image<Rgba32>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Rgba64:
                {
                    var image = source.Internal as Image<Rgba64>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba64>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.RgbaVector:
                {
                    var image = source.Internal as Image<RgbaVector>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<RgbaVector>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Short2:
                {
                    var image = source.Internal as Image<Short2>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Short2>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                case BitmapPixelFormat.Short4:
                {
                    var image = source.Internal as Image<Short4>;
                    pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Short4>()];
                    image.CopyPixelDataTo(pixelBytes);
                    break;
                }

                default:
                    throw new InvalidOperationException("Unknown image format");
            }

            return new BinaryFormat(DataStreamFactory.FromArray(pixelBytes));
        }
    }
}
