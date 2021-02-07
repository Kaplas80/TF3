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
    using OfficeOpenXml;
    using TF3.Plugin.YakuzaKiwami2.Enums;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converts Yakuza Kiwami 2 ARMP files to XLSX.
    /// </summary>
    public class XlsxWriter : IConverter<ArmpTable, BinaryFormat>
    {
        /// <summary>
        /// Converts a Armp into a Excel binary.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The xlsx format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public virtual BinaryFormat Convert(ArmpTable source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            TableToSheet(source, "Main", package);

            byte[] data = package.GetAsByteArray();

            DataStream stream = DataStreamFactory.FromArray(data, 0, data.Length);
            return new BinaryFormat(stream);
        }

        private void TableToSheet(ArmpTable table, string name, ExcelPackage package)
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(name);

            if (table.Indexer != null) {
                TableToSheet(table.Indexer, $"{name}_Idx", package);
            }

            sheet.Cells["A1"].Value = "TABLE INFO";
            sheet.Cells["A1:B1"].Merge = true;
            sheet.Cells["A2"].Value = "ID";
            sheet.Cells["B2"].Value = table.Id;
            sheet.Cells["A3"].Value = "FLAGS";
            sheet.Cells["B3"].Value = table.Flags;
            sheet.Cells["A4"].Value = "FIELD_INVALID";
            sheet.Cells["B4"].Value = table.AreFieldsInvalid;
            sheet.Cells["A5"].Value = "RECORD_INVALID";
            sheet.Cells["B5"].Value = table.AreRecordsInvalid;
            sheet.Cells["A6"].Value = "INDEXER";
            sheet.Cells["B6"].Value = table.Indexer != null;

            sheet.Cells["A8"].Value = "VALUE STRINGS";
            sheet.Cells["A8:B8"].Merge = true;
            sheet.Cells["A9"].Value = "Index";
            sheet.Cells["B9"].Value = "String";

            for (int i = 0; i < table.ValueStringCount; i++) {
                sheet.Cells[10 + i, 1].Value = i;
                sheet.Cells[10 + i, 2].Value = table.GetValueString(i);
            }

            sheet.Cells["G1"].Value = "Order";
            sheet.Cells["G2"].Value = "Type";
            sheet.Cells["G3"].Value = "Raw Type";
            sheet.Cells["G4"].Value = "Existence";
            sheet.Cells["G5"].Value = "ID";

            for (int fieldIndex = 0; fieldIndex < table.FieldCount; fieldIndex++) {
                sheet.Cells[1, 8 + fieldIndex].Value = table.GetFieldOrder(fieldIndex);
                sheet.Cells[2, 8 + fieldIndex].Value = table.GetFieldType(fieldIndex);
                sheet.Cells[3, 8 + fieldIndex].Value = table.GetRawRecordMemberInfo(fieldIndex);
                sheet.Cells[4, 8 + fieldIndex].Value = table.GetFieldExistence(fieldIndex);

                string id = table.GetFieldId(fieldIndex);
                if (string.IsNullOrEmpty(id)) {
                    id = $"Field {fieldIndex}";
                }

                sheet.Cells[5, 8 + fieldIndex].Value = id;
            }

            sheet.Cells["D5"].Value = "Order";
            sheet.Cells["E5"].Value = "FieldInfo";
            sheet.Cells["F5"].Value = "Existence";

            for (int recordIndex = 0; recordIndex < table.RecordCount; recordIndex++) {
                sheet.Cells[6 + recordIndex, 4].Value = table.GetRecordOrder(recordIndex);
                sheet.Cells[6 + recordIndex, 5].Value = table.GetFieldInfo(recordIndex);
                sheet.Cells[6 + recordIndex, 6].Value = table.GetRecordExistence(recordIndex);

                string recordId = table.GetRecordId(recordIndex);
                if (string.IsNullOrEmpty(recordId)) {
                    recordId = $"Record {recordIndex}";
                }

                sheet.Cells[6 + recordIndex, 7].Value = recordId;

                for (int fieldIndex = 0; fieldIndex < table.FieldCount; fieldIndex++) {
                    if (table.GetRawRecordMemberInfo(fieldIndex) != FieldType.Table) {
                        object obj = table.GetValue(recordIndex, fieldIndex);
                        if (obj == null) {
                            continue;
                        }

                        string value = obj.ToString();
                        if (table.GetIsNullValue(recordIndex, fieldIndex)) {
                            value += "(NULL)";
                        }

                        sheet.Cells[6 + recordIndex, 8 + fieldIndex].Value = value;
                    }
                    else {
                        object subTable = table.GetValue(recordIndex, fieldIndex);
                        if (subTable == null) {
                            continue;
                        }

                        int sheetIndex = package.Workbook.Worksheets.Count + 1;
                        sheet.Cells[6 + recordIndex, 8 + fieldIndex].Value = $"Sheet {sheetIndex}";
                        TableToSheet((ArmpTable)subTable, $"Sheet {sheetIndex}", package);
                    }
                }
            }
        }
    }
}
