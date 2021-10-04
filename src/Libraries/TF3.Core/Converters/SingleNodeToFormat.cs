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

namespace TF3.Core.Converters
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    /// <summary>
    /// Converts a NodeContainerFormat to the format of the first children.
    /// </summary>
    public class SingleNodeToFormat : IConverter<NodeContainerFormat, IFormat>
    {
        /// <summary>
        /// Convert a NodeContainerFormat to the format of the first children.
        /// </summary>
        /// <param name="source">A 'single node' NodeContainerFormat.</param>
        /// <returns>The format of the node.</returns>
        public IFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Root.Children.Count != 1)
            {
                throw new FormatException("Node must have 1 children exactly.");
            }

            if (source.Root.Children[0].Format is ICloneableFormat)
            {
                Node clone = new (source.Root.Children[0]);
                return clone.Format;
            }
            else
            {
                return source.Root.Children[0].Format;
            }
        }
    }
}
