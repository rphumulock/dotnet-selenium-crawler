using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Database.Models; // Ensure this is the correct namespace
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<InvoiceRequest> InvoiceRequests { get; set; } // Ensure this DbSet exists

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
                .HasKey(ir => ir.Id); // Set primary key

            modelBuilder.Entity<InvoiceRequest>()
                .Property(ir => ir.Id)
                .ValueGeneratedOnAdd(); // Auto-increment primary key
        }

        protected static void ApplyMigrations(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Applying any pending migrations...");
            dbContext.Database.Migrate(); // Apply any pending migrations
        }
    }
}
