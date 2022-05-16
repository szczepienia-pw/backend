using System;
using System.Collections.Generic;
using System.Linq;
using backend.Dto.Requests.Patient;
using backend.Exceptions;
using backend.Models.Vaccines;
using backend.Models.Visits;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Patient;

public partial class VaccinationTest
{
    [Theory]
    [InlineData(1, 1, 1)]
    public void ItSuccessfullyReserveVaccinationSlot(int patientsId, int vaccinationSlot, int vaccineId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        var rsp = this.vaccinationController
            .ReserveVaccinationSlot(vaccinationSlot, new ReserveSlotRequest() { VaccineId = vaccineId }).Result;
        
        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    [Theory]
    [InlineData(1, 1, 100)]
    public void ItFailToReserveVaccinationSlot(int patientsId, int vaccinationSlot, int vaccineId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        Assert.ThrowsAsync<NotFoundException>(() =>
            this.vaccinationController.ReserveVaccinationSlot(vaccinationSlot,
                new ReserveSlotRequest() { VaccineId = vaccineId }));
    }

    [Fact]
    public void ItSuccessfullyShowAvailableVaccines()
    {
        var vaccines = new List<VaccineModel>()
        {
            new()
            {
                Id = 1,
                Name = "COVID-19",
                Disease = DiseaseEnum.COVID19,
                RequiredDoses = 1
            }
        };
        this.dataContextMock.Setup(v => v.Vaccines).Returns(vaccines.AsQueryable().BuildMockDbSet().Object);

        var rsp = this.vaccinationController.ShowAvailableVaccines(new ShowVaccinesRequest() { Disease = "COVID-19" }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult)rsp).StatusCode);
    }

    [Fact]
    public void ItFailedToShowAvailableVaccines()
    {
        Assert.ThrowsAsync<NotFoundException>(() =>
            this.vaccinationController.ShowAvailableVaccines(new ShowVaccinesRequest() { Disease = "invalid" }));
    }

    [Theory]
    [InlineData(1, 1)]
    public void ItSuccessfullyCancelVaccinationSlot(int patientsId, int vaccinationSlotId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        var doctor = this.dataContextMock.Object.Doctors.First(d => d.Id == 1);
        var vaccinationSlots = new List<VaccinationSlotModel>()
        {
            new()
            {
                Id = 1,
                Date = DateTime.Now,
                Reserved = true,
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
                Doctor = doctor,
                Vaccine = new VaccineModel() {Name = "COVID-19", Disease = DiseaseEnum.COVID19, Id = 1, RequiredDoses = 1}
            }
        };

        this.dataContextMock.Setup(d => d.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
        this.dataContextMock.Setup(d => d.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
        
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        var rsp = this.vaccinationController.CancelVaccinationSlot(vaccinationSlotId).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    [Theory]
    [InlineData(1, 100)]
    public void ItFailToCancelVaccinationSlot(int patientsId, int vaccinationSlotId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        Assert.ThrowsAsync<NotFoundException>(() =>
            this.vaccinationController.CancelVaccinationSlot(vaccinationSlotId));
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyGetVaccinationHistory(int patientsId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        var rsp = this.vaccinationController.GetVaccinationsHistory(new FilterVaccinationsRequest()).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(1, 1)]
    public void ItSuccessfullyGenerateAVaccinationCertificate(int patientsId, int vaccinationId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        var doctor = this.dataContextMock.Object.Doctors.First(d => d.Id == 1);
        var vaccinationSlots = new List<VaccinationSlotModel>()
        {
            new()
            {
                Id = 1,
                Date = DateTime.Now,
                Reserved = true,
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
                Doctor = doctor,
                Vaccine = new VaccineModel() {Name = "COVID-19", Disease = DiseaseEnum.COVID19, Id = 1, RequiredDoses = 1},
                Status = StatusEnum.Completed,
                Patient = patient
            }
        };

        this.dataContextMock.Setup(d => d.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
        this.dataContextMock.Setup(d => d.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        var rsp = this.vaccinationController.DownloadVaccinationCertificate(vaccinationId).Result;
        
        Assert.IsType<FileContentResult>(rsp);
    }
    
    
    [Theory]
    [InlineData(1, 100)]
    public void ItFailToGenerateAVaccinationCertificate(int patientsId, int vaccinationId)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        Assert.ThrowsAsync<NotFoundException>(() =>
            this.vaccinationController.DownloadVaccinationCertificate(vaccinationId));
    }

    [Fact]
    public void SuccessfullyDeletePatient()
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == 1);
        this.vaccinationController.HttpContext.Items.Add("User", patient);

        var rsp = this.vaccinationController.DeletePatient().Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}