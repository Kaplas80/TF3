// Copyright (c) 2021 Luke Vo
// Code from: https://stackoverflow.com/questions/70171426/c-sharp-json-converting-hex-literal-string-to-int

namespace TF3.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Json serializer for hexadecimal number arrays.
    /// </summary>
    /// <typeparam name="T">Output type.</typeparam>
    [ExcludeFromCodeCoverage]
    public sealed class HexStringListJsonConverter<T> : JsonConverter<List<T>>
    {
        /// <inheritdoc/>
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            var result = new List<T>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                string strValue = reader.GetString();

                T value = Type.GetTypeCode(typeToConvert.GenericTypeArguments[0]) switch
                {
                    TypeCode.Byte => (T)Convert.ChangeType(Convert.ToByte(strValue, 16), typeof(T)),
                    TypeCode.SByte => (T)Convert.ChangeType(Convert.ToSByte(strValue, 16), typeof(T)),
                    TypeCode.UInt16 => (T)Convert.ChangeType(Convert.ToUInt16(strValue, 16), typeof(T)),
                    TypeCode.UInt32 => (T)Convert.ChangeType(Convert.ToUInt32(strValue, 16), typeof(T)),
                    TypeCode.UInt64 => (T)Convert.ChangeType(Convert.ToUInt64(strValue, 16), typeof(T)),
                    TypeCode.Int16 => (T)Convert.ChangeType(Convert.ToInt16(strValue, 16), typeof(T)),
                    TypeCode.Int32 => (T)Convert.ChangeType(Convert.ToInt32(strValue, 16), typeof(T)),
                    TypeCode.Int64 => (T)Convert.ChangeType(Convert.ToInt64(strValue, 16), typeof(T)),
                    _ => throw new NotSupportedException("Type not supported in converter"),
                };

                result.Add(value);
            }

            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
