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
    [ExcludeFromCodeCoverage]
    public sealed class HexStringListJsonConverter : JsonConverter<List<ulong>>
    {
        /// <inheritdoc/>
        public override List<ulong> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            var result = new List<ulong>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                string value = reader.GetString();
                result.Add(Convert.ToUInt64(value, 16));
            }

            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, List<ulong> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
