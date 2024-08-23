using OpenQA.Selenium;
using HAI_Selenium.Interfaces;

namespace HAI_Selenium.AbstractClasses
{
    public abstract class InvoiceWorkflowTemplate : IWorkflowStrategy
    {
        public void Execute(IWebDriver driver)
        {
            LoadData();
            ProcessWebsite(driver);
            ProcessData(driver);
        }
        protected abstract void LoadData();
        protected abstract void ProcessWebsite(IWebDriver driver);
        protected abstract void ProcessData(IWebDriver driver);
    }
}
