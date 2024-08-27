using OpenQA.Selenium;
using Serilog;

public class WorkflowChain
{
    private readonly List<IWorkflowStep> _steps = new List<IWorkflowStep>();

    public WorkflowChain AddStep(IWorkflowStep step)
    {
        _steps.Add(step);
        return this;
    }

    public void Execute(IWebDriver driver)
    {
        foreach (var step in _steps)
        {
            try
            {
                Log.Information("Executing workflow step: {StepName}", step.GetType().Name);
                step.Execute(driver);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred during step execution: {StepName}", step.GetType().Name);
                throw;
            }
        }
    }
}
