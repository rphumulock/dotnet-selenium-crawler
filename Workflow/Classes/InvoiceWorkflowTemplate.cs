using OpenQA.Selenium;
using HAI_Selenium.Workflow.Interfaces;

namespace HAI_Selenium.Workflow.Classes
{
    public abstract class InvoiceWorkflowTemplate : IWorkflowStrategy
    {
        public async Task ExecuteAsync(IWebDriver driver)
        {
            await InitializeDataAsync(driver);
            await ProcessDataAsync(driver);
        }

        protected abstract Task InitializeDataAsync(IWebDriver driver);
        protected abstract Task ProcessDataAsync(IWebDriver driver);
    }
}
