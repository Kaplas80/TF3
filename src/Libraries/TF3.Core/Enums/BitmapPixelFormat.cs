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
namespace TF3.Core.Enums
{
    /// <summary>
    /// Bitmap pixel formats.
    /// <remarks>https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.PixelFormats.html#structs</remarks>
    /// </summary>
    public enum BitmapPixelFormat
    {
        /// <summary>
        /// Undefined pixel type.
        /// </summary>
        Undefined,

        /// <summary>
        /// Packed pixel type containing a single 8-bit normalized alpha value.
        /// Ranges from [0, 0, 0, 0] to [0, 0, 0, 1] in vector form.
        /// </summary>
        A8,

        /// <summary>
        /// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in alpha, red, green, and blue order (least significant to most significant byte).
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Abgr32,

        /// <summary>
        /// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in alpha, red, green, and blue order (least significant to most significant byte).
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Argb32,

        /// <summary>
        /// Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in blue, green, red order (least significant to most significant byte).
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Bgr24,

        /// <summary>
        /// Packed pixel type containing unsigned normalized values ranging from 0 to 1. The x and z components use 5 bits, and the y component uses 6 bits.
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Bgr565,

        /// <summary>
        /// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in blue, green, red, and alpha order (least significant to most significant byte). The format is binary compatible with System.Drawing.Imaging.PixelFormat.Format32bppArgb
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Bgra32,

        /// <summary>
        /// Packed pixel type containing unsigned normalized values, ranging from 0 to 1, using 4 bits each for x, y, z, and w.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Bgra4444,

        /// <summary>
        /// Packed pixel type containing unsigned normalized values ranging from 0 to 1. The x , y and z components use 5 bits, and the w component uses 1 bit.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Bgra5551,

        /// <summary>
        /// Packed pixel type containing four 8-bit unsigned integer values, ranging from 0 to 255.
        /// Ranges from [0, 0, 0, 0] to [255, 255, 255, 255] in vector form.
        /// </summary>
        Byte4,

        /// <summary>
        /// Packed pixel type containing a single 16 bit floating point value.
        /// Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.
        /// </summary>
        HalfSingle,

        /// <summary>
        /// Packed pixel type containing two 16-bit floating-point values.
        /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
        /// </summary>
        HalfVector2,

        /// <summary>
        /// Packed pixel type containing four 16-bit floating-point values.
        /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        HalfVector4,

        /// <summary>
        /// Packed pixel type containing a single 16-bit normalized luminance value.
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        L16,

        /// <summary>
        /// Packed pixel type containing a single 8-bit normalized luminance value.
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        L8,

        /// <summary>
        /// Packed pixel type containing two 8-bit normalized values representing luminance and alpha.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        La16,

        /// <summary>
        /// Packed pixel type containing two 16-bit normalized values representing luminance and alpha.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        La32,

        /// <summary>
        /// Packed pixel type containing two 8-bit signed normalized values, ranging from −1 to 1.
        /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
        /// </summary>
        NormalizedByte2,

        /// <summary>
        /// Packed pixel type containing four 8-bit signed normalized values, ranging from −1 to 1.
        /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        NormalizedByte4,

        /// <summary>
        /// Packed pixel type containing two 16-bit signed normalized values, ranging from −1 to 1.
        /// Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
        /// </summary>
        NormalizedShort2,

        /// <summary>
        /// Packed pixel type containing four 16-bit signed normalized values, ranging from −1 to 1.
        /// Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        NormalizedShort4,

        /// <summary>
        /// Packed pixel type containing two 16-bit unsigned normalized values ranging from 0 to 1.
        /// Ranges from [0, 0, 0, 1] to [1, 1, 0, 1] in vector form.
        /// </summary>
        Rg32,

        /// <summary>
        /// Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in red, green, blue order (least significant to most significant byte).
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Rgb24,

        /// <summary>
        /// Packed pixel type containing three 16-bit unsigned normalized values ranging from 0 to 635535.
        /// Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Rgb48,

        /// <summary>
        /// Packed vector type containing unsigned normalized values ranging from 0 to 1. The x, y and z components use 10 bits, and the w component uses 2 bits.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Rgba1010102,

        /// <summary>
        /// Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255. The color components are stored in red, green, blue, and alpha order (least significant to most significant byte).
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Rgba32,

        /// <summary>
        /// Packed pixel type containing four 16-bit unsigned normalized values ranging from 0 to 65535.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        Rgba64,

        /// <summary>
        /// Unpacked pixel type containing four 32-bit floating-point values typically ranging from 0 to 1. The color components are stored in red, green, blue, and alpha order.
        /// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
        /// </summary>
        RgbaVector,

        /// <summary>
        /// Packed pixel type containing two 16-bit signed integer values.
        /// Ranges from [-32767, -32767, 0, 1] to [32767, 32767, 0, 1] in vector form.
        /// </summary>
        Short2,

        /// <summary>
        /// Packed pixel type containing four 16-bit signed integer values.
        /// Ranges from [-37267, -37267, -37267, -37267] to [37267, 37267, 37267, 37267] in vector form.
        /// </summary>
        Short4,
    }
}
