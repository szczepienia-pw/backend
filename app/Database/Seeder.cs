using backend.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class Seeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorModel>().HasData(
                new DoctorModel()
                {
                    Id = 1,
                    Email = "john@doctor.com",
                    FirstName = "John",
                    LastName = "Doctor",
                    Password = "password"
                }
            );
        }
    }
}
