using System.Linq;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Requests.Admin;
using backend.Dto.Requests.Admin.Doctor;
using backend.Exceptions;
using backend.Helpers;
using backend.Services.Admin;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace backend_tests.Admin;

public partial class AdminDoctorsTest
{
    [Theory]
    [InlineData("jane", "doe", "jane.doe@test.com", "password")]
    public void ItSuccesfullyCreateDoctor(string firstName, string lastName, string email, string password)
    {
        var request = new CreateDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email, Password = password };
        var rsp = this.adminDoctorsController.CreateDoctor(request).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("John", "Doctor", "john@doctor.com", "password")]
    public async void ItFailToCreateADoctorWithATakenEmail(string firstName, string lastName, string email, string password)
    {
        // make sure that a doctor with such an email exists already in the DB
        if (this.dataContextMock.Object.Doctors.Where(d => d.Email == email).ToArray().Length == 0)
        {
            await this.adminDoctorsController.CreateDoctor(
                new CreateDoctorRequest()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password
                }
            );
        }

        Assert.ThrowsAsync<ValidationException>(async () => this.adminDoctorsController.CreateDoctor(
            new CreateDoctorRequest()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            }
        ));
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyDeleteADoctor(int doctorsId)
    {
        var rsp = this.adminDoctorsController.DeleteDoctor(doctorsId).Result;
        
        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(100)]
    public void ItFailToDeleteADoctor(int doctorsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.adminDoctorsController.DeleteDoctor(doctorsId));
    }

    [Theory]
    [InlineData("john@doctor.com", 1)]
    public void ItSuccessfullyUpdateDoctor(string email, int doctorsId)
    {
        var rsp = this.adminDoctorsController.UpdateDoctor(doctorsId, new EditDoctorRequest() { Email = email }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("john@doctor.com", 100)]
    public void ItFailureToUpdateADoctor(string email, int doctorsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() =>
            this.adminDoctorsController.UpdateDoctor(doctorsId, new EditDoctorRequest() { Email = email }));
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyShowDoctorsData(int doctorsId)
    {
        var rsp = this.adminDoctorsController.ShowDoctor(doctorsId).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(100)]
    public void ItFailureToShowDoctorsData(int doctorsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.adminDoctorsController.ShowDoctor(doctorsId));
    }

    [Fact]
    public void ItSuccessfullyShowsAllDoctorsData()
    {
        var rsp = this.adminDoctorsController.ShowDoctors(new PaginationRequest() { Page = 1 }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}