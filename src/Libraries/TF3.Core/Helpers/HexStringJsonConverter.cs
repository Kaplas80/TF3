// Copyright (c) 2021 Luke Vo
// Code from: https://stackoverflow.com/questions/70171426/c-sharp-json-converting-hex-literal-string-to-int

namespace TF3.Core.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Json serializer for hexadecimal numbers.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class HexStringJsonConverter : JsonConverter<ulong>
    {
        /// <inheritdoc/>
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            return Convert.ToUInt64(value, 16);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
