using OpenQA.Selenium;
using HAI_Selenium.Workflow.Interfaces;

namespace HAI_Selenium.Workflow.Classes
{
    public abstract class InvoiceWorkflowTemplate : IWorkflowStrategy
    {
        public async Task ExecuteAsync(IWebDriver driver)
        {
            InitializeData(driver);
            await InitializeDataAsync(driver);
            await ProcessDataAsync(driver);
        }

        protected virtual void InitializeData(IWebDriver driver) { }

        protected virtual Task InitializeDataAsync(IWebDriver driver)
        {
            return Task.CompletedTask;
        }

        protected abstract Task ProcessDataAsync(IWebDriver driver);
    }
}
