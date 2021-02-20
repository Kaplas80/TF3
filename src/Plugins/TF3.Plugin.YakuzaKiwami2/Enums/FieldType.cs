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
namespace TF3.Plugin.YakuzaKiwami2.Enums
{
    /// <summary>
    /// Possible field types in ARMP files.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Unsigned integer (8 bits).
        /// </summary>
        UInt8 = 0x00,

        /// <summary>
        /// Unsigned integer (16 bits).
        /// </summary>
        UInt16 = 0x01,

        /// <summary>
        /// Unsigned integer (32 bits).
        /// </summary>
        UInt32 = 0x02,

        /// <summary>
        /// Unsigned integer (64 bits).
        /// </summary>
        UInt64 = 0x03,

        /// <summary>
        /// Signed integer (8 bits).
        /// </summary>
        Int8 = 0x04,

        /// <summary>
        /// Signed integer (16 bits).
        /// </summary>
        Int16 = 0x05,

        /// <summary>
        /// Signed integer (32 bits).
        /// </summary>
        Int32 = 0x06,

        /// <summary>
        /// Signed integer (64 bits).
        /// </summary>
        Int64 = 0x07,

        /// <summary>
        /// Floating-point number (16 bits).
        /// </summary>
        Float16 = 0x08,

        /// <summary>
        /// Floating-point number (32 bits).
        /// </summary>
        Float32 = 0x09,

        /// <summary>
        /// Floating-point number (64 bits).
        /// </summary>
        Float64 = 0x0A,

        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean = 0x0B,

        /// <summary>
        /// String.
        /// </summary>
        String = 0x0C,

        /// <summary>
        /// Table.
        /// </summary>
        Table = 0x0D,

        /// <summary>
        /// Unused field.
        /// </summary>
        Unused = 0xFF,
    }
}
