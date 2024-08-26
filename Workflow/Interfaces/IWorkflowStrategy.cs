using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Interfaces
{
    public interface IWorkflowStrategy
    {
        void Execute(IWebDriver driver);
    }
}
