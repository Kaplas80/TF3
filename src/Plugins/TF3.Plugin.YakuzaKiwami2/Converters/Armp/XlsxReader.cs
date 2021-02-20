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
    using System.Linq;
    using OfficeOpenXml;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converts a XLSX into Yakuza Kiwami 2 ARMP file.
    /// </summary>
    public class XlsxReader : IConverter<BinaryFormat, ArmpTable>
    {
        /// <summary>
        /// Converts a Armp into a Excel binary.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The xlsx format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public ArmpTable Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(source.Stream);

            return SheetToTable(package, package.Workbook.Worksheets["Main"]);
        }

        private ArmpTable SheetToTable(ExcelPackage package, ExcelWorksheet sheet)
        {
            int id = int.Parse(sheet.Cells["B3"].Value.ToString());
            byte flags = byte.Parse(sheet.Cells["B4"].Value.ToString());
            bool fieldInvalid = (bool)sheet.Cells["B5"].Value;
            bool recordInvalid = (bool)sheet.Cells["B6"].Value;

            int recordCount = 0;
            int row = 7;
            while (sheet.Cells[row, 4].Value != null)
            {
                recordCount++;
                row++;
            }

            int fieldCount = 0;
            int col = 8;
            while (sheet.Cells[1, col].Value != null)
            {
                fieldCount++;
                col++;
            }

            int valueStringCount = 0;
            row = 11;
            while (sheet.Cells[row, 1].Value != null)
            {
                valueStringCount++;
                row++;
            }

            var result = new ArmpTable(id, flags, recordCount, recordInvalid, fieldCount, fieldInvalid, valueStringCount);

            ExcelWorksheet indexerSheet = package.Workbook.Worksheets[$"{sheet.Name}_Idx"];
            if (indexerSheet != null)
            {
                result.Indexer = SheetToTable(package, indexerSheet);
            }

            result.ValueStrings = new string[valueStringCount];
            for (row = 11; row < 11 + valueStringCount; row++)
            {
                result.ValueStrings[row - 11] = sheet.Cells[row, 2].Value.ToString();
            }

            int[] recordOrder = new int[recordCount];
            byte[] fieldInfo = new byte[recordCount];
            bool[] recordExistence = new bool[recordCount];
            string[] recordIds = new string[recordCount];
            for (row = 7; row < 7 + recordCount; row++)
            {
                recordOrder[row - 7] = int.Parse(sheet.Cells[row, 4].Value.ToString());
                fieldInfo[row - 7] = byte.Parse(sheet.Cells[row, 5].Value.ToString());
                recordExistence[row - 7] = (bool)sheet.Cells[row, 6].Value;
                recordIds[row - 7] = sheet.Cells[row, 7].Value.ToString() == $"Record {row - 7}" ? string.Empty : sheet.Cells[row, 7].Value.ToString();
            }

            if (recordOrder.Any(x => x != -1))
            {
                result.RecordOrder = recordOrder;
            }

            if (fieldInfo.Any(x => x != 0xFF))
            {
                result.FieldInfo = fieldInfo;
            }

            result.RecordExistence = recordExistence;
            result.RecordIds = recordIds;

            int[] fieldOrder = new int[fieldCount];
            var fieldTypes = new FieldType[fieldCount];
            var rawRecordMemberInfo = new FieldType[fieldCount];
            int[] gameVarFieldType = new int[fieldCount];
            bool[] fieldExistence = new bool[fieldCount];
            string[] fieldIds = new string[fieldCount];
            object[][] values = new object[fieldCount][];
            bool[][] emptyValues = new bool[fieldCount][];
            bool hasEmptyValues = false;

            for (col = 8; col < 8 + fieldCount; col++)
            {
                fieldOrder[col - 8] = int.Parse(sheet.Cells[1, col].Value.ToString());
                if (!Enum.TryParse(sheet.Cells[2, col].Value.ToString(), out fieldTypes[col - 8]))
                {
                    fieldTypes[col - 8] = FieldType.Unused;
                }

                if (!Enum.TryParse(sheet.Cells[3, col].Value.ToString(), out rawRecordMemberInfo[col - 8]))
                {
                    rawRecordMemberInfo[col - 8] = FieldType.Unused;
                }

                gameVarFieldType[col - 8] = !string.IsNullOrEmpty(sheet.Cells[4, col].Value.ToString()) ? int.Parse(sheet.Cells[3, col].Value.ToString()) : -1;
                fieldExistence[col - 8] = (bool)sheet.Cells[5, col].Value;
                fieldIds[col - 8] = sheet.Cells[6, col].Value.ToString() == $"Field {col - 8}" ? string.Empty : sheet.Cells[6, col].Value.ToString();

                if (rawRecordMemberInfo[col - 8] != FieldType.Unused)
                {
                    values[col - 8] = new object[recordCount];
                    emptyValues[col - 8] = new bool[recordCount];

                    for (row = 7; row < 7 + recordCount; row++)
                    {
                        string cellValue = sheet.Cells[row, col].Value.ToString();

                        if (cellValue.EndsWith("(NULL)"))
                        {
                            emptyValues[col - 8][row - 8] = true;
                            cellValue = cellValue.Replace("(NULL)", string.Empty);
                            hasEmptyValues = true;
                        }

                        switch (rawRecordMemberInfo[col - 8])
                        {
                            case FieldType.UInt8:
                                values[col - 8][row - 7] = byte.Parse(cellValue);
                                break;
                            case FieldType.UInt16:
                                values[col - 8][row - 7] = ushort.Parse(cellValue);
                                break;
                            case FieldType.UInt32:
                                values[col - 8][row - 7] = uint.Parse(cellValue);
                                break;
                            case FieldType.UInt64:
                                values[col - 8][row - 7] = ulong.Parse(cellValue);
                                break;
                            case FieldType.Int8:
                                values[col - 8][row - 7] = sbyte.Parse(cellValue);
                                break;
                            case FieldType.Int16:
                                values[col - 8][row - 7] = short.Parse(cellValue);
                                break;
                            case FieldType.Int32:
                                values[col - 8][row - 7] = int.Parse(cellValue);
                                break;
                            case FieldType.Int64:
                                values[col - 8][row - 7] = long.Parse(cellValue);
                                break;
                            case FieldType.Float16:
                                break;
                            case FieldType.Float32:
                                values[col - 8][row - 7] = float.Parse(cellValue);
                                break;
                            case FieldType.Float64:
                                values[col - 8][row - 7] = double.Parse(cellValue);
                                break;
                            case FieldType.Boolean:
                                values[col - 8][row - 7] = cellValue == "True";
                                break;
                            case FieldType.String:
                                values[col - 8][row - 7] = int.Parse(cellValue);
                                break;
                            case FieldType.Table:
                                values[col - 8][row - 7] = SheetToTable(package, package.Workbook.Worksheets[cellValue]);
                                break;
                            case FieldType.Unused:
                                break;
                        }
                    }
                }
            }

            result.EmptyValues = hasEmptyValues ? emptyValues : Array.Empty<bool[]>();

            if (fieldOrder.Any(x => x != -1))
            {
                result.FieldOrder = fieldOrder;
            }

            result.FieldTypes = fieldTypes;
            result.RawRecordMemberInfo = rawRecordMemberInfo;

            result.GameVarFieldType = gameVarFieldType.Any(x => x != -1) ? gameVarFieldType : Array.Empty<int>();

            result.FieldExistence = fieldExistence;
            result.FieldIds = fieldIds;

            result.Values = values;

            return result;
        }
    }
}
