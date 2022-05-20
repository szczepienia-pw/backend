using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;

namespace backend.Helpers.PdfGenerators;

public static class PdfGeneratorHelper
{
    public static readonly int headerSize = 24;
    public static readonly int subheaderSize = 16;
    
    public static (MemoryStream, PdfDocument, Document) InitDocument()
    {
        MemoryStream workStream = new MemoryStream();
        PdfWriter writer = new PdfWriter(workStream);
        writer.SetCloseStream(false);
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);
        return (workStream, pdf, document);
    }

    public static (PdfFont, PdfFont) GetBasicFonts()
    {
        return (PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD), PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
    }
}