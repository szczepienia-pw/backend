using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Controllers.Patient;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses.Common.Vaccination;
using backend.Dto.Responses.Patient.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Models.Visits;
using backend.Services;
using backend.Services.Patient;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Xunit;

namespace backend_tests.Patient
{
    public partial class VaccinationTest
    {
        private readonly Mock<DataContext> dataContextMock;
        private readonly Mock<Mailer> mailerMock;
        private readonly VaccinationService vaccinationServiceMock;
        private readonly PatientModel patientMock;
        private readonly VaccinationController vaccinationController;

        public VaccinationTest()
        {
            // Constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.mailerMock = new Mock<Mailer>();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));

            this.vaccinationServiceMock = new VaccinationService(this.dataContextMock.Object, this.mailerMock.Object);
            this.patientMock = this.dataContextMock.Object.Patients.First();
            this.vaccinationController = new VaccinationController(this.vaccinationServiceMock);
            this.vaccinationController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        // Show available vaccination slots
        [Theory]
        [InlineData("COVID-19", new int[] { 1, 2, 3 })]
        [InlineData("COVID-21", new int[] { 4, 5 })]
        [InlineData("Flu", new int[] { 6, 7 })]
        [InlineData("OTHER", new int[] { 8, 9, 10 })]
        public void UtTestShowVaccinesForValidDiseases(string diseaseName, int[] expectedIds)
        {
            var response = this.vaccinationServiceMock.ShowAvailableVaccines(new ShowVaccinesRequest() { Disease = diseaseName });
            Assert.NotNull(response);

            List<VaccineResponse> vaccines = new List<VaccineResponse>(response.Result.Vaccines);
            Assert.Equal(vaccines.Select(vaccine => vaccine.Id).ToArray(), expectedIds);
        }

        [Theory]
        [InlineData("COVID19")]
        [InlineData("COVID21")]
        [InlineData("FLU")]
        [InlineData("Other")]
        [InlineData("RSV")]
        [InlineData("")]
        public void UtTestShowVaccinesThrowExceptionForInvalidDiseases(string diseaseName)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.vaccinationServiceMock.ShowAvailableVaccines(new ShowVaccinesRequest() { Disease = diseaseName }));
        }

        // Reserve vaccination slot
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(1, 8)]
        [InlineData(1, 9)]
        [InlineData(1, 10)]
        public void UtTestReserveVaccinationSlotForAvailableSlotAndValidVaccineReturnsSuccess(int slotId, int vaccineId)
        {
            var response = this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId).Result;

            Assert.NotNull(response);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(1, 8)]
        [InlineData(1, 9)]
        [InlineData(1, 10)]
        public void UtTestReserveVaccinationSlotForAvailableSlotAndValidVaccineReservesSlot(int slotId, int vaccineId)
        {
            var response = this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId).Result;

            var slot = this.dataContextMock.Object.VaccinationSlots.FirstOrDefault(slot => slot.Id == slotId);

            Assert.NotNull(slot);
            Assert.True(slot.Reserved);
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(1, 8)]
        [InlineData(1, 9)]
        [InlineData(1, 10)]
        public void UtTestReserveVaccinationSlotForAvailableSlotAndValidVaccineCreatesVaccinationRecord(int slotId, int vaccineId)
        {
            List<VaccinationModel> verifyList = new List<VaccinationModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Vaccinations.Add(It.IsAny<VaccinationModel>())).Callback<VaccinationModel>((v) => verifyList.Add(v));
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            var response = this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId).Result;

            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.Single(verifyList);
            var record = verifyList.FirstOrDefault(visit => visit.VaccinationSlot.Id == slotId);
            Assert.NotNull(record);

            Assert.Equal(record.Vaccine.Id, vaccineId);
            Assert.Equal(record.Patient.Id, this.patientMock.Id);

            var slot = this.dataContextMock.Object.VaccinationSlots.FirstOrDefault(slot => slot.Id == slotId);
            Assert.Equal(record.Doctor.Id, slot.Doctor.Id);

            this.mailerMock.Verify(mailer => mailer.SendEmailAsync(
                this.patientMock.Email,
                "Vaccination visit confirmation",
                It.Is<string>(body => body.Contains(slot.Date.ToShortDateString())),
                null
            ), Times.Once);
        }

        [Theory]
        [InlineData(3, 1)]
        [InlineData(-1, 2)]
        [InlineData(0, 3)]
        public void UtTestReserveVaccinationSlotThrowExceptionForInvalidSlot(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        [InlineData(1, 11)]
        public void UtTestReserveVaccinationSlotThrowExceptionForInvalidVaccine(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        [Theory]
        [InlineData(2, 0)]
        public void UtTestReserveVaccinationSlotThrowExceptionForReservedSlot(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<ConflictException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        // Cancel reservation

        [Fact]
        public void UtTestShouldThrowAnExceptionWhenCancelingNotHisVaccination()
        {
            var vaccinationSlot = this.dataContextMock.Object.VaccinationSlots.First();

            Assert.ThrowsAsync<ConflictException>(
                () => this.vaccinationServiceMock.CancelVaccinationSlot(this.patientMock, vaccinationSlot.Id)
            );
        }

        [Fact]
        public async void UtTestShouldCorrectlyCancelVaccination()
        {
            var vaccination = this.dataContextMock.Object.Vaccinations.First();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            await this.vaccinationServiceMock.CancelVaccinationSlot(this.patientMock, vaccination.VaccinationSlotId);

            this.dataContextMock.Verify(dataContext => dataContext.Add(It.IsAny<VaccinationSlotModel>()), Times.Once());
            
            Assert.Equal(true, vaccination.VaccinationSlot?.Reserved);
            Assert.Equal(StatusEnum.Canceled, vaccination.Status);

            this.mailerMock.Verify(mailer => mailer.SendEmailAsync(
                this.patientMock.Email,
                "Vaccination visit canceled",
                It.Is<string>(body => body.Contains(vaccination.VaccinationSlot.Date.ToShortDateString())),
                null
            ), Times.Once);
        }

        // Download certificate

        [Theory]
        [InlineData(StatusEnum.Canceled)]
        [InlineData(StatusEnum.Planned)]
        public void TestDownloadCertificateThrowExceptionForNotCompletedVisit(StatusEnum status)
        {
            var vaccination = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Patient.Id == this.patientMock.Id && vaccination.Status == status);
            Assert.Throws<ConflictException>(() => this.vaccinationServiceMock.DownloadVaccinationCertificate(this.patientMock, vaccination.Id));
        }

        [Theory]
        [InlineData(StatusEnum.Completed)]
        public void TestDownloadCertificateThrowExceptionForAnotherPatientsVisit(StatusEnum status)
        {
            var patient = this.dataContextMock.Object.Patients.First(patient => patient.Id != this.patientMock.Id);
            var vaccination = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Patient.Id == patient.Id && vaccination.Status == status);
            Assert.Throws<NotFoundException>(() => this.vaccinationServiceMock.DownloadVaccinationCertificate(this.patientMock, vaccination.Id));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void TestDownloadCertificateThrowExceptionForInvalidVaccination(int vaccinationId)
        {
            Assert.Throws<NotFoundException>(() => this.vaccinationServiceMock.DownloadVaccinationCertificate(this.patientMock, vaccinationId, false));
        }

        [Fact]
        public void TestDownloadCertificateShouldReturnPdfFile()
        {
            var vaccination = this.dataContextMock.Object.Vaccinations.First(vaccination => vaccination.Patient.Id == this.patientMock.Id && vaccination.Status == StatusEnum.Completed);
            byte[] payload = this.vaccinationServiceMock.DownloadVaccinationCertificate(this.patientMock, vaccination.Id, false);

            // Check payload
            Assert.NotNull(payload);
            Assert.NotEmpty(payload);

            // Check file header
            string header = Encoding.UTF8.GetString(payload[0..5]);
            Assert.Equal("%PDF-", header);
        }
    }
}