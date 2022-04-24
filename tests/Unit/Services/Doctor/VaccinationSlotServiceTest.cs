using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Dto.Responses;
using backend.Dto.Responses.Doctor.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Visits;
using backend.Services.Doctor;
using backend_tests.Helpers;
using Moq;
using Xunit;

namespace backend_tests.Unit.Services.Doctor
{
    public class VaccinationSlotServiceTest
    {
        private Mock<Mailer> mailerMock { get; set; }

        public VaccinationSlotServiceTest()
        {
            // Constructor is being executed before each test
            this.mailerMock = new Mock<Mailer>();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));
        }

        [Theory]
        [InlineData("2023-04-01T14:15:00Z")]
        [InlineData("2023-04-15T14:15:00Z")]
        public async Task TestShouldAddNewDateSlot(string date)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var vaccinationSlotService = new VaccinationSlotService(dataContext, this.mailerMock.Object);
            var doctorModel = dataContext.Doctors.First();

            var response = await vaccinationSlotService.AddNewSlot(
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
        public async Task TestShouldThrowAnExceptionWhenAddingNewDateSlotWithin15Minutes(string firstDate, string secondDate, bool shouldThrow)
        {
            var dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            var doctorModel = dataContextMock.Object.Doctors.First();
            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Doctor = doctorModel,
                    Date = DateTime.Parse(firstDate),
                    Reserved = false
                }
            };

            dataContextMock
                .Setup(dbContext => dbContext.VaccinationSlots)
                .Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);

            var vaccinationSlotService = new VaccinationSlotService(dataContextMock.Object, this.mailerMock.Object);

            // Add second date
            var request = new NewVaccinationSlotRequest() {Date = secondDate};
            if (shouldThrow)
            {
                await Assert.ThrowsAsync<ValidationException>(() => vaccinationSlotService.AddNewSlot(request, doctorModel));
            }
            else
            {
                await vaccinationSlotService.AddNewSlot(request, doctorModel);
                Assert.True(true);
            }
        }

        [Theory]
        [InlineData("2021-04-01T14:00:00Z", true)]
        [InlineData("2022-01-01T14:15:00Z", true)]
        [InlineData("2022-03-01T07:00:00Z", true)]
        [InlineData("2022-09-01T08:00:00Z", false)]
        [InlineData("2032-04-01T20:00:00Z", false)]
        public async Task TestShouldThrowAnExceptionWhenAddingNewDateSlotInPast(string date, bool shouldThrow)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var vaccinationSlotService = new VaccinationSlotService(dataContext, this.mailerMock.Object);
            var doctorModel = dataContext.Doctors.First();

            // Add second date
            var request = new NewVaccinationSlotRequest() { Date = date };
            if (shouldThrow)
            {
                await Assert.ThrowsAsync<ValidationException>(() => vaccinationSlotService.AddNewSlot(request, doctorModel));
            }
            else
            {
                await vaccinationSlotService.AddNewSlot(request, doctorModel);
                Assert.True(true);
            }
        }

        [Fact]
        public async Task TestShouldDeleteOnlyDoctorNotReservedSlots()
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts();
            var firstDoctor = dataContext.Object.Doctors.First(doctor => doctor.Id == 1);
            var secondDoctor = dataContext.Object.Doctors.First(doctor => doctor.Id == 2);

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

            dataContext.Setup(context => context.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            var vaccinationService = new VaccinationSlotService(dataContext.Object, this.mailerMock.Object);

            // Test correct deletion
            var response = await vaccinationService.DeleteSlot(1, firstDoctor);
            Assert.IsType<SuccessResponse>(response);

            // Test reserved
            await Assert.ThrowsAsync<ConflictException>(
                () => vaccinationService.DeleteSlot(2, firstDoctor)
            );

            // Test another doctor
            await Assert.ThrowsAsync<NotFoundException>(
                () => vaccinationService.DeleteSlot(3, firstDoctor)
            );

            // Test not existing vaccination slot
            await Assert.ThrowsAsync<NotFoundException>(
                () => vaccinationService.DeleteSlot(4, firstDoctor)
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
        public async Task TestShouldCorrectlyFilterVaccinationSlots(int? onlyReserved, string? startDate, string? endDate, int[] shownIds)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts();
            var firstDoctor = dataContext.Object.Doctors.First(doctor => doctor.Id == 1);
            var secondDoctor = dataContext.Object.Doctors.First(doctor => doctor.Id == 2);

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

            dataContext.Setup(context => context.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            var vaccinationService = new VaccinationSlotService(dataContext.Object, this.mailerMock.Object);

            var request = new FilterVaccinationSlotsRequest()
            {
                OnlyReserved = onlyReserved,
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await vaccinationService.GetSlots(request, firstDoctor);

            Assert.IsType<PaginatedResponse<VaccinationSlotModel, List<VaccinationSlotResponse>>>(result);

            var vaccinationSlotIds = result.Data.Select(vaccinationSlotResponse => vaccinationSlotResponse.Id).ToArray();
            Assert.Equal(shownIds, vaccinationSlotIds);
        }
    }
}
