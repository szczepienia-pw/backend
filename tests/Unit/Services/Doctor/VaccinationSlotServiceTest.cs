﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Dto.Responses;
using backend.Exceptions;
using backend.Models.Visits;
using backend.Services.Doctor;
using backend_tests.Helpers;
using Xunit;

namespace backend_tests.Unit.Services.Doctor
{
    public class VaccinationSlotServiceTest
    {
        [Theory]
        [InlineData("2022-04-01T14:15:00Z")]
        [InlineData("2022-04-15T14:15:00Z")]
        public async Task TestShouldAddNewDateSlot(string date)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var vaccinationSlotService = new VaccinationSlotService(dataContext);
            var doctorModel = dataContext.Doctors.First();

            var response = await vaccinationSlotService.AddNewSlot(
                new NewVaccinationSlotRequest() { Date = date },
                doctorModel
            );

            Assert.IsType<SuccessResponse>(response);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T14:00:00Z", true)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T14:14:00Z", true)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T14:14:59Z", true)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T14:15:00Z", false)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T13:46:00Z", true)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T13:45:01Z", true)]
        [InlineData("2022-04-01T14:00:00Z", "2022-04-01T13:45:00Z", false)]
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

            var vaccinationSlotService = new VaccinationSlotService(dataContextMock.Object);

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
            var vaccinationService = new VaccinationSlotService(dataContext.Object);

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
    }
}
