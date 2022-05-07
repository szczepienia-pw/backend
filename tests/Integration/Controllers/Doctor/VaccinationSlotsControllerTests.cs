using System;
using System.Collections.Generic;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Exceptions;
using backend.Models.Visits;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using System.Linq;
using backend_tests.Helpers;

namespace backend_tests.Doctor;

public partial class VaccinationSlotTest
{
    [Fact]
    public void ItSuccessfullyGetVaccinationSlots()
    {
        var rsp = this.vaccinationSlotsController.GetVaccinationSlots(new FilterVaccinationSlotsRequest() { Page = 1 })
            .Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("2023-08-24T14:15:22Z")]
    public void ItSuccessfullyCreateAVaccinationSlot(string date)
    {
        var rsp = this.vaccinationSlotsController.AddVaccinationSlot(new NewVaccinationSlotRequest() { Date = date })
            .Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("2019-08-24T14:15:22Z")]
    public void ItFailToCreateAVaccinationSlot(string date)
    {
        Assert.ThrowsAsync<ValidationException>(() =>
            this.vaccinationSlotsController.AddVaccinationSlot(new NewVaccinationSlotRequest() { Date = date }));
    }

    [Theory]
    [InlineData(1, StatusEnum.Completed)]
    public void ItSuccessfullyVaccinatePatient(int slotId, StatusEnum status)
    {
        var doctor = this.dbContext.Object.Doctors.First(d => d.Id == 1);
        var vaccinationSlots = new List<VaccinationSlotModel>()
        {
            new()
            {
                Id = 1,
                Date = DateTime.Now,
                Reserved = false,
                Doctor = doctor
            },
        };
        var vaccinations = new List<VaccinationModel>()
        {
            new()
            {
                Id = 1,
                VaccinationSlot = vaccinationSlots[0],
                VaccinationSlotId = 1,
                PatientId = 1,
                DoctorId = 1,
                Doctor = doctor
            }
        };

        this.dbContext.Setup(d => d.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
        this.dbContext.Setup(d => d.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
        
        var rsp = this.vaccinationSlotsController
            .VaccinatePatient(slotId, new VaccinatePatientRequest() { Status = status.GetDescription() }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    
    [Theory]
    [InlineData(100, StatusEnum.Completed)]
    public void ItFailedToVaccinatePatient(int slotId, StatusEnum status)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationSlotsController.VaccinatePatient(slotId,
            new VaccinatePatientRequest() { Status = status.GetDescription() }));
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyDeleteVaccinationSlot(int slotId)
    {
        var rsp = this.vaccinationSlotsController.DeleteVaccinationSlot(slotId).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(100)]
    public void ItFailedToDeleteVaccinationSlot(int slotId)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.vaccinationSlotsController.DeleteVaccinationSlot(slotId));
    }
}