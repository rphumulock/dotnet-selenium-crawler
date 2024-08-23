using OpenQA.Selenium;
using System;

public abstract class WorkflowStepBase : IWorkflowStep
{
    protected int MaxRetries { get; } = 3; // Default max retries

    public void Execute(IWebDriver driver, ref bool continueExecution)
    {
        int attempts = 0;
        while (attempts < MaxRetries)
        {
            try
            {
                PerformStep(driver); // Attempt to perform the step
                continueExecution = true; // If successful, allow the chain to continue
                return; // Exit the method
            }
            catch (WebDriverException ex)
            {
                attempts++;
                Console.WriteLine($"[ERROR] Attempt {attempts} failed: {ex.Message}");
                if (attempts >= MaxRetries)
                {
                    Console.WriteLine($"[FAILURE] Max retry attempts reached. Halting workflow.");
                    continueExecution = false; // Stop the chain if retries are exhausted
                    return; // Exit the method
                }
            }
        }
    }

    protected abstract void PerformStep(IWebDriver driver);
}
