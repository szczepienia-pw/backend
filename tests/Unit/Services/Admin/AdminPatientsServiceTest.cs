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
using backend.Controllers.Admin;
using backend.Services.Patient;
using Xunit;

namespace backend_tests.Admin
{
    public partial class AdminPatientsTest
    {
        private readonly Mock<DataContext> dataContextMock;
        private readonly AdminPatientsService adminPatientsService;
        private readonly PatientService patientService;
        private readonly SecurePasswordHasher securePasswordHasherMock;
        private readonly AdminPatientController adminPatientController ;

        private DoctorModel? FindDoctor(int doctorId)
        {
            return this.dataContextMock.Object.Doctors.FirstOrDefault(doctor => doctor.Id == doctorId);
        }

        public AdminPatientsTest()
        {
            // constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.securePasswordHasherMock = SecurePasswordHasherHelper.Hasher;
            this.adminPatientsService = new AdminPatientsService(this.dataContextMock.Object);
            this.patientService = new PatientService(this.dataContextMock.Object, this.securePasswordHasherMock);
            this.adminPatientController = new AdminPatientController(this.patientService, this.adminPatientsService);
        }

        [Theory]
        [InlineData(100)]
        public void UtTestShowPatientShouldThrowNotFoundException(int patientId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientsService.ShowPatient(patientId));
        }

        [Theory]
        [InlineData(1)]
        public void UtTestShowPatientShouldReturnTheRightPatient(int patientId)
        {
            var patient = this.adminPatientsService.ShowPatient(patientId).Result;
            Assert.Equal(patientId, patient.Id);
        }

        [Theory]
        [InlineData(100)]
        public void UtTestDeletePatientShouldThrowNotFoundException(int patientId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientsService.ShowPatient(patientId));
        }

        [Theory]
        [InlineData(1)]
        public void UtTestDeletePatientShouldDeletePatientAndTheirFutureSlots(int patientId)
        {
            var patientValidationList = new List<PatientModel>();
            var slotValidationList = new List<VaccinationSlotModel>();

            this.dataContextMock.Setup(dbContext => dbContext.Remove(It.IsAny<PatientModel>())).Callback<PatientModel>(patient => patientValidationList.Add(patient));
            this.dataContextMock.Setup(dbContext => dbContext.RemoveRange(It.IsAny<List<VaccinationSlotModel>>())).Callback<IEnumerable<object>>(slotList => slotValidationList.AddRange(slotList as List<VaccinationSlotModel>));

            var rsp = this.adminPatientsService.DeletePatient(patientId).Result;

            Assert.True(rsp.Success);
            Assert.Single(patientValidationList);
            Assert.Equal(3, slotValidationList.Count);
            Assert.Equal(patientId, patientValidationList[0].Id);
            Assert.Equal(2, slotValidationList[0].Id);
        }
    }
}
