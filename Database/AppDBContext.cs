using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Utilities;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<InvoiceRequest> InvoiceRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string host = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_HOST");
            string port = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_PORT");
            string database = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_NAME");
            string username = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_USER");
            string password = EnvironmentUtils.GetEnvironmentVariableOrThrow("DB_PASSWORD");

            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceRequest>()
                .HasKey(ir => ir.Id); // Setting primary key
            modelBuilder.Entity<InvoiceRequest>()
                .Property(ir => ir.Id)
                .ValueGeneratedOnAdd(); // Serial primary key configuration
        }
    }
}
