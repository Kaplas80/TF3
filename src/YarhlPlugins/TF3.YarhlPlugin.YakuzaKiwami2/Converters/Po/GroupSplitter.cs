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

namespace TF3.YarhlPlugin.YakuzaKiwami2.Converters.Po
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    /// <summary>
    /// Po files splitter.
    /// </summary>
    public class GroupSplitter : IConverter<NodeContainerFormat, NodeContainerFormat>
    {
        /// <summary>
        /// Splits a group of Po files in smaller parts.
        /// </summary>
        /// <param name="source">Original Po files.</param>
        /// <returns>A container with the smaller parts.</returns>
        public NodeContainerFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (Node node in source.Root.Children)
            {
                if (node.Stream == null)
                {
                    continue;
                }

                node.Stream.Seek(0);
                node.TransformWith<Yarhl.Media.Text.Binary2Po>();
                node.TransformWith<Splitter>();

                foreach (Node n in node.Children)
                {
                    n.TransformWith<Yarhl.Media.Text.Po2Binary>();
                }
            }

            return source;
        }
    }
}
