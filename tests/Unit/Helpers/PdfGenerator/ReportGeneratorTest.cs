using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using backend.Database;
using backend.Helpers.PdfGenerators;
using backend.Models.Visits;
using backend_tests.Helpers;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Moq;
using Xunit;

namespace backend_tests.Unit.Helpers.PdfGenerator
{
    public class ReportGeneratorTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private string dateStart { get; set; } = "some date start";
        private string dateEnd { get; set; } = "some date end";
        private List<VaccinationModel> vaccinationsMock { get; set; }

        public ReportGeneratorTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.vaccinationsMock = this.dataContextMock.Object.Vaccinations
                .Where(vaccination => vaccination.Status == StatusEnum.Completed)
                .ToList();
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFile()
        {
            byte[] payload = ReportGenerator.GeneratePDF(this.vaccinationsMock, this.dateStart, this.dateEnd);

            // Check payload
            Assert.NotNull(payload);
            Assert.NotEmpty(payload);

            // Check file header
            string header = Encoding.UTF8.GetString(payload[0..5]);
            Assert.Equal("%PDF-", header);
        }

        [Fact]
        public void TestGeneratePDFShouldReturnPdfFileWithA4Page()
        {
            byte[] payload = ReportGenerator.GeneratePDF(this.vaccinationsMock, this.dateStart, this.dateEnd);

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
            byte[] payload = ReportGenerator.GeneratePDF(this.vaccinationsMock, this.dateStart, this.dateEnd);

            // Extract PDF data
            MemoryStream memoryStream = new MemoryStream(payload);
            PdfReader reader = new PdfReader(memoryStream);
            PdfDocument document = new PdfDocument(reader);
            string pdfText = PdfTextExtractor.GetTextFromPage(document.GetFirstPage());

            // Validate data
            Assert.Contains(this.dateStart, pdfText);
            Assert.Contains(this.dateEnd, pdfText);

            foreach (var vaccination in this.vaccinationsMock)
            {
                Assert.Contains(vaccination.VaccinationSlot.Date.ToString(), pdfText);
                Assert.Contains(vaccination.Vaccine.Disease.ToString(), pdfText);
                Assert.Contains(vaccination.Vaccine.Name, pdfText);
            }
        }
    }
}
