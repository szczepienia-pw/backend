using System.Collections.Generic;
using System.Linq;
using backend.Database;
using backend.Helpers;
using backend.Models;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using backend.Models.Visits;
using Moq;

namespace backend_tests.Helpers
{
    public static class DbHelper
    {
        public static Mock<DataContext> GetMockedDataContextWithAccounts()
        {
            var doctors = new List<DoctorModel>()
            {
                new()
                {
                    Id = 1,
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Email = "john@doctor.com",
                    Password = SecurePasswordHasherHelper.Hasher.Hash("password")
                },
                new()
                {
                    Id = 2,
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Email = "john@doctor2.com",
                    Password = SecurePasswordHasherHelper.Hasher.Hash("password")
                }
            };

            var patients = new List<PatientModel>()
            {
                new()
                {
                    Id = 1,
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Email = "john@patient.com",
                    Password = SecurePasswordHasherHelper.Hasher.Hash("password"),
                    Pesel = "22222222222",
                    Address = new AddressModel()
                    {

                    }
                }
            };

            var admins = new List<AdminModel>()
            {
                new()
                {
                    Id = 1,
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Email = "john@admin.com",
                    Password = SecurePasswordHasherHelper.Hasher.Hash("password")
                }
            };

            var settings = new List<SettingModel>()
            {
                new()
                {
                    Id = 1,
                    Type = SettingType.BugEmail,
                    Value = "bugmail@szczepiania.pw"
                }
            };
            var vaccinationSlots = new List<VaccinationSlotModel>() { };

            var contextMock = new Mock<DataContext>();
            contextMock.Setup(dbContext => dbContext.Doctors).Returns(doctors.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Patients).Returns(patients.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Admins).Returns(admins.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Settings).Returns(settings.AsQueryable().BuildMockDbSet().Object);
            return contextMock;
        }
    }
}
