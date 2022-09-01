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
    public class Translate : IConverter<PortableExecutableFileFormat, PortableExecutableFileFormat>, IInitializer<Po>
    {
        // Asm operations length (in bytes).
        // Used to see how many bytes are need to be skipped to replace a string reference.
        // Value -1 means "Skip. The operation doesn't use string address"
        // Value -2 means "Another byte is needed"
        // WARNING: These values are from "Trails in the sky SC". It is a 32bit app, so
        // other executables might be different.
        private static readonly Dictionary<byte, int> OpLengths1 = new Dictionary<byte, int>
        {
            { 0x0F, -2 },
            { 0x3A, -2 },
            { 0x50, -1 }, // PUSH EAX
            { 0x51, -1 }, // PUSH ECX
            { 0x52, -1 }, // PUSH EDX
            { 0x53, -1 }, // PUSH EBX
            { 0x56, -1 }, // PUSH ESI
            { 0x57, -1 }, // PUSH EDI
            { 0x66, 2 }, // MOV AX, ?
            { 0x68, 1 }, // PUSH
            { 0x89, -1 },
            { 0x8A, -1 }, // MOV AL, ?
            { 0x8D, -2 },
            { 0xA1, 1 }, // MOV EAX, ?
            { 0xB8, 1 }, // MOV EAX, ?
            { 0xB9, 1 }, // MOV ECX, ?
            { 0xBA, 1 }, // MOV EDX, ?
            { 0xBB, 1 }, // MOV EBX, ?
            { 0xBD, 1 }, // MOV EBP, ?
            { 0xBE, 1 }, // MOV ESI, ?
            { 0xBF, 1 }, // MOV EDI, ?
            { 0xC7, -2 },
            { 0xF3, -2 },
        };

        private static readonly Dictionary<ushort, int> OpLengths2 = new Dictionary<ushort, int>
        {
            { 0x0F10, 3 },
            { 0x0F45, -1 },
            { 0x3A11, -1 },
            { 0x3A88, 2 },
            { 0x8D41, -1 },
            { 0x8DB7, 2 },
            { 0xC704, 3 },
            { 0xC744, 4 },
            { 0xC784, 7 },
            { 0xF30F, 4 },
            { 0xF3A4, -1 },
            { 0xF3A5, -1 },
        };

        private readonly Dictionary<string, Encoding> _encodings = new Dictionary<string, Encoding>();

        private Po _translation = null;

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
            _encodings.Add("Shift_JIS", Encoding.GetEncoding(932));
            _encodings.Add("UTF-16", Encoding.GetEncoding(1200));

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
        protected virtual void WriteString(PoEntry entry, DataWriter writer)
        {
            if (entry.Original == "<!empty>")
            {
                writer.Write((byte)0x00);
            }
            else
            {
                string[] context = entry.Context.Split('#');
                string encoding = context[1];
                string text = entry.Translated;
                if (string.IsNullOrEmpty(text))
                {
                    text = entry.Original;
                }

                writer.Write(text, true, _encodings[encoding]);
            }
        }

        /// <summary>
        /// Modifies the string references to point to the new address.
        /// </summary>
        /// <param name="reference">The reference address.</param>
        /// <param name="originalStringAddress">The original string address.</param>
        /// <param name="newStringAddress">The new string address.</param>
        /// <param name="peFile">The PEFile.</param>
        /// <param name="peFileStream">The PEFile bytes.</param>
        /// <exception cref="NotImplementedException">Thrown if instruction opcode is not recognized.</exception>
        protected virtual void WriteReference(uint reference, uint originalStringAddress, uint newStringAddress, PEFile peFile, DataStream peFileStream)
        {
            uint rva = reference - (uint)peFile.OptionalHeader.ImageBase;

            PESection referenceSection = peFile.GetSectionContainingRva(rva);

            using (DataStream stream = DataStreamFactory.FromStream(peFileStream, (long)referenceSection.Offset, referenceSection.GetPhysicalSize()))
            {
                long position = rva - (long)referenceSection.Rva;
                stream.Position = position;

                var reader = new DataReader(stream);
                var writer = new DataWriter(stream);

                uint value = reader.ReadUInt32();
                if (value == originalStringAddress)
                {
                    stream.Position = position;
                }
                else
                {
                    stream.Position = position;
                    int byteOffset = CalculateOpLength(reader, reference);
                    stream.Position = position;

                    if (byteOffset < 0)
                    {
                        return;
                    }

                    stream.Seek(byteOffset, SeekOrigin.Current);
                }

                stream.PushCurrentPosition();
                uint checkValue = reader.ReadUInt32();
                if (checkValue != originalStringAddress)
                {
                    throw new InvalidOperationException($"Pos: 0x{reference:X8} - Addresses doesn't match: 0x{originalStringAddress:X8}!=0x{checkValue:X8}");
                }

                stream.PopPosition();
                writer.Write(newStringAddress);
            }
        }

        private static int CalculateOpLength(DataReader reader, uint reference)
        {
            byte b1 = reader.ReadByte();

            if (!OpLengths1.ContainsKey(b1))
            {
                throw new NotImplementedException($"Pos: 0x{reference:X8} - Op: 0x{b1:X2}");
            }

            if (OpLengths1[b1] > -2)
            {
                return OpLengths1[b1];
            }

            byte b2 = reader.ReadByte();
            ushort u1 = (ushort)(b1 << 8 | b2);

            if (!OpLengths2.ContainsKey(u1))
            {
                throw new NotImplementedException($"Pos: 0x{reference:X8} - Op: 0x{u1:X4}");
            }

            if (OpLengths2[u1] > -2)
            {
                return OpLengths2[u1];
            }

            throw new NotImplementedException($"Pos: 0x{reference:X8} - Op: 0x{u1:X4}");
        }
    }
}
