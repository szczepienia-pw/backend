using System.Diagnostics.CodeAnalysis;
using backend.Models.Visits;
using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace backend.Helpers.PdfGenerators
{
    [ExcludeFromCodeCoverage]
    public static class ReportGenerator
    {
        public static byte[] GeneratePDF(List<VaccinationModel> vaccinations, string dateStart, string dateEnd)
        {
            // Initialize structures
            var (workStream, pdf, document) = PdfGeneratorHelper.InitDocument();

            // Prepare font objects
            // Unfortunately due to limitation of iText these objects cannot be shared by
            // multiple PDF documents, therefore they cannot be declared as static.
            var (headerFont, textFont) = PdfGeneratorHelper.GetBasicFonts();

            // Prepare elements
            Image logo = PdfGeneratorHelper.GetLogo();
            document.Add(logo);

            Paragraph header = new Paragraph($"Vaccinations report")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.headerSize)
                .SetFont(headerFont);

            Paragraph subheader = new Paragraph($"{ dateStart} - { dateEnd }")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.subheaderSize)
                .SetFont(headerFont);

            document.Add(header);
            document.Add(subheader);

            Color color = PdfGeneratorHelper.GetColor();

            Table table = new Table(3, true);
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Disease");
            table.AddHeaderCell("Vaccine");
            table.FlushContent();

            table.GetHeader()
                .SetBackgroundColor(color)
                .SetFontColor(ColorConstants.WHITE)
                .SetFontSize(PdfGeneratorHelper.subheaderSize)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold();

            foreach (var vaccination in vaccinations)
            {
                table.AddCell(vaccination.VaccinationSlot.Date.ToString());
                table.AddCell(vaccination.Vaccine.Disease.ToString());
                table.AddCell(vaccination.Vaccine.Name);
            }

            document.Add(table);

            table.Complete();
            
            // Save document
            document.Close();

            // Return byte array
            return workStream.ToArray();
        }
    }
}