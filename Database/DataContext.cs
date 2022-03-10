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

        public DbSet<AddressModel> Addresses { get; set; }
        public DbSet<AdminModel> Admins { get; set; }
        public DbSet<DoctorModel> Doctors { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<VaccineModel> Vaccines { get; set; }
        public DbSet<VaccinationSlotModel> VaccinationSlots { get; set; }
        public DbSet<VaccinationModel> Vaccinations { get; set; }
    }
}
