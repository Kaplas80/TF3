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

namespace TF3.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using TF3.Core.Exceptions;
    using TF3.Core.Models;
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
        [ExcludeFromCodeCoverage]
        public static void Transform(this Node node, List<ConverterInfo> converters, List<ParameterInfo> parameters)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            };

            var yarhlConverters = PluginManager.Instance.GetConverters().Select(x => x.Metadata).ToList();
            foreach (ConverterInfo converterInfo in converters)
            {
                ConverterMetadata metadata = yarhlConverters.Find(x => x.Name == converterInfo.TypeName);

                if (metadata == null)
                {
                    throw new UnknownConverterException($"Unknown converter: {converterInfo.TypeName}");
                }

                ParameterInfo parameter = parameters.Find(x => x.Id == converterInfo.ParameterId);
                object[] initializerParameters = null;
                if (parameter != null)
                {
                    string json = parameter.Value.GetRawText();
                    var parameterType = Type.GetType(parameter.TypeName);
                    if (parameterType != null)
                    {
                        object value = JsonSerializer.Deserialize(json, parameterType, options);
                        initializerParameters = new[] { value };
                    }
                    else
                    {
                        throw new InvalidCastException($"Can not find {parameter.TypeName}. Please, use full qualified name");
                    }
                }

                node.ApplyChange(metadata, initializerParameters);
            }
        }

        /// <summary>
        /// Calls a translator converter.
        /// </summary>
        /// <param name="node">The original node.</param>
        /// <param name="translation">The node with the translation.</param>
        /// <param name="translator">The translator converter.</param>
        public static void Translate(this Node node, Node translation, string translator)
        {
            if (string.IsNullOrEmpty(translator))
            {
                node.ChangeFormat(translation.Format);
                return;
            }

            var yarhlConverters = PluginManager.Instance.GetConverters().Select(x => x.Metadata).ToList();
            ConverterMetadata metadata = yarhlConverters.Find(x => x.Name == translator);

            if (metadata == null)
            {
                throw new UnknownConverterException($"Unknown converter: {translator}");
            }

            node.ApplyChange(metadata, new object[] { translation.Format });
        }

        private static void ApplyChange(this Node node, ConverterMetadata metadata, object[] parameters)
        {
            var converter = (IConverter)Activator.CreateInstance(metadata.Type);

            System.Reflection.MethodInfo initializer = metadata.Type.GetMethod("Initialize");

            if (initializer != null && parameters != null)
            {
                _ = initializer.Invoke(converter, parameters);
            }

            var newFormat = (IFormat)ConvertFormat.With(converter, node.Format);
            node.ChangeFormat(newFormat);
        }
    }
}
