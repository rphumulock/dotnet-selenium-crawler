using OpenQA.Selenium;

public interface IWorkflowStep
{
    Task ExecuteAsync(IWebDriver driver);
}
