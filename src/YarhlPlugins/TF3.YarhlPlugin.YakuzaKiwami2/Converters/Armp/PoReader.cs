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
namespace TF3.YarhlPlugin.YakuzaKiwami2.Converters.Armp
{
    using System;
    using System.Linq;
    using TF3.YarhlPlugin.YakuzaKiwami2.Enums;
    using TF3.YarhlPlugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Inserts strings from Po files to an Armp table.
    /// </summary>
    public class PoReader : IConverter<NodeContainerFormat, ArmpTable>, IInitializer<ArmpTable>
    {
        private ArmpTable _original = null;

        /// <summary>
        /// Converter initializer.
        /// </summary>
        /// <remarks>
        /// Initialization is mandatory.
        /// </remarks>
        /// <param name="parameters">Original Armp table.</param>
        public void Initialize(ArmpTable parameters) => _original = parameters;

        /// <summary>
        /// Inserts the translated strings from Po files in a Armp table.
        /// </summary>
        /// <param name="source">Collection of nodes in Po format.</param>
        /// <returns>The original Armp table with translated strings.</returns>
        public ArmpTable Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_original == null)
            {
                throw new InvalidOperationException("Uninitialized");
            }

            ArmpTable result = _original;

            InsertStrings(result, "Main", source.Root);

            return result;
        }

        private void InsertStrings(ArmpTable table, string name, Node node)
        {
            Node translationNode = node.Children.FirstOrDefault(x => x.Tags["TableName"] == name);
            if (translationNode != null)
            {
                Po po = translationNode.GetFormatAs<Po>();

                for (int i = 0; i < po.Entries.Count; i++)
                {
                    PoEntry entry = po.Entries[i];
                    int index = int.Parse(entry.Context.Split('#')[1]);
                    table.ValueStrings[index] = entry.Translated.Replace("\n", "\r\n");
                }
            }

            if (table.Indexer != null)
            {
                InsertStrings(table.Indexer, $"{name}_Idx", node);
            }

            for (int fieldIndex = 0; fieldIndex < table.FieldCount; fieldIndex++)
            {
                object[] data = table.Values[fieldIndex];
                if (data == null)
                {
                    continue;
                }

                if (table.RawRecordMemberInfo?.Length > 0)
                {
                    FieldType memberInfo = table.RawRecordMemberInfo[fieldIndex];
                    for (int recordIndex = 0; recordIndex < table.RecordCount; recordIndex++)
                    {
                        object obj = data[recordIndex];
                        if (obj == null)
                        {
                            continue;
                        }

                        if (memberInfo == FieldType.Table)
                        {
                            string fieldId = $"Field {fieldIndex}";
                            if (fieldIndex < table.FieldIds.Length)
                            {
                                fieldId = table.FieldIds[fieldIndex];
                            }

                            string recordId = $"Record {recordIndex}";
                            if (recordIndex < table.RecordIds.Length)
                            {
                                recordId = table.RecordIds[recordIndex];
                            }

                            InsertStrings((ArmpTable)obj, $"[{recordIndex}, {fieldIndex}]{recordId} ({fieldId})", node);
                        }
                    }
                }
            }
        }
    }
}
