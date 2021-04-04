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
    /// Po files merger.
    /// </summary>
    public class Merger : IConverter<NodeContainerFormat, Yarhl.Media.Text.Po>
    {
        /// <summary>
        /// Merges all parts (BinaryFormat) in a Po file.
        /// </summary>
        /// <param name="source">Po parts.</param>
        /// <returns>The merged Po.</returns>
        public Yarhl.Media.Text.Po Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Yarhl.Media.Text.Po po = new ();

            foreach (Node part in source.Root.Children)
            {
                part.TransformWith<Yarhl.Media.Text.Binary2Po>();
                Yarhl.Media.Text.Po poPart = part.GetFormatAs<Yarhl.Media.Text.Po>();
                po.Header = poPart.Header;
                po.Add(poPart.Entries);
            }

            return po;
        }
    }
}
