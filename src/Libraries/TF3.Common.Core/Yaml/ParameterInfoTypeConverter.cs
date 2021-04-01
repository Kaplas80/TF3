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

namespace TF3.Common.Core.Yaml
{
    using System;
    using TF3.Common.Core.Models;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// YamlDotNet type converter for ParameterInfo.
    /// </summary>
    public class ParameterInfoTypeConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type) => type == typeof(ParameterInfo);

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type)
        {
            var result = new ParameterInfo();
            Type parameterType = null;

            parser.Consume<MappingStart>();
            Scalar propertyName = parser.Consume<Scalar>();
            while (propertyName != null)
            {
                switch (propertyName.Value)
                {
                    case "id":
                        Scalar idValue = parser.Consume<Scalar>();
                        result.Id = idValue.Value;
                        break;

                    case "typeName":
                        Scalar typeNameValue = parser.Consume<Scalar>();
                        result.TypeName = typeNameValue.Value;
                        parameterType = Type.GetType(typeNameValue.Value);
                        result.Value = Activator.CreateInstance(parameterType);
                        break;

                    case "value":
                        parser.Consume<MappingStart>();
                        Scalar propertyName2 = parser.Consume<Scalar>();
                        while (propertyName2 != null)
                        {
                            System.Reflection.PropertyInfo property = parameterType!.GetProperty(propertyName2.Value);
                            Scalar value = parser.Consume<Scalar>();
                            property.SetValue(result.Value, value.Value);
                            parser.TryConsume<Scalar>(out propertyName2);
                        }

                        parser.Consume<MappingEnd>();
                        break;
                    default:
                        break;
                }

                parser.TryConsume<Scalar>(out propertyName);
            }

            parser.Consume<MappingEnd>();

            return result;
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            ParameterInfo element = (ParameterInfo)value;
            Type valueType = element.Value.GetType();

            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            emitter.Emit(new Scalar(null, "id"));
            emitter.Emit(new Scalar(null, element.Id));

            emitter.Emit(new Scalar(null, "typeName"));
            emitter.Emit(new Scalar(null, valueType.AssemblyQualifiedName));

            emitter.Emit(new Scalar(null, "value"));

            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));
            foreach (System.Reflection.PropertyInfo property in valueType.GetProperties())
            {
                if (property.CanWrite)
                {
                    emitter.Emit(new Scalar(null, property.Name));
                    object v = property.GetValue(element.Value);
                    if (v == null)
                    {
                        emitter.Emit(new Scalar(null, string.Empty));
                    }
                    else
                    {
                        emitter.Emit(new Scalar(null, v.ToString()));
                    }
                }
            }

            emitter.Emit(new MappingEnd());

            emitter.Emit(new MappingEnd());
        }
    }
}
