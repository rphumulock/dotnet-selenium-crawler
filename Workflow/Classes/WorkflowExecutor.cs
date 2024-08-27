using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Utilities;

using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Classes
{
    public static class WorkflowExecutor
    {
        private const int MaxRetries = 1;

        public static void ExecuteWithRetry(IWorkflowStrategy workflow, IWebDriver driver)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"Starting workflow attempt {attempt}");

                    // Execute the workflow process
                    workflow.Execute(driver);

                    Console.WriteLine("[SUCCESS] Workflow completed successfully.");
                    return; // Exit if successful
                }
                catch (RecoverableError ex)
                {
                    Console.WriteLine($"[ERROR] Non-recoverable error occurred: {ex.Message}");
                    driver?.Quit(); // Close the driver
                    ErrorHandlerUtils.AnalyzeAndHandleFinalException(ex); // Use the ErrorHandler class
                    throw; // Re-throw the exception to indicate final failure
                }
                catch (NonRecoverableError ex)
                {
                    Console.WriteLine($"[ERROR] Non-recoverable error occurred: {ex.Message}");
                    driver?.Quit(); // Close the driver
                    ErrorHandlerUtils.AnalyzeAndHandleFinalException(ex); // Use the ErrorHandler class
                    throw; // Re-throw the exception to indicate final failure
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Attempt {attempt} failed with exception: {ex.Message}");

                    if (attempt == MaxRetries)
                    {
                        Console.WriteLine("[FAILURE] Max retry attempts reached. Analyzing exception...");
                        driver?.Quit(); // Close the driver
                        ErrorHandlerUtils.AnalyzeAndHandleFinalException(ex); // Use the ErrorHandler class
                        throw; // Re-throw the exception to indicate final failure after all retries
                    }

                    // Retry logic
                    HandleRetry(ref driver, attempt);
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
            Console.WriteLine($"Waiting for {delay} milliseconds before retrying...");
            Thread.Sleep(delay); // Wait for the calculated delay
        }

        private static void RestartDriver(ref IWebDriver driver)
        {
            driver = WebDriverUtils.SetupDriver(); // Restart the driver for the next attempt
        }
    }
}
