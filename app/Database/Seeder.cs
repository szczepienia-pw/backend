using backend.Helpers;
using backend.Models;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
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
            modelBuilder.Entity<DoctorModel>().HasData(
                new DoctorModel()
                {
                    Id = 1,
                    Email = "john@doctor.com",
                    FirstName = "John",
                    LastName = "Doctor",
                    Password = this.securePasswordHasher.Hash("password"),
                }
            );
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
        }
    }
}
