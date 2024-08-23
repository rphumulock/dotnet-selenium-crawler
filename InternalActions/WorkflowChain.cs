using OpenQA.Selenium;
public class WorkflowChain
{
    private readonly List<IWorkflowStep> _steps = new();

    public WorkflowChain AddStep(IWorkflowStep step)
    {
        _steps.Add(step);
        return this;
    }

    public void Execute(IWebDriver driver)
    {
        bool continueExecution = true;
        foreach (var step in _steps)
        {
            if (!continueExecution) break;
            step.Execute(driver, ref continueExecution);
        }
    }
}
