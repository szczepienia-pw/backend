using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace backend.Helpers.PdfGenerators;

public static class PdfGeneratorHelper
{
    public static readonly int headerSize = 24;
    public static readonly int subheaderSize = 16;
    private static readonly string logoPath = "Resources/Logo.png";
    
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

    public static Image GetLogo()
    {
        byte[] logoBytes = File.ReadAllBytes(PdfGeneratorHelper.logoPath);
        ImageData logoData = ImageDataFactory.Create(logoBytes);
        Image logo = new Image(logoData)
                .SetHeight(100f)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

        return logo;
    }

    public static Color GetColor()
    {
        return new DeviceRgb(137, 134, 236);
    }
}