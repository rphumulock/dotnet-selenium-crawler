using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Utilities;
using HAI_Selenium.Services;

namespace HAI_Selenium.Workflow.Classes
{
    public static class WorkflowExecutor
    {
        private const int MaxRetries = 3;

        public static async Task ExecuteWithRetryAsync(IWorkflowStrategy workflow, IWebDriver driver, IInvoiceRequestService invoiceRequestService)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Log.Information("Starting workflow attempt {Attempt}", attempt);

                    await workflow.ExecuteAsync(driver);

                    Log.Information("Workflow completed successfully.");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("Attempt {Attempt} failed with exception.", attempt);

                    if (attempt == MaxRetries)
                    {
                        Log.Error("Max retry attempts reached. Analyzing exception...");

                        ErrorHandlerUtils.AnalyzeAndHandleFinalException(ex, invoiceRequestService);
                        driver?.Quit();
                    }
                    else
                    {
                        HandleRetry(ref driver, attempt);
                    }
                }
            }
        }

        private static void HandleRetry(ref IWebDriver driver, int attempt)
        {
            driver?.Quit();
            ExponentialBackoff(attempt);
            RestartDriver(ref driver);
        }

        private static void ExponentialBackoff(int attempt)
        {
            int delay = (int)Math.Pow(2, attempt) * 1000;
            Log.Information("Waiting for {Delay} milliseconds before retrying...", delay);
            Thread.Sleep(delay);
        }

        private static void RestartDriver(ref IWebDriver driver)
        {
            driver = WebDriverUtils.SetupDriver();
        }
    }
}
