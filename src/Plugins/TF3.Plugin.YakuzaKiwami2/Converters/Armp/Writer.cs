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
namespace TF3.Plugin.YakuzaKiwami2.Converters.Armp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Serializes Yakuza Kiwami 2 ARMP files.
    /// </summary>
    public class Writer : IConverter<ArmpTable, BinaryFormat>
    {
        /// <summary>
        /// Converts an Armp into a Binary Format.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The binary format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public BinaryFormat Convert(ArmpTable source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            DataStream stream = DataStreamFactory.FromMemory();

            var writer = new DataWriter(stream) {
                DefaultEncoding = Encoding.UTF8,
                Endianness = EndiannessMode.LittleEndian,
            };

            var header = new FileHeader {
                Magic = "armp",
                PlatformId = Platform.Win32,
                Endianness = Endianness.LittleEndian,
                SizeExtended = 0,
                Relocated = 0,
                Version = 0x0001000C,
                Size = 0,
            };

            writer.WriteOfType(header);
            writer.WriteTimes(0x00, 0x10); // Main table pointer

            (byte[] tableData, int tableOffset) = SerializeTable(source, 0x20);

            writer.Write(tableData);
            writer.Stream.Seek(0x10, SeekMode.Start);
            writer.Write(tableOffset);

            return new BinaryFormat(stream);
        }

        private Tuple<byte[], int> SerializeTable(ArmpTable table, int baseOffset)
        {
            using var currentTable = new System.IO.MemoryStream();
            using DataStream ds = DataStreamFactory.FromStream(currentTable);
            var writer = new DataWriter(ds) {
                DefaultEncoding = Encoding.UTF8,
                Endianness = EndiannessMode.LittleEndian,
            };

            var header = new Types.ArmpTableHeader {
                RecordCount = table.RecordCount,
                FieldCount = table.FieldCount,
                ValueStringCount = table.ValueStringCount,
                RecordInvalid = table.AreRecordsInvalid,

                RecordIdPointer = 0,
                RecordExistencePointer = 0,
                FieldTypePointer = 0,
                ValuesPointer = 0,

                Id = table.Id,
                Flags = table.Flags,
                ValueStringPointer = 0,
                FieldIdPointer = 0,
                FieldInvalid = table.AreFieldsInvalid,

                RecordOrderPointer = 0,
                FieldOrderPointer = 0,
                FieldExistencePointer = 0,
                IndexerPointer = 0,

                GameVarFieldTypePointer = 0,
                EmptyValuesPointer = 0,
                RawRecordMemberInfoPointer = 0,
                FieldInfoPointer = 0,
            };

            int currentOffset = baseOffset;
            var subTablesOffsets = new List<int>();

            // 1. Child tables
            for (int row = 0; row < table.RecordCount; row++) {
                for (int col = 0; col < table.FieldCount; col++) {
                    if (table.GetRawRecordMemberInfo(col) != Enums.FieldType.Table) {
                        continue;
                    }

                    var subTable = (ArmpTable)table.GetValue(row, col);
                    if (subTable == null) {
                        subTablesOffsets.Add(0x00000000);
                        continue;
                    }

                    (byte[] subTableData, int subTableOffset) = SerializeTable(subTable, currentOffset);

                    writer.Write(subTableData);
                    subTablesOffsets.Add(subTableOffset);

                    currentOffset += subTableOffset;
                }
            }

            // 2. Indexer
            int indexerOffset = 0;
            if (table.Indexer != null) {
                (byte[] indexerData, int indexerOffsetAux) = SerializeTable(table.Indexer, currentOffset);
                writer.Write(indexerData);
                indexerOffset = indexerOffsetAux;
                currentOffset += indexerOffsetAux;
            }

            int currentTableOffset = currentOffset;

            long headerOffset = writer.Stream.Position;
            writer.WriteTimes(0x00, 0x50);
            currentOffset += 0x50; // Header size

            WriteRecordExistence(writer, table, header, ref currentOffset);
            WriteFieldExistence(writer, table, header, ref currentOffset);
            WriteRecordIds(writer, table, header, ref currentOffset);
            WriteFieldIds(writer, table, header, ref currentOffset);
            WriteValueStrings(writer, table, header, ref currentOffset);
            WriteFieldTypes(writer, table, header, ref currentOffset);
            WriteRecordMemberInfo(writer, table, header, ref currentOffset);
            WriteValues(writer, table, header, subTablesOffsets, ref currentOffset);
            WriteRecordOrder(writer, table, header, ref currentOffset);
            WriteFieldOrder(writer, table, header, ref currentOffset);
            WriteFieldInfo(writer, table, header, ref currentOffset);

            writer.Stream.Seek(headerOffset, SeekMode.Start);
            writer.WriteOfType(header);

            return new Tuple<byte[], int>(currentTable.ToArray(), currentTableOffset);
        }

        private void WriteRecordExistence(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasRecordExistenceData) {
                return;
            }

            header.RecordExistencePointer = offset;
            WriteBits(writer, (i) => table.GetRecordExistence(i), table.RecordCount, true, ref offset);
        }

        private void WriteFieldExistence(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasFieldExistenceData) {
                return;
            }

            header.FieldExistencePointer = offset;
            WriteBits(writer, (i) => table.GetFieldExistence(i), table.FieldCount, true, ref offset);
        }

        private void WriteRecordIds(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasRecordIds) {
                return;
            }

            header.RecordIdPointer = WriteStrings(writer, table.GetRecordId, table.RecordCount, false, ref offset);
        }

        private void WriteFieldIds(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasFieldIds) {
                return;
            }

            header.FieldIdPointer = WriteStrings(writer, table.GetFieldId, table.FieldCount, false, ref offset);
        }

        private void WriteValueStrings(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.ValueStringCount == 0) {
                return;
            }

            header.ValueStringPointer = WriteStrings(writer, table.GetValueString, table.ValueStringCount, false, ref offset);
        }

        private void WriteFieldTypes(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasFieldTypes) {
                return;
            }

            header.FieldTypePointer = offset;
            WriteTypes(writer, table.GetFieldType, table.FieldCount, true, ref offset);
        }

        private void WriteRecordMemberInfo(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (!table.HasFieldTypes) {
                return;
            }

            header.RawRecordMemberInfoPointer = offset;
            WriteTypes(writer, table.GetRawRecordMemberInfo, table.FieldCount, false, ref offset);
        }

        private void WriteValues(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, List<int> subTablesOffsets, ref int offset)
        {
            int[] offsets = new int[table.FieldCount];

            for (int i = 0; i < table.FieldCount; i++) {
                FieldType fieldType = table.GetRawRecordMemberInfo(i);
                if (fieldType == FieldType.Unused) {
                    offsets[i] = 0;
                    continue;
                }

                offsets[i] = offset;

                Func<int, object> getValue = (index) => table.GetValue(index, i);
                switch (fieldType) {
                    case FieldType.UInt8:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(byte), true, ref offset);
                        break;
                    case FieldType.UInt16:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(ushort), true, ref offset);
                        break;
                    case FieldType.UInt32:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(uint), true, ref offset);
                        break;
                    case FieldType.UInt64:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(ulong), true, ref offset);
                        break;
                    case FieldType.Int8:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(sbyte), true, ref offset);
                        break;
                    case FieldType.Int16:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(short), true, ref offset);
                        break;
                    case FieldType.Int32:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(int), true, ref offset);
                        break;
                    case FieldType.Int64:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(long), true, ref offset);
                        break;
                    case FieldType.Float16:
                        throw new FormatException("Float16 not supported.");
                    case FieldType.Float32:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(float), true, ref offset);
                        break;
                    case FieldType.Float64:
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(double), true, ref offset);
                        break;
                    case FieldType.Boolean:
                        // Booleans are read as a bitmask
                        WriteBits(writer, getValue, table.RecordCount, true, ref offset);
                        break;
                    case FieldType.String:
                        // Strings are stored as Value String index
                        WriteNumbers(writer, getValue, table.RecordCount, typeof(int), true, ref offset);
                        break;
                    case FieldType.Table:
                        WriteSubTables(writer, subTablesOffsets, true, ref offset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown field type: {fieldType}");
                }
            }

            header.ValuesPointer = offset;
            long startPos = writer.Stream.Position;
            foreach (int o in offsets) {
                writer.Write(o);
            }

            writer.WritePadding(0x00, 0x08);
            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private void WriteBits(DataWriter writer, Func<int, object> getBitAsBoolean, in int bitCount, bool writePadding, ref int offset)
        {
            long startPos = writer.Stream.Position;
            int numBytes = (int)Math.Ceiling(bitCount / 8.0);

            byte[] values = new byte[numBytes];
            for (int j = 0; j < bitCount; j++) {
                bool value = (bool)getBitAsBoolean(j);

                if (value) {
                    int byteIndex = Math.DivRem(j, 8, out int bitIndex);
                    values[byteIndex] |= (byte)(0x01 << bitIndex);
                }
            }

            writer.Write(values);

            if (writePadding) {
                writer.WritePadding(0x00, 0x04);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private int WriteStrings(DataWriter writer, Func<int, string> getString, in int stringCount, in bool writePadding, ref int offset)
        {
            long startPos;
            long endPos;
            var offsets = new List<int>();
            for (int i = 0; i < stringCount; i++) {
                offsets.Add(offset);
                string id = getString(i);
                startPos = writer.Stream.Position;
                writer.Write(id);
                endPos = writer.Stream.Position;
                offset += (int)(endPos - startPos);
            }

            startPos = writer.Stream.Position;
            writer.WritePadding(0x00, 0x10);
            endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);

            int pointer = offset;
            startPos = writer.Stream.Position;
            for (int i = 0; i < offsets.Count; i++) {
                writer.Write(offsets[i]);
            }

            if (writePadding) {
                writer.WritePadding(0x00, 0x08);
            }

            endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
            return pointer;
        }

        private void WriteTypes(DataWriter writer, Func<int, FieldType> getType, in int count, in bool writePadding, ref int offset)
        {
            long startPos = writer.Stream.Position;
            for (int i = 0; i < count; i++) {
                FieldType fieldType = getType(i);
                writer.Write((byte)fieldType);
            }

            if (writePadding) {
                writer.WritePadding(0x00, 0x08);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private void WriteNumbers(DataWriter writer, Func<int, object> getValue, in int count, in Type type, in bool writePadding, ref int offset)
        {
            long startPos = writer.Stream.Position;
            for (int i = 0; i < count; i++) {
                object value = getValue(i);
                writer.WriteOfType(type, value);
            }

            if (writePadding) {
                writer.WritePadding(0x00, 0x08);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private void WriteSubTables(DataWriter writer, List<int> offsets, bool writePadding, ref int offset)
        {
            long startPos = writer.Stream.Position;
            for (int i = 0; i < offsets.Count; i++) {
                writer.Write(offsets[i]);
            }

            if (writePadding) {
                writer.WritePadding(0x00, 0x08);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private void WriteRecordOrder(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            header.RecordOrderPointer = offset;
            WriteNumbers(writer, (i) => table.GetRecordOrder(i), table.RecordCount, typeof(int), true, ref offset);
        }

        private void WriteFieldOrder(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            header.FieldOrderPointer = offset;
            WriteNumbers(writer, (i) => table.GetFieldOrder(i), table.FieldCount, typeof(int), true, ref offset);
        }

        private void WriteFieldInfo(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            header.FieldInfoPointer = offset;
            WriteNumbers(writer, (i) => table.GetFieldInfo(i), table.RecordCount, typeof(byte), true, ref offset);
        }
    }
}
