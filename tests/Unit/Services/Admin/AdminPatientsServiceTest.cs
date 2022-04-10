using backend.Database;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Visits;
using backend.Services.Admin;
using backend_tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace backend_tests.Unit.Services.Admin
{
    public class AdminPatientsServiceTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private AdminPatientsService adminPatientsService { get; set; }
        private SecurePasswordHasher securePasswordHasherMock { get; set; }

        private DoctorModel? FindDoctor(int doctorId)
        {
            return this.dataContextMock.Object.Doctors.FirstOrDefault(doctor => doctor.Id == doctorId);
        }

        public AdminPatientsServiceTest()
        {
            // constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.securePasswordHasherMock = SecurePasswordHasherHelper.Hasher;
            this.adminPatientsService = new AdminPatientsService(this.dataContextMock.Object);
        }

        [Theory]
        [InlineData(100)]
        public void TestShowPatientShouldThrowNotFoundException(int patientId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientsService.ShowPatient(patientId));
        }

        [Theory]
        [InlineData(1)]
        public void TestShowPatientShouldReturnTheRightPatient(int patientId)
        {
            var patient = this.adminPatientsService.ShowPatient(patientId).Result;
            Assert.Equal(patientId, patient.Id);
        }

        [Theory]
        [InlineData(100)]
        public void TestDeletePatientShouldThrowNotFoundException(int patientId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientsService.ShowPatient(patientId));
        }

        [Theory]
        [InlineData(1)]
        public void TestDeletePatientShouldDeletePatientAndTheirFutureSlots(int patientId)
        {
            var patientValidationList = new List<PatientModel>();
            var slotValidationList = new List<VaccinationSlotModel>();

            this.dataContextMock.Setup(dbContext => dbContext.Remove(It.IsAny<PatientModel>())).Callback<PatientModel>(patient => patientValidationList.Add(patient));
            this.dataContextMock.Setup(dbContext => dbContext.RemoveRange(It.IsAny<List<VaccinationSlotModel>>())).Callback<IEnumerable<object>>(slotList => slotValidationList.AddRange(slotList as List<VaccinationSlotModel>));

            var rsp = this.adminPatientsService.DeletePatient(patientId).Result;

            Assert.True(rsp.Success);
            Assert.Single(patientValidationList);
            Assert.Single(slotValidationList);
            Assert.Equal(patientId, patientValidationList[0].Id);
            Assert.Equal(2, slotValidationList[0].Id);
        }
    }
}
