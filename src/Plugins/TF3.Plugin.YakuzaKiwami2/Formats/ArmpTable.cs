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
namespace TF3.Plugin.YakuzaKiwami2.Formats
{
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Types;
    using Yarhl.FileFormat;

    /// <summary>
    /// Dragon Engine ARMP table.
    /// </summary>
    public class ArmpTable : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        /// <param name="header">Table info.</param>
        public ArmpTable(ArmpTableHeader header)
        {
            Id = header.Id;
            Flags = header.Flags;

            RecordCount = header.RecordCount;
            AreRecordsInvalid = header.RecordInvalid;

            FieldCount = header.FieldCount;
            AreFieldsInvalid = header.FieldInvalid;

            ValueStringCount = header.ValueStringCount;

            Indexer = null;
        }

        /// <summary>
        /// Gets the table id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the table flags.
        /// </summary>
        public byte Flags { get; }

        /// <summary>
        /// Gets the number of records.
        /// </summary>
        public int RecordCount { get; }

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        public int FieldCount { get; }

        /// <summary>
        /// Gets the number of value strings.
        /// </summary>
        public int ValueStringCount { get; }

        /// <summary>
        /// Gets a value indicating whether the table records has invalid values.
        /// </summary>
        public bool AreRecordsInvalid { get; }

        /// <summary>
        /// Gets a value indicating whether the table fields has invalid values.
        /// </summary>
        public bool AreFieldsInvalid { get; }

        /// <summary>
        /// Gets or sets the table indexer.
        /// </summary>
        public ArmpTable Indexer { get; set; }

        /// <summary>
        /// Gets or sets the record existence info.
        /// </summary>
        public bool[] RecordExistence { get; set; }

        /// <summary>
        /// Gets or sets the field existence info.
        /// </summary>
        public bool[] FieldExistence { get; set; }

        /// <summary>
        /// Gets or sets the record IDs.
        /// </summary>
        public string[] RecordIds { get; set; }

        /// <summary>
        /// Gets or sets the field IDs.
        /// </summary>
        public string[] FieldIds { get; set; }

        /// <summary>
        /// Gets or sets the value strings.
        /// </summary>
        public string[] ValueStrings { get; set; }

        /// <summary>
        /// Gets or sets the field type info.
        /// </summary>
        public FieldType[] FieldTypes { get; set; }

        /// <summary>
        /// Gets or sets the raw record member info.
        /// </summary>
        public FieldType[] RawRecordMemberInfo { get; set; }

        /// <summary>
        /// Gets or sets the values matrix.
        /// </summary>
        public object[][] Values { get; set; }

        /// <summary>
        /// Gets or sets the empty values matrix.
        /// </summary>
        public bool[][] EmptyValues { get; set; }

        /// <summary>
        /// Gets or sets the record order.
        /// </summary>
        public int[] RecordOrder { get; set; }

        /// <summary>
        /// Gets or sets the field order.
        /// </summary>
        public int[] FieldOrder { get; set; }

        /// <summary>
        /// Gets or sets the field info.
        /// </summary>
        public byte[] FieldInfo { get; set; }
    }
}
