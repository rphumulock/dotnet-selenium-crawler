using OpenQA.Selenium;
using HAI_Selenium.Workflow.Interfaces;

namespace HAI_Selenium.Workflow.AbstractClasses
{
    public abstract class InvoiceWorkflowTemplate : IWorkflowStrategy
    {
        public void Execute(IWebDriver driver)
        {
            ProcessData(driver);
        }

        protected abstract void ProcessData(IWebDriver driver);
    }
}
