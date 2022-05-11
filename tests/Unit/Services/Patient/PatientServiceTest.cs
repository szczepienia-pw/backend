using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Patient;
using backend_tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Controllers.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend_tests.Patient
{
    public partial class PatientTest
    {
        private readonly Mock<DataContext> dataContextMock;
        private readonly SecurePasswordHasher securePasswordHasherMock;
        private readonly PatientService patientServiceMock;
        private readonly PatientController patientController;
        private readonly Mock<Mailer> mailerMock;

        private T TestInputParse<T,U>(params string?[] input) 
            where T : PatientRequest, new()
            where U : PatientAddressRequest, new()
        {
            var addressRequest = new U()
            {
                City = input[0],
                ZipCode = input[1],
                Street = input[2],
                HouseNumber = input[3],
                LocalNumber = input[4],
            };

            return new T()
            {
                FirstName = input[5],
                LastName = input[6],
                Email = input[7],
                Pesel = input[9],
                Address = addressRequest
            };
        }

        public PatientTest()
        {
            this.mailerMock = new Mock<Mailer>();
            this.mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));
            
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.securePasswordHasherMock = SecurePasswordHasherHelper.Hasher;
            this.patientServiceMock = new PatientService(
                this.dataContextMock.Object, 
                this.securePasswordHasherMock, 
                this.mailerMock.Object,
                Options.Create(new FrontendUrlsSettings() { ConfirmRegistration = "http://localhost/{token}"})
            );
            this.patientController = new PatientController(this.patientServiceMock);
            this.patientController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Theory]
        [InlineData("Warszawa", "00-528", "Hoża", "5a", "3", "BB", "BB", "bb@hot.com", "passwd", "67062675913")]
        [InlineData("Warszawa", "00-528", "Hoża", "5a", "", "BB", "BB", "bb2@hot.com", "passwd", "78121898585")]
        public void UtPatientShouldBeRegistered(params string?[] input)
        {
            var patientRequest = this.TestInputParse<PatientRegistrationRequest, PatientRegistrationAddressRequest>(input);
            patientRequest.Password = input[8];

            List<PatientModel> verifyList = new List<PatientModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Patients.Add(It.IsAny<PatientModel>())).Callback<PatientModel>(patient => verifyList.Add(patient));

            var rsp = this.patientServiceMock.Register(patientRequest).Result;

            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());
            this.dataContextMock.Verify(dataContext => dataContext.Patients.Add(It.IsAny<PatientModel>()), Times.Once());

            Assert.Single(verifyList);
            var added = verifyList[0];

            Assert.Equal(input[0], added.Address.City);
            Assert.Equal(input[1], added.Address.ZipCode);
            Assert.Equal(input[2], added.Address.Street);
            Assert.Equal(input[3], added.Address.HouseNumber);
            Assert.Equal(input[4], added.Address.LocalNumber);
            Assert.Equal(input[5], added.FirstName);
            Assert.Equal(input[6], added.LastName);
            Assert.Equal(input[7], added.Email);
            Assert.True(this.securePasswordHasherMock.Verify(input[8], added.Password));
            Assert.Equal(input[9], added.Pesel);

            Assert.Equal(input[0], rsp.Address.City);
            Assert.Equal(input[1], rsp.Address.ZipCode);
            Assert.Equal(input[2], rsp.Address.Street);
            Assert.Equal(input[3], rsp.Address.HouseNumber);
            Assert.Equal(input[4], rsp.Address.LocalNumber);
            Assert.Equal(input[5], rsp.FirstName);
            Assert.Equal(input[6], rsp.LastName);
            Assert.Equal(input[7], rsp.Email);
            Assert.Equal(input[9], rsp.Pesel);
            
            Assert.NotNull(added.VerificationToken);
        }

        [Theory]
        [InlineData(1, null, null, "Hoża", null, null, "BB", "BB",null, null, "78121898585")]
        public void UtShouldEditPatientData(int patientId, params string[] input)
        {
            var patientRequest = this.TestInputParse<PatientRequest, PatientAddressRequest>(input);
            var patient = this.dataContextMock.Object.Patients.Where(patient => patient.Id == patientId).First();
            
            List<PatientModel> verifyList = new List<PatientModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Update(It.IsAny<PatientModel>())).Callback<PatientModel>(patient => verifyList.Add(patient));

            var rsp = this.patientServiceMock.EditPatient(patientId, patientRequest).Result;

            this.dataContextMock.Verify(dataContext => dataContext.Update(It.IsAny<PatientModel>()), Times.Once());
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.Single(verifyList);
            var updated = verifyList[0];

            Assert.Equal(input[0] != null ? input[0] : patient?.Address?.City, updated.Address.City);
            Assert.Equal(input[1] != null ? input[1] : patient?.Address?.ZipCode, updated.Address.ZipCode);
            Assert.Equal(input[2] != null ? input[2] : patient?.Address?.Street, updated.Address.Street);
            Assert.Equal(input[3] != null ? input[3] : patient?.Address?.HouseNumber, updated.Address.HouseNumber);
            Assert.Equal(input[4] != null ? input[4] : patient?.Address?.LocalNumber, updated.Address.LocalNumber);
            Assert.Equal(input[5] != null ? input[5] : patient?.FirstName, updated.FirstName);
            Assert.Equal(input[6] != null ? input[6] : patient?.LastName, updated.LastName);
            Assert.Equal(input[7] != null ? input[7] : patient?.Email, updated.Email);
            Assert.Equal(input[9] != null ? input[9] : patient?.Pesel, updated.Pesel);

            Assert.Equal(input[0] != null ? input[0] : patient?.Address?.City, rsp.Address.City);
            Assert.Equal(input[1] != null ? input[1] : patient?.Address?.ZipCode, rsp.Address.ZipCode);
            Assert.Equal(input[2] != null ? input[2] : patient?.Address?.Street, rsp.Address.Street);
            Assert.Equal(input[3] != null ? input[3] : patient?.Address?.HouseNumber, rsp.Address.HouseNumber);
            Assert.Equal(input[4] != null ? input[4] : patient?.Address?.LocalNumber, rsp.Address.LocalNumber);
            Assert.Equal(input[5] != null ? input[5] : patient?.FirstName, rsp.FirstName);
            Assert.Equal(input[6] != null ? input[6] : patient?.LastName, rsp.LastName);
            Assert.Equal(input[7] != null ? input[7] : patient?.Email, rsp.Email);
            Assert.Equal(input[9] != null ? input[9] : patient?.Pesel, rsp.Pesel);
        }

        [Theory]
        [InlineData("john@patient.com", null)]
        [InlineData(null, "22222222222")]
        public void UtValidationShouldThrowConflictException(string? email, string? pesel)
        {
            Assert.Throws<ConflictException>(() => this.patientServiceMock.ValidatePatient(email, pesel));
        }

        [Theory]
        [InlineData(null, "67090884236")]
        [InlineData("john@patientcom", null)]
        [InlineData("john@", null)]
        [InlineData("@patient.com", null)]
        [InlineData("johnpatientcom", null)]
        public void UtValidationShouldThrowValidationException(string? email, string? pesel)
        {
            Assert.Throws<ValidationException>(() => this.patientServiceMock.ValidatePatient(email, pesel));
        }

        [Fact]
        public void UtShouldConfirmRegistration()
        {
            var patient = this.dataContextMock.Object.Patients.First(patient => patient.Id == 3);
            var request = new ConfirmRegistrationRequest() {Token = patient.VerificationToken};

            List<PatientModel> verifyList = new List<PatientModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Update(It.IsAny<PatientModel>())).Callback<PatientModel>(patient => verifyList.Add(patient));
            
            var rsp = this.patientServiceMock.ConfirmRegistration(request).Result;
            
            this.dataContextMock.Verify(dataContext => dataContext.Update(It.IsAny<PatientModel>()), Times.Once());
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());
         
            Assert.Single(verifyList);
            var updated = verifyList[0];
            
            Assert.Null(updated.VerificationToken);
        }

        [Fact]
        public void UtShouldThrowExceptionWhenConfirmingWrongToken()
        {
            var patient = this.dataContextMock.Object.Patients.First(patient => patient.Id == 3);
            var request = new ConfirmRegistrationRequest() {Token = Guid.NewGuid().ToString()};
            
            Assert.ThrowsAsync<UnauthorizedException>(() => this.patientServiceMock.ConfirmRegistration(request));
        }
    }
}
