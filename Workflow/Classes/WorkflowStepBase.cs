using HAI_Selenium.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace HAI_Selenium.Workflow.Classes
{
    public abstract class WorkflowStepBase(WorkflowContext context) : IWorkflowStep
    {
        protected int MaxRetries { get; } = 3;
        private const int DefaultWaitTimeInSeconds = 3;

        protected WorkflowContext Context { get; init; } = context;

        public async Task ExecuteAsync(IWebDriver driver)
        {
            int attempts = 0;
            while (attempts < MaxRetries)
            {
                try
                {
                    await PerformStepAsync(driver);
                    return;
                }
                catch (Exception ex)
                {
                    attempts++;
                    Log.Error("Attempt {Attempt} failed with exception.", attempts);

                    if (attempts >= MaxRetries)
                    {
                        Log.Error("Max retry attempts reached. Halting workflow.");
                        throw new HAIException(ex.Message, Context, ex);
                    }

                    await Task.Delay(1000); // Optional: Wait before retrying
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

        protected abstract Task PerformStepAsync(IWebDriver driver);
    }
}
