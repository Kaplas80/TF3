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
namespace TF3.Common.Yakuza.Enums
{
    /// <summary>
    /// Platforms ids.
    /// <remarks>Only <see cref="PlayStation3"/> is used in PAR archives.</remarks>
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Microsoft Windows 32bits
        /// </summary>
        Win32 = 0x0,

        /// <summary>
        /// Microsoft Xbox 360
        /// </summary>
        Xbox360 = 0x1,

        /// <summary>
        /// Sony PlayStation 3
        /// </summary>
        PlayStation3 = 0x2,

        /// <summary>
        /// Nintendo Wii
        /// </summary>
        Wii = 0x3,

        /// <summary>
        /// Sony PS Vita
        /// </summary>
        Vita = 0x4,

        /// <summary>
        /// Nintendo 3DS
        /// </summary>
        Nintendo3Ds = 0x5,

        /// <summary>
        /// Nintendo Wii U
        /// </summary>
        WiiU = 0x6,

        /// <summary>
        /// Microsoft Windows 64bits
        /// </summary>
        Win64 = 0x20,

        /// <summary>
        /// Sony PlayStation 4
        /// </summary>
        PlayStation4 = 0x21,

        /// <summary>
        /// Durango (Microsoft Xbox ONE codename)
        /// </summary>
        Durango = 0x22,

        /// <summary>
        /// Generic platform
        /// </summary>
        Generic = 0xFF,
    }
}
