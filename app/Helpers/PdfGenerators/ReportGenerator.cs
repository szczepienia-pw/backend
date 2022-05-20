using System.Diagnostics.CodeAnalysis;
using backend.Models.Visits;
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
            Paragraph header = new Paragraph($"Vaccinations report \n {dateStart} - {dateEnd} \n")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.headerSize)
                .SetFont(headerFont);

            document.Add(header);

            Table table = new Table(3, true);
            table.AddHeaderCell("Date").SetFont(textFont).SetFontSize(PdfGeneratorHelper.subheaderSize);
            table.AddHeaderCell("Disease");
            table.AddHeaderCell("Vaccine");

            foreach (var vaccination in vaccinations)
            {
                table.AddCell(vaccination.VaccinationSlot.Date.ToString());
                table.AddCell(vaccination.Vaccine.Disease.ToString());
                table.AddCell(vaccination.Vaccine.Name);
            }

            document.Add(table);
            
            // Save document
            document.Close();

            // Return byte array
            return workStream.ToArray();
        }
    }
}