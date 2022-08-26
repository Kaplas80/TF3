// Copyright (c) 2021 Luke Vo
// Code from: https://stackoverflow.com/questions/70171426/c-sharp-json-converting-hex-literal-string-to-int

namespace TF3.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Json serializer for hexadecimal number arrays.
    /// </summary>
    public sealed class HexStringListJsonConverter : JsonConverter<List<int>>
    {
        /// <inheritdoc/>
        public override List<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            var result = new List<int>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                string value = reader.GetString();
                result.Add(Convert.ToInt32(value, 16));
            }

            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, List<int> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
