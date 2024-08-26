using OpenQA.Selenium;

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
                step.Execute(driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception occurred during step execution: {ex.Message}");
                throw; // Propagate the exception to be handled by the outer retry mechanism
            }
        }
    }
}
