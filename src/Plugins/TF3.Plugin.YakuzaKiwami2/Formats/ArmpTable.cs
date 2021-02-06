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
    using System.Linq;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using Yarhl.FileFormat;

    /// <summary>
    /// Dragon Engine ARMP table.
    /// </summary>
    public class ArmpTable : IFormat
    {
        private readonly Record[] records;
        private readonly Field[] fields;
        private readonly string[] valueStrings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        /// <param name="id">Table id.</param>
        /// <param name="flags">Table flags.</param>
        /// <param name="recordCount">Total records (rows) in table.</param>
        /// <param name="fieldCount">Total fields (columns) in table.</param>
        /// <param name="valueStringCount">Total value strings in table.</param>
        public ArmpTable(int id, byte flags, int recordCount, int fieldCount, int valueStringCount)
        {
            Id = id;
            Flags = flags;
            Indexer = null;

            RecordCount = recordCount;
            records = new Record[recordCount];
            for (int i = 0; i < recordCount; i++) {
                records[i] = new Record(fieldCount);
            }

            FieldCount = fieldCount;
            fields = new Field[fieldCount];
            for (int i = 0; i < fieldCount; i++) {
                fields[i] = new Field();
            }

            ValueStringCount = valueStringCount;
            valueStrings = new string[valueStringCount];
        }

        /// <summary>
        /// Gets the number of value strings.
        /// </summary>
        public int ValueStringCount { get; }

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        public int FieldCount { get; }

        /// <summary>
        /// Gets the number of records.
        /// </summary>
        public int RecordCount { get; }

        /// <summary>
        /// Gets the table id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the table flags.
        /// </summary>
        public byte Flags { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the table has field existence and field order values.
        /// </summary>
        public bool AreFieldsInvalid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the table has record existence and field order values.
        /// </summary>
        public bool AreRecordsInvalid { get; set; }

        /// <summary>
        /// Gets or sets the table indexer.
        /// </summary>
        public ArmpTable Indexer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the table has record existence data.
        /// </summary>
        public bool HasRecordExistenceData => records.Any(x => x.Exists);

        /// <summary>
        /// Gets a value indicating whether the table has field existence data.
        /// </summary>
        public bool HasFieldExistenceData => fields.Any(x => x.Exists);

        /// <summary>
        /// Gets a value indicating whether the table has record ids.
        /// </summary>
        public bool HasRecordIds => records.Any(x => !string.IsNullOrEmpty(x.Id));

        /// <summary>
        /// Gets a value indicating whether the table has field ids.
        /// </summary>
        public bool HasFieldIds => fields.Any(x => !string.IsNullOrEmpty(x.Id));

        /// <summary>
        /// Gets a value indicating whether the table has field types.
        /// </summary>
        public bool HasFieldTypes => fields.Any(x => x.Type != FieldType.Unused);

        /// <summary>
        /// Gets a value string.
        /// </summary>
        /// <param name="index">String index.</param>
        /// <returns>The string.</returns>
        public string GetValueString(int index) => valueStrings[index];

        /// <summary>
        /// Sets a value string.
        /// </summary>
        /// <param name="index">String index.</param>
        /// <param name="value">The string.</param>
        public void SetValueString(int index, string value) => valueStrings[index] = value;

        /// <summary>
        /// Gets a field id.
        /// <remarks>If the field has no id, it returns the field index.</remarks>
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <returns>The field id.</returns>
        public string GetFieldId(int index)
        {
            string value = fields[index].Id;
            if (string.IsNullOrEmpty(value)) {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Sets the field id.
        /// </summary>
        /// <param name="index">The field index.</param>
        /// <param name="value">The id to set.</param>
        public void SetFieldId(int index, string value) => fields[index].Id = value;

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <returns>The field type.</returns>
        public FieldType GetFieldType(int index) => fields[index].Type;

        /// <summary>
        /// Sets the field type.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <param name="type">Field type.</param>
        public void SetFieldType(int index, FieldType type) => fields[index].Type = type;

        /// <summary>
        /// Gets the field order.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <returns>Field order.</returns>
        public int GetFieldOrder(int index) => fields[index].Order;

        /// <summary>
        /// Sets the field order.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <param name="value">Field order.</param>
        public void SetFieldOrder(int index, int value) => fields[index].Order = value;

        /// <summary>
        /// Gets the field existence.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <returns>The field exists.</returns>
        public bool GetFieldExistence(int index) => fields[index].Exists;

        /// <summary>
        /// Sets the field existence.
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <param name="exists">The field exists.</param>
        public void SetFieldExistence(int index, bool exists) => fields[index].Exists = exists;

        /// <summary>
        /// Gets the raw record member info (real field type).
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <returns>The type.</returns>
        public FieldType GetRawRecordMemberInfo(int index) => fields[index].RawType;

        /// <summary>
        /// Sets the raw record member info (real field type).
        /// </summary>
        /// <param name="index">Field index.</param>
        /// <param name="type">The type.</param>
        public void SetRawRecordMemberInfo(int index, FieldType type) => fields[index].RawType = type;

        /// <summary>
        /// Gets a record id.
        /// <remarks>If the record has no id, it returns the record index.</remarks>
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <returns>The record id.</returns>
        public string GetRecordId(int index)
        {
            string value = records[index].Id;
            if (string.IsNullOrEmpty(value)) {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Sets the record id.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <param name="value">The id to set.</param>
        public void SetRecordId(int index, string value) => records[index].Id = value;

        /// <summary>
        /// Gets the record order.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <returns>Record order.</returns>
        public int GetRecordOrder(int index) => records[index].Order;

        /// <summary>
        /// Sets the record order.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <param name="value">Record order.</param>
        public void SetRecordOrder(int index, int value) => records[index].Order = value;

        /// <summary>
        /// Gets the record existence.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <returns>The record exists.</returns>
        public bool GetRecordExistence(int index) => records[index].Exists;

        /// <summary>
        /// Sets the record existence.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <param name="exists">The record exists.</param>
        public void SetRecordExistence(int index, bool exists) => records[index].Exists = exists;

        /// <summary>
        /// Gets the record field info.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <returns>The record field info.</returns>
        public byte GetFieldInfo(int index) => records[index].FieldInfo;

        /// <summary>
        /// Sets the record field info.
        /// </summary>
        /// <param name="index">Record index.</param>
        /// <param name="value">The record field info.</param>
        public void SetFieldInfo(int index, byte value) => records[index].FieldInfo = value;

        /// <summary>
        /// Gets the field value for a record.
        /// </summary>
        /// <param name="recordIndex">Record index.</param>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>The value.</returns>
        public object GetValue(int recordIndex, int fieldIndex) => records[recordIndex].GetContent(fieldIndex);

        /// <summary>
        /// Sets the field value for a record.
        /// </summary>
        /// <param name="recordIndex">Record index.</param>
        /// <param name="fieldIndex">Field index.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(int recordIndex, int fieldIndex, object value) => records[recordIndex].SetContent(fieldIndex, value);

        /// <summary>
        /// Gets a value indicating if the stored content is null.
        /// </summary>
        /// <param name="recordIndex">Record index.</param>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>The content is NULL.</returns>
        public bool GetIsNullValue(int recordIndex, int fieldIndex) => records[recordIndex].GetIsNullContent(fieldIndex);

        /// <summary>
        /// Sets a value indicating if the stored content is null.
        /// </summary>
        /// <param name="recordIndex">Record index.</param>
        /// <param name="fieldIndex">Field index.</param>
        /// <param name="isNull">The content is NULL.</param>
        public void SetIsNullValue(int recordIndex, int fieldIndex, bool isNull) => records[recordIndex].SetIsNullContent(fieldIndex, isNull);

        private class Record
        {
            private readonly object[] contents;
            private readonly bool[] nullContents;

            public Record(int fieldCount)
            {
                contents = new object[fieldCount];
                nullContents = new bool[fieldCount];
                Exists = false;
                Order = -1;
                FieldInfo = 0;
            }

            public string Id { get; set; }

            public bool Exists { get; set; }

            public int Order { get; set; }

            public byte FieldInfo { get; set; }

            public void SetContent(int fieldIndex, object value) => contents[fieldIndex] = value;

            public object GetContent(int fieldIndex) => contents[fieldIndex];

            public bool GetIsNullContent(int fieldIndex) => nullContents[fieldIndex];

            public void SetIsNullContent(int fieldIndex, bool isNull) => nullContents[fieldIndex] = isNull;
        }

        private class Field
        {
            public Field()
            {
                Id = string.Empty;
                Type = FieldType.Unused;
                RawType = FieldType.Unused;
                Order = -1;
                Exists = false;
            }

            public string Id { get; set; }

            public FieldType Type { get; set; }

            public FieldType RawType { get; set; }

            public int Order { get; set; }

            public bool Exists { get; set; }
        }
    }
}
