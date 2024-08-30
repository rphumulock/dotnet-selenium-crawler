using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Database.Models;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<InvoiceRequest> InvoiceRequests { get; set; }
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
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship
            modelBuilder.Entity<InvoiceRequest>()
                .HasMany(ir => ir.ServiceDateRequests)
                .WithOne(sd => sd.InvoiceRequest)
                .HasForeignKey(sd => sd.InvoiceRequestId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior
        }
    }
}
