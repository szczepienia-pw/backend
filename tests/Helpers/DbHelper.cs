using System.Collections.Generic;
using System.Linq;
using backend.Database;
using backend.Helpers;
using backend.Models;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using backend.Models.Vaccines;
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

            var vaccinationSlots = new List<VaccinationSlotModel>()
            {
                new()
                {
                    Id = 1,
                    Date = System.DateTime.Now.AddDays(1),
                    Doctor = doctors.First(),
                    DoctorId = doctors.First().Id,
                    Reserved = false,
                },
                new()
                {
                    Id = 2,
                    Date = System.DateTime.Now.AddDays(1),
                    Doctor = doctors.First(),
                    DoctorId = doctors.First().Id,
                    Reserved = true,
                }
            };

            var vaccines = new List<VaccineModel>()
            {
                new()
                {
                    Id = 1,
                    Disease = DiseaseEnum.COVID19,
                    Name = "Pfizer",
                    RequiredDoses = 3
                },
                new()
                {
                    Id = 2,
                    Disease = DiseaseEnum.COVID19,
                    Name = "Moderna",
                    RequiredDoses = 3
                },
                new()
                {
                    Id = 3,
                    Disease = DiseaseEnum.COVID19,
                    Name = "Johnson&Johnson",
                    RequiredDoses = 2
                },
                new()
                {
                    Id = 4,
                    Disease = DiseaseEnum.COVID21,
                    Name = "Razor",
                    RequiredDoses = 4
                },
                new()
                {
                    Id = 5,
                    Disease = DiseaseEnum.COVID21,
                    Name = "Razor light",
                    RequiredDoses = 1
                },
                new()
                {
                    Id = 6,
                    Disease = DiseaseEnum.Flu,
                    Name = "FluMax",
                    RequiredDoses = 2
                },
                new()
                {
                    Id = 7,
                    Disease = DiseaseEnum.Flu,
                    Name = "FluMini",
                    RequiredDoses = 1
                },
                new()
                {
                    Id = 8,
                    Disease = DiseaseEnum.Other,
                    Name = "Vaccinator1000",
                    RequiredDoses = 1
                },
                new()
                {
                    Id = 9,
                    Disease = DiseaseEnum.Other,
                    Name = "Vaccinator2000",
                    RequiredDoses = 2
                },
                new()
                {
                    Id = 10,
                    Disease = DiseaseEnum.Other,
                    Name = "Vaccinator3000",
                    RequiredDoses = 3
                }
            };

            var tmpSlot = vaccinationSlots.First(slot => slot.Reserved == true);
            var vaccinations = new List<VaccinationModel>()
            {
                new()
                {
                    Id = 1,
                    VaccinationSlot = tmpSlot,
                    Doctor = tmpSlot.Doctor,
                    Patient = patients.First(),
                    Vaccine = vaccines.First(),
                    Status = StatusEnum.Planned
                }
            };

            var contextMock = new Mock<DataContext>();
            contextMock.Setup(dbContext => dbContext.Doctors).Returns(doctors.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Patients).Returns(patients.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Admins).Returns(admins.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Settings).Returns(settings.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Vaccines).Returns(vaccines.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
            return contextMock;
        }
    }
}
