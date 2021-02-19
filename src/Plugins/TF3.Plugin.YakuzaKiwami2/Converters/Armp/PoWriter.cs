﻿// Copyright (c) 2021 Kaplas
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
namespace TF3.Plugin.YakuzaKiwami2.Converters.Armp
{
    using System;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Extracts Yakuza Kiwami 2 ARMP translatable strings to a collection of Po files.
    /// </summary>
    public class PoWriter : IConverter<ArmpTable, NodeContainerFormat>, IInitializer<PoHeader>
    {
        private PoHeader _poHeader = new PoHeader("NoName", "dummy@dummy.com", "en");

        public void Initialize(PoHeader parameters)
        {
            _poHeader = parameters;
        }

        /// <summary>
        /// Extracts strings to a Po files.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The po collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public NodeContainerFormat Convert(ArmpTable source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            Node result = NodeFactory.CreateContainer("root");

            ExtractStrings(source, "Main", result);

            return result.GetFormatAs<NodeContainerFormat>();
        }

        private void ExtractStrings(ArmpTable table, string name, Node node)
        {
            if (table.ValueStringCount > 0) {
                var po = new Po(_poHeader);
                for (int i = 0; i < table.ValueStringCount; i++) {
                    if (!string.IsNullOrEmpty(table.ValueStrings[i])) {
                        var entry = new PoEntry() {
                            Original = table.ValueStrings[i].Replace("\r\n", "\n"),
                            Translated = table.ValueStrings[i].Replace("\r\n", "\n"),
                            Context = $"{name}#{i}",
                        };
                        po.Add(entry);
                    }
                }

                var n = new Node(name, po);
                node.Add(n);
            }

            if (table.Indexer != null) {
                ExtractStrings(table.Indexer, $"{name}_Idx", node);
            }

            for (int fieldIndex = 0; fieldIndex < table.FieldCount; fieldIndex++) {
                object[] data = table.Values[fieldIndex];
                if (data == null) {
                    continue;
                }

                if (table.RawRecordMemberInfo?.Length > 0) {
                    FieldType memberInfo = table.RawRecordMemberInfo[fieldIndex];
                    for (int recordIndex = 0; recordIndex < table.RecordCount; recordIndex++) {
                        object obj = data[recordIndex];
                        if (obj == null) {
                            continue;
                        }

                        if (memberInfo == FieldType.Table) {
                            string fieldId = $"Field {fieldIndex}";
                            if (fieldIndex < table.FieldIds.Length) {
                                fieldId = table.FieldIds[fieldIndex];
                            }

                            string recordId = $"Record {recordIndex}";
                            if (recordIndex < table.RecordIds.Length) {
                                recordId = table.RecordIds[recordIndex];
                            }

                            ExtractStrings((ArmpTable)obj, $"[{recordIndex}, {fieldIndex}]{recordId} ({fieldId})", node);
                        }
                    }
                }
            }
        }
    }
}
