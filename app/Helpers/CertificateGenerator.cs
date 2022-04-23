using System;
using backend.Models.Visits;
using IronBarCode;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace backend.Helpers
{
    public static class CertificateGenerator
    {
        private static readonly int headerSize = 24;
        private static readonly int subheaderSize = 16;


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
                .SetFontSize(CertificateGenerator.headerSize);

            Paragraph patientDetailsHeader = new Paragraph("Personal information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(CertificateGenerator.subheaderSize);

            Paragraph vaccinationDetailsHeader = new Paragraph("Vaccination information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(CertificateGenerator.subheaderSize);

            Paragraph patientDetails = new Paragraph($"First name: {vaccination.Patient.FirstName} \nLast name: {vaccination.Patient.LastName}\nPESEL: {vaccination.Patient.Pesel}");

            Paragraph vaccinationDetails = new Paragraph($"Disease: {vaccination.Vaccine.Disease}\nVaccine: {vaccination.Vaccine.Name}\nVaccination date: {vaccination.VaccinationSlot.Date.ToShortDateString()}");


            // Generate document
            document.Add(header);
            document.Add(patientDetailsHeader);
            document.Add(patientDetails);
            document.Add(vaccinationDetailsHeader);
            document.Add(vaccinationDetails);
            document.Close();

            #region Debug
            using (var fileStream = File.Create("IO_CERTIFICATE_OUTPUT.pdf"))
            {
                workStream.Seek(0, SeekOrigin.Begin);
                workStream.CopyTo(fileStream);
            }
            #endregion

            return workStream.ToArray();
        }
    }
}

