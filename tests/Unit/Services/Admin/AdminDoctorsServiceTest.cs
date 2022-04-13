using backend.Database;
using backend.Dto.Requests.Admin;
using backend.Dto.Requests.Admin.Doctor;
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
    public class AdminDoctorsServiceTest
    {
        private Mock<DataContext> dataContextMock { get; set; }
        private AdminDoctorsService adminDoctorsServiceMock { get; set; }
        private SecurePasswordHasher securePasswordHasherMock { get; set; }

        private DoctorModel? FindDoctor(int doctorId)
        {
            return this.dataContextMock.Object.Doctors.FirstOrDefault(doctor => doctor.Id == doctorId);
        }

        public AdminDoctorsServiceTest()
        {
            // constructor is being executed before each test
            this.dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            this.securePasswordHasherMock = SecurePasswordHasherHelper.Hasher;
            this.adminDoctorsServiceMock = new AdminDoctorsService(this.dataContextMock.Object, this.securePasswordHasherMock);
        }

        // Delete Doctor
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TestShouldDeleteExistingDoctorWithNoSlots(int doctorId)
        {
            var rsp = this.adminDoctorsServiceMock.DeleteDoctor(doctorId).Result;
            
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.True(rsp.Success);
            Assert.True(this.FindDoctor(doctorId)?.IsDeleted);
        }

        [Theory]
        [InlineData(3)]
        public void TestShouldThrowNotFoundExceptionWhenDeleteingANonExistingDoctor(int doctorId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminDoctorsServiceMock.DeleteDoctor(doctorId));
        }

        [Theory]
        [InlineData(1)]
        public void TestShouldDeleteDoctorAndTheirSlots(int doctorId)
        {
            var doctor = this.FindDoctor(doctorId);
            Assert.NotNull(doctor);

            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Id = 1,
                    Date = DateTime.Parse("2023-04-01T14:00:00Z"),
                    Reserved = false,
                    Doctor = doctor
                },
                new()
                {
                    Id = 2,
                    Date = DateTime.Parse("2023-04-01T15:00:00Z"),
                    Reserved = true,
                    Doctor = doctor
                },
            };

            this.dataContextMock.Setup(dbContext => dbContext.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            this.dataContextMock.Setup(dbContext => dbContext.RemoveRange(It.IsAny<IQueryable<VaccinationSlotModel>>())).Callback<object>(
                (param) => vaccinationSlots.RemoveAll(slot => (param as IQueryable<VaccinationSlotModel>).Contains(slot))
                ); 

            var rsp = this.adminDoctorsServiceMock.DeleteDoctor(doctorId).Result;
            var remainingSlots = this.dataContextMock.Object.VaccinationSlots.Where(slot => slot.Doctor.Id == doctorId);

            this.dataContextMock.Verify(dataContext => dataContext.RemoveRange(It.IsAny<IQueryable<VaccinationSlotModel>>()), Times.Exactly(2));
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.True(rsp.Success);
            Assert.Equal(0, remainingSlots.Count());
            Assert.True(this.FindDoctor(doctorId)?.IsDeleted);
        }

        // Create Doctor
        [Theory]
        [InlineData("foo", "bar", "foo@bar.org", "asdf")]
        public void TestShouldCreateDoctorsAccount(string firstName, string lastName, string email, string password)
        {
            List<DoctorModel> verifyList = new List<DoctorModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Add(It.IsAny<DoctorModel>())).Callback<DoctorModel>((d) => verifyList.Add(d));
            
            var rsp = this.adminDoctorsServiceMock.CreateDoctor(new CreateDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email, Password = password }).Result;
            
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());
            this.dataContextMock.Verify(dataContext => dataContext.Add(It.IsAny<DoctorModel>()), Times.Once());

            Assert.Single(verifyList);
            Assert.Contains(verifyList, (obj) => obj == rsp);
            
            Assert.Equal(firstName, rsp.FirstName);
            Assert.Equal(lastName, rsp.LastName);
            Assert.Equal(email, rsp.Email);
            Assert.True(this.securePasswordHasherMock.Verify(password, rsp.Password));
        }

        [Theory]
        [InlineData("foo", "bar", "john@doctor.com", "asdf")]
        public void TestCreatingDoctorWithOccupiedEmailShouldCauseValidationException(string firstName, string lastName, string email, string password)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.adminDoctorsServiceMock.CreateDoctor(new CreateDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email, Password = password }));
        }

        [Theory]
        [InlineData("foo", "bar", "john@doctorcom", "asdf")]
        [InlineData("foo", "bar", "@doctor.com", "asdf")]
        [InlineData("foo", "bar", "john@", "asdf")]
        [InlineData("foo", "bar", "johndoctorcom", "asdf")]
        public void TestCreatingDoctorWithInvalidEmailShouldCauseValidationException(string firstName, string lastName, string email, string password)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.adminDoctorsServiceMock.CreateDoctor(new CreateDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email, Password = password }));
        }

        // Edit Doctor
        [Theory]
        [InlineData(1, "foo", "bar", "john_new@doctor.com")]
        [InlineData(1, null, "bar", "john_new@doctor.com")]
        [InlineData(1, null, null, "john_new@doctor.com")]
        [InlineData(1, null, null, null)]
        [InlineData(1, "foo", "bar", null)]
        [InlineData(1, null, "bar", null)]
        [InlineData(1, "foo", null, null)]
        [InlineData(1, "foo", null, "john_new@doctor.com")]
        [InlineData(1, null, null, "john@doctor.com")]
        public void TestShouldEditDoctorsData(int doctorId, string firstName, string lastName, string email)
        {
            List<DoctorModel> verifyList = new List<DoctorModel>();
            this.dataContextMock.Setup(dataContext => dataContext.Doctors.Update(It.IsAny<DoctorModel>())).Callback<DoctorModel>((param) => verifyList.Add(param));

            var doctorOld = this.FindDoctor(doctorId);
            var rsp = this.adminDoctorsServiceMock.EditDoctor(doctorId, new EditDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email }).Result;

            this.dataContextMock.Verify(dataContext => dataContext.Doctors.Update(It.IsAny<DoctorModel>()), Times.Once());
            this.dataContextMock.Verify(dataContext => dataContext.SaveChanges(), Times.Once());

            Assert.Single(verifyList);
            Assert.Contains(verifyList, (obj) => obj.Id == rsp.Id);

            Assert.Equal(firstName == null ? doctorOld?.FirstName : firstName, rsp.FirstName);
            Assert.Equal(lastName == null ? doctorOld?.LastName : lastName, rsp.LastName);
            Assert.Equal(email == null ? doctorOld?.Email : email, rsp.Email);
            Assert.Equal(rsp.Password, rsp.Password);
        }

        [Theory]
        [InlineData(10, null, null, null)]
        public void TestEditingNonExistentDoctorShouldCauseNotFoundException(int doctorId, string firstName, string lastName, string email)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminDoctorsServiceMock.EditDoctor(doctorId, new EditDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email }));
        }

        [Theory]
        [InlineData(2, null, null, "john@doctor.com")]
        public void TestEditingDoctorWithOccupiedEmailShouldCauseValidationException(int doctorId, string firstName, string lastName, string email)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.adminDoctorsServiceMock.EditDoctor(doctorId, new EditDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email }));
        }

        [Theory]
        [InlineData(2, null, null, "john@doctorcom")]
        [InlineData(2, null, null, "@doctor.com")]
        [InlineData(2, null, null, "john@")]
        [InlineData(2, null, null, "johndoctorcom")]
        public void TestEditingDoctorWithInvalidEmailShouldCauseValidationException(int doctorId, string firstName, string lastName, string email)
        {
            Assert.ThrowsAsync<ValidationException>(() => this.adminDoctorsServiceMock.EditDoctor(doctorId, new EditDoctorRequest() { FirstName = firstName, LastName = lastName, Email = email }));
        }

        [Theory]
        [InlineData(1)]
        public void TestShowDoctorWorks(int doctorId)
        {
            var doctor = this.adminDoctorsServiceMock.ShowDoctor(doctorId).Result;

            Assert.Equal(doctorId, doctor.Id);
        }

        [Theory]
        [InlineData(10)]
        public void TestShowDoctorThrowsNotFoundException(int doctorId)
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.adminDoctorsServiceMock.ShowDoctor(doctorId));
        }
    }
}
