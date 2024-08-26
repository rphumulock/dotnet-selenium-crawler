using OpenQA.Selenium;

public interface IWorkflowStep
{
    void Execute(IWebDriver driver);
}
