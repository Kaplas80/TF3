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
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using AsmResolver.IO;
    using AsmResolver.PE.File;
    using TF3.Core.Formats;
    using TF3.Core.Models;
    using Yarhl.FileFormat;
    using Yarhl.Media.Text;

    /// <summary>
    /// Extracts .exe translatable strings to a Po file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExtractStrings : IConverter<PortableExecutableFileFormat, Po>, IInitializer<PoHeader>
    {
        private readonly Dictionary<string, Encoding> _encodings = new Dictionary<string, Encoding>();

        private PoHeader _poHeader = new PoHeader("NoName", "dummy@dummy.com", "en");

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <param name="parameters">Header to use in created Po elements.</param>
        public void Initialize(PoHeader parameters) => _poHeader = parameters;

        /// <summary>
        /// Extracts strings to a Po file.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The po file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public Po Convert(PortableExecutableFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encodings.Add("Shift_JIS", Encoding.GetEncoding(932));
            _encodings.Add("UTF-16", Encoding.GetEncoding(1200));

            var po = new Po(_poHeader);

            byte[] stringBytes = new byte[4096];

            for (int i = 0; i < source.StringInfo.Count; i++)
            {
                PortableExecutableStringInfo stringInfo = source.StringInfo[i];

                uint stringRelativeVirtualAddress = (uint)stringInfo.Address - (uint)source.Internal.OptionalHeader.ImageBase;

                PESection stringSection = source.Internal.GetSectionContainingRva(stringRelativeVirtualAddress);
                BinaryStreamReader stringReader = stringSection.CreateReader(stringSection.Offset, stringSection.GetPhysicalSize());

                stringReader.Offset = stringSection.RvaToFileOffset(stringRelativeVirtualAddress);
                int bytesRead = stringReader.ReadBytes(stringBytes, 0, stringInfo.Size);

                if (bytesRead != stringInfo.Size)
                {
                    throw new InvalidOperationException("Error reading string");
                }

                string str = _encodings[stringInfo.Encoding].GetString(stringBytes, 0, bytesRead).TrimEnd('\0');

                str = ProcessString(str);

                var entry = new PoEntry()
                {
                    Original = str,
                    Translated = str,
                    Context = $"{stringInfo.Address}#{stringInfo.Encoding}",
                    Reference = string.Join(',', stringInfo.Pointers.ToArray()),
                };
                po.Add(entry);
            }

            return po;
        }

        /// <summary>
        /// Some games may need string processing before writing the Po.
        /// For example, converting from fullwidth to halfwidth or replacing some characters.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <returns>A processed string.</returns>
        protected virtual string ProcessString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "<!empty>";
            }

            return value;
        }
    }
}
