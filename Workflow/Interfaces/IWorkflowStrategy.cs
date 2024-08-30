using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Interfaces
{
    public interface IWorkflowStrategy
    {
        Task ExecuteAsync(IWebDriver driver);
    }
}
