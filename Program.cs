using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OpenQA.Selenium;
using HAI_Selenium.Services;
using HAI_Selenium.Data;
using HAI_Selenium.Utilities;

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

                // Setup and run the workflow
                await SetupAndRunWorkflowAsync(args);

                // Optionally run the host if necessary (e.g., for a web application or background services)
                var host = CreateHostBuilder(args).Build();
                await host.RunAsync();
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

        static async Task SetupAndRunWorkflowAsync(string[] args)
        {
            EnvironmentUtils.LoadEnvVariables();
            EnvironmentUtils.LogCurrentUserInfo();
            var connectionString = EnvironmentUtils.DbConnectionStringBuilder();
            string lockKey = "HAI_Selenium_DistributedLock";
            IWebDriver driver = null;

            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");

                // The factory automatically uses the correct service based on the action
                var workflow = WorkflowFactory.GetWorkflow(action, services);

                using (var lockManager = new DatabaseLockManager(connectionString, lockKey))
                {
                    try
                    {
                        lockManager.AcquireLock();
                        driver = WebDriverUtils.SetupDriver();

                        await workflow.ExecuteAsync(driver);

                        Log.Information("Workflow executed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while acquiring the lock or executing the workflow.");
                    }
                    finally
                    {
                        driver?.Quit();
                        driver?.Dispose();
                        Log.Information("WebDriver closed and quit.");
                    }
                }
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .UseSerilog()
                 .ConfigureServices((context, services) =>
                 {
                     var connectionString = EnvironmentUtils.DbConnectionStringBuilder();
                     services.AddDbContext<ApplicationDbContext>(options =>
                     {
                         options.EnableSensitiveDataLogging();
                         options.UseNpgsql(connectionString);
                     });

                     // Register your specific services
                     services.AddScoped<IInvoiceRequestService, InvoiceRequestService>();
                     services.AddSingleton<INRulesService, NRulesService>();
                 });
    }
}




//static async Task SetupAndRunWorkflowAsync(string[] args)
//{
//    // Setup environment variables and logging
//    EnvironmentUtils.LoadEnvVariables();
//    EnvironmentUtils.LogCurrentUserInfo();
//    var connectionString = EnvironmentUtils.DbConnectionStringBuilder();
//    string lockKey = "HAI_Selenium_DistributedLock";
//    IWebDriver driver = null;

//    // Create HostBuilder and configure services
//    var host = CreateHostBuilder(args).Build();

//    using (var serviceScope = host.Services.CreateScope())
//    {
//        var services = serviceScope.ServiceProvider;

//        // Retrieve required services
//        var invoiceRequestService = services.GetRequiredService<IInvoiceRequestService>();

//        // Initialize the DatabaseLockManager
//        using (var lockManager = new DatabaseLockManager(connectionString, lockKey))
//        {
//            try
//            {
//                // Acquire the lock
//                lockManager.AcquireLock();

//                // Setup driver
//                driver = WebDriverUtils.SetupDriver();

//                // Get the action from the environment and create workflow
//                string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
//                var workflow = WorkflowFactory.GetWorkflow(action, invoiceRequestService);

//                // Execute the workflow with retry logic
//                await workflow.ExecuteAsync(driver);

//                Log.Information("Workflow executed successfully.");
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex, "An error occurred while acquiring the lock or executing the workflow.");
//            }
//            finally
//            {
//                // Cleanup WebDriver
//                driver?.Quit();
//                driver?.Dispose(); // Ensure complete cleanup
//                Log.Information("WebDriver closed and quit.");
//            }
//        }
//    }
//}