using HAI_Selenium.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

public abstract class WorkflowStepBase : IWorkflowStep
{
    protected int MaxRetries { get; } = 3; // Default max retries
    private const int DefaultWaitTimeInSeconds = 2; // Default wait time

    public void Execute(IWebDriver driver)
    {
        int attempts = 0;
        while (attempts < MaxRetries)
        {
            try
            {
                PerformStep(driver); // Attempt to perform the step
                return; // Exit the method if successful
            }
            catch (RecoverableError ex)
            {
                attempts++;
                Console.WriteLine($"[ERROR] Attempt {attempts} failed with exception: {ex.Message}");

                if (attempts >= MaxRetries)
                {
                    Console.WriteLine($"[FAILURE] Max retry attempts reached. Halting workflow.");
                    throw new RecoverableError(ex.Message, ex); // Re-throw the exception to be handled by the outer retry mechanism
                }
            }
            catch (NonRecoverableError ex)
            {
                attempts++;
                Console.WriteLine($"[ERROR] Attempt {attempts} failed with exception: {ex.Message}");

                if (attempts >= MaxRetries)
                {
                    Console.WriteLine($"[FAILURE] Max retry attempts reached. Halting workflow.");
                    throw new NonRecoverableError(ex.Message, ex); // Re-throw the exception to be handled by the outer retry mechanism
                }
            }
            catch (Exception ex)
            {
                attempts++;
                Console.WriteLine($"[ERROR] Attempt {attempts} failed with exception: {ex.Message}");

                if (attempts >= MaxRetries)
                {
                    Console.WriteLine($"[FAILURE] Max retry attempts reached. Halting workflow.");
                    throw; // Re-throw the exception to be handled by the outer retry mechanism
                }
            }
        }
    }

    // Method to get a WebDriverWait instance
    protected WebDriverWait GetWebDriverWait(IWebDriver driver, int waitTimeInSeconds = DefaultWaitTimeInSeconds)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(waitTimeInSeconds));
    }

    // Helper method to use the WebDriverWait with a specific condition
    protected T WaitUntil<T>(IWebDriver driver, Func<IWebDriver, T> condition, int waitTimeInSeconds = DefaultWaitTimeInSeconds)
    {
        var wait = GetWebDriverWait(driver, waitTimeInSeconds);
        return wait.Until(condition);
    }

    // Abstract method to be implemented by derived classes for specific steps
    protected abstract void PerformStep(IWebDriver driver);
}
