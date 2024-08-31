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
                    return; // Exit if the step is successful
                }
                catch (Exception ex)
                {
                    attempts++;
                    Log.Error("Attempt {Attempt} failed with exception: {ExceptionMessage}", attempts, ex.Message);

                    if (attempts >= MaxRetries)
                    {
                        Log.Error("Max retry attempts reached. Halting workflow.");
                        throw new HAIException(ex.Message, Context, ex);
                    }

                    int delay = ExponentialBackoff(attempts);
                    Log.Information("Waiting for {Delay} milliseconds before retrying...", delay);
                    await Task.Delay(delay);
                }
            }
        }

        private static int ExponentialBackoff(int attempt)
        {
            return (int)Math.Pow(2, attempt) * 1000;
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
