using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Database.Models;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ServiceDateRequest> ServiceDateRequests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string host = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_HOST");
                string port = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_PORT");
                string database = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_NAME");
                string username = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_USER");
                string password = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_PASSWORD");

                var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
