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

namespace TF3.YarhlPlugin.Common.Converters.PortableExecutable
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using AsmResolver;
    using AsmResolver.PE.File;
    using AsmResolver.PE.File.Headers;
    using TF3.YarhlPlugin.Common.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    /// <summary>
    /// Inserts strings from Po file to the game Portable executable.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class Translate : IConverter<PortableExecutableFileFormat, PortableExecutableFileFormat>, IInitializer<Po>
    {
        private Po _translation = null;

        /// <summary>
        /// Gets the collection of usable encodings.
        /// </summary>
        protected Dictionary<string, Encoding> Encodings { get; } = new Dictionary<string, Encoding>();

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="parameters">Po with translation.</param>
        public void Initialize(Po parameters) => _translation = parameters;

        /// <summary>
        /// Inserts the translated strings from Po file in a PortableExecutable file.
        /// </summary>
        /// <param name="source">Original Portable executable.</param>
        /// <returns>Translated Portable executable.</returns>
        public PortableExecutableFileFormat Convert(PortableExecutableFileFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_translation == null)
            {
                throw new InvalidOperationException("Uninitialized");
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encodings.Add("Shift_JIS", Encoding.GetEncoding(932));
            Encodings.Add("UTF-16", Encoding.GetEncoding(1200));

            var result = source.DeepClone() as PortableExecutableFileFormat;

            byte[] translationData = new byte[0x100000];

            var newOffsets = new Dictionary<uint, uint>();

            // First, we have to write the translated strings into a new section.
            using (DataStream stream = DataStreamFactory.FromArray(translationData, 0, translationData.Length))
            {
                var writer = new DataWriter(stream);

                for (int i = 0; i < _translation.Entries.Count; i++)
                {
                    PoEntry entry = _translation.Entries[i];

                    string[] context = entry.Context.Split('#');
                    uint originalAddress = uint.Parse(context[0]);

                    newOffsets.Add(originalAddress, (uint)stream.Position);
                    WriteString(entry, writer);
                }
            }

            var translationSection = new PESection(".tf3", SectionFlags.MemoryRead | SectionFlags.ContentInitializedData)
            {
                Contents = new DataSegment(translationData),
            };

            result.Internal.Sections.Add(translationSection);
            result.Internal.UpdateHeaders();

            // If I try to get each section and replace its contents with AsmResolver
            // library, it generates a corrupt exe file if .data or .rdata are modified.
            // The workaround is modifying the file bytes.
            using var peFileStream = new MemoryStream();
            result.Internal.Write(peFileStream);

            byte[] peFileData = peFileStream.ToArray();
            using DataStream finalStream = DataStreamFactory.FromArray(peFileData, 0, peFileData.Length);

            // Now, we need to update the references to each string
            for (int i = 0; i < _translation.Entries.Count; i++)
            {
                PoEntry entry = _translation.Entries[i];
                string[] context = entry.Context.Split('#');
                uint originalAddress = uint.Parse(context[0]);

                if (string.IsNullOrEmpty(entry.Reference))
                {
                    continue;
                }

                uint[] references = Array.ConvertAll<string, uint>(entry.Reference.Split(','), uint.Parse);

                uint newAddress = newOffsets[originalAddress] + translationSection.Rva + (uint)result.Internal.OptionalHeader.ImageBase;
                foreach (uint reference in references)
                {
                    WriteReference(reference, originalAddress, newAddress, result.Internal, finalStream);
                }
            }

            result.Internal = PEFile.FromBytes(peFileData);
            return result;
        }

        /// <summary>
        /// Writes the translated string to the DataWriter, allowing modifications.
        /// </summary>
        /// <param name="entry">The PoEntry with the string info.</param>
        /// <param name="writer">The DataWriter.</param>
        protected abstract void WriteString(PoEntry entry, DataWriter writer);

        /// <summary>
        /// Modifies the string references to point to the new address.
        /// </summary>
        /// <param name="reference">The reference address.</param>
        /// <param name="originalStringAddress">The original string address.</param>
        /// <param name="newStringAddress">The new string address.</param>
        /// <param name="peFile">The PEFile.</param>
        /// <param name="peFileStream">The PEFile bytes.</param>
        /// <exception cref="NotImplementedException">Thrown if instruction opcode is not recognized.</exception>
        protected abstract void WriteReference(uint reference, uint originalStringAddress, uint newStringAddress, PEFile peFile, DataStream peFileStream);
    }
}
