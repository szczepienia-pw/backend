using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Controllers.Doctor;
using backend.Database;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Dto.Responses;
using backend.Dto.Responses.Doctor.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Visits;
using backend.Services.Doctor;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace backend_tests.Doctor
{
    public partial class VaccinationSlotTest
    {
        private readonly Mock<Mailer> mailerMock;
        private readonly Mock<DataContext> dbContext;
        private readonly VaccinationSlotService vaccinationSlotService;
        private readonly VaccinationSlotsController vaccinationSlotsController;
        
        public VaccinationSlotTest()
        {
            // Constructor is being executed before each test
            this.mailerMock = new Mock<Mailer>();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));

            this.dbContext = Helpers.DbHelper.GetMockedDataContextWithAccounts();
            this.vaccinationSlotService = new VaccinationSlotService(this.dbContext.Object, this.mailerMock.Object);
            this.vaccinationSlotsController = new VaccinationSlotsController(this.vaccinationSlotService);

            var doctor = this.dbContext.Object.Doctors.First(d => d.Id == 1);
            this.vaccinationSlotsController.ControllerContext.HttpContext = new DefaultHttpContext();
            this.vaccinationSlotsController.HttpContext.Items = new Dictionary<object, object?>() { {"User", doctor} };
        }

        [Theory]
        [InlineData("2023-04-01T14:15:00Z")]
        [InlineData("2023-04-15T14:15:00Z")]
        public async Task UtTestShouldAddNewDateSlot(string date)
        {
            var doctorModel = this.dbContext.Object.Doctors.First();

            var response = await this.vaccinationSlotService.AddNewSlot(
                new NewVaccinationSlotRequest() { Date = date },
                doctorModel
            );

            Assert.IsType<SuccessResponse>(response);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T14:00:00Z", true)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T14:14:00Z", true)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T14:14:59Z", true)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T14:15:00Z", false)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T13:46:00Z", true)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T13:45:01Z", true)]
        [InlineData("2023-04-01T14:00:00Z", "2023-04-01T13:45:00Z", false)]
        public async Task UtTestShouldThrowAnExceptionWhenAddingNewDateSlotWithin15Minutes(string firstDate, string secondDate, bool shouldThrow)
        {
            var doctorModel = this.dbContext.Object.Doctors.First();
            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Doctor = doctorModel,
                    Date = DateTime.Parse(firstDate),
                    Reserved = false
                }
            };

            this.dbContext
                .Setup(dbContext => dbContext.VaccinationSlots)
                .Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);

            var vaccinationSlotService = new VaccinationSlotService(this.dbContext.Object, this.mailerMock.Object);

            // Add second date
            var request = new NewVaccinationSlotRequest() {Date = secondDate};
            if (shouldThrow)
            {
                await Assert.ThrowsAsync<ValidationException>(() => this.vaccinationSlotService.AddNewSlot(request, doctorModel));
            }
            else
            {
                await this.vaccinationSlotService.AddNewSlot(request, doctorModel);
                Assert.True(true);
            }
        }

        [Theory]
        [InlineData("2021-04-01T14:00:00Z", true)]
        [InlineData("2022-01-01T14:15:00Z", true)]
        [InlineData("2022-03-01T07:00:00Z", true)]
        [InlineData("2022-09-01T08:00:00Z", false)]
        [InlineData("2032-04-01T20:00:00Z", false)]
        public async Task UtTestShouldThrowAnExceptionWhenAddingNewDateSlotInPast(string date, bool shouldThrow)
        {
            var doctorModel = this.dbContext.Object.Doctors.First();

            // Add second date
            var request = new NewVaccinationSlotRequest() { Date = date };
            if (shouldThrow)
            {
                await Assert.ThrowsAsync<ValidationException>(() => this.vaccinationSlotService.AddNewSlot(request, doctorModel));
            }
            else
            {
                await this.vaccinationSlotService.AddNewSlot(request, doctorModel);
                Assert.True(true);
            }
        }

        [Fact]
        public async Task UtTestShouldDeleteOnlyDoctorNotReservedSlots()
        {
            var firstDoctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            var secondDoctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 2);

            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Id = 1,
                    Date = DateTime.Now,
                    Reserved = false,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 2,
                    Date = DateTime.Now,
                    Reserved = true,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 3,
                    Date = DateTime.Now,
                    Reserved = false,
                    Doctor = secondDoctor
                }
            };

            this.dbContext.Setup(context => context.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);

            // Test correct deletion
            var response = await this.vaccinationSlotService.DeleteSlot(1, firstDoctor);
            Assert.IsType<SuccessResponse>(response);

            // Test reserved
            await Assert.ThrowsAsync<ConflictException>(
                () => this.vaccinationSlotService.DeleteSlot(2, firstDoctor)
            );

            // Test another doctor
            await Assert.ThrowsAsync<NotFoundException>(
                () => this.vaccinationSlotService.DeleteSlot(3, firstDoctor)
            );

            // Test not existing vaccination slot
            await Assert.ThrowsAsync<NotFoundException>(
                () => this.vaccinationSlotService.DeleteSlot(4, firstDoctor)
            );
        }

        [Theory]
        // Without start date
        [InlineData(null, null, null, new int[]{ 1, 2, 3, 4, 5, 6 })]
        [InlineData(1, null, null, new int[]{ 2, 4, 6 })]
        [InlineData(0, null, null, new int[]{ 1, 3, 5 })]
        // With start date
        [InlineData(null, "2022-04-02T14:00:00Z", null, new int[]{ 3, 4, 5, 6 })]
        [InlineData(1, "2022-04-02T14:00:00Z", null, new int[] { 4, 6 })]
        [InlineData(0, "2022-04-02T14:00:00Z", null, new int[] { 3, 5 })]
        // With end date
        [InlineData(null, null, "2022-04-19T14:00:00Z", new int[] { 1, 2, 3, 4 })]
        [InlineData(1, null, "2022-04-19T14:00:00Z", new int[] { 2, 4 })]
        [InlineData(0, null, "2022-04-19T14:00:00Z", new int[] { 1, 3 })]
        // With start and end date
        [InlineData(null, "2022-04-02T14:00:00Z", "2022-04-19T14:00:00Z", new int[] { 3, 4 })]
        [InlineData(1, "2022-04-02T14:00:00Z", "2022-04-19T14:00:00Z", new int[] { 4 })]
        [InlineData(0, "2022-04-02T14:00:00Z", "2022-04-19T14:00:00Z", new int[] { 3 })]
        // Edge cases
        [InlineData(null, "2022-04-20T14:00:01Z", null, new int[] {})]
        [InlineData(null, null, "2022-04-01T13:59:59Z", new int[] { })]
        public async Task UtTestShouldCorrectlyFilterVaccinationSlots(int? onlyReserved, string? startDate, string? endDate, int[] shownIds)
        {
            var firstDoctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            var secondDoctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 2);

            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Id = 1,
                    Date = DateTime.Parse("2022-04-01T14:00:00Z"),
                    Reserved = false,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 2,
                    Date = DateTime.Parse("2022-04-01T14:00:00Z"),
                    Reserved = true,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 3,
                    Date = DateTime.Parse("2022-04-10T14:00:00Z"),
                    Reserved = false,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 4,
                    Date = DateTime.Parse("2022-04-10T14:00:00Z"),
                    Reserved = true,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 5,
                    Date = DateTime.Parse("2022-04-20T14:00:00Z"),
                    Reserved = false,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 6,
                    Date = DateTime.Parse("2022-04-20T14:00:00Z"),
                    Reserved = true,
                    Doctor = firstDoctor
                },
                new()
                {
                    Id = 7,
                    Date = DateTime.Parse("2022-04-01T14:00:00Z"),
                    Reserved = false,
                    Doctor = secondDoctor
                },
                new()
                {
                    Id = 8,
                    Date = DateTime.Parse("2022-04-01T14:00:00Z"),
                    Reserved = true,
                    Doctor = secondDoctor
                }
            };

            this.dbContext.Setup(context => context.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);

            var request = new FilterVaccinationSlotsRequest()
            {
                OnlyReserved = onlyReserved,
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await this.vaccinationSlotService.GetSlots(request, firstDoctor);

            Assert.IsType<PaginatedResponse<VaccinationSlotModel, List<VaccinationSlotResponse>>>(result);

            var vaccinationSlotIds = result.Data.Select(vaccinationSlotResponse => vaccinationSlotResponse.Id).ToArray();
            Assert.Equal(shownIds, vaccinationSlotIds);
        }

        // Vaccinate patient
        [Theory]
        [InlineData(-1, StatusEnum.Completed)]
        [InlineData(0, StatusEnum.Completed)]
        [InlineData(int.MaxValue, StatusEnum.Completed)]
        [InlineData(int.MinValue, StatusEnum.Completed)]
        [InlineData(-1, StatusEnum.Canceled)]
        [InlineData(0, StatusEnum.Canceled)]
        [InlineData(int.MaxValue, StatusEnum.Canceled)]
        [InlineData(int.MinValue, StatusEnum.Canceled)]
        public void UtTestVaccinatePatientSlotThrowExceptionForNotExistingSlot(int slotId, StatusEnum status)
        {
            var doctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            Assert.ThrowsAsync<NotFoundException>(() => vaccinationSlotService.VaccinatePatient(slotId, status, doctor));
        }

        [Theory]
        [InlineData(1, StatusEnum.Completed)]
        [InlineData(1, StatusEnum.Canceled)]
        [InlineData(2, StatusEnum.Completed)]
        [InlineData(2, StatusEnum.Canceled)]
        public void UtTestVaccinatePatientSlotThrowExceptionForAnotherDoctorsSlot(int doctorId, StatusEnum status)
        {
            var doctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == doctorId);
            var slot = this.dbContext.Object.VaccinationSlots.First(slot => slot.Doctor.Id != doctor.Id);

            Assert.ThrowsAsync<NotFoundException>(() => vaccinationSlotService.VaccinatePatient(slot.Id, status, doctor));
        }

        [Theory]
        [InlineData(1, StatusEnum.Planned)]
        [InlineData(2, StatusEnum.Planned)]
        [InlineData(0, StatusEnum.Planned)]
        [InlineData(-1, StatusEnum.Planned)]
        public void UtTestVaccinatePatientSlotThrowExceptionForPlannedStatus(int slotId, StatusEnum status)
        {
            var doctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            Assert.ThrowsAsync<ValidationException>(() => vaccinationSlotService.VaccinatePatient(slotId, status, doctor));
        }

        [Theory]
        [InlineData(2, StatusEnum.Completed)]
        [InlineData(2, StatusEnum.Canceled)]
        public async void UtTestVaccinatePatientShouldSlotChangeVisitStatus(int slotId, StatusEnum status)
        {
            var doctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            await this.vaccinationSlotService.VaccinatePatient(slotId, status, doctor);

            Assert.Equal(status, this.dbContext.Object.Vaccinations.First(vaccination => vaccination.VaccinationSlot.Id == slotId).Status);
        }

        [Theory]
        [InlineData(2)]
        public async void TestVaccinatePatientShouldSendEmailAfterCancellation(int slotId)
        {
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));
            var doctor = this.dbContext.Object.Doctors.First(doctor => doctor.Id == 1);
            var vaccination = this.dbContext.Object.Vaccinations.First(vaccination => vaccination.VaccinationSlot.Id == slotId);

            await this.vaccinationSlotService.VaccinatePatient(slotId, StatusEnum.Canceled, doctor);

            this.mailerMock.Verify(mailer => mailer.SendEmailAsync(
                vaccination.Patient.Email,
                "Vaccination visit canceled",
                It.Is<string>(body => body.Contains(vaccination.Vaccine.Disease.ToString()) && 
                                      body.Contains(vaccination.VaccinationSlot.Date.ToShortDateString()) &&
                                      body.Contains(vaccination.Doctor.FirstName) &&
                                      body.Contains(vaccination.Doctor.LastName)),
                null
            ), Times.Once);
        }
    }
}
