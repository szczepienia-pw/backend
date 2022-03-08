using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class DataContext : DbContext
    {
        private const string connectionString = "server=localhost;port=3306;database=io;user=root;password=";

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ServerVersion sv = MariaDbServerVersion.AutoDetect(connectionString);
            optionsBuilder.UseMySql(connectionString, sv);
        }

        public DbSet<Admin> Admins { get; set; }
    }
}
