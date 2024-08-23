using OpenQA.Selenium;

namespace HAI_Selenium.Interfaces
{
    public interface IWorkflowStrategy
    {
        void Execute(IWebDriver driver);
    }
}
