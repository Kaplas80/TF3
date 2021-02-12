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

        private static object[] ReadNumbers(DataReader reader, int count, int offset, Type type)
        {
            object[] result = new object[count];

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < count; i++) {
                result[i] = (object)reader.ReadByType(type);
            }

            reader.Stream.PopPosition();

            return result;
        }

        private static bool[] ReadBits(DataReader reader, int count, int offset)
        {
            bool[] result = new bool[count];

            reader.Stream.PushToPosition(offset);
            byte current = reader.ReadByte();
            int bitsLeft = 8;
            for (int i = 0; i < count; i++) {
                bool isTrue = (current & 0x01) == 0x01;
                current >>= 0x01;
                bitsLeft--;
                if (bitsLeft == 0) {
                    current = reader.ReadByte();
                    bitsLeft = 8;
                }

                result[i] = isTrue;
            }

            reader.Stream.PopPosition();

            return result;
        }

        private static string[] ReadStrings(DataReader reader, int count, int offset)
        {
            string[] result = new string[count];

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < count; i++) {
                int idOffset = reader.ReadInt32();
                reader.Stream.PushToPosition(idOffset);
                string value = reader.ReadString();
                reader.Stream.PopPosition();

                result[i] = value;
            }

            reader.Stream.PopPosition();

            return result;
        }

        private static FieldType[] ReadTypes(DataReader reader, int count, int offset)
        {
            var result = new FieldType[count];

            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < count; i++) {
                result[i] = (FieldType)Enum.ToObject(typeof(FieldType), reader.ReadByte());
            }

            reader.Stream.PopPosition();

            return result;
        }

        private ArmpTable ReadTable(DataReader reader, long fileOffset)
        {
            reader.Stream.PushToPosition(fileOffset);

            var header = reader.Read<ArmpTableHeader>() as ArmpTableHeader;
            var table = new ArmpTable(header);

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

        private void ReadRecordExistence(DataReader reader, ArmpTable table, int offset)
        {
            table.RecordExistence = offset switch {
                -1 => null,
                0 => System.Array.Empty<bool>(),
                _ => ReadBits(reader, table.RecordCount, offset)
            };
        }

        private void ReadFieldExistence(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldExistence = offset switch {
                -1 => null,
                0 => System.Array.Empty<bool>(),
                _ => ReadBits(reader, table.FieldCount, offset)
            };
        }

        private void ReadRecordIds(DataReader reader, ArmpTable table, int offset)
        {
            table.RecordIds = offset switch {
                -1 => null,
                0 => System.Array.Empty<string>(),
                _ => ReadStrings(reader, table.RecordCount, offset)
            };
        }

        private void ReadFieldIds(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldIds = offset switch {
                -1 => null,
                0 => System.Array.Empty<string>(),
                _ => ReadStrings(reader, table.FieldCount, offset)
            };
        }

        private void ReadValueStrings(DataReader reader, ArmpTable table, int offset)
        {
            if (offset <= 0) {
                return;
            }

            table.ValueStrings = ReadStrings(reader, table.ValueStringCount, offset);
        }

        private void ReadFieldTypes(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldTypes = offset switch {
                -1 => null,
                0 => System.Array.Empty<FieldType>(),
                _ => ReadTypes(reader, table.FieldCount, offset)
            };
        }

        private void ReadRecordMemberInfo(DataReader reader, ArmpTable table, int offset)
        {
            table.RawRecordMemberInfo = offset switch {
                -1 => null,
                0 => System.Array.Empty<FieldType>(),
                _ => ReadTypes(reader, table.FieldCount, offset)
            };
        }

        private void ReadValues(DataReader reader, ArmpTable table, int offset)
        {
            if (offset <= 0) {
                return;
            }

            table.Values = new object[table.FieldCount][];
            reader.Stream.PushToPosition(offset);

            for (int i = 0; i < table.FieldCount; i++) {
                FieldType fieldType = table.RawRecordMemberInfo[i];
                int dataOffset = reader.ReadInt32();
                if (dataOffset == -1 || fieldType == FieldType.Unused) {
                    continue;
                }

                switch (fieldType) {
                    case FieldType.Unused:
                        break;
                    case FieldType.UInt8:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(byte));
                        break;
                    case FieldType.UInt16:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(ushort));
                        break;
                    case FieldType.UInt32:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(uint));
                        break;
                    case FieldType.UInt64:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(ulong));
                        break;
                    case FieldType.Int8:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(sbyte));
                        break;
                    case FieldType.Int16:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(short));
                        break;
                    case FieldType.Int32:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(int));
                        break;
                    case FieldType.Int64:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(long));
                        break;
                    case FieldType.Float16:
                        throw new FormatException("Float16 not supported.");
                    case FieldType.Float32:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(float));
                        break;
                    case FieldType.Float64:
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(double));
                        break;
                    case FieldType.Boolean:
                        // Booleans are read as a bitmask
                        bool[] temp = ReadBits(reader, table.RecordCount, dataOffset);
                        table.Values[i] = new object[table.RecordCount];
                        for (int j = 0; j < table.RecordCount; j++) {
                            table.Values[i][j] = temp[j];
                        }

                        break;
                    case FieldType.String:
                        // Strings are stored as Value String index
                        table.Values[i] = ReadNumbers(reader, table.RecordCount, dataOffset, typeof(int));
                        break;
                    case FieldType.Table:
                        table.Values[i] = ReadSubTables(reader, table.RecordCount, dataOffset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown field type: {fieldType}");
                }
            }

            reader.Stream.PopPosition();
        }

        private object[] ReadSubTables(DataReader reader, int count, int offset)
        {
            object[] result = new object[count];
            reader.Stream.PushToPosition(offset);

            for (int i = 0; i < count; i++) {
                ArmpTable subTable = null;
                long subTablePointer = reader.ReadInt64();
                reader.Stream.PushCurrentPosition();
                if (subTablePointer > 0) {
                    subTable = ReadTable(reader, subTablePointer);
                }

                reader.Stream.PopPosition();

                result[i] = subTable;
            }

            reader.Stream.PopPosition();

            return result;
        }

        private void ReadEmptyValues(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == -1) {
                table.EmptyValues = null;
                return;
            }

            if (offset == 0) {
                table.EmptyValues = Array.Empty<bool[]>();
                return;
            }

            table.EmptyValues = new bool[table.FieldCount][];
            reader.Stream.PushToPosition(offset);
            for (int i = 0; i < table.FieldCount; i++) {
                int dataOffset = reader.ReadInt32();
                if (dataOffset <= 0) {
                    table.EmptyValues[i] = null;
                    continue;
                }

                table.EmptyValues[i] = ReadBits(reader, table.RecordCount, dataOffset);
            }

            reader.Stream.PopPosition();
        }

        private void ReadRecordOrder(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == -1) {
                table.RecordOrder = null;
                return;
            }

            if (offset == 0) {
                table.RecordOrder = Array.Empty<int>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.RecordCount, offset, typeof(int));
            table.RecordOrder = new int[table.RecordCount];
            for (int i = 0; i < table.RecordCount; i++) {
                table.RecordOrder[i] = (int)temp[i];
            }
        }

        private void ReadFieldOrder(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == -1) {
                table.FieldOrder = null;
                return;
            }

            if (offset == 0) {
                table.FieldOrder = Array.Empty<int>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.FieldCount, offset, typeof(int));
            table.FieldOrder = new int[table.FieldCount];
            for (int i = 0; i < table.FieldCount; i++) {
                table.FieldOrder[i] = (int)temp[i];
            }
        }

        private void ReadFieldInfo(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == -1) {
                table.FieldInfo = null;
                return;
            }

            if (offset == 0) {
                table.FieldInfo = Array.Empty<byte>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.RecordCount, offset, typeof(byte));
            table.FieldInfo = new byte[table.RecordCount];
            for (int i = 0; i < table.RecordCount; i++) {
                table.FieldInfo[i] = (byte)temp[i];
            }
        }

        private void ReadIndexer(DataReader reader, ArmpTable table, int offset)
        {
            if (offset > 0) {
                table.Indexer = ReadTable(reader, offset);
            }
        }
    }
}
