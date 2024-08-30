using HAI_Selenium.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using HAI_Selenium.Utilities;
using OpenQA.Selenium;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Services;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configure Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Application starting.");

                InvoiceRequest mockRequest = FileUtils.LoadJsonFile<InvoiceRequest>("Utilities/mockData/InvoiceCreateClaimsRequest.json");

                // Setup and run the workflow
                await SetupAndRunWorkflowAsync(args, mockRequest);

                //// Optionally run the host if necessary (e.g., for a web application or background services)
                //var host = CreateHostBuilder(args).Build();
                //await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred during application execution.");
            }
            finally
            {
                Log.Information("Application ending.");
                Log.CloseAndFlush(); // Ensure logs are flushed and closed at the end
            }
        }

        static async Task SetupAndRunWorkflowAsync(string[] args, InvoiceRequest mockRequest)
        {
            // Setup environment variables and logging
            EnvironmentUtils.LoadEnvVariables();
            EnvironmentUtils.LogCurrentUserInfo();
            var connectionString = EnvironmentUtils.DbConnectionStringBuilder();
            string lockKey = "HAI_Selenium_DistributedLock";
            IWebDriver driver = null;

            // Create HostBuilder and configure services
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                // Retrieve required services
                var invoiceRequestService = services.GetRequiredService<IInvoiceRequestService>();

                // Initialize the DatabaseLockManager
                using (var lockManager = new DatabaseLockManager(connectionString, lockKey))
                {
                    try
                    {
                        // Acquire the lock
                        lockManager.AcquireLock();

                        // Setup driver
                        driver = WebDriverUtils.SetupDriver();

                        // Get the action from the environment and create workflow
                        string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
                        var workflow = WorkflowFactory.GetWorkflow(action, invoiceRequestService, mockRequest);

                        // Execute the workflow with retry logic
                        await WorkflowExecutor.ExecuteWithRetryAsync(workflow, driver, invoiceRequestService);
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
                        driver?.Dispose(); // Ensure complete cleanup
                        Log.Information("WebDriver closed and quit.");
                    }
                }
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // Integrate Serilog with Microsoft.Extensions.Logging
                .ConfigureServices((context, services) =>
                {
                    //var connectionString = EnvironmentUtils.DbConnectionStringBuilder();
                    // Register DbContext with connection string
                    //services.AddDbContext<ApplicationDbContext>(options =>
                    //    options.UseNpgsql(connectionString));

                    services.AddDbContext<ApplicationDbContext>();

                    // Register application services
                    services.AddScoped<IInvoiceRequestService, InvoiceRequestService>();
                });
    }
}
