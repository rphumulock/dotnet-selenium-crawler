using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Database.Models;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<InvoiceRequest> InvoiceRequests { get; set; }  // Updated to PascalCase
        public DbSet<ServiceDateRequest> ServiceDateRequests { get; set; }  // Updated to PascalCase

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
            // Configure the one-to-many relationship
            modelBuilder.Entity<InvoiceRequest>()
                .HasMany(ir => ir.ServiceDateRequests)
                .WithOne(sd => sd.InvoiceRequest)
                .HasForeignKey(sd => sd.InvoiceRequestId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior

            base.OnModelCreating(modelBuilder);  // Ensure base method is called after configurations
        }
    }
}
