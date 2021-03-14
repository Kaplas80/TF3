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
namespace TF3.YarhlPlugin.YakuzaKiwami2.Types
{
    using System.Text;
    using Yarhl.IO.Serialization.Attributes;

    /// <summary>
    /// ARMP table header.
    /// </summary>
    [Serializable]
    public class ArmpTableHeader
    {
        /// <summary>
        /// Gets or sets the number of records (rows) included in the table.
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// Gets or sets the number of fields (columns) included in the table.
        /// </summary>
        public int FieldCount { get; set; }

        /// <summary>
        /// Gets or sets the number of value strings included in the table.
        /// </summary>
        public int ValueStringCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the records are invalid (??).
        /// </summary>
        [BinaryBoolean(ReadAs = typeof(int), WriteAs = typeof(int), TrueValue = -1, FalseValue = 0)]
        public bool RecordInvalid { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the record ids table.
        /// </summary>
        /// <remarks>Pointer to pointer.</remarks>
        public int RecordIdPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the record existence (validity) bits.
        /// </summary>
        public int RecordExistencePointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the field types list.
        /// </summary>
        public int FieldTypePointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the values (record content) table.
        /// </summary>
        /// <remarks>Pointer to pointer.</remarks>
        public int ValuesPointer { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [BinaryInt24]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public byte Flags { get; set; }

        /// <summary>
        /// Gets or sets the pointer to value strings table.
        /// </summary>
        /// <remarks>Pointer to pointer.</remarks>
        public int ValueStringPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the field ids table.
        /// </summary>
        /// <remarks>Pointer to pointer.</remarks>
        public int FieldIdPointer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fields are invalid (??).
        /// </summary>
        [BinaryBoolean(ReadAs = typeof(int), WriteAs = typeof(int), TrueValue = -1, FalseValue = 0)]
        public bool FieldInvalid { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the record order list.
        /// </summary>
        public int RecordOrderPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the field order list.
        /// </summary>
        public int FieldOrderPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the field existence (validity) bits.
        /// </summary>
        public int FieldExistencePointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the indexer (??).
        /// </summary>
        public int IndexerPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the game variable field list.
        /// </summary>
        public int GameVarFieldTypePointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the table of empty values.
        /// </summary>
        /// <remarks>Pointer to pointer.</remarks>
        public int EmptyValuesPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the record member info (type) list.
        /// </summary>
        public int RawRecordMemberInfoPointer { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the field info list.
        /// </summary>
        public int FieldInfoPointer { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"Id                         = {Id}");
            _ = stringBuilder.AppendLine($"Flags                      = {Flags}\t{Flags:X2}");
            _ = stringBuilder.AppendLine($"RecordCount                = {RecordCount}\t{RecordCount:X8}");
            _ = stringBuilder.AppendLine($"FieldCount                 = {FieldCount}\t{FieldCount:X8}");
            _ = stringBuilder.AppendLine($"ValueStringCount           = {ValueStringCount}\t{ValueStringCount:X8}");
            _ = stringBuilder.AppendLine($"RecordInvalid              = {(RecordInvalid ? -1 : 0)}");
            _ = stringBuilder.AppendLine($"FieldInvalid               = {(FieldInvalid ? -1 : 0)}");
            _ = stringBuilder.AppendLine($"IndexerPointer             = {IndexerPointer}\t{IndexerPointer:X8}");
            _ = stringBuilder.AppendLine($"RecordExistencePointer     = {RecordExistencePointer}\t{RecordExistencePointer:X8}");
            _ = stringBuilder.AppendLine($"FieldExistencePointer      = {FieldExistencePointer}\t{FieldExistencePointer:X8}");
            _ = stringBuilder.AppendLine($"RecordIdPointer            = {RecordIdPointer}\t{RecordIdPointer:X8}");
            _ = stringBuilder.AppendLine($"FieldIdPointer             = {FieldIdPointer}\t{FieldIdPointer:X8}");
            _ = stringBuilder.AppendLine($"ValueStringPointer         = {ValueStringPointer}\t{ValueStringPointer:X8}");
            _ = stringBuilder.AppendLine($"FieldTypePointer           = {FieldTypePointer}\t{FieldTypePointer:X8}");
            _ = stringBuilder.AppendLine($"RawRecordMemberInfoPointer = {RawRecordMemberInfoPointer}\t{RawRecordMemberInfoPointer:X8}");
            _ = stringBuilder.AppendLine($"ValuesPointer              = {ValuesPointer}\t{ValuesPointer:X8}");
            _ = stringBuilder.AppendLine($"EmptyValuesPointer         = {EmptyValuesPointer}\t{EmptyValuesPointer:X8}");
            _ = stringBuilder.AppendLine($"RecordOrderPointer         = {RecordOrderPointer}\t{RecordOrderPointer:X8}");
            _ = stringBuilder.AppendLine($"FieldOrderPointer          = {FieldOrderPointer}\t{FieldOrderPointer:X8}");
            _ = stringBuilder.AppendLine($"FieldInfoPointer           = {FieldInfoPointer}\t{FieldInfoPointer:X8}");
            _ = stringBuilder.AppendLine($"GameVarFieldTypePointer    = {GameVarFieldTypePointer}\t{GameVarFieldTypePointer:X8}");
            _ = stringBuilder.AppendLine("=====================================");
            return stringBuilder.ToString();
        }
    }
}
