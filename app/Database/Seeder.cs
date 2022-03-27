using backend.Helpers;
using backend.Models;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using backend.Models.Visits;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class Seeder
    {
        private readonly SecurePasswordHasher securePasswordHasher;

        public Seeder(SecurePasswordHasher securePasswordHasher)
        {
            this.securePasswordHasher = securePasswordHasher;
        }

        public void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressModel>().HasData(
                new AddressModel()
                {
                    Id = 1,
                    City = "Warsaw",
                    Street = "Koszykowa",
                    HouseNumber = "64",
                    LocalNumber = "32",
                    ZipCode = "01-123"
                }
            );

            modelBuilder.Entity<PatientModel>().HasData(
                new PatientModel()
                {
                    Id = 1,
                    Email = "john@patient.com",
                    FirstName = "John",
                    LastName = "Patient",
                    Password = this.securePasswordHasher.Hash("password"),
                    Pesel = "22222222222",
                    AddressId = 1
                }
            );

            DoctorModel doctor = new DoctorModel()
            {
                Id = 1,
                Email = "john@doctor.com",
                FirstName = "John",
                LastName = "Doctor",
                Password = this.securePasswordHasher.Hash("password"),
            };
            modelBuilder.Entity<DoctorModel>().HasData(doctor);

            modelBuilder.Entity<AdminModel>().HasData(
                new AdminModel()
                {
                    Id = 1,
                    Email = "john@admin.com",
                    FirstName = "John",
                    LastName = "Admin",
                    Password = this.securePasswordHasher.Hash("password"),
                }
            );

            modelBuilder.Entity<SettingModel>().HasData(
                new SettingModel()
                {
                    Id = 1,
                    Type = SettingType.BugEmail,
                    Value = "bugmail@szczepienia.pw"
                }
            );

            for (int i = 1; i <= 20; i++)
            {
                modelBuilder.Entity<VaccinationSlotModel>().HasData(
                    new VaccinationSlotModel
                    {
                        Id = i,
                        DoctorId = doctor.Id,
                        Date = DateTime.Now.AddDays(i - 1),
                        Reserved = i % 2 == 0
                    }
                );
            }
        }
    }
}
