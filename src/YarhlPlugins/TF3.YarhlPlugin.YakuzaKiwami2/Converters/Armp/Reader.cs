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
    using System.Text;
    using TF3.YarhlPlugin.YakuzaKiwami2.Enums;
    using TF3.YarhlPlugin.YakuzaKiwami2.Formats;
    using TF3.YarhlPlugin.YakuzaKiwami2.Types;
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
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.UTF8,
                Endianness = EndiannessMode.LittleEndian,
            };

            // Read the file header
            var header = reader.Read<FileHeader>() as FileHeader;
            CheckHeader(header);

            if (header.Endianness == Endianness.BigEndian)
            {
                reader.Endianness = EndiannessMode.BigEndian;
            }

            int mainTableOffset = reader.ReadInt32();

            return ReadTable(reader, mainTableOffset);
        }

        private static void CheckHeader(FileHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (header.Magic != "armp")
            {
                throw new FormatException($"armp: Bad magic Id ({header.Magic} != armp)");
            }
        }

        private static object[] ReadNumbers(DataReader reader, int count, int offset, Type type)
        {
            object[] result = new object[count];

            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < count; i++)
            {
                result[i] = (object)reader.ReadByType(type);
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);

            return result;
        }

        private static bool[] ReadBits(DataReader reader, int count, int offset)
        {
            bool[] result = new bool[count];

            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);
            byte current = reader.ReadByte();
            int bitsLeft = 8;
            for (int i = 0; i < count; i++)
            {
                bool isTrue = (current & 0x01) == 0x01;
                current >>= 0x01;
                bitsLeft--;
                if (bitsLeft == 0)
                {
                    current = reader.ReadByte();
                    bitsLeft = 8;
                }

                result[i] = isTrue;
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);

            return result;
        }

        private static string[] ReadStrings(DataReader reader, int count, int offset)
        {
            string[] result = new string[count];

            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < count; i++)
            {
                int idOffset = reader.ReadInt32();
                long returnPosition2 = reader.Stream.Position;
                _ = reader.Stream.Seek(idOffset, System.IO.SeekOrigin.Begin);
                string value = reader.ReadString();
                _ = reader.Stream.Seek(returnPosition2, System.IO.SeekOrigin.Begin);

                result[i] = value;
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);

            return result;
        }

        private static FieldType[] ReadTypes(DataReader reader, int count, int offset)
        {
            var result = new FieldType[count];

            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < count; i++)
            {
                result[i] = (FieldType)Enum.ToObject(typeof(FieldType), reader.ReadByte());
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);

            return result;
        }

        private ArmpTable ReadTable(DataReader reader, long fileOffset)
        {
            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(fileOffset, System.IO.SeekOrigin.Begin);

            var header = reader.Read<ArmpTableHeader>() as ArmpTableHeader;
            var table = new ArmpTable(header);

            ReadIndexer(reader, table, header.IndexerPointer);
            ReadRecordExistence(reader, table, header.RecordExistencePointer);
            ReadFieldExistence(reader, table, header.FieldExistencePointer);
            ReadRecordIds(reader, table, header.RecordIdPointer);
            ReadFieldIds(reader, table, header.FieldIdPointer);
            ReadValueStrings(reader, table, header.ValueStringPointer);
            ReadFieldTypes(reader, table, header.FieldTypePointer);
            ReadRecordMemberInfo(reader, table, header.RawRecordMemberInfoPointer);
            ReadValues(reader, table, header.ValuesPointer);
            ReadEmptyValues(reader, table, header.EmptyValuesPointer);
            ReadRecordOrder(reader, table, header.RecordOrderPointer);
            ReadFieldOrder(reader, table, header.FieldOrderPointer);
            ReadFieldInfo(reader, table, header.FieldInfoPointer);
            ReadGameVarFieldType(reader, table, header.GameVarFieldTypePointer);

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);
            return table;
        }

        private void ReadIndexer(DataReader reader, ArmpTable table, int offset)
        {
            if (offset > 0)
            {
                table.Indexer = ReadTable(reader, offset);
            }
        }

        private void ReadRecordExistence(DataReader reader, ArmpTable table, int offset)
        {
            table.RecordExistence = offset switch
            {
                -1 => null,
                0 => System.Array.Empty<bool>(),
                _ => ReadBits(reader, table.RecordCount, offset)
            };
        }

        private void ReadFieldExistence(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldExistence = offset switch
            {
                -1 => null,
                0 => System.Array.Empty<bool>(),
                _ => ReadBits(reader, table.FieldCount, offset)
            };
        }

        private void ReadRecordIds(DataReader reader, ArmpTable table, int offset)
        {
            table.RecordIds = offset switch
            {
                0 => System.Array.Empty<string>(),
                _ => ReadStrings(reader, table.RecordCount, offset)
            };
        }

        private void ReadFieldIds(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldIds = offset switch
            {
                0 => System.Array.Empty<string>(),
                _ => ReadStrings(reader, table.FieldCount, offset)
            };
        }

        private void ReadValueStrings(DataReader reader, ArmpTable table, int offset)
        {
            table.ValueStrings = ReadStrings(reader, table.ValueStringCount, offset);
        }

        private void ReadFieldTypes(DataReader reader, ArmpTable table, int offset)
        {
            table.FieldTypes = offset switch
            {
                0 => System.Array.Empty<FieldType>(),
                _ => ReadTypes(reader, table.FieldCount, offset)
            };
        }

        private void ReadRecordMemberInfo(DataReader reader, ArmpTable table, int offset)
        {
            table.RawRecordMemberInfo = offset switch
            {
                0 => System.Array.Empty<FieldType>(),
                _ => ReadTypes(reader, table.FieldCount, offset)
            };
        }

        private void ReadValues(DataReader reader, ArmpTable table, int offset)
        {
            table.Values = new object[table.FieldCount][];
            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < table.FieldCount; i++)
            {
                FieldType fieldType = table.RawRecordMemberInfo[i];
                int dataOffset = reader.ReadInt32();
                if (dataOffset == -1 || fieldType == FieldType.Unused)
                {
                    continue;
                }

                switch (fieldType)
                {
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
                        table.Values[i] = Array.ConvertAll(temp, x => (object)x);
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

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);
        }

        private object[] ReadSubTables(DataReader reader, int count, int offset)
        {
            object[] result = new object[count];
            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < count; i++)
            {
                ArmpTable subTable = null;
                long subTablePointer = reader.ReadInt64();
                long returnPosition2 = reader.Stream.Position;
                if (subTablePointer > 0)
                {
                    subTable = ReadTable(reader, subTablePointer);
                }

                _ = reader.Stream.Seek(returnPosition2, System.IO.SeekOrigin.Begin);

                result[i] = subTable;
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);

            return result;
        }

        private void ReadEmptyValues(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == 0)
            {
                table.EmptyValues = Array.Empty<bool[]>();
                return;
            }

            table.EmptyValues = new bool[table.FieldCount][];
            long returnPosition = reader.Stream.Position;
            _ = reader.Stream.Seek(offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < table.FieldCount; i++)
            {
                int dataOffset = reader.ReadInt32();
                if (dataOffset <= 0)
                {
                    table.EmptyValues[i] = null;
                    continue;
                }

                table.EmptyValues[i] = ReadBits(reader, table.RecordCount, dataOffset);
            }

            _ = reader.Stream.Seek(returnPosition, System.IO.SeekOrigin.Begin);
        }

        private void ReadRecordOrder(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == 0)
            {
                table.RecordOrder = Array.Empty<int>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.RecordCount, offset, typeof(int));
            table.RecordOrder = Array.ConvertAll(temp, x => (int)x);
        }

        private void ReadFieldOrder(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == 0)
            {
                table.FieldOrder = Array.Empty<int>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.FieldCount, offset, typeof(int));
            table.FieldOrder = Array.ConvertAll(temp, x => (int)x);
        }

        private void ReadFieldInfo(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == 0)
            {
                table.FieldInfo = Array.Empty<byte>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.RecordCount, offset, typeof(byte));
            table.FieldInfo = Array.ConvertAll(temp, x => (byte)x);
        }

        private void ReadGameVarFieldType(DataReader reader, ArmpTable table, int offset)
        {
            if (offset == 0)
            {
                table.GameVarFieldType = Array.Empty<int>();
                return;
            }

            object[] temp = ReadNumbers(reader, table.FieldCount, offset, typeof(int));
            table.GameVarFieldType = Array.ConvertAll(temp, x => (int)x);
        }
    }
}
