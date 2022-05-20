using System.Diagnostics.CodeAnalysis;
using backend.Models.Visits;
using iText.Layout.Element;
using iText.Layout.Properties;
using backend.Models.Accounts;
using iText.Barcodes;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Xobject;

namespace backend.Helpers.PdfGenerators
{
    [ExcludeFromCodeCoverage]
    public static class CertificateGenerator
    {
        public static byte[] GeneratePDF(VaccinationModel vaccination, bool generateQrCode = true)
        {
            // Initialize structures
            var (workStream, pdf, document) = PdfGeneratorHelper.InitDocument();

            // Prepare font objects
            // Unfortunately due to limitation of iText these objects cannot be shared by
            // multiple PDF documents, therefore they cannot be declared as static.
            var (headerFont, textFont) = PdfGeneratorHelper.GetBasicFonts();

            // Prepare elements
            Paragraph header = new Paragraph("Vaccination certificate")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.headerSize)
                .SetFont(headerFont);

            Paragraph patientDetailsHeader = new Paragraph("Personal information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.subheaderSize)
                .SetFont(headerFont);

            Paragraph vaccinationDetailsHeader = new Paragraph("Vaccination information")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(PdfGeneratorHelper.subheaderSize)
                .SetFont(headerFont);

            Paragraph patientDetails = new Paragraph(CertificateGenerator.GeneratePatientDataString(vaccination.Patient))
                .SetFont(textFont);

            Paragraph vaccinationDetails = new Paragraph(CertificateGenerator.GenerateVaccinationDataString(vaccination))
                .SetFont(textFont);

            // Generate document
            document.Add(header);
            document.Add(patientDetailsHeader);
            document.Add(patientDetails);
            document.Add(vaccinationDetailsHeader);
            document.Add(vaccinationDetails);

            // Prepare QR code
            // Due to licensing limitations of IronBarCode, generating QR code during
            // unit testing is treated as usage outside of Visual Studio development
            // environment, which is not covered by free license. When generating
            // certificate for unit tests flag "generateQrCode" should be set to false.
            if(generateQrCode)
            {
                BarcodeQRCode qrCodeData = CertificateGenerator.GenerateQRCode(vaccination);
                PdfFormXObject qrCodeObject = qrCodeData.CreateFormXObject(ColorConstants.BLACK, pdf);
                Image image = new Image(qrCodeObject)
                    .SetWidth(200f)
                    .SetHeight(200f)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(image);
            }

            // Save document
            document.Close();

            // Return byte array
            return workStream.ToArray();
        }

        private static BarcodeQRCode GenerateQRCode(VaccinationModel vaccination)
        {
            return new BarcodeQRCode(CertificateGenerator.GenerateQRCodeDataString(vaccination));
        }

        private static string GeneratePatientDataString(PatientModel patient)
        {
            return $"First name: {patient.FirstName} \nLast name: {patient.LastName}\nPESEL: {patient.Pesel}";
        }

        private static string GenerateVaccinationDataString(VaccinationModel vaccination)
        {
            return $"Disease: {vaccination.Vaccine.Disease}\nVaccine: {vaccination.Vaccine.Name}\nVaccination date: {vaccination.VaccinationSlot.Date.ToShortDateString()}";
        }

        private static string GenerateQRCodeDataString(VaccinationModel vaccination)
        {
            return $"[Digital Vaccination Certificate]\n{CertificateGenerator.GeneratePatientDataString(vaccination.Patient)}\n{CertificateGenerator.GenerateVaccinationDataString(vaccination)}";
        }
    }
}