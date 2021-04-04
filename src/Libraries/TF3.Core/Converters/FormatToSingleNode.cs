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
    /// Converts a IFormat to a NodeContainerFormat.
    /// </summary>
    public class FormatToSingleNode : IConverter<IFormat, NodeContainerFormat>, IInitializer<string>
    {
        private string _nodeName = "single_node";

        /// <summary>
        /// Set the node name.
        /// </summary>
        /// <param name="parameters">Node name.</param>
        public void Initialize(string parameters) => _nodeName = parameters;

        /// <summary>
        /// Convert a format to a single node NodeContainerFormat.
        /// </summary>
        /// <param name="source">A format.</param>
        /// <returns>The 'single node' NodeContainerFormat.</returns>
        public NodeContainerFormat Convert(IFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrEmpty(_nodeName))
            {
                _nodeName = "single_node";
            }

            NodeContainerFormat result = new NodeContainerFormat();
            result.Root.Add(new Node(_nodeName, source));
            return result;
        }
    }
}
