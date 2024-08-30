using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium
{
    class Program
    {
        static async Task Main(string[] args)
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

            // Initialize the DatabaseLockManager
            using (var lockManager = new DatabaseLockManager(connectionString, clientId))
            {
                try
                {
                    // Acquire the lock
                    lockManager.AcquireLock();

                    // Get the action from the environment and create workflow
                    string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
                    Log.Information("Action retrieved from environment: {Action}", action);
                    var workflow = WorkflowFactory.GetWorkflow(action);

                    // Setup WebDriver and execute the workflow with retry logic
                    driver = WebDriverUtils.SetupDriver();
                    Log.Information("WebDriver setup completed.");
                    await WorkflowExecutor.ExecuteWithRetryAsync(workflow, driver); // Use the async method
                    Log.Information("Workflow executed successfully.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while acquiring the lock or executing the workflow.");
                }
                finally
                {
                    // Cleanup WebDriver
                    driver?.Quit();
                    Log.Information("WebDriver closed and quit.");

                    Log.Information("Application ending.");
                }
            }

            // Ensure to flush and close the log when the application exits
            Log.CloseAndFlush();
        }

    }
}
