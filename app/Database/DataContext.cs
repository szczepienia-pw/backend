﻿using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using backend.Models.Vaccines;
using backend.Models.Visits;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class DataContext : DbContext
    {
        private readonly Seeder seeder;

        public DataContext(Seeder seeder, DbContextOptions<DataContext> options) : base(options)
        {
            this.seeder = seeder;
        }

        public DataContext() {}

        public virtual DbSet<AddressModel> Addresses { get; set; }
        public virtual DbSet<AdminModel> Admins { get; set; }
        public virtual DbSet<DoctorModel> Doctors { get; set; }
        public virtual DbSet<PatientModel> Patients { get; set; }
        public virtual DbSet<VaccineModel> Vaccines { get; set; }
        public virtual DbSet<VaccinationSlotModel> VaccinationSlots { get; set; }
        public virtual DbSet<VaccinationModel> Vaccinations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.seeder.Seed(modelBuilder);
        }
    }
}
