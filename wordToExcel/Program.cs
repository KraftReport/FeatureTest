using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

class Program
{
    static void Main()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var wordPath = "CARGO MANIFEST FILE FOR METHI BHUM V.445 N_DOC.docx";
        var excelPath = "output.xlsx";

        var contents = new List<string>();
        var blNumbers = new List<string>(); // Store BL numbers separately

        using (var wordDoc = WordprocessingDocument.Open(wordPath, false))
        {
            var body = wordDoc.MainDocumentPart.Document.Body;

            if (body != null)
            {
                ExtractTextFromElement(body, contents, blNumbers);
            }
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sheet1");

        // Write all contents from Word to Excel
        for (int i = 0; i < contents.Count; i++)
        {
            ws.Cells[i + 1, 1].Value = contents[i]; // Write each line to a new row
        }

        // Add BL Numbers at the end of the sheet
        int lastRow = contents.Count + 2; // Leave one empty row
        ws.Cells[lastRow, 1].Value = "BL NUMBERS:"; // Header
        for (int i = 0; i < blNumbers.Count; i++)
        {
            ws.Cells[lastRow + i + 1, 1].Value = blNumbers[i]; // Write each BL number
        }

        package.SaveAs(new FileInfo(excelPath));

        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Excel file saved as: {excelPath}");
    }

    static void ExtractTextFromElement(OpenXmlElement element, List<string> contents, List<string> blNumbers)
    {
        foreach (var child in element.Elements())
        {
            if (child is Paragraph para)
            {
                string text = GetFullText(para);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    contents.Add(text);
                    if (text.Contains("BL NUMBER:"))
                    {
                        blNumbers.Add(text.Trim()); // Store detected BL numbers
                    }
                }
            }
            else if (child is Table table)
            {
                foreach (var row in table.Elements<TableRow>())
                {
                    var rowText = new List<string>();
                    foreach (var cell in row.Elements<TableCell>())
                    {
                        rowText.Add(GetFullText(cell));
                    }
                    string formattedRow = string.Join(" | ", rowText);
                    contents.Add(formattedRow);

                    if (formattedRow.Contains("BL NUMBER:"))
                    {
                        blNumbers.Add(formattedRow.Trim()); // Store BL numbers from tables
                    }
                }
            }
            else
            {
                ExtractTextFromElement(child, contents, blNumbers); // Recursively extract nested elements
            }
        }
    }

    static string GetFullText(OpenXmlElement element)
    {
        var texts = new List<string>();
        foreach (var textElement in element.Descendants<Text>())
        {
            texts.Add(textElement.Text);
        }
        return string.Join(" ", texts);
    }
}
