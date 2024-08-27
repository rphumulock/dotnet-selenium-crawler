using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

public abstract class WorkflowStepBase : IWorkflowStep
{
    protected int MaxRetries { get; } = 3;
    private const int DefaultWaitTimeInSeconds = 3;

    public void Execute(IWebDriver driver)
    {
        int attempts = 0;
        while (attempts < MaxRetries)
        {
            try
            {
                PerformStep(driver);
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                Log.Error(ex, "Attempt {Attempt} failed with exception.", attempts);

                if (attempts >= MaxRetries)
                {
                    Log.Error("Max retry attempts reached. Halting workflow.");
                    throw;
                }
            }
        }
    }

    protected WebDriverWait GetWebDriverWait(IWebDriver driver, int waitTimeInSeconds = DefaultWaitTimeInSeconds)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(waitTimeInSeconds));
    }

    protected T WaitUntil<T>(IWebDriver driver, Func<IWebDriver, T> condition, int waitTimeInSeconds = DefaultWaitTimeInSeconds)
    {
        var wait = GetWebDriverWait(driver, waitTimeInSeconds);
        return wait.Until(condition);
    }

    protected abstract void PerformStep(IWebDriver driver);
}
