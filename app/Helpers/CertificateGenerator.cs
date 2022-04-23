using backend.Models.Visits;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace backend.Helpers
{
    public static class CertificateGenerator
    {
        private static readonly int headerSize = 24;
        private static readonly int subheaderSize = 16;
        private static readonly PdfFont headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        private static readonly PdfFont textfont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);


        public static byte[] GeneratePDF(VaccinationModel vaccination)
        {
            // Initialize structures
            MemoryStream workStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(workStream);
            writer.SetCloseStream(false);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            // Prepare elements
            Paragraph header = new Paragraph("Vaccination certificate")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(CertificateGenerator.headerSize)
                .SetFont(headerFont);

            Paragraph patientDetailsHeader = new Paragraph("Personal information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(CertificateGenerator.subheaderSize)
                .SetFont(headerFont);

            Paragraph vaccinationDetailsHeader = new Paragraph("Vaccination information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(CertificateGenerator.subheaderSize)
                .SetFont(headerFont);

            Paragraph patientDetails = new Paragraph($"First name: {vaccination.Patient.FirstName} \nLast name: {vaccination.Patient.LastName}\nPESEL: {vaccination.Patient.Pesel}")
                .SetFont(textfont);

            Paragraph vaccinationDetails = new Paragraph($"Disease: {vaccination.Vaccine.Disease}\nVaccine: {vaccination.Vaccine.Name}\nVaccination date: {vaccination.VaccinationSlot.Date.ToShortDateString()}")
                .SetFont(textfont);

            // Generate document
            document.Add(header);
            document.Add(patientDetailsHeader);
            document.Add(patientDetails);
            document.Add(vaccinationDetailsHeader);
            document.Add(vaccinationDetails);
            document.Close();

            // Return byte array
            return workStream.ToArray();
        }
    }
}