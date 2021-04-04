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
    using System.Drawing;
    using OfficeOpenXml;
    using TF3.YarhlPlugin.YakuzaKiwami2.Enums;
    using TF3.YarhlPlugin.YakuzaKiwami2.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converts Yakuza Kiwami 2 ARMP files to XLSX.
    /// </summary>
    public class ToXlsx : IConverter<ArmpTable, BinaryFormat>
    {
        /// <summary>
        /// Converts a Armp into a Excel binary.
        /// </summary>
        /// <param name="source">Input format.</param>
        /// <returns>The xlsx format.</returns>
        /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
        public BinaryFormat Convert(ArmpTable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            OfficeOpenXml.Style.XmlAccess.ExcelNamedStyleXml namedStyle = package.Workbook.Styles.CreateNamedStyle("HyperLink");
            namedStyle.Style.Font.UnderLine = true;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);

            TableToSheet(source, "Main", package);

            byte[] data = package.GetAsByteArray();

            DataStream stream = DataStreamFactory.FromArray(data, 0, data.Length);
            return new BinaryFormat(stream);
        }

        private void TableToSheet(ArmpTable table, string name, ExcelPackage package)
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(name);

            if (table.Indexer != null)
            {
                TableToSheet(table.Indexer, $"{name}_Idx", package);
            }

            sheet.Cells["A2"].Value = "TABLE INFO";
            sheet.Cells["A2:B2"].Merge = true;
            sheet.Cells["A3"].Value = "ID";
            sheet.Cells["B3"].Value = table.Id;
            sheet.Cells["A4"].Value = "FLAGS";
            sheet.Cells["B4"].Value = table.Flags;
            sheet.Cells["A5"].Value = "FIELD_INVALID";
            sheet.Cells["B5"].Value = table.FieldInvalid;
            sheet.Cells["A6"].Value = "RECORD_INVALID";
            sheet.Cells["B6"].Value = table.RecordInvalid;
            sheet.Cells["A7"].Value = "INDEXER";
            sheet.Cells["B7"].Value = table.Indexer != null;

            sheet.Cells["A9"].Value = "VALUE STRINGS";
            sheet.Cells["A9:B9"].Merge = true;
            sheet.Cells["A10"].Value = "Index";
            sheet.Cells["B10"].Value = "String";

            for (int i = 0; i < table.ValueStringCount; i++)
            {
                sheet.Cells[11 + i, 1].Value = i;
                sheet.Cells[11 + i, 2].Value = table.ValueStrings[i];
            }

            sheet.Cells["D6"].Value = "Order";
            sheet.Cells["E6"].Value = "FieldInfo";
            sheet.Cells["F6"].Value = "Existence";

            for (int recordIndex = 0; recordIndex < table.RecordCount; recordIndex++)
            {
                sheet.Cells[7 + recordIndex, 4].Value = table.RecordOrder?.Length > 0 ? table.RecordOrder[recordIndex] : -1;
                sheet.Cells[7 + recordIndex, 5].Value = table.FieldInfo?.Length > 0 ? table.FieldInfo[recordIndex] : -1;
                sheet.Cells[7 + recordIndex, 6].Value = table.RecordExistence?.Length > 0 && table.RecordExistence[recordIndex];
                sheet.Cells[7 + recordIndex, 7].Value = table.RecordIds?.Length > 0 ? table.RecordIds[recordIndex] : $"Record {recordIndex}";
            }

            sheet.Cells["G1"].Value = "Order";
            sheet.Cells["G2"].Value = "Type";
            sheet.Cells["G3"].Value = "Raw Type";
            sheet.Cells["G4"].Value = "Game Var";
            sheet.Cells["G5"].Value = "Existence";
            sheet.Cells["G6"].Value = "ID";

            for (int fieldIndex = 0; fieldIndex < table.FieldCount; fieldIndex++)
            {
                sheet.Cells[1, 8 + fieldIndex].Value = table.FieldOrder?.Length > 0 ? table.FieldOrder[fieldIndex] : -1;
                sheet.Cells[2, 8 + fieldIndex].Value = table.FieldTypes?.Length > 0 ? table.FieldTypes[fieldIndex] : FieldType.Unused;
                sheet.Cells[3, 8 + fieldIndex].Value = table.RawRecordMemberInfo?.Length > 0 ? table.RawRecordMemberInfo[fieldIndex] : FieldType.Unused;
                sheet.Cells[4, 8 + fieldIndex].Value = table.GameVarFieldType?.Length > 0 ? table.GameVarFieldType[fieldIndex] : string.Empty;
                sheet.Cells[5, 8 + fieldIndex].Value = table.FieldExistence?.Length > 0 && table.FieldExistence[fieldIndex];
                sheet.Cells[6, 8 + fieldIndex].Value = table.FieldIds?.Length > 0 ? table.FieldIds[fieldIndex] : $"Field {fieldIndex}";

                object[] data = table.Values[fieldIndex];
                if (data == null)
                {
                    continue;
                }

                if (table.RawRecordMemberInfo?.Length > 0)
                {
                    FieldType memberInfo = table.RawRecordMemberInfo[fieldIndex];
                    for (int recordIndex = 0; recordIndex < table.RecordCount; recordIndex++)
                    {
                        object obj = data[recordIndex];
                        if (obj == null)
                        {
                            continue;
                        }

                        if (memberInfo != FieldType.Table)
                        {
                            string value = obj.ToString();
                            if (table.EmptyValues?.Length > 0 && table.EmptyValues[fieldIndex]?[recordIndex] == true)
                            {
                                value += "(NULL)";
                            }

                            sheet.Cells[7 + recordIndex, 8 + fieldIndex].Value = value;
                        }
                        else
                        {
                            int sheetIndex = package.Workbook.Worksheets.Count + 1;

                            string value = $"Sheet {sheetIndex}";
                            if (table.EmptyValues?.Length > 0 && table.EmptyValues[fieldIndex]?[recordIndex] == true)
                            {
                                value += "(NULL)";
                            }

                            TableToSheet((ArmpTable)obj, $"Sheet {sheetIndex}", package);
                            sheet.Cells[7 + recordIndex, 8 + fieldIndex].Value = value;
                            sheet.Cells[7 + recordIndex, 8 + fieldIndex].Hyperlink = new Uri($"#'Sheet {sheetIndex}'!A1", UriKind.Relative);
                            sheet.Cells[7 + recordIndex, 8 + fieldIndex].StyleName = "Hyperlink";
                            package.Workbook.Worksheets[$"Sheet {sheetIndex}"].Cells["A1"].Value = "Return";
                            package.Workbook.Worksheets[$"Sheet {sheetIndex}"].Cells["A1"].Hyperlink = new Uri($"#'{sheet.Name}'!{sheet.Cells[6 + recordIndex, 8 + fieldIndex].Address}", UriKind.Relative);
                            package.Workbook.Worksheets[$"Sheet {sheetIndex}"].Cells["A1"].StyleName = "Hyperlink";
                        }
                    }
                }
            }
        }
    }
}
