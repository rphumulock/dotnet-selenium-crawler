using OpenQA.Selenium;
using HAI_Selenium.Workflow.Interfaces;

namespace HAI_Selenium.Workflow.Classes
{
    public abstract class InvoiceWorkflowTemplate : IWorkflowStrategy
    {
        public async Task ExecuteAsync(IWebDriver driver)
        {
            await InitializeDataAsync(driver); // Ensures LoadDataAsync completes before ProcessData starts
            await ProcessDataAsync(driver); // Process data after loading is complete
        }

        protected abstract Task InitializeDataAsync(IWebDriver driver);
        protected abstract Task ProcessDataAsync(IWebDriver driver);
    }
}
