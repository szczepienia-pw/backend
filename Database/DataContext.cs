using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;
using backend.Models.Vaccines;
using backend.Models.Visits;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccinationSlot> VaccinationSlots { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
    }
}
