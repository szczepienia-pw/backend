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
            #region Addresses

            modelBuilder.Entity<AddressModel>().HasKey(address => address.Id);
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

            #endregion

            #region Patients

            modelBuilder.Entity<PatientModel>().HasKey(patient => patient.Id);
            modelBuilder.Entity<PatientModel>(
                entity => entity.HasOne(patient => patient.Address).WithOne().HasForeignKey<PatientModel>(patient => patient.AddressId)
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

            #endregion

            #region Doctors

            DoctorModel doctor = new DoctorModel()
            {
                Id = 1,
                Email = "john@doctor.com",
                FirstName = "John",
                LastName = "Doctor",
                Password = this.securePasswordHasher.Hash("password"),
            };
            modelBuilder.Entity<DoctorModel>().HasData(doctor);
            modelBuilder.Entity<DoctorModel>().HasKey(doctor => doctor.Id);

            #endregion

            #region Admins

            modelBuilder.Entity<AdminModel>().HasKey(admin => admin.Id);
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

            #endregion

            #region Settings

            modelBuilder.Entity<SettingModel>().HasKey(setting => setting.Id);
            modelBuilder.Entity<SettingModel>().HasData(
                new SettingModel()
                {
                    Id = 1,
                    Type = SettingType.BugEmail,
                    Value = "bugmail@szczepienia.pw"
                }
            );

            #endregion

            #region VaccinationSlots

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

            #endregion
        }
    }
}
