using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Workflow.Classes
{
    public static class WorkflowExecutor
    {
        private const int MaxRetries = 3;

        public static void ExecuteWithRetry(IWorkflowStrategy workflow, IWebDriver driver)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Log.Information("Starting workflow attempt {Attempt}", attempt);

                    // Execute the workflow process
                    workflow.Execute(driver);

                    Log.Information("Workflow completed successfully.");
                    return; // Exit if successful
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Attempt {Attempt} failed with exception.", attempt);

                    if (attempt == MaxRetries)
                    {
                        Log.Error("Max retry attempts reached. Analyzing exception...");
                        driver?.Quit(); // Close the driver
                        ErrorHandlerUtils.AnalyzeAndHandleFinalException(ex); // Use the ErrorHandler class
                    }
                    else
                    {
                        // Retry logic
                        HandleRetry(ref driver, attempt);
                    }

                }
            }
        }

        private static void HandleRetry(ref IWebDriver driver, int attempt)
        {
            driver?.Quit(); // Close the driver
            ExponentialBackoff(attempt); // Exponential backoff before retrying
            RestartDriver(ref driver); // Restart the driver for the next attempt
        }

        private static void ExponentialBackoff(int attempt)
        {
            int delay = (int)Math.Pow(2, attempt) * 1000; // Exponential backoff time in milliseconds
            Log.Information("Waiting for {Delay} milliseconds before retrying...", delay);
            Thread.Sleep(delay); // Wait for the calculated delay
        }

        private static void RestartDriver(ref IWebDriver driver)
        {
            driver = WebDriverUtils.SetupDriver(); // Restart the driver for the next attempt
        }
    }
}
