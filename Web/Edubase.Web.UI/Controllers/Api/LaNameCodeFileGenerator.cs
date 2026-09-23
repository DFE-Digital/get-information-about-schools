using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Edubase.Services.Enums;
using Edubase.Web.UI.Models.Guidance;

namespace Edubase.Web.UI.Controllers.Api
{
    public class LaNameCodeFileGenerator : ILaNameCodeFileGenerator
    {
        public const string GiasColumnHeader =
            "Get Information about Schools (GIAS) local authority (LA) code";

        public const string OnsColumnHeader =
            "Office for National Statistics (ONS) local authority (LA) code (also known as Government Statistical Service (GSS) local authority (LA) code on GIAS)";

        private static readonly char[] CsvSpecialChars = { ',', '"', '\r', '\n' };

        public MemoryStream Generate(IEnumerable<LaNameCodes> rows, eFileFormat fileFormat, string nameColumnHeader)
        {
            switch (fileFormat)
            {
                case eFileFormat.CSV:  return GenerateCsv(rows, nameColumnHeader);
                case eFileFormat.XLSX: return GenerateXlsx(rows, nameColumnHeader);
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, "Unsupported file format");
            }
        }

        private static MemoryStream GenerateCsv(IEnumerable<LaNameCodes> rows, string nameColumnHeader)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen: true))
            {
                writer.WriteLine(ToCsvLine(nameColumnHeader, GiasColumnHeader, OnsColumnHeader));
                foreach (var row in rows)
                {
                    writer.WriteLine(ToCsvLine(row.LaName, row.LaCode, row.OnsLaCode));
                }
            }

            stream.Position = 0;
            return stream;
        }

        private static string ToCsvLine(params string[] fields)
        {
            return string.Join(",", fields.Select(EscapeCsvField));
        }

        private static string EscapeCsvField(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.IndexOfAny(CsvSpecialChars) == -1) return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static MemoryStream GenerateXlsx(IEnumerable<LaNameCodes> rows, string nameColumnHeader)
        {
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                var sheetData = new SheetData();
                sheetData.AppendChild(CreateTextRow(1, nameColumnHeader, GiasColumnHeader, OnsColumnHeader));

                uint rowIndex = 2;
                foreach (var row in rows)
                {
                    sheetData.AppendChild(CreateTextRow(rowIndex++, row.LaName, row.LaCode, row.OnsLaCode));
                }

                worksheetPart.Worksheet = new Worksheet(
                    new Columns(
                        new Column { Min = 1, Max = 1, Width = 45, CustomWidth = true },
                        new Column { Min = 2, Max = 3, Width = 80, CustomWidth = true }),
                    sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet
                {
                    Id      = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name    = "LA name codes"
                });

                workbookPart.Workbook.Save();
            }

            stream.Position = 0;
            return stream;
        }

        private static Row CreateTextRow(uint rowIndex, params string[] values)
        {
            var row = new Row { RowIndex = rowIndex };
            foreach (var value in values)
            {
                var inlineString = new InlineString();
                inlineString.AppendChild(new Text(value ?? string.Empty));

                var cell = new Cell { DataType = CellValues.InlineString };
                cell.AppendChild(inlineString);

                row.AppendChild(cell);
            }
            return row;
        }
    }
}
