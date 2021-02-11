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
    using System.Text;
    using TF3.Common.Yakuza.Enums;
    using TF3.Common.Yakuza.Types;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using TF3.Plugin.YakuzaKiwami2.Types;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Deserializes ARMP files.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, ArmpTable>
    {
        /// <summary>
        /// Converts a BinaryFormat into a Armp.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The Armp format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public ArmpTable Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.UTF8,
                Endianness = EndiannessMode.LittleEndian,
            };

            // Read the file header
            var header = reader.Read<FileHeader>() as FileHeader;
            CheckHeader(header);

            if (header.Endianness == Endianness.BigEndian) {
                reader.Endianness = EndiannessMode.BigEndian;
            }

            int mainTableOffset = reader.ReadInt32();

            return ReadTable(reader, mainTableOffset);
        }

        private static void CheckHeader(FileHeader header)
        {
            if (header == null) {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "armp") {
                throw new FormatException($"armp: Bad magic Id ({header.Magic} != armp)");
            }
        }

        private ArmpTable ReadTable(DataReader reader, long fileOffset)
        {
            reader.Stream.PushToPosition(fileOffset);

            var header = reader.Read<ArmpTableHeader>() as ArmpTableHeader;
            var table = new ArmpTable(header.Id, header.Flags, header.RecordCount, header.FieldCount, header.ValueStringCount) {
                AreFieldsInvalid = header.FieldInvalid,
                AreRecordsInvalid = header.RecordInvalid,
            };

            ReadRecordExistence(reader, table, header.RecordExistencePointer);
            ReadFieldExistence(reader, table, header.FieldExistencePointer);
            ReadRecordIds(reader, table, header.RecordIdPointer);
            ReadFieldIds(reader, table, header.FieldIdPointer);
            ReadValueStrings(reader, table, header.ValueStringPointer);
            ReadFieldTypes(reader, table, header.FieldTypePointer);
            ReadRecordMemberInfo(reader, table, header.RawRecordMemberInfoPointer);

            ReadRecordOrder(reader, table, header.RecordOrderPointer);
            ReadFieldOrder(reader, table, header.FieldOrderPointer);
            ReadFieldInfo(reader, table, header.FieldInfoPointer);

            ReadIndexer(reader, table, header.IndexerPointer);
            ReadEmptyValues(reader, table, header.EmptyValuesPointer);

            ReadValues(reader, table, header.ValuesPointer);

            reader.Stream.PopPosition();
            return table;
        }

        private void ReadRecordExistence(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadBits(reader, (i, v) => table.SetRecordExistence(i, (bool)v), table.RecordCount, offset);
        }

        private void ReadFieldExistence(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadBits(reader, (i, v) => table.SetFieldExistence(i, (bool)v), table.FieldCount, offset);
        }

        private void ReadRecordIds(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadStrings(reader, table.SetRecordId, table.RecordCount, offset);
        }

        private void ReadFieldIds(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadStrings(reader, table.SetFieldId, table.FieldCount, offset);
        }

        private void ReadValueStrings(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0 || table.ValueStringCount == 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);
            ReadStrings(reader, table.SetValueString, table.ValueStringCount, offset);
        }

        private void ReadFieldTypes(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadTypes(reader, table.SetFieldType, table.FieldCount, offset);
        }

        private void ReadRecordMemberInfo(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            ReadTypes(reader, table.SetRawRecordMemberInfo, table.FieldCount, offset);
        }

        private void ReadFieldOrder(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < table.FieldCount; i++) {
                int value = reader.ReadInt32();
                table.SetFieldOrder(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadRecordOrder(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < table.RecordCount; i++) {
                int value = reader.ReadInt32();
                table.SetRecordOrder(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadFieldInfo(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < table.RecordCount; i++) {
                byte value = reader.ReadByte();
                table.SetFieldInfo(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadValues(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);

            for (int i = 0; i < table.FieldCount; i++) {
                FieldType fieldType = table.GetRawRecordMemberInfo(i);
                int dataOffset = reader.ReadInt32();
                if (dataOffset == -1 || fieldType == FieldType.Unused) {
                    continue;
                }

                Action<int, object> setValue = (index, value) => table.SetValue(index, i, value);
                switch (fieldType) {
                    case FieldType.Unused:
                        break;
                    case FieldType.UInt8:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(byte));
                        break;
                    case FieldType.UInt16:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(ushort));
                        break;
                    case FieldType.UInt32:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(uint));
                        break;
                    case FieldType.UInt64:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(ulong));
                        break;
                    case FieldType.Int8:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(sbyte));
                        break;
                    case FieldType.Int16:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(short));
                        break;
                    case FieldType.Int32:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(int));
                        break;
                    case FieldType.Int64:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(long));
                        break;
                    case FieldType.Float16:
                        throw new FormatException("Float16 not supported.");
                    case FieldType.Float32:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(float));
                        break;
                    case FieldType.Float64:
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(double));
                        break;
                    case FieldType.Boolean:
                        // Booleans are read as a bitmask
                        ReadBits(reader, setValue, table.RecordCount, dataOffset);
                        break;
                    case FieldType.String:
                        // Strings are stored as Value String index
                        ReadNumbers(reader, setValue, table.RecordCount, dataOffset, typeof(int));
                        break;
                    case FieldType.Table:
                        ReadSubTables(reader, setValue, table.RecordCount, dataOffset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown field type: {fieldType}");
                }
            }

            reader.Stream.PopPosition();
        }

        private void ReadEmptyValues(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset <= 0) {
                return;
            }

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < table.FieldCount; i++) {
                int dataOffset = reader.ReadInt32();
                if (dataOffset <= 0) {
                    continue;
                }

                reader.Stream.PushToPosition(dataOffset);
                byte current = reader.ReadByte();
                int bitsLeft = 8;
                for (int j = 0; j < table.RecordCount; j++) {
                    bool isEmpty = (current & 0x01) == 0x01;
                    current >>= 0x01;
                    bitsLeft--;
                    if (bitsLeft == 0) {
                        current = reader.ReadByte();
                        bitsLeft = 8;
                    }

                    table.SetIsNullValue(j, i, isEmpty);
                }

                reader.Stream.PopPosition();
            }

            reader.Stream.PopPosition();
        }

        private void ReadIndexer(DataReader reader, ArmpTable table, in int offset)
        {
            if (offset > 0) {
                table.Indexer = ReadTable(reader, offset);
            }
        }

        private void ReadBits(DataReader reader, Action<int, object> setBitAsBoolean, in int bitCount, in int offset)
        {
            reader.Stream.PushToPosition(offset);

            byte current = reader.ReadByte();
            int bitsLeft = 8;
            for (int i = 0; i < bitCount; i++) {
                bool isTrue = (current & 0x01) == 0x01;
                current >>= 0x01;
                bitsLeft--;
                if (bitsLeft == 0) {
                    current = reader.ReadByte();
                    bitsLeft = 8;
                }

                setBitAsBoolean(i, isTrue);
            }

            reader.Stream.PopPosition();
        }

        private void ReadStrings(DataReader reader, Action<int, string> setString, in int stringCount, in int offset)
        {
            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < stringCount; i++) {
                int idOffset = reader.ReadInt32();
                reader.Stream.PushToPosition(idOffset);
                string value = reader.ReadString();
                reader.Stream.PopPosition();

                setString(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadNumbers(DataReader reader, Action<int, object> setValue, in int count, in int offset, in Type type)
        {
            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < count; i++) {
                object value = reader.ReadByType(type);
                setValue(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadTypes(DataReader reader, Action<int, FieldType> setType, in int count, in int offset)
        {
            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < count; i++) {
                var value = (FieldType)Enum.ToObject(typeof(FieldType), reader.ReadByte());
                setType(i, value);
            }

            reader.Stream.PopPosition();
        }

        private void ReadSubTables(DataReader reader, Action<int, object> setValue, in int count, in int offset)
        {
            reader.Stream.PushToPosition(offset);

            for (int i = 0; i < count; i++) {
                ArmpTable subTable = null;
                long subTablePointer = reader.ReadInt64();
                reader.Stream.PushCurrentPosition();
                if (subTablePointer > 0) {
                    subTable = ReadTable(reader, subTablePointer);
                }

                reader.Stream.PopPosition();

                setValue(i, subTable);
            }

            reader.Stream.PopPosition();
        }
    }
}
