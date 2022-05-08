using System;
using System.IO;
using System.Linq;
using System.Text;
using backend.Database;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Visits;
using backend_tests.Helpers;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Moq;
using Xunit;

namespace backend_tests.Unit.Helpers
{
    public class CertificateGeneratorTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private PatientModel patientMock { get; set; }
        private VaccinationModel vaccinationMock { get; set; }

        public CertificateGeneratorTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.patientMock = this.dataContextMock.Object.Patients.First();
            this.vaccinationMock = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Patient.Id == this.patientMock.Id && vaccination.Status == StatusEnum.Completed);
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFile()
        {
            byte[] payload = CertificateGenerator.GeneratePDF(this.vaccinationMock, false);

            // Check payload
            Assert.NotNull(payload);
            Assert.NotEmpty(payload);

            // Check file header
            string header = Encoding.UTF8.GetString(payload[0..5]);
            Assert.Equal("%PDF-", header);
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFileWithSinglePage()
        {
            byte[] payload = CertificateGenerator.GeneratePDF(this.vaccinationMock, false);

            // Extract PDF data
            MemoryStream memoryStream = new MemoryStream(payload);
            PdfReader reader = new PdfReader(memoryStream);
            PdfDocument document = new PdfDocument(reader);

            // Validate page count
            Assert.Equal(1, document.GetNumberOfPages());
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFileWithA4Page()
        {
            byte[] payload = CertificateGenerator.GeneratePDF(this.vaccinationMock, false);

            // Extract PDF data
            MemoryStream memoryStream = new MemoryStream(payload);
            PdfReader reader = new PdfReader(memoryStream);
            PdfDocument document = new PdfDocument(reader);

            // Validate page size
            Assert.Equal(PageSize.A4, document.GetDefaultPageSize());
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFileWithCorrectData()
        {
            byte[] payload = CertificateGenerator.GeneratePDF(this.vaccinationMock, false);

            // Extract PDF data
            MemoryStream memoryStream = new MemoryStream(payload);
            PdfReader reader = new PdfReader(memoryStream);
            PdfDocument document = new PdfDocument(reader);
            string pdfText = PdfTextExtractor.GetTextFromPage(document.GetFirstPage());

            // Validate data
            Assert.Contains(this.vaccinationMock.Patient.FirstName, pdfText);
            Assert.Contains(this.vaccinationMock.Patient.LastName, pdfText);
            Assert.Contains(this.vaccinationMock.Patient.Pesel, pdfText);
            Assert.Contains(this.vaccinationMock.Vaccine.Name, pdfText);
            Assert.Contains(this.vaccinationMock.Vaccine.Disease.ToString(), pdfText);
            Assert.Contains(this.vaccinationMock.VaccinationSlot.Date.ToShortDateString(), pdfText);
        }
    }
}
