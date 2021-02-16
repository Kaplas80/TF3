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
    using System;
    using System.Collections;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Types;
    using Yarhl.FileFormat;

    /// <summary>
    /// Dragon Engine ARMP table.
    /// </summary>
    public sealed class ArmpTable : IFormat, IEquatable<ArmpTable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        /// <param name="id">Table id.</param>
        /// <param name="flags">Table flags.</param>
        /// <param name="recordCount">Record count.</param>
        /// <param name="recordInvalid">Record invalid.</param>
        /// <param name="fieldCount">Field count.</param>
        /// <param name="fieldInvalid">Field invalid.</param>
        /// <param name="valueStringCount">Value strings count.</param>
        public ArmpTable(int id, byte flags, int recordCount, bool recordInvalid, int fieldCount, bool fieldInvalid, int valueStringCount)
        {
            Id = id;
            Flags = flags;

            RecordCount = recordCount;
            RecordInvalid = recordInvalid;

            FieldCount = fieldCount;
            FieldInvalid = fieldInvalid;

            ValueStringCount = valueStringCount;

            Indexer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        /// <param name="header">Table info.</param>
        public ArmpTable(ArmpTableHeader header)
        {
            Id = header.Id;
            Flags = header.Flags;

            RecordCount = header.RecordCount;
            RecordInvalid = header.RecordInvalid;

            FieldCount = header.FieldCount;
            FieldInvalid = header.FieldInvalid;

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
        public bool RecordInvalid { get; }

        /// <summary>
        /// Gets a value indicating whether the table fields has invalid values.
        /// </summary>
        public bool FieldInvalid { get; }

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

        /// <summary>
        /// Gets or sets the game var field type.
        /// </summary>
        public int[] GameVarFieldType { get; set; }

        /// <inheritdoc/>
        public bool Equals(ArmpTable other)
        {
            if (other == null) {
                return false;
            }

            if (Id != other.Id ||
                Flags != other.Flags ||
                RecordCount != other.RecordCount ||
                FieldCount != other.FieldCount ||
                ValueStringCount != other.ValueStringCount ||
                RecordInvalid != other.RecordInvalid ||
                FieldInvalid != other.FieldInvalid) {
                return false;
            }

            if (RecordExistence != null && !((IStructuralEquatable)RecordExistence).Equals(other.RecordExistence, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (FieldExistence != null && !((IStructuralEquatable)FieldExistence).Equals(other.FieldExistence, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)RecordIds).Equals(other.RecordIds, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)FieldIds).Equals(other.FieldIds, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)ValueStrings).Equals(other.ValueStrings, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)FieldTypes).Equals(other.FieldTypes, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)RawRecordMemberInfo).Equals(other.RawRecordMemberInfo, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            for (int i = 0; i < FieldCount; i++) {
                if (Values[i] == null && other.Values[i] != null) {
                    return false;
                }

                if (Values[i] != null && !((IStructuralEquatable)Values[i]).Equals(other.Values[i], StructuralComparisons.StructuralEqualityComparer)) {
                    return false;
                }
            }

            if (!((IStructuralEquatable)EmptyValues).Equals(other.EmptyValues, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)RecordOrder).Equals(other.RecordOrder, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)FieldOrder).Equals(other.FieldOrder, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)FieldInfo).Equals(other.FieldInfo, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (!((IStructuralEquatable)GameVarFieldType).Equals(other.GameVarFieldType, StructuralComparisons.StructuralEqualityComparer)) {
                return false;
            }

            if (Indexer?.Equals(other.Indexer) == false) {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as ArmpTable);
    }
}
