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

namespace TF3.Common.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TF3.Common.Core.Exceptions;
    using TF3.Common.Core.Models;
    using Yarhl;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    /// <summary>
    /// Node extension methods.
    /// </summary>
    public static class YarhlNodeExtension
    {
        /// <summary>
        /// Transforms a Yarhl node using a chain of converters.
        /// </summary>
        /// <param name="node">The original node.</param>
        /// <param name="converters">Converters list.</param>
        /// <param name="parameters">Allowed parameters list.</param>
        public static void Transform(this Node node, List<ConverterInfo> converters, List<ParameterInfo> parameters)
        {
            var yarhlConverters = PluginManager.Instance.GetConverters().Select(x => x.Metadata).ToList();
            foreach (ConverterInfo converterInfo in converters)
            {
                ConverterMetadata metadata = yarhlConverters.FirstOrDefault(x => x.Name == converterInfo.TypeName);

                if (metadata == null)
                {
                    throw new UnknownConverterException($"Unknown converter: {converterInfo.TypeName}");
                }

                IConverter converter = (IConverter)Activator.CreateInstance(metadata.Type);

                System.Reflection.MethodInfo initializer = metadata.Type.GetMethod("Initialize");
                ParameterInfo parameter = parameters.FirstOrDefault(x => x.Id == converterInfo.ParameterId);
                if (initializer != null && parameter != null)
                {
                    _ = initializer.Invoke(converter, new object[] { parameter.Value });
                }

                node.ChangeFormat((IFormat)ConvertFormat.With(converter, node.Format));
            }
        }
    }
}
