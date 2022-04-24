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
using System;

namespace backend_tests.Helpers
{
    public static class DbHelper
    {
        public static Mock<DataContext> GetMockedDataContextWithAccounts()
        {
            #region Doctors

            var d1 = new DoctorModel()
            {
                Id = 1,
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Email = "john@doctor.com",
                Password = SecurePasswordHasherHelper.Hasher.Hash("password")
            };

            var d2 = new DoctorModel()
            {
                Id = 2,
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Email = "john@doctor2.com",
                Password = SecurePasswordHasherHelper.Hasher.Hash("password")
            };

            var doctors = new List<DoctorModel>()
            {
                d1,
                d2
            };

            #endregion

            #region Patients

            var p1 = new PatientModel()
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
            };

            var patients = new List<PatientModel>()
            {
                p1
            };

            #endregion

            #region Admins

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

            #endregion

            #region Settings

            var settings = new List<SettingModel>()
            {
                new()
                {
                    Id = 1,
                    Type = SettingType.BugEmail,
                    Value = "bugmail@szczepiania.pw"
                }
            };

            #endregion

            #region Vaccination Slots

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
                },
                new()
                {
                    Id = 3,
                    Date = System.DateTime.Now.AddDays(1),
                    Doctor = doctors[1],
                    DoctorId = doctors[1].Id,
                    Reserved = true,
                }
            };

            #endregion

            #region Vaccines

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

            #endregion

            #region Vaccinations

            var tmpSlot = vaccinationSlots.First(slot => slot.Reserved == true);
            var vaccinations = new List<VaccinationModel>()
            {
                new()
                {
                    Id = 1,
                    VaccinationSlot = tmpSlot,
                    VaccinationSlotId = tmpSlot.Id,
                    Doctor = tmpSlot.Doctor,
                    Patient = patients.First(),
                    PatientId = patients.First().Id,
                    Vaccine = vaccines.First(),
                    Status = StatusEnum.Planned
                }
            };

            #endregion

            var contextMock = new Mock<DataContext>();
            contextMock.Setup(dbContext => dbContext.Doctors).Returns(doctors.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Patients).Returns(patients.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Admins).Returns(admins.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.VaccinationSlots).Returns(vaccinationSlots.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Settings).Returns(settings.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Vaccines).Returns(vaccines.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(dbContext => dbContext.Vaccinations).Returns(vaccinations.AsQueryable().BuildMockDbSet().Object);
            return contextMock;
        }
    }
}
