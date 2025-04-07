

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

var filePath = "C:\\Users\\kraft\\Documents\\General\\oocl\\word file .docx";

using(var wordDoc = WordprocessingDocument.Open(filePath, false))
{
    var body = wordDoc?.MainDocumentPart?.Document.Body;

    var paragraphs = body?.Descendants<Paragraph>();

    foreach (var paragraph in paragraphs)
    {
        var text = string.Concat(paragraph.Descendants<Text>().Select(t => t.Text));
        Console.WriteLine(text);
    }
}