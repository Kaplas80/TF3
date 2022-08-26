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

namespace TF3.Core.Converters.PortableExecutable
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using Dahomey.Json;
    using TF3.Core.Formats;
    using TF3.Core.Models;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Deserializes PE files.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, PortableExecutableFileFormat>, IInitializer<string>
    {
        private string _filename = string.Empty;

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="filename">Reader config.</param>
        public void Initialize(string filename) => _filename = filename;

        /// <summary>
        /// Converts a BinaryFormat into a PEFile.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The PEFile format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public PortableExecutableFileFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrEmpty(_filename))
            {
                throw new InvalidOperationException("Uninitialized");
            }

            if (!File.Exists(string.Concat("./plugins/", _filename)))
            {
                throw new FileNotFoundException("File not found", _filename);
            }

            JsonSerializerOptions options = new JsonSerializerOptions().SetupExtensions();
            options.SetMissingMemberHandling(MissingMemberHandling.Error);
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            string scriptContents = File.ReadAllText(string.Concat("./plugins/", _filename));
            List<PortableExecutableStringInfo> stringInfo = JsonSerializer.Deserialize<List<PortableExecutableStringInfo>>(scriptContents, options);

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);
            byte[] data = reader.ReadBytes((int)source.Stream.Length);
            return new PortableExecutableFileFormat()
            {
                Internal = AsmResolver.PE.File.PEFile.FromBytes(data),
                StringInfo = stringInfo,
            };
        }
    }
}
