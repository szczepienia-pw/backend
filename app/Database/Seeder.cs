﻿using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class Seeder
    {
        public static void Seed(ModelBuilder modelBuilder)
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
                    Password = SecurePasswordHasher.Hash("password"),
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
                    Password = SecurePasswordHasher.Hash("password"),
                }
            );
            modelBuilder.Entity<AdminModel>().HasData(
                new AdminModel()
                {
                    Id = 1,
                    Email = "john@admin.com",
                    FirstName = "John",
                    LastName = "Admin",
                    Password = SecurePasswordHasher.Hash("password"),
                }
            );
        }
    }
}
