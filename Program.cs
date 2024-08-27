using System;
using OpenQA.Selenium;
using Medallion.Threading.Postgres;
using Npgsql;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Setup environment variables and logging
            EnvironmentUtils.LoadEnvVariables();
            Log.Information("Environment variables loaded.");
            EnvironmentUtils.LogCurrentUserInfo();

            IWebDriver driver = null;
            string clientId = "YourClientID"; // Replace with the actual client ID you're locking on
            string connectionString = EnvironmentUtils.DbConnectionStringBuilder();

            // Log application start
            Log.Information("Application starting...");

            IDisposable handle = null; // Declare the lock handle outside try-catch

            // Create a connection to PostgreSQL
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Log.Information("Connected to PostgreSQL database.");

                // Set up the PostgreSQL distributed lock
                var lockId = new PostgresAdvisoryLockKey(clientId.GetHashCode());
                var dbLock = new PostgresDistributedLock(lockId, connection);

                try
                {
                    Log.Information("Attempting to acquire lock for client ID: {ClientId}", clientId);

                    // Acquire the lock and assign to the handle
                    handle = dbLock.Acquire();
                    Log.Information("Lock acquired for client ID: {ClientId}", clientId);

                    // Get the action from the environment and create workflow
                    string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
                    Log.Information("Action retrieved from environment: {Action}", action);
                    var workflow = WorkflowFactory.GetWorkflow(action);

                    // Setup WebDriver and execute the workflow with retry logic
                    driver = WebDriverUtils.SetupDriver();
                    Log.Information("WebDriver setup completed.");
                    WorkflowExecutor.ExecuteWithRetry(workflow, driver);
                    Log.Information("Workflow executed successfully.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while acquiring the lock or executing the workflow.");
                }
                finally
                {
                    // Release the lock if it was acquired
                    handle?.Dispose();
                    Log.Information("Lock released for client ID: {ClientId}", clientId);

                    // Cleanup WebDriver
                    if (driver != null)
                    {
                        driver.Quit();
                        Log.Information("WebDriver closed and quit.");
                    }

                    Log.Information("Application ending.");
                }
            }

            // Ensure to flush and close the log when the application exits
            Log.CloseAndFlush();
        }
    }
}
