using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Classes
{
    public class WorkflowChain
    {
        private readonly List<IWorkflowStep> _steps = new List<IWorkflowStep>();

        public WorkflowChain AddStep(IWorkflowStep step)
        {
            _steps.Add(step);
            return this;
        }

        public async Task ExecuteAsync(IWebDriver driver)
        {
            foreach (var step in _steps)
            {
                try
                {
                    Log.Information("Executing workflow step: {StepName}", step.GetType().Name);
                    await step.ExecuteAsync(driver);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred during step execution: {StepName}", step.GetType().Name);
                    throw;
                }
            }
        }
    }
}
