using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Edubase.Services.Enums;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models.Guidance;
using Xunit;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class LaNameCodeFileGeneratorTests
    {
        private const string NameHeader = "English local authority (LA) name";

        private static readonly LaNameCodes[] Rows =
        {
            new LaNameCodes { LaName = "West Place",    LaCode = "123", OnsLaCode = "E06000037" },
            new LaNameCodes { LaName = "City of England", LaCode = "456", OnsLaCode = "E06000016" },
        };

        [Fact]
        public void Generate_Csv_ProducesHeaderAndAllRows_PreservingCodesAsText()
        {
            var generator = new LaNameCodeFileGenerator();

            string content;
            using (var stream = generator.Generate(Rows, eFileFormat.CSV, NameHeader))
            using (var reader = new StreamReader(stream))
                content = reader.ReadToEnd();

            var lines = content.Trim().Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

            Assert.Equal($"{NameHeader},{LaNameCodeFileGenerator.GiasColumnHeader},{LaNameCodeFileGenerator.OnsColumnHeader}", lines[0]);
            Assert.Equal("West Place,123,E06000037", lines[1]);
            Assert.Equal("City of England,456,E06000016", lines[2]);
        }

        [Fact]
        public void Generate_Csv_QuotesFieldsContainingCommas()
        {
            var generator = new LaNameCodeFileGenerator();
            var rows = new[] { new LaNameCodes { LaName = "The Island, Republic of", LaCode = "999", OnsLaCode = "E01000073" } };

            string content;
            using (var stream = generator.Generate(rows, eFileFormat.CSV, "name"))
            using (var reader = new StreamReader(stream))
                content = reader.ReadToEnd();

            Assert.Contains("\"The Island, Republic of\"", content);
        }

        [Fact]
        public void Generate_Xlsx_ProducesReadableWorkbook_RoundTripVerifiable()
        {
            var generator = new LaNameCodeFileGenerator();

            using (var stream = generator.Generate(Rows, eFileFormat.XLSX, NameHeader))
            using (var document = SpreadsheetDocument.Open(stream, false))
            {
                var sheetData = document.WorkbookPart.WorksheetParts.First()
                                        .Worksheet.Elements<SheetData>().First();
                var rows = sheetData.Elements<Row>().ToList();

                Assert.Equal(3, rows.Count);
                Assert.Equal(NameHeader, CellText(rows[0], 0));
                Assert.Equal("West Place", CellText(rows[1], 0));
                Assert.Equal("123", CellText(rows[1], 1));
                Assert.Equal("E06000037", CellText(rows[1], 2));
                Assert.Equal("City of England", CellText(rows[2], 0));
                Assert.All(rows.Skip(1).SelectMany(r => r.Elements<Cell>()),
                           c => Assert.Equal(CellValues.InlineString, c.DataType.Value));
            }
        }

        private static string CellText(Row row, int cellIndex)
        {
            return row.Elements<Cell>().ElementAt(cellIndex).InlineString.Text.Text;
        }
    }
}
