using HAI_Selenium.AbstractClasses;
using HAI_Selenium.InternalClasses.Invoice;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Utils;

using OpenQA.Selenium;

namespace HAI_Selenium.Workflows
{
    public class InvoiceStatusWorkflow : InvoiceWorkflowTemplate
    {
        private StatusInvoice _invoice;
        private FormDataForProcessing _dataForProcessing;

        protected override void LoadData()
        {
           _invoice = Utilities.LoadJsonFile<StatusInvoice>("Utils/InvoiceStatus.json");
        }

        protected override void ProcessWebsite(IWebDriver driver)
        {
            //LoginActions.LoginToSite(driver);
            //ClaimsStatusActions.CheckClaimsStatus(driver, _invoice);
        }

        protected override void ProcessData(IWebDriver driver)
        {
            return;
        }
    }
}

