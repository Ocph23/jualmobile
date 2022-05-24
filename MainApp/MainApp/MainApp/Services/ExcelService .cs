using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MainApp.Models;
using System;
using System.IO;
using System.Linq;

namespace MainApp.Services
{
    public class ExcelService
    {
        public  string GenerateExcel(String fileName)
        {

            string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            // Creating the SpreadsheetDocument in the indicated FilePath
            var filePath = Path.Combine(AppFolder, fileName);
            var document = SpreadsheetDocument.Create(Path.Combine(AppFolder, fileName), SpreadsheetDocumentType.Workbook);

            var wbPart = document.AddWorkbookPart();
            wbPart.Workbook = new Workbook();

            var part = wbPart.AddNewPart<WorksheetPart>();
            part.Worksheet = new Worksheet(new SheetData());

            //  Here are created the sheets, you can add all the child sheets that you need.
            var sheets = wbPart.Workbook.AppendChild
                (
                   new Sheets(
                            new Sheet()
                            {
                                Id = wbPart.GetIdOfPart(part),
                                SheetId = 1,
                                Name = "Laporan"
                            }
                        )
                );

            // Just save and close you Excel file
            wbPart.Workbook.Save();
            document.Close();
            // Dont't forget return the filePath
            return filePath;
        }

        private Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        public  void InsertDataPenjualanIntoSheet(string fileName, string sheetName, ExcelStructure data)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            using (var document = SpreadsheetDocument.Open(fileName, true))
            {
                var wbPart = document.WorkbookPart;
                WorkbookStylesPart stylePart = wbPart.AddNewPart<WorkbookStylesPart>();
                stylePart.Stylesheet = GenerateStylesheet();
                stylePart.Stylesheet.Save();

                var sheets = wbPart.Workbook.GetFirstChild<Sheets>().
                             Elements<Sheet>().FirstOrDefault().
                             Name = sheetName;

                var part = wbPart.WorksheetParts.First();
                var sheetData = part.Worksheet.Elements<SheetData>().First();

                var col1 = sheetData.AppendChild(new Column() { Width=100});
                col1.Width = 34;
                var rowTitle = sheetData.AppendChild(new Row() { Height=100});
                rowTitle.Append(new Cell()
                {
                    CellValue = new CellValue($"LAPORAN PENJUALAN {data.From.ToString("dd/MM/yyyy")} - {data.To.ToString("dd/MM/yyyy")}"),
                    DataType = new EnumValue<CellValues>(CellValues.String)
                });

                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());

                var row = sheetData.AppendChild(new Row());

              

                foreach (var header in data.Headers)
                {
                    var cell = new Cell()
                    {
                        CellValue = new CellValue(header.Title),
                        StyleIndex = 1,
                        DataType = new EnumValue<CellValues>(CellValues.String)
                    };
                    row.Append(cell);
                   

                }

                Columns lstColumns = sheetData.GetFirstChild<Columns>();

                if(lstColumns == null)
                {
                    lstColumns = new Columns();
                    lstColumns.AppendChild(new Column { Width=500 });
                }


                foreach (var value in data.Values)
                {
                    var dataRow = sheetData.AppendChild(new Row());

                    dataRow.Append(new Cell()
                    {
                        CellValue = new CellValue(value.Tanggal.ToShortDateString()),
                        StyleIndex = 1,
                        DataType = new EnumValue<CellValues>(GetCellType(value.Tanggal.ToShortDateString()))
                    });

                    dataRow.Append(new Cell()
                    {
                        CellValue = new CellValue(value.Id.ToString("D6")),   
                        StyleIndex = 1,
                        
                        DataType = new EnumValue<CellValues>(GetCellType(value.Pelanggan))
                    });

                    foreach (var barang in value.Items)
                    {
                        dataRow.Append(new Cell()
                        {
                            CellValue = new CellValue(barang.Barang.Nama),
                            StyleIndex = 1,
                            DataType = new EnumValue<CellValues>(GetCellType(barang.Barang.Nama))
                        });
                        dataRow.Append(new Cell()
                        {
                            CellValue = new CellValue(barang.Jumlah),
                            StyleIndex = 1,
                            DataType = new EnumValue<CellValues>(GetCellType(barang.Jumlah))
                        });
                        dataRow.Append(new Cell()
                        {
                            CellValue = new CellValue(barang.Satuan.Nama),
                            StyleIndex = 1,
                            DataType = new EnumValue<CellValues>(GetCellType(barang.Satuan.Nama))
                        });

                        dataRow.Append(new Cell()
                        {
                            CellValue = new CellValue(barang.Satuan.HargaJual),
                            StyleIndex = 1,
                            DataType = new EnumValue<CellValues>(GetCellType(barang.Satuan.HargaJual))
                        });

                        dataRow.Append(new Cell()
                        {
                            CellValue = new CellValue(barang.Satuan.HargaJual * barang.Jumlah),
                            StyleIndex = 1,
                            DataType = new EnumValue<CellValues>(GetCellType(barang.Satuan.HargaJual))
                        });
                    }
                }
                wbPart.Workbook.Save();
            }
        }
        public CellFormat GetCellFormat(WorkbookPart workbookPart, uint styleIndex)
        {
            return workbookPart.WorkbookStylesPart.Stylesheet.Elements<CellFormats>().First().Elements<CellFormat>().ElementAt((int)styleIndex);
        }
        private CellValues GetCellType(object total)
        {
            switch (total.GetType().Name)
            {
                case "String":
                    return CellValues.String;   

                case "DateTime":
                    return CellValues.String;

                case "Double":
                    return CellValues.Number;

                case "Int":
                    return CellValues.Number;
                default:
                    return CellValues.String;
            }
        }

        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 10 }

                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 10 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }) // Index 2 - header
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true } // header
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
    }
}
