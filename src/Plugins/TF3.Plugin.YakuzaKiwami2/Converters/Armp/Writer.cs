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
            writer.WritePadding(0x00, 16);
            writer.Stream.Seek(0x10, SeekMode.Start);
            writer.Write(tableOffset);

            return new BinaryFormat(stream);
        }

        private static int WriteNumbers(DataWriter writer, object[] values, Type type, int padding, ref int offset)
        {
            int result = offset;

            long startPos = writer.Stream.Position;
            for (int i = 0; i < values.Length; i++) {
                writer.WriteOfType(type, values[i]);
            }

            if (padding > 0) {
                writer.WritePadding(0x00, padding);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);

            return result;
        }

        private static int WriteBits(DataWriter writer, bool[] values, int padding, ref int offset)
        {
            int result = offset;
            long startPos = writer.Stream.Position;
            int numBytes = (int)Math.Ceiling(values.Length / 8.0);
            if (values.Length % 4 == 0) {
                numBytes++;
            }

            byte[] temp = new byte[numBytes];
            for (int i = 0; i < values.Length; i++) {
                if (values[i]) {
                    int byteIndex = Math.DivRem(i, 8, out int bitIndex);
                    temp[byteIndex] |= (byte)(0x01 << bitIndex);
                }
            }

            writer.Write(temp);

            if (padding > 0) {
                writer.WritePadding(0x00, padding);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);

            return result;
        }

        private static int WriteStrings(DataWriter writer, string[] values, int padding, ref int offset)
        {
            long startPos;
            long endPos;
            var offsets = new List<int>();
            for (int i = 0; i < values.Length; i++) {
                offsets.Add(offset);
                string id = values[i];
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

            if (padding > 0) {
                writer.WritePadding(0x00, padding);
            }

            endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
            return pointer;
        }

        private static int WriteTypes(DataWriter writer, FieldType[] values, int padding, ref int offset)
        {
            int result = offset;

            long startPos = writer.Stream.Position;
            for (int i = 0; i < values.Length; i++) {
                writer.Write((byte)values[i]);
            }

            if (padding > 0) {
                writer.WritePadding(0x00, padding);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);

            return result;
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
                RecordInvalid = table.RecordInvalid,

                RecordIdPointer = table.RecordIds == null ? -1 : 0,
                RecordExistencePointer = table.RecordExistence == null ? -1 : 0,
                FieldTypePointer = table.FieldTypes == null ? -1 : 0,
                ValuesPointer = 0,

                Id = table.Id,
                Flags = table.Flags,
                ValueStringPointer = 0,
                FieldIdPointer = table.FieldIds == null ? -1 : 0,
                FieldInvalid = table.FieldInvalid,

                RecordOrderPointer = table.RecordOrder == null ? -1 : 0,
                FieldOrderPointer = table.FieldOrder == null ? -1 : 0,
                FieldExistencePointer = table.FieldExistence == null ? -1 : 0,
                IndexerPointer = 0,

                GameVarFieldTypePointer = 0,
                EmptyValuesPointer = table.EmptyValues == null ? -1 : 0,
                RawRecordMemberInfoPointer = table.RawRecordMemberInfo == null ? -1 : 0,
                FieldInfoPointer = table.FieldInfo == null ? -1 : 0,
            };

            int currentOffset = baseOffset;
            long[][] subTablesOffsets = new long[table.FieldCount][];

            // 1. Child tables
            for (int field = 0; field < table.FieldCount; field++) {
                subTablesOffsets[field] = new long[table.RecordCount];

                if (table.RawRecordMemberInfo[field] != Enums.FieldType.Table) {
                    continue;
                }

                object[] data = table.Values[field];
                if (data == null) {
                    continue;
                }

                for (int record = 0; record < table.RecordCount; record++) {
                    var subTable = (ArmpTable)data[record];
                    if (subTable == null) {
                        subTablesOffsets[field][record] = 0x00000000;
                        continue;
                    }

                    (byte[] subTableData, int subTableOffset) = SerializeTable(subTable, currentOffset);
                    long startPos = writer.Stream.Position;
                    writer.Write(subTableData);
                    subTablesOffsets[field][record] = subTableOffset;
                    writer.WritePadding(0x00, 0x10);
                    long endPos = writer.Stream.Position;
                    currentOffset += (int)(endPos - startPos);
                }
            }

            // 2. Indexer
            if (table.Indexer != null) {
                (byte[] indexerData, int indexerOffset) = SerializeTable(table.Indexer, currentOffset);
                long startPos = writer.Stream.Position;
                writer.Write(indexerData);
                header.IndexerPointer = indexerOffset;
                writer.WritePadding(0x00, 0x10);
                long endPos = writer.Stream.Position;
                currentOffset += (int)(endPos - startPos);
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
            WriteEmptyValues(writer, table, header, ref currentOffset);
            WriteRecordOrder(writer, table, header, ref currentOffset);
            WriteFieldOrder(writer, table, header, ref currentOffset);
            WriteFieldInfo(writer, table, header, ref currentOffset);
            WriteGameVarFieldType(writer, table, header, ref currentOffset);

            writer.Stream.Seek(headerOffset, SeekMode.Start);
            writer.WriteOfType(header);

            return new Tuple<byte[], int>(currentTable.ToArray(), currentTableOffset);
        }

        private void WriteRecordExistence(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.RecordExistence == null) {
                return;
            }

            if (table.RecordExistence.Length == 0) {
                return;
            }

            header.RecordExistencePointer = WriteBits(writer, table.RecordExistence, 4, ref offset);
        }

        private void WriteFieldExistence(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.FieldExistence == null) {
                return;
            }

            if (table.FieldExistence.Length == 0) {
                writer.Write(0);
                offset += 4;
                return;
            }

            header.FieldExistencePointer = WriteBits(writer, table.FieldExistence, 4, ref offset);
        }

        private void WriteRecordIds(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.RecordIds.Length == 0) {
                return;
            }

            header.RecordIdPointer = WriteStrings(writer, table.RecordIds, 0, ref offset);
        }

        private void WriteFieldIds(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.FieldIds.Length == 0) {
                return;
            }

            header.FieldIdPointer = WriteStrings(writer, table.FieldIds, 0, ref offset);
        }

        private void WriteValueStrings(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.ValueStrings.Length == 0) {
                long startPos = writer.Stream.Position;
                writer.Write(0);
                writer.WritePadding(0x00, 16);
                long endPos = writer.Stream.Position;
                offset += (int)(endPos - startPos);
                header.ValueStringPointer = offset;
                return;
            }

            header.ValueStringPointer = WriteStrings(writer, table.ValueStrings, 8, ref offset);
        }

        private void WriteFieldTypes(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.FieldTypes.Length == 0) {
                return;
            }

            header.FieldTypePointer = WriteTypes(writer, table.FieldTypes, table.FieldCount <= 2 ? 4 : 8, ref offset);
        }

        private void WriteRecordMemberInfo(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.RawRecordMemberInfo.Length == 0) {
                return;
            }

            header.RawRecordMemberInfoPointer = WriteTypes(writer, table.RawRecordMemberInfo, table.FieldCount <= 2 ? 4 : 0, ref offset);
        }

        private void WriteValues(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, long[][] subTablesOffsets, ref int offset)
        {
            int[] offsets = new int[table.FieldCount];

            for (int i = 0; i < table.FieldCount; i++) {
                FieldType fieldType = table.RawRecordMemberInfo[i];
                if (fieldType == FieldType.Unused) {
                    offsets[i] = 0;
                    continue;
                }

                object[] data = table.Values[i];
                if (data == null) {
                    offsets[i] = -1;
                    continue;
                }

                switch (fieldType) {
                    case FieldType.UInt8:
                        offsets[i] = WriteNumbers(writer, data, typeof(byte), 4, ref offset);
                        break;
                    case FieldType.UInt16:
                        offsets[i] = WriteNumbers(writer, data, typeof(ushort), 4, ref offset);
                        break;
                    case FieldType.UInt32:
                        offsets[i] = WriteNumbers(writer, data, typeof(uint), 8, ref offset);
                        break;
                    case FieldType.UInt64:
                        offsets[i] = WriteNumbers(writer, data, typeof(ulong), 8, ref offset);
                        break;
                    case FieldType.Int8:
                        offsets[i] = WriteNumbers(writer, data, typeof(sbyte), 4, ref offset);
                        break;
                    case FieldType.Int16:
                        offsets[i] = WriteNumbers(writer, data, typeof(short), 4, ref offset);
                        break;
                    case FieldType.Int32:
                        offsets[i] = WriteNumbers(writer, data, typeof(int), 8, ref offset);
                        break;
                    case FieldType.Int64:
                        offsets[i] = WriteNumbers(writer, data, typeof(long), 8, ref offset);
                        break;
                    case FieldType.Float16:
                        throw new FormatException("Float16 not supported.");
                    case FieldType.Float32:
                        offsets[i] = WriteNumbers(writer, data, typeof(float), 8, ref offset);
                        break;
                    case FieldType.Float64:
                        offsets[i] = WriteNumbers(writer, data, typeof(double), 8, ref offset);
                        break;
                    case FieldType.Boolean:
                        // Booleans are read as a bitmask
                        try {
                            bool[] temp = Array.ConvertAll(data, x => (bool)x);
                            offsets[i] = WriteBits(writer, temp, 4, ref offset);
                        }
                        catch (NullReferenceException) {
                            offsets[i] = -1;
                            continue;
                        }

                        break;
                    case FieldType.String:
                        // Strings are stored as Value String index
                        offsets[i] = WriteNumbers(writer, data, typeof(int), 8, ref offset);
                        break;
                    case FieldType.Table:
                        offsets[i] = WriteSubTables(writer, subTablesOffsets[i], 8, ref offset);
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

            long mod = (writer.Stream.Position - startPos) % 8;
            if (mod > 0) {
                writer.WriteTimes(0x00, 8 - mod);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private int WriteSubTables(DataWriter writer, long[] offsets, int padding, ref int offset)
        {
            int result = offset;

            long startPos = writer.Stream.Position;
            for (int i = 0; i < offsets.Length; i++) {
                writer.Write(offsets[i]);
            }

            if (padding > 0) {
                writer.WritePadding(0x00, padding);
            }

            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);

            return result;
        }

        private void WriteEmptyValues(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.EmptyValues.Length == 0) {
                return;
            }

            int[] offsets = new int[table.FieldCount];
            for (int i = 0; i < table.FieldCount; i++) {
                bool[] data = table.EmptyValues[i];
                offsets[i] = data == null ? -1 : WriteBits(writer, data, 4, ref offset);
            }

            header.EmptyValuesPointer = offset;
            long startPos = writer.Stream.Position;
            foreach (int o in offsets) {
                writer.Write(o);
            }

            writer.WritePadding(0x00, 0x08);
            long endPos = writer.Stream.Position;
            offset += (int)(endPos - startPos);
        }

        private void WriteRecordOrder(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.RecordOrder.Length == 0) {
                return;
            }

            object[] temp = Array.ConvertAll(table.RecordOrder, x => (object)x);
            long startPos = writer.Stream.Position;
            header.RecordOrderPointer = WriteNumbers(writer, temp, typeof(int), 0, ref offset);
            long mod = (writer.Stream.Position - startPos) % 8;
            if (mod > 0) {
                writer.WriteTimes(0x00, 8 - mod);
                offset += (int)(8 - mod);
            }
        }

        private void WriteFieldOrder(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.FieldOrder.Length == 0) {
                return;
            }

            object[] temp = Array.ConvertAll(table.FieldOrder, x => (object)x);
            header.FieldOrderPointer = WriteNumbers(writer, temp, typeof(int), 16, ref offset);
        }

        private void WriteFieldInfo(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.FieldInfo.Length == 0) {
                return;
            }

            object[] temp = Array.ConvertAll(table.FieldInfo, x => (object)x);
            header.FieldInfoPointer = WriteNumbers(writer, temp, typeof(byte), 8, ref offset);
        }

        private void WriteGameVarFieldType(DataWriter writer, ArmpTable table, Types.ArmpTableHeader header, ref int offset)
        {
            if (table.GameVarFieldType.Length == 0) {
                return;
            }

            object[] temp = Array.ConvertAll(table.GameVarFieldType, x => (object)x);
            header.GameVarFieldTypePointer = WriteNumbers(writer, temp, typeof(int), 8, ref offset);
        }
    }
}
