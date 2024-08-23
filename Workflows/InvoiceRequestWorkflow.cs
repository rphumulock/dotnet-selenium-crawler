using HAI_Selenium.AbstractClasses;
using HAI_Selenium.InternalClasses.Invoice;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Utils;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflows
{
    public class InvoiceRequestWorkflow : InvoiceWorkflowTemplate
    {
        private RequestInvoice _invoice;
        private PaymentData _paymentData;
        private FormDataForProcessing _dataForProcessing;

        protected override void LoadData()
        {
            _invoice = Utilities.LoadJsonFile<RequestInvoice>("Utils/InvoiceRequest.json");
            _paymentData = Utilities.LoadJsonFile<PaymentData>("Utils/PaymentBreakdown.json");
        }

        protected override void ProcessWebsite(IWebDriver driver)
        {
            // Create a new WorkflowChain and add steps to it
            WorkflowChain workflowChain = new WorkflowChain()
          .AddStep(new NavigateToSiteAction())  
          .AddStep(new LoginAction())          
          .AddStep(new NavigateToMembershipSearchAction())
          .AddStep(new FindPatientAction(_invoice))       
          .AddStep(new SelectPatientAction());

            workflowChain.Execute(driver);
        }

        protected override void ProcessData(IWebDriver driver)
        {
            //ClaimsActions.ProcessData(driver, _dataForProcessing);
        }
    }
}



//protected override void BuildRequirements(IWebDriver driver)
//{
//    WorkflowChain workflowChain = new WorkflowChain()
//     .AddStep(new NavigateToSiteAction())
//     .AddStep(new LoginAction());

//    workflowChain.Execute(driver);

//    //LoginActions.LoginToSite(driver);
//    //    PatientActions.SelectPatient(driver, _invoice);
//    //    string serviceDatesMonth = Utilities.ValidateServiceDateMonth(_invoice);
//    //    ServiceRequest authNumberServiceRequest = ServiceRequestActions.GetServiceRequestWithAuthNumber(driver, serviceDatesMonth);
//    //    _dataForProcessing = Utilities.CreateFormDataForProcessing(_invoice, _paymentData, authNumberServiceRequest);
//}
