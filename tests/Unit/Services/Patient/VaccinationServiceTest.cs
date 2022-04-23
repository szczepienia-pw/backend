using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses.Common.Vaccination;
using backend.Dto.Responses.Patient.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Models.Visits;
using backend.Services.Patient;
using backend_tests.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Xunit;

namespace backend_tests.Unit.Services.Patient
{
    public class VaccinationServiceTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private Mock<Mailer> mailerMock { get; set; }
        private VaccinationService vaccinationServiceMock { get; set; }
        private PatientModel patientMock { get; set; }

        public VaccinationServiceTest()
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
        }

        // Show available vaccination slots
        [Theory]
        [InlineData("COVID-19", new int[] { 1, 2, 3 })]
        [InlineData("COVID-21", new int[] { 4, 5 })]
        [InlineData("Flu", new int[] { 6, 7 })]
        [InlineData("OTHER", new int[] { 8, 9, 10 })]
        public void TestShowVaccinesForValidDiseases(string diseaseName, int[] expectedIds)
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
        public void TestShowVaccinesThrowExceptionForInvalidDiseases(string diseaseName)
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
        public void TestReserveVaccinationSlotForAvailableSlotAndValidVaccineReturnsSuccess(int slotId, int vaccineId)
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
        public void TestReserveVaccinationSlotForAvailableSlotAndValidVaccineReservesSlot(int slotId, int vaccineId)
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
        public void TestReserveVaccinationSlotForAvailableSlotAndValidVaccineCreatesVaccinationRecord(int slotId, int vaccineId)
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
                It.Is<string>(body => body.Contains(slot.Date.ToString())),
                null
            ), Times.Once);
        }

        [Theory]
        [InlineData(3, 1)]
        [InlineData(-1, 2)]
        [InlineData(0, 3)]
        public void TestReserveVaccinationSlotThrowExceptionForInvalidSlot(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        [InlineData(1, 11)]
        public void TestReserveVaccinationSlotThrowExceptionForInvalidVaccine(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        [Theory]
        [InlineData(2, 0)]
        public void TestReserveVaccinationSlotThrowExceptionForReservedSlot(int slotId, int vaccineId)
        {
            Assert.ThrowsAsync<ConflictException>(() => this.vaccinationServiceMock.ReserveVaccinationSlot(this.patientMock, slotId, vaccineId));
        }

        // Get available vaccination slot
        [Fact]
        public void TestGetAvailableVaccinationSlots()
        {
            var response = this.vaccinationServiceMock.GetAvailableVaccinationSlots();
            Assert.NotNull(response);
            
            List<AvailableSlotResponse> slots = new List<AvailableSlotResponse>(response.Result);
            Assert.Equal(slots.Select(slot => slot.Id).ToArray(), this.dataContextMock.Object.VaccinationSlots.Where(slot => !slot.Reserved).Select(slot => slot.Id).ToArray());
            Assert.Equal(slots.Select(slot => slot.Date).ToArray(), this.dataContextMock.Object.VaccinationSlots.Where(slot => !slot.Reserved).Select(slot => slot.Date.ToUniversalTime()).ToArray());
        }

        // Cancel reservation

        [Fact]
        public void TestShouldThrowAnExceptionWhenCancelingNotHisVaccination()
        {
            var vaccinationSlot = this.dataContextMock.Object.VaccinationSlots.First();

            Assert.ThrowsAsync<ConflictException>(
                () => this.vaccinationServiceMock.CancelVaccinationSlot(this.patientMock, vaccinationSlot.Id)
            );
        }

        [Fact]
        public async void TestShouldCorrectlyCancelVaccination()
        {
            var vaccination = this.dataContextMock.Object.Vaccinations.First();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            await this.vaccinationServiceMock.CancelVaccinationSlot(this.patientMock, vaccination.VaccinationSlotId);

            Assert.Equal(false, vaccination.VaccinationSlot?.Reserved);
            Assert.Equal(StatusEnum.Canceled, vaccination.Status);

            this.mailerMock.Verify(mailer => mailer.SendEmailAsync(
                this.patientMock.Email,
                "Vaccination visit canceled",
                It.Is<string>(body => body.Contains(vaccination.VaccinationSlot.Date.ToString())),
                null
            ), Times.Once);
        }
    }
}