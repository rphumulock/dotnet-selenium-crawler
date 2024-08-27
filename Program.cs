using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Data; // To use ApplicationDbContext
using HAI_Selenium.Database.Models; // To use InvoiceRequest model

namespace HAI_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Setup environment variables and logging
                EnvironmentUtils.LoadEnvVariables();
                EnvironmentUtils.LogCurrentUserInfo();

                // Initialize the database context
                using (var dbContext = new ApplicationDbContext())
                {
                    // Example: Add a new InvoiceRequest
                    var newInvoiceRequest = new InvoiceRequest
                    {
                        // Set properties for your InvoiceRequest object
                    };
                    dbContext.InvoiceRequests.Add(newInvoiceRequest);
                    dbContext.SaveChanges(); // Save changes to the database

                    // Example: Query the InvoiceRequests table
                    var invoiceRequests = dbContext.InvoiceRequests.ToList();
                    foreach (var request in invoiceRequests)
                    {
                        Console.WriteLine($"InvoiceRequest ID: {request.Id}");
                    }
                }

                // Get the action from the environment and create workflow
                string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
                var workflow = WorkflowFactory.GetWorkflow(action);

                // Execute the workflow with retry logic
                IWebDriver driver = WebDriverUtils.SetupDriver();
                WorkflowExecutor.ExecuteWithRetry(workflow, driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during workflow execution: {ex.Message}");
            }
            finally
            {
                // Clean up resources if needed
            }
        }

        static void ApplyMigrations(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Applying any pending migrations...");
            dbContext.Database.Migrate(); // Apply any pending migrations
        }
    }
}
